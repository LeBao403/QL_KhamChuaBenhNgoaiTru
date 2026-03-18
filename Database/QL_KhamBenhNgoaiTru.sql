CREATE DATABASE QL_KhamBenhNgoaiTru;
GO
USE QL_KhamBenhNgoaiTru;
GO

-- ======================================================================================
-- I. QUẢN LÝ CON NGƯỜI & HỆ THỐNG
-- ======================================================================================

-- 1. Khách hàng / Bệnh nhân
CREATE TABLE BENHNHAN (
    MaBN CHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    CCCD NVARCHAR(20),
    SDT NVARCHAR(15),
    Email NVARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(200),
    BHYT BIT NOT NULL DEFAULT 0,
    SoTheBHYT NVARCHAR(50),
    HanSuDungBHYT DATE,
    TuyenKham NVARCHAR(50) CHECK (TuyenKham IN (N'Đúng tuyến', N'Trái tuyến')),
    MucHuongBHYT INT, -- Lưu phần trăm hưởng (VD: 80, 95, 100)
    MaTK INT
);

CREATE TABLE DANHMUC_TIENSU_YTE (
    MaTSYT CHAR(10) PRIMARY KEY,
    TenTSYT NVARCHAR(MAX),
);

CREATE TABLE TIENSU_YTE_BENHNHAN (
    MaTSYT CHAR(10),
    MaBN CHAR(10),
    PRIMARY KEY(MaTSYT, MaBN)
);

CREATE TABLE KHOA (
    MaKhoa INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(MAX),
    TrangThai BIT DEFAULT 1
);

CREATE TABLE NHANVIEN (
    MaNV CHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    SDT NVARCHAR(15) UNIQUE,
    Email NVARCHAR(100) UNIQUE,
    DiaChi NVARCHAR(200),
    MaChucVu INT,
    TrangThai BIT DEFAULT 1, 
    MaTK INT UNIQUE,
    MaKhoa INT NULL,    
    MaPhong INT NULL
);

