using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class NhanVienDBTests
    {
        private NhanVienDB _nhanVienDb;

        [TestInitialize]
        public void Setup()
        {
            _nhanVienDb = new NhanVienDB();
        }

        // Test Case: Hàm kiểm tra tồn tại Username hoạt động chính xác
        [TestMethod]
        public void UsernameExists_KiemTra_HoatDongDung()
        {
            // Act
            bool existTrue = _nhanVienDb.UsernameExists("admin"); // Cần có user admin trong DB
            bool existFalse = _nhanVienDb.UsernameExists("username_ao_tu_ong_chao");

            // Assert
            Assert.IsTrue(existTrue, "Phải trả về true với username có thật.");
            Assert.IsFalse(existFalse, "Phải trả về false với username không có thật.");
        }

        // Test Case: Sinh mã Nhân viên tự động (GenerateNextMaNV) phải bắt đầu bằng "NV"
        [TestMethod]
        public void GenerateNextMaNV_SinhMa_DungChuoiNV()
        {
            // Act
            string newMaNV = _nhanVienDb.GenerateNextMaNV();

            // Assert
            Assert.IsNotNull(newMaNV);
            Assert.IsTrue(newMaNV.StartsWith("NV"), "Mã nhân viên sinh tự động phải bắt đầu bằng 'NV'.");
            Assert.IsTrue(newMaNV.Length >= 5, "Độ dài mã nhân viên phải từ 5 ký tự trở lên (VD: NV001).");
        }

        // Test Case: Tìm kiếm nhân viên không phân biệt chữ hoa chữ thường
        [TestMethod]
        public void GetAll_TimKiemKhongPhanBietHoaThuong_TraVeKetQua()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string keyword = "A"; // Chữ hoa

            // Act
            var resultsHoa = _nhanVienDb.GetAll(page, pageSize, keyword, "all", null, null);
            var resultsThuong = _nhanVienDb.GetAll(page, pageSize, keyword.ToLower(), "all", null, null);

            // Assert
            Assert.AreEqual(resultsHoa.Count, resultsThuong.Count, "Tìm kiếm 'A' và 'a' phải ra số lượng kết quả như nhau.");
        }
    }
}