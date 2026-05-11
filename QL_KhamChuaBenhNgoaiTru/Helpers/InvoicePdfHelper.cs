using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Helpers
{
    public class InvoicePdfData
    {
        public string MaHD { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string HinhThucThanhToan { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public List<InvoicePdfItem> Items { get; set; } = new List<InvoicePdfItem>();
    }

    public class InvoicePdfItem
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal TongTienGoc { get; set; }
        public decimal TienBHYTChiTra { get; set; }
        public decimal TienBenhNhanTra { get; set; }
    }

    public static class InvoicePdfHelper
    {
        private const int PageWidth = 1240;
        private const int PageHeight = 1754;
        private const int Margin = 90;

        public static string CreateInvoicePdf(InvoicePdfData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            string fileName = $"HoaDon_{SanitizeFileName(data.MaHD)}.pdf";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            byte[] jpgBytes = RenderInvoicePage(data);
            byte[] pdfBytes = BuildSingleImagePdf(jpgBytes, PageWidth, PageHeight);
            File.WriteAllBytes(filePath, pdfBytes);
            return filePath;
        }

        public static InvoicePdfData FromDataTable(string maHD, string maBN, string hoTen, string email, string hinhThucThanhToan, DataTable chiTiet)
        {
            var data = new InvoicePdfData
            {
                MaHD = maHD,
                MaBN = maBN,
                HoTen = hoTen,
                Email = email,
                HinhThucThanhToan = string.IsNullOrWhiteSpace(hinhThucThanhToan) ? "Chuyen khoan" : hinhThucThanhToan,
                NgayThanhToan = DateTime.Now
            };

            if (chiTiet == null) return data;

            foreach (DataRow row in chiTiet.Rows)
            {
                string trangThai = GetString(row, "TrangThaiThanhToan");
                if (trangThai.Equals("Huy", StringComparison.OrdinalIgnoreCase) || trangThai.Contains("Hủy"))
                {
                    continue;
                }

                data.Items.Add(new InvoicePdfItem
                {
                    Ten = GetString(row, "TenDV"),
                    SoLuong = GetInt(row, "SoLuong", 1),
                    DonGia = GetDecimal(row, "DonGia"),
                    TongTienGoc = GetDecimal(row, "TongTienGoc"),
                    TienBHYTChiTra = GetDecimal(row, "TienBHYTChiTra"),
                    TienBenhNhanTra = GetDecimal(row, "TienBenhNhanTra")
                });
            }

            return data;
        }

        private static byte[] RenderInvoicePage(InvoicePdfData data)
        {
            using (var bitmap = new Bitmap(PageWidth, PageHeight))
            using (var g = Graphics.FromImage(bitmap))
            using (var titleFont = new Font("Arial", 30, FontStyle.Bold))
            using (var brandFont = new Font("Arial", 18, FontStyle.Bold))
            using (var normalFont = new Font("Arial", 14))
            using (var smallFont = new Font("Arial", 12))
            using (var boldFont = new Font("Arial", 14, FontStyle.Bold))
            using (var tableHeaderFont = new Font("Arial", 12, FontStyle.Bold))
            {
                g.Clear(Color.White);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var black = Brushes.Black;
                var muted = new SolidBrush(Color.FromArgb(71, 85, 105));
                var blue = new SolidBrush(Color.FromArgb(30, 64, 175));
                var red = new SolidBrush(Color.FromArgb(185, 28, 28));
                var lightBlue = new SolidBrush(Color.FromArgb(239, 246, 255));
                var border = new Pen(Color.FromArgb(148, 163, 184), 1);
                var darkBorder = new Pen(Color.FromArgb(15, 23, 42), 2);

                int y = Margin;

                DrawClinicLogo(g, brandFont, blue, darkBorder, Margin, y, 70, 70);
                g.DrawString("PHÒNG KHÁM ĐA KHOA DIGIMED", brandFont, black, Margin + 88, y + 2);
                g.DrawString("Hệ thống Y tế Thông minh", normalFont, black, Margin + 88, y + 30);
                g.DrawString("Địa chỉ: 786 Nguyễn Kiệm, Quận Gò Vấp, TP.HCM", normalFont, black, Margin + 88, y + 54);

                DrawRight(g, "Mã BN: " + Safe(data.MaBN), boldFont, black, PageWidth - Margin, y + 8);
                DrawRight(g, "Mã hóa đơn: " + Safe(data.MaHD), boldFont, black, PageWidth - Margin, y + 34);

                y += 125;
                DrawCentered(g, "HÓA ĐƠN THANH TOÁN", titleFont, black, new RectangleF(Margin, y, PageWidth - Margin * 2, 42));
                y += 72;

                DrawSectionTitle(g, "THÔNG TIN HÓA ĐƠN", tableHeaderFont, muted, y);
                y += 32;
                DrawInfoRow(g, "Bệnh nhân:", data.HoTen, "Email:", data.Email, normalFont, boldFont, black, muted, y);
                y += 30;
                DrawInfoRow(g, "Ngày thanh toán:", data.NgayThanhToan.ToString("dd/MM/yyyy HH:mm"), "Hình thức:", data.HinhThucThanhToan, normalFont, boldFont, black, muted, y);
                y += 52;

                DrawSectionTitle(g, "CHI TIẾT THANH TOÁN", tableHeaderFont, muted, y);
                y += 32;

                int[] widths = { 55, 455, 90, 140, 140, 150, 170 };
                string[] headers = { "STT", "Dịch vụ/Thuốc", "SL", "Đơn giá", "Tổng gốc", "BHYT trả", "BN trả" };
                int x = Margin;
                int rowH = 38;
                for (int i = 0; i < headers.Length; i++)
                {
                    g.FillRectangle(lightBlue, x, y, widths[i], rowH);
                    g.DrawRectangle(border, x, y, widths[i], rowH);
                    DrawCentered(g, headers[i], tableHeaderFont, blue, new RectangleF(x + 3, y + 9, widths[i] - 6, 18));
                    x += widths[i];
                }
                y += rowH;

                int stt = 1;
                foreach (var item in data.Items.Take(18))
                {
                    x = Margin;
                    rowH = 42;
                    string[] cells =
                    {
                        stt.ToString(),
                        Safe(item.Ten),
                        item.SoLuong.ToString(),
                        Money(item.DonGia),
                        Money(item.TongTienGoc),
                        Money(item.TienBHYTChiTra),
                        Money(item.TienBenhNhanTra)
                    };

                    for (int i = 0; i < cells.Length; i++)
                    {
                        g.DrawRectangle(border, x, y, widths[i], rowH);
                        var rect = new RectangleF(x + 6, y + 8, widths[i] - 12, rowH - 12);
                        if (i == 1)
                            g.DrawString(TrimToFit(cells[i], 52), smallFont, black, rect);
                        else if (i >= 3)
                            DrawRight(g, cells[i], smallFont, black, x + widths[i] - 6, y + 13);
                        else
                            DrawCentered(g, cells[i], smallFont, black, rect);
                        x += widths[i];
                    }
                    y += rowH;
                    stt++;
                }

                if (data.Items.Count > 18)
                {
                    g.DrawString($"... và {data.Items.Count - 18} dòng chi tiết khác", smallFont, muted, Margin + 8, y + 8);
                    y += 34;
                }

                decimal tongGoc = data.Items.Sum(i => i.TongTienGoc);
                decimal tongBhyt = data.Items.Sum(i => i.TienBHYTChiTra);
                decimal tongBn = data.Items.Sum(i => i.TienBenhNhanTra);

                y += 26;
                DrawTotalLine(g, "Tổng tiền gốc:", Money(tongGoc), normalFont, boldFont, black, muted, y);
                y += 30;
                DrawTotalLine(g, "BHYT chi trả:", Money(tongBhyt), normalFont, boldFont, black, muted, y);
                y += 34;
                using (var totalFont = new Font("Arial", 18, FontStyle.Bold))
                {
                    DrawTotalLine(g, "Bệnh nhân thanh toán:", Money(tongBn) + " VNĐ", normalFont, totalFont, red, muted, y);
                }

                y += 72;
                g.DrawString("Ghi chú: Hóa đơn điện tử được phát hành tự động sau khi hệ thống xác nhận thanh toán.", smallFont, muted, Margin, y);

                using (var ms = new MemoryStream())
                {
                    var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    using (var parameters = new EncoderParameters(1))
                    {
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 92L);
                        bitmap.Save(ms, encoder, parameters);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static byte[] BuildSingleImagePdf(byte[] jpgBytes, int width, int height)
        {
            using (var ms = new MemoryStream())
            {
                var offsets = new List<long> { 0 };
                WriteAscii(ms, "%PDF-1.4\n");

                offsets.Add(ms.Position);
                WriteAscii(ms, "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n");
                offsets.Add(ms.Position);
                WriteAscii(ms, "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n");
                offsets.Add(ms.Position);
                WriteAscii(ms, $"3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 {width} {height}] /Resources << /XObject << /Im0 4 0 R >> >> /Contents 5 0 R >> endobj\n");
                offsets.Add(ms.Position);
                WriteAscii(ms, $"4 0 obj << /Type /XObject /Subtype /Image /Width {width} /Height {height} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {jpgBytes.Length} >> stream\n");
                ms.Write(jpgBytes, 0, jpgBytes.Length);
                WriteAscii(ms, "\nendstream endobj\n");
                string content = $"q\n{width} 0 0 {height} 0 0 cm\n/Im0 Do\nQ\n";
                byte[] contentBytes = Encoding.ASCII.GetBytes(content);
                offsets.Add(ms.Position);
                WriteAscii(ms, $"5 0 obj << /Length {contentBytes.Length} >> stream\n");
                ms.Write(contentBytes, 0, contentBytes.Length);
                WriteAscii(ms, "endstream endobj\n");

                long xref = ms.Position;
                WriteAscii(ms, "xref\n0 6\n0000000000 65535 f \n");
                for (int i = 1; i <= 5; i++)
                {
                    WriteAscii(ms, offsets[i].ToString("0000000000", CultureInfo.InvariantCulture) + " 00000 n \n");
                }
                WriteAscii(ms, $"trailer << /Size 6 /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
                return ms.ToArray();
            }
        }

        private static void DrawClinicLogo(Graphics g, Font fallbackFont, Brush fallbackBrush, Pen fallbackBorder, int x, int y, int width, int height)
        {
            string logoPath = GetLogoPath();
            if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
            {
                using (var logo = Image.FromFile(logoPath))
                {
                    Rectangle target = FitImage(logo.Width, logo.Height, new Rectangle(x, y, width, height));
                    g.DrawImage(logo, target);
                }
                return;
            }

            g.DrawRectangle(fallbackBorder, x, y, width, height);
            DrawCentered(g, "DM", fallbackFont, fallbackBrush, new RectangleF(x, y + 19, width, 32));
        }

        private static string GetLogoPath()
        {
            var roots = new List<string>();
            try { AddRoot(roots, HttpRuntime.AppDomainAppPath); } catch { }
            AddRoot(roots, AppDomain.CurrentDomain.BaseDirectory);
            AddRoot(roots, Directory.GetCurrentDirectory());

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(baseDir))
            {
                var dir = new DirectoryInfo(baseDir);
                AddRoot(roots, dir.Parent?.FullName);
                AddRoot(roots, dir.Parent?.Parent?.FullName);
            }

            foreach (string root in roots)
            {
                string path = Path.Combine(root, "Images", "Logo", "app_favicon.png");
                if (File.Exists(path)) return path;

                path = Path.Combine(root, "Images", "Logo", "app_logo.png");
                if (File.Exists(path)) return path;
            }

            return "";
        }

        private static void AddRoot(List<string> roots, string root)
        {
            if (!string.IsNullOrWhiteSpace(root) && !roots.Contains(root))
            {
                roots.Add(root);
            }
        }

        private static Rectangle FitImage(int sourceWidth, int sourceHeight, Rectangle box)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0) return box;

            float ratio = Math.Min((float)box.Width / sourceWidth, (float)box.Height / sourceHeight);
            int width = Math.Max(1, (int)(sourceWidth * ratio));
            int height = Math.Max(1, (int)(sourceHeight * ratio));
            int x = box.X + (box.Width - width) / 2;
            int y = box.Y + (box.Height - height) / 2;
            return new Rectangle(x, y, width, height);
        }

        private static void DrawSectionTitle(Graphics g, string text, Font font, Brush brush, int y)
        {
            g.DrawString(text, font, brush, Margin, y);
            g.DrawLine(new Pen(Color.FromArgb(203, 213, 225), 1), Margin, y + 24, PageWidth - Margin, y + 24);
        }

        private static void DrawInfoRow(Graphics g, string l1, string v1, string l2, string v2, Font normal, Font bold, Brush text, Brush muted, int y)
        {
            g.DrawString(l1, normal, muted, Margin, y);
            g.DrawString(Safe(v1), bold, text, Margin + 150, y);
            g.DrawString(l2, normal, muted, Margin + 575, y);
            g.DrawString(Safe(v2), bold, text, Margin + 690, y);
        }

        private static void DrawTotalLine(Graphics g, string label, string value, Font normal, Font valueFont, Brush valueBrush, Brush muted, int y)
        {
            DrawRight(g, label, normal, muted, PageWidth - Margin - 250, y);
            DrawRight(g, value, valueFont, valueBrush, PageWidth - Margin, y);
        }

        private static void DrawCentered(Graphics g, string text, Font font, Brush brush, RectangleF rect)
        {
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, brush, rect, format);
            }
        }

        private static void DrawRight(Graphics g, string text, Font font, Brush brush, float right, float y)
        {
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, brush, right - size.Width, y);
        }

        private static string Money(decimal value)
        {
            return value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }

        private static string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string TrimToFit(string text, int max)
        {
            text = Safe(text);
            return text.Length <= max ? text : text.Substring(0, max - 3) + "...";
        }

        private static string SanitizeFileName(string value)
        {
            value = Safe(value);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }
            return value;
        }

        private static string GetString(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? row[column].ToString() : "";
        }

        private static int GetInt(DataRow row, string column, int fallback)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return fallback;
            return Convert.ToInt32(row[column]);
        }

        private static decimal GetDecimal(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return 0;
            return Convert.ToDecimal(row[column]);
        }

        private static void WriteAscii(Stream stream, string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
