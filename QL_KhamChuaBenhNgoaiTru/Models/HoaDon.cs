using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class HoaDon
    {
        [Key]
        public int MaHD { get; set; }
        public string MaBN { get; set; }
        public int? MaPhieuKhamBenh { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayThanhToan { get; set; }

        public decimal? TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public string HinhThucThanhToan { get; set; }
        public string GhiChu { get; set; }
    }

    public class CT_HoaDon_DV
    {
        [Key]
        public int MaCTHD { get; set; }

        [Required(ErrorMessage = "Mã hóa đơn là bắt buộc")]
        [DisplayName("Mã hóa đơn")]
        public int MaHD { get; set; }

        [DisplayName("Mã dịch vụ")]
        [StringLength(10)]
        public string MaDV { get; set; }

        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [DisplayName("Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal DonGia { get; set; }

        // === PHẦN TÁCH TIỀN BHYT ===
        [Required(ErrorMessage = "Tổng tiền gốc là bắt buộc")]
        [DisplayName("Tổng tiền gốc")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TongTienGoc { get; set; }

        [DisplayName("Quỹ BHYT chi trả")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? TienBHYTChiTra { get; set; } // Để nullable vì có thể là 0 hoặc không có

        [Required(ErrorMessage = "Tiền bệnh nhân trả là bắt buộc")]
        [DisplayName("Bệnh nhân đồng chi trả")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TienBenhNhanTra { get; set; }
        // ============================

        [DisplayName("Thu ngân")]
        [StringLength(10)]
        public string MaNV_ThuNgan { get; set; }
    }

    public class CT_HoaDon_Thuoc
    {
        [Key]
        public int MaCTHD { get; set; }

        [Required(ErrorMessage = "Mã hóa đơn là bắt buộc")]
        [DisplayName("Mã hóa đơn")]
        public int MaHD { get; set; }

        [DisplayName("Mã đơn thuốc")]
        public int? MaDonThuoc { get; set; }

        // Bảng này không có Đơn Giá vì nó là tổng tiền của cả cái Đơn Thuốc

        // === PHẦN TÁCH TIỀN BHYT ===
        [Required(ErrorMessage = "Tổng tiền gốc là bắt buộc")]
        [DisplayName("Tổng tiền thuốc (Gốc)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TongTienGoc { get; set; }

        [DisplayName("Quỹ BHYT chi trả")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? TienBHYTChiTra { get; set; }

        [Required(ErrorMessage = "Tiền bệnh nhân trả là bắt buộc")]
        [DisplayName("Bệnh nhân đồng chi trả")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TienBenhNhanTra { get; set; }
        // ============================

        [DisplayName("Thu ngân")]
        [StringLength(10)]
        public string MaNV_ThuNgan { get; set; }
    }
}