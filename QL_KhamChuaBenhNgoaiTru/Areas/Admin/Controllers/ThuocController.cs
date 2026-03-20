using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class ThuocController : BaseAdminController
    {
        // Giả định bạn đã tạo ThuocDB
        private readonly ThuocDB db = new ThuocDB();

        // --- DANH SÁCH THUỐC ---
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10;
            try
            {
                var dsThuoc = db.GetAllThuoc(page, pageSize);
                int totalCount = db.GetCountThuoc();

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(dsThuoc);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View("Error");
            }
        }

        public ActionResult Search(string q)
        {
            var list = string.IsNullOrEmpty(q) ? db.GetAllThuoc(1, 100) : db.SearchThuoc(q);
            return PartialView("_ThuocTable", list);
        }

        // --- XEM CHI TIẾT ---
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(400);

            var thuoc = db.GetThuocById(id);
            if (thuoc == null) return HttpNotFound();

            var model = new ThuocManageViewModel
            {
                Thuoc = thuoc,
                TenLoaiThuoc = db.GetTenLoaiThuoc(thuoc.MaLoaiThuoc),
                TenNSX = thuoc.MaNSX.HasValue ? db.GetTenNSX(thuoc.MaNSX.Value) : "N/A",
                ChiTietThanhPhan = db.GetThanhPhanThuocDisplay(id) // Lấy list hoạt chất & hàm lượng
            };

            return View(model);
        }

        // --- THÊM THUỐC ---
        public ActionResult Create()
        {
            var model = new ThuocManageViewModel
            {
                Thuoc = new Thuoc { TrangThai = true },
                DanhSachThanhPhan = new List<ThanhPhanThuoc>(),
                DanhSachLoaiThuoc = new SelectList(db.GetAllLoaiThuoc(), "MaDanhMuc", "TenDanhMuc"),
                DanhSachNSX = new SelectList(db.GetAllNSX(), "MaNSX", "TenNSX"),
                DanhSachHoatChat = new SelectList(db.GetAllHoatChat(), "MaHoatChat", "TenHoatChat")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThuocManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Hàm CreateThuoc này trong ThuocDB cần INSERT bảng THUỐC 
                    // và lặp mảng model.DanhSachThanhPhan để INSERT vào THANHPHAN_THUOC
                    bool result = db.CreateThuoc(model.Thuoc, model.DanhSachThanhPhan);
                    if (result)
                    {
                        TempData["Success"] = "Thêm thuốc thành công!";
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("", "Không thể lưu vào CSDL.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            // Load lại dropdown nếu lỗi
            model.DanhSachLoaiThuoc = new SelectList(db.GetAllLoaiThuoc(), "MaDanhMuc", "TenDanhMuc");
            model.DanhSachNSX = new SelectList(db.GetAllNSX(), "MaNSX", "TenNSX");
            model.DanhSachHoatChat = new SelectList(db.GetAllHoatChat(), "MaHoatChat", "TenHoatChat");
            return View(model);
        }

        // --- SỬA THUỐC ---
        public ActionResult Edit(string id)
        {
            var thuoc = db.GetThuocById(id);
            if (thuoc == null) return HttpNotFound();

            var model = new ThuocManageViewModel
            {
                Thuoc = thuoc,
                DanhSachThanhPhan = db.GetThanhPhanThuocRaw(id), // Lấy list ThanhPhanThuoc thuần
                DanhSachLoaiThuoc = new SelectList(db.GetAllLoaiThuoc(), "MaDanhMuc", "TenDanhMuc"),
                DanhSachNSX = new SelectList(db.GetAllNSX(), "MaNSX", "TenNSX"),
                DanhSachHoatChat = new SelectList(db.GetAllHoatChat(), "MaHoatChat", "TenHoatChat")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ThuocManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Hàm UpdateThuoc: UPDATE bảng THUỐC, 
                    // XÓA các ThanhPhanThuoc cũ của thuốc này, và INSERT lại model.DanhSachThanhPhan
                    bool result = db.UpdateThuoc(model.Thuoc, model.DanhSachThanhPhan);
                    if (result)
                    {
                        TempData["Success"] = "Cập nhật thành công!";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            // Load lại dropdown nếu lỗi
            model.DanhSachLoaiThuoc = new SelectList(db.GetAllLoaiThuoc(), "MaDanhMuc", "TenDanhMuc");
            model.DanhSachNSX = new SelectList(db.GetAllNSX(), "MaNSX", "TenNSX");
            model.DanhSachHoatChat = new SelectList(db.GetAllHoatChat(), "MaHoatChat", "TenHoatChat");
            return View(model);
        }

        // --- XÓA THUỐC ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                // Nhớ xóa THANHPHAN_THUOC trước khi xóa THUỐC trong DB
                bool result = db.DeleteThuoc(id);
                if (result) TempData["Success"] = "Đã xóa thuốc thành công.";
                else TempData["Error"] = "Không thể xóa (có thể thuốc đã được kê đơn).";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}