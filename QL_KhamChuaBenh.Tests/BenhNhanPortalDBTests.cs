using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenh.Tests
{
    [TestClass]
    public class BenhNhanPortalDBTests
    {
        private BenhNhanPortalDB _db;
        private string _connectStr;

        // Đã sửa mã < 10 ký tự để không bị tràn viền (CHAR(10))
        private const string _testMaBN = "PT_TEST";
        private const string _testUsername = "pt_test99";

        [TestInitialize]
        public void Setup()
        {
            _db = new BenhNhanPortalDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData(); // Dọn rác sạch sẽ trước khi bơm mồi

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                // 1. Tạo Tài khoản mồi
                string sqlTk = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@U, 'hash', 1, GETDATE())";
                SqlCommand cmdTk = new SqlCommand(sqlTk, conn);
                cmdTk.Parameters.AddWithValue("@U", _testUsername);
                int maTK = (int)cmdTk.ExecuteScalar();

                // 2. Tạo Bệnh nhân mồi (Khớp CHAR(10))
                string sqlBn = @"INSERT INTO BENHNHAN (MaBN, HoTen, SDT, CCCD, Email, BHYT, MaTK) 
                                 VALUES (@MaBN, N'Bệnh Nhân Portal', '0888888888', '098765432109', 'portal@test.com', 0, @MaTK)";
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
                // Xóa Phiếu Đăng Ký (có liên kết khóa ngoại)
                new SqlCommand($"DELETE FROM PHIEUDANGKY WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery();

                // Xóa Bệnh Nhân
                new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery();

                // Xóa thẳng bằng Username để trị dứt điểm lỗi UNIQUE KEY
                new SqlCommand($"DELETE FROM TAIKHOAN WHERE Username = '{_testUsername}'", conn).ExecuteNonQuery();
            }
        }

        // =========================================================
        // KIỂM THỬ NHÓM: THÔNG TIN CÁ NHÂN
        // =========================================================
        [TestMethod]
        public void GetBenhNhanByMaBN_ShouldReturnProfile()
        {
            var profile = _db.GetBenhNhanByMaBN(_testMaBN);
            Assert.IsNotNull(profile, "Lỗi: Không lấy được Profile!");
            Assert.AreEqual("Bệnh Nhân Portal", profile.HoTen);
            Assert.AreEqual(_testUsername, profile.Username);
        }

        [TestMethod]
        public void UpdateBenhNhan_ShouldSaveNewInfo()
        {
            var profile = _db.GetBenhNhanByMaBN(_testMaBN);
            profile.HoTen = "Portal Cập Nhật";
            profile.DiaChi = "123 Test";
            profile.NgaySinh = new DateTime(1995, 5, 5);

            bool isUpdated = _db.UpdateBenhNhan(profile);

            Assert.IsTrue(isUpdated);
            var checkDb = _db.GetBenhNhanByMaBN(_testMaBN);
            Assert.AreEqual("Portal Cập Nhật", checkDb.HoTen);
            Assert.AreEqual("123 Test", checkDb.DiaChi);
        }

        // =========================================================
        // KIỂM THỬ NHÓM: LỊCH KHÁM & PHÒNG KHÁM
        // =========================================================
        [TestMethod]
        public void GetAllPhongKham_ShouldReturnList()
        {
            var phongList = _db.GetAllPhongKham();
            Assert.IsNotNull(phongList);
            // Có thể = 0 nếu DB bác chưa có phòng nào, nhưng list phải được khởi tạo
        }

        [TestMethod]
        public void DatLichKham_And_HuyLichKham_ShouldWork()
        {
            DateTime ngayKhamTest = DateTime.Now.AddDays(1);
            string tenQuay;

            // 1. Test Đặt lịch
            int maPhieuDK = _db.DatLichKham(_testMaBN, ngayKhamTest, "DV_TEST", "Đau bụng", out tenQuay);
            Assert.IsTrue(maPhieuDK > 0, "Lỗi: Không tạo được Phiếu Đăng Ký!");
            Assert.IsNotNull(tenQuay, "Lỗi: Lấy tên quầy tiếp tân thất bại!");

            // 2. Test GetLichKhamByMaBN
            var dsLichKham = _db.GetLichKhamByMaBN(_testMaBN);
            Assert.IsTrue(dsLichKham.Count > 0);
            Assert.AreEqual("Chờ xử lý", dsLichKham[0].TrangThai);

            // 3. Test Hủy Lịch
            bool isCancel = _db.HuyLichKham(maPhieuDK, _testMaBN);
            Assert.IsTrue(isCancel);
            Assert.AreEqual("Hủy", _db.GetLichKhamByMaBN(_testMaBN)[0].TrangThai);
        }

        // =========================================================
        // KIỂM THỬ NHÓM: LỊCH SỬ, THUỐC, HÓA ĐƠN VÀ DASHBOARD
        // =========================================================
        [TestMethod]
        public void GetTrangThaiKham_ShouldReturnList()
        {
            var result = _db.GetTrangThaiKhamByMaBN(_testMaBN);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetLichSuKham_ShouldReturnList()
        {
            var result = _db.GetLichSuKhamByMaBN(_testMaBN);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetDonThuoc_And_ChiTiet_ShouldReturnData()
        {
            // Chạy kiểm tra xem list đơn thuốc có trả về mảng không bị null không
            var result = _db.GetDonThuocByMaBN(_testMaBN);
            Assert.IsNotNull(result);

            // Test hàm gọi chi tiết (dù mã 0 có thể không ra chi tiết, nhưng đảm bảo code không vỡ)
            var chiTiet = _db.GetChiTietDonThuoc(0);
            Assert.IsNotNull(chiTiet);
            Assert.IsNotNull(chiTiet.ChiTiet);
        }

        [TestMethod]
        public void GetHoaDon_And_ChiTiet_ShouldReturnData()
        {
            var result = _db.GetHoaDonByMaBN(_testMaBN);
            Assert.IsNotNull(result);

            var chiTiet = _db.GetChiTietHoaDon(0);
            Assert.IsNotNull(chiTiet);
        }

        [TestMethod]
        public void GetDashboard_ShouldReturnMetrics()
        {
            _db.DatLichKham(_testMaBN, DateTime.Now.AddDays(2), "DV_TEST", "Test Dashboard", out _);
            var dashboard = _db.GetDashboard(_testMaBN);

            Assert.IsNotNull(dashboard);
            Assert.IsTrue(dashboard.SoLichKhamChoDuyet >= 1);
        }
    }
}