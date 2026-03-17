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

        public string HinhThucDangKy { get; set; } // Online/Offline
        public string TrangThai { get; set; }      // Chờ xử lý/Đã xác nhận...
    }
}