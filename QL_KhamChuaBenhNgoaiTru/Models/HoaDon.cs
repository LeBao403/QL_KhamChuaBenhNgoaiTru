using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class HoaDon
    {
        [Key]
        public int MaHD { get; set; }
        public string MaKH { get; set; }
        public int? MaPhieuKhamBenh { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayThanhToan { get; set; }

        public decimal? TongTien { get; set; }
        public string MaNV_ThuNgan { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public string HinhThucThanhToan { get; set; }
        public string GhiChu { get; set; }
    }

    public class ChiTietHoaDon
    {
        [Key]
        public int MaCTHD { get; set; }
        public int MaHD { get; set; }
        public string LoaiKhoanThu { get; set; } // Dịch vụ CLS/Thuốc/Khám bệnh
        public int Ref_ID { get; set; }
        public string TenKhoanThu { get; set; }
        public int? SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}