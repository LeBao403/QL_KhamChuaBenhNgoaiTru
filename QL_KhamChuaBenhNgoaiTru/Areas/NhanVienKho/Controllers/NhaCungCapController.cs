using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Areas.NhanVienKho.Controllers
{
    public class NhaCungCapController : BaseNhanVienKhoController
    {
        private readonly KhoNhapDB db = new KhoNhapDB();

        public ActionResult Index()
        {
            ViewBag.Title = "Nhà cung cấp";

            try
            {
                var dsNSX = db.GetAllNhaCungCapDetail();
                ViewBag.DsNSX = dsNSX;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View("Error");
            }
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Thêm nhà cung cấp";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            try
            {
                string tenNSX = form["TenNSX"]?.Trim();
                string diaChi = form["DiaChi"]?.Trim();
                string sdt = form["SDT"]?.Trim();
                string email = form["Email"]?.Trim();

                if (string.IsNullOrEmpty(tenNSX))
                {
                    TempData["Error"] = "Tên nhà cung cấp không được trống.";
                    return RedirectToAction("Create");
                }

                if (db.NhaCungCapExists(tenNSX))
                {
                    TempData["Error"] = "Nhà cung cấp '" + tenNSX + "' đã tồn tại.";
                    return RedirectToAction("Create");
                }

                bool ok = db.CreateNhaCungCap(tenNSX, diaChi, sdt, email);
                if (ok)
                {
                    TempData["Success"] = "Thêm nhà cung cấp '" + tenNSX + "' thành công!";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Không thể thêm nhà cung cấp. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Create");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Sửa nhà cung cấp";

            try
            {
                var nsx = db.GetNhaCungCapById(id);
                if (nsx == null)
                {
                    return HttpNotFound("Không tìm thấy nhà cung cấp!");
                }

                ViewBag.NSX = nsx;
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
        public ActionResult Edit(int id, FormCollection form)
        {
            try
            {
                string tenNSX = form["TenNSX"]?.Trim();
                string diaChi = form["DiaChi"]?.Trim();
                string sdt = form["SDT"]?.Trim();
                string email = form["Email"]?.Trim();

                if (string.IsNullOrEmpty(tenNSX))
                {
                    TempData["Error"] = "Tên nhà cung cấp không được trống.";
                    return RedirectToAction("Edit", new { id });
                }

                if (db.NhaCungCapExists(tenNSX, id))
                {
                    TempData["Error"] = "Nhà cung cấp '" + tenNSX + "' đã tồn tại.";
                    return RedirectToAction("Edit", new { id });
                }

                bool ok = db.UpdateNhaCungCap(id, tenNSX, diaChi, sdt, email);
                if (ok)
                {
                    TempData["Success"] = "Cập nhật nhà cung cấp '" + tenNSX + "' thành công!";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Không thể cập nhật nhà cung cấp. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Edit", new { id });
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var nsx = db.GetNhaCungCapById(id);
                if (nsx == null)
                {
                    TempData["Error"] = "Không tìm thấy nhà cung cấp.";
                    return RedirectToAction("Index");
                }

                bool ok = db.DeleteNhaCungCap(id);
                if (ok)
                    TempData["Success"] = "Xóa nhà cung cấp thành công!";
                else
                    TempData["Error"] = "Không thể xóa nhà cung cấp vì đã có phiếu nhập liên kết.";

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
