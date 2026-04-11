using System;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class KetQuaCLS
    {
        public string MaKetQua { get; set; }
        public string MaPhieuChiDinh { get; set; }
        public string MaPhieuKhamBenh { get; set; }
        public string MaDV { get; set; }
        public string TrangThai { get; set; }
        public string NoiDungKetQua { get; set; }
        public string FileKetQua { get; set; }
        public string MaBacSiThucHien { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string MauXetNghiem { get; set; }
        public string ChatLuongMau { get; set; }

        public string TenBenhNhan { get; set; }
        public string TenDichVu { get; set; }
    }
}