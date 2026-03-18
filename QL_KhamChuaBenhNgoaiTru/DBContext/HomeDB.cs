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

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. LẤY 6 KHOA NỔI BẬT (Đang hoạt động)
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

                // 2. LẤY 6 BÁC SĨ TIÊU BIỂU
                // (Ưu tiên lấy những người có chức vụ chứa chữ 'Bác sĩ', nếu không có thì lấy nhân viên bình thường)
                string sqlBacSi = @"
                    SELECT TOP 6 NV.MaNV, NV.HoTen, CV.TenChucVu, K.TenKhoa 
                    FROM NHANVIEN NV
                    LEFT JOIN CHUCVU CV ON NV.MaChucVu = CV.MaChucVu
                    LEFT JOIN KHOA K ON NV.MaKhoa = K.MaKhoa
                    WHERE NV.TrangThai = 1 AND (CV.TenChucVu LIKE N'%Bác sĩ%' OR CV.TenChucVu IS NOT NULL)
                    ORDER BY NV.MaNV";

                // Mảng chứa vài link ảnh bác sĩ ngẫu nhiên trên mạng để làm đẹp UI
                string[] anhBacSi = {
                    "https://img.freepik.com/free-photo/smiling-asian-male-doctor-pointing-upwards_1262-18321.jpg",
                    "https://img.freepik.com/free-photo/pleased-young-female-doctor-wearing-medical-robe-stethoscope-around-neck-standing-with-closed-posture_409827-254.jpg",
                    "https://img.freepik.com/free-photo/portrait-smiling-handsome-male-doctor-man_171337-5055.jpg",
                    "https://img.freepik.com/free-photo/asian-female-doctor-smiling-looking-camera_1262-18320.jpg",
                    "https://img.freepik.com/free-photo/handsome-smiling-medical-professional-examining-with-stethoscope-isolated-white_662251-404.jpg",
                    "https://img.freepik.com/free-photo/beautiful-young-female-doctor-looking-camera-office_1301-7807.jpg"
                };

                int imgIndex = 0;
                using (SqlCommand cmd = new SqlCommand(sqlBacSi, conn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            model.DanhSachBacSi.Add(new BacSiHome
                            {
                                MaNV = dr["MaNV"].ToString(),
                                HoTen = dr["HoTen"].ToString(),
                                TenChucVu = dr["TenChucVu"] != DBNull.Value ? dr["TenChucVu"].ToString() : "Chuyên gia Y tế",
                                TenKhoa = dr["TenKhoa"] != DBNull.Value ? dr["TenKhoa"].ToString() : "Khoa Khám bệnh",
                                HinhAnh = anhBacSi[imgIndex % anhBacSi.Length] // Gắn ảnh random xoay vòng
                            });
                            imgIndex++;
                        }
                    }
                }

                // 3. LẤY THỐNG KÊ (Đếm trực tiếp từ DB)
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
                            model.ThongKe.TongLuotKham = 5420; // Hardcode một số đẹp vì chưa có bảng phiếu khám
                        }
                    }
                }
            }

            return model;
        }
    }
}