CREATE TABLE TAIKHOAN (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

CREATE TABLE CHUCVU (
    MaChucVu INT IDENTITY(1,1) PRIMARY KEY,
    TenChucVu NVARCHAR(50) UNIQUE
);

CREATE TABLE DANHMUC_LOAIPHONG (
    MaLoaiPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiPhong NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE PHONG (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL,
	MaLoaiPhong INT,
    TrangThai BIT DEFAULT 1, 
    MaKhoa INT NULL     
);

-- ======================================================================================
-- II. QUẢN LÝ DƯỢC & KHO (UPDATE: LOGIC ĐƠN VỊ & LÔ GỘP)
-- ======================================================================================

-- 1. Danh mục Nhóm/Hoạt chất/NSX
CREATE TABLE DANHMUC_THUOC (
    MaDanhMuc CHAR(10) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL UNIQUE, 
    MoTa NVARCHAR(MAX)
);

CREATE TABLE DANHMUC_HOATCHAT (
    MaHoatChat CHAR(10) PRIMARY KEY,
    TenHoatChat NVARCHAR(200) NOT NULL UNIQUE, 
    MoTa NVARCHAR(500)
);

CREATE TABLE NHASANXUAT (
    MaNSX INT IDENTITY(1,1) PRIMARY KEY,
    TenNSX NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    SDT CHAR(50),
    Email NVARCHAR(100),
    QuocGia NVARCHAR(100)
);

-- 2. BẢNG THUỐC (Thông tin định danh sản phẩm)
CREATE TABLE THUOC (
    MaThuoc CHAR(10) PRIMARY KEY,
    TenThuoc NVARCHAR(200) NOT NULL, 
    
    -- Quy cách đóng gói/Dung tích (VD: Chai 100ml, Tuýp 10g)
    QuyCach NVARCHAR(100), 
    
    -- Đơn vị cơ bản nhỏ nhất để tính kho (VD: Viên, Chai, Tuýp)
    DonViCoBan NVARCHAR(20) NOT NULL 
        CHECK (DonViCoBan IN (N'Viên', N'Chai', N'Lọ', N'Tuýp', N'Gói', N'Ống')),
    
    MaLoaiThuoc CHAR(10), 
    DuongDung NVARCHAR(50), 
    GiaBan DECIMAL(18,2), -- Giá bán niêm yết tham khảo

    CoBHYT BIT DEFAULT 0, -- Thuốc này có thuộc danh mục BHYT không
    GiaBHYT DECIMAL(18,2), -- Giá trần do BHYT quy định

    MaNSX INT,
    TrangThai BIT DEFAULT 1
);

-- 3. BẢNG ĐƠN VỊ QUY ĐỔI (Giải quyết Hộp/Vỉ)
/*
CREATE TABLE DONVI_QUYDOI (
    MaQuyDoi INT IDENTITY(1,1) PRIMARY KEY,
    MaThuoc CHAR(10) NOT NULL,
    
    -- Tên đơn vị lớn (VD: Hộp, Vỉ)
    TenDonVi NVARCHAR(20) NOT NULL,
    
    -- Tỷ lệ quy đổi ra Đơn vị cơ bản (VD: 1 Hộp = 100 Viên -> Nhập 100)
    TyLeQuyDoi INT NOT NULL, 
    
    GiaBanQuyDoi DECIMAL(18,2),
    CapDo INT DEFAULT 1 -- 1=Vỉ, 2=Hộp (Sắp xếp hiển thị)
);
*/

-- 4. BẢNG TỒN KHO (TÍCH HỢP LÔ + HẠN DÙNG + VỊ TRÍ)
CREATE TABLE TONKHO (
    MaTonKho INT IDENTITY(1,1) PRIMARY KEY,
    
    MaPhong INT NOT NULL,      -- Thuốc đang ở phòng nào (Kho chẵn/Nhà thuốc)
    MaThuoc CHAR(10) NOT NULL,
    
    -- [OPTION 2] Lưu thông tin Lô trực tiếp tại đây
    MaLo NVARCHAR(50) NOT NULL, -- Mã lô in trên vỏ hộp
    HanSuDung DATE NOT NULL,    -- Ngày hết hạn
    NgaySanXuat DATE,           -- Ngày sản xuất
    GiaNhap DECIMAL(18,2),      -- Giá vốn nhập của lô này
    
    -- SỐ LƯỢNG LUÔN LƯU THEO ĐƠN VỊ CƠ BẢN (VIÊN/CHAI)
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0), 
    
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    
    -- Một phòng, một thuốc, một lô chỉ có 1 dòng duy nhất
    CONSTRAINT UQ_TONKHO_PHONG_THUOC_LO UNIQUE (MaPhong, MaThuoc, MaLo)
);

CREATE TABLE THANHPHAN_THUOC (
    MaThanhPhan CHAR(10) PRIMARY KEY,
    MaThuoc CHAR(10) NOT NULL,
    MaHoatChat CHAR(10) NOT NULL,
    HamLuong NVARCHAR(50), 
);

-- ======================================================================================
-- III. QUY TRÌNH KHÁM BỆNH & DANH MỤC BỆNH/CLS
-- ======================================================================================

CREATE TABLE DANHMUC_BENH (
    MaBenh CHAR(10) PRIMARY KEY, 
    TenBenh NVARCHAR(200) NOT NULL,
    TrieuChung NVARCHAR(200),
    MoTa NVARCHAR(MAX),
    SoGiaiDoan BIT DEFAULT 0 
);

CREATE TABLE LOAI_DICHVU (
    MaLoaiDV CHAR(10) PRIMARY KEY,
    TenLoaiDV NVARCHAR(100) NOT NULL,
    Gia DECIMAL(18,2) NOT NULL,
    MoTa NVARCHAR(MAX)
);

CREATE TABLE DICHVU (
    MaDV CHAR(10) PRIMARY KEY,
    TenDV NVARCHAR(200) NOT NULL,
    MaLoaiDV CHAR(10) NOT NULL, 
    GiaDichVu DECIMAL(18,2) NOT NULL,
    DonViTinh NVARCHAR(20), 
    
    CoBHYT BIT DEFAULT 0, -- Dịch vụ này có được BHYT hỗ trợ không
    GiaBHYT DECIMAL(18,2), -- Mức giá dịch vụ do BHYT chi trả

    TrangThai BIT DEFAULT 1,
);

CREATE TABLE PHIEUDANGKY (
    MaPhieuDK INT IDENTITY(1,1) PRIMARY KEY,
    MaBN CHAR(10),
    NgayDangKy DATE DEFAULT GETDATE(),
    HinhThucDangKy NVARCHAR(20) CHECK (HinhThucDangKy IN (N'Online', N'Offline')),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Chờ xử lý', N'Đã xác nhận', N'Hủy'))
);

/*
CREATE TABLE PHIEUKHAMSANGLOC (
    MaPhieuKhamSL INT PRIMARY KEY,
    ChieuCao DECIMAL(5,2),
    CanNang DECIMAL(5,2),
    Mach INT,
    HuyetAp NVARCHAR(20),
    NhipTho INT,   
    KetLuan NVARCHAR(50) CHECK (KetLuan IN (N'Đủ điều kiện', N'Không đủ điều kiện')),
    GhiChu NVARCHAR(MAX),
    MaBacSiKham CHAR(10),
    MaPhong INT
);
*/

CREATE TABLE PHIEUKHAMBENH ( 
    MaPhieuKhamBenh INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuDK INT, 
    MaBN CHAR(10), 
    STT INT NULL,
    LyDoDenKham NVARCHAR(MAX),
    NgayLap DATE DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Đang khám', N'Hoàn thành', N'Đã hủy')) DEFAULT N'Đang khám',
    TrieuChung NVARCHAR(MAX),
    KetLuan NVARCHAR(200),
    MaBacSiKham CHAR(10),
    MaPhong INT
);

CREATE TABLE CHITIET_CHANDOAN ( 
    MaCTChanDoan INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKhamBenh INT NOT NULL,
    MaBenh CHAR(10) NOT NULL,
    LoaiBenh NVARCHAR(50) CHECK (LoaiBenh IN (N'Bệnh chính', N'Bệnh kèm theo')),
    KetLuanChiTiet NVARCHAR(200),
    GiaiDoan BIT NOT NULL DEFAULT 0 
);

-- ======================================================================================
-- IV. CẬN LÂM SÀNG & KÊ ĐƠN
-- ======================================================================================

CREATE TABLE PHIEU_CHIDINH (
    MaPhieuChiDinh INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKhamBenh INT NOT NULL, 
    MaBacSiChiDinh CHAR(10),     
    NgayChiDinh DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán', N'Đang thực hiện', N'Hoàn tất')) DEFAULT N'Chưa thanh toán',
    TongTien DECIMAL(18,2), 
    MaPhong INT
);

CREATE TABLE CHITIET_CHIDINH (
    MaCTChiDinh INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuChiDinh INT NOT NULL,
    MaDV CHAR(10) NOT NULL, 
    DonGia DECIMAL(18,2), 
    MaBacSiThucHien CHAR(10), 
    KetQua NVARCHAR(MAX),     
    FileKetQua NVARCHAR(MAX), 
    ThoiGianCoKetQua TIME,
    TrangThai NVARCHAR(50) DEFAULT N'Chưa thực hiện'
        CHECK (TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện', N'Đã có kết quả')),
);

CREATE TABLE DON_THUOC (
    MaDonThuoc INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKhamBenh INT NOT NULL UNIQUE, 
    NgayKe DATETIME DEFAULT GETDATE(),
    LoiDanBS NVARCHAR(MAX),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN (N'Chưa phát', N'Đã phát thuốc', N'Đã hủy')) DEFAULT N'Chưa phát',
);

CREATE TABLE CT_DON_THUOC (
    MaCTDonThuoc INT IDENTITY(1,1) PRIMARY KEY,
    MaDonThuoc INT NOT NULL,
    MaThuoc CHAR(10) NOT NULL,
    
    -- Dùng DECIMAL và cài CHECK để bắt buộc phải là bội số của 0.5 (0.5, 1, 1.5, 2...)
    SoLuongSang DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongSang * 10) % 5 = 0),
    SoLuongTrua DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongTrua * 10) % 5 = 0),
    SoLuongChieu DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongChieu * 10) % 5 = 0),
    SoLuongToi DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongToi * 10) % 5 = 0),
    SoNgayDung INT DEFAULT 1 CHECK (SoNgayDung > 0),
    
    SoLuong INT NOT NULL, -- Tổng số lượng (Phần mềm tự tính và truyền xuống)
    
    DonViTinh NVARCHAR(20), 
    DonGia DECIMAL(18,2), 
    GhiChu NVARCHAR(200), 
);

