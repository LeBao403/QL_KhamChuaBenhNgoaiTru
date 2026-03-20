using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    // Kế thừa Controller mặc định
    public class BaseStaffController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var nv = Session["NhanVien"] as NhanVien;

            // 1. Nếu Session mất hoặc chưa đăng nhập -> Văng ra trang Login
            if (nv == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "TaiKhoan", action = "Login", area = "" })
                );
                return;
            }

            // 2. Nếu Admin (1,2) vô tình vào khu vực Staff -> Trả về trang Admin
            int chucVu = nv.MaChucVu ?? 0;
            if (chucVu == 1 || chucVu == 2)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "BenhNhan", action = "Index", area = "Admin" })
                );
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}