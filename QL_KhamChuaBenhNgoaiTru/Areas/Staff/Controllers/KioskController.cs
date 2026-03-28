using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}