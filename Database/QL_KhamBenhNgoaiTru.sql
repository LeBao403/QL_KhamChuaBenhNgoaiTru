USE QL_KhamBenhNgoaiTru;
GO

-- Insert 20 Danh mục bệnh (Mã ICD-10 cơ bản)
INSERT INTO DANHMUC_BENH (MaBenh, TenBenh)
VALUES 
    ('J00', N'Viêm mũi họng cấp (cảm lạnh)'),
    ('J01', N'Viêm xoang cấp'),
    ('J02', N'Viêm họng cấp'),
    ('J03', N'Viêm amidan cấp'),
    ('J04', N'Viêm thanh quản và khí quản cấp'),
    ('J20', N'Viêm phế quản cấp'),
    ('I10', N'Tăng huyết áp vô căn (nguyên phát)'),
    ('E11', N'Bệnh tim thiếu máu cục bộ'),
    ('E14', N'Đái tháo đường không phụ thuộc insulin (Type 2)'),
    ('K21', N'Trào ngược dạ dày - thực quản (GERD)'),
    ('K29', N'Viêm dạ dày và tá tràng'),
    ('K30', N'Hội chứng ruột kích thích (IBS)'),
    ('A09', N'Tiêu chảy và viêm dạ dày ruột do nhiễm khuẩn'),
    ('M10', N'Bệnh gút (Gout)'),
    ('M54', N'Thoái hóa khớp (Osteoarthritis)'),
    ('M15', N'Đau lưng dưới (Đau thắt lưng)'),
    ('L50', N'Viêm da dị ứng (Mày đay)'),
    ('H50', N'Viêm kết mạc (Đau mắt đỏ)'),
    ('H10', N'Viêm tai giữa'),
    ('N39', N'Viêm đường tiết niệu');
GO

PRINT N'Đã thêm thành công 20 danh mục bệnh!';