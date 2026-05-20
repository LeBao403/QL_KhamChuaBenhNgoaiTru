using System;
using System.Configuration;
using System.Data.SqlClient;
using QL_KhamChuaBenhNgoaiTru.Helpers;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class TaiKhoanDB
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public TaiKhoan CheckLogin(string username, string password)
        {
            username = username?.Trim();
            password = password?.Trim();

            var tk = GetTaiKhoanByUsernameOrSdt(username);
            if (tk == null || !tk.IsActive || !PasswordSecurityHelper.VerifyPassword(password, tk.PasswordHash))
                return null;

            if (!PasswordSecurityHelper.IsHashed(tk.PasswordHash))
            {
                tk.PasswordHash = PasswordSecurityHelper.HashPassword(password);
                UpdatePasswordHash(tk.MaTK, tk.PasswordHash);
            }

            return tk;
        }

        public TaiKhoan GetTaiKhoanByMaTK(int maTK)
        {
            TaiKhoan tk = null;
            using (var con = new SqlConnection(connectionString))
            {
                const string sql = "SELECT TOP 1 * FROM TAIKHOAN WHERE MaTK = @MaTK";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTK", maTK);

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        tk = MapTaiKhoan(rd);
                }
            }
            return tk;
        }

        public TaiKhoan GetTaiKhoanByUsernameOrSdt(string username)
        {
            username = username?.Trim();
            if (string.IsNullOrWhiteSpace(username)) return null;

            TaiKhoan tk = null;
            using (var con = new SqlConnection(connectionString))
            {
                const string sql = @"
                    SELECT TOP 1 tk.*
                    FROM TAIKHOAN tk
                    LEFT JOIN BENHNHAN bn ON bn.MaTK = tk.MaTK
                    WHERE tk.Username = @Username OR bn.SDT = @Username OR bn.Email = @Username";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Username", username);

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                        tk = MapTaiKhoan(rd);
                }
            }
            return tk;
        }

        public bool InsertTaiKhoan(TaiKhoan tk)
        {
            using (var con = new SqlConnection(connectionString))
            {
                const string checkSql = "SELECT COUNT(*) FROM TAIKHOAN WHERE Username = @Username";
                var checkCmd = new SqlCommand(checkSql, con);
                checkCmd.Parameters.AddWithValue("@Username", tk.Username);
                con.Open();
                var count = (int)checkCmd.ExecuteScalar();
                if (count > 0) return false;

                const string sql = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) VALUES (@Username, @PasswordHash, 1, GETDATE())";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Username", tk.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", PasswordSecurityHelper.EnsureHashed(tk.PasswordHash));

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool InsertTaiKhoanWithEmail(TaiKhoan tk, string email, string hoTen)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var trans = con.BeginTransaction())
                {
                    try
                    {
                        const string checkSql = @"
                            SELECT
                                (SELECT COUNT(*) FROM TAIKHOAN WHERE Username = @Username)
                              + (SELECT COUNT(*) FROM BENHNHAN WHERE Email = @Email)";
                        var checkCmd = new SqlCommand(checkSql, con, trans);
                        checkCmd.Parameters.AddWithValue("@Username", tk.Username);
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        var count = (int)checkCmd.ExecuteScalar();
                        if (count > 0) return false;

                        const string sql = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@Username, @PasswordHash, 1, GETDATE())";
                        var cmd = new SqlCommand(sql, con, trans);
                        cmd.Parameters.AddWithValue("@Username", tk.Username);
                        cmd.Parameters.AddWithValue("@PasswordHash", PasswordSecurityHelper.EnsureHashed(tk.PasswordHash));

                        var maTK = (int)cmd.ExecuteScalar();

                        const string sqlMaBN = @"
                            SELECT ISNULL(MAX(TRY_CONVERT(INT, SUBSTRING(MaBN, PATINDEX('%[0-9]%', MaBN), 10))), 0) + 1
                            FROM BENHNHAN WITH (UPDLOCK, HOLDLOCK)
                            WHERE PATINDEX('%[0-9]%', MaBN) > 0";
                        var cmdMaBN = new SqlCommand(sqlMaBN, con, trans);
                        var nextMaBNNumber = Convert.ToInt32(cmdMaBN.ExecuteScalar());
                        var maBN = "BN" + nextMaBNNumber.ToString().PadLeft(4, '0');

                        var sdt = "";
                        if (tk.Username != null && tk.Username.Length >= 9 && tk.Username.Length <= 11)
                        {
                            var isNum = true;
                            foreach (var c in tk.Username)
                                if (c < '0' || c > '9') isNum = false;
                            if (isNum) sdt = tk.Username;
                        }

                        const string insertBnSql = "INSERT INTO BENHNHAN (MaBN, HoTen, SDT, Email, MaTK) VALUES (@MaBN, @HoTen, @SDT, @Email, @MaTK)";
                        var cmdBn = new SqlCommand(insertBnSql, con, trans);
                        cmdBn.Parameters.AddWithValue("@MaBN", maBN);
                        cmdBn.Parameters.AddWithValue("@HoTen", hoTen);
                        cmdBn.Parameters.AddWithValue("@SDT", sdt);
                        cmdBn.Parameters.AddWithValue("@Email", email);
                        cmdBn.Parameters.AddWithValue("@MaTK", maTK);
                        cmdBn.ExecuteNonQuery();

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Loi insert tai khoan: " + ex.Message);
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        public NhanVien GetNhanVienByMaTK(int maTK)
        {
            NhanVien nv = null;
            using (var con = new SqlConnection(connectionString))
            {
                const string sql = "SELECT * FROM NHANVIEN WHERE MaTK = @MaTK AND TrangThai = 1";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTK", maTK);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        nv = new NhanVien
                        {
                            MaNV = rd["MaNV"].ToString(),
                            HoTen = rd["HoTen"].ToString(),
                            MaChucVu = rd["MaChucVu"] != DBNull.Value ? Convert.ToInt32(rd["MaChucVu"]) : 0,
                            MaPhong = rd["MaPhong"] != DBNull.Value ? Convert.ToInt32(rd["MaPhong"]) : (int?)null,
                            MaKhoa = rd["MaKhoa"] != DBNull.Value ? Convert.ToInt32(rd["MaKhoa"]) : (int?)null
                        };
                    }
                }
            }
            return nv;
        }

        public BenhNhan GetBenhNhanByMaTK(int maTK)
        {
            BenhNhan bn = null;
            using (var con = new SqlConnection(connectionString))
            {
                const string sql = "SELECT * FROM BENHNHAN WHERE MaTK = @MaTK";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTK", maTK);

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        bn = new BenhNhan
                        {
                            MaBN = rd["MaBN"].ToString(),
                            HoTen = rd["HoTen"].ToString(),
                            SDT = rd["SDT"] != DBNull.Value ? rd["SDT"].ToString() : "",
                            Email = rd["Email"] != DBNull.Value ? rd["Email"].ToString() : "",
                            CCCD = rd["CCCD"] != DBNull.Value ? rd["CCCD"].ToString() : "",
                            BHYT = rd["BHYT"] != DBNull.Value && Convert.ToBoolean(rd["BHYT"])
                        };
                    }
                }
            }
            return bn;
        }

        public bool UpdatePasswordHash(int maTK, string passwordHash)
        {
            using (var con = new SqlConnection(connectionString))
            {
                const string sql = "UPDATE TAIKHOAN SET PasswordHash = @PasswordHash WHERE MaTK = @MaTK";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTK", maTK);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private static TaiKhoan MapTaiKhoan(SqlDataReader rd)
        {
            return new TaiKhoan
            {
                MaTK = Convert.ToInt32(rd["MaTK"]),
                Username = rd["Username"].ToString(),
                PasswordHash = rd["PasswordHash"].ToString(),
                IsActive = Convert.ToBoolean(rd["IsActive"])
            };
        }
    }
}
