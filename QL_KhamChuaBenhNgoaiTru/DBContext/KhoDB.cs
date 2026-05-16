using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using QL_KhamChuaBenhNgoaiTru.Models;

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
        public List<TonKhoViewModel> GetTonKho(int page, int pageSize, string keyword, string maKho, string trangThaiTon)
        {
            var list = new List<TonKhoViewModel>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                var query = @"
                    SELECT
                        TK.MaTonKho,
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
                        K.TenKho
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
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

        public int GetTonKhoCount(string keyword, string maKho, string trangThaiTon)
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
                        T.TenThuoc, T.DonViCoBan, T.GiaBan, T.CoBHYT,
                        NSX.TenNSX,
                        K.TenKho
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
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
                            GiaBHYT = dr["CoBHYT"] != DBNull.Value && Convert.ToBoolean(dr["CoBHYT"])
                                ? (dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : 0)
                                : 0,
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
                        PN.MaKho,
                        K.TenKho,
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
                    LEFT JOIN KHO K ON PN.MaKho = K.MaKho
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
                            MaKho = dr["MaKho"] != DBNull.Value ? Convert.ToInt32(dr["MaKho"]) : (int?)null,
                            NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null,
                            TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                            MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : "",
                            NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null,
                            TenNguoiLap = dr["TenNguoiLap"] != DBNull.Value ? dr["TenNguoiLap"].ToString() : "",
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : "",
                            TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : ""
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
                        NSX.TenNSX,
                        K.TenKho
                    FROM PHIEUNHAP PN
                    INNER JOIN NHANVIEN NV ON PN.MaNV_LapPhieu = NV.MaNV
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    LEFT JOIN NHANVIEN NV_Duyet ON PN.MaNV_Duyet = NV_Duyet.MaNV
                    LEFT JOIN KHO K ON PN.MaKho = K.MaKho
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
                            MaKho = dr["MaKho"] != DBNull.Value ? Convert.ToInt32(dr["MaKho"]) : (int?)null,
                            NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null,
                            TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                            MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : "",
                            NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null,
                            TenNguoiLap = dr["TenNguoiLap"] != DBNull.Value ? dr["TenNguoiLap"].ToString() : "",
                            TenNguoiDuyet = dr["TenNguoiDuyet"] != DBNull.Value ? dr["TenNguoiDuyet"].ToString() : "",
                            TenNSX = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : "",
                            TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : ""
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
        public int TaoPhieuNhap(string maNV, int maNSX, int maKho, string ghiChu, List<CT_PhieuNhapInput> chiTiets)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Tạo phiếu nhập (MaKho đã có sau ALTER TABLE)
                    string sqlPN = @"
                        INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, TongTienNhap, TrangThai, GhiChu)
                        VALUES (@MaNV, @MaNSX, @MaKho, 0, N'Chờ duyệt', @GhiChu);
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdPN = new SqlCommand(sqlPN, conn, tran);
                    cmdPN.Parameters.AddWithValue("@MaNV", maNV);
                    cmdPN.Parameters.AddWithValue("@MaNSX", maNSX);
                    cmdPN.Parameters.AddWithValue("@MaKho", maKho);
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

        // ==================== DUYỆT/HỦY/XÓA CHI TIẾT PHIẾU NHẬP ====================
        public bool XoaChiTietPhieuNhap(int maCTPN)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // Lấy MaPhieuNhap và ThanhTien của chi tiết sắp xóa
                    string sqlGetInfo = "SELECT MaPhieuNhap, ThanhTien FROM CT_PHIEUNHAP WHERE MaCTPN = @MaCTPN";
                    SqlCommand cmdGet = new SqlCommand(sqlGetInfo, conn, tran);
                    cmdGet.Parameters.AddWithValue("@MaCTPN", maCTPN);
                    
                    int maPhieuNhap = 0;
                    decimal thanhTienXoa = 0;

                    using (SqlDataReader dr = cmdGet.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            maPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]);
                            thanhTienXoa = Convert.ToDecimal(dr["ThanhTien"]);
                        }
                    }

                    if (maPhieuNhap == 0)
                    {
                        tran.Rollback();
                        return false;
                    }

                    // Kiểm tra trạng thái phiếu
                    string sqlCheckTrangThai = "SELECT TrangThai FROM PHIEUNHAP WHERE MaPhieuNhap = @MaPhieuNhap";
                    SqlCommand cmdCheck = new SqlCommand(sqlCheckTrangThai, conn, tran);
                    cmdCheck.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    var trangThai = cmdCheck.ExecuteScalar()?.ToString();

                    if (trangThai != "Chờ duyệt")
                    {
                        tran.Rollback();
                        return false;
                    }

                    // Xóa chi tiết
                    string sqlDelete = "DELETE FROM CT_PHIEUNHAP WHERE MaCTPN = @MaCTPN";
                    SqlCommand cmdDel = new SqlCommand(sqlDelete, conn, tran);
                    cmdDel.Parameters.AddWithValue("@MaCTPN", maCTPN);
                    cmdDel.ExecuteNonQuery();

                    // Cập nhật lại tổng tiền phiếu nhập
                    string sqlUpdate = "UPDATE PHIEUNHAP SET TongTienNhap = TongTienNhap - @ThanhTienXoa WHERE MaPhieuNhap = @MaPhieuNhap";
                    SqlCommand cmdUpd = new SqlCommand(sqlUpdate, conn, tran);
                    cmdUpd.Parameters.AddWithValue("@ThanhTienXoa", thanhTienXoa);
                    cmdUpd.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    cmdUpd.ExecuteNonQuery();

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

        // ==================== DUYỆT/HỦY PHIẾU NHẬP ====================
        public bool DuyetPhieuNhap(int maPhieuNhap, string maNVDuyet)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Lấy MaKho từ phiếu nhập
                    SqlCommand cmdPN = new SqlCommand(
                        "SELECT MaKho FROM PHIEUNHAP WHERE MaPhieuNhap = @MaPhieuNhap", conn, tran);
                    cmdPN.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    var maKhoObj = cmdPN.ExecuteScalar();

                    if (maKhoObj == null || maKhoObj == DBNull.Value)
                    {
                        tran.Rollback();
                        return false;
                    }

                    int maKhoNhan = Convert.ToInt32(maKhoObj);

                    // 2. Lấy chi tiết phiếu nhập
                    var chiTiets = GetCTPhieuNhap(maPhieuNhap);

                    // 3. Cập nhật tồn kho theo kho nhận
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
                            string sqlInsert = @"
                                INSERT INTO TONKHO (MaKho, MaThuoc, MaLo, HanSuDung, NgaySanXuat, GiaNhap, SoLuongTon)
                                VALUES (@MaKho, @MaThuoc, @MaLo, @HanSuDung, @NgaySanXuat, @GiaNhap, @SoLuongNhap)";
                            SqlCommand cmdI = new SqlCommand(sqlInsert, conn, tran);
                            cmdI.Parameters.AddWithValue("@MaKho", maKhoNhan);
                            cmdI.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                            cmdI.Parameters.AddWithValue("@MaLo", ct.MaLo);
                            cmdI.Parameters.AddWithValue("@HanSuDung", ct.HanSuDung);
                            cmdI.Parameters.AddWithValue("@NgaySanXuat", (object)ct.NgaySanXuat ?? DBNull.Value);
                            cmdI.Parameters.AddWithValue("@GiaNhap", ct.DonGiaNhap);
                            cmdI.Parameters.AddWithValue("@SoLuongNhap", ct.SoLuongNhap);
                            cmdI.ExecuteNonQuery();
                        }
                    }

                    // 4. Cập nhật phiếu nhập
                    string sqlPN = @"
                        UPDATE PHIEUNHAP
                        SET TrangThai = N'Đã duyệt',
                            MaNV_Duyet = @MaNVDuyet,
                            NgayDuyet = GETDATE()
                        WHERE MaPhieuNhap = @MaPhieuNhap";

                    SqlCommand cmdUPN = new SqlCommand(sqlPN, conn, tran);
                    cmdUPN.Parameters.AddWithValue("@MaNVDuyet", maNVDuyet);
                    cmdUPN.Parameters.AddWithValue("@MaPhieuNhap", maPhieuNhap);
                    cmdUPN.ExecuteNonQuery();

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
        // ==================== THUỐC TỒN KHO THEO KHO ====================
        public List<TonKhoViewModel> GetThuocTonKhoByMaKho(int maKho)
        {
            var list = new List<TonKhoViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        TK.MaTonKho, TK.MaKho, TK.MaThuoc, TK.MaLo, TK.HanSuDung, TK.NgaySanXuat, TK.GiaNhap, TK.SoLuongTon,
                        T.TenThuoc, T.DonViCoBan
                    FROM TONKHO TK
                    INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    WHERE TK.MaKho = @MaKho AND TK.SoLuongTon > 0
                    ORDER BY T.TenThuoc, TK.HanSuDung";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaKho", maKho);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new TonKhoViewModel
                        {
                            MaTonKho = Convert.ToInt32(dr["MaTonKho"]),
                            MaKho = Convert.ToInt32(dr["MaKho"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            HanSuDung = Convert.ToDateTime(dr["HanSuDung"]),
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            GiaNhap = Convert.ToDecimal(dr["GiaNhap"]),
                            SoLuongTon = Convert.ToInt32(dr["SoLuongTon"]),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            DonViCoBan = dr["DonViCoBan"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==================== THỐNG KÊ KHO ====================
        public List<KhoThongKe> GetThongKeCacKho()
        {
            var list = new List<KhoThongKe>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        K.MaKho, 
                        K.TenKho,
                        COUNT(DISTINCT TK.MaThuoc) as SoMatHang,
                        ISNULL(SUM(TK.SoLuongTon), 0) as TongSoLuong,
                        ISNULL(SUM(TK.SoLuongTon * ISNULL(T.GiaBan, 0)), 0) as TongGiaTri
                    FROM KHO K
                    LEFT JOIN TONKHO TK ON K.MaKho = TK.MaKho AND TK.SoLuongTon > 0
                    LEFT JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
                    WHERE K.TrangThai = 1
                    GROUP BY K.MaKho, K.TenKho
                    ORDER BY K.TenKho";

                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new KhoThongKe
                        {
                            MaKho = Convert.ToInt32(dr["MaKho"]),
                            TenKho = dr["TenKho"].ToString(),
                            SoMatHang = Convert.ToInt32(dr["SoMatHang"]),
                            TongSoLuong = Convert.ToInt32(dr["TongSoLuong"]),
                            TongGiaTri = Convert.ToDecimal(dr["TongGiaTri"])
                        });
                    }
                }
            }
            return list;
        }

        // ==================== PHIẾU CHUYỂN KHO ====================
        public int TaoPhieuChuyen(string maNV, int maKhoNguon, int maKhoDich, string ghiChu, List<CT_PhieuChuyenInput> chiTiets)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string sqlPC = @"
                        INSERT INTO PHIEUCHUYENKHO (MaKhoNguon, MaKhoDich, MaNV_LapPhieu, TrangThai, GhiChu)
                        VALUES (@MaKhoNguon, @MaKhoDich, @MaNV, N'Chờ duyệt', @GhiChu);
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdPC = new SqlCommand(sqlPC, conn, tran);
                    cmdPC.Parameters.AddWithValue("@MaKhoNguon", maKhoNguon);
                    cmdPC.Parameters.AddWithValue("@MaKhoDich", maKhoDich);
                    cmdPC.Parameters.AddWithValue("@MaNV", maNV);
                    cmdPC.Parameters.AddWithValue("@GhiChu", (object)ghiChu ?? DBNull.Value);

                    int maPhieuChuyen = Convert.ToInt32(cmdPC.ExecuteScalar());

                    foreach (var ct in chiTiets)
                    {
                        string sqlCT = @"
                            INSERT INTO CT_PHIEUCHUYENKHO (MaPhieuChuyen, MaThuoc, MaLo, NgaySanXuat, HanSuDung, SoLuongChuyen)
                            VALUES (@MaPhieuChuyen, @MaThuoc, @MaLo, @NgaySanXuat, @HanSuDung, @SoLuongChuyen)";

                        SqlCommand cmdCT = new SqlCommand(sqlCT, conn, tran);
                        cmdCT.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);
                        cmdCT.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdCT.Parameters.AddWithValue("@MaLo", ct.MaLo);
                        cmdCT.Parameters.AddWithValue("@NgaySanXuat", (object)ct.NgaySanXuat ?? DBNull.Value);
                        cmdCT.Parameters.AddWithValue("@HanSuDung", ct.HanSuDung);
                        cmdCT.Parameters.AddWithValue("@SoLuongChuyen", ct.SoLuongChuyen);
                        cmdCT.ExecuteNonQuery();
                    }

                    tran.Commit();
                    return maPhieuChuyen;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public List<PhieuChuyenKhoViewModel> GetPhieuChuyen(int page, int pageSize, string keyword, string trangThai)
        {
            var list = new List<PhieuChuyenKhoViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                var query = @"
                    SELECT
                        PC.MaPhieuChuyen,
                        PC.MaKhoNguon, KN.TenKho AS TenKhoNguon,
                        PC.MaKhoDich, KD.TenKho AS TenKhoDich,
                        PC.NgayChuyen,
                        PC.TrangThai,
                        PC.GhiChu,
                        PC.MaNV_LapPhieu, NV1.HoTen AS TenNguoiLap,
                        PC.MaNV_Duyet, NV2.HoTen AS TenNguoiDuyet,
                        PC.NgayDuyet
                    FROM PHIEUCHUYENKHO PC
                    INNER JOIN KHO KN ON PC.MaKhoNguon = KN.MaKho
                    INNER JOIN KHO KD ON PC.MaKhoDich = KD.MaKho
                    INNER JOIN NHANVIEN NV1 ON PC.MaNV_LapPhieu = NV1.MaNV
                    LEFT JOIN NHANVIEN NV2 ON PC.MaNV_Duyet = NV2.MaNV
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " AND (PC.MaPhieuChuyen LIKE @Keyword OR KN.TenKho LIKE @Keyword OR KD.TenKho LIKE @Keyword)";
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }
                if (!string.IsNullOrEmpty(trangThai))
                {
                    query += " AND PC.TrangThai = @TrangThai";
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                }

                query += " ORDER BY PC.NgayChuyen DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PhieuChuyenKhoViewModel
                        {
                            MaPhieuChuyen = Convert.ToInt32(dr["MaPhieuChuyen"]),
                            MaKhoNguon = Convert.ToInt32(dr["MaKhoNguon"]),
                            TenKhoNguon = dr["TenKhoNguon"].ToString(),
                            MaKhoDich = Convert.ToInt32(dr["MaKhoDich"]),
                            TenKhoDich = dr["TenKhoDich"].ToString(),
                            NgayChuyen = dr["NgayChuyen"] != DBNull.Value ? Convert.ToDateTime(dr["NgayChuyen"]) : (DateTime?)null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                            MaNV_LapPhieu = dr["MaNV_LapPhieu"].ToString(),
                            TenNguoiLap = dr["TenNguoiLap"].ToString(),
                            MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : "",
                            TenNguoiDuyet = dr["TenNguoiDuyet"] != DBNull.Value ? dr["TenNguoiDuyet"].ToString() : "",
                            NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null
                        });
                    }
                }
            }
            return list;
        }

        public int GetPhieuChuyenCount(string keyword, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                var query = @"
                    SELECT COUNT(*) 
                    FROM PHIEUCHUYENKHO PC
                    INNER JOIN KHO KN ON PC.MaKhoNguon = KN.MaKho
                    INNER JOIN KHO KD ON PC.MaKhoDich = KD.MaKho
                    WHERE 1=1";

                var cmd = new SqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " AND (PC.MaPhieuChuyen LIKE @Keyword OR KN.TenKho LIKE @Keyword OR KD.TenKho LIKE @Keyword)";
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                }
                if (!string.IsNullOrEmpty(trangThai))
                {
                    query += " AND PC.TrangThai = @TrangThai";
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                }

                cmd.CommandText = query;
                return (int)cmd.ExecuteScalar();
            }
        }

        public PhieuChuyenKhoViewModel GetPhieuChuyenById(int maPhieuChuyen)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string query = @"
                    SELECT
                        PC.MaPhieuChuyen,
                        PC.MaKhoNguon, KN.TenKho AS TenKhoNguon,
                        PC.MaKhoDich, KD.TenKho AS TenKhoDich,
                        PC.NgayChuyen,
                        PC.TrangThai,
                        PC.GhiChu,
                        PC.MaNV_LapPhieu, NV1.HoTen AS TenNguoiLap,
                        PC.MaNV_Duyet, NV2.HoTen AS TenNguoiDuyet,
                        PC.NgayDuyet
                    FROM PHIEUCHUYENKHO PC
                    INNER JOIN KHO KN ON PC.MaKhoNguon = KN.MaKho
                    INNER JOIN KHO KD ON PC.MaKhoDich = KD.MaKho
                    INNER JOIN NHANVIEN NV1 ON PC.MaNV_LapPhieu = NV1.MaNV
                    LEFT JOIN NHANVIEN NV2 ON PC.MaNV_Duyet = NV2.MaNV
                    WHERE PC.MaPhieuChuyen = @MaPhieuChuyen";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new PhieuChuyenKhoViewModel
                        {
                            MaPhieuChuyen = Convert.ToInt32(dr["MaPhieuChuyen"]),
                            MaKhoNguon = Convert.ToInt32(dr["MaKhoNguon"]),
                            TenKhoNguon = dr["TenKhoNguon"].ToString(),
                            MaKhoDich = Convert.ToInt32(dr["MaKhoDich"]),
                            TenKhoDich = dr["TenKhoDich"].ToString(),
                            NgayChuyen = dr["NgayChuyen"] != DBNull.Value ? Convert.ToDateTime(dr["NgayChuyen"]) : (DateTime?)null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            GhiChu = dr["GhiChu"] != DBNull.Value ? dr["GhiChu"].ToString() : "",
                            MaNV_LapPhieu = dr["MaNV_LapPhieu"].ToString(),
                            TenNguoiLap = dr["TenNguoiLap"].ToString(),
                            MaNV_Duyet = dr["MaNV_Duyet"] != DBNull.Value ? dr["MaNV_Duyet"].ToString() : "",
                            TenNguoiDuyet = dr["TenNguoiDuyet"] != DBNull.Value ? dr["TenNguoiDuyet"].ToString() : "",
                            NgayDuyet = dr["NgayDuyet"] != DBNull.Value ? Convert.ToDateTime(dr["NgayDuyet"]) : (DateTime?)null
                        };
                    }
                }
            }
            return null;
        }

        public List<CT_PhieuChuyenKhoViewModel> GetCTPhieuChuyen(int maPhieuChuyen)
        {
            var list = new List<CT_PhieuChuyenKhoViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string query = @"
                    SELECT
                        CTP.*,
                        T.TenThuoc,
                        T.DonViCoBan
                    FROM CT_PHIEUCHUYENKHO CTP
                    INNER JOIN THUOC T ON CTP.MaThuoc = T.MaThuoc
                    WHERE CTP.MaPhieuChuyen = @MaPhieuChuyen
                    ORDER BY CTP.MaCTPC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CT_PhieuChuyenKhoViewModel
                        {
                            MaCTPC = Convert.ToInt32(dr["MaCTPC"]),
                            MaPhieuChuyen = Convert.ToInt32(dr["MaPhieuChuyen"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            MaLo = dr["MaLo"].ToString(),
                            NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                            HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now,
                            SoLuongChuyen = dr["SoLuongChuyen"] != DBNull.Value ? Convert.ToInt32(dr["SoLuongChuyen"]) : 0,
                            TenThuoc = dr["TenThuoc"] != DBNull.Value ? dr["TenThuoc"].ToString() : "",
                            DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public bool DuyetPhieuChuyen(int maPhieuChuyen, string maNVDuyet)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    SqlCommand cmdPC = new SqlCommand(
                        "SELECT MaKhoNguon, MaKhoDich FROM PHIEUCHUYENKHO WHERE MaPhieuChuyen = @MaPhieuChuyen AND TrangThai = N'Chờ duyệt'", conn, tran);
                    cmdPC.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);
                    int maKhoNguon = 0, maKhoDich = 0;

                    using (SqlDataReader dr = cmdPC.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            maKhoNguon = Convert.ToInt32(dr["MaKhoNguon"]);
                            maKhoDich = Convert.ToInt32(dr["MaKhoDich"]);
                        }
                    }

                    if (maKhoNguon == 0 || maKhoDich == 0)
                    {
                        tran.Rollback();
                        return false;
                    }

                    var chiTiets = new List<CT_PhieuChuyenKhoViewModel>();
                    string sqlGetCT = "SELECT * FROM CT_PHIEUCHUYENKHO WHERE MaPhieuChuyen = @MaPhieuChuyen";
                    using (SqlCommand cmdCT = new SqlCommand(sqlGetCT, conn, tran))
                    {
                        cmdCT.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);
                        using (SqlDataReader dr = cmdCT.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                chiTiets.Add(new CT_PhieuChuyenKhoViewModel
                                {
                                    MaThuoc = dr["MaThuoc"].ToString(),
                                    MaLo = dr["MaLo"].ToString(),
                                    NgaySanXuat = dr["NgaySanXuat"] != DBNull.Value ? Convert.ToDateTime(dr["NgaySanXuat"]) : (DateTime?)null,
                                    HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now,
                                    SoLuongChuyen = Convert.ToInt32(dr["SoLuongChuyen"])
                                });
                            }
                        }
                    }

                    foreach (var ct in chiTiets)
                    {
                        // 1. Trừ tồn kho nguồn
                        string sqlUpdateNguon = @"
                            UPDATE TONKHO 
                            SET SoLuongTon = SoLuongTon - @SoLuong, NgayCapNhat = GETDATE() 
                            WHERE MaKho = @MaKhoNguon AND MaThuoc = @MaThuoc AND MaLo = @MaLo AND SoLuongTon >= @SoLuong";
                        SqlCommand cmdUNguon = new SqlCommand(sqlUpdateNguon, conn, tran);
                        cmdUNguon.Parameters.AddWithValue("@SoLuong", ct.SoLuongChuyen);
                        cmdUNguon.Parameters.AddWithValue("@MaKhoNguon", maKhoNguon);
                        cmdUNguon.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdUNguon.Parameters.AddWithValue("@MaLo", ct.MaLo);
                        
                        int rowsNguon = cmdUNguon.ExecuteNonQuery();
                        if (rowsNguon == 0)
                        {
                            // Không đủ tồn kho nguồn, huỷ bỏ
                            tran.Rollback();
                            return false;
                        }

                        // Lấy Giá Nhập từ lô nguồn
                        decimal giaNhap = 0;
                        string sqlGiaNhap = "SELECT TOP 1 GiaNhap FROM TONKHO WHERE MaKho = @MaKhoNguon AND MaThuoc = @MaThuoc AND MaLo = @MaLo";
                        SqlCommand cmdGN = new SqlCommand(sqlGiaNhap, conn, tran);
                        cmdGN.Parameters.AddWithValue("@MaKhoNguon", maKhoNguon);
                        cmdGN.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdGN.Parameters.AddWithValue("@MaLo", ct.MaLo);
                        var giaNhapObj = cmdGN.ExecuteScalar();
                        if (giaNhapObj != null && giaNhapObj != DBNull.Value) giaNhap = Convert.ToDecimal(giaNhapObj);

                        // 2. Cộng tồn kho đích
                        string sqlCheckDich = "SELECT MaTonKho FROM TONKHO WHERE MaKho = @MaKhoDich AND MaThuoc = @MaThuoc AND MaLo = @MaLo";
                        SqlCommand cmdCheckD = new SqlCommand(sqlCheckDich, conn, tran);
                        cmdCheckD.Parameters.AddWithValue("@MaKhoDich", maKhoDich);
                        cmdCheckD.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                        cmdCheckD.Parameters.AddWithValue("@MaLo", ct.MaLo);
                        var existing = cmdCheckD.ExecuteScalar();

                        if (existing != null)
                        {
                            string sqlUpdateDich = @"
                                UPDATE TONKHO
                                SET SoLuongTon = SoLuongTon + @SoLuong, NgayCapNhat = GETDATE()
                                WHERE MaKho = @MaKhoDich AND MaThuoc = @MaThuoc AND MaLo = @MaLo";
                            SqlCommand cmdUDich = new SqlCommand(sqlUpdateDich, conn, tran);
                            cmdUDich.Parameters.AddWithValue("@SoLuong", ct.SoLuongChuyen);
                            cmdUDich.Parameters.AddWithValue("@MaKhoDich", maKhoDich);
                            cmdUDich.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                            cmdUDich.Parameters.AddWithValue("@MaLo", ct.MaLo);
                            cmdUDich.ExecuteNonQuery();
                        }
                        else
                        {
                            string sqlInsertDich = @"
                                INSERT INTO TONKHO (MaKho, MaThuoc, MaLo, HanSuDung, NgaySanXuat, GiaNhap, SoLuongTon)
                                VALUES (@MaKhoDich, @MaThuoc, @MaLo, @HanSuDung, @NgaySanXuat, @GiaNhap, @SoLuong)";
                            SqlCommand cmdIDich = new SqlCommand(sqlInsertDich, conn, tran);
                            cmdIDich.Parameters.AddWithValue("@MaKhoDich", maKhoDich);
                            cmdIDich.Parameters.AddWithValue("@MaThuoc", ct.MaThuoc);
                            cmdIDich.Parameters.AddWithValue("@MaLo", ct.MaLo);
                            cmdIDich.Parameters.AddWithValue("@HanSuDung", ct.HanSuDung);
                            cmdIDich.Parameters.AddWithValue("@NgaySanXuat", (object)ct.NgaySanXuat ?? DBNull.Value);
                            cmdIDich.Parameters.AddWithValue("@GiaNhap", giaNhap);
                            cmdIDich.Parameters.AddWithValue("@SoLuong", ct.SoLuongChuyen);
                            cmdIDich.ExecuteNonQuery();
                        }
                    }

                    string sqlPN = @"
                        UPDATE PHIEUCHUYENKHO
                        SET TrangThai = N'Đã duyệt', MaNV_Duyet = @MaNVDuyet, NgayDuyet = GETDATE()
                        WHERE MaPhieuChuyen = @MaPhieuChuyen";

                    SqlCommand cmdUPN = new SqlCommand(sqlPN, conn, tran);
                    cmdUPN.Parameters.AddWithValue("@MaNVDuyet", maNVDuyet);
                    cmdUPN.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);
                    cmdUPN.ExecuteNonQuery();

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

        public bool HuyPhieuChuyen(int maPhieuChuyen, string maNVHuy)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    string sql = @"
                        UPDATE PHIEUCHUYENKHO 
                        SET TrangThai = N'Đã hủy'
                        WHERE MaPhieuChuyen = @MaPhieuChuyen AND TrangThai = N'Chờ duyệt'";

                    SqlCommand cmd = new SqlCommand(sql, conn, tran);
                    cmd.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);
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

        public bool XoaChiTietPhieuChuyen(int maCTPC)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    string sqlGet = "SELECT MaPhieuChuyen FROM CT_PHIEUCHUYENKHO WHERE MaCTPC = @MaCTPC";
                    SqlCommand cmdGet = new SqlCommand(sqlGet, conn, tran);
                    cmdGet.Parameters.AddWithValue("@MaCTPC", maCTPC);
                    var maPhieuChuyen = cmdGet.ExecuteScalar();

                    if (maPhieuChuyen == null)
                    {
                        tran.Rollback();
                        return false;
                    }

                    string sqlCheck = "SELECT TrangThai FROM PHIEUCHUYENKHO WHERE MaPhieuChuyen = @MaPhieuChuyen";
                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn, tran);
                    cmdCheck.Parameters.AddWithValue("@MaPhieuChuyen", maPhieuChuyen);
                    var trangThai = cmdCheck.ExecuteScalar()?.ToString();

                    if (trangThai != "Chờ duyệt")
                    {
                        tran.Rollback();
                        return false;
                    }

                    string sqlDelete = "DELETE FROM CT_PHIEUCHUYENKHO WHERE MaCTPC = @MaCTPC";
                    SqlCommand cmdDel = new SqlCommand(sqlDelete, conn, tran);
                    cmdDel.Parameters.AddWithValue("@MaCTPC", maCTPC);
                    cmdDel.ExecuteNonQuery();

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
        public int? MaKho { get; set; }
        public DateTime? NgayLap { get; set; }
        public decimal TongTienNhap { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string MaNV_Duyet { get; set; }
        public DateTime? NgayDuyet { get; set; }

        public string TenNguoiLap { get; set; }
        public string TenNguoiDuyet { get; set; }
        public string TenNSX { get; set; }
        public string TenKho { get; set; }

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
