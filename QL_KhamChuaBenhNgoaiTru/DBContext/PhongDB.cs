using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class PhongDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // ==========================================
        // 0. LẤY DANH MỤC LOẠI PHÒNG (THÊM MỚI)
        // ==========================================
        public List<DanhMucLoaiPhong> GetAllLoaiPhong()
        {
            List<DanhMucLoaiPhong> list = new List<DanhMucLoaiPhong>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM DANHMUC_LOAIPHONG ORDER BY TenLoaiPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new DanhMucLoaiPhong
                        {
                            MaLoaiPhong = Convert.ToInt32(dr["MaLoaiPhong"]),
                            TenLoaiPhong = dr["TenLoaiPhong"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==========================================
        // 1. LẤY DỮ LIỆU & TÌM KIẾM
        // ==========================================

        // Lấy tất cả phòng (kèm Tên khoa, Tên loại phòng và Số lượng NV)
        public List<PhongManageViewModel> GetAll()
        {
            return Search("", null, 0); // Đổi param thứ 2 thành null
        }

        // Tìm kiếm và Lọc đa điều kiện
        public List<PhongManageViewModel> Search(string keyword, int? maLoaiPhong, int maKhoa)
        {
            List<PhongManageViewModel> list = new List<PhongManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Thêm LEFT JOIN DANHMUC_LOAIPHONG L
                string query = @"
                SELECT P.*, K.TenKhoa, L.TenLoaiPhong, 
                       (SELECT COUNT(*) FROM NHANVIEN NV WHERE NV.MaPhong = P.MaPhong) AS SoLuongNV
                FROM PHONG P
                LEFT JOIN KHOA K ON P.MaKhoa = K.MaKhoa
                LEFT JOIN DANHMUC_LOAIPHONG L ON P.MaLoaiPhong = L.MaLoaiPhong
                WHERE 1=1 ";

                if (!string.IsNullOrWhiteSpace(keyword))
                    query += " AND (P.TenPhong LIKE @kw OR CAST(P.MaPhong AS NVARCHAR) LIKE @kw) ";

                if (maLoaiPhong.HasValue && maLoaiPhong.Value > 0)
                    query += " AND P.MaLoaiPhong = @MaLoaiPhong ";

                if (maKhoa > 0)
                    query += " AND P.MaKhoa = @MaKhoa ";

                query += " ORDER BY P.TenPhong";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrWhiteSpace(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                if (maLoaiPhong.HasValue && maLoaiPhong.Value > 0) cmd.Parameters.AddWithValue("@MaLoaiPhong", maLoaiPhong.Value);
                if (maKhoa > 0) cmd.Parameters.AddWithValue("@MaKhoa", maKhoa);

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PhongManageViewModel
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString(),
                            MaLoaiPhong = dr["MaLoaiPhong"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaLoaiPhong"]) : null,
                            TenLoaiPhong = dr["TenLoaiPhong"] != DBNull.Value ? dr["TenLoaiPhong"].ToString() : "Chưa xác định",
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false,
                            MaKhoa = dr["MaKhoa"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaKhoa"]) : null,
                            TenKhoa = dr["TenKhoa"] != DBNull.Value ? dr["TenKhoa"].ToString() : "Độc lập (Không thuộc Khoa)",
                            SoLuongNV = Convert.ToInt32(dr["SoLuongNV"])
                        });
                    }
                }
            }
            return list;
        }

        public Phong GetById(int maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM PHONG WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Phong
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString(),
                            MaLoaiPhong = dr["MaLoaiPhong"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaLoaiPhong"]) : null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false,
                            MaKhoa = dr["MaKhoa"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaKhoa"]) : null
                        };
                    }
                }
            }
            return null;
        }

        // ==========================================
        // 2. KIỂM TRA ĐIỀU KIỆN (VALIDATION RẤT GẮT)
        // ==========================================

        public bool CheckTenPhongExists(string tenPhong, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(tenPhong)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM PHONG WHERE TenPhong = @TenPhong AND MaPhong <> @MaPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenPhong", tenPhong.Trim());
                cmd.Parameters.AddWithValue("@MaPhong", excludeId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool CheckCanDelete(int maPhong, out string errorMessage)
        {
            errorMessage = "";
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. Check Nhân viên đang ngồi
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM NHANVIEN WHERE MaPhong = @MaPhong", conn);
                cmd1.Parameters.AddWithValue("@MaPhong", maPhong);
                if (Convert.ToInt32(cmd1.ExecuteScalar()) > 0)
                {
                    errorMessage = "Phòng này đang có nhân viên trực thuộc."; return false;
                }

                // 2. Check Tồn kho (Phòng kho/Thuốc)
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM TONKHO WHERE MaPhong = @MaPhong", conn);
                cmd2.Parameters.AddWithValue("@MaPhong", maPhong);
                if (Convert.ToInt32(cmd2.ExecuteScalar()) > 0)
                {
                    errorMessage = "Phòng này đang chứa dữ liệu Tồn kho vật tư/thuốc."; return false;
                }

                // 3. Check Lịch sử Khám bệnh
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM PHIEUKHAMBENH WHERE MaPhong = @MaPhong", conn);
                cmd3.Parameters.AddWithValue("@MaPhong", maPhong);
                if (Convert.ToInt32(cmd3.ExecuteScalar()) > 0)
                {
                    errorMessage = "Phòng này chứa lịch sử Khám bệnh của bệnh nhân."; return false;
                }

                // 4. Check Lịch sử Chỉ định Cận lâm sàng
                SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM PHIEU_CHIDINH WHERE MaPhong = @MaPhong", conn);
                cmd4.Parameters.AddWithValue("@MaPhong", maPhong);
                if (Convert.ToInt32(cmd4.ExecuteScalar()) > 0)
                {
                    errorMessage = "Phòng này chứa lịch sử Chỉ định dịch vụ cận lâm sàng."; return false;
                }

                return true;
            }
        }

        // ==========================================
        // 3. CRUD (THÊM, SỬA, XÓA, ĐẢO TRẠNG THÁI)
        // ==========================================

        public bool Create(Phong model)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "INSERT INTO PHONG (TenPhong, MaLoaiPhong, TrangThai, MaKhoa) VALUES (@TenPhong, @MaLoaiPhong, @TrangThai, @MaKhoa)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenPhong", model.TenPhong.Trim());
                cmd.Parameters.AddWithValue("@MaLoaiPhong", (object)model.MaLoaiPhong ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", model.TrangThai);
                cmd.Parameters.AddWithValue("@MaKhoa", (object)model.MaKhoa ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(Phong model)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "UPDATE PHONG SET TenPhong = @TenPhong, MaLoaiPhong = @MaLoaiPhong, TrangThai = @TrangThai, MaKhoa = @MaKhoa WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenPhong", model.TenPhong.Trim());
                cmd.Parameters.AddWithValue("@MaLoaiPhong", (object)model.MaLoaiPhong ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", model.TrangThai);
                cmd.Parameters.AddWithValue("@MaKhoa", (object)model.MaKhoa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaPhong", model.MaPhong);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "DELETE FROM PHONG WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool? ToggleStatus(int maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlCommand cmdGet = new SqlCommand("SELECT TrangThai FROM PHONG WHERE MaPhong = @MaPhong", conn);
                cmdGet.Parameters.AddWithValue("@MaPhong", maPhong);
                object result = cmdGet.ExecuteScalar();
                if (result == null || result == DBNull.Value) return null;

                bool newStatus = !Convert.ToBoolean(result);
                SqlCommand cmdUpdate = new SqlCommand("UPDATE PHONG SET TrangThai = @NewStatus WHERE MaPhong = @MaPhong", conn);
                cmdUpdate.Parameters.AddWithValue("@NewStatus", newStatus);
                cmdUpdate.Parameters.AddWithValue("@MaPhong", maPhong);
                cmdUpdate.ExecuteNonQuery();
                return newStatus;
            }
        }

        // ==========================================
        // 4. QUẢN LÝ NHÂN SỰ TRONG PHÒNG (LOGIC ĐIỀU PHỐI)
        // ==========================================

        // Lấy nhân viên đang trực thuộc phòng này
        public List<NhanVienManageViewModel> GetNhanVienByRoom(int maPhong)
        {
            List<NhanVienManageViewModel> list = new List<NhanVienManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                SELECT NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.GioiTinh, NV.TrangThai, CV.TenChucVu 
                FROM NHANVIEN NV
                LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
                WHERE NV.MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new NhanVienManageViewModel
                        {
                            MaNV = dr["MaNV"].ToString(),
                            HoTen = dr["HoTen"].ToString(),
                            SDT = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : "---",
                            Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : "---",
                            GioiTinh = dr["GioiTinh"] != DBNull.Value ? dr["GioiTinh"].ToString() : "---",
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false,
                            TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "Chưa phân chức vụ"
                        });
                    }
                }
            }
            return list;
        }

        // Lấy nhân viên CÙNG KHOA nhưng CHƯA CÓ PHÒNG (Để hiện lên Popup thêm vào phòng)
        public List<NhanVienManageViewModel> GetAvailableNhanVienForRoom(int? maKhoa)
        {
            List<NhanVienManageViewModel> list = new List<NhanVienManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                SELECT NV.MaNV, NV.HoTen, CV.TenChucVu 
                FROM NHANVIEN NV
                LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
                WHERE NV.MaPhong IS NULL AND NV.TrangThai = 1 ";

                if (maKhoa.HasValue && maKhoa.Value > 0)
                    query += " AND NV.MaKhoa = @MaKhoa";
                else
                    query += " AND NV.MaKhoa IS NULL"; // Xử lý cho phòng Hành chính/Thu ngân

                SqlCommand cmd = new SqlCommand(query, conn);
                if (maKhoa.HasValue && maKhoa.Value > 0)
                    cmd.Parameters.AddWithValue("@MaKhoa", maKhoa.Value);

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new NhanVienManageViewModel
                        {
                            MaNV = dr["MaNV"].ToString(),
                            HoTen = dr["HoTen"].ToString(),
                            TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "Chưa có chức vụ"
                        });
                    }
                }
            }
            return list;
        }

        // Đưa nhân viên vào phòng
        public bool AddNhanVienToRoom(string maNV, int maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "UPDATE NHANVIEN SET MaPhong = @MaPhong WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Rút nhân viên khỏi phòng (Trả về khoa)
        public bool RemoveNhanVienFromRoom(string maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "UPDATE NHANVIEN SET MaPhong = NULL WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}