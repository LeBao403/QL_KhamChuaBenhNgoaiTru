using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class BenhNhanController : Controller
    {
        private readonly BenhNhanDB db = new BenhNhanDB();

        // --- DANH SÁCH KHÁCH HÀNG ---
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10;
            try
            {
                var dsbn = db.GetAll(page, pageSize);
                int totalCount = db.GetCount();

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(dsbn);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách khách hàng: " + ex.Message;
                return View("Error");
            }
        }

        public ActionResult Search(string q)
        {
            var list = string.IsNullOrEmpty(q) ? db.GetAll() : db.Search(q);
            return PartialView("_BenhNhanTable", list);
        }


        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã bệnh nhân không hợp lệ");

            // Lấy thông tin bệnh nhân
            var bn = db.GetById(id);
            if (bn == null)
                return HttpNotFound("Không tìm thấy bệnh nhân!");

            // === BỔ SUNG: LẤY THÔNG TIN TÀI KHOẢN ===
            TaiKhoan tk = null;
            if (bn.MaTK.HasValue) // bn.MaTK là kiểu int? (nullable)
            {
                // Gọi hàm để lấy tài khoản
                tk = db.GetTaiKhoanByMaTK(bn.MaTK);
            }
            // =========================================

            // Tạo ViewModel
            var model = new BenhNhanManageViewModel
            {
                BenhNhan = bn,
                TaiKhoan = tk  // Gán tài khoản vào ViewModel
            };

            return View(model);
        }

        // --- THÊM KHÁCH HÀNG ---
        public ActionResult Create()
        {
            // Tạo model mới tinh, xóa sạch mọi dấu vết lỗi cũ
            ModelState.Clear();
            var model = new BenhNhanManageViewModel();

            // Khởi tạo sẵn object để tránh lỗi Null ở View
            model.BenhNhan = new BenhNhan();
            model.TaiKhoan = new TaiKhoan();

            model.BenhNhan.MaBN = db.GenerateNextMaBN();
            // Mặc định ngày sinh là hôm nay để input date không bị lỗi
            model.BenhNhan.NgaySinh = DateTime.Now;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BenhNhanManageViewModel model)
        {
            // --- 1. KIỂM TRA TRÙNG LẶP TRÊN BẢNG BENHNHAN ---

            // Kiểm tra CCCD (Nếu có nhập)
            if (!string.IsNullOrWhiteSpace(model.BenhNhan.CCCD) && db.BenhNhanCccdExists(model.BenhNhan.CCCD))
                ModelState.AddModelError("BenhNhan.CCCD", "Số CCCD này đã được đăng ký cho bệnh nhân khác.");

            // Kiểm tra Email (Nếu có nhập)
            if (!string.IsNullOrWhiteSpace(model.BenhNhan.Email) && db.BenhNhanEmailExists(model.BenhNhan.Email))
                ModelState.AddModelError("BenhNhan.Email", "Email này đã tồn tại trong hệ thống.");

            // Kiểm tra Số điện thoại (Bắt buộc hoặc nếu có nhập)
            if (!string.IsNullOrWhiteSpace(model.BenhNhan.SDT) && db.BenhNhanPhoneExists(model.BenhNhan.SDT))
                ModelState.AddModelError("BenhNhan.SDT", "Số điện thoại này đã được sử dụng.");

            // [MỚI] Kiểm tra trùng số thẻ BHYT (Cực kỳ quan trọng)
            if (model.BenhNhan.BHYT)
            {
                if (string.IsNullOrWhiteSpace(model.BenhNhan.SoTheBHYT))
                {
                    ModelState.AddModelError("BenhNhan.SoTheBHYT", "Vui lòng nhập Số thẻ BHYT khi đã tích chọn Có BHYT.");
                }
                else if (db.BenhNhanBhytExists(model.BenhNhan.SoTheBHYT))
                {
                    ModelState.AddModelError("BenhNhan.SoTheBHYT", "Số thẻ BHYT này đã tồn tại trên hệ thống.");
                }
            }

            // --- 2. KIỂM TRA LOGIC TÀI KHOẢN ---

            if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
            {
                // Ưu tiên dùng SĐT làm Username nếu bác không có ô nhập Username riêng
                string usernameToSearch = !string.IsNullOrWhiteSpace(model.TaiKhoan.Username)
                                          ? model.TaiKhoan.Username
                                          : model.BenhNhan.SDT;

                if (string.IsNullOrWhiteSpace(usernameToSearch))
                {
                    ModelState.AddModelError("", "Cần có Số điện thoại hoặc Username để tạo tài khoản.");
                }
                else if (db.UsernameExists(usernameToSearch))
                {
                    ModelState.AddModelError("TaiKhoan.Username", "Tên đăng nhập này đã có người sử dụng.");
                }
            }

            if (!ModelState.IsValid) return View(model);

            // --- 3. TIẾN HÀNH LƯU DỮ LIỆU ---

            TaiKhoan tk = null;
            if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
            {
                tk = new TaiKhoan
                {
                    Username = !string.IsNullOrWhiteSpace(model.TaiKhoan.Username) ? model.TaiKhoan.Username : model.BenhNhan.SDT,
                    PasswordHash = model.TaiKhoan.PasswordHash,
                    IsActive = true
                };
            }

            try
            {
                bool result = db.Create(model.BenhNhan, tk);
                if (result)
                {
                    TempData["Success"] = "Thêm bệnh nhân mới thành công!";
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




        // --- XÓA BỆNH NHÂN ---
        [HttpPost]
        [ValidateAntiForgeryToken] // Nên có để bảo mật
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ViewBag.ErrorMessage = "Mã bệnh nhân không hợp lệ.";
                    return View("Error");
                }

                bool result = db.Delete(id);
                if (result)
                {
                    TempData["Success"] = "Đã xóa hồ sơ bệnh nhân thành công.";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy bệnh nhân hoặc không thể xóa.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi xóa bệnh nhân: " + ex.Message;
                return View("Error");
            }
        }

        // [GET]
        public ActionResult Edit_KH(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã khách hàng không hợp lệ");

            var bn = db.GetById(id);
            if (bn == null)
                return HttpNotFound("Không tìm thấy khách hàng!");

            // --- BẮT ĐẦU SỬA ĐỔI: Lấy thông tin tài khoản ---
            TaiKhoan tk = null;
            if (bn.MaTK.HasValue && bn.MaTK.Value > 0)
            {
                // Giả sử bạn đã có hàm GetTaiKhoanByMaTK từ yêu cầu trước
                tk = db.GetTaiKhoanByMaTK(bn.MaTK);
            }

            // Nếu không có tài khoản, tạo đối tượng rỗng
            if (tk == null)
            {
                tk = new TaiKhoan();
                // Gán Username = SDT hiện tại để hiển thị
                tk.Username = bn.SDT;
                // Mặc định là active khi form load
                tk.IsActive = true;
            }
            // --- KẾT THÚC SỬA ĐỔI ---

            // Sử dụng lại CreateViewModel
            var model = new BenhNhanManageViewModel
            {
                BenhNhan = bn,
                TaiKhoan = tk // Thêm tài khoản vào model
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_KH(BenhNhanManageViewModel model)
        {
            // 1. Lấy dữ liệu hiện tại từ Database để so sánh (tránh báo trùng chính mình)
            var currentBN = db.GetById(model.BenhNhan.MaBN);
            if (currentBN == null)
            {
                return HttpNotFound("Không tìm thấy bệnh nhân để cập nhật.");
            }

            // 2. KIỂM TRA TRÙNG LẶP (Chỉ check nếu có sự thay đổi so với DB)

            // Check CCCD
            if (!string.IsNullOrWhiteSpace(model.BenhNhan.CCCD) &&
                model.BenhNhan.CCCD != currentBN.CCCD &&
                db.BenhNhanCccdExists(model.BenhNhan.CCCD))
            {
                ModelState.AddModelError("BenhNhan.CCCD", "Số CCCD này đã được sử dụng bởi bệnh nhân khác.");
            }

            // Check Số điện thoại
            if (!string.IsNullOrWhiteSpace(model.BenhNhan.SDT) &&
                model.BenhNhan.SDT != currentBN.SDT &&
                db.BenhNhanPhoneExists(model.BenhNhan.SDT))
            {
                ModelState.AddModelError("BenhNhan.SDT", "Số điện thoại này đã được sử dụng bởi bệnh nhân khác.");
            }

            // Check Email
            if (!string.IsNullOrWhiteSpace(model.BenhNhan.Email) &&
                model.BenhNhan.Email != currentBN.Email &&
                db.BenhNhanEmailExists(model.BenhNhan.Email))
            {
                ModelState.AddModelError("BenhNhan.Email", "Email này đã được sử dụng bởi bệnh nhân khác.");
            }

            // Check BHYT (Chỉ check nếu bật dùng BHYT và có thay đổi số thẻ)
            if (model.BenhNhan.BHYT && !string.IsNullOrWhiteSpace(model.BenhNhan.SoTheBHYT))
            {
                if (model.BenhNhan.SoTheBHYT != currentBN.SoTheBHYT && db.BenhNhanBhytExists(model.BenhNhan.SoTheBHYT))
                {
                    ModelState.AddModelError("BenhNhan.SoTheBHYT", "Số thẻ BHYT này đã tồn tại trên hệ thống.");
                }
            }

            // 3. Nếu có lỗi Validate thì trả về View luôn
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 4. XỬ LÝ LOGIC TÀI KHOẢN (TK)
                TaiKhoan tkToUpdate = null;

                if (model.TaiKhoan.MaTK > 0) // Trường hợp đã có tài khoản
                {
                    tkToUpdate = db.GetTaiKhoanByMaTK(model.TaiKhoan.MaTK);
                    if (tkToUpdate != null)
                    {
                        tkToUpdate.Username = model.BenhNhan.SDT; // Luôn đồng bộ Username theo SĐT
                        tkToUpdate.IsActive = model.TaiKhoan.IsActive;

                        // Chỉ cập nhật mật khẩu nếu người dùng có nhập mới trên form
                        if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
                        {
                            tkToUpdate.PasswordHash = model.TaiKhoan.PasswordHash;
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash)) // Bệnh nhân cũ chưa có TK, giờ mới tạo
                {
                    tkToUpdate = new TaiKhoan
                    {
                        MaTK = 0,
                        Username = model.BenhNhan.SDT,
                        PasswordHash = model.TaiKhoan.PasswordHash,
                        IsActive = true
                    };
                }

                // 5. GỌI HÀM UPDATE DƯỚI DB
                bool result = db.Update(model.BenhNhan, tkToUpdate);

                if (result)
                {
                    TempData["Success"] = "Cập nhật thông tin bệnh nhân thành công!";
                    return RedirectToAction("Details", new { id = model.BenhNhan.MaBN.Trim() });
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật thông tin vào Cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

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
                        TempData["Success"] = "Đã khóa tài khoản bệnh nhân!";
                }
                else
                {
                    TempData["Error"] = "Bệnh nhân này chưa được cấp tài khoản.";
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