-- ======================================================================================
-- V. TÀI CHÍNH
-- ======================================================================================

CREATE TABLE HOADON (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaBN CHAR(10),
    MaPhieuKhamBenh INT, 
    NgayThanhToan DATE ,
    TongTien DECIMAL(18,2),
    TrangThaiThanhToan NVARCHAR(50) NOT NULL 
        CHECK (TrangThaiThanhToan IN (N'Đã thanh toán', N'Chưa thanh toán'))
        DEFAULT N'Chưa thanh toán',
    HinhThucThanhToan NVARCHAR(50) 
        CHECK (HinhThucThanhToan IN (N'Tiền mặt', N'Chuyển khoản', N'Thẻ')),
    GhiChu NVARCHAR(200),
);

CREATE TABLE CT_HOADON_DV (
    MaCTHD INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaDV CHAR(10),
    DonGia DECIMAL(18,2) NOT NULL,
    
    -- [BỔ SUNG BHYT] (Tách ThanhTien thành 3 khoản)
    TongTienGoc DECIMAL(18,2) NOT NULL,       -- Tổng tiền dịch vụ ban đầu
    TienBHYTChiTra DECIMAL(18,2) DEFAULT 0,   -- Số tiền quỹ BHYT chịu
    TienBenhNhanTra DECIMAL(18,2) NOT NULL,   -- Số tiền khách hàng tự móc ví đóng (Đồng chi trả)
    -- [KẾT THÚC BỔ SUNG]

    MaNV_ThuNgan CHAR(10),
);

