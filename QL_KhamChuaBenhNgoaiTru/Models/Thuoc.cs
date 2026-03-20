using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class DanhMucThuoc
    {
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }
    }

    public class DanhMucHoatChat
    {
        public string MaHoatChat { get; set; }
        public string TenHoatChat { get; set; }
        public string MoTa { get; set; }
    }

    public class NhaSanXuat
    {
        public int MaNSX { get; set; }
        public string TenNSX { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string QuocGia { get; set; }
    }

    public class Thuoc
    {
        [Key]
        [DisplayName("Mã thuốc")]
        [Required(ErrorMessage = "Mã thuốc không được để trống")]
        [StringLength(10, ErrorMessage = "Mã thuốc không được dài quá 10 ký tự")]
        public string MaThuoc { get; set; }

        [DisplayName("Tên thuốc")]
        [Required(ErrorMessage = "Tên thuốc là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên thuốc không được dài quá 200 ký tự")]
        public string TenThuoc { get; set; }

        [DisplayName("Quy cách")]
        [StringLength(100, ErrorMessage = "Quy cách không được dài quá 100 ký tự")]
        public string QuyCach { get; set; }

        [DisplayName("Đơn vị cơ bản")]
        [Required(ErrorMessage = "Đơn vị cơ bản là bắt buộc")]
        [StringLength(20)]
        public string DonViCoBan { get; set; }

        [DisplayName("Loại thuốc")]
        [StringLength(10)]
        public string MaLoaiThuoc { get; set; }

        [DisplayName("Đường dùng")]
        [StringLength(50)]
        public string DuongDung { get; set; }

        [DisplayName("Giá bán")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? GiaBan { get; set; }

        [DisplayName("Có BHYT")]
        public bool CoBHYT { get; set; }

        [DisplayName("Giá BHYT quy định")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? GiaBHYT { get; set; }

        [DisplayName("Nhà sản xuất")]
        public int? MaNSX { get; set; }

        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }
    }

    public class ThanhPhanThuoc
    {
        [Key]
        public string MaThanhPhan { get; set; }
        public string MaThuoc { get; set; }
        public string MaHoatChat { get; set; }
        public string HamLuong { get; set; }
    }
    public class ThuocManageViewModel
    {
        public Thuoc Thuoc { get; set; }

        // Danh sách thành phần của thuốc (dùng cho Create/Edit)
        public List<ThanhPhanThuoc> DanhSachThanhPhan { get; set; }

        // Các SelectList để hiển thị Dropdown
        public SelectList DanhSachLoaiThuoc { get; set; }
        public SelectList DanhSachNSX { get; set; }

        // Danh sách tất cả hoạt chất (để load vào dropdown khi thêm dòng thành phần mới)
        public SelectList DanhSachHoatChat { get; set; }

        // Tên hiển thị thêm cho trang Details/Index
        public string TenLoaiThuoc { get; set; }
        public string TenNSX { get; set; }
        public List<ThanhPhanThuocDisplay> ChiTietThanhPhan { get; set; } // Class phụ hiển thị tên hoạt chất
    }

    // Class phụ để hiển thị chi tiết có chứa Tên hoạt chất
    public class ThanhPhanThuocDisplay
    {
        public string MaHoatChat { get; set; }
        public string TenHoatChat { get; set; }
        public string HamLuong { get; set; }
    }
}