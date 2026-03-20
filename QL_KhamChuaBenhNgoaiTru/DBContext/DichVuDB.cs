using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class DichVuDB
    {

        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // =======================================================================
        // 1. LẤY DANH SÁCH DANH MỤC (ĐỔ VÀO DROPDOWN LỌC)
        // =======================================================================
        public List<LoaiDichVu> GetAllLoaiDichVu()
        {
            var list = new List<LoaiDichVu>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = "SELECT MaLoaiDV, TenLoaiDV FROM LOAI_DICHVU ORDER BY TenLoaiDV";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    list.Add(new LoaiDichVu
                    {
                        MaLoaiDV = rd["MaLoaiDV"].ToString(),
                        TenLoaiDV = rd["TenLoaiDV"].ToString()
                    });
                }
            }
            return list;
        }

        // =======================================================================
        // 2. HÀM CORE: LẤY DANH SÁCH CÓ LỌC, TÌM KIẾM, SẮP XẾP & PHÂN TRANG
        // =======================================================================
        public List<DichVuViewModel> GetDanhSachDichVu(int pageIndex, int pageSize, string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null, string sortPrice = "")
        {
            var list = new List<DichVuViewModel>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();

                // Nối chuỗi SQL động để xử lý bộ lọc
                string sql = @"
                    SELECT D.*, L.TenLoaiDV 
                    FROM DICHVU D
                    INNER JOIN LOAI_DICHVU L ON D.MaLoaiDV = L.MaLoaiDV
                    WHERE 1=1 "; 

                if (!string.IsNullOrEmpty(keyword)) sql += " AND (D.TenDV LIKE @Keyword OR D.MaDV LIKE @Keyword) ";
                if (!string.IsNullOrEmpty(maLoai)) sql += " AND D.MaLoaiDV = @MaLoai ";
                if (minPrice.HasValue) sql += " AND D.GiaDichVu >= @MinPrice ";
                if (maxPrice.HasValue) sql += " AND D.GiaDichVu <= @MaxPrice ";

                // Xử lý sắp xếp (Order By)
                if (sortPrice == "asc") sql += " ORDER BY D.GiaDichVu ASC ";
                else if (sortPrice == "desc") sql += " ORDER BY D.GiaDichVu DESC ";
                else sql += " ORDER BY D.MaDV DESC "; // Mặc định cái mới nhất lên đầu

                // Xử lý Phân trang (OFFSET - FETCH)
                sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(sql, con);

                // Gán tham số an toàn (Chống SQL Injection)
                if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                if (!string.IsNullOrEmpty(maLoai)) cmd.Parameters.AddWithValue("@MaLoai", maLoai);
                if (minPrice.HasValue) cmd.Parameters.AddWithValue("@MinPrice", minPrice.Value);
                if (maxPrice.HasValue) cmd.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                cmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DichVuViewModel
                        {
                            MaDV = rd["MaDV"].ToString().Trim(),
                            TenDV = rd["TenDV"].ToString(),
                            MaLoaiDV = rd["MaLoaiDV"].ToString(),
                            TenLoaiDV = rd["TenLoaiDV"].ToString(),
                            GiaDichVu = Convert.ToDecimal(rd["GiaDichVu"]),
                            DonViTinh = rd["DonViTinh"]?.ToString(),
                            CoBHYT = rd["CoBHYT"] != DBNull.Value ? Convert.ToBoolean(rd["CoBHYT"]) : false,
                            GiaBHYT = rd["GiaBHYT"] != DBNull.Value ? Convert.ToDecimal(rd["GiaBHYT"]) : (decimal?)null,
                            TrangThai = rd["TrangThai"] != DBNull.Value ? Convert.ToBoolean(rd["TrangThai"]) : true,
                            MoTa = rd["MoTa"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        // =======================================================================
        // 3. LẤY TỔNG SỐ BẢN GHI (ĐỂ VIEW TÍNH RA SỐ TRANG PAGING)
        // =======================================================================
        public int GetTotalRecord(string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null)
        {
            int total = 0;
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string sql = "SELECT COUNT(1) FROM DICHVU D WHERE 1=1 ";

                if (!string.IsNullOrEmpty(keyword)) sql += " AND (D.TenDV LIKE @Keyword OR D.MaDV LIKE @Keyword) ";
                if (!string.IsNullOrEmpty(maLoai)) sql += " AND D.MaLoaiDV = @MaLoai ";
                if (minPrice.HasValue) sql += " AND D.GiaDichVu >= @MinPrice ";
                if (maxPrice.HasValue) sql += " AND D.GiaDichVu <= @MaxPrice ";

                SqlCommand cmd = new SqlCommand(sql, con);
                if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim() + "%");
                if (!string.IsNullOrEmpty(maLoai)) cmd.Parameters.AddWithValue("@MaLoai", maLoai);
                if (minPrice.HasValue) cmd.Parameters.AddWithValue("@MinPrice", minPrice.Value);
                if (maxPrice.HasValue) cmd.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                total = (int)cmd.ExecuteScalar();
            }
            return total;
        }

        // =======================================================================
        // 4. LẤY CHI TIẾT 1 DỊCH VỤ (DÙNG CHO TRANG EDIT / DETAILS)
        // =======================================================================
        public DichVuViewModel GetDichVuById(string maDV)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT D.*, L.TenLoaiDV 
                    FROM DICHVU D
                    LEFT JOIN LOAI_DICHVU L ON D.MaLoaiDV = L.MaLoaiDV
                    WHERE D.MaDV = @MaDV";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDV", maDV);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    return new DichVuViewModel
                    {
                        MaDV = rd["MaDV"].ToString().Trim(),
                        TenDV = rd["TenDV"].ToString(),
                        MaLoaiDV = rd["MaLoaiDV"].ToString(),
                        TenLoaiDV = rd["TenLoaiDV"]?.ToString(),
                        GiaDichVu = Convert.ToDecimal(rd["GiaDichVu"]),
                        DonViTinh = rd["DonViTinh"]?.ToString(),
                        CoBHYT = rd["CoBHYT"] != DBNull.Value ? Convert.ToBoolean(rd["CoBHYT"]) : false,
                        GiaBHYT = rd["GiaBHYT"] != DBNull.Value ? Convert.ToDecimal(rd["GiaBHYT"]) : (decimal?)null,
                        TrangThai = rd["TrangThai"] != DBNull.Value ? Convert.ToBoolean(rd["TrangThai"]) : true,
                        MoTa = rd["MoTa"]?.ToString()
                    };
                }
            }
            return null;
        }

        // =======================================================================
        // 5. THÊM MỚI DỊCH VỤ (CÓ AUTO GENERATE MÃ)
        // =======================================================================
        public bool InsertDichVu(DichVuViewModel dv)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"INSERT INTO DICHVU (MaDV, TenDV, MaLoaiDV, GiaDichVu, DonViTinh, CoBHYT, GiaBHYT, TrangThai, MoTa) 
                               VALUES (@MaDV, @TenDV, @MaLoaiDV, @GiaDichVu, @DonViTinh, @CoBHYT, @GiaBHYT, 1, @MoTa)";
                SqlCommand cmd = new SqlCommand(sql, con);

                // Tự động sinh mã mới (VD: DV067)
                cmd.Parameters.AddWithValue("@MaDV", GenerateNextMaDV());
                cmd.Parameters.AddWithValue("@TenDV", dv.TenDV);
                cmd.Parameters.AddWithValue("@MaLoaiDV", dv.MaLoaiDV);
                cmd.Parameters.AddWithValue("@GiaDichVu", dv.GiaDichVu);
                cmd.Parameters.AddWithValue("@DonViTinh", (object)dv.DonViTinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CoBHYT", dv.CoBHYT);
                cmd.Parameters.AddWithValue("@GiaBHYT", dv.CoBHYT && dv.GiaBHYT.HasValue ? (object)dv.GiaBHYT.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@MoTa", (object)dv.MoTa ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // =======================================================================
        // 6. CẬP NHẬT DỊCH VỤ
        // =======================================================================
        public bool UpdateDichVu(DichVuViewModel dv)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                // THÊM TrangThai = @TrangThai VÀO CÂU LỆNH SQL
                string sql = @"UPDATE DICHVU 
                       SET TenDV = @TenDV, 
                           MaLoaiDV = @MaLoaiDV, 
                           GiaDichVu = @GiaDichVu, 
                           DonViTinh = @DonViTinh, 
                           CoBHYT = @CoBHYT, 
                           GiaBHYT = @GiaBHYT, 
                           TrangThai = @TrangThai,
                           MoTa = @MoTa
                       WHERE MaDV = @MaDV";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDV", dv.MaDV);
                cmd.Parameters.AddWithValue("@TenDV", dv.TenDV);
                cmd.Parameters.AddWithValue("@MaLoaiDV", dv.MaLoaiDV);
                cmd.Parameters.AddWithValue("@GiaDichVu", dv.GiaDichVu);
                cmd.Parameters.AddWithValue("@DonViTinh", (object)dv.DonViTinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CoBHYT", dv.CoBHYT);

                // Ràng buộc logic: Bỏ check BHYT thì tự gán Giá BHYT = NULL
                if (!dv.CoBHYT)
                    cmd.Parameters.AddWithValue("@GiaBHYT", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@GiaBHYT", dv.GiaBHYT.HasValue ? (object)dv.GiaBHYT.Value : DBNull.Value);

                // Bơm tham số TrangThai xuống DB
                cmd.Parameters.AddWithValue("@TrangThai", dv.TrangThai);

                cmd.Parameters.AddWithValue("@MoTa", (object)dv.MoTa ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // =======================================================================
        // 7. XÓA 
        // =======================================================================
        public string DeleteDichVu(string maDV)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();

                // BƯỚC 1: Kiểm tra xem dịch vụ này đã từng xuất hiện trong Chỉ định hoặc Hóa đơn chưa
                string checkSql = @"
            SELECT 
                (SELECT COUNT(*) FROM CHITIET_CHIDINH WHERE MaDV = @MaDV) +
                (SELECT COUNT(*) FROM CT_HOADON_DV WHERE MaDV = @MaDV)";

                SqlCommand checkCmd = new SqlCommand(checkSql, con);
                checkCmd.Parameters.AddWithValue("@MaDV", maDV);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    // Nếu đã có dữ liệu liên quan, trả về thông báo lỗi thay vì xóa
                    return "Dữ liệu đang được sử dụng! Dịch vụ này đã có trong Chỉ định khám hoặc Hóa đơn, không thể xóa hẳn. Vui lòng sử dụng chức năng 'Khóa' dịch vụ.";
                }

                // BƯỚC 2: Nếu chưa dùng ở đâu cả, thực hiện XÓA HẲN (Physical Delete)
                string deleteSql = "DELETE FROM DICHVU WHERE MaDV = @MaDV";
                SqlCommand deleteCmd = new SqlCommand(deleteSql, con);
                deleteCmd.Parameters.AddWithValue("@MaDV", maDV);

                int result = deleteCmd.ExecuteNonQuery();
                return result > 0 ? "OK" : "Không tìm thấy dịch vụ để xóa.";
            }
        }

        // =======================================================================
        // 8. AUTO GENERATE MÃ DỊCH VỤ (Hàm hỗ trợ ẩn)
        // =======================================================================
        public string GenerateNextMaDV()
        {
            string newMa = "DV001";
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string sql = "SELECT TOP 1 MaDV FROM DICHVU WHERE MaDV LIKE 'DV%' ORDER BY MaDV DESC";
                SqlCommand cmd = new SqlCommand(sql, con);
                object result = cmd.ExecuteScalar();

                if (result != null && result.ToString() != "")
                {
                    string lastMa = result.ToString();
                    int number = int.Parse(lastMa.Substring(2));
                    number++;
                    newMa = "DV" + number.ToString("D3");
                }
            }
            return newMa;
        }
    }
}