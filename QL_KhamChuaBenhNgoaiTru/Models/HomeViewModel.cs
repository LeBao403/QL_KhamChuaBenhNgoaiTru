using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    // Lớp tổng chứa toàn bộ dữ liệu cho Trang chủ
    public class HomeViewModel
    {
        public List<Khoa> DanhSachKhoa { get; set; }
        public List<BacSiHome> DanhSachBacSi { get; set; }
        public List<TinTucHome> DanhSachTinTuc { get; set; }
        public ThongKeHome ThongKe { get; set; }
    }

    // Class mỏng nhẹ chỉ chứa những gì cần hiển thị cho Bác sĩ ở trang chủ
    public class BacSiHome
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenChucVu { get; set; }
        public string TenKhoa { get; set; }
        public string HinhAnh { get; set; } // Link ảnh mạng
    }

    // Class Thống kê con số
    public class ThongKeHome
    {
        public int TongSoKhoa { get; set; }
        public int TongSoPhong { get; set; }
        public int TongSoNhanVien { get; set; }
        public int TongLuotKham { get; set; }
    }

    // Class giả lập Tin tức (Vì Database của bạn chưa có bảng TinTuc)
    public class TinTucHome
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string TomTat { get; set; }
        public string HinhAnh { get; set; }
        public DateTime NgayDang { get; set; }
    }
}