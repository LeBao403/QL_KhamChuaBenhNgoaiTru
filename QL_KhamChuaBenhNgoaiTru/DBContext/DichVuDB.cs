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

        // 1. LẤY DANH SÁCH LOẠI DỊCH VỤ
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

        // 2. LẤY DANH SÁCH DỊCH VỤ (PHÂN TRANG, LỌC)
        public List<DichVuViewModel> GetDanhSachDichVu(int pageIndex, int pageSize, string keyword = "", string maLoai = "", decimal? minPrice = null, decimal? maxPrice = null, string sortPrice = "")
        {
            var list = new List<DichVuViewModel>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string sql = @"
                    SELECT D.*, L.TenLoaiDV 
                    FROM DICHVU D
                    INNER JOIN LOAI_DICHVU L ON D.MaLoaiDV = L.MaLoaiDV
                    WHERE 1=1 ";

                if (!string.IsNullOrEmpty(keyword)) sql += " AND (D.TenDV LIKE @Keyword OR D.MaDV LIKE @Keyword) ";
                if (!string.IsNullOrEmpty(maLoai)) sql += " AND D.MaLoaiDV = @MaLoai ";
                if (minPrice.HasValue) sql += " AND D.GiaDichVu >= @MinPrice ";
                if (maxPrice.HasValue) sql += " AND D.GiaDichVu <= @MaxPrice ";

                if (sortPrice == "asc") sql += " ORDER BY D.GiaDichVu ASC ";
                else if (sortPrice == "desc") sql += " ORDER BY D.GiaDichVu DESC ";
                else sql += " ORDER BY D.MaDV DESC ";

                sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(sql, con);
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
                            TrangThai = rd["TrangThai"] != DBNull.Value ? Convert.ToBoolean(rd["TrangThai"]) : true,
                            MoTa = rd["MoTa"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 3. TÍNH TỔNG SỐ BẢN GHI
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

        // 4. CHI TIẾT DỊCH VỤ
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
                        TrangThai = rd["TrangThai"] != DBNull.Value ? Convert.ToBoolean(rd["TrangThai"]) : true,
                        MoTa = rd["MoTa"]?.ToString()
                    };
                }
            }
            return null;
        }

        // 5. THÊM MỚI (Đã bỏ GiaBHYT)
        public bool InsertDichVu(DichVuViewModel dv)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"INSERT INTO DICHVU (MaDV, TenDV, MaLoaiDV, GiaDichVu, DonViTinh, CoBHYT, TrangThai, MoTa) 
                               VALUES (@MaDV, @TenDV, @MaLoaiDV, @GiaDichVu, @DonViTinh, @CoBHYT, 1, @MoTa)";
                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@MaDV", GenerateNextMaDV());
                cmd.Parameters.AddWithValue("@TenDV", dv.TenDV);
                cmd.Parameters.AddWithValue("@MaLoaiDV", dv.MaLoaiDV);
                cmd.Parameters.AddWithValue("@GiaDichVu", dv.GiaDichVu);
                cmd.Parameters.AddWithValue("@DonViTinh", (object)dv.DonViTinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CoBHYT", dv.CoBHYT);
                cmd.Parameters.AddWithValue("@MoTa", (object)dv.MoTa ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 6. CẬP NHẬT (Đã bỏ GiaBHYT)
        public bool UpdateDichVu(DichVuViewModel dv)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"UPDATE DICHVU 
                               SET TenDV = @TenDV, 
                                   MaLoaiDV = @MaLoaiDV, 
                                   GiaDichVu = @GiaDichVu, 
                                   DonViTinh = @DonViTinh, 
                                   CoBHYT = @CoBHYT, 
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
                cmd.Parameters.AddWithValue("@TrangThai", dv.TrangThai);
                cmd.Parameters.AddWithValue("@MoTa", (object)dv.MoTa ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 7. XÓA
        public string DeleteDichVu(string maDV)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string checkSql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM CHITIET_CHIDINH WHERE MaDV = @MaDV) +
                        (SELECT COUNT(*) FROM CT_HOADON_DV WHERE MaDV = @MaDV)";

                SqlCommand checkCmd = new SqlCommand(checkSql, con);
                checkCmd.Parameters.AddWithValue("@MaDV", maDV);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    return "Dữ liệu đang được sử dụng! Không thể xóa hẳn. Vui lòng sử dụng chức năng 'Khóa' dịch vụ.";
                }

                string deleteSql = "DELETE FROM DICHVU WHERE MaDV = @MaDV";
                SqlCommand deleteCmd = new SqlCommand(deleteSql, con);
                deleteCmd.Parameters.AddWithValue("@MaDV", maDV);

                int result = deleteCmd.ExecuteNonQuery();
                return result > 0 ? "OK" : "Không tìm thấy dịch vụ để xóa.";
            }
        }

        // 8. SINH MÃ TỰ ĐỘNG
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