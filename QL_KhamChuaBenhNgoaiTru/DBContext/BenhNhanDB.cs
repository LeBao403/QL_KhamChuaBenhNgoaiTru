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
                        NgaySinh = Convert.IsDBNull(dr["NgaySinh"]) ? DateTime.MinValue : Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = Convert.IsDBNull(dr["GioiTinh"]) ? null : dr["GioiTinh"].ToString(),
                        SDT = Convert.IsDBNull(dr["SDT"]) ? null : dr["SDT"].ToString(),
                        Email = Convert.IsDBNull(dr["Email"]) ? null : dr["Email"].ToString(),
                        DiaChi = Convert.IsDBNull(dr["DiaChi"]) ? null : dr["DiaChi"].ToString(),
                        CCCD = Convert.IsDBNull(dr["CCCD"]) ? null : dr["CCCD"].ToString(),
                        MaTK = Convert.IsDBNull(dr["MaTK"]) ? (int?)null : Convert.ToInt32(dr["MaTK"]),
                        BHYT = !Convert.IsDBNull(dr["BHYT"]) && Convert.ToBoolean(dr["BHYT"]),
                        SoTheBHYT = Convert.IsDBNull(dr["SoTheBHYT"]) ? null : dr["SoTheBHYT"].ToString(),
                        HanSuDungBHYT = Convert.IsDBNull(dr["HanSuDungBHYT"]) ? (DateTime?)null : Convert.ToDateTime(dr["HanSuDungBHYT"]),
                        TuyenKham = Convert.IsDBNull(dr["TuyenKham"]) ? null : dr["TuyenKham"].ToString(),
                        MucHuongBHYT = Convert.IsDBNull(dr["MucHuongBHYT"]) ? (int?)null : Convert.ToInt32(dr["MucHuongBHYT"])
                    };

                    TaiKhoan tk = null;
                    if (!Convert.IsDBNull(dr["Username"]) && bn.MaTK.HasValue)
                    {
                        tk = new TaiKhoan
                        {
                            MaTK = bn.MaTK.Value,
                            Username = dr["Username"].ToString(),
                            PasswordHash = Convert.IsDBNull(dr["PasswordHash"]) ? null : dr["PasswordHash"].ToString(),
                            IsActive = !Convert.IsDBNull(dr["IsActive"]) && Convert.ToBoolean(dr["IsActive"]),
                            CreatedAt = DateTime.Now
                        };
                    }

                    list.Add(new BenhNhanManageViewModel { BenhNhan = bn, TaiKhoan = tk });
                }
            }
            return list;
        }

        public List<BenhNhanManageViewModel> GetAll()
        {
            return GetAll(1, int.MaxValue);
        }

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
                        NgaySinh = Convert.IsDBNull(dr["NgaySinh"]) ? DateTime.MinValue : Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = Convert.IsDBNull(dr["GioiTinh"]) ? null : dr["GioiTinh"].ToString(),
                        SDT = Convert.IsDBNull(dr["SDT"]) ? null : dr["SDT"].ToString(),
                        Email = Convert.IsDBNull(dr["Email"]) ? null : dr["Email"].ToString(),
                        DiaChi = Convert.IsDBNull(dr["DiaChi"]) ? null : dr["DiaChi"].ToString(),
                        CCCD = Convert.IsDBNull(dr["CCCD"]) ? null : dr["CCCD"].ToString(),
                        MaTK = Convert.IsDBNull(dr["MaTK"]) ? (int?)null : Convert.ToInt32(dr["MaTK"]),
                        BHYT = !Convert.IsDBNull(dr["BHYT"]) && Convert.ToBoolean(dr["BHYT"]),
                        SoTheBHYT = Convert.IsDBNull(dr["SoTheBHYT"]) ? null : dr["SoTheBHYT"].ToString(),
                        HanSuDungBHYT = Convert.IsDBNull(dr["HanSuDungBHYT"]) ? (DateTime?)null : Convert.ToDateTime(dr["HanSuDungBHYT"]),
                        TuyenKham = Convert.IsDBNull(dr["TuyenKham"]) ? null : dr["TuyenKham"].ToString(),
                        MucHuongBHYT = Convert.IsDBNull(dr["MucHuongBHYT"]) ? (int?)null : Convert.ToInt32(dr["MucHuongBHYT"])
                    };
                }
            }
            return bn;
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
                        PasswordHash = Convert.IsDBNull(dr["PasswordHash"]) ? null : dr["PasswordHash"].ToString(),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                    };
                }
            }
            return tk;
        }

        public string GenerateNextMaBN()
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 MaBN FROM BENHNHAN ORDER BY MaBN DESC", conn);
                string lastMaBN = cmd.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(lastMaBN)) return "BN0001";
                string numberPart = System.Text.RegularExpressions.Regex.Match(lastMaBN, @"\d+").Value;
                if (int.TryParse(numberPart, out int num))
                {
                    num++;
                    return "BN" + num.ToString("D4");
                }
                return "BN0001";
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
                    if (tk != null)
                    {
                        string queryTk = @"INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt)
                                 OUTPUT INSERTED.MaTK
                                 VALUES (@Username, @Password, 1, GETDATE())";
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                        cmdTk.Parameters.AddWithValue("@Username", tk.Username);
                        cmdTk.Parameters.AddWithValue("@Password", tk.PasswordHash);
                        bn.MaTK = Convert.ToInt32(cmdTk.ExecuteScalar());
                    }

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
                    cmdBn.Parameters.AddWithValue("@MaBN", bn.MaBN.Trim());
                    cmdBn.Parameters.AddWithValue("@HoTen", bn.HoTen);
                    cmdBn.Parameters.AddWithValue("@CCCD", (object)bn.CCCD ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@SDT", (object)bn.SDT ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@Email", (object)bn.Email ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@NgaySinh", bn.NgaySinh);
                    cmdBn.Parameters.AddWithValue("@GioiTinh", (object)bn.GioiTinh ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@DiaChi", (object)bn.DiaChi ?? DBNull.Value);
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
                catch { tran.Rollback(); throw; }
            }
        }

        public bool Delete(string maBN)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    int? maTK = null;
                    string queryCheck = "SELECT MaTK FROM BENHNHAN WHERE MaBN = @MaBN";
                    SqlCommand cmdCheck = new SqlCommand(queryCheck, conn, tran);
                    cmdCheck.Parameters.AddWithValue("@MaBN", maBN.Trim());
                    object result = cmdCheck.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        maTK = Convert.ToInt32(result);

                    string queryBn = "DELETE FROM BENHNHAN WHERE MaBN = @MaBN";
                    SqlCommand cmdBn = new SqlCommand(queryBn, conn, tran);
                    cmdBn.Parameters.AddWithValue("@MaBN", maBN.Trim());
                    int rows = cmdBn.ExecuteNonQuery();
                    if (rows == 0) { tran.Rollback(); return false; }

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
                catch { tran.Rollback(); throw; }
            }
        }

        public List<BenhNhanManageViewModel> Search(string keyword)
        {
            List<BenhNhanManageViewModel> list = new List<BenhNhanManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT bn.*, tk.Username, tk.PasswordHash
                    FROM BENHNHAN bn
                    LEFT JOIN TAIKHOAN tk ON bn.MaTK = tk.MaTK
                    WHERE bn.MaBN LIKE @kw OR bn.HoTen LIKE @kw OR bn.SDT LIKE @kw";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BenhNhan bn = new BenhNhan
                    {
                        MaBN = dr["MaBN"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = Convert.IsDBNull(dr["NgaySinh"]) ? DateTime.MinValue : Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = Convert.IsDBNull(dr["GioiTinh"]) ? null : dr["GioiTinh"].ToString(),
                        SDT = Convert.IsDBNull(dr["SDT"]) ? null : dr["SDT"].ToString(),
                        Email = Convert.IsDBNull(dr["Email"]) ? null : dr["Email"].ToString(),
                        DiaChi = Convert.IsDBNull(dr["DiaChi"]) ? null : dr["DiaChi"].ToString(),
                        MaTK = Convert.IsDBNull(dr["MaTK"]) ? (int?)null : Convert.ToInt32(dr["MaTK"]),
                        CCCD = Convert.IsDBNull(dr["CCCD"]) ? null : dr["CCCD"].ToString()
                    };

                    TaiKhoan tk = null;
                    if (!Convert.IsDBNull(dr["Username"]) && bn.MaTK.HasValue)
                    {
                        tk = new TaiKhoan
                        {
                            MaTK = bn.MaTK.Value,
                            Username = dr["Username"].ToString(),
                            PasswordHash = Convert.IsDBNull(dr["PasswordHash"]) ? null : dr["PasswordHash"].ToString()
                        };
                    }

                    list.Add(new BenhNhanManageViewModel { BenhNhan = bn, TaiKhoan = tk });
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
                    if (tk != null)
                    {
                        if (tk.MaTK > 0)
                        {
                            string queryTk = "UPDATE TAIKHOAN SET Username = @U, PasswordHash = @P, IsActive = @A WHERE MaTK = @ID";
                            SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                            cmdTk.Parameters.AddWithValue("@U", tk.Username);
                            cmdTk.Parameters.AddWithValue("@P", (object)tk.PasswordHash ?? DBNull.Value);
                            cmdTk.Parameters.AddWithValue("@A", tk.IsActive);
                            cmdTk.Parameters.AddWithValue("@ID", tk.MaTK);
                            cmdTk.ExecuteNonQuery();
                            bn.MaTK = tk.MaTK;
                        }
                        else if (!string.IsNullOrWhiteSpace(tk.PasswordHash))
                        {
                            string queryTkIns = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@U, @P, @A, GETDATE())";
                            SqlCommand cmdTkIns = new SqlCommand(queryTkIns, conn, tran);
                            cmdTkIns.Parameters.AddWithValue("@U", tk.Username);
                            cmdTkIns.Parameters.AddWithValue("@P", tk.PasswordHash);
                            cmdTkIns.Parameters.AddWithValue("@A", tk.IsActive);
                            bn.MaTK = Convert.ToInt32(cmdTkIns.ExecuteScalar());
                        }
                    }

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
                    cmdBn.Parameters.AddWithValue("@GT", (object)bn.GioiTinh ?? DBNull.Value);
                    cmdBn.Parameters.AddWithValue("@DC", (object)bn.DiaChi ?? DBNull.Value);
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

        public bool BenhNhanCccdExists(string cccd)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE CCCD = @CCCD";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool BenhNhanEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool BenhNhanPhoneExists(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE SDT = @SDT";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SDT", sdt.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool BenhNhanBhytExists(string soThe)
        {
            if (string.IsNullOrWhiteSpace(soThe)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM BENHNHAN WHERE SoTheBHYT = @SoThe";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SoThe", soThe.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
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
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool UsernameExistsForAnotherAccount(string username, int currentMaTK)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM TAIKHOAN WHERE Username = @Username AND MaTK != @MaTK";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaTK", currentMaTK);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool CustomerCccdExists(string cccd, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM BENHNHAN WHERE CCCD = @CCCD";
                if (!string.IsNullOrEmpty(excludeMaKH))
                    query += " AND MaBN <> @MaKH";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool CustomerEmailExists(string email, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM BENHNHAN WHERE Email = @Email";
                if (!string.IsNullOrEmpty(excludeMaKH))
                    query += " AND MaBN <> @MaKH";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool CustomerPhoneExists(string sdt, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM BENHNHAN WHERE SDT = @SDT";
                if (!string.IsNullOrEmpty(excludeMaKH))
                    query += " AND MaBN <> @MaKH";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SDT", sdt.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
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
                            maTK = Convert.ToInt32(dr["MaTK"]);
                            currentStatus = Convert.ToBoolean(dr["IsActive"]);
                        }
                        else return null;
                    }

                    bool newStatus = !currentStatus;
                    string sqlUpdate = "UPDATE TAIKHOAN SET IsActive = @NewStatus WHERE MaTK = @MaTK";
                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, tran);
                    cmdUpdate.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmdUpdate.Parameters.AddWithValue("@MaTK", maTK);
                    cmdUpdate.ExecuteNonQuery();
                    tran.Commit();
                    return newStatus;
                }
                catch { tran.Rollback(); throw; }
            }
        }
    }
}
