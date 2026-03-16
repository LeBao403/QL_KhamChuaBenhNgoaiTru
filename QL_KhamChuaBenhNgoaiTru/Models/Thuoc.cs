using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

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
        public string MaThuoc { get; set; }

        [DisplayName("Tên thuốc")]
        [Required(ErrorMessage = "Tên thuốc là bắt buộc")]
        public string TenThuoc { get; set; }

        [DisplayName("Quy cách")]
        public string QuyCach { get; set; }

        [DisplayName("Đơn vị cơ bản")]
        [Required]
        public string DonViCoBan { get; set; }

        [DisplayName("Loại thuốc")]
        public string MaLoaiThuoc { get; set; }

        [DisplayName("Đường dùng")]
        public string DuongDung { get; set; }

        [DisplayName("Giá bán")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? GiaBan { get; set; }

        [DisplayName("Nhà sản xuất")]
        public int? MaNSX { get; set; }

        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }
    }

    public class DonViQuyDoi
    {
        [Key]
        public int MaQuyDoi { get; set; }

        [DisplayName("Mã thuốc")]
        [Required]
        public string MaThuoc { get; set; }

        [DisplayName("Tên đơn vị (Hộp/Vỉ)")]
        [Required]
        public string TenDonVi { get; set; }

        [DisplayName("Tỷ lệ quy đổi")]
        [Required]
        public int TyLeQuyDoi { get; set; }

        [DisplayName("Giá bán quy đổi")]
        public decimal? GiaBanQuyDoi { get; set; }

        [DisplayName("Cấp độ hiển thị")]
        public int? CapDo { get; set; }
    }

    public class ThanhPhanThuoc
    {
        [Key]
        public string MaThanhPhan { get; set; }
        public string MaThuoc { get; set; }
        public string MaHoatChat { get; set; }
        public string HamLuong { get; set; }
    }
}