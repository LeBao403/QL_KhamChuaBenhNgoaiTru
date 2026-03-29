using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class HoaDonDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. LẤY DANH SÁCH HÓA ĐƠN (Lọc theo khoảng ngày)
        public DataTable GetDanhSachHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                SELECT 
                    hd.MaHD, hd.NgayThanhToan, hd.TongTienGoc, hd.TongTienBHYTChiTra, hd.TongTienBenhNhanTra, hd.TrangThaiThanhToan,
                    bn.MaBN, bn.HoTen, bn.SDT
                FROM HOADON hd
                JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
                WHERE CAST(hd.NgayThanhToan AS DATE) >= CAST(@TuNgay AS DATE)
                  AND CAST(hd.NgayThanhToan AS DATE) <= CAST(@DenNgay AS DATE)
                ORDER BY hd.NgayThanhToan DESC"; // Ưu tiên Hóa đơn mới nhất lên đầu

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. LẤY THÔNG TIN CHUNG CỦA 1 HÓA ĐƠN (Dùng cho trang Detail)
        public DataTable GetHoaDonById(int maHD)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                SELECT hd.*, bn.HoTen, bn.SDT, bn.DiaChi, bn.NgaySinh, bn.GioiTinh, pkb.STT AS STTKham
                FROM HOADON hd
                JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
                LEFT JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                WHERE hd.MaHD = @MaHD";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. LẤY CHI TIẾT CÁC DỊCH VỤ TRONG HÓA ĐƠN
        public DataTable GetChiTietDichVu(int maHD)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                SELECT ct.MaDV, ct.DonGia, ct.TongTienGoc, ct.TienBHYTChiTra, ct.TienBenhNhanTra, 
                       ISNULL(dv.TenDV, CASE WHEN ct.MaDV = 'DV999' THEN N'Phụ thu phí đặt lịch Online' ELSE N'Dịch vụ khác' END) AS TenDV
                FROM CT_HOADON_DV ct
                LEFT JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @MaHD";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}