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

        // Mã mồi giả định
        private const string _testMaBN = "BN_CLS99";
        private const string _testMaDV = "DV_CLS99";
        private const string _testMaLoaiDV = "L_TEST99";
        private const string _testMaBS = "BS_TEST99";

        // Lưu lại ID tự tăng để dọn dẹp
        private int _testMaPKB;
        private int _testMaPCD;
        private int _testMaKetQua;

        // =========================================================
        // BƯỚC 1: DỌN CỖ (Tạo chuỗi Data mồi liên hoàn)
        // =========================================================
        [TestInitialize]
        public void Setup()
        {
            _db = new CLSDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData(); // Dọn rác đề phòng chạy lỗi lần trước

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();

                // 1. Tạo Bác sĩ mồi (Tránh lỗi FK khi cập nhật kết quả)
                new SqlCommand($"INSERT INTO NHANVIEN (MaNV, HoTen) VALUES ('{_testMaBS}', N'Bác Sĩ Test')", conn).ExecuteNonQuery();

                // 2. Tạo Bệnh nhân mồi
                new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen, BHYT) VALUES ('{_testMaBN}', N'Bệnh Nhân CLS', 0)", conn).ExecuteNonQuery();

                // 3. Tạo Loại Dịch Vụ mồi
                try { new SqlCommand($"INSERT INTO LOAI_DICHVU (MaLoaiDV, TenLoaiDV) VALUES ('{_testMaLoaiDV}', N'Loại Test')", conn).ExecuteNonQuery(); } catch { }

                // 4. Tạo Dịch vụ mồi (Bắt buộc có MaLoaiDV)
                new SqlCommand($"INSERT INTO DICHVU (MaDV, TenDV, GiaDichVu, TrangThai, MaLoaiDV) VALUES ('{_testMaDV}', N'Siêu âm Test', 100000, 1, '{_testMaLoaiDV}')", conn).ExecuteNonQuery();

                // 5. Tạo Phiếu Khám Bệnh (Trạng thái: Chờ cận lâm sàng)
                string sqlPKB = @"INSERT INTO PHIEUKHAMBENH (MaBN, NgayLap, TrangThai) 
                                  OUTPUT INSERTED.MaPhieuKhamBenh 
                                  VALUES (@MaBN, GETDATE(), N'Chờ cận lâm sàng')";
                SqlCommand cmdPKB = new SqlCommand(sqlPKB, conn);
                cmdPKB.Parameters.AddWithValue("@MaBN", _testMaBN);
                _testMaPKB = (int)cmdPKB.ExecuteScalar();

                // 6. Tạo Phiếu Chỉ Định
                string sqlPCD = @"INSERT INTO PHIEU_CHIDINH (MaPhieuKhamBenh, NgayChiDinh, TrangThai, TongTien) 
                                  OUTPUT INSERTED.MaPhieuChiDinh 
                                  VALUES (@MaPKB, GETDATE(), N'Đã thanh toán', 100000)";
                SqlCommand cmdPCD = new SqlCommand(sqlPCD, conn);
                cmdPCD.Parameters.AddWithValue("@MaPKB", _testMaPKB);
                _testMaPCD = (int)cmdPCD.ExecuteScalar();

                // 7. Tạo Chi Tiết Chỉ Định (Dòng kết quả mục tiêu)
                string sqlCT = @"INSERT INTO CHITIET_CHIDINH (MaPhieuChiDinh, MaDV, DonGia, TrangThai) 
                                 OUTPUT INSERTED.MaCTChiDinh 
                                 VALUES (@MaPCD, @MaDV, 100000, N'Chưa thực hiện')";
                SqlCommand cmdCT = new SqlCommand(sqlCT, conn);
                cmdCT.Parameters.AddWithValue("@MaPCD", _testMaPCD);
                cmdCT.Parameters.AddWithValue("@MaDV", _testMaDV);
                _testMaKetQua = (int)cmdCT.ExecuteScalar();
            }
        }

        // =========================================================
        // BƯỚC 3: RỬA BÁT (Xóa ngược từ trong ra ngoài)
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
                // Xóa theo thứ tự ngược lại để không dính ràng buộc khóa ngoại
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
        // BƯỚC 2: XƠI CỖ - CÁC KỊCH BẢN KIỂM THỬ
        // =========================================================

        [TestMethod]
        public void GetDanhSachChoThucHien_ShouldReturnPendingItems()
        {
            var ds = _db.GetDanhSachChoThucHien();
            Assert.IsNotNull(ds);

            var itemMoi = ds.FirstOrDefault(x => x.MaKetQua == _testMaKetQua);
            Assert.IsNotNull(itemMoi, "Lỗi: Không tìm thấy ca CLS mồi trong hàng đợi!");
            Assert.AreEqual("Bệnh Nhân CLS", itemMoi.TenBenhNhan);
        }

        [TestMethod]
        public void GetThongTinChiTietCLS_ShouldReturnDynamicObject()
        {
            // Sử dụng dynamic và ép kiểu tường minh để tránh lỗi RuntimeBinder
            dynamic chiTiet = _db.GetThongTinChiTietCLS(_testMaKetQua);

            Assert.IsNotNull(chiTiet);
            string tenBenhNhan = (string)chiTiet.TenBenhNhan;
            Assert.AreEqual("Bệnh Nhân CLS", tenBenhNhan);
        }

        [TestMethod]
        public void GetKetQuaById_ShouldReturnData_And_TrimChar()
        {
            var kq = _db.GetKetQuaById(_testMaKetQua);

            Assert.IsNotNull(kq);
            // .Trim() để xử lý kiểu dữ liệu CHAR(10) trong SQL
            Assert.AreEqual(_testMaDV, kq.MaDV.Trim());
            Assert.AreEqual("Chưa thực hiện", kq.TrangThai);
        }

        [TestMethod]
        public void GetKetQuaById_ShouldReturnNull_WhenIdIsInvalid()
        {
            var kq = _db.GetKetQuaById(-1);
            Assert.IsNull(kq);
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_FullParams_ShouldUpdateSuccessfully()
        {
            string kqNoiDung = "Ket qua xet nghiem on dinh";
            bool isUpdated = _db.CapNhatKetQuaTuLIS(_testMaKetQua, kqNoiDung, _testMaPKB, _testMaBS, "test.jpg", "Mau", "Tot");

            Assert.IsTrue(isUpdated);
            var check = _db.GetKetQuaById(_testMaKetQua);
            Assert.AreEqual("Đã có kết quả", check.TrangThai);
            Assert.AreEqual(kqNoiDung, check.NoiDungKetQua);
            Assert.AreEqual(_testMaBS, check.MaBacSiThucHien.Trim());
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_ShortParams_ShouldUpdateSuccessfully()
        {
            // Test hàm nạp chồng (overload) 3 tham số
            bool isUpdated = _db.CapNhatKetQuaTuLIS(_testMaKetQua, "KQ Rut Gon", _testMaPKB);

            Assert.IsTrue(isUpdated);
            var check = _db.GetKetQuaById(_testMaKetQua);
            Assert.AreEqual("Đã có kết quả", check.TrangThai);
        }

        [TestMethod]
        public void GetLichSuXetNghiem_ShouldReturnData()
        {
            // Arrange: Đưa trạng thái về "Đã có kết quả" mới lôi lên lịch sử được
            _db.CapNhatKetQuaTuLIS(_testMaKetQua, "Test History", _testMaPKB);

            // Act
            var ds = _db.GetLichSuXetNghiem(10);

            // Assert
            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Any(x => x.MaKetQua == _testMaKetQua));
        }
    }
}