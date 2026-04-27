using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(QL_KhamChuaBenhNgoaiTru.Startup))]
namespace QL_KhamChuaBenhNgoaiTru
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Kích hoạt SignalR Hubs
            app.MapSignalR();
        }
    }
}