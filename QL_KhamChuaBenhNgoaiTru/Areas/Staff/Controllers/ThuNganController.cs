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
using System.Linq;

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
        // [ĐÃ SỬA] int -> string
        public JsonResult GetChiTiet(string maHD)
        {
            try
            {
                DataTable dt = db.GetChiTietHoaDon(maHD);
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        // [ĐÃ SỬA] Đổi sang string cho MaCTHD
                        MaCTHD = row["MaCTHD"].ToString(),
                        LoaiItem = row["LoaiItem"].ToString(),
                        TrangThai = row["TrangThaiThanhToan"].ToString(),
                        TenDV = row["TenDV"].ToString(),
                        DonGia = Convert.ToDecimal(row["DonGia"]),
                        TienBHYT = Convert.ToDecimal(row["TienBHYTChiTra"]),
                        TienBenhNhan = Convert.ToDecimal(row["TienBenhNhanTra"]),

                        SoLuong = row["SoLuong"] != DBNull.Value ? Convert.ToInt32(row["SoLuong"]) : 1,
                        SoNgayDung = row["SoNgayDung"] != DBNull.Value ? Convert.ToInt32(row["SoNgayDung"]) : 1,
                        SoLuongGoc = row.Table.Columns.Contains("SoLuongGoc") && row["SoLuongGoc"] != DBNull.Value ? Convert.ToInt32(row["SoLuongGoc"]) : 1,

                        LiSang = row.Table.Columns.Contains("SoLuongSang") && row["SoLuongSang"] != DBNull.Value ? Convert.ToDecimal(row["SoLuongSang"]) : 0,
                        LiTrua = row.Table.Columns.Contains("SoLuongTrua") && row["SoLuongTrua"] != DBNull.Value ? Convert.ToDecimal(row["SoLuongTrua"]) : 0,
                        LiChieu = row.Table.Columns.Contains("SoLuongChieu") && row["SoLuongChieu"] != DBNull.Value ? Convert.ToDecimal(row["SoLuongChieu"]) : 0,
                        LiToi = row.Table.Columns.Contains("SoLuongToi") && row["SoLuongToi"] != DBNull.Value ? Convert.ToDecimal(row["SoLuongToi"]) : 0
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
        // [ĐÃ SỬA] int -> string
        public JsonResult ThanhToan(string maHD, string maPKB, string phuongThucTT, string dsHuyDV, string dsHuyThuoc, string dsThuocCapNhat)
        {
            if (string.IsNullOrEmpty(maHD) || string.IsNullOrEmpty(maPKB)) return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            string errorMsg;
            List<dynamic> listThuoc = new List<dynamic>();

            if (!string.IsNullOrEmpty(dsThuocCapNhat))
            {
                try
                {
                    listThuoc = JsonConvert.DeserializeObject<List<dynamic>>(dsThuocCapNhat);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Dữ liệu cập nhật thuốc không hợp lệ: " + ex.Message });
                }
            }

            bool result = db.XacNhanThuTien(maHD, maPKB, phuongThucTT, dsHuyDV, dsHuyThuoc, listThuoc, out errorMsg);

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
        // [ĐÃ SỬA] int -> string
        public async Task<JsonResult> TaoMaQR(string maHD, string maPKB, int tongTien)
        {
            try
            {
                if (tongTien <= 0) return Json(new { success = false, message = "Hóa đơn 0đ, không cần quét QR." });

                string clientId = ConfigurationManager.AppSettings["PayOS:ClientId"];
                string apiKey = ConfigurationManager.AppSettings["PayOS:ApiKey"];
                string checksumKey = ConfigurationManager.AppSettings["PayOS:ChecksumKey"];

                // [BÍ KÍP TRÁNH CRASH PAYOS] Cắt chữ "HD" ra, chỉ lấy dãy số + giờ để ép sang long
                string cleanMaHD = maHD.Replace("HD", "");
                long orderCode = long.Parse(cleanMaHD + DateTime.Now.ToString("HHmmss"));

                string returnUrl = ConfigurationManager.AppSettings["PayOS:ReturnUrl"] ?? "https://localhost:44326/Staff/ThuNgan";
                string cancelUrl = returnUrl;
                string description = "Thanh toan vien phi";

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

        public ActionResult LichSu(string tuNgay, string denNgay, string tuKhoa, string trangThai, string sortCol = "NgayThanhToan", string sortDir = "desc", int page = 1)
        {
            DateTime dtTuNgay = DateTime.Now.Date;
            DateTime dtDenNgay = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParse(tuNgay, out dtTuNgay);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParse(denNgay, out dtDenNgay);

            ViewBag.TuNgay = dtTuNgay.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = dtDenNgay.ToString("yyyy-MM-dd");
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;
            ViewBag.SortCol = sortCol;
            ViewBag.SortDir = sortDir;

            DataTable fullDt = db.GetLichSuThuTien(dtTuNgay, dtDenNgay);

            if (fullDt.Rows.Count > 0)
            {
                var rows = fullDt.AsEnumerable();

                // 1. TÌM KIẾM
                if (!string.IsNullOrEmpty(tuKhoa))
                {
                    string keyword = tuKhoa.ToLower().Trim();
                    rows = rows.Where(r =>
                        r["MaHD"].ToString().ToLower().Contains(keyword) ||
                        r["MaPhieuKhamBenh"].ToString().ToLower().Contains(keyword) ||
                        r["HoTen"].ToString().ToLower().Contains(keyword) ||
                        r["SDT"].ToString().Contains(keyword)
                    );
                }

                // 2. LỌC TRẠNG THÁI
                if (!string.IsNullOrEmpty(trangThai))
                {
                    rows = rows.Where(r => r["TrangThaiThanhToan"].ToString() == trangThai);
                }

                // 3. [ĐÃ SỬA] SẮP XẾP CAO -> THẤP (Ép kiểu chuỗi cho mã)
                if (rows.Any())
                {
                    if (sortDir == "asc")
                    {
                        switch (sortCol)
                        {
                            case "MaHD": rows = rows.OrderBy(r => r["MaHD"].ToString()); break; // Đổi thành so sánh chuỗi
                            case "HoTen": rows = rows.OrderBy(r => r["HoTen"].ToString()); break;
                            case "TongTienBenhNhanTra": rows = rows.OrderBy(r => Convert.ToDecimal(r["TongTienBenhNhanTra"])); break;
                            case "TrangThaiThanhToan": rows = rows.OrderBy(r => r["TrangThaiThanhToan"].ToString()); break;
                            default: rows = rows.OrderBy(r => Convert.ToDateTime(r["NgayThanhToan"])); break;
                        }
                    }
                    else
                    {
                        switch (sortCol)
                        {
                            case "MaHD": rows = rows.OrderByDescending(r => r["MaHD"].ToString()); break; // Đổi thành so sánh chuỗi
                            case "HoTen": rows = rows.OrderByDescending(r => r["HoTen"].ToString()); break;
                            case "TongTienBenhNhanTra": rows = rows.OrderByDescending(r => Convert.ToDecimal(r["TongTienBenhNhanTra"])); break;
                            case "TrangThaiThanhToan": rows = rows.OrderByDescending(r => r["TrangThaiThanhToan"].ToString()); break;
                            default: rows = rows.OrderByDescending(r => Convert.ToDateTime(r["NgayThanhToan"])); break;
                        }
                    }
                    fullDt = rows.CopyToDataTable();
                }
                else { fullDt = fullDt.Clone(); }
            }

            // 4. PHÂN TRANG
            int pageSize = 15;
            int totalRows = fullDt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            DataTable pagedDt = fullDt.Clone();
            if (totalRows > 0) { pagedDt = fullDt.AsEnumerable().Skip((page - 1) * pageSize).Take(pageSize).CopyToDataTable(); }

            ViewBag.LichSuHoaDon = pagedDt;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRows = totalRows;

            return View();
        }
    }
}