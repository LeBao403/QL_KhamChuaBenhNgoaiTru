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

        // �� s?a m� < 10 k� t? d? kh�ng b? tr�n vi?n (CHAR(10))
        private const string _testMaBN = "PT_TEST";
        private const string _testUsername = "pt_test99";

        [TestInitialize]
        public void Setup()
        {
            _db = new BenhNhanPortalDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData(); // D?n r�c s?ch s? tru?c khi bom m?i

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                // 1. T?o T�i kho?n m?i
                string sqlTk = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@U, 'hash', 1, GETDATE())";
                SqlCommand cmdTk = new SqlCommand(sqlTk, conn);
                cmdTk.Parameters.AddWithValue("@U", _testUsername);
                int maTK = (int)cmdTk.ExecuteScalar();

                // 2. T?o B?nh nh�n m?i (Kh?p CHAR(10))
                string sqlBn = @"INSERT INTO BENHNHAN (MaBN, HoTen, SDT, CCCD, Email, BHYT, MaTK) 
                                 VALUES (@MaBN, N'B?nh Nh�n Portal', '0888888888', '098765432109', 'portal@test.com', 0, @MaTK)";
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
                // X�a Phi?u �ang K� (c� li�n k?t kh�a ngo?i)
                new SqlCommand($"DELETE FROM PHIEUDANGKY WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery();

                // X�a B?nh Nh�n
                new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery();

                // X�a th?ng b?ng Username d? tr? d?t di?m l?i UNIQUE KEY
                new SqlCommand($"DELETE FROM TAIKHOAN WHERE Username = '{_testUsername}'", conn).ExecuteNonQuery();
            }
        }

        // =========================================================
        // KI?M TH? NH�M: TH�NG TIN C� NH�N
        // =========================================================
        [TestMethod]
        public void GetBenhNhanByMaBN_ShouldReturnProfile()
        {
            var profile = _db.GetBenhNhanByMaBN(_testMaBN);
            Assert.IsNotNull(profile, "L?i: Kh�ng l?y du?c Profile!");
            Assert.AreEqual("B?nh Nh�n Portal", profile.HoTen);
            Assert.AreEqual(_testUsername, profile.Username);
        }

        [TestMethod]
        public void UpdateBenhNhan_ShouldSaveNewInfo()
        {
            var profile = _db.GetBenhNhanByMaBN(_testMaBN);
            profile.HoTen = "Portal C?p Nh?t";
            profile.DiaChi = "123 Test";
            profile.NgaySinh = new DateTime(1995, 5, 5);

            bool isUpdated = _db.UpdateBenhNhan(profile);

            Assert.IsTrue(isUpdated);
            var checkDb = _db.GetBenhNhanByMaBN(_testMaBN);
            Assert.AreEqual("Portal C?p Nh?t", checkDb.HoTen);
            Assert.AreEqual("123 Test", checkDb.DiaChi);
        }

        // =========================================================
        // KI?M TH? NH�M: L?CH KH�M & PH�NG KH�M
        // =========================================================
        [TestMethod]
        public void GetAllPhongKham_ShouldReturnList()
        {
            var phongList = _db.GetAllPhongKham();
            Assert.IsNotNull(phongList);
            // C� th? = 0 n?u DB b�c chua c� ph�ng n�o, nhung list ph?i du?c kh?i t?o
        }

        [TestMethod]
        public void DatLichKham_And_HuyLichKham_ShouldWork()
        {
            DateTime ngayKhamTest = DateTime.Now.AddDays(1);
            string tenQuay;

            // 1. Test �?t l?ch
            string maHD; string maPhieuDK = _db.DatLichKham(_testMaBN, ngayKhamTest, 1, "DV_TEST", "�au b?ng", out tenQuay, out maHD);
            Assert.IsTrue(!string.IsNullOrEmpty(maPhieuDK), "L?i: Kh�ng t?o du?c Phi?u �ang K�!");
            Assert.IsNotNull(tenQuay, "L?i: L?y t�n qu?y ti?p t�n th?t b?i!");

            // 2. Test GetLichKhamByMaBN
            var dsLichKham = _db.GetLichKhamByMaBN(_testMaBN);
            Assert.IsTrue(dsLichKham.Count > 0);
            Assert.AreEqual("Ch? x? l�", dsLichKham[0].TrangThai);

            // 3. Test H?y L?ch
            bool isCancel = _db.HuyLichKham(maPhieuDK, _testMaBN);
            Assert.IsTrue(isCancel);
            Assert.AreEqual("H?y", _db.GetLichKhamByMaBN(_testMaBN)[0].TrangThai);
        }

        // =========================================================
        // KI?M TH? NH�M: L?CH S?, THU?C, H�A �ON V� DASHBOARD
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
            // Ch?y ki?m tra xem list don thu?c c� tr? v? m?ng kh�ng b? null kh�ng
            var result = _db.GetDonThuocByMaBN(_testMaBN);
            Assert.IsNotNull(result);

            // Test h�m g?i chi ti?t (d� m� 0 c� th? kh�ng ra chi ti?t, nhung d?m b?o code kh�ng v?)
            var chiTiet = _db.GetChiTietDonThuoc("0");
            Assert.IsNotNull(chiTiet);
            Assert.IsNotNull(chiTiet.ChiTiet);
        }

        [TestMethod]
        public void GetHoaDon_And_ChiTiet_ShouldReturnData()
        {
            var result = _db.GetHoaDonByMaBN(_testMaBN);
            Assert.IsNotNull(result);

            var chiTiet = _db.GetChiTietHoaDon("0");
            Assert.IsNotNull(chiTiet);
        }

        [TestMethod]
        public void GetDashboard_ShouldReturnMetrics()
        {
            _db.DatLichKham(_testMaBN, DateTime.Now.AddDays(2), 1, "DV_TEST", "Test Dashboard", out _, out _);
            var dashboard = _db.GetDashboard(_testMaBN);

            Assert.IsNotNull(dashboard);
            Assert.IsTrue(dashboard.SoLichKhamChoDuyet >= 1);
        }
    }
}
