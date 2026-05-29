using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class BenhNhanPortalDBTests
    {
        private BenhNhanPortalDB _db;
        private string _connectStr;

        private const string _testMaBN = "PT_TEST";
        private const string _testUsername = "pt_test99";

        [TestInitialize]
        public void Setup()
        {
            _db = new BenhNhanPortalDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData(); // Dọn sạch trước khi bơm

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                // 1. Tạo Tài khoản mới
                string sqlTk = "INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive, CreatedAt) OUTPUT INSERTED.MaTK VALUES (@U, 'hash', 1, GETDATE())";
                SqlCommand cmdTk = new SqlCommand(sqlTk, conn);
                cmdTk.Parameters.AddWithValue("@U", _testUsername);
                int maTK = (int)cmdTk.ExecuteScalar();

                // 2. Tạo Bệnh nhân mới (Sửa lại font chữ tiếng Việt bị lỗi)
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

        // [SỬA LỖI CHÍ MẠNG Ở ĐÂY] - Phải xóa từ bảng con (CT_HOADON, HOADON) trước rồi mới xóa bảng cha (BENHNHAN)
        private void CleanupData()
        {
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                try { new SqlCommand($"DELETE FROM CT_HOADON_DV WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MaBN = '{_testMaBN}')", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM HOADON WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEUDANGKY WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM TAIKHOAN WHERE Username = '{_testUsername}'", conn).ExecuteNonQuery(); } catch { }
            }
        }

        #region KỊCH BẢN 1: THÔNG TIN CÁ NHÂN (Positive & Negative)

        [TestMethod]
        public void GetBenhNhanByMaBN_DuLieuChuan_ShouldReturnProfile()
        {
            var profile = _db.GetBenhNhanByMaBN(_testMaBN);
            Assert.IsNotNull(profile, "Lỗi: Không lấy được Profile!");
            Assert.AreEqual("Bệnh Nhân Portal", profile.HoTen);
        }

        [TestMethod]
        public void GetBenhNhanByMaBN_MaBNKhongTonTai_ReturnsNull()
        {
            var profile = _db.GetBenhNhanByMaBN("BN_MA_AO_123");
            Assert.IsNull(profile, "Mã bệnh nhân không tồn tại phải trả về null để tránh crash App.");
        }

        [TestMethod]
        public void UpdateBenhNhan_UpdateVoiEmailVaSdtNull_ThucThiThanhCongKhongLoiDB()
        {
            // Test case: Bệnh nhân cố tình xóa sđt và email (cho phép null theo CSDL)
            var profile = _db.GetBenhNhanByMaBN(_testMaBN);
            profile.SDT = null;
            profile.Email = null;

            bool isUpdated = _db.UpdateBenhNhan(profile);

            Assert.IsTrue(isUpdated);
            var checkDb = _db.GetBenhNhanByMaBN(_testMaBN);
            Assert.AreEqual("", checkDb.SDT, "SDT null phải được chuyển thành rỗng hoặc lưu thành công");
        }
        #endregion

        #region KỊCH BẢN 2: LUỒNG ĐẶT LỊCH KHÁM CHỮA BỆNH

        [TestMethod]
        public void DatLichKham_TaoTransaction_ThanhCong()
        {
            DateTime ngayKhamTest = DateTime.Now.AddDays(1);
            string tenQuay;
            string maHD;

            string maPhieuDK = _db.DatLichKham(_testMaBN, ngayKhamTest, 1, "DV001", "Đau bụng", out tenQuay, out maHD);

            Assert.IsTrue(!string.IsNullOrEmpty(maPhieuDK), "Lỗi: Không tạo được Phiếu Đăng Ký!");
            Assert.IsTrue(!string.IsNullOrEmpty(maHD), "Lỗi: Giao dịch không sinh ra được Hóa đơn tiện ích!");
        }

        [TestMethod]
        public void HuyLichKham_LichDangChoXuLy_HuyThanhCong()
        {
            // Arrange
            _db.DatLichKham(_testMaBN, DateTime.Now.AddDays(1), 1, "DV001", "Đau bụng", out _, out _);
            var dsLichKham = _db.GetLichKhamByMaBN(_testMaBN);
            string maPhieuDK = dsLichKham[0].MaPhieuDK;

            // Act
            bool isCancel = _db.HuyLichKham(maPhieuDK, _testMaBN);

            // Assert
            Assert.IsTrue(isCancel, "Phải cho phép hủy lịch đang ở trạng thái Chờ xử lý.");
        }

        [TestMethod]
        public void HuyLichKham_LichDaXacNhan_KhongChoPhepHuy()
        {
            // Arrange
            _db.DatLichKham(_testMaBN, DateTime.Now.AddDays(1), 1, "DV001", "Đau bụng", out _, out _);
            var dsLichKham = _db.GetLichKhamByMaBN(_testMaBN);
            string maPhieuDK = dsLichKham[0].MaPhieuDK;

            // Cố tình dùng SQL chuyển trạng thái sang "Đã xác nhận"
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                new SqlCommand($"UPDATE PHIEUDANGKY SET TrangThai = N'Đã xác nhận' WHERE MaPhieuDK = '{maPhieuDK}'", conn).ExecuteNonQuery();
            }

            // Act
            bool isCancel = _db.HuyLichKham(maPhieuDK, _testMaBN);

            // Assert
            Assert.IsFalse(isCancel, "Nghiệp vụ sai: Không được phép hủy lịch đã được Tiếp tân xác nhận!");
        }

        [TestMethod]
        public void GetKhungGioHopLe_NgayTrongTuongLai_TraVeDanhSach()
        {
            var result = _db.GetKhungGioHopLe(DateTime.Now.AddDays(1));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0, "Phải load được khung giờ từ danh mục.");
        }
        #endregion

        #region KỊCH BẢN 3: LỊCH SỬ KHÁM & HỒ SƠ BỆNH ÁN

        [TestMethod]
        public void GetChiTietDonThuoc_MaDonThuocAo_TraVeListRongKhongCrash()
        {
            // Rất nhiều dev dính lỗi Null Reference ở đây, phải test
            var chiTiet = _db.GetChiTietDonThuoc("DT_KHONG_TON_TAI_123");
            Assert.IsNotNull(chiTiet, "Đối tượng trả về không được phép null");
            Assert.IsNotNull(chiTiet.ChiTiet, "Danh sách chi tiết bên trong không được null (phải là list rỗng)");
            Assert.AreEqual(0, chiTiet.ChiTiet.Count);
        }

        [TestMethod]
        public void GetChiTietHoaDon_MaHoaDonAo_TraVeObjectRong()
        {
            var chiTiet = _db.GetChiTietHoaDon("HD_GIA_999");
            Assert.IsNotNull(chiTiet);
            Assert.IsNull(chiTiet.NgayThanhToan, "Không tìm thấy hóa đơn thì các trường phải rỗng/null");
        }
        #endregion
    }
}