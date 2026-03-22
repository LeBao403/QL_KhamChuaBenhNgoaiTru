using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.BenhNhan
{
    public class BenhNhanAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "BenhNhan"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BenhNhan_default",
                "BenhNhan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "QL_KhamChuaBenhNgoaiTru.Areas.BenhNhan.Controllers" }
            );
        }
    }
}
