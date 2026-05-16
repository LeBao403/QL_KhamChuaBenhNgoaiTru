using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Helpers
{
    public static class InvoiceEmailService
    {
        public static bool SendInvoicePdfByMaHD(string maHD, string hinhThucThanhToan, DataTable chiTiet = null)
        {
            if (string.IsNullOrWhiteSpace(maHD)) return false;

            var patient = GetInvoicePatient(maHD);
            if (patient == null || string.IsNullOrWhiteSpace(patient.Email)) return false;

            if (chiTiet == null)
            {
                var thuNganDb = new ThuNganDB();
                chiTiet = thuNganDb.GetChiTietHoaDon(maHD);
            }

            var pdfData = InvoicePdfHelper.FromDataTable(
                maHD,
                patient.MaBN,
                patient.HoTen,
                patient.Email,
                string.IsNullOrWhiteSpace(hinhThucThanhToan) ? patient.HinhThucThanhToan : hinhThucThanhToan,
                chiTiet);

            string pdfPath = InvoicePdfHelper.CreateInvoicePdf(pdfData);
            decimal tongTien = 0;
            foreach (var item in pdfData.Items)
            {
                tongTien += item.TienBenhNhanTra;
            }

            return EmailHelper.SendInvoiceEmail(patient.Email, patient.HoTen, maHD, "", tongTien, pdfPath);
        }

        private static InvoicePatient GetInvoicePatient(string maHD)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                string sql = @"
                    SELECT TOP 1 bn.MaBN, bn.HoTen, bn.Email, hd.HinhThucThanhToan
                    FROM HOADON hd
                    JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
                    WHERE hd.MaHD = @MaHD";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return null;
                        return new InvoicePatient
                        {
                            MaBN = rd["MaBN"] == DBNull.Value ? "" : rd["MaBN"].ToString(),
                            HoTen = rd["HoTen"] == DBNull.Value ? "" : rd["HoTen"].ToString(),
                            Email = rd["Email"] == DBNull.Value ? "" : rd["Email"].ToString(),
                            HinhThucThanhToan = rd["HinhThucThanhToan"] == DBNull.Value ? "" : rd["HinhThucThanhToan"].ToString()
                        };
                    }
                }
            }
        }

        private class InvoicePatient
        {
            public string MaBN { get; set; }
            public string HoTen { get; set; }
            public string Email { get; set; }
            public string HinhThucThanhToan { get; set; }
        }
    }
}
