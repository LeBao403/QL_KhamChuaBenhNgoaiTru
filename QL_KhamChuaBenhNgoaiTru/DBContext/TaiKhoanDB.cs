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
            TaiKhoan tk = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM TAIKHOAN WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1";
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
                }
            }
            return nv;
        }
    }
}