using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Helpers;

namespace QL_KhamChuaBenhNgoaiTru.Filters
{
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly TaiKhoanDB _taiKhoanDb = new TaiKhoanDB();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var authHeader = request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                filterContext.Result = Unauthorized("Thiếu JWT token. Vui lòng đăng nhập lại.");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (!JwtTokenHelper.TryValidateToken(token, out var jwtUser))
            {
                filterContext.Result = Unauthorized("JWT token không hợp lệ hoặc đã hết hạn.");
                return;
            }

            var tk = _taiKhoanDb.GetTaiKhoanByMaTK(jwtUser.MaTK);
            if (tk == null || !tk.IsActive)
            {
                filterContext.Result = Unauthorized("Tài khoản không tồn tại hoặc đã bị khóa.");
                return;
            }

            var session = filterContext.HttpContext.Session;
            session["TaiKhoan"] = tk;
            session["Username"] = tk.Username;
            session["MaTK"] = tk.MaTK;

            var bn = _taiKhoanDb.GetBenhNhanByMaTK(tk.MaTK);
            if (bn != null)
            {
                session["BenhNhan"] = bn;
                session["MaBN"] = bn.MaBN;
            }

            var nv = _taiKhoanDb.GetNhanVienByMaTK(tk.MaTK);
            if (nv != null)
            {
                session["NhanVien"] = nv;
                session["MaNV"] = nv.MaNV;
                session["HoTenNV"] = nv.HoTen;
                session["MaChucVu"] = nv.MaChucVu;
            }

            base.OnActionExecuting(filterContext);
        }

        private static JsonResult Unauthorized(string message)
        {
            return new JsonResult
            {
                Data = new { success = false, message },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
