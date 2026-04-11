using QL_KhamChuaBenhNgoaiTru.Helpers;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class TiepTanDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // Hàm phát sinh Mã Bệnh Nhân theo chuẩn: KH + 4 số (Ví dụ: KH0001, KH0011)
        public string GenerateMaBN(SqlConnection con, SqlTransaction trans)
        {
            string prefix = "KH";
            string sql = "SELECT TOP 1 MaBN FROM BENHNHAN WHERE MaBN LIKE @Prefix ORDER BY MaBN DESC";
            SqlCommand cmd = new SqlCommand(sql, con, trans);
            cmd.Parameters.AddWithValue("@Prefix", prefix + "%");

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int nextNum = int.Parse(result.ToString().Substring(2)) + 1;
                return prefix + nextNum.ToString("D4");
            }
            return prefix + "0001";
        }

        // HÀM CORE: Xử lý Quét thẻ -> Bệnh nhân -> Đăng Ký Offline
        public PhieuDangKyResult DangKyKhamTuThe(QL_KhamChuaBenhNgoaiTru.Models.BenhNhan bn, string loaiThe)
        {
            PhieuDangKyResult res = new PhieuDangKyResult { Success = false };

            object ngaySinhSafe = DBNull.Value;
            if (bn.NgaySinh != null && Convert.ToDateTime(bn.NgaySinh).Year > 1753)
                ngaySinhSafe = bn.NgaySinh;

            object hanSDSafe = DBNull.Value;
            if (bn.HanSuDungBHYT != null && Convert.ToDateTime(bn.HanSuDungBHYT).Year > 1753)
                hanSDSafe = bn.HanSuDungBHYT;

            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        string maBN = "";

                        // BƯỚC 1: TÌM HOẶC THÊM MỚI BỆNH NHÂN (Giữ nguyên logic của bác)
                        string checkSql = @"
                            SELECT TOP 1 MaBN FROM BENHNHAN 
                            WHERE 
                                (CCCD = @CCCD AND @CCCD IS NOT NULL) OR 
                                (SoTheBHYT = @SoBHYT AND @SoBHYT IS NOT NULL) OR
                                (HoTen = @HoTen AND NgaySinh = @NgaySinh)";

                        SqlCommand checkCmd = new SqlCommand(checkSql, con, trans);
                        checkCmd.Parameters.AddWithValue("@CCCD", string.IsNullOrEmpty(bn.CCCD) ? DBNull.Value : (object)bn.CCCD);
                        checkCmd.Parameters.AddWithValue("@SoBHYT", string.IsNullOrEmpty(bn.SoTheBHYT) ? DBNull.Value : (object)bn.SoTheBHYT);
                        checkCmd.Parameters.AddWithValue("@HoTen", bn.HoTen);
                        checkCmd.Parameters.AddWithValue("@NgaySinh", ngaySinhSafe);

                        object existMaBN = checkCmd.ExecuteScalar();

                        if (existMaBN != null)
                        {
                            maBN = existMaBN.ToString();
                            string updateSql = @"
                                UPDATE BENHNHAN SET 
                                    HoTen = ISNULL(@HoTen, HoTen), 
                                    DiaChi = ISNULL(@DiaChi, DiaChi), 
                                    NgaySinh = ISNULL(@NgaySinh, NgaySinh), 
                                    GioiTinh = ISNULL(@GioiTinh, GioiTinh), 
                                    SDT = ISNULL(@SDT, SDT) ";

                            if (loaiThe == "BHYT")
                            {
                                updateSql += ", BHYT = 1, SoTheBHYT = ISNULL(@SoTheBHYT, SoTheBHYT), HanSuDungBHYT = ISNULL(@HanSD, HanSuDungBHYT), MucHuongBHYT = ISNULL(@MucHuong, MucHuongBHYT), TuyenKham = ISNULL(@TuyenKham, TuyenKham) ";
                            }
                            else if (loaiThe == "CCCD")
                            {
                                updateSql += ", CCCD = ISNULL(@CCCD, CCCD) ";
                            }

                            updateSql += " WHERE MaBN = @MaBN";

                            SqlCommand updateCmd = new SqlCommand(updateSql, con, trans);
                            updateCmd.Parameters.AddWithValue("@MaBN", maBN);
                            updateCmd.Parameters.AddWithValue("@HoTen", string.IsNullOrEmpty(bn.HoTen) ? DBNull.Value : (object)bn.HoTen);
                            updateCmd.Parameters.AddWithValue("@DiaChi", string.IsNullOrEmpty(bn.DiaChi) ? DBNull.Value : (object)bn.DiaChi);
                            updateCmd.Parameters.AddWithValue("@NgaySinh", ngaySinhSafe);
                            updateCmd.Parameters.AddWithValue("@GioiTinh", string.IsNullOrEmpty(bn.GioiTinh) ? DBNull.Value : (object)bn.GioiTinh);
                            updateCmd.Parameters.AddWithValue("@SDT", string.IsNullOrEmpty(bn.SDT) ? DBNull.Value : (object)bn.SDT);

                            if (loaiThe == "BHYT")
                            {
                                updateCmd.Parameters.AddWithValue("@SoTheBHYT", string.IsNullOrEmpty(bn.SoTheBHYT) ? DBNull.Value : (object)bn.SoTheBHYT);
                                updateCmd.Parameters.AddWithValue("@HanSD", hanSDSafe);
                                updateCmd.Parameters.AddWithValue("@MucHuong", bn.MucHuongBHYT.HasValue ? (object)bn.MucHuongBHYT.Value : DBNull.Value);
                                updateCmd.Parameters.AddWithValue("@TuyenKham", string.IsNullOrEmpty(bn.TuyenKham) ? DBNull.Value : (object)bn.TuyenKham);
                            }
                            else if (loaiThe == "CCCD")
                            {
                                updateCmd.Parameters.AddWithValue("@CCCD", string.IsNullOrEmpty(bn.CCCD) ? DBNull.Value : (object)bn.CCCD);
                            }
                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            maBN = GenerateMaBN(con, trans);
                            string insertSql = @"INSERT INTO BENHNHAN (MaBN, HoTen, CCCD, SDT, NgaySinh, GioiTinh, DiaChi, BHYT, SoTheBHYT, HanSuDungBHYT, MucHuongBHYT, TuyenKham) 
                                         VALUES (@MaBN, @HoTen, @CCCD, @SDT, @NgaySinh, @GioiTinh, @DiaChi, @HasBHYT, @SoTheBHYT, @HanSD, @MucHuong, @TuyenKham)";
                            SqlCommand insertCmd = new SqlCommand(insertSql, con, trans);

                            insertCmd.Parameters.AddWithValue("@MaBN", maBN);
                            insertCmd.Parameters.AddWithValue("@HoTen", bn.HoTen);
                            insertCmd.Parameters.AddWithValue("@CCCD", string.IsNullOrEmpty(bn.CCCD) ? DBNull.Value : (object)bn.CCCD);
                            insertCmd.Parameters.AddWithValue("@SDT", string.IsNullOrEmpty(bn.SDT) ? DBNull.Value : (object)bn.SDT);
                            insertCmd.Parameters.AddWithValue("@NgaySinh", ngaySinhSafe);
                            insertCmd.Parameters.AddWithValue("@GioiTinh", string.IsNullOrEmpty(bn.GioiTinh) ? DBNull.Value : (object)bn.GioiTinh);
                            insertCmd.Parameters.AddWithValue("@DiaChi", string.IsNullOrEmpty(bn.DiaChi) ? DBNull.Value : (object)bn.DiaChi);
                            insertCmd.Parameters.AddWithValue("@HasBHYT", loaiThe == "BHYT" ? 1 : 0);
                            insertCmd.Parameters.AddWithValue("@SoTheBHYT", (loaiThe == "BHYT" && !string.IsNullOrEmpty(bn.SoTheBHYT)) ? (object)bn.SoTheBHYT : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@HanSD", hanSDSafe);
                            insertCmd.Parameters.AddWithValue("@MucHuong", (loaiThe == "BHYT" && bn.MucHuongBHYT.HasValue) ? (object)bn.MucHuongBHYT.Value : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@TuyenKham", (loaiThe == "BHYT" && !string.IsNullOrEmpty(bn.TuyenKham)) ? (object)bn.TuyenKham : DBNull.Value);

                            insertCmd.ExecuteNonQuery();
                        }

                        // BƯỚC 2: TÌM QUẦY TIẾP TÂN VẮNG NHẤT
                        string sqlTimQuay = @"
                        SELECT TOP 1 p.MaPhong, p.TenPhong
                        FROM PHONG p
                        LEFT JOIN PHIEUDANGKY pdk ON p.MaPhong = pdk.MaPhong 
                            AND CAST(pdk.NgayDangKy AS DATE) = CAST(GETDATE() AS DATE) 
                            AND pdk.TrangThai = N'Chờ xử lý'
                            AND pdk.HinhThucDangKy = N'Offline'
                        WHERE p.MaLoaiPhong = 1 AND p.TrangThai = 1
                        GROUP BY p.MaPhong, p.TenPhong
                        ORDER BY COUNT(pdk.MaPhieuDK) ASC, p.TenPhong ASC";

                        int maQuayChiDinh = 0;
                        string tenQuay = "";
                        using (SqlCommand cmdQuay = new SqlCommand(sqlTimQuay, con, trans))
                        {
                            using (SqlDataReader reader = cmdQuay.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    maQuayChiDinh = (int)reader["MaPhong"];
                                    tenQuay = reader["TenPhong"].ToString();
                                }
                            }
                        }

                        if (maQuayChiDinh == 0) throw new Exception("Hệ thống lỗi: Không tìm thấy Quầy tiếp tân nào đang mở!");

                        // BƯỚC 3: TẠO PHIẾU ĐĂNG KÝ VÀO ĐÚNG QUẦY VỪA TÌM ĐƯỢC
                        string sttSql = "SELECT ISNULL(MAX(STT), 0) + 1 FROM PHIEUDANGKY WHERE MaPhong = @MaPhong AND CAST(NgayDangKy AS DATE) = CAST(GETDATE() AS DATE)";
                        SqlCommand sttCmd = new SqlCommand(sttSql, con, trans);
                        sttCmd.Parameters.AddWithValue("@MaPhong", maQuayChiDinh);
                        int sttMoi = (int)sttCmd.ExecuteScalar();

                        // --- MỚI: TỰ SINH SMART ID CHO PHIẾU ĐĂNG KÝ ---
                        string maPhieuDK = Utilities.Generate(con, trans, "DK", "PHIEUDANGKY", "MaPhieuDK", 4);

                        // Bỏ SCOPE_IDENTITY, thay bằng chuỗi maPhieuDK
                        string insertPhieuSql = @"INSERT INTO PHIEUDANGKY (MaPhieuDK, MaBN, NgayDangKy, STT, HinhThucDangKy, TrangThai, MaPhong) 
                                                  VALUES (@MaPhieuDK, @MaBN, GETDATE(), @STT, N'Offline', N'Chờ xử lý', @MaPhong);";
                        SqlCommand insertPhieuCmd = new SqlCommand(insertPhieuSql, con, trans);
                        insertPhieuCmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK);
                        insertPhieuCmd.Parameters.AddWithValue("@MaBN", maBN);
                        insertPhieuCmd.Parameters.AddWithValue("@STT", sttMoi);
                        insertPhieuCmd.Parameters.AddWithValue("@MaPhong", maQuayChiDinh);

                        insertPhieuCmd.ExecuteNonQuery();

                        trans.Commit();

                        res.Success = true;
                        res.MaBN = maBN;
                        res.TenBN = bn.HoTen;
                        res.STT = sttMoi;
                        res.MaPhieuDK = maPhieuDK; // Lưu ý: Model PhieuDangKyResult phải đổi MaPhieuDK thành string
                        res.TenPhong = tenQuay;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        res.Success = false;
                        res.ErrorMessage = ex.Message;
                    }
                }
            }
            return res;
        }


        // [HÀM ĐÃ ĐƯỢC NÂNG CẤP LUỒNG BHYT, TÀI CHÍNH & PHỤ THU ONLINE 200K]
        public PhieuDangKyResult XacNhanDichVuKham(string maPhieuDK, string maDV, int maPhong, string lyDo, out string tenPhong, out string tenKhoa, out bool requirePayment)
        {
            PhieuDangKyResult res = new PhieuDangKyResult { Success = false };
            tenPhong = "";
            tenKhoa = "";
            requirePayment = true;

            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 1. LẤY THÔNG TIN BỆNH NHÂN (Giữ nguyên)
                        string sqlGetBN = @"
                            SELECT pdk.MaBN, pdk.HinhThucDangKy, bn.BHYT, bn.MucHuongBHYT 
                            FROM PHIEUDANGKY pdk 
                            JOIN BENHNHAN bn ON pdk.MaBN = bn.MaBN 
                            WHERE pdk.MaPhieuDK = @MaPDK";
                        SqlCommand cmdGetBN = new SqlCommand(sqlGetBN, con, trans);
                        cmdGetBN.Parameters.AddWithValue("@MaPDK", maPhieuDK);

                        string maBN = "";
                        string hinhThucDK = "";
                        bool hasBHYT = false;
                        int mucHuong = 0;

                        using (SqlDataReader dr = cmdGetBN.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                maBN = dr["MaBN"].ToString();
                                hinhThucDK = dr["HinhThucDangKy"]?.ToString();
                                hasBHYT = dr["BHYT"] != DBNull.Value && Convert.ToBoolean(dr["BHYT"]);
                                mucHuong = dr["MucHuongBHYT"] != DBNull.Value ? Convert.ToInt32(dr["MucHuongBHYT"]) : 0;
                            }
                        }

                        // 2. LẤY THÔNG TIN DỊCH VỤ CHÍNH (Giữ nguyên)
                        string sqlDV = "SELECT GiaDichVu, CoBHYT FROM DICHVU WHERE MaDV = @MaDV";
                        SqlCommand cmdDV = new SqlCommand(sqlDV, con, trans);
                        cmdDV.Parameters.AddWithValue("@MaDV", maDV);

                        decimal giaDichVuChinh = 0;
                        bool dvCoBHYT = false;
                        using (SqlDataReader drDV = cmdDV.ExecuteReader())
                        {
                            if (drDV.Read())
                            {
                                giaDichVuChinh = Convert.ToDecimal(drDV["GiaDichVu"]);
                                dvCoBHYT = drDV["CoBHYT"] != DBNull.Value && Convert.ToBoolean(drDV["CoBHYT"]);
                            }
                        }

                        // 3. TÍNH TIỀN & ĐỊNH TUYẾN (Giữ nguyên)
                        decimal tienBHYTChiTra = 0;
                        decimal tienBenhNhanTra = giaDichVuChinh;
                        decimal tongTienGoc = giaDichVuChinh;

                        if (hasBHYT && dvCoBHYT)
                        {
                            tienBHYTChiTra = giaDichVuChinh * mucHuong / 100;
                            tienBenhNhanTra = giaDichVuChinh - tienBHYTChiTra;
                        }

                        if (tienBenhNhanTra == 0 || (hasBHYT && dvCoBHYT)) requirePayment = false;
                        else requirePayment = true;

                        string trangThaiKham = requirePayment ? "Chờ thanh toán" : "Chờ khám";

                        // 4. TÍNH STT (Giữ nguyên)
                        int sttPhong = 0;
                        object sttValue = DBNull.Value;
                        if (!requirePayment)
                        {
                            string sqlSTT = "SELECT ISNULL(MAX(STT), 0) + 1 FROM PHIEUKHAMBENH WHERE MaPhong = @MaPhong AND CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE)";
                            SqlCommand cmdSTT = new SqlCommand(sqlSTT, con, trans);
                            cmdSTT.Parameters.AddWithValue("@MaPhong", maPhong);
                            sttPhong = Convert.ToInt32(cmdSTT.ExecuteScalar());
                            sttValue = sttPhong;
                        }

                        // --- MỚI: TỰ SINH SMART ID CHO PHIẾU KHÁM BỆNH ---
                        string maPhieuKhamBenh = Utilities.Generate(con, trans, "KB", "PHIEUKHAMBENH", "MaPhieuKhamBenh", 4);

                        // 5. TẠO PHIEUKHAMBENH (Bỏ OUTPUT INSERTED)
                        string sqlInsertPKB = @"
                            INSERT INTO PHIEUKHAMBENH (MaPhieuKhamBenh, MaPhieuDK, MaBN, STT, NgayLap, TrangThai, MaPhong, LyDoDenKham) 
                            VALUES (@MaPKB, @MaPDK, @MaBN, @STT, GETDATE(), @TrangThai, @MaPhong, @LyDo)";
                        SqlCommand cmdPKB = new SqlCommand(sqlInsertPKB, con, trans);
                        cmdPKB.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                        cmdPKB.Parameters.AddWithValue("@MaPDK", maPhieuDK);
                        cmdPKB.Parameters.AddWithValue("@MaBN", maBN);
                        cmdPKB.Parameters.AddWithValue("@STT", sttValue);
                        cmdPKB.Parameters.AddWithValue("@TrangThai", trangThaiKham);
                        cmdPKB.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmdPKB.Parameters.AddWithValue("@LyDo", string.IsNullOrEmpty(lyDo) ? DBNull.Value : (object)lyDo);
                        cmdPKB.ExecuteNonQuery();

                        // 6. XỬ LÝ HÓA ĐƠN TỔNG
                        string maHD = ""; // Đổi thành chuỗi
                        string sqlCheckHD = "SELECT MaHD, TrangThaiThanhToan FROM HOADON WHERE MaPhieuDK = @MaPDK";
                        SqlCommand cmdCheckHD = new SqlCommand(sqlCheckHD, con, trans);
                        cmdCheckHD.Parameters.AddWithValue("@MaPDK", maPhieuDK);

                        using (SqlDataReader drHD = cmdCheckHD.ExecuteReader())
                        {
                            if (drHD.Read())
                            {
                                maHD = drHD["MaHD"].ToString();
                                string trangThaiHDOld = drHD["TrangThaiThanhToan"].ToString();

                                if (trangThaiHDOld == "Chưa thanh toán")
                                {
                                    requirePayment = true;
                                    trangThaiKham = "Chờ thanh toán";
                                    sttValue = DBNull.Value;
                                    sttPhong = 0;

                                    string sqlRevertPKB = "UPDATE PHIEUKHAMBENH SET TrangThai = N'Chờ thanh toán', STT = NULL WHERE MaPhieuKhamBenh = @MaPKB";
                                    SqlCommand cmdRevert = new SqlCommand(sqlRevertPKB, con, trans);
                                    cmdRevert.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                                    cmdRevert.ExecuteNonQuery();
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(maHD))
                        {
                            // Cập nhật Hóa đơn Online cũ
                            string sqlUpdateHD = @"
                                UPDATE HOADON 
                                SET MaPhieuKhamBenh = @MaPKB,
                                    TongTienGoc = TongTienGoc + @TienGoc,
                                    TongTienBHYTChiTra = TongTienBHYTChiTra + @TienBHYT,
                                    TongTienBenhNhanTra = TongTienBenhNhanTra + @TienBN,
                                    TrangThaiThanhToan = CASE WHEN @TienBN > 0 THEN N'Chưa thanh toán' ELSE TrangThaiThanhToan END
                                WHERE MaHD = @MaHD";
                            SqlCommand cmdUpdateHD = new SqlCommand(sqlUpdateHD, con, trans);
                            cmdUpdateHD.Parameters.AddWithValue("@MaHD", maHD);
                            cmdUpdateHD.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            cmdUpdateHD.Parameters.AddWithValue("@TienGoc", tongTienGoc);
                            cmdUpdateHD.Parameters.AddWithValue("@TienBHYT", tienBHYTChiTra);
                            cmdUpdateHD.Parameters.AddWithValue("@TienBN", tienBenhNhanTra);
                            cmdUpdateHD.ExecuteNonQuery();
                        }
                        else
                        {
                            // --- MỚI: TỰ SINH SMART ID CHO HÓA ĐƠN ---
                            maHD = Utilities.Generate(con, trans, "HD", "HOADON", "MaHD", 6);

                            string sqlInsertHD = @"
                                INSERT INTO HOADON (MaHD, MaBN, MaPhieuDK, MaPhieuKhamBenh, NgayThanhToan, TongTienGoc, TongTienBHYTChiTra, TongTienBenhNhanTra, TrangThaiThanhToan)
                                VALUES (@MaHD, @MaBN, @MaPDK, @MaPKB, NULL, @TienGoc, @TienBHYT, @TienBN, N'Chưa thanh toán')";
                            SqlCommand cmdHD = new SqlCommand(sqlInsertHD, con, trans);
                            cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                            cmdHD.Parameters.AddWithValue("@MaBN", maBN);
                            cmdHD.Parameters.AddWithValue("@MaPDK", string.IsNullOrEmpty(maPhieuDK) ? DBNull.Value : (object)maPhieuDK);
                            cmdHD.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            cmdHD.Parameters.AddWithValue("@TienGoc", tongTienGoc);
                            cmdHD.Parameters.AddWithValue("@TienBHYT", tienBHYTChiTra);
                            cmdHD.Parameters.AddWithValue("@TienBN", tienBenhNhanTra);
                            cmdHD.ExecuteNonQuery();
                        }

                        // 7. TẠO CHI TIẾT HÓA ĐƠN
                        string maCTHD = Utilities.Generate(con, trans, "CHDV", "CT_HOADON_DV", "MaCTHD", 6);

                        string sqlInsertCTHD_Chinh = @"
                            INSERT INTO CT_HOADON_DV (MaCTHD, MaHD, MaDV, DonGia, TongTienGoc, TienBHYTChiTra, TienBenhNhanTra, TrangThaiThanhToan)
                            VALUES (@MaCTHD, @MaHD, @MaDV, @DonGia, @TienGoc, @TienBHYT, @TienBN, N'Chưa thanh toán')";
                        SqlCommand cmdCTHD_Chinh = new SqlCommand(sqlInsertCTHD_Chinh, con, trans);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@MaCTHD", maCTHD);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@MaHD", maHD);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@MaDV", maDV);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@DonGia", giaDichVuChinh);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@TienGoc", giaDichVuChinh);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@TienBHYT", tienBHYTChiTra);
                        cmdCTHD_Chinh.Parameters.AddWithValue("@TienBN", tienBenhNhanTra);
                        cmdCTHD_Chinh.ExecuteNonQuery();

                        // 8. CẬP NHẬT PHIẾU ĐĂNG KÝ
                        string sqlUpdatePDK = "UPDATE PHIEUDANGKY SET TrangThai = N'Đã xác nhận' WHERE MaPhieuDK = @MaPDK";
                        SqlCommand cmdUpdatePDK = new SqlCommand(sqlUpdatePDK, con, trans);
                        cmdUpdatePDK.Parameters.AddWithValue("@MaPDK", maPhieuDK);
                        cmdUpdatePDK.ExecuteNonQuery();

                        // 9. LẤY TÊN PHÒNG/KHOA ĐỂ HIỂN THỊ
                        string sqlInfo = "SELECT p.TenPhong, k.TenKhoa FROM PHONG p JOIN KHOA k ON p.MaKhoa = k.MaKhoa WHERE p.MaPhong = @MaPhong";
                        SqlCommand cmdInfo = new SqlCommand(sqlInfo, con, trans);
                        cmdInfo.Parameters.AddWithValue("@MaPhong", maPhong);
                        using (SqlDataReader reader = cmdInfo.ExecuteReader())
                        {
                            if (reader.Read()) { tenPhong = reader["TenPhong"].ToString(); tenKhoa = reader["TenKhoa"].ToString(); }
                        }

                        trans.Commit();
                        res.Success = true;
                        res.MaPhieuDK = maPhieuDK; 
                        res.MaBN = maBN;
                        res.STT = requirePayment ? 0 : sttPhong;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        res.Success = false;
                        res.ErrorMessage = ex.Message;
                    }
                }
            }
            return res;
        }

        // 1. Lấy danh sách OFFLINE (Từ Kiosk) - CÓ THÊM LỌC NGÀY
        public DataTable GetDanhSachOffline(int maPhongTiepTan, DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
        SELECT 
            pdk.MaPhieuDK, pdk.STT, pdk.NgayDangKy,
            pdk.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.CCCD, bn.SDT, bn.Email, bn.DiaChi, bn.AvatarPath,
            bn.BHYT, bn.SoTheBHYT, bn.HanSuDungBHYT, bn.TuyenKham, bn.MucHuongBHYT,
            ISNULL(pkb.TrangThai, N'Chưa tiếp nhận') AS TrangThaiPKB, 
            pkb.MaPhieuKhamBenh, pkb.STT AS STT_Kham, pkb.MaPhong AS PhongKhamChiDinh
        FROM PHIEUDANGKY pdk
        JOIN BENHNHAN bn ON pdk.MaBN = bn.MaBN
        LEFT JOIN PHIEUKHAMBENH pkb ON pdk.MaPhieuDK = pkb.MaPhieuDK
        WHERE pdk.MaPhong = @MaPhong
          AND pdk.HinhThucDangKy = N'Offline'
          -- ĐỔI THÀNH LỌC THEO KHOẢNG THỜI GIAN
          AND CAST(pdk.NgayDangKy AS DATE) >= CAST(@TuNgay AS DATE)
          AND CAST(pdk.NgayDangKy AS DATE) <= CAST(@DenNgay AS DATE)
          AND (pdk.TrangThai = N'Chờ xử lý' OR (pdk.TrangThai = N'Đã xác nhận' AND pkb.TrangThai IN (N'Chờ thanh toán', N'Chờ cấp số')))
        ORDER BY pdk.NgayDangKy ASC, pdk.STT ASC";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaPhong", maPhongTiepTan);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. Lấy danh sách ONLINE (Từ Web) - CÓ THÊM LỌC NGÀY
        public DataTable GetDanhSachOnline(int maPhongTiepTan, DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
        SELECT 
            pdk.MaPhieuDK, pdk.LyDo, pdk.NgayDangKy,
            pdk.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.CCCD, bn.SDT, bn.Email, bn.DiaChi, bn.AvatarPath,
            bn.BHYT, bn.SoTheBHYT, bn.HanSuDungBHYT, bn.TuyenKham, bn.MucHuongBHYT,
            ISNULL(pkb.TrangThai, N'Chưa tiếp nhận') AS TrangThaiPKB, 
            pkb.MaPhieuKhamBenh, pkb.STT AS STT_Kham, pkb.MaPhong AS PhongKhamChiDinh
        FROM PHIEUDANGKY pdk
        JOIN BENHNHAN bn ON pdk.MaBN = bn.MaBN
        LEFT JOIN PHIEUKHAMBENH pkb ON pdk.MaPhieuDK = pkb.MaPhieuDK
        WHERE pdk.MaPhong = @MaPhong
          AND pdk.HinhThucDangKy = N'Online'
          -- ĐỔI THÀNH LỌC THEO KHOẢNG THỜI GIAN
          AND CAST(pdk.NgayDangKy AS DATE) >= CAST(@TuNgay AS DATE)
          AND CAST(pdk.NgayDangKy AS DATE) <= CAST(@DenNgay AS DATE)
          AND (pdk.TrangThai = N'Chờ xử lý' OR (pdk.TrangThai = N'Đã xác nhận' AND pkb.TrangThai IN (N'Chờ thanh toán', N'Chờ cấp số')))
        ORDER BY pdk.NgayDangKy ASC, pdk.MaPhieuDK ASC";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaPhong", maPhongTiepTan);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. HÀM MỚI: Cấp STT cho bệnh nhân Khám Dịch Vụ đã đóng tiền xong
        public PhieuDangKyResult ChotCapSoKham(string maPhieuKhamBenh, out string tenPhong, out string tenKhoa)
        {
            PhieuDangKyResult res = new PhieuDangKyResult { Success = false };
            tenPhong = ""; tenKhoa = "";

            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        string checkSql = "SELECT TrangThai, MaPhong FROM PHIEUKHAMBENH WHERE MaPhieuKhamBenh = @MaPKB";
                        SqlCommand cmdCheck = new SqlCommand(checkSql, con, trans);
                        cmdCheck.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                        string trangThai = "";
                        int maPhong = 0;
                        using (SqlDataReader dr = cmdCheck.ExecuteReader())
                        {
                            if (dr.Read()) { trangThai = dr["TrangThai"].ToString(); maPhong = Convert.ToInt32(dr["MaPhong"]); }
                        }

                        if (trangThai == "Chờ thanh toán") throw new Exception("Bệnh nhân CHƯA ĐÓNG TIỀN khám dịch vụ! Không thể cấp số.");
                        if (trangThai != "Chờ cấp số") throw new Exception("Hồ sơ này đã được cấp số hoặc bị hủy.");

                        // Tính STT
                        string sqlSTT = "SELECT ISNULL(MAX(STT), 0) + 1 FROM PHIEUKHAMBENH WHERE MaPhong = @MaPhong AND CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE)";
                        SqlCommand cmdSTT = new SqlCommand(sqlSTT, con, trans);
                        cmdSTT.Parameters.AddWithValue("@MaPhong", maPhong);
                        int sttPhong = Convert.ToInt32(cmdSTT.ExecuteScalar());

                        // Cập nhật STT và Trạng thái
                        string updateSql = "UPDATE PHIEUKHAMBENH SET STT = @STT, TrangThai = N'Chờ khám' WHERE MaPhieuKhamBenh = @MaPKB";
                        SqlCommand cmdUpdate = new SqlCommand(updateSql, con, trans);
                        cmdUpdate.Parameters.AddWithValue("@STT", sttPhong);
                        cmdUpdate.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                        cmdUpdate.ExecuteNonQuery();

                        // Lấy thông tin phòng
                        string sqlInfo = "SELECT p.TenPhong, k.TenKhoa FROM PHONG p JOIN KHOA k ON p.MaKhoa = k.MaKhoa WHERE p.MaPhong = @MaPhong";
                        SqlCommand cmdInfo = new SqlCommand(sqlInfo, con, trans);
                        cmdInfo.Parameters.AddWithValue("@MaPhong", maPhong);
                        using (SqlDataReader reader = cmdInfo.ExecuteReader())
                        {
                            if (reader.Read()) { tenPhong = reader["TenPhong"].ToString(); tenKhoa = reader["TenKhoa"].ToString(); }
                        }

                        trans.Commit();
                        res.Success = true; res.STT = sttPhong;
                    }
                    catch (Exception ex) { trans.Rollback(); res.ErrorMessage = ex.Message; }
                }
            }
            return res;
        }

        // Lấy danh sách dịch vụ Khám Bệnh (LDV01)
        public DataTable GetDanhSachDichVuKham()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = "SELECT MaDV, TenDV, GiaDichVu FROM DICHVU WHERE MaLoaiDV = 'LDV01' AND TrangThai = 1";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.Fill(dt);
            }
            return dt;
        }

        // Hàm lấy danh sách PHÒNG KHÁM vắng nhất
        public DataTable GetPhongTheoDichVu(string maDV)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
            SELECT p.MaPhong, p.TenPhong,
                   (SELECT COUNT(*) FROM PHIEUKHAMBENH pkb 
                    WHERE pkb.MaPhong = p.MaPhong 
                      AND CAST(pkb.NgayLap AS DATE) = CAST(GETDATE() AS DATE) 
                      AND pkb.TrangThai = N'Chờ khám') AS SoNguoiCho
            FROM PHONG p
            WHERE p.MaKhoa = (SELECT MaKhoa FROM DICHVU WHERE MaDV = @MaDV)
              AND p.TrangThai = 1
            ORDER BY SoNguoiCho ASC";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDV", maDV);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public string GetTenPhong(int maPhong)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string sql = "SELECT TenPhong FROM PHONG WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Quầy không xác định";
            }
        }


        // 4. HÀM ĐÃ SỬA: Lấy TẤT CẢ Lịch Sử Tiếp Nhận (Không phân biệt quầy)
        public DataTable GetLichSuTiepNhan(DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
        SELECT 
            pdk.MaPhieuDK, pdk.HinhThucDangKy, pdk.NgayDangKy,
            bn.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.SDT,
            pkb.MaPhieuKhamBenh, pkb.STT AS STT_Kham, pkb.TrangThai AS TrangThaiPKB,
            p.TenPhong AS PhongKhamChiDinh
        FROM PHIEUDANGKY pdk
        JOIN BENHNHAN bn ON pdk.MaBN = bn.MaBN
        JOIN PHIEUKHAMBENH pkb ON pdk.MaPhieuDK = pkb.MaPhieuDK
        JOIN PHONG p ON pkb.MaPhong = p.MaPhong
        WHERE CAST(pdk.NgayDangKy AS DATE) >= CAST(@TuNgay AS DATE)
          AND CAST(pdk.NgayDangKy AS DATE) <= CAST(@DenNgay AS DATE)
          AND pkb.TrangThai IN (N'Chờ khám', N'Đang khám', N'Hoàn thành')
        ORDER BY pdk.NgayDangKy DESC, pkb.STT ASC"; // Ưu tiên ngày mới nhất lên đầu

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