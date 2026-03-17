using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class DonThuoc
    {
        [Key]
        public int MaDonThuoc { get; set; }
        public int MaPhieuKhamBenh { get; set; }
        public DateTime? NgayKe { get; set; }
        public string LoiDanBS { get; set; }
        public string TrangThai { get; set; }
    }

    public class ChiTietDonThuoc
    {
        [Key]
        public int MaCTDonThuoc { get; set; }

        [Required]
        public int MaDonThuoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thuốc")]
        [StringLength(10)]
        public string MaThuoc { get; set; }

        // === BỘ CỘT LIỀU DÙNG CHI TIẾT ===
        [DisplayName("Sáng")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? SoLuongSang { get; set; }

        [DisplayName("Trưa")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? SoLuongTrua { get; set; }

        [DisplayName("Chiều")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? SoLuongChieu { get; set; }

        [DisplayName("Tối")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? SoLuongToi { get; set; }

        [DisplayName("Số ngày dùng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số ngày dùng phải lớn hơn 0")]
        public int? SoNgayDung { get; set; }

        [DisplayName("Tổng số lượng")]
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [DisplayName("Đơn vị tính")]
        [StringLength(20)]
        public string DonViTinh { get; set; }

        [DisplayName("Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? DonGia { get; set; }

        [DisplayName("Ghi chú")]
        [StringLength(200, ErrorMessage = "Ghi chú không được dài quá 200 ký tự")]
        public string GhiChu { get; set; }

    }
}