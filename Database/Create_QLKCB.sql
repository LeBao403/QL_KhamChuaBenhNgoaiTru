CREATE DATABASE QL_KhamBenhNgoaiTru;
GO

USE QL_KhamBenhNgoaiTru;
GO

-- ======================================================================================
-- I. QUẢN LÝ HỆ THỐNG & NHÂN SỰ (CÁC BẢNG NỀN TẢNG)
-- ======================================================================================

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

CREATE TABLE KHOA (
    MaKhoa INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(MAX),
    TrangThai BIT DEFAULT 1
);

CREATE TABLE DANHMUC_LOAIPHONG (
    MaLoaiPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiPhong NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE PHONG (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL,
    MaLoaiPhong INT,
    TrangThai BIT DEFAULT 1, 
    MaKhoa INT NULL     
);

CREATE TABLE NHANVIEN (
    MaNV CHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
	GioiTinh NVARCHAR(15),
    SDT NVARCHAR(15) UNIQUE,
    Email NVARCHAR(100) UNIQUE,
    DiaChi NVARCHAR(200),
    MaChucVu INT,
    TrangThai BIT DEFAULT 1, 
    MaTK INT NULL,  
    MaKhoa INT NULL,    
    MaPhong INT NULL,
    HinhAnh NVARCHAR(255) NULL
);
CREATE UNIQUE INDEX IX_NHANVIEN_MaTK ON NHANVIEN(MaTK) WHERE MaTK IS NOT NULL;


-- ======================================================================================
-- II. QUẢN LÝ BỆNH NHÂN & TIỀN SỬ Y TẾ
-- ======================================================================================

CREATE TABLE BENHNHAN (
    MaBN CHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    CCCD NVARCHAR(20),
    SDT NVARCHAR(15),
    Email NVARCHAR(100),
    NgaySinh DATE,
	GioiTinh NVARCHAR(15),
    DiaChi NVARCHAR(200),
    BHYT BIT NOT NULL DEFAULT 0,
    SoTheBHYT NVARCHAR(50),
    HanSuDungBHYT DATE,
    TuyenKham NVARCHAR(50) CHECK (TuyenKham IN (N'Đúng tuyến', N'Trái tuyến')),
    MucHuongBHYT INT, 
    AvatarPath NVARCHAR(500) NULL,
    MaTK INT
);

CREATE TABLE DANHMUC_TIENSU_YTE (
    MaTSYT CHAR(10) PRIMARY KEY,
    TenTSYT NVARCHAR(MAX)
);

CREATE TABLE TIENSU_YTE_BENHNHAN (
    MaTSYT CHAR(10),
    MaBN CHAR(10),
    PRIMARY KEY(MaTSYT, MaBN)
);


-- ======================================================================================
-- III. DANH MỤC DỊCH VỤ & BỆNH LÝ
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
    TenLoaiDV NVARCHAR(100) NOT NULL
);

CREATE TABLE DICHVU (
    MaDV CHAR(10) PRIMARY KEY,
    MaKhoa INT,
    TenDV NVARCHAR(200) NOT NULL,
    MaLoaiDV CHAR(10) NOT NULL, 
    GiaDichVu DECIMAL(18,2) NOT NULL,
    DonViTinh NVARCHAR(20), 
    CoBHYT BIT DEFAULT 0, 
    TrangThai BIT DEFAULT 1,
    MoTa NVARCHAR(MAX)
);


-- ======================================================================================
-- IV. QUẢN LÝ DƯỢC & KHO (Phần quản lý danh mục này vẫn để INT cho nhẹ DB)
-- ======================================================================================

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

CREATE TABLE KHO (
    MaKho INT IDENTITY(1,1) PRIMARY KEY,
    TenKho NVARCHAR(100) NOT NULL,
    LoaiKho NVARCHAR(50),
    DiaChi NVARCHAR(200),
    TrangThai BIT DEFAULT 1
);

CREATE TABLE THUOC (
    MaThuoc CHAR(20) PRIMARY KEY,
    TenThuoc NVARCHAR(MAX) NOT NULL, 
    QuyCach NVARCHAR(100), 
    DonViCoBan NVARCHAR(20) NOT NULL 
        CHECK (DonViCoBan IN (N'Viên', N'Chai', N'Lọ', N'Tuýp', N'Gói', N'Ống')),
    MaLoaiThuoc CHAR(10), 
    DuongDung NVARCHAR(50), 
    GiaBan DECIMAL(18,2), 
    CoBHYT BIT DEFAULT 0, 
    MaNSX INT,
    TrangThai BIT DEFAULT 1
);

CREATE TABLE THANHPHAN_THUOC (
    MaThanhPhan CHAR(10) PRIMARY KEY,
    MaThuoc CHAR(20) NOT NULL,
    MaHoatChat CHAR(10) NOT NULL,
    HamLuong NVARCHAR(50)
);

CREATE TABLE TONKHO (
    MaTonKho INT IDENTITY(1,1) PRIMARY KEY,
    MaKho INT,
    MaThuoc CHAR(20) NOT NULL,
    MaLo NVARCHAR(50) NOT NULL, 
    HanSuDung DATE NOT NULL,    
    NgaySanXuat DATE,           
    GiaNhap DECIMAL(18,2),      
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0), 
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_TONKHO_PHONG_THUOC_LO UNIQUE (MaThuoc, MaLo)
);

