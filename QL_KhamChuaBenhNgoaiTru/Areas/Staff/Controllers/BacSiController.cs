using QL_KhamChuaBenhNgoaiTru.Models;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    public class BacSiController : BaseStaffController
    {
        private BacSiDB db = new BacSiDB();
        private static readonly string MlApiUrl = "http://127.0.0.1:8000/recommend";

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
        // [ĐÃ SỬA] Đổi int maPhieu -> string maPhieu
        public JsonResult TiepNhan(string maPhieu)
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
        // [ĐÃ SỬA] Đổi int maPhieu -> string maPhieu
        public JsonResult ChiTietDangKham(string maPhieu)
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
        // [ĐÃ SỬA] Đổi int maPhieu -> string maPhieu
        public JsonResult GetKetQuaCLS(string maPhieu)
        {
            var list = db.GetKetQuaCLS(maPhieu);
            return Json(list);
        }

        [HttpPost]
        public ActionResult LuuKhamBenh(KhamBenhViewModel model)
        {
            string maBS = Session["MaNV"]?.ToString() ?? "NV001";

            if (string.IsNullOrEmpty(model.MaPhieuKhamBenh)) return RedirectToAction("Index");

            string errorMsg = "";
            bool result = db.LuuKhamBenh(model, maBS, out errorMsg);

            if (result)
                TempData["SuccessMsg"] = model.YeuCauCanLamSang ? "Đã chuyển bệnh nhân đi Cận Lâm Sàng." : "Hoàn tất khám và kê đơn!";
            else
                TempData["ErrorMsg"] = "Lỗi khi lưu dữ liệu. Chi tiết: " + errorMsg;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GoiYThuoc(string trieuChung, string ketLuan, int? tuoi, string gioiTinh, string maPhieu)
        {
            try
            {
                var dsThuoc = db.GetDanhSachThuoc();
                bool hasBhyt = false;
                if (!string.IsNullOrEmpty(maPhieu))
                {
                    dynamic info = db.GetThongTinChiTiet(maPhieu);
                    if (info != null)
                    {
                        hasBhyt = info.BHYT;
                    }
                }

                var req = new
                {
                    age = tuoi ?? 30,
                    sex = string.IsNullOrWhiteSpace(gioiTinh) ? "Nam" : gioiTinh,
                    age_group = (tuoi ?? 30) < 13 ? "TreEm" : (tuoi ?? 30) < 18 ? "ThieuNien" : ((tuoi ?? 30) < 40 ? "NguoiLonTre" : ((tuoi ?? 30) < 60 ? "TrungNien" : "NguoiCaoTuoi")),
                    disease = string.IsNullOrWhiteSpace(ketLuan) ? "Cảm cúm" : ketLuan,
                    symptoms = string.IsNullOrWhiteSpace(trieuChung) ? "sốt | ho" : trieuChung,
                    comorbidity = "None",
                    allergy = "None",
                    has_bhyt = hasBhyt,
                    candidates = new List<object>()
                };

                foreach (var t in dsThuoc)
                {
                    req.candidates.Add(new
                    {
                        drug_code = t.MaThuoc,
                        drug_name = t.TenThuoc,
                        drug_group = t.MaLoaiThuoc ?? "Khac",
                        route = t.DuongDung ?? t.DonViCoBan ?? "Uống",
                        drug_family = string.Format("{0} {1}", t.TenThuoc, t.DonViCoBan),
                        in_stock = 1,
                        contraindication = 0,
                        interaction_risk = 0,
                        is_bhyt = t.CoBHYT ? 1 : 0
                    });
                }

                var json = System.Web.Helpers.Json.Encode(req);
                var response = PostJson(MlApiUrl, json);
                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // --- TÍNH NĂNG TRA CỨU HỒ SƠ ---
        [HttpGet]
        public JsonResult LayDanhSachBenhNhan()
        {
            try
            {
                var data = db.GetAllBenhNhan();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LayLichSuKham(string maBN)
        {
            try
            {
                var data = db.GetLichSuKhamToanDien(maBN);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private static string PostJson(string url, string jsonBody)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            var bytes = Encoding.UTF8.GetBytes(jsonBody);
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}