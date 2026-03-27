using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class ThuNganDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. Lấy danh sách bệnh nhân đang nợ tiền (Chưa thanh toán)
        public DataTable GetDanhSachChoThuTien()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                // Lấy những hóa đơn Chưa thanh toán trong ngày hôm nay
                string sql = @"
                    SELECT hd.MaHD, hd.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.BHYT,
                           hd.TongTienBenhNhanTra, hd.NgayThanhToan, pkb.MaPhieuKhamBenh, pkb.TrangThai
                    FROM HOADON hd
                    JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
                    JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    WHERE hd.TrangThaiThanhToan = N'Chưa thanh toán' 
                      AND CAST(hd.NgayThanhToan AS DATE) = CAST(GETDATE() AS DATE)
                    ORDER BY hd.MaHD ASC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. Xem chi tiết các dịch vụ trong 1 Hóa Đơn
        public DataTable GetChiTietHoaDon(int maHD)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                // Lấy chi tiết chưa thanh toán (Không có cột SoLuong vì đã bỏ như bác dặn)
                string sql = @"
                    SELECT ct.MaDV, dv.TenDV, ct.DonGia, ct.TongTienGoc, ct.TienBHYTChiTra, ct.TienBenhNhanTra
                    FROM CT_HOADON_DV ct
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    WHERE ct.MaHD = @MaHD AND ct.TrangThaiThanhToan = N'Chưa thanh toán'";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. XÁC NHẬN THU TIỀN (Chốt sổ)
        public bool XacNhanThuTien(int maHD, int maPhieuKhamBenh, out string message)
        {
            message = "";
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 3.1. Cập nhật Hóa Đơn Tổng
                        string updateHD = "UPDATE HOADON SET TrangThaiThanhToan = N'Đã thanh toán', NgayThanhToan = GETDATE() WHERE MaHD = @MaHD";
                        SqlCommand cmdHD = new SqlCommand(updateHD, con, trans);
                        cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                        cmdHD.ExecuteNonQuery();

                        // 3.2. Cập nhật Chi tiết Hóa Đơn Dịch Vụ
                        string updateCTHD = "UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD";
                        SqlCommand cmdCTHD = new SqlCommand(updateCTHD, con, trans);
                        cmdCTHD.Parameters.AddWithValue("@MaHD", maHD);
                        cmdCTHD.ExecuteNonQuery();

                        // 3.3. Móc nối quy trình: Kiểm tra xem đang ở khâu nào để búng trạng thái cho đúng
                        string checkPKB = "SELECT TrangThai FROM PHIEUKHAMBENH WHERE MaPhieuKhamBenh = @MaPKB";
                        SqlCommand cmdCheck = new SqlCommand(checkPKB, con, trans);
                        cmdCheck.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                        string trangThaiHienTai = cmdCheck.ExecuteScalar()?.ToString();

                        // Nếu là thu tiền Công Khám Lần 1 (Tiếp tân đẩy qua) -> Đổi thành "Chờ cấp số"
                        if (trangThaiHienTai == "Chờ thanh toán")
                        {
                            string updatePKB = "UPDATE PHIEUKHAMBENH SET TrangThai = N'Chờ cấp số' WHERE MaPhieuKhamBenh = @MaPKB";
                            SqlCommand cmdPKB = new SqlCommand(updatePKB, con, trans);
                            cmdPKB.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            cmdPKB.ExecuteNonQuery();
                        }
                        // (Sau này nếu thu tiền Cận Lâm Sàng thì thêm IF else ở đây)

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        message = ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}