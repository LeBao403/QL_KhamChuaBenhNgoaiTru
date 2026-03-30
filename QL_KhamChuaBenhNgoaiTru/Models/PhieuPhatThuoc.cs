using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    // =========================================================================
    // 1. CLASS: PHIẾU PHÁT THUỐC TỔNG
    // =========================================================================
    public class PhieuPhatThuoc
    {
        public int MaPhieuPhat { get; set; }
        public int MaDonThuoc { get; set; }

        // Cột này trong DB là NULL được nên kiểu string (mặc định cho phép null) là OK
        public string MaNV_Phat { get; set; }

        // Dùng int? vì trong DB không có NOT NULL (có thể có phiếu phát chưa map hóa đơn)
        public int? MaHD { get; set; }

        // Dùng int? vì Quầy/Phòng có thể để trống
        public int? MaPhong { get; set; }

        // Dùng DateTime? để hứng phòng trường hợp DB trả về NULL
        public DateTime? NgayPhat { get; set; }

        public string TrangThai { get; set; } // Mặc định: 'Hoàn thành' hoặc 'Đã hủy'

        public string GhiChu { get; set; }
    }

    // =========================================================================
    // 2. CLASS: CHI TIẾT PHIẾU PHÁT
    // =========================================================================
    public class CT_PhieuPhat
    {
        public int MaCTPhieuPhat { get; set; }
        public int MaPhieuPhat { get; set; }

        public string MaThuoc { get; set; }
        public string MaLo { get; set; }

        public int SoLuongPhat { get; set; } // Trong DB có Ràng buộc NOT NULL và > 0

        public string GhiChu { get; set; }
    }


    public class DanhSachPhatThuocVM
    {
        public int MaHD { get; set; }
        public int? MaDonThuoc { get; set; }
        public string BenhNhan { get; set; } // Hứng Tên BN từ bảng BENHNHAN
        public string TTThanhToan { get; set; } // Hứng trạng thái từ bảng HOADON
        public string TTPhatThuoc { get; set; } // Hứng trạng thái từ bảng DON_THUOC
        public bool HasDonThuoc { get; set; }
        public string NgayThanhToanStr { get; set; }
    }

    public class ChiTietDonThuocPhatVM
    {
        public string MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongKe { get; set; }
        public int SoLuongDaThanhToan { get; set; }
        public int SoLuongDaPhat { get; set; }
        public int SoLuongCanPhat { get; set; } // Bằng DaThanhToan - DaPhat
        public string CachDung { get; set; }
    }

    public class LichSuPhatThuocVM
    {
        public int MaPhieuPhat { get; set; }
        public DateTime? NgayPhat { get; set; }
        public string NguoiPhat { get; set; }
        public string TenThuoc { get; set; }
        public int SoLuongPhat { get; set; }
    }

    public class ChiTietDonThuocVM
    {
        public string TenThuoc { get; set; }
        public int SoLuongKe { get; set; }
        public int SoLuongMua { get; set; }
        public int SoLuongDaPhat { get; set; }
        public string TrangThai { get; set; }
        public int SoLuongCanPhat { get; set; }

        public string LoDuKien { get; set; }
        public string HSDDuKien { get; set; }
        public int ThucPhat { get; set; } // Hứng số lượng thực tế đã phát (Từ bảng CT_PHIEU_PHAT)
        public string DanhSachLoDaPhat { get; set; } // Hứng chuỗi STUFF chứa các Lô đã phát (VD: "L639 (10v), L842 (5v)")
    }

    public class ThongTinPhatThuocVM
    {
        // Thông tin hành chính & Khám bệnh
        public string TenBN { get; set; }
        public string MaBN { get; set; }
        public string SDT { get; set; }
        public string CCCD { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; }
        public string Email { get; set; }
        public int Tuoi { get; set; }
        public string BacSiKe { get; set; }
        public string NgayKe { get; set; }
        public string KetLuan { get; set; }

        public string DiaChi { get; set; }
        public string BHYT { get; set; }
        public int MaPhieuKhamBenh { get; set; }
        public string TrieuChung { get; set; }
        public string LoiDanBacSi { get; set; }

        public int MaHD { get; set; }
        public int? MaPhieuPhat { get; set; }
        public int MaDonThuoc { get; set; }

        // Danh sách thuốc
        public List<ChiTietDonThuocVM> DanhSachThuoc { get; set; } = new List<ChiTietDonThuocVM>();
    }
}