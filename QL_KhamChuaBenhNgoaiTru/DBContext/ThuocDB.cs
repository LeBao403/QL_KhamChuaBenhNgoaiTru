using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class ThuocDB
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. LẤY DANH SÁCH THUỐC (CÓ PHÂN TRANG)
        public List<ThuocManageViewModel> GetAllThuoc(int page, int pageSize)
        {
            var list = new List<ThuocManageViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"SELECT * FROM (
                                SELECT T.*, DM.TenDanhMuc, ROW_NUMBER() OVER (ORDER BY T.MaThuoc) AS RowNum 
                                FROM THUOC T LEFT JOIN DANHMUC_THUOC DM ON T.MaLoaiThuoc = DM.MaDanhMuc
                               ) AS PagedResult WHERE RowNum >= @Start AND RowNum < @End";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Start", (page - 1) * pageSize + 1);
                cmd.Parameters.AddWithValue("@End", page * pageSize + 1);

                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ThuocManageViewModel
                        {
                            Thuoc = MapThuoc(rd),
                            TenLoaiThuoc = rd["TenDanhMuc"] != DBNull.Value ? rd["TenDanhMuc"].ToString() : "N/A"
                        });
                    }
                }
            }
            return list;
        }

        public int GetCountThuoc()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM THUOC", con);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<ThuocManageViewModel> SearchThuoc(string keyword)
        {
            var list = new List<ThuocManageViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"SELECT T.*, DM.TenDanhMuc FROM THUOC T 
                               LEFT JOIN DANHMUC_THUOC DM ON T.MaLoaiThuoc = DM.MaDanhMuc 
                               WHERE T.MaThuoc LIKE @Kw OR T.TenThuoc LIKE @Kw";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Kw", "%" + keyword + "%");

                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ThuocManageViewModel
                        {
                            Thuoc = MapThuoc(rd),
                            TenLoaiThuoc = rd["TenDanhMuc"] != DBNull.Value ? rd["TenDanhMuc"].ToString() : "N/A"
                        });
                    }
                }
            }
            return list;
        }

        // 2. LẤY CHI TIẾT 1 THUỐC
        public Thuoc GetThuocById(string id)
        {
            Thuoc t = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM THUOC WHERE MaThuoc = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read()) t = MapThuoc(rd);
                }
            }
            return t;
        }

        // 3. THAO TÁC CRUD VỚI TRANSACTION (THUỐC + THÀNH PHẦN)
        public bool CreateThuoc(Thuoc thuoc, List<ThanhPhanThuoc> thanhPhans)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 3.1 Thêm Thuốc
                        string sqlThuoc = @"INSERT INTO THUOC (MaThuoc, TenThuoc, QuyCach, DonViCoBan, MaLoaiThuoc, DuongDung, GiaBan, CoBHYT, GiaBHYT, MaNSX, TrangThai) 
                                            VALUES (@Ma, @Ten, @Qc, @Dvt, @Loai, @Dd, @Gia, @Bhyt, @GiaBhyt, @Nsx, @Tt)";
                        SqlCommand cmd = new SqlCommand(sqlThuoc, con, trans);
                        cmd.Parameters.AddWithValue("@Ma", thuoc.MaThuoc);
                        cmd.Parameters.AddWithValue("@Ten", thuoc.TenThuoc);
                        cmd.Parameters.AddWithValue("@Qc", (object)thuoc.QuyCach ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dvt", thuoc.DonViCoBan);
                        cmd.Parameters.AddWithValue("@Loai", (object)thuoc.MaLoaiThuoc ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dd", (object)thuoc.DuongDung ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gia", (object)thuoc.GiaBan ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Bhyt", thuoc.CoBHYT);
                        cmd.Parameters.AddWithValue("@GiaBhyt", (object)thuoc.GiaBHYT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nsx", (object)thuoc.MaNSX ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Tt", thuoc.TrangThai ?? true);
                        cmd.ExecuteNonQuery();

                        // 3.2 Thêm Thành phần
                        if (thanhPhans != null && thanhPhans.Count > 0)
                        {
                            string sqlTp = "INSERT INTO THANHPHAN_THUOC (MaThanhPhan, MaThuoc, MaHoatChat, HamLuong) VALUES (@MaTP, @MaThuoc, @MaHC, @HamLuong)";
                            foreach (var tp in thanhPhans)
                            {
                                if (string.IsNullOrEmpty(tp.MaHoatChat)) continue; // Bỏ qua dòng trống
                                SqlCommand cmdTp = new SqlCommand(sqlTp, con, trans);
                                // Random chuỗi 10 ký tự cho Khóa chính bảng THANHPHAN_THUOC
                                cmdTp.Parameters.AddWithValue("@MaTP", Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());
                                cmdTp.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                                cmdTp.Parameters.AddWithValue("@MaHC", tp.MaHoatChat);
                                cmdTp.Parameters.AddWithValue("@HamLuong", (object)tp.HamLuong ?? DBNull.Value);
                                cmdTp.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdateThuoc(Thuoc thuoc, List<ThanhPhanThuoc> thanhPhans)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Cập nhật bảng THUOC
                        string sqlThuoc = @"UPDATE THUOC SET TenThuoc=@Ten, QuyCach=@Qc, DonViCoBan=@Dvt, MaLoaiThuoc=@Loai, 
                                            DuongDung=@Dd, GiaBan=@Gia, CoBHYT=@Bhyt, GiaBHYT=@GiaBhyt, MaNSX=@Nsx, TrangThai=@Tt 
                                            WHERE MaThuoc=@Ma";
                        SqlCommand cmd = new SqlCommand(sqlThuoc, con, trans);
                        cmd.Parameters.AddWithValue("@Ma", thuoc.MaThuoc);
                        cmd.Parameters.AddWithValue("@Ten", thuoc.TenThuoc);
                        cmd.Parameters.AddWithValue("@Qc", (object)thuoc.QuyCach ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dvt", thuoc.DonViCoBan);
                        cmd.Parameters.AddWithValue("@Loai", (object)thuoc.MaLoaiThuoc ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dd", (object)thuoc.DuongDung ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gia", (object)thuoc.GiaBan ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Bhyt", thuoc.CoBHYT);
                        cmd.Parameters.AddWithValue("@GiaBhyt", (object)thuoc.GiaBHYT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nsx", (object)thuoc.MaNSX ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Tt", thuoc.TrangThai ?? true);
                        cmd.ExecuteNonQuery();

                        // 2. Xóa toàn bộ thành phần cũ của thuốc này
                        SqlCommand cmdDel = new SqlCommand("DELETE FROM THANHPHAN_THUOC WHERE MaThuoc = @MaThuoc", con, trans);
                        cmdDel.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                        cmdDel.ExecuteNonQuery();

                        // 3. Thêm lại danh sách thành phần mới từ Form
                        if (thanhPhans != null && thanhPhans.Count > 0)
                        {
                            string sqlTp = "INSERT INTO THANHPHAN_THUOC (MaThanhPhan, MaThuoc, MaHoatChat, HamLuong) VALUES (@MaTP, @MaThuoc, @MaHC, @HamLuong)";
                            foreach (var tp in thanhPhans)
                            {
                                if (string.IsNullOrEmpty(tp.MaHoatChat)) continue;
                                SqlCommand cmdTp = new SqlCommand(sqlTp, con, trans);
                                cmdTp.Parameters.AddWithValue("@MaTP", Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());
                                cmdTp.Parameters.AddWithValue("@MaThuoc", thuoc.MaThuoc);
                                cmdTp.Parameters.AddWithValue("@MaHC", tp.MaHoatChat);
                                cmdTp.Parameters.AddWithValue("@HamLuong", (object)tp.HamLuong ?? DBNull.Value);
                                cmdTp.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool DeleteThuoc(string id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmdTp = new SqlCommand("DELETE FROM THANHPHAN_THUOC WHERE MaThuoc = @Id", con, trans);
                        cmdTp.Parameters.AddWithValue("@Id", id);
                        cmdTp.ExecuteNonQuery();

                        SqlCommand cmd = new SqlCommand("DELETE FROM THUOC WHERE MaThuoc = @Id", con, trans);
                        cmd.Parameters.AddWithValue("@Id", id);
                        int r = cmd.ExecuteNonQuery();

                        trans.Commit();
                        return r > 0;
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
        }

        // 4. CÁC HÀM TIỆN ÍCH LẤY DANH MỤC & HIỂN THỊ
        public List<ThanhPhanThuocDisplay> GetThanhPhanThuocDisplay(string maThuoc)
        {
            var list = new List<ThanhPhanThuocDisplay>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = @"SELECT TP.MaHoatChat, HC.TenHoatChat, TP.HamLuong 
                               FROM THANHPHAN_THUOC TP 
                               JOIN DANHMUC_HOATCHAT HC ON TP.MaHoatChat = HC.MaHoatChat 
                               WHERE TP.MaThuoc = @MaThuoc";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaThuoc", maThuoc);
                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ThanhPhanThuocDisplay
                        {
                            MaHoatChat = rd["MaHoatChat"].ToString(),
                            TenHoatChat = rd["TenHoatChat"].ToString(),
                            HamLuong = rd["HamLuong"] != DBNull.Value ? rd["HamLuong"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public List<ThanhPhanThuoc> GetThanhPhanThuocRaw(string maThuoc)
        {
            var list = new List<ThanhPhanThuoc>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM THANHPHAN_THUOC WHERE MaThuoc = @Ma", con);
                cmd.Parameters.AddWithValue("@Ma", maThuoc);
                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new ThanhPhanThuoc
                        {
                            MaThanhPhan = rd["MaThanhPhan"].ToString(),
                            MaThuoc = rd["MaThuoc"].ToString(),
                            MaHoatChat = rd["MaHoatChat"].ToString(),
                            HamLuong = rd["HamLuong"] != DBNull.Value ? rd["HamLuong"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        // Các hàm lấy danh sách Dropdown (Bạn có thể gộp chung vào 1 class DB riêng nếu muốn)
        public List<DanhMucThuoc> GetAllLoaiThuoc()
        {
            var list = new List<DanhMucThuoc>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM DANHMUC_THUOC", con);
                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) list.Add(new DanhMucThuoc { MaDanhMuc = rd["MaDanhMuc"].ToString(), TenDanhMuc = rd["TenDanhMuc"].ToString() });
                }
            }
            return list;
        }

        public List<NhaSanXuat> GetAllNSX()
        {
            var list = new List<NhaSanXuat>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM NHASANXUAT", con);
                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) list.Add(new NhaSanXuat { MaNSX = Convert.ToInt32(rd["MaNSX"]), TenNSX = rd["TenNSX"].ToString() });
                }
            }
            return list;
        }

        public List<DanhMucHoatChat> GetAllHoatChat()
        {
            var list = new List<DanhMucHoatChat>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM DANHMUC_HOATCHAT", con);
                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) list.Add(new DanhMucHoatChat { MaHoatChat = rd["MaHoatChat"].ToString(), TenHoatChat = rd["TenHoatChat"].ToString() });
                }
            }
            return list;
        }

        public string GetTenLoaiThuoc(string ma) { /* Viết câu Query tương tự GetAll, trả về Tên */ return "Tên Loại (Code omitted for brevity)"; }
        public string GetTenNSX(int ma) { return "Tên NSX (Code omitted for brevity)"; }

        // Hàm Map data
        private Thuoc MapThuoc(SqlDataReader rd)
        {
            return new Thuoc
            {
                MaThuoc = rd["MaThuoc"].ToString(),
                TenThuoc = rd["TenThuoc"].ToString(),
                QuyCach = rd["QuyCach"]?.ToString(),
                DonViCoBan = rd["DonViCoBan"].ToString(),
                MaLoaiThuoc = rd["MaLoaiThuoc"]?.ToString(),
                DuongDung = rd["DuongDung"]?.ToString(),
                GiaBan = rd["GiaBan"] != DBNull.Value ? Convert.ToDecimal(rd["GiaBan"]) : (decimal?)null,
                CoBHYT = rd["CoBHYT"] != DBNull.Value ? Convert.ToBoolean(rd["CoBHYT"]) : false,
                GiaBHYT = rd["GiaBHYT"] != DBNull.Value ? Convert.ToDecimal(rd["GiaBHYT"]) : (decimal?)null,
                MaNSX = rd["MaNSX"] != DBNull.Value ? Convert.ToInt32(rd["MaNSX"]) : (int?)null,
                TrangThai = rd["TrangThai"] != DBNull.Value ? Convert.ToBoolean(rd["TrangThai"]) : true
            };
        }
    }
}