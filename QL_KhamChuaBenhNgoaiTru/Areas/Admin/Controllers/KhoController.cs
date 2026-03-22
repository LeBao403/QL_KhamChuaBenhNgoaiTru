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
            string maPhong = "",
            string maKho = "",
            string trangThaiTon = "")
        {
            ViewBag.Title = "Kho Dược - Vật tư";

            int pageSize = 15;

            try
            {
                // 1. Tổng quan kho
                int? maKhoFilter = null;
                if (!string.IsNullOrEmpty(maKho) && int.TryParse(maKho, out int mk))
                    maKhoFilter = mk;

                var dsKho = db.GetAllKho();
                ViewBag.DsKho = dsKho;
                ViewBag.DsPhong = db.GetAllPhong();

                var tongQuan = db.GetTongQuanKho(maKhoFilter);
                ViewBag.TongQuan = tongQuan;

                // 2. Danh sách tồn kho
                var dsTonKho = db.GetTonKho(page, pageSize, keyword, maPhong, maKho, trangThaiTon);
                int totalCount = db.GetTonKhoCount(keyword, maPhong, maKho, trangThaiTon);

                ViewBag.DsTonKho = dsTonKho;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                ViewBag.Keyword = keyword;
                ViewBag.MaPhong = maPhong;
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
            string maPhong = "",
            string maKho = "",
            string trangThaiTon = "")
        {
            int pageSize = 15;
            var dsTonKho = db.GetTonKho(page, pageSize, keyword, maPhong, maKho, trangThaiTon);
            int totalCount = db.GetTonKhoCount(keyword, maPhong, maKho, trangThaiTon);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Keyword = keyword;
            ViewBag.MaPhong = maPhong;
            ViewBag.MaKho = maKho;
            ViewBag.TrangThaiTon = trangThaiTon;

            return PartialView("_TonKhoTable", dsTonKho);
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
        public ActionResult DuyetPhieuNhap(int id, int maKhoNhan)
        {
            try
            {
                var nv = Session["NhanVien"] as NhanVien;
                if (nv == null)
                    return Json(new { success = false, message = "Không xác định được nhân viên." });

                bool result = db.DuyetPhieuNhap(id, nv.MaNV, maKhoNhan);

                if (result)
                    return Json(new { success = true, message = "Duyệt phiếu nhập thành công!" });
                else
                    return Json(new { success = false, message = "Không thể duyệt phiếu nhập." });
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
