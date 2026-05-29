using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class DashboardDBTests
    {
        private DashboardDB _dashboardDb;

        [TestInitialize]
        public void Setup()
        {
            _dashboardDb = new DashboardDB();
        }

        // Test Case: Thống kê tổng quan không được phép trả về đối tượng null hoặc số âm
        [TestMethod]
        public void GetThongKeTongQuan_LuonTraVeSoGiaTriDuong()
        {
            // Act
            var tk = _dashboardDb.GetThongKeTongQuan();

            // Assert
            Assert.IsNotNull(tk, "Đối tượng thống kê không được null.");
            Assert.IsTrue(tk.TongBenhNhan >= 0);
            Assert.IsTrue(tk.TongNhanVien >= 0);
            Assert.IsTrue(tk.TongThuoc >= 0);
            Assert.IsTrue(tk.TongKhoa >= 0);
            Assert.IsTrue(tk.TongPhong >= 0);
        }

        // Test Case: Kiểm tra logic các Group By theo ngày, tuần, tháng, năm không bị lỗi SQL Syntax
        [TestMethod]
        public void GetPhanTichDoanhThu_DieuKienGroupBy_KhongGayLoiSQL()
        {
            // Arrange
            DateTime tuNgay = DateTime.Now.AddDays(-30);
            DateTime denNgay = DateTime.Now;

            // Act & Assert (Nếu SQL lỗi, các lệnh này sẽ quăng Exception và test sẽ Fail)
            var resultDay = _dashboardDb.GetPhanTichDoanhThu(tuNgay, denNgay, "day");
            var resultWeek = _dashboardDb.GetPhanTichDoanhThu(tuNgay, denNgay, "week");
            var resultMonth = _dashboardDb.GetPhanTichDoanhThu(tuNgay, denNgay, "month");
            var resultYear = _dashboardDb.GetPhanTichDoanhThu(tuNgay, denNgay, "year");

            Assert.IsNotNull(resultDay.BieuDoXuHuong);
            Assert.IsNotNull(resultWeek.BieuDoXuHuong);
            Assert.IsNotNull(resultMonth.BieuDoXuHuong);
            Assert.IsNotNull(resultYear.BieuDoXuHuong);
        }

        // Test Case: Tính doanh thu với tham số có và không có Tháng/Năm
        [TestMethod]
        public void GetDoanhThu_CoVaKhongThamSo_VanHoatDongTot()
        {
            // Act
            var dtHienTai = _dashboardDb.GetDoanhThu(); // Khong truyền -> lấy tháng hiện tại
            var dtQuaKhu = _dashboardDb.GetDoanhThu(1, 2020); // Truyền cố định

            // Assert
            Assert.IsNotNull(dtHienTai, "Doanh thu mặc định bị lỗi.");
            Assert.IsNotNull(dtQuaKhu, "Doanh thu truyền tham số bị lỗi.");
        }

        // Test Case: Thống kê Thuốc sắp hết hàng với cấu hình số lượng tồn kho Min tùy chỉnh
        [TestMethod]
        public void GetThuocSapHetHang_TruyenSoLuongMin_TraVeDanhSachChuan()
        {
            // Arrange
            int minTonKho = 50; // Set mức cảnh báo khá cao để dễ có dữ liệu

            // Act
            var result = _dashboardDb.GetThuocSapHetHang(minTonKho);

            // Assert
            Assert.IsNotNull(result);
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    Assert.IsTrue(item.TongTon <= minTonKho,
                        $"Lỗi: Thuốc {item.TenThuoc} có tồn kho {item.TongTon} nhưng lại bị liệt vào danh sách sắp hết (min = {minTonKho}).");
                }
            }
        }

        // Test Case: Cảnh báo thuốc hết hạn trong khoảng ngày tùy chỉnh
        [TestMethod]
        public void GetThuocSapHetHan_NgayTuyChinh_ThucThiThanhCong()
        {
            // Arrange
            int canhBaoTruocNgay = 120; // 4 tháng

            // Act
            var result = _dashboardDb.GetThuocSapHetHan(canhBaoTruocNgay);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}