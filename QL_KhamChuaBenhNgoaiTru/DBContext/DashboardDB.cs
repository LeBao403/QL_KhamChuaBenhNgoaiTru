using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class DashboardDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        #region 1. KHU VỰC: TỔNG QUAN HỆ THỐNG
        // -------------------------------------------------------------------------
        public DashboardThongKe GetThongKeTongQuan()
        {
            var tk = new DashboardThongKe();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sqlCount = @"
                    SELECT 
                        (SELECT COUNT(*) FROM BENHNHAN) AS TongBenhNhan,
                        (SELECT COUNT(*) FROM NHANVIEN WHERE TrangThai = 1) AS TongNhanVien,
                        (SELECT COUNT(*) FROM THUOC WHERE TrangThai = 1) AS TongThuoc,
                        (SELECT COUNT(*) FROM KHOA WHERE TrangThai = 1) AS TongKhoa,
                        (SELECT COUNT(*) FROM PHONG WHERE TrangThai = 1) AS TongPhong";
                using (SqlCommand cmd = new SqlCommand(sqlCount, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        tk.TongBenhNhan = dr["TongBenhNhan"] != DBNull.Value ? Convert.ToInt32(dr["TongBenhNhan"]) : 0;
                        tk.TongNhanVien = dr["TongNhanVien"] != DBNull.Value ? Convert.ToInt32(dr["TongNhanVien"]) : 0;
                        tk.TongThuoc = dr["TongThuoc"] != DBNull.Value ? Convert.ToInt32(dr["TongThuoc"]) : 0;
                        tk.TongKhoa = dr["TongKhoa"] != DBNull.Value ? Convert.ToInt32(dr["TongKhoa"]) : 0;
                        tk.TongPhong = dr["TongPhong"] != DBNull.Value ? Convert.ToInt32(dr["TongPhong"]) : 0;
                    }
                }
            }
            return tk;
        }

        public DashboardThongKe GetThongKeKhamBenh()
        {
            var tk = new DashboardThongKe();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM PHIEUKHAMBENH WHERE TrangThai = N'Đang khám') AS DangKham,
                        (SELECT COUNT(*) FROM PHIEUKHAMBENH WHERE TrangThai = N'Hoàn thành') AS HoanThanh,
                        (SELECT COUNT(*) FROM PHIEUKHAMBENH WHERE NgayLap = CAST(GETDATE() AS DATE)) AS HomNay,
                        (SELECT COUNT(*) FROM PHIEUKHAMBENH WHERE NgayLap >= DATEADD(day, -7, CAST(GETDATE() AS DATE))) AS TuanNay";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        tk.DangKham = dr["DangKham"] != DBNull.Value ? Convert.ToInt32(dr["DangKham"]) : 0;
                        tk.HoanThanh = dr["HoanThanh"] != DBNull.Value ? Convert.ToInt32(dr["HoanThanh"]) : 0;
                        tk.KhamHomNay = dr["HomNay"] != DBNull.Value ? Convert.ToInt32(dr["HomNay"]) : 0;
                        tk.KhamTuanNay = dr["TuanNay"] != DBNull.Value ? Convert.ToInt32(dr["TuanNay"]) : 0;
                    }
                }
            }
            return tk;
        }

        public DashboardThongKe GetDoanhThu(int? thang = null, int? nam = null)
        {
            var tk = new DashboardThongKe();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql; SqlCommand cmd;
                if (thang.HasValue && nam.HasValue)
                {
                    sql = @"SELECT ISNULL(SUM(TongTienGoc), 0) AS TongTienGoc, ISNULL(SUM(TienBHYTChiTra), 0) AS TienBHYT, ISNULL(SUM(TienBenhNhanTra), 0) AS TienBenhNhan
                            FROM CT_HOADON_DV WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MONTH(NgayThanhToan) = @Thang AND YEAR(NgayThanhToan) = @Nam)";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Thang", thang.Value); cmd.Parameters.AddWithValue("@Nam", nam.Value);
                }
                else
                {
                    sql = @"SELECT ISNULL(SUM(TongTienGoc), 0) AS TongTienGoc, ISNULL(SUM(TienBHYTChiTra), 0) AS TienBHYT, ISNULL(SUM(TienBenhNhanTra), 0) AS TienBenhNhan
                            FROM CT_HOADON_DV WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MONTH(NgayThanhToan) = MONTH(GETDATE()) AND YEAR(NgayThanhToan) = YEAR(GETDATE()))";
                    cmd = new SqlCommand(sql, conn);
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        tk.TongTienGoc = dr["TongTienGoc"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienGoc"]) : 0;
                        tk.TienBHYT = dr["TienBHYT"] != DBNull.Value ? Convert.ToDecimal(dr["TienBHYT"]) : 0;
                        tk.TienBenhNhanTra = dr["TienBenhNhan"] != DBNull.Value ? Convert.ToDecimal(dr["TienBenhNhan"]) : 0;
                    }
                }
            }
            return tk;
        }

        public List<DoanhThuNgay> GetDoanhThu7Ngay()
        {
            var list = new List<DoanhThuNgay>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"SELECT CAST(H.NgayThanhToan AS DATE) AS Ngay, ISNULL(SUM(CT.TongTienGoc), 0) AS DoanhThu
                               FROM HOADON H LEFT JOIN CT_HOADON_DV CT ON H.MaHD = CT.MaHD
                               WHERE H.NgayThanhToan >= DATEADD(day, -6, CAST(GETDATE() AS DATE)) AND H.NgayThanhToan <= CAST(GETDATE() AS DATE)
                               GROUP BY CAST(H.NgayThanhToan AS DATE) ORDER BY Ngay";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) list.Add(new DoanhThuNgay { Ngay = dr["Ngay"] != DBNull.Value ? Convert.ToDateTime(dr["Ngay"]) : DateTime.Now, DoanhThu = dr["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(dr["DoanhThu"]) : 0 });
            }
            return list;
        }

        public List<DoanhThuThang> GetDoanhThu12Thang(int nam = 0)
        {
            if (nam == 0) nam = DateTime.Now.Year;
            var list = new List<DoanhThuThang>();
            var thangNames = new[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" };

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"SELECT MONTH(H.NgayThanhToan) AS Thang, ISNULL(SUM(CT.TongTienGoc), 0) AS DoanhThu
                               FROM HOADON H LEFT JOIN CT_HOADON_DV CT ON H.MaHD = CT.MaHD
                               WHERE YEAR(H.NgayThanhToan) = @Nam GROUP BY MONTH(H.NgayThanhToan) ORDER BY Thang";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        while (dr.Read()) list.Add(new DoanhThuThang { Thang = Convert.ToInt32(dr["Thang"]), TenThang = thangNames[Convert.ToInt32(dr["Thang"]) - 1], DoanhThu = dr["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(dr["DoanhThu"]) : 0 });
                }
            }
            return list;
        }

        public List<PhieuNhapItem> GetPhieuNhapGanDay(int top = 5)
        {
            var list = new List<PhieuNhapItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = $@"SELECT TOP {top} PN.MaPhieuNhap, PN.NgayLap, PN.TongTienNhap, PN.TrangThai, NV.HoTen AS NguoiLap, NSX.TenNSX
                                FROM PHIEUNHAP PN INNER JOIN NHANVIEN NV ON PN.MaNV_LapPhieu = NV.MaNV INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                                ORDER BY PN.NgayLap DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) list.Add(new PhieuNhapItem { MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]), NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null, TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0, TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "", NguoiLap = dr["NguoiLap"] != DBNull.Value ? dr["NguoiLap"].ToString() : "", NhaCungCap = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : "" });
            }
            return list;
        }

        public List<BacSiThongKe> GetBacSiKhamNhieuNhat(int top = 5)
        {
            var list = new List<BacSiThongKe>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = $@"SELECT TOP {top} NV.MaNV, NV.HoTen, CV.TenChucVu, COUNT(PKB.MaPhieuKhamBenh) AS SoLuotKham
                                FROM NHANVIEN NV INNER JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu LEFT JOIN PHIEUKHAMBENH PKB ON NV.MaNV = PKB.MaBacSiKham
                                WHERE CV.TenChucVu LIKE N'%Bác sĩ%' AND PKB.TrangThai = N'Hoàn thành' AND PKB.NgayLap >= DATEADD(day, -30, CAST(GETDATE() AS DATE))
                                GROUP BY NV.MaNV, NV.HoTen, CV.TenChucVu ORDER BY SoLuotKham DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                    while (dr.Read()) list.Add(new BacSiThongKe { MaNV = dr["MaNV"].ToString(), HoTen = dr["HoTen"].ToString(), TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "", SoLuotKham = dr["SoLuotKham"] != DBNull.Value ? Convert.ToInt32(dr["SoLuotKham"]) : 0 });
            }
            return list;
        }
        #endregion

        #region 2. KHU VỰC: PHÂN TÍCH DOANH THU
        // -------------------------------------------------------------------------
        public DoanhThuChiTietViewModel GetPhanTichDoanhThu(DateTime tuNgay, DateTime denNgay, string groupBy = "day")
        {
            var result = new DoanhThuChiTietViewModel();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                DateTime exactTuNgay = tuNgay.Date;
                DateTime exactDenNgay = denNgay.Date.AddDays(1).AddSeconds(-1);

                GetKpiDoanhThu(conn, exactTuNgay, exactDenNgay, result);
                GetBieuDoXuHuongDoanhThu(conn, exactTuNgay, exactDenNgay, groupBy, result);
                GetPhuongThucThanhToan(conn, exactTuNgay, exactDenNgay, result);
                GetTopDichVuDoanhThu(conn, exactTuNgay, exactDenNgay, result);
                GetCoCauNguonThu(conn, exactTuNgay, exactDenNgay, result);
            }
            return result;
        }

        private void GetKpiDoanhThu(SqlConnection conn, DateTime tuNgay, DateTime denNgay, DoanhThuChiTietViewModel result)
        {
            // Tách làm 2 bước tính toán bằng biến SQL để tránh tuyệt đối lỗi lồng hàm SUM
            string sqlKpi = @"
                DECLARE @TienVonThuoc DECIMAL(18,2) = 0;

                -- BƯỚC 1: Tính tổng tiền vốn thuốc xuất ra (Dùng OUTER APPLY thay vì Subquery lồng)
                SELECT @TienVonThuoc = ISNULL(SUM(CT.SoLuong * ISNULL(K.GiaNhap, 0)), 0)
                FROM CT_HOADON_THUOC CT 
                INNER JOIN CT_DON_THUOC CTD ON CT.MaCTDonThuoc = CTD.MaCTDonThuoc
                INNER JOIN HOADON HD ON CT.MaHD = HD.MaHD 
                OUTER APPLY (
                    SELECT TOP 1 GiaNhap 
                    FROM TONKHO 
                    WHERE MaThuoc = CTD.MaThuoc 
                    ORDER BY NgayCapNhat DESC
                ) K
                WHERE HD.TrangThaiThanhToan = N'Đã thanh toán' 
                  AND HD.NgayThanhToan >= @TuNgay AND HD.NgayThanhToan <= @DenNgay;

                -- BƯỚC 2: Tính tổng doanh thu và ghép biến Tiền vốn thuốc vào
                SELECT 
                    ISNULL(SUM(HD.TongTienGoc), 0) AS TongGoc, 
                    ISNULL(SUM(HD.TongTienBenhNhanTra), 0) AS ThucThu, 
                    ISNULL(SUM(HD.TongTienBHYTChiTra), 0) AS BHYT, 
                    COUNT(DISTINCT HD.MaHD) AS SoLuot,
                    @TienVonThuoc AS TienVonThuoc
                FROM HOADON HD 
                WHERE HD.TrangThaiThanhToan = N'Đã thanh toán' 
                  AND HD.NgayThanhToan >= @TuNgay AND HD.NgayThanhToan <= @DenNgay;";

            using (SqlCommand cmd = new SqlCommand(sqlKpi, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        result.TongGoc = Convert.ToDecimal(dr["TongGoc"]);
                        result.ThucThu = Convert.ToDecimal(dr["ThucThu"]);
                        result.BHYT = Convert.ToDecimal(dr["BHYT"]);
                        result.SoLuotThanhToan = Convert.ToInt32(dr["SoLuot"]);
                        result.TienVonThuoc = Convert.ToDecimal(dr["TienVonThuoc"]);
                    }
                }
            }
        }

        private void GetBieuDoXuHuongDoanhThu(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string groupBy, DoanhThuChiTietViewModel result)
        {
            string sqlTrend = "";
            if (groupBy == "year") sqlTrend = @"SELECT FORMAT(NgayThanhToan, 'yyyy') AS Ngay, ISNULL(SUM(TongTienBenhNhanTra), 0) AS SoTien, MAX(CAST(NgayThanhToan AS DATE)) AS SortDate FROM HOADON WHERE TrangThaiThanhToan = N'Đã thanh toán' AND NgayThanhToan >= @TuNgay AND NgayThanhToan <= @DenNgay GROUP BY FORMAT(NgayThanhToan, 'yyyy') ORDER BY SortDate";
            else if (groupBy == "month") sqlTrend = @"SELECT FORMAT(NgayThanhToan, 'MM/yyyy') AS Ngay, ISNULL(SUM(TongTienBenhNhanTra), 0) AS SoTien, MAX(CAST(NgayThanhToan AS DATE)) AS SortDate FROM HOADON WHERE TrangThaiThanhToan = N'Đã thanh toán' AND NgayThanhToan >= @TuNgay AND NgayThanhToan <= @DenNgay GROUP BY FORMAT(NgayThanhToan, 'MM/yyyy') ORDER BY SortDate";
            else if (groupBy == "week") sqlTrend = @"SELECT N'Tuần ' + CAST(DATEPART(iso_week, NgayThanhToan) AS VARCHAR) + '/' + CAST(YEAR(NgayThanhToan) AS VARCHAR) AS Ngay, ISNULL(SUM(TongTienBenhNhanTra), 0) AS SoTien, MAX(CAST(NgayThanhToan AS DATE)) AS SortDate FROM HOADON WHERE TrangThaiThanhToan = N'Đã thanh toán' AND NgayThanhToan >= @TuNgay AND NgayThanhToan <= @DenNgay GROUP BY DATEPART(iso_week, NgayThanhToan), YEAR(NgayThanhToan) ORDER BY SortDate";
            else sqlTrend = @"SELECT FORMAT(NgayThanhToan, 'dd/MM/yyyy') AS Ngay, ISNULL(SUM(TongTienBenhNhanTra), 0) AS SoTien, MAX(CAST(NgayThanhToan AS DATE)) AS SortDate FROM HOADON WHERE TrangThaiThanhToan = N'Đã thanh toán' AND NgayThanhToan >= @TuNgay AND NgayThanhToan <= @DenNgay GROUP BY FORMAT(NgayThanhToan, 'dd/MM/yyyy'), CAST(NgayThanhToan AS DATE) ORDER BY SortDate";

            using (SqlCommand cmd = new SqlCommand(sqlTrend, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.BieuDoXuHuong.Add(new BieuDoDoanhThu { Ngay = dr["Ngay"].ToString(), SoTien = Convert.ToDecimal(dr["SoTien"]) });
            }
        }

        private void GetPhuongThucThanhToan(SqlConnection conn, DateTime tuNgay, DateTime denNgay, DoanhThuChiTietViewModel result)
        {
            string sqlPTTT = @"SELECT ISNULL(HinhThucThanhToan, N'Khác') AS Ten, ISNULL(SUM(TongTienBenhNhanTra), 0) AS SoTien FROM HOADON WHERE TrangThaiThanhToan = N'Đã thanh toán' AND NgayThanhToan >= @TuNgay AND NgayThanhToan <= @DenNgay GROUP BY HinhThucThanhToan HAVING SUM(TongTienBenhNhanTra) > 0";
            using (SqlCommand cmd = new SqlCommand(sqlPTTT, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.BieuDoPhuongThuc.Add(new PhieuToThanhToan { Ten = dr["Ten"].ToString(), SoTien = Convert.ToDecimal(dr["SoTien"]) });
            }
        }

        private void GetTopDichVuDoanhThu(SqlConnection conn, DateTime tuNgay, DateTime denNgay, DoanhThuChiTietViewModel result)
        {
            string sqlTopDV = @"SELECT TOP 5 DV.TenDV, SUM(CT.TienBenhNhanTra) AS SoTien, COUNT(CT.MaCTHD) AS SoLuot FROM CT_HOADON_DV CT INNER JOIN DICHVU DV ON CT.MaDV = DV.MaDV INNER JOIN HOADON HD ON CT.MaHD = HD.MaHD WHERE HD.TrangThaiThanhToan = N'Đã thanh toán' AND HD.NgayThanhToan >= @TuNgay AND HD.NgayThanhToan <= @DenNgay GROUP BY DV.TenDV ORDER BY SoTien DESC";
            using (SqlCommand cmd = new SqlCommand(sqlTopDV, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.TopDichVu.Add(new TopDichVu { TenDV = dr["TenDV"].ToString(), SoTien = Convert.ToDecimal(dr["SoTien"]), SoLuot = Convert.ToInt32(dr["SoLuot"]) });
            }
        }

        private void GetCoCauNguonThu(SqlConnection conn, DateTime tuNgay, DateTime denNgay, DoanhThuChiTietViewModel result)
        {
            string sqlNguonThu = @"
                SELECT LDV.TenLoaiDV AS Ten, SUM(CT.TienBenhNhanTra) AS SoTien FROM CT_HOADON_DV CT INNER JOIN HOADON HD ON CT.MaHD = HD.MaHD INNER JOIN DICHVU DV ON CT.MaDV = DV.MaDV INNER JOIN LOAI_DICHVU LDV ON DV.MaLoaiDV = LDV.MaLoaiDV WHERE HD.TrangThaiThanhToan = N'Đã thanh toán' AND HD.NgayThanhToan >= @TuNgay AND HD.NgayThanhToan <= @DenNgay GROUP BY LDV.TenLoaiDV HAVING SUM(CT.TienBenhNhanTra) > 0
                UNION ALL
                SELECT N'Tiền Bán Thuốc' AS Ten, SUM(CT.TienBenhNhanTra) AS SoTien FROM CT_HOADON_THUOC CT INNER JOIN HOADON HD ON CT.MaHD = HD.MaHD WHERE HD.TrangThaiThanhToan = N'Đã thanh toán' AND HD.NgayThanhToan >= @TuNgay AND HD.NgayThanhToan <= @DenNgay HAVING SUM(CT.TienBenhNhanTra) > 0";

            using (SqlCommand cmd = new SqlCommand(sqlNguonThu, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.BieuDoNguonThu.Add(new PhieuToThanhToan { Ten = dr["Ten"].ToString(), SoTien = Convert.ToDecimal(dr["SoTien"]) });
            }
        }
        #endregion

        #region 3. KHU VỰC: THỐNG KÊ BỆNH NHÂN
        // -------------------------------------------------------------------------
        public BenhNhanThongKeViewModel GetThongKeBenhNhan(DateTime tuNgay, DateTime denNgay, int? maKhoa = null)
        {
            var result = new BenhNhanThongKeViewModel();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                DateTime exactTuNgay = tuNgay.Date;
                DateTime exactDenNgay = denNgay.Date.AddDays(1).AddSeconds(-1);
                string filterKhoa = maKhoa.HasValue && maKhoa.Value > 0 ? " AND P.MaKhoa = @MaKhoa " : "";

                GetKpiBenhNhan(conn, exactTuNgay, exactDenNgay, filterKhoa, maKhoa, result);
                GetLuuLuongTheoGio(conn, exactTuNgay, exactDenNgay, filterKhoa, maKhoa, result);
                GetPhieuGioiTinhVaTuyen(conn, exactTuNgay, exactDenNgay, filterKhoa, maKhoa, result);
                GetPhanKhucTuoi(conn, exactTuNgay, exactDenNgay, filterKhoa, maKhoa, result);
                GetTopBenhLy(conn, exactTuNgay, exactDenNgay, filterKhoa, maKhoa, result);
            }
            return result;
        }

        private void GetKpiBenhNhan(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKhoa, int? maKhoa, BenhNhanThongKeViewModel result)
        {
            string sqlKpi = $@"
                SELECT 
                    COUNT(PKB.MaPhieuKhamBenh) AS TongLuotKham, 
                    SUM(CAST(BN.BHYT AS INT)) AS SoBHYT, 
                    SUM(CASE WHEN PDK.HinhThucDangKy = N'Online' THEN 1 ELSE 0 END) AS SoOnline, 
                    SUM(CASE WHEN PDK.HinhThucDangKy = N'Offline' THEN 1 ELSE 0 END) AS SoOffline,
                    SUM(CASE WHEN ISNULL(Prev.LanKhamCu, 0) = 0 THEN 1 ELSE 0 END) AS BenhNhanMoi,
                    SUM(CASE WHEN ISNULL(Prev.LanKhamCu, 0) > 0 THEN 1 ELSE 0 END) AS TaiKham
                FROM PHIEUKHAMBENH PKB 
                INNER JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN 
                LEFT JOIN PHIEUDANGKY PDK ON PKB.MaPhieuDK = PDK.MaPhieuDK 
                LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong 
                OUTER APPLY (
                    SELECT COUNT(*) AS LanKhamCu 
                    FROM PHIEUKHAMBENH P2 
                    WHERE P2.MaBN = PKB.MaBN AND P2.NgayLap < PKB.NgayLap
                ) Prev
                WHERE PKB.NgayLap >= @TuNgay AND PKB.NgayLap <= @DenNgay {filterKhoa}";

            using (SqlCommand cmd = new SqlCommand(sqlKpi, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKhoa.HasValue && maKhoa.Value > 0) cmd.Parameters.AddWithValue("@MaKhoa", maKhoa.Value);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        result.TongLuotKham = dr["TongLuotKham"] != DBNull.Value ? Convert.ToInt32(dr["TongLuotKham"]) : 0;
                        int soBHYT = dr["SoBHYT"] != DBNull.Value ? Convert.ToInt32(dr["SoBHYT"]) : 0;
                        result.TyLeBHYT = result.TongLuotKham > 0 ? Math.Round((soBHYT * 100.0) / result.TongLuotKham, 1) : 0;
                        result.SoDangKyOnline = dr["SoOnline"] != DBNull.Value ? Convert.ToInt32(dr["SoOnline"]) : 0;
                        result.SoDangKyOffline = dr["SoOffline"] != DBNull.Value ? Convert.ToInt32(dr["SoOffline"]) : 0;
                        result.BenhNhanMoi = dr["BenhNhanMoi"] != DBNull.Value ? Convert.ToInt32(dr["BenhNhanMoi"]) : 0;
                        result.TaiKham = dr["TaiKham"] != DBNull.Value ? Convert.ToInt32(dr["TaiKham"]) : 0;
                    }
                }
            }
        }

        private void GetLuuLuongTheoGio(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKhoa, int? maKhoa, BenhNhanThongKeViewModel result)
        {
            string sqlGio = $@"SELECT ISNULL(KG.TenKhungGio, N'Khác/Cấp cứu') AS KhungGio, COUNT(PKB.MaPhieuKhamBenh) AS SoLuong
                               FROM PHIEUKHAMBENH PKB LEFT JOIN PHIEUDANGKY PDK ON PKB.MaPhieuDK = PDK.MaPhieuDK LEFT JOIN DANHMUC_KHUNGGIO KG ON PDK.MaKhungGio = KG.MaKhungGio LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong
                               WHERE PKB.NgayLap >= @TuNgay AND PKB.NgayLap <= @DenNgay {filterKhoa} GROUP BY KG.MaKhungGio, KG.TenKhungGio ORDER BY KG.MaKhungGio";
            using (SqlCommand cmd = new SqlCommand(sqlGio, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKhoa.HasValue && maKhoa.Value > 0) cmd.Parameters.AddWithValue("@MaKhoa", maKhoa.Value);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.LuuLuongTheoGio.Add(new BieuDoCot { Nhan = dr["KhungGio"].ToString(), GiaTri = Convert.ToInt32(dr["SoLuong"]) });
            }
        }

        private void GetPhieuGioiTinhVaTuyen(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKhoa, int? maKhoa, BenhNhanThongKeViewModel result)
        {
            string sqlPhanBo = $@"SELECT ISNULL(BN.GioiTinh, N'Chưa rõ') AS GioiTinh, ISNULL(BN.TuyenKham, N'Trái tuyến') AS TuyenKham, COUNT(PKB.MaPhieuKhamBenh) AS SoLuong
                                  FROM PHIEUKHAMBENH PKB INNER JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong WHERE PKB.NgayLap >= @TuNgay AND PKB.NgayLap <= @DenNgay {filterKhoa} GROUP BY BN.GioiTinh, BN.TuyenKham";
            using (SqlCommand cmd = new SqlCommand(sqlPhanBo, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKhoa.HasValue && maKhoa.Value > 0) cmd.Parameters.AddWithValue("@MaKhoa", maKhoa.Value);

                var listTemp = new List<dynamic>();
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) listTemp.Add(new { GT = dr["GioiTinh"].ToString(), TK = dr["TuyenKham"].ToString(), SL = Convert.ToInt32(dr["SoLuong"]) });

                result.PhieuGioiTinh = listTemp.GroupBy(x => x.GT).Select(g => new BieuDoTron { Nhan = g.Key, GiaTri = g.Sum(x => x.SL) }).ToList();
                result.PhieuTuyenKham = listTemp.GroupBy(x => x.TK).Select(g => new BieuDoTron { Nhan = g.Key, GiaTri = g.Sum(x => x.SL) }).ToList();
            }
        }

        private void GetPhanKhucTuoi(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKhoa, int? maKhoa, BenhNhanThongKeViewModel result)
        {
            string sqlTuoi = $@"SELECT CASE WHEN DATEDIFF(YEAR, BN.NgaySinh, GETDATE()) <= 12 THEN N'Trẻ em (≤12)' WHEN DATEDIFF(YEAR, BN.NgaySinh, GETDATE()) BETWEEN 13 AND 25 THEN N'Thanh thiếu niên (13-25)' WHEN DATEDIFF(YEAR, BN.NgaySinh, GETDATE()) BETWEEN 26 AND 55 THEN N'Trưởng thành (26-55)' ELSE N'Cao tuổi (>55)' END AS NhomTuoi, COUNT(PKB.MaPhieuKhamBenh) AS SoLuong
                                FROM PHIEUKHAMBENH PKB INNER JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong WHERE PKB.NgayLap >= @TuNgay AND PKB.NgayLap <= @DenNgay {filterKhoa}
                                GROUP BY CASE WHEN DATEDIFF(YEAR, BN.NgaySinh, GETDATE()) <= 12 THEN N'Trẻ em (≤12)' WHEN DATEDIFF(YEAR, BN.NgaySinh, GETDATE()) BETWEEN 13 AND 25 THEN N'Thanh thiếu niên (13-25)' WHEN DATEDIFF(YEAR, BN.NgaySinh, GETDATE()) BETWEEN 26 AND 55 THEN N'Trưởng thành (26-55)' ELSE N'Cao tuổi (>55)' END";
            using (SqlCommand cmd = new SqlCommand(sqlTuoi, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKhoa.HasValue && maKhoa.Value > 0) cmd.Parameters.AddWithValue("@MaKhoa", maKhoa.Value);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.PhanKhucTuoi.Add(new BieuDoCot { Nhan = dr["NhomTuoi"].ToString(), GiaTri = Convert.ToInt32(dr["SoLuong"]) });
            }
        }

        private void GetTopBenhLy(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKhoa, int? maKhoa, BenhNhanThongKeViewModel result)
        {
            string sqlBenh = $@"SELECT TOP 7 DMB.TenBenh, COUNT(CT.MaCTChanDoan) AS SoCa FROM CHITIET_CHANDOAN CT INNER JOIN DANHMUC_BENH DMB ON CT.MaBenh = DMB.MaBenh INNER JOIN PHIEUKHAMBENH PKB ON CT.MaPhieuKhamBenh = PKB.MaPhieuKhamBenh LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong WHERE PKB.NgayLap >= @TuNgay AND PKB.NgayLap <= @DenNgay {filterKhoa} GROUP BY DMB.TenBenh ORDER BY SoCa DESC";
            using (SqlCommand cmd = new SqlCommand(sqlBenh, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKhoa.HasValue && maKhoa.Value > 0) cmd.Parameters.AddWithValue("@MaKhoa", maKhoa.Value);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.TopBenhLy.Add(new BieuDoCot { Nhan = dr["TenBenh"].ToString(), GiaTri = Convert.ToInt32(dr["SoCa"]) });
            }
        }
        #endregion

        #region 4. KHU VỰC: QUẢN LÝ KHO DƯỢC
        // -------------------------------------------------------------------------
        public KhoDuocThongKeViewModel GetThongKeKhoDuoc(DateTime tuNgay, DateTime denNgay, int maKho = 0)
        {
            var result = new KhoDuocThongKeViewModel();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                DateTime exactTuNgay = tuNgay.Date;
                DateTime exactDenNgay = denNgay.Date.AddDays(1).AddSeconds(-1);
                string filterKho = maKho > 0 ? " AND MaKho = @MaKho " : "";

                GetKpiKhoDuoc(conn, exactTuNgay, exactDenNgay, filterKho, maKho, result);
                GetBieuDoNhapKho(conn, exactTuNgay, exactDenNgay, filterKho, maKho, result);
                GetTyTrongDanhMuc(conn, filterKho, maKho, result);
                GetTopThuocXuat(conn, exactTuNgay, exactDenNgay, filterKho, maKho, result);
                GetDanhSachCanhBao(conn, filterKho, maKho, result);
            }
            return result;
        }

        private void GetKpiKhoDuoc(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKho, int maKho, KhoDuocThongKeViewModel result)
        {
            string sqlTon = $"SELECT ISNULL(SUM(SoLuongTon * GiaNhap), 0) FROM TONKHO WHERE SoLuongTon > 0 {filterKho}";
            string sqlNhap = $"SELECT ISNULL(SUM(TongTienNhap), 0) FROM PHIEUNHAP WHERE TrangThai = N'Đã duyệt' AND NgayLap >= @TuNgay AND NgayLap <= @DenNgay {filterKho}";
            string sqlHan = $"SELECT COUNT(MaTonKho) FROM TONKHO WHERE HanSuDung <= DATEADD(day, 90, GETDATE()) AND SoLuongTon > 0 {filterKho}";
            string sqlHang = $"SELECT COUNT(*) FROM (SELECT MaThuoc, SUM(SoLuongTon) as SL FROM TONKHO WHERE 1=1 {filterKho} GROUP BY MaThuoc HAVING SUM(SoLuongTon) < 50) AS T";

            using (SqlCommand cmd = new SqlCommand(sqlTon, conn)) { if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho); result.TongGiaTriTon = Convert.ToDecimal(cmd.ExecuteScalar()); }
            using (SqlCommand cmd = new SqlCommand(sqlNhap, conn)) { cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay); if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho); result.GiaTriNhapThang = Convert.ToDecimal(cmd.ExecuteScalar()); }
            using (SqlCommand cmd = new SqlCommand(sqlHan, conn)) { if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho); result.ThuocSapHetHan = Convert.ToInt32(cmd.ExecuteScalar()); }
            using (SqlCommand cmd = new SqlCommand(sqlHang, conn)) { if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho); result.ThuocSapHetHang = Convert.ToInt32(cmd.ExecuteScalar()); }
        }

        private void GetBieuDoNhapKho(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKho, int maKho, KhoDuocThongKeViewModel result)
        {
            string sql = $@"SELECT FORMAT(NgayLap, 'MM/yyyy') AS Thang, ISNULL(SUM(TongTienNhap), 0) AS SoTien, MAX(CAST(NgayLap AS DATE)) AS SortDate FROM PHIEUNHAP WHERE TrangThai = N'Đã duyệt' AND NgayLap >= @TuNgay AND NgayLap <= @DenNgay {filterKho} GROUP BY FORMAT(NgayLap, 'MM/yyyy') ORDER BY SortDate";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.BieuDoNhapKho.Add(new BieuDoCot { Nhan = dr["Thang"].ToString(), GiaTri = Convert.ToDecimal(dr["SoTien"]) });
            }
        }

        private void GetTyTrongDanhMuc(SqlConnection conn, string filterKho, int maKho, KhoDuocThongKeViewModel result)
        {
            string sql = $@"SELECT TOP 5 DM.TenDanhMuc, SUM(TK.SoLuongTon * ISNULL(TK.GiaNhap, 0)) AS GiaTri FROM TONKHO TK INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc INNER JOIN DANHMUC_THUOC DM ON T.MaLoaiThuoc = DM.MaDanhMuc WHERE TK.SoLuongTon > 0 {filterKho.Replace("MaKho", "TK.MaKho")} GROUP BY DM.TenDanhMuc ORDER BY GiaTri DESC";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.TyTrongDanhMuc.Add(new BieuDoTron { Nhan = dr["TenDanhMuc"].ToString(), GiaTri = Convert.ToDecimal(dr["GiaTri"]) });
            }
        }

        private void GetTopThuocXuat(SqlConnection conn, DateTime tuNgay, DateTime denNgay, string filterKho, int maKho, KhoDuocThongKeViewModel result)
        {
            string sql = $@"SELECT TOP 7 T.TenThuoc, SUM(CT.SoLuongPhat) AS SoLuong FROM CT_PHIEU_PHAT CT INNER JOIN THUOC T ON CT.MaThuoc = T.MaThuoc INNER JOIN PHIEU_PHAT_THUOC PT ON CT.MaPhieuPhat = PT.MaPhieuPhat WHERE PT.NgayPhat >= @TuNgay AND PT.NgayPhat <= @DenNgay AND PT.TrangThai = N'Hoàn thành' {filterKho.Replace("MaKho", "PT.MaKho")} GROUP BY T.TenThuoc ORDER BY SoLuong DESC";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay); cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.TopThuocXuat.Add(new BieuDoCot { Nhan = dr["TenThuoc"].ToString(), GiaTri = Convert.ToDecimal(dr["SoLuong"]) });
            }
        }

        private void GetDanhSachCanhBao(SqlConnection conn, string filterKho, int maKho, KhoDuocThongKeViewModel result)
        {
            string sql = $@"SELECT TOP 5 T.TenThuoc, TK.MaLo, FORMAT(TK.HanSuDung, 'dd/MM/yyyy') AS HSD, TK.SoLuongTon FROM TONKHO TK INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc WHERE TK.HanSuDung <= DATEADD(day, 90, GETDATE()) AND TK.SoLuongTon > 0 {filterKho.Replace("MaKho", "TK.MaKho")} ORDER BY TK.HanSuDung ASC";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (maKho > 0) cmd.Parameters.AddWithValue("@MaKho", maKho);
                using (SqlDataReader dr = cmd.ExecuteReader()) while (dr.Read()) result.DanhSachCanhBao.Add(new ChiTietThuocHetHan { TenThuoc = dr["TenThuoc"].ToString(), MaLo = dr["MaLo"].ToString(), HanSuDung = dr["HSD"].ToString(), SoLuongTon = Convert.ToInt32(dr["SoLuongTon"]) });
            }
        }

        public List<ThuocSapHetHan> GetThuocSapHetHan(int ngay = 30)
        {
            var list = new List<ThuocSapHetHan>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"SELECT TOP 10 T.MaThuoc, T.TenThuoc, TK.MaLo, TK.HanSuDung, TK.SoLuongTon, K.TenKho FROM TONKHO TK INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc INNER JOIN KHO K ON TK.MaKho = K.MaKho WHERE TK.HanSuDung <= DATEADD(day, @Ngay, CAST(GETDATE() AS DATE)) AND TK.SoLuongTon > 0 ORDER BY TK.HanSuDung ASC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Ngay", ngay);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        while (dr.Read()) list.Add(new ThuocSapHetHan { MaThuoc = dr["MaThuoc"].ToString(), TenThuoc = dr["TenThuoc"].ToString(), MaLo = dr["MaLo"].ToString(), HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now, SoLuongTon = dr["SoLuongTon"] != DBNull.Value ? Convert.ToInt32(dr["SoLuongTon"]) : 0, TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : "" });
                }
            }
            return list;
        }

        public List<ThuocSapHetHang> GetThuocSapHetHang(int minSoLuong = 10)
        {
            var list = new List<ThuocSapHetHang>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"SELECT TOP 10 T.MaThuoc, T.TenThuoc, T.DonViCoBan, SUM(TK.SoLuongTon) AS TongTon, K.TenKho FROM TONKHO TK INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc INNER JOIN KHO K ON TK.MaKho = K.MaKho GROUP BY T.MaThuoc, T.TenThuoc, T.DonViCoBan, K.TenKho HAVING SUM(TK.SoLuongTon) <= @MinSoLuong ORDER BY TongTon ASC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MinSoLuong", minSoLuong);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        while (dr.Read()) list.Add(new ThuocSapHetHang { MaThuoc = dr["MaThuoc"].ToString(), TenThuoc = dr["TenThuoc"].ToString(), DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : "", TongTon = dr["TongTon"] != DBNull.Value ? Convert.ToInt32(dr["TongTon"]) : 0, TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : "" });
                }
            }
            return list;
        }
        #endregion
    }
}