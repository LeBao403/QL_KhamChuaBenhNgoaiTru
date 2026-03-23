using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class DashboardDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // ==================== THỐNG KÊ TỔNG QUAN ====================
        public DashboardThongKe GetThongKeTongQuan()
        {
            var tk = new DashboardThongKe();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. Đếm tổng số
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

        // ==================== THỐNG KÊ KHÁM BỆNH ====================
        public DashboardThongKe GetThongKeKhamBenh()
        {
            var tk = new DashboardThongKe();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // Đếm phiếu khám bệnh theo trạng thái
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

        // ==================== DOANH THU ====================
        public DashboardThongKe GetDoanhThu(int? thang = null, int? nam = null)
        {
            var tk = new DashboardThongKe();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string sql;
                SqlCommand cmd;

                if (thang.HasValue && nam.HasValue)
                {
                    // Doanh thu theo tháng cụ thể
                    sql = @"
                        SELECT 
                            ISNULL(SUM(TongTienGoc), 0) AS TongTienGoc,
                            ISNULL(SUM(TienBHYTChiTra), 0) AS TienBHYT,
                            ISNULL(SUM(TienBenhNhanTra), 0) AS TienBenhNhan
                        FROM CT_HOADON_DV
                        WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MONTH(NgayThanhToan) = @Thang AND YEAR(NgayThanhToan) = @Nam)";

                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Thang", thang.Value);
                    cmd.Parameters.AddWithValue("@Nam", nam.Value);
                }
                else
                {
                    // Doanh thu tháng hiện tại
                    sql = @"
                        SELECT 
                            ISNULL(SUM(TongTienGoc), 0) AS TongTienGoc,
                            ISNULL(SUM(TienBHYTChiTra), 0) AS TienBHYT,
                            ISNULL(SUM(TienBenhNhanTra), 0) AS TienBenhNhan
                        FROM CT_HOADON_DV
                        WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MONTH(NgayThanhToan) = MONTH(GETDATE()) AND YEAR(NgayThanhToan) = YEAR(GETDATE()))";

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

        // ==================== BIỂU ĐỒ TUẦN ====================
        public List<DoanhThuNgay> GetDoanhThu7Ngay()
        {
            var list = new List<DoanhThuNgay>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        CAST(H.NgayThanhToan AS DATE) AS Ngay,
                        ISNULL(SUM(CT.TongTienGoc), 0) AS DoanhThu
                    FROM HOADON H
                    LEFT JOIN CT_HOADON_DV CT ON H.MaHD = CT.MaHD
                    WHERE H.NgayThanhToan >= DATEADD(day, -6, CAST(GETDATE() AS DATE))
                      AND H.NgayThanhToan <= CAST(GETDATE() AS DATE)
                    GROUP BY CAST(H.NgayThanhToan AS DATE)
                    ORDER BY Ngay";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new DoanhThuNgay
                        {
                            Ngay = dr["Ngay"] != DBNull.Value ? Convert.ToDateTime(dr["Ngay"]) : DateTime.Now,
                            DoanhThu = dr["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(dr["DoanhThu"]) : 0
                        });
                    }
                }
            }

            return list;
        }

        // ==================== BIỂU ĐỒ THÁNG (12 THÁNG) ====================
        public List<DoanhThuThang> GetDoanhThu12Thang(int nam = 0)
        {
            if (nam == 0) nam = DateTime.Now.Year;

            var list = new List<DoanhThuThang>();
            var thangNames = new[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" };

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        MONTH(H.NgayThanhToan) AS Thang,
                        ISNULL(SUM(CT.TongTienGoc), 0) AS DoanhThu
                    FROM HOADON H
                    LEFT JOIN CT_HOADON_DV CT ON H.MaHD = CT.MaHD
                    WHERE YEAR(H.NgayThanhToan) = @Nam
                    GROUP BY MONTH(H.NgayThanhToan)
                    ORDER BY Thang";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new DoanhThuThang
                            {
                                Thang = Convert.ToInt32(dr["Thang"]),
                                TenThang = thangNames[Convert.ToInt32(dr["Thang"]) - 1],
                                DoanhThu = dr["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(dr["DoanhThu"]) : 0
                            });
                        }
                    }
                }
            }

            return list;
        }

        // ==================== THUỐC SẮP HẾT HẠN ====================
        public List<ThuocSapHetHan> GetThuocSapHetHan(int ngay = 30)
        {
            var list = new List<ThuocSapHetHan>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
            SELECT TOP 10
                T.MaThuoc,
                T.TenThuoc,
                TK.MaLo,
                TK.HanSuDung,
                TK.SoLuongTon,
                K.TenKho
            FROM TONKHO TK
            INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
            INNER JOIN KHO K ON TK.MaKho = K.MaKho
            WHERE TK.HanSuDung <= DATEADD(day, @Ngay, CAST(GETDATE() AS DATE))
              AND TK.SoLuongTon > 0
            ORDER BY TK.HanSuDung ASC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Ngay", ngay);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ThuocSapHetHan
                            {
                                MaThuoc = dr["MaThuoc"].ToString(),
                                TenThuoc = dr["TenThuoc"].ToString(),
                                MaLo = dr["MaLo"].ToString(),
                                HanSuDung = dr["HanSuDung"] != DBNull.Value ? Convert.ToDateTime(dr["HanSuDung"]) : DateTime.Now,
                                SoLuongTon = dr["SoLuongTon"] != DBNull.Value ? Convert.ToInt32(dr["SoLuongTon"]) : 0,
                                TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : "" // Map vào TenKho
                            });
                        }
                    }
                }
            }
            return list;
        }

        // ==================== THUỐC SẮP HẾT HÀNG ====================
        public List<ThuocSapHetHang> GetThuocSapHetHang(int minSoLuong = 10)
        {
            var list = new List<ThuocSapHetHang>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
            SELECT TOP 10
                T.MaThuoc,
                T.TenThuoc,
                T.DonViCoBan,
                SUM(TK.SoLuongTon) AS TongTon,
                K.TenKho
            FROM TONKHO TK
            INNER JOIN THUOC T ON TK.MaThuoc = T.MaThuoc
            INNER JOIN KHO K ON TK.MaKho = K.MaKho
            GROUP BY T.MaThuoc, T.TenThuoc, T.DonViCoBan, K.TenKho
            HAVING SUM(TK.SoLuongTon) <= @MinSoLuong
            ORDER BY TongTon ASC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MinSoLuong", minSoLuong);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ThuocSapHetHang
                            {
                                MaThuoc = dr["MaThuoc"].ToString(),
                                TenThuoc = dr["TenThuoc"].ToString(),
                                DonViCoBan = dr["DonViCoBan"] != DBNull.Value ? dr["DonViCoBan"].ToString() : "",
                                TongTon = dr["TongTon"] != DBNull.Value ? Convert.ToInt32(dr["TongTon"]) : 0,
                                TenKho = dr["TenKho"] != DBNull.Value ? dr["TenKho"].ToString() : "" // Map vào TenKho
                            });
                        }
                    }
                }
            }
            return list;
        }

        // ==================== PHIẾU NHẬP GẦN ĐÂY ====================
        public List<PhieuNhapItem> GetPhieuNhapGanDay(int top = 5)
        {
            var list = new List<PhieuNhapItem>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string sql = @"
                    SELECT TOP " + top + @" 
                        PN.MaPhieuNhap,
                        PN.NgayLap,
                        PN.TongTienNhap,
                        PN.TrangThai,
                        NV.HoTen AS NguoiLap,
                        NSX.TenNSX
                    FROM PHIEUNHAP PN
                    INNER JOIN NHANVIEN NV ON PN.MaNV_LapPhieu = NV.MaNV
                    INNER JOIN NHASANXUAT NSX ON PN.MaNSX = NSX.MaNSX
                    ORDER BY PN.NgayLap DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PhieuNhapItem
                        {
                            MaPhieuNhap = Convert.ToInt32(dr["MaPhieuNhap"]),
                            NgayLap = dr["NgayLap"] != DBNull.Value ? Convert.ToDateTime(dr["NgayLap"]) : (DateTime?)null,
                            TongTienNhap = dr["TongTienNhap"] != DBNull.Value ? Convert.ToDecimal(dr["TongTienNhap"]) : 0,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? dr["TrangThai"].ToString() : "",
                            NguoiLap = dr["NguoiLap"] != DBNull.Value ? dr["NguoiLap"].ToString() : "",
                            NhaCungCap = dr["TenNSX"] != DBNull.Value ? dr["TenNSX"].ToString() : ""
                        });
                    }
                }
            }

            return list;
        }

        // ==================== BÁC SĨ KHÁM NHIỀU NHẤT ====================
        public List<BacSiThongKe> GetBacSiKhamNhieuNhat(int top = 5)
        {
            var list = new List<BacSiThongKe>();

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string sql = @"
                    SELECT TOP " + top + @" 
                        NV.MaNV,
                        NV.HoTen,
                        CV.TenChucVu,
                        COUNT(PKB.MaPhieuKhamBenh) AS SoLuotKham
                    FROM NHANVIEN NV
                    INNER JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
                    LEFT JOIN PHIEUKHAMBENH PKB ON NV.MaNV = PKB.MaBacSiKham
                    WHERE CV.TenChucVu LIKE N'%Bác sĩ%'
                      AND PKB.TrangThai = N'Hoàn thành'
                      AND PKB.NgayLap >= DATEADD(day, -30, CAST(GETDATE() AS DATE))
                    GROUP BY NV.MaNV, NV.HoTen, CV.TenChucVu
                    ORDER BY SoLuotKham DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new BacSiThongKe
                        {
                            MaNV = dr["MaNV"].ToString(),
                            HoTen = dr["HoTen"].ToString(),
                            TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "",
                            SoLuotKham = dr["SoLuotKham"] != DBNull.Value ? Convert.ToInt32(dr["SoLuotKham"]) : 0
                        });
                    }
                }
            }

            return list;
        }
    }

    // ==================== CLASS HỖ TRỢ ====================
    public class DashboardThongKe
    {
        // Tổng quan
        public int TongBenhNhan { get; set; }
        public int TongNhanVien { get; set; }
        public int TongThuoc { get; set; }
        public int TongKhoa { get; set; }
        public int TongPhong { get; set; }

        // Khám bệnh
        public int DangKham { get; set; }
        public int HoanThanh { get; set; }
        public int KhamHomNay { get; set; }
        public int KhamTuanNay { get; set; }

        // Doanh thu
        public decimal TongTienGoc { get; set; }
        public decimal TienBHYT { get; set; }
        public decimal TienBenhNhanTra { get; set; }

        public decimal TongDoanhThu => TongTienGoc;
        public decimal DoanhThuThucNhan => TienBenhNhanTra;
    }

    public class DoanhThuNgay
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        public string NgayDisplay => Ngay.ToString("dd/MM");
    }

    public class DoanhThuThang
    {
        public int Thang { get; set; }
        public string TenThang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class ThuocSapHetHan
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string MaLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public int SoLuongTon { get; set; }
        public string TenKho { get; set; }
        public int SoNgayConLai => (HanSuDung - DateTime.Now.Date).Days;
        public string HanSuDungDisplay => HanSuDung.ToString("dd/MM/yyyy");
    }

    public class ThuocSapHetHang
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViCoBan { get; set; }
        public int TongTon { get; set; }
        public string TenKho { get; set; }
    }

    public class PhieuNhapItem
    {
        public int MaPhieuNhap { get; set; }
        public DateTime? NgayLap { get; set; }
        public decimal TongTienNhap { get; set; }
        public string TrangThai { get; set; }
        public string NguoiLap { get; set; }
        public string NhaCungCap { get; set; }
        public string NgayLapDisplay => NgayLap.HasValue ? NgayLap.Value.ToString("dd/MM/yyyy") : "";
    }

    public class BacSiThongKe
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenChucVu { get; set; }
        public int SoLuotKham { get; set; }
    }
}
