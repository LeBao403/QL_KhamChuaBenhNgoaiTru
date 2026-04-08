using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QL_KhamChuaBenhNgoaiTru
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Khởi động trình lên lịch sao lưu tự động
            QL_KhamChuaBenhNgoaiTru.Services.BackupScheduler.Start();
            ModelBinders.Binders.Add(typeof(decimal), new QL_KhamChuaBenhNgoaiTru.Models.DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new QL_KhamChuaBenhNgoaiTru.Models.DecimalModelBinder());
        }
    }
}
