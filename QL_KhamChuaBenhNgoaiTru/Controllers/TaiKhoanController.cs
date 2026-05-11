using System;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.Models;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class TaiKhoanController : Controller
    {
        private TaiKhoanDB db = new TaiKhoanDB();

        // Mở trang Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Xử lý logic Đăng nhập
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tài khoản và mật khẩu!";
                return View();
            }

            var tk = db.CheckLogin(username, password);
            if (tk != null)
            {
                // 1. Lưu session chung (Dùng cho cả Nhân viên lẫn Bệnh nhân)
                Session["TaiKhoan"] = tk;
                Session["Username"] = tk.Username;
                Session["MaTK"] = tk.MaTK;

                // 2. Kiểm tra xem user này có phải là Nhân viên không
                var nv = db.GetNhanVienByMaTK(tk.MaTK);
                if (nv != null)
                {
                    // Lưu session riêng cho Nhân viên
                    Session["NhanVien"] = nv;
                    Session["MaNV"] = nv.MaNV;
                    Session["HoTenNV"] = nv.HoTen; // Lưu tên để hiển thị góc phải màn hình
                    Session["MaChucVu"] = nv.MaChucVu;

                    int chucVu = nv.MaChucVu.Value;
                    switch (chucVu)
                    {
                        case 1: // Giám đốc
                        case 2: // Admin
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                        case 3: // Trưởng khoa
                        case 4: // BS điều trị
                            return RedirectToAction("Index", "BacSi", new { area = "Staff" });

                        case 8: // Tiếp đón
                            return RedirectToAction("Index", "TiepTan", new { area = "Staff" });

                        case 9: // Thu ngân
                            return RedirectToAction("Index", "ThuNgan", new { area = "Staff" });

                        case 12: // Nhân viên kho
                            return RedirectToAction("Index", "PhatThuoc", new { area = "Staff" });

                        case 5: // Điều dưỡng
                        case 6: // KTV CLS
                            return RedirectToAction("Index", "CLS", new { area = "Staff" });

                        //case 5: // Điều dưỡng
                        case 7: // Dược sĩ
                        case 10: // Bảo vệ
                        case 11: // Tạp vụ
                            return RedirectToAction("Index", "Home", new { area = "" });

                        default:
                            return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
                else
                {
                    // 3. Nếu KHÔNG phải Nhân viên -> Đi tìm xem có phải Bệnh Nhân không
                    // Bác nhớ viết hàm GetBenhNhanByMaTK bên file TaiKhoanDB nhé
                    var bn = db.GetBenhNhanByMaTK(tk.MaTK);

                    if (bn != null)
                    {
                        // Có thông tin bệnh nhân -> Lưu Session Bệnh nhân
                        Session["BenhNhan"] = bn;
                        Session["MaBN"] = bn.MaBN;
                        Session["HoTenBN"] = bn.HoTen; // Giả sử cột tên trong DB của bác là HoTen
                    }

                    // Cuối cùng, điều hướng Bệnh nhân về trang Cổng Bệnh nhân
                    return RedirectToAction("LichKham", "BenhNhanPortal");
                }
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác, hoặc đã bị khóa!";
            return View();
        }

        // Mở trang Đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Xử lý logic Đăng ký
        [HttpPost]
        public ActionResult Register(string hoTen, string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // Kiểm tra trùng username hoặc email
            var existTk = db.GetTaiKhoanByUsernameOrSdt(username);
            if (existTk != null)
            {
                ViewBag.Error = "Tên đăng nhập (Số điện thoại) này đã có người sử dụng!";
                return View();
            }

            // Sinh mã OTP 6 số
            var existEmail = db.GetTaiKhoanByUsernameOrSdt(email);
            if (existEmail != null)
            {
                ViewBag.Error = "Email này đã có người sử dụng!";
                return View();
            }

            Random rand = new Random();
            string otp = rand.Next(100000, 999999).ToString();

            // Lưu thông tin đăng ký vào Session
            Session["Register_HoTen"] = hoTen;
            Session["Register_Username"] = username;
            Session["Register_Email"] = email;
            Session["Register_Password"] = password;
            Session["Register_OTP"] = otp;
            Session["Register_OTP_Time"] = DateTime.Now;

            // Gửi OTP qua Email
            bool sendOk = QL_KhamChuaBenhNgoaiTru.Helpers.EmailHelper.SendOTP(email, otp);
            if (sendOk)
            {
                return RedirectToAction("VerifyOTP");
            }
            else
            {
                ViewBag.Error = "Có lỗi xảy ra khi gửi email xác thực. Vui lòng kiểm tra lại địa chỉ email hoặc thử lại sau.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult VerifyOTP()
        {
            if (Session["Register_OTP"] == null)
            {
                return RedirectToAction("Register");
            }
            return View();
        }

        [HttpPost]
        public ActionResult VerifyOTP(string otp)
        {
            if (Session["Register_OTP"] == null || Session["Register_Username"] == null)
            {
                return RedirectToAction("Register");
            }

            string savedOtp = Session["Register_OTP"].ToString();
            DateTime otpTime = (DateTime)Session["Register_OTP_Time"];

            if (DateTime.Now.Subtract(otpTime).TotalMinutes > 5)
            {
                ViewBag.Error = "Mã OTP đã hết hạn. Vui lòng đăng ký lại.";
                Session.Remove("Register_OTP");
                return View();
            }

            if (otp == savedOtp)
            {
                // Xác nhận thành công -> Lưu vào DB
                TaiKhoan tk = new TaiKhoan()
                {
                    Username = Session["Register_Username"].ToString(),
                    PasswordHash = Session["Register_Password"].ToString()
                };

                // Ta sẽ truyền thêm Email vào InsertTaiKhoan để lưu vào Database
                string email = Session["Register_Email"].ToString();
                string hoTen = Session["Register_HoTen"]?.ToString() ?? "Chưa cập nhật";

                bool isSuccess = db.InsertTaiKhoanWithEmail(tk, email, hoTen); // Hàm này ta sẽ cập nhật trong TaiKhoanDB
                if (isSuccess)
                {
                    // Xóa session
                    Session.Remove("Register_HoTen");
                    Session.Remove("Register_Username");
                    Session.Remove("Register_Email");
                    Session.Remove("Register_Password");
                    Session.Remove("Register_OTP");
                    Session.Remove("Register_OTP_Time");

                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Lỗi DEBUG: InsertTaiKhoanWithEmail trả về false.";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Mã OTP không chính xác!";
                return View();
            }
        }

        // Xử lý Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa sạch mọi Session (cả Bệnh nhân lẫn Nhân viên)
            return RedirectToAction("Login");
        }
    }
}
