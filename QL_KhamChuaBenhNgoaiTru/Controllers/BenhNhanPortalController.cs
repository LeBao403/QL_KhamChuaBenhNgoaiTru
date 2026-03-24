using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;

using BenhNhanModel = QL_KhamChuaBenhNgoaiTru.Models.BenhNhan;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class BenhNhanPortalController : Controller
    {
        private readonly BenhNhanPortalDB db = new BenhNhanPortalDB();
        TiepTanDB TTdb = new TiepTanDB();

        // ==================== 0. TRANG CHỦ / DASHBOARD ====================
        public ActionResult Index()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var dashboard = db.GetDashboard(bn.MaBN);
            ViewBag.Profile = db.GetBenhNhanByMaBN(bn.MaBN);
            return View(dashboard);
        }

        // ==================== 1. XEM & CẬP NHẬT THÔNG TIN CÁ NHÂN ====================
        public ActionResult ThongTinCaNhan()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var profile = db.GetBenhNhanByMaBN(bn.MaBN);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinCaNhan(BenhNhanPortalDB.BenhNhanProfile model)
        {
            var bnSession = Session["BenhNhan"] as BenhNhanModel;
            if (bnSession == null) return RedirectToAction("Login", "TaiKhoan");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                model.MaBN = bnSession.MaBN;
                bool ok = db.UpdateBenhNhan(model);
                if (ok)
                {
                    bnSession.HoTen = model.HoTen;
                    bnSession.SDT = model.SDT;
                    bnSession.Email = model.Email;
                    bnSession.NgaySinh = model.NgaySinh ?? DateTime.MinValue;
                    bnSession.GioiTinh = model.GioiTinh;
                    bnSession.DiaChi = model.DiaChi;
                    bnSession.CCCD = model.CCCD;
                    Session["BenhNhan"] = bnSession;

                    TempData["Success"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("ThongTinCaNhan");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return View(model);
        }

        // ==================== 2. ĐẶT LỊCH KHÁM ====================
        public ActionResult DatLichKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            // 1. Tận dụng hàm của Tiếp Tân để lấy danh sách Dịch Vụ
            var tiepTanDb = new QL_KhamChuaBenhNgoaiTru.DBContext.TiepTanDB();
            var dtDichVu = tiepTanDb.GetDanhSachDichVuKham();

            var listDV = new List<SelectListItem>();
            foreach (System.Data.DataRow row in dtDichVu.Rows)
            {
                listDV.Add(new SelectListItem
                {
                    Value = row["MaDV"].ToString(),
                    Text = $"{row["TenDV"]} - {Convert.ToDecimal(row["GiaDichVu"]).ToString("N0")} VNĐ"
                });
            }
            ViewBag.ListDichVu = listDV;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatLichKham(DateTime ngayKham, string maDV, string lyDo)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            if (ngayKham < DateTime.Today)
            {
                TempData["Error"] = "Ngày khám không được nhỏ hơn ngày hiện tại.";
                return RedirectToAction("DatLichKham");
            }

            try
            {
                int maPhieu = db.DatLichKham(bn.MaBN, ngayKham, maDV, lyDo);

                TempData["Success"] = "Đặt lịch khám thành công! Mã phiếu: " + maPhieu;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi đặt lịch: " + ex.Message;
            }

            return RedirectToAction("DatLichKham");
        }

        // ==================== 3. XEM / HỦY LỊCH KHÁM ====================
        public ActionResult LichKham(string trangThai = "")
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var lichKham = db.GetLichKhamByMaBN(bn.MaBN);

            if (!string.IsNullOrEmpty(trangThai))
            {
                if (trangThai == "cho")
                    lichKham = lichKham.FindAll(x => x.TrangThai == "Chờ xử lý");
                else if (trangThai == "xacnhan")
                    lichKham = lichKham.FindAll(x => x.TrangThai == "Đã xác nhận");
                else if (trangThai == "huy")
                    lichKham = lichKham.FindAll(x => x.TrangThai == "Hủy");
            }

            return View(lichKham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyLich(int id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            try
            {
                bool ok = db.HuyLichKham(id, bn.MaBN);
                if (ok)
                    TempData["Success"] = "Hủy lịch khám thành công!";
                else
                    TempData["Error"] = "Không thể hủy lịch. Chỉ lịch ở trạng thái 'Chờ xử lý' mới được hủy.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("LichKham");
        }

        // ==================== 4. TRẠNG THÁI KHÁM ====================
        public ActionResult TrangThaiKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetTrangThaiKhamByMaBN(bn.MaBN);
            return View(list);
        }

        // ==================== 5. LỊCH SỬ KHÁM ====================
        public ActionResult LichSuKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetLichSuKhamByMaBN(bn.MaBN);
            return View(list);
        }

        // ==================== 6. ĐƠN THUỐC ====================
        public ActionResult DonThuoc()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetDonThuocByMaBN(bn.MaBN);
            return View(list);
        }

        public ActionResult ChiTietDonThuoc(int id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var detail = db.GetChiTietDonThuoc(id);
            if (detail == null || detail.ChiTiet.Count == 0)
                return HttpNotFound("Không tìm thấy đơn thuốc!");

            return View(detail);
        }

        // ==================== 7. HÓA ĐƠN ====================
        public ActionResult HoaDon()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var list = db.GetHoaDonByMaBN(bn.MaBN);
            return View(list);
        }

        public ActionResult ChiTietHoaDon(int id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return RedirectToAction("Login", "TaiKhoan");

            var detail = db.GetChiTietHoaDon(id);
            if (detail == null)
                return HttpNotFound("Không tìm thấy hóa đơn!");

            return View(detail);
        }
    }
}
