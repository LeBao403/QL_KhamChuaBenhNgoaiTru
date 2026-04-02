using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenh.Tests
{
    [TestClass]
    public class BenhNhanDBTests
    {
        private BenhNhanDB _db;
        private string _connectStr;

        // Hằng số dùng chung
        private const string _testMaBN = "BN_TEST99";
        private const string _testUsername = "testuser99";

        // =========================================================
        // BƯỚC 1: DỌN CỖ (Tạo Data Mồi Full Option)
        // =========================================================
        [TestInitialize]
        public void Setup()
        {
            _db = new BenhNhanDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData();

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                // Bơm tài khoản mồi
                string sqlTk = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@U, 'hash_test', 1, GETDATE())";
                SqlCommand cmdTk = new SqlCommand(sqlTk, conn);
                cmdTk.Parameters.AddWithValue("@U", _testUsername);
                int maTK = (int)cmdTk.ExecuteScalar();

                // Bơm bệnh nhân mồi (Full thông tin để test các hàm Exists)
                string sqlBn = @"INSERT INTO BENHNHAN (MaBN, HoTen, SDT, CCCD, Email, SoTheBHYT, MaTK, BHYT, NgaySinh) 
                 VALUES (@MaBN, N'Bệnh Nhân Mồi', '0999999999', '012345678912', 'test@moi.com', 'BHYT123456', @MaTK, 1, '1990-01-01')";
                SqlCommand cmdBn = new SqlCommand(sqlBn, conn);
                cmdBn.Parameters.AddWithValue("@MaBN", _testMaBN);
                cmdBn.Parameters.AddWithValue("@MaTK", maTK);
                cmdBn.ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            CleanupData();
        }

        private void CleanupData()
        {
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                SqlCommand cmdFind = new SqlCommand("SELECT MaTK FROM BENHNHAN WHERE MaBN = @MaBN", conn);
                cmdFind.Parameters.AddWithValue("@MaBN", _testMaBN);
                object maTkObj = cmdFind.ExecuteScalar();

                new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery();

                if (maTkObj != null && maTkObj != DBNull.Value)
                {
                    new SqlCommand($"DELETE FROM TAIKHOAN WHERE MaTK = {maTkObj}", conn).ExecuteNonQuery();
                }

                // Dọn rác của các test case khác sinh ra
                new SqlCommand("DELETE FROM BENHNHAN WHERE MaBN = 'BN_TEST98'", conn).ExecuteNonQuery();
            }
        }

        // =========================================================
        // BƯỚC 2: XƠI CỖ - KIỂM THỬ TẤT CẢ CÁC HÀM
        // =========================================================

        #region Nhóm hàm Đọc (Read / Search)

        [TestMethod]
        public void GetById_ShouldReturnData_WhenMaBNExists()
        {
            var result = _db.GetById(_testMaBN);
            Assert.IsNotNull(result, "Lỗi: Không lôi được dữ liệu mồi lên!");
            Assert.AreEqual("Bệnh Nhân Mồi", result.HoTen);
            Assert.AreEqual("test@moi.com", result.Email);
        }

        [TestMethod]
        public void GetAll_ShouldReturnPagedData()
        {
            // Act: Test thử lấy trang 1, mỗi trang 5 người
            var result = _db.GetAll(1, 5);

            // Assert: Số lượng trả về không được vượt quá 5
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count <= 5, "Lỗi: Phân trang sai, lấy lên nhiều hơn pageSize yêu cầu!");
        }

        [TestMethod]
        public void GetCount_ShouldReturnGreaterThanZero()
        {
            int count = _db.GetCount();
            Assert.IsTrue(count > 0, "Lỗi: Hàm đếm trả về 0 trong khi rõ ràng đã có dữ liệu mồi!");
        }

        [TestMethod]
        public void Search_ShouldReturnMatchedRecords()
        {
            // Act: Tìm theo tên
            var result = _db.Search("Mồi");
            Assert.IsTrue(result.Count > 0, "Lỗi: Tìm kiếm theo tên không ra kết quả!");
            Assert.IsTrue(result.Any(x => x.BenhNhan.MaBN.Trim() == _testMaBN), "Lỗi: Không tìm thấy bệnh nhân mồi!");

            // Act: Tìm theo SĐT
            var result2 = _db.Search("0999999999");
            Assert.IsTrue(result2.Count > 0, "Lỗi: Tìm kiếm theo SĐT không hoạt động!");
        }

        #endregion

        #region Nhóm hàm Thêm/Sửa/Xóa (Create / Update / Delete)

        [TestMethod]
        public void Create_ShouldInsertSuccessfully()
        {
            BenhNhan newBn = new BenhNhan
            {
                MaBN = "BN_TEST98",
                HoTen = "Bệnh Nhân Mới Tạo",
                BHYT = false,
                NgaySinh = new DateTime(2000, 1, 1) 
            };

            bool isSuccess = _db.Create(newBn, null);
            Assert.IsTrue(isSuccess);
            Assert.IsNotNull(_db.GetById("BN_TEST98"));
        }

        [TestMethod]
        public void Update_ShouldModifyData()
        {
            var bnToUpdate = _db.GetById(_testMaBN);
            bnToUpdate.HoTen = "Đã Sửa Tên";
            bnToUpdate.SDT = "0888888888";

            bool isSuccess = _db.Update(bnToUpdate, null);

            Assert.IsTrue(isSuccess);
            var checkDB = _db.GetById(_testMaBN);
            Assert.AreEqual("Đã Sửa Tên", checkDB.HoTen);
            Assert.AreEqual("0888888888", checkDB.SDT);
        }

        [TestMethod]
        public void Delete_ShouldRemoveFromDB()
        {
            bool isSuccess = _db.Delete(_testMaBN);
            Assert.IsTrue(isSuccess);
            Assert.IsNull(_db.GetById(_testMaBN), "Lỗi: Bản ghi vẫn còn tồn tại sau khi chạy hàm Delete!");
        }

        #endregion

        #region Nhóm hàm Nghiệp vụ (Check Exists / Toggle Status / Gen Mã)

        [TestMethod]
        public void GenerateNextMaBN_ShouldReturnValidFormat()
        {
            string newMaBN = _db.GenerateNextMaBN();

            Assert.IsFalse(string.IsNullOrEmpty(newMaBN), "Lỗi: Hàm sinh mã không trả về kết quả!");
            Assert.IsTrue(newMaBN.StartsWith("BN"), "Lỗi: Mã sinh ra không bắt đầu bằng chữ BN!");
            Assert.IsTrue(newMaBN.Length >= 6, "Lỗi: Định dạng mã chưa đủ độ dài chuẩn (VD: BN0001)!");
        }

        [TestMethod]
        public void ToggleAccountStatus_ShouldChangeStatus()
        {
            // Act: Toggle status của thằng mồi (Ban đầu là 1 -> giờ phải thành 0/false)
            bool? newStatus = _db.ToggleAccountStatus(_testMaBN);

            Assert.IsNotNull(newStatus, "Lỗi: Hàm ToggleAccountStatus không tìm thấy tài khoản!");
            Assert.IsFalse(newStatus.Value, "Lỗi: Trạng thái tài khoản chưa bị khóa (Active vẫn là true)!");
        }

        [TestMethod]
        public void ExistsMethods_ShouldWorkCorrectly()
        {
            // Kiểm tra các trường hợp TỒN TẠI (True)
            Assert.IsTrue(_db.BenhNhanCccdExists("012345678912"), "Lỗi Check CCCD: Đã có trong DB nhưng báo không!");
            Assert.IsTrue(_db.BenhNhanEmailExists("test@moi.com"), "Lỗi Check Email: Đã có trong DB nhưng báo không!");
            Assert.IsTrue(_db.BenhNhanPhoneExists("0999999999"), "Lỗi Check SDT: Đã có trong DB nhưng báo không!");
            Assert.IsTrue(_db.BenhNhanBhytExists("BHYT123456"), "Lỗi Check BHYT: Đã có trong DB nhưng báo không!");
            Assert.IsTrue(_db.UsernameExists(_testUsername), "Lỗi Check Username: Đã có trong DB nhưng báo không!");

            // Kiểm tra các trường hợp KHÔNG TỒN TẠI (False)
            Assert.IsFalse(_db.BenhNhanPhoneExists("0111111111"), "Lỗi Check SDT: Số ảo mà dám báo có!");
            Assert.IsFalse(_db.BenhNhanEmailExists("ao@ao.com"), "Lỗi Check Email: Email ảo mà dám báo có!");

            // Kiểm tra logic Exclude (Bỏ qua chính mình khi sửa thông tin)
            // Ví dụ: Bệnh nhân Mồi đang cập nhật thông tin, check sđt của chính nó thì phải báo False (Hợp lệ)
            Assert.IsFalse(_db.CustomerPhoneExists("0999999999", _testMaBN), "Lỗi Exclude: Không bỏ qua mã KH hiện tại!");
        }

        #endregion
    }
}