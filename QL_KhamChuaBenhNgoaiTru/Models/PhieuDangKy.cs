using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class DanhMucKhungGio
    {
        [Key]
        public int MaKhungGio { get; set; }
        public string TenKhungGio { get; set; }
        public int GioiHanSoNguoi { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class PhieuDangKy
    {
        [Key]
        public int MaPhieuDK { get; set; }
        public string MaKH { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayDangKy { get; set; }
        public int? STT { get; set; }
        public string HinhThucDangKy { get; set; }
        public string TrangThai { get; set; }
        public int? MaPhong { get; set; }
        public int? MaKhungGio { get; set; }
    }

    public class PhieuDangKyResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public int STT { get; set; }
        public string MaPhieuDK { get; set; }
        public string TenPhong { get; set; }
        public string TenKhungGio { get; set; }
    }
}