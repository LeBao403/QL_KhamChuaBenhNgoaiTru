using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class PhongDBTests
    {
        private PhongDB _phongDb;
        private string _connectStr;

        private int _testMaKhoa;
        private int _testMaPhong;

        [TestInitialize]
        public void Setup()
        {
            _phongDb = new PhongDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                // 1. Tạo Khoa test
                _testMaKhoa = (int)new SqlCommand("INSERT INTO KHOA (TenKhoa, TrangThai) OUTPUT INSERTED.MaKhoa VALUES (N'Khoa Test Quản Lý Phòng', 1)", conn).ExecuteScalar();

                // 2. Tạo Phòng test
                _testMaPhong = (int)new SqlCommand($"INSERT INTO PHONG (TenPhong, MaKhoa, TrangThai) OUTPUT INSERTED.MaPhong VALUES (N'Phòng Test Unit', {_testMaKhoa}, 1)", conn).ExecuteScalar();
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                try { new SqlCommand($"UPDATE NHANVIEN SET MaPhong = NULL WHERE MaPhong = {_testMaPhong}", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHONG WHERE MaPhong = {_testMaPhong}", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM KHOA WHERE MaKhoa = {_testMaKhoa}", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // NHÓM 1: ĐỌC DỮ LIỆU & TÌM KIẾM
        // ==========================================
        [TestMethod]
        public void GetAllLoaiPhong_KhongTraVeNull()
        {
            var list = _phongDb.GetAllLoaiPhong();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetAll_TraVeDanhSach()
        {
            var list = _phongDb.GetAll();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void GetById_MaPhongHopLe_TraVeDoiTuong()
        {
            var phong = _phongDb.GetById(_testMaPhong);
            Assert.IsNotNull(phong);
            Assert.AreEqual("Phòng Test Unit", phong.TenPhong);
        }

        [TestMethod]
        public void GetById_MaPhongAo_TraVeNull()
        {
            var phong = _phongDb.GetById(-999);
            Assert.IsNull(phong);
        }

        [TestMethod]
        public void Search_TheoTuKhoa_TraVeDungResult()
        {
            var list = _phongDb.Search("Test Unit", null, 0);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void Search_MaKhoaAo_TraVeRong()
        {
            var list = _phongDb.Search("", null, -999);
            Assert.AreEqual(0, list.Count);
        }

        // ==========================================
        // NHÓM 2: VALIDATION VÀ RÀNG BUỘC (BUSINESS RULES)
        // ==========================================
        [TestMethod]
        public void CheckTenPhongExists_TenTonTai_ReturnsTrue()
        {
            bool exists = _phongDb.CheckTenPhongExists("Phòng Test Unit");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void CheckTenPhongExists_ExcludeSelf_ReturnsFalse()
        {
            bool exists = _phongDb.CheckTenPhongExists("Phòng Test Unit", _testMaPhong);
            Assert.IsFalse(exists, "Nếu update chính nó thì không được báo trùng.");
        }

        [TestMethod]
        public void CheckCanDelete_PhongRong_ReturnsTrue()
        {
            string err;
            bool canDel = _phongDb.CheckCanDelete(_testMaPhong, out err);
            Assert.IsTrue(canDel);
            Assert.AreEqual("", err);
        }

        [TestMethod]
        public void CheckCanDelete_PhongCoNhanVien_ReturnsFalse()
        {
            // Ép 1 nhân viên ảo vào phòng
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                new SqlCommand($"INSERT INTO NHANVIEN (MaNV, HoTen, MaPhong) VALUES ('NV_TEST_P', N'Test', {_testMaPhong})", conn).ExecuteNonQuery();
            }

            string err;
            bool canDel = _phongDb.CheckCanDelete(_testMaPhong, out err);

            Assert.IsFalse(canDel);
            Assert.IsTrue(err.Contains("nhân viên trực thuộc"), "Lỗi: Không bắt được ràng buộc nhân viên.");

            // Dọn ngay NV để không cản trở Teardown
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                new SqlCommand($"DELETE FROM NHANVIEN WHERE MaNV = 'NV_TEST_P'", conn).ExecuteNonQuery();
            }
        }

        // ==========================================
        // NHÓM 3: CRUD & ĐIỀU PHỐI NHÂN SỰ
        // ==========================================
        [TestMethod]
        public void ToggleStatus_DaoTrangThai_ThanhCong()
        {
            bool? stt = _phongDb.ToggleStatus(_testMaPhong);
            Assert.IsNotNull(stt);
            Assert.IsFalse(stt.Value, "Đang 1 phải chuyển thành 0.");
        }

        [TestMethod]
        public void AddNhanVienToRoom_MaNVAo_TraVeFalse()
        {
            bool isAdded = _phongDb.AddNhanVienToRoom("NV_AO_123", _testMaPhong);
            Assert.IsFalse(isAdded);
        }

        [TestMethod]
        public void RemoveNhanVienFromRoom_MaNVAo_TraVeFalse()
        {
            bool isRemoved = _phongDb.RemoveNhanVienFromRoom("NV_AO_123");
            Assert.IsFalse(isRemoved);
        }

        [TestMethod]
        public void GetNhanVienByRoom_MaPhongAo_TraVeRong()
        {
            var nv = _phongDb.GetNhanVienByRoom(-999);
            Assert.IsNotNull(nv);
            Assert.AreEqual(0, nv.Count);
        }

        [TestMethod]
        public void GetAvailableNhanVienForRoom_KhongBiNull()
        {
            var nv = _phongDb.GetAvailableNhanVienForRoom(_testMaKhoa);
            Assert.IsNotNull(nv);
        }
    }
}