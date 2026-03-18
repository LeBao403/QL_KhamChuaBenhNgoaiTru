using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class HomeController : Controller
    {
        HomeDB db = new HomeDB();

        public ActionResult Index()
        {
            // Chỉ lấy dữ liệu thật từ Database (Khoa, Bác sĩ, Thống kê)
            var model = db.GetHomeData();
            return View(model);
        }

        public ActionResult DatLich()
        {
            return View();
        }

        public ActionResult BangGia()
        {
            return View();
        }
    }
}