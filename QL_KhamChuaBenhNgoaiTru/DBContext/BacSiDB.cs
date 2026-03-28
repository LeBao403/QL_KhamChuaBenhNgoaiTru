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
            WHERE PKB.TrangThai = @TrangThai ";

                // LOGIC MỚI:
                // - Nếu là danh sách "Chờ khám": Lấy tất cả bệnh nhân được phân vào PHÒNG của bác sĩ này
                // - Nếu là danh sách "Đang khám": Lấy đích danh các ca mà BÁC SĨ NÀY đã bấm tiếp nhận
                if (trangThai == "Chờ khám" || trangThai == "Chờ Khám")
                {
                    sql += " AND PKB.MaPhong = @MaPhong ";
                }
                else
                {
                    sql += " AND PKB.MaBacSiKham = @MaBS ";
                }

                sql += " ORDER BY PKB.STT ASC";

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
            SELECT PKB.MaPhieuKhamBenh, PKB.LyDoDenKham, BN.HoTen, BN.GioiTinh, BN.NgaySinh
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
                                LyDoDenKham = dr["LyDoDenKham"].ToString()
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
                string sql = "SELECT MaThuoc, TenThuoc, DonViCoBan FROM THUOC WHERE TrangThai = 1";
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
                                DonViCoBan = dr["DonViCoBan"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 5. Lưu Kết quả khám & Đơn thuốc (Dùng Transaction để đảm bảo tính toàn vẹn)
        public bool LuuKhamBenh(KhamBenhViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 5.1 Cập nhật trạng thái và kết luận phiếu khám
                    string trangThai = model.YeuCauCanLamSang ? "Chờ cận lâm sàng" : "Hoàn thành";
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

                    // Nếu không phải đi Cận lâm sàng thì tiến hành lưu bệnh và kê thuốc
                    if (!model.YeuCauCanLamSang)
                    {
                        // 5.2 Lưu danh sách Bệnh (Chẩn đoán)
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

                        // 5.3 Kê Đơn Thuốc
                        if (model.DonThuoc != null && model.DonThuoc.Count > 0)
                        {
                            string sqlTaoDon = "INSERT INTO DON_THUOC (MaPhieuKhamBenh, NgayKe, TrangThai) OUTPUT INSERTED.MaDonThuoc VALUES (@MaPhieu, GETDATE(), N'Chưa phát')";
                            int maDonThuoc = 0;
                            using (SqlCommand cmdDT = new SqlCommand(sqlTaoDon, conn, tran))
                            {
                                cmdDT.Parameters.AddWithValue("@MaPhieu", model.MaPhieuKhamBenh);
                                maDonThuoc = (int)cmdDT.ExecuteScalar();
                            }

                            string sqlChiTietThuoc = @"INSERT INTO CT_DON_THUOC 
                                (MaDonThuoc, MaThuoc, SoLuongSang, SoLuongTrua, SoLuongChieu, SoLuongToi, SoNgayDung, SoLuong, DonViTinh, DonGia, GhiChu) 
                                VALUES (@MaDon, @MaThuoc, @S, @T, @C, @Toi, @SoNgay, @TongSL, @DVT, @DonGia, @GhiChu)";

                            using (SqlCommand cmdCT = new SqlCommand(sqlChiTietThuoc, conn, tran))
                            {
                                cmdCT.Parameters.Add("@MaDon", System.Data.SqlDbType.Int);
                                cmdCT.Parameters.Add("@MaThuoc", System.Data.SqlDbType.Char, 10);
                                cmdCT.Parameters.Add("@S", System.Data.SqlDbType.Decimal);
                                cmdCT.Parameters.Add("@T", System.Data.SqlDbType.Decimal);
                                cmdCT.Parameters.Add("@C", System.Data.SqlDbType.Decimal);
                                cmdCT.Parameters.Add("@Toi", System.Data.SqlDbType.Decimal);
                                cmdCT.Parameters.Add("@SoNgay", System.Data.SqlDbType.Int);
                                cmdCT.Parameters.Add("@TongSL", System.Data.SqlDbType.Int);
                                cmdCT.Parameters.Add("@DVT", System.Data.SqlDbType.NVarChar, 20);
                                cmdCT.Parameters.Add("@DonGia", System.Data.SqlDbType.Decimal);
                                cmdCT.Parameters.Add("@GhiChu", System.Data.SqlDbType.NVarChar, 200);

                                foreach (var t in model.DonThuoc)
                                {
                                    // Lấy Đơn vị và Giá từ DB
                                    string dvt = "Viên";
                                    decimal donGia = 0;
                                    string sqlInfo = "SELECT DonViCoBan, GiaBan FROM THUOC WHERE MaThuoc = @MaTh";
                                    using (SqlCommand cmdInfo = new SqlCommand(sqlInfo, conn, tran))
                                    {
                                        cmdInfo.Parameters.AddWithValue("@MaTh", t.MaThuoc);
                                        using (SqlDataReader drInfo = cmdInfo.ExecuteReader())
                                        {
                                            if (drInfo.Read())
                                            {
                                                dvt = drInfo["DonViCoBan"].ToString();
                                                donGia = drInfo["GiaBan"] != DBNull.Value ? Convert.ToDecimal(drInfo["GiaBan"]) : 0;
                                            }
                                        }
                                    }

                                    // Tính tổng số lượng
                                    decimal tongSLLeo = (t.Sang + t.Trua + t.Chieu + t.Toi) * t.SoNgay;
                                    int tongSoLuong = (int)Math.Ceiling(tongSLLeo);

                                    cmdCT.Parameters["@MaDon"].Value = maDonThuoc;
                                    cmdCT.Parameters["@MaThuoc"].Value = t.MaThuoc;
                                    cmdCT.Parameters["@S"].Value = t.Sang;
                                    cmdCT.Parameters["@T"].Value = t.Trua;
                                    cmdCT.Parameters["@C"].Value = t.Chieu;
                                    cmdCT.Parameters["@Toi"].Value = t.Toi;
                                    cmdCT.Parameters["@SoNgay"].Value = t.SoNgay;
                                    cmdCT.Parameters["@TongSL"].Value = tongSoLuong;
                                    cmdCT.Parameters["@DVT"].Value = dvt;
                                    cmdCT.Parameters["@DonGia"].Value = donGia;
                                    cmdCT.Parameters["@GhiChu"].Value = t.GhiChu ?? (object)DBNull.Value;
                                    cmdCT.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    tran.Commit();
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return false;
                }
            }
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
                string sql = "SELECT MaDV, TenDV, GiaDichVu FROM DICHVU WHERE TrangThai = 1";
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

        // 4. BẠN CẦN CẬP NHẬT LẠI HÀM LuuKhamBenh để thêm tham số maBS và xử lý lưu CLS
        public bool LuuKhamBenh(KhamBenhViewModel model, string maBS)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Cập nhật trạng thái phiếu khám chính
                    string trangThai = model.YeuCauCanLamSang ? "Chờ cận lâm sàng" : "Hoàn thành";
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

                    // ======= NẾU CHỈ ĐỊNH CẬN LÂM SÀNG =======
                    if (model.YeuCauCanLamSang && model.ChiDinhs != null && model.ChiDinhs.Count > 0)
                    {
                        // Gom nhóm các dịch vụ theo Phòng để tạo Phiếu Chỉ Định
                        var groupByPhong = model.ChiDinhs.GroupBy(x => x.MaPhong);
                        foreach (var group in groupByPhong)
                        {
                            string sqlPhieuCD = @"INSERT INTO PHIEU_CHIDINH (MaPhieuKhamBenh, MaBacSiChiDinh, NgayChiDinh, TrangThai, MaPhong) 
                                          OUTPUT INSERTED.MaPhieuChiDinh 
                                          VALUES (@MaP, @MaBS, GETDATE(), N'Chưa thanh toán', @MaPhong)";

                            int maPhieuCD = 0;
                            using (SqlCommand cmdPCD = new SqlCommand(sqlPhieuCD, conn, tran))
                            {
                                cmdPCD.Parameters.AddWithValue("@MaP", model.MaPhieuKhamBenh);
                                cmdPCD.Parameters.AddWithValue("@MaBS", maBS);
                                cmdPCD.Parameters.AddWithValue("@MaPhong", group.Key);
                                maPhieuCD = (int)cmdPCD.ExecuteScalar();
                            }

                            // Lưu chi tiết từng dịch vụ vào Phiếu
                            foreach (var dv in group)
                            {
                                string sqlCT = @"INSERT INTO CHITIET_CHIDINH (MaPhieuChiDinh, MaDV, DonGia, TrangThai) 
                                         VALUES (@MaPCD, @MaDV, (SELECT GiaDichVu FROM DICHVU WHERE MaDV = @MaDV), N'Chưa thực hiện')";
                                using (SqlCommand cmdCT = new SqlCommand(sqlCT, conn, tran))
                                {
                                    cmdCT.Parameters.AddWithValue("@MaPCD", maPhieuCD);
                                    cmdCT.Parameters.AddWithValue("@MaDV", dv.MaDV);
                                    cmdCT.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    // ======= NẾU KHÔNG CÓ CLS THÌ LƯU BỆNH & KÊ THUỐC (Code cũ giữ nguyên) =======
                    else
                    {
                        // ... (Giữ nguyên đoạn code Insert CHITIET_CHANDOAN và DON_THUOC của bạn trước đó ở đây) ...
                    }

                    tran.Commit();
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
    }
}