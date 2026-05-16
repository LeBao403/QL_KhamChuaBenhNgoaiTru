using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Libraries;
using QL_KhamChuaBenhNgoaiTru.Models;

using BenhNhanModel = QL_KhamChuaBenhNgoaiTru.Models.BenhNhan;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    /// <summary>
    /// JSON API Controller dành cho ứng dụng Flutter Mobile.
    /// Route: /MobileApi/[Action]
    /// </summary>
    public class MobileApiController : Controller
    {
        private readonly TaiKhoanDB _tkDb = new TaiKhoanDB();
        private readonly BenhNhanPortalDB _bnDb = new BenhNhanPortalDB();
        private readonly TiepTanDB _ttDb = new TiepTanDB();
        private readonly HomeDB _homeDb = new HomeDB();

        // ─────────────────────────────────────────────────────────────────────
        // AUTH
        // ─────────────────────────────────────────────────────────────────────

        // POST /MobileApi/Login
        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Json(new { success = false, message = "Vui lòng nhập tài khoản và mật khẩu!" });

            username = username.Trim();
            password = password.Trim();

            var tk = _tkDb.GetTaiKhoanByUsernameOrSdt(username);
            if (tk == null)
                return Json(new { success = false, message = "Tài khoản không tồn tại!" });

            if (!tk.IsActive)
                return Json(new { success = false, message = "Tài khoản đã bị khóa!" });

            if (!string.Equals(tk.PasswordHash, password, StringComparison.Ordinal))
                return Json(new { success = false, message = "Mật khẩu tài khoản chưa chính xác!" });

            var nv = _tkDb.GetNhanVienByMaTK(tk.MaTK);
            if (nv != null)
                return Json(new { success = false, message = "Tài khoản nhân viên không thể dùng app bệnh nhân!" });

            var bn = _tkDb.GetBenhNhanByMaTK(tk.MaTK);

            Session["TaiKhoan"] = tk;
            Session["Username"] = tk.Username;
            Session["MaTK"] = tk.MaTK;

            if (bn != null)
            {
                Session["BenhNhan"] = bn;
                Session["MaBN"] = bn.MaBN;
            }

            return Json(new
            {
                success = true,
                user = new
                {
                    MaTK = tk.MaTK,
                    Username = tk.Username,
                    MaBN = bn?.MaBN ?? "",
                    HoTen = bn?.HoTen ?? tk.Username,
                    SDT = bn?.SDT ?? "",
                    Email = bn?.Email ?? "",
                    CCCD = bn?.CCCD ?? "",
                    BHYT = bn?.BHYT ?? false
                }
            });
        }

        // POST /MobileApi/Register
        [HttpPost]
        public JsonResult Register(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });

            username = username.Trim();
            password = password.Trim();
            confirmPassword = confirmPassword?.Trim();

            if (password != confirmPassword)
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp!" });

            if (username.Length < 4)
                return Json(new { success = false, message = "Tên đăng nhập ít nhất 4 ký tự!" });

            var tk = new TaiKhoan { Username = username, PasswordHash = password };
            bool ok = _tkDb.InsertTaiKhoan(tk);

            if (ok) return Json(new { success = true, message = "Đăng ký thành công!" });
            return Json(new { success = false, message = "Tên đăng nhập đã được sử dụng!" });
        }

        // GET /MobileApi/GetCurrentUser
        public JsonResult GetCurrentUser()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            var tk = Session["TaiKhoan"] as TaiKhoan;
            if (bn == null) return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                success = true,
                user = new
                {
                    MaTK = tk?.MaTK ?? 0,
                    Username = tk?.Username ?? "",
                    MaBN = bn.MaBN,
                    HoTen = bn.HoTen ?? "",
                    SDT = bn.SDT ?? "",
                    Email = bn.Email ?? "",
                    CCCD = bn.CCCD ?? "",
                    BHYT = bn.BHYT
                }
            }, JsonRequestBehavior.AllowGet);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PROFILE
        // ─────────────────────────────────────────────────────────────────────

        // GET /MobileApi/GetProfile
        public JsonResult GetProfile()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            var tk = Session["TaiKhoan"] as TaiKhoan;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            try
            {
                var profile = _bnDb.GetBenhNhanByMaBN(bn.MaBN);
                if (profile == null)
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    success = true,
                    profile = new
                    {
                        MaTK = tk?.MaTK ?? 0,
                        Username = tk?.Username ?? "",
                        MaBN = profile.MaBN,
                        HoTen = profile.HoTen ?? "",
                        NgaySinh = profile.NgaySinh?.ToString("yyyy-MM-dd"),
                        GioiTinh = profile.GioiTinh ?? "",
                        SDT = profile.SDT ?? "",
                        Email = profile.Email ?? "",
                        DiaChi = profile.DiaChi ?? "",
                        CCCD = profile.CCCD ?? "",
                        BHYT = profile.BHYT,
                        SoTheBHYT = profile.SoTheBHYT ?? "",
                        HanSuDungBHYT = profile.HanSuDungBHYT?.ToString("yyyy-MM-dd"),
                        TuyenKham = profile.TuyenKham ?? "",
                        MucHuongBHYT = profile.MucHuongBHYT
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST /MobileApi/UpdateProfile
        [HttpPost]
        public JsonResult UpdateProfile(BenhNhanPortalDB.BenhNhanProfile model)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" });

            try
            {
                model.MaBN = bn.MaBN;
                bool ok = _bnDb.UpdateBenhNhan(model);
                if (ok)
                {
                    bn.HoTen = model.HoTen;
                    bn.SDT = model.SDT;
                    bn.Email = model.Email;
                    bn.CCCD = model.CCCD;
                    Session["BenhNhan"] = bn;
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Cập nhật thất bại!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // DISCOVERY / LANDING
        // ─────────────────────────────────────────────────────────────────────

        // GET /MobileApi/GetHomeData
        public JsonResult GetHomeData()
        {
            try
            {
                var model = _homeDb.GetHomeData();

                var specialties = model.DanhSachKhoa.Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa,
                    MoTa = string.IsNullOrWhiteSpace(k.MoTa)
                        ? "Chuyên khoa được đầu tư đồng bộ, hỗ trợ khám và điều trị hiệu quả."
                        : k.MoTa
                }).ToList();

                var doctors = model.DanhSachBacSi.Select(bs => new
                {
                    bs.MaNV,
                    bs.HoTen,
                    ChucDanh = bs.TenChucVu,
                    ChuyenKhoa = bs.TenKhoa,
                    bs.HinhAnh
                }).ToList();

                var news = new[]
                {
                    new
                    {
                        TieuDe = "MedicHub triển khai hệ thống đặt lịch khám thông minh",
                        TomTat = "Người bệnh có thể chủ động chọn ngày, khung giờ và thanh toán QR ngay trên ứng dụng.",
                        NgayDang = "2026-04-08",
                        ChuyenMuc = "Chuyển đổi số"
                    },
                    new
                    {
                        TieuDe = "Khuyến nghị phòng bệnh giao mùa cho trẻ nhỏ và người cao tuổi",
                        TomTat = "Bác sĩ khuyến cáo theo dõi hô hấp, dinh dưỡng và tiêm chủng đầy đủ trong giai đoạn giao mùa.",
                        NgayDang = "2026-04-05",
                        ChuyenMuc = "Cẩm nang sức khỏe"
                    },
                    new
                    {
                        TieuDe = "Mở rộng khung giờ khám ngoài giờ tại nhiều chuyên khoa",
                        TomTat = "Bệnh viện tăng thêm khung giờ linh hoạt giúp người bệnh thuận tiện sắp xếp lịch làm việc.",
                        NgayDang = "2026-04-02",
                        ChuyenMuc = "Thông báo"
                    }
                };

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        specialties,
                        doctors,
                        stats = new
                        {
                            TongSoKhoa = model.ThongKe.TongSoKhoa,
                            TongSoPhong = model.ThongKe.TongSoPhong,
                            TongSoNhanVien = model.ThongKe.TongSoNhanVien,
                            TongLuotKham = model.ThongKe.TongLuotKham
                        },
                        news
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetDoctors
        public JsonResult GetDoctors(string searchString = "", string khoa = "", int page = 1, int pageSize = 20)
        {
            try
            {
                var doctors = _homeDb.GetAllBacSi();

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    doctors = doctors.Where(b =>
                        !string.IsNullOrWhiteSpace(b.HoTen) &&
                        b.HoTen.IndexOf(searchString.Trim(), StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList();
                }

                if (!string.IsNullOrWhiteSpace(khoa))
                {
                    doctors = doctors.Where(b =>
                        !string.IsNullOrWhiteSpace(b.TenKhoa) &&
                        b.TenKhoa.IndexOf(khoa.Trim(), StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList();
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;

                var totalItems = doctors.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagedItems = doctors
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(bs => new
                    {
                        bs.MaNV,
                        bs.HoTen,
                        ChucDanh = string.IsNullOrWhiteSpace(bs.TenChucVu) ? "Bác sĩ chuyên khoa" : bs.TenChucVu,
                        ChuyenKhoa = string.IsNullOrWhiteSpace(bs.TenKhoa) ? "Đa khoa" : bs.TenKhoa,
                        bs.HinhAnh
                    })
                    .ToList();

                var specialties = _homeDb.GetAllKhoa()
                    .Select(k => k.TenKhoa)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = pagedItems,
                    paging = new { page, pageSize, totalItems, totalPages },
                    filters = new { specialties }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetSpecialties
        public JsonResult GetSpecialties()
        {
            try
            {
                var specialties = _homeDb.GetAllKhoa()
                    .Select(k => new
                    {
                        k.MaKhoa,
                        k.TenKhoa,
                        MoTa = string.IsNullOrWhiteSpace(k.MoTa)
                            ? "Chuyên khoa được đầu tư trang thiết bị hiện đại và đội ngũ bác sĩ giàu kinh nghiệm."
                            : k.MoTa
                    })
                    .OrderBy(k => k.TenKhoa)
                    .ToList();

                return Json(new { success = true, data = specialties }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // BOOKING
        // ─────────────────────────────────────────────────────────────────────

        // GET /MobileApi/GetDichVu
        public JsonResult GetDichVu()
        {
            try
            {
                var dt = _ttDb.GetDanhSachDichVuKham();
                var list = new List<object>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaDV = row["MaDV"].ToString(),
                        TenDV = row["TenDV"].ToString(),
                        GiaDichVu = Convert.ToDecimal(row["GiaDichVu"])
                    });
                }
                return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetLichKham
        public JsonResult GetLichKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            try
            {
                var list = _bnDb.GetLichKhamByMaBN(bn.MaBN);
                var result = list.Select(lk => new
                {
                    lk.MaPhieuDK,
                    NgayDangKy = lk.NgayDangKy?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    lk.HinhThucDangKy,
                    lk.TrangThai,
                    lk.MaPhieuKhamBenh,
                    lk.STT,
                    lk.TrangThaiKham,
                    NgayKham = lk.NgayKham?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    lk.LyDoDenKham,
                    lk.TenPhong,
                    lk.TenKhoa,
                    lk.TenBacSi
                }).ToList();
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetLichSuKham
        public JsonResult GetLichSuKham()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            try
            {
                var list = _bnDb.GetLichSuKhamByMaBN(bn.MaBN);
                var result = list.Select(item => new
                {
                    item.MaPhieuKhamBenh,
                    NgayKham = item.NgayKham.ToString("yyyy-MM-ddTHH:mm:ss"),
                    item.LyDoDenKham,
                    item.TrieuChung,
                    item.KetLuan,
                    item.TrangThai,
                    item.TenPhong,
                    item.TenKhoa,
                    item.TenBacSi
                }).ToList();
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetDonThuoc
        public JsonResult GetDonThuoc()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            try
            {
                var list = _bnDb.GetDonThuocByMaBN(bn.MaBN);
                var result = list.Select(item => new
                {
                    item.MaDonThuoc,
                    NgayKe = item.NgayKe.ToString("yyyy-MM-ddTHH:mm:ss"),
                    item.LoiDanBS,
                    item.TrangThai,
                    NgayKham = item.NgayKham.ToString("yyyy-MM-ddTHH:mm:ss"),
                    item.MaPhieuKhamBenh,
                    item.TenBacSi
                }).ToList();
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetChiTietDonThuoc?id=DT2604110001
        public JsonResult GetChiTietDonThuoc(string id)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            try
            {
                var detail = _bnDb.GetChiTietDonThuoc(id);
                if (detail == null || detail.ChiTiet == null || detail.ChiTiet.Count == 0)
                    return Json(new { success = false, message = "Không tìm thấy chi tiết đơn thuốc!" }, JsonRequestBehavior.AllowGet);

                var result = detail.ChiTiet.Select(item => new
                {
                    item.MaCTDonThuoc,
                    item.MaThuoc,
                    item.TenThuoc,
                    item.SoLuongSang,
                    item.SoLuongTrua,
                    item.SoLuongChieu,
                    item.SoLuongToi,
                    item.SoNgayDung,
                    item.SoLuong,
                    item.DonViTinh,
                    item.DonGia,
                    item.GhiChu,
                    item.DonViCoBan
                }).ToList();
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET /MobileApi/GetHoaDon
        public JsonResult GetHoaDon()
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            try
            {
                var list = _bnDb.GetHoaDonByMaBN(bn.MaBN);
                var result = list.Select(item => new
                {
                    item.MaHD,
                    NgayThanhToan = item.NgayThanhToan?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    item.TongTien,
                    item.TrangThaiThanhToan,
                    item.HinhThucThanhToan,
                    item.GhiChu,
                    item.MaPhieuKhamBenh,
                    NgayKham = item.NgayKham?.ToString("yyyy-MM-ddTHH:mm:ss")
                }).ToList();
                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST /MobileApi/LoadKhungGio
        [HttpPost]
        public JsonResult LoadKhungGio(DateTime ngayKham)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Chưa đăng nhập" });

            try
            {
                var danhSachGio = _bnDb.GetKhungGioHopLe(ngayKham);
                return Json(new { success = true, data = danhSachGio });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST /MobileApi/DatLichKham
        [HttpPost]
        public JsonResult DatLichKham(DateTime ngayKham, int maKhungGio, string maDV, string lyDo)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại!" });

            if (ngayKham < DateTime.Today)
                return Json(new { success = false, message = "Ngày khám không được nhỏ hơn ngày hiện tại." });

            try
            {
                string tenQuay;
                string maHD;
                string maPhieuDK = _bnDb.DatLichKham(bn.MaBN, ngayKham, maKhungGio, maDV, lyDo, out tenQuay, out maHD);

                return Json(new
                {
                    success = true,
                    maPhieuDK = maPhieuDK,
                    maHD = maHD,
                    tenQuay = tenQuay
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST /MobileApi/TaoThanhToanTheUrl
        [HttpPost]
        public JsonResult TaoThanhToanTheUrl(string maHD, string maPhieuDK, string returnUrl = null)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại!" });

            try
            {
                if (!HoaDonDatLichThuocBenhNhan(maHD, maPhieuDK, bn.MaBN))
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn đặt lịch hợp lệ!" });

                string paymentUrl = TaoUrlVnPay(maHD, 100000m, returnUrl);
                return Json(new { success = true, paymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST /MobileApi/KiemTraThanhToanThe
        [HttpPost]
        public JsonResult KiemTraThanhToanThe(string maHD, string maPhieuDK)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại!" });

            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
                {
                    conn.Open();
                    const string sql = @"
                        SELECT TOP 1 hd.TrangThaiThanhToan, pdk.TrangThai AS TrangThaiPhieu
                        FROM HOADON hd
                        LEFT JOIN PHIEUDANGKY pdk ON hd.MaPhieuDK = pdk.MaPhieuDK
                        WHERE hd.MaHD = @MaHD
                          AND hd.MaPhieuDK = @MaPhieuDK
                          AND hd.MaBN = @MaBN";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaHD", maHD ?? "");
                        cmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK ?? "");
                        cmd.Parameters.AddWithValue("@MaBN", bn.MaBN);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                                return Json(new { success = false, message = "Không tìm thấy hóa đơn đặt lịch!" });

                            string trangThaiHD = dr["TrangThaiThanhToan"]?.ToString() ?? "";
                            string trangThaiPhieu = dr["TrangThaiPhieu"]?.ToString() ?? "";

                            return Json(new
                            {
                                success = true,
                                isPaid = trangThaiHD == "Đã thanh toán",
                                isCanceled = trangThaiHD == "Đã hủy" || trangThaiPhieu == "Hủy",
                                trangThaiThanhToan = trangThaiHD,
                                trangThaiPhieu = trangThaiPhieu
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool HoaDonDatLichThuocBenhNhan(string maHD, string maPhieuDK, string maBN)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString))
            {
                conn.Open();
                const string sql = @"
                    SELECT COUNT(*)
                    FROM HOADON
                    WHERE MaHD = @MaHD
                      AND MaPhieuDK = @MaPhieuDK
                      AND MaBN = @MaBN
                      AND TrangThaiThanhToan <> N'Đã hủy'";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaHD", maHD ?? "");
                    cmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK ?? "");
                    cmd.Parameters.AddWithValue("@MaBN", maBN ?? "");
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        private string TaoUrlVnPay(string maHoaDon, decimal tongTien, string returnUrlOverride = null)
        {
            string vnpReturnUrl = !string.IsNullOrWhiteSpace(returnUrlOverride) &&
                                  (returnUrlOverride.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                   returnUrlOverride.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                ? returnUrlOverride.Trim()
                : ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnpUrl = ConfigurationManager.AppSettings["vnp_Url"];
            string vnpTmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnpHashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            if (string.IsNullOrEmpty(vnpTmnCode) || string.IsNullOrEmpty(vnpHashSecret))
                throw new Exception("Không đọc được cấu hình VNPAY. Hãy kiểm tra lại file Secrets.");

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnpTmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(tongTien * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");

            string ipAddr = Utils.GetIpAddress();
            if (string.IsNullOrEmpty(ipAddr) || ipAddr == "::1") ipAddr = "127.0.0.1";
            vnpay.AddRequestData("vnp_IpAddr", ipAddr);

            vnpay.AddRequestData("vnp_Locale", "vn");
            string cleanMaHD = (maHoaDon ?? "HD").Trim();
            vnpay.AddRequestData("vnp_OrderInfo", "ThanhToanHoaDon_" + cleanMaHD);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnpReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", cleanMaHD + "_" + DateTime.Now.Ticks);

            return vnpay.CreateRequestUrl(vnpUrl, vnpHashSecret);
        }

        // POST /MobileApi/HuyLich
        [HttpPost]
        public JsonResult HuyLich(string maPhieuDK)
        {
            var bn = Session["BenhNhan"] as BenhNhanModel;
            if (bn == null) return Json(new { success = false, message = "Vui lòng đăng nhập lại!" });

            try
            {
                bool ok = _bnDb.HuyLichKham(maPhieuDK, bn.MaBN);
                if (ok)
                    return Json(new { success = true, message = "Hủy lịch thành công!" });

                return Json(new
                {
                    success = false,
                    message = "Không thể hủy lịch. Chỉ lịch ở trạng thái 'Chờ xử lý' mới được hủy."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
