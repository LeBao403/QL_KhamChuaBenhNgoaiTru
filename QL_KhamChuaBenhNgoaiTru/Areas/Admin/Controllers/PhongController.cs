using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class PhongController : Controller
    {
        PhongDB db = new PhongDB();
        KhoaDB khoaDb = new KhoaDB(); // Dùng để load danh sách Khoa lên Dropdown

        // Hàm hỗ trợ tạo danh sách Loại Phòng (LẤY ĐỘNG TỪ DATABASE)
        private SelectList GetLoaiPhongList(int? selectedValue = null)
        {
            var danhMuc = db.GetAllLoaiPhong();
            return new SelectList(danhMuc, "MaLoaiPhong", "TenLoaiPhong", selectedValue);
        }

        // ==========================================
        // 1. DANH SÁCH & TÌM KIẾM
        // ==========================================
        // Đổi tham số từ string loaiPhong sang int? maLoaiPhong
        public ActionResult Index(string keyword = "", int? maLoaiPhong = null, int maKhoa = 0)
        {
            // Load danh sách Khoa và Loại phòng để làm bộ lọc Dropdown
            ViewBag.MaKhoa = new SelectList(khoaDb.GetAll(), "MaKhoa", "TenKhoa", maKhoa);
            ViewBag.MaLoaiPhong = GetLoaiPhongList(maLoaiPhong);

            var list = db.Search(keyword, maLoaiPhong, maKhoa);

            // Nếu gọi bằng AJAX (Live search) thì chỉ trả về phần Table
            if (Request.IsAjaxRequest())
            {
                return PartialView("_PhongTable", list);
            }

            return View(list);
        }

        // ==========================================
        // 2. THÊM MỚI PHÒNG
        // ==========================================
        public ActionResult Create()
        {
            // Đổi tên ViewBag để tránh xung đột với Model
            ViewBag.DSKhoa = new SelectList(khoaDb.GetAll(), "MaKhoa", "TenKhoa");
            ViewBag.DSLoaiPhong = GetLoaiPhongList();
            return View(new Phong { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Phong model)
        {
            if (string.IsNullOrWhiteSpace(model.TenPhong))
                ModelState.AddModelError("TenPhong", "Tên phòng không được để trống.");
            else if (db.CheckTenPhongExists(model.TenPhong))
                ModelState.AddModelError("TenPhong", "Tên phòng này đã tồn tại.");

            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Create(model))
                    {
                        TempData["Success"] = "Đã thêm phòng mới thành công!";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            ViewBag.DSKhoa = new SelectList(khoaDb.GetAll(), "MaKhoa", "TenKhoa", model.MaKhoa);
            ViewBag.DSLoaiPhong = GetLoaiPhongList(model.MaLoaiPhong);
            return View(model);
        }

        // ==========================================
        // 3. SỬA THÔNG TIN PHÒNG
        // ==========================================
        public ActionResult Edit(int id)
        {
            var model = db.GetById(id);
            if (model == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phòng.";
                return RedirectToAction("Index");
            }

            // Đổi tên ViewBag, truyền giá trị cũ (model.MaKhoa, model.MaLoaiPhong) vào để nó tự Select
            ViewBag.DSKhoa = new SelectList(khoaDb.GetAll(), "MaKhoa", "TenKhoa", model.MaKhoa);
            ViewBag.DSLoaiPhong = GetLoaiPhongList(model.MaLoaiPhong);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Phong model)
        {
            if (string.IsNullOrWhiteSpace(model.TenPhong))
                ModelState.AddModelError("TenPhong", "Tên phòng không được để trống.");
            else if (db.CheckTenPhongExists(model.TenPhong, model.MaPhong))
                ModelState.AddModelError("TenPhong", "Tên phòng này đã bị trùng với phòng khác.");

            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Update(model))
                    {
                        TempData["Success"] = "Cập nhật thông tin phòng thành công!";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                }
            }

            ViewBag.DSKhoa = new SelectList(khoaDb.GetAll(), "MaKhoa", "TenKhoa", model.MaKhoa);
            ViewBag.DSLoaiPhong = GetLoaiPhongList(model.MaLoaiPhong);
            return View(model);
        }

        // ==========================================
        // 4. XÓA & ĐẢO TRẠNG THÁI
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                string errorMsg;
                if (!db.CheckCanDelete(id, out errorMsg))
                {
                    TempData["Error"] = errorMsg + " Hãy sử dụng chức năng Đóng cửa thay vì Xóa.";
                    return RedirectToAction("Index");
                }

                if (db.Delete(id))
                    TempData["Success"] = "Đã xóa vĩnh viễn phòng khỏi hệ thống!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            try
            {
                bool? newStatus = db.ToggleStatus(id);
                if (newStatus.HasValue)
                {
                    TempData["Success"] = newStatus.Value ? "Đã Mở cửa phòng hoạt động lại." : "Đã Đóng cửa (Tạm ngưng) phòng.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // 5. CHI TIẾT PHÒNG & QUẢN LÝ NHÂN SỰ
        // ==========================================
        public ActionResult Details(int id)
        {
            var phong = db.GetById(id);
            if (phong == null)
            {
                TempData["Error"] = "Không tìm thấy phòng này.";
                return RedirectToAction("Index");
            }

            // Gọi hàm từ Database lấy thông tin
            var listNVTrongPhong = db.GetNhanVienByRoom(id);
            var listNVKhaDung = db.GetAvailableNhanVienForRoom(phong.MaKhoa);

            // Tìm Tên khoa để hiển thị (Nếu phòng độc lập thì gán chuỗi trống)
            string tenKhoa = "Độc lập (Không thuộc khoa)";
            if (phong.MaKhoa.HasValue)
            {
                var khoaInfo = khoaDb.GetById(phong.MaKhoa.Value);
                if (khoaInfo != null) tenKhoa = khoaInfo.TenKhoa;
            }

            // Tìm Tên loại phòng để hiển thị
            string tenLoaiPhong = "Chưa xác định";
            if (phong.MaLoaiPhong.HasValue)
            {
                var danhMucLoaiPhong = db.GetAllLoaiPhong();
                var loaiPhongObj = danhMucLoaiPhong.FirstOrDefault(x => x.MaLoaiPhong == phong.MaLoaiPhong.Value);
                if (loaiPhongObj != null) tenLoaiPhong = loaiPhongObj.TenLoaiPhong;
            }

            // Đóng gói vào ViewModel
            var model = new PhongDetailsViewModel
            {
                Phong = new PhongManageViewModel
                {
                    MaPhong = phong.MaPhong,
                    TenPhong = phong.TenPhong,
                    MaLoaiPhong = phong.MaLoaiPhong,
                    TenLoaiPhong = tenLoaiPhong,
                    TrangThai = phong.TrangThai,
                    MaKhoa = phong.MaKhoa,
                    TenKhoa = tenKhoa
                },
                DanhSachNhanVien = listNVTrongPhong,
                DanhSachNhanVienKhaDung = listNVKhaDung
            };

            return View(model);
        }

        // Gọi qua AJAX/Form POST khi bấm "Thêm vào phòng"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNhanVien(string maNV, int maPhong)
        {
            try
            {
                if (db.AddNhanVienToRoom(maNV, maPhong))
                    TempData["Success"] = "Đã phân bổ nhân viên vào phòng thành công!";
                else
                    TempData["Error"] = "Không thể thêm nhân viên lúc này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Details", new { id = maPhong });
        }

        // Gọi qua Form POST khi bấm "Rút khỏi phòng"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveNhanVien(string maNV, int maPhong)
        {
            try
            {
                if (db.RemoveNhanVienFromRoom(maNV))
                    TempData["Success"] = "Đã rút nhân viên ra khỏi phòng!";
                else
                    TempData["Error"] = "Không thể rút nhân viên lúc này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Details", new { id = maPhong });
        }
    }
}