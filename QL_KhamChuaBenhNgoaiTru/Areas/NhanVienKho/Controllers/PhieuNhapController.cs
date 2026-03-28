using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Areas.NhanVienKho.Controllers
{
    public class PhieuNhapController : BaseNhanVienKhoController
    {
        private readonly KhoNhapDB db = new KhoNhapDB();

        // ===================== INDEX =====================
        public ActionResult Index(int page = 1, string keyword = "", string trangThai = "")
        {
            ViewBag.Title = "Phiếu nhập kho";

            int pageSize = 10;

            try
            {
                var dsPhieuNhap = db.GetPhieuNhap(page, pageSize, keyword, trangThai);
                int totalCount = db.GetPhieuNhapCount(keyword, trangThai);

                ViewBag.DsPhieuNhap = dsPhieuNhap;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                ViewBag.Keyword = keyword;
                ViewBag.TrangThai = trangThai;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // ===================== CREATE =====================
        public ActionResult Create()
        {
            ViewBag.Title = "Tạo phiếu nhập kho";

            try
            {
                ViewBag.DsNhaCungCap = db.GetAllNhaCungCap();
                ViewBag.DsThuoc = db.GetAllThuoc();
                ViewBag.DsKho = db.GetAllKho();
                ViewBag.DsPhong = db.GetAllPhongKho();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            try
            {
                string maNSXStr = form["MaNSX"];
                string maKhoStr = form["MaKho"];
                string ghiChu = form["GhiChu"];

                if (string.IsNullOrEmpty(maNSXStr) || !int.TryParse(maNSXStr, out int maNSX))
                {
                    TempData["Error"] = "Vui lòng chọn nhà cung cấp.";
                    return RedirectToAction("Create");
                }

                if (string.IsNullOrEmpty(maKhoStr) || !int.TryParse(maKhoStr, out int maKho))
                {
                    TempData["Error"] = "Vui lòng chọn kho nhập hàng.";
                    return RedirectToAction("Create");
                }

                var chiTiet = new List<KhoNhapDB.CT_PhieuNhap_Insert>();

                var thuocKeys = form.AllKeys.Where(k => k != null && k.StartsWith("MaThuoc_")).ToList();

                foreach (var key in thuocKeys)
                {
                    var idx = key.Replace("MaThuoc_", "");
                    var maThuoc = form[key];
                    var maLo = form["MaLo_" + idx];
                    var soLuongStr = form["SoLuong_" + idx];
                    var donGiaStr = form["DonGia_" + idx];
                    var ngaySXStr = form["NgaySanXuat_" + idx];
                    var hanSDStr = form["HanSuDung_" + idx];

                    if (string.IsNullOrWhiteSpace(maThuoc)) continue;

                    if (string.IsNullOrWhiteSpace(soLuongStr) || !int.TryParse(soLuongStr, out int soLuong) || soLuong <= 0)
                    {
                        TempData["Error"] = $"Số lượng dòng #{idx} không hợp lệ.";
                        return RedirectToAction("Create");
                    }

                    if (string.IsNullOrWhiteSpace(donGiaStr) || !decimal.TryParse(donGiaStr, out decimal donGia) || donGia < 0)
                    {
                        TempData["Error"] = $"Đơn giá dòng #{idx} không hợp lệ.";
                        return RedirectToAction("Create");
                    }

                    var ct = new KhoNhapDB.CT_PhieuNhap_Insert
                    {
                        MaThuoc = maThuoc.Trim(),
                        MaLo = maLo?.Trim() ?? "",
                        SoLuongNhap = soLuong,
                        DonGiaNhap = donGia
                    };

                    if (!string.IsNullOrWhiteSpace(ngaySXStr) && DateTime.TryParse(ngaySXStr, out DateTime ngaySX))
                        ct.NgaySanXuat = ngaySX;

                    if (!string.IsNullOrWhiteSpace(hanSDStr) && DateTime.TryParse(hanSDStr, out DateTime hanSD))
                        ct.HanSuDung = hanSD;

                    chiTiet.Add(ct);
                }

                if (chiTiet.Count == 0)
                {
                    TempData["Error"] = "Vui lòng nhập ít nhất một chi tiết phiếu nhập.";
                    return RedirectToAction("Create");
                }

                string maNV = Session["MaNV"]?.ToString();
                bool result = db.CreatePhieuNhap(maNV, maNSX, maKho, ghiChu, chiTiet);

                if (result)
                {
                    TempData["Success"] = "Tạo phiếu nhập kho thành công! Vui lòng chờ quản lý duyệt.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Không thể tạo phiếu nhập. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Create");
        }

        // ===================== DETAILS =====================
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Chi tiết phiếu nhập";

            try
            {
                var phieu = db.GetPhieuNhapById(id);
                if (phieu == null)
                {
                    return HttpNotFound("Không tìm thấy phiếu nhập!");
                }

                var chiTiet = db.GetCT_PhieuNhap(id);

                ViewBag.Phieu = phieu;
                ViewBag.ChiTiet = chiTiet;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // ===================== HỦY PHIẾU NHẬP =====================
        public ActionResult HuyPhieu(int id)
        {
            try
            {
                string maNV = Session["MaNV"]?.ToString();
                if (string.IsNullOrEmpty(maNV))
                {
                    TempData["Error"] = "Không xác định được nhân viên.";
                    return RedirectToAction("Index");
                }

                var phieu = db.GetPhieuNhapById(id);
                if (phieu == null)
                {
                    TempData["Error"] = "Không tìm thấy phiếu nhập.";
                    return RedirectToAction("Index");
                }

                if (phieu.TrangThai != "Chờ duyệt")
                {
                    TempData["Error"] = "Chỉ có thể hủy phiếu đang ở trạng thái 'Chờ duyệt'.";
                    return RedirectToAction("Index");
                }

                bool ok = db.HuyPhieuNhap(id, maNV);
                if (ok)
                    TempData["Success"] = "Đã hủy phiếu nhập #" + id + " thành công!";
                else
                    TempData["Error"] = "Bạn chỉ có thể hủy phiếu do mình tạo.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
