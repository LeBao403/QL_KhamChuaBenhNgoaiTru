using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class BacSiDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. Lấy danh sách phiếu khám theo trạng thái (Chờ khám, Đang khám)
        // 1. CẬP NHẬT HÀM LẤY DANH SÁCH KHÁM
        public List<PhieuKhamBenhInfo> GetDanhSachPhieuKham(string maBS, string trangThai)
        {
            var list = new List<PhieuKhamBenhInfo>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // BƯỚC 1: Lấy Mã Phòng mà bác sĩ này đang ngồi trực
                int maPhongBS = 0;
                string sqlPhong = "SELECT MaPhong FROM NHANVIEN WHERE MaNV = @MaBS";
                using (SqlCommand cmdP = new SqlCommand(sqlPhong, conn))
                {
                    cmdP.Parameters.AddWithValue("@MaBS", maBS);
                    var obj = cmdP.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value) maPhongBS = Convert.ToInt32(obj);
                }

                // BƯỚC 2: Xây dựng câu lệnh lấy danh sách bệnh nhân
                string sql = @"
            SELECT PKB.MaPhieuKhamBenh, PKB.MaBN, PKB.STT, PKB.LyDoDenKham, PKB.TrangThai, 
                   BN.HoTen, BN.GioiTinh, BN.NgaySinh
            FROM PHIEUKHAMBENH PKB
            JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN
            WHERE 1=1 ";

                // LOGIC MỚI:
                // - Ưu tiên bệnh nhân 'Đã có kết quả CLS' lên đầu, sau đó mới tới 'Chờ khám'
                if (trangThai == "Chờ khám" || trangThai == "Chờ Khám")
                {
                    sql += " AND PKB.MaPhong = @MaPhong AND (PKB.TrangThai = N'Chờ khám' OR PKB.TrangThai = N'Đã có kết quả CLS') ";
                    // Order by ưu tiên: Đã có kết quả CLS (ưu tiên 1), Chờ khám (ưu tiên 2), sau đó sắp xếp theo STT
                    sql += " ORDER BY CASE WHEN PKB.TrangThai = N'Đã có kết quả CLS' THEN 1 ELSE 2 END ASC, PKB.STT ASC";
                }
                else
                {
                    sql += " AND PKB.MaBacSiKham = @MaBS AND PKB.TrangThai = @TrangThai ";
                    sql += " ORDER BY PKB.STT ASC";
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Vẫn giữ parameter nhưng không map cứng ở WHERE nữa vì câu lệnh if trên đã gộp chung hoặc tách riêng.
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
                                MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]),
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

        // 2. CẬP NHẬT HÀM TIẾP NHẬN BỆNH NHÂN (LƯU THÊM MÃ BÁC SĨ)
        public PhieuKhamBenhInfo TiepNhan(int maPhieu, string maBS)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                // Gán đích danh MaBacSiKham = @MaBS khi bác sĩ nhấn nút Tiếp nhận
                string sqlUpdate = "UPDATE PHIEUKHAMBENH SET TrangThai = N'Đang khám', MaBacSiKham = @MaBS WHERE MaPhieuKhamBenh = @MaPhieu";
                using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdUpdate.Parameters.AddWithValue("@MaBS", maBS);
                    cmdUpdate.ExecuteNonQuery();
                }

                // Kéo dữ liệu bệnh nhân lên View
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
                                MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                                TenBN = dr["HoTen"].ToString(),
                                GioiTinh = dr["GioiTinh"].ToString(),
                                Tuoi = dr["NgaySinh"] != DBNull.Value ? DateTime.Now.Year - Convert.ToDateTime(dr["NgaySinh"]).Year : 0,
                                LyDoDenKham = dr["LyDoDenKham"].ToString(),
                                TrieuChung = dr["TrieuChung"] != DBNull.Value ? dr["TrieuChung"].ToString() : "" // ---> THÊM DÒNG NÀY
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 3. Lấy Danh mục bệnh
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

        // 4. Lấy Danh mục thuốc đang hoạt động
        public List<Thuoc> GetDanhSachThuoc()
        {
            var list = new List<Thuoc>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Đã bổ sung GiaBan và CoBHYT vào câu SELECT
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
        public PhieuKhamBenhInfo GetThongTinPhieuKham(int maPhieu)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string sqlGet = @"
            SELECT PKB.MaPhieuKhamBenh, PKB.LyDoDenKham, BN.HoTen, BN.GioiTinh, BN.NgaySinh
            FROM PHIEUKHAMBENH PKB
            JOIN BENHNHAN BN ON PKB.MaBN = BN.MaBN
            WHERE PKB.MaPhieuKhamBenh = @MaPhieu";

                using (SqlCommand cmdGet = new SqlCommand(sqlGet, conn))
                {
                    cmdGet.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    conn.Open();
                    using (SqlDataReader dr = cmdGet.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new PhieuKhamBenhInfo
                            {
                                MaPhieuKhamBenh = Convert.ToInt32(dr["MaPhieuKhamBenh"]),
                                TenBN = dr["HoTen"].ToString(),
                                GioiTinh = dr["GioiTinh"].ToString(),
                                Tuoi = dr["NgaySinh"] != DBNull.Value ? DateTime.Now.Year - Convert.ToDateTime(dr["NgaySinh"]).Year : 0,
                                LyDoDenKham = dr["LyDoDenKham"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 2. Hàm lấy danh sách Dịch vụ CLS (Trạng thái = 1)
        public List<DichVu> GetDanhSachDichVuCLS()
        {
            var list = new List<DichVu>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Chỉ lấy các dịch vụ thuộc nhóm Cận Lâm Sàng dựa theo DB thực tế:
                // LDV02: Xét nghiệm, LDV03: Chẩn đoán hình ảnh, LDV04: Thăm dò chức năng, LDV05: Nội soi
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
                                TenDV = dr["TenDV"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 3. Hàm tìm các Phòng phù hợp cho Dịch vụ (Sắp xếp theo số người chờ ít nhất)
        public List<dynamic> GetPhongPhuHop(string maDV)
        {
            var list = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Lấy Khoa của Dịch vụ, sau đó tìm các phòng thuộc Khoa đó.
                // Đếm số lượng phiếu chỉ định đang chờ thực hiện tại phòng đó.
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

        // 4. HÀM LƯU KHÁM BỆNH (Đã tích hợp Tự động trừ BHYT, cập nhật Hóa đơn CLS và Hóa đơn Thuốc)
        public bool LuuKhamBenh(KhamBenhViewModel model, string maBS, out string errorMsg)
        {
            errorMsg = "";
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. LẤY MÃ BỆNH NHÂN (Dùng chung cho việc tạo Hóa đơn và tính BHYT ở cả 2 nhánh)
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
                        int maHD = 0;

                        // 1.1 Tìm hóa đơn hiện tại của lượt khám
                        string sqlGetHD = "SELECT TOP 1 MaHD FROM HOADON WHERE MaPhieuKhamBenh = @MaPKB ORDER BY MaHD DESC";
                        using (SqlCommand cmdGetHD = new SqlCommand(sqlGetHD, conn, tran))
                        {
                            cmdGetHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                            object hdObj = cmdGetHD.ExecuteScalar();
                            maHD = hdObj != null ? Convert.ToInt32(hdObj) : 0;
                        }

                        // 1.2 Nếu chưa có Hóa đơn thì tạo mới
                        if (maHD == 0)
                        {
                            string sqlCreateHD = @"
                        INSERT INTO HOADON (MaBN, MaPhieuKhamBenh, NgayThanhToan, TongTienGoc, TongTienBHYTChiTra, TongTienBenhNhanTra, TrangThaiThanhToan)
                        OUTPUT INSERTED.MaHD
                        VALUES (@MaBN, @MaPKB, GETDATE(), 0, 0, 0, N'Chưa thanh toán')";
                            using (SqlCommand cmdCreateHD = new SqlCommand(sqlCreateHD, conn, tran))
                            {
                                cmdCreateHD.Parameters.AddWithValue("@MaBN", maBN);
                                cmdCreateHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                                maHD = Convert.ToInt32(cmdCreateHD.ExecuteScalar());
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

                            // Tạo phiếu chỉ định cho từng phòng
                            string sqlPhieuCD = @"
                        INSERT INTO PHIEU_CHIDINH (MaPhieuKhamBenh, MaBacSiChiDinh, NgayChiDinh, TrangThai, TongTien, MaPhong)
                        OUTPUT INSERTED.MaPhieuChiDinh
                        VALUES (@MaP, @MaBS, GETDATE(), N'Chưa thanh toán', @TongTien, @MaPhong)";
                            int maPhieuCD;
                            using (SqlCommand cmdPCD = new SqlCommand(sqlPhieuCD, conn, tran))
                            {
                                cmdPCD.Parameters.AddWithValue("@MaP", model.MaPhieuKhamBenh);
                                cmdPCD.Parameters.AddWithValue("@MaBS", maBS);
                                cmdPCD.Parameters.AddWithValue("@TongTien", tongTienPhieu);
                                cmdPCD.Parameters.AddWithValue("@MaPhong", group.Key);
                                maPhieuCD = (int)cmdPCD.ExecuteScalar();
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

                                // Lưu vào chi tiết chỉ định
                                string sqlCT = @"
                            INSERT INTO CHITIET_CHIDINH (MaPhieuChiDinh, MaDV, DonGia, TrangThai)
                            VALUES (@MaPCD, @MaDV, @DonGia, N'Chưa thực hiện')";
                                using (SqlCommand cmdCT = new SqlCommand(sqlCT, conn, tran))
                                {
                                    cmdCT.Parameters.AddWithValue("@MaPCD", maPhieuCD);
                                    cmdCT.Parameters.AddWithValue("@MaDV", dv.MaDV);
                                    cmdCT.Parameters.AddWithValue("@DonGia", donGia);
                                    cmdCT.ExecuteNonQuery();
                                }

                                // Đẩy vào Hóa đơn & TỰ ĐỘNG TRỪ BHYT
                                string sqlInsHD = @"
                            INSERT INTO CT_HOADON_DV (MaHD, MaDV, DonGia, TongTienGoc, TienBHYTChiTra, TienBenhNhanTra, TrangThaiThanhToan)
                            SELECT 
                                @MaHD, @MaDV, @DonGia, @DonGia,
                                CASE WHEN bn.BHYT = 1 AND dv.CoBHYT = 1 THEN @DonGia * (ISNULL(bn.MucHuongBHYT, 0) / 100.0) ELSE 0 END AS BHYTChiTra,
                                @DonGia - (CASE WHEN bn.BHYT = 1 AND dv.CoBHYT = 1 THEN @DonGia * (ISNULL(bn.MucHuongBHYT, 0) / 100.0) ELSE 0 END) AS BenhNhanTra,
                                N'Chưa thanh toán'
                            FROM BENHNHAN bn
                            CROSS JOIN DICHVU dv
                            WHERE bn.MaBN = @MaBN AND dv.MaDV = @MaDV";

                                using (SqlCommand cmdInsHD = new SqlCommand(sqlInsHD, conn, tran))
                                {
                                    cmdInsHD.Parameters.AddWithValue("@MaHD", maHD);
                                    cmdInsHD.Parameters.AddWithValue("@MaDV", dv.MaDV);
                                    cmdInsHD.Parameters.AddWithValue("@DonGia", donGia);
                                    cmdInsHD.Parameters.AddWithValue("@MaBN", maBN);
                                    cmdInsHD.ExecuteNonQuery();
                                }
                            }
                        }

                        // 1.4 Tính tổng tiền Hóa đơn CLS và chốt Trạng thái (ĐÃ SỬA ÉP CỨNG TẠI ĐÂY)
                        string sqlRecalcHD = @"
UPDATE hd
SET TongTienGoc = ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0),
    TongTienBHYTChiTra = ISNULL(dv.TongBHYT, 0) + ISNULL(th.TongBHYT, 0),
    TongTienBenhNhanTra = ISNULL(dv.TongBN, 0) + ISNULL(th.TongBN, 0),
    TrangThaiThanhToan = N'Thanh toán 1 phần'  -- <--- ĐÃ SỬA Ở ĐÂY
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
                            string sqlChanDoan = "INSERT INTO CHITIET_CHANDOAN (MaPhieuKhamBenh, MaBenh, LoaiBenh, GiaiDoan) VALUES (@MaPhieu, @MaBenh, N'Bệnh chính', 0)";
                            using (SqlCommand cmdCD = new SqlCommand(sqlChanDoan, conn, tran))
                            {
                                cmdCD.Parameters.Add("@MaPhieu", System.Data.SqlDbType.Int);
                                cmdCD.Parameters.Add("@MaBenh", System.Data.SqlDbType.Char, 10);
                                foreach (var maBenh in model.DanhSachMaBenh)
                                {
                                    cmdCD.Parameters["@MaPhieu"].Value = model.MaPhieuKhamBenh;
                                    cmdCD.Parameters["@MaBenh"].Value = maBenh;
                                    cmdCD.ExecuteNonQuery();
                                }
                            }
                        }

                        // 2.2 Kê Đơn Thuốc & Tạo Hóa Đơn Thuốc
                        if (model.DonThuoc != null && model.DonThuoc.Count > 0)
                        {
                            int maHD = 0;
                            // Lấy hoặc tạo Hóa đơn
                            string sqlGetHD = "SELECT TOP 1 MaHD FROM HOADON WHERE MaPhieuKhamBenh = @MaPKB ORDER BY MaHD DESC";
                            using (SqlCommand cmdGetHD = new SqlCommand(sqlGetHD, conn, tran))
                            {
                                cmdGetHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                                object hdObj = cmdGetHD.ExecuteScalar();
                                maHD = hdObj != null ? Convert.ToInt32(hdObj) : 0;
                            }

                            if (maHD == 0)
                            {
                                string sqlCreateHD = @"
                            INSERT INTO HOADON (MaBN, MaPhieuKhamBenh, NgayThanhToan, TongTienGoc, TongTienBHYTChiTra, TongTienBenhNhanTra, TrangThaiThanhToan)
                            OUTPUT INSERTED.MaHD
                            VALUES (@MaBN, @MaPKB, GETDATE(), 0, 0, 0, N'Chưa thanh toán')";
                                using (SqlCommand cmdCreateHD = new SqlCommand(sqlCreateHD, conn, tran))
                                {
                                    cmdCreateHD.Parameters.AddWithValue("@MaBN", maBN);
                                    cmdCreateHD.Parameters.AddWithValue("@MaPKB", model.MaPhieuKhamBenh);
                                    maHD = Convert.ToInt32(cmdCreateHD.ExecuteScalar());
                                }
                            }

                            // Tạo ĐƠN THUỐC tổng
                            string sqlTaoDon = "INSERT INTO DON_THUOC (MaPhieuKhamBenh, NgayKe, TrangThai) OUTPUT INSERTED.MaDonThuoc VALUES (@MaPhieu, GETDATE(), N'Chưa phát')";
                            int maDonThuoc = 0;
                            using (SqlCommand cmdDT = new SqlCommand(sqlTaoDon, conn, tran))
                            {
                                cmdDT.Parameters.AddWithValue("@MaPhieu", model.MaPhieuKhamBenh);
                                maDonThuoc = (int)cmdDT.ExecuteScalar();
                            }

                            // Thêm Chi Tiết Thuốc và đẩy sang Hóa Đơn
                            string sqlChiTietThuoc = @"INSERT INTO CT_DON_THUOC 
    (MaDonThuoc, MaThuoc, SoLuongSang, SoLuongTrua, SoLuongChieu, SoLuongToi, SoNgayDung, SoLuong, DonViTinh, DonGia, GhiChu) 
    OUTPUT INSERTED.MaCTDonThuoc
    VALUES (@MaDon, @MaThuoc, @S, @T, @C, @Toi, @SoNgay, @TongSL, @DVT, @DonGia, @GhiChu)";

                            string sqlHDThuoc = @"INSERT INTO CT_HOADON_THUOC 
    (MaHD, MaCTDonThuoc, SoLuong, TongTienGoc, TienBHYTChiTra, TienBenhNhanTra, TrangThaiThanhToan)
    VALUES (@MaHD, @MaCTDonThuoc, @TongSL, @TongGoc, @TienBHYT, @TienBN, N'Chưa thanh toán')";

                            // Lấy thông tin BHYT của Bệnh Nhân (LẤY 1 LẦN TRƯỚC VÒNG LẶP ĐỂ TỐI ƯU HIỆU NĂNG)
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

                                    // TÍNH TOÁN (Lấy trực tiếp Tổng Số lượng nguyên từ giao diện để né lỗi ép kiểu thập phân)
                                    int tongSoLuong = t.SoLuong > 0 ? t.SoLuong : (int)Math.Ceiling((t.Sang + t.Trua + t.Chieu + t.Toi) * t.SoNgay);

                                    decimal tongGoc = tongSoLuong * donGia;
                                    // BHYT chỉ trừ khi Khách có BHYT VÀ Thuốc đó nằm trong danh mục hỗ trợ BHYT
                                    decimal tienBHYT = (bnCoBHYT && coBHYT) ? (tongGoc * (mucHuongBHYT / 100m)) : 0;
                                    decimal tienBN = tongGoc - tienBHYT;

                                    // 1. Thêm vào CT_DON_THUOC (Sử dụng AddWithValue để ép chuẩn định dạng dữ liệu xuống CSDL)
                                    cmdCT.Parameters.Clear();
                                    cmdCT.Parameters.AddWithValue("@MaDon", maDonThuoc);
                                    cmdCT.Parameters.AddWithValue("@MaThuoc", t.MaThuoc);
                                    cmdCT.Parameters.AddWithValue("@S", t.Sang);
                                    cmdCT.Parameters.AddWithValue("@T", t.Trua);
                                    cmdCT.Parameters.AddWithValue("@C", t.Chieu);
                                    cmdCT.Parameters.AddWithValue("@Toi", t.Toi);
                                    cmdCT.Parameters.AddWithValue("@SoNgay", t.SoNgay);
                                    cmdCT.Parameters.AddWithValue("@TongSL", tongSoLuong);
                                    cmdCT.Parameters.AddWithValue("@DVT", dvt);
                                    cmdCT.Parameters.AddWithValue("@DonGia", donGia);
                                    cmdCT.Parameters.AddWithValue("@GhiChu", t.GhiChu ?? (object)DBNull.Value);

                                    int maCTDonThuoc = (int)cmdCT.ExecuteScalar();

                                    // 2. Đẩy sang Thu Ngân (CT_HOADON_THUOC)
                                    cmdHDT.Parameters.Clear();
                                    cmdHDT.Parameters.AddWithValue("@MaHD", maHD);
                                    cmdHDT.Parameters.AddWithValue("@MaCTDonThuoc", maCTDonThuoc);
                                    cmdHDT.Parameters.AddWithValue("@TongSL", tongSoLuong);
                                    cmdHDT.Parameters.AddWithValue("@TongGoc", tongGoc);
                                    cmdHDT.Parameters.AddWithValue("@TienBHYT", tienBHYT);
                                    cmdHDT.Parameters.AddWithValue("@TienBN", tienBN);

                                    cmdHDT.ExecuteNonQuery();
                                }
                            }

                            // Cập nhật lại tổng tiền Hóa Đơn chính (ĐÃ SỬA ÉP CỨNG TẠI ĐÂY)
                            string sqlRecalcHD2 = @"
UPDATE hd
SET TongTienGoc = ISNULL(dv.TongGoc, 0) + ISNULL(th.TongGoc, 0),
    TongTienBHYTChiTra = ISNULL(dv.TongBHYT, 0) + ISNULL(th.TongBHYT, 0),
    TongTienBenhNhanTra = ISNULL(dv.TongBN, 0) + ISNULL(th.TongBN, 0),
    TrangThaiThanhToan = N'Thanh toán 1 phần'  -- <--- ĐÃ SỬA Ở ĐÂY
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
        public List<dynamic> GetKetQuaCLS(int maPKB)
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
