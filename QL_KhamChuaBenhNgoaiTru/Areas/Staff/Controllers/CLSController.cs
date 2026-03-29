using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Web.Mvc;
//using QL_KhamChuaBenhNgoaiTru.Helpers; // Uncomment nếu project cần Auth login để vô Staff

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    // [StaffAuthorize(Roles = "1,3")] // Tuỳ chỉnh theo file Auth của project
    public class CLSController : Controller
    {
        private CLSDB db = new CLSDB();

        // GET: Staff/CLS
        public ActionResult Index()
        {
            try
            {
                var danhSachCho = db.GetDanhSachChoThucHien();
                return View(danhSachCho);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lấy dữ liệu: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JsonResult GetChiTiet(int maKetQua)
        {
            var data = db.GetThongTinChiTietCLS(maKetQua);
            if (data != null) return Json(new { success = true, Data = data });
            return Json(new { success = false, message = "Không tìm thấy thông tin." });
        }

        [HttpPost]
        public JsonResult XacNhanKetQua(int maKetQua, string noiDung, int maPhieuKhamBenh)
        {
            if (string.IsNullOrEmpty(noiDung))
                return Json(new { success = false, message = "Vui lòng nhập kết quả." });

            bool res = db.CapNhatKetQuaTuLIS(maKetQua, noiDung, maPhieuKhamBenh);
            if (res) return Json(new { success = true });
            else return Json(new { success = false, message = "Lỗi khi lưu kết quả." });
        }
    }
}
