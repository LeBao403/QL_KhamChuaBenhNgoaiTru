    using System;
using System.Collections.Generic;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class KhamBenhViewModel
    {
        public int MaPhieuKhamBenh { get; set; }
        public string TrieuChung { get; set; }
        public string KetLuan { get; set; }

        // Cận lâm sàng
        public bool YeuCauCanLamSang { get; set; }

        // Danh sách bệnh (Chẩn đoán)
        public List<string> DanhSachMaBenh { get; set; }

        // Danh sách thuốc (Kê đơn)
        public List<ChiTietDonThuocViewModel> DonThuoc { get; set; }
        public List<ChiDinhCLSViewModel> ChiDinhs { get; set; }
    }

    public class ChiTietDonThuocViewModel
    {
        public string MaThuoc { get; set; }
        public decimal Sang { get; set; }
        public decimal Trua { get; set; }
        public decimal Chieu { get; set; }
        public decimal Toi { get; set; }
        public int SoNgay { get; set; }
        public string GhiChu { get; set; }
    }
    // Thêm class này vào trong file KhamBenhViewModel.cs hoặc tạo file mới trong Models
    public class PhieuKhamBenhInfo
    {
        public int MaPhieuKhamBenh { get; set; }
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public int STT { get; set; }
        public string LyDoDenKham { get; set; }
        public string TrieuChung { get; set; }
        public string TrangThai { get; set; }
        public string GioiTinh { get; set; }
        public int Tuoi { get; set; }
    }
    public class ChiDinhCLSViewModel
    {
        public string MaDV { get; set; }
        public int MaPhong { get; set; }
    }
}