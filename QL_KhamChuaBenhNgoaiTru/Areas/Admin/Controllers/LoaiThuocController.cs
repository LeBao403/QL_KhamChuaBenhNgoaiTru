using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class LoaiThuocController : BaseAdminController
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public ActionResult Index(string q = "", int page = 1)
        {
            try
            {
                var list = SearchLoaiThuoc(q);
                int totalCount = list.Count;
                int pageSize = 10;
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                if (page < 1) page = 1;
                if (page > totalPages && totalPages > 0) page = totalPages;

                var pagedList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Keyword = q;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalWithThuoc = list.Count(x => x.SoLuongThuoc > 0);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                return View(pagedList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách loại thuốc: " + ex.Message;
                return View("Error");
            }
        }

        public ActionResult Search(string q = "", int page = 1)
        {
            var list = SearchLoaiThuoc(q);
            int totalCount = list.Count;
            int pageSize = 10;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Keyword = q;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalWithThuoc = list.Count(x => x.SoLuongThuoc > 0);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return PartialView("_LoaiThuocTable", pagedList);
        }

        public ActionResult GetStats(string q = "")
        {
            var list = SearchLoaiThuoc(q);
            return Json(new
            {
                totalCount = list.Count,
                totalWithThuoc = list.Count(x => x.SoLuongThuoc > 0)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã loại thuốc không hợp lệ");

            var item = GetLoaiThuocById(id);
            if (item == null)
                return HttpNotFound("Không tìm thấy loại thuốc!");

            return View(item);
        }

        public ActionResult Create()
        {
            ModelState.Clear();
            return View(new LoaiThuocViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiThuocViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.MaDanhMuc))
                ModelState.AddModelError("MaDanhMuc", "Mã loại thuốc là bắt buộc.");
            else if (model.MaDanhMuc.Length > 10)
                ModelState.AddModelError("MaDanhMuc", "Mã loại thuốc không được dài quá 10 ký tự.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(model.MaDanhMuc, @"^[a-zA-Z0-9]+$"))
                ModelState.AddModelError("MaDanhMuc", "Mã loại thuốc chỉ được chứa chữ và số, không dấu.");

            if (string.IsNullOrWhiteSpace(model.TenDanhMuc))
                ModelState.AddModelError("TenDanhMuc", "Tên loại thuốc là bắt buộc.");
            else if (model.TenDanhMuc.Length > 100)
                ModelState.AddModelError("TenDanhMuc", "Tên loại thuốc không được dài quá 100 ký tự.");

            if (model.TenDanhMucExists(model.TenDanhMuc))
                ModelState.AddModelError("TenDanhMuc", "Tên loại thuốc này đã tồn tại trong hệ thống.");

            if (model.MaDanhMucExists(model.MaDanhMuc))
                ModelState.AddModelError("MaDanhMuc", "Mã loại thuốc này đã tồn tại trong hệ thống.");

            if (!ModelState.IsValid) return View(model);

            try
            {
                bool result = CreateLoaiThuoc(model);
                if (result)
                {
                    TempData["Success"] = "Thêm loại thuốc mới thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Lỗi không xác định khi lưu vào CSDL.");
            }
            catch (SqlException sqlex) when (sqlex.Number == 2627)
            {
                ModelState.AddModelError("MaDanhMuc", "Mã loại thuốc đã tồn tại.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Mã loại thuốc không hợp lệ");

            var item = GetLoaiThuocById(id);
            if (item == null)
                return HttpNotFound("Không tìm thấy loại thuốc!");

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiThuocViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.MaDanhMuc))
                ModelState.AddModelError("MaDanhMuc", "Mã loại thuốc là bắt buộc.");

            if (string.IsNullOrWhiteSpace(model.TenDanhMuc))
                ModelState.AddModelError("TenDanhMuc", "Tên loại thuốc là bắt buộc.");

            if (model.TenDanhMucExists(model.TenDanhMuc, model.MaDanhMuc))
                ModelState.AddModelError("TenDanhMuc", "Tên loại thuốc này đã tồn tại trong hệ thống.");

            if (!ModelState.IsValid) return View(model);

            try
            {
                bool result = UpdateLoaiThuoc(model);
                if (result)
                {
                    TempData["Success"] = "Cập nhật loại thuốc thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Không thể cập nhật thông tin.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã loại thuốc không hợp lệ.";
                    return RedirectToAction("Index");
                }

                if (HasThuocInLoaiThuoc(id))
                {
                    TempData["Error"] = "Không thể xóa loại thuốc này vì đã có thuốc thuộc loại này trong hệ thống.";
                    return RedirectToAction("Index");
                }

                if (HasThuocInHoaDon(id))
                {
                    TempData["Error"] = "Không thể xóa loại thuốc này vì đã có thuốc thuộc loại này xuất hiện trong hóa đơn.";
                    return RedirectToAction("Index");
                }

                bool result = DeleteLoaiThuoc(id);
                if (result)
                    TempData["Success"] = "Đã xóa loại thuốc thành công.";
                else
                    TempData["Error"] = "Không tìm thấy loại thuốc để xóa.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi xóa loại thuốc: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private List<LoaiThuocViewModel> GetAllLoaiThuoc()
        {
            return SearchLoaiThuoc("");
        }

        private List<LoaiThuocViewModel> SearchLoaiThuoc(string keyword)
        {
            var list = new List<LoaiThuocViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT dm.MaDanhMuc, dm.TenDanhMuc, dm.MoTa,
                           ISNULL(COUNT(t.MaThuoc), 0) AS SoLuongThuoc
                    FROM DANHMUC_THUOC dm
                    LEFT JOIN THUOC t ON dm.MaDanhMuc = t.MaLoaiThuoc
                    WHERE (@Keyword = ''
                           OR dm.MaDanhMuc LIKE @Pattern
                           OR dm.TenDanhMuc LIKE @Pattern
                           OR dm.MoTa LIKE @Pattern)
                    GROUP BY dm.MaDanhMuc, dm.TenDanhMuc, dm.MoTa
                    ORDER BY dm.TenDanhMuc";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Keyword", string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim());
                cmd.Parameters.AddWithValue("@Pattern", "%" + (string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim()) + "%");
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new LoaiThuocViewModel
                        {
                            MaDanhMuc = dr["MaDanhMuc"].ToString(),
                            TenDanhMuc = dr["TenDanhMuc"].ToString(),
                            MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : "",
                            SoLuongThuoc = Convert.ToInt32(dr["SoLuongThuoc"])
                        });
                    }
                }
            }
            return list;
        }

        private LoaiThuocViewModel GetLoaiThuocById(string maDanhMuc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT MaDanhMuc, TenDanhMuc, MoTa FROM DANHMUC_THUOC WHERE MaDanhMuc = @MaDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc.Trim());
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new LoaiThuocViewModel
                        {
                            MaDanhMuc = dr["MaDanhMuc"].ToString(),
                            TenDanhMuc = dr["TenDanhMuc"].ToString(),
                            MoTa = dr["MoTa"] != DBNull.Value ? dr["MoTa"].ToString() : ""
                        };
                    }
                }
            }
            return null;
        }

        private bool CreateLoaiThuoc(LoaiThuocViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"INSERT INTO DANHMUC_THUOC (MaDanhMuc, TenDanhMuc, MoTa)
                                 VALUES (@MaDanhMuc, @TenDanhMuc, @MoTa)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDanhMuc", model.MaDanhMuc.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@TenDanhMuc", model.TenDanhMuc.Trim());
                cmd.Parameters.AddWithValue("@MoTa", (object)model.MoTa?.Trim() ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private bool UpdateLoaiThuoc(LoaiThuocViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"UPDATE DANHMUC_THUOC
                                 SET TenDanhMuc = @TenDanhMuc, MoTa = @MoTa
                                 WHERE MaDanhMuc = @MaDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDanhMuc", model.MaDanhMuc.Trim());
                cmd.Parameters.AddWithValue("@TenDanhMuc", model.TenDanhMuc.Trim());
                cmd.Parameters.AddWithValue("@MoTa", (object)model.MoTa?.Trim() ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private bool DeleteLoaiThuoc(string maDanhMuc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM DANHMUC_THUOC WHERE MaDanhMuc = @MaDanhMuc", conn, tran);
                    cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc.Trim());
                    int rows = cmd.ExecuteNonQuery();
                    tran.Commit();
                    return rows > 0;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        private bool HasThuocInLoaiThuoc(string maDanhMuc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM THUOC WHERE MaLoaiThuoc = @MaDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool HasThuocInHoaDon(string maDanhMuc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM CT_DON_THUOC ct
                    INNER JOIN DON_THUOC d ON ct.MaDonThuoc = d.MaDonThuoc
                    INNER JOIN THUOC t ON ct.MaThuoc = t.MaThuoc
                    INNER JOIN CT_HOADON_THUOC cthd ON d.MaDonThuoc = cthd.MaDonThuoc
                    WHERE t.MaLoaiThuoc = @MaDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public class LoaiThuocViewModel
    {
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }
        public int SoLuongThuoc { get; set; }

        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public bool TenDanhMucExists(string tenDanhMuc, string excludeMaDanhMuc = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM DANHMUC_THUOC WHERE TenDanhMuc = @TenDanhMuc";
                if (!string.IsNullOrEmpty(excludeMaDanhMuc))
                    query += " AND MaDanhMuc <> @MaDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDanhMuc", tenDanhMuc.Trim());
                if (!string.IsNullOrEmpty(excludeMaDanhMuc))
                    cmd.Parameters.AddWithValue("@MaDanhMuc", excludeMaDanhMuc);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool MaDanhMucExists(string maDanhMuc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(*) FROM DANHMUC_THUOC WHERE MaDanhMuc = @MaDanhMuc";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaDanhMuc", maDanhMuc.Trim());
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
