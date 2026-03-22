using System;
using System.Collections.Generic;
using System.Linq;
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

            try
            {
                var thongKe = db.GetThongKeDashboard();
                if (thongKe == null) thongKe = new KhoNhapDB.ThongKeDashboard();

                int currentYear = DateTime.Now.Year;
                ViewBag.ChartTonKhoTheoKho = db.GetTonKhoTheoKho() ?? new List<KhoNhapDB.ChartDataItem>();
                ViewBag.ChartPhieuNhapThang = db.GetPhieuNhapTheoThang(currentYear) ?? new List<KhoNhapDB.ChartDataItem>();
                ViewBag.ChartTonKhoLoai = db.GetTonKhoTheoLoai() ?? new List<KhoNhapDB.ChartDataItem>();
                ViewBag.Year = currentYear;

                return View(thongKe);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard Error: " + ex.Message);
                var empty = new KhoNhapDB.ThongKeDashboard();
                ViewBag.ChartTonKhoTheoKho = new List<KhoNhapDB.ChartDataItem>();
                ViewBag.ChartPhieuNhapThang = new List<KhoNhapDB.ChartDataItem>();
                ViewBag.ChartTonKhoLoai = new List<KhoNhapDB.ChartDataItem>();
                ViewBag.Year = DateTime.Now.Year;
                return View(empty);
            }
        }
    }
}
