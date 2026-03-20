using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    // 1. ĐỔI KẾ THỪA TỪ Controller SANG BaseStaffController
    public class TiepTanController : BaseStaffController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext); // Luôn chạy code của BaseStaffController trước

            var nv = Session["NhanVien"] as NhanVien;

            // 2. Chặn Bác sĩ: Chỉ Tiếp đón (8) mới được ở lại đây
            if (nv != null && nv.MaChucVu != 8)
            {
                // Nếu là Bác sĩ (3, 4) đang cố ấn vào "Tiếp nhận", điều hướng họ an toàn về lại trang Bác Sĩ
                if (nv.MaChucVu == 3 || nv.MaChucVu == 4)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "BacSi", action = "Index", area = "Staff" })
                    );
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}