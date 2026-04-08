using System;
using System.ComponentModel.DataAnnotations;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class BienLaiThuTien
    {
        [Key]
        public int MaBienLai { get; set; }
        public int MaPhieuKhamBenh { get; set; }
        public int LoaiBienLai { get; set; } // 1: Phí Khám, 2: Phí Dịch Vụ CLS, 3: Phí Thuốc
        public decimal TongTien { get; set; }
        public DateTime? NgayThu { get; set; }
        public string TrangThai { get; set; }
        public string MaNV_ThuNgan { get; set; }
    }

    public class ThanhToanViewModel
    {
        public int MaPhieuKhamBenh { get; set; }
        public string TenBenhNhan { get; set; }
        public string TrangThai { get; set; }

        public decimal PhiKham { get; set; }
        public bool DaThuPhiKham { get; set; }

        public decimal PhiDichVuCLS { get; set; }
        public bool DaThuPhiDichVuCLS { get; set; }

        public decimal PhiThuoc { get; set; }
        public bool DaThuPhiThuoc { get; set; }

        public decimal TongTienCanThu
        {
            get
            {
                decimal tong = 0;
                if (!DaThuPhiKham) tong += PhiKham;
                if (!DaThuPhiDichVuCLS) tong += PhiDichVuCLS;
                if (!DaThuPhiThuoc) tong += PhiThuoc;
                return tong;
            }
        }
    }
}
