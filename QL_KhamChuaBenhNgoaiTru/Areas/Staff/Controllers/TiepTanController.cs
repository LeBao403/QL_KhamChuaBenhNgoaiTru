using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    // ĐỔI KẾ THỪA TỪ Controller SANG BaseStaffController
    public class TiepTanController : BaseStaffController
    {
        TiepTanDB db = new TiepTanDB();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext); // Luôn chạy code của BaseStaffController trước

            var nv = Session["NhanVien"] as NhanVien;

            // Chặn Bác sĩ: Chỉ Tiếp đón (8) mới được ở lại đây
            if (nv != null && nv.MaChucVu != 8)
            {
                // Nếu là Bác sĩ (3, 4) đang cố ấn vào "Tiếp nhận", điều hướng họ an toàn về lại trang Bác Sĩ
                if (nv.MaChucVu == 3 || nv.MaChucVu == 4)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "BacSi", action = "Index", area = "Staff" })
                    );
                }
            }
        }

        // GET: Staff/TiepTan/Index
        public ActionResult Index()
        {
            var nv = Session["NhanVien"] as NhanVien;

            if (nv == null || nv.MaPhong == null || nv.MaPhong <= 0)
            {
                ViewBag.LoiPhanPhong = "Tài khoản của bạn chưa được phân công ngồi trực tại Quầy Tiếp Tân nào. Vui lòng liên hệ Admin để cập nhật thông tin!";
                return View();
            }

            int maQuayHienTai = nv.MaPhong.Value;
            ViewBag.TenQuayHienTai = db.GetTenPhong(maQuayHienTai);

            // Gọi 2 hàm mới tách
            ViewBag.DanhSachOffline = db.GetDanhSachOffline(maQuayHienTai);
            ViewBag.DanhSachOnline = db.GetDanhSachOnline(maQuayHienTai);

            var dtDichVu = db.GetDanhSachDichVuKham();
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
        public JsonResult XacNhanDichVu(int maPhieuDK, string maDV, int maPhong, string lyDo)
        {
            if (maPhieuDK <= 0 || string.IsNullOrEmpty(maDV) || maPhong <= 0)
                return Json(new { success = false, message = "Vui lòng chọn đầy đủ Dịch vụ và Phòng khám!" });

            string tenPhong, tenKhoa;
            bool requirePayment; // Bổ sung biến hứng trạng thái thu tiền

            // Gọi hàm DB đã update
            PhieuDangKyResult result = db.XacNhanDichVuKham(maPhieuDK, maDV, maPhong, lyDo, out tenPhong, out tenKhoa, out requirePayment);

            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    stt = result.STT,
                    phong = tenPhong,
                    khoa = tenKhoa,
                    requirePayment = requirePayment // Quăng cờ này ra View để JavaScript xử lý UI
                });
            }
            return Json(new { success = false, message = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult ChotCapSo(int maPhieuKhamBenh)
        {
            string tenPhong, tenKhoa;
            var result = db.ChotCapSoKham(maPhieuKhamBenh, out tenPhong, out tenKhoa);

            if (result.Success)
            {
                return Json(new { success = true, stt = result.STT, phong = tenPhong, khoa = tenKhoa });
            }
            return Json(new { success = false, message = result.ErrorMessage });
        }

        [HttpPost]
        public JsonResult GetDanhSachPhong(string maDV)
        {
            var dtPhong = db.GetPhongTheoDichVu(maDV);
            var list = new List<object>();

            foreach (System.Data.DataRow row in dtPhong.Rows)
            {
                string valueStr = row["MaPhong"].ToString();
                string textStr = $"{row["TenPhong"]} (Đang chờ: {row["SoNguoiCho"]})";
                list.Add(new { Value = valueStr, Text = textStr });
            }
            return Json(list);
        }
    }
}