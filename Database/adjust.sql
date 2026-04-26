USE QL_KhamBenhNgoaiTru;
GO

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

SELECT COUNT(*) AS TongSoThuoc FROM THUOC;
SELECT COUNT(*) AS TongSoDonThuoc FROM DON_THUOC;
SELECT COUNT(*) AS TongChiTietDon FROM CT_DON_THUOC;


SELECT COUNT(*) AS ToaThuocHopLe
FROM (
    SELECT MaDonThuoc 
    FROM CT_DON_THUOC 
    GROUP BY MaDonThuoc 
    HAVING COUNT(MaThuoc) > 1
) AS T;