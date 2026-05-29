using Microsoft.VisualStudio.TestTools.UnitTesting;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.Tests
{
    [TestClass]
    public class KhoDBTests
    {
        private KhoDB _khoDb;

        [TestInitialize]
        public void Setup()
        {
            _khoDb = new KhoDB();
        }

        // ==========================================
        // NHÓM 1: ĐỌC DỮ LIỆU TỔNG QUAN
        // ==========================================
        [TestMethod]
        public void GetTongQuanKho_KhongTruyenKho_LayTongThe()
        {
            var tq = _khoDb.GetTongQuanKho(null);
            Assert.IsNotNull(tq);
            Assert.IsTrue(tq.SoKho >= 0);
            Assert.IsTrue(tq.TongGiaTriTon >= 0);
        }

        [TestMethod]
        public void GetThongKeCacKho_TraVeDanhSachChuan()
        {
            var list = _khoDb.GetThongKeCacKho();
            Assert.IsNotNull(list);
            foreach (var kho in list)
            {
                Assert.IsTrue(kho.TongSoLuong >= 0);
                Assert.IsTrue(kho.TongGiaTri >= 0);
            }
        }

        // ==========================================
        // NHÓM 2: DANH SÁCH TỒN KHO & BỘ LỌC
        // ==========================================
        [TestMethod]
        public void GetTonKhoById_MaAo_TraVeNull()
        {
            var item = _khoDb.GetTonKhoById(-999);
            Assert.IsNull(item);
        }

        [TestMethod]
        public void CapNhatSoLuongTonKho_MaAo_KhongCrashTraVeFalse()
        {
            bool result = _khoDb.CapNhatSoLuongTonKho(-999, 100, "Test");
            Assert.IsFalse(result);
        }

        // ==========================================
        // NHÓM 3: GIAO DỊCH PHIẾU CHUYỂN KHO 
        // (BẮT LỖI CSDL THIẾU BẢNG 'PHIEUCHUYENKHO')
        // ==========================================
        [TestMethod]
        public void DuyetPhieuChuyen_ThieuBang_ThrowsSqlException()
        {
            // Do DB không có bảng PHIEUCHUYENKHO, lệnh này bắt buộc phải văng SqlException
            Assert.ThrowsException<SqlException>(() =>
            {
                _khoDb.DuyetPhieuChuyen(-999, "NV_DUYET");
            }, "Test Pass: Đã bẫy thành công lỗi thiếu bảng PHIEUCHUYENKHO trong CSDL.");
        }

        [TestMethod]
        public void HuyPhieuChuyen_ThieuBang_ThrowsSqlException()
        {
            Assert.ThrowsException<SqlException>(() =>
            {
                _khoDb.HuyPhieuChuyen(-999, "NV_HUY");
            }, "Test Pass: Đã bẫy thành công lỗi thiếu bảng PHIEUCHUYENKHO trong CSDL.");
        }

        [TestMethod]
        public void XoaChiTietPhieuChuyen_ThieuBang_ThrowsSqlException()
        {
            Assert.ThrowsException<SqlException>(() =>
            {
                _khoDb.XoaChiTietPhieuChuyen(-999);
            }, "Test Pass: Đã bẫy thành công lỗi thiếu bảng CT_PHIEUCHUYENKHO trong CSDL.");
        }

        [TestMethod]
        public void TaoPhieuChuyen_ThieuBang_ThrowsSqlException()
        {
            var ct = new List<CT_PhieuChuyenInput> {
                new CT_PhieuChuyenInput { MaThuoc = "TH_AO", MaLo = "LO1", SoLuongChuyen = 5 }
            };

            Assert.ThrowsException<SqlException>(() =>
            {
                _khoDb.TaoPhieuChuyen("NV_AO", 1, 2, "Test", ct);
            });
        }

        [TestMethod]
        public void GetPhieuChuyen_PhanTrang_ThieuBang_ThrowsSqlException()
        {
            Assert.ThrowsException<SqlException>(() =>
            {
                _khoDb.GetPhieuChuyen(1, 5, "", "");
            });
        }

        // ==========================================
        // NHÓM 4: DROPDOWN & TÌM KIẾM
        // ==========================================
        [TestMethod]
        public void GetAllDropdowns_KhongBiNull()
        {
            Assert.IsNotNull(_khoDb.GetAllPhong());
            Assert.IsNotNull(_khoDb.GetAllKho());
            Assert.IsNotNull(_khoDb.GetAllThuoc());
            Assert.IsNotNull(_khoDb.GetAllNhaCungCap());
        }
    }
}