CREATE TABLE PHIEUNHAP (
    MaPhieuNhap INT IDENTITY(1,1) PRIMARY KEY,
    MaNV_LapPhieu CHAR(10) NOT NULL,    
    MaNSX INT NOT NULL,                 
    MaKho INT,
    NgayLap DATETIME DEFAULT GETDATE(),
    TongTienNhap DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(50) DEFAULT N'Chờ duyệt'
        CHECK (TrangThai IN (N'Chờ duyệt', N'Đã duyệt', N'Đã hủy')),
    GhiChu NVARCHAR(200),
    MaNV_Duyet CHAR(10) NULL,
    NgayDuyet DATETIME NULL
);

CREATE TABLE CT_PHIEUNHAP (
    MaCTPN INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuNhap INT NOT NULL,
    MaThuoc CHAR(20) NOT NULL,
    MaLo NVARCHAR(50) NOT NULL,
    NgaySanXuat DATE,
    HanSuDung DATE NOT NULL,
    SoLuongNhap INT NOT NULL CHECK (SoLuongNhap > 0),
    DonGiaNhap DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL 
);


-- ======================================================================================
-- V. QUY TRÌNH KHÁM BỆNH & CẬN LÂM SÀNG (CHUYỂN ĐỔI SANG MÃ SMART ID: VARCHAR)
-- ======================================================================================

CREATE TABLE DANHMUC_KHUNGGIO (
    MaKhungGio INT IDENTITY(1,1) PRIMARY KEY,
    TenKhungGio NVARCHAR(50) NOT NULL UNIQUE, 
    GioiHanSoNguoi INT NOT NULL DEFAULT 20,   
    TrangThai BIT DEFAULT 1                   
);

-- Thay thế IDENTITY bằng VARCHAR(20)
CREATE TABLE PHIEUDANGKY (
    MaPhieuDK VARCHAR(20) PRIMARY KEY, -- Mã: DK2604110001
    MaBN CHAR(10),
    NgayDangKy DATETIME DEFAULT GETDATE(),
    STT INT,
    HinhThucDangKy NVARCHAR(20) CHECK (HinhThucDangKy IN (N'Online', N'Offline')),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Chờ xử lý', N'Đã xác nhận', N'Hủy')),
    LyDo NVARCHAR(500),
    MaPhong INT,
    MaKhungGio INT
);

CREATE TABLE PHIEUKHAMBENH ( 
    MaPhieuKhamBenh VARCHAR(20) PRIMARY KEY, -- Mã: KB2604110001
    MaPhieuDK VARCHAR(20), 
    MaBN CHAR(10), 
    STT INT NULL,
    LyDoDenKham NVARCHAR(MAX),
    NgayLap DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Chờ thanh toán', N'Chờ cấp số', N'Chờ khám', N'Đang khám', N'Hoàn thành', N'Đã hủy')) DEFAULT N'Chờ thanh toán',
    TrieuChung NVARCHAR(MAX),
    KetLuan NVARCHAR(200),
    DanDo NVARCHAR(MAX),
    NgayTaiKham DATE,
    MaBacSiKham CHAR(10),
    MaPhong INT
);

CREATE TABLE PHIEUKHAMSANGLOC (
    MaPhieu VARCHAR(20) PRIMARY KEY, -- Mã: SL2604110001
    NgayKham DATETIME DEFAULT GETDATE(),
    ChieuCao DECIMAL(5,2) CHECK (ChieuCao > 30 AND ChieuCao < 250), 
    CanNang DECIMAL(5,2) CHECK (CanNang > 1 AND CanNang < 300),
    NhietDo DECIMAL(4,2) CHECK (NhietDo >= 30 AND NhietDo <= 45),
    Mach INT CHECK (Mach >= 20 AND Mach <= 250),
    HuyetApTamThu INT CHECK (HuyetApTamThu >= 40 AND HuyetApTamThu <= 250),
    HuyetApTamTruong INT CHECK (HuyetApTamTruong >= 30 AND HuyetApTamTruong <= 150),
    NhipTho INT CHECK (NhipTho >= 5 AND NhipTho <= 100),
    SpO2 INT CHECK (SpO2 >= 0 AND SpO2 <= 100),
    KetLuan NVARCHAR(50) CHECK (KetLuan IN (N'Bình thường', N'Bất thường', N'Đủ điều kiện', N'Không đủ điều kiện')),
    GhiChu NVARCHAR(MAX),
    MaBacSiKham CHAR(10),
    MaPhong INT,
    MaPhieuKhamBenh VARCHAR(20) UNIQUE
);

CREATE TABLE CHITIET_CHANDOAN ( 
    MaCTChanDoan VARCHAR(20) PRIMARY KEY, -- Mã: CD2604110001
    MaPhieuKhamBenh VARCHAR(20) NOT NULL,
    MaBenh CHAR(10) NOT NULL,
    LoaiBenh NVARCHAR(50) CHECK (LoaiBenh IN (N'Bệnh chính', N'Bệnh kèm theo')),
    KetLuanChiTiet NVARCHAR(200),
    GiaiDoan BIT NOT NULL DEFAULT 0 
);

