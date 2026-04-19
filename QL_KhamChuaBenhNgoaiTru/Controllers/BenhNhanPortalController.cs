using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;

using BenhNhanModel = QL_KhamChuaBenhNgoaiTru.Models.BenhNhan;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class BenhNhanPortalController : Controller
    {
        private readonly BenhNhanPortalDB db = new BenhNhanPortalDB();
        TiepTanDB TTdb = new TiepTanDB();

        // ==================== 0. TRANG CHỦ / DASHBOARD ====================
        public ActionResult Index()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var dashboard = db.GetDashboard(bn.MaBN);
            ViewBag.Profile = db.GetBenhNhanByMaBN(bn.MaBN);
            return View(dashboard);
        }

        // ==================== 1. XEM & CẬP NHẬT THÔNG TIN CÁ NHÂN ====================
        public ActionResult ThongTinCaNhan()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var profile = db.GetBenhNhanByMaBN(bn.MaBN);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinCaNhan(BenhNhanPortalDB.BenhNhanProfile model)
        {
            var bnSession = Session["BenhNhan"] as BenhNhanModel;
            if (bnSession == null) return RedirectToAction("Login", "TaiKhoan");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                model.MaBN = bnSession.MaBN;
                bool ok = db.UpdateBenhNhan(model);
                if (ok)
                {
                    bnSession.HoTen = model.HoTen;
                    bnSession.SDT = model.SDT;
                    bnSession.Email = model.Email;
                    bnSession.NgaySinh = model.NgaySinh ?? DateTime.MinValue;
                    bnSession.GioiTinh = model.GioiTinh;
                    bnSession.DiaChi = model.DiaChi;
                    bnSession.CCCD = model.CCCD;
                    Session["BenhNhan"] = bnSession;

                    TempData["Success"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("ThongTinCaNhan");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return View(model);
        }

        // ==================== 2. ĐẶT LỊCH KHÁM & THANH TOÁN ONLINE ====================

        // GET: Hiển thị form đăng ký và tải sẵn Dịch vụ
        public ActionResult DatLichKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var tiepTanDb = new QL_KhamChuaBenhNgoaiTru.DBContext.TiepTanDB();
            var dtDichVu = tiepTanDb.GetDanhSachDichVuKham();

            var listDV = new List<SelectListItem>();
            foreach (System.Data.DataRow row in dtDichVu.Rows)
            {
                listDV.Add(new SelectListItem
                {
                    Value = row["MaDV"].ToString(),
                    Text = $"{row["TenDV"]} - {Convert.ToDecimal(row["GiaDichVu"]).ToString("N0")} VNĐ"
                });
            }
            ViewBag.ListDichVu = listDV;

            return View();
        }

        // POST: Hàm phụ trợ để dùng AJAX lấy danh sách khung giờ (Gọi từ JS)
        [HttpPost]
        public JsonResult LoadKhungGio(DateTime ngayKham)
        {
            try
            {
                var danhSachGio = db.GetKhungGioHopLe(ngayKham);
                return Json(new { success = true, data = danhSachGio });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DatLichKham(DateTime ngayKham, int maKhungGio, string maDV, string lyDo)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel; // Tùy class model của bác
            if (bn == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại!" });

            if (ngayKham < DateTime.Today)
            {
                return Json(new { success = false, message = "Ngày khám không được nhỏ hơn ngày hiện tại." });
            }

            try
            {
                string tenQuay;
                string maHD;

                // Gọi DB để chèn dữ liệu và lấy ra Mã Phiếu DK + Mã Hóa Đơn (đều là string)
                string maPhieuDK = db.DatLichKham(bn.MaBN, ngayKham, maKhungGio, maDV, lyDo, out tenQuay, out maHD);

                // Ném dữ liệu trực tiếp về cho Javascript mở Popup QR / VNPay
                return Json(new
                {
                    success = true,
                    maPhieuDK = maPhieuDK,
                    maHD = maHD,
                    tenQuay = tenQuay
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==================== 2.5 THANH TOÁN QR PAYOS ====================

        // GET: Hiển thị màn hình Thanh Toán
        public ActionResult ThanhToanOnline()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            // Lấy thông tin từ hàm Đặt lịch ném sang
            if (TempData["MaHD"] == null || TempData["MaPhieuDK"] == null)
            {
                return RedirectToAction("DatLichKham");
            }

            ViewBag.MaHD = TempData["MaHD"];
            ViewBag.MaPhieuDK = TempData["MaPhieuDK"];
            ViewBag.TenQuay = TempData["TenQuay"];

            // Giữ lại TempData để F5 không bị mất
            TempData.Keep();

            return View();
        }

        private string CreateSignature(long amount, string cancelUrl, string description, long orderCode, string returnUrl, string checksumKey)
        {
            string data = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // POST: Gọi API PayOS tạo mã QR
        [HttpPost]
        public async Task<JsonResult> TaoMaQROnline(string maHD, string maPhieuDK)
        {
            try
            {
                // Phí đặt lịch cố định 100k theo yêu cầu
                int tongTien = 100000;

                string clientId = (ConfigurationManager.AppSettings["PayOS:ClientId"] ?? "").Trim();
                string apiKey = (ConfigurationManager.AppSettings["PayOS:ApiKey"] ?? "").Trim();
                string checksumKey = (ConfigurationManager.AppSettings["PayOS:ChecksumKey"] ?? "").Trim();

                string cleanMaHD = (maHD ?? "").Replace("HD", "").Trim();
                long orderCode = long.Parse(cleanMaHD + DateTime.Now.ToString("HHmmss"));
                string returnUrl = (ConfigurationManager.AppSettings["PayOS:ReturnUrl"] ?? "https://localhost:44326/BenhNhanPortal/LichKham").Trim();
                string cancelUrl = returnUrl;
                string description = "Phi dat lich Online";

                string signature = CreateSignature(tongTien, cancelUrl, description, orderCode, returnUrl, checksumKey);

                var requestData = new
                {
                    orderCode = orderCode,
                    amount = tongTien,
                    description = description,
                    items = new[] { new { name = "Phi tien ich dat lich", quantity = 1, price = tongTien } },
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

        // POST: Kiểm tra trạng thái thanh toán từ PayOS
        [HttpPost]
        public async Task<JsonResult> KiemTraThanhToanOnline(long orderCode, string maPhieuDK, string maHD)
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
                        if (status == "PAID")
                        {
                            // 1. UPDATE DB: Hóa đơn -> Đã thanh toán, Phiếu ĐK -> Chờ xử lý (hoặc Đã xác nhận)
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                            {
                                conn.Open();
                                string sql = @"
                                    UPDATE HOADON SET TrangThaiThanhToan = N'Đã thanh toán', NgayThanhToan = GETDATE() WHERE MaHD = @MaHD;
                                    UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD;
                                    UPDATE PHIEUDANGKY SET TrangThai = N'Chờ xử lý' WHERE MaPhieuDK = @MaPhieuDK;";
                                SqlCommand cmd = new SqlCommand(sql, conn);
                                cmd.Parameters.AddWithValue("@MaHD", maHD);
                                cmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK);
                                cmd.ExecuteNonQuery();
                            }
                            return Json(new { success = true, isPaid = true });
                        }
                    }
                    return Json(new { success = true, isPaid = false });
                }
            }
            catch
            {
                return Json(new { success = true, isPaid = false });
            }
        }

        // ==================== 3. XEM / HỦY LỊCH KHÁM ====================
        public ActionResult LichKham(string trangThai = "")
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var lichKham = db.GetLichKhamByMaBN(bn.MaBN);

            if (!string.IsNullOrEmpty(trangThai))
            {
                if (trangThai == "cho")
                    lichKham = lichKham.FindAll(x => x.TrangThai == "Chờ xử lý");
                else if (trangThai == "xacnhan")
                    lichKham = lichKham.FindAll(x => x.TrangThai == "Đã xác nhận");
                else if (trangThai == "huy")
                    lichKham = lichKham.FindAll(x => x.TrangThai == "Hủy");
            }

            return View(lichKham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyLich(string id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            try
            {
                bool ok = db.HuyLichKham(id, bn.MaBN);
                if (ok)
                    TempData["Success"] = "Hủy lịch khám thành công!";
                else
                    TempData["Error"] = "Không thể hủy lịch. Chỉ lịch ở trạng thái 'Chờ xử lý' mới được hủy.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // ==================== 4. TRẠNG THÁI KHÁM ====================
        public ActionResult TrangThaiKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetTrangThaiKhamByMaBN(bn.MaBN);
            return View(list);
        }

        // ==================== 5. LỊCH SỬ KHÁM ====================
        public ActionResult LichSuKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetLichSuKhamByMaBN(bn.MaBN);
            return View(list);
        }

        // ==================== 6. ĐƠN THUỐC ====================
        public ActionResult DonThuoc()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetDonThuocByMaBN(bn.MaBN);
            return View(list);
        }

        public ActionResult ChiTietDonThuoc(string id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var detail = db.GetChiTietDonThuoc(id);
            if (detail == null || detail.ChiTiet.Count == 0)
                return HttpNotFound("Không tìm thấy đơn thuốc!");

            return View(detail);
        }

        // ==================== 7. HÓA ĐƠN ====================
        public ActionResult HoaDon()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetHoaDonByMaBN(bn.MaBN);
            return View(list);
        }

        public ActionResult ChiTietHoaDon(string id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var detail = db.GetChiTietHoaDon(id);
            if (detail == null)
                return HttpNotFound("Không tìm thấy hóa đơn!");

            return View(detail);
        }




        // ==================== HÀM PHỤ TRỢ: GIẢ LẬP THANH TOÁN THẺ THÀNH CÔNG ====================
        [HttpPost]
        public JsonResult XacNhanThanhToanTheMock(string maHD, string maPhieuDK)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                {
                    conn.Open();
                    // Cập nhật Hóa đơn thành Đã thanh toán, hình thức là THẺ, và đổi Phiếu ĐK thành Chờ xử lý
                    string sql = @"
                        UPDATE HOADON 
                        SET TrangThaiThanhToan = N'Đã thanh toán', NgayThanhToan = GETDATE(), HinhThucThanhToan = N'Thẻ' 
                        WHERE MaHD = @MaHD;

                        UPDATE CT_HOADON_DV 
                        SET TrangThaiThanhToan = N'Đã thanh toán' 
                        WHERE MaHD = @MaHD;

                        UPDATE PHIEUDANGKY 
                        SET TrangThai = N'Chờ xử lý' 
                        WHERE MaPhieuDK = @MaPhieuDK;";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    cmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK);
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        // ==================== HÀM PHỤ TRỢ: HỦY LỊCH KHI KHÁCH KHÔNG THANH TOÁN ====================
        [HttpPost]
        public JsonResult HuyDatLichOnline(string maPhieuDK)
        {
            // Check bảo mật: Tránh gọi API ẩn danh
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại!" });

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                {
                    conn.Open();
                    // Cập nhật cả Hóa đơn và Phiếu Đăng Ký thành Hủy. 
                    // Chặn thêm điều kiện MaBN để chắc chắn đây là lịch của ông nội đang đăng nhập
                    string sql = @"
                        UPDATE HOADON 
                        SET TrangThaiThanhToan = N'Đã hủy' 
                        WHERE MaPhieuDK = @MaPhieuDK AND MaBN = @MaBN;

                        UPDATE CT_HOADON_DV 
                        SET TrangThaiThanhToan = N'Hủy' 
                        WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MaPhieuDK = @MaPhieuDK);

                        UPDATE PHIEUDANGKY 
                        SET TrangThai = N'Hủy' 
                        WHERE MaPhieuDK = @MaPhieuDK AND MaBN = @MaBN;";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK);
                    cmd.Parameters.AddWithValue("@MaBN", bn.MaBN);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        return Json(new { success = true, message = "Hủy thành công" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy dữ liệu để hủy." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
