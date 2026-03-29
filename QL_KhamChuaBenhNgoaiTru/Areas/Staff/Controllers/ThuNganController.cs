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
        public JsonResult GetChiTiet(int maPKB)
        {
            try
            {
                var vm = db.GetChiTietCongNo(maPKB);
                return Json(new { success = true, data = vm });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ThanhToan(int maPKB, bool thuPhiKham, bool thuPhiCLS, bool thuPhiThuoc)
        {
            if (maPKB <= 0) return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            
            var nv = Session["NhanVien"] as NhanVien;
            string maNV = nv != null ? nv.MaNV : "NV001";

            string errorMsg;
            bool result = db.XacNhanThuTienTungPhan(maPKB, thuPhiKham, thuPhiCLS, thuPhiThuoc, maNV, out errorMsg);

            if (result) return Json(new { success = true });
            return Json(new { success = false, message = errorMsg });
        }

        // ======================================================================
        // CÁC HÀM TÍCH HỢP PAYOS
        // ======================================================================

        private string CreateSignature(long amount, string cancelUrl, string description, long orderCode, string returnUrl, string checksumKey)
        {
            string data = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        [HttpPost]
        public async Task<JsonResult> TaoMaQR(int maPKB, int tongTien)
        {
            try
            {
                if (tongTien <= 0) return Json(new { success = false, message = "Không có số tiền cần thanh toán." });

                string clientId = ConfigurationManager.AppSettings["PayOS:ClientId"];
                string apiKey = ConfigurationManager.AppSettings["PayOS:ApiKey"];
                string checksumKey = ConfigurationManager.AppSettings["PayOS:ChecksumKey"];

                long orderCode = long.Parse(maPKB.ToString() + DateTime.Now.ToString("HHmmss"));
                string returnUrl = ConfigurationManager.AppSettings["PayOS:ReturnUrl"] ?? "https://localhost:44326/Staff/ThuNgan";
                string cancelUrl = returnUrl;
                string description = "Thu tien PKB " + maPKB;

                string signature = CreateSignature(tongTien, cancelUrl, description, orderCode, returnUrl, checksumKey);

                var requestData = new
                {
                    orderCode = orderCode,
                    amount = tongTien,
                    description = description,
                    items = new[] { new { name = "Thanh toan tung phan", quantity = 1, price = tongTien } },
                    returnUrl = returnUrl,
                    cancelUrl = cancelUrl,
                    signature = signature
                };

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-client-id", clientId);
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                    string jsonBody = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", content);
                    string responseString = await response.Content.ReadAsStringAsync();

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