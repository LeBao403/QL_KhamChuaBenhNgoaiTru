using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    public class ThuNganController : BaseStaffController
    {
        ThuNganDB db = new ThuNganDB();

        // CHẶN QUYỀN
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var nv = Session["NhanVien"] as NhanVien;
            if (nv != null && nv.MaChucVu != 9)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Unauthorized", area = "Staff" }));
            }
        }

        public ActionResult Index()
        {
            ViewBag.DanhSachThuTien = db.GetDanhSachChoThuTien();
            return View();
        }

        [HttpPost]
        public JsonResult GetChiTiet(int maHD)
        {
            try
            {
                DataTable dt = db.GetChiTietHoaDon(maHD);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        TenDV = row["TenDV"].ToString(),
                        DonGia = Convert.ToDecimal(row["DonGia"]),
                        TienBHYT = Convert.ToDecimal(row["TienBHYTChiTra"]),
                        TienBenhNhan = Convert.ToDecimal(row["TienBenhNhanTra"])
                    });
                }
                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ThanhToan(int maHD, int maPKB)
        {
            if (maHD <= 0 || maPKB <= 0) return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            string errorMsg;
            bool result = db.XacNhanThuTien(maHD, maPKB, out errorMsg);

            if (result) return Json(new { success = true });
            return Json(new { success = false, message = errorMsg });
        }

        // ======================================================================
        // CÁC HÀM TÍCH HỢP PAYOS "GỌI CHAY" (KHÔNG CẦN NUGET)
        // ======================================================================

        // Hàm hỗ trợ băm chữ ký bảo mật (Signature) chuẩn thuật toán PayOS
        private string CreateSignature(long amount, string cancelUrl, string description, long orderCode, string returnUrl, string checksumKey)
        {
            // Các tham số phải được sắp xếp theo thứ tự alphabet A-Z
            string data = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        [HttpPost]
        public async Task<JsonResult> TaoMaQR(int maHD, int maPKB)
        {
            try
            {
                // 1. Tính tổng tiền cần thu
                DataTable dt = db.GetChiTietHoaDon(maHD);
                int tongTien = 0;
                foreach (DataRow row in dt.Rows)
                {
                    tongTien += Convert.ToInt32(row["TienBenhNhanTra"]);
                }

                if (tongTien <= 0) return Json(new { success = false, message = "Hóa đơn 0đ, không cần quét QR." });

                // 2. Chuẩn bị dữ liệu gửi đi
                string clientId = ConfigurationManager.AppSettings["PayOS:ClientId"];
                string apiKey = ConfigurationManager.AppSettings["PayOS:ApiKey"];
                string checksumKey = ConfigurationManager.AppSettings["PayOS:ChecksumKey"];

                long orderCode = long.Parse(maHD.ToString() + DateTime.Now.ToString("HHmmss"));
                string returnUrl = ConfigurationManager.AppSettings["PayOS:ReturnUrl"] ?? "https://localhost:44326/Staff/ThuNgan";
                string cancelUrl = returnUrl;
                string description = "Thanh toan vien phi";

                // Băm mã Signature
                string signature = CreateSignature(tongTien, cancelUrl, description, orderCode, returnUrl, checksumKey);

                var requestData = new
                {
                    orderCode = orderCode,
                    amount = tongTien,
                    description = description,
                    items = new[] { new { name = "Vien phi Kham benh", quantity = 1, price = tongTien } },
                    returnUrl = returnUrl,
                    cancelUrl = cancelUrl,
                    signature = signature
                };

                // 3. GỌI API TRỰC TIẾP LÊN PAYOS
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-client-id", clientId);
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                    string jsonBody = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    // 4. Đọc dữ liệu trả về
                    JObject resJson = JObject.Parse(responseString);

                    if (resJson["code"]?.ToString() == "00")
                    {
                        string qrString = resJson["data"]["qrCode"].ToString();
                        return Json(new { success = true, qrString = qrString, orderCode = orderCode });
                    }
                    else
                    {
                        return Json(new { success = false, message = resJson["desc"]?.ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tạo QR PayOS: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> KiemTraThanhToan(long orderCode)
        {
            try
            {
                string clientId = ConfigurationManager.AppSettings["PayOS:ClientId"];
                string apiKey = ConfigurationManager.AppSettings["PayOS:ApiKey"];

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-client-id", clientId);
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                    HttpResponseMessage response = await client.GetAsync($"https://api-merchant.payos.vn/v2/payment-requests/{orderCode}");
                    string responseString = await response.Content.ReadAsStringAsync();

                    JObject resJson = JObject.Parse(responseString);

                    if (resJson["code"]?.ToString() == "00")
                    {
                        string status = resJson["data"]["status"].ToString();
                        if (status == "PAID") return Json(new { success = true, isPaid = true });
                    }
                    return Json(new { success = true, isPaid = false });
                }
            }
            catch
            {
                return Json(new { success = true, isPaid = false });
            }
        }
    }
}