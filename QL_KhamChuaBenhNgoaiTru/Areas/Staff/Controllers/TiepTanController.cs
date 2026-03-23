using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    // 1. ĐỔI KẾ THỪA TỪ Controller SANG BaseStaffController
    public class TiepTanController : Controller
    {
        TiepTanDB db = new TiepTanDB();


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext); // Luôn chạy code của BaseStaffController trước

            var nv = Session["NhanVien"] as NhanVien;

            // 2. Chặn Bác sĩ: Chỉ Tiếp đón (8) mới được ở lại đây
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

        public ActionResult Lobby()
        {
            return View();
        }

        [HttpPost]
        public JsonResult XuLyQuetThe(QL_KhamChuaBenhNgoaiTru.Models.BenhNhan model, string loaiThe)
        {
            try
            {
                // Gọi hàm xử lý Transaction dưới DB
                var result = db.DangKyKhamTuThe(model, loaiThe);

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        stt = result.STT,
                        maBN = result.MaBN,
                        tenBN = result.TenBN,

                        // === BỔ SUNG DÒNG NÀY ĐỂ TRẢ TÊN QUẦY RA ===
                        tenPhong = result.TenPhong,

                        message = "Đăng ký khám thành công!"
                    });
                }
                return Json(new { success = false, message = "Lỗi Database: " + result.ErrorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi máy chủ: " + ex.Message });
            }
        }


        // GET: Staff/TiepTan/Index
        public ActionResult Index()
        {
            // Lấy thông tin nhân viên đang đăng nhập từ Session
            var nv = Session["NhanVien"] as NhanVien;

            // Nếu nhân viên có phân phòng (MaPhong != null), dùng phòng đó.
            // Nếu chưa gán hoặc bị null, mặc định giả lập là Quầy 01 (ID: 1) để test.
            int maQuayHienTai = (nv != null && nv.MaPhong != null) ? (int)nv.MaPhong : 1;

            // Lấy danh sách chờ ĐÚNG CỦA QUẦY ĐÓ
            ViewBag.DanhSachCho = db.GetDanhSachChoXyLy(maQuayHienTai);

            // Lấy danh sách dịch vụ ném vào Dropdown
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
            PhieuDangKyResult result = db.XacNhanDichVuKham(maPhieuDK, maDV, maPhong, lyDo, out tenPhong, out tenKhoa);

            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    stt = result.STT,
                    phong = tenPhong,
                    khoa = tenKhoa
                });
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
                string valueStr = row["MaPhong"].ToString(); // Bây giờ value chỉ chứa MaPhong
                string textStr = $"{row["TenPhong"]} (Đang chờ: {row["SoNguoiCho"]})";
                list.Add(new { Value = valueStr, Text = textStr });
            }
            return Json(list);
        }
    }
}