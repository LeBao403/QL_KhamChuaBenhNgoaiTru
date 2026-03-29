using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class ThuNganDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // 1. Lấy danh sách bệnh nhân đang nợ tiền (Khám, CLS, Thuốc)
        public DataTable GetDanhSachChoThuTien()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                // Gom 3 nhóm nợ (Khám, CLS, Thuốc)
                string sql = @"
                    SELECT DISTINCT pkb.MaPhieuKhamBenh, bn.MaBN, bn.HoTen, bn.NgaySinh, bn.GioiTinh, bn.BHYT, pkb.NgayLap, pkb.TrangThai
                    FROM PHIEUKHAMBENH pkb
                    JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN
                    WHERE (pkb.TrangThai = N'Chờ thanh toán')
                       OR EXISTS (SELECT 1 FROM PHIEU_CHIDINH pcd WHERE pcd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh AND pcd.TrangThai = N'Chưa thanh toán')
                       OR EXISTS (SELECT 1 FROM DON_THUOC dt WHERE dt.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh AND dt.TrangThai = N'Chưa phát')
                    ORDER BY pkb.MaPhieuKhamBenh ASC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. Lấy Chi tiết nợ từng phần của 1 Bệnh Nhân
        public ThanhToanViewModel GetChiTietCongNo(int maPKB)
        {
            ThanhToanViewModel vm = new ThanhToanViewModel { MaPhieuKhamBenh = maPKB };
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                
                // 1. Phí Khám
                string sqlPKB = "SELECT bn.HoTen, pkb.TrangThai FROM PHIEUKHAMBENH pkb JOIN BENHNHAN bn ON pkb.MaBN = bn.MaBN WHERE pkb.MaPhieuKhamBenh = @MaPKB";
                using (SqlCommand cmd = new SqlCommand(sqlPKB, con))
                {
                    cmd.Parameters.AddWithValue("@MaPKB", maPKB);
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            vm.TenBenhNhan = rd["HoTen"].ToString();
                            vm.TrangThai = rd["TrangThai"].ToString();
                            vm.PhiKham = 150000;
                            vm.DaThuPhiKham = (vm.TrangThai != "Chờ thanh toán");
                        }
                    }
                }

                // 2. Phí Dịch vụ CLS
                string sqlCLS = "SELECT SUM(DonGia) FROM CHITIET_CHIDINH cd JOIN PHIEU_CHIDINH pcd ON cd.MaPhieuChiDinh = pcd.MaPhieuChiDinh WHERE pcd.MaPhieuKhamBenh = @MaPKB AND pcd.TrangThai = N'Chưa thanh toán'";
                using (SqlCommand cmd = new SqlCommand(sqlCLS, con))
                {
                    cmd.Parameters.AddWithValue("@MaPKB", maPKB);
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                    {
                        vm.PhiDichVuCLS = Convert.ToDecimal(obj);
                        vm.DaThuPhiDichVuCLS = false;
                    }
                    else vm.DaThuPhiDichVuCLS = true;
                }

                // 3. Phí Thuốc
                string sqlThuoc = "SELECT SUM(DonGia * SoLuong) FROM CT_DON_THUOC ctdt JOIN DON_THUOC dt ON ctdt.MaDonThuoc = dt.MaDonThuoc WHERE dt.MaPhieuKhamBenh = @MaPKB AND dt.TrangThai = N'Chưa phát'";
                using (SqlCommand cmd = new SqlCommand(sqlThuoc, con))
                {
                    cmd.Parameters.AddWithValue("@MaPKB", maPKB);
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                    {
                        vm.PhiThuoc = Convert.ToDecimal(obj);
                        vm.DaThuPhiThuoc = false;
                    }
                    else vm.DaThuPhiThuoc = true;
                }
            }
            return vm;
        }

        // 3. XÁC NHẬN THU TIỀN TỪNG PHẦN
        public bool XacNhanThuTienTungPhan(int maPKB, bool thuPhiKham, bool thuPhiCLS, bool thuPhiThuoc, string maNV, out string message)
        {
            message = "";
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        if (thuPhiKham)
                        {
                            string updatePKB = "UPDATE PHIEUKHAMBENH SET TrangThai = N'Chờ cấp số' WHERE MaPhieuKhamBenh = @MaPKB";
                            SqlCommand cmdPKB = new SqlCommand(updatePKB, con, trans);
                            cmdPKB.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdPKB.ExecuteNonQuery();

                            // Huỷ Hoá đơn cũ (nếu Tiếp Tân đã lỡ tạo để không nợ lủng củng)
                            string updateHD = "UPDATE HOADON SET TrangThaiThanhToan = N'Đã thanh toán', NgayThanhToan = GETDATE() WHERE MaPhieuKhamBenh = @MaPKB AND TrangThaiThanhToan = N'Chưa thanh toán'";
                            SqlCommand cmdHD = new SqlCommand(updateHD, con, trans);
                            cmdHD.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdHD.ExecuteNonQuery();

                            string insertBL = "INSERT INTO BIENLAI_THUTIEN (MaPhieuKhamBenh, LoaiBienLai, TongTien, MaNV_ThuNgan) VALUES (@MaPKB, 1, 150000, @MaNV)";
                            SqlCommand cmdBL = new SqlCommand(insertBL, con, trans);
                            cmdBL.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdBL.Parameters.AddWithValue("@MaNV", maNV);
                            cmdBL.ExecuteNonQuery();
                        }

                        if (thuPhiCLS)
                        {
                            string sqlCLS = "SELECT SUM(DonGia) FROM CHITIET_CHIDINH cd JOIN PHIEU_CHIDINH pcd ON cd.MaPhieuChiDinh = pcd.MaPhieuChiDinh WHERE pcd.MaPhieuKhamBenh = @MaPKB AND pcd.TrangThai = N'Chưa thanh toán'";
                            SqlCommand cmdSum = new SqlCommand(sqlCLS, con, trans);
                            cmdSum.Parameters.AddWithValue("@MaPKB", maPKB);
                            decimal sumCLS = Convert.ToDecimal(cmdSum.ExecuteScalar() ?? 0);

                            // TRIGGER đẩy sang KETQUA_CLS trước khi update TrangThai
                            string insertKQ = @"
                                INSERT INTO KETQUA_CLS (MaPhieuKhamBenh, MaDV, TrangThai)
                                SELECT pcd.MaPhieuKhamBenh, ct.MaDV, N'Chờ thực hiện'
                                FROM CHITIET_CHIDINH ct
                                JOIN PHIEU_CHIDINH pcd ON ct.MaPhieuChiDinh = pcd.MaPhieuChiDinh
                                WHERE pcd.MaPhieuKhamBenh = @MaPKB AND pcd.TrangThai = N'Chưa thanh toán'
                            ";
                            SqlCommand cmdKQ = new SqlCommand(insertKQ, con, trans);
                            cmdKQ.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdKQ.ExecuteNonQuery();

                            string updatePCD = "UPDATE PHIEU_CHIDINH SET TrangThai = N'Đã thanh toán' WHERE MaPhieuKhamBenh = @MaPKB AND TrangThai = N'Chưa thanh toán'";
                            SqlCommand cmdPCD = new SqlCommand(updatePCD, con, trans);
                            cmdPCD.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdPCD.ExecuteNonQuery();

                            string insertBL = "INSERT INTO BIENLAI_THUTIEN (MaPhieuKhamBenh, LoaiBienLai, TongTien, MaNV_ThuNgan) VALUES (@MaPKB, 2, @Sum, @MaNV)";
                            SqlCommand cmdBL = new SqlCommand(insertBL, con, trans);
                            cmdBL.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdBL.Parameters.AddWithValue("@Sum", sumCLS);
                            cmdBL.Parameters.AddWithValue("@MaNV", maNV);
                            cmdBL.ExecuteNonQuery();
                        }

                        if (thuPhiThuoc)
                        {
                            string sqlThuoc = "SELECT SUM(DonGia * SoLuong) FROM CT_DON_THUOC ctdt JOIN DON_THUOC dt ON ctdt.MaDonThuoc = dt.MaDonThuoc WHERE dt.MaPhieuKhamBenh = @MaPKB AND dt.TrangThai = N'Chưa phát'";
                            SqlCommand cmdSumThuoc = new SqlCommand(sqlThuoc, con, trans);
                            cmdSumThuoc.Parameters.AddWithValue("@MaPKB", maPKB);
                            decimal sumThuoc = Convert.ToDecimal(cmdSumThuoc.ExecuteScalar() ?? 0);

                            string updateDT = "UPDATE DON_THUOC SET TrangThai = N'Đã thanh toán' WHERE MaPhieuKhamBenh = @MaPKB AND TrangThai = N'Chưa phát'";
                            SqlCommand cmdDT = new SqlCommand(updateDT, con, trans);
                            cmdDT.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdDT.ExecuteNonQuery();

                            string insertBL = "INSERT INTO BIENLAI_THUTIEN (MaPhieuKhamBenh, LoaiBienLai, TongTien, MaNV_ThuNgan) VALUES (@MaPKB, 3, @Sum, @MaNV)";
                            SqlCommand cmdBL = new SqlCommand(insertBL, con, trans);
                            cmdBL.Parameters.AddWithValue("@MaPKB", maPKB);
                            cmdBL.Parameters.AddWithValue("@Sum", sumThuoc);
                            cmdBL.Parameters.AddWithValue("@MaNV", maNV);
                            cmdBL.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        message = ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}