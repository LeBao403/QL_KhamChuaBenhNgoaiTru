using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class PhatThuocDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. LẤY DANH SÁCH CHỜ PHÁT (CHỈ CHO INDEX - LỌC THEO PHÒNG)
        public List<DanhSachPhatThuocVM> GetDanhSachChoPhat(int maPhong, DateTime tuNgay, DateTime denNgay, string search, string sortCol, string sortDir)
        {
            var list = new List<DanhSachPhatThuocVM>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Viết lại SQL bám sát 100% câu truy vấn của bác
                string sql = @"
        SELECT 
            hd.MaHD,
            dt.MaDonThuoc,
            bn.HoTen + ' (' + bn.MaBN + ')' AS BenhNhan,
            hd.TrangThaiThanhToan AS TTThanhToan,
            dt.TrangThai AS TTPhatThuoc,
            hd.NgayThanhToan
        FROM HOADON hd
        INNER JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
        INNER JOIN DON_THUOC dt ON hd.MaPhieuKhamBenh = dt.MaPhieuKhamBenh
        INNER JOIN PHIEU_PHAT_THUOC ppt ON ppt.MaHD = hd.MaHD AND ppt.MaDonThuoc = dt.MaDonThuoc
        WHERE CAST(hd.NgayThanhToan AS DATE) BETWEEN @TuNgay AND @DenNgay 
          AND ppt.MaPhong = @MaPhong
          AND hd.TrangThaiThanhToan = N'Đã thanh toán'
          AND dt.TrangThai = N'Chưa phát'";

                // Giữ lại logic Search
                if (!string.IsNullOrEmpty(search))
                    sql += " AND (bn.HoTen LIKE @Search OR bn.MaBN LIKE @Search OR hd.MaHD LIKE @Search)";

                // Giữ lại logic Sắp xếp
                string orderBy = sortCol == "BenhNhan" ? "bn.HoTen" : "hd.NgayThanhToan";
                sql += $" ORDER BY {orderBy} {sortDir}";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Gán Parameters chống SQL Injection
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    if (!string.IsNullOrEmpty(search)) cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new DanhSachPhatThuocVM
                            {
                                MaHD = Convert.ToInt32(dr["MaHD"]),
                                MaDonThuoc = Convert.ToInt32(dr["MaDonThuoc"]),
                                BenhNhan = dr["BenhNhan"].ToString(),
                                TTThanhToan = dr["TTThanhToan"].ToString(),
                                TTPhatThuoc = dr["TTPhatThuoc"].ToString(),
                                HasDonThuoc = true,
                                NgayThanhToanStr = Convert.ToDateTime(dr["NgayThanhToan"]).ToString("dd/MM/yyyy HH:mm")
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 2. CHI TIẾT ĐƠN THUỐC (FIX LỖI TRẮNG BẢNG THUỐC VÀ HIỆN LÔ ĐÃ PHÁT BÊN LỊCH SỬ)
        public ThongTinPhatThuocVM GetChiTietDonThuocVaThongTin(int maDonThuoc, int maHD)
        {
            var result = new ThongTinPhatThuocVM { DanhSachThuoc = new List<ChiTietDonThuocVM>() };
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                //    // =========================================================================
                //    // A. THÔNG TIN HÀNH CHÍNH & LÂM SÀNG
                //    // =========================================================================
                //    string sqlInfo = @"
                //SELECT TOP 1 
                //    bn.HoTen, bn.MaBN, bn.SDT, bn.NgaySinh, bn.GioiTinh, bn.DiaChi, bn.BHYT,
                //    (YEAR(GETDATE()) - YEAR(bn.NgaySinh)) AS Tuoi, 
                //    nv.HoTen AS BacSiKe, dt.NgayKe, 
                //    dt.MaDonThuoc, dt.LoiDanBS, 
                //    pkb.MaPhieuKhamBenh, pkb.KetLuan, pkb.TrieuChung
                //FROM DON_THUOC dt 
                //JOIN PHIEUKHAMBENH pkb ON dt.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh 
                //JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN 
                //LEFT JOIN NHANVIEN nv ON pkb.MaBacSiKham = nv.MaNV 
                //WHERE dt.MaDonThuoc = @MaDonThuoc";

                //    using (SqlCommand cmd = new SqlCommand(sqlInfo, conn))
                //    {
                //        cmd.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                //        using (SqlDataReader dr = cmd.ExecuteReader())
                //        {
                //            if (dr.Read())
                //            {
                //                result.TenBN = dr["HoTen"].ToString();
                //                result.MaBN = dr["MaBN"].ToString();
                //                result.GioiTinh = dr["GioiTinh"].ToString();
                //                result.Tuoi = dr["Tuoi"] != DBNull.Value ? Convert.ToInt32(dr["Tuoi"]) : 0;
                //                result.SDT = dr["SDT"].ToString();
                //                result.DiaChi = dr["DiaChi"].ToString();
                //                result.NgaySinh = dr["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySinh"]).ToString("dd/MM/yyyy") : "---";
                //                result.BacSiKe = dr["BacSiKe"].ToString();
                //                result.NgayKe = dr["NgayKe"] != DBNull.Value ? Convert.ToDateTime(dr["NgayKe"]).ToString("dd/MM/yyyy HH:mm") : "---";
                //                result.KetLuan = dr["KetLuan"].ToString();
                //                result.TrieuChung = dr["TrieuChung"].ToString();
                //                result.MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]);
                //                result.LoiDanBacSi = dr["LoiDanBS"].ToString();
                //            }
                //        }
                //    }
                // =========================================================================
                // A. THÔNG TIN HÀNH CHÍNH & LÂM SÀNG
                // =========================================================================
                string sqlInfo = @"
    SELECT TOP 1 
        bn.HoTen, bn.MaBN, bn.SDT, bn.CCCD, bn.Email, bn.NgaySinh, bn.GioiTinh, bn.DiaChi, bn.BHYT,
        (YEAR(GETDATE()) - YEAR(bn.NgaySinh)) AS Tuoi, 
        nv.HoTen AS BacSiKe, dt.NgayKe, 
        dt.MaDonThuoc, dt.LoiDanBS, 
        pkb.MaPhieuKhamBenh, pkb.KetLuan, pkb.TrieuChung,
        ppt.MaPhieuPhat
    FROM DON_THUOC dt 
    JOIN PHIEUKHAMBENH pkb ON dt.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh 
    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN 
    LEFT JOIN NHANVIEN nv ON pkb.MaBacSiKham = nv.MaNV 
    LEFT JOIN PHIEU_PHAT_THUOC ppt ON ppt.MaDonThuoc = dt.MaDonThuoc AND ppt.MaHD = @MaHD
    WHERE dt.MaDonThuoc = @MaDonThuoc";

                using (SqlCommand cmd = new SqlCommand(sqlInfo, conn))
                {
                    cmd.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                    cmd.Parameters.AddWithValue("@MaHD", maHD); // BỔ SUNG DÒNG NÀY

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            result.TenBN = dr["HoTen"].ToString();
                            result.MaBN = dr["MaBN"].ToString();
                            result.GioiTinh = dr["GioiTinh"].ToString();
                            result.Tuoi = dr["Tuoi"] != DBNull.Value ? Convert.ToInt32(dr["Tuoi"]) : 0;
                            result.SDT = dr["SDT"].ToString();
                            result.CCCD = dr["CCCD"].ToString();
                            result.Email = dr["Email"].ToString();
                            result.DiaChi = dr["DiaChi"].ToString();
                            result.NgaySinh = dr["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySinh"]).ToString("dd/MM/yyyy") : "---";
                            result.BacSiKe = dr["BacSiKe"].ToString();
                            result.NgayKe = dr["NgayKe"] != DBNull.Value ? Convert.ToDateTime(dr["NgayKe"]).ToString("dd/MM/yyyy HH:mm") : "---";
                            result.KetLuan = dr["KetLuan"].ToString();
                            result.TrieuChung = dr["TrieuChung"].ToString();
                            result.MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]);
                            result.LoiDanBacSi = dr["LoiDanBS"].ToString();

                            // Lấy mã chứng từ
                            result.MaHD = maHD;
                            result.MaDonThuoc = maDonThuoc;
                            result.MaPhieuPhat = dr["MaPhieuPhat"] != DBNull.Value ? Convert.ToInt32(dr["MaPhieuPhat"]) : (int?)null;
                        }
                    }
                }

                // =========================================================================
                // B. DANH SÁCH THUỐC (Kèm thông tin Lô FEFO dự kiến & Lô ĐÃ PHÁT thực tế)
                // =========================================================================
                string sqlThuoc = @"
            SELECT 
                t.TenThuoc, 
                cdt.SoLuong AS SoLuongKe, 
                ISNULL(cht.SoLuong, 0) AS SoLuongMua,
                ISNULL(cdt.SoLuongDaPhat, 0) AS ThucPhat,
                ISNULL(cht.TrangThaiThanhToan, N'Hủy') AS TrangThai,
                
                -- Số lượng CẦN bốc = Mua - Đã bốc
                CASE 
                    WHEN ISNULL(cht.TrangThaiThanhToan, '') = N'Đã thanh toán' THEN (ISNULL(cht.SoLuong, 0) - ISNULL(cdt.SoLuongDaPhat, 0))
                    ELSE 0 
                END AS SoLuongCanPhat,

                -- Lô dự kiến (FEFO) (Lấy lô còn hạn)
                (SELECT TOP 1 MaLo FROM TONKHO tk WHERE tk.MaThuoc = t.MaThuoc AND tk.SoLuongTon > 0 AND tk.HanSuDung >= CAST(GETDATE() AS DATE) ORDER BY tk.HanSuDung ASC) AS LoDuKien,
                (SELECT TOP 1 HanSuDung FROM TONKHO tk WHERE tk.MaThuoc = t.MaThuoc AND tk.SoLuongTon > 0 AND tk.HanSuDung >= CAST(GETDATE() AS DATE) ORDER BY tk.HanSuDung ASC) AS HSDDuKien,

                -- Danh sách Lô ĐÃ PHÁT THỰC TẾ (Dùng hàm STUFF gộp chuỗi các mã lô từ CT_PHIEU_PHAT)
                ISNULL(STUFF((
                    SELECT ', ' + cp.MaLo + ' (' + CAST(cp.SoLuongPhat AS VARCHAR) + ')'
                    FROM CT_PHIEU_PHAT cp
                    JOIN PHIEU_PHAT_THUOC pp ON cp.MaPhieuPhat = pp.MaPhieuPhat
                    WHERE pp.MaDonThuoc = @MaDonThuoc AND cp.MaThuoc = t.MaThuoc
                    FOR XML PATH('')
                ), 1, 2, ''), '---') AS DanhSachLoDaPhat

            FROM CT_DON_THUOC cdt 
            INNER JOIN THUOC t ON cdt.MaThuoc = t.MaThuoc
            LEFT JOIN CT_HOADON_THUOC cht ON cdt.MaCTDonThuoc = cht.MaCTDonThuoc AND cht.MaHD = @MaHD
            WHERE cdt.MaDonThuoc = @MaDonThuoc";

                using (SqlCommand cmd = new SqlCommand(sqlThuoc, conn))
                {
                    cmd.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.DanhSachThuoc.Add(new ChiTietDonThuocVM
                            {
                                TenThuoc = dr["TenThuoc"].ToString(),
                                SoLuongKe = Convert.ToInt32(dr["SoLuongKe"]),
                                SoLuongMua = Convert.ToInt32(dr["SoLuongMua"]),
                                TrangThai = dr["TrangThai"].ToString(),
                                SoLuongCanPhat = Convert.ToInt32(dr["SoLuongCanPhat"]), // Đã tính toán lại bên SQL

                                LoDuKien = dr["LoDuKien"]?.ToString() ?? "Hết kho",
                                HSDDuKien = dr["HSDDuKien"] != DBNull.Value ? Convert.ToDateTime(dr["HSDDuKien"]).ToString("dd/MM/yyyy") : "---",

                                // Hứng 2 trường mới cho Lịch sử
                                ThucPhat = Convert.ToInt32(dr["ThucPhat"]),
                                DanhSachLoDaPhat = dr["DanhSachLoDaPhat"].ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }

        // ==================================================================================
        // 3. XÁC NHẬN PHÁT THUỐC (Gọi Store Procedure)
        // ==================================================================================
        public (bool IsSuccess, string Message) XacNhanPhatThuoc(int maDonThuoc, int maHD, string maNVPhat, int maPhong)
{
    using (SqlConnection conn = new SqlConnection(connectStr))
    {
        conn.Open();

        // Bắt buộc dùng Transaction: Lỗi 1 bước là Rollback trả lại nguyên trạng, không bị mất thuốc oan
        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            try
            {
                // ==========================================================
                // BƯỚC 1: LẤY DANH SÁCH THUỐC CẦN PHÁT (Đã đóng tiền & chưa phát hết)
                // ==========================================================
                string getThuocSql = @"
                    SELECT 
                        cdt.MaCTDonThuoc,
                        cdt.MaThuoc, 
                        (ISNULL(cht.SoLuong, 0) - ISNULL(cdt.SoLuongDaPhat, 0)) AS SoLuongCanPhat
                    FROM CT_DON_THUOC cdt
                    JOIN CT_HOADON_THUOC cht ON cdt.MaCTDonThuoc = cht.MaCTDonThuoc
                    JOIN HOADON hd ON cht.MaHD = hd.MaHD
                    WHERE cdt.MaDonThuoc = @MaDonThuoc 
                      AND cht.MaHD = @MaHD 
                      AND cht.TrangThaiThanhToan = N'Đã thanh toán'
                      AND hd.TrangThaiThanhToan != N'Đã hủy'
                      AND ISNULL(cdt.SoLuongDaPhat, 0) < ISNULL(cht.SoLuong, 0)";

                var dsThuocCanPhat = new List<(int MaCTDonThuoc, string MaThuoc, int SoLuongCanPhat)>();

                using (SqlCommand cmd = new SqlCommand(getThuocSql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                    cmd.Parameters.AddWithValue("@MaHD", maHD);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dsThuocCanPhat.Add((
                                Convert.ToInt32(reader["MaCTDonThuoc"]),
                                reader["MaThuoc"].ToString(),
                                Convert.ToInt32(reader["SoLuongCanPhat"])
                            ));
                        }
                    }
                }

                if (dsThuocCanPhat.Count == 0)
                {
                    return (false, "Không có thuốc nào cần phát (Có thể đã phát đủ, hóa đơn bị hủy, hoặc chưa đóng tiền).");
                }

                // ==========================================================
                // BƯỚC 2: CẬP NHẬT PHIẾU PHÁT THUỐC (Do Thu Ngân đã tạo sẵn)
                // ==========================================================
                string updatePhieuSql = @"
                    DECLARE @MaPhieuPhat INT = (SELECT TOP 1 MaPhieuPhat FROM PHIEU_PHAT_THUOC WHERE MaDonThuoc = @MaDonThuoc AND MaHD = @MaHD AND TrangThai != N'Đã hủy');
                    
                    IF @MaPhieuPhat IS NULL
                        THROW 50001, N'Không tìm thấy Phiếu xuất kho do Thu ngân tạo!', 1;

                    UPDATE PHIEU_PHAT_THUOC 
                    SET MaNV_Phat = @MaNV_Phat, 
                        MaPhong = @MaPhong, 
                        NgayPhat = GETDATE(),
                        TrangThai = N'Hoàn thành'
                    WHERE MaPhieuPhat = @MaPhieuPhat;

                    SELECT @MaPhieuPhat;"; // Trả về mã phiếu để dùng cho Bước 3

                int maPhieuPhat = 0;
                using (SqlCommand cmdUpdatePhieu = new SqlCommand(updatePhieuSql, conn, trans))
                {
                    cmdUpdatePhieu.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                    cmdUpdatePhieu.Parameters.AddWithValue("@MaHD", maHD);
                    cmdUpdatePhieu.Parameters.AddWithValue("@MaNV_Phat", string.IsNullOrEmpty(maNVPhat) ? DBNull.Value : (object)maNVPhat);
                    cmdUpdatePhieu.Parameters.AddWithValue("@MaPhong", maPhong);

                    maPhieuPhat = Convert.ToInt32(cmdUpdatePhieu.ExecuteScalar());
                }

                // ==========================================================
                // BƯỚC 3: QUÉT TRỪ KHO FEFO VÀ GHI CHI TIẾT THEO TỪNG LÔ
                // ==========================================================
                foreach (var thuoc in dsThuocCanPhat)
                {
                    int soLuongConThieu = thuoc.SoLuongCanPhat;
                    int soLuongDaLayTong = 0; // Tổng số lượng đã lấy cho loại thuốc này

                    // UPDLOCK, ROWLOCK để tránh đụng độ. Bổ sung lấy thêm MaLo.
                    string getKhoSql = @"
                        SELECT MaTonKho, MaLo, SoLuongTon 
                        FROM TONKHO WITH (UPDLOCK, ROWLOCK) 
                        WHERE MaThuoc = @MaThuoc 
                          AND SoLuongTon > 0 
                          AND HanSuDung >= CAST(GETDATE() AS DATE) 
                        ORDER BY HanSuDung ASC";

                    var dsLoThuoc = new List<(int MaTonKho, string MaLo, int SoLuongTon)>();
                    using (SqlCommand cmdKho = new SqlCommand(getKhoSql, conn, trans))
                    {
                        cmdKho.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                        using (SqlDataReader readerKho = cmdKho.ExecuteReader())
                        {
                            while (readerKho.Read())
                            {
                                dsLoThuoc.Add((
                                    Convert.ToInt32(readerKho["MaTonKho"]),
                                    readerKho["MaLo"].ToString(),
                                    Convert.ToInt32(readerKho["SoLuongTon"])
                                ));
                            }
                        }
                    }

                    // Vét từng lô một cho đến khi đủ số lượng
                    foreach (var lo in dsLoThuoc)
                    {
                        if (soLuongConThieu <= 0) break;

                        int soLuongTru = Math.Min(lo.SoLuongTon, soLuongConThieu);

                        // Trừ tồn kho
                        string updateKhoSql = "UPDATE TONKHO SET SoLuongTon = SoLuongTon - @SLTru WHERE MaTonKho = @MaTonKho";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateKhoSql, conn, trans))
                        {
                            cmdUpdate.Parameters.AddWithValue("@SLTru", soLuongTru);
                            cmdUpdate.Parameters.AddWithValue("@MaTonKho", lo.MaTonKho);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        // Ghi Chi Tiết Phiếu Phát (LƯU Ý: Ghi cho từng lô để lưu đúng mã lô)
                        string insertCTPhieuSql = "INSERT INTO CT_PHIEU_PHAT (MaPhieuPhat, MaThuoc, MaLo, SoLuongPhat) VALUES (@MaPhieuPhat, @MaThuoc, @MaLo, @SoLuongPhat)";
                        using (SqlCommand cmdInsertCT = new SqlCommand(insertCTPhieuSql, conn, trans))
                        {
                            cmdInsertCT.Parameters.AddWithValue("@MaPhieuPhat", maPhieuPhat);
                            cmdInsertCT.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                            cmdInsertCT.Parameters.AddWithValue("@MaLo", lo.MaLo);
                            cmdInsertCT.Parameters.AddWithValue("@SoLuongPhat", soLuongTru);
                            cmdInsertCT.ExecuteNonQuery();
                        }

                        soLuongDaLayTong += soLuongTru;
                        soLuongConThieu -= soLuongTru;
                    }

                    // Nếu quét hết kho mà vẫn thiếu hàng hợp lệ
                    if (soLuongConThieu > 0)
                    {
                        throw new Exception($"Thuốc [{thuoc.MaThuoc}] không đủ tồn kho hợp lệ. Cần thêm {soLuongConThieu}. Vui lòng kiểm tra HSD hoặc nhập kho!");
                    }

                    // Cập nhật tổng số lượng đã phát vào Đơn thuốc
                    string updateCTDonSql = "UPDATE CT_DON_THUOC SET SoLuongDaPhat = ISNULL(SoLuongDaPhat, 0) + @SoLuongDaLay WHERE MaCTDonThuoc = @MaCTDonThuoc";
                    using (SqlCommand cmdUpdateCTDon = new SqlCommand(updateCTDonSql, conn, trans))
                    {
                        cmdUpdateCTDon.Parameters.AddWithValue("@SoLuongDaLay", soLuongDaLayTong);
                        cmdUpdateCTDon.Parameters.AddWithValue("@MaCTDonThuoc", thuoc.MaCTDonThuoc);
                        cmdUpdateCTDon.ExecuteNonQuery();
                    }
                }

                        // ==========================================================
                        // BƯỚC 4: CHỐT TRẠNG THÁI TỔNG CỦA ĐƠN THUỐC (Sửa công thức đếm)
                        // ==========================================================
                        string chotTrangThaiSql = @"
                        -- 1. Tổng số loại thuốc Bác sĩ kê ban đầu
                        DECLARE @TongMonBSKe INT = (SELECT COUNT(*) FROM CT_DON_THUOC WHERE MaDonThuoc = @MaDonThuoc);
    
                        -- 2. Số loại thuốc đã phát ĐỦ số lượng so với y lệnh
                        DECLARE @DaPhatDu INT = (SELECT COUNT(*) FROM CT_DON_THUOC WHERE MaDonThuoc = @MaDonThuoc AND SoLuongDaPhat >= SoLuong);
    
                        -- 3. Đếm xem có phát ra được viên nào chưa
                        DECLARE @CoPhat INT = (SELECT COUNT(*) FROM CT_DON_THUOC WHERE MaDonThuoc = @MaDonThuoc AND SoLuongDaPhat > 0);
    
                        -- CHỐT LOGIC Y KHOA:
                        IF @CoPhat = 0 
                            UPDATE DON_THUOC SET TrangThai = N'Chưa phát' WHERE MaDonThuoc = @MaDonThuoc;
                        ELSE IF @DaPhatDu = @TongMonBSKe 
                            UPDATE DON_THUOC SET TrangThai = N'Đã phát thuốc' WHERE MaDonThuoc = @MaDonThuoc;
                        ELSE 
                            UPDATE DON_THUOC SET TrangThai = N'Đã phát 1 phần' WHERE MaDonThuoc = @MaDonThuoc;";

                        using (SqlCommand cmdChot = new SqlCommand(chotTrangThaiSql, conn, trans))
                        {
                            cmdChot.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                            cmdChot.Parameters.AddWithValue("@MaHD", maHD);
                            cmdChot.ExecuteNonQuery();
                        }

                        // XONG XUÔI -> LƯU VÀO DATABASE
                        trans.Commit();
                return (true, "Phát thuốc và trừ kho thành công!");
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                // Bắt lỗi THROW từ SQL (mã 50001)
                if (ex.Number >= 50000) return (false, ex.Message);
                return (false, "Lỗi hệ thống CSDL: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Hụt kho hoặc các lỗi logic khác
                trans.Rollback();
                return (false, ex.Message);
            }
        }
    }
}


        public (List<DanhSachPhatThuocVM> Data, int TotalRow) GetLichSuPhatThuoc_Pagination(
    string search, DateTime tuNgay, DateTime denNgay, int page, int pageSize, string sortCol, string sortDir)
        {
            var list = new List<DanhSachPhatThuocVM>();
            int totalRow = 0;
            int offset = (page - 1) * pageSize;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // ==============================================================
                // LOGIC LỊCH SỬ CHUẨN MỚI NHẤT:
                // 1. Phải có Đơn thuốc (Xử lý bằng INNER JOIN ở dưới)
                // 2. Nằm trong khoảng ngày lọc (TuNgay - DenNgay)
                // 3. Thuộc 1 trong 2 nhóm: Đã phát xong trọn vẹn HOẶC Đơn cũ qua ngày hôm sau
                // ==============================================================
                string whereClause = @"
            WHERE CAST(hd.NgayThanhToan AS DATE) BETWEEN @TuNgay AND @DenNgay
            AND (
                ISNULL(dt.TrangThai, '') IN (N'Đã phát thuốc', N'Hoàn thành', N'Đã phát 1 phần') -- Nhóm 1: Xong xuôi
                OR CAST(hd.NgayThanhToan AS DATE) < CAST(GETDATE() AS DATE)   -- Nhóm 2: Nợ/Bỏ thuốc qua ngày hôm sau
            )";

                if (!string.IsNullOrEmpty(search))
                {
                    // Ép kiểu cho MaHD và MaDonThuoc để dùng hàm LIKE an toàn
                    whereClause += @" AND (
                bn.HoTen LIKE @Search 
                OR bn.MaBN LIKE @Search 
                OR CAST(hd.MaHD AS NVARCHAR) LIKE @Search 
                OR CAST(dt.MaDonThuoc AS NVARCHAR) LIKE @Search
            )";
                }

                // Đếm tổng số dòng (INNER JOIN DON_THUOC để loại bỏ bọn Không có toa)
                string sqlCount = $@"
            SELECT COUNT(DISTINCT hd.MaHD) 
            FROM HOADON hd
            JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
            JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
            INNER JOIN DON_THUOC dt ON pkb.MaPhieuKhamBenh = dt.MaPhieuKhamBenh -- CHỐT CHẶN: Ép buộc phải có toa
            {whereClause}";

                // Lấy dữ liệu 
                string dir = (sortDir == "ASC" || sortDir == "asc") ? "ASC" : "DESC";

                // Cấu hình cột sắp xếp linh hoạt
                string orderBy = "hd.MaHD"; // Mặc định
                if (sortCol == "BenhNhan") orderBy = "bn.HoTen";
                else if (sortCol == "NgayThanhToan") orderBy = "ThoiGianHienThi";

                string sqlData = $@"
            SELECT 
                hd.MaHD, 
                dt.MaDonThuoc, 
                bn.HoTen + ' (' + bn.MaBN + ')' AS BenhNhan,
                hd.TrangThaiThanhToan AS TTThanhToan, 
                ISNULL(dt.TrangThai, N'Chưa phát') AS TTPhatThuoc,
                COALESCE(ppt.NgayPhat, hd.NgayThanhToan) AS ThoiGianHienThi,
                1 AS HasDonThuoc -- Đã INNER JOIN nên chắc chắn = 1
            FROM HOADON hd
            JOIN PHIEUKHAMBENH pkb ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
            JOIN BENHNHAN bn ON hd.MaBN = bn.MaBN
            INNER JOIN DON_THUOC dt ON pkb.MaPhieuKhamBenh = dt.MaPhieuKhamBenh -- CHỐT CHẶN
            LEFT JOIN PHIEU_PHAT_THUOC ppt ON ppt.MaHD = hd.MaHD AND ppt.MaDonThuoc = dt.MaDonThuoc
            {whereClause}
            ORDER BY {orderBy} {dir}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Date);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay.Date);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    if (!string.IsNullOrEmpty(search))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + search.Trim() + "%");
                    }

                    conn.Open();

                    // Thực thi đếm số trang
                    cmd.CommandText = sqlCount;
                    totalRow = (int)cmd.ExecuteScalar();

                    // Thực thi lấy danh sách
                    cmd.CommandText = sqlData;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DanhSachPhatThuocVM
                            {
                                MaHD = Convert.ToInt32(reader["MaHD"]),
                                MaDonThuoc = Convert.ToInt32(reader["MaDonThuoc"]), // Không cần check DBNull nữa
                                BenhNhan = reader["BenhNhan"].ToString(),
                                TTThanhToan = reader["TTThanhToan"].ToString(),
                                TTPhatThuoc = reader["TTPhatThuoc"].ToString(),
                                HasDonThuoc = true,
                                NgayThanhToanStr = reader["ThoiGianHienThi"] != DBNull.Value
                                                   ? Convert.ToDateTime(reader["ThoiGianHienThi"]).ToString("dd/MM/yyyy HH:mm")
                                                   : "---"
                            });
                        }
                    }
                }
            }
            return (list, totalRow);
        }
    }
}