CREATE TABLE CT_HOADON_THUOC (
    MaCTHD INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaDonThuoc INT,

    -- [BỔ SUNG BHYT] (Tách ThanhTien thành 3 khoản)
    TongTienGoc DECIMAL(18,2) NOT NULL,       -- Tổng tiền thuốc ban đầu
    TienBHYTChiTra DECIMAL(18,2) DEFAULT 0,   -- Số tiền quỹ BHYT chịu
    TienBenhNhanTra DECIMAL(18,2) NOT NULL,   -- Số tiền khách hàng tự móc ví đóng (Đồng chi trả)
    -- [KẾT THÚC BỔ SUNG]

    MaNV_ThuNgan CHAR(10),
);

-- ======================================================================================
-- VI. KHÓA NGOẠI (FOREIGN KEYS)
-- ======================================================================================

-- 1. Nhân viên & Bệnh nhân
ALTER TABLE BENHNHAN ADD CONSTRAINT FK_BENHNHAN_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK);
ALTER TABLE TIENSU_YTE_BENHNHAN ADD CONSTRAINT FK_TSYTBN_TSYT FOREIGN KEY (MaTSYT) REFERENCES DANHMUC_TIENSU_YTE(MaTSYT);
ALTER TABLE TIENSU_YTE_BENHNHAN ADD CONSTRAINT FK_TSYTBN_BN FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);

ALTER TABLE NHANVIEN ADD CONSTRAINT FK_NV_CHUCVU FOREIGN KEY (MaChucVu) REFERENCES CHUCVU(MaChucVu);
ALTER TABLE NHANVIEN ADD CONSTRAINT FK_NHANVIEN_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK);
ALTER TABLE NHANVIEN ADD CONSTRAINT FK_NV_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);
ALTER TABLE PHONG ADD CONSTRAINT FK_PHONG_LOAIPHONG FOREIGN KEY (MaLoaiPhong) REFERENCES DANHMUC_LOAIPHONG(MaLoaiPhong);

