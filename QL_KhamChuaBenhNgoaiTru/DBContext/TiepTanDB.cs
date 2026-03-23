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

        // Hàm phát sinh Mã Bệnh Nhân (VD: BN2603001)
        public string GenerateMaBN(SqlConnection con, SqlTransaction trans)
        {
            string prefix = "BN" + DateTime.Now.ToString("yyMM");
            string sql = "SELECT TOP 1 MaBN FROM BENHNHAN WHERE MaBN LIKE @Prefix ORDER BY MaBN DESC";
            SqlCommand cmd = new SqlCommand(sql, con, trans);
            cmd.Parameters.AddWithValue("@Prefix", prefix + "%");

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int nextNum = int.Parse(result.ToString().Substring(6)) + 1;
                return prefix + nextNum.ToString("D3"); // BN2603002...
            }
            return prefix + "001"; // BN2603001
        }

        // HÀM CORE: Xử lý Quét thẻ -> Lưu Bệnh nhân -> Tự động chia Quầy -> Tạo Phiếu Đăng Ký
        public PhieuDangKyResult DangKyKhamTuThe(QL_KhamChuaBenhNgoaiTru.Models.BenhNhan bn, string loaiThe)
        {
            PhieuDangKyResult res = new PhieuDangKyResult { Success = false };

            // ====================================================================
            // BƯỚC 0: TIỀN XỬ LÝ DỮ LIỆU ĐỂ TRÁNH LỖI "SQLDATETIME OVERFLOW 1753"
            // ====================================================================
            object ngaySinhSafe = DBNull.Value;
            if (bn.NgaySinh != null && Convert.ToDateTime(bn.NgaySinh).Year > 1753)
            {
                ngaySinhSafe = bn.NgaySinh;
            }

            object hanSDSafe = DBNull.Value;
            if (bn.HanSuDungBHYT != null && Convert.ToDateTime(bn.HanSuDungBHYT).Year > 1753)
            {
                hanSDSafe = bn.HanSuDungBHYT;
            }

            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        string maBN = "";

                        // BƯỚC 1: TÌM HOẶC THÊM MỚI BỆNH NHÂN
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

                        // Sử dụng biến an toàn đã bọc DBNull
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

                            // Sử dụng biến an toàn
                            updateCmd.Parameters.AddWithValue("@NgaySinh", ngaySinhSafe);

                            updateCmd.Parameters.AddWithValue("@GioiTinh", string.IsNullOrEmpty(bn.GioiTinh) ? DBNull.Value : (object)bn.GioiTinh);
                            updateCmd.Parameters.AddWithValue("@SDT", string.IsNullOrEmpty(bn.SDT) ? DBNull.Value : (object)bn.SDT);

                            if (loaiThe == "BHYT")
                            {
                                updateCmd.Parameters.AddWithValue("@SoTheBHYT", string.IsNullOrEmpty(bn.SoTheBHYT) ? DBNull.Value : (object)bn.SoTheBHYT);

                                // Sử dụng biến an toàn
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

                            // Sử dụng biến an toàn
                            insertCmd.Parameters.AddWithValue("@NgaySinh", ngaySinhSafe);

                            insertCmd.Parameters.AddWithValue("@GioiTinh", string.IsNullOrEmpty(bn.GioiTinh) ? DBNull.Value : (object)bn.GioiTinh);
                            insertCmd.Parameters.AddWithValue("@DiaChi", string.IsNullOrEmpty(bn.DiaChi) ? DBNull.Value : (object)bn.DiaChi);

                            insertCmd.Parameters.AddWithValue("@HasBHYT", loaiThe == "BHYT" ? 1 : 0);
                            insertCmd.Parameters.AddWithValue("@SoTheBHYT", (loaiThe == "BHYT" && !string.IsNullOrEmpty(bn.SoTheBHYT)) ? (object)bn.SoTheBHYT : DBNull.Value);

                            // Sử dụng biến an toàn
                            insertCmd.Parameters.AddWithValue("@HanSD", hanSDSafe);

                            insertCmd.Parameters.AddWithValue("@MucHuong", (loaiThe == "BHYT" && bn.MucHuongBHYT.HasValue) ? (object)bn.MucHuongBHYT.Value : DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@TuyenKham", (loaiThe == "BHYT" && !string.IsNullOrEmpty(bn.TuyenKham)) ? (object)bn.TuyenKham : DBNull.Value);

                            insertCmd.ExecuteNonQuery();
                        }

                        // BƯỚC 2: TÌM QUẦY TIẾP TÂN VẮNG NHẤT (MaLoaiPhong = 1)
                        string sqlTimQuay = @"
                        SELECT TOP 1 p.MaPhong, p.TenPhong
                        FROM PHONG p
                        LEFT JOIN PHIEUDANGKY pdk ON p.MaPhong = pdk.MaPhong 
                            AND CAST(pdk.NgayDangKy AS DATE) = CAST(GETDATE() AS DATE) 
                            AND pdk.TrangThai = N'Chờ xử lý'
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

                        string insertPhieuSql = @"INSERT INTO PHIEUDANGKY (MaBN, NgayDangKy, STT, HinhThucDangKy, TrangThai, MaPhong) 
                                          VALUES (@MaBN, GETDATE(), @STT, N'Offline', N'Chờ xử lý', @MaPhong);
                                          SELECT SCOPE_IDENTITY();";
                        SqlCommand insertPhieuCmd = new SqlCommand(insertPhieuSql, con, trans);
                        insertPhieuCmd.Parameters.AddWithValue("@MaBN", maBN);
                        insertPhieuCmd.Parameters.AddWithValue("@STT", sttMoi);
                        insertPhieuCmd.Parameters.AddWithValue("@MaPhong", maQuayChiDinh);

                        int maPhieuDK = Convert.ToInt32(insertPhieuCmd.ExecuteScalar());

                        trans.Commit();

                        res.Success = true;
                        res.MaBN = maBN;
                        res.TenBN = bn.HoTen;
                        res.STT = sttMoi;
                        res.MaPhieuDK = maPhieuDK;
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



        public PhieuDangKyResult XacNhanDichVuKham(int maPhieuDK, string maDV, int maPhong, string lyDo, out string tenPhong, out string tenKhoa)
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
                        // 1. LẤY MaBN
                        string sqlGetBN = "SELECT MaBN FROM PHIEUDANGKY WHERE MaPhieuDK = @MaPDK";
                        SqlCommand cmdGetBN = new SqlCommand(sqlGetBN, con, trans);
                        cmdGetBN.Parameters.AddWithValue("@MaPDK", maPhieuDK);
                        string maBN = cmdGetBN.ExecuteScalar().ToString();

                        // 2. TÍNH STT TẠI PHÒNG
                        string sqlSTT = "SELECT ISNULL(MAX(STT), 0) + 1 FROM PHIEUKHAMBENH WHERE MaPhong = @MaPhong AND CAST(NgayLap AS DATE) = CAST(GETDATE() AS DATE)";
                        SqlCommand cmdSTT = new SqlCommand(sqlSTT, con, trans);
                        cmdSTT.Parameters.AddWithValue("@MaPhong", maPhong);
                        int sttPhong = Convert.ToInt32(cmdSTT.ExecuteScalar());

                        // 3. TẠO PHIEUKHAMBENH (Lưu ý: Không truyền MaBacSiKham nữa)
                        string sqlInsertPKB = @"
                    INSERT INTO PHIEUKHAMBENH (MaPhieuDK, MaBN, STT, NgayLap, TrangThai, MaPhong, LyDoDenKham) 
                    VALUES (@MaPDK, @MaBN, @STT, GETDATE(), N'Đang khám', @MaPhong, @LyDo)";
                        SqlCommand cmdPKB = new SqlCommand(sqlInsertPKB, con, trans);
                        cmdPKB.Parameters.AddWithValue("@MaPDK", maPhieuDK);
                        cmdPKB.Parameters.AddWithValue("@MaBN", maBN);
                        cmdPKB.Parameters.AddWithValue("@STT", sttPhong);
                        cmdPKB.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmdPKB.Parameters.AddWithValue("@LyDo", string.IsNullOrEmpty(lyDo) ? DBNull.Value : (object)lyDo);
                        cmdPKB.ExecuteNonQuery();

                        // 4. UPDATE PHIEUDANGKY
                        string sqlUpdatePDK = "UPDATE PHIEUDANGKY SET TrangThai = N'Đã xác nhận' WHERE MaPhieuDK = @MaPDK";
                        SqlCommand cmdUpdatePDK = new SqlCommand(sqlUpdatePDK, con, trans);
                        cmdUpdatePDK.Parameters.AddWithValue("@MaPDK", maPhieuDK);
                        cmdUpdatePDK.ExecuteNonQuery();

                        // 5. LẤY TÊN PHÒNG/KHOA ĐỂ HIỂN THỊ POPUP
                        string sqlInfo = "SELECT p.TenPhong, k.TenKhoa FROM PHONG p JOIN KHOA k ON p.MaKhoa = k.MaKhoa WHERE p.MaPhong = @MaPhong";
                        SqlCommand cmdInfo = new SqlCommand(sqlInfo, con, trans);
                        cmdInfo.Parameters.AddWithValue("@MaPhong", maPhong);
                        using (SqlDataReader reader = cmdInfo.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tenPhong = reader["TenPhong"].ToString();
                                tenKhoa = reader["TenKhoa"].ToString();
                            }
                        }

                        trans.Commit();
                        res.Success = true; res.MaPhieuDK = maPhieuDK; res.STT = sttPhong; res.MaBN = maBN;
                    }
                    catch (Exception ex) { trans.Rollback(); res.Success = false; res.ErrorMessage = ex.Message; }
                }
            }
            return res;
        }

        // 1. Lấy danh sách bệnh nhân đang chờ (Từ Kiosk)
        // 1. Lấy danh sách bệnh nhân đang chờ (Lọc theo Quầy trực của nhân viên)
        public DataTable GetDanhSachChoXyLy(int maPhongTiepTan)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
            SELECT pdk.MaPhieuDK, pdk.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, pdk.STT 
            FROM PHIEUDANGKY pdk
            JOIN BENHNHAN bn ON pdk.MaBN = bn.MaBN
            WHERE pdk.TrangThai = N'Chờ xử lý' 
              AND pdk.MaPhong = @MaPhong
              AND CAST(pdk.NgayDangKy AS DATE) = CAST(GETDATE() AS DATE)
            ORDER BY pdk.STT ASC";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaPhong", maPhongTiepTan);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. Lấy danh sách dịch vụ Khám Bệnh (LDV01)
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

        // 1. Hàm lấy danh sách PHÒNG KHÁM (không lấy Bác sĩ)
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
                      AND pkb.TrangThai = N'Đang khám') AS SoNguoiCho
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
    }
}