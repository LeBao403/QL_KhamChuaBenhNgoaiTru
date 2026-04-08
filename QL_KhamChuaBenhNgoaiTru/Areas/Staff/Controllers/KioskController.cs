using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Staff.Controllers
{
    public class KioskController : Controller
    {
        TiepTanDB db = new TiepTanDB();

        // Trang màn hình chờ Kiosk (Không cần đăng nhập)
        public ActionResult Lobby()
        {
            return View();
        }

        [HttpPost]
        public JsonResult XuLyQuetThe(BenhNhan model, string loaiThe)
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

        [HttpPost]
        public JsonResult TaoBenhNhanFake()
        {
            try
            {
                string token = DateTime.Now.ToString("yyMMddHHmmssfff");
                string cccd = token.PadLeft(12, '0');
                string sdt = "09" + token.Substring(Math.Max(0, token.Length - 8), 8);

                var bnFake = new BenhNhan
                {
                    HoTen = "TEST KIOSK " + DateTime.Now.ToString("HHmmss"),
                    CCCD = cccd,
                    SDT = sdt,
                    NgaySinh = new DateTime(1995, 1, 1),
                    GioiTinh = "Nam",
                    DiaChi = "Dia chi test kiosk",
                    BHYT = false
                };

                var result = db.DangKyKhamTuThe(bnFake, "CCCD");
                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        stt = result.STT,
                        maBN = result.MaBN,
                        tenBN = result.TenBN,
                        tenPhong = result.TenPhong,
                        message = "Tạo bệnh nhân fake thành công"
                    });
                }

                return Json(new { success = false, message = "Lỗi Database: " + result.ErrorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi máy chủ: " + ex.Message });
            }
        }
    }
}
