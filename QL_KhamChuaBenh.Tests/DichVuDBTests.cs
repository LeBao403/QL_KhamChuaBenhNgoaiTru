using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class DichVuDBTests
    {
        private DichVuDB _dichVuDb;

        [TestInitialize]
        public void Setup()
        {
            _dichVuDb = new DichVuDB();
        }

        // Test Case: Sinh mã Dịch vụ tự động phải đúng định dạng DVxxx
        [TestMethod]
        public void GenerateNextMaDV_SinhMa_DungDinhDang()
        {
            // Act
            string newMaDV = _dichVuDb.GenerateNextMaDV();

            // Assert
            Assert.IsNotNull(newMaDV);
            Assert.IsTrue(newMaDV.StartsWith("DV"), "Mã dịch vụ phải bắt đầu bằng chữ 'DV'.");
            Assert.IsTrue(newMaDV.Length >= 5, "Mã dịch vụ phải có độ dài từ 5 ký tự trở lên.");
        }

        // Test Case: Kiểm tra thuật toán Phân trang (Pagination)
        [TestMethod]
        public void GetDanhSachDichVu_PhanTrang_TraVeDungSoLuong()
        {
            // Arrange
            int pageIndex = 1;
            int pageSize = 3; // Chỉ lấy tối đa 3 dòng

            // Act
            var result = _dichVuDb.GetDanhSachDichVu(pageIndex, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count <= pageSize, "Số lượng trả về không được vượt quá pageSize đã định.");
        }

        // Test Case: Kiểm tra bộ lọc khoảng giá tiền (Min Price -> Max Price)
        [TestMethod]
        public void GetDanhSachDichVu_LocTheoGia_TraVeKetQuaHopLe()
        {
            // Arrange
            decimal minPrice = 50000;
            decimal maxPrice = 200000;

            // Act
            var result = _dichVuDb.GetDanhSachDichVu(1, 50, "", "", minPrice, maxPrice);

            // Assert
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    Assert.IsTrue(item.GiaDichVu >= minPrice && item.GiaDichVu <= maxPrice,
                        $"Dịch vụ {item.TenDV} có giá {item.GiaDichVu} bị lọt ngoài khoảng lọc.");
                }
            }
        }

        // Test Case: Đếm tổng số bản ghi (Count) phải đồng bộ với hàm Lấy danh sách
        [TestMethod]
        public void GetTotalRecord_CoTuKhoa_DongBoVoiDanhSach()
        {
            // Arrange
            string keyword = "Khám"; // Giả sử từ khóa này phổ biến

            // Act
            int totalRecords = _dichVuDb.GetTotalRecord(keyword);
            var listRecords = _dichVuDb.GetDanhSachDichVu(1, 1000, keyword); // Lấy max để chứa hết

            // Assert
            Assert.AreEqual(totalRecords, listRecords.Count, "Hàm đếm tổng và hàm lấy danh sách trả về số lượng lệch nhau.");
        }

        // Test Case: Ràng buộc khi Xóa dịch vụ không tồn tại
        [TestMethod]
        public void DeleteDichVu_KhongTonTai_TraVeThongBaoLoi()
        {
            // Arrange
            string maDVAo = "DV999999";

            // Act
            string message = _dichVuDb.DeleteDichVu(maDVAo);

            // Assert
            Assert.AreEqual("Không tìm thấy dịch vụ để xóa.", message, "Hệ thống phải báo lỗi khi xóa mã DV không tồn tại.");
        }
    }
}