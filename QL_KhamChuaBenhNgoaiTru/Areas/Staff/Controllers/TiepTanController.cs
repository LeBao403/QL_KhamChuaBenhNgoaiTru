using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System.Linq;

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
        public ActionResult Index(string tuNgay, string denNgay)
        {
            var nv = Session["NhanVien"] as NhanVien;

            if (nv == null || nv.MaPhong == null || nv.MaPhong <= 0)
            {
                ViewBag.LoiPhanPhong = "Tài khoản của bạn chưa được phân công ngồi trực tại Quầy Tiếp Tân nào. Vui lòng liên hệ Admin để cập nhật thông tin!";
                return View();
            }

            // Xử lý Ngày tháng (Mặc định là Hôm nay)
            DateTime dtTuNgay = DateTime.Now.Date;
            DateTime dtDenNgay = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParse(tuNgay, out dtTuNgay);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParse(denNgay, out dtDenNgay);

            // Truyền lại ra View để binding vào input date
            ViewBag.TuNgay = dtTuNgay.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = dtDenNgay.ToString("yyyy-MM-dd");

            int maQuayHienTai = nv.MaPhong.Value;
            ViewBag.TenQuayHienTai = db.GetTenPhong(maQuayHienTai);

            // Truyền tham số ngày vào 2 hàm
            ViewBag.DanhSachOffline = db.GetDanhSachOffline(maQuayHienTai, dtTuNgay, dtDenNgay);
            ViewBag.DanhSachOnline = db.GetDanhSachOnline(maQuayHienTai, dtTuNgay, dtDenNgay);

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

        // GET: Staff/TiepTan/LichSuTiepNhan
        public ActionResult LichSuTiepNhan(string tuNgay, string denNgay, string tuKhoa, int page = 1)
        {
            var nv = Session["NhanVien"] as NhanVien;
            if (nv == null || nv.MaPhong == null || nv.MaPhong <= 0)
            {
                return RedirectToAction("Index"); // Đẩy về trang chủ nếu lỗi phân quyền
            }

            // Xử lý Ngày Xem (Mặc định là hôm nay)
            DateTime dtTuNgay = DateTime.Now.Date;
            DateTime dtDenNgay = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParse(tuNgay, out dtTuNgay);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParse(denNgay, out dtDenNgay);

            ViewBag.TuNgay = dtTuNgay.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = dtDenNgay.ToString("yyyy-MM-dd");
            ViewBag.TuKhoa = tuKhoa; // Giữ lại từ khóa trên ô tìm kiếm

            int maQuayHienTai = nv.MaPhong.Value;
            ViewBag.TenQuayHienTai = db.GetTenPhong(maQuayHienTai);

            // 1. LẤY FULL DATA TỪ SQL
            DataTable fullDt = db.GetLichSuTiepNhan(dtTuNgay, dtDenNgay);

            // 2. TÌM KIẾM (SEARCH) TRONG C# NẾU CÓ TỪ KHÓA
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                string keyword = tuKhoa.ToLower().Trim();
                var filteredRows = fullDt.AsEnumerable().Where(r =>
                    r["MaBN"].ToString().ToLower().Contains(keyword) ||
                    r["HoTen"].ToString().ToLower().Contains(keyword) ||
                    r["SDT"].ToString().ToLower().Contains(keyword)
                );

                if (filteredRows.Any())
                {
                    fullDt = filteredRows.CopyToDataTable();
                }
                else
                {
                    fullDt = fullDt.Clone(); // Trả về bảng rỗng nếu không tìm thấy
                }
            }

            // 3. THUẬT TOÁN PHÂN TRANG (PAGINATION)
            int pageSize = 15; // Số dòng trên 1 trang
            int totalRows = fullDt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            DataTable pagedDt = fullDt.Clone(); // Tạo bảng rỗng cùng cấu trúc
            if (totalRows > 0)
            {
                // Cắt khúc dữ liệu: Bỏ qua các trang trước -> Lấy 15 dòng của trang hiện tại
                pagedDt = fullDt.AsEnumerable().Skip((page - 1) * pageSize).Take(pageSize).CopyToDataTable();
            }

            // Đẩy xuống View
            ViewBag.LichSuTiepNhan = pagedDt;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRows = totalRows;

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