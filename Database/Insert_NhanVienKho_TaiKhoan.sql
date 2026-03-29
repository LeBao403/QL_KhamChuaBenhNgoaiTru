USE QL_KhamBenhNgoaiTru;
GO

-- Xoá dữ liệu cũ nếu cần thiết (Cẩn thận khi chạy lệnh này trên DB thật)
-- DELETE FROM TAIKHOAN WHERE Username IN ('bs1', 'bs2', 'tt1', 'tt2', 'tn1', 'tn2', 'kho1', 'admin');

-- Insert danh sách tài khoản
INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive)
VALUES 
    -- Tài khoản Bác sĩ (bs)
    ('bs1', '1', 1),
    ('bs2', '1', 1),
    ('bs3', '1', 1),
    
    -- Tài khoản Tiếp tân (tt)
    ('tt1', '1', 1),
    ('tt2', '1', 1),
    
    -- Tài khoản Thu ngân (tn)
    ('tn1', '1', 1),
    ('tn2', '1', 1),
    
    -- Tài khoản Nhân viên kho/Dược sĩ (kho)
    ('kho1', '1', 1),
    ('kho2', '1', 1),
    
    -- Tài khoản Quản trị viên (admin)
    ('admin', '1', 1);
GO

PRINT N'Đã thêm thành công các tài khoản mẫu!';