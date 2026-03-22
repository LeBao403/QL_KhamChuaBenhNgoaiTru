using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.NhanVienKho.Controllers
{
    public class BaseNhanVienKhoController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var nv = Session["NhanVien"] as NhanVien;

            // 1. Chưa đăng nhập -> Quay về Login
            if (nv == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "TaiKhoan", action = "Login", area = "" })
                );
                return;
            }

            // 2. Chỉ cho phép nhân viên kho (MaChucVu = 12)
            int chucVu = nv.MaChucVu ?? 0;
            if (chucVu != 12)
            {
                // Admin -> Admin, Bác sĩ -> BacSi, Thu ngân -> ThuNgan
                switch (chucVu)
                {
                    case 1:
                    case 2:
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Dashboard", action = "Index", area = "Admin" }));
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "BacSi", action = "Index", area = "Staff" }));
                        break;
                    case 8:
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "TiepTan", action = "Index", area = "Staff" }));
                        break;
                    case 9:
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "ThuNgan", action = "Index", area = "Staff" }));
                        break;
                    default:
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "TaiKhoan", action = "Login", area = "" }));
                        break;
                }
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
