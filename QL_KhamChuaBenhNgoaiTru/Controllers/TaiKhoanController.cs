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
                // Lưu session chung
                Session["TaiKhoan"] = tk;
                Session["Username"] = tk.Username;
                Session["MaTK"] = tk.MaTK;

                // Kiểm tra xem user này có phải là Nhân viên không
                // Kiểm tra xem user này có phải là Nhân viên không
                var nv = db.GetNhanVienByMaTK(tk.MaTK);
                if (nv != null)
                {
                    // Lưu session nhân viên
                    Session["NhanVien"] = nv;
                    Session["MaNV"] = nv.MaNV;
                    Session["HoTenNV"] = nv.HoTen;
                    Session["MaChucVu"] = nv.MaChucVu;

                    // Phân quyền điều hướng dựa vào chức vụ mới
                    // Phân quyền điều hướng dựa vào chức vụ
                    int chucVu = nv.MaChucVu.Value;

                    switch (chucVu)
                    {
                        case 1: // Giám đốc
                        case 2: // Admin
                                // Vào trang Dashboard của Admin
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                        case 3: // Trưởng khoa
                        case 4: // BS điều trị
                                // Vào màn hình phòng khám của Bác sĩ
                            return RedirectToAction("Index", "BacSi", new { area = "Staff" });

                        case 8: // Tiếp đón
                                // Vào màn hình tiếp nhận
                            return RedirectToAction("Index", "TiepTan", new { area = "Staff" });

                        case 9: // Thu ngân
                                // Tạm thời điều hướng vào ThuNgan (bạn cần tạo Controller này sau)
                            return RedirectToAction("Index", "ThuNgan", new { area = "Staff" });

                        case 5: // Điều dưỡng
                        case 6: // KTV CLS
                        case 7: // Dược sĩ
                        case 10: // Bảo vệ
                        case 11: // Tạp vụ
                                 // TODO: Bạn có thể tạo thêm Controller (VD: DieuDuongController) trong Area Staff sau.
                                 // Tạm thời, khi đăng nhập thành công, tạm điều hướng về Trang chủ.
                            return RedirectToAction("Index", "Home", new { area = "" });

                        default:
                            // Các vai trò không xác định hoặc lỗi -> về trang chủ
                            return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

                // Nếu không phải nhân viên -> Là bệnh nhân, về trang chủ
                return RedirectToAction("Index", "Home", new { area = "" });
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
        public ActionResult Register(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            TaiKhoan tk = new TaiKhoan()
            {
                Username = username,
                PasswordHash = password
            };

            bool isSuccess = db.InsertTaiKhoan(tk);
            if (isSuccess)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "Tên đăng nhập này đã có người sử dụng!";
                return View();
            }
        }

        // Xử lý Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}