using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    [Authorize]
    public class BaseAdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            // 1. Kiểm tra Đăng nhập VÀ là Nhân viên
            if (session["UserType"] == null || session["UserType"].ToString() != "Employee")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Login",
                        area = ""
                    })
                );
                return; // Dừng thực thi
            }

            // 2. Kiểm tra có Mã Chức Vụ không (Quan trọng)
            if (session["MaChucVu"] == null || (int)session["MaChucVu"] == 0)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new System.Web.Routing.RouteValueDictionary(new
                   {
                       controller = "Account",
                       action = "Login",
                       area = "",
                       message = "Tài khoản nhân viên chưa được phân quyền."
                   })
               );
                return; // Dừng thực thi
            }

            // (Admin không bắt buộc phải có MaCoSo, nên ta bỏ qua kiểm tra đó)

            base.OnActionExecuting(filterContext);
        }
    }
}