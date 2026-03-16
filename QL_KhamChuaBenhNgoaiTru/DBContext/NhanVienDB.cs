using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class NhanVienDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // Lấy tất cả nhân viên
        public List<NhanVien> GetAll()
        {
            var list = new List<NhanVien>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM NHANVIEN";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(ReadNhanVien(dr));
                }
            }
            return list;
        }

        // Lấy nhân viên theo MaNV
        public NhanVien GetById(string maNV)
        {
            NhanVien nv = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM NHANVIEN WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                conn.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    nv = ReadNhanVien(dr);
                }
            }
            return nv;
        }

        public List<ChucVu> GetAllChucVu()
        {
            var list = new List<ChucVu>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaChucVu, TenChucVu FROM CHUCVU ORDER BY TenChucVu";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ChucVu
                        {
                            MaChucVu = Convert.ToInt32(dr["MaChucVu"]),
                            TenChucVu = dr["TenChucVu"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<NhanVienManageViewModel> SearchNhanVien(string keyword = "", int? maChucVu = null, int? maCoSo = null)
        {
            List<NhanVienManageViewModel> dsNhanVien = new List<NhanVienManageViewModel>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                StringBuilder queryBuilder = new StringBuilder(@"
        SELECT
            NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.GioiTinh, NV.TrangThai, NV.DiaChi, 
            TK.Username, TK.PasswordHash,
            CV.TenChucVu,
            CS.TenCoSo
        FROM NHANVIEN NV
        LEFT JOIN TAIKHOAN TK ON NV.MaTK = TK.MaTK
        LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
        LEFT JOIN COSOTIEM CS ON NV.MaCoSo = CS.MaCoSo
        WHERE 1=1 AND NV.TrangThai = 1 ");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    string searchPattern = "%" + keyword.Trim() + "%";
                    queryBuilder.Append(@" AND (NV.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword 
                                     OR NV.SDT COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR NV.MaNV LIKE @Keyword) ");
                    cmd.Parameters.AddWithValue("@Keyword", searchPattern);
                }

                if (maChucVu.HasValue && maChucVu > 0)
                {
                    queryBuilder.Append(" AND NV.MaChucVu = @MaChucVu ");
                    cmd.Parameters.AddWithValue("@MaChucVu", maChucVu.Value);
                }

                if (maCoSo.HasValue && maCoSo > 0)
                {
                    queryBuilder.Append(" AND NV.MaCoSo = @MaCoSo ");
                    cmd.Parameters.AddWithValue("@MaCoSo", maCoSo.Value);
                }

                queryBuilder.Append(" ORDER BY NV.MaNV");

                cmd.CommandText = queryBuilder.ToString();
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        dsNhanVien.Add(new NhanVienManageViewModel
                        {
                            MaNV = dr["MaNV"].ToString().Trim(),
                            HoTen = dr["HoTen"].ToString(),
                            TenChucVu = dr["TenChucVu"] == DBNull.Value ? "Chưa gán" : dr["TenChucVu"].ToString(),
                            TenCoSo = dr["TenCoSo"] == DBNull.Value ? "Chưa gán" : dr["TenCoSo"].ToString(),
                            GioiTinh = dr["GioiTinh"] == DBNull.Value ? "" : dr["GioiTinh"].ToString(),
                            SDT = dr["SDT"] == DBNull.Value ? "" : dr["SDT"].ToString(),
                            Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                            DiaChi = dr["DiaChi"] == DBNull.Value ? "" : dr["DiaChi"].ToString(),
                            Username = dr["Username"] == DBNull.Value ? "N/A" : dr["Username"].ToString(),
                            PasswordHash = dr["PasswordHash"] == DBNull.Value ? "N/A" : dr["PasswordHash"].ToString(),
                            TrangThai = Convert.ToBoolean(dr["TrangThai"])
                        });
                    }
                }
            }
            return dsNhanVien;
        }

        public NhanVienManageViewModel GetNhanVienDetailsById(string maNV)
        {
            if (string.IsNullOrWhiteSpace(maNV))
                return null;

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
        SELECT
            NV.MaNV,
            NV.HoTen,
            NV.SDT,
            NV.Email,
            NV.GioiTinh,
            NV.DiaChi,
            NV.TrangThai,
            CV.TenChucVu,
            CS.TenCoSo,
            TK.Username,
            TK.PasswordHash,
            P.TenPhong
        FROM NHANVIEN NV
        LEFT JOIN CHUCVU   CV ON NV.MaChucVu = CV.MaChucVu
        LEFT JOIN COSOTIEM CS ON NV.MaCoSo   = CS.MaCoSo
        LEFT JOIN TAIKHOAN TK ON NV.MaTK     = TK.MaTK
        LEFT JOIN PHONG    P  ON NV.MaPhong  = P.MaPhong
        WHERE NV.MaNV = @MaNV;
    ", conn))
            {
                cmd.Parameters.AddWithValue("@MaNV", maNV.Trim());
                conn.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    return new NhanVienManageViewModel
                    {
                        MaNV = dr["MaNV"].ToString().Trim(),
                        HoTen = dr["HoTen"].ToString(),
                        SDT = dr["SDT"] == DBNull.Value ? "" : dr["SDT"].ToString(),
                        Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                        GioiTinh = dr["GioiTinh"] == DBNull.Value ? "" : dr["GioiTinh"].ToString(),
                        DiaChi = dr["DiaChi"] == DBNull.Value ? "" : dr["DiaChi"].ToString(),
                        TrangThai = dr["TrangThai"] != DBNull.Value && Convert.ToBoolean(dr["TrangThai"]),
                        TenChucVu = dr["TenChucVu"] == DBNull.Value ? "Chưa gán" : dr["TenChucVu"].ToString(),
                        TenCoSo = dr["TenCoSo"] == DBNull.Value ? "Chưa gán" : dr["TenCoSo"].ToString(),
                        Username = dr["Username"] == DBNull.Value ? "N/A" : dr["Username"].ToString(),
                        PasswordHash = dr["PasswordHash"] == DBNull.Value ? "N/A" : dr["PasswordHash"].ToString(),

                        // 👉 Tên phòng lấy luôn từ bảng PHONG
                        TenPhong = dr["TenPhong"] == DBNull.Value ? null : dr["TenPhong"].ToString()
                    };
                }
            }
        }

        

        // Thêm nhân viên
        public bool Insert(NhanVien nv)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"INSERT INTO NHANVIEN 
                                 (MaNV, HoTen, NgaySinh, GioiTinh, SDT, Email, DiaChi, MaChucVu, MaCoSo, MaPhong, TrangThai, MaTK)
                                 VALUES 
                                 (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @SDT, @Email, @DiaChi, @MaChucVu, @MaCoSo, @MaPhong, @TrangThai, @MaTK)";
                SqlCommand cmd = new SqlCommand(query, conn);
                AddParameters(cmd, nv);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Sinh mã NV
        public string GenerateMaNV()
        {
            string prefix = "NV";
            int nextNum = 1;

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
    SELECT MAX(TRY_CAST(SUBSTRING(MaNV, 3, LEN(MaNV)-2) AS INT))
    FROM NHANVIEN
    WHERE MaNV LIKE 'NV%';  -- chỉ lấy những mã NVxxx
", conn))
            {
                conn.Open();
                var result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                    nextNum = Convert.ToInt32(result) + 1;
            }

            return $"{prefix}{nextNum:D3}";
        }

        // Tạo tài khoản cho nhân viên
        public int CreateTaiKhoanNV(string username, string rawPassword, int? maChucVu)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Thiếu thông tin tài khoản.");

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
        INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt)
        OUTPUT INSERTED.MaTK
        VALUES (@u, @p, 1, GETDATE());
    ", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", rawPassword); // lưu raw
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // 1) Lấy Username theo MaTK
        public string GetUsernameByMaTK(int? maTK)
        {
            if (!maTK.HasValue) return null;

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand("SELECT Username FROM TAIKHOAN WHERE MaTK = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", maTK.Value);
                conn.Open();
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? null : o.ToString();
            }
        }

        public void UpdateTaiKhoanUsername(int maTK, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new ArgumentException("Username mới không được trống.", nameof(newUsername));

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
        UPDATE TAIKHOAN SET Username = @u WHERE MaTK = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@u", newUsername.Trim());
                cmd.Parameters.AddWithValue("@id", maTK);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateTaiKhoanPassword(int maTK, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Mật khẩu mới không được để trống.", nameof(rawPassword));

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
        UPDATE TAIKHOAN
        SET PasswordHash = @p
        WHERE MaTK = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", maTK);
                cmd.Parameters.AddWithValue("@p", rawPassword); // lưu raw
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Cập nhật nhân viên
        public bool Update(NhanVien nv)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"UPDATE NHANVIEN 
                                 SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, SDT=@SDT, Email=@Email,
                                     DiaChi=@DiaChi, MaChucVu=@MaChucVu, MaCoSo=@MaCoSo, MaPhong=@MaPhong, TrangThai=@TrangThai, MaTK=@MaTK
                                 WHERE MaNV=@MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                AddParameters(cmd, nv);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Xóa nhân viên
        public bool Delete(string maNV)
        {
            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand("DELETE FROM NHANVIEN WHERE MaNV = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", maNV.Trim());
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool EmailExists(string email, string excludeMaNV = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE Email=@Email";
                if (!string.IsNullOrEmpty(excludeMaNV))
                    query += " AND MaNV <> @MaNV";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                if (!string.IsNullOrEmpty(excludeMaNV))
                    cmd.Parameters.AddWithValue("@MaNV", excludeMaNV);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool SDTExists(string sdt, string excludeMaNV = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE SDT=@SDT";
                if (!string.IsNullOrEmpty(excludeMaNV))
                    query += " AND MaNV <> @MaNV";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SDT", sdt);
                if (!string.IsNullOrEmpty(excludeMaNV))
                    cmd.Parameters.AddWithValue("@MaNV", excludeMaNV);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Đọc NhanVien từ SqlDataReader
        private NhanVien ReadNhanVien(SqlDataReader dr)
        {
            return new NhanVien
            {
                MaNV = dr["MaNV"].ToString(),
                HoTen = dr["HoTen"].ToString(),
                NgaySinh = dr["NgaySinh"] == DBNull.Value ? null : (DateTime?)dr["NgaySinh"],
                GioiTinh = dr["GioiTinh"] == DBNull.Value ? null : dr["GioiTinh"].ToString(),
                SDT = dr["SDT"] == DBNull.Value ? null : dr["SDT"].ToString(),
                Email = dr["Email"] == DBNull.Value ? null : dr["Email"].ToString(),
                DiaChi = dr["DiaChi"] == DBNull.Value ? null : dr["DiaChi"].ToString(),
                MaChucVu = dr["MaChucVu"] == DBNull.Value ? null : (int?)dr["MaChucVu"],
                MaPhong = dr["MaPhong"] == DBNull.Value ? null : (int?)dr["MaPhong"],
                TrangThai = dr["TrangThai"] == DBNull.Value ? false : Convert.ToBoolean(dr["TrangThai"]),
                MaTK = dr["MaTK"] == DBNull.Value ? null : (int?)dr["MaTK"],
            };
        }

        // Thêm tham số cho SqlCommand
        private void AddParameters(SqlCommand cmd, NhanVien nv)
        {
            cmd.Parameters.AddWithValue("@MaNV", nv.MaNV ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HoTen", nv.HoTen ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NgaySinh", nv.NgaySinh ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@GioiTinh", nv.GioiTinh ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SDT", nv.SDT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DiaChi", nv.DiaChi ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MaChucVu", nv.MaChucVu ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MaPhong", nv.MaPhong ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai);
            cmd.Parameters.AddWithValue("@MaTK", nv.MaTK ?? (object)DBNull.Value);
        }
        public string GetMaNVByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
        SELECT nv.MaNV
        FROM NHANVIEN nv
        INNER JOIN TAIKHOAN tk ON nv.MaTK = tk.MaTK
        WHERE tk.Username = @u;
    ", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                conn.Open();

                var obj = cmd.ExecuteScalar();
                return (obj == null || obj == DBNull.Value)
                    ? null
                    : obj.ToString();
            }
        }
    }

}