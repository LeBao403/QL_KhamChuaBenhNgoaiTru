using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class ThuocDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // ==================== LẤY DROPDOWN DATA ====================

        public List<NhaSanXuat> GetAllNSX()
        {
            var list = new List<NhaSanXuat>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaNSX, TenNSX FROM NHASANXUAT ORDER BY TenNSX";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new NhaSanXuat
                        {
                            MaNSX = Convert.ToInt32(dr["MaNSX"]),
                            TenNSX = dr["TenNSX"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<DanhMucHoatChat> GetAllHoatChat()
        {
            var list = new List<DanhMucHoatChat>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaHoatChat, TenHoatChat FROM DANHMUC_HOATCHAT ORDER BY TenHoatChat";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new DanhMucHoatChat
                        {
                            MaHoatChat = dr["MaHoatChat"].ToString(),
                            TenHoatChat = dr["TenHoatChat"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<KhoNhapDB.LoaiThuocItem> GetAllLoaiThuoc()
        {
            var list = new List<KhoNhapDB.LoaiThuocItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaDanhMuc AS MaLoaiThuoc, TenDanhMuc AS TenLoaiThuoc FROM DANHMUC_THUOC ORDER BY TenDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new KhoNhapDB.LoaiThuocItem
                        {
                            MaLoaiThuoc = dr["MaLoaiThuoc"] != DBNull.Value ? dr["MaLoaiThuoc"].ToString().Trim() : "",
                            TenLoaiThuoc = dr["TenLoaiThuoc"] != DBNull.Value ? dr["TenLoaiThuoc"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public List<string> GetAllDuongDung()
        {
            var list = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT DISTINCT DuongDung FROM THUOC WHERE DuongDung IS NOT NULL ORDER BY DuongDung";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(dr["DuongDung"].ToString());
                    }
                }
            }
            return list;
        }

        // ==================== LẤY DANH SÁCH CÓ FILTER + PHÂN TRANG ====================

        public List<ThuocManageViewModel> GetAll(int page, int pageSize, string keyword, string maLoaiThuoc, string duongDung, bool? coBHYT, bool? trangThai)
        {
            var list = new List<ThuocManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                StringBuilder queryBuilder = new StringBuilder(@"
                    SELECT
                        T.MaThuoc, T.TenThuoc, T.QuyCach, T.DonViCoBan, T.MaLoaiThuoc,
                        T.DuongDung, T.GiaBan, T.CoBHYT, T.MaNSX, T.TrangThai,
                        NSX.TenNSX
                    FROM THUOC T
                    LEFT JOIN NHASANXUAT NSX ON T.MaNSX = NSX.MaNSX
                    WHERE 1=1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryBuilder.Append(@" AND (T.TenThuoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR T.MaThuoc LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(maLoaiThuoc))
                {
                    queryBuilder.Append(" AND T.MaLoaiThuoc = @MaLoaiThuoc ");
                    cmd.Parameters.AddWithValue("@MaLoaiThuoc", maLoaiThuoc);
                }

                if (!string.IsNullOrEmpty(duongDung))
                {
                    queryBuilder.Append(" AND T.DuongDung = @DuongDung ");
                    cmd.Parameters.AddWithValue("@DuongDung", duongDung);
                }

                if (coBHYT.HasValue)
                {
                    queryBuilder.Append(" AND T.CoBHYT = @CoBHYT ");
                    cmd.Parameters.AddWithValue("@CoBHYT", coBHYT.Value);
                }

                if (trangThai.HasValue)
                {
                    queryBuilder.Append(" AND T.TrangThai = @TrangThai ");
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai.Value);
                }

                queryBuilder.Append(" ORDER BY T.MaThuoc OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
                cmd.CommandText = queryBuilder.ToString();
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ThuocManageViewModel
                        {
                            MaThuoc = dr["MaThuoc"].ToString().Trim(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            QuyCach = dr["QuyCach"] != DBNull.Value ? dr["QuyCach"].ToString() : null,
                            DonViCoBan = dr["DonViCoBan"].ToString(),
                            MaLoaiThuoc = dr["MaLoaiThuoc"] != DBNull.Value ? dr["MaLoaiThuoc"].ToString() : null,
                            DuongDung = dr["DuongDung"] != DBNull.Value ? dr["DuongDung"].ToString() : null,
                            GiaBan = dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : (decimal?)null,
                            CoBHYT = dr["CoBHYT"] != DBNull.Value && Convert.ToBoolean(dr["CoBHYT"]),
                            // Bỏ lấy GiaBHYT
                            MaNSX = dr["MaNSX"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaNSX"]) : null,
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : null,
                            TrangThai = dr["TrangThai"] != DBNull.Value && Convert.ToBoolean(dr["TrangThai"])
                        });
                    }
                }
            }
            return list;
        }

        public int GetCount(string keyword, string maLoaiThuoc, string duongDung, bool? coBHYT, bool? trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                StringBuilder queryBuilder = new StringBuilder("SELECT COUNT(*) FROM THUOC T WHERE 1=1");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    queryBuilder.Append(@" AND (T.TenThuoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                     OR T.MaThuoc LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(maLoaiThuoc))
                {
                    queryBuilder.Append(" AND T.MaLoaiThuoc = @MaLoaiThuoc ");
                    cmd.Parameters.AddWithValue("@MaLoaiThuoc", maLoaiThuoc);
                }

                if (!string.IsNullOrEmpty(duongDung))
                {
                    queryBuilder.Append(" AND T.DuongDung = @DuongDung ");
                    cmd.Parameters.AddWithValue("@DuongDung", duongDung);
                }

                if (coBHYT.HasValue)
                {
                    queryBuilder.Append(" AND T.CoBHYT = @CoBHYT ");
                    cmd.Parameters.AddWithValue("@CoBHYT", coBHYT.Value);
                }

                if (trangThai.HasValue)
                {
                    queryBuilder.Append(" AND T.TrangThai = @TrangThai ");
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai.Value);
                }

                cmd.CommandText = queryBuilder.ToString();
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // ==================== LẤY CHI TIẾT ====================

        public Thuoc GetById(string maThuoc)
        {
            Thuoc t = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM THUOC WHERE MaThuoc = @MaThuoc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaThuoc", maThuoc.Trim());
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        t = new Thuoc
                        {
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            QuyCach = dr["QuyCach"] != DBNull.Value ? dr["QuyCach"].ToString() : null,
                            DonViCoBan = dr["DonViCoBan"].ToString(),
                            MaLoaiThuoc = dr["MaLoaiThuoc"] != DBNull.Value ? dr["MaLoaiThuoc"].ToString() : null,
                            DuongDung = dr["DuongDung"] != DBNull.Value ? dr["DuongDung"].ToString() : null,
                            GiaBan = dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : (decimal?)null,
                            CoBHYT = dr["CoBHYT"] != DBNull.Value && Convert.ToBoolean(dr["CoBHYT"]),
                            // Bỏ gán GiaBHYT
                            MaNSX = dr["MaNSX"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaNSX"]) : null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["TrangThai"]) : null
                        };
                    }
                }
            }
            return t;
        }

        // Lấy chi tiết đầy đủ (kèm thành phần)
        public ThuocManageViewModel GetByIdWithThanhPhan(string maThuoc)
        {
            var t = GetById(maThuoc);
            if (t == null) return null;

            var vm = new ThuocManageViewModel
            {
                MaThuoc = t.MaThuoc,
                TenThuoc = t.TenThuoc,
                QuyCach = t.QuyCach,
                DonViCoBan = t.DonViCoBan,
                MaLoaiThuoc = t.MaLoaiThuoc,
                DuongDung = t.DuongDung,
                GiaBan = t.GiaBan,
                CoBHYT = t.CoBHYT,
                // Bỏ GiaBHYT
                MaNSX = t.MaNSX,
                TrangThai = t.TrangThai ?? true,
                ThanhPhans = GetThanhPhanByMaThuoc(maThuoc)
            };

            // Lấy thêm tên NSX
            if (t.MaNSX.HasValue)
            {
                using (SqlConnection conn = new SqlConnection(connectStr))
                {
                    string q = "SELECT TenNSX FROM NHASANXUAT WHERE MaNSX = @MaNSX";
                    SqlCommand cmd = new SqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@MaNSX", t.MaNSX.Value);
                    conn.Open();
                    var o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        vm.TenNSX = o.ToString();
                }
            }

            return vm;
        }

        // ==================== THÀNH PHẦN THUỐC ====================

        public List<ThanhPhanViewModel> GetThanhPhanByMaThuoc(string maThuoc)
        {
            var list = new List<ThanhPhanViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT TP.MaThanhPhan, TP.MaThuoc, TP.MaHoatChat, TP.HamLuong,
                           HC.TenHoatChat
                    FROM THANHPHAN_THUOC TP
                    LEFT JOIN DANHMUC_HOATCHAT HC ON TP.MaHoatChat = HC.MaHoatChat
                    WHERE TP.MaThuoc = @MaThuoc
                    ORDER BY HC.TenHoatChat";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaThuoc", maThuoc.Trim());
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ThanhPhanViewModel
                        {
                            MaThanhPhan = dr["MaThanhPhan"].ToString(),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            MaHoatChat = dr["MaHoatChat"].ToString(),
                            TenHoatChat = dr["TenHoatChat"] != DBNull.Value ? dr["TenHoatChat"].ToString() : null,
                            HamLuong = dr["HamLuong"] != DBNull.Value ? dr["HamLuong"].ToString() : null
                        });
                    }
                }
            }
            return list;
        }

        // ==================== CRUD THUỐC ====================

        /// <summary>
        /// Lấy số bắt đầu cho MaThanhPhan (dùng MAX theo số, không theo chuỗi để tránh TP00000009 > TP00000010)
        /// </summary>
        private int GenerateNextMaThanhPhanStart(SqlConnection conn, SqlTransaction tran)
        {
            string sql = @"SELECT ISNULL(MAX(CAST(SUBSTRING(MaThanhPhan, 3, 10) AS INT)), 0) + 1 
                          FROM THANHPHAN_THUOC WHERE MaThanhPhan LIKE 'TP%'";
            SqlCommand cmd = new SqlCommand(sql, conn, tran);
            var nextNum = cmd.ExecuteScalar();
            return nextNum != null && nextNum != DBNull.Value ? Convert.ToInt32(nextNum) : 1;
        }

        public string GenerateNextMaThuoc()
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                // Sắp xếp theo phần số để tránh T0009 > T0010 > ... > T0006
                SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 1 MaThuoc FROM THUOC WHERE MaThuoc LIKE 'T%' " +
                    "ORDER BY CAST(SUBSTRING(MaThuoc, 2, 10) AS INT) DESC", conn);
                var result = cmd.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(result)) return "T0001";

                string numberPart = System.Text.RegularExpressions.Regex.Match(result, @"\d+").Value;
                if (int.TryParse(numberPart, out int num))
                {
                    num++;
                    return "T" + num.ToString("D4");
                }
                return "T0001";
            }
        }

        public bool Create(Thuoc thuoc, List<ThanhPhanThuoc> thanhPhans)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. Thêm thuốc (Bỏ GiaBHYT khỏi câu SQL)
                    string queryThuoc = @"
                        INSERT INTO THUOC (MaThuoc, TenThuoc, QuyCach, DonViCoBan, MaLoaiThuoc, DuongDung,
                                         GiaBan, CoBHYT, MaNSX, TrangThai)
                        VALUES (@MaThuoc, @TenThuoc, @QuyCach, @DonViCoBan, @MaLoaiThuoc, @DuongDung,
                                @GiaBan, @CoBHYT, @MaNSX, @TrangThai)";

                    SqlCommand cmdT = new SqlCommand(queryThuoc, conn, tran);
                    cmdT.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                    cmdT.Parameters.AddWithValue("@TenThuoc", thuoc.TenThuoc);
                    cmdT.Parameters.AddWithValue("@QuyCach", (object)thuoc.QuyCach ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@DonViCoBan", thuoc.DonViCoBan);
                    cmdT.Parameters.AddWithValue("@MaLoaiThuoc", (object)thuoc.MaLoaiThuoc ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@DuongDung", (object)thuoc.DuongDung ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@GiaBan", (object)thuoc.GiaBan ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@CoBHYT", thuoc.CoBHYT);
                    // Bỏ add parameter @GiaBHYT
                    cmdT.Parameters.AddWithValue("@MaNSX", (object)thuoc.MaNSX ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@TrangThai", thuoc.TrangThai ?? true);

                    cmdT.ExecuteNonQuery();

                    // 2. Thêm thành phần
                    if (thanhPhans != null && thanhPhans.Count > 0)
                    {
                        int nextSeq = GenerateNextMaThanhPhanStart(conn, tran);
                        foreach (var tp in thanhPhans)
                        {
                            if (string.IsNullOrWhiteSpace(tp.MaHoatChat)) continue;

                            string maTP = "TP" + nextSeq.ToString("D8");
                            nextSeq++;
                            string queryTP = @"
                                INSERT INTO THANHPHAN_THUOC (MaThanhPhan, MaThuoc, MaHoatChat, HamLuong)
                                VALUES (@MaThanhPhan, @MaThuoc, @MaHoatChat, @HamLuong)";
                            SqlCommand cmdTP = new SqlCommand(queryTP, conn, tran);
                            cmdTP.Parameters.AddWithValue("@MaThanhPhan", maTP);
                            cmdTP.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                            cmdTP.Parameters.AddWithValue("@MaHoatChat", tp.MaHoatChat);
                            cmdTP.Parameters.AddWithValue("@HamLuong", (object)tp.HamLuong ?? DBNull.Value);
                            cmdTP.ExecuteNonQuery();
                        }
                    }

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

        public bool Update(Thuoc thuoc, List<ThanhPhanThuoc> thanhPhans)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. Cập nhật thuốc (Bỏ GiaBHYT khỏi câu SQL)
                    string queryThuoc = @"
                        UPDATE THUOC SET
                            TenThuoc = @TenThuoc,
                            QuyCach = @QuyCach,
                            DonViCoBan = @DonViCoBan,
                            MaLoaiThuoc = @MaLoaiThuoc,
                            DuongDung = @DuongDung,
                            GiaBan = @GiaBan,
                            CoBHYT = @CoBHYT,
                            MaNSX = @MaNSX,
                            TrangThai = @TrangThai
                        WHERE MaThuoc = @MaThuoc";

                    SqlCommand cmdT = new SqlCommand(queryThuoc, conn, tran);
                    cmdT.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                    cmdT.Parameters.AddWithValue("@TenThuoc", thuoc.TenThuoc);
                    cmdT.Parameters.AddWithValue("@QuyCach", (object)thuoc.QuyCach ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@DonViCoBan", thuoc.DonViCoBan);
                    cmdT.Parameters.AddWithValue("@MaLoaiThuoc", (object)thuoc.MaLoaiThuoc ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@DuongDung", (object)thuoc.DuongDung ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@GiaBan", (object)thuoc.GiaBan ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@CoBHYT", thuoc.CoBHYT);
                    // Bỏ add parameter @GiaBHYT
                    cmdT.Parameters.AddWithValue("@MaNSX", (object)thuoc.MaNSX ?? DBNull.Value);
                    cmdT.Parameters.AddWithValue("@TrangThai", thuoc.TrangThai ?? true);

                    cmdT.ExecuteNonQuery();

                    // 2. Xóa thành phần cũ
                    SqlCommand cmdDel = new SqlCommand("DELETE FROM THANHPHAN_THUOC WHERE MaThuoc = @MaThuoc", conn, tran);
                    cmdDel.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                    cmdDel.ExecuteNonQuery();

                    // 3. Thêm thành phần mới
                    if (thanhPhans != null && thanhPhans.Count > 0)
                    {
                        int nextSeq = GenerateNextMaThanhPhanStart(conn, tran);
                        foreach (var tp in thanhPhans)
                        {
                            if (string.IsNullOrWhiteSpace(tp.MaHoatChat)) continue;

                            string maTP = "TP" + nextSeq.ToString("D8");
                            nextSeq++;
                            string queryTP = @"
                                INSERT INTO THANHPHAN_THUOC (MaThanhPhan, MaThuoc, MaHoatChat, HamLuong)
                                VALUES (@MaThanhPhan, @MaThuoc, @MaHoatChat, @HamLuong)";
                            SqlCommand cmdTP = new SqlCommand(queryTP, conn, tran);
                            cmdTP.Parameters.AddWithValue("@MaThanhPhan", maTP);
                            cmdTP.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                            cmdTP.Parameters.AddWithValue("@MaHoatChat", tp.MaHoatChat);
                            cmdTP.Parameters.AddWithValue("@HamLuong", (object)tp.HamLuong ?? DBNull.Value);
                            cmdTP.ExecuteNonQuery();
                        }
                    }

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

        public bool Delete(string maThuoc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Xóa thành phần trước
                    SqlCommand cmdTP = new SqlCommand("DELETE FROM THANHPHAN_THUOC WHERE MaThuoc = @MaThuoc", conn, tran);
                    cmdTP.Parameters.AddWithValue("@MaThuoc", maThuoc.Trim());
                    cmdTP.ExecuteNonQuery();

                    // Xóa thuốc
                    SqlCommand cmdT = new SqlCommand("DELETE FROM THUOC WHERE MaThuoc = @MaThuoc", conn, tran);
                    cmdT.Parameters.AddWithValue("@MaThuoc", maThuoc.Trim());
                    int rows = cmdT.ExecuteNonQuery();

                    tran.Commit();
                    return rows > 0;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public bool? ToggleTrangThai(string maThuoc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    string sqlGet = "SELECT TrangThai FROM THUOC WHERE MaThuoc = @MaThuoc";
                    SqlCommand cmdGet = new SqlCommand(sqlGet, conn, tran);
                    cmdGet.Parameters.AddWithValue("@MaThuoc", maThuoc.Trim());

                    object o = cmdGet.ExecuteScalar();
                    if (o == null || o == DBNull.Value) return null;

                    bool current = Convert.ToBoolean(o);
                    bool newStatus = !current;

                    string sqlUp = "UPDATE THUOC SET TrangThai = @NewStatus WHERE MaThuoc = @MaThuoc";
                    SqlCommand cmdUp = new SqlCommand(sqlUp, conn, tran);
                    cmdUp.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmdUp.Parameters.AddWithValue("@MaThuoc", maThuoc.Trim());
                    cmdUp.ExecuteNonQuery();

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

        // ==================== KIỂM TRA TRÙNG ====================

        public bool TenThuocExists(string tenThuoc, string excludeMaThuoc = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM THUOC WHERE TenThuoc = @TenThuoc";
                if (!string.IsNullOrEmpty(excludeMaThuoc))
                    query += " AND MaThuoc <> @MaThuoc";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenThuoc", tenThuoc.Trim());
                if (!string.IsNullOrEmpty(excludeMaThuoc))
                    cmd.Parameters.AddWithValue("@MaThuoc", excludeMaThuoc);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}