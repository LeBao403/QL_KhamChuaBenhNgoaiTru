using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace QL_KhamChuaBenhNgoaiTru.Helpers
{
    public static class EmailHelper
    {
        private static readonly string LogPath = Path.Combine(Path.GetTempPath(), "log_email.txt");

        private static void WriteLog(string message)
        {
            try
            {
                File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}{Environment.NewLine}");
            }
            catch { }
        }

        public static bool SendEmail(string toEmail, string subject, string body)
        {
            return SendEmail(toEmail, subject, body, null);
        }

        public static bool SendEmail(string toEmail, string subject, string body, IEnumerable<string> attachmentPaths)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    WriteLog("Bo qua gui email vi dia chi nguoi nhan rong.");
                    return false;
                }

                string smtpHost = (ConfigurationManager.AppSettings["SmtpHost"] ?? "").Trim();
                int smtpPort = int.Parse((ConfigurationManager.AppSettings["SmtpPort"] ?? "587").Trim());
                string smtpEmail = (ConfigurationManager.AppSettings["SmtpEmail"] ?? "").Trim();
                string smtpPassword = (ConfigurationManager.AppSettings["SmtpPassword"] ?? "").Trim();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpEmail, "Phòng Khám Đa Khoa");
                    mail.To.Add(toEmail.Trim());
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    if (attachmentPaths != null)
                    {
                        foreach (string attachmentPath in attachmentPaths)
                        {
                            if (!string.IsNullOrWhiteSpace(attachmentPath) && File.Exists(attachmentPath))
                            {
                                mail.Attachments.Add(new Attachment(attachmentPath));
                            }
                        }
                    }

                    using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                }
                WriteLog($"Gui email thanh cong toi {toEmail.Trim()} - {subject}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi gửi email: " + ex.Message);
                WriteLog($"Loi gui email toi {toEmail}: {ex}");
                return false;
            }
        }

        public static bool SendOTP(string toEmail, string otp)
        {
            string subject = "Mã xác thực đăng ký tài khoản (OTP)";
            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #007bff;'>Xác thực địa chỉ Email</h2>
                    <p>Xin chào,</p>
                    <p>Bạn đang đăng ký tài khoản trên hệ thống của chúng tôi. Vui lòng sử dụng mã OTP dưới đây để hoàn tất quá trình xác thực:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; font-size: 24px; font-weight: bold; text-align: center; letter-spacing: 5px; color: #d9534f; border-radius: 5px; margin: 20px 0;'>
                        {otp}
                    </div>
                    <p>Mã này sẽ hết hạn trong vòng 5 phút.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                    <hr style='border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #999;'>Phòng khám đa khoa chân thành cảm ơn.</p>
                </div>
            ";
            return SendEmail(toEmail, subject, body);
        }

        public static bool SendInvoiceEmail(string toEmail, string patientName, string maHD, string htmlChiTiet, decimal tongTien)
        {
            string subject = $"Hóa đơn điện tử - Mã Hóa Đơn: {maHD}";
            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 8px;'>
                    <div style='text-align: center; border-bottom: 2px solid #007bff; padding-bottom: 10px; margin-bottom: 20px;'>
                        <h2 style='color: #007bff; margin: 0;'>PHÒNG KHÁM ĐA KHOA</h2>
                        <p style='margin: 5px 0; color: #555;'>Hóa Đơn Thanh Toán Dịch Vụ</p>
                    </div>
                    
                    <p>Xin chào <strong>{patientName}</strong>,</p>
                    <p>Cảm ơn bạn đã sử dụng dịch vụ tại phòng khám. Dưới đây là chi tiết hóa đơn của bạn:</p>
                    
                    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin-bottom: 20px;'>
                        <p style='margin: 0;'><strong>Mã hóa đơn:</strong> {maHD}</p>
                        <p style='margin: 5px 0 0 0;'><strong>Ngày thanh toán:</strong> {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>
                    </div>

                    <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                        <thead>
                            <tr style='background-color: #007bff; color: white;'>
                                <th style='padding: 10px; text-align: left; border: 1px solid #ddd;'>Dịch vụ/Thuốc</th>
                                <th style='padding: 10px; text-align: center; border: 1px solid #ddd;'>SL</th>
                                <th style='padding: 10px; text-align: right; border: 1px solid #ddd;'>Thành tiền</th>
                            </tr>
                        </thead>
                        <tbody>
                            {htmlChiTiet}
                        </tbody>
                        <tfoot>
                            <tr>
                                <th colspan='2' style='padding: 10px; text-align: right; border: 1px solid #ddd;'>Tổng thanh toán:</th>
                                <th style='padding: 10px; text-align: right; border: 1px solid #ddd; color: #d9534f; font-size: 18px;'>{tongTien:N0} VNĐ</th>
                            </tr>
                        </tfoot>
                    </table>

                    <p style='font-size: 14px;'>Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi qua số điện thoại hỗ trợ hoặc trả lời email này.</p>
                    
                    <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #999;'>
                        <p style='margin: 0;'>Email này được gửi tự động từ hệ thống Quản lý Phòng Khám.</p>
                        <p style='margin: 5px 0 0 0;'>Vui lòng không cung cấp mã thanh toán cho bất kỳ ai.</p>
                    </div>
                </div>
            ";
            return SendEmail(toEmail, subject, body);
        }

        public static bool SendInvoiceEmail(string toEmail, string patientName, string maHD, string htmlChiTiet, decimal tongTien, string pdfPath)
        {
            string subject = $"Hóa đơn điện tử - Mã hóa đơn: {maHD}";
            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #007bff; margin-top: 0;'>Hóa đơn thanh toán</h2>
                    <p>Xin chào <strong>{patientName}</strong>,</p>
                    <p>Phòng khám gửi bạn hóa đơn điện tử cho giao dịch <strong>{maHD}</strong>.</p>
                    <p>Tổng thanh toán: <strong style='color:#d9534f;'>{tongTien:N0} VNĐ</strong></p>
                    <p>Vui lòng xem file PDF đính kèm để biết chi tiết.</p>
                    <hr style='border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #777;'>Email này được gửi tự động từ hệ thống Quản lý Phòng Khám.</p>
                </div>";

            return SendEmail(toEmail, subject, body, new[] { pdfPath });
        }
    }
}
