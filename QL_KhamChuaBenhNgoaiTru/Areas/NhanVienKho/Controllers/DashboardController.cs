using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Areas.NhanVienKho.Controllers
{
    public class DashboardController : BaseNhanVienKhoController
    {
        private readonly KhoNhapDB db = new KhoNhapDB();

        public ActionResult Index()
        {
            ViewBag.Title = "Dashboard";
            int currentYear = DateTime.Now.Year;
            ViewBag.Year = currentYear;

            // Tách từng bước: nếu một query biểu đồ lỗi SQL thì vẫn hiển thị được thống kê tổng (tránh toàn bộ = 0).
            var thongKe = new KhoNhapDB.ThongKeDashboard();
            try
            {
                var tk = db.GetThongKeDashboard();
                if (tk != null) thongKe = tk;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard GetThongKeDashboard: " + ex);
            }

            ViewBag.ChartTonKhoTheoKho = new List<KhoNhapDB.ChartDataItem>();
            try
            {
                var c = db.GetTonKhoTheoKho();
                if (c != null) ViewBag.ChartTonKhoTheoKho = c;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard GetTonKhoTheoKho: " + ex);
            }

            ViewBag.ChartPhieuNhapThang = new List<KhoNhapDB.ChartDataItem>();
            try
            {
                var c = db.GetPhieuNhapTheoThang(currentYear);
                if (c != null) ViewBag.ChartPhieuNhapThang = c;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard GetPhieuNhapTheoThang: " + ex);
            }

            ViewBag.ChartTonKhoLoai = new List<KhoNhapDB.ChartDataItem>();
            try
            {
                var c = db.GetTonKhoTheoLoai();
                if (c != null) ViewBag.ChartTonKhoLoai = c;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard GetTonKhoTheoLoai: " + ex);
            }

            return View(thongKe);
        }
    }
}
