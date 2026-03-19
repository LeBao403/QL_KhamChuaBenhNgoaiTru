using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class PhieuKhamSangLoc
    {
        [Key]
        public int MaPhieuKhamSL { get; set; }
        public decimal? ChieuCao { get; set; }
        public decimal? CanNang { get; set; }
        public int? Mach { get; set; }
        public string HuyetAp { get; set; }
        public int? NhipTho { get; set; }
        public string KetLuan { get; set; }
        public string GhiChu { get; set; }
        public string MaBacSiKham { get; set; }
        public int? MaPhong { get; set; }
    }
}