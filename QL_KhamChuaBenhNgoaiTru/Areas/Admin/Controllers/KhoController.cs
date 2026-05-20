using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

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

                ViewBag.ThongKeCacKho = db.GetThongKeCacKho();
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
        public ActionResult XoaChiTietPhieuNhap(int id)
        {
            try
            {
                bool result = db.XoaChiTietPhieuNhap(id);
                if (result)
                    return Json(new { success = true, message = "Đã xóa chi tiết!" });

                return Json(new
                {
                    success = false,
                    message = "Không thể xóa chi tiết này. Phiếu có thể không ở trạng thái chờ duyệt hoặc chi tiết không tồn tại."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

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


        // ===================== PHIẾU CHUYỂN KHO =====================
        public ActionResult DanhSachPhieuChuyen(int page = 1, string keyword = "", string trangThai = "")
        {
            int pageSize = 10;
            var phieuChuyens = db.GetPhieuChuyen(page, pageSize, keyword, trangThai);
            int totalRecords = db.GetPhieuChuyenCount(keyword, trangThai);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;

            return View(phieuChuyens);
        }



        public ActionResult ChiTietPhieuChuyen(int id)
        {
            var phieu = db.GetPhieuChuyenById(id);
            if (phieu == null) return HttpNotFound();

            ViewBag.ChiTiet = db.GetCTPhieuChuyen(id);
            return View(phieu);
        }

        [HttpPost]
        public ActionResult DuyetPhieuChuyen(int id)
        {
            try
            {
                string maNV = Session["MaNV"].ToString();
                bool result = db.DuyetPhieuChuyen(id, maNV);
                if (result)
                    return Json(new { success = true, message = "Đã duyệt phiếu chuyển kho!" });
                else
                    return Json(new { success = false, message = "Không thể duyệt. Kiểm tra lại tồn kho nguồn." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult XoaChiTietPhieuChuyen(int id)
        {
            try
            {
                bool result = db.XoaChiTietPhieuChuyen(id);
                if (result)
                    return Json(new { success = true, message = "Đã xóa chi tiết!" });

                return Json(new
                {
                    success = false,
                    message = "Không thể xóa chi tiết này. Phiếu có thể không ở trạng thái chờ duyệt hoặc chi tiết không tồn tại."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult HuyPhieuChuyen(int id)
        {
            try
            {
                string maNV = Session["MaNV"].ToString();
                bool result = db.HuyPhieuChuyen(id, maNV);
                if (result)
                    return Json(new { success = true, message = "Đã hủy phiếu chuyển kho!" });
                else
                    return Json(new { success = false, message = "Không thể hủy phiếu này." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }



        // ===================== IN PHIẾU NHẬP =====================
        public ActionResult PrintPhieuNhap(int id)
        {
            var phieu = db.GetPhieuNhapById(id);
            if (phieu == null) return HttpNotFound();

            ViewBag.ChiTiet = db.GetCTPhieuNhap(id);
            return View(phieu);
        }

        // ===================== IN PHIẾU CHUYỂN KHO =====================
        public ActionResult PrintPhieuChuyen(int id)
        {
            var phieu = db.GetPhieuChuyenById(id);
            if (phieu == null) return HttpNotFound();

            ViewBag.ChiTiet = db.GetCTPhieuChuyen(id);
            return View(phieu);
        }
    }
}
