using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class PhieuDangKy
    {
        [Key]
        public int MaPhieuDK { get; set; }
        public string MaKH { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayDangKy { get; set; }
        public int? STT { get; set; } // Thêm luôn STT vào Model cho đồng bộ với Database nhé bác
        public string HinhThucDangKy { get; set; } // Online/Offline
        public string TrangThai { get; set; }      // Chờ xử lý/Đã xác nhận...
        public int? MaPhong { get; set; }
    }

    public class PhieuDangKyResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public int STT { get; set; }
        public int MaPhieuDK { get; set; }
        public string TenPhong { get; set; }
    }
}