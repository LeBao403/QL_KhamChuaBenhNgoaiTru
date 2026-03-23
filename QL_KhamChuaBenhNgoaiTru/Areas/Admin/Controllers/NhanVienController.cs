using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class NhanVienController : BaseAdminController
    {
        private readonly NhanVienDB db = new NhanVienDB();

        // ===================== DANH SÁCH NHÂN VIÊN =====================
        public ActionResult Index(int page = 1,
            string keyword = "",
            string gioiTinh = "",
            int? maChucVu = 0,
            bool? trangThai = null)
        {
            int pageSize = 10;

            try
            {
                // Load dropdown data
                ViewBag.DsChucVu = db.GetAllChucVu();
                ViewBag.DsKhoa = db.GetAllKhoa();

                // Load danh sách với filter + phân trang
                var dsNv = db.GetAll(page, pageSize, keyword, gioiTinh, maChucVu, trangThai);
                int totalCount = db.GetCount(keyword, gioiTinh, maChucVu, trangThai);

                // Đẩy filter ra ViewBag để giữ giá trị trên form
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.Keyword = keyword;
                ViewBag.GioiTinh = gioiTinh;
                ViewBag.MaChucVu = maChucVu;
                ViewBag.TrangThai = trangThai;

                return View(dsNv);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách nhân viên: " + ex.Message;
                return View("Error");
            }
        }

        // AJAX: Tìm kiếm + filter (trả về partial table)
        public ActionResult Search(string q, string gioiTinh = "", int? maChucVu = 0, bool? trangThai = null)
        {
            var list = db.GetAll(1, 9999, q, gioiTinh, maChucVu, trangThai);
            return PartialView("_NhanVienTable", list);
        }

        // ===================== CHI TIẾT NHÂN VIÊN =====================
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã nhân viên không hợp lệ");

            var nvDetails = db.GetNhanVienDetailsById(id);
            if (nvDetails == null)
                return HttpNotFound("Không tìm thấy nhân viên!");

            var currentNv = db.GetById(id);
            TaiKhoan tk = null;
            if (currentNv != null && currentNv.MaTK.HasValue && currentNv.MaTK.Value > 0)
            {
                tk = db.GetTaiKhoanByMaTK(currentNv.MaTK);
            }

            var model = new NhanVienManageViewModel2
            {
                NhanVien = currentNv ?? new NhanVien { MaNV = id },
                TaiKhoan = tk ?? new TaiKhoan(),
                TenKhoa = nvDetails.TenKhoa,
                TenChucVu = nvDetails.TenChucVu
            };

            return View(model);
        }

        // ===================== THÊM NHÂN VIÊN =====================
        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new NhanVienManageViewModel2
            {
                NhanVien = new NhanVien(),
                TaiKhoan = new TaiKhoan()
            };

            model.NhanVien.MaNV = db.GenerateNextMaNV();
            model.NhanVien.NgaySinh = DateTime.Now;
            model.NhanVien.TrangThai = true;

            // Load dropdown
            ViewBag.DsChucVu = db.GetAllChucVu();
            ViewBag.DsKhoa = db.GetAllKhoa();
            ViewBag.DsPhong = db.GetAllPhong();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhanVienManageViewModel2 model, HttpPostedFileBase HinhAnh_File)
        {
            // Load dropdown
            ViewBag.DsChucVu = db.GetAllChucVu();
            ViewBag.DsKhoa = db.GetAllKhoa();
            ViewBag.DsPhong = db.GetAllPhong();

            // --- 0. XU LY ANH DAI DIEN ---
            if (HinhAnh_File != null && HinhAnh_File.ContentLength > 0)
            {
                string uploadsDir = Server.MapPath("~/Images/doctors/");
                if (!System.IO.Directory.Exists(uploadsDir))
                    System.IO.Directory.CreateDirectory(uploadsDir);

                string ext = System.IO.Path.GetExtension(HinhAnh_File.FileName);
                string fileName = model.NhanVien.MaNV + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string fullPath = System.IO.Path.Combine(uploadsDir, fileName);

                HinhAnh_File.SaveAs(fullPath);
                model.NhanVien.HinhAnh = "/Images/doctors/" + fileName;
            }

            // --- 1. KIEM TRA TRUNG LAP ---

            // Check Email
            if (!string.IsNullOrWhiteSpace(model.NhanVien.Email) && db.EmailExists(model.NhanVien.Email))
                ModelState.AddModelError("NhanVien.Email", "Email này đã được sử dụng bởi nhân viên khác.");

            // Check SĐT
            if (!string.IsNullOrWhiteSpace(model.NhanVien.SDT) && db.SDTExists(model.NhanVien.SDT))
                ModelState.AddModelError("NhanVien.SDT", "Số điện thoại này đã được sử dụng.");

            // --- 2. KIỂM TRA TÀI KHOẢN BẮT BUỘC ---
            if (string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
            {
                ModelState.AddModelError("TaiKhoan.PasswordHash", "Mật khẩu là bắt buộc khi tạo nhân viên.");
            }
            else if (model.TaiKhoan.PasswordHash.Length < 6)
            {
                ModelState.AddModelError("TaiKhoan.PasswordHash", "Mật khẩu phải có ít nhất 6 ký tự.");
            }

            string usernameToUse = !string.IsNullOrWhiteSpace(model.TaiKhoan.Username)
                                   ? model.TaiKhoan.Username
                                   : model.NhanVien.SDT;

            if (string.IsNullOrWhiteSpace(usernameToUse))
            {
                ModelState.AddModelError("", "Cần có Số điện thoại hoặc Username để tạo tài khoản.");
            }
            else if (db.UsernameExists(usernameToUse))
            {
                ModelState.AddModelError("TaiKhoan.Username", "Tên đăng nhập '" + usernameToUse + "' đã có người sử dụng.");
            }

            if (!ModelState.IsValid) return View(model);

            // --- 3. TIẾN HÀNH LƯU DỮ LIỆU ---
            TaiKhoan tk = new TaiKhoan
            {
                Username = usernameToUse,
                PasswordHash = model.TaiKhoan.PasswordHash,
                IsActive = true
            };

            try
            {
                bool result = db.Create(model.NhanVien, tk);
                if (result)
                {
                    TempData["Success"] = "Thêm nhân viên mới thành công!";
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

        // ===================== SỬA NHÂN VIÊN =====================
        public ActionResult Edit_NV(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã nhân viên không hợp lệ");

            var nv = db.GetById(id);
            if (nv == null)
                return HttpNotFound("Không tìm thấy nhân viên!");

            TaiKhoan tk = null;
            if (nv.MaTK.HasValue && nv.MaTK.Value > 0)
            {
                tk = db.GetTaiKhoanByMaTK(nv.MaTK);
            }

            if (tk == null)
            {
                tk = new TaiKhoan { IsActive = true };
                tk.Username = nv.SDT;
            }

            var model = new NhanVienManageViewModel2
            {
                NhanVien = nv,
                TaiKhoan = tk
            };

            // Tạo SelectList cho Chức vụ
            var dsChucVu = db.GetAllChucVu();
            var chucVuList = new SelectList(
                dsChucVu?.Select(cv => new SelectListItem
                {
                    Value = cv.MaChucVu.ToString(),
                    Text = cv.TenChucVu
                }) ?? Enumerable.Empty<SelectListItem>(),
                "Value", "Text",
                nv.MaChucVu.HasValue ? nv.MaChucVu.Value.ToString() : null
            );
            ViewBag.DsChucVu = chucVuList;
            ViewBag.DsKhoa = db.GetAllKhoa();
            ViewBag.DsPhong = db.GetAllPhong();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_NV(NhanVienManageViewModel2 model, HttpPostedFileBase HinhAnh_File)
        {
            // Tạo SelectList cho Chức vụ (để hiển thị lại form nếu có lỗi)
            var dsChucVu = db.GetAllChucVu();
            var chucVuList = new SelectList(
                dsChucVu?.Select(cv => new SelectListItem
                {
                    Value = cv.MaChucVu.ToString(),
                    Text = cv.TenChucVu
                }) ?? Enumerable.Empty<SelectListItem>(),
                "Value", "Text",
                model.NhanVien.MaChucVu.HasValue ? model.NhanVien.MaChucVu.Value.ToString() : null
            );
            ViewBag.DsChucVu = chucVuList;
            ViewBag.DsKhoa = db.GetAllKhoa();
            ViewBag.DsPhong = db.GetAllPhong();

            // 0. XU LY ANH DAI DIEN
            if (HinhAnh_File != null && HinhAnh_File.ContentLength > 0)
            {
                string uploadsDir = Server.MapPath("~/Images/doctors/");
                if (!System.IO.Directory.Exists(uploadsDir))
                    System.IO.Directory.CreateDirectory(uploadsDir);

                string ext = System.IO.Path.GetExtension(HinhAnh_File.FileName);
                string fileName = model.NhanVien.MaNV + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string fullPath = System.IO.Path.Combine(uploadsDir, fileName);

                HinhAnh_File.SaveAs(fullPath);
                model.NhanVien.HinhAnh = "/Images/doctors/" + fileName;
            }
            else
            {
                model.NhanVien.HinhAnh = model.NhanVien.HinhAnh;
            }

            // 1. Lay du lieu hien tai tu DB
            var currentNv = db.GetById(model.NhanVien.MaNV);
            if (currentNv == null)
                return HttpNotFound("Không tìm thấy nhân viên!");

            // 2. KIỂM TRA TRÙNG LẶP (chỉ check nếu có sự thay đổi)
            if (!string.IsNullOrWhiteSpace(model.NhanVien.Email) &&
                model.NhanVien.Email != currentNv.Email &&
                db.EmailExists(model.NhanVien.Email, model.NhanVien.MaNV))
            {
                ModelState.AddModelError("NhanVien.Email", "Email này đã được sử dụng bởi nhân viên khác.");
            }

            if (!string.IsNullOrWhiteSpace(model.NhanVien.SDT) &&
                model.NhanVien.SDT != currentNv.SDT &&
                db.SDTExists(model.NhanVien.SDT, model.NhanVien.MaNV))
            {
                ModelState.AddModelError("NhanVien.SDT", "Số điện thoại này đã được sử dụng bởi nhân viên khác.");
            }

            if (!ModelState.IsValid) return View(model);

            try
            {
                // XỬ LÝ TÀI KHOẢN
                TaiKhoan tkToUpdate = null;

                if (model.TaiKhoan.MaTK > 0) // Đã có tài khoản
                {
                    tkToUpdate = db.GetTaiKhoanByMaTK(model.TaiKhoan.MaTK);
                    if (tkToUpdate != null)
                    {
                        tkToUpdate.Username = model.NhanVien.SDT;
                        tkToUpdate.IsActive = model.TaiKhoan.IsActive;
                        if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
                            tkToUpdate.PasswordHash = model.TaiKhoan.PasswordHash;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash)) // Tạo mới tài khoản
                {
                    tkToUpdate = new TaiKhoan
                    {
                        MaTK = 0,
                        Username = model.NhanVien.SDT,
                        PasswordHash = model.TaiKhoan.PasswordHash,
                        IsActive = true
                    };
                }

                bool result = db.Update(model.NhanVien, tkToUpdate);

                if (result)
                {
                    TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật thông tin.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        // ===================== XÓA NHÂN VIÊN =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã nhân viên không hợp lệ.";
                    return RedirectToAction("Index");
                }

                bool result = db.Delete(id);
                if (result)
                    TempData["Success"] = "Đã xóa hồ sơ nhân viên thành công.";
                else
                    TempData["Error"] = "Không tìm thấy nhân viên hoặc không thể xóa.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi xóa nhân viên: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ===================== KHÓA/MỞ TÀI KHOẢN =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleLock(string id)
        {
            if (string.IsNullOrEmpty(id)) return HttpNotFound();

            try
            {
                bool? newStatus = db.ToggleAccountStatus(id);

                if (newStatus.HasValue)
                {
                    if (newStatus.Value == true)
                        TempData["Success"] = "Mở khóa tài khoản thành công!";
                    else
                        TempData["Success"] = "Đã khóa tài khoản nhân viên!";
                }
                else
                {
                    TempData["Error"] = "Nhân viên này chưa được cấp tài khoản.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
