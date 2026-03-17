using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class NhanVien
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public int? MaChucVu { get; set; }
        public bool TrangThai { get; set; }
        public int? MaTK { get; set; }
        public int? MaPhong { get; set; }
        public int? MaKhoa { get; set; }
    }

    public class ChucVu
    {
        public int MaChucVu { get; set; }
        public string TenChucVu { get; set; }
    }
    public class Phong
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string LoaiPhong { get; set; }
        public bool TrangThai { get; set; }
        public int? MaKhoa { get; set; }
    }

    public class Khoa
    {
        [Key]
        [DisplayName("Mã khoa")]
        public int MaKhoa { get; set; }

        [Required(ErrorMessage = "Tên khoa là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khoa không được dài quá 100 ký tự")]
        [DisplayName("Tên khoa")]
        public string TenKhoa { get; set; }

        [DisplayName("Mô tả")]
        public string MoTa { get; set; }

        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }
    }

    public class NhanVienViewModel
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenChucVu { get; set; }
        public string SDT { get; set; }
    }


    public class NhanVienManageViewModel
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenChucVu { get; set; }
        public string TenCoSo { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string Username { get; set; } // Username từ TAIKHOAN
        public string PasswordHash { get; set; } // Mật khẩu gốc từ TAIKHOAN
        public bool TrangThai { get; set; } // Trạng thái làm việc
        public string TenPhong { get; set; }
    }
}