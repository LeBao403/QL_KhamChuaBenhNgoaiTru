using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class DichVuController : BaseAdminController
    {
        private DichVuDB db = new DichVuDB();

        // 1. TRANG CHỦ & BỘ LỌC
        public ActionResult Index(int page = 1, string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null, string sortPrice = "")
        {
            int pageSize = 10;
            try
            {
                var dsdv = db.GetDanhSachDichVu(page, pageSize, keyword, maLoai, minPrice, maxPrice, sortPrice);
                int totalCount = db.GetTotalRecord(keyword, maLoai, minPrice, maxPrice);

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.Keyword = keyword;
                ViewBag.MaLoai = maLoai;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SortPrice = sortPrice;
                ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();

                return View(dsdv);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // 2. AJAX LIVE SEARCH
        public ActionResult Search(string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null, string sortPrice = "")
        {
            var dsdv = db.GetDanhSachDichVu(1, 10, keyword, maLoai, minPrice, maxPrice, sortPrice);
            return PartialView("_DichVuTable", dsdv);
        }

        // 3. XEM CHI TIẾT
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            var dv = db.GetDichVuById(id);
            if (dv == null) return HttpNotFound();
            return View(dv);
        }

        // 4. THÊM MỚI (POST) - ĐÃ BỎ CHECK GIA BHYT
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            var model = new DichVuViewModel { MaDV = db.GenerateNextMaDV() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DichVuViewModel model)
        {
            // --- Đã lược bỏ đoạn check logic GiaBHYT ở đây ---

            if (ModelState.IsValid)
            {
                try
                {
                    if (db.InsertDichVu(model))
                    {
                        TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "Lỗi khi lưu vào cơ sở dữ liệu.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            return View(model);
        }

        // 5. CẬP NHẬT (POST) - ĐÃ BỎ CHECK GIA BHYT
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            var dv = db.GetDichVuById(id);
            if (dv == null) return HttpNotFound();

            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            return View(dv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DichVuViewModel model)
        {
            // --- Đã lược bỏ đoạn check logic GiaBHYT ở đây ---

            if (ModelState.IsValid)
            {
                try
                {
                    if (db.UpdateDichVu(model))
                    {
                        TempData["SuccessMessage"] = "Cập nhật thành công!";
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "Không tìm thấy dịch vụ để cập nhật.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            return View(model);
        }

        // 6. XÓA (JSON)
        [HttpPost]
        public JsonResult Delete(string id)
        {
            string result = db.DeleteDichVu(id);
            return Json(new { success = (result == "OK"), message = result });
        }

        // 7. BẬT/TẮT TRẠNG THÁI
        [HttpPost]
        public JsonResult ToggleStatus(string id, bool status)
        {
            try
            {
                using (var con = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                {
                    string sql = "UPDATE DICHVU SET TrangThai = @Status WHERE MaDV = @MaDV";
                    var cmd = new System.Data.SqlClient.SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Status", status ? 1 : 0);
                    cmd.Parameters.AddWithValue("@MaDV", id);
                    con.Open();
                    return Json(new { success = cmd.ExecuteNonQuery() > 0 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}