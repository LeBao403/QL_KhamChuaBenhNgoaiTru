using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    // ==================== MODELS TỔNG QUAN ====================
    public class DashboardThongKe
    {
        public int TongBenhNhan { get; set; }
        public int TongNhanVien { get; set; }
        public int TongThuoc { get; set; }
        public int TongKhoa { get; set; }
        public int TongPhong { get; set; }

        public int DangKham { get; set; }
        public int HoanThanh { get; set; }
        public int KhamHomNay { get; set; }
        public int KhamTuanNay { get; set; }

        public decimal TongTienGoc { get; set; }
        public decimal TienBHYT { get; set; }
        public decimal TienBenhNhanTra { get; set; }

        public decimal TongDoanhThu => TongTienGoc;
        public decimal DoanhThuThucNhan => TienBenhNhanTra;
    }

    public class DoanhThuNgay
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        public string NgayDisplay => Ngay.ToString("dd/MM");
    }

    public class DoanhThuThang
    {
        public int Thang { get; set; }
        public string TenThang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class PhieuNhapItem
    {
        public int MaPhieuNhap { get; set; }
        public DateTime? NgayLap { get; set; }
        public decimal TongTienNhap { get; set; }
        public string TrangThai { get; set; }
        public string NguoiLap { get; set; }
        public string NhaCungCap { get; set; }
        public string NgayLapDisplay => NgayLap.HasValue ? NgayLap.Value.ToString("dd/MM/yyyy") : "";
    }

    public class BacSiThongKe
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenChucVu { get; set; }
        public int SoLuotKham { get; set; }
    }

    // ==================== MODELS DOANH THU ====================
    public class DoanhThuChiTietViewModel
    {
        public decimal TongGoc { get; set; }
        public decimal ThucThu { get; set; }
        public decimal BHYT { get; set; }
        public int SoLuotThanhToan { get; set; }
        public decimal TrungBinhLuot => SoLuotThanhToan > 0 ? ThucThu / SoLuotThanhToan : 0;

        public List<BieuDoDoanhThu> BieuDoXuHuong { get; set; } = new List<BieuDoDoanhThu>();
        public List<PhieuToThanhToan> BieuDoPhuongThuc { get; set; } = new List<PhieuToThanhToan>();
        public List<PhieuToThanhToan> BieuDoNguonThu { get; set; } = new List<PhieuToThanhToan>();
        public List<TopDichVu> TopDichVu { get; set; } = new List<TopDichVu>();
    }

    public class BieuDoDoanhThu { public string Ngay { get; set; } public decimal SoTien { get; set; } }
    public class PhieuToThanhToan { public string Ten { get; set; } public decimal SoTien { get; set; } }
    public class TopDichVu { public string TenDV { get; set; } public decimal SoTien { get; set; } public int SoLuot { get; set; } }

    // ==================== MODELS BỆNH NHÂN ====================
    public class BenhNhanThongKeViewModel
    {
        public int TongLuotKham { get; set; }
        public double TyLeBHYT { get; set; }
        public int SoDangKyOnline { get; set; }
        public int SoDangKyOffline { get; set; }

        public List<BieuDoCot> LuuLuongTheoGio { get; set; } = new List<BieuDoCot>();
        public List<BieuDoTron> PhieuGioiTinh { get; set; } = new List<BieuDoTron>();
        public List<BieuDoTron> PhieuTuyenKham { get; set; } = new List<BieuDoTron>();
        public List<BieuDoCot> PhanKhucTuoi { get; set; } = new List<BieuDoCot>();
        public List<BieuDoCot> TopBenhLy { get; set; } = new List<BieuDoCot>();
    }

    public class BieuDoCot { public string Nhan { get; set; } public decimal GiaTri { get; set; } }
    public class BieuDoTron { public string Nhan { get; set; } public decimal GiaTri { get; set; } }

    // ==================== MODELS KHO DƯỢC ====================
    public class KhoDuocThongKeViewModel
    {
        public decimal TongGiaTriTon { get; set; }
        public decimal GiaTriNhapThang { get; set; }
        public int ThuocSapHetHan { get; set; }
        public int ThuocSapHetHang { get; set; }

        public List<BieuDoCot> BieuDoNhapKho { get; set; } = new List<BieuDoCot>();
        public List<BieuDoTron> TyTrongDanhMuc { get; set; } = new List<BieuDoTron>();
        public List<BieuDoCot> TopThuocXuat { get; set; } = new List<BieuDoCot>();
        public List<ChiTietThuocHetHan> DanhSachCanhBao { get; set; } = new List<ChiTietThuocHetHan>();
    }

    public class ChiTietThuocHetHan
    {
        public string TenThuoc { get; set; }
        public string MaLo { get; set; }
        public string HanSuDung { get; set; }
        public int SoLuongTon { get; set; }
    }

    public class ThuocSapHetHan
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string MaLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public int SoLuongTon { get; set; }
        public string TenKho { get; set; }
        public int SoNgayConLai => (HanSuDung - DateTime.Now.Date).Days;
        public string HanSuDungDisplay => HanSuDung.ToString("dd/MM/yyyy");
    }

    public class ThuocSapHetHang
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViCoBan { get; set; }
        public int TongTon { get; set; }
        public string TenKho { get; set; }
    }
}