using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class ThuNganDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. LẤY DANH SÁCH BỆNH NHÂN ĐANG NỢ TIỀN (Chưa thanh toán hoặc Thanh toán 1 phần)
        public DataTable GetDanhSachChoThuTien()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                // LOGIC: Lấy các Hóa đơn còn nợ và CHỈ SUM(TienBenhNhanTra) ở các chi tiết chưa thanh toán
                string sql = @"
    SELECT * FROM (
        SELECT hd.MaHD, hd.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.BHYT,
               hd.NgayThanhToan, hd.GhiChu, 
               pkb.MaPhieuKhamBenh, pkb.TrangThai,
               (
                   ISNULL((SELECT SUM(TienBenhNhanTra) FROM CT_HOADON_DV WHERE MaHD = hd.MaHD AND TrangThaiThanhToan = N'Chưa thanh toán'), 0) +
                   ISNULL((SELECT SUM(TienBenhNhanTra) FROM CT_HOADON_THUOC WHERE MaHD = hd.MaHD AND TrangThaiThanhToan = N'Chưa thanh toán'), 0)
               ) AS TongTienBenhNhanTra
        FROM HOADON hd
        JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
        JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
        
        -- SỬA Ở ĐÂY: Quét cả 2 trạng thái còn nợ tiền
        WHERE hd.TrangThaiThanhToan IN (N'Chưa thanh toán', N'Thanh toán 1 phần') 
          AND CAST(hd.NgayThanhToan AS DATE) = CAST(GETDATE() AS DATE)
    ) AS DanhSachNo
    WHERE TongTienBenhNhanTra > 0
    ORDER BY MaHD ASC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. LẤY CHI TIẾT TỔNG HỢP (Móc thêm liều lượng Sáng/Trưa/Chiều/Tối)
        public DataTable GetChiTietHoaDon(int maHD)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT ct.MaCTHD, 'DV' AS LoaiItem, ct.MaDV AS MaItem, dv.TenDV, 
                           ct.DonGia, ct.TongTienGoc, ct.TienBHYTChiTra, ct.TienBenhNhanTra, ct.TrangThaiThanhToan, 
                           1 AS SoNgayDung, 1 AS SoLuong, 1 AS SoLuongGoc,
                           CAST(0 AS DECIMAL(5,2)) AS SoLuongSang, CAST(0 AS DECIMAL(5,2)) AS SoLuongTrua, 
                           CAST(0 AS DECIMAL(5,2)) AS SoLuongChieu, CAST(0 AS DECIMAL(5,2)) AS SoLuongToi
                    FROM CT_HOADON_DV ct
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    JOIN HOADON hd ON ct.MaHD = hd.MaHD
                    WHERE hd.MaHD = @MaHD AND ct.TrangThaiThanhToan IN (N'Chưa thanh toán', N'Đã thanh toán', N'Hủy')
    
                    UNION ALL
    
                    SELECT ctt.MaCTHD, 'THUOC' AS LoaiItem, t.MaThuoc AS MaItem, t.TenThuoc AS TenDV, 
                           t.GiaBan AS DonGia, ctt.TongTienGoc, ctt.TienBHYTChiTra, ctt.TienBenhNhanTra, ctt.TrangThaiThanhToan, 
                           ctdt.SoNgayDung, ctt.SoLuong, ctdt.SoLuong AS SoLuongGoc,
                           ctdt.SoLuongSang, ctdt.SoLuongTrua, ctdt.SoLuongChieu, ctdt.SoLuongToi
                    FROM CT_HOADON_THUOC ctt
                    JOIN CT_DON_THUOC ctdt ON ctt.MaCTDonThuoc = ctdt.MaCTDonThuoc
                    JOIN THUOC t ON ctdt.MaThuoc = t.MaThuoc
                    JOIN HOADON hd ON ctt.MaHD = hd.MaHD
                    WHERE hd.MaHD = @MaHD AND ctt.TrangThaiThanhToan IN (N'Chưa thanh toán', N'Đã thanh toán', N'Hủy')";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. XÁC NHẬN THU TIỀN (Có xử lý Hủy món, Cập nhật Số lượng thuốc khách mua, Tính lại tiền)
        // Tham so moi dsThuocCapNhat: Danh sach cac mon thuoc ma khach muon doi so luong mua.
        // Moi item la 1 object chua { MaCTHD, SoLuongMoi }
        public bool XacNhanThuTien(int maHD, int maPhieuKhamBenh, string phuongThucTT, string dsHuyDV, string dsHuyThuoc, List<dynamic> dsThuocCapNhat, out string message)
        {
            message = "";
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 3.1. Hủy các Dịch vụ khách không lấy
                        if (!string.IsNullOrEmpty(dsHuyDV))
                        {
                            string sqlHuyDV = $"UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Hủy', TongTienGoc = 0, TienBHYTChiTra = 0, TienBenhNhanTra = 0 WHERE MaHD = @MaHD AND MaCTHD IN (SELECT value FROM STRING_SPLIT(@ds, ','))";
                            SqlCommand cmdHuyDV = new SqlCommand(sqlHuyDV, con, trans);
                            cmdHuyDV.Parameters.AddWithValue("@MaHD", maHD);
                            cmdHuyDV.Parameters.AddWithValue("@ds", dsHuyDV);
                            cmdHuyDV.ExecuteNonQuery();
                        }

                        // 3.2. Hủy các loại Thuốc khách không lấy
                        if (!string.IsNullOrEmpty(dsHuyThuoc))
                        {
                            string sqlHuyThuoc = $"UPDATE CT_HOADON_THUOC SET TrangThaiThanhToan = N'Hủy', TongTienGoc = 0, TienBHYTChiTra = 0, TienBenhNhanTra = 0 WHERE MaHD = @MaHD AND MaCTHD IN (SELECT value FROM STRING_SPLIT(@ds, ','))";
                            SqlCommand cmdHuyThuoc = new SqlCommand(sqlHuyThuoc, con, trans);
                            cmdHuyThuoc.Parameters.AddWithValue("@MaHD", maHD);
                            cmdHuyThuoc.Parameters.AddWithValue("@ds", dsHuyThuoc);
                            cmdHuyThuoc.ExecuteNonQuery();
                        }

                        // 3.3. CẬP NHẬT LẠI SỐ LƯỢNG THUỐC & TÍNH LẠI TIỀN THEO YÊU CẦU KHÁCH
                        if (dsThuocCapNhat != null && dsThuocCapNhat.Count > 0)
                        {
                            // Đọc lại thông tin Hóa Đơn để lấy % BHYT
                            string sqlInfo = @"
                                SELECT bn.BHYT, bn.MucHuongBHYT 
                                FROM HOADON hd 
                                JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN 
                                WHERE hd.MaHD = @MaHD";
                            SqlCommand cmdInfo = new SqlCommand(sqlInfo, con, trans);
                            cmdInfo.Parameters.AddWithValue("@MaHD", maHD);
                            bool hasBHYT = false;
                            int mucHuong = 0;
                            using (SqlDataReader dr = cmdInfo.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    hasBHYT = Convert.ToBoolean(dr["BHYT"]);
                                    mucHuong = dr["MucHuongBHYT"] != DBNull.Value ? Convert.ToInt32(dr["MucHuongBHYT"]) : 0;
                                }
                            }

                            foreach (var thuoc in dsThuocCapNhat)
                            {
                                int maCTHD = Convert.ToInt32(thuoc.MaCTHD);
                                int soLuongMoi = Convert.ToInt32(thuoc.SoLuongMoi);

                                // Lấy giá bán gốc và xem thuốc này có BHYT gánh không
                                string sqlGia = @"
                                    SELECT t.GiaBan, t.CoBHYT 
                                    FROM CT_HOADON_THUOC cht
                                    JOIN CT_DON_THUOC cdt ON cht.MaCTDonThuoc = cdt.MaCTDonThuoc
                                    JOIN THUOC t ON cdt.MaThuoc = t.MaThuoc
                                    WHERE cht.MaCTHD = @MaCTHD";
                                SqlCommand cmdGia = new SqlCommand(sqlGia, con, trans);
                                cmdGia.Parameters.AddWithValue("@MaCTHD", maCTHD);

                                decimal donGia = 0;
                                bool coBHYT = false;
                                using (SqlDataReader drGia = cmdGia.ExecuteReader())
                                {
                                    if (drGia.Read())
                                    {
                                        donGia = Convert.ToDecimal(drGia["GiaBan"]);
                                        coBHYT = Convert.ToBoolean(drGia["CoBHYT"]);
                                    }
                                }

                                // Tính lại tiền
                                decimal tienGocMoi = donGia * soLuongMoi;
                                decimal tienBHYTMoi = 0;
                                decimal tienKhachMoi = tienGocMoi;

                                if (hasBHYT && coBHYT)
                                {
                                    tienBHYTMoi = tienGocMoi * mucHuong / 100;
                                    tienKhachMoi = tienGocMoi - tienBHYTMoi;
                                }

                                // Update lại database cho dòng chi tiết thuốc đó
                                string sqlUpdateCTThuoc = @"
                                    UPDATE CT_HOADON_THUOC 
                                    SET SoLuong = @SL, TongTienGoc = @Goc, TienBHYTChiTra = @BHYT, TienBenhNhanTra = @BN
                                    WHERE MaCTHD = @MaCTHD AND TrangThaiThanhToan != N'Hủy'";
                                SqlCommand cmdUpdateThuoc = new SqlCommand(sqlUpdateCTThuoc, con, trans);
                                cmdUpdateThuoc.Parameters.AddWithValue("@SL", soLuongMoi);
                                cmdUpdateThuoc.Parameters.AddWithValue("@Goc", tienGocMoi);
                                cmdUpdateThuoc.Parameters.AddWithValue("@BHYT", tienBHYTMoi);
                                cmdUpdateThuoc.Parameters.AddWithValue("@BN", tienKhachMoi);
                                cmdUpdateThuoc.Parameters.AddWithValue("@MaCTHD", maCTHD);
                                cmdUpdateThuoc.ExecuteNonQuery();
                            }
                        }

                        // 3.4. Xác nhận "Đã thanh toán"
                        SqlCommand cmdThanhToanDV = new SqlCommand("UPDATE CT_HOADON_DV SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD AND TrangThaiThanhToan = N'Chưa thanh toán'", con, trans);
                        cmdThanhToanDV.Parameters.AddWithValue("@MaHD", maHD); cmdThanhToanDV.ExecuteNonQuery();

                        SqlCommand cmdThanhToanThuoc = new SqlCommand("UPDATE CT_HOADON_THUOC SET TrangThaiThanhToan = N'Đã thanh toán' WHERE MaHD = @MaHD AND TrangThaiThanhToan = N'Chưa thanh toán'", con, trans);
                        cmdThanhToanThuoc.Parameters.AddWithValue("@MaHD", maHD); cmdThanhToanThuoc.ExecuteNonQuery();

                        // 3.5. Cập nhật lại TỔNG TIỀN cho Hóa Đơn (Tự động Sum từ chi tiết lên)
                        string updateHD = @"
                UPDATE hd
                SET TongTienGoc = ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0),
                    TongTienBHYTChiTra = ISNULL(dv.TongBHYT, 0) + ISNULL(th.TongBHYT, 0),
                    TongTienBenhNhanTra = ISNULL(dv.TongBN, 0) + ISNULL(th.TongBN, 0),
                    TrangThaiThanhToan = CASE WHEN ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0) = 0 THEN N'Đã hủy' ELSE N'Đã thanh toán' END,
                    NgayThanhToan = GETDATE(), 
                    HinhThucThanhToan = CASE WHEN @PhuongThuc IN (N'Tiền mặt', N'Chuyển khoản', N'Thẻ') THEN @PhuongThuc ELSE hd.HinhThucThanhToan END
                FROM HOADON hd
                LEFT JOIN (SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN FROM CT_HOADON_DV WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD) dv ON hd.MaHD = dv.MaHD
                LEFT JOIN (SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN FROM CT_HOADON_THUOC WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD) th ON hd.MaHD = th.MaHD
                WHERE hd.MaHD = @MaHD";

                        SqlCommand cmdHD = new SqlCommand(updateHD, con, trans);
                        cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                        cmdHD.Parameters.AddWithValue("@PhuongThuc", phuongThucTT ?? "");
                        cmdHD.ExecuteNonQuery();

                        // =========================================================================
                        // 3.6. LOGIC PHÂN PHÒNG PHÁT THUỐC (Mã phòng 22 -> 26)
                        // =========================================================================
                        string sqlKiemTraThuoc = @"
                    SELECT DISTINCT cdt.MaDonThuoc 
                    FROM CT_HOADON_THUOC cht
                    JOIN CT_DON_THUOC cdt ON cht.MaCTDonThuoc = cdt.MaCTDonThuoc
                    WHERE cht.MaHD = @MaHD AND cht.TrangThaiThanhToan = N'Đã thanh toán'";

                        List<int> danhSachDonThuoc = new List<int>();
                        using (SqlCommand cmdKiemTraThuoc = new SqlCommand(sqlKiemTraThuoc, con, trans))
                        {
                            cmdKiemTraThuoc.Parameters.AddWithValue("@MaHD", maHD);
                            using (SqlDataReader dr = cmdKiemTraThuoc.ExecuteReader())
                            {
                                while (dr.Read()) danhSachDonThuoc.Add(Convert.ToInt32(dr["MaDonThuoc"]));
                            }
                        }

                        if (danhSachDonThuoc.Count > 0)
                        {
                            string sqlGetOrder = "SELECT COUNT(*) FROM PHIEU_PHAT_THUOC WHERE CAST(NgayPhat AS DATE) = CAST(GETDATE() AS DATE)";
                            int totalToday = 0;
                            using (SqlCommand cmdOrder = new SqlCommand(sqlGetOrder, con, trans))
                            {
                                totalToday = (int)cmdOrder.ExecuteScalar();
                            }

                            int targetRoomId = 22 + (totalToday % 5);

                            foreach (int maDT in danhSachDonThuoc)
                            {
                                string checkTonTai = "SELECT COUNT(*) FROM PHIEU_PHAT_THUOC WHERE MaHD = @MaHD AND MaDonThuoc = @MaDT";
                                using (SqlCommand cmdCheck = new SqlCommand(checkTonTai, con, trans))
                                {
                                    cmdCheck.Parameters.AddWithValue("@MaHD", maHD);
                                    cmdCheck.Parameters.AddWithValue("@MaDT", maDT);
                                    if ((int)cmdCheck.ExecuteScalar() == 0)
                                    {
                                        string insertPhieuPhat = @"
                                    INSERT INTO PHIEU_PHAT_THUOC (MaDonThuoc, MaHD, MaPhong, NgayPhat, TrangThai)
                                    VALUES (@MaDT, @MaHD, @MaPhong, GETDATE(), N'Hoàn thành')";

                                        using (SqlCommand cmdInsertPP = new SqlCommand(insertPhieuPhat, con, trans))
                                        {
                                            cmdInsertPP.Parameters.AddWithValue("@MaDT", maDT);
                                            cmdInsertPP.Parameters.AddWithValue("@MaHD", maHD);
                                            cmdInsertPP.Parameters.AddWithValue("@MaPhong", targetRoomId);
                                            cmdInsertPP.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        // 3.7. Đồng bộ trạng thái Cận Lâm Sàng
                        SqlCommand cmdUpdatePCD = new SqlCommand(@"
                            UPDATE pc
                            SET pc.TrangThai = N'Đã thanh toán'
                            FROM PHIEU_CHIDINH pc
                            WHERE pc.MaPhieuKhamBenh = @MaPKB
                              AND pc.TrangThai = N'Chưa thanh toán'
                              AND EXISTS (
                                  SELECT 1
                                  FROM CHITIET_CHIDINH ct
                                  JOIN HOADON hd ON hd.MaPhieuKhamBenh = pc.MaPhieuKhamBenh
                                  JOIN CT_HOADON_DV dv ON dv.MaHD = hd.MaHD
                                  WHERE ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                                    AND dv.MaDV = ct.MaDV
                                    AND dv.TrangThaiThanhToan = N'Đã thanh toán'
                              )", con, trans);
                        cmdUpdatePCD.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                        cmdUpdatePCD.ExecuteNonQuery();

                        // 3.8. Cập nhật trạng thái phiếu khám bệnh
                        bool hasPendingCls = false;
                        using (SqlCommand cmdCheckCLS = new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM PHIEU_CHIDINH pc
                            JOIN CHITIET_CHIDINH ct ON pc.MaPhieuChiDinh = ct.MaPhieuChiDinh
                            WHERE pc.MaPhieuKhamBenh = @MaPKB
                              AND ct.ThoiGianCoKetQua IS NULL
                              AND EXISTS (
                                  SELECT 1
                                  FROM HOADON hd
                                  JOIN CT_HOADON_DV dv ON dv.MaHD = hd.MaHD
                                  WHERE hd.MaPhieuKhamBenh = pc.MaPhieuKhamBenh
                                    AND dv.MaDV = ct.MaDV
                                    AND dv.TrangThaiThanhToan = N'Đã thanh toán'
                              )", con, trans))
                        {
                            cmdCheckCLS.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            hasPendingCls = Convert.ToInt32(cmdCheckCLS.ExecuteScalar()) > 0;
                        }

                        if (hasPendingCls)
                        {
                            SqlCommand cmdPKB = new SqlCommand(@"
                                UPDATE PHIEUKHAMBENH
                                SET TrangThai = N'Chờ cận lâm sàng'
                                WHERE MaPhieuKhamBenh = @MaPKB", con, trans);
                            cmdPKB.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            cmdPKB.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand cmdPKB = new SqlCommand(@"
                                UPDATE PHIEUKHAMBENH
                                SET TrangThai = N'Chờ cấp số'
                                WHERE MaPhieuKhamBenh = @MaPKB
                                  AND TrangThai = N'Chờ thanh toán'", con, trans);
                            cmdPKB.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            cmdPKB.ExecuteNonQuery();
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
            LEFT JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
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