CREATE TABLE PHIEU_CHIDINH (
    MaPhieuChiDinh VARCHAR(20) PRIMARY KEY, -- Mã: PC2604110001
    MaPhieuKhamBenh VARCHAR(20) NOT NULL, 
    MaBacSiChiDinh CHAR(10),     
    NgayChiDinh DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán', N'Đang thực hiện', N'Hoàn tất')) DEFAULT N'Chưa thanh toán',
    TongTien DECIMAL(18,2), 
    MaPhong INT
);

CREATE TABLE CHITIET_CHIDINH (
    MaCTChiDinh VARCHAR(20) PRIMARY KEY, -- Mã: CC2604110001
    MaPhieuChiDinh VARCHAR(20) NOT NULL,
    MaDV CHAR(10) NOT NULL, 
    DonGia DECIMAL(18,2), 
    MaBacSiThucHien CHAR(10), 
    KetQua NVARCHAR(MAX),     
    FileKetQua NVARCHAR(MAX), 
    ThoiGianCoKetQua TIME,
    TrangThai NVARCHAR(50) DEFAULT N'Chưa thực hiện'
        CHECK (TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện', N'Đã có kết quả'))
);


-- ======================================================================================
-- VI. KÊ ĐƠN THUỐC & QUẦY DƯỢC
-- ======================================================================================

CREATE TABLE DON_THUOC (
    MaDonThuoc VARCHAR(20) PRIMARY KEY, -- Mã: DT2604110001
    MaPhieuKhamBenh VARCHAR(20) NOT NULL UNIQUE, 
    NgayKe DATETIME DEFAULT GETDATE(),
    LoiDanBS NVARCHAR(MAX),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN (N'Chưa phát', N'Đã phát thuốc', N'Đã phát 1 phần', N'Đã hủy')) DEFAULT N'Chưa phát'
);

CREATE TABLE CT_DON_THUOC (
    MaCTDonThuoc VARCHAR(20) PRIMARY KEY, -- Mã: CTDT2604110001
    MaDonThuoc VARCHAR(20) NOT NULL,
    MaThuoc CHAR(20) NOT NULL,
    SoLuongSang DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongSang * 10) % 5 = 0),
    SoLuongTrua DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongTrua * 10) % 5 = 0),
    SoLuongChieu DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongChieu * 10) % 5 = 0),
    SoLuongToi DECIMAL(5,2) DEFAULT 0 CHECK ((SoLuongToi * 10) % 5 = 0),
    SoNgayDung INT DEFAULT 1 CHECK (SoNgayDung > 0),
    SoLuong INT NOT NULL, 
    SoLuongDaPhat INT DEFAULT 0, 
    DonViTinh NVARCHAR(20), 
    DonGia DECIMAL(18,2), 
    GhiChu NVARCHAR(200) 
);

CREATE TABLE PHIEU_PHAT_THUOC (
    MaPhieuPhat VARCHAR(20) PRIMARY KEY, -- Mã: PT2604110001
    MaDonThuoc VARCHAR(20) NOT NULL,           
    MaNV_Phat CHAR(10) NULL,
    MaHD VARCHAR(20), -- Khóa ngoại trỏ sang HOADON               
    MaPhong INT NULL,                 
    NgayPhat DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Hoàn thành' CHECK (TrangThai IN (N'Hoàn thành', N'Đã hủy')), 
    GhiChu NVARCHAR(200)               
);

CREATE TABLE CT_PHIEU_PHAT (
    MaCTPhieuPhat VARCHAR(20) PRIMARY KEY, -- Mã: CTPT2604110001
    MaPhieuPhat VARCHAR(20) NOT NULL,
    MaThuoc CHAR(20) NOT NULL,
    SoLuongPhat INT NOT NULL CHECK (SoLuongPhat > 0), 
    GhiChu NVARCHAR(200),
    MaLo NVARCHAR(50)
);


-- ======================================================================================
-- VII. TÀI CHÍNH / THU NGÂN
-- ======================================================================================

CREATE TABLE HOADON (
    MaHD VARCHAR(20) PRIMARY KEY, -- Mã: HD2604110001
    MaBN CHAR(10),
    MaPhieuKhamBenh VARCHAR(20), 
    NgayThanhToan DATETIME DEFAULT GETDATE(),
    TongTienGoc DECIMAL(18,2) DEFAULT 0,
    TongTienBHYTChiTra DECIMAL(18,2) DEFAULT 0,
    TongTienBenhNhanTra DECIMAL(18,2) DEFAULT 0,
    TrangThaiThanhToan NVARCHAR(50) NOT NULL 
        CHECK (TrangThaiThanhToan IN (N'Đã thanh toán', N'Chưa thanh toán', N'Đã hủy', N'Thanh toán 1 phần'))
        DEFAULT N'Chưa thanh toán',
    HinhThucThanhToan NVARCHAR(50) 
        CHECK (HinhThucThanhToan IN (N'Tiền mặt', N'Chuyển khoản', N'Thẻ', N'Quẹt thẻ', N'Không thu tiền')),
    GhiChu NVARCHAR(200),
    MaPhieuDK VARCHAR(20) NULL
);

