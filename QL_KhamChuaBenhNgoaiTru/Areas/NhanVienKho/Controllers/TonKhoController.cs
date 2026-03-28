using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Areas.NhanVienKho.Controllers
{
    public class TonKhoController : BaseNhanVienKhoController
    {
        private readonly KhoNhapDB db = new KhoNhapDB();

        public ActionResult Index(int page = 1, string keyword = "", string maKho = "", string trangThaiTon = "")
        {
            ViewBag.Title = "Tồn kho";

            int pageSize = 15;

            try
            {
                var dsTonKho = db.GetTonKho(page, pageSize, keyword, maKho, trangThaiTon);
                int totalCount = db.GetTonKhoCount(keyword, maKho, trangThaiTon);

                ViewBag.DsTonKho = dsTonKho;
                ViewBag.DsKho = db.GetAllKho();
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                ViewBag.Keyword = keyword;
                ViewBag.MaKho = maKho;
                ViewBag.TrangThaiTon = trangThaiTon;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // ===================== CHI TIẾT TỒN KHO =====================
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Chi tiết tồn kho";

            try
            {
                var item = db.GetTonKhoById(id);
                if (item == null)
                    return HttpNotFound("Không tìm thấy lô thuốc trong kho!");

                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        // ===================== ĐIỀU CHỈNH SỐ LƯỢNG TỒN =====================
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Điều chỉnh số lượng tồn kho";

            try
            {
                var item = db.GetTonKhoById(id);
                if (item == null)
                    return HttpNotFound("Không tìm thấy lô thuốc trong kho!");

                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection form)
        {
            try
            {
                string soLuongStr = form["SoLuongMoi"];
                if (!int.TryParse(soLuongStr, out int soLuongMoi) || soLuongMoi < 0)
                {
                    TempData["Error"] = "Số lượng không hợp lệ.";
                    return RedirectToAction("Edit", new { id });
                }

                bool ok = db.UpdateTonKhoSoLuong(id, soLuongMoi);
                if (ok)
                    TempData["Success"] = "Cập nhật số lượng tồn kho thành công!";
                else
                    TempData["Error"] = "Không tìm thấy lô thuốc để cập nhật.";

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
