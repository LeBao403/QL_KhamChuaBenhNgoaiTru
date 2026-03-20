using QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class DichVuController : Controller
    {
        private DichVuDB db = new DichVuDB();

        // ==========================================================
        // 1. TRANG CHỦ & BỘ LỌC TỔNG HỢP
        // ==========================================================
        public ActionResult Index(int page = 1, string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null, string sortPrice = "")
        {
            int pageSize = 10;
            try
            {
                // Gọi DB lấy data có phân trang
                var dsdv = db.GetDanhSachDichVu(page, pageSize, keyword, maLoai, minPrice, maxPrice, sortPrice);
                int totalCount = db.GetTotalRecord(keyword, maLoai, minPrice, maxPrice);

                // Gửi dữ liệu xuống View cho Paging
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                // Giữ lại trạng thái Form Lọc
                ViewBag.Keyword = keyword;
                ViewBag.MaLoai = maLoai;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SortPrice = sortPrice;

                // Load Combobox Danh mục
                ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();

                return View(dsdv);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách: " + ex.Message;
                return View("Error");
            }
        }

        // ==========================================================
        // 2. AJAX LIVE SEARCH (Trả về Partial View)
        // ==========================================================
        public ActionResult Search(string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null, string sortPrice = "")
        {
            // Tìm kiếm nhanh thì luôn ép về trang 1
            var dsdv = db.GetDanhSachDichVu(1, 10, keyword, maLoai, minPrice, maxPrice, sortPrice);
            return PartialView("_DichVuTable", dsdv);
        }

        // ==========================================================
        // 3. XEM CHI TIẾT
        // ==========================================================
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var dv = db.GetDichVuById(id);
            if (dv == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ!";
                return RedirectToAction("Index");
            }

            return View(dv);
        }

        // ==========================================================
        // 4. THÊM MỚI (GET & POST)
        // ==========================================================
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();

            var model = new DichVuViewModel();
            model.MaDV = db.GenerateNextMaDV();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Chống giả mạo Request
        public ActionResult Create(DichVuViewModel model)
        {
            if (model.CoBHYT)
            {
                if (!model.GiaBHYT.HasValue || model.GiaBHYT <= 0)
                {
                    ModelState.AddModelError("GiaBHYT", "Vui lòng nhập giá BHYT chi trả lớn hơn 0!");
                }
                else if (model.GiaBHYT > model.GiaDichVu)
                {
                    ModelState.AddModelError("GiaBHYT", "Vô lý! Mức giá BHYT chi trả không được lớn hơn Giá dịch vụ gốc.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool isSuccess = db.InsertDichVu(model);
                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi lưu vào cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Nếu lỗi, phải load lại Combobox trước khi trả về View
            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            return View(model);
        }

        // ==========================================================
        // 5. CẬP NHẬT (GET & POST)
        // ==========================================================
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var dv = db.GetDichVuById(id);
            if (dv == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy dịch vụ để sửa!";
                return RedirectToAction("Index");
            }

            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            return View(dv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DichVuViewModel model)
        {
            // === CHẶN LỖI LOGIC: GIÁ BHYT KHÔNG ĐƯỢC VƯỢT QUÁ GIÁ DỊCH VỤ ===
            if (model.CoBHYT)
            {
                if (!model.GiaBHYT.HasValue || model.GiaBHYT <= 0)
                {
                    ModelState.AddModelError("GiaBHYT", "Vui lòng nhập giá BHYT chi trả lớn hơn 0!");
                }
                else if (model.GiaBHYT > model.GiaDichVu)
                {
                    ModelState.AddModelError("GiaBHYT", "Vô lý! Mức giá BHYT chi trả không được lớn hơn Giá dịch vụ gốc.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool isSuccess = db.UpdateDichVu(model);
                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Cập nhật dịch vụ thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi cập nhật cơ sở dữ liệu. Không tìm thấy dịch vụ.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Nếu có lỗi (do ModelState không hợp lệ), phải load lại Combobox trước khi trả về View
            ViewBag.ListLoaiDV = db.GetAllLoaiDichVu();
            return View(model);
        }

        // ==========================================================
        // 6. XÓA MỀM (POST)
        // ==========================================================
        [HttpPost]
        public JsonResult Delete(string id)
        {
            // Gọi hàm xóa và lấy kết quả trả về
            string result = db.DeleteDichVu(id);

            if (result == "OK")
            {
                return Json(new { success = true, message = "Đã xóa vĩnh viễn dịch vụ thành công!" });
            }
            else
            {
                // Trả về success = false để phía Giao diện hiện thông báo cảnh báo (SweetAlert)
                return Json(new { success = false, message = result });
            }
        }

        [HttpPost]
        public JsonResult ToggleStatus(string id, bool status)
        {
            try
            {
                // Gọi câu SQL Update nhanh trạng thái
                using (var con = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                {
                    string sql = "UPDATE DICHVU SET TrangThai = @Status WHERE MaDV = @MaDV";
                    var cmd = new System.Data.SqlClient.SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@Status", status ? 1 : 0);
                    cmd.Parameters.AddWithValue("@MaDV", id);
                    con.Open();
                    int row = cmd.ExecuteNonQuery();

                    if (row > 0) return Json(new { success = true });
                    else return Json(new { success = false, message = "Không tìm thấy dịch vụ!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}