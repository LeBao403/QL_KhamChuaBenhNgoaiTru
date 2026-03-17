using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class PhieuKhamBenh
    {
        [Key]
        public int MaPhieuKhamBenh { get; set; }
        public int? MaPhieuDK { get; set; }
        public string MaBN { get; set; }
        public int? STT { get; set; }
        public string LyDoDenKham { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayLap { get; set; }

        public string TrangThai { get; set; } // Đang khám/Hoàn thành...
        public string TrieuChung { get; set; }
        public string KetLuan { get; set; }
        public string MaBacSiKham { get; set; }
        public int? MaPhong { get; set; }
    }

    public class ChiTietChanDoan
    {
        [Key]
        public int MaCTChanDoan { get; set; }
        public int MaPhieuKhamBenh { get; set; }
        public string MaBenh { get; set; }
        public string LoaiBenh { get; set; } // Bệnh chính/Bệnh kèm theo
        public string KetLuanChiTiet { get; set; }
        public bool GiaiDoan { get; set; }
    }


    /* Dich vu can lam sang*/
    public class PhieuChiDinh
    {
        [Key]
        public int MaPhieuChiDinh { get; set; }
        public int MaPhieuKhamBenh { get; set; }
        public string MaBacSiChiDinh { get; set; }
        public DateTime? NgayChiDinh { get; set; }
        public string TrangThai { get; set; }
        public decimal? TongTien { get; set; }
        public int? MaPhong { get; set; }
    }

    public class ChiTietChiDinh
    {
        [Key]
        public int MaCTChiDinh { get; set; }
        public int MaPhieuChiDinh { get; set; }
        public string MaDV { get; set; }
        public decimal? DonGia { get; set; }
        public string MaBacSiThucHien { get; set; }
        public string KetQua { get; set; }
        public string FileKetQua { get; set; }

        // Trong SQL là TIME, trong C# dùng TimeSpan hoặc DateTime đều được
        // Ở đây nên dùng TimeSpan cho đúng chuẩn TIME SQL
        public TimeSpan? ThoiGianCoKetQua { get; set; }

        public string TrangThai { get; set; }
    }
}