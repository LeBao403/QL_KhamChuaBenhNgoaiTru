using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    // 1. ĐỔI KẾ THỪA TỪ Controller SANG BaseStaffController
    public class BacSiController : BaseStaffController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var nv = Session["NhanVien"] as NhanVien;

            // 2. Chỉ có Trưởng khoa (3) và BS điều trị (4) mới được vào
            if (nv != null && nv.MaChucVu != 3 && nv.MaChucVu != 4)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" })
                );
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}