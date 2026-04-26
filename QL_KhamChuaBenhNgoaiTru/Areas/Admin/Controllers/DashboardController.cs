using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        private readonly DashboardDB db = new DashboardDB();

        // GET: Admin/Dashboard
        // 1. Hàm Index giờ chỉ đóng vai trò cái "Vỏ"
        public ActionResult Index()
        {
            ViewBag.Title = "Trung tâm điều khiển";
            return View();
        }

        // 2. Hàm mới: Xử lý data và trả về "Ruột" Tổng quan
        public ActionResult LoadDashboardTongQuan()
        {
            try
            {
                var model = new DashboardViewModel();
                model.ThongKe = db.GetThongKeTongQuan();
                model.ThongKeKhamBenh = db.GetThongKeKhamBenh();
                model.DoanhThuThang = db.GetDoanhThu();
                model.DoanhThu7Ngay = db.GetDoanhThu7Ngay();
                model.DoanhThu12Thang = db.GetDoanhThu12Thang(DateTime.Now.Year);
                model.ThuocSapHetHan = db.GetThuocSapHetHan(30);
                model.ThuocSapHetHang = db.GetThuocSapHetHang(10);
                model.PhieuNhapGanDay = db.GetPhieuNhapGanDay(5);
                model.BacSiKhamNhieuNhat = db.GetBacSiKhamNhieuNhat(5);

                // Chú ý: Trả về PartialView thay vì View
                return PartialView("_DashboardTongQuan", model);
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger'>Lỗi tải dữ liệu: " + ex.Message + "</div>");
            }
        }

        // ===================== AJAX: Lấy doanh thu theo tháng =====================
        public ActionResult GetDoanhThuThang(int thang, int nam)
        {
            try
            {
                var data = db.GetDoanhThu(thang, nam);
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        TongTienGoc = data.TongTienGoc,
                        TienBHYT = data.TienBHYT,
                        TienBenhNhanTra = data.TienBenhNhanTra,
                        TongDoanhThu = data.TongDoanhThu,
                        DoanhThuThucNhan = data.DoanhThuThucNhan
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ===================== AJAX: Lấy dữ liệu biểu đồ =====================
        public ActionResult GetChartData(string type = "week", int nam = 0)
        {
            try
            {
                if (nam == 0) nam = DateTime.Now.Year;

                if (type == "month")
                {
                    var data = db.GetDoanhThu12Thang(nam);
                    return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = db.GetDoanhThu7Ngay();
                    return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Load Giao diện Dashboard Doanh Thu
        public ActionResult LoadDashboardDoanhThu()
        {
            try
            {
                DateTime denNgay = DateTime.Now;
                DateTime tuNgay = denNgay.AddYears(-1);

                // Mặc định load lên là xem theo "month" (Tháng)
                var model = db.GetPhanTichDoanhThu(tuNgay, denNgay, "month");

                ViewBag.TuNgay = tuNgay.ToString("yyyy-MM-dd");
                ViewBag.DenNgay = denNgay.ToString("yyyy-MM-dd");

                return PartialView("_DashboardDoanhThu", model);
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger'>Lỗi: " + ex.Message + "</div>");
            }
        }

        // API AJAX xử lý khi chọn Lịch hoặc Dropdown
        [HttpPost]
        public ActionResult FilterDoanhThu(string tuNgay, string denNgay, string groupBy = "day")
        {
            try
            {
                // Ép kiểu thủ công để chống lỗi Format ngày của C#
                DateTime dtTu = DateTime.Parse(tuNgay);
                DateTime dtDen = DateTime.Parse(denNgay);

                // Truyền biến groupBy (day/week/month/year) xuống DB
                var data = db.GetPhanTichDoanhThu(dtTu, dtDen, groupBy);
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // Load Giao diện Dashboard Bệnh Nhân
        public ActionResult LoadDashboardBenhNhan()
        {
            try
            {
                DateTime denNgay = DateTime.Now;

                // SỬA Ở ĐÂY: Lùi về 1 năm thay vì 1 tháng
                DateTime tuNgay = denNgay.AddYears(-1);

                var model = db.GetThongKeBenhNhan(tuNgay, denNgay, 0);

                ViewBag.TuNgay = tuNgay.ToString("yyyy-MM-dd");
                ViewBag.DenNgay = denNgay.ToString("yyyy-MM-dd");

                return PartialView("_DashboardBenhNhan", model);
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger'>Lỗi: " + ex.Message + "</div>");
            }
        }

        // API AJAX xử lý lọc Dashboard Bệnh nhân
        [HttpPost]
        public ActionResult FilterBenhNhan(string tuNgay, string denNgay, int maKhoa = 0)
        {
            try
            {
                DateTime dtTu = DateTime.Parse(tuNgay);
                DateTime dtDen = DateTime.Parse(denNgay);

                var data = db.GetThongKeBenhNhan(dtTu, dtDen, maKhoa);
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }






        // Load Giao diện Dashboard Kho Dược
        public ActionResult LoadDashboardKhoDuoc()
        {
            try
            {
                DateTime denNgay = DateTime.Now;
                DateTime tuNgay = denNgay.AddMonths(-6); // Xem 6 tháng gần nhất

                var model = db.GetThongKeKhoDuoc(tuNgay, denNgay, 0);

                ViewBag.TuNgay = tuNgay.ToString("yyyy-MM-dd");
                ViewBag.DenNgay = denNgay.ToString("yyyy-MM-dd");

                return PartialView("_DashboardKhoDuoc", model);
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger'>Lỗi: " + ex.Message + "</div>");
            }
        }

        // API AJAX xử lý lọc Kho Dược
        [HttpPost]
        public ActionResult FilterKhoDuoc(string tuNgay, string denNgay, int maKho = 0)
        {
            try
            {
                DateTime dtTu = DateTime.Parse(tuNgay);
                DateTime dtDen = DateTime.Parse(denNgay);

                var data = db.GetThongKeKhoDuoc(dtTu, dtDen, maKho);
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        // ===================== AJAX: Cảnh báo kho =====================
        public ActionResult GetCanhBaoKho()
        {
            try
            {
                var hetHan = db.GetThuocSapHetHan(30);
                var hetHang = db.GetThuocSapHetHang(10);

                int totalCanhBao = hetHan.Count + hetHang.Count;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        SoCanhBao = totalCanhBao,
                        SoHetHan = hetHan.Count,
                        SoHetHang = hetHang.Count
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }





    // ViewModel cho Dashboard
    public class DashboardViewModel
    {
        public DashboardThongKe ThongKe { get; set; } = new DashboardThongKe();
        public DashboardThongKe ThongKeKhamBenh { get; set; } = new DashboardThongKe();
        public DashboardThongKe DoanhThuThang { get; set; } = new DashboardThongKe();
        public List<DoanhThuNgay> DoanhThu7Ngay { get; set; } = new List<DoanhThuNgay>();
        public List<DoanhThuThang> DoanhThu12Thang { get; set; } = new List<DoanhThuThang>();
        public List<ThuocSapHetHan> ThuocSapHetHan { get; set; } = new List<ThuocSapHetHan>();
        public List<ThuocSapHetHang> ThuocSapHetHang { get; set; } = new List<ThuocSapHetHang>();
        public List<PhieuNhapItem> PhieuNhapGanDay { get; set; } = new List<PhieuNhapItem>();
        public List<BacSiThongKe> BacSiKhamNhieuNhat { get; set; } = new List<BacSiThongKe>();
    }
}
