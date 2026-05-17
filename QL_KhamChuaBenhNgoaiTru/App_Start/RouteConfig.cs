using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QL_KhamChuaBenhNgoaiTru
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "MobileApiRegisterRequestOtp",
                url: "MobileApi/RegisterRequestOtp",
                defaults: new { controller = "MobileApi", action = "RegisterRequestOtp" },
                namespaces: new[] { "QL_KhamChuaBenhNgoaiTru.Controllers" }
            );

            routes.MapRoute(
                name: "MobileApiRegisterVerifyOtp",
                url: "MobileApi/RegisterVerifyOtp",
                defaults: new { controller = "MobileApi", action = "RegisterVerifyOtp" },
                namespaces: new[] { "QL_KhamChuaBenhNgoaiTru.Controllers" }
            );

            routes.MapRoute(
                name: "MobileApiRegister",
                url: "MobileApi/Register",
                defaults: new { controller = "MobileApi", action = "Register" },
                namespaces: new[] { "QL_KhamChuaBenhNgoaiTru.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "QL_KhamChuaBenhNgoaiTru.Controllers" }
            );
        }
    }
}
