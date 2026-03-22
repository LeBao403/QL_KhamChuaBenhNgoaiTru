using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class KhoNhapDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // ==================== VIEW MODELS ====================
        public class ThongKeDashboard
        {
            public int SoKho { get; set; }
            public int SoMatHang { get; set; }
            public int TongSoLuongTon { get; set; }
            public decimal TongGiaTriTon { get; set; }
            public int SoHetHan { get; set; }
            public int SoItHang { get; set; }
            public int PhieuNhapChoDuyet { get; set; }
            public int PhieuNhapDaDuyet { get; set; }
            public int PhieuNhapTong { get; set; }
        }

        public class ThuocItem
        {
            public string MaThuoc { get; set; }
            public string TenThuoc { get; set; }
            public string DonViCoBan { get; set; }
        }

        public class NhaCungCapItem
        {
            public int MaNSX { get; set; }
            public string TenNSX { get; set; }
        }

        public class KhoItem
        {
            public int MaKho { get; set; }
            public string TenKho { get; set; }
            public string LoaiKho { get; set; }
            public string DiaChi { get; set; }
            public bool TrangThai { get; set; }
        }

        public class PhongItem
        {
            public int MaPhong { get; set; }
            public string TenPhong { get; set; }
        }

        public class TonKhoViewModel
        {
            public int MaTonKho { get; set; }
            public string MaThuoc { get; set; }
            public string TenThuoc { get; set; }
            public string DonViCoBan { get; set; }
            public int MaKho { get; set; }
            public string TenKho { get; set; }
            public string MaLo { get; set; }
            public DateTime? HanSuDung { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public decimal? GiaNhap { get; set; }
            public int SoLuongTon { get; set; }
            public decimal? ThanhTien { get; set; }
            public DateTime? NgayCapNhat { get; set; }
            public int MaPhong { get; set; }
            public string TenPhong { get; set; }
        }

        public class PhieuNhapItem
        {
            public int MaPhieuNhap { get; set; }
            public string MaNV_LapPhieu { get; set; }
            public string TenNV_Lap { get; set; }
            public int MaNSX { get; set; }
            public string TenNSX { get; set; }
            public DateTime? NgayLap { get; set; }
            public decimal? TongTienNhap { get; set; }
            public string TrangThai { get; set; }
            public string GhiChu { get; set; }
            public string MaNV_Duyet { get; set; }
            public string TenNV_Duyet { get; set; }
            public DateTime? NgayDuyet { get; set; }
        }

        public class CT_PhieuNhapItem
        {
            public int MaCTPN { get; set; }
            public int MaPhieuNhap { get; set; }
            public string MaThuoc { get; set; }
            public string TenThuoc { get; set; }
            public string MaLo { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public DateTime? HanSuDung { get; set; }
            public int SoLuongNhap { get; set; }
            public decimal? DonGiaNhap { get; set; }
            public decimal? ThanhTien { get; set; }
        }

        // ==================== THỐNG KÊ DASHBOARD ====================
        public ThongKeDashboard GetThongKeDashboard()
        {
            var kq = new ThongKeDashboard();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                using (SqlCommand cmdKho = new SqlCommand(
                    "SELECT COUNT(*) FROM KHO WHERE TrangThai = 1", conn))
                {
                    kq.SoKho = (int)cmdKho.ExecuteScalar();
                }

                string sqlTK = @"
                    SELECT
                        COUNT(DISTINCT T.MaThuoc) AS SoMatHang,
                        ISNULL(SUM(TK.SoLuongTon), 0) AS TongSoLuongTon,
                        ISNULL(SUM(TK.SoLuongTon * ISNULL(T.GiaBan, 0)), 0) AS TongGiaTriTon,
                        (SELECT COUNT(*) FROM TONKHO WHERE HanSuDung <= DATEADD(day, 30, GETDATE()) AND SoLuongTon > 0) AS SoHetHan,
                        (SELECT COUNT(*) FROM (SELECT MaThuoc FROM THUOC T INNER JOIN TONKHO TK ON T.MaThuoc = TK.MaThuoc GROUP BY T.MaThuoc HAVING SUM(TK.SoLuongTon) <= 10) AS X) AS SoItHang
                    FROM THUOC T
                    LEFT JOIN TONKHO TK ON T.MaThuoc = TK.MaThuoc
                    WHERE T.TrangThai = 1";

                using (SqlCommand cmdTK = new SqlCommand(sqlTK, conn))
                using (SqlDataReader dr = cmdTK.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        kq.SoMatHang = dr["SoMatHang"] != DBNull.Value ? Convert.ToInt32(dr["SoMatHang"]) : 0;
                        kq.TongSoLuongTon = dr["TongSoLuongTon"] != DBNull.Value ? Convert.ToInt32(dr["TongSoLuongTon"]) : 0;
                        kq.TongGiaTriTon = dr["TongGiaTriTon"] != DBNull.Value ? Convert.ToDecimal(dr["TongGiaTriTon"]) : 0;
                        kq.SoHetHan = dr["SoHetHan"] != DBNull.Value ? Convert.ToInt32(dr["SoHetHan"]) : 0;
                        kq.SoItHang = dr["SoItHang"] != DBNull.Value ? Convert.ToInt32(dr["SoItHang"]) : 0;
                    }
                }

                using (SqlCommand cmdPN = new SqlCommand(
                    @"SELECT TrangThai, COUNT(*) AS SoLuong FROM PHIEUNHAP GROUP BY TrangThai", conn))
                using (SqlDataReader dr = cmdPN.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string tt = dr["TrangThai"].ToString();
                        int count = Convert.ToInt32(dr["SoLuong"]);
                        kq.PhieuNhapTong += count;
                        if (tt == "Chờ duyệt") kq.PhieuNhapChoDuyet = count;
                        else if (tt == "Đã duyệt") kq.PhieuNhapDaDuyet = count;
                    }
                }
            }

            return kq;
        }

        // ==================== PHIẾU NHẬP ====================
        public List<PhieuNhapItem> GetPhieuNhap(int page, int pageSize, string keyword = "", string trangThai = "")
        {
            var list = new List<PhieuNhapItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                StringBuilder sql = new StringBuilder(@"
                    SELECT PN.MaPhieuNhap, PN.MaNV_LapPhieu, NV_HoTen = NV_Lap.HoTen,
                           PN.MaNSX, NSX.TenNSX, PN.NgayLap, PN.TongTienNhap,
                           PN.TrangThai, PN.GhiChu, PN.MaNV_Duyet, PN.NgayDuyet,
                           NV_Duyet.HoTen AS TenNV_Duyet
                    FROM PHIEUNHAP PN
                    INNER JOIN NHANVIEN NV_Lap ON PN.MaNV_LapPhieu = NV_Lap.MaNV
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    LEFT JOIN NHANVIEN NV_Duyet ON PN.MaNV_Duyet = NV_Duyet.MaNV
                    WHERE 1=1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql.Append(" AND (NSX.TenNSX LIKE @Keyword OR PN.MaPhieuNhap IN (SELECT MaPhieuNhap FROM CT_PHIEUNHAP CT INNER JOIN THUOC T ON CT.MaThuoc = T.MaThuoc WHERE T.TenThuoc LIKE @Keyword))");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                }

                if (!string.IsNullOrEmpty(trangThai))
                {
                    sql.Append(" AND PN.TrangThai = @TrangThai");
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                }

                sql.Append(" ORDER BY PN.NgayLap DESC");
                sql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.CommandText = sql.ToString();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(ReadPhieuNhap(dr));
                    }
                }
            }
            return list;
        }

        public int GetPhieuNhapCount(string keyword = "", string trangThai = "")
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                StringBuilder sql = new StringBuilder(@"
                    SELECT COUNT(*) FROM PHIEUNHAP PN
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    WHERE 1=1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql.Append(" AND (NSX.TenNSX LIKE @Keyword OR PN.MaPhieuNhap IN (SELECT MaPhieuNhap FROM CT_PHIEUNHAP CT INNER JOIN THUOC T ON CT.MaThuoc = T.MaThuoc WHERE T.TenThuoc LIKE @Keyword))");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                }

                if (!string.IsNullOrEmpty(trangThai))
                {
                    sql.Append(" AND PN.TrangThai = @TrangThai");
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                }

                cmd.CommandText = sql.ToString();
                return (int)cmd.ExecuteScalar();
            }
        }

        public PhieuNhapItem GetPhieuNhapById(int maPhieuNhap)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT PN.MaPhieuNhap, PN.MaNV_LapPhieu, NV_HoTen = NV_Lap.HoTen,
                           PN.MaNSX, NSX.TenNSX, PN.NgayLap, PN.TongTienNhap,
                           PN.TrangThai, PN.GhiChu, PN.MaNV_Duyet, PN.NgayDuyet,
                           NV_Duyet.HoTen AS TenNV_Duyet
                    FROM PHIEUNHAP PN
                    INNER JOIN NHANVIEN NV_Lap ON PN.MaNV_LapPhieu = NV_Lap.MaNV
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    LEFT JOIN NHANVIEN NV_Duyet ON PN.MaNV_Duyet = NV_Duyet.MaNV
                    WHERE PN.MaPhieuNhap = @MaPhieuNhap";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        return ReadPhieuNhap(dr);
                }
            }
            return null;
        }

        public List<CT_PhieuNhapItem> GetCT_PhieuNhap(int maPhieuNhap)
        {
            var list = new List<CT_PhieuNhapItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT CT.MaCTPN, CT.MaPhieuNhap, CT.MaThuoc, T.TenThuoc,
                           CT.MaLo, CT.NgaySanXuat, CT.HanSuDung,
                           CT.SoLuongNhap, CT.DonGiaNhap, CT.ThanhTien
                    FROM CT_PHIEUNHAP CT
                    INNER JOIN THUOC T ON CT.MaThuoc = T.MaThuoc
                    WHERE CT.MaPhieuNhap = @MaPhieuNhap
                    ORDER BY CT.MaCTPN";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CT_PhieuNhapItem
                        {
                            MaCTPN = Convert.ToInt32(dr["MaCTPN"]),
                            MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : (DateTime?)null,
                            SoLuongNhap = Convert.ToInt32(dr["SoLuongNhap"]),
                            DonGiaNhap = dr["DonGiaNhap"] != DBNull.Value ? Convert.ToDecimal(dr["DonGiaNhap"]) : 0,
                            ThanhTien = dr["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(dr["ThanhTien"]) : 0
                        });
                    }
                }
            }
            return list;
        }

        public class CT_PhieuNhap_Insert
        {
            public string MaThuoc { get; set; }
            public string MaLo { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public DateTime? HanSuDung { get; set; }
            public int SoLuongNhap { get; set; }
            public decimal DonGiaNhap { get; set; }
        }

        public bool CreatePhieuNhap(string maNV, int maNSX, string ghiChu, List<CT_PhieuNhap_Insert> chiTiet)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    decimal tongTien = 0;
                    foreach (var ct in chiTiet)
                        tongTien += ct.SoLuongNhap * ct.DonGiaNhap;

                    SqlCommand cmdPN = new SqlCommand(@"
                        INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, TongTienNhap, TrangThai, GhiChu)
                        VALUES (@MaNV, @MaNSX, @TongTien, N'Chờ duyệt', @GhiChu);
                        SELECT SCOPE_IDENTITY();", conn, tran);

                    cmdPN.Parameters.AddWithValue("@MaNV", maNV);
                    cmdPN.Parameters.AddWithValue("@MaNSX", maNSX);
                    cmdPN.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdPN.Parameters.AddWithValue("@GhiChu", (object)ghiChu ?? DBNull.Value);

                    int maPN = Convert.ToInt32(cmdPN.ExecuteScalar());

                    foreach (var ct in chiTiet)
                    {
                        SqlCommand cmdCT = new SqlCommand(@"
                            INSERT INTO CT_PHIEUNHAP (MaPhieuNhap, MaThuoc, MaLo, NgaySanXuat, HanSuDung, SoLuongNhap, DonGiaNhap, ThanhTien)
                            VALUES (@MaPN, @MaThuoc, @MaLo, @NgaySX, @HanSD, @SoLuong, @DonGia, @ThanhTien)", conn, tran);

                        cmdCT.Parameters.AddWithValue("@MaPN", maPN);
                        cmdCT.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdCT.Parameters.AddWithValue("@MaLo", ct.MaLo);
                        cmdCT.Parameters.AddWithValue("@NgaySX", ct.NgaySanXuat.HasValue ? ct.NgaySanXuat.Value : (object)DBNull.Value);
                        cmdCT.Parameters.AddWithValue("@HanSD", ct.HanSuDung.HasValue ? ct.HanSuDung.Value : (object)DBNull.Value);
                        cmdCT.Parameters.AddWithValue("@SoLuong", ct.SoLuongNhap);
                        cmdCT.Parameters.AddWithValue("@DonGia", ct.DonGiaNhap);
                        cmdCT.Parameters.AddWithValue("@ThanhTien", ct.SoLuongNhap * ct.DonGiaNhap);
                        cmdCT.ExecuteNonQuery();
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

        // ==================== TỒN KHO ====================
        public List<TonKhoViewModel> GetTonKho(int page, int pageSize, string keyword = "", string maKho = "", string trangThaiTon = "")
        {
            var list = new List<TonKhoViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                StringBuilder sql = new StringBuilder(@"
                    SELECT TK.MaTonKho, TK.MaThuoc, T.TenThuoc, T.DonViCoBan,
                           TK.MaKho, K.TenKho, TK.MaLo, TK.HanSuDung, TK.NgaySanXuat,
                           TK.GiaNhap, TK.SoLuongTon,
                           ThanhTien = ISNULL(TK.SoLuongTon * TK.GiaNhap, 0),
                           TK.NgayCapNhat, TK.MaPhong, P.TenPhong
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    INNER JOIN KHO K ON TK.MaKho = K.MaKho
                    LEFT JOIN PHONG P ON TK.MaPhong = P.MaPhong
                    WHERE 1=1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql.Append(" AND (T.TenThuoc LIKE @Keyword OR TK.MaThuoc LIKE @Keyword OR TK.MaLo LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                }

                if (!string.IsNullOrEmpty(maKho) && int.TryParse(maKho, out int mk))
                {
                    sql.Append(" AND TK.MaKho = @MaKho");
                    cmd.Parameters.AddWithValue("@MaKho", mk);
                }

                if (trangThaiTon == "hethan")
                    sql.Append(" AND TK.HanSuDung <= DATEADD(day, 30, GETDATE())");
                else if (trangThaiTon == "saphethan")
                    sql.Append(" AND TK.HanSuDung BETWEEN DATEADD(day, 30, GETDATE()) AND DATEADD(day, 90, GETDATE())");
                else if (trangThaiTon == "conhan")
                    sql.Append(" AND TK.HanSuDung > DATEADD(day, 90, GETDATE())");
                else if (trangThaiTon == "hethang")
                    sql.Append(" AND TK.SoLuongTon <= 10");

                sql.Append(" ORDER BY TK.NgayCapNhat DESC");
                sql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.CommandText = sql.ToString();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new TonKhoViewModel
                        {
                            MaTonKho = Convert.ToInt32(dr["MaTonKho"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            DonViCoBan = dr["DonViCoBan"].ToString(),
                            MaKho = dr["MaKho"] != DBNull.Value ? Convert.ToInt32(dr["MaKho"]) : 0,
                            TenKho = dr["TenKho"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : (DateTime?)null,
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            GiaNhap = dr["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(dr["GiaNhap"]) : 0,
                            SoLuongTon = Convert.ToInt32(dr["SoLuongTon"]),
                            ThanhTien = dr["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(dr["ThanhTien"]) : 0,
                            NgayCapNhat = dr["NgayCapNhat"] != DBNull.Value ? Convert.ToDateTime(dr["NgayCapNhat"]) : (DateTime?)null,
                            MaPhong = dr["MaPhong"] != DBNull.Value ? Convert.ToInt32(dr["MaPhong"]) : 0,
                            TenPhong = dr["TenPhong"] != DBNull.Value ? dr["TenPhong"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public int GetTonKhoCount(string keyword = "", string maKho = "", string trangThaiTon = "")
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                StringBuilder sql = new StringBuilder(@"
                    SELECT COUNT(*) FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    INNER JOIN KHO K ON TK.MaKho = K.MaKho
                    WHERE 1=1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql.Append(" AND (T.TenThuoc LIKE @Keyword OR TK.MaThuoc LIKE @Keyword OR TK.MaLo LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                }

                if (!string.IsNullOrEmpty(maKho) && int.TryParse(maKho, out int mk))
                {
                    sql.Append(" AND TK.MaKho = @MaKho");
                    cmd.Parameters.AddWithValue("@MaKho", mk);
                }

                if (trangThaiTon == "hethan")
                    sql.Append(" AND TK.HanSuDung <= DATEADD(day, 30, GETDATE())");
                else if (trangThaiTon == "saphethan")
                    sql.Append(" AND TK.HanSuDung BETWEEN DATEADD(day, 30, GETDATE()) AND DATEADD(day, 90, GETDATE())");
                else if (trangThaiTon == "conhan")
                    sql.Append(" AND TK.HanSuDung > DATEADD(day, 90, GETDATE())");
                else if (trangThaiTon == "hethang")
                    sql.Append(" AND TK.SoLuongTon <= 10");

                cmd.CommandText = sql.ToString();
                return (int)cmd.ExecuteScalar();
            }
        }

        // ==================== DANH SÁCH DROPDOWN ====================
        public List<ThuocItem> GetAllThuoc()
        {
            var list = new List<ThuocItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaThuoc, TenThuoc, DonViCoBan FROM THUOC WHERE TrangThai = 1 ORDER BY TenThuoc";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ThuocItem
                        {
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public List<NhaCungCapItem> GetAllNhaCungCap()
        {
            var list = new List<NhaCungCapItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaNSX, TenNSX FROM NHASANXUAT ORDER BY TenNSX";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new NhaCungCapItem
                        {
                            MaNSX = Convert.ToInt32(dr["MaNSX"]),
                            TenNSX = dr["TenNSX"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<KhoItem> GetAllKho()
        {
            var list = new List<KhoItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaKho, TenKho, LoaiKho, DiaChi, TrangThai FROM KHO WHERE TrangThai = 1 ORDER BY TenKho";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new KhoItem
                        {
                            MaKho = Convert.ToInt32(dr["MaKho"]),
                            TenKho = dr["TenKho"].ToString(),
                            LoaiKho = dr["LoaiKho"] != DBNull.Value ? dr["LoaiKho"].ToString() : "",
                            DiaChi = dr["DiaChi"] != DBNull.Value ? dr["DiaChi"].ToString() : "",
                            TrangThai = dr["TrangThai"] != DBNull.Value && Convert.ToBoolean(dr["TrangThai"])
                        });
                    }
                }
            }
            return list;
        }

        public List<PhongItem> GetAllPhongKho()
        {
            var list = new List<PhongItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT P.MaPhong, P.TenPhong
                    FROM PHONG P
                    INNER JOIN DANHMUC_LOAIPHONG LP ON P.MaLoaiPhong = LP.MaLoaiPhong
                    WHERE LP.TenLoaiPhong = N'Kho' AND P.TrangThai = 1
                    ORDER BY P.TenPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PhongItem
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==================== HELPER ====================
        private PhieuNhapItem ReadPhieuNhap(SqlDataReader dr)
        {
            return new PhieuNhapItem
            {
                MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]),
                MaNV_LapPhieu = dr["MaNV_LapPhieu"].ToString(),
                TenNV_Lap = dr["TenNV_Lap"] != DBNull.Value ? dr["TenNV_Lap"].ToString() : "",
                MaNSX = Convert.ToInt32(dr["MaNSX"]),
                TenNSX = dr["TenNSX"].ToString(),
                NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null,
                TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0,
                TrangThai = dr["TrangThai"].ToString(),
                GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : null,
                TenNV_Duyet = dr["TenNV_Duyet"] != DBNull.Value ? dr["TenNV_Duyet"].ToString() : "",
                NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null
            };
        }

        // ==================== TỒN KHO CHI TIẾT ====================
        public TonKhoViewModel GetTonKhoById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT TK.MaTonKho, TK.MaThuoc, T.TenThuoc, T.DonViCoBan, T.QuyCach,
                           TK.MaKho, K.TenKho, TK.MaLo, TK.HanSuDung, TK.NgaySanXuat,
                           TK.GiaNhap, TK.SoLuongTon,
                           ThanhTien = ISNULL(TK.SoLuongTon * TK.GiaNhap, 0),
                           TK.NgayCapNhat, TK.MaPhong, P.TenPhong
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    INNER JOIN KHO K ON TK.MaKho = K.MaKho
                    LEFT JOIN PHONG P ON TK.MaPhong = P.MaPhong
                    WHERE TK.MaTonKho = @MaTonKho";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTonKho", id);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new TonKhoViewModel
                        {
                            MaTonKho = Convert.ToInt32(dr["MaTonKho"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            DonViCoBan = dr["DonViCoBan"].ToString(),
                            MaKho = dr["MaKho"] != DBNull.Value ? Convert.ToInt32(dr["MaKho"]) : 0,
                            TenKho = dr["TenKho"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : (DateTime?)null,
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            GiaNhap = dr["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(dr["GiaNhap"]) : 0,
                            SoLuongTon = Convert.ToInt32(dr["SoLuongTon"]),
                            ThanhTien = dr["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(dr["ThanhTien"]) : 0,
                            NgayCapNhat = dr["NgayCapNhat"] != DBNull.Value ? Convert.ToDateTime(dr["NgayCapNhat"]) : (DateTime?)null,
                            MaPhong = dr["MaPhong"] != DBNull.Value ? Convert.ToInt32(dr["MaPhong"]) : 0,
                            TenPhong = dr["TenPhong"] != DBNull.Value ? dr["TenPhong"].ToString() : ""
                        };
                    }
                }
            }
            return null;
        }

        public bool UpdateTonKhoSoLuong(int maTonKho, int soLuongMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    UPDATE TONKHO
                    SET SoLuongTon = @SoLuong, NgayCapNhat = GETDATE()
                    WHERE MaTonKho = @MaTonKho";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTonKho", maTonKho);
                cmd.Parameters.AddWithValue("@SoLuong", soLuongMoi);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================== THUỐC TRA CỨU ====================
        public class ThuocSearchItem
        {
            public string MaThuoc { get; set; }
            public string TenThuoc { get; set; }
            public string DonViCoBan { get; set; }
            public string QuyCach { get; set; }
            public string DuongDung { get; set; }
            public string TenLoaiThuoc { get; set; }
            public string TenNSX { get; set; }
            public decimal? GiaBan { get; set; }
            public bool? CoBHYT { get; set; }
        }

        public List<ThuocSearchItem> SearchThuoc(string keyword = "", string maLoaiThuoc = "")
        {
            var list = new List<ThuocSearchItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                StringBuilder sql = new StringBuilder(@"
                    SELECT T.MaThuoc, T.TenThuoc, T.DonViCoBan, T.QuyCach, T.DuongDung,
                           T.MaLoaiThuoc, L.TenLoaiThuoc, NSX.TenNSX, T.GiaBan, T.CoBHYT
                    FROM THUOC T
                    LEFT JOIN DANHMUC_THUOC L ON T.MaLoaiThuoc = L.MaLoaiThuoc
                    LEFT JOIN NHASANXUAT NSX ON T.MaNSX = NSX.MaNSX
                    WHERE T.TrangThai = 1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql.Append(" AND (T.TenThuoc LIKE @Keyword OR T.MaThuoc LIKE @Keyword)");
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                }

                if (!string.IsNullOrEmpty(maLoaiThuoc))
                {
                    sql.Append(" AND T.MaLoaiThuoc = @MaLoai");
                    cmd.Parameters.AddWithValue("@MaLoai", maLoaiThuoc);
                }

                sql.Append(" ORDER BY T.TenThuoc");
                cmd.CommandText = sql.ToString();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ThuocSearchItem
                        {
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : "",
                            QuyCach = dr["QuyCach"] != DBNull.Value ? dr["QuyCach"].ToString() : "",
                            DuongDung = dr["DuongDung"] != DBNull.Value ? dr["DuongDung"].ToString() : "",
                            TenLoaiThuoc = dr["TenLoaiThuoc"] != DBNull.Value ? dr["TenLoaiThuoc"].ToString() : "",
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : "",
                            GiaBan = dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : (decimal?)null,
                            CoBHYT = dr["CoBHYT"] != DBNull.Value ? Convert.ToBoolean(dr["CoBHYT"]) : (bool?)null
                        });
                    }
                }
            }
            return list;
        }

        public List<LoaiThuocItem> GetAllLoaiThuoc()
        {
            var list = new List<LoaiThuocItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaLoaiThuoc, TenLoaiThuoc FROM DANHMUC_THUOC ORDER BY TenLoaiThuoc";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new LoaiThuocItem
                        {
                            MaLoaiThuoc = Convert.ToInt32(dr["MaLoaiThuoc"]),
                            TenLoaiThuoc = dr["TenLoaiThuoc"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public class LoaiThuocItem
        {
            public int MaLoaiThuoc { get; set; }
            public string TenLoaiThuoc { get; set; }
        }

        // ==================== NHÀ CUNG CẤP (NSX) ====================
        public class NhaCungCapDetailItem
        {
            public int MaNSX { get; set; }
            public string TenNSX { get; set; }
            public string DiaChi { get; set; }
            public string SDT { get; set; }
            public string Email { get; set; }
            public int SoPhieuNhap { get; set; }
            public decimal? TongTienNhap { get; set; }
        }

        public List<NhaCungCapDetailItem> GetAllNhaCungCapDetail()
        {
            var list = new List<NhaCungCapDetailItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT NSX.MaNSX, NSX.TenNSX, NSX.DiaChi, NSX.SDT, NSX.Email,
                           ISNULL(COUNT(PN.MaPhieuNhap), 0) AS SoPhieuNhap,
                           ISNULL(SUM(PN.TongTienNhap), 0) AS TongTienNhap
                    FROM NHASANXUAT NSX
                    LEFT JOIN PHIEUNHAP PN ON NSX.MaNSX = PN.MaNSX
                    GROUP BY NSX.MaNSX, NSX.TenNSX, NSX.DiaChi, NSX.SDT, NSX.Email
                    ORDER BY NSX.TenNSX";

                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new NhaCungCapDetailItem
                        {
                            MaNSX = Convert.ToInt32(dr["MaNSX"]),
                            TenNSX = dr["TenNSX"].ToString(),
                            DiaChi = dr["DiaChi"] != DBNull.Value ? dr["DiaChi"].ToString() : "",
                            SDT = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : "",
                            Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : "",
                            SoPhieuNhap = Convert.ToInt32(dr["SoPhieuNhap"]),
                            TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0
                        });
                    }
                }
            }
            return list;
        }

        public NhaCungCapDetailItem GetNhaCungCapById(int maNSX)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT NSX.MaNSX, NSX.TenNSX, NSX.DiaChi, NSX.SDT, NSX.Email
                    FROM NHASANXUAT NSX
                    WHERE NSX.MaNSX = @MaNSX";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNSX", maNSX);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new NhaCungCapDetailItem
                        {
                            MaNSX = Convert.ToInt32(dr["MaNSX"]),
                            TenNSX = dr["TenNSX"].ToString(),
                            DiaChi = dr["DiaChi"] != DBNull.Value ? dr["DiaChi"].ToString() : "",
                            SDT = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : "",
                            Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : "",
                            SoPhieuNhap = 0,
                            TongTienNhap = 0
                        };
                    }
                }
            }
            return null;
        }

        public bool NhaCungCapExists(string tenNSX, int? excludeMaNSX = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM NHASANXUAT WHERE TenNSX = @TenNSX";
                if (excludeMaNSX.HasValue)
                    query += " AND MaNSX <> @MaNSX";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenNSX", tenNSX.Trim());
                if (excludeMaNSX.HasValue)
                    cmd.Parameters.AddWithValue("@MaNSX", excludeMaNSX.Value);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool CreateNhaCungCap(string tenNSX, string diaChi, string sdt, string email)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO NHASANXUAT (TenNSX, DiaChi, SDT, Email)
                    VALUES (@TenNSX, @DiaChi, @SDT, @Email)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TenNSX", tenNSX);
                cmd.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", (object)sdt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateNhaCungCap(int maNSX, string tenNSX, string diaChi, string sdt, string email)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    UPDATE NHASANXUAT
                    SET TenNSX = @TenNSX, DiaChi = @DiaChi, SDT = @SDT, Email = @Email
                    WHERE MaNSX = @MaNSX";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNSX", maNSX);
                cmd.Parameters.AddWithValue("@TenNSX", tenNSX);
                cmd.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", (object)sdt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteNhaCungCap(int maNSX)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                // Check if has related PHIEUNHAP
                SqlCommand cmdCheck = new SqlCommand(
                    "SELECT COUNT(*) FROM PHIEUNHAP WHERE MaNSX = @MaNSX", conn);
                cmdCheck.Parameters.AddWithValue("@MaNSX", maNSX);
                int count = (int)cmdCheck.ExecuteScalar();
                if (count > 0) return false;

                SqlCommand cmd = new SqlCommand("DELETE FROM NHASANXUAT WHERE MaNSX = @MaNSX", conn);
                cmd.Parameters.AddWithValue("@MaNSX", maNSX);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================== HỦY PHIẾU NHẬP ====================
        public bool HuyPhieuNhap(int maPhieuNhap, string maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    UPDATE PHIEUNHAP
                    SET TrangThai = N'Đã hủy',
                        MaNV_Duyet = @MaNV,
                        NgayDuyet = GETDATE()
                    WHERE MaPhieuNhap = @MaPN
                      AND TrangThai = N'Chờ duyệt'
                      AND MaNV_LapPhieu = @MaNV";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaPN", maPhieuNhap);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================== BIỂU ĐỒ DASHBOARD ====================
        public class ChartDataItem
        {
            public string Label { get; set; }
            public int Value { get; set; }
        }

        public List<ChartDataItem> GetTonKhoTheoKho()
        {
            var list = new List<ChartDataItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT K.TenKho,
                           ISNULL(SUM(TK.SoLuongTon), 0) AS TongTon
                    FROM KHO K
                    LEFT JOIN TONKHO TK ON K.MaKho = TK.MaKho
                    WHERE K.TrangThai = 1
                    GROUP BY K.MaKho, K.TenKho
                    ORDER BY TongTon DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ChartDataItem
                        {
                            Label = dr["TenKho"].ToString(),
                            Value = Convert.ToInt32(dr["TongTon"])
                        });
                    }
                }
            }
            return list;
        }

        public List<ChartDataItem> GetPhieuNhapTheoThang(int year)
        {
            var list = new List<ChartDataItem>();
            string[] thang = { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                               "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT MONTH(NgayLap) AS Thang,
                           COUNT(*) AS SoPhieu
                    FROM PHIEUNHAP
                    WHERE YEAR(NgayLap) = @Year AND TrangThai = N'Đã duyệt'
                    GROUP BY MONTH(NgayLap)
                    ORDER BY MONTH(NgayLap)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Year", year);
                var dict = new Dictionary<int, int>();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        dict[Convert.ToInt32(dr["Thang"])] = Convert.ToInt32(dr["SoPhieu"]);
                    }
                }

                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new ChartDataItem
                    {
                        Label = thang[i - 1],
                        Value = dict.ContainsKey(i) ? dict[i] : 0
                    });
                }
            }
            return list;
        }

        public List<ChartDataItem> GetTonKhoTheoLoai()
        {
            var list = new List<ChartDataItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT ISNULL(L.TenLoaiThuoc, N'Khác') AS TenLoai,
                           ISNULL(SUM(TK.SoLuongTon), 0) AS TongTon
                    FROM DANHMUC_THUOC L
                    LEFT JOIN THUOC T ON L.MaLoaiThuoc = T.MaLoaiThuoc AND T.TrangThai = 1
                    LEFT JOIN TONKHO TK ON T.MaThuoc = TK.MaThuoc
                    GROUP BY L.TenLoaiThuoc
                    ORDER BY TongTon DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ChartDataItem
                        {
                            Label = dr["TenLoai"].ToString(),
                            Value = Convert.ToInt32(dr["TongTon"])
                        });
                    }
                }
            }
            return list;
        }
    }
}
