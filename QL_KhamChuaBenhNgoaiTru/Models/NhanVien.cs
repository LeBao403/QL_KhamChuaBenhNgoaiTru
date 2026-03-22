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
        public string HinhAnh { get; set; }
    }

    public class ChucVu
    {
        public int MaChucVu { get; set; }
        public string TenChucVu { get; set; }
    }

    // [Thêm mới] Class cho bảng Danh Mục Loại Phòng
    public class DanhMucLoaiPhong
    {
        public int MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }
    }

    public class Phong
    {
        [Key]
        public int MaPhong { get; set; }

        [Required(ErrorMessage = "Tên phòng không được để trống")]
        [DisplayName("Tên phòng")]
        public string TenPhong { get; set; }

        // [Sửa đổi] Đổi từ chuỗi LoaiPhong sang Khóa ngoại MaLoaiPhong
        [DisplayName("Loại phòng")]
        public int? MaLoaiPhong { get; set; }

        [DisplayName("Trạng thái")]
        public bool TrangThai { get; set; }

        [DisplayName("Khoa trực thuộc")]
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

    public class KhoaManageViewModel
    {
        public Khoa Khoa { get; set; }
        public List<PhongManageViewModel> DanhSachPhong { get; set; }
        public List<NhanVienManageViewModel> DanhSachNhanVien { get; set; }
        public List<PhongManageViewModel> DanhSachPhongKhaDung { get; set; }
        public List<NhanVienManageViewModel> DanhSachNhanVienKhaDung { get; set; }
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
        public string TenKhoa { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool TrangThai { get; set; }
        public string TenPhong { get; set; }
        public int? MaChucVu { get; set; }
        public int? MaPhong { get; set; }
        public int? MaKhoa { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string HinhAnh { get; set; }
    }

    // ViewModel dùng cho Create/Edit/Details
    public class NhanVienManageViewModel2
    {
        public NhanVien NhanVien { get; set; } = new NhanVien();
        public TaiKhoan TaiKhoan { get; set; } = new TaiKhoan();
        public string TenKhoa { get; set; }
        public string TenChucVu { get; set; }
    }

    // Dùng để hiển thị ở trang Danh sách (Index)
    public class PhongManageViewModel
    {
        public int MaPhong { get; set; }
        public string TenPhong { get; set; }

        // [Sửa đổi] Tách ra mã và tên để tiện hiển thị và xử lý logic
        public int? MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } // Lấy từ lệnh JOIN với bảng DANHMUC_LOAIPHONG

        public bool TrangThai { get; set; }

        public int? MaKhoa { get; set; }
        public string TenKhoa { get; set; } // Lấy từ lệnh JOIN với bảng KHOA

        public int SoLuongNV { get; set; }  // Đếm số nhân viên đang trực thuộc
    }

    // Dùng cho trang Details Phòng
    public class PhongDetailsViewModel
    {
        public PhongManageViewModel Phong { get; set; }
        public List<NhanVienManageViewModel> DanhSachNhanVien { get; set; }
        public List<NhanVienManageViewModel> DanhSachNhanVienKhaDung { get; set; } // NV chưa có phòng để thêm vào
    }
    
}