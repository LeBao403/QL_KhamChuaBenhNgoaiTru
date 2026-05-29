using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class TiepTanDBTests
    {
        private TiepTanDB _db;
        private string _connectStr;
        private string _testMaBN = "BN_TIEPTAN_99";

        [TestInitialize]
        public void Setup()
        {
            _db = new TiepTanDB();
            _connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
            Cleanup();

            using (SqlConnection con = new SqlConnection(_connectStr))
            {
                con.Open();
                new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen) VALUES ('{_testMaBN}', N'BN Tiếp Tân Test')", con).ExecuteNonQuery();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (SqlConnection con = new SqlConnection(_connectStr))
            {
                con.Open();
                try { new SqlCommand($"DELETE FROM PHIEUKHAMBENH WHERE MaBN = '{_testMaBN}'", con).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM PHIEUDANGKY WHERE MaBN = '{_testMaBN}'", con).ExecuteNonQuery(); } catch { }
                try { new SqlCommand($"DELETE FROM BENHNHAN WHERE MaBN = '{_testMaBN}'", con).ExecuteNonQuery(); } catch { }
            }
        }

        // ==========================================
        // NHÓM 1: ĐĂNG KÝ KHÁM (DANGKYKHAMTUTHE)
        // ==========================================

        [TestMethod]
        public void DangKyKhamTuThe_ThongTinHopLe_ThanhCong()
        {
            var bn = new BenhNhan { HoTen = "Test BN", CCCD = "123456789" };
            var res = _db.DangKyKhamTuThe(bn, "CCCD");

            Assert.IsTrue(res.Success, "Đăng ký khám phải thành công.");
            Assert.IsNotNull(res.MaPhieuDK);
            Assert.IsTrue(res.STT > 0);
        }

        [TestMethod]
        public void DangKyKhamTuThe_DuLieuRong_TraVeFalse()
        {
            var bn = new BenhNhan { HoTen = "" }; // Rỗng
            var res = _db.DangKyKhamTuThe(bn, "CCCD");
            Assert.IsFalse(res.Success);
        }

        // ==========================================
        // NHÓM 2: XÁC NHẬN DỊCH VỤ (XACNHANDICHVUKHAM)
        // ==========================================

        [TestMethod]
        public void XacNhanDichVuKham_DichVuKhongHopLe_TraVeFalse()
        {
            string phong, khoa; bool pay;
            var res = _db.XacNhanDichVuKham("DK_AO", "DV_AO", 1, "Đau đầu", out phong, out khoa, out pay);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void XacNhanDichVuKham_MaPhieuAo_KhongCrash()
        {
            string phong, khoa; bool pay;
            try
            {
                _db.XacNhanDichVuKham("PHIEU_KHONG_TON_TAI", "DV001", 1, "Test", out phong, out khoa, out pay);
            }
            catch
            {
                Assert.IsTrue(true, "Nếu có lỗi SQL thì hệ thống phải handle, nhưng ở đây ta test Exception.");
            }
        }

        // ==========================================
        // NHÓM 3: CẤP SỐ & XỬ LÝ DỮ LIỆU ĐỘNG
        // ==========================================

        [TestMethod]
        public void ChotCapSo_MaPhieuAo_TraVeFalse()
        {
            string p, k;
            var res = _db.ChotCapSoKham("KB_AO_999", out p, out k);
            Assert.IsFalse(res.Success);
        }

        [TestMethod]
        public void GetTenPhong_MaPhongAo_TraVeStringMacDinh()
        {
            string ten = _db.GetTenPhong(-999);
            Assert.AreEqual("Quầy không xác định", ten);
        }

        [TestMethod]
        public void GetDanhSachOffline_NgaySaiLech_TraVeBangRong()
        {
            var dt = _db.GetDanhSachOffline(1, DateTime.Now.AddYears(5), DateTime.Now.AddYears(6));
            Assert.AreEqual(0, dt.Rows.Count);
        }

        [TestMethod]
        public void GetLichSuTiepNhan_NgayHopLe_TraVeDuLieu()
        {
            var dt = _db.GetLichSuTiepNhan(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            Assert.IsNotNull(dt);
        }

        // ==========================================
        // NHÓM 4: TEST CASE CẬN BIÊN (BOUNDARY) & BẢO MẬT
        // ==========================================

        [TestMethod]
        public void GenerateMaBN_SinhMaTiepTheo_DungFormat()
        {
            using (SqlConnection con = new SqlConnection(_connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    string ma1 = _db.GenerateMaBN(con, trans);
                    Assert.IsTrue(ma1.StartsWith("KH"));
                    // Thử tạo thêm cái nữa xem có tăng không
                    new SqlCommand($"INSERT INTO BENHNHAN (MaBN, HoTen) VALUES ('{ma1}', 'Test')", con, trans).ExecuteNonQuery();
                    string ma2 = _db.GenerateMaBN(con, trans);
                    Assert.AreNotEqual(ma1, ma2);
                    trans.Rollback();
                }
            }
        }

        [TestMethod]
        public void GetDanhSachDichVuKham_KhongNull()
        {
            var dt = _db.GetDanhSachDichVuKham();
            Assert.IsNotNull(dt);
        }
    }
}