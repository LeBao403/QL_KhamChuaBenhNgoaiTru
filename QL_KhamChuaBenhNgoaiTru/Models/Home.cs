using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    // ====================================================================
    // MODEL DÙNG RIÊNG CHO TRANG GIỚI THIỆU (ABOUT US)
    // ====================================================================
    public class GioiThieuViewModel
    {
        // Thông tin Giám đốc kéo từ bảng NhanVien (Chức vụ = 1)
        public BacSiHome GiamDoc { get; set; }

        // Tái sử dụng class Thống kê có sẵn để chạy hiệu ứng đếm số
        public ThongKeHome ThongKe { get; set; }
    }
    public class BacSiClientViewModel
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenChucVu { get; set; }
        public string TenKhoa { get; set; }
        public string HinhAnh { get; set; }
    }
}