using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Areas.NhanVienKho.Controllers
{
    public class ThuocController : BaseNhanVienKhoController
    {
        private readonly KhoNhapDB db = new KhoNhapDB();

        public ActionResult Index(string keyword = "", string maLoaiThuoc = "")
        {
            ViewBag.Title = "Tra cứu thuốc";

            try
            {
                var dsThuoc = db.SearchThuoc(keyword, maLoaiThuoc);
                var dsLoaiThuoc = db.GetAllLoaiThuoc();

                ViewBag.DsThuoc = dsThuoc;
                ViewBag.DsLoaiThuoc = dsLoaiThuoc;
                ViewBag.Keyword = keyword;
                ViewBag.MaLoaiThuoc = maLoaiThuoc;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }
    }
}
