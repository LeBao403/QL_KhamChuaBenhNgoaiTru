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
        public ActionResult Index()
        {
            ViewBag.Title = "Tổng quan";

            try
            {
                var model = new DashboardViewModel();

                // 1. Thống kê tổng quan
                model.ThongKe = db.GetThongKeTongQuan();

                // 2. Thống kê khám bệnh
                model.ThongKeKhamBenh = db.GetThongKeKhamBenh();

                // 3. Doanh thu tháng hiện tại
                model.DoanhThuThang = db.GetDoanhThu();

                // 4. Doanh thu 7 ngày gần nhất (cho biểu đồ)
                model.DoanhThu7Ngay = db.GetDoanhThu7Ngay();

                // 5. Doanh thu 12 tháng (cho biểu đồ)
                model.DoanhThu12Thang = db.GetDoanhThu12Thang(DateTime.Now.Year);

                // 6. Thuốc sắp hết hạn
                model.ThuocSapHetHan = db.GetThuocSapHetHan(30);

                // 7. Thuốc sắp hết hàng
                model.ThuocSapHetHang = db.GetThuocSapHetHang(10);

                // 8. Phiếu nhập gần đây
                model.PhieuNhapGanDay = db.GetPhieuNhapGanDay(5);

                // 9. Bác sĩ khám nhiều nhất
                model.BacSiKhamNhieuNhat = db.GetBacSiKhamNhieuNhat(5);

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải dữ liệu Dashboard: " + ex.Message;
                return View("Error");
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
