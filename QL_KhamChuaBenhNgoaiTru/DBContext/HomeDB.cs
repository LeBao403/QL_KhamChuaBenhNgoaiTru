using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class HomeDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public HomeViewModel GetHomeData()
        {
            var model = new HomeViewModel
            {
                DanhSachKhoa = new List<Khoa>(),
                DanhSachBacSi = new List<BacSiHome>(),
                ThongKe = new ThongKeHome()
            };

            // Nhớ đổi connectStr thành biến chứa chuỗi kết nối SQL của bác nhé
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // =========================================================
                // 1. LẤY 6 KHOA NỔI BẬT (Đang hoạt động)
                // =========================================================
                string sqlKhoa = "SELECT TOP 6 * FROM KHOA WHERE TrangThai = 1 ORDER BY TenKhoa";
                using (SqlCommand cmd = new SqlCommand(sqlKhoa, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            model.DanhSachKhoa.Add(new Khoa
                            {
                                MaKhoa = Convert.ToInt32(dr["MaKhoa"]),
                                TenKhoa = dr["TenKhoa"].ToString(),
                                MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : "Chuyên khoa mũi nhọn với trang thiết bị hiện đại."
                            });
                        }
                    }
                }

                // =========================================================
                // 2. LẤY 6 BÁC SĨ TIÊU BIỂU (CÓ LẤY CỘT HINHANH TỪ DB)
                // =========================================================
                string sqlBacSi = @"
            SELECT TOP 6 NV.MaNV, NV.HoTen, CV.TenChucVu, K.TenKhoa, NV.HinhAnh 
            FROM NHANVIEN NV
            LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
            LEFT JOIN KHOA K ON NV.MaKhoa = K.MaKhoa
            WHERE NV.TrangThai = 1 
              AND (CV.TenChucVu LIKE N'%Bác sĩ%' OR CV.TenChucVu LIKE N'%Giám đốc%' OR CV.TenChucVu LIKE N'%Trưởng khoa%')
            ORDER BY NV.MaNV";

                using (SqlCommand cmd = new SqlCommand(sqlBacSi, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Xử lý đường dẫn ảnh từ Database
                            string imgFileName = dr["HinhAnh"]?.ToString();
                            string imgPath = "";

                            if (!string.IsNullOrEmpty(imgFileName))
                            {
                                // Nối thư mục local vào tên file (vd: /Images/doctors/nv001.jpg)
                                imgPath = $"/Images/doctors/{imgFileName}";
                            }
                            else
                            {
                                // Ảnh mặc định nếu CSDL của ông bác sĩ này bị trống
                                imgPath = "/Images/default-doctor.png";
                            }

                            // Thêm bác sĩ vào List
                            model.DanhSachBacSi.Add(new BacSiHome
                            {
                                MaNV = dr["MaNV"].ToString(),
                                HoTen = dr["HoTen"].ToString(),
                                TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "Chuyên gia Y tế",
                                TenKhoa = dr["TenKhoa"] != DBNull.Value ? dr["TenKhoa"].ToString() : "Khoa Khám bệnh",
                                HinhAnh = imgPath 
                            });
                        }
                    }
                }

                // =========================================================
                // 3. LẤY THỐNG KÊ (Đếm trực tiếp từ DB)
                // =========================================================
                string sqlStat = @"
            SELECT 
                (SELECT COUNT(*) FROM KHOA WHERE TrangThai = 1) AS TongKhoa,
                (SELECT COUNT(*) FROM PHONG WHERE TrangThai = 1) AS TongPhong,
                (SELECT COUNT(*) FROM NHANVIEN WHERE TrangThai = 1) AS TongNV";

                using (SqlCommand cmd = new SqlCommand(sqlStat, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model.ThongKe.TongSoKhoa = Convert.ToInt32(dr["TongKhoa"]);
                            model.ThongKe.TongSoPhong = Convert.ToInt32(dr["TongPhong"]);
                            model.ThongKe.TongSoNhanVien = Convert.ToInt32(dr["TongNV"]);
                            model.ThongKe.TongLuotKham = 5420; // Hardcode một số lượt khám cho đẹp
                        }
                    }
                }
            }

            return model;
        }

        // ====================================================================
        // 1. HÀM LẤY TOÀN BỘ DANH SÁCH BÁC SĨ (Dành cho trang Đội ngũ Bác sĩ)
        // ====================================================================
        public List<BacSiClientViewModel> GetAllBacSi()
        {
            List<BacSiClientViewModel> list = new List<BacSiClientViewModel>();

            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string sql = @"
            SELECT nv.MaNV, nv.HoTen, cv.TenChucVu, k.TenKhoa, nv.HinhAnh
            FROM NhanVien nv 
            LEFT JOIN ChucVu cv ON nv.MaChucVu = cv.MaChucVu 
            LEFT JOIN Khoa k ON nv.MaKhoa = k.MaKhoa 
            WHERE cv.TenChucVu LIKE N'%Bác sĩ%' 
               OR cv.TenChucVu LIKE N'%Trưởng khoa%'
               OR cv.TenChucVu LIKE N'%Giám đốc%'";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            BacSiClientViewModel bs = new BacSiClientViewModel();
                            bs.MaNV = rd["MaNV"]?.ToString();
                            bs.HoTen = rd["HoTen"]?.ToString();
                            bs.TenChucVu = rd["TenChucVu"]?.ToString() != "" ? rd["TenChucVu"].ToString() : "Bác sĩ chuyên khoa";
                            bs.TenKhoa = rd["TenKhoa"]?.ToString() != "" ? rd["TenKhoa"].ToString() : "Đa khoa";

                            // --- XỬ LÝ HÌNH ẢNH GHÉP CHUỖI ---
                            string imgFileName = rd["HinhAnh"]?.ToString();

                            if (!string.IsNullOrEmpty(imgFileName))
                            {
                                // Ghép thư mục vào trước tên file đã lưu trong SQL
                                bs.HinhAnh = $"/Images/doctors/{imgFileName}";
                            }
                            else
                            {
                                // Sinh avatar ảo nếu DB bị null
                                bs.HinhAnh = "https://ui-avatars.com/api/?name=" + Uri.EscapeDataString(bs.HoTen) + "&background=0d6efd&color=fff&size=300";
                            }

                            list.Add(bs);
                        }
                    }
                }
            }
            return list;
        }

        // ====================================================================
        // 2. HÀM LẤY TOÀN BỘ CHUYÊN KHOA (Dành cho trang Chuyên khoa)
        // ====================================================================
        public List<Khoa> GetAllKhoa()
        {
            List<Khoa> list = new List<Khoa>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                string sql = "SELECT MaKhoa, TenKhoa, MoTa FROM Khoa WHERE TrangThai = 1"; // Chỉ lấy khoa đang hoạt động

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Khoa k = new Khoa();
                            k.MaKhoa = Convert.ToInt32(rd["MaKhoa"]);
                            k.TenKhoa = rd["TenKhoa"].ToString();
                            k.MoTa = rd["MoTa"].ToString();
                            list.Add(k);
                        }
                    }
                }
            }
            return list;
        }
    }
}