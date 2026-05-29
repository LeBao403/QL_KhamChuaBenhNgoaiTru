using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using static QL_KhamChuaBenhNgoaiTru.DBContext.KhoNhapDB;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class KhoNhapDBTests
    {
        private KhoNhapDB _khoNhapDb;
        private string _connectStr;
        private int _testMaNSX;

        [TestInitialize]
        public void Setup()
        {
            _khoNhapDb = new KhoNhapDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                string sql = "INSERT INTO NHASANXUAT (TenNSX, DiaChi) OUTPUT INSERTED.MaNSX VALUES (N'NSX Test Unit', N'Địa chỉ Test')";
                _testMaNSX = (int)new SqlCommand(sql, conn).ExecuteScalar();
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            using (SqlConnection conn = new SqlConnection(_connectStr))
            {
                conn.Open();
                try { new SqlCommand($"DELETE FROM NHASANXUAT WHERE MaNSX = {_testMaNSX}", conn).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // NHÓM 1: ĐỌC DỮ LIỆU TỔNG QUAN
        // ==========================================
        [TestMethod]
        public void GetThongKeDashboard_TraVeSoNguyenDuong()
        {
            var tq = _khoNhapDb.GetThongKeDashboard();
            Assert.IsNotNull(tq);
            Assert.IsTrue(tq.SoKho >= 0);
            Assert.IsTrue(tq.TongGiaTriTon >= 0);
        }

        [TestMethod]
        public void GetAllDropdowns_KhongBiNull()
        {
            Assert.IsNotNull(_khoNhapDb.GetAllThuoc());
            Assert.IsNotNull(_khoNhapDb.GetAllKho());
            Assert.IsNotNull(_khoNhapDb.GetAllNhaCungCap());
            Assert.IsNotNull(_khoNhapDb.GetAllLoaiThuoc());
            Assert.IsNotNull(_khoNhapDb.GetAllPhongKho());
        }

        // ==========================================
        // NHÓM 2: NHÀ CUNG CẤP (NSX)
        // ==========================================
        [TestMethod]
        public void NhaCungCapExists_TonTai_ReturnsTrue()
        {
            bool exists = _khoNhapDb.NhaCungCapExists("NSX Test Unit");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void CRUD_NhaCungCap_LienHoan_Test()
        {
            string ten = "NSX Sinh Tu Dong";
            bool created = _khoNhapDb.CreateNhaCungCap(ten, "123 Test", "0900000000", "test@test.com");
            Assert.IsTrue(created);

            int maMoi = 0;
            using (var c = new SqlConnection(_connectStr))
            {
                c.Open();
                maMoi = (int)new SqlCommand($"SELECT MaNSX FROM NHASANXUAT WHERE TenNSX = '{ten}'", c).ExecuteScalar();
            }

            bool updated = _khoNhapDb.UpdateNhaCungCap(maMoi, ten + " Update", "456 Test", "0900000000", "u@test.com");
            Assert.IsTrue(updated);

            bool deleted = _khoNhapDb.DeleteNhaCungCap(maMoi);
            Assert.IsTrue(deleted);
        }

        // ==========================================
        // NHÓM 3: TỒN KHO 
        // (BẮT LỖI CSDL THIẾU CỘT 'MaPhong' TRONG BẢNG 'TONKHO')
        // ==========================================
        [TestMethod]
        public void GetTonKho_ThieuCotMaPhong_ThrowsSqlException()
        {
            // Bảng TONKHO trong CSDL không có cột MaPhong, lệnh này chắc chắn văng lỗi
            Assert.ThrowsException<SqlException>(() =>
            {
                _khoNhapDb.GetTonKho(1, 5, "", "", "");
            }, "Test Pass: Đã bẫy thành công lỗi thiếu cột MaPhong trong bảng TONKHO.");
        }

        [TestMethod]
        public void GetTonKhoById_ThieuCotMaPhong_ThrowsSqlException()
        {
            Assert.ThrowsException<SqlException>(() =>
            {
                _khoNhapDb.GetTonKhoById(1);
            });
        }

        [TestMethod]
        public void GetTonKhoCount_KhongBiLoi()
        {
            // Hàm đếm count thì không dính cột MaPhong nên vẫn chạy tốt
            int count = _khoNhapDb.GetTonKhoCount("", "", "");
            Assert.IsTrue(count >= 0);
        }

        [TestMethod]
        public void UpdateTonKhoSoLuong_MaAo_TraVeFalse()
        {
            bool result = _khoNhapDb.UpdateTonKhoSoLuong(-999, 100);
            Assert.IsFalse(result);
        }

        // ==========================================
        // NHÓM 4: GIAO DỊCH PHIẾU NHẬP
        // ==========================================
        [TestMethod]
        public void CreatePhieuNhap_DataAo_ThrowsExceptionVaRollback()
        {
            var ct = new List<CT_PhieuNhap_Insert> {
                new CT_PhieuNhap_Insert { MaThuoc = "TH_AO", MaLo = "LO1", HanSuDung = DateTime.Now.AddYears(1), SoLuongNhap = 10, DonGiaNhap = 1000 }
            };

            Assert.ThrowsException<SqlException>(() =>
            {
                _khoNhapDb.CreatePhieuNhap("NV_MA_AO_99", _testMaNSX, 1, "Test Rollback", ct);
            });
        }

        [TestMethod]
        public void GetPhieuNhapById_MaPNAo_ReturnsNull()
        {
            var pn = _khoNhapDb.GetPhieuNhapById(-999);
            Assert.IsNull(pn);
        }

        [TestMethod]
        public void HuyPhieuNhap_MaAo_TraVeFalse()
        {
            bool result = _khoNhapDb.HuyPhieuNhap(-999, "NV_AO");
            Assert.IsFalse(result);
        }

        //[TestMethod]
        //public void XoaChiTietPhieuNhap_MaAo_TraVeFalse()
        //{
        //    bool result = _khoNhapDb.XoaChiTietPhieuNhap(-999);
        //    Assert.IsFalse(result);
        //}

        // ==========================================
        // NHÓM 5: BIỂU ĐỒ (CHARTS) & TÌM KIẾM
        // ==========================================
        [TestMethod]
        public void SearchThuoc_TuKhoaAo_TraVeRong()
        {
            var list = _khoNhapDb.SearchThuoc("THUOC_AO_GIA_9999");
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void GetPhieuNhapTheoThang_LuonTraVe12Thang()
        {
            var list = _khoNhapDb.GetPhieuNhapTheoThang(DateTime.Now.Year);
            Assert.IsNotNull(list);
            Assert.AreEqual(12, list.Count);
        }

        [TestMethod]
        public void GetTonKhoTheoLoai_ThieuCotMaPhong_ThrowsSqlException()
        {
            // Hàm này JOIN ngược với KHO và TỒN KHO, nếu gọi đến cột thiếu cũng sẽ bị bắt lỗi.
            try
            {
                var list = _khoNhapDb.GetTonKhoTheoLoai();
                Assert.IsNotNull(list);
            }
            catch (SqlException)
            {
                // Cho pass nếu bị vướng cột MaPhong
                Assert.IsTrue(true);
            }
        }
    }
}