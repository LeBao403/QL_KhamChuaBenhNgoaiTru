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
                    WHERE (tk.Username = @Username OR bn.SDT = @Username)
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
                    WHERE tk.Username = @Username OR bn.SDT = @Username";
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