ALTER TABLE NHANVIEN ADD CONSTRAINT FK_NV_KHOA FOREIGN KEY (MaKhoa) REFERENCES KHOA(MaKhoa);
ALTER TABLE PHONG ADD CONSTRAINT FK_PHONG_KHOA FOREIGN KEY (MaKhoa) REFERENCES KHOA(MaKhoa);

-- 2. Dược & Kho
ALTER TABLE THUOC ADD CONSTRAINT FK_THUOC_NSX FOREIGN KEY (MaNSX) REFERENCES NHASANXUAT(MaNSX);
ALTER TABLE THUOC ADD CONSTRAINT FK_THUOC_DM FOREIGN KEY (MaLoaiThuoc) REFERENCES DANHMUC_THUOC(MaDanhMuc);

-- ALTER TABLE DONVI_QUYDOI ADD CONSTRAINT FK_DVQD_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);

ALTER TABLE THANHPHAN_THUOC ADD CONSTRAINT FK_TP_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);
ALTER TABLE THANHPHAN_THUOC ADD CONSTRAINT FK_TP_HOATCHAT FOREIGN KEY (MaHoatChat) REFERENCES DANHMUC_HOATCHAT(MaHoatChat);

-- [QUAN TRỌNG] Tồn kho link trực tiếp tới Thuốc (Option 2: không có bảng Lô riêng)
ALTER TABLE TONKHO ADD CONSTRAINT FK_TK_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);
ALTER TABLE TONKHO ADD CONSTRAINT FK_TK_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);

-- 3. Khám bệnh & CLS
ALTER TABLE DICHVU ADD CONSTRAINT FK_DV_LOAI FOREIGN KEY (MaLoaiDV) REFERENCES LOAI_DICHVU(MaLoaiDV);

ALTER TABLE PHIEUDANGKY ADD CONSTRAINT FK_PDK_BNDK FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);

-- ALTER TABLE PHIEUKHAMSANGLOC ADD CONSTRAINT FK_PKSL_BS FOREIGN KEY (MaBacSiKham) REFERENCES NHANVIEN(MaNV);
-- ALTER TABLE PHIEUKHAMSANGLOC ADD CONSTRAINT FK_PKSL_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);

ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_PDK FOREIGN KEY (MaPhieuDK) REFERENCES PHIEUDANGKY(MaPhieuDK);
ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_BN FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);
ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_BS FOREIGN KEY (MaBacSiKham) REFERENCES NHANVIEN(MaNV);
-- ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_PKSL FOREIGN KEY (MaPhieuKhamSL) REFERENCES PHIEUKHAMSANGLOC(MaPhieuKhamSL);
ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);

ALTER TABLE CHITIET_CHANDOAN ADD CONSTRAINT FK_CTCD_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);
ALTER TABLE CHITIET_CHANDOAN ADD CONSTRAINT FK_CTCD_BENH FOREIGN KEY (MaBenh) REFERENCES DANHMUC_BENH(MaBenh);

ALTER TABLE PHIEU_CHIDINH ADD CONSTRAINT FK_PCD_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);
ALTER TABLE PHIEU_CHIDINH ADD CONSTRAINT FK_PCD_BS FOREIGN KEY (MaBacSiChiDinh) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEU_CHIDINH ADD CONSTRAINT FK_PCD_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);

ALTER TABLE CHITIET_CHIDINH ADD CONSTRAINT FK_CTCDI_PCD FOREIGN KEY (MaPhieuChiDinh) REFERENCES PHIEU_CHIDINH(MaPhieuChiDinh);
ALTER TABLE CHITIET_CHIDINH ADD CONSTRAINT FK_CTCDI_DV FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV);
ALTER TABLE CHITIET_CHIDINH ADD CONSTRAINT FK_CTCDI_BS FOREIGN KEY (MaBacSiThucHien) REFERENCES NHANVIEN(MaNV);

ALTER TABLE DON_THUOC ADD CONSTRAINT FK_DT_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);

