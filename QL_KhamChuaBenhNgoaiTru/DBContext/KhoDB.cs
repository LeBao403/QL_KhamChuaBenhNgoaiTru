using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class KhoDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // ==================== TỔNG QUAN KHO ====================
        public KhoTongQuan GetTongQuanKho(int? maKho = null)
        {
            var kq = new KhoTongQuan();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. Tổng số kho
                using (SqlCommand cmdKho = new SqlCommand(
                    "SELECT COUNT(*) FROM KHO WHERE TrangThai = 1", conn))
                {
                    kq.SoKho = (int)cmdKho.ExecuteScalar();
                }

                // 2. Tổng số thuốc, tổng giá trị tồn kho
                var cmd = new SqlCommand();
                cmd.Connection = conn;

                var whereKho = maKho.HasValue ? "AND TK.MaKho = @MaKho" : "";

                string sql = $@"
                    SELECT
                        (SELECT COUNT(DISTINCT MaThuoc) FROM TONKHO TK WHERE TK.SoLuongTon > 0 {whereKho}) AS SoMatHang,
                        ISNULL(SUM(TK.SoLuongTon * ISNULL(T.GiaBan, 0)), 0) AS TongGiaTriTon,
                        ISNULL(SUM(TK.SoLuongTon), 0) AS TongSoLuongTon,
                        (SELECT COUNT(*) FROM TONKHO TK WHERE TK.HanSuDung <= DATEADD(day, 30, GETDATE()) AND TK.SoLuongTon > 0 {whereKho}) AS SoHetHan,
                        (SELECT COUNT(*) FROM (SELECT MaThuoc FROM TONKHO TK WHERE 1=1 {whereKho} GROUP BY MaThuoc HAVING SUM(TK.SoLuongTon) <= 10) AS X) AS SoItHang
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    WHERE TK.SoLuongTon > 0 {whereKho}";

                if (maKho.HasValue)
                    cmd.Parameters.AddWithValue("@MaKho", maKho.Value);

                cmd.CommandText = sql;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        kq.SoMatHang = dr["SoMatHang"] != DBNull.Value ? Convert.ToInt32(dr["SoMatHang"]) : 0;
                        kq.SoMatHangTheoKho = maKho.HasValue ? kq.SoMatHang : 0;
                        kq.TongGiaTriTon = dr["TongGiaTriTon"] != DBNull.Value ? Convert.ToDecimal(dr["TongGiaTriTon"]) : 0;
                        kq.TongSoLuongTon = dr["TongSoLuongTon"] != DBNull.Value ? Convert.ToInt32(dr["TongSoLuongTon"]) : 0;
                        kq.SoHetHan = dr["SoHetHan"] != DBNull.Value ? Convert.ToInt32(dr["SoHetHan"]) : 0;
                        kq.SoItHang = dr["SoItHang"] != DBNull.Value ? Convert.ToInt32(dr["SoItHang"]) : 0;
                    }
                }
            }

            return kq;
        }

        // ==================== DANH SÁCH TỒN KHO ====================
        public List<TonKhoViewModel> GetTonKho(int page, int pageSize, string keyword, string maPhong, string maKho, string trangThaiTon)
        {
            var list = new List<TonKhoViewModel>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                var query = @"
                    SELECT
                        TK.MaTonKho,
                        TK.MaPhong,
                        TK.MaKho,
                        TK.MaThuoc,
                        TK.MaLo,
                        TK.HanSuDung,
                        TK.NgaySanXuat,
                        TK.GiaNhap,
                        TK.SoLuongTon,
                        TK.NgayCapNhat,
                        T.TenThuoc,
                        T.DonViCoBan,
                        T.GiaBan,
                        T.CoBHYT,
                        P.TenPhong,
                        K.TenKho
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    INNER JOIN PHONG P ON TK.MaPhong = P.MaPhong
                    LEFT JOIN KHO K ON TK.MaKho = K.MaKho
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += @" AND (T.TenThuoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                OR TK.MaThuoc LIKE @Keyword
                                OR TK.MaLo LIKE @Keyword)";
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(maKho))
                {
                    query += " AND TK.MaKho = @MaKho";
                    cmd.Parameters.AddWithValue("@MaKho", int.Parse(maKho));
                }

                if (!string.IsNullOrEmpty(maPhong))
                {
                    query += " AND TK.MaPhong = @MaPhong";
                    cmd.Parameters.AddWithValue("@MaPhong", int.Parse(maPhong));
                }

                if (!string.IsNullOrEmpty(trangThaiTon))
                {
                    if (trangThaiTon == "het_han")
                        query += " AND TK.HanSuDung <= CAST(GETDATE() AS DATE)";
                    else if (trangThaiTon == "sap_het_han")
                        query += " AND TK.HanSuDung <= DATEADD(day, 30, CAST(GETDATE() AS DATE)) AND TK.HanSuDung > CAST(GETDATE() AS DATE)";
                    else if (trangThaiTon == "sap_het_hang")
                        query += " AND TK.SoLuongTon <= 10";
                    else if (trangThaiTon == "con_hang")
                        query += " AND TK.SoLuongTon > 10";
                }

                query += " ORDER BY TK.NgayCapNhat DESC";
                query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var item = new TonKhoViewModel
                        {
                            MaTonKho = Convert.ToInt32(dr["MaTonKho"]),
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            MaKho = dr["MaKho"] != DBNull.Value ? Convert.ToInt32(dr["MaKho"]) : 0,
                            MaThuoc = dr["MaThuoc"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now,
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            GiaNhap = dr["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(dr["GiaNhap"]) : 0,
                            SoLuongTon = dr["SoLuongTon"] != DBNull.Value ? Convert.ToInt32(dr["SoLuongTon"]) : 0,
                            NgayCapNhat = dr["NgayCapNhat"] != DBNull.Value ? Convert.ToDateTime(dr["NgayCapNhat"]) : (DateTime?)null,
                            TenThuoc = dr["TenThuoc"].ToString(),
                            DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : "",
                            GiaBan = dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : 0,
                            CoBHYT = dr["CoBHYT"] != DBNull.Value && Convert.ToBoolean(dr["CoBHYT"]),
                            TenPhong = dr["TenPhong"] != DBNull.Value ? dr["TenPhong"].ToString() : "",
                            TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : ""
                        };

                        item.SoNgayConLai = (item.HanSuDung.Date - DateTime.Now.Date).Days;
                        item.GiaTriTon = item.SoLuongTon * item.GiaBan;
                        item.TrangThaiHSD = item.SoNgayConLai < 0 ? "hết_han" :
                                           item.SoNgayConLai <= 30 ? "sap_het" : "con_han";
                        item.TrangThaiTonKho = item.SoLuongTon <= 0 ? "het_hang" :
                                              item.SoLuongTon <= 10 ? "sap_het_hang" : "con_hang";

                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public int GetTonKhoCount(string keyword, string maPhong, string maKho, string trangThaiTon)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                var query = @"
                    SELECT COUNT(*)
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += @" AND (T.TenThuoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @Keyword
                                OR TK.MaThuoc LIKE @Keyword
                                OR TK.MaLo LIKE @Keyword)";
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(maKho))
                {
                    query += " AND TK.MaKho = @MaKho";
                    cmd.Parameters.AddWithValue("@MaKho", int.Parse(maKho));
                }

                if (!string.IsNullOrEmpty(maPhong))
                {
                    query += " AND TK.MaPhong = @MaPhong";
                    cmd.Parameters.AddWithValue("@MaPhong", int.Parse(maPhong));
                }

                if (!string.IsNullOrEmpty(trangThaiTon))
                {
                    if (trangThaiTon == "het_han")
                        query += " AND TK.HanSuDung <= CAST(GETDATE() AS DATE)";
                    else if (trangThaiTon == "sap_het_han")
                        query += " AND TK.HanSuDung <= DATEADD(day, 30, CAST(GETDATE() AS DATE)) AND TK.HanSuDung > CAST(GETDATE() AS DATE)";
                    else if (trangThaiTon == "sap_het_hang")
                        query += " AND TK.SoLuongTon <= 10";
                    else if (trangThaiTon == "con_hang")
                        query += " AND TK.SoLuongTon > 10";
                }

                cmd.CommandText = query;
                return (int)cmd.ExecuteScalar();
            }
        }

        // ==================== LẤY DROPDOWN DATA ====================
        public List<PhongItem> GetAllPhong()
        {
            var list = new List<PhongItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaPhong, TenPhong FROM PHONG WHERE TrangThai = 1 ORDER BY TenPhong";
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

        public List<KhoItem> GetAllKho()
        {
            var list = new List<KhoItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaKho, TenKho, LoaiKho FROM KHO WHERE TrangThai = 1 ORDER BY TenKho";
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
                            LoaiKho = dr["LoaiKho"] != DBNull.Value ? dr["LoaiKho"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

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

        // ==================== CHI TIẾT TỒN KHO ====================
        public TonKhoViewModel GetTonKhoById(int maTonKho)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string query = @"
                    SELECT
                        TK.*,
                        T.TenThuoc, T.DonViCoBan, T.GiaBan, T.CoBHYT, T.GiaBHYT,
                        P.TenPhong,
                        NSX.TenNSX,
                        K.TenKho
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    INNER JOIN PHONG P ON TK.MaPhong = P.MaPhong
                    LEFT JOIN NHASANXUAT NSX ON T.MaNSX = NSX.MaNSX
                    LEFT JOIN KHO K ON TK.MaKho = K.MaKho
                    WHERE TK.MaTonKho = @MaTonKho";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaTonKho", maTonKho);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        var item = new TonKhoViewModel
                        {
                            MaTonKho = Convert.ToInt32(dr["MaTonKho"]),
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            MaKho = dr["MaKho"] != DBNull.Value ? Convert.ToInt32(dr["MaKho"]) : 0,
                            MaThuoc = dr["MaThuoc"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now,
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            GiaNhap = dr["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(dr["GiaNhap"]) : 0,
                            SoLuongTon = dr["SoLuongTon"] != DBNull.Value ? Convert.ToInt32(dr["SoLuongTon"]) : 0,
                            NgayCapNhat = dr["NgayCapNhat"] != DBNull.Value ? Convert.ToDateTime(dr["NgayCapNhat"]) : (DateTime?)null,
                            TenThuoc = dr["TenThuoc"] != DBNull.Value ? dr["TenThuoc"].ToString() : "",
                            DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : "",
                            GiaBan = dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : 0,
                            CoBHYT = dr["CoBHYT"] != DBNull.Value && Convert.ToBoolean(dr["CoBHYT"]),
                            GiaBHYT = dr["GiaBHYT"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBHYT"]) : 0,
                            TenPhong = dr["TenPhong"] != DBNull.Value ? dr["TenPhong"].ToString() : "",
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : "",
                            TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : ""
                        };

                        item.SoNgayConLai = (item.HanSuDung.Date - DateTime.Now.Date).Days;
                        item.GiaTriTon = item.SoLuongTon * item.GiaBan;
                        item.TrangThaiHSD = item.SoNgayConLai < 0 ? "hết_han" :
                                           item.SoNgayConLai <= 30 ? "sap_het" : "con_han";
                        item.TrangThaiTonKho = item.SoLuongTon <= 0 ? "het_hang" :
                                              item.SoLuongTon <= 10 ? "sap_het_hang" : "con_hang";

                        return item;
                    }
                }
            }
            return null;
        }

        // ==================== CẬP NHẬT TỒN KHO ====================
        public bool CapNhatSoLuongTonKho(int maTonKho, int soLuongMoi, string ghiChu = "")
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string sql = @"
                        UPDATE TONKHO 
                        SET SoLuongTon = @SoLuongMoi,
                            NgayCapNhat = GETDATE()
                        WHERE MaTonKho = @MaTonKho";

                    SqlCommand cmd = new SqlCommand(sql, conn, tran);
                    cmd.Parameters.AddWithValue("@SoLuongMoi", soLuongMoi);
                    cmd.Parameters.AddWithValue("@MaTonKho", maTonKho);

                    int rows = cmd.ExecuteNonQuery();
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

        // ==================== PHIẾU NHẬP KHO ====================
        public List<PhieuNhapViewModel> GetPhieuNhap(int page, int pageSize, string keyword, string trangThai)
        {
            var list = new List<PhieuNhapViewModel>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                var query = @"
                    SELECT
                        PN.MaPhieuNhap,
                        PN.MaNV_LapPhieu,
                        PN.MaNSX,
                        PN.NgayLap,
                        PN.TongTienNhap,
                        PN.TrangThai,
                        PN.GhiChu,
                        PN.MaNV_Duyet,
                        PN.NgayDuyet,
                        NV.HoTen AS TenNguoiLap,
                        NSX.TenNSX
                    FROM PHIEUNHAP PN
                    INNER JOIN NHANVIEN NV ON PN.MaNV_LapPhieu = NV.MaNV
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " AND (PN.MaPhieuNhap LIKE @Keyword OR NSX.TenNSX LIKE @Keyword)";
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(trangThai))
                {
                    query += " AND PN.TrangThai = @TrangThai";
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                }

                query += " ORDER BY PN.NgayLap DESC";
                query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PhieuNhapViewModel
                        {
                            MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]),
                            MaNV_LapPhieu = dr["MaNV_LapPhieu"].ToString(),
                            MaNSX = Convert.ToInt32(dr["MaNSX"]),
                            NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null,
                            TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                            MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : "",
                            NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null,
                            TenNguoiLap = dr["TenNguoiLap"] != DBNull.Value ? dr["TenNguoiLap"].ToString() : "",
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : ""
                        });
                    }
                }
            }

            return list;
        }

        public int GetPhieuNhapCount(string keyword, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                var query = @"
                    SELECT COUNT(*) 
                    FROM PHIEUNHAP PN
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " AND (PN.MaPhieuNhap LIKE @Keyword OR NSX.TenNSX LIKE @Keyword)";
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(trangThai))
                {
                    query += " AND PN.TrangThai = @TrangThai";
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                }

                cmd.CommandText = query;
                return (int)cmd.ExecuteScalar();
            }
        }

        // Chi tiết phiếu nhập
        public PhieuNhapViewModel GetPhieuNhapById(int maPhieuNhap)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string query = @"
                    SELECT
                        PN.*,
                        NV.HoTen AS TenNguoiLap,
                        NV_Duyet.HoTen AS TenNguoiDuyet,
                        NSX.TenNSX
                    FROM PHIEUNHAP PN
                    INNER JOIN NHANVIEN NV ON PN.MaNV_LapPhieu = NV.MaNV
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    LEFT JOIN NHANVIEN NV_Duyet ON PN.MaNV_Duyet = NV_Duyet.MaNV
                    WHERE PN.MaPhieuNhap = @MaPhieuNhap";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new PhieuNhapViewModel
                        {
                            MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]),
                            MaNV_LapPhieu = dr["MaNV_LapPhieu"].ToString(),
                            MaNSX = Convert.ToInt32(dr["MaNSX"]),
                            NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null,
                            TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                            MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : "",
                            NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null,
                            TenNguoiLap = dr["TenNguoiLap"] != DBNull.Value ? dr["TenNguoiLap"].ToString() : "",
                            TenNguoiDuyet = dr["TenNguoiDuyet"] != DBNull.Value ? dr["TenNguoiDuyet"].ToString() : "",
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : ""
                        };
                    }
                }
            }
            return null;
        }

        // Chi tiết thuốc trong phiếu nhập
        public List<CT_PhieuNhapViewModel> GetCTPhieuNhap(int maPhieuNhap)
        {
            var list = new List<CT_PhieuNhapViewModel>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string query = @"
                    SELECT
                        CTP.*,
                        T.TenThuoc,
                        T.DonViCoBan
                    FROM CT_PHIEUNHAP CTP
                    INNER JOIN THUOC T ON CTP.MaThuoc = T.MaThuoc
                    WHERE CTP.MaPhieuNhap = @MaPhieuNhap
                    ORDER BY CTP.MaCTPN";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CT_PhieuNhapViewModel
                        {
                            MaCTPN = Convert.ToInt32(dr["MaCTPN"]),
                            MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now,
                            SoLuongNhap = dr["SoLuongNhap"] != DBNull.Value ? Convert.ToInt32(dr["SoLuongNhap"]) : 0,
                            DonGiaNhap = dr["DonGiaNhap"] != DBNull.Value ? Convert.ToDecimal(dr["DonGiaNhap"]) : 0,
                            ThanhTien = dr["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(dr["ThanhTien"]) : 0,
                            TenThuoc = dr["TenThuoc"] != DBNull.Value ? dr["TenThuoc"].ToString() : "",
                            DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : ""
                        });
                    }
                }
            }

            return list;
        }

        // ==================== TẠO PHIẾU NHẬP ====================
        public int TaoPhieuNhap(string maNV, int maNSX, string ghiChu, List<CT_PhieuNhapInput> chiTiets)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Tạo phiếu nhập
                    string sqlPN = @"
                        INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, TongTienNhap, TrangThai, GhiChu)
                        VALUES (@MaNV, @MaNSX, 0, N'Chờ duyệt', @GhiChu);
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdPN = new SqlCommand(sqlPN, conn, tran);
                    cmdPN.Parameters.AddWithValue("@MaNV", maNV);
                    cmdPN.Parameters.AddWithValue("@MaNSX", maNSX);
                    cmdPN.Parameters.AddWithValue("@GhiChu", (object)ghiChu ?? DBNull.Value);

                    int maPhieuNhap = Convert.ToInt32(cmdPN.ExecuteScalar());

                    decimal tongTien = 0;

                    // 2. Thêm chi tiết phiếu nhập
                    foreach (var ct in chiTiets)
                    {
                        decimal thanhTien = ct.SoLuongNhap * ct.DonGiaNhap;
                        tongTien += thanhTien;

                        string sqlCT = @"
                            INSERT INTO CT_PHIEUNHAP (MaPhieuNhap, MaThuoc, MaLo, NgaySanXuat, HanSuDung, SoLuongNhap, DonGiaNhap, ThanhTien)
                            VALUES (@MaPhieuNhap, @MaThuoc, @MaLo, @NgaySanXuat, @HanSuDung, @SoLuongNhap, @DonGiaNhap, @ThanhTien)";

                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, tran);
                        cmdCT.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                        cmdCT.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdCT.Parameters.AddWithValue("@MaLo", ct.MaLo);
                        cmdCT.Parameters.AddWithValue("@NgaySanXuat", (object)ct.NgaySanXuat ?? DBNull.Value);
                        cmdCT.Parameters.AddWithValue("@HanSuDung", ct.HanSuDung);
                        cmdCT.Parameters.AddWithValue("@SoLuongNhap", ct.SoLuongNhap);
                        cmdCT.Parameters.AddWithValue("@DonGiaNhap", ct.DonGiaNhap);
                        cmdCT.Parameters.AddWithValue("@ThanhTien", thanhTien);
                        cmdCT.ExecuteNonQuery();
                    }

                    // 3. Cập nhật tổng tiền
                    string sqlUpdate = "UPDATE PHIEUNHAP SET TongTienNhap = @TongTien WHERE MaPhieuNhap = @MaPhieuNhap";
                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, tran);
                    cmdUpdate.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdUpdate.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    cmdUpdate.ExecuteNonQuery();

                    tran.Commit();
                    return maPhieuNhap;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        // ==================== DUYỆT/HỦY PHIẾU NHẬP ====================
        public bool DuyetPhieuNhap(int maPhieuNhap, string maNVDuyet, int maKhoNhan)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Lấy chi tiết phiếu nhập
                    var chiTiets = GetCTPhieuNhap(maPhieuNhap);

                    // 2. Cập nhật tồn kho theo kho nhận
                    foreach (var ct in chiTiets)
                    {
                        // Kiểm tra đã có lô trong kho chưa
                        string sqlCheck = @"
                            SELECT MaTonKho, SoLuongTon FROM TONKHO
                            WHERE MaKho = @MaKho AND MaThuoc = @MaThuoc AND MaLo = @MaLo";

                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, tran);
                        cmdCheck.Parameters.AddWithValue("@MaKho", maKhoNhan);
                        cmdCheck.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdCheck.Parameters.AddWithValue("@MaLo", ct.MaLo);

                        var existing = cmdCheck.ExecuteScalar();

                        if (existing != null)
                        {
                            // Cộng dồn số lượng
                            string sqlUpdate = @"
                                UPDATE TONKHO
                                SET SoLuongTon = SoLuongTon + @SoLuongNhap,
                                    NgayCapNhat = GETDATE()
                                WHERE MaKho = @MaKho AND MaThuoc = @MaThuoc AND MaLo = @MaLo";
                            SqlCommand cmdU = new SqlCommand(sqlUpdate, conn, tran);
                            cmdU.Parameters.AddWithValue("@SoLuongNhap", ct.SoLuongNhap);
                            cmdU.Parameters.AddWithValue("@MaKho", maKhoNhan);
                            cmdU.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                            cmdU.Parameters.AddWithValue("@MaLo", ct.MaLo);
                            cmdU.ExecuteNonQuery();
                        }
                        else
                        {
                            // Thêm mới dòng tồn kho
                            // Lấy MaPhong mặc định của kho từ PHONG
                            SqlCommand cmdPhong = new SqlCommand(
                                "SELECT TOP 1 MaPhong FROM PHONG WHERE TrangThai = 1 ORDER BY MaPhong", conn, tran);
                            int maPhongDefault = Convert.ToInt32(cmdPhong.ExecuteScalar());

                            string sqlInsert = @"
                                INSERT INTO TONKHO (MaKho, MaPhong, MaThuoc, MaLo, HanSuDung, NgaySanXuat, GiaNhap, SoLuongTon)
                                VALUES (@MaKho, @MaPhong, @MaThuoc, @MaLo, @HanSuDung, @NgaySanXuat, @GiaNhap, @SoLuongNhap)";
                            SqlCommand cmdI = new SqlCommand(sqlInsert, conn, tran);
                            cmdI.Parameters.AddWithValue("@MaKho", maKhoNhan);
                            cmdI.Parameters.AddWithValue("@MaPhong", maPhongDefault);
                            cmdI.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                            cmdI.Parameters.AddWithValue("@MaLo", ct.MaLo);
                            cmdI.Parameters.AddWithValue("@HanSuDung", ct.HanSuDung);
                            cmdI.Parameters.AddWithValue("@NgaySanXuat", (object)ct.NgaySanXuat ?? DBNull.Value);
                            cmdI.Parameters.AddWithValue("@GiaNhap", ct.DonGiaNhap);
                            cmdI.Parameters.AddWithValue("@SoLuongNhap", ct.SoLuongNhap);
                            cmdI.ExecuteNonQuery();
                        }
                    }

                    // 3. Cập nhật phiếu nhập
                    string sqlPN = @"
                        UPDATE PHIEUNHAP
                        SET TrangThai = N'Đã duyệt',
                            MaNV_Duyet = @MaNVDuyet,
                            NgayDuyet = GETDATE()
                        WHERE MaPhieuNhap = @MaPhieuNhap";

                    SqlCommand cmdPN = new SqlCommand(sqlPN, conn, tran);
                    cmdPN.Parameters.AddWithValue("@MaNVDuyet", maNVDuyet);
                    cmdPN.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    cmdPN.ExecuteNonQuery();

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

        public bool HuyPhieuNhap(int maPhieuNhap, string maNVHuy)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string sql = @"
                        UPDATE PHIEUNHAP 
                        SET TrangThai = N'Đã hủy'
                        WHERE MaPhieuNhap = @MaPhieuNhap AND TrangThai = N'Chờ duyệt'";

                    SqlCommand cmd = new SqlCommand(sql, conn, tran);
                    cmd.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    int rows = cmd.ExecuteNonQuery();

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
    }

    // ==================== CLASS HỖ TRỢ ====================
    public class KhoTongQuan
    {
        public int SoMatHang { get; set; }
        public int SoKho { get; set; }
        public int SoMatHangTheoKho { get; set; }
        public decimal TongGiaTriTon { get; set; }
        public int TongSoLuongTon { get; set; }
        public int SoHetHan { get; set; }
        public int SoItHang { get; set; }
    }

    public class TonKhoViewModel
    {
        public int MaTonKho { get; set; }
        public int MaPhong { get; set; }
        public int MaKho { get; set; }
        public string MaThuoc { get; set; }
        public string MaLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public decimal GiaNhap { get; set; }
        public int SoLuongTon { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        public string TenThuoc { get; set; }
        public string DonViCoBan { get; set; }
        public decimal GiaBan { get; set; }
        public bool CoBHYT { get; set; }
        public decimal GiaBHYT { get; set; }
        public string TenPhong { get; set; }
        public string TenNSX { get; set; }
        public string TenKho { get; set; }

        // Tính toán
        public int SoNgayConLai { get; set; }
        public decimal GiaTriTon { get; set; }
        public string TrangThaiHSD { get; set; }
        public string TrangThaiTonKho { get; set; }

        // Display
        public string HanSuDungDisplay => HanSuDung.ToString("dd/MM/yyyy");
        public string NgaySanXuatDisplay => NgaySanXuat.HasValue ? NgaySanXuat.Value.ToString("dd/MM/yyyy") : "-";
    }

    public class PhieuNhapViewModel
    {
        public int MaPhieuNhap { get; set; }
        public string MaNV_LapPhieu { get; set; }
        public int MaNSX { get; set; }
        public DateTime? NgayLap { get; set; }
        public decimal TongTienNhap { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string MaNV_Duyet { get; set; }
        public DateTime? NgayDuyet { get; set; }

        public string TenNguoiLap { get; set; }
        public string TenNguoiDuyet { get; set; }
        public string TenNSX { get; set; }

        public string NgayLapDisplay => NgayLap.HasValue ? NgayLap.Value.ToString("dd/MM/yyyy HH:mm") : "";
        public string NgayDuyetDisplay => NgayDuyet.HasValue ? NgayDuyet.Value.ToString("dd/MM/yyyy HH:mm") : "";
    }

    public class CT_PhieuNhapViewModel
    {
        public int MaCTPN { get; set; }
        public int MaPhieuNhap { get; set; }
        public string MaThuoc { get; set; }
        public string MaLo { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal ThanhTien { get; set; }

        public string TenThuoc { get; set; }
        public string DonViCoBan { get; set; }

        public string NgaySanXuatDisplay => NgaySanXuat.HasValue ? NgaySanXuat.Value.ToString("dd/MM/yyyy") : "-";
        public string HanSuDungDisplay => HanSuDung.ToString("dd/MM/yyyy");
    }

    public class CT_PhieuNhapInput
    {
        public string MaThuoc { get; set; }
        public string MaLo { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
    }

    public class PhongItem
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; }
    }

    public class KhoItem
    {
        public int MaKho { get; set; }
        public string TenKho { get; set; }
        public string LoaiKho { get; set; }
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
}
