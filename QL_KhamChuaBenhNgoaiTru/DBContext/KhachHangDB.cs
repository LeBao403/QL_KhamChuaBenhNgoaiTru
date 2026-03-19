using QL_KhamChuaBenhNgoaiTru.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class KhachHangDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // --- Lấy toàn bộ khách hàng theo trang ---
        public List<KhachHangManageViewModel> GetAll(int page = 1, int pageSize = 10)
        {
            List<KhachHangManageViewModel> list = new List<KhachHangManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // SỬA CÂU QUERY: Thêm "tk.PasswordHash"
                string query = @"
                    SELECT 
                        kh.*, 
                        tk.Username, tk.PasswordHash
                    FROM KHACHHANG kh
                    LEFT JOIN TAIKHOAN tk ON kh.MaTK = tk.MaTK
                    ORDER BY kh.MaKH
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    KhachHang kh = new KhachHang
                    {
                        MaKH = dr["MaKH"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = dr["GioiTinh"].ToString(),
                        SDT = dr["SDT"].ToString(),
                        Email = dr["Email"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        MaNGH = dr["MaNGH"] != DBNull.Value ? dr["MaNGH"].ToString() : null,
                        MaTK = dr["MaTK"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaTK"]) : null,
                        CCCD = dr["CCCD"] != DBNull.Value ? dr["CCCD"].ToString() : null
                    };

                    TaiKhoan tk = null;
                    if (dr["Username"] != DBNull.Value)
                    {
                        tk = new TaiKhoan
                        {
                            MaTK = kh.MaTK.Value,
                            Username = dr["Username"].ToString(),
                            // THÊM DÒNG NÀY:
                            PasswordHash = dr["PasswordHash"] != DBNull.Value ? dr["PasswordHash"].ToString() : null
                        };
                    }

                    list.Add(new KhachHangManageViewModel
                    {
                        KhachHang = kh,
                        TaiKhoan = tk
                    });
                }
            }
            return list;
        }

        // --- Lấy tổng số khách hàng để phân trang ---
        public int GetCount()
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM KHACHHANG", conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }


        // --- Lấy thông tin 1 khách hàng theo mã ---
        public KhachHang GetById(string maKH)
        {
            KhachHang kh = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM KHACHHANG WHERE MaKH = @MaKH";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKH", maKH.Trim());

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    kh = new KhachHang
                    {
                        MaKH = dr["MaKH"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = dr["GioiTinh"].ToString(),
                        SDT = dr["SDT"].ToString(),
                        Email = dr["Email"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        MaNGH = dr["MaNGH"] != DBNull.Value ? dr["MaNGH"].ToString() : null,
                        CCCD = dr["CCCD"] != DBNull.Value ? dr["CCCD"].ToString() : null,
                        MaTK = dr["MaTK"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaTK"]) : null
                    };
                }
            }
            return kh;
        }

        // --- Lấy người giám hộ theo mã ---
        public NguoiGiamHo GetNguoiGiamHoById(string maNGH)
        {
            if (string.IsNullOrEmpty(maNGH)) return null;

            NguoiGiamHo ngh = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM NGUOIGIAMHO WHERE MaNGH = @MaNGH";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaNGH", maNGH);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    ngh = new NguoiGiamHo
                    {
                        MaNGH = dr["MaNGH"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = dr["NgaySinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["NgaySinh"]) : null,
                        GioiTinh = dr["GioiTinh"].ToString(),
                        SDT = dr["SDT"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        MoiQuanHe = dr["MoiQuanHe"].ToString()
                    };
                }
            }
            return ngh;
        }

        

        // --- Xóa khách hàng ---
        public bool Delete(string maKH)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();

                int? maTK = null;
                string queryCheck = "SELECT MaTK FROM KHACHHANG WHERE MaKH = @MaKH";
                SqlCommand cmdCheck = new SqlCommand(queryCheck, conn);
                cmdCheck.Parameters.AddWithValue("@MaKH", maKH);

                // Dùng ExecuteScalar để lấy 1 giá trị duy nhất
                object result = cmdCheck.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    maTK = (int?)result;
                }

                // Bước 2: Xoá khách hàng (giống code cũ của bạn)
                string queryKh = "DELETE FROM KHACHHANG WHERE MaKH = @MaKH";
                SqlCommand cmdKh = new SqlCommand(queryKh, conn);
                cmdKh.Parameters.AddWithValue("@MaKH", maKH);
                int rows = cmdKh.ExecuteNonQuery();

                if (rows == 0)
                {
                    // Không tìm thấy khách hàng nào để xoá
                    return false;
                }

                // Bước 3: Nếu xoá KH thành công (rows > 0) VÀ khách hàng này có MaTK
                if (maTK.HasValue)
                {
                    try
                    {
                        // Thì tiến hành xoá TAIKHOAN
                        string queryTk = "DELETE FROM TAIKHOAN WHERE MaTK = @MaTK";
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn);
                        cmdTk.Parameters.AddWithValue("@MaTK", maTK.Value);
                        cmdTk.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Ghi log lỗi ở đây (ví dụ: Log.Error(ex.Message))
                        // CẢNH BÁO: Khách hàng đã bị xoá, nhưng tài khoản xoá lỗi và VẪN CÒN.
                        // Đây chính là rủi ro khi không dùng Transaction.
                    }
                }

                return true; // Xoá KHACHHANG thành công
            }
        }

        public bool Create(KhachHang kh, NguoiGiamHo ngh = null, TaiKhoan tk = null) // Thêm tham số TaiKhoan
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. THÊM TÀI KHOẢN (NẾU CÓ)
                    if (tk != null)
                    {
                        string queryTk = @"
                            INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) 
                            OUTPUT INSERTED.MaTK 
                            VALUES (@Username, @Password, 1)"; // Lưu plain text vào PasswordHash
                        SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                        cmdTk.Parameters.AddWithValue("@Username", tk.Username);
                        cmdTk.Parameters.AddWithValue("@Password", tk.PasswordHash); // PasswordHash giờ chứa plain text

                        // Lấy MaTK vừa insert
                        object insertedMaTK = cmdTk.ExecuteScalar();
                        if (insertedMaTK != null && insertedMaTK != DBNull.Value)
                        {
                            kh.MaTK = Convert.ToInt32(insertedMaTK); // Gán MaTK cho khách hàng
                        }
                        else
                        {
                            throw new Exception("Không thể tạo tài khoản."); // Nếu không lấy được MaTK -> lỗi
                        }
                    }

                    // 2. THÊM NGƯỜI GIÁM HỘ (NẾU CÓ)
                    if (ngh != null)
                    {
                        string queryNgh = @"
                            INSERT INTO NGUOIGIAMHO (MaNGH, HoTen, NgaySinh, GioiTinh, SDT, DiaChi, MoiQuanHe)
                            VALUES (@MaNGH, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @MoiQuanHe)";
                        SqlCommand cmdNgh = new SqlCommand(queryNgh, conn, tran);
                        cmdNgh.Parameters.AddWithValue("@MaNGH", ngh.MaNGH);
                        cmdNgh.Parameters.AddWithValue("@HoTen", ngh.HoTen);
                        cmdNgh.Parameters.AddWithValue("@NgaySinh", (object)ngh.NgaySinh ?? DBNull.Value);
                        cmdNgh.Parameters.AddWithValue("@GioiTinh", (object)ngh.GioiTinh ?? DBNull.Value); // Chấp nhận NULL
                        cmdNgh.Parameters.AddWithValue("@SDT", (object)ngh.SDT ?? DBNull.Value);
                        cmdNgh.Parameters.AddWithValue("@DiaChi", (object)ngh.DiaChi ?? DBNull.Value);
                        cmdNgh.Parameters.AddWithValue("@MoiQuanHe", (object)ngh.MoiQuanHe ?? DBNull.Value);
                        cmdNgh.ExecuteNonQuery();

                        kh.MaNGH = ngh.MaNGH; // Gán MaNGH cho khách hàng
                    }

                    // 3. THÊM KHÁCH HÀNG
                    string queryKh = @"
                        INSERT INTO KHACHHANG (MaKH, HoTen, CCCD, SDT, Email, NgaySinh, GioiTinh, DiaChi, MaTK, MaNGH)
                        VALUES (@MaKH, @HoTen, @CCCD, @SDT, @Email, @NgaySinh, @GioiTinh, @DiaChi, @MaTK, @MaNGH)";
                    SqlCommand cmdKh = new SqlCommand(queryKh, conn, tran);
                    cmdKh.Parameters.AddWithValue("@MaKH", kh.MaKH);
                    cmdKh.Parameters.AddWithValue("@HoTen", kh.HoTen);
                    cmdKh.Parameters.AddWithValue("@CCCD", (object)kh.CCCD ?? DBNull.Value); // Chấp nhận NULL
                    cmdKh.Parameters.AddWithValue("@SDT", (object)kh.SDT ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@Email", (object)kh.Email ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@NgaySinh", kh.NgaySinh);
                    cmdKh.Parameters.AddWithValue("@GioiTinh", kh.GioiTinh);
                    cmdKh.Parameters.AddWithValue("@DiaChi", kh.DiaChi);
                    cmdKh.Parameters.AddWithValue("@MaTK", (object)kh.MaTK ?? DBNull.Value); // Có thể null nếu ko tạo TK
                    cmdKh.Parameters.AddWithValue("@MaNGH", (object)kh.MaNGH ?? DBNull.Value); // Có thể null nếu ko có NGH

                    int rows = cmdKh.ExecuteNonQuery();

                    tran.Commit(); // Hoàn tất nếu không có lỗi
                    return rows > 0;
                }
                catch (Exception ex) // Bắt lỗi để Rollback
                {
                    tran.Rollback(); // Hoàn tác tất cả thay đổi nếu có lỗi
                    // Ghi log lỗi ở đây nếu cần
                    // Log.Error("Lỗi khi tạo khách hàng: " + ex.ToString());
                    throw; // Ném lại lỗi để Controller biết và báo cho người dùng
                }
            }
        }

        public string GenerateNextMaKH()
        {
            string lastMaKH = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 MaKH FROM KHACHHANG ORDER BY MaKH DESC", conn);
                lastMaKH = cmd.ExecuteScalar() as string;
            }

            if (string.IsNullOrEmpty(lastMaKH))
                return "KH0001";

            int num = int.Parse(lastMaKH.Substring(2)) + 1;
            return "KH" + num.ToString("D4"); // D4 để luôn 4 chữ số
        }

        public string GenerateNextMaNGH()
        {
            string lastMaNGH = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 MaNGH FROM NGUOIGIAMHO ORDER BY MaNGH DESC", conn);
                lastMaNGH = cmd.ExecuteScalar() as string;
            }

            if (string.IsNullOrEmpty(lastMaNGH))
                return "NGH0001";

            int num = int.Parse(lastMaNGH.Substring(3)) + 1;
            return "NGH" + num.ToString("D4");
        }

        public List<KhachHangManageViewModel> Search(string keyword)
        {
            List<KhachHangManageViewModel> list = new List<KhachHangManageViewModel>();
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = @"
                    SELECT 
                        kh.*, 
                        tk.Username, tk.PasswordHash
                    FROM KHACHHANG kh
                    LEFT JOIN TAIKHOAN tk ON kh.MaTK = tk.MaTK
                    WHERE kh.MaKH LIKE @kw OR kh.HoTen LIKE @kw OR kh.SDT LIKE @kw";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // 1. Tạo đối tượng KhachHang (model DB)
                    KhachHang kh = new KhachHang
                    {
                        MaKH = dr["MaKH"].ToString(),
                        HoTen = dr["HoTen"].ToString(),
                        NgaySinh = Convert.ToDateTime(dr["NgaySinh"]),
                        GioiTinh = dr["GioiTinh"].ToString(),
                        SDT = dr["SDT"].ToString(),
                        Email = dr["Email"].ToString(),
                        DiaChi = dr["DiaChi"].ToString(),
                        MaNGH = dr["MaNGH"] != DBNull.Value ? dr["MaNGH"].ToString() : null,
                        MaTK = dr["MaTK"] != DBNull.Value ? (int?)Convert.ToInt32(dr["MaTK"]) : null,
                        CCCD = dr["CCCD"] != DBNull.Value ? dr["CCCD"].ToString() : null
                    };

                    // 2. Tạo đối tượng TaiKhoan (nếu có)
                    TaiKhoan tk = null;
                    if (dr["Username"] != DBNull.Value)
                    {
                        tk = new TaiKhoan
                        {
                            MaTK = kh.MaTK.Value,
                            Username = dr["Username"].ToString(),
                            PasswordHash = dr["PasswordHash"] != DBNull.Value ? dr["PasswordHash"].ToString() : null
                        };
                    }

                    // 3. Đóng gói vào ViewModel
                    list.Add(new KhachHangManageViewModel
                    {
                        KhachHang = kh,
                        TaiKhoan = tk
                    });
                }
            }
            return list;
        }

        public bool Update(KhachHang kh, NguoiGiamHo ngh, TaiKhoan tk = null)
        {
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // --- PHẦN 1: XỬ LÝ NGƯỜI GIÁM HỘ (NGH) ---
                    // (Toàn bộ logic NGH của bạn ở đây là chính xác, giữ nguyên)
                    string oldMaNGH = null;
                    SqlCommand cmdGetOldNGH = new SqlCommand("SELECT MaNGH FROM KHACHHANG WHERE MaKH = @MaKH", conn, tran);
                    cmdGetOldNGH.Parameters.AddWithValue("@MaKH", kh.MaKH);
                    var result = cmdGetOldNGH.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        oldMaNGH = result.ToString();
                    }

                    if (ngh != null)
                    {
                        SqlCommand cmdCheckNGH = new SqlCommand("SELECT COUNT(*) FROM NGUOIGIAMHO WHERE MaNGH = @MaNGH", conn, tran);
                        cmdCheckNGH.Parameters.AddWithValue("@MaNGH", ngh.MaNGH);
                        bool nghExists = (int)cmdCheckNGH.ExecuteScalar() > 0;

                        if (nghExists)
                        {
                            // --- UPDATE NGUOIGIAMHO ---
                            string queryNgh = @"UPDATE NGUOIGIAMHO SET HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, SDT = @SDT, DiaChi = @DiaChi, MoiQuanHe = @MoiQuanHe WHERE MaNGH = @MaNGH";
                            SqlCommand cmdNgh = new SqlCommand(queryNgh, conn, tran);
                            cmdNgh.Parameters.AddWithValue("@MaNGH", ngh.MaNGH);
                            cmdNgh.Parameters.AddWithValue("@HoTen", ngh.HoTen);
                            cmdNgh.Parameters.AddWithValue("@NgaySinh", (object)ngh.NgaySinh ?? DBNull.Value);
                            // Dùng (object) ?? DBNull.Value thay vì ?? "" để an toàn hơn cho CSDL
                            cmdNgh.Parameters.AddWithValue("@GioiTinh", (object)ngh.GioiTinh ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@SDT", (object)ngh.SDT ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@DiaChi", (object)ngh.DiaChi ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@MoiQuanHe", (object)ngh.MoiQuanHe ?? DBNull.Value);
                            cmdNgh.ExecuteNonQuery();
                        }
                        else
                        {
                            // --- INSERT NGUOIGIAMHO MỚI ---
                            string queryNgh = @"INSERT INTO NGUOIGIAMHO (MaNGH, HoTen, NgaySinh, GioiTinh, SDT, DiaChi, MoiQuanHe) VALUES (@MaNGH, @HoTen, @NgaySinh, @GioiTinh, @SDT, @DiaChi, @MoiQuanHe)";
                            SqlCommand cmdNgh = new SqlCommand(queryNgh, conn, tran);
                            cmdNgh.Parameters.AddWithValue("@MaNGH", ngh.MaNGH);
                            cmdNgh.Parameters.AddWithValue("@HoTen", ngh.HoTen);
                            cmdNgh.Parameters.AddWithValue("@NgaySinh", (object)ngh.NgaySinh ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@GioiTinh", (object)ngh.GioiTinh ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@SDT", (object)ngh.SDT ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@DiaChi", (object)ngh.DiaChi ?? DBNull.Value);
                            cmdNgh.Parameters.AddWithValue("@MoiQuanHe", (object)ngh.MoiQuanHe ?? DBNull.Value);
                            cmdNgh.ExecuteNonQuery();
                        }
                        kh.MaNGH = ngh.MaNGH;
                    }
                    else
                    {
                        // --- TRƯỜNG HỢP 4 (VẪN RẤT CẦN THIẾT) ---
                        kh.MaNGH = null;
                        // (Tùy chọn: Xóa NGH mồ côi (oldMaNGH) nếu bạn muốn)
                    }
                    // --- KẾT THÚC PHẦN 1 (NGH) ---


                    // === SỬA 2: BỔ SUNG PHẦN XỬ LÝ TÀI KHOẢN (TK) ===
                    if (tk != null)
                    {
                        // TH 1: Tài khoản đã tồn tại (MaTK > 0) -> UPDATE
                        if (tk.MaTK > 0)
                        {
                            string queryTk = @"
                        UPDATE TAIKHOAN SET
                            Username = @Username,
                            PasswordHash = @Password,
                            IsActive = @IsActive
                        WHERE MaTK = @MaTK";

                            SqlCommand cmdTk = new SqlCommand(queryTk, conn, tran);
                            cmdTk.Parameters.AddWithValue("@Username", tk.Username);
                            cmdTk.Parameters.AddWithValue("@Password", tk.PasswordHash); // PasswordHash này đã được xử lý ở Controller (nếu rỗng là giữ pass cũ)
                            cmdTk.Parameters.AddWithValue("@IsActive", tk.IsActive);
                            cmdTk.Parameters.AddWithValue("@MaTK", tk.MaTK);

                            cmdTk.ExecuteNonQuery();
                            kh.MaTK = tk.MaTK; // Gán MaTK vào khách hàng
                        }
                        // TH 2: Tài khoản mới (MaTK=0) và có nhập mật khẩu -> INSERT
                        else if (tk.MaTK == 0 && !string.IsNullOrWhiteSpace(tk.PasswordHash))
                        {
                            string queryTkInsert = @"
                        INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) 
                        OUTPUT INSERTED.MaTK 
                        VALUES (@Username, @Password, @IsActive)";

                            SqlCommand cmdTkInsert = new SqlCommand(queryTkInsert, conn, tran);
                            cmdTkInsert.Parameters.AddWithValue("@Username", tk.Username);
                            cmdTkInsert.Parameters.AddWithValue("@Password", tk.PasswordHash);
                            cmdTkInsert.Parameters.AddWithValue("@IsActive", tk.IsActive);

                            object insertedMaTK = cmdTkInsert.ExecuteScalar();
                            if (insertedMaTK != null && insertedMaTK != DBNull.Value)
                            {
                                kh.MaTK = Convert.ToInt32(insertedMaTK); // Gán MaTK mới vào khách hàng
                            }
                            else
                            {
                                throw new Exception("Không thể tạo tài khoản mới.");
                            }
                        }
                    }
                    // === KẾT THÚC SỬA 2 ===


                    // --- SỬA 3: CẬP NHẬT LẠI QUERY CHO KHÁCH HÀNG ---
                    string queryKh = @"
                UPDATE KHACHHANG SET
                    HoTen = @HoTen, 
                    CCCD = @CCCD, 
                    SDT = @SDT, 
                    Email = @Email, 
                    NgaySinh = @NgaySinh, 
                    GioiTinh = @GioiTinh, 
                    DiaChi = @DiaChi, 
                    MaNGH = @MaNGH,
                    MaTK = @MaTK  -- Bổ sung MaTK
                WHERE MaKH = @MaKH";

                    SqlCommand cmdKh = new SqlCommand(queryKh, conn, tran);
                    cmdKh.Parameters.AddWithValue("@MaKH", kh.MaKH);
                    cmdKh.Parameters.AddWithValue("@HoTen", kh.HoTen);
                    cmdKh.Parameters.AddWithValue("@CCCD", (object)kh.CCCD ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@SDT", (object)kh.SDT ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@Email", (object)kh.Email ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@NgaySinh", kh.NgaySinh);
                    cmdKh.Parameters.AddWithValue("@GioiTinh", (object)kh.GioiTinh ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@DiaChi", (object)kh.DiaChi ?? DBNull.Value);
                    cmdKh.Parameters.AddWithValue("@MaNGH", (object)kh.MaNGH ?? DBNull.Value);
                    // === SỬA 4: Thêm tham số MaTK ===
                    cmdKh.Parameters.AddWithValue("@MaTK", (object)kh.MaTK ?? DBNull.Value);

                    int rows = cmdKh.ExecuteNonQuery();

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
        public bool UsernameExistsForAnotherAccount(string username, int currentMaTK)
        {
            if (string.IsNullOrEmpty(username)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                // Kiểm tra xem username này có tồn tại VÀ MaTK không phải là của tài khoản hiện tại
                string query = "SELECT COUNT(1) FROM TAIKHOAN WHERE Username = @Username AND MaTK != @MaTK";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaTK", currentMaTK); // ID của tài khoản đang sửa

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public TaiKhoan GetTaiKhoanByMaTK(int? maTK)
        {
            // Nếu không có MaTK thì không cần tìm
            if (maTK == null || maTK == 0) return null;

            TaiKhoan tk = null;
            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT * FROM TAIKHOAN WHERE MaTK = @MaTK";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaTK", maTK.Value);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    tk = new TaiKhoan
                    {
                        MaTK = Convert.ToInt32(dr["MaTK"]),
                        Username = dr["Username"].ToString(),
                        // Lấy cả mật khẩu (đang là plain text)
                        PasswordHash = dr["PasswordHash"].ToString(),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                    };
                }
            }
            return tk;
        }

        // === HÀM KIỂM TRA USERNAME TỒN TẠI ===
        public bool UsernameExists(string username)
        {
            if (string.IsNullOrEmpty(username)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM TAIKHOAN WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        
        // Kiểm tra CCCD đã tồn tại trong bảng KHACHHANG hay chưa
        // excludeMaKH: truyền MaKH hiện tại khi sửa để bỏ qua chính nó
        public bool CustomerCccdExists(string cccd, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM KHACHHANG WHERE CCCD = @CCCD";
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    query += " AND MaKH <> @MaKH";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", cccd.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Kiểm tra Email đã tồn tại trong bảng KHACHHANG hay chưa
        public bool CustomerEmailExists(string email, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM KHACHHANG WHERE Email = @Email";
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    query += " AND MaKH <> @MaKH";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // Kiểm tra SĐT đã tồn tại trong bảng KHACHHANG hay chưa
        public bool CustomerPhoneExists(string sdt, string excludeMaKH = null)
        {
            if (string.IsNullOrWhiteSpace(sdt)) return false;

            using (SqlConnection conn = new SqlConnection(connectStr))
            {
                string query = "SELECT COUNT(1) FROM KHACHHANG WHERE SDT = @SDT";
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    query += " AND MaKH <> @MaKH";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SDT", sdt.Trim());
                if (!string.IsNullOrEmpty(excludeMaKH))
                {
                    cmd.Parameters.AddWithValue("@MaKH", excludeMaKH.Trim());
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }
}