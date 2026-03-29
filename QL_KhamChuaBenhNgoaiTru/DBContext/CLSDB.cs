using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class CLSDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. Lấy danh sách hàng đợi
        public List<KetQuaCLS> GetDanhSachChoThucHien()
        {
            List<KetQuaCLS> list = new List<KetQuaCLS>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT kq.MaKetQua, kq.MaPhieuKhamBenh, kq.MaDV, kq.TrangThai, kq.NgayThucHien,
                           bn.HoTen as TenBenhNhan, dv.TenDV as TenDichVu
                    FROM KETQUA_CLS kq
                    JOIN PHIEUKHAMBENH pkb ON kq.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN
                    JOIN DICHVU dv ON kq.MaDV = dv.MaDV
                    WHERE kq.TrangThai = N'Chờ thực hiện'
                    ORDER BY kq.MaKetQua ASC";
                
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            KetQuaCLS kq = new KetQuaCLS();
                            kq.MaKetQua = (int)rd["MaKetQua"];
                            kq.MaPhieuKhamBenh = (int)rd["MaPhieuKhamBenh"];
                            kq.MaDV = rd["MaDV"].ToString();
                            kq.TrangThai = rd["TrangThai"].ToString();
                            kq.NgayThucHien = rd["NgayThucHien"] != DBNull.Value ? (DateTime?)rd["NgayThucHien"] : null;
                            kq.TenBenhNhan = rd["TenBenhNhan"].ToString();
                            kq.TenDichVu = rd["TenDichVu"].ToString();
                            list.Add(kq);
                        }
                    }
                }
            }
            return list;
        }

        // 2. Lấy thông tin 1 kết quả theo ID
        public KetQuaCLS GetKetQuaById(int maKetQua)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT kq.*, dv.TenDV 
                    FROM KETQUA_CLS kq 
                    JOIN DICHVU dv ON kq.MaDV = dv.MaDV
                    WHERE kq.MaKetQua = @MaKetQua";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaKetQua", maKetQua);
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            KetQuaCLS kq = new KetQuaCLS();
                            kq.MaKetQua = (int)rd["MaKetQua"];
                            kq.MaPhieuKhamBenh = (int)rd["MaPhieuKhamBenh"];
                            kq.MaDV = rd["MaDV"].ToString();
                            kq.TrangThai = rd["TrangThai"].ToString();
                            kq.TenDichVu = rd["TenDV"].ToString(); // Lấy tên DV để LIS tự sinh KQ
                            return kq;
                        }
                    }
                }
            }
            return null;
        }

        public dynamic GetThongTinChiTietCLS(int maKetQua)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                string sql = @"
                    SELECT kq.MaKetQua, kq.MaPhieuKhamBenh, kq.MaDV, kq.TrangThai, dv.TenDV,
                           bn.HoTen, bn.GioiTinh, bn.NgaySinh, pkb.LyDoDenKham
                    FROM KETQUA_CLS kq
                    JOIN PHIEUKHAMBENH pkb ON kq.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN
                    JOIN DICHVU dv ON kq.MaDV = dv.MaDV
                    WHERE kq.MaKetQua = @MaKetQua";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaKetQua", maKetQua);
                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            int tuoi = rd["NgaySinh"] != DBNull.Value ? DateTime.Now.Year - Convert.ToDateTime(rd["NgaySinh"]).Year : 0;
                            return new {
                                MaKetQua = rd["MaKetQua"],
                                MaPhieuKhamBenh = rd["MaPhieuKhamBenh"],
                                TenDV = rd["TenDV"].ToString(),
                                TenBenhNhan = rd["HoTen"].ToString(),
                                GioiTinh = rd["GioiTinh"].ToString(),
                                Tuoi = tuoi,
                                TrangThai = rd["TrangThai"].ToString(),
                                LyDoDenKham = rd["LyDoDenKham"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 3. Cập nhật kết quả LIS tự động và Đẩy về tay Bác sĩ
        public bool CapNhatKetQuaTuLIS(int maKetQua, string noiDungKetQua, int maPhieuKhamBenh)
        {
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Cập nhật kết quả hiện tại cho dịch vụ này
                        string sql = "UPDATE KETQUA_CLS SET NoiDungKetQua = @NoiDung, TrangThai = N'Đã có kết quả', NgayThucHien = GETDATE() WHERE MaKetQua = @MaKetQua";
                        using (SqlCommand cmd = new SqlCommand(sql, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@NoiDung", noiDungKetQua);
                            cmd.Parameters.AddWithValue("@MaKetQua", maKetQua);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Kiểm tra xem bệnh nhân này còn dịch vụ nào 'Chờ thực hiện' không
                        string sqlCheck = "SELECT COUNT(*) FROM KETQUA_CLS WHERE MaPhieuKhamBenh = @MaPKB AND TrangThai = N'Chờ thực hiện'";
                        using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, con, trans))
                        {
                            cmdCheck.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                            int count = (int)cmdCheck.ExecuteScalar();

                            // 3. Nếu đã làm xong hết toàn bộ dự án CLS của Hóa đơn đó, báo hiệu để Bác sĩ gọi vào khám tiếp
                            if (count == 0)
                            {
                                string sqlUpdatePKB = "UPDATE PHIEUKHAMBENH SET TrangThai = N'Đã có kết quả CLS' WHERE MaPhieuKhamBenh = @MaPKB";
                                using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdatePKB, con, trans))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@MaPKB", maPhieuKhamBenh);
                                    cmdUpdate.ExecuteNonQuery();
                                }
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
    }
}
