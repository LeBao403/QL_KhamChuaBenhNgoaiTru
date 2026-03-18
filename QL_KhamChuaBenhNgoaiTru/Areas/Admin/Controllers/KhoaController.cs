using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class KhoaController : Controller
    {
        // Khởi tạo tầng Database Access
        KhoaDB db = new KhoaDB();

        // ==========================================
        // 1. DANH SÁCH KHOA
        // ==========================================
        public ActionResult Index()
        {
            var listKhoa = db.GetAll();
            return View(listKhoa);
        }

        // ==========================================
        // 2. THÊM MỚI KHOA
        // ==========================================
        public ActionResult Create()
        {
            // Mặc định tạo Khoa mới thì trạng thái là Hoạt động (true)
            var model = new Khoa { TrangThai = true };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Khoa model)
        {
            // 1. Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(model.TenKhoa))
            {
                ModelState.AddModelError("TenKhoa", "Tên khoa không được để trống.");
            }
            // 2. Kiểm tra trùng tên
            else if (db.CheckTenKhoaExists(model.TenKhoa))
            {
                ModelState.AddModelError("TenKhoa", "Tên khoa này đã tồn tại trên hệ thống. Vui lòng chọn tên khác.");
            }

            // Nếu form có lỗi thì đứng lại, hiện chữ đỏ
            if (!ModelState.IsValid) return View(model);

            // 3. Thực thi lưu
            try
            {
                if (db.Create(model))
                {
                    TempData["Success"] = "Đã thêm khoa mới thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Lỗi không xác định khi lưu vào CSDL.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        // ==========================================
        // 3. SỬA THÔNG TIN KHOA
        // ==========================================
        public ActionResult Edit(int id)
        {
            var model = db.GetById(id);
            if (model == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khoa này.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Khoa model)
        {
            // 1. Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(model.TenKhoa))
            {
                ModelState.AddModelError("TenKhoa", "Tên khoa không được để trống.");
            }
            // 2. Kiểm tra trùng tên (Bỏ qua chính nó)
            else if (db.CheckTenKhoaExists(model.TenKhoa, model.MaKhoa))
            {
                ModelState.AddModelError("TenKhoa", "Tên khoa này đã bị trùng với một khoa khác.");
            }

            if (!ModelState.IsValid) return View(model);

            // 3. Thực thi cập nhật
            try
            {
                if (db.Update(model))
                {
                    TempData["Success"] = "Cập nhật thông tin khoa thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Không thể cập nhật vào cơ sở dữ liệu.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        // ==========================================
        // 4. XÓA KHOA (Ràng buộc chặt)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                string errorMsg;

                // GỌI HÀM CHECK RÀNG BUỘC (Nhân viên / Phòng)
                if (!db.CheckCanDelete(id, out errorMsg))
                {
                    // Nếu vướng khóa ngoại -> Gửi thông báo lỗi sang SweetAlert2
                    TempData["Error"] = errorMsg + " Gợi ý: Hãy Khóa (Tạm ngưng) thay vì Xóa vĩnh viễn.";
                    return RedirectToAction("Index");
                }

                // Nếu sạch sẽ -> Tiến hành trảm
                if (db.Delete(id))
                {
                    TempData["Success"] = "Đã xóa khoa thành công khỏi hệ thống!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy khoa để xóa.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống khi xóa: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ==========================================
        // 5. XEM CHI TIẾT KHOA 
        // ==========================================
        public ActionResult Details(int id)
        {
            // Gọi hàm lấy tổng hợp 3 cục data: Khoa, Phòng, Nhân viên
            var model = db.GetKhoaDetails(id);

            if (model == null || model.Khoa == null)
            {
                TempData["Error"] = "Không tìm thấy dữ liệu của Khoa này trên hệ thống.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // ==========================================
        // 6. ĐẢO TRẠNG THÁI (KHÓA / MỞ)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            try
            {
                bool? newStatus = db.ToggleStatus(id);

                if (newStatus.HasValue)
                {
                    if (newStatus.Value == true)
                        TempData["Success"] = "Đã Mở khóa (Kích hoạt) khoa thành công!";
                    else
                        TempData["Success"] = "Đã Tạm ngưng hoạt động của khoa!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu khoa.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Search(string q)
        {
            var list = string.IsNullOrEmpty(q) ? db.GetAll() : db.Search(q);
            return PartialView("_KhoaTable", list);
        }

        // ==========================================
        // 7. ĐIỀU PHỐI (KÉO/RÚT) PHÒNG BAN VÀ NHÂN SỰ
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhong(int maPhong, int maKhoa)
        {
            try
            {
                if (db.AddPhongToKhoa(maPhong, maKhoa))
                    TempData["Success"] = "Đã thêm Phòng vào Khoa thành công!";
                else
                    TempData["Error"] = "Lỗi: Không thể thêm phòng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }
            return RedirectToAction("Details", new { id = maKhoa });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemovePhong(int maPhong, int maKhoa)
        {
            try
            {
                string errorMsg;
                if (db.RemovePhongFromKhoa(maPhong, out errorMsg))
                    TempData["Success"] = "Đã rút Phòng ra khỏi Khoa (trở thành phòng độc lập)!";
                else
                    TempData["Error"] = errorMsg; // Sẽ hiện: "Phòng này đang có nhân viên làm việc..."
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }
            return RedirectToAction("Details", new { id = maKhoa });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNhanVien(string maNV, int maKhoa)
        {
            try
            {
                if (db.AddNhanVienToKhoa(maNV, maKhoa))
                    TempData["Success"] = "Đã phân bổ Nhân viên vào Khoa thành công!";
                else
                    TempData["Error"] = "Lỗi: Không thể phân bổ nhân viên.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }
            return RedirectToAction("Details", new { id = maKhoa });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveNhanVien(string maNV, int maKhoa)
        {
            try
            {
                if (db.RemoveNhanVienFromKhoa(maNV))
                    TempData["Success"] = "Đã rút Nhân viên khỏi Khoa và các phòng trực thuộc!";
                else
                    TempData["Error"] = "Lỗi: Không thể rút nhân viên lúc này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }
            return RedirectToAction("Details", new { id = maKhoa });
        }
    }
}