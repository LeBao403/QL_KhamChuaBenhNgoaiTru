using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    // Kế thừa Controller mặc định
    public class BaseAdminController : Controller
    {
        // Hàm này sẽ tự động chạy TRƯỚC MỖI ACTION trong khu vực Admin
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Lấy thông tin session nhân viên
            var nv = Session["NhanVien"] as NhanVien;

            if (nv == null)
            {
                // Chưa đăng nhập -> Đá về trang đăng nhập ở bên ngoài (Area = "")
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "TaiKhoan", action = "Login", area = "" })
                );
            }
            else
            {
                // 2. Đã đăng nhập, kiểm tra chức vụ (Phân quyền)
                // Theo logic của bạn: 1 = Giám đốc, 2 = Admin
                int chucVu = nv.MaChucVu ?? 0;

                if (chucVu != 1 && chucVu != 2)
                {
                    // Đăng nhập rồi nhưng không phải Admin/Giám đốc -> Không cho vào
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" })
                    );
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}