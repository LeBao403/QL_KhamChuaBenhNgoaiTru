using Microsoft.AspNet.SignalR;

namespace QL_KhamChuaBenhNgoaiTru.Hubs
{
    public class ClinicHub : Hub
    {
        // Hàm này để mốt Client thích thì gọi lên Server (ít dùng hơn)
        public void HelloServer(string message)
        {
            Clients.All.nhanThongBaoTuServer("Server đã nhận: " + message);
        }
    }
}