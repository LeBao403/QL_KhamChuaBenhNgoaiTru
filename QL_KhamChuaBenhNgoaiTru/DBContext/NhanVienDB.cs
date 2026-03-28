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

        // Lấy tất cả nhân viên (cho Staff area)
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

        // Lấy danh sách nhân viên có phân trang
        public List<NhanVienManageViewModel> GetAll(int page, int pageSize)
        {
            List<NhanVienManageViewModel> dsNhanVien = new List<NhanVienManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT
                        NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.GioiTinh, NV.TrangThai, NV.DiaChi, NV.HinhAnh,
                        TK.Username, TK.PasswordHash, TK.IsActive,
                        CV.TenChucVu
                    FROM NHANVIEN NV
                    LEFT JOIN TAIKHOAN TK ON NV.MaTK = TK.MaTK
                    LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
                    ORDER BY NV.MaNV
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
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
                            GioiTinh = dr["GioiTinh"] == DBNull.Value ? "" : dr["GioiTinh"].ToString(),
                            SDT = dr["SDT"] == DBNull.Value ? "" : dr["SDT"].ToString(),
                            Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                            DiaChi = dr["DiaChi"] == DBNull.Value ? "" : dr["DiaChi"].ToString(),
                            Username = dr["Username"] == DBNull.Value ? null : dr["Username"].ToString(),
                            PasswordHash = dr["PasswordHash"] == DBNull.Value ? null : dr["PasswordHash"].ToString(),
                            TrangThai = dr["TrangThai"] != DBNull.Value && Convert.ToBoolean(dr["TrangThai"]),
                            HinhAnh = dr["HinhAnh"] == DBNull.Value ? null : dr["HinhAnh"].ToString()
                        });
                    }
                }
            }
            return dsNhanVien;
        }

        // Lấy danh sách có phân trang + filter
        public List<NhanVienManageViewModel> GetAll(int page, int pageSize, string keyword, string gioiTinh, int? maChucVu, bool? trangThai)
        {
            List<NhanVienManageViewModel> dsNhanVien = new List<NhanVienManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                StringBuilder queryBuilder = new StringBuilder(@"
                    SELECT
                        NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.GioiTinh, NV.TrangThai, NV.DiaChi, NV.HinhAnh,
                        TK.Username, TK.PasswordHash, TK.IsActive,
                        CV.TenChucVu
                    FROM NHANVIEN NV
                    LEFT JOIN TAIKHOAN TK ON NV.MaTK = TK.MaTK
                    LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
                    WHERE 1=1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryBuilder.Append(@" AND (NV.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR NV.SDT COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR NV.MaNV LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(gioiTinh) && gioiTinh != "all")
                {
                    queryBuilder.Append(" AND NV.GioiTinh = @GioiTinh ");
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                }

                if (maChucVu.HasValue && maChucVu > 0)
                {
                    queryBuilder.Append(" AND NV.MaChucVu = @MaChucVu ");
                    cmd.Parameters.AddWithValue("@MaChucVu", maChucVu.Value);
                }

                if (trangThai.HasValue)
                {
                    queryBuilder.Append(" AND NV.TrangThai = @TrangThai ");
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai.Value);
                }

                // Count query
                StringBuilder countBuilder = new StringBuilder(queryBuilder.ToString().Replace(
                    @"SELECT
                        NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.GioiTinh, NV.TrangThai, NV.DiaChi, NV.HinhAnh,
                        TK.Username, TK.PasswordHash, TK.IsActive,
                        CV.TenChucVu", "SELECT COUNT(*)"));

                SqlCommand cmdCount = new SqlCommand(countBuilder.ToString(), conn);
                foreach (SqlParameter p in cmd.Parameters)
                    cmdCount.Parameters.AddWithValue(p.ParameterName, p.Value);

                conn.Open();
                int totalCount = (int)cmdCount.ExecuteScalar();

                queryBuilder.Append(" ORDER BY NV.MaNV OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
                cmd.CommandText = queryBuilder.ToString();
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        dsNhanVien.Add(new NhanVienManageViewModel
                        {
                            MaNV = dr["MaNV"].ToString().Trim(),
                            HoTen = dr["HoTen"].ToString(),
                            TenChucVu = dr["TenChucVu"] == DBNull.Value ? "Chưa gán" : dr["TenChucVu"].ToString(),
                            GioiTinh = dr["GioiTinh"] == DBNull.Value ? "" : dr["GioiTinh"].ToString(),
                            SDT = dr["SDT"] == DBNull.Value ? "" : dr["SDT"].ToString(),
                            Email = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString(),
                            DiaChi = dr["DiaChi"] == DBNull.Value ? "" : dr["DiaChi"].ToString(),
                            Username = dr["Username"] == DBNull.Value ? null : dr["Username"].ToString(),
                            PasswordHash = dr["PasswordHash"] == DBNull.Value ? null : dr["PasswordHash"].ToString(),
                            TrangThai = dr["TrangThai"] != DBNull.Value && Convert.ToBoolean(dr["TrangThai"]),
                            HinhAnh = dr["HinhAnh"] == DBNull.Value ? null : dr["HinhAnh"].ToString()
                        });
                    }
                }
            }
            return dsNhanVien;
        }

        // Lấy tổng số nhân viên để phân trang
        public int GetCount()
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NHANVIEN", conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // Lấy tổng số nhân viên với filter
        public int GetCount(string keyword, string gioiTinh, int? maChucVu, bool? trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                StringBuilder queryBuilder = new StringBuilder("SELECT COUNT(*) FROM NHANVIEN NV WHERE 1=1");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryBuilder.Append(@" AND (NV.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR NV.SDT COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR NV.MaNV LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(gioiTinh) && gioiTinh != "all")
                {
                    queryBuilder.Append(" AND NV.GioiTinh = @GioiTinh ");
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                }

                if (maChucVu.HasValue && maChucVu > 0)
                {
                    queryBuilder.Append(" AND NV.MaChucVu = @MaChucVu ");
                    cmd.Parameters.AddWithValue("@MaChucVu", maChucVu.Value);
                }

                if (trangThai.HasValue)
                {
                    queryBuilder.Append(" AND NV.TrangThai = @TrangThai ");
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai.Value);
                }

                cmd.CommandText = queryBuilder.ToString();
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
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

        public List<Khoa> GetAllKhoa()
        {
            var list = new List<Khoa>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaKhoa, TenKhoa FROM KHOA WHERE TrangThai = 1 ORDER BY TenKhoa";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Khoa
                        {
                            MaKhoa = Convert.ToInt32(dr["MaKhoa"]),
                            TenKhoa = dr["TenKhoa"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<Phong> GetAllPhong()
        {
            var list = new List<Phong>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaPhong, TenPhong FROM PHONG WHERE TrangThai = 1 ORDER BY TenPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Phong
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Toggle trạng thái tài khoản nhân viên
        public bool? ToggleAccountStatus(string maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. Lấy MaTK và trạng thái hiện tại
                    string sqlGet = @"SELECT T.MaTK, T.IsActive
                             FROM NHANVIEN N JOIN TAIKHOAN T ON N.MaTK = T.MaTK
                             WHERE N.MaNV = @MaNV";
                    SqlCommand cmdGet = new SqlCommand(sqlGet, conn, tran);
                    cmdGet.Parameters.AddWithValue("@MaNV", maNV.Trim());

                    bool currentStatus = false;
                    int maTK = 0;

                    using (SqlDataReader dr = cmdGet.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            maTK = (int)dr["MaTK"];
                            currentStatus = (bool)dr["IsActive"];
                        }
                        else return null;
                    }

                    // 2. Đảo ngược trạng thái
                    bool newStatus = !currentStatus;
                    string sqlUpdate = "UPDATE TAIKHOAN SET IsActive = @NewStatus WHERE MaTK = @MaTK";
                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, tran);
                    cmdUpdate.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmdUpdate.Parameters.AddWithValue("@MaTK", maTK);
                    cmdUpdate.ExecuteNonQuery();

                    tran.Commit();
                    return newStatus;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
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
            NV.HinhAnh,
            CV.TenChucVu,
            K.TenKhoa,
            TK.Username,
            TK.PasswordHash,
            P.TenPhong
        FROM NHANVIEN NV
        LEFT JOIN CHUCVU   CV ON NV.MaChucVu = CV.MaChucVu
        LEFT JOIN KHOA     K  ON NV.MaKhoa   = K.MaKhoa
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
                        TenKhoa = dr["TenKhoa"] == DBNull.Value ? null : dr["TenKhoa"].ToString(),
                        Username = dr["Username"] == DBNull.Value ? "N/A" : dr["Username"].ToString(),
                        PasswordHash = dr["PasswordHash"] == DBNull.Value ? "N/A" : dr["PasswordHash"].ToString(),
                        HinhAnh = dr["HinhAnh"] == DBNull.Value ? null : dr["HinhAnh"].ToString(),

                        // Tên phòng lấy luôn từ bảng PHONG
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
                                 (MaNV, HoTen, NgaySinh, GioiTinh, SDT, Email, DiaChi, MaChucVu, MaPhong, TrangThai, MaTK, MaKhoa, HinhAnh)
                                 VALUES 
                                 (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @SDT, @Email, @DiaChi, @MaChucVu, @MaPhong, @TrangThai, @MaTK, @MaKhoa, @HinhAnh)";
                SqlCommand cmd = new SqlCommand(query, conn);
                AddParameters(cmd, nv);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Sinh mã NV
        public string GenerateNextMaNV()
        {
            string prefix = "NV";
            int nextNum = 1;

            using (var conn = new SqlConnection(connectStr))
            using (var cmd = new SqlCommand(@"
    SELECT MAX(TRY_CAST(SUBSTRING(MaNV, 3, LEN(MaNV)-2) AS INT))
    FROM NHANVIEN
    WHERE MaNV LIKE 'NV%';", conn))
            {
                conn.Open();
                var result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                    nextNum = Convert.ToInt32(result) + 1;
            }

            return $"{prefix}{nextNum:D3}";
        }

        // Tạo nhân viên mới (với transaction để tạo tài khoản cùng lúc)
        public bool Create(NhanVien nv, TaiKhoan tk = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. THÊM TÀI KHOẢN (NẾU CÓ)
                    int maTKCreated = 0;
                    if (tk != null && !string.IsNullOrWhiteSpace(tk.PasswordHash))
                    {
                        string queryTk = @"INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt)
                                 OUTPUT INSERTED.MaTK
                                 VALUES (@Username, @Password, 1, GETDATE())";
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                        cmdTk.Parameters.AddWithValue("@Username", tk.Username);
                        cmdTk.Parameters.AddWithValue("@Password", tk.PasswordHash);
                        maTKCreated = Convert.ToInt32(cmdTk.ExecuteScalar());
                    }

                    // 2. THÊM NHÂN VIÊN
                    string queryNv = @"INSERT INTO NHANVIEN
                                 (MaNV, HoTen, NgaySinh, GioiTinh, SDT, Email, DiaChi, MaChucVu, MaPhong, TrangThai, MaTK, MaKhoa, HinhAnh)
                                 VALUES
                                 (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @SDT, @Email, @DiaChi, @MaChucVu, @MaPhong, @TrangThai, @MaTK, @MaKhoa, @HinhAnh)";
                    SqlCommand cmdNv = new SqlCommand(queryNv, conn, tran);
                    cmdNv.Parameters.AddWithValue("@MaNV", nv.MaNV ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@HoTen", nv.HoTen ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@NgaySinh", nv.NgaySinh ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@GioiTinh", nv.GioiTinh ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@SDT", nv.SDT ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@DiaChi", nv.DiaChi ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@MaChucVu", nv.MaChucVu ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@MaPhong", nv.MaPhong ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@TrangThai", nv.TrangThai);
                    cmdNv.Parameters.AddWithValue("@MaTK", maTKCreated > 0 ? (object)maTKCreated : DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@MaKhoa", nv.MaKhoa ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@HinhAnh", nv.HinhAnh ?? (object)DBNull.Value);

                    cmdNv.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
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

        // Cập nhật nhân viên (đơn giản, ko transaction)
        public bool Update(NhanVien nv)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"UPDATE NHANVIEN
                                 SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, SDT=@SDT, Email=@Email,
                                     DiaChi=@DiaChi, MaChucVu=@MaChucVu, MaPhong=@MaPhong, TrangThai=@TrangThai, MaTK=@MaTK, MaKhoa=@MaKhoa, HinhAnh=@HinhAnh
                                 WHERE MaNV=@MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                AddParameters(cmd, nv);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Cập nhật nhân viên (có transaction để update cả tài khoản)
        public bool Update(NhanVien nv, TaiKhoan tk = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. XỬ LÝ TÀI KHOẢN
                    if (tk != null)
                    {
                        if (tk.MaTK > 0) // Cập nhật tài khoản cũ
                        {
                            string queryTk = "UPDATE TAIKHOAN SET Username = @U, PasswordHash = @P, IsActive = @A WHERE MaTK = @ID";
                            SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                            cmdTk.Parameters.AddWithValue("@U", tk.Username);
                            cmdTk.Parameters.AddWithValue("@P", tk.PasswordHash);
                            cmdTk.Parameters.AddWithValue("@A", tk.IsActive);
                            cmdTk.Parameters.AddWithValue("@ID", tk.MaTK);
                            cmdTk.ExecuteNonQuery();
                            nv.MaTK = tk.MaTK;
                        }
                        else if (!string.IsNullOrWhiteSpace(tk.PasswordHash)) // Tạo mới tài khoản
                        {
                            string queryTkIns = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@U, @P, @A, GETDATE())";
                            SqlCommand cmdTkIns = new SqlCommand(queryTkIns, conn, tran);
                            cmdTkIns.Parameters.AddWithValue("@U", tk.Username);
                            cmdTkIns.Parameters.AddWithValue("@P", tk.PasswordHash);
                            cmdTkIns.Parameters.AddWithValue("@A", tk.IsActive);
                            nv.MaTK = Convert.ToInt32(cmdTkIns.ExecuteScalar());
                        }
                    }

                    // 2. CẬP NHẬT NHÂN VIÊN
                    string queryNv = @"UPDATE NHANVIEN
                                 SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, SDT=@SDT, Email=@Email,
                                     DiaChi=@DiaChi, MaChucVu=@MaChucVu, MaPhong=@MaPhong, TrangThai=@TrangThai, MaTK=@MaTK, MaKhoa=@MaKhoa, HinhAnh=@HinhAnh
                                 WHERE MaNV=@MaNV";
                    SqlCommand cmdNv = new SqlCommand(queryNv, conn, tran);
                    cmdNv.Parameters.AddWithValue("@MaNV", nv.MaNV ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@HoTen", nv.HoTen ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@NgaySinh", nv.NgaySinh ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@GioiTinh", nv.GioiTinh ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@SDT", nv.SDT ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@Email", nv.Email ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@DiaChi", nv.DiaChi ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@MaChucVu", nv.MaChucVu ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@MaPhong", nv.MaPhong ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@TrangThai", nv.TrangThai);
                    cmdNv.Parameters.AddWithValue("@MaTK", nv.MaTK ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@MaKhoa", nv.MaKhoa ?? (object)DBNull.Value);
                    cmdNv.Parameters.AddWithValue("@HinhAnh", nv.HinhAnh ?? (object)DBNull.Value);

                    cmdNv.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
                catch { tran.Rollback(); throw; }
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

        public bool UsernameExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM TAIKHOAN WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public TaiKhoan GetTaiKhoanByMaTK(int? maTK)
        {
            if (maTK == null || maTK == 0) return null;

            TaiKhoan tk = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM TAIKHOAN WHERE MaTK = @MaTK";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaTK", maTK.Value);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    tk = new TaiKhoan
                    {
                        MaTK = Convert.ToInt32(dr["MaTK"]),
                        Username = dr["Username"].ToString(),
                        PasswordHash = dr["PasswordHash"] != DBNull.Value ? dr["PasswordHash"].ToString() : null,
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                    };
                }
            }
            return tk;
        }

        // Xóa nhân viên (có transaction xóa cả tài khoản)
        public bool Delete(string maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Bước 1: Lấy MaTK trước
                    int? maTK = null;
                    string queryCheck = "SELECT MaTK FROM NHANVIEN WHERE MaNV = @MaNV";
                    SqlCommand cmdCheck = new SqlCommand(queryCheck, conn, tran);
                    cmdCheck.Parameters.AddWithValue("@MaNV", maNV.Trim());
                    object result = cmdCheck.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        maTK = Convert.ToInt32(result);

                    // Bước 2: Xóa nhân viên
                    string queryNv = "DELETE FROM NHANVIEN WHERE MaNV = @MaNV";
                    SqlCommand cmdNv = new SqlCommand(queryNv, conn, tran);
                    cmdNv.Parameters.AddWithValue("@MaNV", maNV.Trim());
                    int rows = cmdNv.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        tran.Rollback();
                        return false;
                    }

                    // Bước 3: Xóa tài khoản nếu có
                    if (maTK.HasValue)
                    {
                        string queryTk = "DELETE FROM TAIKHOAN WHERE MaTK = @MaTK";
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                        cmdTk.Parameters.AddWithValue("@MaTK", maTK.Value);
                        cmdTk.ExecuteNonQuery();
                    }

                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
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
                MaKhoa = dr["MaKhoa"] == DBNull.Value ? null : (int?)dr["MaKhoa"],
                HinhAnh = dr["HinhAnh"] == DBNull.Value ? null : dr["HinhAnh"].ToString()
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
            cmd.Parameters.AddWithValue("@MaKhoa", nv.MaKhoa ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HinhAnh", nv.HinhAnh ?? (object)DBNull.Value);
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