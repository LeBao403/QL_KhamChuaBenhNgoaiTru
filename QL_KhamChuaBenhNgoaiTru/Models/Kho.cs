using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class TonKho
    {
        [Key]
        public int MaTonKho { get; set; }
        public int MaKho { get; set; }

        [DisplayName("Mã thuốc")]
        [Required(ErrorMessage = "Mã thuốc không được để trống")]
        [StringLength(10)]
        public string MaThuoc { get; set; }

        [DisplayName("Số lô")]
        [Required(ErrorMessage = "Số lô là bắt buộc")]
        [StringLength(50)]
        public string MaLo { get; set; }

        [DisplayName("Hạn sử dụng")]
        [Required(ErrorMessage = "Hạn sử dụng là bắt buộc")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HanSuDung { get; set; }

        [DisplayName("Ngày sản xuất")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NgaySanXuat { get; set; }

        [DisplayName("Giá nhập")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? GiaNhap { get; set; }

        [DisplayName("Số lượng tồn")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        public int SoLuongTon { get; set; }

        [DisplayName("Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
    }

    public class PhieuNhap
    {
        [Key]
        [DisplayName("Mã phiếu nhập")]
        public int MaPhieuNhap { get; set; }

        [DisplayName("Người lập phiếu")]
        [Required(ErrorMessage = "Người lập phiếu không được để trống")]
        [StringLength(10)]
        public string MaNV_LapPhieu { get; set; }

        [DisplayName("Nhà cung cấp (NSX)")]
        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public int MaNSX { get; set; }

        [DisplayName("Ngày lập")]
        public DateTime? NgayLap { get; set; }

        [DisplayName("Tổng tiền nhập")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? TongTienNhap { get; set; }

        [DisplayName("Trạng thái")]
        [StringLength(50)]
        public string TrangThai { get; set; }

        [DisplayName("Ghi chú")]
        [StringLength(200)]
        public string GhiChu { get; set; }

        [DisplayName("Người duyệt")]
        [StringLength(10)]
        public string MaNV_Duyet { get; set; }

        [DisplayName("Ngày duyệt")]
        public DateTime? NgayDuyet { get; set; }
    }

    public class CT_PhieuNhap
    {
        [Key]
        public int MaCTPN { get; set; }

        [DisplayName("Mã phiếu nhập")]
        [Required]
        public int MaPhieuNhap { get; set; }

        [DisplayName("Mã thuốc")]
        [Required(ErrorMessage = "Mã thuốc là bắt buộc")]
        [StringLength(10)]
        public string MaThuoc { get; set; }

        [DisplayName("Số lô")]
        [Required(ErrorMessage = "Số lô là bắt buộc")]
        [StringLength(50)]
        public string MaLo { get; set; }

        [DisplayName("Ngày sản xuất")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NgaySanXuat { get; set; }

        [DisplayName("Hạn sử dụng")]
        [Required(ErrorMessage = "Hạn sử dụng là bắt buộc")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HanSuDung { get; set; }

        [DisplayName("Số lượng nhập")]
        [Required(ErrorMessage = "Số lượng nhập là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0")]
        public int SoLuongNhap { get; set; }

        [DisplayName("Đơn giá nhập")]
        [Required(ErrorMessage = "Đơn giá nhập là bắt buộc")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal DonGiaNhap { get; set; }

        [DisplayName("Thành tiền")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal ThanhTien { get; set; }
    }
}