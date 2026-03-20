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
        // Xử lý logic Đăng nhập
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tài khoản và mật khẩu!";
                return View();
            }

            // 1. Tìm tài khoản dựa trên Username
            var tk = db.GetTaiKhoanByUsername(username);

            // Bắt lỗi: Không có tài khoản
            if (tk == null)
            {
                ViewBag.Error = "Tài khoản không tồn tại trên hệ thống!";
                return View();
            }

            // Bắt lỗi: Sai mật khẩu
            if (tk.PasswordHash != password)
            {
                ViewBag.Error = "Mật khẩu không chính xác!";
                return View();
            }

            // Bắt lỗi: Tài khoản bị khóa (IsActive = 0 / false)
            // Lưu ý: Tuỳ vào việc model TaiKhoan của bạn thuộc tính IsActive là bool hay bool?
            // Nếu là bool? (có thể null) thì dùng tk.IsActive == false hoặc tk.IsActive != true
            if (tk.IsActive == false)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên!";
                return View();
            }

            // Nếu qua hết các kiểm tra trên -> Đăng nhập thành công
            // 1. Lưu session chung
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
                Session["HoTenNV"] = nv.HoTen;
                Session["MaChucVu"] = nv.MaChucVu;

                int chucVu = nv.MaChucVu.HasValue ? nv.MaChucVu.Value : 0;
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

                    case 5: // Điều dưỡng
                    case 6: // KTV CLS
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
                // 3. Bệnh Nhân
                var bn = db.GetBenhNhanByMaTK(tk.MaTK);

                if (bn != null)
                {
                    Session["BenhNhan"] = bn;
                    Session["MaBN"] = bn.MaBN;
                    Session["HoTenBN"] = bn.HoTen;
                }

                // Điều hướng Bệnh nhân về trang chủ
                return RedirectToAction("Index", "Home", new { area = "" });
            }
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
            Session.Clear(); // Xóa sạch mọi Session (cả Bệnh nhân lẫn Nhân viên)
            return RedirectToAction("Login");
        }
    }
}