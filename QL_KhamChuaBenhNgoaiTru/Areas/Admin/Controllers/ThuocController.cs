using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class ThuocController : BaseAdminController
    {
        private readonly ThuocDB db = new ThuocDB();

        // ===================== DANH SÁCH THUỐC =====================
        public ActionResult Index(int page = 1,
            string keyword = "",
            string maLoaiThuoc = "",
            string duongDung = "",
            bool? coBHYT = null,
            bool? trangThai = null)
        {
            int pageSize = 10;

            try
            {
                ViewBag.DsLoaiThuoc = db.GetAllLoaiThuoc();
                ViewBag.DsDuongDung = db.GetAllDuongDung();

                var dsThuoc = db.GetAll(page, pageSize, keyword, maLoaiThuoc, duongDung, coBHYT, trangThai);
                int totalCount = db.GetCount(keyword, maLoaiThuoc, duongDung, coBHYT, trangThai);

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.Keyword = keyword;
                ViewBag.MaLoaiThuoc = maLoaiThuoc;
                ViewBag.DuongDung = duongDung;
                ViewBag.CoBHYT = coBHYT;
                ViewBag.TrangThai = trangThai;

                return View(dsThuoc);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách thuốc: " + ex.Message;
                return View("Error");
            }
        }

        // AJAX: Search + filter (partial table)
        public ActionResult Search(string q = "", string maLoaiThuoc = "", string duongDung = "", bool? coBHYT = null, bool? trangThai = null)
        {
            var list = db.GetAll(1, 9999, q, maLoaiThuoc, duongDung, coBHYT, trangThai);
            return PartialView("_ThuocTable", list);
        }

        // ===================== CHI TIẾT THUỐC =====================
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã thuốc không hợp lệ");

            var thuoc = db.GetByIdWithThanhPhan(id);
            if (thuoc == null)
                return HttpNotFound("Không tìm thấy thuốc!");

            return View(thuoc);
        }

        // ===================== THÊM THUỐC =====================
        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new ThuocManageViewModel2
            {
                Thuoc = new Thuoc(),
                ThanhPhans = new List<ThanhPhanThuoc>()
            };

            model.Thuoc.MaThuoc = db.GenerateNextMaThuoc();
            model.Thuoc.TrangThai = true;

            ViewBag.DsLoaiThuoc = db.GetAllLoaiThuoc();
            ViewBag.DsDuongDung = db.GetAllDuongDung();
            ViewBag.DsNSX = db.GetAllNSX();
            ViewBag.DsHoatChat = db.GetAllHoatChat();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThuocManageViewModel2 model)
        {
            ViewBag.DsLoaiThuoc = db.GetAllLoaiThuoc();
            ViewBag.DsDuongDung = db.GetAllDuongDung();
            ViewBag.DsNSX = db.GetAllNSX();
            ViewBag.DsHoatChat = db.GetAllHoatChat();

            // Validate tên thuốc trùng
            if (!string.IsNullOrWhiteSpace(model.Thuoc.TenThuoc) && db.TenThuocExists(model.Thuoc.TenThuoc))
                ModelState.AddModelError("Thuoc.TenThuoc", "Tên thuốc này đã tồn tại trong hệ thống.");

            // Validate bắt buộc - thông tin cơ bản
            if (string.IsNullOrWhiteSpace(model.Thuoc.TenThuoc))
                ModelState.AddModelError("Thuoc.TenThuoc", "Tên thuốc là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.Thuoc.DonViCoBan))
                ModelState.AddModelError("Thuoc.DonViCoBan", "Đơn vị cơ bản là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.Thuoc.MaLoaiThuoc))
                ModelState.AddModelError("Thuoc.MaLoaiThuoc", "Loại thuốc là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.Thuoc.DuongDung))
                ModelState.AddModelError("Thuoc.DuongDung", "Đường dùng là bắt buộc.");

            if (!model.Thuoc.GiaBan.HasValue || model.Thuoc.GiaBan <= 0)
                ModelState.AddModelError("Thuoc.GiaBan", "Giá bán phải lớn hơn 0.");

            if (!model.Thuoc.MaNSX.HasValue || model.Thuoc.MaNSX <= 0)
                ModelState.AddModelError("Thuoc.MaNSX", "Nhà sản xuất là bắt buộc.");

            // Validate hoạt chất không trùng lặp
            var thanhPhansTemp = ThuocHelpers.ParseThanhPhansFromForm(Request.Form);
            if (thanhPhansTemp.Count == 0)
            {
                ModelState.AddModelError("", "Phải có ít nhất 1 hoạt chất.");
            }
            else
            {
                var hoatChatCodes = thanhPhansTemp
                    .Where(tp => !string.IsNullOrWhiteSpace(tp.MaHoatChat))
                    .Select(tp => tp.MaHoatChat.Trim())
                    .ToList();
                if (hoatChatCodes.Count != hoatChatCodes.Distinct().Count())
                {
                    ModelState.AddModelError("", "Danh sách hoạt chất có giá trị trùng lặp. Vui lòng chọn các hoạt chất khác nhau.");
                }
            }

            if (!ModelState.IsValid) return View(model);

            try
            {
                // Thu thập thành phần từ form
                var thanhPhans = ThuocHelpers.ParseThanhPhansFromForm(Request.Form);

                bool result = db.Create(model.Thuoc, thanhPhans);
                if (result)
                {
                    TempData["Success"] = "Thêm thuốc mới thành công!";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Lỗi không xác định khi lưu vào CSDL.");
            }
            catch (SqlException sqlex) when (sqlex.Number == 2627)
            {
                // Primary key violation - mã trùng, tạo mã mới
                model.Thuoc.MaThuoc = db.GenerateNextMaThuoc();
                ModelState.Remove("Thuoc.MaThuoc");
                ModelState.AddModelError("Thuoc.MaThuoc", "Mã thuốc bị trùng. Mã mới: " + model.Thuoc.MaThuoc + ". Vui lòng thử lại.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        // ===================== SỬA THUỐC =====================
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã thuốc không hợp lệ");

            var thuoc = db.GetById(id);
            if (thuoc == null)
                return HttpNotFound("Không tìm thấy thuốc!");

            var thanhPhans = db.GetThanhPhanByMaThuoc(id);
            var thanhPhanEntities = new List<ThanhPhanThuoc>();
            foreach (var tp in thanhPhans)
            {
                thanhPhanEntities.Add(new ThanhPhanThuoc
                {
                    MaThuoc = tp.MaThuoc,
                    MaHoatChat = tp.MaHoatChat,
                    HamLuong = tp.HamLuong
                });
            }

            var model = new ThuocManageViewModel2
            {
                Thuoc = thuoc,
                ThanhPhans = thanhPhanEntities
            };

            ViewBag.DsLoaiThuoc = db.GetAllLoaiThuoc();
            ViewBag.DsDuongDung = db.GetAllDuongDung();
            ViewBag.DsNSX = db.GetAllNSX();
            ViewBag.DsHoatChat = db.GetAllHoatChat();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ThuocManageViewModel2 model)
        {
            ViewBag.DsLoaiThuoc = db.GetAllLoaiThuoc();
            ViewBag.DsDuongDung = db.GetAllDuongDung();
            ViewBag.DsNSX = db.GetAllNSX();
            ViewBag.DsHoatChat = db.GetAllHoatChat();

            // Validate tên thuốc trùng (bỏ qua chính nó)
            if (!string.IsNullOrWhiteSpace(model.Thuoc.TenThuoc) && db.TenThuocExists(model.Thuoc.TenThuoc, model.Thuoc.MaThuoc))
                ModelState.AddModelError("Thuoc.TenThuoc", "Tên thuốc này đã tồn tại trong hệ thống.");

            // Validate bắt buộc - thông tin cơ bản
            if (string.IsNullOrWhiteSpace(model.Thuoc.TenThuoc))
                ModelState.AddModelError("Thuoc.TenThuoc", "Tên thuốc là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.Thuoc.DonViCoBan))
                ModelState.AddModelError("Thuoc.DonViCoBan", "Đơn vị cơ bản là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.Thuoc.MaLoaiThuoc))
                ModelState.AddModelError("Thuoc.MaLoaiThuoc", "Loại thuốc là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.Thuoc.DuongDung))
                ModelState.AddModelError("Thuoc.DuongDung", "Đường dùng là bắt buộc.");

            if (!model.Thuoc.GiaBan.HasValue || model.Thuoc.GiaBan <= 0)
                ModelState.AddModelError("Thuoc.GiaBan", "Giá bán phải lớn hơn 0.");

            if (!model.Thuoc.MaNSX.HasValue || model.Thuoc.MaNSX <= 0)
                ModelState.AddModelError("Thuoc.MaNSX", "Nhà sản xuất là bắt buộc.");

            // Validate hoạt chất không trùng lặp
            var thanhPhansTemp = ThuocHelpers.ParseThanhPhansFromForm(Request.Form);
            if (thanhPhansTemp.Count == 0)
            {
                ModelState.AddModelError("", "Phải có ít nhất 1 hoạt chất.");
            }
            else
            {
                var hoatChatCodes = thanhPhansTemp
                    .Where(tp => !string.IsNullOrWhiteSpace(tp.MaHoatChat))
                    .Select(tp => tp.MaHoatChat.Trim())
                    .ToList();
                if (hoatChatCodes.Count != hoatChatCodes.Distinct().Count())
                {
                    ModelState.AddModelError("", "Danh sách hoạt chất có giá trị trùng lặp. Vui lòng chọn các hoạt chất khác nhau.");
                }
            }

            if (!ModelState.IsValid) return View(model);

            try
            {
                var thanhPhans = ThuocHelpers.ParseThanhPhansFromForm(Request.Form);

                bool result = db.Update(model.Thuoc, thanhPhans);
                if (result)
                {
                    TempData["Success"] = "Cập nhật thuốc thành công!";
                    return RedirectToAction("Details", new { id = model.Thuoc.MaThuoc.Trim() });
                }

                ModelState.AddModelError("", "Không thể cập nhật thông tin.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        // ===================== XÓA THUỐC =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã thuốc không hợp lệ.";
                    return RedirectToAction("Index");
                }

                bool result = db.Delete(id);
                if (result)
                    TempData["Success"] = "Đã xóa thuốc thành công.";
                else
                    TempData["Error"] = "Không tìm thấy thuốc để xóa.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi xóa thuốc: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ===================== Toggle Trạng thái =====================
        [HttpPost]
        public ActionResult ToggleStatus(string id, bool status)
        {
            try
            {
                bool? newStatus = db.ToggleTrangThai(id);
                return Json(new { success = true, message = "OK", newStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // ===================== HELPER CLASS =====================
    public static class ThuocHelpers
    {
        public static List<ThanhPhanThuoc> ParseThanhPhansFromForm(System.Collections.Specialized.NameValueCollection form)
        {
            var list = new List<ThanhPhanThuoc>();
            var keys = form.AllKeys.Where(k => k != null && k.StartsWith("MaHoatChat_")).OrderBy(k => k).ToList();

            foreach (var key in keys)
            {
                var idx = key.Replace("MaHoatChat_", "");
                var maHoatChat = form[key];
                var hamLuong = form["HamLuong_" + idx];

                if (!string.IsNullOrWhiteSpace(maHoatChat))
                {
                    list.Add(new ThanhPhanThuoc
                    {
                        MaHoatChat = maHoatChat.Trim(),
                        HamLuong = hamLuong?.Trim()
                    });
                }
            }
            return list;
        }
    }
}
