using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class KhoaDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
        // ==========================================
        // 1. LẤY DỮ LIỆU (READ)
        // ==========================================

        // Lấy danh sách tất cả các Khoa
        public List<Khoa> GetAll()
        {
            List<Khoa> list = new List<Khoa>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM KHOA ORDER BY TenKhoa";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Khoa
                        {
                            MaKhoa = Convert.ToInt32(dr["MaKhoa"]),
                            TenKhoa = dr["TenKhoa"].ToString(),
                            MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false
                        });
                    }
                }
            }
            return list;
        }

        // Lấy 1 Khoa theo Mã (Dùng cho Edit, Details)
        public Khoa GetById(int maKhoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM KHOA WHERE MaKhoa = @MaKhoa";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKhoa", maKhoa);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Khoa
                        {
                            MaKhoa = Convert.ToInt32(dr["MaKhoa"]),
                            TenKhoa = dr["TenKhoa"].ToString(),
                            MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false
                        };
                    }
                }
            }
            return null;
        }

        // ==========================================
        // 2. KIỂM TRA ĐIỀU KIỆN (VALIDATION)
        // ==========================================

        // Kiểm tra trùng Tên Khoa (Dùng chung cho cả Create và Edit)
        // excludeMaKhoa: Bỏ qua chính nó khi đang Edit
        public bool CheckTenKhoaExists(string tenKhoa, int excludeMaKhoa = 0)
        {
            if (string.IsNullOrWhiteSpace(tenKhoa)) return false;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM KHOA WHERE TenKhoa = @TenKhoa AND MaKhoa <> @MaKhoa";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenKhoa", tenKhoa.Trim());
                cmd.Parameters.AddWithValue("@MaKhoa", excludeMaKhoa);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Kiểm tra Ràng buộc trước khi XÓA
        public bool CheckCanDelete(int maKhoa, out string errorMessage)
        {
            errorMessage = "";
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. Kiểm tra Nhân viên
                string checkNV = "SELECT COUNT(*) FROM NHANVIEN WHERE MaKhoa = @MaKhoa";
                SqlCommand cmdNV = new SqlCommand(checkNV, conn);
                cmdNV.Parameters.AddWithValue("@MaKhoa", maKhoa);
                int countNV = Convert.ToInt32(cmdNV.ExecuteScalar());

                if (countNV > 0)
                {
                    errorMessage = $"Khoa này đang có {countNV} nhân viên trực thuộc.";
                    return false;
                }

                // 2. Kiểm tra Phòng ban
                string checkPhong = "SELECT COUNT(*) FROM PHONG WHERE MaKhoa = @MaKhoa";
                SqlCommand cmdPhong = new SqlCommand(checkPhong, conn);
                cmdPhong.Parameters.AddWithValue("@MaKhoa", maKhoa);
                int countPhong = Convert.ToInt32(cmdPhong.ExecuteScalar());

                if (countPhong > 0)
                {
                    errorMessage = $"Khoa này đang quản lý {countPhong} phòng ban.";
                    return false;
                }

                return true;
            }
        }

        // ==========================================
        // 3. THAO TÁC (CREATE, UPDATE, DELETE)
        // ==========================================

        // Thêm mới
        public bool Create(Khoa khoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "INSERT INTO KHOA (TenKhoa, MoTa, TrangThai) VALUES (@TenKhoa, @MoTa, @TrangThai)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenKhoa", khoa.TenKhoa.Trim());
                cmd.Parameters.AddWithValue("@MoTa", (object)khoa.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", khoa.TrangThai);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Cập nhật
        public bool Update(Khoa khoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "UPDATE KHOA SET TenKhoa = @TenKhoa, MoTa = @MoTa, TrangThai = @TrangThai WHERE MaKhoa = @MaKhoa";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenKhoa", khoa.TenKhoa.Trim());
                cmd.Parameters.AddWithValue("@MoTa", (object)khoa.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", khoa.TrangThai);
                cmd.Parameters.AddWithValue("@MaKhoa", khoa.MaKhoa);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Xóa vĩnh viễn (Chỉ gọi khi CheckCanDelete trả về true)
        public bool Delete(int maKhoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "DELETE FROM KHOA WHERE MaKhoa = @MaKhoa";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKhoa", maKhoa);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Đảo trạng thái (Khóa <-> Mở) bằng thuật toán XOR (^)
        public bool? ToggleStatus(int maKhoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                // Lấy trạng thái hiện tại
                string sqlGet = "SELECT TrangThai FROM KHOA WHERE MaKhoa = @MaKhoa";
                SqlCommand cmdGet = new SqlCommand(sqlGet, conn);
                cmdGet.Parameters.AddWithValue("@MaKhoa", maKhoa);
                object result = cmdGet.ExecuteScalar();

                if (result == null || result == DBNull.Value) return null;

                bool currentStatus = Convert.ToBoolean(result);
                bool newStatus = !currentStatus;

                // Cập nhật trạng thái mới
                string sqlUpdate = "UPDATE KHOA SET TrangThai = @NewStatus WHERE MaKhoa = @MaKhoa";
                SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn);
                cmdUpdate.Parameters.AddWithValue("@NewStatus", newStatus);
                cmdUpdate.Parameters.AddWithValue("@MaKhoa", maKhoa);

                cmdUpdate.ExecuteNonQuery();
                return newStatus;
            }
        }

        public List<Khoa> Search(string keyword)
        {
            List<Khoa> list = new List<Khoa>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Ép kiểu MaKhoa sang chuỗi để có thể tìm kiếm bằng LIKE
                string query = @"
            SELECT MaKhoa, TenKhoa, MoTa, TrangThai
            FROM KHOA
            WHERE TenKhoa LIKE @kw 
               OR MoTa LIKE @kw 
               OR CAST(MaKhoa AS NVARCHAR) LIKE @kw
            ORDER BY TenKhoa";

                SqlCommand cmd = new SqlCommand(query, conn);
                // Bọc từ khóa bằng % để tìm kiếm chứa chuỗi (chữ hoa chữ thường SQL tự lo)
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Khởi tạo và nạp thẳng vào List<Khoa>
                    list.Add(new Khoa
                    {
                        MaKhoa = Convert.ToInt32(dr["MaKhoa"]),
                        TenKhoa = dr["TenKhoa"].ToString(),
                        MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : null,
                        TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false
                    });
                }
            }
            return list;
        }

        public KhoaManageViewModel GetKhoaDetails(int maKhoa)
        {
            var model = new KhoaManageViewModel
            {
                DanhSachPhong = new List<PhongManageViewModel>(),
                DanhSachNhanVien = new List<NhanVienManageViewModel>(),
                DanhSachPhongKhaDung = new List<PhongManageViewModel>(), // Mới
                DanhSachNhanVienKhaDung = new List<NhanVienManageViewModel>() // Mới
            };

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. Lấy thông tin Khoa
                string sqlKhoa = "SELECT * FROM KHOA WHERE MaKhoa = @MaKhoa";
                SqlCommand cmdKhoa = new SqlCommand(sqlKhoa, conn);
                cmdKhoa.Parameters.AddWithValue("@MaKhoa", maKhoa);
                using (SqlDataReader dr = cmdKhoa.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model.Khoa = new Khoa
                        {
                            MaKhoa = Convert.ToInt32(dr["MaKhoa"]),
                            TenKhoa = dr["TenKhoa"].ToString(),
                            MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : null,
                            TrangThai = dr["TrangThai"] != DBNull.Value ? (bool?)Convert.ToBoolean(dr["TrangThai"]) : null
                        };
                    }
                    else return null;
                }

                // 2. Lấy danh sách Phòng thuộc Khoa 
                string sqlPhong = @"
            SELECT P.*, L.TenLoaiPhong, (SELECT COUNT(*) FROM NHANVIEN NV WHERE NV.MaPhong = P.MaPhong) AS SoLuongNV
            FROM PHONG P
            LEFT JOIN DANHMUC_LOAIPHONG L ON P.MaLoaiPhong = L.MaLoaiPhong
            WHERE P.MaKhoa = @MaKhoa";
                SqlCommand cmdPhong = new SqlCommand(sqlPhong, conn);
                cmdPhong.Parameters.AddWithValue("@MaKhoa", maKhoa);
                using (SqlDataReader dr = cmdPhong.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.DanhSachPhong.Add(new PhongManageViewModel
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString(),
                            TenLoaiPhong = dr["TenLoaiPhong"] != DBNull.Value ? dr["TenLoaiPhong"].ToString() : "Chưa phân loại",
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false,
                            SoLuongNV = Convert.ToInt32(dr["SoLuongNV"]), // Đếm số người trong phòng
                            MaKhoa = maKhoa
                        });
                    }
                }

                // 3. Lấy danh sách Nhân viên thuộc Khoa
                string sqlNV = @"
            SELECT NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.GioiTinh, NV.DiaChi, NV.TrangThai, 
                   CV.TenChucVu, P.TenPhong 
            FROM NHANVIEN NV
            LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
            LEFT JOIN PHONG P ON NV.MaPhong = P.MaPhong
            WHERE NV.MaKhoa = @MaKhoa";
                SqlCommand cmdNV = new SqlCommand(sqlNV, conn);
                cmdNV.Parameters.AddWithValue("@MaKhoa", maKhoa);
                using (SqlDataReader dr = cmdNV.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.DanhSachNhanVien.Add(new NhanVienManageViewModel
                        {
                            MaNV = dr["MaNV"].ToString(),
                            HoTen = dr["HoTen"].ToString(),
                            SDT = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : "---",
                            Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : "---",
                            GioiTinh = dr["GioiTinh"] != DBNull.Value ? dr["GioiTinh"].ToString() : "---",
                            TrangThai = dr["TrangThai"] != DBNull.Value ? Convert.ToBoolean(dr["TrangThai"]) : false,
                            TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "Chưa phân chức vụ",
                            TenPhong = dr["TenPhong"] != DBNull.Value ? dr["TenPhong"].ToString() : "Chưa xếp phòng"
                        });
                    }
                }

                // 4. [MỚI] Lấy danh sách Phòng Khả dụng (Chưa có khoa)
                string sqlPhongRanh = @"
            SELECT P.*, L.TenLoaiPhong 
            FROM PHONG P
            LEFT JOIN DANHMUC_LOAIPHONG L ON P.MaLoaiPhong = L.MaLoaiPhong
            WHERE P.MaKhoa IS NULL AND P.TrangThai = 1";
                SqlCommand cmdPhongRanh = new SqlCommand(sqlPhongRanh, conn);
                using (SqlDataReader dr = cmdPhongRanh.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.DanhSachPhongKhaDung.Add(new PhongManageViewModel
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString(),
                            TenLoaiPhong = dr["TenLoaiPhong"] != DBNull.Value ? dr["TenLoaiPhong"].ToString() : "Chưa phân loại",
                        });
                    }
                }

                // 5. [MỚI] Lấy danh sách Nhân viên Khả dụng (Chưa có khoa)
                string sqlNVRanh = @"
            SELECT NV.MaNV, NV.HoTen, CV.TenChucVu 
            FROM NHANVIEN NV
            LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
            WHERE NV.MaKhoa IS NULL AND NV.TrangThai = 1";
                SqlCommand cmdNVRanh = new SqlCommand(sqlNVRanh, conn);
                using (SqlDataReader dr = cmdNVRanh.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        model.DanhSachNhanVienKhaDung.Add(new NhanVienManageViewModel
                        {
                            MaNV = dr["MaNV"].ToString(),
                            HoTen = dr["HoTen"].ToString(),
                            TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "Chưa có chức vụ",
                        });
                    }
                }
            }
            return model;
        }

        public bool AddPhongToKhoa(int maPhong, int maKhoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "UPDATE PHONG SET MaKhoa = @MaKhoa WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKhoa", maKhoa);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool AddNhanVienToKhoa(string maNV, int maKhoa)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "UPDATE NHANVIEN SET MaKhoa = @MaKhoa WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKhoa", maKhoa);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Rút phòng: Chỉ cho rút khi KHÔNG CÓ NHÂN VIÊN nào đang ngồi trong đó
        public bool RemovePhongFromKhoa(int maPhong, out string errorMessage)
        {
            errorMessage = "";
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                // Check số lượng nhân viên
                SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM NHANVIEN WHERE MaPhong = @MaPhong", conn);
                cmdCheck.Parameters.AddWithValue("@MaPhong", maPhong);
                if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                {
                    errorMessage = "Phòng này đang có nhân viên làm việc. Vui lòng chuyển nhân viên ra khỏi phòng trước khi rút phòng khỏi Khoa.";
                    return false;
                }

                // Thực thi rút (Set MaKhoa = NULL)
                SqlCommand cmdUpdate = new SqlCommand("UPDATE PHONG SET MaKhoa = NULL WHERE MaPhong = @MaPhong", conn);
                cmdUpdate.Parameters.AddWithValue("@MaPhong", maPhong);
                return cmdUpdate.ExecuteNonQuery() > 0;
            }
        }

        // Rút nhân viên: Trả về tự do, ĐỒNG THỜI rút luôn khỏi phòng hiện tại (nếu có)
        public bool RemoveNhanVienFromKhoa(string maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Set cả MaKhoa và MaPhong về NULL
                string query = "UPDATE NHANVIEN SET MaKhoa = NULL, MaPhong = NULL WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNV", maNV);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}