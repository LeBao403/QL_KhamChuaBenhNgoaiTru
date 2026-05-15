using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.Libraries;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class ThanhToanController : Controller
    {
        // =================================================================
        // 1. TẠO URL VÀ CHUYỂN HƯỚNG SANG VNPAY
        // =================================================================
        [HttpPost]
        public ActionResult ThanhToanVnPay(string maHoaDon, decimal tongTien)
        {
            // Lấy thông tin từ file AppSettings.Secrets.config
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                return Content("Lỗi: Không đọc được cấu hình VNPAY. Hãy kiểm tra lại file Secrets.");
            }

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

            // Xử lý tiền: Nhân 100 theo đúng chuẩn VNPAY (100k -> 10000000)
            long amount = (long)(tongTien * 100);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");

            // Xử lý IP: Ép về IPv4 để tránh lỗi ::1 của localhost
            string ipAddr = Utils.GetIpAddress();
            if (string.IsNullOrEmpty(ipAddr) || ipAddr == "::1")
            {
                ipAddr = "127.0.0.1";
            }
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);

            vnpay.AddRequestData("vnp_Locale", "vn");

            // Viết liền không dấu, không ký tự đặc biệt để tránh lệch Hash
            string cleanMaHD = (maHoaDon ?? "HD").Trim();
            vnpay.AddRequestData("vnp_OrderInfo", "ThanhToanHoaDon_" + cleanMaHD);

            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

            // TxnRef phải là duy nhất, thêm Ticks để tránh trùng
            vnpay.AddRequestData("vnp_TxnRef", cleanMaHD + "_" + DateTime.Now.Ticks.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        // =================================================================
        // 2. VNPAY TRẢ KẾT QUẢ VỀ (Return URL)
        // =================================================================
        public ActionResult KetQua()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in Request.QueryString)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                        vnpay.AddResponseData(s, Request.QueryString[s]);
                }

                string maHoaDonFull = vnpay.GetResponseData("vnp_TxnRef");
                string maHoaDon = maHoaDonFull.Split('_')[0];
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        try
                        {
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                            {
                                conn.Open();
                                string sql = @"
                                    DECLARE @MaPhieuDK VARCHAR(20) = (SELECT MaPhieuDK FROM HOADON WHERE MaHD = @MaHD);
                                    UPDATE HOADON SET TrangThaiThanhToan = N'Đã thanh toán', NgayThanhToan = GETDATE(), HinhThucThanhToan = N'Thẻ' WHERE MaHD = @MaHD;
                                    UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD;
                                    UPDATE CT_HOADON_THUOC SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD;
                                    IF @MaPhieuDK IS NOT NULL BEGIN
                                        UPDATE PHIEUDANGKY SET TrangThai = N'Chờ xử lý' WHERE MaPhieuDK = @MaPhieuDK;
                                    END";
                                SqlCommand cmd = new SqlCommand(sql, conn);
                                cmd.Parameters.AddWithValue("@MaHD", maHoaDon);
                                cmd.ExecuteNonQuery();
                            }
                            QL_KhamChuaBenhNgoaiTru.Helpers.InvoiceEmailService.SendInvoicePdfByMaHD(maHoaDon, "Thẻ VNPAY");
                            ViewBag.Message = "Thanh toán thành công hóa đơn: " + maHoaDon;
                            ViewBag.Status = "success";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Thanh toán thành công nhưng có lỗi cập nhật CSDL: " + ex.Message;
                            ViewBag.Status = "error";
                        }
                    }
                    else
                    {
                        // ==========================================================
                        // FIX LỖI GHOST BOOKING: KHÁCH BẤM HỦY HOẶC THẺ LỖI
                        // ==========================================================
                        try
                        {
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                            {
                                conn.Open();
                                string sql = @"
                                    DECLARE @MaPhieuDK VARCHAR(20) = (SELECT MaPhieuDK FROM HOADON WHERE MaHD = @MaHD);
                                    
                                    -- Cập nhật Hóa đơn và Chi tiết dịch vụ thành Đã hủy
                                    UPDATE HOADON SET TrangThaiThanhToan = N'Đã hủy' WHERE MaHD = @MaHD;
                                    UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Hủy' WHERE MaHD = @MaHD;
                                    
                                    -- Rất Quan Trọng: Cập nhật Phiếu đăng ký thành Hủy để nhả lại Slot
                                    IF @MaPhieuDK IS NOT NULL BEGIN
                                        UPDATE PHIEUDANGKY SET TrangThai = N'Hủy' WHERE MaPhieuDK = @MaPhieuDK;
                                    END";
                                SqlCommand cmd = new SqlCommand(sql, conn);
                                cmd.Parameters.AddWithValue("@MaHD", maHoaDon);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch { /* Bỏ qua lỗi nếu việc cập nhật thất bại */ }

                        ViewBag.Message = "Giao dịch không thành công. Bạn đã hủy thanh toán hoặc thẻ bị lỗi. Hệ thống đã tự động hủy lịch đặt của bạn để nhường chỗ cho người khác.";
                        ViewBag.Status = "error";
                    }
                }
                else
                {
                    ViewBag.Message = "Lỗi bảo mật: Sai chữ ký xác thực!";
                    ViewBag.Status = "error";
                }
            }
            return View();
        }
    }
}