CREATE TABLE CT_HOADON_DV (
    MaCTHD VARCHAR(20) PRIMARY KEY, -- Mã: CHDV2604110001
    MaHD VARCHAR(20) NOT NULL,
    MaDV CHAR(10),
    DonGia DECIMAL(18,2) NOT NULL,
    TongTienGoc DECIMAL(18,2) NOT NULL,        
    TienBHYTChiTra DECIMAL(18,2) DEFAULT 0,    
    TienBenhNhanTra DECIMAL(18,2) NOT NULL,    
    TrangThaiThanhToan NVARCHAR(50) DEFAULT N'Chưa thanh toán' 
        CHECK (TrangThaiThanhToan IN (N'Chưa thanh toán', N'Đã thanh toán', N'Hủy')),
    MaNV_ThuNgan CHAR(10) NULL 
);

CREATE TABLE CT_HOADON_THUOC (
    MaCTHD VARCHAR(20) PRIMARY KEY, -- Mã: CHTH2604110001
    MaHD VARCHAR(20) NOT NULL,
    MaCTDonThuoc VARCHAR(20) NOT NULL,
    SoLuong INT DEFAULT 1,
    TongTienGoc DECIMAL(18,2) NOT NULL,        
    TienBHYTChiTra DECIMAL(18,2) DEFAULT 0,    
    TienBenhNhanTra DECIMAL(18,2) NOT NULL,    
    TrangThaiThanhToan NVARCHAR(50) DEFAULT N'Chưa thanh toán' 
        CHECK (TrangThaiThanhToan IN (N'Chưa thanh toán', N'Đã thanh toán', N'Hủy')),
    MaNV_ThuNgan CHAR(10) NULL 
);


-- ======================================================================================
-- VIII. KHAI BÁO TẤT CẢ KHÓA NGOẠI (FOREIGN KEYS) BÊN DƯỚI CÙNG
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

-- 2. Dược & Nhập Kho
ALTER TABLE THUOC ADD CONSTRAINT FK_THUOC_NSX FOREIGN KEY (MaNSX) REFERENCES NHASANXUAT(MaNSX);
ALTER TABLE THUOC ADD CONSTRAINT FK_THUOC_DM FOREIGN KEY (MaLoaiThuoc) REFERENCES DANHMUC_THUOC(MaDanhMuc);

ALTER TABLE THANHPHAN_THUOC ADD CONSTRAINT FK_TP_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);
ALTER TABLE THANHPHAN_THUOC ADD CONSTRAINT FK_TP_HOATCHAT FOREIGN KEY (MaHoatChat) REFERENCES DANHMUC_HOATCHAT(MaHoatChat);

ALTER TABLE TONKHO ADD CONSTRAINT FK_TK_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);
ALTER TABLE TONKHO ADD CONSTRAINT FK_TONKHO_KHO FOREIGN KEY (MaKho) REFERENCES KHO(MaKho);

ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_NVLAP FOREIGN KEY (MaNV_LapPhieu) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_NVDUYET FOREIGN KEY (MaNV_Duyet) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_NSX FOREIGN KEY (MaNSX) REFERENCES NHASANXUAT(MaNSX);
ALTER TABLE PHIEUNHAP ADD CONSTRAINT FK_PN_KHO FOREIGN KEY (MaKho) REFERENCES KHO(MaKho);

ALTER TABLE CT_PHIEUNHAP ADD CONSTRAINT FK_CTPN_PN FOREIGN KEY (MaPhieuNhap) REFERENCES PHIEUNHAP(MaPhieuNhap);
ALTER TABLE CT_PHIEUNHAP ADD CONSTRAINT FK_CTPN_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);

-- 3. Khám bệnh & CLS
ALTER TABLE DICHVU ADD CONSTRAINT FK_DV_LOAI FOREIGN KEY (MaLoaiDV) REFERENCES LOAI_DICHVU(MaLoaiDV);
ALTER TABLE DICHVU ADD CONSTRAINT FK_DichVu_Khoa FOREIGN KEY (MaKhoa) REFERENCES KHOA(MaKhoa);

ALTER TABLE PHIEUDANGKY ADD CONSTRAINT FK_PDK_BNDK FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);
ALTER TABLE PHIEUDANGKY ADD CONSTRAINT FK_PDK_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);
ALTER TABLE PHIEUDANGKY ADD CONSTRAINT FK_PDK_KHUNGGIO FOREIGN KEY (MaKhungGio) REFERENCES DANHMUC_KHUNGGIO(MaKhungGio);

ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_PDK FOREIGN KEY (MaPhieuDK) REFERENCES PHIEUDANGKY(MaPhieuDK);
ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_BN FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);
ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_BS FOREIGN KEY (MaBacSiKham) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT FK_PKB_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);

ALTER TABLE PHIEUKHAMSANGLOC ADD CONSTRAINT FK_SangLoc_BacSi FOREIGN KEY (MaBacSiKham) REFERENCES NHANVIEN(MaNV); -- (Sửa lại BACSI thành NHANVIEN cho khớp khóa ngoại)
ALTER TABLE PHIEUKHAMSANGLOC ADD CONSTRAINT FK_SangLoc_Phong FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);
ALTER TABLE PHIEUKHAMSANGLOC ADD CONSTRAINT FK_SangLoc_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);

ALTER TABLE CHITIET_CHANDOAN ADD CONSTRAINT FK_CTCD_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);
ALTER TABLE CHITIET_CHANDOAN ADD CONSTRAINT FK_CTCD_BENH FOREIGN KEY (MaBenh) REFERENCES DANHMUC_BENH(MaBenh);

