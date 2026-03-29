using System;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class KetQuaCLS
    {
        public int MaKetQua { get; set; }
        public int MaPhieuKhamBenh { get; set; }
        public string MaDV { get; set; }
        public string TrangThai { get; set; }
        public string NoiDungKetQua { get; set; }
        public DateTime? NgayThucHien { get; set; }
        
        // Thêm các thuộc tính phục vụ việc join hiển thị
        public string TenBenhNhan { get; set; }
        public string TenDichVu { get; set; }
    }
}
