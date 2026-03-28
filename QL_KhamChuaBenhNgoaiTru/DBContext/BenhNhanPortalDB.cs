using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class BenhNhanPortalDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // ==================== 1. THÔNG TIN CÁ NHÂN ====================
        public BenhNhanProfile GetBenhNhanByMaBN(string maBN)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT bn.*, tk.Username, tk.IsActive
                    FROM BENHNHAN bn
                    LEFT JOIN TAIKHOAN tk ON bn.MaTK = tk.MaTK
                    WHERE bn.MaBN = @MaBN";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new BenhNhanProfile
                        {
                            MaBN = dr["MaBN"].ToString(),
                            HoTen = dr["HoTen"].ToString(),
                            NgaySinh = Convert.IsDBNull(dr["NgaySinh"]) ? (DateTime?)null : Convert.ToDateTime(dr["NgaySinh"]),
                            GioiTinh = Convert.IsDBNull(dr["GioiTinh"]) ? "" : dr["GioiTinh"].ToString(),
                            SDT = Convert.IsDBNull(dr["SDT"]) ? "" : dr["SDT"].ToString(),
                            Email = Convert.IsDBNull(dr["Email"]) ? "" : dr["Email"].ToString(),
                            DiaChi = Convert.IsDBNull(dr["DiaChi"]) ? "" : dr["DiaChi"].ToString(),
                            CCCD = Convert.IsDBNull(dr["CCCD"]) ? "" : dr["CCCD"].ToString(),
                            BHYT = !Convert.IsDBNull(dr["BHYT"]) && Convert.ToBoolean(dr["BHYT"]),
                            SoTheBHYT = Convert.IsDBNull(dr["SoTheBHYT"]) ? "" : dr["SoTheBHYT"].ToString(),
                            HanSuDungBHYT = Convert.IsDBNull(dr["HanSuDungBHYT"]) ? (DateTime?)null : Convert.ToDateTime(dr["HanSuDungBHYT"]),
                            TuyenKham = Convert.IsDBNull(dr["TuyenKham"]) ? "" : dr["TuyenKham"].ToString(),
                            MucHuongBHYT = Convert.IsDBNull(dr["MucHuongBHYT"]) ? (int?)null : Convert.ToInt32(dr["MucHuongBHYT"]),
                            Username = Convert.IsDBNull(dr["Username"]) ? "" : dr["Username"].ToString(),
                            IsActive = !Convert.IsDBNull(dr["IsActive"]) && Convert.ToBoolean(dr["IsActive"])
                        };
                    }
                }
            }
            return null;
        }

        public bool UpdateBenhNhan(BenhNhanProfile bn)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    UPDATE BENHNHAN SET
                        HoTen = @HoTen,
                        NgaySinh = @NgaySinh,
                        GioiTinh = @GioiTinh,
                        SDT = @SDT,
                        Email = @Email,
                        DiaChi = @DiaChi,
                        CCCD = @CCCD,
                        BHYT = @BHYT,
                        SoTheBHYT = @SoTheBHYT,
                        HanSuDungBHYT = @HanSuDungBHYT,
                        TuyenKham = @TuyenKham,
                        MucHuongBHYT = @MucHuongBHYT
                    WHERE MaBN = @MaBN";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", bn.MaBN);
                cmd.Parameters.AddWithValue("@HoTen", bn.HoTen ?? "");
                cmd.Parameters.AddWithValue("@NgaySinh", bn.NgaySinh.HasValue ? (object)bn.NgaySinh.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@GioiTinh", bn.GioiTinh ?? "");
                cmd.Parameters.AddWithValue("@SDT", string.IsNullOrEmpty(bn.SDT) ? (object)DBNull.Value : bn.SDT);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(bn.Email) ? (object)DBNull.Value : bn.Email);
                cmd.Parameters.AddWithValue("@DiaChi", bn.DiaChi ?? "");
                cmd.Parameters.AddWithValue("@CCCD", string.IsNullOrEmpty(bn.CCCD) ? (object)DBNull.Value : bn.CCCD);
                cmd.Parameters.AddWithValue("@BHYT", bn.BHYT);
                cmd.Parameters.AddWithValue("@SoTheBHYT", string.IsNullOrEmpty(bn.SoTheBHYT) ? (object)DBNull.Value : bn.SoTheBHYT);
                cmd.Parameters.AddWithValue("@HanSuDungBHYT", bn.HanSuDungBHYT.HasValue ? (object)bn.HanSuDungBHYT.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@TuyenKham", string.IsNullOrEmpty(bn.TuyenKham) ? (object)DBNull.Value : bn.TuyenKham);
                cmd.Parameters.AddWithValue("@MucHuongBHYT", bn.MucHuongBHYT.HasValue ? (object)bn.MucHuongBHYT.Value : DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================== 2. ĐẶT LỊCH KHÁM ====================
        public List<PhongInfo> GetAllPhongKham()
        {
            var list = new List<PhongInfo>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT P.MaPhong, P.TenPhong, K.TenKhoa
                    FROM PHONG P
                    INNER JOIN KHOA K ON P.MaKhoa = K.MaKhoa
                    INNER JOIN DANHMUC_LOAIPHONG L ON P.MaLoaiPhong = L.MaLoaiPhong
                    WHERE P.TrangThai = 1
                      AND L.TenLoaiPhong LIKE N'%Khám%'
                    ORDER BY K.TenKhoa, P.TenPhong";
                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PhongInfo
                        {
                            MaPhong = Convert.ToInt32(dr["MaPhong"]),
                            TenPhong = dr["TenPhong"].ToString(),
                            TenKhoa = dr["TenKhoa"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public int DatLichKham(string maBN, DateTime ngayKham, string maDV, string lyDo, out string tenQuayTiepTan)
        {
            tenQuayTiepTan = "Quầy Tiếp Tân"; // Giá trị mặc định phòng hờ
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // ==========================================================
                // 0. KIỂM TRA GIỚI HẠN 100 LƯỢT / NGÀY TẠI ĐÂY
                // ==========================================================
                string sqlCheckLimit = @"
            SELECT COUNT(*) 
            FROM PHIEUDANGKY 
            WHERE CAST(NgayDangKy AS DATE) = CAST(@NgayKham AS DATE) 
              AND HinhThucDangKy = N'Online'";

                SqlCommand cmdCheck = new SqlCommand(sqlCheckLimit, conn);
                cmdCheck.Parameters.AddWithValue("@NgayKham", ngayKham);
                int soLuotDaDangKy = Convert.ToInt32(cmdCheck.ExecuteScalar());

                if (soLuotDaDangKy >= 100)
                {
                    // Bắn lỗi ra để Controller bắt được
                    throw new Exception($"Rất tiếc, ngày {ngayKham:dd/MM/yyyy} đã nhận đủ giới hạn 100 lượt đặt trước. Vui lòng chọn một ngày khác!");
                }

                // ==========================================================
                // 1. Lấy tên Dịch vụ để ghép vào Lý do 
                string sqlTenDV = "SELECT TenDV FROM DICHVU WHERE MaDV = @MaDV";
                SqlCommand cmdDV = new SqlCommand(sqlTenDV, conn);
                cmdDV.Parameters.AddWithValue("@MaDV", maDV);
                var tenDV = cmdDV.ExecuteScalar()?.ToString() ?? "Khám bệnh";

                // Ghi rõ Dịch vụ và Triệu chứng
                string lyDoGop = $"Dịch vụ: {tenDV} | Triệu chứng: {lyDo}";

                // 2. Tìm QUẦY TIẾP TÂN vắng nhất (Lấy CẢ MaPhong VÀ TenPhong)
                string sqlTimQuay = @"
        SELECT TOP 1 p.MaPhong, p.TenPhong
        FROM PHONG p
        LEFT JOIN PHIEUDANGKY pdk ON p.MaPhong = pdk.MaPhong 
            AND CAST(pdk.NgayDangKy AS DATE) = CAST(@NgayKham AS DATE)
            AND pdk.HinhThucDangKy = N'Online' 
        WHERE p.MaLoaiPhong = 1 AND p.TrangThai = 1
        GROUP BY p.MaPhong, p.TenPhong
        ORDER BY COUNT(pdk.MaPhieuDK) ASC, p.TenPhong ASC";

                SqlCommand cmdQuay = new SqlCommand(sqlTimQuay, conn);
                cmdQuay.Parameters.AddWithValue("@NgayKham", ngayKham);

                int maQuayTiepTan = 1;
                using (SqlDataReader dr = cmdQuay.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        maQuayTiepTan = Convert.ToInt32(dr["MaPhong"]);
                        tenQuayTiepTan = dr["TenPhong"].ToString(); // Lấy tên quầy thật từ DB!
                    }
                }

                // 3. Tạo PHIEUDANGKY
                string sqlInsert = @"
        INSERT INTO PHIEUDANGKY (MaBN, NgayDangKy, STT, HinhThucDangKy, TrangThai, LyDo, MaPhong)
        OUTPUT INSERTED.MaPhieuDK
        VALUES (@MaBN, @NgayKham, NULL, N'Online', N'Chờ xử lý', @LyDo, @MaPhong)";

                SqlCommand cmd = new SqlCommand(sqlInsert, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                cmd.Parameters.AddWithValue("@NgayKham", ngayKham);
                cmd.Parameters.AddWithValue("@LyDo", lyDoGop);
                cmd.Parameters.AddWithValue("@MaPhong", maQuayTiepTan);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ==================== 3. XEM / HỦY LỊCH KHÁM ====================
        public List<LichKhamItem> GetLichKhamByMaBN(string maBN)
        {
            var list = new List<LichKhamItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT PDK.MaPhieuDK, PDK.NgayDangKy, PDK.HinhThucDangKy, PDK.TrangThai,
                           PKB.MaPhieuKhamBenh, PKB.STT, PKB.TrangThai AS TrangThaiKham,
                           PKB.NgayLap AS NgayKham, PKB.LyDoDenKham,
                           P.TenPhong, K.TenKhoa,
                           NV.HoTen AS TenBacSi
                    FROM PHIEUDANGKY PDK
                    LEFT JOIN PHIEUKHAMBENH PKB ON PDK.MaPhieuDK = PKB.MaPhieuDK
                    LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong
                    LEFT JOIN KHOA K ON P.MaKhoa = K.MaKhoa
                    LEFT JOIN NHANVIEN NV ON PKB.MaBacSiKham = NV.MaNV
                    WHERE PDK.MaBN = @MaBN
                    ORDER BY PDK.NgayDangKy DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new LichKhamItem
                        {
                            MaPhieuDK = Convert.ToInt32(dr["MaPhieuDK"]),
                            NgayDangKy = Convert.IsDBNull(dr["NgayDangKy"]) ? (DateTime?)null : Convert.ToDateTime(dr["NgayDangKy"]),
                            HinhThucDangKy = Convert.IsDBNull(dr["HinhThucDangKy"]) ? "" : dr["HinhThucDangKy"].ToString(),
                            TrangThai = Convert.IsDBNull(dr["TrangThai"]) ? "" : dr["TrangThai"].ToString(),
                            MaPhieuKhamBenh = Convert.IsDBNull(dr["MaPhieuKhamBenh"]) ? (int?)null : Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                            STT = Convert.IsDBNull(dr["STT"]) ? (int?)null : Convert.ToInt32(dr["STT"]),
                            TrangThaiKham = Convert.IsDBNull(dr["TrangThaiKham"]) ? "" : dr["TrangThaiKham"].ToString(),
                            NgayKham = Convert.IsDBNull(dr["NgayKham"]) ? (DateTime?)null : Convert.ToDateTime(dr["NgayKham"]),
                            LyDoDenKham = Convert.IsDBNull(dr["LyDoDenKham"]) ? "" : dr["LyDoDenKham"].ToString(),
                            TenPhong = Convert.IsDBNull(dr["TenPhong"]) ? "" : dr["TenPhong"].ToString(),
                            TenKhoa = Convert.IsDBNull(dr["TenKhoa"]) ? "" : dr["TenKhoa"].ToString(),
                            TenBacSi = Convert.IsDBNull(dr["TenBacSi"]) ? "" : dr["TenBacSi"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool HuyLichKham(int maPhieuDK, string maBN)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = "UPDATE PHIEUDANGKY SET TrangThai = N'Hủy' WHERE MaPhieuDK = @MaPhieuDK AND MaBN = @MaBN AND TrangThai = N'Chờ xử lý'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaPhieuDK", maPhieuDK);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==================== 4. TRẠNG THÁI KHÁM ====================
        public List<TrangThaiKhamItem> GetTrangThaiKhamByMaBN(string maBN)
        {
            var list = new List<TrangThaiKhamItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT PKB.MaPhieuKhamBenh, PKB.NgayLap, PKB.LyDoDenKham,
                           PKB.TrangThai, PKB.STT,
                           P.TenPhong, K.TenKhoa,
                           NV.HoTen AS TenBacSi
                    FROM PHIEUKHAMBENH PKB
                    LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong
                    LEFT JOIN KHOA K ON P.MaKhoa = K.MaKhoa
                    LEFT JOIN NHANVIEN NV ON PKB.MaBacSiKham = NV.MaNV
                    WHERE PKB.MaBN = @MaBN
                    ORDER BY PKB.NgayLap DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new TrangThaiKhamItem
                        {
                            MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                            NgayLap = Convert.ToDateTime(dr["NgayLap"]),
                            LyDoDenKham = Convert.IsDBNull(dr["LyDoDenKham"]) ? "" : dr["LyDoDenKham"].ToString(),
                            TrangThai = dr["TrangThai"].ToString(),
                            STT = Convert.IsDBNull(dr["STT"]) ? (int?)null : Convert.ToInt32(dr["STT"]),
                            TenPhong = Convert.IsDBNull(dr["TenPhong"]) ? "" : dr["TenPhong"].ToString(),
                            TenKhoa = Convert.IsDBNull(dr["TenKhoa"]) ? "" : dr["TenKhoa"].ToString(),
                            TenBacSi = Convert.IsDBNull(dr["TenBacSi"]) ? "" : dr["TenBacSi"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==================== 5. LỊCH SỬ KHÁM ====================
        public List<LichSuKhamItem> GetLichSuKhamByMaBN(string maBN)
        {
            var list = new List<LichSuKhamItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT PKB.MaPhieuKhamBenh, PKB.NgayLap, PKB.LyDoDenKham,
                           PKB.TrieuChung, PKB.KetLuan,
                           PKB.TrangThai,
                           P.TenPhong, K.TenKhoa,
                           NV.HoTen AS TenBacSi
                    FROM PHIEUKHAMBENH PKB
                    LEFT JOIN PHONG P ON PKB.MaPhong = P.MaPhong
                    LEFT JOIN KHOA K ON P.MaKhoa = K.MaKhoa
                    LEFT JOIN NHANVIEN NV ON PKB.MaBacSiKham = NV.MaNV
                    WHERE PKB.MaBN = @MaBN
                    ORDER BY PKB.NgayLap DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new LichSuKhamItem
                        {
                            MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                            NgayKham = Convert.ToDateTime(dr["NgayLap"]),
                            LyDoDenKham = Convert.IsDBNull(dr["LyDoDenKham"]) ? "" : dr["LyDoDenKham"].ToString(),
                            TrieuChung = Convert.IsDBNull(dr["TrieuChung"]) ? "" : dr["TrieuChung"].ToString(),
                            KetLuan = Convert.IsDBNull(dr["KetLuan"]) ? "" : dr["KetLuan"].ToString(),
                            TrangThai = dr["TrangThai"].ToString(),
                            TenPhong = Convert.IsDBNull(dr["TenPhong"]) ? "" : dr["TenPhong"].ToString(),
                            TenKhoa = Convert.IsDBNull(dr["TenKhoa"]) ? "" : dr["TenKhoa"].ToString(),
                            TenBacSi = Convert.IsDBNull(dr["TenBacSi"]) ? "" : dr["TenBacSi"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==================== 6. ĐƠN THUỐC ====================
        public List<DonThuocItem> GetDonThuocByMaBN(string maBN)
        {
            var list = new List<DonThuocItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT DT.MaDonThuoc, DT.NgayKe, DT.LoiDanBS, DT.TrangThai,
                           PKB.NgayLap AS NgayKham, PKB.MaPhieuKhamBenh,
                           NV.HoTen AS TenBacSi
                    FROM DON_THUOC DT
                    INNER JOIN PHIEUKHAMBENH PKB ON DT.MaPhieuKhamBenh = PKB.MaPhieuKhamBenh
                    LEFT JOIN NHANVIEN NV ON PKB.MaBacSiKham = NV.MaNV
                    WHERE PKB.MaBN = @MaBN
                    ORDER BY DT.NgayKe DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new DonThuocItem
                        {
                            MaDonThuoc = Convert.ToInt32(dr["MaDonThuoc"]),
                            NgayKe = Convert.ToDateTime(dr["NgayKe"]),
                            LoiDanBS = Convert.IsDBNull(dr["LoiDanBS"]) ? "" : dr["LoiDanBS"].ToString(),
                            TrangThai = Convert.IsDBNull(dr["TrangThai"]) ? "" : dr["TrangThai"].ToString(),
                            NgayKham = Convert.ToDateTime(dr["NgayKham"]),
                            MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                            TenBacSi = Convert.IsDBNull(dr["TenBacSi"]) ? "" : dr["TenBacSi"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public DonThuocChiTiet GetChiTietDonThuoc(int maDonThuoc)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sqlThuoc = @"
                    SELECT CT.*, T.TenThuoc, T.DonViCoBan
                    FROM CT_DON_THUOC CT
                    INNER JOIN THUOC T ON CT.MaThuoc = T.MaThuoc
                    WHERE CT.MaDonThuoc = @MaDonThuoc
                    ORDER BY CT.MaCTDonThuoc";
                SqlCommand cmdThuoc = new SqlCommand(sqlThuoc, conn);
                cmdThuoc.Parameters.AddWithValue("@MaDonThuoc", maDonThuoc);
                var chiTietList = new List<ChiTietThuocItem>();
                using (SqlDataReader dr = cmdThuoc.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        chiTietList.Add(new ChiTietThuocItem
                        {
                            MaCTDonThuoc = Convert.ToInt32(dr["MaCTDonThuoc"]),
                            MaThuoc = dr["MaThuoc"].ToString(),
                            TenThuoc = dr["TenThuoc"].ToString(),
                            SoLuongSang = Convert.IsDBNull(dr["SoLuongSang"]) ? 0 : Convert.ToDecimal(dr["SoLuongSang"]),
                            SoLuongTrua = Convert.IsDBNull(dr["SoLuongTrua"]) ? 0 : Convert.ToDecimal(dr["SoLuongTrua"]),
                            SoLuongChieu = Convert.IsDBNull(dr["SoLuongChieu"]) ? 0 : Convert.ToDecimal(dr["SoLuongChieu"]),
                            SoLuongToi = Convert.IsDBNull(dr["SoLuongToi"]) ? 0 : Convert.ToDecimal(dr["SoLuongToi"]),
                            SoNgayDung = Convert.IsDBNull(dr["SoNgayDung"]) ? 0 : Convert.ToInt32(dr["SoNgayDung"]),
                            SoLuong = Convert.ToInt32(dr["SoLuong"]),
                            DonViTinh = Convert.IsDBNull(dr["DonViTinh"]) ? "" : dr["DonViTinh"].ToString(),
                            DonGia = Convert.IsDBNull(dr["DonGia"]) ? 0 : Convert.ToDecimal(dr["DonGia"]),
                            GhiChu = Convert.IsDBNull(dr["GhiChu"]) ? "" : dr["GhiChu"].ToString(),
                            DonViCoBan = Convert.IsDBNull(dr["DonViCoBan"]) ? "" : dr["DonViCoBan"].ToString()
                        });
                    }
                }
                return new DonThuocChiTiet { ChiTiet = chiTietList };
            }
        }

        // ==================== 7. HÓA ĐƠN ====================
        public List<HoaDonItem> GetHoaDonByMaBN(string maBN)
        {
            var list = new List<HoaDonItem>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sql = @"
                    SELECT HD.MaHD, HD.NgayThanhToan, HD.TongTien, HD.TrangThaiThanhToan,
                           HD.HinhThucThanhToan, HD.GhiChu,
                           PKB.MaPhieuKhamBenh, PKB.NgayLap AS NgayKham
                    FROM HOADON HD
                    LEFT JOIN PHIEUKHAMBENH PKB ON HD.MaPhieuKhamBenh = PKB.MaPhieuKhamBenh
                    WHERE HD.MaBN = @MaBN
                    ORDER BY HD.NgayThanhToan DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBN", maBN);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new HoaDonItem
                        {
                            MaHD = Convert.ToInt32(dr["MaHD"]),
                            NgayThanhToan = Convert.IsDBNull(dr["NgayThanhToan"]) ? (DateTime?)null : Convert.ToDateTime(dr["NgayThanhToan"]),
                            TongTien = Convert.IsDBNull(dr["TongTien"]) ? 0 : Convert.ToDecimal(dr["TongTien"]),
                            TrangThaiThanhToan = Convert.IsDBNull(dr["TrangThaiThanhToan"]) ? "" : dr["TrangThaiThanhToan"].ToString(),
                            HinhThucThanhToan = Convert.IsDBNull(dr["HinhThucThanhToan"]) ? "" : dr["HinhThucThanhToan"].ToString(),
                            GhiChu = Convert.IsDBNull(dr["GhiChu"]) ? "" : dr["GhiChu"].ToString(),
                            MaPhieuKhamBenh = Convert.IsDBNull(dr["MaPhieuKhamBenh"]) ? (int?)null : Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                            NgayKham = Convert.IsDBNull(dr["NgayKham"]) ? (DateTime?)null : Convert.ToDateTime(dr["NgayKham"])
                        });
                    }
                }
            }
            return list;
        }

        public HoaDonChiTiet GetChiTietHoaDon(int maHD)
        {
            var ketQua = new HoaDonChiTiet();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                string sqlHD = "SELECT * FROM HOADON WHERE MaHD = @MaHD";
                SqlCommand cmdHD = new SqlCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@MaHD", maHD);
                using (SqlDataReader dr = cmdHD.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        ketQua.MaHD = Convert.ToInt32(dr["MaHD"]);
                        ketQua.NgayThanhToan = Convert.IsDBNull(dr["NgayThanhToan"]) ? (DateTime?)null : Convert.ToDateTime(dr["NgayThanhToan"]);
                        ketQua.TongTien = Convert.IsDBNull(dr["TongTien"]) ? 0 : Convert.ToDecimal(dr["TongTien"]);
                        ketQua.TienBHYTChiTra = Convert.IsDBNull(dr["TienBHYTChiTra"]) ? 0 : Convert.ToDecimal(dr["TienBHYTChiTra"]);
                        ketQua.TienBenhNhanTra = Convert.IsDBNull(dr["TienBenhNhanTra"]) ? 0 : Convert.ToDecimal(dr["TienBenhNhanTra"]);
                        ketQua.TrangThaiThanhToan = Convert.IsDBNull(dr["TrangThaiThanhToan"]) ? "" : dr["TrangThaiThanhToan"].ToString();
                        ketQua.HinhThucThanhToan = Convert.IsDBNull(dr["HinhThucThanhToan"]) ? "" : dr["HinhThucThanhToan"].ToString();
                    }
                }
            }
            return ketQua;
        }

        // ==================== 8. DASHBOARD TỔNG QUAN ====================
        public DashboardBenhNhan GetDashboard(string maBN)
        {
            var kq = new DashboardBenhNhan();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                var lichKham = GetLichKhamByMaBN(maBN);
                kq.SoLichKhamChoDuyet = 0;
                kq.SoLichKhamDaXacNhan = 0;
                foreach (var lk in lichKham)
                {
                    if (lk.TrangThai == "Chờ xử lý") kq.SoLichKhamChoDuyet++;
                    else if (lk.TrangThai == "Đã xác nhận") kq.SoLichKhamDaXacNhan++;
                }

                string sqlKham = "SELECT COUNT(*) FROM PHIEUKHAMBENH WHERE MaBN = @MaBN";
                SqlCommand cmdKham = new SqlCommand(sqlKham, conn);
                cmdKham.Parameters.AddWithValue("@MaBN", maBN);
                kq.TongSoLanKham = Convert.ToInt32(cmdKham.ExecuteScalar());

                string sqlDonThuoc = "SELECT COUNT(DISTINCT DT.MaDonThuoc) FROM DON_THUOC DT INNER JOIN PHIEUKHAMBENH PKB ON DT.MaPhieuKhamBenh = PKB.MaPhieuKhamBenh WHERE PKB.MaBN = @MaBN";
                SqlCommand cmdDon = new SqlCommand(sqlDonThuoc, conn);
                cmdDon.Parameters.AddWithValue("@MaBN", maBN);
                kq.TongDonThuoc = Convert.ToInt32(cmdDon.ExecuteScalar());

                string sqlHD = "SELECT COUNT(*) FROM HOADON WHERE MaBN = @MaBN";
                SqlCommand cmdHD = new SqlCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@MaBN", maBN);
                kq.TongHoaDon = Convert.ToInt32(cmdHD.ExecuteScalar());
            }
            return kq;
        }

        // ==================== VIEW MODELS ====================
        public class BenhNhanProfile
        {
            public string MaBN { get; set; }
            public string HoTen { get; set; }
            public DateTime? NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string SDT { get; set; }
            public string Email { get; set; }
            public string DiaChi { get; set; }
            public string CCCD { get; set; }
            public bool BHYT { get; set; }
            public string SoTheBHYT { get; set; }
            public DateTime? HanSuDungBHYT { get; set; }
            public string TuyenKham { get; set; }
            public int? MucHuongBHYT { get; set; }
            public string Username { get; set; }
            public bool IsActive { get; set; }
        }

        public class PhongInfo
        {
            public int MaPhong { get; set; }
            public string TenPhong { get; set; }
            public string TenKhoa { get; set; }
        }

        public class LichKhamItem
        {
            public int MaPhieuDK { get; set; }
            public DateTime? NgayDangKy { get; set; }
            public string HinhThucDangKy { get; set; }
            public string TrangThai { get; set; }
            public string LyDo { get; set; }     
            public string TenDV { get; set; }
            public int? MaPhieuKhamBenh { get; set; }
            public int? STT { get; set; }
            public string TrangThaiKham { get; set; }
            public DateTime? NgayKham { get; set; }
            public string LyDoDenKham { get; set; }
            public string TenPhong { get; set; }
            public string TenKhoa { get; set; }
            public string TenBacSi { get; set; }
        }

        public class TrangThaiKhamItem
        {
            public int MaPhieuKhamBenh { get; set; }
            public DateTime NgayLap { get; set; }
            public string LyDoDenKham { get; set; }
            public string TrangThai { get; set; }
            public int? STT { get; set; }
            public string TenPhong { get; set; }
            public string TenKhoa { get; set; }
            public string TenBacSi { get; set; }
        }

        public class LichSuKhamItem
        {
            public int MaPhieuKhamBenh { get; set; }
            public DateTime NgayKham { get; set; }
            public string LyDoDenKham { get; set; }
            public string TrieuChung { get; set; }
            public string KetLuan { get; set; }
            public string TrangThai { get; set; }
            public string TenPhong { get; set; }
            public string TenKhoa { get; set; }
            public string TenBacSi { get; set; }
        }

        public class DonThuocItem
        {
            public int MaDonThuoc { get; set; }
            public DateTime NgayKe { get; set; }
            public string LoiDanBS { get; set; }
            public string TrangThai { get; set; }
            public DateTime NgayKham { get; set; }
            public int MaPhieuKhamBenh { get; set; }
            public string TenBacSi { get; set; }
        }

        public class ChiTietThuocItem
        {
            public int MaCTDonThuoc { get; set; }
            public string MaThuoc { get; set; }
            public string TenThuoc { get; set; }
            public decimal SoLuongSang { get; set; }
            public decimal SoLuongTrua { get; set; }
            public decimal SoLuongChieu { get; set; }
            public decimal SoLuongToi { get; set; }
            public int SoNgayDung { get; set; }
            public int SoLuong { get; set; }
            public string DonViTinh { get; set; }
            public decimal DonGia { get; set; }
            public string GhiChu { get; set; }
            public string DonViCoBan { get; set; }
        }

        public class DonThuocChiTiet
        {
            public List<ChiTietThuocItem> ChiTiet { get; set; } = new List<ChiTietThuocItem>();
        }

        public class HoaDonItem
        {
            public int MaHD { get; set; }
            public DateTime? NgayThanhToan { get; set; }
            public decimal TongTien { get; set; }
            public string TrangThaiThanhToan { get; set; }
            public string HinhThucThanhToan { get; set; }
            public string GhiChu { get; set; }
            public int? MaPhieuKhamBenh { get; set; }
            public DateTime? NgayKham { get; set; }
        }

        public class HoaDonChiTiet
        {
            public int MaHD { get; set; }
            public DateTime? NgayThanhToan { get; set; }
            public decimal TongTien { get; set; }
            public decimal TienBHYTChiTra { get; set; }
            public decimal TienBenhNhanTra { get; set; }
            public string TrangThaiThanhToan { get; set; }
            public string HinhThucThanhToan { get; set; }
        }

        public class DashboardBenhNhan
        {
            public int SoLichKhamChoDuyet { get; set; }
            public int SoLichKhamDaXacNhan { get; set; }
            public int TongSoLanKham { get; set; }
            public int TongDonThuoc { get; set; }
            public int TongHoaDon { get; set; }
        }
    }
}
