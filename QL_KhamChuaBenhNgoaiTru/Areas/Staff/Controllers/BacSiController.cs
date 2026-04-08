using QL_KhamChuaBenhNgoaiTru.Models;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    public class BacSiController : BaseStaffController
    {
        private BacSiDB db = new BacSiDB();

        public ActionResult Index()
        {
            string maBS = Session["MaNV"]?.ToString() ?? "NV001";

            ViewBag.ChoKham = db.GetDanhSachPhieuKham(maBS, "Chờ khám");
            ViewBag.DangKham = db.GetDanhSachPhieuKham(maBS, "Đang khám");
            ViewBag.DaKham = db.GetDanhSachPhieuKham(maBS, "Hoàn thành");
            ViewBag.DanhSachBenh = db.GetDanhSachBenh();
            ViewBag.DanhSachThuoc = db.GetDanhSachThuoc();
            ViewBag.DanhSachDichVu = db.GetDanhSachDichVuCLS(); // Load List CLS

            return View();
        }

        [HttpPost]
        public JsonResult TiepNhan(int maPhieu)
        {
            // Lấy mã bác sĩ đang đăng nhập
            string maBS = Session["MaNV"]?.ToString() ?? "NV001";

            // Gọi hàm TiepNhan cũ để update CSDL
            db.TiepNhan(maPhieu, maBS);

            // Lấy data xịn có đầy đủ địa chỉ, SĐT trả về View
            var info = db.GetThongTinChiTiet(maPhieu);
            if (info != null) return Json(new { success = true, Data = info });
            return Json(new { success = false });
        }

        // MỚI: API Lấy thông tin bệnh nhân đang khám
        [HttpPost]
        public JsonResult ChiTietDangKham(int maPhieu)
        {
            // Sử dụng hàm GetThongTinChiTiet thay cho hàm cũ
            var info = db.GetThongTinChiTiet(maPhieu);
            if (info != null) return Json(new { success = true, Data = info });
            return Json(new { success = false });
        }

        // MỚI: API Lấy danh sách phòng cho dịch vụ cụ thể
        [HttpPost]
        public JsonResult GetPhongDichVu(string maDV)
        {
            var list = db.GetPhongPhuHop(maDV);
            return Json(list);
        }

        // MỚI: API Lấy kết quả CLS của Phiếu Khám
        [HttpPost]
        public JsonResult GetKetQuaCLS(int maPhieu)
        {
            var list = db.GetKetQuaCLS(maPhieu);
            return Json(list);
        }

        [HttpPost]
        public ActionResult LuuKhamBenh(KhamBenhViewModel model)
        {
            string maBS = Session["MaNV"]?.ToString() ?? "NV001";
            if (model.MaPhieuKhamBenh == 0) return RedirectToAction("Index");

            string errorMsg = "";
            bool result = db.LuuKhamBenh(model, maBS, out errorMsg); // Truyền thêm maBS

            if (result)
                TempData["SuccessMsg"] = model.YeuCauCanLamSang ? "Đã chuyển bệnh nhân đi Cận Lâm Sàng." : "Hoàn tất khám và kê đơn!";
            else
                TempData["ErrorMsg"] = "Lỗi khi lưu dữ liệu. Chi tiết: " + errorMsg;

            return RedirectToAction("Index");
        }
    }
}