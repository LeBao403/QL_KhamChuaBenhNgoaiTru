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
    public class KhachHangController : Controller
    {
        private readonly KhachHangDB db = new KhachHangDB();

        // --- DANH SÁCH KHÁCH HÀNG ---
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10;
            try
            {
                var dsKh = db.GetAll(page, pageSize);
                int totalCount = db.GetCount();

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(dsKh);
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
            return PartialView("_KhachHangTable", list);
        }


        //public ActionResult Details(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã khách hàng không hợp lệ");

        //    // Lấy thông tin khách hàng
        //    var kh = db.GetById(id);
        //    if (kh == null)
        //        return HttpNotFound("Không tìm thấy khách hàng!");

        //    // Lấy người giám hộ nếu có
        //    NguoiGiamHo ngh = null;
        //    if (!string.IsNullOrEmpty(kh.MaNGH))
        //    {
        //        ngh = db.GetNguoiGiamHoById(kh.MaNGH);
        //    }

        //    // Lấy lịch sử tiêm
        //    var lichSuTiem = db.GetLichSuTiem(id);

        //    // === BỔ SUNG: LẤY THÔNG TIN TÀI KHOẢN ===
        //    TaiKhoan tk = null;
        //    if (kh.MaTK.HasValue) // kh.MaTK là kiểu int? (nullable)
        //    {
        //        // Gọi hàm mới để lấy tài khoản
        //        tk = db.GetTaiKhoanByMaTK(kh.MaTK);
        //    }
        //    // =========================================

        //    // Tạo ViewModel
        //    var model = new KhachHangDetailsViewModel
        //    {
        //        KhachHang = kh,
        //        NguoiGiamHo = ngh,
        //        HoSoTiemList = lichSuTiem,
        //        TaiKhoan = tk  // Gán tài khoản vào ViewModel
        //    };

        //    return View(model);
        //}

        // --- THÊM KHÁCH HÀNG ---
        public ActionResult Create()
        {
            var model = new KhachHangManageViewModel();
            model.KhachHang.MaKH = db.GenerateNextMaKH();
            model.NguoiGiamHo.MaNGH = db.GenerateNextMaNGH();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhachHangManageViewModel model)
        {
            // Chỉ kiểm tra SĐT nếu người dùng CỐ GẮNG tạo tài khoản (tức là có nhập Mật khẩu)
            if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
            {
                if (string.IsNullOrWhiteSpace(model.KhachHang.SDT))
                {
                    ModelState.AddModelError("KhachHang.SDT", "Phải nhập Số điện thoại để dùng làm Username khi tạo tài khoản.");
                }
                else if (db.UsernameExists(model.KhachHang.SDT))
                {
                    ModelState.AddModelError("KhachHang.SDT", "Số điện thoại này đã được dùng làm username. Vui lòng chọn SĐT khác hoặc không tạo tài khoản.");
                }
            }

            // === KIỂM TRA TRÙNG CCCD / EMAIL / SĐT TRONG BẢNG KHACHHANG ===
            if (!string.IsNullOrWhiteSpace(model.KhachHang.CCCD) &&
                db.CustomerCccdExists(model.KhachHang.CCCD))
            {
                ModelState.AddModelError("KhachHang.CCCD", "CCCD này đã tồn tại trong hệ thống.");
            }

            if (!string.IsNullOrWhiteSpace(model.KhachHang.Email) &&
                db.CustomerEmailExists(model.KhachHang.Email))
            {
                ModelState.AddModelError("KhachHang.Email", "Email này đã tồn tại trong hệ thống.");
            }

            if (!string.IsNullOrWhiteSpace(model.KhachHang.SDT) &&
                db.CustomerPhoneExists(model.KhachHang.SDT))
            {
                ModelState.AddModelError("KhachHang.SDT", "Số điện thoại này đã tồn tại trong hệ thống.");
            }
            // === HẾT PHẦN KIỂM TRA TRÙNG ===

            if (!ModelState.IsValid)
                return View(model);

            // Xử lý Người giám hộ
            NguoiGiamHo ngh = model.NguoiGiamHo;
            // Chỉ coi là null nếu TẤT CẢ các trường bắt buộc (theo IValidatableObject) đều trống
            if (string.IsNullOrWhiteSpace(ngh?.HoTen) &&
                string.IsNullOrWhiteSpace(ngh?.SDT) &&
                string.IsNullOrWhiteSpace(ngh?.DiaChi))
            {
                ngh = null;
            }

            // Xử lý Tài khoản
            TaiKhoan tk = null;
            if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
            {
                tk = new TaiKhoan
                {
                    Username = model.KhachHang.SDT, // Lấy username từ SĐT
                                                    // === LƯU PLAIN TEXT PASSWORD ===
                    PasswordHash = model.TaiKhoan.PasswordHash
                };
            }


            try
            {
                // Gọi hàm Create mới trong DBContext (truyền cả tk vào)
                bool result = db.Create(model.KhachHang, ngh, tk);

                if (result)
                    return RedirectToAction("Index", "KhachHang", new { Area = "Admin" });
                else
                {
                    ModelState.AddModelError("", "Không thể thêm khách hàng (Lỗi không xác định từ DB).");
                    return View(model);
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Mã lỗi UNIQUE constraint
            {
                if (ex.Message.Contains("Username"))
                {
                    ModelState.AddModelError("KhachHang.SDT", "Số điện thoại này đã được sử dụng làm Username. Vui lòng kiểm tra lại.");
                }
                else if (ex.Message.Contains("NGUOIGIAMHO"))
                {
                    ModelState.AddModelError("", "Lỗi khi thêm người giám hộ: " + ex.Message); // Thông báo lỗi NGH rõ hơn
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi CSDL (Ràng buộc UNIQUE): " + ex.Message);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi ở đây
                // Log.Error("Lỗi hệ thống khi tạo KH: " + ex.ToString());
                ModelState.AddModelError("", "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại hoặc liên hệ quản trị viên.");
                return View(model);
            }
        }




        // --- XÓA KHÁCH HÀNG ---
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ViewBag.ErrorMessage = "Mã khách hàng không hợp lệ.";
                    return View("Error");
                }

                db.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi xóa khách hàng: " + ex.Message;
                return View("Error");
            }
        }

        // [GET]
        public ActionResult Edit_KH(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã khách hàng không hợp lệ");

            var kh = db.GetById(id);
            if (kh == null)
                return HttpNotFound("Không tìm thấy khách hàng!");

            // Xử lý Người giám hộ
            NguoiGiamHo ngh = null;
            if (!string.IsNullOrEmpty(kh.MaNGH))
            {
                ngh = db.GetNguoiGiamHoById(kh.MaNGH);
            }
            if (ngh == null)
            {
                ngh = new NguoiGiamHo
                {
                    MaNGH = db.GenerateNextMaNGH() // Tạo sẵn mã NGH mới
                };
            }

            // --- BẮT ĐẦU SỬA ĐỔI: Lấy thông tin tài khoản ---
            TaiKhoan tk = null;
            if (kh.MaTK.HasValue && kh.MaTK.Value > 0)
            {
                // Giả sử bạn đã có hàm GetTaiKhoanByMaTK từ yêu cầu trước
                tk = db.GetTaiKhoanByMaTK(kh.MaTK);
            }

            // Nếu không có tài khoản, tạo đối tượng rỗng
            if (tk == null)
            {
                tk = new TaiKhoan();
                // Gán Username = SDT hiện tại để hiển thị
                tk.Username = kh.SDT;
                // Mặc định là active khi form load
                tk.IsActive = true;
            }
            // --- KẾT THÚC SỬA ĐỔI ---

            // Sử dụng lại CreateViewModel
            var model = new KhachHangManageViewModel
            {
                KhachHang = kh,
                NguoiGiamHo = ngh,
                TaiKhoan = tk // Thêm tài khoản vào model
            };

            return View(model);
        }

        // [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_KH(KhachHangManageViewModel model)
        {
            // --- BẮT ĐẦU SỬA ĐỔI: Thêm Validation cho Tài khoản ---

            // Chỉ kiểm tra tài khoản nếu người dùng có ý định cập nhật/tạo
            // (Hoặc là tài khoản đã tồn tại, hoặc là đang nhập mật khẩu mới)
            if (model.TaiKhoan.MaTK > 0 || !string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
            {
                // 1. Kiểm tra SĐT (bắt buộc nếu có TK)
                if (string.IsNullOrWhiteSpace(model.KhachHang.SDT))
                {
                    ModelState.AddModelError("KhachHang.SDT", "Phải nhập SĐT để dùng làm Username.");
                }
                // 2. Kiểm tra SĐT (Username) có bị trùng với người khác không
                // (Chúng ta cần hàm mới `UsernameExistsForAnotherAccount`)
                else if (db.UsernameExistsForAnotherAccount(model.KhachHang.SDT, model.TaiKhoan.MaTK))
                {
                    ModelState.AddModelError("KhachHang.SDT", "SĐT này đã được tài khoản KHÁC sử dụng làm Username.");
                }
            }
            // --- KẾT THÚC SỬA ĐỔI ---
            // === KIỂM TRA TRÙNG CCCD / EMAIL / SĐT TRONG BẢNG KHACHHANG (BỎ QUA CHÍNH KHÁCH HIỆN TẠI) ===
            string currentMaKH = model.KhachHang.MaKH;

            if (!string.IsNullOrWhiteSpace(model.KhachHang.CCCD) &&
                db.CustomerCccdExists(model.KhachHang.CCCD, currentMaKH))
            {
                ModelState.AddModelError("KhachHang.CCCD", "CCCD này đã được sử dụng bởi khách hàng khác.");
            }

            if (!string.IsNullOrWhiteSpace(model.KhachHang.Email) &&
                db.CustomerEmailExists(model.KhachHang.Email, currentMaKH))
            {
                ModelState.AddModelError("KhachHang.Email", "Email này đã được sử dụng bởi khách hàng khác.");
            }

            if (!string.IsNullOrWhiteSpace(model.KhachHang.SDT) &&
                db.CustomerPhoneExists(model.KhachHang.SDT, currentMaKH))
            {
                ModelState.AddModelError("KhachHang.SDT", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Xử lý logic nếu người dùng xóa trắng thông tin NGH
                NguoiGiamHo nghToUpdate = model.NguoiGiamHo;
                if (string.IsNullOrWhiteSpace(nghToUpdate.HoTen) &&
                    string.IsNullOrWhiteSpace(nghToUpdate.SDT) &&
                    string.IsNullOrWhiteSpace(nghToUpdate.DiaChi))
                {
                    nghToUpdate = null; // Coi như không có người giám hộ
                    model.KhachHang.MaNGH = null; // Quan trọng: ngắt kết nối MaNGH
                }

                // --- BẮT ĐẦU SỬA ĐỔI: Xử lý thông tin Tài Khoản ---

                TaiKhoan tkToUpdate = null;

                // TH 1: Tài khoản đã tồn tại (MaTK > 0)
                if (model.TaiKhoan.MaTK > 0)
                {
                    // Lấy tài khoản cũ từ DB
                    tkToUpdate = db.GetTaiKhoanByMaTK(model.TaiKhoan.MaTK);
                    if (tkToUpdate != null)
                    {
                        // Cập nhật các giá trị từ form
                        tkToUpdate.Username = model.KhachHang.SDT; // Username LUÔN là SĐT
                        tkToUpdate.IsActive = model.TaiKhoan.IsActive; // Cập nhật trạng thái

                        // QUAN TRỌNG: Chỉ cập nhật Mật khẩu NẾU người dùng nhập mật khẩu mới
                        // Nếu để trống, ta giữ nguyên mật khẩu cũ.
                        if (!string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
                        {
                            tkToUpdate.PasswordHash = model.TaiKhoan.PasswordHash;
                        }
                        // Nếu model.TaiKhoan.PasswordHash rỗng -> không làm gì cả, mật khẩu cũ được giữ nguyên.
                    }
                }
                // TH 2: Tài khoản chưa tồn tại (MaTK = 0) VÀ người dùng đang nhập mật khẩu
                else if (model.TaiKhoan.MaTK == 0 && !string.IsNullOrWhiteSpace(model.TaiKhoan.PasswordHash))
                {
                    // Đây là trường hợp TẠO MỚI tài khoản cho khách hàng cũ
                    tkToUpdate = new TaiKhoan
                    {
                        MaTK = 0,
                        Username = model.KhachHang.SDT,
                        PasswordHash = model.TaiKhoan.PasswordHash,
                        IsActive = model.TaiKhoan.IsActive // Lấy trạng thái từ form
                    };
                }
                // (TH 3: MaTK = 0 và Mật khẩu rỗng -> tkToUpdate = null -> không làm gì cả)

                // --- KẾT THÚC SỬA ĐỔI ---

                // Gọi hàm Update đã được nâng cấp (xem mục 3)
                bool result = db.Update(model.KhachHang, nghToUpdate, tkToUpdate);

                if (result)
                {
                    return RedirectToAction("Details", new { id = model.KhachHang.MaKH.Trim() });
                }
                else
                {
                    ModelState.AddModelError("", "Không thể cập nhật thông tin khách hàng.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                return View(model);
            }
        }
    }
}