using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class MockLISController : Controller
    {
        private CLSDB db = new CLSDB();

        [HttpPost]
        public ActionResult NhanKetQuaTuMay(string maKetQua)
        {
            try
            {
                var ketQua = db.GetThongTinChiTietCLS(maKetQua);
                if (ketQua == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy yêu cầu xét nghiệm/siêu âm này." });
                }

                if (ketQua.TrangThai == "Đã có kết quả")
                {
                    return Json(new { success = false, message = "Dịch vụ đã có kết quả." });
                }

                string noiDungSinhRa = "";
                Random rnd = new Random();
                string tenDV = ketQua.TenDichVu?.ToLower() ?? "";

                if (tenDV.Contains("máu") || tenDV.Contains("huyết") || tenDV.Contains("đường"))
                {
                    double rbc = Math.Round(rnd.NextDouble() * (6.0 - 4.0) + 4.0, 2);
                    double wbc = Math.Round(rnd.NextDouble() * (10.0 - 4.0) + 4.0, 2);
                    double plt = Math.Round(rnd.NextDouble() * (400 - 150) + 150, 0);
                    
                    noiDungSinhRa = $@"
                        <table class='table table-sm table-bordered mt-2'>
                            <thead class='bg-light'>
                                <tr><th>Chỉ số</th><th>Kết quả đo được</th><th>Khoảng tham chiếu</th></tr>
                            </thead>
                            <tbody>
                                <tr><td>Hồng cầu (RBC)</td><td><strong class='text-primary'>{rbc}</strong> T/L</td><td>4.0 - 6.0 T/L</td></tr>
                                <tr><td>Bạch cầu (WBC)</td><td><strong class='text-primary'>{wbc}</strong> G/L</td><td>4.0 - 10.0 G/L</td></tr>
                                <tr><td>Tiểu cầu (PLT)</td><td><strong class='text-primary'>{plt}</strong> G/L</td><td>150 - 400 G/L</td></tr>
                            </tbody>
                        </table>
                        <div class='mt-2'><strong>Kết luận LIS:</strong> Các chỉ số huyết học nằm trong giới hạn bình thường.</div>";
                }
                else if (tenDV.Contains("x-quang") || tenDV.Contains("xquang") || tenDV.Contains("ct"))
                {
                    noiDungSinhRa = "<ul class='mt-2'><li><strong>Kết quả chẩn đoán hình ảnh:</strong> Hình ảnh các cơ quan bình thường, không thấy tổn thương ngoại khu hoặc cấu trúc bất thường.</li></ul>";
                }
                else if (tenDV.Contains("siêu âm"))
                {
                    noiDungSinhRa = "<ul class='mt-2'><li><strong>Kết quả siêu âm 4D:</strong> Tổ chức mô kích thước bình thường, nhu mô đều, không phát hiện khối choán chỗ bất thường.</li></ul>";
                }
                else
                {
                    noiDungSinhRa = "<p class='mt-2 text-success'>Các chỉ số phân tích sinh hoá cơ bản đo lường ổn định.</p>";
                }

                bool IsUpdated = db.CapNhatKetQuaTuLIS(maKetQua, noiDungSinhRa, ketQua.MaPhieuKhamBenh, "", "", null, null);
                
                if (IsUpdated)
                {
                    return Json(new { success = true, message = "Lấy kết quả từ mô phỏng LIS thành công!", data = noiDungSinhRa });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật kết quả vào cơ sở dữ liệu." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống khi tương tác máy đo: " + ex.Message });
            }
        }
    }
}