ALTER TABLE PHIEU_CHIDINH ADD CONSTRAINT FK_PCD_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);
ALTER TABLE PHIEU_CHIDINH ADD CONSTRAINT FK_PCD_BS FOREIGN KEY (MaBacSiChiDinh) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEU_CHIDINH ADD CONSTRAINT FK_PCD_PHONG FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);

ALTER TABLE CHITIET_CHIDINH ADD CONSTRAINT FK_CTCDI_PCD FOREIGN KEY (MaPhieuChiDinh) REFERENCES PHIEU_CHIDINH(MaPhieuChiDinh);
ALTER TABLE CHITIET_CHIDINH ADD CONSTRAINT FK_CTCDI_DV FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV);
ALTER TABLE CHITIET_CHIDINH ADD CONSTRAINT FK_CTCDI_BS FOREIGN KEY (MaBacSiThucHien) REFERENCES NHANVIEN(MaNV);

-- 4. Kê đơn & Quầy dược
ALTER TABLE DON_THUOC ADD CONSTRAINT FK_DT_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);

ALTER TABLE CT_DON_THUOC ADD CONSTRAINT FK_CTDT_DT FOREIGN KEY (MaDonThuoc) REFERENCES DON_THUOC(MaDonThuoc);
ALTER TABLE CT_DON_THUOC ADD CONSTRAINT FK_CTDT_THUOC FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);

ALTER TABLE PHIEU_PHAT_THUOC ADD CONSTRAINT FK_PhieuPhat_DonThuoc FOREIGN KEY (MaDonThuoc) REFERENCES DON_THUOC(MaDonThuoc);
ALTER TABLE PHIEU_PHAT_THUOC ADD CONSTRAINT FK_PhieuPhat_NhanVien FOREIGN KEY (MaNV_Phat) REFERENCES NHANVIEN(MaNV);
ALTER TABLE PHIEU_PHAT_THUOC ADD CONSTRAINT FK_PhieuPhat_Phong FOREIGN KEY (MaPhong) REFERENCES PHONG(MaPhong);
ALTER TABLE PHIEU_PHAT_THUOC ADD CONSTRAINT FK_PhieuPhat_HoaDon FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD);

ALTER TABLE CT_PHIEU_PHAT ADD CONSTRAINT FK_CTPhieuPhat_PhieuPhat FOREIGN KEY (MaPhieuPhat) REFERENCES PHIEU_PHAT_THUOC(MaPhieuPhat);
ALTER TABLE CT_PHIEU_PHAT ADD CONSTRAINT FK_CTPhieuPhat_Thuoc FOREIGN KEY (MaThuoc) REFERENCES THUOC(MaThuoc);

-- 5. Tài chính
ALTER TABLE HOADON ADD CONSTRAINT FK_HD_BN FOREIGN KEY (MaBN) REFERENCES BENHNHAN(MaBN);
ALTER TABLE HOADON ADD CONSTRAINT FK_HD_PKB FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh);
ALTER TABLE HOADON ADD CONSTRAINT FK_HD_PDK FOREIGN KEY (MaPhieuDK) REFERENCES PHIEUDANGKY(MaPhieuDK);

ALTER TABLE CT_HOADON_DV ADD CONSTRAINT FK_CTHDDV_NV FOREIGN KEY (MaNV_ThuNgan) REFERENCES NHANVIEN(MaNV);
ALTER TABLE CT_HOADON_DV ADD CONSTRAINT FK_CTHDDV_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD);
ALTER TABLE CT_HOADON_DV ADD CONSTRAINT FK_CTHDDV_DV FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV);

ALTER TABLE CT_HOADON_THUOC ADD CONSTRAINT FK_CTHDT_NV FOREIGN KEY (MaNV_ThuNgan) REFERENCES NHANVIEN(MaNV);
ALTER TABLE CT_HOADON_THUOC ADD CONSTRAINT FK_CTHDT_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD);
ALTER TABLE CT_HOADON_THUOC ADD CONSTRAINT FK_CTHDT_CTDT FOREIGN KEY (MaCTDonThuoc) REFERENCES CT_DON_THUOC(MaCTDonThuoc);
GO

CREATE PROCEDURE SP_XacNhanPhatThuoc

    @MaDonThuoc INT,

    @MaHD INT,

    @MaNV_Phat CHAR(10),

    @MaPhong INT

AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        -- 1. KIỂM TRA BẢO MẬT: Hóa đơn đã thanh toán chưa?
        IF NOT EXISTS (SELECT 1 FROM HOADON WHERE MaHD = @MaHD AND TrangThaiThanhToan = N'Đã thanh toán')
        BEGIN
            THROW 50001, N'Lỗi: Hóa đơn này chưa được thanh toán hoặc không tồn tại!', 1;
        END
        -- 2. TẠO PHIẾU PHÁT THUỐC TỔNG
        DECLARE @MaPhieuPhat INT;
        INSERT INTO PHIEU_PHAT_THUOC (MaDonThuoc, MaNV_Phat, MaHD, MaPhong, NgayPhat, TrangThai)
        VALUES (@MaDonThuoc, @MaNV_Phat, @MaHD, @MaPhong, GETDATE(), N'Hoàn thành');
        SET @MaPhieuPhat = SCOPE_IDENTITY(); -- Lấy ID vừa tạo
        -- 3. CHUẨN BỊ DUYỆT CÁC MÓN THUỐC KHÁCH "ĐÃ ĐÓNG TIỀN"
        DECLARE @MaThuoc CHAR(20), @SoLuongCanPhat INT, @MaCTDonThuoc INT;
		DECLARE cur_Thuoc CURSOR FOR
        SELECT cdt.MaThuoc, cdt.SoLuong, cdt.MaCTDonThuoc
        FROM CT_DON_THUOC cdt
        JOIN CT_HOADON_THUOC cht ON cdt.MaCTDonThuoc = cht.MaCTDonThuoc

        WHERE cdt.MaDonThuoc = @MaDonThuoc 

          AND cht.MaHD = @MaHD 

          AND cht.TrangThaiThanhToan = N'Đã thanh toán'; -- CHỈ LẤY MÓN ĐÃ MUA



        OPEN cur_Thuoc;

        FETCH NEXT FROM cur_Thuoc INTO @MaThuoc, @SoLuongCanPhat, @MaCTDonThuoc;



        WHILE @@FETCH_STATUS = 0

        BEGIN

            DECLARE @SoLuongDaLay INT = 0;

            DECLARE @SoLuongConThieu INT = @SoLuongCanPhat;



            -- 4. THUẬT TOÁN TRỪ KHO FIFO (Ưu tiên Lô có Hạn sử dụng gần nhất)

            DECLARE @MaTonKho INT, @TonLopNay INT;

            

            DECLARE cur_Kho CURSOR FOR

            SELECT MaTonKho, SoLuongTon 

            FROM TONKHO 

            WHERE MaThuoc = @MaThuoc AND SoLuongTon > 0 

            ORDER BY HanSuDung ASC; -- Sắp xếp ngày hết hạn tăng dần



            OPEN cur_Kho;

            FETCH NEXT FROM cur_Kho INTO @MaTonKho, @TonLopNay;



            WHILE @@FETCH_STATUS = 0 AND @SoLuongConThieu > 0

            BEGIN

                IF @TonLopNay >= @SoLuongConThieu

                BEGIN

                    -- Lô này đủ hàng: Trừ một nhát là xong

                    UPDATE TONKHO SET SoLuongTon = SoLuongTon - @SoLuongConThieu WHERE MaTonKho = @MaTonKho;

                    SET @SoLuongDaLay = @SoLuongDaLay + @SoLuongConThieu;

                    SET @SoLuongConThieu = 0;

                END

                ELSE

                BEGIN

                    -- Lô này KHÔNG ĐỦ: Vét sạch lô này (về 0) rồi tìm Lô tiếp theo

                    UPDATE TONKHO SET SoLuongTon = 0 WHERE MaTonKho = @MaTonKho;

                    SET @SoLuongDaLay = @SoLuongDaLay + @TonLopNay;

                    SET @SoLuongConThieu = @SoLuongConThieu - @TonLopNay;

                END

                FETCH NEXT FROM cur_Kho INTO @MaTonKho, @TonLopNay;

            END

            CLOSE cur_Kho;

            DEALLOCATE cur_Kho;



            -- Cảnh báo: Nếu quét hết các lô mà vẫn thiếu hàng -> Rollback toàn bộ!

            IF @SoLuongConThieu > 0

            BEGIN

                DECLARE @ErrMsg NVARCHAR(200) = N'Lỗi: Thuốc ' + RTRIM(@MaThuoc) + N' không đủ tồn kho để xuất. Còn thiếu: ' + CAST(@SoLuongConThieu AS NVARCHAR) + N' viên/chai.';

                THROW 50002, @ErrMsg, 1;

            END



            -- 5. GHI NHẬN CHI TIẾT PHIẾU PHÁT & CẬP NHẬT ĐƠN THUỐC

            INSERT INTO CT_PHIEU_PHAT (MaPhieuPhat, MaThuoc, SoLuongPhat)

            VALUES (@MaPhieuPhat, @MaThuoc, @SoLuongDaLay);



            UPDATE CT_DON_THUOC 

            SET SoLuongDaPhat = @SoLuongDaLay 

            WHERE MaCTDonThuoc = @MaCTDonThuoc;



            FETCH NEXT FROM cur_Thuoc INTO @MaThuoc, @SoLuongCanPhat, @MaCTDonThuoc;

        END

        CLOSE cur_Thuoc;

        DEALLOCATE cur_Thuoc;



        -- 6. CHỐT TRẠNG THÁI ĐƠN THUỐC (Đã phát 1 phần / Đã phát thuốc)

        DECLARE @TongSoMonBSKe INT, @TongSoMonDaPhat INT;

        

        -- Đếm số loại thuốc Bác sĩ kê

        SELECT @TongSoMonBSKe = COUNT(*) FROM CT_DON_THUOC WHERE MaDonThuoc = @MaDonThuoc;

        

        -- Đếm số loại thuốc Khách THỰC TẾ đã lấy

        SELECT @TongSoMonDaPhat = COUNT(*) FROM CT_DON_THUOC WHERE MaDonThuoc = @MaDonThuoc AND SoLuongDaPhat > 0;



        IF @TongSoMonDaPhat < @TongSoMonBSKe

        BEGIN

            UPDATE DON_THUOC SET TrangThai = N'Đã phát 1 phần' WHERE MaDonThuoc = @MaDonThuoc;

        END

        ELSE

        BEGIN

            UPDATE DON_THUOC SET TrangThai = N'Đã phát thuốc' WHERE MaDonThuoc = @MaDonThuoc;

        END



        -- NẾU MỌI THỨ TRƠN TRU -> LƯU VÀO DATABASE

        COMMIT TRANSACTION;

        SELECT 1 AS StatusCode, N'Phát thuốc thành công!' AS Message;



    END TRY

    BEGIN CATCH

        -- NẾU CÓ LỖI BẤT KỲ Ở BƯỚC NÀO -> HOÀN TÁC TOÀN BỘ (KHÔNG BỊ MẤT THUỐC OAN)

        ROLLBACK TRANSACTION;

        

        -- Dọn dẹp bộ nhớ nếu Cursor đang mở dở dang

        IF CURSOR_STATUS('global', 'cur_Kho') >= -1 DEALLOCATE cur_Kho;

        IF CURSOR_STATUS('global', 'cur_Thuoc') >= -1 DEALLOCATE cur_Thuoc;



        SELECT 0 AS StatusCode, ERROR_MESSAGE() AS Message;

    END CATCH

