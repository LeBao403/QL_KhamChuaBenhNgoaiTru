using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class KhoaDBTests
    {
        private KhoaDB _khoaDb;
        private string _connectStr;
        private int _testMaKhoa;

        [TestInitialize]
        public void Setup()
        {
            _khoaDb = new KhoaDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            // Tạo 1 Khoa Ảo để làm bia đỡ đạn cho các test case
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                string sql = "INSERT INTO KHOA (TenKhoa, MoTa, TrangThai) OUTPUT INSERTED.MaKhoa VALUES (N'Khoa Test Unit', N'Khoa dùng để chạy Test', 1)";
                _testMaKhoa = (int)new SqlCommand(sql, conn).ExecuteScalar();
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            // Dọn sạch Khoa ảo sau khi test xong
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                try { new SqlCommand($"DELETE FROM NHANVIEN WHERE MaKhoa = {_testMaKhoa}", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"UPDATE PHONG SET MaKhoa = NULL WHERE MaKhoa = {_testMaKhoa}", conn).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM KHOA WHERE MaKhoa = {_testMaKhoa}", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // NHÓM 1: ĐỌC DỮ LIỆU & TÌM KIẾM
        // ==========================================
        [TestMethod]
        public void GetAll_TraVeDanhSachKhongNull()
        {
            var list = _khoaDb.GetAll();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "Ít nhất phải có 1 khoa (Khoa Test) vừa được tạo.");
        }

        [TestMethod]
        public void GetById_MaKhoaHopLe_TraVeDoiTuong()
        {
            var khoa = _khoaDb.GetById(_testMaKhoa);
            Assert.IsNotNull(khoa);
            Assert.AreEqual("Khoa Test Unit", khoa.TenKhoa);
        }

        [TestMethod]
        public void GetById_MaKhoaAo_TraVeNull()
        {
            var khoa = _khoaDb.GetById(-999);
            Assert.IsNull(khoa, "Mã ảo phải trả về null.");
        }

        [TestMethod]
        public void Search_CoTuKhoa_TraVeDungKhoa()
        {
            var list = _khoaDb.Search("Test Unit");
            Assert.IsTrue(list.Any(k => k.MaKhoa == _testMaKhoa));
        }

        [TestMethod]
        public void Search_KhongCoTuKhoa_TraVeRong()
        {
            var list = _khoaDb.Search("TuKhoaKhongTonTai_9999");
            Assert.AreEqual(0, list.Count);
        }

        // ==========================================
        // NHÓM 2: KIỂM THỬ VALIDATION TÊN KHOA
        // ==========================================
        [TestMethod]
        public void CheckTenKhoaExists_TenDaCo_ReturnsTrue()
        {
            bool exists = _khoaDb.CheckTenKhoaExists("Khoa Test Unit");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void CheckTenKhoaExists_TenMoi_ReturnsFalse()
        {
            bool exists = _khoaDb.CheckTenKhoaExists("Khoa Chua Tung Co Tren Doi");
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void CheckTenKhoaExists_TenRong_ReturnsFalse()
        {
            bool exists = _khoaDb.CheckTenKhoaExists("   ");
            Assert.IsFalse(exists, "Tên rỗng không được tính là tồn tại, phải return false sớm.");
        }

        [TestMethod]
        public void CheckTenKhoaExists_ExcludeSelf_ReturnsFalse()
        {
            // Edit Khoa Test Unit, vẫn giữ nguyên tên đó -> Không bị báo trùng
            bool exists = _khoaDb.CheckTenKhoaExists("Khoa Test Unit", _testMaKhoa);
            Assert.IsFalse(exists);
        }

        // ==========================================
        // NHÓM 3: RÀNG BUỘC KHI XÓA KHOA (BUSINESS RULES)
        // ==========================================
        [TestMethod]
        public void CheckCanDelete_KhoaRong_ChoPhepXoa()
        {
            string error;
            bool canDelete = _khoaDb.CheckCanDelete(_testMaKhoa, out error);
            Assert.IsTrue(canDelete);
            Assert.AreEqual("", error);
        }

        [TestMethod]
        public void CheckCanDelete_KhoaCoNhanVien_KhongChoXoa()
        {
            // Bơm 1 NV ảo vào khoa Test
            using (var conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                new SqlCommand($"INSERT INTO NHANVIEN (MaNV, HoTen, MaKhoa) VALUES ('NV_TEST_K', N'Test', {_testMaKhoa})", conn).ExecuteNonQuery();
            }

            string error;
            bool canDelete = _khoaDb.CheckCanDelete(_testMaKhoa, out error);

            Assert.IsFalse(canDelete);
            Assert.IsTrue(error.Contains("nhân viên trực thuộc"), "Phải báo lỗi chứa nhân viên.");
        }

        // ==========================================
        // NHÓM 4: CẬP NHẬT & ĐẢO TRẠNG THÁI
        // ==========================================
        [TestMethod]
        public void Update_KhoaHopLe_ReturnsTrue()
        {
            var khoa = _khoaDb.GetById(_testMaKhoa);
            khoa.TenKhoa = "Khoa Test Unit Updated";
            khoa.MoTa = null;

            bool result = _khoaDb.Update(khoa);
            Assert.IsTrue(result);

            var checkKhoa = _khoaDb.GetById(_testMaKhoa);
            Assert.AreEqual("Khoa Test Unit Updated", checkKhoa.TenKhoa);
        }

        [TestMethod]
        public void ToggleStatus_DaoTrangThai_ThanhCong()
        {
            bool? newStatus = _khoaDb.ToggleStatus(_testMaKhoa); // Đang 1 -> 0
            Assert.IsNotNull(newStatus);
            Assert.IsFalse(newStatus.Value);
        }

        [TestMethod]
        public void ToggleStatus_MaKhoaAo_ReturnsNull()
        {
            bool? newStatus = _khoaDb.ToggleStatus(-999);
            Assert.IsNull(newStatus, "Mã ảo phải trả về null.");
        }

        [TestMethod]
        public void RemovePhongFromKhoa_PhongAo_TraVeFalse()
        {
            string errorMsg;
            bool result = _khoaDb.RemovePhongFromKhoa(-999, out errorMsg);
            Assert.IsFalse(result);
        }
    }
}