using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class HoaDonController : BaseAdminController
    {
        // GET: Admin/HoaDon
        HoaDonDB db = new HoaDonDB();

        // GET: Admin/HoaDon/Index
        public ActionResult Index(string tuNgay, string denNgay, string tuKhoa, string trangThai, string sortCol = "NgayThanhToan", string sortDir = "desc", int page = 1)
        {
            // Mặc định lấy ngày hôm nay
            DateTime dtTuNgay = DateTime.Now.Date;
            DateTime dtDenNgay = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(tuNgay)) DateTime.TryParse(tuNgay, out dtTuNgay);
            if (!string.IsNullOrEmpty(denNgay)) DateTime.TryParse(denNgay, out dtDenNgay);

            ViewBag.TuNgay = dtTuNgay.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = dtDenNgay.ToString("yyyy-MM-dd");
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;
            ViewBag.SortCol = sortCol; // Lưu lại cột đang sort
            ViewBag.SortDir = sortDir; // Lưu lại chiều đang sort (asc/desc)

            DataTable fullDt = db.GetDanhSachHoaDon(dtTuNgay, dtDenNgay);

            if (fullDt.Rows.Count > 0)
            {
                var rows = fullDt.AsEnumerable();

                // 1. LỌC TỪ KHÓA
                if (!string.IsNullOrEmpty(tuKhoa))
                {
                    string keyword = tuKhoa.ToLower().Trim();
                    rows = rows.Where(r =>
                        r["MaHD"].ToString().Contains(keyword) ||
                        r["MaBN"].ToString().ToLower().Contains(keyword) ||
                        r["HoTen"].ToString().ToLower().Contains(keyword) ||
                        r["SDT"].ToString().Contains(keyword)
                    );
                }

                // 2. LỌC TRẠNG THÁI
                if (!string.IsNullOrEmpty(trangThai))
                {
                    rows = rows.Where(r => r["TrangThaiThanhToan"].ToString() == trangThai);
                }

                // 3. SẮP XẾP (SORTING) KHI BẤM VÀO HEADER BẢNG
                if (rows.Any())
                {
                    if (sortDir == "asc")
                    {
                        switch (sortCol)
                        {
                            case "MaHD": rows = rows.OrderBy(r => Convert.ToInt32(r["MaHD"])); break;
                            case "HoTen": rows = rows.OrderBy(r => r["HoTen"].ToString()); break;
                            case "NgayThanhToan": rows = rows.OrderBy(r => Convert.ToDateTime(r["NgayThanhToan"])); break;
                            case "TongTienGoc": rows = rows.OrderBy(r => Convert.ToDecimal(r["TongTienGoc"])); break;
                            case "TongTienBHYTChiTra": rows = rows.OrderBy(r => Convert.ToDecimal(r["TongTienBHYTChiTra"])); break;
                            case "TongTienBenhNhanTra": rows = rows.OrderBy(r => Convert.ToDecimal(r["TongTienBenhNhanTra"])); break;
                            case "TrangThaiThanhToan": rows = rows.OrderBy(r => r["TrangThaiThanhToan"].ToString()); break;
                        }
                    }
                    else // Mặc định là desc (Giảm dần)
                    {
                        switch (sortCol)
                        {
                            case "MaHD": rows = rows.OrderByDescending(r => Convert.ToInt32(r["MaHD"])); break;
                            case "HoTen": rows = rows.OrderByDescending(r => r["HoTen"].ToString()); break;
                            case "NgayThanhToan": rows = rows.OrderByDescending(r => Convert.ToDateTime(r["NgayThanhToan"])); break;
                            case "TongTienGoc": rows = rows.OrderByDescending(r => Convert.ToDecimal(r["TongTienGoc"])); break;
                            case "TongTienBHYTChiTra": rows = rows.OrderByDescending(r => Convert.ToDecimal(r["TongTienBHYTChiTra"])); break;
                            case "TongTienBenhNhanTra": rows = rows.OrderByDescending(r => Convert.ToDecimal(r["TongTienBenhNhanTra"])); break;
                            case "TrangThaiThanhToan": rows = rows.OrderByDescending(r => r["TrangThaiThanhToan"].ToString()); break;
                        }
                    }
                    fullDt = rows.CopyToDataTable();
                }
                else
                {
                    fullDt = fullDt.Clone();
                }
            }

            // 4. PHÂN TRANG
            int pageSize = 15;
            int totalRows = fullDt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            DataTable pagedDt = fullDt.Clone();
            if (totalRows > 0)
            {
                pagedDt = fullDt.AsEnumerable().Skip((page - 1) * pageSize).Take(pageSize).CopyToDataTable();
            }

            ViewBag.DanhSachHoaDon = pagedDt;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRows = totalRows;

            return View();
        }

        // GET: Admin/HoaDon/Details/5
        public ActionResult Details(int id)
        {
            DataTable dtHD = db.GetHoaDonById(id);
            if (dtHD.Rows.Count == 0) return HttpNotFound();

            ViewBag.HoaDonInfo = dtHD.Rows[0];
            ViewBag.ChiTietDV = db.GetChiTietDichVu(id);

            return View();
        }
    }
}