END


-- ===== MIGRATIONS / ADJUSTMENTS =====

-- =========================================================================
-- TASK 4: BO SUNG DU LIEU CON THIEU CHO BO BANG CHI DINH CLS
-- =========================================================================

-- 4.1 Dong bo TongTien cho PHIEU_CHIDINH theo tong chi tiet chi dinh.
UPDATE pc
SET pc.TongTien = ISNULL(x.TongTien, 0)
FROM PHIEU_CHIDINH pc
LEFT JOIN (
    SELECT MaPhieuChiDinh, SUM(ISNULL(DonGia, 0)) AS TongTien
    FROM CHITIET_CHIDINH
    GROUP BY MaPhieuChiDinh
) x ON x.MaPhieuChiDinh = pc.MaPhieuChiDinh;
GO

-- 4.2 Backfill MaBacSiThucHien cho ket qua da co (neu con null) theo bac si chi dinh.
UPDATE ct
SET ct.MaBacSiThucHien = pc.MaBacSiChiDinh
FROM CHITIET_CHIDINH ct
JOIN PHIEU_CHIDINH pc ON ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
WHERE ct.TrangThai = N'Đã có kết quả'
  AND ct.MaBacSiThucHien IS NULL
  AND pc.MaBacSiChiDinh IS NOT NULL;
GO

-- 4.3 Backfill FileKetQua placeholder cho ket qua da co (neu con null).
UPDATE ct
SET ct.FileKetQua = CONCAT(N'KQCLS_', ct.MaCTChiDinh, N'_BACKFILL.pdf')
FROM CHITIET_CHIDINH ct
WHERE ct.TrangThai = N'Đã có kết quả'
  AND (ct.FileKetQua IS NULL OR LTRIM(RTRIM(ct.FileKetQua)) = N'');
GO

-- 4.4 Chuan hoa text trang thai thanh toan de tranh loi lech ma hoa chuoi.
UPDATE CT_HOADON_DV
SET TrangThaiThanhToan = N'Đã thanh toán'
WHERE LOWER(TrangThaiThanhToan COLLATE Latin1_General_CI_AI) LIKE N'%thanh toan%';
GO

UPDATE CT_HOADON_THUOC
SET TrangThaiThanhToan = N'Đã thanh toán'
WHERE LOWER(TrangThaiThanhToan COLLATE Latin1_General_CI_AI) LIKE N'%thanh toan%';
GO

UPDATE HOADON
SET TrangThaiThanhToan = N'Đã thanh toán'
WHERE LOWER(TrangThaiThanhToan COLLATE Latin1_General_CI_AI) LIKE N'%thanh toan%';
GO

/*
    Migration cho luong CLS moi:
    - Bo bang cu KETQUA_CLS
    - Dam bao trang thai PHIEUKHAMBENH cho phep 2 gia tri moi:
      + N'Chờ cận lâm sàng'
      + N'Đã có kết quả CLS'
*/

-- 1) Neu con bang cu thi xoa
IF OBJECT_ID(N'dbo.KETQUA_CLS', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.KETQUA_CLS;
    PRINT N'Da xoa bang cu KETQUA_CLS';
END
ELSE
BEGIN
    PRINT N'Khong ton tai bang KETQUA_CLS - bo qua';
END
GO

-- 2) Drop toan bo CHECK constraint dang gan truc tiep vao cot PHIEUKHAMBENH.TrangThai
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql = @sql + N'ALTER TABLE dbo.PHIEUKHAMBENH DROP CONSTRAINT [' + cc.name + N'];' + CHAR(10)
FROM sys.check_constraints cc
JOIN sys.columns c
    ON c.object_id = cc.parent_object_id
   AND c.column_id = cc.parent_column_id
WHERE cc.parent_object_id = OBJECT_ID(N'dbo.PHIEUKHAMBENH')
  AND c.name = N'TrangThai';

IF (@sql <> N'')
BEGIN
    PRINT N'Dang drop CHECK constraint cu cua PHIEUKHAMBENH.TrangThai...';
    EXEC sp_executesql @sql;
END
ELSE
BEGIN
    PRINT N'Khong tim thay CHECK constraint cu tren PHIEUKHAMBENH.TrangThai';
