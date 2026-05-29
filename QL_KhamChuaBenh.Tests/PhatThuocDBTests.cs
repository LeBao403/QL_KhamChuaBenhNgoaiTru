using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class PhatThuocDBTests
    {
        private PhatThuocDB _phatThuocDb;
        private string _connectStr;

        // Khai báo sẵn các ID ảo (VARCHAR) để không bị lỗi NULL khóa chính
        private string _testMaBN = "BN_PHAT_99";
        private string _testMaPKB = "PKB_TEST_99";
        private string _testMaHD = "HD_TEST_99";
        private string _testMaDonThuoc = "DT_TEST_99";
        private string _testMaCTDonThuoc = "CTDT_TEST_99";
        private string _testMaThuoc = "TH_TEST_99";
        private string _testMaPhieuPhat = "PPT_TEST_99";
        private int _testMaPhong;

        [TestInitialize]
        public void Setup()
        {
            _phatThuocDb = new PhatThuocDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
            CleanupData();

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();

                // 1. Tạo Phòng Test (Phòng vẫn là IDENTITY int nên dùng OUTPUT INSERTED)
                _testMaPhong = (int)new SqlCommand("INSERT INTO PHONG (TenPhong, TrangThai) OUTPUT INSERTED.MaPhong VALUES (N'Quầy Thuốc Test', 1)", conn).ExecuteScalar();

                // 2. Tạo Bệnh nhân
                new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen, BHYT) VALUES ('{_testMaBN}', N'BN Test Phát Thuốc', 0)", conn).ExecuteNonQuery();

                // 3. Tạo Thuốc & Tồn kho (Dư dả để test)
                try { new SqlCommand($"INSERT INTO THUOC (MaThuoc, TenThuoc, DonViCoBan) VALUES ('{_testMaThuoc}', N'Thuốc Test', N'Viên')", conn).ExecuteNonQuery(); } catch { }
                new SqlCommand($"INSERT INTO TONKHO (MaThuoc, MaLo, HanSuDung, SoLuongTon, GiaNhap) VALUES ('{_testMaThuoc}', 'LO_TEST', DATEADD(year, 1, GETDATE()), 1000, 5000)", conn).ExecuteNonQuery();

                // 4. Tạo Phiếu Khám (Đã truyền cứng MaPhieuKhamBenh)
                new SqlCommand($"INSERT INTO PHIEUKHAMBENH (MaPhieuKhamBenh, MaBN, NgayLap, TrangThai) VALUES ('{_testMaPKB}', '{_testMaBN}', GETDATE(), N'Đang khám')", conn).ExecuteNonQuery();

                // 5. Tạo Đơn Thuốc & Chi Tiết (Bác sĩ kê 10 viên)
                new SqlCommand($"INSERT INTO DON_THUOC (MaDonThuoc, MaPhieuKhamBenh, TrangThai) VALUES ('{_testMaDonThuoc}', '{_testMaPKB}', N'Chưa phát')", conn).ExecuteNonQuery();
                new SqlCommand($"INSERT INTO CT_DON_THUOC (MaCTDonThuoc, MaDonThuoc, MaThuoc, SoLuong, SoLuongDaPhat) VALUES ('{_testMaCTDonThuoc}', '{_testMaDonThuoc}', '{_testMaThuoc}', 10, 0)", conn).ExecuteNonQuery();

                // 6. Tạo Hóa Đơn & CT Hóa Đơn Thuốc (ĐÃ THANH TOÁN)
                new SqlCommand($"INSERT INTO HOADON (MaHD, MaBN, MaPhieuKhamBenh, TrangThaiThanhToan, NgayThanhToan) VALUES ('{_testMaHD}', '{_testMaBN}', '{_testMaPKB}', N'Đã thanh toán', GETDATE())", conn).ExecuteNonQuery();
                new SqlCommand($"INSERT INTO CT_HOADON_THUOC (MaCTHD, MaHD, MaCTDonThuoc, SoLuong, TongTienGoc, TienBenhNhanTra, TrangThaiThanhToan) VALUES ('CTHD_TEST_99', '{_testMaHD}', '{_testMaCTDonThuoc}', 10, 50000, 50000, N'Đã thanh toán')", conn).ExecuteNonQuery();

                // 7. Tạo sẵn Phiếu Phát Thuốc nháp (Được đẩy từ Thu Ngân sang)
                new SqlCommand($"INSERT INTO PHIEU_PHAT_THUOC (MaPhieuPhat, MaDonThuoc, MaHD, TrangThai, MaPhong) VALUES ('{_testMaPhieuPhat}', '{_testMaDonThuoc}', '{_testMaHD}', N'Chờ duyệt', {_testMaPhong})", conn).ExecuteNonQuery();
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
                try { new SqlCommand($"DELETE FROM CT_PHIEU_PHAT WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEU_PHAT_THUOC WHERE MaHD = '{_testMaHD}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM CT_HOADON_THUOC WHERE MaHD = '{_testMaHD}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM HOADON WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM CT_DON_THUOC WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM DON_THUOC WHERE MaPhieuKhamBenh = '{_testMaPKB}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEUKHAMBENH WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM TONKHO WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM THUOC WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHONG WHERE TenPhong = N'Quầy Thuốc Test'", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // MODULE 1: ĐỌC VÀ LỌC DANH SÁCH CHỜ PHÁT
        // ==========================================
        [TestMethod]
        public void GetDanhSachChoPhat_KhongChuaDonThuocHuy_HoacDaPhat()
        {
            var ds = _phatThuocDb.GetDanhSachChoPhat(_testMaPhong, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), "", "NgayThanhToan", "DESC");
            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Count > 0, "Dữ liệu mồi đã tạo Phiếu phát thuốc, danh sách phải có ít nhất 1 dòng.");
        }

        [TestMethod]
        public void GetChiTietDonThuocVaThongTin_ValidIds_ReturnsDetails()
        {
            var detail = _phatThuocDb.GetChiTietDonThuocVaThongTin(_testMaDonThuoc, _testMaHD);
            Assert.IsNotNull(detail);
            Assert.AreEqual("BN Test Phát Thuốc", detail.TenBN);
            Assert.AreEqual(1, detail.DanhSachThuoc.Count);
            Assert.AreEqual(10, detail.DanhSachThuoc[0].SoLuongKe);
        }

        [TestMethod]
        public void GetChiTietDonThuocVaThongTin_InvalidIds_ReturnsEmpty()
        {
            var detail = _phatThuocDb.GetChiTietDonThuocVaThongTin("DT_AO", "HD_AO");
            Assert.IsNotNull(detail);
            Assert.IsNull(detail.TenBN, "Mã ảo không thể truy xuất được tên Bệnh nhân.");
            Assert.AreEqual(0, detail.DanhSachThuoc.Count);
        }

        // ==========================================
        // MODULE 2: GIAO DỊCH XÁC NHẬN PHÁT THUỐC (TRANSACTIONS)
        // ==========================================
        [TestMethod]
        public void XacNhanPhatThuoc_HoaDonChuaThanhToan_TraVeLoiVaRollback()
        {
            // Cố tình Set hóa đơn về trạng thái chưa thanh toán
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                new SqlCommand($"UPDATE HOADON SET TrangThaiThanhToan = N'Chưa thanh toán' WHERE MaHD = '{_testMaHD}'", conn).ExecuteNonQuery();
            }

            var result = _phatThuocDb.XacNhanPhatThuoc(_testMaDonThuoc, _testMaHD, null, _testMaPhong);
            Assert.IsFalse(result.IsSuccess, "Hệ thống phải chặn không cho phát thuốc khi chưa đóng tiền.");
            Assert.IsTrue(result.Message.Contains("chưa được thanh toán"));
        }

        [TestMethod]
        public void XacNhanPhatThuoc_ThieuTonKho_TraVeLoiVaRollback()
        {
            // Cố tình ép Tồn kho về 0
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                new SqlCommand($"UPDATE TONKHO SET SoLuongTon = 0 WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery();
            }

            var result = _phatThuocDb.XacNhanPhatThuoc(_testMaDonThuoc, _testMaHD, null, _testMaPhong);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Message.Contains("không đủ tồn kho"));
        }

        [TestMethod]
        public void XacNhanPhatThuoc_DuKieuKien_ThucThiThanhCong()
        {
            var result = _phatThuocDb.XacNhanPhatThuoc(_testMaDonThuoc, _testMaHD, null, _testMaPhong);
            Assert.IsTrue(result.IsSuccess, "Phát thuốc phải thành công: " + result.Message);

            // Kiểm tra DB xem kho có bị trừ đi 10 viên không
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                int tonKho = (int)new SqlCommand($"SELECT SoLuongTon FROM TONKHO WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteScalar();
                Assert.AreEqual(990, tonKho, "Tồn kho không được trừ đúng thuật toán FIFO/FEFO.");
            }
        }

        [TestMethod]
        public void XacNhanPhatThuoc_DonThuocAo_ReturnsFalse()
        {
            var result = _phatThuocDb.XacNhanPhatThuoc("DT_AO", "HD_AO", null, 1);
            Assert.IsFalse(result.IsSuccess);
        }

        // ==========================================
        // MODULE 3: LỊCH SỬ VÀ PHÂN TRANG
        // ==========================================
        [TestMethod]
        public void GetLichSuPhatThuoc_PhanTrang_KhongVuotQuaPageSize()
        {
            var result = _phatThuocDb.GetLichSuPhatThuoc_Pagination("", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(1), 1, 5, "BenhNhan", "ASC");
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data.Count <= 5, "Phân trang bị lố pageSize.");
            Assert.IsTrue(result.TotalRow >= 0);
        }

        [TestMethod]
        public void GetLichSuPhatThuoc_SearchTuKhoaAo_TraVeRong()
        {
            var result = _phatThuocDb.GetLichSuPhatThuoc_Pagination("TUKHOA_AO_KHONG_THE_CO", DateTime.Now.AddDays(-10), DateTime.Now, 1, 10, "BenhNhan", "ASC");
            Assert.AreEqual(0, result.Data.Count);
            Assert.AreEqual(0, result.TotalRow);
        }

        [TestMethod]
        public void GetLichSuPhatThuoc_SortDirection_ThrowsNoException()
        {
            // Kiểm thử truyền mã độc vào Sort để test SQL Injection
            var result = _phatThuocDb.GetLichSuPhatThuoc_Pagination("", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), 1, 10, "BenhNhan", "DROP TABLE");
            Assert.IsNotNull(result.Data, "Hệ thống phải tự ép kiểu SortDir về ASC/DESC an toàn.");
        }
    }
}