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

        [DisplayName("Phòng/Kho")]
        [Required]
        public int MaPhong { get; set; }

        [DisplayName("Mã thuốc")]
        [Required]
        public string MaThuoc { get; set; }

        [DisplayName("Số lô")]
        [Required]
        public string MaLo { get; set; }

        [DisplayName("Hạn sử dụng")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime HanSuDung { get; set; }

        [DisplayName("Ngày sản xuất")]
        [DataType(DataType.Date)]
        public DateTime? NgaySanXuat { get; set; }

        [DisplayName("Giá nhập")]
        public decimal? GiaNhap { get; set; }

        [DisplayName("Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [DisplayName("Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }
    }
}