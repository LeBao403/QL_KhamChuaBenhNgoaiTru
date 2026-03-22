-- ===============================================================================
-- SCRIPT THÊM CHỨC VỤ "NHÂN VIÊN KHO" VÀ TÀI KHOẢN CHO NHÂN VIÊN KHO
-- (An toàn - không báo lỗi nếu đã tồn tại)
-- ===============================================================================
USE QL_KhamBenhNgoaiTru;
GO

-- 1. Thêm chức vụ "Nhân viên kho" (MaChucVu = 12)
IF NOT EXISTS (SELECT 1 FROM CHUCVU WHERE MaChucVu = 12)
BEGIN
    INSERT INTO CHUCVU (TenChucVu) VALUES (N'Nhân viên kho');
    PRINT N'1. Đã thêm chức vụ Nhân viên kho (ID=12)';
END
ELSE
BEGIN
    PRINT N'1. Chức vụ Nhân viên kho đã tồn tại (ID=12)';
END
GO

-- 2. Cập nhật 6 nhân viên Dược sĩ thành "Nhân viên kho" (MaChucVu = 12)
UPDATE NHANVIEN SET MaChucVu = 12 WHERE MaNV IN ('NV021','NV022','NV023','NV024','NV025','NV026') AND MaChucVu <> 12;
PRINT N'2. Đã cập nhật MaChucVu = 12 cho NV021-NV026';
GO

-- 3. Thêm tài khoản cho nhân viên kho (chỉ nếu chưa tồn tại)
IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE Username = 'kho001')
BEGIN
    INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) VALUES ('kho001', '123456', 1);
    PRINT N'3. Đã tạo tài khoản kho001';
END

IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE Username = 'kho002')
BEGIN
    INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) VALUES ('kho002', '123456', 1);
    PRINT N'3. Đã tạo tài khoản kho002';
END

IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE Username = 'kho003')
BEGIN
    INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) VALUES ('kho003', '123456', 1);
    PRINT N'3. Đã tạo tài khoản kho003';
END

IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE Username = 'kho004')
BEGIN
    INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) VALUES ('kho004', '123456', 1);
    PRINT N'3. Đã tạo tài khoản kho004';
END

IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE Username = 'kho005')
BEGIN
    INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) VALUES ('kho005', '123456', 1);
    PRINT N'3. Đã tạo tài khoản kho005';
END

IF NOT EXISTS (SELECT 1 FROM TAIKHOAN WHERE Username = 'kho006')
BEGIN
    INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive) VALUES ('kho006', '123456', 1);
    PRINT N'3. Đã tạo tài khoản kho006';
END
GO

-- 4. Cập nhật MaTK cho các nhân viên kho
UPDATE NHANVIEN SET MaTK = (SELECT TOP 1 MaTK FROM TAIKHOAN WHERE Username = 'kho001') WHERE MaNV = 'NV021' AND (MaTK IS NULL OR MaTK NOT IN (SELECT MaTK FROM TAIKHOAN WHERE Username LIKE 'kho%'));
UPDATE NHANVIEN SET MaTK = (SELECT TOP 1 MaTK FROM TAIKHOAN WHERE Username = 'kho002') WHERE MaNV = 'NV022' AND (MaTK IS NULL OR MaTK NOT IN (SELECT MaTK FROM TAIKHOAN WHERE Username LIKE 'kho%'));
UPDATE NHANVIEN SET MaTK = (SELECT TOP 1 MaTK FROM TAIKHOAN WHERE Username = 'kho003') WHERE MaNV = 'NV023' AND (MaTK IS NULL OR MaTK NOT IN (SELECT MaTK FROM TAIKHOAN WHERE Username LIKE 'kho%'));
UPDATE NHANVIEN SET MaTK = (SELECT TOP 1 MaTK FROM TAIKHOAN WHERE Username = 'kho004') WHERE MaNV = 'NV024' AND (MaTK IS NULL OR MaTK NOT IN (SELECT MaTK FROM TAIKHOAN WHERE Username LIKE 'kho%'));
UPDATE NHANVIEN SET MaTK = (SELECT TOP 1 MaTK FROM TAIKHOAN WHERE Username = 'kho005') WHERE MaNV = 'NV025' AND (MaTK IS NULL OR MaTK NOT IN (SELECT MaTK FROM TAIKHOAN WHERE Username LIKE 'kho%'));
UPDATE NHANVIEN SET MaTK = (SELECT TOP 1 MaTK FROM TAIKHOAN WHERE Username = 'kho006') WHERE MaNV = 'NV026' AND (MaTK IS NULL OR MaTK NOT IN (SELECT MaTK FROM TAIKHOAN WHERE Username LIKE 'kho%'));
PRINT N'4. Đã cập nhật MaTK cho NV021-NV026';
GO

PRINT '';
PRINT N'========================================';
PRINT N'Tài khoản đăng nhập: kho001 - kho006';
PRINT N'Mật khẩu: 123456';
PRINT N'========================================';
PRINT N'Xem danh sách tài khoản:';
SELECT MaTK, Username, IsActive FROM TAIKHOAN WHERE Username LIKE 'kho%';
PRINT N'Xem nhân viên kho:';
SELECT MaNV, HoTen, MaChucVu, MaTK FROM NHANVIEN WHERE MaNV IN ('NV021','NV022','NV023','NV024','NV025','NV026');
