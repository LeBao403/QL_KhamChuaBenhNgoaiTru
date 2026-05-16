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
    public class CLSDBTests
    {
        private CLSDB _db;
        private string _connectStr;

        // Mã m?i gi? d?nh
        private const string _testMaBN = "BN_CLS99";
        private const string _testMaDV = "DV_CLS99";
        private const string _testMaLoaiDV = "L_TEST99";
        private const string _testMaBS = "BS_TEST99";

        // Luu l?i ID t? tang d? d?n d?p
        private string _testMaPKB;
        private string _testMaPCD;
        private string _testMaKetQua;

        // =========================================================
        // BU?C 1: D?N C? (T?o chu?i Data m?i liên hoàn)
        // =========================================================
        [TestInitialize]
        public void Setup()
        {
            _db = new CLSDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData(); // D?n rác d? phòng ch?y l?i l?n tru?c

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();

                // 1. T?o Bác si m?i (Tránh l?i FK khi c?p nh?t k?t qu?)
                new SqlCommand($"INSERT INTO NHANVIEN (MaNV, HoTen) VALUES ('{_testMaBS}', N'Bác Si Test')", conn).ExecuteNonQuery();

                // 2. T?o B?nh nhân m?i
                new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen, BHYT) VALUES ('{_testMaBN}', N'B?nh Nhân CLS', 0)", conn).ExecuteNonQuery();

                // 3. T?o Lo?i D?ch V? m?i
                try { new SqlCommand($"INSERT INTO LOAI_DICHVU (MaLoaiDV, TenLoaiDV) VALUES ('{_testMaLoaiDV}', N'Lo?i Test')", conn).ExecuteNonQuery(); } catch { }

                // 4. T?o D?ch v? m?i (B?t bu?c có MaLoaiDV)
                new SqlCommand($"INSERT INTO DICHVU (MaDV, TenDV, GiaDichVu, TrangThai, MaLoaiDV) VALUES ('{_testMaDV}', N'Siêu âm Test', 100000, 1, '{_testMaLoaiDV}')", conn).ExecuteNonQuery();

                // 5. T?o Phi?u Khám B?nh (Tr?ng thái: Ch? c?n lâm sàng)
                string sqlPKB = @"INSERT INTO PHIEUKHAMBENH (MaBN, NgayLap, TrangThai) 
                                  OUTPUT INSERTED.MaPhieuKhamBenh 
                                  VALUES (@MaBN, GETDATE(), N'Ch? c?n lâm sàng')";
                SqlCommand cmdPKB = new SqlCommand(sqlPKB, conn);
                cmdPKB.Parameters.AddWithValue("@MaBN", _testMaBN);
                _testMaPKB = cmdPKB.ExecuteScalar().ToString();

                // 6. T?o Phi?u Ch? Ð?nh
                string sqlPCD = @"INSERT INTO PHIEU_CHIDINH (MaPhieuKhamBenh, NgayChiDinh, TrangThai, TongTien) 
                                  OUTPUT INSERTED.MaPhieuChiDinh 
                                  VALUES (@MaPKB, GETDATE(), N'Ðã thanh toán', 100000)";
                SqlCommand cmdPCD = new SqlCommand(sqlPCD, conn);
                cmdPCD.Parameters.AddWithValue("@MaPKB", _testMaPKB);
                _testMaPCD = cmdPCD.ExecuteScalar().ToString();

                // 7. T?o Chi Ti?t Ch? Ð?nh (Dòng k?t qu? m?c tiêu)
                string sqlCT = @"INSERT INTO CHITIET_CHIDINH (MaPhieuChiDinh, MaDV, DonGia, TrangThai) 
                                 OUTPUT INSERTED.MaCTChiDinh 
                                 VALUES (@MaPCD, @MaDV, 100000, N'Chua th?c hi?n')";
                SqlCommand cmdCT = new SqlCommand(sqlCT, conn);
                cmdCT.Parameters.AddWithValue("@MaPCD", _testMaPCD);
                cmdCT.Parameters.AddWithValue("@MaDV", _testMaDV);
                _testMaKetQua = cmdCT.ExecuteScalar().ToString();
            }
        }

        // =========================================================
        // BU?C 3: R?A BÁT (Xóa ngu?c t? trong ra ngoài)
        // =========================================================
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
                // Xóa theo th? t? ngu?c l?i d? không dính ràng bu?c khóa ngo?i
                try { new SqlCommand($"DELETE FROM CHITIET_CHIDINH WHERE MaDV = '{_testMaDV}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEU_CHIDINH WHERE MaPhieuKhamBenh IN (SELECT MaPhieuKhamBenh FROM PHIEUKHAMBENH WHERE MaBN = '{_testMaBN}')", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEUKHAMBENH WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM DICHVU WHERE MaDV = '{_testMaDV}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM LOAI_DICHVU WHERE MaLoaiDV = '{_testMaLoaiDV}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM NHANVIEN WHERE MaNV = '{_testMaBS}'", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // =========================================================
        // BU?C 2: XOI C? - CÁC K?CH B?N KI?M TH?
        // =========================================================

        [TestMethod]
        public void GetDanhSachChoThucHien_ShouldReturnPendingItems()
        {
            var ds = _db.GetDanhSachChoThucHien("");
            Assert.IsNotNull(ds);

            var itemMoi = ds.FirstOrDefault(x => x.MaKetQua == _testMaKetQua);
            Assert.IsNotNull(itemMoi, "L?i: Không tìm th?y ca CLS m?i trong hàng d?i!");
            Assert.AreEqual("B?nh Nhân CLS", itemMoi.TenBenhNhan);
        }

        [TestMethod]
        public void GetThongTinChiTietCLS_ShouldReturnDynamicObject()
        {
            // S? d?ng dynamic và ép ki?u tu?ng minh d? tránh l?i RuntimeBinder
            dynamic chiTiet = _db.GetThongTinChiTietCLS(_testMaKetQua);

            Assert.IsNotNull(chiTiet);
            string tenBenhNhan = (string)chiTiet.TenBenhNhan;
            Assert.AreEqual("B?nh Nhân CLS", tenBenhNhan);
        }

        [TestMethod]
        public void GetKetQuaById_ShouldReturnData_And_TrimChar()
        {
            var kq = _db.GetKetQuaById(_testMaKetQua);

            Assert.IsNotNull(kq);
            // .Trim() d? x? lý ki?u d? li?u CHAR(10) trong SQL
            Assert.AreEqual(_testMaDV, kq.MaDV.Trim());
            Assert.AreEqual("Chua th?c hi?n", kq.TrangThai);
        }

        [TestMethod]
        public void GetKetQuaById_ShouldReturnNull_WhenIdIsInvalid()
        {
            var kq = _db.GetKetQuaById("-1");
            Assert.IsNull(kq);
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_FullParams_ShouldUpdateSuccessfully()
        {
            string kqNoiDung = "Ket qua xet nghiem on dinh";
            bool isUpdated = _db.CapNhatKetQuaTuLIS(_testMaKetQua, kqNoiDung, _testMaPKB, _testMaBS, "test.jpg", "Mau", "Tot");

            Assert.IsTrue(isUpdated);
            var check = _db.GetKetQuaById(_testMaKetQua);
            Assert.AreEqual("Ðã có k?t qu?", check.TrangThai);
            Assert.AreEqual(kqNoiDung, check.NoiDungKetQua);
            Assert.AreEqual(_testMaBS, check.MaBacSiThucHien.Trim());
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_ShortParams_ShouldUpdateSuccessfully()
        {
            // Test hàm n?p ch?ng (overload) 3 tham s?
            bool isUpdated = _db.CapNhatKetQuaTuLIS(_testMaKetQua, "KQ Rut Gon", _testMaPKB);

            Assert.IsTrue(isUpdated);
            var check = _db.GetKetQuaById(_testMaKetQua);
            Assert.AreEqual("Ðã có k?t qu?", check.TrangThai);
        }

        [TestMethod]
        public void GetLichSuXetNghiem_ShouldReturnData()
        {
            // Arrange: Ðua tr?ng thái v? "Ðã có k?t qu?" m?i lôi lên l?ch s? du?c
            _db.CapNhatKetQuaTuLIS(_testMaKetQua, "Test History", _testMaPKB);

            // Act
            var ds = _db.GetLichSuXetNghiem(10);

            // Assert
            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Any(x => x.MaKetQua == _testMaKetQua));
        }
    }
}
