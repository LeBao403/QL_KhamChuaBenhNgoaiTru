using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Linq;
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

        // =========================================================
        // TRANG ĐỘI NGŨ BÁC SĨ (AJAX + Partial View)
        // =========================================================
        public ActionResult BacSi(string searchString, string khoa, int page = 1)
        {
            var danhSachBacSi = db.GetAllBacSi();

            // 1. Lọc theo Tên
            if (!string.IsNullOrEmpty(searchString))
            {
                danhSachBacSi = danhSachBacSi.Where(b =>
                    b.HoTen != null && b.HoTen.ToLower().Contains(searchString.ToLower())
                ).ToList();
            }

            // 2. Lọc theo Khoa
            if (!string.IsNullOrEmpty(khoa))
            {
                danhSachBacSi = danhSachBacSi.Where(b =>
                    b.TenKhoa != null && b.TenKhoa.ToLower().Contains(khoa.ToLower())
                ).ToList();
            }

            // 3. Phân trang
            int pageSize = 8;
            int totalItems = danhSachBacSi.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedList = danhSachBacSi.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentKhoa = khoa;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            // 4. KIỂM TRA AJAX ĐỂ TRẢ VỀ PARTIAL VIEW
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BacSiList", pagedList);
            }

            return View(pagedList);
        }

        // =========================================================
        // 3. TRANG DANH SÁCH CHUYÊN KHOA
        // =========================================================
        public ActionResult ChuyenKhoa()
        {
            // Gọi DB lấy toàn bộ các Khoa
            var danhSachKhoa = db.GetAllKhoa();
            return View(danhSachKhoa);
        }


        // =========================================================
        // TRANG GIỚI THIỆU (VỀ CHÚNG TÔI)
        // =========================================================
        public ActionResult GioiThieu()
        {
            // Gọi hàm kéo thông tin Giám đốc và Thống kê từ DB
            var model = db.GetGioiThieuData();

            // Trả Model ra View để render
            return View(model);
        }
    }
}