ALTER TABLE CT_DON_THUOC ADD CONSTRAINT FK_CTDT_DT FOREIGN KEY (MaDonThuoc) REFERENCES DON_THUOC(MaDonThuoc);
ALTER TABLE CT_DON_THUOC ADD CONSTRAINT FK_CTDT_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);

-- 4. Tài chính
ALTER TABLE HOADON ADD CONSTRAINT FK_HD_BN FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);
ALTER TABLE HOADON ADD CONSTRAINT FK_HD_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);
ALTER TABLE CT_HOADON_DV ADD CONSTRAINT FK_CTHDDV_NV FOREIGN KEY (MaNV_ThuNgan) REFERENCES NHANVIEN(MaNV);
ALTER TABLE CT_HOADON_DV ADD CONSTRAINT FK_CTHDDV_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD);
ALTER TABLE CT_HOADON_DV ADD CONSTRAINT FK_CTHDDV_DV FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV);

ALTER TABLE CT_HOADON_THUOC ADD CONSTRAINT FK_CTHDT_NV FOREIGN KEY (MaNV_ThuNgan) REFERENCES NHANVIEN(MaNV);
ALTER TABLE CT_HOADON_THUOC ADD CONSTRAINT FK_CTHDT_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD);
ALTER TABLE CT_HOADON_THUOC ADD CONSTRAINT FK_CTHDT_DT FOREIGN KEY (MaDonThuoc) REFERENCES DON_THUOC(MaDonThuoc);
GO



-- ======================================================================================
-- VII. QUẢN LÝ NHẬP KHO
-- ======================================================================================

-- 1. Bảng Phiếu Nhập (Đại diện cho 1 đợt giao hàng/1 hóa đơn từ nhà cung cấp)
CREATE TABLE PHIEUNHAP (
    MaPhieuNhap INT IDENTITY(1,1) PRIMARY KEY,
    MaNV_LapPhieu CHAR(10) NOT NULL,    -- Người tạo phiếu (Thủ kho)
    MaNSX INT NOT NULL,                 -- Nhập của nhà cung cấp nào
    NgayLap DATETIME DEFAULT GETDATE(),
    
    TongTienNhap DECIMAL(18,2) DEFAULT 0,
    
    -- Trạng thái để quản lý luồng phê duyệt
    TrangThai NVARCHAR(50) DEFAULT N'Chờ duyệt'
        CHECK (TrangThai IN (N'Chờ duyệt', N'Đã duyệt', N'Đã hủy')),
        
    GhiChu NVARCHAR(200),
    
    -- Thông tin người duyệt (Quản lý/Giám đốc)
    MaNV_Duyet CHAR(10) NULL,
    NgayDuyet DATETIME NULL
);

-- 2. Bảng Chi Tiết Phiếu Nhập (Danh sách các lô thuốc nhập về)
CREATE TABLE CT_PHIEUNHAP (
    MaCTPN INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuNhap INT NOT NULL,
    MaThuoc CHAR(10) NOT NULL,
    
    -- Thông tin vật lý của lô hàng nhập
    MaLo NVARCHAR(50) NOT NULL,
    NgaySanXuat DATE,
    HanSuDung DATE NOT NULL,
    
    -- Số lượng nhập tính theo DonViCoBan (VD: Nhập 5 hộp, mỗi hộp 100 viên -> Ghi 500)
    SoLuongNhap INT NOT NULL CHECK (SoLuongNhap > 0),
    
    -- Đơn giá nhập tính theo DonViCoBan (VD: Giá 1 viên)
    DonGiaNhap DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL -- = SoLuongNhap * DonGiaNhap
);

-- Thêm Khóa ngoại cho 2 bảng mới
ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_NVLAP FOREIGN KEY (MaNV_LapPhieu) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_NVDUYET FOREIGN KEY (MaNV_Duyet) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_NSX FOREIGN KEY (MaNSX) REFERENCES NHASANXUAT(MaNSX);

ALTER TABLE CT_PHIEUNHAP ADD CONSTRAINT FK_CTPN_PN FOREIGN KEY (MaPhieuNhap) REFERENCES PHIEUNHAP(MaPhieuNhap);
ALTER TABLE CT_PHIEUNHAP ADD CONSTRAINT FK_CTPN_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);