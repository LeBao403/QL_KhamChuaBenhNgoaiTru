using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class ThuNganDBTests
    {
        private ThuNganDB _thuNganDb;
        private string _connectStr;

        private string _testMaBN = "BN_THUNGAN";
        private string _testMaPKB = "PKB_THUNGAN";
        private string _testMaHD = "HD_THUNGAN";
        private string _testMaCTHD = "CTHD_TEST";

        [TestInitialize]
        public void Setup()
        {
            _thuNganDb = new ThuNganDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
            CleanupData();

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();

                // 1. Bệnh nhân
                new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen, BHYT) VALUES ('{_testMaBN}', N'BN Test Thu Ngân', 0)", conn).ExecuteNonQuery();

                // 2. Phiếu khám
                new SqlCommand($"INSERT INTO PHIEUKHAMBENH (MaPhieuKhamBenh, MaBN, NgayLap, TrangThai) VALUES ('{_testMaPKB}', '{_testMaBN}', GETDATE(), N'Chờ thanh toán')", conn).ExecuteNonQuery();

                // 3. Dịch vụ
                try { new SqlCommand("INSERT INTO DICHVU (MaDV, TenDV, GiaDichVu, MaLoaiDV) VALUES ('DV_TEST', N'Dịch vụ Test', 100000, 'L_TEST')", conn).ExecuteNonQuery(); } catch { }

                // 4. Hóa đơn (Chưa thanh toán)
                new SqlCommand($"INSERT INTO HOADON (MaHD, MaBN, MaPhieuKhamBenh, TrangThaiThanhToan, NgayThanhToan) VALUES ('{_testMaHD}', '{_testMaBN}', '{_testMaPKB}', N'Chưa thanh toán', GETDATE())", conn).ExecuteNonQuery();

                // 5. Chi tiết hóa đơn
                new SqlCommand($"INSERT INTO CT_HOADON_DV (MaCTHD, MaHD, MaDV, DonGia, TongTienGoc, TienBenhNhanTra, TrangThaiThanhToan) VALUES ('{_testMaCTHD}', '{_testMaHD}', 'DV_TEST', 100000, 100000, 100000, N'Chưa thanh toán')", conn).ExecuteNonQuery();
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
                try { new SqlCommand($"DELETE FROM CT_HOADON_DV WHERE MaHD = '{_testMaHD}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM HOADON WHERE MaHD = '{_testMaHD}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEUKHAMBENH WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM DICHVU WHERE MaDV = 'DV_TEST'", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // NHÓM 1: LẤY DANH SÁCH & CHI TIẾT
        // ==========================================
        [TestMethod]
        public void GetDanhSachChoThuTien_LuonTraVeTable()
        {
            var dt = _thuNganDb.GetDanhSachChoThuTien();
            Assert.IsNotNull(dt);
            // Có ít nhất 1 hóa đơn rác vừa được ném vào DB
            Assert.IsTrue(dt.Rows.Count > 0);
        }

        [TestMethod]
        public void GetChiTietHoaDon_MaHDAo_TraVeTableRong()
        {
            var dt = _thuNganDb.GetChiTietHoaDon("HD_AO_9999");
            Assert.IsNotNull(dt);
            Assert.AreEqual(0, dt.Rows.Count);
        }

        [TestMethod]
        public void GetChiTietHoaDon_HopLe_TraVeCauTrucCotChuan()
        {
            var dt = _thuNganDb.GetChiTietHoaDon(_testMaHD);
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
            Assert.IsTrue(dt.Columns.Contains("LoaiItem")); // UNION All giữa thuốc và Dịch vụ
        }

        [TestMethod]
        public void GetLichSuThuTien_NgayTuongLai_TraVeBangRong()
        {
            var dt = _thuNganDb.GetLichSuThuTien(DateTime.Now.AddYears(1), DateTime.Now.AddYears(2));
            Assert.AreEqual(0, dt.Rows.Count);
        }

        [TestMethod]
        public void GetPatientInfoByMaHD_MaHDAo_ReturnsNull()
        {
            var info = _thuNganDb.GetPatientInfoByMaHD("HD_AO_999");
            Assert.IsNull(info);
        }

        [TestMethod]
        public void GetPatientInfoByMaHD_MaHDHopLe_ReturnsInfo()
        {
            var info = _thuNganDb.GetPatientInfoByMaHD(_testMaHD);
            Assert.IsNotNull(info);
            Assert.AreEqual("BN Test Thu Ngân", info.Item2);
        }

        // ==========================================
        // NHÓM 2: GIAO DỊCH XÁC NHẬN THU TIỀN (TRANSACTIONS)
        // ==========================================
        [TestMethod]
        public void XacNhanThuTien_ThanhToanToanBo_TrangThaiChuyenThanhCong()
        {
            string msg;
            DataTable dtNow;

            // Truyền ds Thuốc trống, Hủy trống -> Thanh toán toàn bộ
            bool isSuccess = _thuNganDb.XacNhanThuTien(_testMaHD, _testMaPKB, "Tiền mặt", "", "", new List<dynamic>(), out msg, out dtNow);

            Assert.IsTrue(isSuccess, "Thanh toán thất bại: " + msg);

            // Kiểm tra DB
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                var stt = new SqlCommand($"SELECT TrangThaiThanhToan FROM HOADON WHERE MaHD = '{_testMaHD}'", conn).ExecuteScalar()?.ToString();
                Assert.AreEqual("Đã thanh toán", stt);

                var sttPkb = new SqlCommand($"SELECT TrangThai FROM PHIEUKHAMBENH WHERE MaPhieuKhamBenh = '{_testMaPKB}'", conn).ExecuteScalar()?.ToString();
                Assert.AreEqual("Chờ cấp số", sttPkb, "PKB phải được trả về trạng thái Chờ cấp số sau khi thu tiền Dịch vụ Khám đầu vào.");
            }
        }

        [TestMethod]
        public void XacNhanThuTien_HuyTatCaDichVu_TrangThaiChuyenThanhDaHuy()
        {
            string msg;
            DataTable dtNow;

            // Cố tình HỦY cái CTHD dịch vụ đang có
            bool isSuccess = _thuNganDb.XacNhanThuTien(_testMaHD, _testMaPKB, "Tiền mặt", _testMaCTHD, "", new List<dynamic>(), out msg, out dtNow);

            Assert.IsTrue(isSuccess);

            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                var stt = new SqlCommand($"SELECT TrangThaiThanhToan FROM HOADON WHERE MaHD = '{_testMaHD}'", conn).ExecuteScalar()?.ToString();
                Assert.AreEqual("Đã hủy", stt, "Hủy hết sạch dịch vụ trong Hóa Đơn thì Hóa đơn tự động sang Đã hủy.");

                var tienTra = Convert.ToDecimal(new SqlCommand($"SELECT TongTienBenhNhanTra FROM HOADON WHERE MaHD = '{_testMaHD}'", conn).ExecuteScalar());
                Assert.AreEqual(0, tienTra, "Đã hủy hết thì tiền phải bằng 0.");
            }
        }

        [TestMethod]
        public void XacNhanThuTien_MaHDAo_NenQuangExceptionVaRollback()
        {
            string msg;
            DataTable dt;

            // Gửi dữ liệu ảo
            bool isSuccess = _thuNganDb.XacNhanThuTien("HD_AO", "PKB_AO", "Thẻ", "", "", new List<dynamic>(), out msg, out dt);

            // Xử lý logic lỗi phải chặn không cho code tiếp tục (Tùy logic cụ thể, nhưng thường sẽ văng Exception do UPDATE không thấy dòng)
            Assert.IsFalse(isSuccess, "Xác nhận hóa đơn ảo phải trả về False.");
        }
    }
}