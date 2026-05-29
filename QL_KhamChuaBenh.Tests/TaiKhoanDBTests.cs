using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using QL_KhamChuaBenhNgoaiTru.Helpers;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class TaiKhoanDBTests
    {
        private TaiKhoanDB _taiKhoanDb;

        [TestInitialize]
        public void Setup()
        {
            // Khởi tạo đối tượng trước mỗi lần chạy test
            _taiKhoanDb = new TaiKhoanDB();
        }

        // Test Case: Đăng nhập sai thông tin trả về null
        [TestMethod]
        public void CheckLogin_SaiThongTin_ReturnsNull()
        {
            // Arrange
            string username = "user_khong_ton_tai_" + System.DateTime.Now.Ticks;
            string password = "wrong_password";

            // Act
            var result = _taiKhoanDb.CheckLogin(username, password);

            // Assert
            Assert.IsNull(result, "Đăng nhập sai tài khoản phải trả về null.");
        }

        // Test Case: Insert tài khoản với Username đã tồn tại phải bị chặn
        [TestMethod]
        public void InsertTaiKhoan_UsernameDaTonTai_ReturnsFalse()
        {
            // Arrange
            var tk = new TaiKhoan
            {
                Username = "admin", // Giả sử 'admin' luôn tồn tại trong DB của bác
                PasswordHash = "123456"
            };

            // Act
            bool result = _taiKhoanDb.InsertTaiKhoan(tk);

            // Assert
            Assert.IsFalse(result, "Hệ thống phải chặn không cho tạo tài khoản trùng Username.");
        }

        // Test Case: Sinh mã Bệnh nhân tự động phải đúng định dạng BNxxxx
        [TestMethod]
        public void InsertTaiKhoanWithEmail_TaoMoi_SinhMaBNDungDinhDang()
        {
            // Arrange
            string uniqueUsername = "testbn_" + System.DateTime.Now.Ticks;
            var tk = new TaiKhoan { Username = uniqueUsername, PasswordHash = "123456" };
            string email = uniqueUsername + "@gmail.com";
            string hoTen = "Bệnh Nhân Test Tự Động";

            // Act
            bool result = _taiKhoanDb.InsertTaiKhoanWithEmail(tk, email, hoTen);

            // Assert
            Assert.IsTrue(result, "Thêm mới tài khoản bệnh nhân hợp lệ phải thành công.");

            // Lấy lại để kiểm tra
            var bn = _taiKhoanDb.GetTaiKhoanByUsernameOrSdt(uniqueUsername);
            Assert.IsNotNull(bn, "Phải tìm thấy tài khoản vừa tạo.");
        }
    }
}