using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class CLSDBTests
    {
        private CLSDB _db;
        private string _connectStr;

        // Mã mới giả định
        private const string _testMaBN = "KH0001";
        private const string _testMaDV = "DV011";
        private const string _testMaLoaiDV = "LDV02";
        private const string _testMaBS = "NV021";

        // Lưu lại ID tự tăng để dọn dẹp
        private string _testMaPKB;
        private string _testMaPCD;
        private string _testMaKetQua;
        private int _testIdKhoa;
        private int _testIdPhong;

        // =========================================================
        // BƯỚC 1: DỌN CỎ & TẠO DỮ LIỆU LIÊN HOÀN (CHUẨN 100% KHÓA NGOẠI)
        // =========================================================
        [TestInitialize]
        public void Setup()
        {
            _db = new CLSDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            CleanupData(); // Dọn rác trước khi chạy

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();

                // 1. TẠO KHOA & PHÒNG CHUẨN ĐỂ KHÔNG BỊ LỖI KHÓA NGOẠI
                // Dùng OUTPUT INSERTED để bắt lấy ID tự tăng do SQL Server sinh ra
                _testIdKhoa = (int)new SqlCommand("INSERT INTO KHOA (TenKhoa, TrangThai) OUTPUT INSERTED.MaKhoa VALUES (N'Khoa Test CLS', 1)", conn).ExecuteScalar();

                string sqlPhong = $"INSERT INTO PHONG (TenPhong, TrangThai, MaKhoa) OUTPUT INSERTED.MaPhong VALUES (N'Phòng Test CLS', 1, {_testIdKhoa})";
                _testIdPhong = (int)new SqlCommand(sqlPhong, conn).ExecuteScalar();

                // 2. Tạo Bác sĩ mới (Gắn liền với cái Phòng vừa tạo thành công ở trên)
                new SqlCommand($"INSERT INTO NHANVIEN (MaNV, HoTen, MaPhong) VALUES ('{_testMaBS}', N'Bác Sĩ Test', {_testIdPhong})", conn).ExecuteNonQuery();

                // 3. Tạo Bệnh nhân mới
                new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen, BHYT) VALUES ('{_testMaBN}', N'Bệnh Nhân CLS', 0)", conn).ExecuteNonQuery();

                // 4. Tạo Loại Dịch Vụ mới
                try { new SqlCommand($"INSERT INTO LOAI_DICHVU (MaLoaiDV, TenLoaiDV) VALUES ('{_testMaLoaiDV}', N'Loại Test')", conn).ExecuteNonQuery(); } catch { }

                // 5. Tạo Dịch vụ mới (Bắt buộc có MaLoaiDV)
                new SqlCommand($"INSERT INTO DICHVU (MaDV, TenDV, GiaDichVu, TrangThai, MaLoaiDV) VALUES ('{_testMaDV}', N'Siêu âm Test', 100000, 1, '{_testMaLoaiDV}')", conn).ExecuteNonQuery();

                // 6. Tạo Phiếu Khám Bệnh (Trạng thái: Chờ cận lâm sàng)
                string sqlPKB = @"INSERT INTO PHIEUKHAMBENH (MaBN, NgayLap, TrangThai) 
                                  OUTPUT INSERTED.MaPhieuKhamBenh 
                                  VALUES (@MaBN, GETDATE(), N'Chờ cận lâm sàng')";
                SqlCommand cmdPKB = new SqlCommand(sqlPKB, conn);
                cmdPKB.Parameters.AddWithValue("@MaBN", _testMaBN);
                _testMaPKB = cmdPKB.ExecuteScalar().ToString();

                // 7. Tạo Phiếu Chỉ Định (Gắn vô cái Phòng ảo luôn)
                string sqlPCD = @"INSERT INTO PHIEU_CHIDINH (MaPhieuKhamBenh, NgayChiDinh, TrangThai, TongTien, MaPhong) 
                                  OUTPUT INSERTED.MaPhieuChiDinh 
                                  VALUES (@MaPKB, GETDATE(), N'Đã thanh toán', 100000, @MaPhong)";
                SqlCommand cmdPCD = new SqlCommand(sqlPCD, conn);
                cmdPCD.Parameters.AddWithValue("@MaPKB", _testMaPKB);
                cmdPCD.Parameters.AddWithValue("@MaPhong", _testIdPhong);
                _testMaPCD = cmdPCD.ExecuteScalar().ToString();

                // 8. Tạo Chi Tiết Chỉ Định (Dòng kết quả mục tiêu)
                string sqlCT = @"INSERT INTO CHITIET_CHIDINH (MaPhieuChiDinh, MaDV, DonGia, TrangThai) 
                                 OUTPUT INSERTED.MaCTChiDinh 
                                 VALUES (@MaPCD, @MaDV, 100000, N'Chưa thực hiện')";
                SqlCommand cmdCT = new SqlCommand(sqlCT, conn);
                cmdCT.Parameters.AddWithValue("@MaPCD", _testMaPCD);
                cmdCT.Parameters.AddWithValue("@MaDV", _testMaDV);
                _testMaKetQua = cmdCT.ExecuteScalar().ToString();
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

                // Xóa phòng và khoa Test
                try { new SqlCommand($"DELETE FROM PHONG WHERE TenPhong = N'Phòng Test CLS'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM KHOA WHERE TenKhoa = N'Khoa Test CLS'", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // =========================================================
        // BƯỚC 2: XỚI CỎ - CÁC KỊCH BẢN KIỂM THỬ
        // =========================================================

        [TestMethod]
        public void GetDanhSachChoThucHien_ShouldReturnPendingItems()
        {
            var ds = _db.GetDanhSachChoThucHien(_testMaBS); // Đã có phòng xịn nên quét được
            Assert.IsNotNull(ds);

            var itemMoi = ds.FirstOrDefault(x => x.MaKetQua == _testMaKetQua);
            Assert.IsNotNull(itemMoi, "Lỗi: Không tìm thấy ca CLS mới trong hàng đợi!");
            Assert.AreEqual("Bệnh Nhân CLS", itemMoi.TenBenhNhan);
        }

        [TestMethod]
        public void GetThongTinChiTietCLS_ShouldReturnDynamicObject()
        {
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
            Assert.AreEqual(_testMaDV, kq.MaDV.Trim());
            Assert.AreEqual("Chưa thực hiện", kq.TrangThai);
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
            Assert.AreEqual("Đã có kết quả", check.TrangThai);
            Assert.AreEqual(kqNoiDung, check.NoiDungKetQua);
            Assert.AreEqual(_testMaBS, check.MaBacSiThucHien.Trim());
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_ShortParams_ShouldUpdateSuccessfully()
        {
            bool isUpdated = _db.CapNhatKetQuaTuLIS(_testMaKetQua, "KQ Rut Gon", _testMaPKB);

            Assert.IsTrue(isUpdated);
            var check = _db.GetKetQuaById(_testMaKetQua);
            Assert.AreEqual("Đã có kết quả", check.TrangThai);
        }

        [TestMethod]
        public void GetLichSuXetNghiem_ShouldReturnData()
        {
            _db.CapNhatKetQuaTuLIS(_testMaKetQua, "Test History", _testMaPKB);

            var ds = _db.GetLichSuXetNghiem(10);

            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Any(x => x.MaKetQua == _testMaKetQua));
        }

        [TestMethod]
        public void GetDanhSachChoThucHien_MaNVKhongTonTai_ReturnsEmptyList()
        {
            var ds = _db.GetDanhSachChoThucHien("BS_AO_9999");
            Assert.IsNotNull(ds, "Hàm không được trả về null làm crash giao diện.");
            Assert.AreEqual(0, ds.Count, "Bác sĩ không tồn tại hoặc không có phòng trực phải trả về list rỗng.");
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_MaKetQuaAo_NenThrowExceptionVaRollback()
        {
            string maAo = "CT_AO_999";
            bool isSuccess = false;
            try
            {
                isSuccess = _db.CapNhatKetQuaTuLIS(maAo, "Kết quả ảo", _testMaPKB);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Khong tim thay chi dinh CLS can cap nhat.", ex.Message);
            }

            Assert.IsFalse(isSuccess, "Cập nhật với mã ảo phải bị chặn và trả về false.");
        }

        [TestMethod]
        public void CapNhatKetQuaTuLIS_KiemTraCapNhatDayChuyenTrangThai_Success()
        {
            bool isUpdated = _db.CapNhatKetQuaTuLIS(_testMaKetQua, "Chỉ số máu bình thường", _testMaPKB);
            Assert.IsTrue(isUpdated);

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();

                var cmdPCD = new SqlCommand($"SELECT TrangThai FROM PHIEU_CHIDINH WHERE MaPhieuChiDinh='{_testMaPCD}'", conn);
                Assert.AreEqual("Hoàn tất", cmdPCD.ExecuteScalar().ToString().Trim());

                var cmdPKB = new SqlCommand($"SELECT TrangThai FROM PHIEUKHAMBENH WHERE MaPhieuKhamBenh='{_testMaPKB}'", conn);
                Assert.AreEqual("Đã có kết quả CLS", cmdPKB.ExecuteScalar().ToString().Trim());
            }
        }

        [TestMethod]
        public void GetDanhSachDaThucHien_NgayHienTai_TraVeDanhSach()
        {
            _db.CapNhatKetQuaTuLIS(_testMaKetQua, "Đã siêu âm", _testMaPKB);

            var ds = _db.GetDanhSachDaThucHien(_testMaBS);
            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Count > 0, "Phải lấy ra được danh sách đã thực hiện trong ngày.");
        }

        [TestMethod]
        public void GetTenPhongByMaNV_MaNVAo_ReturnsEmpty()
        {
            string tenPhong = _db.GetTenPhongByMaNV("NV_MA_MA");
            Assert.AreEqual("", tenPhong, "Phải trả về chuỗi rỗng thay vì ném Exception.");
        }

        [TestMethod]
        public void GetThongTinIn_IdHopLe_ReturnsDTOForPrinting()
        {
            var thongTin = _db.GetThongTinIn(_testMaKetQua);

            Assert.IsNotNull(thongTin);
            Assert.AreEqual("Bệnh Nhân CLS", thongTin.TenBN);
            Assert.AreEqual("Siêu âm Test", thongTin.TenDV);
            Assert.IsNotNull(thongTin.MaKetQua);
        }

        [TestMethod]
        public void GetThongTinIn_IdAo_ReturnsNull()
        {
            var thongTin = _db.GetThongTinIn("CT_MA_AO_999");
            Assert.IsNull(thongTin, "Mã ảo phải trả về null để Controller điều hướng báo lỗi.");
        }
    }
}