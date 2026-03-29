-- Chạy vào Database QL_KhamChuaBenhNgoaiTru
-- Hãy DROP bảng KETQUA_CLS cũ nếu đã lỡ bấm Execute trước đó: 
-- DROP TABLE KETQUA_CLS;
CREATE TABLE KETQUA_CLS (
    MaKetQua INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKhamBenh INT NOT NULL,
    MaDV CHAR(10) NOT NULL, -- Cho khớp với bảng DICHVU
    TrangThai NVARCHAR(50) DEFAULT N'Chờ thực hiện',
    NoiDungKetQua NVARCHAR(MAX),
    NgayThucHien DATETIME,
    
    FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh),
    FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV)
);
GO

INSERT INTO KETQUA_CLS (MaPhieuKhamBenh, MaDV, TrangThai, NgayThucHien)
VALUES 
(1, 'DV01', N'Chờ thực hiện', NULL),
(1, 'DV02', N'Chờ thực hiện', NULL),
(2, 'DV03', N'Chờ thực hiện', NULL);
GO
-- =========================================================================
-- TASK 1: THANH TO�N T?NG PH?N (PARTIAL PAYMENT)
-- B?ng Bi�n Lai Thu Ti?n d?c l?p d? luu v?t thu t?ng ph?n (Kh�m, CLS, Thu?c)
-- =========================================================================
CREATE TABLE BIENLAI_THUTIEN (
    MaBienLai INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKhamBenh INT NOT NULL,
    LoaiBienLai INT NOT NULL, 
    TongTien DECIMAL(18,2) NOT NULL,
    NgayThu DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Chưa thanh toán',
    MaNV_ThuNgan CHAR(10),
    FOREIGN KEY (MaPhieuKhamBenh) REFERENCES PHIEUKHAMBENH(MaPhieuKhamBenh)
);
GO

-- =========================================================================
-- TASK 2: CẬP NHẬT TRẠNG THÁI PHIẾU KHÁM BỆNH CHO LUỒNG CẬN LÂM SÀNG
-- Check Constraint cũ (CK__PHIEUKHAM__Trang__0D7A0286) không cho phép giá trị "Chờ cận lâm sàng", 
-- điều này gây lỗi khi Bác sĩ lưu phiếu có chỉ định CLS.
-- =========================================================================
ALTER TABLE PHIEUKHAMBENH DROP CONSTRAINT CK__PHIEUKHAM__Trang__0D7A0286;
GO

ALTER TABLE PHIEUKHAMBENH ADD CONSTRAINT CK_PHIEUKHAMBENH_TrangThai CHECK (
    TrangThai IN (N'Đã hủy', N'Hoàn thành', N'Đang khám', N'Chờ khám', N'Chờ cấp số', N'Chờ thanh toán', N'Chờ cận lâm sàng', N'Đã có kết quả CLS')
);
GO
