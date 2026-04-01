using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    public class CLSController : Controller
    {
        private readonly CLSDB db = new CLSDB();

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

        public ActionResult LichSu()
        {
            try
            {
                var lichSu = db.GetLichSuXetNghiem(300);
                return View(lichSu);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi lấy lịch sử xét nghiệm: " + ex.Message;
                return View(new System.Collections.Generic.List<QL_KhamChuaBenhNgoaiTru.Models.KetQuaCLS>());
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
        public JsonResult XacNhanKetQua(int maKetQua, string noiDung, int maPhieuKhamBenh, string mauXN, string chatLuong)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
                return Json(new { success = false, message = "Vui lòng nhập kết quả." });

            string maBS = Session["MaNV"]?.ToString() ?? "NV001";
            string fileKetQua = "KQCLS_" + maKetQua + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";

            bool res = db.CapNhatKetQuaTuLIS(maKetQua, noiDung, maPhieuKhamBenh, maBS, fileKetQua, mauXN, chatLuong);
            if (res) return Json(new { success = true, fileKetQua = fileKetQua, maBacSi = maBS });
            return Json(new { success = false, message = "Lỗi khi lưu kết quả." });
        }
    }
}
