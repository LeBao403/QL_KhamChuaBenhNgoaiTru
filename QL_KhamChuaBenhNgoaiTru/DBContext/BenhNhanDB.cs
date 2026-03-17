using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class BenhNhanDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public List<BenhNhanManageViewModel> GetAll(int page = 1, int pageSize = 10)
        {
            List<BenhNhanManageViewModel> list = new List<BenhNhanManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // === SỬA: Bổ sung tk.IsActive vào câu SELECT ===
                string query = @"
        SELECT 
            bn.*, 
            tk.Username, tk.PasswordHash, tk.IsActive
        FROM BENHNHAN bn
        LEFT JOIN TAIKHOAN tk ON bn.MaTK = tk.MaTK
        ORDER BY bn.MaBN
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BenhNhan bn = new BenhNhan
                    {
                        MaBN = dr["MaBN"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = dr["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySinh"]) : DateTime.MinValue,
                        GioiTinh = dr["GioiTinh"] != DBNull.Value ? dr["GioiTinh"].ToString() : null,
                        SDT = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : null,
                        Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null,
                        DiaChi = dr["DiaChi"] != DBNull.Value ? dr["DiaChi"].ToString() : null,
                        CCCD = dr["CCCD"] != DBNull.Value ? dr["CCCD"].ToString() : null,
                        MaTK = dr["MaTK"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaTK"]) : null,

                        BHYT = dr["BHYT"] != DBNull.Value ? Convert.ToBoolean(dr["BHYT"]) : false,
                        SoTheBHYT = dr["SoTheBHYT"] != DBNull.Value ? dr["SoTheBHYT"].ToString() : null,
                        HanSuDungBHYT = dr["HanSuDungBHYT"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HanSuDungBHYT"]) : null,
                        TuyenKham = dr["TuyenKham"] != DBNull.Value ? dr["TuyenKham"].ToString() : null,
                        MucHuongBHYT = dr["MucHuongBHYT"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MucHuongBHYT"]) : null
                    };

                    TaiKhoan tk = null;
                    if (dr["Username"] != DBNull.Value)
                    {
                        if (bn.MaTK.HasValue)
                        {
                            tk = new TaiKhoan
                            {
                                MaTK = bn.MaTK.Value,
                                Username = dr["Username"].ToString(),
                                PasswordHash = dr["PasswordHash"] != DBNull.Value ? dr["PasswordHash"].ToString() : null,
                                // === SỬA: Đọc giá trị IsActive từ DB lên ===
                                IsActive = dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false
                            };
                        }
                    }

                    list.Add(new BenhNhanManageViewModel { BenhNhan = bn, TaiKhoan = tk });
                }
            }
            return list;
        }

        // --- Lấy tổng số khách hàng để phân trang ---
        public int GetCount()
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM BENHNHAN", conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }


        public BenhNhan GetById(string maBN)
        {
            BenhNhan bn = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM BENHNHAN WHERE MaBN = @MaBN";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN.Trim());

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    bn = new BenhNhan
                    {
                        MaBN = dr["MaBN"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        // Check null an toàn
                        NgaySinh = dr["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySinh"]) : DateTime.MinValue,
                        GioiTinh = dr["GioiTinh"] != DBNull.Value ? dr["GioiTinh"].ToString() : null,
                        SDT = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : null,
                        Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null,
                        DiaChi = dr["DiaChi"] != DBNull.Value ? dr["DiaChi"].ToString() : null,
                        CCCD = dr["CCCD"] != DBNull.Value ? dr["CCCD"].ToString() : null,
                        MaTK = dr["MaTK"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaTK"]) : null,

                        // === BỔ SUNG CÁC CỘT BHYT ĐỂ ĐỒNG BỘ ===
                        BHYT = dr["BHYT"] != DBNull.Value ? Convert.ToBoolean(dr["BHYT"]) : false,
                        SoTheBHYT = dr["SoTheBHYT"] != DBNull.Value ? dr["SoTheBHYT"].ToString() : null,
                        HanSuDungBHYT = dr["HanSuDungBHYT"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["HanSuDungBHYT"]) : null,
                        TuyenKham = dr["TuyenKham"] != DBNull.Value ? dr["TuyenKham"].ToString() : null,
                        MucHuongBHYT = dr["MucHuongBHYT"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MucHuongBHYT"]) : null
                    };
                }
            }
            return bn;
        }

        public bool Delete(string maBN)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction(); // Bắt đầu giao dịch

                try
                {
                    // Bước 1: Lấy MaTK của bệnh nhân này trước khi xóa bệnh nhân
                    int? maTK = null;
                    string queryCheck = "SELECT MaTK FROM BENHNHAN WHERE MaBN = @MaBN";
                    SqlCommand cmdCheck = new SqlCommand(queryCheck, conn, tran);
                    cmdCheck.Parameters.AddWithValue("@MaBN", maBN.Trim());

                    object result = cmdCheck.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        maTK = Convert.ToInt32(result);
                    }

                    // Bước 2: Xóa hồ sơ Bệnh nhân trước
                    // (Vì bảng BENHNHAN chứa khóa ngoại MaTK trỏ tới bảng TAIKHOAN)
                    string queryBn = "DELETE FROM BENHNHAN WHERE MaBN = @MaBN";
                    SqlCommand cmdBn = new SqlCommand(queryBn, conn, tran);
                    cmdBn.Parameters.AddWithValue("@MaBN", maBN.Trim());
                    int rows = cmdBn.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        tran.Rollback();
                        return false;
                    }

                    // Bước 3: Nếu bệnh nhân có tài khoản, xóa nốt bên bảng TAIKHOAN
                    if (maTK.HasValue)
                    {
                        string queryTk = "DELETE FROM TAIKHOAN WHERE MaTK = @MaTK";
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                        cmdTk.Parameters.AddWithValue("@MaTK", maTK.Value);
                        cmdTk.ExecuteNonQuery();
                    }

                    tran.Commit(); // Hoàn tất giao dịch xóa cả 2 bảng
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback(); // Nếu có bất kỳ lỗi gì, khôi phục lại dữ liệu ban đầu
                    throw;
                }
            }
        }

        public bool Create(BenhNhan bn, TaiKhoan tk = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. THÊM TÀI KHOẢN (NẾU CÓ)
                    if (tk != null)
                    {
                        string queryTk = @"INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) 
                                 OUTPUT INSERTED.MaTK 
                                 VALUES (@Username, @Password, 1)";
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                        cmdTk.Parameters.AddWithValue("@Username", tk.Username);
                        cmdTk.Parameters.AddWithValue("@Password", tk.PasswordHash);

                        // Sửa: Dùng Convert.ToInt32 để tránh lỗi ép kiểu Identity
                        bn.MaTK = Convert.ToInt32(cmdTk.ExecuteScalar());
                    }

                    // 2. THÊM BỆNH NHÂN (Map đầy đủ các cột BHYT mới)
                    string queryBn = @"
                INSERT INTO BENHNHAN (
                    MaBN, HoTen, CCCD, SDT, Email, NgaySinh, GioiTinh, DiaChi, 
                    BHYT, SoTheBHYT, HanSuDungBHYT, TuyenKham, MucHuongBHYT, MaTK
                )
                VALUES (
                    @MaBN, @HoTen, @CCCD, @SDT, @Email, @NgaySinh, @GioiTinh, @DiaChi, 
                    @BHYT, @SoTheBHYT, @HanSuDungBHYT, @TuyenKham, @MucHuongBHYT, @MaTK
                )";

                    SqlCommand cmdBn = new SqlCommand(queryBn, conn, tran);
                    cmdBn.Parameters.AddWithValue("@MaBN", bn.MaBN.Trim()); // Đảm bảo không dư khoảng trắng
                    cmdBn.Parameters.AddWithValue("@HoTen", bn.HoTen);
                    cmdBn.Parameters.AddWithValue("@CCCD", (object)bn.CCCD ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@SDT", (object)bn.SDT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@Email", (object)bn.Email ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@NgaySinh", bn.NgaySinh);
                    cmdBn.Parameters.AddWithValue("@GioiTinh", bn.GioiTinh);
                    cmdBn.Parameters.AddWithValue("@DiaChi", bn.DiaChi);

                    cmdBn.Parameters.AddWithValue("@BHYT", bn.BHYT);
                    cmdBn.Parameters.AddWithValue("@SoTheBHYT", (object)bn.SoTheBHYT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@HanSuDungBHYT", (object)bn.HanSuDungBHYT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@TuyenKham", (object)bn.TuyenKham ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@MucHuongBHYT", (object)bn.MucHuongBHYT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@MaTK", (object)bn.MaTK ?? DBNull.Value);

                    cmdBn.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex; // Quăng lỗi thật sự ra để Controller bắt
                }
            }
        }

        public string GenerateNextMaBN()
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                // Lấy mã lớn nhất hiện tại
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 MaBN FROM BENHNHAN ORDER BY MaBN DESC", conn);
                string lastMaBN = cmd.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(lastMaBN)) return "BN0001";

                // Dùng Regex để chỉ lấy các ký tự số, loại bỏ "BN" và các khoảng trắng dư thừa của CHAR(10)
                string numberPart = System.Text.RegularExpressions.Regex.Match(lastMaBN, @"\d+").Value;

                if (int.TryParse(numberPart, out int num))
                {
                    num++;
                    return "BN" + num.ToString("D4");
                }

                return "BN0001"; // Phòng hờ mã lỗi thì reset về đầu
            }
        }

        // Kiểm tra trùng CCCD
        public bool BenhNhanCccdExists(string cccd)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return false; // Nếu không nhập thì coi như không trùng

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE CCCD = @CCCD";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd.Trim());
                conn.Open();

                // Dùng Convert.ToInt32 cho chắc chắn vì ExecuteScalar trả về object
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Kiểm tra trùng Email
        public bool BenhNhanEmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Kiểm tra trùng Số điện thoại
        public bool BenhNhanPhoneExists(string sdt)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE SDT = @SDT";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SDT", sdt.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Kiểm tra trùng Username (Bảng TAIKHOAN)
        public bool UsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM TAIKHOAN WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool BenhNhanBhytExists(string soThe)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE SoTheBHYT = @SoThe";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SoThe", soThe.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public List<BenhNhanManageViewModel> Search(string keyword)
        {
            List<BenhNhanManageViewModel> list = new List<BenhNhanManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT 
                        bn.*, 
                        tk.Username, tk.PasswordHash
                    FROM BENHNHAN bn
                    LEFT JOIN TAIKHOAN tk ON bn.MaTK = tk.MaTK
                    WHERE bn.MaBN LIKE @kw OR bn.HoTen LIKE @kw OR bn.SDT LIKE @kw";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // 1. Tạo đối tượng BenhNhan (model DB)
                    BenhNhan bn = new BenhNhan
                    {
                        MaBN = dr["MaBN"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = dr["GioiTinh"].ToString(),
                        SDT = dr["SDT"].ToString(),
                        Email = dr["Email"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        MaTK = dr["MaTK"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaTK"]) : null,
                        CCCD = dr["CCCD"] != DBNull.Value ? dr["CCCD"].ToString() : null
                    };

                    // 2. Tạo đối tượng TaiKhoan (nếu có)
                    TaiKhoan tk = null;
                    if (dr["Username"] != DBNull.Value)
                    {
                        tk = new TaiKhoan
                        {
                            MaTK = bn.MaTK.Value,
                            Username = dr["Username"].ToString(),
                            PasswordHash = dr["PasswordHash"] != DBNull.Value ? dr["PasswordHash"].ToString() : null
                        };
                    }

                    // 3. Đóng gói vào ViewModel
                    list.Add(new BenhNhanManageViewModel
                    {
                        BenhNhan = bn,
                        TaiKhoan = tk
                    });
                }
            }
            return list;
        }

        public bool Update(BenhNhan bn, TaiKhoan tk = null)
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
                            bn.MaTK = tk.MaTK;
                        }
                        else if (!string.IsNullOrWhiteSpace(tk.PasswordHash)) // Tạo mới tài khoản cho BN cũ
                        {
                            string queryTkIns = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) OUTPUT INSERTED.MaTK VALUES (@U, @P, @A)";
                            SqlCommand cmdTkIns = new SqlCommand(queryTkIns, conn, tran);
                            cmdTkIns.Parameters.AddWithValue("@U", tk.Username);
                            cmdTkIns.Parameters.AddWithValue("@P", tk.PasswordHash);
                            cmdTkIns.Parameters.AddWithValue("@A", tk.IsActive);
                            bn.MaTK = Convert.ToInt32(cmdTkIns.ExecuteScalar());
                        }
                    }

                    // 2. CẬP NHẬT BỆNH NHÂN (Map đủ 100% cột mới)
                    string queryBn = @"UPDATE BENHNHAN SET 
                                HoTen=@HT, CCCD=@CC, SDT=@SD, Email=@EM, NgaySinh=@NS, GioiTinh=@GT, DiaChi=@DC, 
                                BHYT=@BY, SoTheBHYT=@ST, HanSuDungBHYT=@HS, TuyenKham=@TK, MucHuongBHYT=@MH, MaTK=@MTK
                               WHERE MaBN=@ID";

                    SqlCommand cmdBn = new SqlCommand(queryBn, conn, tran);
                    cmdBn.Parameters.AddWithValue("@HT", bn.HoTen);
                    cmdBn.Parameters.AddWithValue("@CC", (object)bn.CCCD ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@SD", (object)bn.SDT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@EM", (object)bn.Email ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@NS", bn.NgaySinh);
                    cmdBn.Parameters.AddWithValue("@GT", bn.GioiTinh);
                    cmdBn.Parameters.AddWithValue("@DC", bn.DiaChi);
                    cmdBn.Parameters.AddWithValue("@BY", bn.BHYT);
                    cmdBn.Parameters.AddWithValue("@ST", (object)bn.SoTheBHYT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@HS", (object)bn.HanSuDungBHYT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@TK", (object)bn.TuyenKham ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@MH", (object)bn.MucHuongBHYT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@MTK", (object)bn.MaTK ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@ID", bn.MaBN.Trim());

                    cmdBn.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
                catch { tran.Rollback(); throw; }
            }
        }
        public bool UsernameExistsForAnotherAccount(string username, int currentMaTK)
        {
            if (string.IsNullOrEmpty(username)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Kiểm tra xem username này có tồn tại VÀ MaTK không phải là của tài khoản hiện tại
                string query = "SELECT COUNT(1) FROM TAIKHOAN WHERE Username = @Username AND MaTK != @MaTK";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaTK", currentMaTK); // ID của tài khoản đang sửa

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public TaiKhoan GetTaiKhoanByMaTK(int? maTK)
        {
            // Nếu không có MaTK thì không cần tìm
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
                        // Lấy cả mật khẩu (đang là plain text)
                        PasswordHash = dr["PasswordHash"].ToString(),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                    };
                }
            }
            return tk;
        }
        
        // Kiểm tra CCCD đã tồn tại trong bảng BenhNhan hay chưa
        // excludeMaKH: truyền MaKH hiện tại khi sửa để bỏ qua chính nó
        public bool CustomerCccdExists(string cccd, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM BenhNhan WHERE CCCD = @CCCD";
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    query += " AND MaKH <> @MaKH";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Kiểm tra Email đã tồn tại trong bảng BenhNhan hay chưa
        public bool CustomerEmailExists(string email, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM BenhNhan WHERE Email = @Email";
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    query += " AND MaKH <> @MaKH";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Kiểm tra SĐT đã tồn tại trong bảng BenhNhan hay chưa
        public bool CustomerPhoneExists(string sdt, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM BenhNhan WHERE SDT = @SDT";
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    query += " AND MaKH <> @MaKH";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SDT", sdt.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool? ToggleAccountStatus(string maBN)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. Lấy MaTK và trạng thái hiện tại
                    string sqlGet = @"SELECT T.MaTK, T.IsActive 
                             FROM BENHNHAN B JOIN TAIKHOAN T ON B.MaTK = T.MaTK 
                             WHERE B.MaBN = @MaBN";
                    SqlCommand cmdGet = new SqlCommand(sqlGet, conn, tran);
                    cmdGet.Parameters.AddWithValue("@MaBN", maBN.Trim());

                    bool currentStatus = false;
                    int maTK = 0;

                    using (SqlDataReader dr = cmdGet.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            maTK = (int)dr["MaTK"];
                            currentStatus = (bool)dr["IsActive"];
                        }
                        else return null; // Không tìm thấy tài khoản
                    }

                    // 2. Đảo ngược trạng thái
                    bool newStatus = !currentStatus;
                    string sqlUpdate = "UPDATE TAIKHOAN SET IsActive = @NewStatus WHERE MaTK = @MaTK";
                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, tran);
                    cmdUpdate.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmdUpdate.Parameters.AddWithValue("@MaTK", maTK);

                    cmdUpdate.ExecuteNonQuery();
                    tran.Commit();

                    return newStatus; // Trả về trạng thái mới để Controller biết đường mà báo cáo
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}