using System;
using System.Data.SqlClient;
using System.Configuration;
using QL_KhamChuaBenhNgoaiTru.Models;
using QL_KhamChuaBenhNgoaiTru.Controllers;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class TaiKhoanDB
    {
        // Lấy chuỗi kết nối từ Web.config
        string connectionString = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. Kiểm tra đăng nhập
        public TaiKhoan CheckLogin(string username, string passwordHash)
        {
            username = username?.Trim();
            passwordHash = passwordHash?.Trim();

            TaiKhoan tk = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT TOP 1 tk.*
                    FROM TAIKHOAN tk
                    LEFT JOIN BENHNHAN bn ON bn.MaTK = tk.MaTK
                    WHERE (tk.Username = @Username OR bn.SDT = @Username OR bn.Email = @Username)
                      AND tk.PasswordHash = @PasswordHash
                      AND tk.IsActive = 1";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash); // Ở thực tế nên dùng Hash (MD5/SHA)

                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    tk = new TaiKhoan();
                    tk.MaTK = Convert.ToInt32(rd["MaTK"]);
                    tk.Username = rd["Username"].ToString();
                    tk.PasswordHash = rd["PasswordHash"].ToString();
                    tk.IsActive = Convert.ToBoolean(rd["IsActive"]);
                }
            }
            return tk;
        }

        public TaiKhoan GetTaiKhoanByUsernameOrSdt(string username)
        {
            username = username?.Trim();
            if (string.IsNullOrWhiteSpace(username)) return null;

            TaiKhoan tk = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT TOP 1 tk.*
                    FROM TAIKHOAN tk
                    LEFT JOIN BENHNHAN bn ON bn.MaTK = tk.MaTK
                    WHERE tk.Username = @Username OR bn.SDT = @Username OR bn.Email = @Username";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Username", username);

                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    tk = new TaiKhoan();
                    tk.MaTK = Convert.ToInt32(rd["MaTK"]);
                    tk.Username = rd["Username"].ToString();
                    tk.PasswordHash = rd["PasswordHash"].ToString();
                    tk.IsActive = Convert.ToBoolean(rd["IsActive"]);
                }
            }
            return tk;
        }

        // 2. Thêm tài khoản mới (Đăng ký)
        public bool InsertTaiKhoan(TaiKhoan tk)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Kiểm tra trùng lặp Username
                string checkSql = "SELECT COUNT(*) FROM TAIKHOAN WHERE Username = @Username";
                SqlCommand checkCmd = new SqlCommand(checkSql, con);
                checkCmd.Parameters.AddWithValue("@Username", tk.Username);
                con.Open();
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0) return false; // Đã tồn tại

                // Thêm mới
                string sql = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) VALUES (@Username, @PasswordHash, 1, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Username", tk.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", tk.PasswordHash);

                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        // 3. Thêm tài khoản kèm Email và thông tin Bệnh nhân
        public bool InsertTaiKhoanWithEmail(TaiKhoan tk, string email, string hoTen)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra trùng lặp Username hoặc Email
                        string checkSql = @"
                            SELECT
                                (SELECT COUNT(*) FROM TAIKHOAN WHERE Username = @Username)
                              + (SELECT COUNT(*) FROM BENHNHAN WHERE Email = @Email)";
                        SqlCommand checkCmd = new SqlCommand(checkSql, con, trans);
                        checkCmd.Parameters.AddWithValue("@Username", tk.Username);
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0) return false; // Đã tồn tại

                        // Thêm mới TAIKHOAN và lấy MaTK vừa tạo
                        string sql = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@Username, @PasswordHash, 1, GETDATE())";
                        SqlCommand cmd = new SqlCommand(sql, con, trans);
                        cmd.Parameters.AddWithValue("@Username", tk.Username);
                        cmd.Parameters.AddWithValue("@PasswordHash", tk.PasswordHash);

                        int maTK = (int)cmd.ExecuteScalar();

                        // Tự động sinh MaBN định dạng BN0001 (để vừa với varchar(10))
                        string sqlMaBN = @"
                            SELECT ISNULL(MAX(TRY_CONVERT(INT, SUBSTRING(MaBN, PATINDEX('%[0-9]%', MaBN), 10))), 0) + 1
                            FROM BENHNHAN WITH (UPDLOCK, HOLDLOCK)
                            WHERE PATINDEX('%[0-9]%', MaBN) > 0";
                        SqlCommand cmdMaBN = new SqlCommand(sqlMaBN, con, trans);
                        int nextMaBNNumber = Convert.ToInt32(cmdMaBN.ExecuteScalar());
                        string maBN = "BN" + nextMaBNNumber.ToString().PadLeft(4, '0');
                        
                        // Ở form đăng ký, Username thường là SĐT
                        string sdt = "";
                        if (tk.Username != null && tk.Username.Length >= 9 && tk.Username.Length <= 11)
                        {
                            bool isNum = true;
                            foreach(char c in tk.Username) if(c < '0' || c > '9') isNum = false;
                            if (isNum) sdt = tk.Username;
                        }

                        // Thêm vào BENHNHAN
                        string insertBnSql = "INSERT INTO BENHNHAN (MaBN, HoTen, SDT, Email, MaTK) VALUES (@MaBN, @HoTen, @SDT, @Email, @MaTK)";
                        SqlCommand cmdBn = new SqlCommand(insertBnSql, con, trans);
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
                        System.Diagnostics.Debug.WriteLine("Lỗi insert tài khoản: " + ex.Message);
                        try { System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "log_taikhoan.txt"), DateTime.Now.ToString() + ": " + ex.ToString() + "\n"); } catch {}
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }
        // Kiểm tra xem tài khoản này có phải là nhân viên không
        public NhanVien GetNhanVienByMaTK(int maTK)
        {
            NhanVien nv = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM NHANVIEN WHERE MaTK = @MaTK AND TrangThai = 1";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTK", maTK);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    nv = new NhanVien();
                    nv.MaNV = rd["MaNV"].ToString();
                    nv.HoTen = rd["HoTen"].ToString();
                    nv.MaChucVu = rd["MaChucVu"] != DBNull.Value ? Convert.ToInt32(rd["MaChucVu"]) : 0;
                    nv.MaPhong = rd["MaPhong"] != DBNull.Value ? Convert.ToInt32(rd["MaPhong"]) : (int?)null;
                    nv.MaKhoa = rd["MaKhoa"] != DBNull.Value ? Convert.ToInt32(rd["MaKhoa"]) : (int?)null;
                }
            }
            return nv;
        }

        public BenhNhan GetBenhNhanByMaTK(int maTK)
        {
            BenhNhan bn = null;
            using (SqlConnection con = new SqlConnection(connectionString)) // Nhớ đổi tên biến kết nối cho đúng với file của bác
            {
                string sql = "SELECT * FROM BENHNHAN WHERE MaTK = @MaTK";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaTK", maTK);

                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    bn = new BenhNhan();
                    bn.MaBN = rd["MaBN"].ToString();
                    bn.HoTen = rd["HoTen"].ToString();

                    // Mình map sẵn thêm mấy trường này luôn, lỡ sau này qua View bác cần hiển thị SDT, Email hay Profile thì gọi ra xài luôn cho tiện
                    bn.SDT = rd["SDT"] != DBNull.Value ? rd["SDT"].ToString() : "";
                    bn.Email = rd["Email"] != DBNull.Value ? rd["Email"].ToString() : "";
                    bn.CCCD = rd["CCCD"] != DBNull.Value ? rd["CCCD"].ToString() : "";
                    bn.BHYT = rd["BHYT"] != DBNull.Value ? Convert.ToBoolean(rd["BHYT"]) : false;
                }
            }
            return bn;
        }
    }
}
