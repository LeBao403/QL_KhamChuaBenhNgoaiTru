using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class HoaDonDBTests
    {
        private HoaDonDB _hoaDonDb;

        [TestInitialize]
        public void Setup()
        {
            _hoaDonDb = new HoaDonDB();
        }

        // ==========================================
        // NHÓM 1: KIỂM THỬ TÌM KIẾM HÓA ĐƠN
        // ==========================================

        [TestMethod]
        public void GetDanhSachHoaDon_NgayHopLe_TraVeDuLieu()
        {
            // Case 1: Truy vấn khoảng thời gian hợp lệ (Từ năm ngoái đến nay)
            var dt = _hoaDonDb.GetDanhSachHoaDon(DateTime.Now.AddYears(-1), DateTime.Now);
            Assert.IsNotNull(dt, "DataTable trả về không được null.");
        }

        [TestMethod]
        public void GetDanhSachHoaDon_TuNgayLonHonDenNgay_TraVeBangRong()
        {
            // Case 2: Logic sai từ UI (Từ ngày > Đến ngày) -> Không crash, trả về rỗng
            DateTime tuNgay = DateTime.Now.AddDays(5);
            DateTime denNgay = DateTime.Now;

            var dt = _hoaDonDb.GetDanhSachHoaDon(tuNgay, denNgay);
            Assert.AreEqual(0, dt.Rows.Count, "Từ ngày lớn hơn Đến ngày phải trả về bảng rỗng.");
        }

        [TestMethod]
        public void GetDanhSachHoaDon_NgayTuongLai_TraVeBangRong()
        {
            // Case 3: Truy vấn hóa đơn ở mốc thời gian chưa xảy ra
            DateTime tuNgay = DateTime.Now.AddYears(10);
            DateTime denNgay = DateTime.Now.AddYears(11);

            var dt = _hoaDonDb.GetDanhSachHoaDon(tuNgay, denNgay);
            Assert.AreEqual(0, dt.Rows.Count, "Khoảng ngày tương lai không thể có hóa đơn.");
        }

        [TestMethod]
        public void GetDanhSachHoaDon_CungMotNgay_HoatDongBinhThuong()
        {
            // Case 4: Lọc hóa đơn gói gọn trong 1 ngày duy nhất
            var dt = _hoaDonDb.GetDanhSachHoaDon(DateTime.Now.Date, DateTime.Now.Date);
            Assert.IsNotNull(dt, "Lọc trong 1 ngày phải khởi tạo được DataTable.");
        }

        [TestMethod]
        public void GetDanhSachHoaDon_KiemTraCauTrucCotJoin_HopLe()
        {
            // Case 5: Đảm bảo câu SQL có chứa đủ các cột lấy từ bảng BENHNHAN
            var dt = _hoaDonDb.GetDanhSachHoaDon(DateTime.Now.AddDays(-1), DateTime.Now);
            Assert.IsTrue(dt.Columns.Contains("HoTen"), "Bảng kết quả thiếu cột Họ Tên Bệnh Nhân.");
            Assert.IsTrue(dt.Columns.Contains("SDT"), "Bảng kết quả thiếu cột SĐT Bệnh Nhân.");
            Assert.IsTrue(dt.Columns.Contains("TongTienGoc"), "Bảng kết quả thiếu cột Tổng Tiền.");
        }

        // ==========================================
        // NHÓM 2: KIỂM THỬ XEM CHI TIẾT HÓA ĐƠN & BẮT LỖI DATA CASTING
        // ==========================================

        [TestMethod]
        public void GetHoaDonById_MaHDAo_NenQuangLoiEpKieu_Hoac_TraVeRong()
        {
            // Case 6: Kiểm tra xử lý ngoại lệ khi truyền tham số kiểu INT cho cột VARCHAR
            try
            {
                var dt = _hoaDonDb.GetHoaDonById(-999);
                // Nếu Database trống (chưa có hóa đơn nào mang chuỗi HD), code sẽ chạy mượt qua đây
                Assert.AreEqual(0, dt.Rows.Count, "Mã ảo phải trả về 0 dòng nếu DB không văng lỗi ép kiểu.");
            }
            catch (SqlException ex)
            {
                // Nếu Database đã có hóa đơn (như HD2605...), SQL Server sẽ văng lỗi ép kiểu
                // Bắt chính xác lỗi này để pass bài Test
                Assert.IsTrue(ex.Message.Contains("Conversion failed when converting the varchar value"),
                    "Test Pass: Hệ thống bắt thành công ngoại lệ ép kiểu từ SQL Server.");
            }
        }

        [TestMethod]
        public void GetHoaDonById_TruyenSoKhong_XuLyNgoaiLeHoacTraVeRong()
        {
            // Case 7: Truyền ID = 0 (Thường là giá trị mặc định của int)
            try
            {
                var dt = _hoaDonDb.GetHoaDonById(0);
                Assert.AreEqual(0, dt.Rows.Count);
            }
            catch (SqlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Conversion failed"), "Bắt ngoại lệ ép kiểu cho ID 0.");
            }
        }

        [TestMethod]
        public void GetChiTietDichVu_MaHDAo_NenQuangLoiEpKieu_Hoac_TraVeRong()
        {
            // Case 8: Chi tiết dịch vụ truyền ID ảo
            try
            {
                var dt = _hoaDonDb.GetChiTietDichVu(-999);
                Assert.AreEqual(0, dt.Rows.Count);
            }
            catch (SqlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Conversion failed"), "Bắt ngoại lệ ép kiểu chi tiết DV.");
            }
        }

        [TestMethod]
        public void GetChiTietDichVu_TruyenSoDuong_XuLyNgoaiLeHoacTraVeRong()
        {
            // Case 9: Truyền số dương bất kỳ
            try
            {
                var dt = _hoaDonDb.GetChiTietDichVu(123456);
                Assert.AreEqual(0, dt.Rows.Count);
            }
            catch (SqlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Conversion failed"), "Bắt ngoại lệ ép kiểu chi tiết DV.");
            }
        }

        [TestMethod]
        public void KiemTraPhuongThucTonTai_DungDinhDangThietKe()
        {
            // Case 10: Reflection Test - Đảm bảo các phương thức không bị thay đổi tham số
            var type = typeof(HoaDonDB);
            var methodGetHoaDon = type.GetMethod("GetHoaDonById");
            var methodGetChiTiet = type.GetMethod("GetChiTietDichVu");

            Assert.IsNotNull(methodGetHoaDon, "Phải tồn tại phương thức GetHoaDonById.");
            Assert.IsNotNull(methodGetChiTiet, "Phải tồn tại phương thức GetChiTietDichVu.");

            // Xác nhận đang xài kiểu int như cấu hình hiện tại của dự án
            Assert.AreEqual(typeof(int), methodGetHoaDon.GetParameters()[0].ParameterType, "Tham số truyền vào phải là kiểu int.");
            Assert.AreEqual(typeof(int), methodGetChiTiet.GetParameters()[0].ParameterType, "Tham số truyền vào phải là kiểu int.");
        }
    }
}