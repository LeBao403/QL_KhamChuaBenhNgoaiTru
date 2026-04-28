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

            // ==============================================================================
            // 1. XỬ LÝ LỖI NHÂN ĐÔI ID (Cắt bỏ phần bị trùng lặp do MVC Model Binder)
            // ==============================================================================
            if (!string.IsNullOrEmpty(model.MaPhieuKhamBenh) && model.MaPhieuKhamBenh.Contains(","))
            {
                model.MaPhieuKhamBenh = model.MaPhieuKhamBenh.Split(',')[0].Trim();
            }

            if (string.IsNullOrEmpty(model.MaPhieuKhamBenh)) return RedirectToAction("Index");

            // ==============================================================================
            // 2. KHẮC PHỤC TRIỆT ĐỂ LỖI SINH HIỆU & ÉP KIỂU CULTURE (vi-VN / en-US)
            // ==============================================================================
            try
            {
                // Hàm lấy số an toàn
                decimal? GetNumberSafe(string key)
                {
                    var val = Request.Form[key];
                    if (string.IsNullOrEmpty(val)) return null;

                    if (val.Contains(", "))
                        val = val.Split(new[] { ", " }, StringSplitOptions.None)[0];
                    else if (val.Contains(",") && val.Split(',').Length > 2)
                        val = val.Split(',')[0];

                    val = val.Replace(",", ".").Trim();

                    if (decimal.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsedValue))
                        return parsedValue;

                    return null;
                }

                var spo2 = GetNumberSafe("SpO2");
                if (spo2.HasValue) model.SpO2 = (int)Math.Round(spo2.Value);

                var nt = GetNumberSafe("NhipTho");
                if (nt.HasValue) model.NhipTho = (int)Math.Round(nt.Value);

                var mach = GetNumberSafe("Mach");
                if (mach.HasValue) model.Mach = (int)Math.Round(mach.Value);

                var haThu = GetNumberSafe("HuyetApTamThu");
                if (haThu.HasValue) model.HuyetApTamThu = (int)Math.Round(haThu.Value);

                var haTruong = GetNumberSafe("HuyetApTamTruong");
                if (haTruong.HasValue) model.HuyetApTamTruong = (int)Math.Round(haTruong.Value);

                var cc = GetNumberSafe("ChieuCao");
                if (cc.HasValue) model.ChieuCao = cc.Value;

                var cn = GetNumberSafe("CanNang");
                if (cn.HasValue) model.CanNang = cn.Value;

                var nd = GetNumberSafe("NhietDo");
                if (nd.HasValue) model.NhietDo = nd.Value;
            }
            catch { /* Bỏ qua lỗi nếu có ngoại lệ bất ngờ */ }

            // ==============================================================================
            // 3. THỰC HIỆN LƯU DỮ LIỆU BẰNG HÀM GỐC TRONG BACSIDB
            // ==============================================================================
            string errorMsg = "";
            bool result = db.LuuKhamBenh(model, maBS, out errorMsg);

            if (result)
                TempData["SuccessMsg"] = model.YeuCauCanLamSang ? "Đã chuyển bệnh nhân đi Cận Lâm Sàng." : "Hoàn tất khám và kê đơn!";
            else
                TempData["ErrorMsg"] = "Lỗi khi lưu dữ liệu. Chi tiết: " + errorMsg;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GoiYThuocTheoBenh(string maBenh)
        {
            try
            {
                if (string.IsNullOrEmpty(maBenh))
                    return Json(new { GoiY = new object[0] }, JsonRequestBehavior.AllowGet);

                // Gọi sang API của Python ở cổng 8000
                string apiUrl = "http://127.0.0.1:8000/api/recommend?node_id=" + maBenh;
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(apiUrl);
                request.Method = "GET";

                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();
                    // Trả thẳng nguyên file JSON của Python về cho View xử lý
                    return Content(jsonResponse, "application/json");
                }
            }
            catch (System.Exception)
            {
                // Trả về mảng rỗng nếu Python bị tắt hoặc lỗi
                return Json(new { GoiY = new object[0] }, JsonRequestBehavior.AllowGet);
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