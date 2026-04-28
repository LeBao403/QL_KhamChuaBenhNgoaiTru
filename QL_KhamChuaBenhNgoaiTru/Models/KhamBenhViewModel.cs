using System;
using System.Collections.Generic;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class KhamBenhViewModel
    {
        // [ĐÃ SỬA] Đổi int thành string
        public string MaPhieuKhamBenh { get; set; }
        public string TrieuChung { get; set; }
        public string KetLuan { get; set; }

        // Cận lâm sàng
        public bool YeuCauCanLamSang { get; set; }

        // Danh sách bệnh (Chẩn đoán)
        public List<string> DanhSachMaBenh { get; set; }

        // Danh sách thuốc (Kê đơn)
        public List<ChiTietDonThuocViewModel> DonThuoc { get; set; }
        public List<ChiDinhCLSViewModel> ChiDinhs { get; set; }

        // THÔNG TIN KHÁM SÀNG LỌC (SINH HIỆU)
        public decimal? ChieuCao { get; set; }
        public decimal? CanNang { get; set; }
        public decimal? NhietDo { get; set; }
        public int? HuyetApTamThu { get; set; }       // Đã sửa
        public int? HuyetApTamTruong { get; set; }    // Đã sửa
        public int? Mach { get; set; }                // Đã đổi từ NhipTim -> Mach
        public int? NhipTho { get; set; }
        public decimal? SpO2 { get; set; }

        // DẶN DÒ VÀ TÁI KHÁM
        public DateTime? NgayTaiKham { get; set; }
        // Thêm chuỗi Ngày tái khám String để đẩy xuống JS dễ dàng
        public string NgayTaiKhamStr { get; set; }
        public string DanDo { get; set; }
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
        public int SoLuong { get; set; }
    }

    // Thêm class này vào trong file KhamBenhViewModel.cs hoặc tạo file mới trong Models
    public class PhieuKhamBenhInfo
    {
        // [ĐÃ SỬA] Đổi int thành string
        public string MaPhieuKhamBenh { get; set; }
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public int STT { get; set; }
        public string LyDoDenKham { get; set; }
        public string TrieuChung { get; set; }
        public string TrangThai { get; set; }
        public string GioiTinh { get; set; }
        public int Tuoi { get; set; }

        // THÔNG TIN KHÁM SÀNG LỌC (SINH HIỆU)
        public decimal? ChieuCao { get; set; }
        public decimal? CanNang { get; set; }
        public decimal? NhietDo { get; set; }
        public int? HuyetApTamThu { get; set; }       // Đã sửa
        public int? HuyetApTamTruong { get; set; }    // Đã sửa
        public int? Mach { get; set; }                // Đã đổi từ NhipTim -> Mach
        public int? NhipTho { get; set; }
        public decimal? SpO2 { get; set; }

        // DẶN DÒ VÀ TÁI KHÁM
        public DateTime? NgayTaiKham { get; set; }
        // Thêm chuỗi Ngày tái khám String để đẩy xuống JS dễ dàng
        public string NgayTaiKhamStr { get; set; }
        public string DanDo { get; set; }
    }

    public class ChiDinhCLSViewModel
    {
        public string MaDV { get; set; }
        public int MaPhong { get; set; }
    }
}