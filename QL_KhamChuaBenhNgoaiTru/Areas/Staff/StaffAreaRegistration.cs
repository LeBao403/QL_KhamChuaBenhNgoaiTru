using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff
{
    public class StaffAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Staff";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Staff_default",
                "Staff/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers" }
            );
        }
    }
}