using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using QL_KhamChuaBenhNgoaiTru.Helpers; // <-- Gọi Utilities

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class BacSiDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. CẬP NHẬT HÀM LẤY DANH SÁCH KHÁM
        public List<PhieuKhamBenhInfo> GetDanhSachPhieuKham(string maBS, string trangThai)
        {
            var list = new List<PhieuKhamBenhInfo>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // 1. Lấy Mã Phòng của bác sĩ
                int maPhongBS = 0;
                string sqlPhong = "SELECT MaPhong FROM NHANVIEN WHERE MaNV = @MaBS";
                using (SqlCommand cmdP = new SqlCommand(sqlPhong, conn))
                {
                    cmdP.Parameters.AddWithValue("@MaBS", maBS);
                    var obj = cmdP.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value) maPhongBS = Convert.ToInt32(obj);
                }

                // 2. Xây dựng câu lệnh SQL với điều kiện LỌC THEO NGÀY HÔM NAY
                string sql = @"
            SELECT PKB.MaPhieuKhamBenh, PKB.MaBN, PKB.STT, PKB.LyDoDenKham, PKB.TrangThai, 
                   BN.HoTen, BN.GioiTinh, BN.NgaySinh
            FROM PHIEUKHAMBENH PKB
            JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN
            WHERE CAST(PKB.NgayLap AS DATE) = CAST(GETDATE() AS DATE) ";

                // 3. Phân loại theo trạng thái
                if (trangThai == "Chờ khám" || trangThai == "Chờ Khám")
                {
                    sql += " AND PKB.MaPhong = @MaPhong AND PKB.TrangThai = N'Chờ khám' ";
                    sql += " ORDER BY PKB.STT ASC";
                }
                else if (trangThai == "Hoàn thành")
                {
                    sql += " AND PKB.MaBacSiKham = @MaBS AND PKB.TrangThai = N'Hoàn thành' ";
                    sql += " ORDER BY PKB.NgayLap DESC";
                }
                else
                {
                    sql += " AND PKB.MaBacSiKham = @MaBS AND PKB.TrangThai IN (N'Đang khám', N'Chờ thanh toán', N'Chờ cận lâm sàng', N'Đã có kết quả CLS') ";
                    sql += " ORDER BY CASE WHEN PKB.TrangThai = N'Đã có kết quả CLS' THEN 1 WHEN PKB.TrangThai = N'Đang khám' THEN 2 ELSE 3 END ASC, PKB.NgayLap ASC";
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    cmd.Parameters.AddWithValue("@MaPhong", maPhongBS);
                    cmd.Parameters.AddWithValue("@MaBS", maBS);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int tuoi = dr["NgaySinh"] != DBNull.Value ? DateTime.Now.Year - Convert.ToDateTime(dr["NgaySinh"]).Year : 0;
                            list.Add(new PhieuKhamBenhInfo
                            {
                                // [ĐÃ SỬA] Đổi sang ToString()
                                MaPhieuKhamBenh = dr["MaPhieuKhamBenh"].ToString(),
                                MaBN = dr["MaBN"].ToString(),
                                STT = dr["STT"] != DBNull.Value ? Convert.ToInt32(dr["STT"]) : 0,
                                LyDoDenKham = dr["LyDoDenKham"].ToString(),
                                TrangThai = dr["TrangThai"].ToString(),
                                TenBN = dr["HoTen"].ToString(),
                                GioiTinh = dr["GioiTinh"].ToString(),
                                Tuoi = tuoi
                            });
                        }
                    }
                }
            }
            return list;
        }

        // [ĐÃ SỬA] Đổi int maPhieu thành string
        public object GetThongTinChiTiet(string maPhieu)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                // 1. Lấy thông tin hành chính, nợ và TÊN BÁC SĨ 
                string sql = @"
        SELECT PKB.MaPhieuKhamBenh, PKB.LyDoDenKham, PKB.TrieuChung, PKB.KetLuan, PKB.TrangThai,
               BN.HoTen, BN.GioiTinh, BN.NgaySinh, BN.DiaChi, BN.SDT,
               (SELECT TOP 1 HoTen FROM NHANVIEN WHERE MaNV = PKB.MaBacSiKham) AS TenBS,
               (SELECT TOP 1 TrangThaiThanhToan FROM HOADON WHERE MaPhieuKhamBenh = PKB.MaPhieuKhamBenh ORDER BY MaHD ASC) AS TrangThaiHD,
               (SELECT TOP 1 TienBenhNhanTra FROM CT_HOADON_DV ct JOIN HOADON hd ON ct.MaHD = hd.MaHD WHERE hd.MaPhieuKhamBenh = PKB.MaPhieuKhamBenh ORDER BY ct.MaCTHD ASC) AS TienKham
        FROM PHIEUKHAMBENH PKB
        JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN
        WHERE PKB.MaPhieuKhamBenh = @MaPhieu";

                var info = new
                {
                    MaPhieuKhamBenh = "", // [ĐÃ SỬA] int = 0 -> string = ""
                    TenBN = "",
                    GioiTinh = "",
                    NgaySinhStr = "",
                    Tuoi = 0,
                    DiaChi = "",
                    SoDienThoai = "",
                    LyDoDenKham = "",
                    TrieuChung = "",
                    KetLuan = "",
                    TrangThai = "",
                    TenBS = "",
                    DaThanhToanKham = true,
                    PhiKham = 0m,
                    DanhSachBenh = new List<object>(),
                    DanhSachThuoc = new List<object>(),
                    DanhSachDichVu = new List<object>()
                };

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            DateTime? ns = dr["NgaySinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["NgaySinh"]) : null;

                            string trangThaiHD = dr["TrangThaiHD"] != DBNull.Value ? dr["TrangThaiHD"].ToString() : "Đã thanh toán";
                            decimal phiKham = dr["TienKham"] != DBNull.Value ? Convert.ToDecimal(dr["TienKham"]) : 0m;

                            info = new
                            {
                                // [ĐÃ SỬA] Đổi sang ToString()
                                MaPhieuKhamBenh = dr["MaPhieuKhamBenh"].ToString(),
                                TenBN = dr["HoTen"].ToString(),
                                GioiTinh = dr["GioiTinh"].ToString(),
                                NgaySinhStr = ns.HasValue ? ns.Value.ToString("dd/MM/yyyy") : "Chưa cập nhật",
                                Tuoi = ns.HasValue ? DateTime.Now.Year - ns.Value.Year : 0,
                                DiaChi = dr["DiaChi"] != DBNull.Value ? dr["DiaChi"].ToString() : "Chưa cập nhật",
                                SoDienThoai = dr["SDT"] != DBNull.Value ? dr["SDT"].ToString() : "Chưa cập nhật",
                                LyDoDenKham = dr["LyDoDenKham"].ToString(),
                                TrieuChung = dr["TrieuChung"] != DBNull.Value ? dr["TrieuChung"].ToString() : "",
                                KetLuan = dr["KetLuan"] != DBNull.Value ? dr["KetLuan"].ToString() : "",
                                TrangThai = dr["TrangThai"].ToString(),
                                TenBS = dr["TenBS"] != DBNull.Value ? dr["TenBS"].ToString() : "Bác sĩ điều trị",
                                DaThanhToanKham = (trangThaiHD == "Đã thanh toán"),
                                PhiKham = phiKham,
                                DanhSachBenh = new List<object>(),
                                DanhSachThuoc = new List<object>(),
                                DanhSachDichVu = new List<object>()
                            };
                        }
                    }
                }

                // 2. Lấy danh sách Bệnh đã kê
                string sqlBenh = @"SELECT ct.MaBenh, b.TenBenh FROM CHITIET_CHANDOAN ct JOIN DANHMUC_BENH b ON ct.MaBenh = b.MaBenh WHERE ct.MaPhieuKhamBenh = @MaPhieu";
                using (SqlCommand cmdB = new SqlCommand(sqlBenh, conn))
                {
                    cmdB.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    using (SqlDataReader drB = cmdB.ExecuteReader())
                        while (drB.Read()) info.DanhSachBenh.Add(new { MaBenh = drB["MaBenh"].ToString().Trim(), TenBenh = drB["TenBenh"].ToString() });
                }

                // 3. Lấy danh sách Thuốc đã kê
                string sqlThuoc = @"SELECT ct.*, t.TenThuoc FROM CT_DON_THUOC ct JOIN DON_THUOC d ON ct.MaDonThuoc = d.MaDonThuoc JOIN THUOC t ON ct.MaThuoc = t.MaThuoc WHERE d.MaPhieuKhamBenh = @MaPhieu";
                using (SqlCommand cmdT = new SqlCommand(sqlThuoc, conn))
                {
                    cmdT.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    using (SqlDataReader drT = cmdT.ExecuteReader())
                        while (drT.Read()) info.DanhSachThuoc.Add(new
                        {
                            MaThuoc = drT["MaThuoc"].ToString().Trim(),
                            TenThuoc = drT["TenThuoc"].ToString(),
                            Sang = Convert.ToDecimal(drT["SoLuongSang"]),
                            Trua = Convert.ToDecimal(drT["SoLuongTrua"]),
                            Chieu = Convert.ToDecimal(drT["SoLuongChieu"]),
                            Toi = Convert.ToDecimal(drT["SoLuongToi"]),
                            SoNgay = Convert.ToInt32(drT["SoNgayDung"]),
                            SoLuong = Convert.ToInt32(drT["SoLuong"]),
                            GhiChu = drT["GhiChu"].ToString()
                        });
                }

                // 4. LẤY DANH SÁCH DỊCH VỤ CẬN LÂM SÀNG ĐÃ CHỈ ĐỊNH 
                string sqlDV = @"SELECT ct.MaDV, dv.TenDV, ct.DonGia, pc.MaPhong 
                 FROM CHITIET_CHIDINH ct
                 JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                 JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                 WHERE pc.MaPhieuKhamBenh = @MaPhieu";
                using (SqlCommand cmdDV = new SqlCommand(sqlDV, conn))
                {
                    cmdDV.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    using (SqlDataReader drDV = cmdDV.ExecuteReader())
                    {
                        while (drDV.Read())
                        {
                            info.DanhSachDichVu.Add(new
                            {
                                MaDV = drDV["MaDV"].ToString().Trim(),
                                TenDV = drDV["TenDV"].ToString(),
                                Gia = drDV["DonGia"] != DBNull.Value ? Convert.ToDecimal(drDV["DonGia"]) : 0,
                                GiaDichVu = drDV["DonGia"] != DBNull.Value ? Convert.ToDecimal(drDV["DonGia"]) : 0,
                                MaPhong = drDV["MaPhong"] != DBNull.Value ? Convert.ToInt32(drDV["MaPhong"]) : 0
                            });
                        }
                    }
                }
                return info;
            }
        }

        // 2. CẬP NHẬT HÀM TIẾP NHẬN BỆNH NHÂN (LƯU THÊM MÃ BÁC SĨ)
        // [ĐÃ SỬA] int maPhieu -> string maPhieu
        public PhieuKhamBenhInfo TiepNhan(string maPhieu, string maBS)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                string sqlUpdate = "UPDATE PHIEUKHAMBENH SET TrangThai = N'Đang khám', MaBacSiKham = @MaBS WHERE MaPhieuKhamBenh = @MaPhieu";
                using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdUpdate.Parameters.AddWithValue("@MaBS", maBS);
                    cmdUpdate.ExecuteNonQuery();
                }

                string sqlGet = @"
            SELECT PKB.MaPhieuKhamBenh, PKB.LyDoDenKham, PKB.TrieuChung, BN.HoTen, BN.GioiTinh, BN.NgaySinh
    FROM PHIEUKHAMBENH PKB
    JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN
    WHERE PKB.MaPhieuKhamBenh = @MaPhieu";

                using (SqlCommand cmdGet = new SqlCommand(sqlGet, conn))
                {
                    cmdGet.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    using (SqlDataReader dr = cmdGet.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new PhieuKhamBenhInfo
                            {
                                // [ĐÃ SỬA] Đổi sang ToString()
                                MaPhieuKhamBenh = dr["MaPhieuKhamBenh"].ToString(),
                                TenBN = dr["HoTen"].ToString(),
                                GioiTinh = dr["GioiTinh"].ToString(),
                                Tuoi = dr["NgaySinh"] != DBNull.Value ? DateTime.Now.Year - Convert.ToDateTime(dr["NgaySinh"]).Year : 0,
                                LyDoDenKham = dr["LyDoDenKham"].ToString(),
                                TrieuChung = dr["TrieuChung"] != DBNull.Value ? dr["TrieuChung"].ToString() : ""
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 3. Lấy Danh mục bệnh (Giữ nguyên)
        public List<DanhMucBenh> GetDanhSachBenh()
        {
            var list = new List<DanhMucBenh>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string sql = "SELECT MaBenh, TenBenh FROM DANHMUC_BENH";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new DanhMucBenh
                            {
                                MaBenh = dr["MaBenh"].ToString(),
                                TenBenh = dr["TenBenh"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 4. Lấy Danh mục thuốc đang hoạt động (Giữ nguyên)
        public List<Thuoc> GetDanhSachThuoc()
        {
            var list = new List<Thuoc>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string sql = "SELECT MaThuoc, TenThuoc, DonViCoBan, GiaBan, CoBHYT FROM THUOC WHERE TrangThai = 1";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Thuoc
                            {
                                MaThuoc = dr["MaThuoc"].ToString(),
                                TenThuoc = dr["TenThuoc"].ToString(),
                                DonViCoBan = dr["DonViCoBan"].ToString(),
                                GiaBan = dr["GiaBan"] != DBNull.Value ? Convert.ToDecimal(dr["GiaBan"]) : 0,
                                CoBHYT = dr["CoBHYT"] != DBNull.Value ? Convert.ToBoolean(dr["CoBHYT"]) : false
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 2. Hàm lấy danh sách Dịch vụ CLS (Giữ nguyên)
        public List<DichVu> GetDanhSachDichVuCLS()
        {
            var list = new List<DichVu>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string sql = @"
            SELECT MaDV, TenDV, GiaDichVu 
            FROM DICHVU 
            WHERE TrangThai = 1 
              AND MaLoaiDV IN ('LDV02', 'LDV03', 'LDV04', 'LDV05')";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new DichVu
                            {
                                MaDV = dr["MaDV"].ToString(),
                                TenDV = dr["TenDV"].ToString(),
                                GiaDichVu = dr["GiaDichVu"] != DBNull.Value ? Convert.ToDecimal(dr["GiaDichVu"]) : 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 3. Hàm tìm các Phòng phù hợp cho Dịch vụ (Giữ nguyên)
        public List<dynamic> GetPhongPhuHop(string maDV)
        {
            var list = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string sql = @"
            DECLARE @MaKhoa INT = (SELECT MaKhoa FROM DICHVU WHERE MaDV = @MaDV);
            
            SELECT P.MaPhong, P.TenPhong,
                   (SELECT COUNT(*) FROM PHIEU_CHIDINH PCD 
                    WHERE PCD.MaPhong = P.MaPhong AND PCD.TrangThai IN (N'Chưa thanh toán', N'Đang thực hiện')) AS SoNguoiCho
            FROM PHONG P
            WHERE (@MaKhoa IS NULL OR P.MaKhoa = @MaKhoa) AND P.TrangThai = 1
            ORDER BY SoNguoiCho ASC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaDV", maDV);
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new
                            {
                                MaPhong = Convert.ToInt32(dr["MaPhong"]),
                                TenPhong = dr["TenPhong"].ToString(),
                                SoNguoiCho = Convert.ToInt32(dr["SoNguoiCho"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 4. HÀM LƯU KHÁM BỆNH 
        public bool LuuKhamBenh(KhamBenhViewModel model, string maBS, out string errorMsg)
        {
            errorMsg = "";
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. LẤY MÃ BỆNH NHÂN 
                    string maBN = "";
                    using (SqlCommand cmdBN = new SqlCommand("SELECT MaBN FROM PHIEUKHAMBENH WHERE MaPhieuKhamBenh = @MaPKB", conn, tran))
                    {
                        cmdBN.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                        object bnObj = cmdBN.ExecuteScalar();
                        if (bnObj == null) throw new Exception("Không tìm thấy bệnh nhân của phiếu khám.");
                        maBN = bnObj.ToString();
                    }

                    // 2. CẬP NHẬT TRẠNG THÁI PHIẾU KHÁM CHÍNH
                    string trangThai = model.YeuCauCanLamSang ? "Chờ thanh toán" : "Hoàn thành";
                    string sqlUpdatePhieu = @"UPDATE PHIEUKHAMBENH 
                                      SET TrieuChung = @TrieuChung, KetLuan = @KetLuan, TrangThai = @TrangThai 
                                      WHERE MaPhieuKhamBenh = @MaPhieu";
                    using (SqlCommand cmd = new SqlCommand(sqlUpdatePhieu, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@TrieuChung", model.TrieuChung ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@KetLuan", model.KetLuan ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                        cmd.Parameters.AddWithValue("@MaPhieu", model.MaPhieuKhamBenh);
                        cmd.ExecuteNonQuery();
                    }

                    // ========================================================================
                    // NHÁNH 1: NẾU BÁC SĨ CHỈ ĐỊNH CẬN LÂM SÀNG
                    // ========================================================================
                    if (model.YeuCauCanLamSang && model.ChiDinhs != null && model.ChiDinhs.Count > 0)
                    {
                        string maHD = ""; // [ĐÃ SỬA] Đổi thành chuỗi

                        // 1.1 Tìm hóa đơn hiện tại của lượt khám
                        string sqlGetHD = "SELECT TOP 1 MaHD FROM HOADON WHERE MaPhieuKhamBenh = @MaPKB ORDER BY MaHD DESC";
                        using (SqlCommand cmdGetHD = new SqlCommand(sqlGetHD, conn, tran))
                        {
                            cmdGetHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                            object hdObj = cmdGetHD.ExecuteScalar();
                            maHD = hdObj != null ? hdObj.ToString() : ""; // [ĐÃ SỬA]
                        }

                        // 1.2 Nếu chưa có Hóa đơn thì tạo mới
                        if (string.IsNullOrEmpty(maHD))
                        {
                            // [MỚI] Tự sinh Smart ID cho Hóa đơn (6 số)
                            maHD = Utilities.Generate(conn, tran, "HD", "HOADON", "MaHD", 6);

                            string sqlCreateHD = @"
                        INSERT INTO HOADON (MaHD, MaBN, MaPhieuKhamBenh, NgayThanhToan, TongTienGoc, TongTienBHYTChiTra, TongTienBenhNhanTra, TrangThaiThanhToan)
                        VALUES (@MaHD, @MaBN, @MaPKB, GETDATE(), 0, 0, 0, N'Chưa thanh toán')";
                            using (SqlCommand cmdCreateHD = new SqlCommand(sqlCreateHD, conn, tran))
                            {
                                cmdCreateHD.Parameters.AddWithValue("@MaHD", maHD);
                                cmdCreateHD.Parameters.AddWithValue("@MaBN", maBN);
                                cmdCreateHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                                cmdCreateHD.ExecuteNonQuery();
                            }
                        }

                        // 1.3 Nhóm các chỉ định theo phòng và lưu
                        var groupByPhong = model.ChiDinhs.GroupBy(x => x.MaPhong);
                        foreach (var group in groupByPhong)
                        {
                            decimal tongTienPhieu = 0;
                            foreach (var dvTmp in group)
                            {
                                using (SqlCommand cmdTong = new SqlCommand("SELECT GiaDichVu FROM DICHVU WHERE MaDV = @MaDV", conn, tran))
                                {
                                    cmdTong.Parameters.AddWithValue("@MaDV", dvTmp.MaDV);
                                    object giaObj = cmdTong.ExecuteScalar();
                                    if (giaObj == null) throw new Exception("Không tìm thấy giá dịch vụ CLS: " + dvTmp.MaDV);
                                    tongTienPhieu += Convert.ToDecimal(giaObj);
                                }
                            }

                            // [MỚI] Tự sinh Smart ID cho Phiếu chỉ định (6 số)
                            string maPhieuCD = Utilities.Generate(conn, tran, "PC", "PHIEU_CHIDINH", "MaPhieuChiDinh", 6);

                            // Tạo phiếu chỉ định cho từng phòng (Bỏ OUTPUT INSERTED)
                            string sqlPhieuCD = @"
                        INSERT INTO PHIEU_CHIDINH (MaPhieuChiDinh, MaPhieuKhamBenh, MaBacSiChiDinh, NgayChiDinh, TrangThai, TongTien, MaPhong)
                        VALUES (@MaPCD, @MaP, @MaBS, GETDATE(), N'Chưa thanh toán', @TongTien, @MaPhong)";

                            using (SqlCommand cmdPCD = new SqlCommand(sqlPhieuCD, conn, tran))
                            {
                                cmdPCD.Parameters.AddWithValue("@MaPCD", maPhieuCD);
                                cmdPCD.Parameters.AddWithValue("@MaP", model.MaPhieuKhamBenh);
                                cmdPCD.Parameters.AddWithValue("@MaBS", maBS);
                                cmdPCD.Parameters.AddWithValue("@TongTien", tongTienPhieu);
                                cmdPCD.Parameters.AddWithValue("@MaPhong", group.Key);
                                cmdPCD.ExecuteNonQuery();
                            }

                            // Xử lý từng dịch vụ
                            foreach (var dv in group)
                            {
                                decimal donGia;
                                using (SqlCommand cmdGia = new SqlCommand("SELECT GiaDichVu FROM DICHVU WHERE MaDV = @MaDV", conn, tran))
                                {
                                    cmdGia.Parameters.AddWithValue("@MaDV", dv.MaDV);
                                    donGia = Convert.ToDecimal(cmdGia.ExecuteScalar());
                                }

                                // [MỚI] Tự sinh Smart ID cho Chi tiết chỉ định (6 số)
                                string maCTChiDinh = Utilities.Generate(conn, tran, "CC", "CHITIET_CHIDINH", "MaCTChiDinh", 6);

                                // Lưu vào chi tiết chỉ định
                                string sqlCT = @"
                            INSERT INTO CHITIET_CHIDINH (MaCTChiDinh, MaPhieuChiDinh, MaDV, DonGia, TrangThai)
                            VALUES (@MaCTCD, @MaPCD, @MaDV, @DonGia, N'Chưa thực hiện')";
                                using (SqlCommand cmdCT = new SqlCommand(sqlCT, conn, tran))
                                {
                                    cmdCT.Parameters.AddWithValue("@MaCTCD", maCTChiDinh);
                                    cmdCT.Parameters.AddWithValue("@MaPCD", maPhieuCD);
                                    cmdCT.Parameters.AddWithValue("@MaDV", dv.MaDV);
                                    cmdCT.Parameters.AddWithValue("@DonGia", donGia);
                                    cmdCT.ExecuteNonQuery();
                                }

                                // [MỚI] Tự sinh Smart ID cho Chi tiết Hóa đơn Dịch vụ (6 số)
                                string maCTHD_DV = Utilities.Generate(conn, tran, "CHDV", "CT_HOADON_DV", "MaCTHD", 6);

                                // Đẩy vào Hóa đơn & TỰ ĐỘNG TRỪ BHYT
                                string sqlInsHD = @"
                            INSERT INTO CT_HOADON_DV (MaCTHD, MaHD, MaDV, DonGia, TongTienGoc, TienBHYTChiTra, TienBenhNhanTra, TrangThaiThanhToan)
                            SELECT 
                                @MaCTHD_DV, @MaHD, @MaDV, @DonGia, @DonGia,
                                CASE WHEN bn.BHYT = 1 AND dv.CoBHYT = 1 THEN @DonGia * (ISNULL(bn.MucHuongBHYT, 0) / 100.0) ELSE 0 END AS BHYTChiTra,
                                @DonGia - (CASE WHEN bn.BHYT = 1 AND dv.CoBHYT = 1 THEN @DonGia * (ISNULL(bn.MucHuongBHYT, 0) / 100.0) ELSE 0 END) AS BenhNhanTra,
                                N'Chưa thanh toán'
                            FROM BENHNHAN bn
                            CROSS JOIN DICHVU dv
                            WHERE bn.MaBN = @MaBN AND dv.MaDV = @MaDV";

                                using (SqlCommand cmdInsHD = new SqlCommand(sqlInsHD, conn, tran))
                                {
                                    cmdInsHD.Parameters.AddWithValue("@MaCTHD_DV", maCTHD_DV);
                                    cmdInsHD.Parameters.AddWithValue("@MaHD", maHD);
                                    cmdInsHD.Parameters.AddWithValue("@MaDV", dv.MaDV);
                                    cmdInsHD.Parameters.AddWithValue("@DonGia", donGia);
                                    cmdInsHD.Parameters.AddWithValue("@MaBN", maBN);
                                    cmdInsHD.ExecuteNonQuery();
                                }
                            }
                        }

                        // 1.4 Tính tổng tiền Hóa đơn CLS và chốt Trạng thái 
                        string sqlRecalcHD = @"
UPDATE hd
SET TongTienGoc = ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0),
    TongTienBHYTChiTra = ISNULL(dv.TongBHYT, 0) + ISNULL(th.TongBHYT, 0),
    TongTienBenhNhanTra = ISNULL(dv.TongBN, 0) + ISNULL(th.TongBN, 0),
    TrangThaiThanhToan = N'Thanh toán 1 phần'  
FROM HOADON hd
LEFT JOIN (
    SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN
    FROM CT_HOADON_DV WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD
) dv ON hd.MaHD = dv.MaHD
LEFT JOIN (
    SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN
    FROM CT_HOADON_THUOC WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD
) th ON hd.MaHD = th.MaHD
WHERE hd.MaHD = @MaHD";

                        using (SqlCommand cmdRecalc = new SqlCommand(sqlRecalcHD, conn, tran))
                        {
                            cmdRecalc.Parameters.AddWithValue("@MaHD", maHD);
                            cmdRecalc.ExecuteNonQuery();
                        }
                    }
                    // ========================================================================
                    // NHÁNH 2: NẾU KHÔNG CÓ CLS THÌ LƯU CHẨN ĐOÁN & KÊ THUỐC
                    // ========================================================================
                    else if (!model.YeuCauCanLamSang)
                    {
                        // 2.1 Lưu danh sách Bệnh (Chẩn đoán)
                        if (model.DanhSachMaBenh != null && model.DanhSachMaBenh.Count > 0)
                        {
                            // [MỚI] Thêm cột MaCTChanDoan
                            string sqlChanDoan = "INSERT INTO CHITIET_CHANDOAN (MaCTChanDoan, MaPhieuKhamBenh, MaBenh, LoaiBenh, GiaiDoan) VALUES (@MaCTCD, @MaPhieu, @MaBenh, N'Bệnh chính', 0)";
                            using (SqlCommand cmdCD = new SqlCommand(sqlChanDoan, conn, tran))
                            {
                                cmdCD.Parameters.Add("@MaCTCD", System.Data.SqlDbType.VarChar, 20); // Mới
                                cmdCD.Parameters.Add("@MaPhieu", System.Data.SqlDbType.VarChar, 20); // [ĐÃ SỬA]
                                cmdCD.Parameters.Add("@MaBenh", System.Data.SqlDbType.Char, 10);
                                foreach (var maBenh in model.DanhSachMaBenh)
                                {
                                    // [MỚI] Tự sinh Smart ID cho Chi tiết chẩn đoán
                                    string maCTChanDoan = Utilities.Generate(conn, tran, "CD", "CHITIET_CHANDOAN", "MaCTChanDoan", 6);
                                    cmdCD.Parameters["@MaCTCD"].Value = maCTChanDoan;
                                    cmdCD.Parameters["@MaPhieu"].Value = model.MaPhieuKhamBenh;
                                    cmdCD.Parameters["@MaBenh"].Value = maBenh;
                                    cmdCD.ExecuteNonQuery();
                                }
                            }
                        }

                        // 2.2 Kê Đơn Thuốc & Tạo Hóa Đơn Thuốc
                        if (model.DonThuoc != null && model.DonThuoc.Count > 0)
                        {
                            string maHD = ""; // [ĐÃ SỬA]
                            // Lấy hoặc tạo Hóa đơn
                            string sqlGetHD = "SELECT TOP 1 MaHD FROM HOADON WHERE MaPhieuKhamBenh = @MaPKB ORDER BY MaHD DESC";
                            using (SqlCommand cmdGetHD = new SqlCommand(sqlGetHD, conn, tran))
                            {
                                cmdGetHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                                object hdObj = cmdGetHD.ExecuteScalar();
                                maHD = hdObj != null ? hdObj.ToString() : ""; // [ĐÃ SỬA]
                            }

                            if (string.IsNullOrEmpty(maHD))
                            {
                                // [MỚI] Tự sinh Smart ID
                                maHD = Utilities.Generate(conn, tran, "HD", "HOADON", "MaHD", 6);

                                string sqlCreateHD = @"
                            INSERT INTO HOADON (MaHD, MaBN, MaPhieuKhamBenh, NgayThanhToan, TongTienGoc, TongTienBHYTChiTra, TongTienBenhNhanTra, TrangThaiThanhToan)
                            VALUES (@MaHD, @MaBN, @MaPKB, GETDATE(), 0, 0, 0, N'Chưa thanh toán')";
                                using (SqlCommand cmdCreateHD = new SqlCommand(sqlCreateHD, conn, tran))
                                {
                                    cmdCreateHD.Parameters.AddWithValue("@MaHD", maHD);
                                    cmdCreateHD.Parameters.AddWithValue("@MaBN", maBN);
                                    cmdCreateHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                                    cmdCreateHD.ExecuteNonQuery();
                                }
                            }

                            // [MỚI] Tự sinh Smart ID cho Đơn thuốc
                            string maDonThuoc = Utilities.Generate(conn, tran, "DT", "DON_THUOC", "MaDonThuoc", 6);

                            // Tạo ĐƠN THUỐC tổng (Bỏ OUTPUT INSERTED)
                            string sqlTaoDon = "INSERT INTO DON_THUOC (MaDonThuoc, MaPhieuKhamBenh, NgayKe, TrangThai) VALUES (@MaDT, @MaPhieu, GETDATE(), N'Chưa phát')";
                            using (SqlCommand cmdDT = new SqlCommand(sqlTaoDon, conn, tran))
                            {
                                cmdDT.Parameters.AddWithValue("@MaDT", maDonThuoc);
                                cmdDT.Parameters.AddWithValue("@MaPhieu", model.MaPhieuKhamBenh);
                                cmdDT.ExecuteNonQuery();
                            }

                            // Thêm Chi Tiết Thuốc và đẩy sang Hóa Đơn
                            string sqlChiTietThuoc = @"INSERT INTO CT_DON_THUOC 
    (MaCTDonThuoc, MaDonThuoc, MaThuoc, SoLuongSang, SoLuongTrua, SoLuongChieu, SoLuongToi, SoNgayDung, SoLuong, DonViTinh, DonGia, GhiChu) 
    VALUES (@MaCTDT, @MaDon, @MaThuoc, @S, @T, @C, @Toi, @SoNgay, @TongSL, @DVT, @DonGia, @GhiChu)";

                            string sqlHDThuoc = @"INSERT INTO CT_HOADON_THUOC 
    (MaCTHD, MaHD, MaCTDonThuoc, SoLuong, TongTienGoc, TienBHYTChiTra, TienBenhNhanTra, TrangThaiThanhToan)
    VALUES (@MaCTHD_T, @MaHD, @MaCTDonThuoc, @TongSL, @TongGoc, @TienBHYT, @TienBN, N'Chưa thanh toán')";

                            // Lấy thông tin BHYT của Bệnh Nhân 
                            bool bnCoBHYT = false;
                            decimal mucHuongBHYT = 0;
                            string sqlBN = "SELECT BHYT, MucHuongBHYT FROM BENHNHAN WHERE MaBN = @MaBN";
                            using (SqlCommand cmdBNInfo = new SqlCommand(sqlBN, conn, tran))
                            {
                                cmdBNInfo.Parameters.AddWithValue("@MaBN", maBN);
                                using (SqlDataReader drBN = cmdBNInfo.ExecuteReader())
                                {
                                    if (drBN.Read())
                                    {
                                        bnCoBHYT = drBN["BHYT"] != DBNull.Value ? Convert.ToBoolean(drBN["BHYT"]) : false;
                                        mucHuongBHYT = drBN["MucHuongBHYT"] != DBNull.Value ? Convert.ToDecimal(drBN["MucHuongBHYT"]) : 0;
                                    }
                                }
                            }

                            using (SqlCommand cmdCT = new SqlCommand(sqlChiTietThuoc, conn, tran))
                            using (SqlCommand cmdHDT = new SqlCommand(sqlHDThuoc, conn, tran))
                            {
                                foreach (var t in model.DonThuoc)
                                {
                                    string dvt = "Viên";
                                    decimal donGia = 0;
                                    bool coBHYT = false;

                                    // Lấy thông tin Thuốc
                                    string sqlInfo = "SELECT DonViCoBan, GiaBan, CoBHYT FROM THUOC WHERE MaThuoc = @MaTh";
                                    using (SqlCommand cmdInfo = new SqlCommand(sqlInfo, conn, tran))
                                    {
                                        cmdInfo.Parameters.AddWithValue("@MaTh", t.MaThuoc);
                                        using (SqlDataReader drInfo = cmdInfo.ExecuteReader())
                                        {
                                            if (drInfo.Read())
                                            {
                                                dvt = drInfo["DonViCoBan"].ToString();
                                                donGia = drInfo["GiaBan"] != DBNull.Value ? Convert.ToDecimal(drInfo["GiaBan"]) : 0;
                                                coBHYT = drInfo["CoBHYT"] != DBNull.Value ? Convert.ToBoolean(drInfo["CoBHYT"]) : false;
                                            }
                                        }
                                    }

                                    // TÍNH TOÁN 
                                    int tongSoLuong = t.SoLuong > 0 ? t.SoLuong : (int)Math.Ceiling((t.Sang + t.Trua + t.Chieu + t.Toi) * t.SoNgay);

                                    decimal tongGoc = tongSoLuong * donGia;
                                    decimal tienBHYT = (bnCoBHYT && coBHYT) ? (tongGoc * (mucHuongBHYT / 100m)) : 0;
                                    decimal tienBN = tongGoc - tienBHYT;

                                    // [MỚI] Tự sinh Smart ID cho Chi tiết Đơn thuốc
                                    string maCTDonThuoc = Utilities.Generate(conn, tran, "CTDT", "CT_DON_THUOC", "MaCTDonThuoc", 6);

                                    // 1. Thêm vào CT_DON_THUOC 
                                    cmdCT.Parameters.Clear();
                                    cmdCT.Parameters.AddWithValue("@MaCTDT", maCTDonThuoc); // [MỚI]
                                    cmdCT.Parameters.AddWithValue("@MaDon", maDonThuoc);
                                    cmdCT.Parameters.AddWithValue("@MaThuoc", t.MaThuoc);

                                    cmdCT.Parameters.Add("@S", System.Data.SqlDbType.Decimal).Value = t.Sang;
                                    cmdCT.Parameters["@S"].Precision = 5;
                                    cmdCT.Parameters["@S"].Scale = 2;

                                    cmdCT.Parameters.Add("@T", System.Data.SqlDbType.Decimal).Value = t.Trua;
                                    cmdCT.Parameters["@T"].Precision = 5;
                                    cmdCT.Parameters["@T"].Scale = 2;

                                    cmdCT.Parameters.Add("@C", System.Data.SqlDbType.Decimal).Value = t.Chieu;
                                    cmdCT.Parameters["@C"].Precision = 5;
                                    cmdCT.Parameters["@C"].Scale = 2;

                                    cmdCT.Parameters.Add("@Toi", System.Data.SqlDbType.Decimal).Value = t.Toi;
                                    cmdCT.Parameters["@Toi"].Precision = 5;
                                    cmdCT.Parameters["@Toi"].Scale = 2;

                                    cmdCT.Parameters.AddWithValue("@SoNgay", t.SoNgay);
                                    cmdCT.Parameters.AddWithValue("@TongSL", tongSoLuong);
                                    cmdCT.Parameters.AddWithValue("@DVT", dvt);
                                    cmdCT.Parameters.AddWithValue("@DonGia", donGia);
                                    cmdCT.Parameters.AddWithValue("@GhiChu", t.GhiChu ?? (object)DBNull.Value);
                                    cmdCT.ExecuteNonQuery();

                                    // [MỚI] Tự sinh Smart ID cho Chi tiết hóa đơn thuốc
                                    string maCTHD_Thuoc = Utilities.Generate(conn, tran, "CHTH", "CT_HOADON_THUOC", "MaCTHD", 6);

                                    // 2. Đẩy sang Thu Ngân (CT_HOADON_THUOC)
                                    cmdHDT.Parameters.Clear();
                                    cmdHDT.Parameters.AddWithValue("@MaCTHD_T", maCTHD_Thuoc); // [MỚI]
                                    cmdHDT.Parameters.AddWithValue("@MaHD", maHD);
                                    cmdHDT.Parameters.AddWithValue("@MaCTDonThuoc", maCTDonThuoc);
                                    cmdHDT.Parameters.AddWithValue("@TongSL", tongSoLuong);
                                    cmdHDT.Parameters.AddWithValue("@TongGoc", tongGoc);
                                    cmdHDT.Parameters.AddWithValue("@TienBHYT", tienBHYT);
                                    cmdHDT.Parameters.AddWithValue("@TienBN", tienBN);
                                    cmdHDT.ExecuteNonQuery();
                                }
                            }

                            // Cập nhật lại tổng tiền Hóa Đơn chính 
                            string sqlRecalcHD2 = @"
UPDATE hd
SET TongTienGoc = ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0),
    TongTienBHYTChiTra = ISNULL(dv.TongBHYT, 0) + ISNULL(th.TongBHYT, 0),
    TongTienBenhNhanTra = ISNULL(dv.TongBN, 0) + ISNULL(th.TongBN, 0),
    TrangThaiThanhToan = N'Thanh toán 1 phần'  
FROM HOADON hd
LEFT JOIN (
    SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN
    FROM CT_HOADON_DV WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD
) dv ON hd.MaHD = dv.MaHD
LEFT JOIN (
    SELECT MaHD, SUM(TongTienGoc) AS TongGoc, SUM(TienBHYTChiTra) AS TongBHYT, SUM(TienBenhNhanTra) AS TongBN
    FROM CT_HOADON_THUOC WHERE TrangThaiThanhToan != N'Hủy' GROUP BY MaHD
) th ON hd.MaHD = th.MaHD
WHERE hd.MaHD = @MaHD";

                            using (SqlCommand cmdRecalc2 = new SqlCommand(sqlRecalcHD2, conn, tran))
                            {
                                cmdRecalc2.Parameters.AddWithValue("@MaHD", maHD);
                                cmdRecalc2.ExecuteNonQuery();
                            }
                        }
                    }

                    // Mọi thao tác đều thành công, Commit dữ liệu vào Database
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Có lỗi xảy ra, lùi lại (Undo) toàn bộ thao tác để bảo toàn dữ liệu
                    tran.Rollback();
                    errorMsg = ex.Message;
                    return false;
                }
            }
        }

        // Lấy danh sách kết quả Cận lâm sàng của Phiếu khám
        // [ĐÃ SỬA] int maPKB -> string
        public List<dynamic> GetKetQuaCLS(string maPKB)
        {
            var list = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT
                        d.TenDV,
                        ct.KetQua,
                        pc.NgayChiDinh,
                        ct.ThoiGianCoKetQua
                    FROM CHITIET_CHIDINH ct
                    JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                    JOIN DICHVU d ON ct.MaDV = d.MaDV
                    WHERE pc.MaPhieuKhamBenh = @MaPKB
                      AND ct.TrangThai = N'Đã có kết quả'
                    ORDER BY pc.NgayChiDinh DESC, ct.MaCTChiDinh DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPKB", maPKB);
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string ngayThucHien = "";
                            if (dr["NgayChiDinh"] != DBNull.Value)
                            {
                                DateTime ngay = Convert.ToDateTime(dr["NgayChiDinh"]);
                                string gio = dr["ThoiGianCoKetQua"] != DBNull.Value ? dr["ThoiGianCoKetQua"].ToString() : "";
                                ngayThucHien = string.IsNullOrEmpty(gio)
                                    ? ngay.ToString("dd/MM/yyyy")
                                    : $"{ngay:dd/MM/yyyy} {gio}";
                            }

                            list.Add(new
                            {
                                TenDV = dr["TenDV"].ToString(),
                                NoiDungKetQua = dr["KetQua"] != DBNull.Value ? dr["KetQua"].ToString() : "",
                                NgayThucHien = ngayThucHien
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}