END
GO

-- 3) Kiem tra du lieu hien tai truoc khi tao CHECK moi
IF EXISTS (
    SELECT 1
    FROM dbo.PHIEUKHAMBENH
    WHERE TrangThai NOT IN (
        N'Đã hủy',
        N'Hoàn thành',
        N'Đang khám',
        N'Chờ khám',
        N'Chờ cấp số',
        N'Chờ thanh toán',
        N'Chờ cận lâm sàng',
        N'Đã có kết quả CLS'
    )
)
BEGIN
    PRINT N'Co du lieu TrangThai khong hop le. Vui long xu ly du lieu truoc khi tao constraint moi:';
    SELECT DISTINCT TrangThai
    FROM dbo.PHIEUKHAMBENH
    WHERE TrangThai NOT IN (
        N'Đã hủy',
        N'Hoàn thành',
        N'Đang khám',
        N'Chờ khám',
        N'Chờ cấp số',
        N'Chờ thanh toán',
        N'Chờ cận lâm sàng',
        N'Đã có kết quả CLS'
    );

    THROW 50001, N'Du lieu TrangThai khong hop le, migration dung lai.', 1;
END
GO

-- 4) Tao CHECK constraint moi
IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_PHIEUKHAMBENH_TrangThai'
      AND parent_object_id = OBJECT_ID(N'dbo.PHIEUKHAMBENH')
)
BEGIN
    ALTER TABLE dbo.PHIEUKHAMBENH
    ADD CONSTRAINT CK_PHIEUKHAMBENH_TrangThai
    CHECK (
        TrangThai IN (
            N'Đã hủy',
            N'Hoàn thành',
            N'Đang khám',
            N'Chờ khám',
            N'Chờ cấp số',
            N'Chờ thanh toán',
            N'Chờ cận lâm sàng',
            N'Đã có kết quả CLS'
        )
    );

    PRINT N'Da tao CK_PHIEUKHAMBENH_TrangThai moi';
END
ELSE
BEGIN
    PRINT N'CK_PHIEUKHAMBENH_TrangThai da ton tai - bo qua';
END
GO

-- =========================================================================
-- TASK 3: DONG BO LUONG CLS -> THU NGAN -> PHONG CLS (du lieu ton dong)
-- =========================================================================

-- 3.1 Neu PKB dang o 'Cho can lam sang' nhung van con chi dinh CLS chua thanh toan,
--     day ve 'Cho thanh toan' de thu ngan thu phi.
UPDATE pkb
SET pkb.TrangThai = N'Chờ thanh toán'
FROM PHIEUKHAMBENH pkb
WHERE pkb.TrangThai = N'Chờ cận lâm sàng'
  AND EXISTS (
      SELECT 1
      FROM PHIEU_CHIDINH pc
      JOIN CHITIET_CHIDINH ct ON pc.MaPhieuChiDinh = ct.MaPhieuChiDinh
      WHERE pc.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
        AND ct.TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện')
        AND pc.TrangThai = N'Chưa thanh toán'
  );
GO

-- 3.2 Chi cap nhat PHIEU_CHIDINH sang 'Da thanh toan' khi co it nhat 1 DV CLS da thu tien.
--     Tranh day nham vao hang cho CLS neu dich vu bi huy/khong thu.
UPDATE pc
SET pc.TrangThai = N'Đã thanh toán'
FROM PHIEU_CHIDINH pc
WHERE pc.TrangThai = N'Chưa thanh toán'
  AND EXISTS (
      SELECT 1
      FROM CHITIET_CHIDINH ct
      JOIN HOADON hd ON hd.MaPhieuKhamBenh = pc.MaPhieuKhamBenh
      JOIN CT_HOADON_DV dv ON dv.MaHD = hd.MaHD AND dv.MaDV = ct.MaDV
      WHERE ct.MaPhieuChiDinh = pc.MaPhieuChiDinh
        AND dv.TrangThaiThanhToan = N'Đã thanh toán'
  );
GO

-- 3.3 Neu da thanh toan CLS va con chi tiet chua thuc hien -> PKB o 'Cho can lam sang'.
UPDATE pkb
SET pkb.TrangThai = N'Chờ cận lâm sàng'
FROM PHIEUKHAMBENH pkb
WHERE EXISTS (
    SELECT 1
    FROM PHIEU_CHIDINH pc
    JOIN CHITIET_CHIDINH ct ON pc.MaPhieuChiDinh = ct.MaPhieuChiDinh
    WHERE pc.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
      AND pc.TrangThai IN (N'Đã thanh toán', N'Đang thực hiện')
      AND ct.TrangThai IN (N'Chưa thực hiện', N'Đang thực hiện')
);
GO

-- =========================================================================
-- TASK 5: NANG CAP LUONG CLS MOI (MAU XET NGHIEM, LIS DA DONG)
-- =========================================================================

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'MauXetNghiem' AND Object_ID = Object_ID(N'CHITIET_CHIDINH'))
BEGIN
    ALTER TABLE CHITIET_CHIDINH ADD MauXetNghiem NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ChatLuongMau' AND Object_ID = Object_ID(N'CHITIET_CHIDINH'))
BEGIN
    ALTER TABLE CHITIET_CHIDINH ADD ChatLuongMau NVARCHAR(100) NULL;
END
GO
