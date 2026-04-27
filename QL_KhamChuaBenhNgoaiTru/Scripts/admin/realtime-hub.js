// File: Scripts/admin/realtime-hub.js

$(function () {
    // 1. Kết nối tới Hub trên Server (lưu ý: tên biến viết thường chữ cái đầu)
    var clinicHub = $.connection.clinicHub;

    if (!clinicHub) {
        console.error("🔴 SignalR: Không tìm thấy clinicHub. Hãy kiểm tra lại file /signalr/hubs");
        return;
    }

    // =========================================================
    // 2. KHAI BÁO CÁC HÀM LẮNG NGHE TÍN HIỆU TỪ SERVER
    // (Bất kỳ trang nào cũng sẽ nhận được thông báo này)
    // =========================================================

    // Hàm 1: Nhận thông báo chung (Ví dụ: Có bệnh nhân mới đặt lịch)
    clinicHub.client.nhanThongBaoChung = function (title, message) {
        console.log("🔔 TÍN HIỆU MỚI: " + title + " - " + message);

        // Nếu web bác có cài thư viện Toastr thì dùng lệnh dưới cho đẹp:
        // toastr.info(message, title);

        // Dùng tạm Alert nếu chưa có Toastr
        alert("🔔 " + title + "\n" + message);
    };

    // Hàm 2: Tự động tải lại bảng Dữ liệu nếu đang ở đúng trang đó
    clinicHub.client.lamMoiDanhSachKham = function () {
        // Kiểm tra xem web có đang đứng ở trang Khám Bệnh không
        if (window.location.href.indexOf("KhamBenh") > -1) {
            console.log("🔄 Đang tải lại danh sách khám bệnh...");
            // Gọi hàm load lại bảng (bác tự định nghĩa sau)
            // loadTableData(); 
        }
    };

    // =========================================================
    // 3. KHỞI ĐỘNG ĐƯỜNG TRUYỀN (MỞ BỘ ĐÀM)
    // =========================================================
    $.connection.hub.start().done(function () {
        console.log("🟢 SignalR: Đã kết nối Real-time toàn hệ thống thành công!");
    }).fail(function (error) {
        console.error("🔴 SignalR Lỗi kết nối: " + error);
    });
});