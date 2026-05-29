using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class ThuocDBTests
    {
        private ThuocDB _thuocDb;
        private string _connectStr;
        private string _testMaThuoc = "T_TEST_99";

        [TestInitialize]
        public void Setup()
        {
            _thuocDb = new ThuocDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
            CleanupData();

            // Bơm sẵn 1 viên thuốc mồi
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                try { new SqlCommand("INSERT INTO DANHMUC_THUOC (MaDanhMuc, TenDanhMuc) VALUES ('DM_TEST', N'Danh mục Test')", conn).ExecuteNonQuery(); } catch { }
                new SqlCommand($"INSERT INTO THUOC (MaThuoc, TenThuoc, DonViCoBan, MaLoaiThuoc, GiaBan) VALUES ('{_testMaThuoc}', N'Thuốc Mồi Test', N'Viên', 'DM_TEST', 10000)", conn).ExecuteNonQuery();
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
                try { new SqlCommand($"DELETE FROM THANHPHAN_THUOC WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM THUOC WHERE MaThuoc = '{_testMaThuoc}'", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand("DELETE FROM DANHMUC_THUOC WHERE MaDanhMuc = 'DM_TEST'", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // NHÓM 1: LẤY DỮ LIỆU DROPDOWN
        // ==========================================
        [TestMethod]
        public void GetAll_Dropdowns_KhongBaoGioNull()
        {
            Assert.IsNotNull(_thuocDb.GetAllNSX());
            Assert.IsNotNull(_thuocDb.GetAllHoatChat());
            Assert.IsNotNull(_thuocDb.GetAllLoaiThuoc());
            Assert.IsNotNull(_thuocDb.GetAllDuongDung());
        }

        // ==========================================
        // NHÓM 2: LỌC DANH SÁCH & TÌM KIẾM
        // ==========================================
        [TestMethod]
        public void GetAll_PhanTrang_DungGioiHan()
        {
            var ds = _thuocDb.GetAll(1, 5, "", "", "", null, null);
            Assert.IsTrue(ds.Count <= 5);
        }

        [TestMethod]
        public void GetCount_TuKhoaAo_TraVe0()
        {
            int count = _thuocDb.GetCount("THUOC_AO_KHONG_BAO_GIO_CO", "", "", null, null);
            Assert.AreEqual(0, count);
        }

        //[TestMethod]
        //public void SearchThuoc_TuKhoaCoDau_TraVeKetQuaChinhXac()
        //{
        //    // Bảng thuốc bắt buộc phải hỗ trợ tìm kiếm Tiếng Việt có dấu (Collate AI)
        //    var ds = _thuocDb.SearchThuoc("Thuốc Mồi Test", "");
        //    Assert.IsTrue(ds.Any(t => t.MaThuoc == _testMaThuoc));
        //}

        // ==========================================
        // NHÓM 3: XEM CHI TIẾT
        // ==========================================
        [TestMethod]
        public void GetById_MaThuocHopLe_TraVeThuoc()
        {
            var thuoc = _thuocDb.GetById(_testMaThuoc);
            Assert.IsNotNull(thuoc);
            Assert.AreEqual("Thuốc Mồi Test", thuoc.TenThuoc);
        }

        [TestMethod]
        public void GetById_MaThuocAo_TraVeNull()
        {
            var thuoc = _thuocDb.GetById("T_AO_9999");
            Assert.IsNull(thuoc);
        }

        [TestMethod]
        public void GetByIdWithThanhPhan_KiemTraTraVeCaThanhPhan()
        {
            var thuoc = _thuocDb.GetByIdWithThanhPhan(_testMaThuoc);
            Assert.IsNotNull(thuoc);
            Assert.IsNotNull(thuoc.ThanhPhans, "List thành phần được khởi tạo dù không có thành phần nào.");
        }

        [TestMethod]
        public void GetThanhPhanByMaThuoc_MaAo_TraVeListRong()
        {
            var list = _thuocDb.GetThanhPhanByMaThuoc("T_AO_999");
            Assert.AreEqual(0, list.Count);
        }

        // ==========================================
        // NHÓM 4: RÀNG BUỘC VÀ SINH MÃ TỰ ĐỘNG
        // ==========================================
        [TestMethod]
        public void TenThuocExists_TenDaCo_ReturnsTrue()
        {
            bool exists = _thuocDb.TenThuocExists("Thuốc Mồi Test");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void TenThuocExists_ExcludeMaThuocHienTai_ReturnsFalse()
        {
            bool exists = _thuocDb.TenThuocExists("Thuốc Mồi Test", _testMaThuoc);
            Assert.IsFalse(exists, "Nếu update chính nó thì không báo trùng tên.");
        }

        [TestMethod]
        public void GenerateNextMaThuoc_LuonTraVeDinhDangTxxx()
        {
            string maMoi = _thuocDb.GenerateNextMaThuoc();
            Assert.IsNotNull(maMoi);
            Assert.IsTrue(maMoi.StartsWith("T"), "Mã thuốc phải bắt đầu bằng chữ T.");
            Assert.IsTrue(maMoi.Length >= 5, "Mã thuốc phải có ít nhất 4 số theo sau (T0001).");
        }

        // ==========================================
        // NHÓM 5: CRUD
        // ==========================================
        [TestMethod]
        public void ToggleTrangThai_MaAo_ReturnsNull()
        {
            var result = _thuocDb.ToggleTrangThai("T_AO_999");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Update_KhongCoThanhPhan_CapNhatThanhCong()
        {
            var t = _thuocDb.GetById(_testMaThuoc);
            t.TenThuoc = "Thuốc Mồi Đã Sửa";

            // Sửa nhưng truyền list thành phần rỗng
            bool result = _thuocDb.Update(t, new List<ThanhPhanThuoc>());

            Assert.IsTrue(result);
            Assert.AreEqual("Thuốc Mồi Đã Sửa", _thuocDb.GetById(_testMaThuoc).TenThuoc);
        }
    }
}