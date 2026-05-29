using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class HomeDBTests
    {
        private HomeDB _homeDb;
        private KhoaDB _khoaDb; // Thêm đối tượng KhoaDB để test các hàm của Khoa

        [TestInitialize]
        public void Setup()
        {
            _homeDb = new HomeDB();
            _khoaDb = new KhoaDB(); // Khởi tạo KhoaDB
        }

        // ==========================================
        // MODULE 1: TRANG CHỦ & DỮ LIỆU CHUNG
        // ==========================================

        [TestMethod]
        public void GetHomeData_KhoiTaoModel_KhongBaoGioNull()
        {
            var model = _homeDb.GetHomeData();
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.DanhSachKhoa);
            Assert.IsNotNull(model.DanhSachBacSi);
            Assert.IsNotNull(model.ThongKe);
        }

        [TestMethod]
        public void GetHomeData_GioiHanSoLuong_ToiDa6KhoaVa6BacSi()
        {
            var model = _homeDb.GetHomeData();
            Assert.IsTrue(model.DanhSachKhoa.Count <= 6, "Chỉ được phép hiển thị tối đa 6 Khoa nổi bật.");
            Assert.IsTrue(model.DanhSachBacSi.Count <= 6, "Chỉ được phép hiển thị tối đa 6 Bác sĩ tiêu biểu.");
        }

        [TestMethod]
        public void GetGioiThieuData_CoDuLieuGiamDoc()
        {
            var model = _homeDb.GetGioiThieuData();
            Assert.IsNotNull(model.GiamDoc, "Phải khởi tạo object GiamDoc.");
        }

        // ==========================================
        // MODULE 2: DANH SÁCH BÁC SĨ & LOGIC HÌNH ẢNH
        // ==========================================

        [TestMethod]
        public void GetAllBacSi_LogicAnhFallback_AvatarAoHoatDongTot()
        {
            var list = _homeDb.GetAllBacSi();
            var bsKhongAnh = list.FirstOrDefault(b => b.HinhAnh.Contains("ui-avatars.com"));

            if (bsKhongAnh != null)
            {
                Assert.IsTrue(bsKhongAnh.HinhAnh.Contains(System.Uri.EscapeDataString(bsKhongAnh.HoTen)),
                    "Avatar ảo sinh ra phải chứa tên của Bác sĩ được mã hóa URL.");
            }
        }

        [TestMethod]
        public void GetAllBacSi_ChucVuRong_CoChuoiMacDinh()
        {
            var list = _homeDb.GetAllBacSi();
            Assert.IsTrue(list.All(b => !string.IsNullOrEmpty(b.TenChucVu)), "Tên chức vụ không được để trống trên UI.");
        }

        // ==========================================
        // MODULE 3: BẢNG GIÁ DỊCH VỤ CÔNG KHAI
        // ==========================================

        [TestMethod]
        public void GetMenuLoaiDichVu_TraVeList_KhongNull()
        {
            var menu = _homeDb.GetMenuLoaiDichVu();
            Assert.IsNotNull(menu);
        }

        [TestMethod]
        public void GetBangGiaDichVu_KhongThamSo_LayToanBo()
        {
            var bg = _homeDb.GetBangGiaDichVu("", "");
            Assert.IsNotNull(bg);
        }

        [TestMethod]
        public void GetBangGiaDichVu_TimKiemTuKhoaKhongTonTai_TraVeRong()
        {
            var bg = _homeDb.GetBangGiaDichVu("DICH_VU_SIEU_NHAN_999", "");
            Assert.AreEqual(0, bg.Count, "Không có dịch vụ này, phải trả về List rỗng.");
        }

        [TestMethod]
        public void GetBangGiaDichVu_MaLoaiAo_TraVeRong()
        {
            var bg = _homeDb.GetBangGiaDichVu("", "LOAI_AO_XYZ");
            Assert.AreEqual(0, bg.Count);
        }

        // ==========================================
        // MODULE 4: QUẢN LÝ KHOA & PHÒNG CHUYÊN MÔN (Đã chuyển sang KhoaDB)
        // ==========================================

        [TestMethod]
        public void GetKhoaDetails_MaKhoaAo_TraVeNull()
        {
            // Sửa lại thành gọi _khoaDb vì hàm này nằm trong KhoaDB
            var details = _khoaDb.GetKhoaDetails(-999);
            Assert.IsNull(details, "Mã khoa ảo phải làm hàm trả về NULL để chặn luồng.");
        }

        [TestMethod]
        public void RemovePhongFromKhoa_KiemTraLogicRangBuoc_ChinhXac()
        {
            string errorMsg;
            // Sửa lại thành gọi _khoaDb
            bool result = _khoaDb.RemovePhongFromKhoa(-999, out errorMsg);

            Assert.IsFalse(result, "Phòng không tồn tại thì không thể rút được.");
            Assert.AreEqual("", errorMsg, "Không có nhân viên thì errorMsg phải rỗng.");
        }

        [TestMethod]
        public void AddNhanVienToKhoa_ThucThiAnToanVoiMaAo()
        {
            // Sửa lại thành gọi _khoaDb
            bool result = _khoaDb.AddNhanVienToKhoa("NV_AO", -999);
            Assert.IsFalse(result, "ExecuteNonQuery phải trả về 0 dòng affected.");
        }

        [TestMethod]
        public void RemoveNhanVienFromKhoa_ThucThiAnToanVoiMaAo()
        {
            // Sửa lại thành gọi _khoaDb
            bool result = _khoaDb.RemoveNhanVienFromKhoa("NV_AO_123");
            Assert.IsFalse(result);
        }
    }
}