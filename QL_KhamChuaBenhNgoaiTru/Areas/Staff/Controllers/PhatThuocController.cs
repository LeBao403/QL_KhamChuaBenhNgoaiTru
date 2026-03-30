using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    public class PhatThuocController : BaseStaffController
    {
        private PhatThuocDB db = new PhatThuocDB();

        // 1. TRANG CHỜ PHÁT THUỐC (INDEX)
        public ActionResult Index(string search = "", string sortCol = "MaHD", string sortDir = "DESC",
                                  string tuNgay = "", string denNgay = "", int page = 1)
        {
            var nv = Session["NhanVien"] as NhanVien;
            if (nv == null) return RedirectToAction("Login", "Account");

            // Lấy mã phòng từ Session, ép kiểu int? về int
            int maPhongTruc = nv.MaPhong ?? 22;

            // Mặc định xem trong 7 ngày gần nhất
            DateTime dTu = string.IsNullOrEmpty(tuNgay) ? DateTime.Now.AddDays(-7).Date : DateTime.Parse(tuNgay);
            DateTime dDen = string.IsNullOrEmpty(denNgay) ? DateTime.Now.Date : DateTime.Parse(denNgay);

            // Gọi hàm chuyên biệt lấy danh sách chờ phát theo phòng
            var allData = db.GetDanhSachChoPhat(maPhongTruc, dTu, dDen, search, sortCol, sortDir);

            // Phân trang đơn giản cho Index
            int pageSize = 10;
            int totalRow = allData.Count;
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            page = page < 1 ? 1 : (page > totalPage && totalPage > 0 ? totalPage : page);
            var pagedData = allData.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.DanhSach = pagedData;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalRow = totalRow;
            ViewBag.Search = search;
            ViewBag.TuNgay = dTu.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = dDen.ToString("yyyy-MM-dd");
            ViewBag.MaPhong = maPhongTruc;

            return View();
        }

        // 2. TRANG LỊCH SỬ PHÁT THUỐC
        public ActionResult LichSu(string search = "", string sortCol = "NgayThanhToan", string sortDir = "DESC",
                          string tuNgay = "", string denNgay = "", int page = 1)
        {
            // Mặc định lọc trong 7 ngày gần nhất nếu chưa chọn ngày
            DateTime dTu = string.IsNullOrEmpty(tuNgay) ? DateTime.Now.AddDays(-7).Date : DateTime.Parse(tuNgay);
            DateTime dDen = string.IsNullOrEmpty(denNgay) ? DateTime.Now.Date : DateTime.Parse(denNgay);

            int pageSize = 15;

            // Gọi hàm phân trang từ DB
            var result = db.GetLichSuPhatThuoc_Pagination(search, dTu, dDen, page, pageSize, sortCol, sortDir);

            int totalPage = (int)Math.Ceiling((double)result.TotalRow / pageSize);

            ViewBag.DanhSach = result.Data;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalRow = result.TotalRow;
            ViewBag.TuNgay = dTu.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = dDen.ToString("yyyy-MM-dd");
            ViewBag.Search = search;
            ViewBag.SortCol = sortCol;

            return View();
        }

        // 3. API LẤY CHI TIẾT (Cho Modal - AJAX cái này là bắt buộc)
        [HttpGet]
        public JsonResult GetChiTietDonThuoc(int maDonThuoc, int maHD)
        {
            try
            {
                var data = db.GetChiTietDonThuocVaThongTin(maDonThuoc, maHD);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 4. API XÁC NHẬN PHÁT THUỐC
        [HttpPost]
        public JsonResult XacNhanPhat(int maDonThuoc, int maHD, int maPhong)
        {
            try
            {
                var nv = Session["NhanVien"] as NhanVien;
                string maNV = nv != null ? nv.MaNV : "NV040";

                var result = db.XacNhanPhatThuoc(maDonThuoc, maHD, maNV, maPhong);
                return Json(new { success = result.IsSuccess, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}