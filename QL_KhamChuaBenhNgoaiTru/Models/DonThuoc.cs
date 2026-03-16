using System;
using System.Collections.Generic;
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
        public int MaDonThuoc { get; set; }
        public string MaThuoc { get; set; }
        public int SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public string CachDung { get; set; }
        public string GhiChu { get; set; }
    }
}