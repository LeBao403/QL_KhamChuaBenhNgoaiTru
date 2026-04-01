using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class CLSDB
    {
        private readonly string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public List<KetQuaCLS> GetDanhSachChoThucHien()
        {
            var list = new List<KetQuaCLS>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT
                        ct.MaCTChiDinh AS MaKetQua,
                        ct.MaPhieuChiDinh,
                        pc.MaPhieuKhamBenh,
                        ct.MaDV,
                        ct.TrangThai,
                        bn.HoTen AS TenBenhNhan,
                        dv.TenDV AS TenDichVu,
                        pc.NgayChiDinh
                    FROM CHITIET_CHIDINH ct
                    JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                    JOIN PHIEUKHAMBENH pkb ON pc.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    WHERE ct.ThoiGianCoKetQua IS NULL
                      AND (
                          pkb.TrangThai = N'Chờ cận lâm sàng'
                          OR EXISTS (
                              SELECT 1
                              FROM HOADON hd
                              JOIN CT_HOADON_DV dvPay ON dvPay.MaHD = hd.MaHD
                              WHERE hd.MaPhieuKhamBenh = pc.MaPhieuKhamBenh
                                AND dvPay.MaDV = ct.MaDV
                                AND dvPay.TrangThaiThanhToan = N'Đã thanh toán'
                          )
                      )
                    ORDER BY pc.NgayChiDinh ASC, ct.MaCTChiDinh ASC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            list.Add(new KetQuaCLS
                            {
                                MaKetQua = Convert.ToInt32(rd["MaKetQua"]),
                                MaPhieuChiDinh = Convert.ToInt32(rd["MaPhieuChiDinh"]),
                                MaPhieuKhamBenh = Convert.ToInt32(rd["MaPhieuKhamBenh"]),
                                MaDV = rd["MaDV"].ToString(),
                                TrangThai = rd["TrangThai"].ToString(),
                                NgayThucHien = rd["NgayChiDinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rd["NgayChiDinh"]) : null,
                                TenBenhNhan = rd["TenBenhNhan"].ToString(),
                                TenDichVu = rd["TenDichVu"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public dynamic GetThongTinChiTietCLS(int maKetQua)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT
                        ct.MaCTChiDinh AS MaKetQua,
                        ct.MaPhieuChiDinh,
                        pc.MaPhieuKhamBenh,
                        ct.MaDV,
                        ct.TrangThai,
                        ct.KetQua,
                        ct.FileKetQua,
                        dv.TenDV,
                        bn.HoTen,
                        bn.GioiTinh,
                        bn.NgaySinh,
                        pkb.LyDoDenKham,
                        ct.MaBacSiThucHien,
                        ct.ThoiGianCoKetQua,
                        pc.NgayChiDinh
                    FROM CHITIET_CHIDINH ct
                    JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                    JOIN PHIEUKHAMBENH pkb ON pc.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    WHERE ct.MaCTChiDinh = @MaKetQua";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaKetQua", maKetQua);
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            int tuoi = rd["NgaySinh"] != DBNull.Value ? DateTime.Now.Year - Convert.ToDateTime(rd["NgaySinh"]).Year : 0;
                            string thoiGian = "";
                            if (rd["NgayChiDinh"] != DBNull.Value)
                            {
                                DateTime ngay = Convert.ToDateTime(rd["NgayChiDinh"]);
                                string gio = rd["ThoiGianCoKetQua"] != DBNull.Value ? rd["ThoiGianCoKetQua"].ToString() : "";
                                thoiGian = string.IsNullOrWhiteSpace(gio) ? ngay.ToString("dd/MM/yyyy") : $"{ngay:dd/MM/yyyy} {gio}";
                            }

                            return new
                            {
                                MaKetQua = rd["MaKetQua"],
                                MaPhieuChiDinh = rd["MaPhieuChiDinh"],
                                MaPhieuKhamBenh = rd["MaPhieuKhamBenh"],
                                TenDV = rd["TenDV"].ToString(),
                                TenBenhNhan = rd["HoTen"].ToString(),
                                GioiTinh = rd["GioiTinh"] != DBNull.Value ? rd["GioiTinh"].ToString() : "",
                                Tuoi = tuoi,
                                TrangThai = rd["TrangThai"].ToString(),
                                LyDoDenKham = rd["LyDoDenKham"] != DBNull.Value ? rd["LyDoDenKham"].ToString() : "",
                                NoiDungKetQua = rd["KetQua"] != DBNull.Value ? rd["KetQua"].ToString() : "",
                                FileKetQua = rd["FileKetQua"] != DBNull.Value ? rd["FileKetQua"].ToString() : "",
                                MaBacSiThucHien = rd["MaBacSiThucHien"] != DBNull.Value ? rd["MaBacSiThucHien"].ToString() : "",
                                ThoiGianCoKetQua = thoiGian
                            };
                        }
                    }
                }
            }

            return null;
        }

        public KetQuaCLS GetKetQuaById(int maKetQua)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT
                        ct.MaCTChiDinh AS MaKetQua,
                        ct.MaPhieuChiDinh,
                        pc.MaPhieuKhamBenh,
                        ct.MaDV,
                        ct.TrangThai,
                        ct.KetQua,
                        ct.FileKetQua,
                        ct.MaBacSiThucHien,
                        ct.MauXetNghiem,
                        ct.ChatLuongMau,
                        dv.TenDV
                    FROM CHITIET_CHIDINH ct
                    JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    WHERE ct.MaCTChiDinh = @MaKetQua";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaKetQua", maKetQua);
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            return new KetQuaCLS
                            {
                                MaKetQua = Convert.ToInt32(rd["MaKetQua"]),
                                MaPhieuChiDinh = Convert.ToInt32(rd["MaPhieuChiDinh"]),
                                MaPhieuKhamBenh = Convert.ToInt32(rd["MaPhieuKhamBenh"]),
                                MaDV = rd["MaDV"].ToString(),
                                TrangThai = rd["TrangThai"].ToString(),
                                NoiDungKetQua = rd["KetQua"] != DBNull.Value ? rd["KetQua"].ToString() : "",
                                FileKetQua = rd["FileKetQua"] != DBNull.Value ? rd["FileKetQua"].ToString() : "",
                                MaBacSiThucHien = rd["MaBacSiThucHien"] != DBNull.Value ? rd["MaBacSiThucHien"].ToString() : "",
                                MauXetNghiem = rd["MauXetNghiem"] != DBNull.Value ? rd["MauXetNghiem"].ToString() : "",
                                ChatLuongMau = rd["ChatLuongMau"] != DBNull.Value ? rd["ChatLuongMau"].ToString() : "",
                                TenDichVu = rd["TenDV"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool CapNhatKetQuaTuLIS(int maKetQua, string noiDungKetQua, int maPhieuKhamBenh)
        {
            return CapNhatKetQuaTuLIS(maKetQua, noiDungKetQua, maPhieuKhamBenh, null, null, null, null);
        }

        public bool CapNhatKetQuaTuLIS(int maKetQua, string noiDungKetQua, int maPhieuKhamBenh, string maBacSiThucHien, string fileKetQua, string mauXN, string chatLuong)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        string sqlUpdateCt = @"
                            UPDATE CHITIET_CHIDINH
                            SET KetQua = @NoiDung,
                                TrangThai = N'Đã có kết quả',
                                ThoiGianCoKetQua = CONVERT(TIME, GETDATE()),
                                MaBacSiThucHien = @MaBacSi,
                                FileKetQua = @FileKetQua,
                                MauXetNghiem = @MauXN,
                                ChatLuongMau = @ChatLuong
                            WHERE MaCTChiDinh = @MaKetQua";

                        using (SqlCommand cmd = new SqlCommand(sqlUpdateCt, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@NoiDung", noiDungKetQua);
                            cmd.Parameters.AddWithValue("@MaKetQua", maKetQua);
                            cmd.Parameters.AddWithValue("@MaBacSi", string.IsNullOrWhiteSpace(maBacSiThucHien) ? (object)DBNull.Value : maBacSiThucHien);
                            cmd.Parameters.AddWithValue("@FileKetQua", string.IsNullOrWhiteSpace(fileKetQua) ? (object)DBNull.Value : fileKetQua);
                            cmd.Parameters.AddWithValue("@MauXN", string.IsNullOrWhiteSpace(mauXN) ? (object)DBNull.Value : mauXN);
                            cmd.Parameters.AddWithValue("@ChatLuong", string.IsNullOrWhiteSpace(chatLuong) ? (object)DBNull.Value : chatLuong);
                            int affected = cmd.ExecuteNonQuery();
                            if (affected == 0) throw new Exception("Khong tim thay chi dinh CLS can cap nhat.");
                        }

                        int maPhieuChiDinh;
                        using (SqlCommand cmdGet = new SqlCommand("SELECT MaPhieuChiDinh FROM CHITIET_CHIDINH WHERE MaCTChiDinh = @MaKetQua", con, trans))
                        {
                            cmdGet.Parameters.AddWithValue("@MaKetQua", maKetQua);
                            maPhieuChiDinh = Convert.ToInt32(cmdGet.ExecuteScalar());
                        }

                        int pendingInPhieu;
                        using (SqlCommand cmdCheck = new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM CHITIET_CHIDINH
                            WHERE MaPhieuChiDinh = @MaPhieuChiDinh
                              AND TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện')", con, trans))
                        {
                            cmdCheck.Parameters.AddWithValue("@MaPhieuChiDinh", maPhieuChiDinh);
                            pendingInPhieu = (int)cmdCheck.ExecuteScalar();
                        }

                        using (SqlCommand cmdUpdatePhieu = new SqlCommand("UPDATE PHIEU_CHIDINH SET TrangThai = @TrangThai WHERE MaPhieuChiDinh = @MaPhieuChiDinh", con, trans))
                        {
                            cmdUpdatePhieu.Parameters.AddWithValue("@TrangThai", pendingInPhieu == 0 ? "Hoàn tất" : "Đang thực hiện");
                            cmdUpdatePhieu.Parameters.AddWithValue("@MaPhieuChiDinh", maPhieuChiDinh);
                            cmdUpdatePhieu.ExecuteNonQuery();
                        }

                        int pendingAll;
                        using (SqlCommand cmdCheckAll = new SqlCommand(@"
                            SELECT COUNT(*)
                            FROM CHITIET_CHIDINH ct
                            JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                            WHERE pc.MaPhieuKhamBenh = @MaPKB
                              AND ct.TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện')", con, trans))
                        {
                            cmdCheckAll.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            pendingAll = (int)cmdCheckAll.ExecuteScalar();
                        }

                        if (pendingAll == 0)
                        {
                            using (SqlCommand cmdUpdatePKB = new SqlCommand("UPDATE PHIEUKHAMBENH SET TrangThai = N'Đã có kết quả CLS' WHERE MaPhieuKhamBenh = @MaPKB", con, trans))
                            {
                                cmdUpdatePKB.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                                cmdUpdatePKB.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        public List<KetQuaCLS> GetLichSuXetNghiem(int top = 200)
        {
            var list = new List<KetQuaCLS>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT TOP (@Top)
                        ct.MaCTChiDinh AS MaKetQua,
                        ct.MaPhieuChiDinh,
                        pc.MaPhieuKhamBenh,
                        ct.MaDV,
                        ct.TrangThai,
                        ct.KetQua,
                        ct.FileKetQua,
                        ISNULL(nv.HoTen, ct.MaBacSiThucHien) AS MaBacSiThucHien,
                        CASE
                            WHEN ct.ThoiGianCoKetQua IS NULL THEN pc.NgayChiDinh
                            ELSE DATEADD(SECOND, DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ct.ThoiGianCoKetQua), CAST(pc.NgayChiDinh AS DATETIME))
                        END AS NgayThucHien,
                        bn.HoTen AS TenBenhNhan,
                        dv.TenDV AS TenDichVu
                    FROM CHITIET_CHIDINH ct
                    JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
                    JOIN PHIEUKHAMBENH pkb ON pc.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    LEFT JOIN NHANVIEN nv ON ct.MaBacSiThucHien = nv.MaNV
                    WHERE ct.TrangThai = N'Đã có kết quả'
                    ORDER BY ISNULL(ct.ThoiGianCoKetQua, CONVERT(TIME, '00:00:00')) DESC, pc.NgayChiDinh DESC, ct.MaCTChiDinh DESC";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Top", top);
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            list.Add(new KetQuaCLS
                            {
                                MaKetQua = Convert.ToInt32(rd["MaKetQua"]),
                                MaPhieuChiDinh = Convert.ToInt32(rd["MaPhieuChiDinh"]),
                                MaPhieuKhamBenh = Convert.ToInt32(rd["MaPhieuKhamBenh"]),
                                MaDV = rd["MaDV"].ToString(),
                                TrangThai = rd["TrangThai"].ToString(),
                                NoiDungKetQua = rd["KetQua"] != DBNull.Value ? rd["KetQua"].ToString() : "",
                                FileKetQua = rd["FileKetQua"] != DBNull.Value ? rd["FileKetQua"].ToString() : "",
                                MaBacSiThucHien = rd["MaBacSiThucHien"] != DBNull.Value ? rd["MaBacSiThucHien"].ToString() : "",
                                NgayThucHien = rd["NgayThucHien"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rd["NgayThucHien"]) : null,
                                TenBenhNhan = rd["TenBenhNhan"].ToString(),
                                TenDichVu = rd["TenDichVu"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
