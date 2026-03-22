using System;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.Models;

using BenhNhanModel = QL_KhamChuaBenhNgoaiTru.Models.BenhNhan;

namespace QL_KhamChuaBenhNgoaiTru.Areas.BenhNhan.Controllers
{
    public class BaseBenhNhanController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;

            if (bn == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "TaiKhoan", action = "Login", area = "" }));
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
