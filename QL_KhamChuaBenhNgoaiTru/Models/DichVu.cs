using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class LoaiDichVu
    {
        [Key]
        public string MaLoaiDV { get; set; }
        public string TenLoaiDV { get; set; }
    }

    public class DichVu
    {
        [Key]
        [DisplayName("Mã dịch vụ")]
        [Required(ErrorMessage = "Mã dịch vụ là bắt buộc")]
        [StringLength(10, ErrorMessage = "Mã dịch vụ không được dài quá 10 ký tự")]
        public string MaDV { get; set; }

        [DisplayName("Tên dịch vụ")]
        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên dịch vụ không được dài quá 200 ký tự")]
        public string TenDV { get; set; }

        [DisplayName("Loại dịch vụ")]
        [Required(ErrorMessage = "Vui lòng chọn loại dịch vụ")]
        [StringLength(10)]
        public string MaLoaiDV { get; set; }

        [DisplayName("Giá dịch vụ")]
        [Required(ErrorMessage = "Giá dịch vụ là bắt buộc")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal GiaDichVu { get; set; }

        [DisplayName("Đơn vị tính")]
        [StringLength(20, ErrorMessage = "Đơn vị tính không được dài quá 20 ký tự")]
        public string DonViTinh { get; set; }

        [DisplayName("Áp dụng BHYT")]
        public bool CoBHYT { get; set; } // Giữ lại để biết dịch vụ này có được BHYT hỗ trợ không

        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }

        public string MoTa { get; set; }
    }

    // ====================================================================
    // MODEL DÙNG RIÊNG CHO TRANG BẢNG GIÁ DỊCH VỤ
    // ====================================================================
    public class DichVuItem
    {
        public string TenDV { get; set; }
        public decimal GiaDichVu { get; set; }
        public string DonViTinh { get; set; }
        public string MoTa { get; set; }
        public bool CoBHYT { get; set; }
    }

    public class LoaiDichVuGroup
    {
        public string MaLoaiDV { get; set; }
        public string TenLoaiDV { get; set; }
        public List<DichVuItem> DanhSachDichVu { get; set; } = new List<DichVuItem>();
    }

    public class DichVuViewModel
    {
        public string MaDV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ!")]
        [StringLength(200, ErrorMessage = "Tên dịch vụ không được dài quá 200 ký tự")]
        public string TenDV { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục cho dịch vụ!")]
        public string MaLoaiDV { get; set; }

        public string TenLoaiDV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá dịch vụ!")]
        public decimal GiaDichVu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính!")]
        [StringLength(20, ErrorMessage = "Đơn vị tính không được dài quá 20 ký tự")]
        public string DonViTinh { get; set; }

        public bool CoBHYT { get; set; }

        public bool TrangThai { get; set; }

        public string MoTa { get; set; }
    }
}