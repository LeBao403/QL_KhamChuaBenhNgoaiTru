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

        // 1. LẤY DANH SÁCH BỆNH NHÂN ĐANG NỢ TIỀN (Chưa thanh toán)
        public DataTable GetDanhSachChoThuTien()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                // Bổ sung thêm hd.GhiChu để Thu Ngân biết là đang thu tiền Khám, CLS hay Thuốc
                string sql = @"
                    SELECT hd.MaHD, hd.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.BHYT,
                           hd.TongTienBenhNhanTra, hd.NgayThanhToan, hd.GhiChu, 
                           pkb.MaPhieuKhamBenh, pkb.TrangThai
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

        // 2. LẤY CHI TIẾT TỔNG HỢP THEO LƯỢT KHÁM (Lấy tất cả các Hóa Đơn của lượt khám đó)
        public DataTable GetChiTietHoaDon(int maPKB)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
            SELECT ct.MaCTHD, 'DV' AS LoaiItem, ct.MaDV AS MaItem, dv.TenDV, ct.DonGia, ct.TongTienGoc, ct.TienBHYTChiTra, ct.TienBenhNhanTra, ct.TrangThaiThanhToan
            FROM CT_HOADON_DV ct
            JOIN DICHVU dv ON ct.MaDV = dv.MaDV
            JOIN HOADON hd ON ct.MaHD = hd.MaHD
            WHERE hd.MaPhieuKhamBenh = @MaPKB AND ct.TrangThaiThanhToan IN (N'Chưa thanh toán', N'Đã thanh toán', N'Hủy')
            
            UNION ALL
            
            SELECT ctt.MaCTHD, 'THUOC' AS LoaiItem, t.MaThuoc AS MaItem, t.TenThuoc AS TenDV, t.GiaBan AS DonGia, ctt.TongTienGoc, ctt.TienBHYTChiTra, ctt.TienBenhNhanTra, ctt.TrangThaiThanhToan
            FROM CT_HOADON_THUOC ctt
            JOIN CT_DON_THUOC ctdt ON ctt.MaCTDonThuoc = ctdt.MaCTDonThuoc
            JOIN THUOC t ON ctdt.MaThuoc = t.MaThuoc
            JOIN HOADON hd ON ctt.MaHD = hd.MaHD
            WHERE hd.MaPhieuKhamBenh = @MaPKB AND ctt.TrangThaiThanhToan IN (N'Chưa thanh toán', N'Đã thanh toán', N'Hủy')";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaPKB", maPKB);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. XÁC NHẬN THU TIỀN (Có xử lý Hủy món & Tính lại tiền Hóa Đơn)
        public bool XacNhanThuTien(int maHD, int maPhieuKhamBenh, string phuongThucTT, string dsHuyDV, string dsHuyThuoc, out string message)
        {
            message = "";
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 3.1. Hủy các Dịch vụ khách không lấy (Trừ tiền về 0)
                        if (!string.IsNullOrEmpty(dsHuyDV))
                        {
                            string sqlHuyDV = $"UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Hủy', TongTienGoc = 0, TienBHYTChiTra = 0, TienBenhNhanTra = 0 WHERE MaHD = @MaHD AND MaCTHD IN (SELECT value FROM STRING_SPLIT(@ds, ','))";
                            SqlCommand cmdHuyDV = new SqlCommand(sqlHuyDV, con, trans);
                            cmdHuyDV.Parameters.AddWithValue("@MaHD", maHD);
                            cmdHuyDV.Parameters.AddWithValue("@ds", dsHuyDV);
                            cmdHuyDV.ExecuteNonQuery();
                        }

                        // 3.2. Hủy các loại Thuốc khách không lấy (Trừ tiền về 0)
                        if (!string.IsNullOrEmpty(dsHuyThuoc))
                        {
                            string sqlHuyThuoc = $"UPDATE CT_HOADON_THUOC SET TrangThaiThanhToan = N'Hủy', TongTienGoc = 0, TienBHYTChiTra = 0, TienBenhNhanTra = 0 WHERE MaHD = @MaHD AND MaCTHD IN (SELECT value FROM STRING_SPLIT(@ds, ','))";
                            SqlCommand cmdHuyThuoc = new SqlCommand(sqlHuyThuoc, con, trans);
                            cmdHuyThuoc.Parameters.AddWithValue("@MaHD", maHD);
                            cmdHuyThuoc.Parameters.AddWithValue("@ds", dsHuyThuoc);
                            cmdHuyThuoc.ExecuteNonQuery();
                        }

                        // 3.3. Xác nhận "Đã thanh toán" cho các món còn lại
                        SqlCommand cmdThanhToanDV = new SqlCommand("UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD AND TrangThaiThanhToan = N'Chưa thanh toán'", con, trans);
                        cmdThanhToanDV.Parameters.AddWithValue("@MaHD", maHD); cmdThanhToanDV.ExecuteNonQuery();

                        SqlCommand cmdThanhToanThuoc = new SqlCommand("UPDATE CT_HOADON_THUOC SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD AND TrangThaiThanhToan = N'Chưa thanh toán'", con, trans);
                        cmdThanhToanThuoc.Parameters.AddWithValue("@MaHD", maHD); cmdThanhToanThuoc.ExecuteNonQuery();

                        // 3.4. Cập nhật lại TỔNG TIỀN cho Hóa Đơn (Xử lý thông minh trạng thái 0đ)
                        // 3.4. Cập nhật lại TỔNG TIỀN cho Hóa Đơn
                        string updateHD = @"
                        UPDATE hd
                        SET TongTienGoc = ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0),
                            TongTienBHYTChiTra = ISNULL(dv.TongBHYT, 0) + ISNULL(th.TongBHYT, 0),
                            TongTienBenhNhanTra = ISNULL(dv.TongBN, 0) + ISNULL(th.TongBN, 0),
        
                            -- Nếu tổng tiền sau khi hủy món = 0 thì Hủy bill. Nếu > 0 thì là Đã thanh toán trọn vẹn
                            TrangThaiThanhToan = CASE 
                                                    WHEN ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0) = 0 THEN N'Đã hủy' 
                                                    ELSE N'Đã thanh toán' 
                                                 END,
                             
                            NgayThanhToan = GETDATE(), 
        
                            -- Chỉ update Hình thức TT nếu có thu tiền. Nếu bỏ tick hết (truyền rỗng) thì GIỮ NGUYÊN HTTT cũ
                            HinhThucThanhToan = CASE 
                                                    WHEN @PhuongThuc IN (N'Tiền mặt', N'Chuyển khoản', N'Thẻ') THEN @PhuongThuc 
                                                    ELSE hd.HinhThucThanhToan 
                                                END
                        FROM HOADON hd
                        LEFT JOIN (SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN FROM CT_HOADON_DV WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD) dv ON hd.MaHD = dv.MaHD
                        LEFT JOIN (SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN FROM CT_HOADON_THUOC WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD) th ON hd.MaHD = th.MaHD
                        WHERE hd.MaHD = @MaHD";

                        SqlCommand cmdHD = new SqlCommand(updateHD, con, trans);
                        cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                        cmdHD.Parameters.AddWithValue("@PhuongThuc", phuongThucTT ?? ""); // Truyền vào
                        cmdHD.ExecuteNonQuery();

                        // 3.5. Búng trạng thái Phiếu Khám
                        string checkPKB = "SELECT TrangThai FROM PHIEUKHAMBENH WHERE MaPhieuKhamBenh = @MaPKB";
                        string trangThaiHienTai = new SqlCommand(checkPKB, con, trans) { Parameters = { new SqlParameter("@MaPKB", maPhieuKhamBenh) } }.ExecuteScalar()?.ToString();

                        if (trangThaiHienTai == "Chờ thanh toán")
                        {
                            SqlCommand cmdPKB = new SqlCommand("UPDATE PHIEUKHAMBENH SET TrangThai = N'Chờ cấp số' WHERE MaPhieuKhamBenh = @MaPKB", con, trans);
                            cmdPKB.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh); cmdPKB.ExecuteNonQuery();
                        }

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


        // 4. LẤY LỊCH SỬ THU TIỀN (Đã thanh toán / Đã hủy)
        public DataTable GetLichSuThuTien(DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
            SELECT hd.MaHD, hd.MaBN, bn.HoTen, bn.SDT, 
                   hd.TongTienGoc, hd.TongTienBHYTChiTra, hd.TongTienBenhNhanTra, 
                   hd.NgayThanhToan, hd.GhiChu, hd.TrangThaiThanhToan, hd.HinhThucThanhToan,
                   pkb.MaPhieuKhamBenh
            FROM HOADON hd
            JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
            JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
            WHERE CAST(hd.NgayThanhToan AS DATE) >= CAST(@TuNgay AS DATE)
              AND CAST(hd.NgayThanhToan AS DATE) <= CAST(@DenNgay AS DATE)
              AND hd.TrangThaiThanhToan IN (N'Đã thanh toán', N'Đã hủy')
            ORDER BY hd.NgayThanhToan DESC";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}