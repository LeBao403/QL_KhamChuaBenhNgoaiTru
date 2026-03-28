using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class KhoController : BaseAdminController
    {
        private readonly KhoDB db = new KhoDB();

        // ===================== INDEX - TỔNG QUAN KHO =====================
        public ActionResult Index(int page = 1,
            string keyword = "",
            string maKho = "",
            string trangThaiTon = "")
        {
            ViewBag.Title = "Kho Dược - Vật tư";

            int pageSize = 15;

            try
            {
                int? maKhoFilter = null;
                if (!string.IsNullOrEmpty(maKho) && int.TryParse(maKho, out int mk))
                    maKhoFilter = mk;

                var dsKho = db.GetAllKho();
                ViewBag.DsKho = dsKho;

                var tongQuan = db.GetTongQuanKho(maKhoFilter);
                ViewBag.TongQuan = tongQuan;

                var dsTonKho = db.GetTonKho(page, pageSize, keyword, maKho, trangThaiTon);
                int totalCount = db.GetTonKhoCount(keyword, maKho, trangThaiTon);

                ViewBag.DsTonKho = dsTonKho;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                ViewBag.Keyword = keyword;
                ViewBag.MaKho = maKho;
                ViewBag.TrangThaiTon = trangThaiTon;

                return View();
            }
            catch (SqlException sqlex)
            {
                System.Diagnostics.Debug.WriteLine("SQL Error: " + sqlex.Message);
                ViewBag.ErrorMessage = "Lỗi cơ sở dữ liệu: " + sqlex.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // AJAX: Load bảng tồn kho
        public ActionResult LoadTonKhoTable(int page = 1,
            string keyword = "",
            string maKho = "",
            string trangThaiTon = "")
        {
            int pageSize = 15;
            var dsTonKho = db.GetTonKho(page, pageSize, keyword, maKho, trangThaiTon);
            int totalCount = db.GetTonKhoCount(keyword, maKho, trangThaiTon);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Keyword = keyword;
            ViewBag.MaKho = maKho;
            ViewBag.TrangThaiTon = trangThaiTon;

            return PartialView("_TonKhoTable", dsTonKho);
        }

        // ===================== TẠO PHIẾU NHẬP =====================
        public ActionResult Create()
        {
            ViewBag.Title = "Tạo phiếu nhập kho";
            try
            {
                ViewBag.DsThuoc = db.GetAllThuoc();
                ViewBag.DsNhaCungCap = db.GetAllNhaCungCap();
                ViewBag.DsKho = db.GetAllKho();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            try
            {
                var nv = Session["NhanVien"] as NhanVien;
                if (nv == null)
                {
                    TempData["Error"] = "Không xác định được nhân viên đăng nhập.";
                    return RedirectToAction("Create");
                }

                string maKhoStr = form["MaKho"];
                string maNSXStr = form["MaNSX"];
                string ghiChu = form["GhiChu"] ?? "";

                if (string.IsNullOrEmpty(maKhoStr) || !int.TryParse(maKhoStr, out int maKho))
                {
                    TempData["Error"] = "Vui lòng chọn kho nhập hàng.";
                    return RedirectToAction("Create");
                }

                if (string.IsNullOrEmpty(maNSXStr) || !int.TryParse(maNSXStr, out int maNSX))
                {
                    TempData["Error"] = "Vui lòng chọn nhà cung cấp.";
                    return RedirectToAction("Create");
                }

                var chiTiets = new List<CT_PhieuNhapInput>();
                var thuocKeys = form.AllKeys.Where(k => k != null && k.StartsWith("MaThuoc_")).ToList();

                foreach (var key in thuocKeys)
                {
                    var idx = key.Replace("MaThuoc_", "");
                    var maThuoc = form[key];
                    var maLo = form["MaLo" + idx];
                    var soLuongStr = form["SoLuong" + idx];
                    var donGiaStr = form["DonGia" + idx];
                    var ngaySXStr = form["NgaySanXuat" + idx];
                    var hanSDStr = form["HanSuDung" + idx];

                    if (string.IsNullOrWhiteSpace(maThuoc)) continue;

                    if (string.IsNullOrWhiteSpace(soLuongStr) || !int.TryParse(soLuongStr, out int soLuong) || soLuong <= 0)
                    {
                        TempData["Error"] = "Số lượng dòng không hợp lệ. Vui lòng kiểm tra lại.";
                        return RedirectToAction("Create");
                    }

                    if (string.IsNullOrWhiteSpace(donGiaStr) || !decimal.TryParse(donGiaStr, out decimal donGia) || donGia < 0)
                    {
                        TempData["Error"] = "Đơn giá dòng không hợp lệ. Vui lòng kiểm tra lại.";
                        return RedirectToAction("Create");
                    }

                    DateTime? ngaySX = !string.IsNullOrWhiteSpace(ngaySXStr) && DateTime.TryParse(ngaySXStr, out DateTime parsedNSX) ? (DateTime?)parsedNSX : null;
                    DateTime hanSD = DateTime.Now.AddMonths(6);
                    if (!string.IsNullOrWhiteSpace(hanSDStr) && DateTime.TryParse(hanSDStr, out DateTime parsedHSD))
                        hanSD = parsedHSD;

                    chiTiets.Add(new CT_PhieuNhapInput
                    {
                        MaThuoc = maThuoc.Trim(),
                        MaLo = maLo?.Trim() ?? "",
                        NgaySanXuat = ngaySX,
                        HanSuDung = hanSD,
                        SoLuongNhap = soLuong,
                        DonGiaNhap = donGia
                    });
                }

                if (chiTiets.Count == 0)
                {
                    TempData["Error"] = "Vui lòng thêm ít nhất một chi tiết thuốc vào phiếu nhập.";
                    return RedirectToAction("Create");
                }

                int maPhieuNhap = db.TaoPhieuNhap(nv.MaNV, maNSX, maKho, ghiChu, chiTiets);

                if (maPhieuNhap > 0)
                {
                    TempData["Success"] = "Tạo phiếu nhập #" + maPhieuNhap + " thành công! Phiếu đang chờ duyệt.";
                    return RedirectToAction("DanhSachPhieuNhap");
                }

                TempData["Error"] = "Không thể tạo phiếu nhập. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Create");
        }

        // ===================== DANH SÁCH PHIẾU NHẬP =====================
        public ActionResult DanhSachPhieuNhap(int page = 1, string keyword = "", string trangThai = "")
        {
            ViewBag.Title = "Danh sách phiếu nhập kho";

            int pageSize = 15;

            try
            {
                var dsPhieuNhap = db.GetPhieuNhap(page, pageSize, keyword, trangThai);
                int totalCount = db.GetPhieuNhapCount(keyword, trangThai);

                ViewBag.DsPhieuNhap = dsPhieuNhap;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                ViewBag.Keyword = keyword;
                ViewBag.TrangThai = trangThai;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // AJAX: Load bảng phiếu nhập
        public ActionResult LoadPhieuNhapTable(int page = 1, string keyword = "", string trangThai = "")
        {
            int pageSize = 15;
            var dsPhieuNhap = db.GetPhieuNhap(page, pageSize, keyword, trangThai);
            int totalCount = db.GetPhieuNhapCount(keyword, trangThai);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;

            return PartialView("_PhieuNhapTable", dsPhieuNhap);
        }

        // ===================== CHI TIẾT PHIẾU NHẬP =====================
        public ActionResult ChiTietPhieuNhap(int id)
        {
            var phieu = db.GetPhieuNhapById(id);
            if (phieu == null)
                return HttpNotFound("Không tìm thấy phiếu nhập!");

            var chiTiet = db.GetCTPhieuNhap(id);

            ViewBag.ChiTiet = chiTiet;
            ViewBag.DsKho = db.GetAllKho();
            ViewBag.DsPhong = db.GetAllPhong();

            return View(phieu);
        }

        // ===================== DUYỆT PHIẾU NHẬP =====================
        [HttpPost]
        public ActionResult DuyetPhieuNhap(int id)
        {
            try
            {
                var nv = Session["NhanVien"] as NhanVien;
                if (nv == null)
                    return Json(new { success = false, message = "Không xác định được nhân viên." });

                bool result = db.DuyetPhieuNhap(id, nv.MaNV);

                if (result)
                    return Json(new { success = true, message = "Duyệt phiếu nhập thành công!" });
                else
                    return Json(new { success = false, message = "Không thể duyệt phiếu nhập. Phiếu có thể chưa chọn kho." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ===================== HỦY PHIẾU NHẬP =====================
        [HttpPost]
        public ActionResult HuyPhieuNhap(int id)
        {
            try
            {
                var nv = Session["NhanVien"] as NhanVien;
                if (nv == null)
                    return Json(new { success = false, message = "Không xác định được nhân viên." });

                bool result = db.HuyPhieuNhap(id, nv.MaNV);

                if (result)
                    return Json(new { success = true, message = "Đã hủy phiếu nhập thành công!" });
                else
                    return Json(new { success = false, message = "Không thể hủy phiếu nhập. Phiếu có thể đã được duyệt." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ===================== CẬP NHẬT SỐ LƯỢNG TỒN KHO =====================
        [HttpPost]
        public ActionResult CapNhatSoLuong(int maTonKho, int soLuongMoi)
        {
            try
            {
                bool result = db.CapNhatSoLuongTonKho(maTonKho, soLuongMoi);

                if (result)
                    return Json(new { success = true, message = "Cập nhật số lượng thành công!" });
                else
                    return Json(new { success = false, message = "Không tìm thấy dữ liệu để cập nhật." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ===================== AJAX: LẤY ĐƠN VỊ THUỐC =====================
        public ActionResult GetDonViThuoc(string maThuoc)
        {
            var dsThuoc = db.GetAllThuoc();
            var thuoc = dsThuoc.FirstOrDefault(x => x.MaThuoc == maThuoc);
            if (thuoc != null)
                return Json(new { success = true, donVi = thuoc.DonViCoBan }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // ===================== AJAX: LẤY GIÁ NHẬP MẶC ĐỊNH =====================
        public ActionResult GetGiaNhapMacDinh(string maThuoc)
        {
            // Lấy giá bán làm tham khảo (có thể điều chỉnh)
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                conn.Open();
                string sql = "SELECT GiaBan FROM THUOC WHERE MaThuoc = @MaThuoc";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaThuoc", maThuoc);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Json(new { success = true, giaNhap = Convert.ToDecimal(result) }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
    }
}
