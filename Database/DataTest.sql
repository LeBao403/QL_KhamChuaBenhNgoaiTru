USE QL_KhamBenhNgoaiTru;
GO


INSERT INTO DANHMUC_KHUNGGIO (TenKhungGio, GioiHanSoNguoi, TrangThai) VALUES 
(N'07:00 - 08:00', 20, 1),
(N'08:00 - 09:00', 25, 1), -- Giờ cao điểm cho lên 25 người
(N'09:00 - 10:00', 25, 1),
(N'10:00 - 11:00', 20, 1),
(N'13:00 - 14:00', 20, 1),
(N'14:00 - 15:00', 20, 1),
(N'15:00 - 16:00', 15, 1); -- Cuối ngày cho ít lại để bác sĩ về sớm :)
GO

INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive)
VALUES
('an.nv', '123456', 1),          -- NV0001 Nguyễn Văn An
('bich.tt', '123456', 1),        -- NV0002 Trần Thị Bích
('cuong.lv', '123456', 1),       -- NV0003 Lê Văn Cường
('dung.pt', '123456', 1),        -- NV0004 Phạm Thị Dung
('dong.hv', '123456', 1),        -- NV0005 Hoàng Văn Đông
('hanh.vt', '123456', 1),        -- NV0006 Võ Thị Hạnh
('hoa.dv', '123456', 1),         -- NV0007 Đặng Văn Hòa
('huong.nt', '123456', 1),       -- NV0008 Ngô Thị Hương
('khanh.bv', '123456', 1),       -- NV0009 Bùi Văn Khánh
('lan.dt', '123456', 1),         -- NV0010 Đỗ Thị Lan

('long.nv', '123456', 1),        -- NV0011 Nguyễn Văn Long
('mai.tt', '123456', 1),         -- NV0012 Trần Thị Mai
('manh.pv', '123456', 1),        -- NV0013 Phan Văn Mạnh
('minh.lt', '123456', 1),        -- NV0014 Lê Thị Minh
('nam.hv', '123456', 1),         -- NV0015 Hoàng Văn Nam
('ngoc.vt', '123456', 1),        -- NV0016 Vũ Thị Ngọc
('phat.dv', '123456', 1),        -- NV0017 Đinh Văn Phát
('phuong.nt', '123456', 1),      -- NV0018 Nguyễn Thị Phương
('quan.pv', '123456', 1),        -- NV0019 Phạm Văn Quân
('quynh.tt', '123456', 1),       -- NV0020 Trương Thị Quỳnh

('tram.db', '123456', 1),        -- NV0021 Đỗ Bảo Trâm
('quan.vv', '123456', 1),        -- NV0022 Võ Văn Quân
('lan.lt', '123456', 1),         -- NV0023 Lý Thị Lan
('tu.tt', '123456', 1),          -- NV0024 Trần Thanh Tú
('dung.hm', '123456', 1),        -- NV0025 Hoàng Mỹ Dung
('thinh.dq', '123456', 1),       -- NV0026 Đinh Quốc Thịnh
('hieu.nn', '123456', 1),        -- NV0027 Nguyễn Ngọc Hiếu
('phuc.th', '123456', 1),        -- NV0028 Trương Hồng Phúc
('linh.nt', '123456', 1),        -- NV0029 Nguyễn Thuỳ Linh
('vu.ph', '123456', 1),          -- NV0030 Phạm Hoàng Vũ

('quynh.ntq', '123456', 1),      -- NV0031 Ngô Thị Quỳnh
('tam.lm', '123456', 1),         -- NV0032 Lương Minh Tâm
('ngoc.tb', '123456', 1),        -- NV0033 Tạ Thị Bích Ngọc
('khoi.pv', '123456', 1),        -- NV0034 Phan Văn Khôi
('mai.th', '123456', 1),         -- NV0035 Trần Hồng Mai
('thanh.bv', '123456', 1),       -- NV0036 Bùi Văn Thành
('bao.ln', '123456', 1),         -- NV0037 Lê Ngọc Bảo
('huong.ht', '123456', 1),       -- NV0038 Huỳnh Thị Hương
('phu.dv', '123456', 1),         -- NV0039 Đỗ Văn Phú
('an.nk', '123456', 1),          -- NV0040 Nguyễn Khánh An

('thanh.vt', '123456', 1),       -- NV0041 Vũ Thị Thanh
('duc.nm', '123456', 1),         -- NV0042 Nguyễn Minh Đức
('tai.hv', '123456', 1),         -- NV0043 Hồ Văn Tài
('thuy.pt', '123456', 1),        -- NV0044 Phạm Thị Thuỷ
('viet.tq', '123456', 1),        -- NV0045 Trần Quốc Việt
('son.lh', '123456', 1),         -- NV0046 Lê Hồng Sơn
('cuc.nt', '123456', 1),         -- NV0047 Nguyễn Thị Cúc
('thang.vv', '123456', 1),       -- NV0048 Võ Văn Thắng
('hong.nt', '123456', 1),        -- NV0049 Nguyễn Thị Hồng
('lam.pv', '123456', 1),         -- NV0050 Phan Văn Lâm

('hatracngon', '123456', 1),  -- NV051
('lamvinhhai', '123456', 1),   -- NV052
('phamngocvananh', '123456', 1), -- NV053
('lequanbao', '123456', 1),    -- NV054
('dinhdoquynhnhu', '123456', 1), -- NV055
('phamxuanmanh', '123456', 1), -- NV056
('phamngocquynh', '123456', 1), -- NV057
('nguyenminhtam', '123456', 1),   -- NV058
('nguyenngochan', '123456', 1),   -- NV059
('nguyenminhquoc', '123456', 1),  -- NV060

('nguyenphinhung', '123456', 1),  -- NV061
('nguyenminhha', '123456', 1),    -- NV062
('nguyenngocchau', '123456', 1),  -- NV063
('nguyenminhhoang', '123456', 1);  -- NV064

INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive)
VALUES
(N'nguyet.lta', N'123456', 1), -- 115 (NV065 - KSL CS1)
(N'quan.tm', N'123456', 1),   -- 116 (NV066 - Tiem CS1)
(N'thao.ht', N'123456', 1),   -- 117 (NV067 - KSL CS2)
(N'trung.dq', N'123456', 1),  -- 118 (NV068 - Tiem CS2)
(N'anh.bp', N'123456', 1),    -- 119 (NV069 - KSL CS3)
(N'thang.vd', N'123456', 1),  -- 120 (NV070 - Tiem CS3)
(N'huong.mtl', N'123456', 1), -- 121 (NV071 - KSL CS4)
(N'tuan.pa', N'123456', 1),   -- 122 (NV072 - Tiem CS4)
(N'diep.tn', N'123456', 1),   -- 123 (NV073 - KSL CS5)
(N'hieu.tm', N'123456', 1),   -- 124 (NV074 - Tiem CS5)
(N'trang.nk', N'123456', 1),  -- 125 (NV075 - KSL CS6)
(N'kiet.ht', N'123456', 1),   -- 126 (NV076 - Tiem CS6)
(N'my.vh', N'123456', 1),     -- 127 (NV077 - KSL CS7)
(N'vinh.lt', N'123456', 1),   -- 128 (NV078 - Tiem CS7)
(N'linh.dm', N'123456', 1),   -- 129 (NV079 - KSL CS8)
(N'bao.ng', N'123456', 1),    -- 130 (NV080 - Tiem CS8)
(N'hang.lt', N'123456', 1),   -- 131 (NV081 - KSL CS9)
(N'quang.td', N'123456', 1),  -- 132 (NV082 - Tiem CS9)
(N'anh.cd', N'123456', 1),    -- 133 (NV083 - KSL CS10)
(N'long.th', N'123456', 1),   -- 134 (NV084 - Tiem CS10)
(N'ngoc.hb', N'123456', 1),   -- 135 (NV085 - KSL CS11)
(N'khoi.dm', N'123456', 1);   -- 136 (NV086 - Tiem CS11)
GO

-- KHÁCH HÀNG (Username = SDT)
INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive)
VALUES
('0908000001', '123456', 1),      -- Nguyễn Văn Hùng
('0908000002', '123456', 1),      -- Trần Thị Lan
('0908000003', '123456', 1),      -- Lê Văn Minh
('0908000004', '123456', 1),      -- Phạm Thị Hoa
('0908000005', '123456', 1),      -- Hoàng Văn Tuấn
('0908000006', '123456', 1),      -- Võ Thị Hạnh
('0908000007', '123456', 1),      -- Đặng Văn Long
('0908000008', '123456', 1),      -- Ngô Thị Mai
('0908000009', '123456', 1),      -- Bùi Văn Khánh
('0908000010', '123456', 1),      -- Đỗ Thị Yến

('0908000011', '123456', 1),      -- Nguyễn Thị Hương
('0908000012', '123456', 1),      -- Trần Văn Nam
('0908000013', '123456', 1),      -- Lê Thị Thu
('0908000014', '123456', 1),      -- Phạm Văn Hoàng
('0908000015', '123456', 1),      -- Hoàng Thị Mai
('0908000016', '123456', 1),      -- Vũ Văn Dũng
('0908000017', '123456', 1),      -- Đinh Thị Hòa
('0908000018', '123456', 1),      -- Nguyễn Văn Lâm
('0908000019', '123456', 1),      -- Trần Thị Ngọc
('0908000020', '123456', 1),      -- Lê Văn Thành

('0915789456', '123456', 1),      -- Đoàn Minh Phúc
('0902349876', '123456', 1),      -- Lê Hải Yến
('0984563217', '123456', 1),      -- Trần Nhật Nam
('0932145896', '123456', 1),      -- Nguyễn Thị Mỹ Duyên
('0916325478', '123456', 1),      -- Phạm Văn Khánh
('0978456321', '123456', 1),      -- Hoàng Lan Chi
('0931458796', '123456', 1),      -- Nguyễn Quang Huy
('0907856321', '123456', 1),      -- Lý Thị Bảo Ngọc
('0917458963', '123456', 1),      -- Phan Hoàng Long
('0932147856', '123456', 1),      -- Vũ Thu Trang

('0987456321', '123456', 1),      -- Nguyễn Hữu Toàn
('0978562143', '123456', 1),      -- Trịnh Mai Phương
('0912348756', '123456', 1),      -- Tạ Văn Cường
('0907854213', '123456', 1),      -- Nguyễn Kim Oanh
('0914789562', '123456', 1),      -- Đỗ Quang Minh
('0932654789', '123456', 1),      -- Lưu Thanh Hà
('0907896541', '123456', 1),      -- Ngô Văn An
('0978456213', '123456', 1),      -- Trần Thị Thu Hằng
('0912354789', '123456', 1),      -- Nguyễn Đức Mạnh
('0932145879', '123456', 1),      -- Phạm Thị Hòa

('0914785963', '123456', 1),      -- Nguyễn Văn Thắng
('0901452369', '123456', 1),      -- Trần Ngọc Ánh
('0987451236', '123456', 1),      -- Lê Minh Quân
('0932147852', '123456', 1),      -- Nguyễn Thanh Mai
('0912564789', '123456', 1),      -- Phan Văn Đức
('0903248756', '123456', 1),      -- Hồ Thị Yến Nhi
('0978563214', '123456', 1),      -- Nguyễn Văn Hải
('0914785632', '123456', 1),      -- Đinh Thị Thảo
('0902147856', '123456', 1),      -- Võ Quang Khải
('0987456325', '123456', 1);      -- Lê Mỹ Linh

-- KHÁCH HÀNG (Username = SDT)
INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive)
VALUES
('0383156689', '1', 1),      -- Lê Quân Bảo
('0976865715', '1', 1),      -- Đinh Đỗ Quỳnh Như
('0978610519', '1', 1);      -- Lại Phước Thịnh





-- ===========================================================================================================================
---------------------------------------------------- THÔNG TIN KHÁCH HÀNG ----------------------------------------------------
-- ===========================================================================================================================

-- ================== NGƯỜI LỚN ==================

INSERT INTO BENHNHAN (MaBN, HoTen, NgaySinh, GioiTinh, CCCD, SDT, Email, DiaChi, BHYT, MaTK, SoTheBHYT, HanSuDungBHYT, TuyenKham, MucHuongBHYT)
VALUES
-- NGƯỜI LỚN
('KH0001', N'Nguyễn Văn Hùng', '1985-03-12', N'Nam', '012345678901', '0908000001', 'hung.nguyen@vnvc.vn', N'Quận 1, TP.HCM', 1, 87, 'DN4018273645001', '2026-12-31', N'Đúng tuyến', 80),
('KH0002', N'Trần Thị Lan', '1990-07-22', N'Nữ', '012345678902', '0908000002', 'lan.tran@vnvc.vn', N'Quận Hai Bà Trưng, Hà Nội', 1, 88, 'HC4018273645002', '2026-10-15', N'Trái tuyến', 80),
('KH0003', N'Lê Văn Minh', '1982-11-10', N'Nam', '012345678903', '0908000003', 'minh.le@vnvc.vn', N'Hải Châu, Đà Nẵng', 0, 89, NULL, NULL, NULL, NULL),
('KH0004', N'Phạm Thị Hoa', '1995-05-09', N'Nữ', '012345678904', '0908000004', 'hoa.pham@vnvc.vn', N'Bình Thủy, Cần Thơ', 1, 90, 'DN4018273645004', '2027-01-20', N'Đúng tuyến', 80),
('KH0005', N'Hoàng Văn Tuấn', '1989-09-15', N'Nam', '012345678905', '0908000005', 'tuan.hoang@vnvc.vn', N'Ngô Quyền, Hải Phòng', 0, 91, NULL, NULL, NULL, NULL),
('KH0006', N'Võ Thị Hạnh', '1987-12-01', N'Nữ', '012345678906', '0908000006', 'hanh.vo@vnvc.vn', N'TP. Vinh, Nghệ An', 1, 92, 'CB4018273645006', '2026-11-30', N'Đúng tuyến', 95),
('KH0007', N'Đặng Văn Long', '1984-04-18', N'Nam', '012345678907', '0908000007', 'long.dang@vnvc.vn', N'Thủ Dầu Một, Bình Dương', 0, 93, NULL, NULL, NULL, NULL),
('KH0008', N'Ngô Thị Mai', '1991-06-25', N'Nữ', '012345678908', '0908000008', 'mai.ngo@vnvc.vn', N'TP. Nha Trang, Khánh Hòa', 1, 94, 'DN4018273645008', '2026-09-01', N'Trái tuyến', 80),
('KH0009', N'Bùi Văn Khánh', '1986-08-30', N'Nam', '012345678909', '0908000009', 'khanh.bui@vnvc.vn', N'TP. Huế, Thừa Thiên Huế', 0, 95, NULL, NULL, NULL, NULL),
('KH0010', N'Đỗ Thị Yến', '1993-01-19', N'Nữ', '012345678910', '0908000010', 'yen.do@vnvc.vn', N'TP. Biên Hòa, Đồng Nai', 1, 96, 'DN4018273645010', '2027-05-15', N'Đúng tuyến', 80),

('KH0011', N'Nguyễn Thị Hương', '1988-06-14', N'Nữ', '012345678911', '0908000011', 'huong.nguyen@vnvc.vn', N'Nam Từ Liêm, Hà Nội', 1, 97, 'HC4018273645011', '2026-12-31', N'Đúng tuyến', 100),
('KH0012', N'Trần Văn Nam', '1981-02-21', N'Nam', '012345678912', '0908000012', 'nam.tran@vnvc.vn', N'Quận 3, TP.HCM', 0, 98, NULL, NULL, NULL, NULL),
('KH0013', N'Lê Thị Thu', '1996-08-09', N'Nữ', '012345678913', '0908000013', 'thu.le@vnvc.vn', N'Quận 7, TP.HCM', 1, 99, 'DN4018273645013', '2026-11-11', N'Trái tuyến', 80),
('KH0014', N'Phạm Văn Hoàng', '1983-04-27', N'Nam', '012345678914', '0908000014', 'hoang.pham@vnvc.vn', N'Hồng Bàng, Hải Phòng', 1, 100, 'GD4018273645014', '2027-08-20', N'Đúng tuyến', 80),
('KH0015', N'Hoàng Thị Mai', '1989-10-13', N'Nữ', '012345678915', '0908000015', 'mai.hoang@vnvc.vn', N'Thanh Khê, Đà Nẵng', 0, 101, NULL, NULL, NULL, NULL),
('KH0016', N'Vũ Văn Dũng', '1986-07-19', N'Nam', '012345678916', '0908000016', 'dung.vu@vnvc.vn', N'Thủ Đức, TP.HCM', 1, 102, 'DN4018273645016', '2026-10-25', N'Đúng tuyến', 80),
('KH0017', N'Đinh Thị Hòa', '1992-03-05', N'Nữ', '012345678917', '0908000017', 'hoa.dinh@vnvc.vn', N'Hồng Bàng, Hải Phòng', 0, 103, NULL, NULL, NULL, NULL),
('KH0018', N'Nguyễn Văn Lâm', '1987-09-11', N'Nam', '012345678918', '0908000018', 'lam.nguyen@vnvc.vn', N'Cẩm Lệ, Đà Nẵng', 1, 104, 'CB4018273645018', '2026-12-31', N'Đúng tuyến', 100),
('KH0019', N'Trần Thị Ngọc', '1994-05-24', N'Nữ', '012345678919', '0908000019', 'ngoc.tran@vnvc.vn', N'Cầu Giấy, Hà Nội', 0, 105, NULL, NULL, NULL, NULL),
('KH0020', N'Lê Văn Thành', '1985-12-30', N'Nam', '012345678920', '0908000020', 'thanh.le@vnvc.vn', N'Huế', 1, 106, 'DN4018273645020', '2027-02-28', N'Trái tuyến', 80),

('KH0021', N'Đoàn Minh Phúc', '1985-11-20', N'Nam', '026985001234', '0915789456', 'minhphuc.doan@example.com', N'25 Lê Lợi, Hà Nội', 1, 107, 'HC4018273645021', '2026-12-31', N'Đúng tuyến', 80),
('KH0022', N'Lê Hải Yến', '1991-04-10', N'Nữ', '034896752134', '0902349876', 'haiyen.le@example.com', N'88 Nguyễn Văn Cừ, Bắc Ninh', 0, 108, NULL, NULL, NULL, NULL),
('KH0023', N'Trần Nhật Nam', '1978-07-05', N'Nam', '021548963215', '0984563217', 'nhatnam.tran@example.com', N'15 Lạch Tray, Hải Phòng', 1, 109, 'GD4018273645023', '2027-04-15', N'Trái tuyến', 80),
('KH0024', N'Nguyễn Thị Mỹ Duyên', '1989-12-12', N'Nữ', '025489631254', '0932145896', 'myduyen.nguyen@example.com', N'42 Trần Quang Diệu, Hà Nội', 0, 110, NULL, NULL, NULL, NULL),
('KH0025', N'Phạm Văn Khánh', '1993-09-14', N'Nam', '028945632541', '0916325478', 'vankhanh.pham@example.com', N'19 Hùng Vương, Đà Nẵng', 1, 111, 'DN4018273645025', '2026-12-31', N'Đúng tuyến', 95),
('KH0026', N'Hoàng Lan Chi', '1986-06-18', N'Nữ', '037894562315', '0978456321', 'lanchi.hoang@example.com', N'67 Nguyễn Chí Thanh, Hà Nội', 0, 112, NULL, NULL, NULL, NULL),
('KH0027', N'Nguyễn Quang Huy', '1990-02-27', N'Nam', '022145896321', '0931458796', 'quanghuy.nguyen@example.com', N'8 Đường 30/4, Cần Thơ', 1, 113, 'CB4018273645027', '2027-07-20', N'Đúng tuyến', 80),
('KH0028', N'Lý Thị Bảo Ngọc', '1992-05-22', N'Nữ', '029875463215', '0907856321', 'baongoc.ly@example.com', N'33 Võ Thị Sáu, TP.HCM', 0, 114, NULL, NULL, NULL, NULL),
('KH0029', N'Phan Hoàng Long', '1981-03-03', N'Nam', '027894562317', '0917458963', 'hoanglong.phan@example.com', N'12 Phan Bội Châu, Đà Nẵng', 1, 115, 'DN4018273645029', '2026-11-30', N'Đúng tuyến', 100),
('KH0030', N'Vũ Thu Trang', '1995-08-28', N'Nữ', '024785693214', '0932147856', 'thutrang.vu@example.com', N'55 Nguyễn Du, Hà Nội', 0, 116, NULL, NULL, NULL, NULL),

('KH0031', N'Nguyễn Hữu Toàn', '1979-01-10', N'Nam', '023654987521', '0987456321', 'huutoan.nguyen@example.com', N'9 Phạm Văn Đồng, TP.HCM', 1, 117, 'DN4018273645031', '2026-12-31', N'Đúng tuyến', 80),
('KH0032', N'Trịnh Mai Phương', '1987-10-17', N'Nữ', '035214789652', '0978562143', 'maiphuong.trinh@example.com', N'21 Nguyễn Trãi, Hà Nội', 0, 118, NULL, NULL, NULL, NULL),
('KH0033', N'Tạ Văn Cường', '1984-04-21', N'Nam', '038965214785', '0912348756', 'vancuong.ta@example.com', N'18 Lê Lợi, Thanh Hóa', 1, 119, 'HC4018273645033', '2027-10-10', N'Đúng tuyến', 95),
('KH0034', N'Nguyễn Kim Oanh', '1991-09-19', N'Nữ', '022154789632', '0907854213', 'kimoanh.nguyen@example.com', N'70 Cách Mạng Tháng 8, TP.HCM', 0, 120, NULL, NULL, NULL, NULL),
('KH0035', N'Đỗ Quang Minh', '1983-06-25', N'Nam', '021478965321', '0914789562', 'quangminh.do@example.com', N'32 Hoàng Hoa Thám, Hà Nội', 1, 121, 'DN4018273645035', '2026-12-31', N'Trái tuyến', 80),
('KH0036', N'Lưu Thanh Hà', '1988-07-07', N'Nữ', '026321547896', '0932654789', 'thanhha.luu@example.com', N'11 Ngô Quyền, Hải Phòng', 0, 122, NULL, NULL, NULL, NULL),
('KH0037', N'Ngô Văn An', '1990-05-05', N'Nam', '024789632154', '0907896541', 'vann.an@example.com', N'23 Nguyễn Văn Linh, Đà Nẵng', 1, 123, 'GD4018273645037', '2027-01-31', N'Đúng tuyến', 80),
('KH0038', N'Trần Thị Thu Hằng', '1992-11-15', N'Nữ', '027896321547', '0978456213', 'thuhang.tran@example.com', N'101 Hai Bà Trưng, Hà Nội', 0, 124, NULL, NULL, NULL, NULL),
('KH0039', N'Nguyễn Đức Mạnh', '1985-12-30', N'Nam', '025478963215', '0912354789', 'ducmanh.nguyen@example.com', N'14 Điện Biên Phủ, TP.HCM', 1, 125, 'DN4018273645039', '2026-12-31', N'Đúng tuyến', 80),
('KH0040', N'Phạm Thị Hòa', '1989-03-08', N'Nữ', '021547896325', '0932145879', 'thihoa.pham@example.com', N'7 Nguyễn Huệ, Huế', 0, 126, NULL, NULL, NULL, NULL),

('KH0041', N'Nguyễn Văn Thắng', '1986-09-12', N'Nam', '023654789521', '0914785963', 'vanthang.nguyen@example.com', N'5 Hoàng Văn Thụ, Hà Nội', 1, 127, 'DN4018273645041', '2026-08-15', N'Trái tuyến', 80),
('KH0042', N'Trần Ngọc Ánh', '1993-01-29', N'Nữ', '029875641325', '0901452369', 'ngocanh.tran@example.com', N'77 Bạch Đằng, Đà Nẵng', 0, 128, NULL, NULL, NULL, NULL),
('KH0043', N'Lê Minh Quân', '1980-02-02', N'Nam', '028974563215', '0987451236', 'minhquan.le@example.com', N'28 Trần Hưng Đạo, TP.HCM', 1, 129, 'CB4018273645043', '2027-11-20', N'Đúng tuyến', 100),
('KH0044', N'Nguyễn Thanh Mai', '1987-08-18', N'Nữ', '025478963214', '0932147852', 'thanhmai.nguyen@example.com', N'44 Nguyễn Khuyến, Hà Nội', 0, 130, NULL, NULL, NULL, NULL),
('KH0045', N'Phan Văn Đức', '1982-05-09', N'Nam', '026325478965', '0912564789', 'vanduc.phan@example.com', N'35 Nguyễn Thị Minh Khai, Huế', 1, 131, 'DN4018273645045', '2026-12-31', N'Đúng tuyến', 80),
('KH0046', N'Hồ Thị Yến Nhi', '1994-04-14', N'Nữ', '037854621354', '0903248756', 'yennhi.ho@example.com', N'10 Bùi Thị Xuân, Đà Nẵng', 0, 132, NULL, NULL, NULL, NULL),
('KH0047', N'Nguyễn Văn Hải', '1983-03-30', N'Nam', '021478563219', '0978563214', 'vanhai.nguyen@example.com', N'60 Lý Thường Kiệt, Hà Nội', 1, 133, 'HC4018273645047', '2027-03-01', N'Đúng tuyến', 80),
('KH0048', N'Đinh Thị Thảo', '1988-10-22', N'Nữ', '024789653214', '0914785632', 'thithao.dinh@example.com', N'17 Trần Cao Vân, TP.HCM', 0, 134, NULL, NULL, NULL, NULL),
('KH0049', N'Võ Quang Khải', '1981-06-06', N'Nam', '023698745123', '0902147856', 'quangkhai.vo@example.com', N'19 Nguyễn Công Trứ, Đà Nẵng', 1, 135, 'DN4018273645049', '2026-12-31', N'Đúng tuyến', 95),
('KH0050', N'Lê Mỹ Linh', '1990-12-19', N'Nữ', '022541236985', '0987456325', 'mylinh.le@example.com', N'82 Nguyễn Trường Tộ, Hà Nội', 0, 136, NULL, NULL, NULL, NULL),

-- 20 TRẺ EM (Trẻ em thường có mức hưởng 100%)
('KH0051', N'Nguyễn Anh Tuấn', '2015-04-10', N'Nam', NULL, NULL, NULL, N'Quận 1, TP.HCM', 1, NULL, 'TE1018273645051', '2026-12-31', N'Đúng tuyến', 100),
('KH0052', N'Trần Ngọc Bích', '2017-08-22', N'Nữ', NULL, NULL, NULL, N'Hà Đông, Hà Nội', 0, NULL, NULL, NULL, NULL, NULL),
('KH0053', N'Lê Đức Huy', '2014-12-30', N'Nam', NULL, NULL, NULL, N'Hải Châu, Đà Nẵng', 1, NULL, 'TE1018273645053', '2026-12-31', N'Đúng tuyến', 100),
('KH0054', N'Phạm Thảo Nhi', '2016-03-05', N'Nữ', NULL, NULL, NULL, N'Ngô Quyền, Hải Phòng', 0, NULL, NULL, NULL, NULL, NULL),
('KH0055', N'Hoàng Gia Bảo', '2013-11-15', N'Nam', NULL, NULL, NULL, N'Bình Thủy, Cần Thơ', 1, NULL, 'TE1018273645055', '2027-06-30', N'Đúng tuyến', 100),
('KH0056', N'Võ Minh Quân', '2018-01-25', N'Nam', NULL, NULL, NULL, N'TP. Vinh, Nghệ An', 0, NULL, NULL, NULL, NULL, NULL),
('KH0057', N'Đặng Khánh Linh', '2019-09-07', N'Nữ', NULL, NULL, NULL, N'Thủ Dầu Một, Bình Dương', 1, NULL, 'TE1018273645057', '2026-12-31', N'Trái tuyến', 100),
('KH0058', N'Ngô Hoài Nam', '2014-02-18', N'Nam', NULL, NULL, NULL, N'TP. Nha Trang, Khánh Hòa', 0, NULL, NULL, NULL, NULL, NULL),
('KH0059', N'Bùi Thanh Thảo', '2016-10-21', N'Nữ', NULL, NULL, NULL, N'TP. Huế, Thừa Thiên Huế', 1, NULL, 'TE1018273645059', '2027-12-31', N'Đúng tuyến', 100),
('KH0060', N'Đỗ Minh Anh', '2012-07-14', N'Nam', NULL, NULL, NULL, N'TP. Biên Hòa, Đồng Nai', 0, NULL, NULL, NULL, NULL, NULL),

('KH0061', N'Nguyễn Phương Thảo', '2015-06-02', N'Nữ', NULL, NULL, NULL, N'Quận 5, TP.HCM', 1, NULL, 'TE1018273645061', '2026-12-31', N'Đúng tuyến', 100),
('KH0062', N'Trần Hoàng Nam', '2013-11-09', N'Nam', NULL, NULL, NULL, N'Nam Từ Liêm, Hà Nội', 0, NULL, NULL, NULL, NULL, NULL),
('KH0063', N'Lê Khánh Vy', '2017-02-25', N'Nữ', NULL, NULL, NULL, N'Hải Châu, Đà Nẵng', 1, NULL, 'TE1018273645063', '2026-12-31', N'Đúng tuyến', 100),
('KH0064', N'Phạm Anh Dũng', '2014-08-19', N'Nam', NULL, NULL, NULL, N'Ngô Quyền, Hải Phòng', 0, NULL, NULL, NULL, NULL, NULL),
('KH0065', N'Hoàng Bảo Ngọc', '2016-12-28', N'Nữ', NULL, NULL, NULL, N'Bình Thủy, Cần Thơ', 1, NULL, 'TE1018273645065', '2027-12-31', N'Đúng tuyến', 100),
('KH0066', N'Võ Hữu Tài', '2015-10-10', N'Nam', NULL, NULL, NULL, N'TP. Vinh, Nghệ An', 0, NULL, NULL, NULL, NULL, NULL),
('KH0067', N'Đặng Ngọc Hân', '2018-04-23', N'Nữ', NULL, NULL, NULL, N'Thủ Dầu Một, Bình Dương', 1, NULL, 'TE1018273645067', '2026-12-31', N'Đúng tuyến', 100),
('KH0068', N'Ngô Minh Đức', '2013-01-07', N'Nam', NULL, NULL, NULL, N'TP. Nha Trang, Khánh Hòa', 0, NULL, NULL, NULL, NULL, NULL),
('KH0069', N'Bùi Hồng Anh', '2017-09-17', N'Nữ', NULL, NULL, NULL, N'TP. Huế, Thừa Thiên Huế', 1, NULL, 'TE1018273645069', '2026-12-31', N'Trái tuyến', 100),
('KH0070', N'Đỗ Quốc Khánh', '2014-05-12', N'Nam', NULL, NULL, NULL, N'TP. Biên Hòa, Đồng Nai', 0, NULL, NULL, NULL, NULL, NULL),

-- NHÓM TUỔI TEEN (Tài khoản User)
('KH0071', N'Lê Quân Bảo', '2005-03-04', N'Nam', NULL, '0383156689', 'baocenzo@gmail.com', N'TP. Huế, Thừa Thiên Huế', 1, 137, 'HS4018273645071', '2026-12-31', N'Đúng tuyến', 80),
('KH0072', N'Đinh Đỗ Quỳnh Như', '2004-09-01', N'Nữ', NULL, '0976865715', 'dinhdoquynhnhuss@gmail.com', N'TP. Biên Hòa, Đồng Nai', 0, 138, NULL, NULL, NULL, NULL),
('KH0073', N'Lại Phước Thịnh', '2004-09-01', N'Nam', NULL, '0978610519', 'laiphuocthinh.bp@gmail.com', N'TP. Biên Hòa, Đồng Nai', 1, 139, 'HS4018273645073', '2026-12-31', N'Đúng tuyến', 80);


INSERT INTO DANHMUC_TIENSU_YTE VALUES 
	('TSYT001',N'Đã tiêm vaccine COVID-19 trước đó'),
	('TSYT002',N'Đã bị COVID-19 trong 6 tháng'),
	('TSYT003',N'Đang mắc bệnh cấp tính'),
	('TSYT004',N'Có phẫu thuật không'),
	('TSYT005',N'Tuổi phẫu thuật (<13 hoặc ≥13)'),
	('TSYT006',N'Phân vệ độ 3 trên nền bệnh lý'),
	('TSYT007',N'Suy giảm miễn dịch'),
	('TSYT008',N'Tổn thương gan/thận giai đoạn điều trị'),
	('TSYT009',N'Dùng thuốc ức chế miễn dịch'),
	('TSYT010',N'Rối loạn đông máu/cầm máu'),
	('TSYT011',N'Rối loạn tri giác/rối loạn hành vi'),
	('TSYT012', N'Tiền sử tăng huyết áp'),
	('TSYT013', N'Tiền sử bệnh tim bẩm sinh'),
	('TSYT014', N'Tiền sử bệnh mạch vành'),
	('TSYT015', N'Suy tim mạn tính'),
	('TSYT016', N'Rối loạn nhịp tim'),
	('TSYT017', N'Tiền sử đột quỵ não'),
	('TSYT018', N'Tiền sử thiếu máu não thoáng qua'),
	('TSYT019', N'Đang điều trị bệnh phổi tắc nghẽn mạn tính (COPD)'),
	('TSYT020', N'Hen phế quản'),
	('TSYT021', N'Lao phổi đã điều trị'),
	('TSYT022', N'Lao phổi đang điều trị'),
	('TSYT023', N'Viêm phổi tái diễn'),
	('TSYT024', N'Tiền sử lao hạch'),
	('TSYT025', N'Xơ gan'),
	('TSYT026', N'Viêm gan B mạn tính'),
	('TSYT027', N'Viêm gan C mạn tính'),
	('TSYT028', N'Sỏi mật'),
	('TSYT029', N'Sỏi thận'),
	('TSYT030', N'Suy thận mạn'),
	('TSYT031', N'Hội chứng thận hư'),
	('TSYT032', N'Tiểu đường type 1'),
	('TSYT033', N'Tiểu đường type 2'),
	('TSYT034', N'Rối loạn mỡ máu'),
	('TSYT035', N'Béo phì độ 2 trở lên'),
	('TSYT036', N'Suy dinh dưỡng'),
	('TSYT037', N'Tiền sử viêm loét dạ dày - tá tràng'),
	('TSYT038', N'Viêm đại tràng mạn tính'),
	('TSYT039', N'Hội chứng ruột kích thích'),
	('TSYT040', N'Bệnh Crohn'),
	('TSYT041', N'Tiền sử viêm tụy cấp'),
	('TSYT042', N'Viêm tụy mạn'),
	('TSYT043', N'Tiền sử động kinh'),
	('TSYT044', N'Parkinson'),
	('TSYT045', N'Alzheimer'),
	('TSYT046', N'Tiền sử bệnh thần kinh ngoại biên'),
	('TSYT047', N'Rối loạn lo âu'),
	('TSYT048', N'Trầm cảm'),
	('TSYT049', N'Rối loạn lưỡng cực'),
	('TSYT050', N'Tiền sử dị ứng thuốc'),
	('TSYT051', N'Tiền sử dị ứng thức ăn'),
	('TSYT052', N'Tiền sử dị ứng nọc côn trùng'),
	('TSYT053', N'Phản ứng phản vệ trước đó'),
	('TSYT054', N'Tiền sử ung thư'),
	('TSYT055', N'Tiền sử hóa trị ung thư'),
	('TSYT056', N'Tiền sử xạ trị'),
	('TSYT057', N'Tiền sử cấy ghép tạng'),
	('TSYT058', N'Ghép tủy xương'),
	('TSYT059', N'Phụ nữ đang mang thai'),
	('TSYT060', N'Phụ nữ đang cho con bú'),
	('TSYT061', N'Tiền sử sảy thai nhiều lần'),
	('TSYT062', N'Cận thị nặng'),
	('TSYT063', N'Glôcôm (tăng nhãn áp)'),
	('TSYT064', N'Thóai hóa điểm vàng'),
	('TSYT065', N'Đục thủy tinh thể'),
	('TSYT066', N'Điếc bẩm sinh'),
	('TSYT067', N'Suy giảm thính lực do tuổi già'),
	('TSYT068', N'Viêm xoang mạn tính'),
	('TSYT069', N'Viêm mũi dị ứng'),
	('TSYT070', N'Viêm amidan tái phát'),
	('TSYT071', N'Polyp mũi'),
	('TSYT072', N'Viêm da cơ địa'),
	('TSYT073', N'Vảy nến'),
	('TSYT074', N'Lupus ban đỏ hệ thống'),
	('TSYT075', N'Viêm khớp dạng thấp'),
	('TSYT076', N'Thoái hóa khớp gối'),
	('TSYT077', N'Thoát vị đĩa đệm'),
	('TSYT078', N'Loãng xương'),
	('TSYT079', N'Gout'),
	('TSYT080', N'Chấn thương sọ não đã phẫu thuật'),
	('TSYT081', N'Chấn thương cột sống'),
	('TSYT082', N'Chấn thương chi'),
	('TSYT083', N'Mất chi (cụt chi)'),
	('TSYT084', N'Tiền sử phẫu thuật tim'),
	('TSYT085', N'Tiền sử phẫu thuật não'),
	('TSYT086', N'Tiền sử phẫu thuật tiêu hóa'),
	('TSYT087', N'Tiền sử phẫu thuật tiết niệu'),
	('TSYT088', N'Tiền sử phẫu thuật sản phụ khoa'),
	('TSYT089', N'Tiền sử ghép giác mạc'),
	('TSYT090', N'Tiền sử ghép da'),
	('TSYT091', N'Tiền sử truyền máu'),
	('TSYT092', N'Tiền sử phản ứng sau truyền máu'),
	('TSYT093', N'Thiếu máu bẩm sinh Thalassemia'),
	('TSYT094', N'Bệnh máu khó đông Hemophilia'),
	('TSYT095', N'Bệnh bạch cầu'),
	('TSYT096', N'Bệnh đa u tủy xương'),
	('TSYT097', N'Tiền sử bệnh lý tuyến giáp'),
	('TSYT098', N'Suy giáp'),
	('TSYT099', N'Cường giáp'),
	('TSYT100', N'U tuyến yên'),
	('TSYT101', N'U não lành tính'),
	('TSYT102', N'U ác tính di căn'),
	('TSYT103', N'Tiền sử viêm khớp nhiễm khuẩn'),
	('TSYT104', N'Tiền sử viêm màng não'),
	('TSYT105', N'Tiền sử viêm cơ tim'),
	('TSYT106', N'Hội chứng Down'),
	('TSYT107', N'Hội chứng Turner'),
	('TSYT108', N'Hội chứng Klinefelter'),
	('TSYT109', N'Bệnh lý tự miễn khác'),
	('TSYT110', N'Lao màng não đã điều trị'),
	('TSYT111', N'Bệnh nghề nghiệp liên quan hóa chất');


INSERT INTO TIENSU_YTE_BENHNHAN (MaTSYT, MaBN)
VALUES
-- KH0001 - Nguyễn Văn Hùng: có 3 tiền sử
('TSYT012', 'KH0001'), -- Tăng huyết áp
('TSYT033', 'KH0001'), -- Tiểu đường type 2
('TSYT034', 'KH0001'), -- Rối loạn mỡ máu

-- KH0002 - Trần Thị Lan: có 2 tiền sử
('TSYT025', 'KH0002'), -- Xơ gan
('TSYT026', 'KH0002'), -- Viêm gan B mạn tính

-- KH0003 - Lê Văn Minh: có 4 tiền sử
('TSYT017', 'KH0003'), -- Đột quỵ não
('TSYT012', 'KH0003'), -- Tăng huyết áp
('TSYT016', 'KH0003'), -- Rối loạn nhịp tim
('TSYT020', 'KH0003'), -- Hen phế quản

-- KH0004 - Phạm Thị Hoa: không có tiền sử

-- KH0005 - Hoàng Văn Tuấn: có 2 tiền sử
('TSYT039', 'KH0005'), -- Hội chứng ruột kích thích
('TSYT037', 'KH0005'), -- Viêm loét dạ dày

-- KH0006 - Võ Thị Hạnh: có 5 tiền sử
('TSYT032', 'KH0006'), -- Tiểu đường type 1
('TSYT043', 'KH0006'), -- Động kinh
('TSYT047', 'KH0006'), -- Rối loạn lo âu
('TSYT048', 'KH0006'), -- Trầm cảm
('TSYT050', 'KH0006'), -- Dị ứng thuốc

-- KH0007 - Đặng Văn Long: có 1 tiền sử
('TSYT079', 'KH0007'), -- Gout

-- KH0008 - Ngô Thị Mai: có 3 tiền sử
('TSYT059', 'KH0008'), -- Mang thai
('TSYT072', 'KH0008'), -- Viêm da cơ địa
('TSYT051', 'KH0008'), -- Dị ứng thức ăn

-- KH0009 - Bùi Văn Khánh: có 4 tiền sử
('TSYT019', 'KH0009'), -- COPD
('TSYT021', 'KH0009'), -- Lao phổi đã điều trị
('TSYT031', 'KH0009'), -- Hội chứng thận hư
('TSYT034', 'KH0009'), -- Rối loạn mỡ máu

-- KH0010 - Đỗ Thị Yến: có 2 tiền sử
('TSYT060', 'KH0010'), -- Đang cho con bú
('TSYT076', 'KH0010'), -- Thoái hóa khớp gối

-- KH0011 - Nguyễn Thị Hương: 3 tiền sử
('TSYT012', 'KH0011'), -- Tăng huyết áp
('TSYT033', 'KH0011'), -- Tiểu đường type 2
('TSYT072', 'KH0011'), -- Viêm da cơ địa

-- KH0012 - Trần Văn Nam: 4 tiền sử
('TSYT017', 'KH0012'), -- Đột quỵ não
('TSYT016', 'KH0012'), -- Rối loạn nhịp tim
('TSYT034', 'KH0012'), -- Rối loạn mỡ máu
('TSYT079', 'KH0012'), -- Gout

-- KH0013 - Lê Thị Thu: không có tiền sử

-- KH0014 - Phạm Văn Hoàng: 2 tiền sử
('TSYT037', 'KH0014'), -- Viêm loét dạ dày
('TSYT039', 'KH0014'), -- Hội chứng ruột kích thích

-- KH0015 - Hoàng Thị Mai: 1 tiền sử
('TSYT025', 'KH0015'), -- Xơ gan

-- KH0016 - Vũ Văn Dũng: 5 tiền sử
('TSYT043', 'KH0016'), -- Động kinh
('TSYT048', 'KH0016'), -- Trầm cảm
('TSYT047', 'KH0016'), -- Rối loạn lo âu
('TSYT012', 'KH0016'), -- Tăng huyết áp
('TSYT033', 'KH0016'), -- Tiểu đường type 2

-- KH0017 - Đinh Thị Hòa: 2 tiền sử
('TSYT059', 'KH0017'), -- Mang thai
('TSYT050', 'KH0017'), -- Dị ứng thuốc

-- KH0018 - Nguyễn Văn Lâm: 3 tiền sử
('TSYT019', 'KH0018'), -- COPD
('TSYT021', 'KH0018'), -- Lao phổi đã điều trị
('TSYT020', 'KH0018'), -- Hen phế quản

-- KH0019 - Trần Thị Ngọc: 1 tiền sử
('TSYT051', 'KH0019'), -- Dị ứng thức ăn

-- KH0020 - Lê Văn Thành: 4 tiền sử
('TSYT031', 'KH0020'), -- Hội chứng thận hư
('TSYT034', 'KH0020'), -- Rối loạn mỡ máu
('TSYT076', 'KH0020'), -- Thoái hóa khớp gối
('TSYT079', 'KH0020'), -- Gout

-- KH021 - Đoàn Minh Phúc: có 1 tiền sử
('TSYT012', 'KH0021'), -- Tăng huyết áp

-- KH022 - Lê Hải Yến: không có tiền sử
-- (bỏ trống)

-- KH023 - Trần Nhật Nam: có 2 tiền sử
('TSYT033', 'KH0023'), -- Tiểu đường type 2
('TSYT034', 'KH0023'), -- Rối loạn mỡ máu

-- KH024 - Nguyễn Thị Mỹ Duyên: có 1 tiền sử
('TSYT007', 'KH0024'), -- Suy giảm miễn dịch

-- KH025 - Phạm Văn Khánh: không có tiền sử
-- (bỏ trống)

-- KH026 - Hoàng Lan Chi: có 1 tiền sử
('TSYT027', 'KH0026'), -- Bệnh tim bẩm sinh

-- KH027 - Nguyễn Quang Huy: có 2 tiền sử
('TSYT001', 'KH0027'), -- Đã tiêm vaccine COVID-19 trước đó
('TSYT002', 'KH0027'), -- Đã bị COVID-19 trong 6 tháng

-- KH028 - Lý Thị Bảo Ngọc: không có tiền sử
-- (bỏ trống)

-- KH029 - Phan Hoàng Long: có 1 tiền sử
('TSYT041', 'KH0029'), -- Hen phế quản

-- KH030 - Vũ Thu Trang: có 2 tiền sử
('TSYT010', 'KH0030'), -- Rối loạn đông máu/cầm máu
('TSYT045', 'KH0030'), -- Viêm da cơ địa

-- KH031 - Nguyễn Hữu Toàn: có 2 tiền sử
('TSYT012', 'KH0031'), -- Tăng huyết áp
('TSYT033', 'KH0031'), -- Tiểu đường type 2

-- KH032 - Trịnh Mai Phương: có 1 tiền sử
('TSYT046', 'KH0032'), -- Viêm dạ dày mạn

-- KH033 - Tạ Văn Cường: có 3 tiền sử
('TSYT041', 'KH0033'), -- Hen phế quản
('TSYT034', 'KH0033'), -- Rối loạn mỡ máu
('TSYT055', 'KH0033'), -- Đau thắt ngực ổn định

-- KH034 - Nguyễn Kim Oanh: không có tiền sử
-- (bỏ trống)

-- KH035 - Đỗ Quang Minh: có 2 tiền sử
('TSYT025', 'KH0035'), -- Xơ gan
('TSYT026', 'KH0035'), -- Viêm gan B mạn tính

-- KH036 - Lưu Thanh Hà: có 1 tiền sử
('TSYT007', 'KH0036'), -- Suy giảm miễn dịch

-- KH037 - Ngô Văn An: có 3 tiền sử
('TSYT001', 'KH0037'), -- Đã tiêm vaccine COVID-19 trước đó
('TSYT002', 'KH0037'), -- Đã bị COVID-19 trong 6 tháng
('TSYT010', 'KH0037'), -- Rối loạn đông máu/cầm máu

-- KH038 - Trần Thị Thu Hằng: có 1 tiền sử
('TSYT049', 'KH0038'), -- Thiếu máu do thiếu sắt

-- KH039 - Nguyễn Đức Mạnh: có 4 tiền sử
('TSYT012', 'KH0039'), -- Tăng huyết áp
('TSYT033', 'KH0039'), -- Tiểu đường type 2
('TSYT034', 'KH0039'), -- Rối loạn mỡ máu
('TSYT047', 'KH0039'), -- Viêm loét dạ dày tá tràng

-- KH040 - Phạm Thị Hòa: không có tiền sử
-- (bỏ trống)

-- KH041 - Nguyễn Văn Thắng: có 2 tiền sử
('TSYT012', 'KH0041'), -- Tăng huyết áp
('TSYT034', 'KH0041'), -- Rối loạn mỡ máu

-- KH042 - Trần Ngọc Ánh: có 1 tiền sử
('TSYT049', 'KH0042'), -- Thiếu máu do thiếu sắt

-- KH043 - Lê Minh Quân: có 3 tiền sử
('TSYT033', 'KH0043'), -- Tiểu đường type 2
('TSYT041', 'KH0043'), -- Hen phế quản
('TSYT055', 'KH0043'), -- Đau thắt ngực ổn định

-- KH044 - Nguyễn Thanh Mai: không có tiền sử
-- (bỏ trống)

-- KH045 - Phan Văn Đức: có 4 tiền sử
('TSYT012', 'KH0045'), -- Tăng huyết áp
('TSYT033', 'KH0045'), -- Tiểu đường type 2
('TSYT025', 'KH0045'), -- Xơ gan
('TSYT026', 'KH0045'), -- Viêm gan B mạn tính

-- KH046 - Hồ Thị Yến Nhi: có 1 tiền sử
('TSYT038', 'KH0046'), -- Dị ứng thuốc

-- KH047 - Nguyễn Văn Hải: có 2 tiền sử
('TSYT010', 'KH0047'), -- Rối loạn đông máu/cầm máu
('TSYT046', 'KH0047'), -- Viêm dạ dày mạn

-- KH048 - Đinh Thị Thảo: có 3 tiền sử
('TSYT002', 'KH0048'), -- Đã bị COVID-19 trong 6 tháng
('TSYT047', 'KH0048'), -- Viêm loét dạ dày tá tràng
('TSYT056', 'KH0048'), -- Béo phì

-- KH049 - Võ Quang Khải: có 2 tiền sử
('TSYT012', 'KH0049'), -- Tăng huyết áp
('TSYT033', 'KH0049'), -- Tiểu đường type 2

-- KH050 - Lê Mỹ Linh: không có tiền sử
-- (bỏ trống)

-- KH0051 - Nguyễn Anh Tuấn: 2 tiền sử (viêm da cơ địa, dị ứng thức ăn)
('TSYT072', 'KH0051'),
('TSYT051', 'KH0051'),

-- KH0052 - Trần Ngọc Bích: không có tiền sử
-- (bỏ trống)

-- KH0053 - Lê Đức Huy: 1 tiền sử (hen phế quản)
('TSYT020', 'KH0053'),

-- KH0054 - Phạm Thảo Nhi: 1 tiền sử (viêm amidan tái phát)
('TSYT070', 'KH0054'),

-- KH0055 - Hoàng Gia Bảo: 1 tiền sử (Thalassemia - bệnh máu bẩm sinh)
('TSYT093', 'KH0055'),

-- KH0056 - Võ Minh Quân: không có tiền sử
-- (bỏ trống)

-- KH0057 - Đặng Khánh Linh: 1 tiền sử (dị ứng thức ăn)
('TSYT051', 'KH0057'),

-- KH0058 - Ngô Hoài Nam: 1 tiền sử (động kinh)
('TSYT043', 'KH0058'),

-- KH0059 - Bùi Thanh Thảo: 1 tiền sử (viêm da cơ địa)
('TSYT072', 'KH0059'),

-- KH0060 - Đỗ Minh Anh: 1 tiền sử (đã tiêm vaccine COVID-19 trước đó)
('TSYT001', 'KH0060'),

-- KH0061 - Nguyễn Phương Thảo: 2 tiền sử (viêm da cơ địa, dị ứng thức ăn)
('TSYT072', 'KH0061'),
('TSYT051', 'KH0061'),

-- KH0062 - Trần Hoàng Nam: không có tiền sử
-- (bỏ trống)

-- KH0063 - Lê Khánh Vy: 1 tiền sử (hen phế quản)
('TSYT020', 'KH0063'),

-- KH0064 - Phạm Anh Dũng: 1 tiền sử (viêm amidan tái phát)
('TSYT070', 'KH0064'),

-- KH0065 - Hoàng Bảo Ngọc: 1 tiền sử (Thiếu máu bẩm sinh - Thalassemia)
('TSYT093', 'KH0065'),

-- KH0066 - Võ Hữu Tài: không có tiền sử
-- (bỏ trống)

-- KH0067 - Đặng Ngọc Hân: 1 tiền sử (dị ứng thức ăn)
('TSYT051', 'KH0067'),

-- KH0068 - Ngô Minh Đức: 2 tiền sử (động kinh, đã tiêm vaccine COVID-19 trước đó)
('TSYT043', 'KH0068'),
('TSYT001', 'KH0068'),

-- KH0069 - Bùi Hồng Anh: 1 tiền sử (viêm da cơ địa)
('TSYT072', 'KH0069'),

-- KH0070 - Đỗ Quốc Khánh: 1 tiền sử (phản vệ trước đó)
('TSYT053', 'KH0070');




-- ===========================================================================================================================
--------------------------------------------------- NHÂN VIÊN & TÀI KHOẢN ----------------------------------------------------
-- ===========================================================================================================================

INSERT INTO CHUCVU (TenChucVu) 
VALUES
	(N'Giám đốc phòng khám'),
	(N'Quản trị hệ thống'), -- Admin kỹ thuật
	(N'Bác sĩ trưởng khoa'), -- Quản lý chuyên môn
	(N'Bác sĩ điều trị'),   -- Người khám và kê đơn chính
	(N'Điều dưỡng'),        -- Hỗ trợ khám và sàng lọc
	(N'Kỹ thuật viên CLS'), -- Thực hiện xét nghiệm, X-quang, siêu âm
	(N'Dược sĩ'),           -- Quản lý kho thuốc và phát thuốc
	(N'Nhân viên tiếp đón'),-- Đăng ký và phân phòng khám
	(N'Kế toán thu ngân'),  -- Xử lý hóa đơn và thanh toán
	(N'Nhân viên bảo vệ'),
	(N'Nhân viên tạp vụ'),
	(N'Nhân viên phát thuốc');
GO

INSERT INTO KHOA (TenKhoa, MoTa, TrangThai)
VALUES
(N'Khoa Khám bệnh', N'Tiếp nhận, khám ban đầu, phân loại bệnh nhân và điều trị ngoại trú.', 1), -- ID: 1
(N'Khoa Nội tổng hợp', N'Khám và điều trị các bệnh lý nội khoa: tim mạch, tiêu hóa, hô hấp, nội tiết...', 1), -- ID: 2
(N'Khoa Ngoại tổng hợp', N'Khám và xử lý các tiểu phẫu, chấn thương nhẹ, các bệnh lý cần can thiệp ngoại khoa cơ bản.', 1), -- ID: 3
(N'Khoa Nhi', N'Khám, tư vấn dinh dưỡng và điều trị các bệnh lý ở trẻ em và trẻ sơ sinh.', 1), -- ID: 4
(N'Khoa Sản - Phụ khoa', N'Khám thai, tư vấn sinh sản, khám và điều trị các bệnh lý phụ khoa.', 1), -- ID: 5
(N'Khoa Tai Mũi Họng', N'Khám, nội soi và điều trị các bệnh lý về Tai, Mũi, Họng.', 1), -- ID: 6
(N'Khoa Răng Hàm Mặt', N'Khám, nhổ răng, nha chu, phục hình răng và các bệnh lý răng miệng.', 1), -- ID: 7
(N'Khoa Mắt', N'Đo thị lực, khám và điều trị các bệnh lý về mắt.', 1), -- ID: 8
(N'Khoa Da liễu', N'Khám và điều trị các bệnh về da, tóc, móng và thẩm mỹ da.', 1), -- ID: 9
(N'Khoa Chẩn đoán hình ảnh', N'Thực hiện siêu âm, X-quang, MRI, CT cắt lớp phục vụ chẩn đoán.', 1), -- ID: 10
(N'Khoa Xét nghiệm', N'Thực hiện các xét nghiệm sinh hóa, huyết học, vi sinh, miễn dịch.', 1), -- ID: 11
(N'Khoa Dược', N'Quản lý kho thuốc, cấp phát thuốc BHYT và bán thuốc dịch vụ.', 1); -- ID: 12
GO

INSERT INTO DANHMUC_LOAIPHONG (TenLoaiPhong)
VALUES 
    (N'Quầy Tiếp nhận'),         -- ID: 1
    (N'Phòng Khám bệnh'),        -- ID: 2
    (N'Phòng Xét nghiệm'),       -- ID: 3
    (N'Phòng X-Quang'),          -- ID: 4
    (N'Phòng Siêu âm'),          -- ID: 5
    (N'Phòng Nội soi'),          -- ID: 6  (Thêm mới cho DV Nội soi)
    (N'Phòng Thủ thuật'),        -- ID: 7  (Thêm mới cho Cắt chỉ, nặn mụn, nhổ răng...)
    (N'Quầy Thu ngân'),          -- ID: 8
    (N'Nhà thuốc'),              -- ID: 9
    (N'Kho Dược / Vật tư');      -- ID: 10
GO


INSERT INTO PHONG (TenPhong, MaLoaiPhong, TrangThai, MaKhoa) VALUES 
-- ==========================================
-- 1. KHU VỰC TIẾP NHẬN & HÀNH CHÍNH (MaKhoa để NULL vì không thuộc Khoa lâm sàng)
-- ==========================================
(N'Quầy Tiếp Tân 01', 1, 1, NULL), 
(N'Quầy Tiếp Tân 02', 1, 1, NULL), 
(N'Quầy Tiếp Tân 03', 1, 1, NULL), 
(N'Quầy Thu ngân 01', 8, 1, NULL), 
(N'Quầy Thu ngân 02', 8, 1, NULL),

-- ==========================================
-- 2. KHU VỰC KHÁM BỆNH CHUYÊN KHOA (Thuộc các khoa tương ứng)
-- ==========================================
(N'Phòng khám Nội tổng quát 01', 2, 1, 2),
(N'Phòng khám Nội tổng quát 02', 2, 1, 2),
(N'Phòng khám Ngoại tổng hợp 01', 2, 1, 3),
(N'Phòng khám Nhi khoa 01', 2, 1, 4),
(N'Phòng khám Nhi khoa 02', 2, 1, 4),
(N'Phòng khám Sản phụ khoa 01', 2, 1, 5),
(N'Phòng khám Tai Mũi Họng 01', 2, 1, 6),
(N'Phòng khám Răng Hàm Mặt 01', 2, 1, 7),
(N'Phòng khám Mắt 01', 2, 1, 8),
(N'Phòng khám Da liễu 01', 2, 1, 9),

-- ==========================================
-- 3. KHU VỰC THỦ THUẬT & NỘI SOI 
-- ==========================================
(N'Phòng Nội soi Tiêu hóa', 6, 1, 10),      -- Thuộc CĐHA (ID 10)
(N'Phòng Tiểu phẫu & Thay băng', 7, 1, 3),  -- Thuộc Khoa Ngoại (ID 3)
(N'Phòng Thực hiện Thủ thuật Da liễu', 7, 1, 9), -- Nặn mụn, đốt mụn ruồi (ID 9)

-- ==========================================
-- 4. KHU VỰC CẬN LÂM SÀNG 
-- ==========================================
(N'Phòng Xét nghiệm Huyết học', 3, 1, 11),  -- Thuộc Khoa Xét nghiệm (ID 11)
(N'Phòng Siêu âm 4D', 5, 1, 10),            -- Thuộc Khoa CĐHA (ID 10)
(N'Phòng Chụp X-Quang Kỹ thuật số', 4, 1, 10), 

-- ==========================================
-- 5. KHU VỰC DƯỢC & KHO 
-- ==========================================
(N'Nhà thuốc 01', 9, 1, 12), -- Thuộc Khoa Dược (ID 12)
(N'Nhà thuốc 02', 9, 1, 12),
(N'Nhà thuốc 03', 9, 1, 12),
(N'Nhà thuốc 04', 9, 1, 12),
(N'Nhà thuốc 05', 9, 1, 12),
(N'Kho Dược phẩm Trung tâm', 10, 1, 12),
(N'Kho Vật tư Y tế', 10, 1, 12);
GO


-- Chức vụ: 1-Giám đốc, 2-Admin, 3-Trưởng khoa, 4-BS điều trị, 5-Điều dưỡng, 
--          6-KTV CLS, 7-Dược sĩ, 8-Tiếp đón, 9-Thu ngân, 10-Bảo vệ, 11-Tạp vụ.
-- Khoa: 1-Khám bệnh, 2-Nội, 3-Ngoại, 4-Nhi, 5-Sản, 6-TMH, 7-RHM, 8-Mắt, 9-Da liễu, 10-CĐHA, 11-Xét nghiệm, 12-Dược.

INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, SDT, Email, DiaChi, MaChucVu, TrangThai, MaTK, MaKhoa, MaPhong, HinhAnh)
VALUES
-- Nhóm Quản lý & Admin 
('NV001', N'Nguyễn Văn An', '1985-03-12', N'Nam', '0901123451', 'an.nguyen@vnvc.vn', N'Quận 1, TP.HCM', 1, 1, 1, NULL, NULL, 'nv001.jpg'), 
('NV002', N'Trần Thị Bích', '1990-07-22', N'Nữ', '0902123452', 'bich.tran@vnvc.vn', N'Quận Hai Bà Trưng, Hà Nội', 2, 1, 2, NULL, NULL, NULL), 

-- Nhóm Bác sĩ
('NV003', N'Lê Văn Cường', '1987-11-10', N'Nam', '0903123453', 'cuong.le@vnvc.vn', N'Hải Châu, Đà Nẵng', 3, 1, 3, 2, 3, 'nv003.jpg'),
('NV004', N'Phạm Thị Dung', '1992-05-09', N'Nữ', '0904123454', 'dung.pham@vnvc.vn', N'Bình Thuỷ, Cần Thơ', 4, 1, 4, 2, 3, 'nv004.jpg'),
('NV005', N'Hoàng Văn Đông', '1989-09-15', N'Nam', '0905123455', 'dong.hoang@vnvc.vn', N'Ngô Quyền, Hải Phòng', 4, 1, 5, 2, 4, 'nv005.jpg'),
('NV006', N'Võ Thị Hạnh', '1993-12-01', N'Nữ', '0906123456', 'hanh.vo@vnvc.vn', N'TP. Vinh, Nghệ An', 4, 1, 6, 3, 5, 'nv006.jpg'),
('NV007', N'Đặng Văn Hòa', '1984-04-18', N'Nam', '0907123457', 'hoa.dang@vnvc.vn', N'Thủ Dầu Một, Bình Dương', 4, 1, 7, 3, 6, 'nv007.jpg'),
('NV008', N'Ngô Thị Hương', '1991-06-25', N'Nữ', '0908123458', 'huong.ngo@vnvc.vn', N'TP. Nha Trang, Khánh Hòa', 4, 1, 8, 5, 7, 'nv008.jpg'),
('NV009', N'Bùi Văn Khánh', '1986-08-30', N'Nam', '0909123459', 'khanh.bui@vnvc.vn', N'TP. Huế, Thừa Thiên Huế', 4, 1, 9, 5, 8, 'nv009.jpg'),
('NV010', N'Đỗ Thị Lan', '1995-01-19', N'Nữ', '0910123460', 'lan.do@vnvc.vn', N'TP. Biên Hòa, Đồng Nai', 4, 1, 10, 4, 9, 'nv010.jpg'),

-- Nhóm Điều dưỡng
('NV011', N'Nguyễn Văn Long', '1982-02-14', N'Nam', '0911123461', 'long.nguyen@vnvc.vn', N'Hà Đông, Hà Nội', 5, 1, 11, 2, 3, NULL), 
('NV012', N'Trần Thị Mai', '1990-05-27', N'Nữ', '0912123462', 'mai.tran@vnvc.vn', N'Quận 1, TP.HCM', 5, 1, 12, 2, 4, NULL),
('NV013', N'Phan Văn Mạnh', '1988-03-08', N'Nam', '0913123463', 'manh.phan@vnvc.vn', N'Hai Bà Trưng, Hà Nội', 5, 1, 13, 3, 5, NULL),
('NV014', N'Lê Thị Minh', '1994-06-13', N'Nữ', '0914123464', 'minh.le@vnvc.vn', N'Hải Châu, Đà Nẵng', 5, 1, 14, 3, 6, NULL),
('NV015', N'Hoàng Văn Nam', '1986-09-22', N'Nam', '0915123465', 'nam.hoang@vnvc.vn', N'Cần Thơ', 5, 1, 15, 5, 7, NULL),

-- Nhóm Kỹ thuật viên CLS
('NV016', N'Vũ Thị Ngọc', '1991-12-04', N'Nữ', '0916123466', 'ngoc.vu@vnvc.vn', N'Hải Phòng', 6, 1, 16, 11, 15, NULL),
('NV017', N'Đinh Văn Phát', '1985-10-16', N'Nam', '0917123467', 'phat.dinh@vnvc.vn', N'Nghệ An', 6, 1, 17, 11, 15, NULL),
('NV018', N'Nguyễn Thị Phương', '1992-04-07', N'Nữ', '0918123468', 'phuong.nguyen@vnvc.vn', N'Bình Dương', 6, 1, 18, 10, 16, NULL),
('NV019', N'Phạm Văn Quân', '1989-07-28', N'Nam', '0919123469', 'quan.pham@vnvc.vn', N'Nha Trang', 6, 1, 19, 10, 16, NULL),
('NV020', N'Trương Thị Quỳnh', '1996-11-11', N'Nữ', '0920123470', 'quynh.truong@vnvc.vn', N'Huế', 6, 1, 20, 10, 17, NULL),

-- Nhóm Dược sĩ
('NV021', N'Đỗ Bảo Trâm', '1990-07-21', N'Nữ', '0903200111', 'tram.db@vnvc.vn', N'39 Nguyễn Huệ, TP.HCM', 7, 1, 21, 12, 18, NULL), 
('NV022', N'Võ Văn Quân', '1993-01-12', N'Nam', '0903222333', 'quan.vv@vnvc.vn', N'15 Lê Lợi, TP.HCM', 7, 1, 22, 12, 19, NULL), 
('NV023', N'Lý Thị Lan', '1996-08-19', N'Nữ', '0903555666', 'lan.lt@vnvc.vn', N'60 Pasteur, TP.HCM', 7, 1, 23, 12, 20, NULL), 
('NV024', N'Trần Thanh Tú', '1992-11-30', N'Nam', '0903888999', 'tu.tt@vnvc.vn', N'72 Hai Bà Trưng, TP.HCM', 7, 1, 24, 12, 21, NULL), 
('NV025', N'Hoàng Mỹ Dung', '1994-03-03', N'Nữ', '0903111000', 'dung.hm@vnvc.vn', N'25 CMT8, TP.HCM', 7, 1, 25, 12, 22, NULL), 
('NV026', N'Đinh Quốc Thịnh', '1987-12-17', N'Nam', '0903444555', 'thinh.dq@vnvc.vn', N'27 Lê Quang Định, TP.HCM', 7, 1, 26, 12, 23, NULL), 

-- Nhóm Tiếp đón 
('NV027', N'Nguyễn Ngọc Hiếu', '1989-06-22', N'Nam', '0903777888', 'hieu.nn@vnvc.vn', N'140 Điện Biên Phủ, TP.HCM', 8, 1, 27, 1, 1, NULL),
('NV028', N'Trương Hồng Phúc', '1991-04-05', N'Nam', '0903999111', 'phuc.th@vnvc.vn', N'20 Nguyễn Trãi, TP.HCM', 8, 1, 28, 1, 1, NULL),
('NV029', N'Nguyễn Thuỳ Linh', '1993-09-25', N'Nữ', '0903666999', 'linh.nt@vnvc.vn', N'55 Hai Bà Trưng, Hà Nội', 8, 1, 29, 1, 1, NULL),

-- Nhóm Thu ngân
('NV030', N'Phạm Hoàng Vũ', '1988-02-18', N'Nam', '0903444666', 'vu.ph@vnvc.vn', N'12 Trần Quang Khải, Hà Nội', 9, 1, 30, NULL, 2, NULL),
('NV031', N'Ngô Thị Quỳnh', '1995-05-14', N'Nữ', '0903999222', 'quynh.ntq@vnvc.vn', N'18 Cầu Giấy, Hà Nội', 9, 1, 31, NULL, 2, NULL),

-- Nhóm Bảo vệ & Tạp vụ
('NV032', N'Lương Minh Tâm', '1990-08-08', N'Nam', '0903777999', 'tam.lm@vnvc.vn', N'75 Nguyễn Chí Thanh, Hà Nội', 10, 1, 32, NULL, 1, NULL), 
('NV033', N'Tạ Thị Bích Ngọc', '1994-12-10', N'Nữ', '0903555777', 'ngoc.tb@vnvc.vn', N'200 Xuân Thuỷ, Hà Nội', 11, 1, 33, NULL, NULL, NULL), 

-- Bổ sung thêm Bác sĩ 
('NV034', N'Phan Văn Khôi', '1986-09-09', N'Nam', '0903222111', 'khoi.pv@vnvc.vn', N'180 Tây Sơn, Hà Nội', 4, 1, 34, 4, 10, 'nv034.jpg'),
('NV035', N'Trần Hồng Mai', '1997-07-07', N'Nữ', '0903888777', 'mai.th@vnvc.vn', N'22 Nguyễn Khuyến, Hà Nội', 4, 1, 35, 6, 11, 'nv035.jpg'),
('NV036', N'Bùi Văn Thành', '1991-10-20', N'Nam', '0903000333', 'thanh.bv@vnvc.vn', N'10 Hùng Vương, Hà Nội', 4, 1, 36, 6, 12, 'nv036.jpg'),
('NV037', N'Lê Ngọc Bảo', '1989-01-15', N'Nam', '0903111222', 'bao.ln@vnvc.vn', N'30 Kim Mã, Hà Nội', 4, 1, 37, 7, 13, 'nv037.jpg'),
('NV038', N'Huỳnh Thị Hương', '1992-02-22', N'Nữ', '0903444777', 'huong.ht@vnvc.vn', N'42 Giải Phóng, Hà Nội', 4, 1, 38, 7, 14, 'nv038.jpg'),

-- Nhân viên dự phòng
('NV039', N'Đỗ Văn Phú', '1985-03-13', N'Nam', '0903555999', 'phu.dv@vnvc.vn', N'95 Nguyễn Thái Học, Hà Nội', 4, 1, 39, 2, NULL, 'nv039.jpg'), 
('NV040', N'Nguyễn Khánh An', '1994-04-04', N'Nữ', '0903666000', 'an.nk@vnvc.vn', N'12 Điện Biên Phủ, Đà Nẵng', 12, 1, 40, 4, 22, NULL), 
('NV041', N'Vũ Thị Thanh', '1996-05-28', N'Nữ', '0903777000', 'thanh.vt@vnvc.vn', N'80 Nguyễn Văn Linh, Đà Nẵng', 12, 1, 41, 5, 23, NULL), 
('NV042', N'Nguyễn Minh Đức', '1987-06-16', N'Nam', '0903888000', 'duc.nm@vnvc.vn', N'40 Bạch Đằng, Đà Nẵng', 12, 1, 42, 11, 24, NULL),
('NV043', N'Hồ Văn Tài', '1992-07-07', N'Nam', '0903999000', 'tai.hv@vnvc.vn', N'90 Trần Phú, Đà Nẵng', 12, 1, 43, 10, 25, NULL), 
('NV044', N'Phạm Thị Thuỷ', '1990-08-19', N'Nữ', '0903111333', 'thuy.pt@vnvc.vn', N'15 Lê Duẩn, Đà Nẵng', 12, 1, 44, 1, 26, NULL), 
('NV045', N'Trần Quốc Việt', '1993-09-30', N'Nam', '0903222555', 'viet.tq@vnvc.vn', N'110 Nguyễn Văn Linh, Đà Nẵng', 8, 0, 45, 1, NULL, NULL);
GO

-- Ví dụ chia đều
UPDATE NHANVIEN SET MaPhong = 1 WHERE MaNV = 'NV027';
UPDATE NHANVIEN SET MaPhong = 2 WHERE MaNV = 'NV028';
UPDATE NHANVIEN SET MaPhong = 3 WHERE MaNV = 'NV029';

-- Nội tổng quát
UPDATE NHANVIEN SET MaPhong = 6 WHERE MaNV IN ('NV003','NV004');

-- Nội tổng quát 02
UPDATE NHANVIEN SET MaPhong = 7 WHERE MaNV IN ('NV005');

-- Ngoại
UPDATE NHANVIEN SET MaPhong = 8 WHERE MaNV IN ('NV006');

-- Nhi
UPDATE NHANVIEN SET MaPhong = 9 WHERE MaNV IN ('NV007');
UPDATE NHANVIEN SET MaPhong = 10 WHERE MaNV IN ('NV008');

-- Sản
UPDATE NHANVIEN SET MaPhong = 11 WHERE MaNV IN ('NV009');

-- Tai Mũi Họng
UPDATE NHANVIEN SET MaPhong = 12 WHERE MaNV IN ('NV010');

-- RHM
UPDATE NHANVIEN SET MaPhong = 13 WHERE MaNV IN ('NV034');

-- Mắt
UPDATE NHANVIEN SET MaPhong = 14 WHERE MaNV IN ('NV035');

-- Da liễu
UPDATE NHANVIEN SET MaPhong = 15 WHERE MaNV IN ('NV036');

INSERT INTO TAIKHOAN (Username, PasswordHash, IsActive)
VALUES
('baolq', '1', 1);


-- 3. CHÈN TÀI KHOẢN VÀ BÁC SĨ QUẢN TRỊ
INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, SDT, Email, DiaChi, MaChucVu, TrangThai, MaTK, HinhAnh)
VALUES (
    'NV_BAO', N'Lê Quân Bảo', '1999-01-01', N'Nam', '0999888777', 'baolq30@vnvc.vn', N'TP. Hồ Chí Minh', 
    1, 1, (SELECT MaTK FROM TAIKHOAN WHERE Username = 'baolq'), 'nv_bao.jpg'
);
GO


-- =========================================================================================
-- THÊM DỮ LIỆU BẢNG LOAI_DICHVU 
-- =========================================================================================
INSERT INTO LOAI_DICHVU (MaLoaiDV, TenLoaiDV) VALUES
('LDV01', N'Khám bệnh'),
('LDV02', N'Xét nghiệm'),
('LDV03', N'Chẩn đoán hình ảnh'),
('LDV04', N'Thăm dò chức năng'),
('LDV05', N'Nội soi'),
('LDV06', N'Thủ thuật Ngoại khoa'),
('LDV07', N'Dịch vụ Răng Hàm Mặt'),
('LDV08', N'Dịch vụ Sản Phụ khoa'),
('LDV09', N'Thủ thuật Mắt - Da liễu');
GO



-- =========================================================================================
-- BỘ DATA DỊCH VỤ Y TẾ KHỔNG LỒ (CHUẨN CÓ CỘT MoTa)
-- =========================================================================================

INSERT INTO DICHVU (MaDV, TenDV, MaLoaiDV, GiaDichVu, DonViTinh, CoBHYT, TrangThai, MoTa, MaKhoa) VALUES
-- ================= NHÓM 1: KHÁM BỆNH (LDV01) =================
('DV001', N'Khám bệnh Nội tổng quát', 'LDV01', 150000, N'Lần', 1, 1, N'Khám tim mạch, hô hấp, nội tiết.', 2),
('DV002', N'Khám bệnh Ngoại tổng quát', 'LDV01', 150000, N'Lần', 1, 1, N'Khám chấn thương, ngoại khoa.', 3),
('DV003', N'Khám chuyên khoa Nhi', 'LDV01', 150000, N'Lần', 1, 1, N'Khám trẻ em và tư vấn dinh dưỡng.', 4),
('DV004', N'Khám chuyên khoa Phụ Sản', 'LDV01', 150000, N'Lần', 1, 1, N'Khám thai định kỳ và phụ khoa.', 5),
('DV005', N'Khám chuyên khoa Tai Mũi Họng', 'LDV01', 150000, N'Lần', 1, 1, N'Khám và nội soi Tai Mũi Họng.', 6),
('DV006', N'Khám chuyên khoa Răng Hàm Mặt', 'LDV01', 150000, N'Lần', 1, 1, N'Khám răng miệng tổng quát.', 7),
('DV007', N'Khám chuyên khoa Mắt', 'LDV01', 150000, N'Lần', 1, 1, N'Khám mắt và đo thị lực.', 8),
('DV008', N'Khám chuyên khoa Da liễu', 'LDV01', 150000, N'Lần', 1, 1, N'Khám các bệnh lý da liễu.', 9),
('DV009', N'Khám bệnh yêu cầu (Giáo sư)', 'LDV01', 500000, N'Lần', 0, 1, N'Khám trực tiếp với chuyên gia.', 1),
('DV010', N'Khám cấp cứu ngoài giờ', 'LDV01', 250000, N'Lần', 1, 1, N'Khám ban đầu tại phòng cấp cứu.', 1),

-- ===== 🚀 DỊCH VỤ ẢO: PHÍ ĐẶT LỊCH ONLINE (Nằm chìm, tự động nổ ra khi thanh toán) =====
('DV999', N'Phí tiện ích đặt lịch Online', 'LDV01', 200000, N'Lần', 0, 1, N'Phụ thu đặt lịch hẹn trước, không áp dụng BHYT.', 1),

-- ================= NHÓM 2: XÉT NGHIỆM (LDV02) -> Map Khoa Xét nghiệm (ID: 11) =================
('DV011', N'Tổng phân tích tế bào máu', 'LDV02', 90000, N'Mẫu', 1, 1, N'Kiểm tra hồng cầu, bạch cầu.', 11),
('DV012', N'Định lượng Glucose', 'LDV02', 40000, N'Mẫu', 1, 1, N'Kiểm tra đường huyết.', 11),
('DV013', N'Xét nghiệm HbA1c', 'LDV02', 120000, N'Mẫu', 1, 1, N'Tầm soát tiểu đường mạn tính.', 11),
('DV014', N'Định lượng Cholesterol', 'LDV02', 45000, N'Mẫu', 1, 1, N'Xét nghiệm mỡ máu.', 11),
('DV015', N'Chức năng gan (AST/ALT)', 'LDV02', 80000, N'Mẫu', 1, 1, N'Đánh giá tổn thương gan.', 11),
('DV016', N'Định lượng Creatinin', 'LDV02', 40000, N'Mẫu', 1, 1, N'Đánh giá chức năng thận.', 11),
('DV017', N'Tổng phân tích nước tiểu', 'LDV02', 50000, N'Mẫu', 1, 1, N'Kiểm tra đường tiết niệu.', 11),
('DV018', N'Test nhanh Viêm gan B', 'LDV02', 80000, N'Mẫu', 1, 1, N'Tầm soát virus HBsAg.', 11),

-- ================= NHÓM 3: CHẨN ĐOÁN HÌNH ẢNH (LDV03) -> Map Khoa CĐHA (ID: 10) =================
('DV019', N'Siêu âm ổ bụng tổng quát', 'LDV03', 250000, N'Lần', 1, 1, N'Siêu âm gan, mật, tụy, thận.', 10),
('DV020', N'Siêu âm tuyến giáp', 'LDV03', 150000, N'Lần', 1, 1, N'Siêu âm phát hiện u tuyến giáp.', 10),
('DV021', N'Siêu âm doppler tim', 'LDV03', 350000, N'Lần', 1, 1, N'Siêu âm màu chức năng van tim.', 10),
('DV022', N'Chụp X-Quang ngực thẳng', 'LDV03', 120000, N'Lần', 1, 1, N'Chụp tim phổi kỹ thuật số.', 10),
('DV023', N'Chụp X-Quang cột sống', 'LDV03', 150000, N'Lần', 1, 1, N'Kiểm tra thoái hóa cột sống.', 10),

-- ================= NHÓM 4: THĂM DÒ & NỘI SOI (LDV04, 05) -> Map Khoa CĐHA (ID: 10) =================
('DV024', N'Đo Điện tâm đồ (ECG)', 'LDV04', 80000, N'Lần', 1, 1, N'Kiểm tra rối loạn nhịp tim.', 10),
('DV025', N'Nội soi Tai Mũi Họng ống mềm', 'LDV05', 200000, N'Lần', 1, 1, N'Nội soi không đau.', 10),
('DV026', N'Nội soi dạ dày không gây mê', 'LDV05', 600000, N'Lần', 1, 1, N'Kiểm tra dạ dày, tá tràng.', 10),

-- ================= NHÓM 5: THỦ THUẬT (LDV06 -> 09) -> Map về Khoa chuyên biệt =================
('DV027', N'Thay băng vết thương', 'LDV06', 80000, N'Lần', 1, 1, N'Chăm sóc vết mổ.', 3), 
('DV028', N'Khâu vết thương < 5cm', 'LDV06', 250000, N'Lần', 1, 1, N'Khâu thẩm mỹ gây tê.', 3), 
('DV029', N'Cạo vôi răng (Lấy cao răng)', 'LDV07', 250000, N'Lần', 0, 1, N'Làm sạch mảng bám răng.', 7), 
('DV030', N'Nhổ răng vĩnh viễn', 'LDV07', 300000, N'Răng', 1, 1, N'Nhổ răng sâu hỏng nặng.', 7), 
('DV031', N'Siêu âm thai 4D', 'LDV08', 350000, N'Lần', 0, 1, N'Khảo sát dị tật thai nhi.', 5), 
('DV032', N'Đo khúc xạ mắt tự động', 'LDV09', 50000, N'Lần', 1, 1, N'Kiểm tra độ cận, viễn.', 8), 
('DV033', N'Lấy nhân mụn chuẩn y khoa', 'LDV09', 400000, N'Lần', 0, 1, N'Điều trị mụn vô khuẩn.', 9); 
GO



INSERT INTO DANHMUC_THUOC (MaDanhMuc, TenDanhMuc, MoTa) VALUES 
('DM001', N'Thuốc kháng sinh', N'Thuốc diệt khuẩn, kìm khuẩn điều trị các bệnh nhiễm trùng'),
('DM002', N'Thuốc giảm đau, hạ sốt, chống viêm', N'Bao gồm NSAIDs, Paracetamol, Corticoid...'),
('DM003', N'Thuốc tim mạch - huyết áp', N'Điều trị cao huyết áp, suy tim, rối loạn nhịp tim'),
('DM004', N'Thuốc đường tiêu hóa', N'Thuốc dạ dày, tá tràng, chống nôn, tiêu chảy, nhuận tràng'),
('DM005', N'Thuốc đường hô hấp', N'Thuốc ho, long đờm, giãn phế quản, hen suyễn'),
('DM006', N'Thuốc dị ứng - Miễn dịch', N'Thuốc kháng Histamin, chống dị ứng, mẩn ngứa'),
('DM007', N'Vitamin và Khoáng chất', N'Các loại Vitamin A, B, C, D, Canxi, Sắt, Kẽm bổ sung'),
('DM008', N'Thuốc nội tiết - Đái tháo đường', N'Insulin, thuốc hạ đường huyết, hormone tuyến giáp'),
('DM009', N'Thuốc thần kinh - Tâm thần', N'Thuốc an thần, chống trầm cảm, rối loạn tuần hoàn não'),
('DM010', N'Thuốc dùng ngoài - Da liễu', N'Thuốc bôi ngoài da, trị nấm, trị viêm da cơ địa');
GO

INSERT INTO NHASANXUAT (TenNSX, DiaChi, SDT, Email, QuocGia) VALUES 
(N'Công ty Cổ phần Dược Hậu Giang (DHG Pharma)', N'288 Bis Nguyễn Văn Cừ, Cần Thơ', '02923891433', 'dhgpharma@dhgpharma.com.vn', N'Việt Nam'),
(N'Công ty Cổ phần Traphaco', N'75 Yên Ninh, Ba Đình, Hà Nội', '18006612', 'info@traphaco.com.vn', N'Việt Nam'),
(N'Công ty Cổ phần Xuất nhập khẩu Y tế Domesco', N'65A Đại lộ Đồng Khởi, Phú Mỹ, Bến Tre', '02753822129', 'domesco@domesco.com', N'Việt Nam'),
(N'Pfizer Inc.', N'New York, NY 10017, Hoa Kỳ', '+1-212-733-2323', 'contact@pfizer.com', N'Hoa Kỳ'),
(N'Sanofi', N'82 Avenue Raspail, Gentilly, Pháp', '+33-1-5377-4000', 'contact@sanofi.com', N'Pháp'),
(N'AstraZeneca', N'1 Francis Crick Avenue, Cambridge, Anh', '+44-20-3749-5000', 'info@astrazeneca.com', N'Vương quốc Anh'),
(N'Novartis AG', N'Lichtstrasse 35, Basel, Thụy Sĩ', '+41-61-324-1111', 'info@novartis.com', N'Thụy Sĩ'),
(N'GlaxoSmithKline (GSK)', N'980 Great West Road, Brentford, Anh', '+44-20-8047-5000', 'customer@gsk.com', N'Vương quốc Anh'),
(N'Công ty TNHH Dược phẩm Hisamitsu', N'Tashu, Saga, Nhật Bản', '+81-942-83-2101', 'info@hisamitsu.co.jp', N'Nhật Bản'),
(N'Abbott Laboratories', N'Abbott Park, Illinois, Hoa Kỳ', '+1-224-667-6100', 'contact@abbott.com', N'Hoa Kỳ');
GO

INSERT INTO KHO (TenKho, LoaiKho, DiaChi)
VALUES
(N'Kho tổng', N'Kho chính', N'Tầng 1'),
(N'Nhà thuốc', N'Kho bán lẻ', N'Tầng trệt'),
(N'Kho dự phòng', N'Kho phụ', N'Tầng 2');

-- ========================================================
-- INSERT DATA CHO BẢNG DANHMUC_BENH (BULK INSERT)
-- ========================================================

INSERT INTO DANHMUC_BENH (MaBenh, TenBenh, TrieuChung, MoTa, SoGiaiDoan) VALUES 
('80967001', N'Dental caries', NULL, NULL, 0),
('314529007', N'Medication review due (situation)', NULL, NULL, 0),
('82423001', N'Chronic pain (finding)', NULL, NULL, 0),
('278860009', N'Chronic low back pain (finding)', NULL, NULL, 0),
('224299000', N'Received higher education (finding)', NULL, NULL, 0),
('160903007', N'Full-time employment (finding)', NULL, NULL, 0),
('73595000', N'Stress (finding)', NULL, NULL, 0),
('10509002', N'Acute bronchitis (disorder)', NULL, NULL, 0),
('160968000', N'Risk activity involvement (finding)', NULL, NULL, 0),
('66383009', N'Gingivitis (disorder)', NULL, NULL, 0),
('24079001', N'Atopic dermatitis (disorder)', NULL, NULL, 0),
('125605004', N'Fracture of bone (disorder)', NULL, NULL, 0),
('359817006', N'Closed fracture of hip (disorder)', NULL, NULL, 0),
('444814009', N'Viral sinusitis (disorder)', NULL, NULL, 0),
('234949000', N'Tooth eruption disorder', NULL, NULL, 0),
('65966004', N'Fracture of forearm (disorder)', NULL, NULL, 0),
('72892002', N'Normal pregnancy (finding)', NULL, NULL, 0),
('1121000119', N'Chronic neck pain (finding)', NULL, NULL, 0),
('160904001', N'Part-time employment (finding)', NULL, NULL, 0),
('1093988100', N'Unhealthy alcohol drinking behavior (finding)', NULL, NULL, 0),
('423315002', N'Limited social contact (finding)', NULL, NULL, 0),
('266948004', N'Has a criminal record (finding)', NULL, NULL, 0),
('110030002', N'Concussion injury of brain (disorder)', NULL, NULL, 0),
('62106007', N'Concussion with no loss of consciousness (disorder)', NULL, NULL, 0),
('232353008', N'Perennial allergic rhinitis with seasonal variation (disorder)', NULL, NULL, 0),
('233678006', N'Childhood asthma (disorder)', NULL, NULL, 0),
('K08.9', N'Disorder of teeth and supporting structures  unspecified', NULL, NULL, 0),
('65363002', N'Otitis media (disorder)', NULL, NULL, 0),
('422650009', N'Social isolation (finding)', NULL, NULL, 0),
('741062008', N'Not in labor force (finding)', NULL, NULL, 0),
('183996000', N'Sterilization requested (situation)', NULL, NULL, 0),
('267020005', N'History of tubal ligation (situation)', NULL, NULL, 0),
('90460009', N'Injury of neck (disorder)', NULL, NULL, 0),
('39848009', N'Whiplash injury to neck (disorder)', NULL, NULL, 0),
('361055000', N'Misuses drugs (finding)', NULL, NULL, 0),
('56786000', N'Pulmonic valve stenosis (disorder)', NULL, NULL, 0),
('239873007', N'Osteoarthritis of knee (disorder)', NULL, NULL, 0),
('162864005', N'Body mass index 30+ - obesity (finding)', NULL, NULL, 0),
('714628002', N'Prediabetes (finding)', NULL, NULL, 0),
('713458007', N'Lack of access to transportation (finding)', NULL, NULL, 0),
('161744009', N'Past pregnancy history of miscarriage (situation)', NULL, NULL, 0),
('266934004', N'Transport problem (finding)', NULL, NULL, 0),
('271737000', N'Anemia (disorder)', NULL, NULL, 0),
('706893006', N'Victim of intimate partner abuse (finding)', NULL, NULL, 0),
('40055000', N'Chronic sinusitis (disorder)', NULL, NULL, 0),
('65510006', N'Generalised gingival recession', NULL, NULL, 0),
('718052004', N'Asymptomatic periapical periodontitis', NULL, NULL, 0),
('307426000', N'Acute infective cystitis (disorder)', NULL, NULL, 0),
('59621000', N'Essential hypertension (disorder)', NULL, NULL, 0),
('237602007', N'Metabolic syndrome X (disorder)', NULL, NULL, 0),
('S02.5XXA', N'Fracture of tooth (traumatic)  initial encounter for closed fracture', NULL, NULL, 0),
('M26.31', N'Crowding of fully erupted teeth', NULL, NULL, 0),
('K02.9', N'Dental caries  unspecified', NULL, NULL, 0),
('44054006', N'Diabetes mellitus type 2 (disorder)', NULL, NULL, 0),
('73438004', N'Unemployed (finding)', NULL, NULL, 0),
('302870006', N'Hypertriglyceridemia (disorder)', NULL, NULL, 0),
('K08.8', N'Other specified disorders of teeth and supporting structures', NULL, NULL, 0),
('195662009', N'Acute viral pharyngitis (disorder)', NULL, NULL, 0),
('197927001', N'Recurrent urinary tract infection (disorder)', NULL, NULL, 0),
('80583007', N'Severe anxiety (panic) (finding)', NULL, NULL, 0),
('43878008', N'Streptococcal sore throat (disorder)', NULL, NULL, 0),
('K00.6', N'Disturbances in tooth eruption', NULL, NULL, 0),
('398254007', N'Pre-eclampsia (disorder)', NULL, NULL, 0),
('424393004', N'Reports of violence in the environment (finding)', NULL, NULL, 0),
('312608009', N'Laceration - injury (disorder)', NULL, NULL, 0),
('283385000', N'Laceration of thigh (disorder)', NULL, NULL, 0),
('414545008', N'Ischemic heart disease (disorder)', NULL, NULL, 0),
('274531002', N'Abnormal findings diagnostic imaging heart+coronary circulat (finding)', NULL, NULL, 0),
('1187604002', N'Serving in military service (finding)', NULL, NULL, 0),
('7200002', N'Alcoholism (disorder)', NULL, NULL, 0),
('128613002', N'Seizure disorder (disorder)', NULL, NULL, 0),
('1290882004', N'History of seizure (situation)', NULL, NULL, 0),
('39898005', N'Sleep disorder (disorder)', NULL, NULL, 0),
('78275009', N'Obstructive sleep apnea syndrome (disorder)', NULL, NULL, 0),
('K08.499', N'Partial loss of teeth due to other specified cause  unspecified class', NULL, NULL, 0),
('Z51.0', N'Encounter for antineoplastic radiation therapy', NULL, NULL, 0),
('315268008', N'Suspected prostate cancer (situation)', NULL, NULL, 0),
('126906006', N'Neoplasm of prostate (disorder)', NULL, NULL, 0),
('92691004', N'Carcinoma in situ of prostate (disorder)', NULL, NULL, 0),
('68496003', N'Polyp of colon (disorder)', NULL, NULL, 0),
('K01.1', N'Impacted teeth', NULL, NULL, 0),
('431855005', N'Chronic kidney disease stage 1 (disorder)', NULL, NULL, 0),
('127013003', N'Disorder of kidney due to diabetes mellitus (disorder)', NULL, NULL, 0),
('384709000', N'Sprain (morphologic abnormality)', NULL, NULL, 0),
('44465007', N'Sprain of ankle (disorder)', NULL, NULL, 0),
('C77.0', N'Secondary and unspecified malignant neoplasm of lymph nodes of head  face and neck', NULL, NULL, 0),
('M27.8', N'Other specified diseases of jaws', NULL, NULL, 0),
('307731004', N'Injury of tendon of the rotator cuff of shoulder (disorder)', NULL, NULL, 0),
('431856006', N'Chronic kidney disease stage 2 (disorder)', NULL, NULL, 0),
('9078100011', N'Microalbuminuria due to type 2 diabetes mellitus (disorder)', NULL, NULL, 0),
('31642005', N'Acute gingivitis', NULL, NULL, 0),
('5689008', N'Chronic periodontitis', NULL, NULL, 0),
('K08.3', N'Retained dental root', NULL, NULL, 0),
('203082005', N'Fibromyalgia (disorder)', NULL, NULL, 0),
('196416002', N'Impacted molars (disorder)', NULL, NULL, 0),
('8004003', N'Teething syndrome', NULL, NULL, 0),
('473461003', N'Educated to high school level (finding)', NULL, NULL, 0),
('1241710001', N'Chronic intractable migraine without aura (disorder)', NULL, NULL, 0),
('1571410001', N'Proteinuria due to type 2 diabetes mellitus (disorder)', NULL, NULL, 0),
('201834006', N'Localized  primary osteoarthritis of the hand (disorder)', NULL, NULL, 0),
('718061004', N'Generalised severe chronic periodontitis', NULL, NULL, 0),
('6525002', N'Dependent drug abuse (disorder)', NULL, NULL, 0),
('80394007', N'Hyperglycemia (disorder)', NULL, NULL, 0),
('K04.90', N'Unspecified diseases of pulp and periapical tissues', NULL, NULL, 0),
('161665007', N'History of renal transplant (situation)', NULL, NULL, 0),
('M27.2', N'Inflammatory conditions of jaws', NULL, NULL, 0),
('1149222004', N'Overdose (disorder)', NULL, NULL, 0),
('48333001', N'Burn injury (morphologic abnormality)', NULL, NULL, 0),
('403191005', N'Partial thickness burn (disorder)', NULL, NULL, 0),
('840544004', N'Suspected disease caused by Severe acute respiratory coronavirus 2 (situation)', NULL, NULL, 0),
('49727002', N'Cough (finding)', NULL, NULL, 0),
('248595008', N'Sputum finding (finding)', NULL, NULL, 0),
('84229001', N'Fatigue (finding)', NULL, NULL, 0),
('68962001', N'Muscle pain (finding)', NULL, NULL, 0),
('57676002', N'Joint pain', NULL, NULL, 0),
('386661006', N'Fever (finding)', NULL, NULL, 0),
('36955009', N'Loss of taste (finding)', NULL, NULL, 0),
('840539006', N'Disease caused by severe acute respiratory syndrome coronavirus 2 (disorder)', NULL, NULL, 0),
('55822004', N'Hyperlipidemia (disorder)', NULL, NULL, 0),
('K04.0', N'Pulpitis', NULL, NULL, 0),
('88805009', N'Chronic congestive heart failure (disorder)', NULL, NULL, 0),
('399261000', N'History of coronary artery bypass grafting (situation)', NULL, NULL, 0),
('K02.53', N'Dental caries on pit and fissure surface penetrating into pulp', NULL, NULL, 0),
('26929004', N'Alzheimer''s disease (disorder)', NULL, NULL, 0),
('1551000119', N'Nonproliferative diabetic retinopathy due to type II diabetes mellitus', NULL, NULL, 0),
('707251008', N'Generalised chronic periodontitis', NULL, NULL, 0),
('224295006', N'Only received primary school education (finding)', NULL, NULL, 0),
('5602001', N'Opioid abuse', NULL, NULL, 0),
('16114001', N'Fracture of ankle (disorder)', NULL, NULL, 0),
('K04.7', N'Periapical abscess without sinus', NULL, NULL, 0),
('1085991000', N'Dental caries on smooth surface penetrating into dentin', NULL, NULL, 0),
('105531004', N'Housing unsatisfactory (finding)', NULL, NULL, 0),
('58150001', N'Fracture of clavicle (disorder)', NULL, NULL, 0),
('64859006', N'Osteoporosis (disorder)', NULL, NULL, 0),
('713197008', N'Recurrent rectal polyp (disorder)', NULL, NULL, 0),
('109838007', N'Overlapping malignant neoplasm of colon (disorder)', NULL, NULL, 0),
('K08.531', N'Fractured dental restorative material with loss of material', NULL, NULL, 0),
('K02.63', N'Dental caries on smooth surface penetrating into pulp', NULL, NULL, 0),
('C04.0', N'Malignant neoplasm of anterior floor of mouth', NULL, NULL, 0),
('C76.0', N'Malignant neoplasm of head  face and neck', NULL, NULL, 0),
('446654005', N'Refugee (person)', NULL, NULL, 0),
('105995000', N'Disorder of teeth AND/OR supporting structures', NULL, NULL, 0),
('90560007', N'Gout', NULL, NULL, 0),
('82212003', N'Erosion of teeth', NULL, NULL, 0),
('283545005', N'Gunshot wound (disorder)', NULL, NULL, 0),
('262574004', N'Bullet wound (disorder)', NULL, NULL, 0),
('K00.7', N'Teething syndrome', NULL, NULL, 0),
('2556008', N'Periodontal disease', NULL, NULL, 0),
('66569006', N'Retained dental root', NULL, NULL, 0),
('1172608001', N'Accretion on tooth', NULL, NULL, 0),
('283371005', N'Laceration of forearm (disorder)', NULL, NULL, 0),
('433144002', N'Chronic kidney disease stage 3 (disorder)', NULL, NULL, 0),
('36971009', N'Sinusitis (disorder)', NULL, NULL, 0),
('K08.109', N'Complete loss of teeth  unspecified cause  unspecified class', NULL, NULL, 0),
('M26.11', N'Maxillary asymmetry', NULL, NULL, 0),
('Z00.00', N'Encounter for general adult medical examination without abnormal findings', NULL, NULL, 0),
('Z13.1', N'Encounter for screening for diabetes mellitus', NULL, NULL, 0),
('K05.313', N'Chronic periodontitis  localized  severe', NULL, NULL, 0),
('83664006', N'Idiopathic atrophic hypothyroidism (disorder)', NULL, NULL, 0),
('431857002', N'Chronic kidney disease stage 4 (disorder)', NULL, NULL, 0),
('698306007', N'Awaiting transplantation of kidney (situation)', NULL, NULL, 0),
('K09.0', N'Developmental odontogenic cysts', NULL, NULL, 0),
('K03.81', N'Cracked tooth', NULL, NULL, 0),
('D16.4', N'Benign neoplasm of bones of skull and face', NULL, NULL, 0),
('428251008', N'History of appendectomy (situation)', NULL, NULL, 0),
('370247008', N'Facial laceration (disorder)', NULL, NULL, 0),
('C10.9', N'Malignant neoplasm of oropharynx  unspecified', NULL, NULL, 0),
('K02.62', N'Dental caries on smooth surface penetrating into dentin', NULL, NULL, 0),
('715859008', N'Localised moderate aggressive periodontitis', NULL, NULL, 0),
('32911000', N'Homeless (finding)', NULL, NULL, 0),
('19169002', N'Miscarriage in first trimester (disorder)', NULL, NULL, 0),
('156073000', N'Complete miscarriage (disorder)', NULL, NULL, 0),
('25064002', N'Headache (finding)', NULL, NULL, 0),
('267102003', N'Sore throat (finding)', NULL, NULL, 0),
('233604007', N'Pneumonia (disorder)', NULL, NULL, 0),
('389087006', N'Hypoxemia (disorder)', NULL, NULL, 0),
('271825005', N'Respiratory distress (finding)', NULL, NULL, 0),
('65710008', N'Acute respiratory failure (disorder)', NULL, NULL, 0),
('1322810001', N'Acute deep venous thrombosis (disorder)', NULL, NULL, 0),
('267036007', N'Dyspnea (finding)', NULL, NULL, 0),
('56018004', N'Wheezing (finding)', NULL, NULL, 0),
('422587007', N'Nausea (finding)', NULL, NULL, 0),
('249497008', N'Vomiting symptom (finding)', NULL, NULL, 0),
('401314000', N'Acute non-ST segment elevation myocardial infarction (disorder)', NULL, NULL, 0),
('399211009', N'History of myocardial infarction (situation)', NULL, NULL, 0),
('770349000', N'Sepsis caused by virus (disorder)', NULL, NULL, 0),
('706870000', N'Acute pulmonary embolism (disorder)', NULL, NULL, 0),
('70704007', N'Sprain of wrist (disorder)', NULL, NULL, 0),
('84114007', N'Heart failure (disorder)', NULL, NULL, 0),
('87628006', N'Bacterial infectious disease (disorder)', NULL, NULL, 0),
('203646004', N'Adolescent idiopathic scoliosis (disorder)', NULL, NULL, 0),
('446096008', N'Perennial allergic rhinitis (disorder)', NULL, NULL, 0),
('4356008', N'Gingival recession', NULL, NULL, 0),
('3685810001', N'Neuropathy due to type 2 diabetes mellitus (disorder)', NULL, NULL, 0),
('254837009', N'Malignant neoplasm of breast (disorder)', NULL, NULL, 0),
('K08.0', N'Exfoliation of teeth due to systemic causes', NULL, NULL, 0),
('K13.70', N'Unspecified lesions of oral mucosa', NULL, NULL, 0),
('K05.10', N'Chronic gingivitis  plaque induced', NULL, NULL, 0),
('C32.9', N'Malignant neoplasm of larynx  unspecified', NULL, NULL, 0),
('403190006', N'Epidermal burn of skin (disorder)', NULL, NULL, 0),
('198992004', N'Eclampsia in pregnancy (disorder)', NULL, NULL, 0),
('443165006', N'Osteoporotic fracture of bone (disorder)', NULL, NULL, 0),
('60234000', N'Aortic valve regurgitation (disorder)', NULL, NULL, 0),
('46177005', N'End-stage renal disease (disorder)', NULL, NULL, 0),
('284551006', N'Laceration of foot (disorder)', NULL, NULL, 0),
('1231000119', N'History of aortic valve replacement (situation)', NULL, NULL, 0),
('241929008', N'Acute allergic reaction (disorder)', NULL, NULL, 0),
('K08.409', N'Partial loss of teeth  unspecified cause  unspecified class', NULL, NULL, 0),
('K03.6', N'Deposits [accretions] on teeth', NULL, NULL, 0),
('196273001', N'Supplemental tooth', NULL, NULL, 0),
('91302008', N'Sepsis (disorder)', NULL, NULL, 0),
('62564004', N'Concussion with loss of consciousness (disorder)', NULL, NULL, 0),
('M26.52', N'Limited mandibular range of motion', NULL, NULL, 0),
('75498004', N'Acute bacterial sinusitis (disorder)', NULL, NULL, 0),
('230265002', N'Familial Alzheimer''s disease of early onset (disorder)', NULL, NULL, 0),
('C05.1', N'Malignant neoplasm of soft palate', NULL, NULL, 0),
('27942005', N'Shock (disorder)', NULL, NULL, 0),
('235104008', N'Impacted tooth', NULL, NULL, 0),
('40275004', N'Contact dermatitis (disorder)', NULL, NULL, 0),
('427419006', N'Transformed migraine (disorder)', NULL, NULL, 0),
('707252001', N'Localised chronic periodontitis', NULL, NULL, 0),
('35999006', N'Blighted ovum (disorder)', NULL, NULL, 0),
('73430006', N'Sleep apnea (disorder)', NULL, NULL, 0),
('C02.1', N'Malignant neoplasm of border of tongue', NULL, NULL, 0),
('125601008', N'Injury of knee (disorder)', NULL, NULL, 0),
('30832001', N'Rupture of patellar tendon (disorder)', NULL, NULL, 0),
('1085971000', N'Dental caries on pit and fissure surface penetrating into pulp', NULL, NULL, 0),
('K08.20', N'Unspecified atrophy of edentulous alveolar ridge', NULL, NULL, 0),
('C02.9', N'Malignant neoplasm of tongue  unspecified', NULL, NULL, 0),
('49436004', N'Atrial fibrillation (disorder)', NULL, NULL, 0),
('K13.79', N'Other lesions of oral mucosa', NULL, NULL, 0),
('429280009', N'History of amputation of foot (situation)', NULL, NULL, 0),
('263102004', N'Fracture subluxation of wrist (disorder)', NULL, NULL, 0),
('K13.29', N'Other disturbances of oral epithelium  including tongue', NULL, NULL, 0),
('444448004', N'Injury of medial collateral ligament of knee (disorder)', NULL, NULL, 0),
('79586000', N'Tubal pregnancy (disorder)', NULL, NULL, 0),
('74400008', N'Appendicitis (disorder)', NULL, NULL, 0),
('444470001', N'Injury of anterior cruciate ligament (disorder)', NULL, NULL, 0),
('54711002', N'Gingival enlargement', NULL, NULL, 0),
('408512008', N'Body mass index 40+ - severely obese (finding)', NULL, NULL, 0),
('401303003', N'Acute ST segment elevation myocardial infarction (disorder)', NULL, NULL, 0),
('C44.1222', N'Squamous cell carcinoma of skin of right lower eyelid  including canthus', NULL, NULL, 0),
('48724000', N'Mitral valve regurgitation (disorder)', NULL, NULL, 0),
('Z40.8', N'Encounter for other prophylactic surgery', NULL, NULL, 0),
('1085961000', N'Dental caries on pit and fissure surface penetrating into dentin', NULL, NULL, 0),
('K12.2', N'Cellulitis and abscess of mouth', NULL, NULL, 0),
('R69', N'Illness  unspecified', NULL, NULL, 0),
('192127007', N'Child attention deficit disorder (disorder)', NULL, NULL, 0),
('67782005', N'Acute respiratory distress syndrome (disorder)', NULL, NULL, 0),
('84757009', N'Epilepsy (disorder)', NULL, NULL, 0),
('K13.21', N'Leukoplakia of oral mucosa  including tongue', NULL, NULL, 0),
('87433001', N'Pulmonary emphysema (disorder)', NULL, NULL, 0),
('C06.9', N'Malignant neoplasm of mouth  unspecified', NULL, NULL, 0),
('C03.0', N'Malignant neoplasm of upper gum', NULL, NULL, 0),
('1085741000', N'Contour of existing restoration of tooth biologically incompatible with oral health', NULL, NULL, 0),
('22298006', N'Myocardial infarction (disorder)', NULL, NULL, 0),
('C09.9', N'Malignant neoplasm of tonsil  unspecified', NULL, NULL, 0),
('284549007', N'Laceration of hand (disorder)', NULL, NULL, 0),
('53963006', N'Excessive attrition of teeth', NULL, NULL, 0),
('K05.312', N'Chronic periodontitis  localized  moderate', NULL, NULL, 0),
('K04.1', N'Necrosis of pulp', NULL, NULL, 0),
('93761005', N'Primary malignant neoplasm of colon (disorder)', NULL, NULL, 0),
('K05.6', N'Periodontal disease  unspecified', NULL, NULL, 0),
('Z13.9', N'Encounter for screening  unspecified', NULL, NULL, 0),
('185086009', N'Chronic obstructive bronchitis (disorder)', NULL, NULL, 0),
('K05.321', N'Chronic periodontitis  generalized  slight', NULL, NULL, 0),
('127294003', N'Traumatic or nontraumatic brain injury (disorder)', NULL, NULL, 0),
('69896004', N'Rheumatoid arthritis (disorder)', NULL, NULL, 0),
('367498001', N'Seasonal allergic rhinitis (disorder)', NULL, NULL, 0),
('C05.0', N'Malignant neoplasm of hard palate', NULL, NULL, 0),
('C41.1', N'Malignant neoplasm of mandible', NULL, NULL, 0),
('K05.4', N'Periodontosis', NULL, NULL, 0),
('C19', N'Malignant neoplasm of rectosigmoid junction', NULL, NULL, 0),
('C03.1', N'Malignant neoplasm of lower gum', NULL, NULL, 0),
('K04.5', N'Chronic apical periodontitis', NULL, NULL, 0),
('1085981000', N'Dental caries on smooth surface limited to enamel', NULL, NULL, 0),
('K03.9', N'Disease of hard tissues of teeth  unspecified', NULL, NULL, 0),
('C07', N'Malignant neoplasm of parotid gland', NULL, NULL, 0),
('C44.42', N'Squamous cell carcinoma of skin of scalp and neck', NULL, NULL, 0),
('109735001', N'Dental restoration failure of marginal integrity', NULL, NULL, 0),
('C06.1', N'Malignant neoplasm of vestibule of mouth', NULL, NULL, 0),
('449868002', N'Smokes tobacco daily (finding)', NULL, NULL, 0),
('M26.213', N'Malocclusion  Angle''s class III', NULL, NULL, 0),
('33737001', N'Fracture of rib (disorder)', NULL, NULL, 0),
('M27.1', N'Giant cell granuloma  central', NULL, NULL, 0),
('49915006', N'Tricuspid valve stenosis (disorder)', NULL, NULL, 0),
('109747007', N'Cracked tooth', NULL, NULL, 0),
('93143009', N'Leukemia  disease (disorder)', NULL, NULL, 0),
('Z01.20', N'Encounter for dental examination and cleaning without abnormal findings', NULL, NULL, 0),
('1533510001', N'History of peripheral stem cell transplant (situation)', NULL, NULL, 0),
('162573006', N'Suspected lung cancer (situation)', NULL, NULL, 0),
('254637007', N'Non-small cell lung cancer (disorder)', NULL, NULL, 0),
('424132000', N'Non-small cell carcinoma of lung  TNM stage 1 (disorder)', NULL, NULL, 0),
('1085951000', N'Dental caries on pit and fissure surface limited to enamel', NULL, NULL, 0),
('254632001', N'Small cell carcinoma of lung (disorder)', NULL, NULL, 0),
('6781100011', N'Primary small cell malignant neoplasm of lung  TNM stage 1 (disorder)', NULL, NULL, 0),
('239872002', N'Osteoarthritis of hip (disorder)', NULL, NULL, 0),
('1086001000', N'Dental caries on smooth surface penetrating into pulp', NULL, NULL, 0),
('K11.7', N'Disturbances of salivary secretion', NULL, NULL, 0),
('370143000', N'Major depressive disorder (disorder)', NULL, NULL, 0),
('363406005', N'Malignant neoplasm of colon (disorder)', NULL, NULL, 0),
('D10.30', N'Benign neoplasm of unspecified part of mouth', NULL, NULL, 0),
('76571007', N'Septic shock (disorder)', NULL, NULL, 0),
('K08.411', N'Partial loss of teeth due to trauma  class I', NULL, NULL, 0),
('45816000', N'Pyelonephritis (disorder)', NULL, NULL, 0),
('K08.22', N'Moderate atrophy of the mandible', NULL, NULL, 0),
('C32.8', N'Malignant neoplasm of overlapping sites of larynx', NULL, NULL, 0),
('C06.2', N'Malignant neoplasm of retromolar area', NULL, NULL, 0),
('698303004', N'Awaiting transplantation of bone marrow (situation)', NULL, NULL, 0),
('1086310001', N'History of autologous bone marrow transplant (situation)', NULL, NULL, 0),
('K08.111', N'Complete loss of teeth due to trauma  class I', NULL, NULL, 0),
('262521009', N'Traumatic injury of spinal cord and/or vertebral column (disorder)', NULL, NULL, 0),
('15724005', N'Fracture of vertebral column without spinal cord injury (disorder)', NULL, NULL, 0),
('128188000', N'Cerebral palsy (disorder)', NULL, NULL, 0),
('22253000', N'Pain (finding)', NULL, NULL, 0),
('235595009', N'Gastroesophageal reflux disease (disorder)', NULL, NULL, 0),
('K05.323', N'Chronic periodontitis  generalized  severe', NULL, NULL, 0),
('C06.0', N'Malignant neoplasm of cheek mucosa', NULL, NULL, 0),
('K02.7', N'Dental root caries', NULL, NULL, 0),
('234466008', N'Acquired coagulation disorder (disorder)', NULL, NULL, 0),
('86175003', N'Injury of heart (disorder)', NULL, NULL, 0),
('59898000', N'Localised gingival recession', NULL, NULL, 0),
('698754002', N'Chronic paralysis due to lesion of spinal cord (disorder)', NULL, NULL, 0),
('C50.919', N'Malignant neoplasm of unspecified site of unspecified female breast', NULL, NULL, 0),
('876882001', N'Died in hospice (finding)', NULL, NULL, 0),
('M26.62', N'Arthralgia of temporomandibular joint', NULL, NULL, 0),
('85116003', N'Miscarriage in second trimester (disorder)', NULL, NULL, 0),
('37849005', N'Congenital uterine anomaly (disorder)', NULL, NULL, 0),
('M26.69', N'Other specified disorders of temporomandibular joint', NULL, NULL, 0),
('C90.00', N'Multiple myeloma not having achieved remission', NULL, NULL, 0),
('K04.4', N'Acute apical periodontitis of pulpal origin', NULL, NULL, 0),
('C02.2', N'Malignant neoplasm of ventral surface of tongue', NULL, NULL, 0),
('230690007', N'Cerebrovascular accident (disorder)', NULL, NULL, 0),
('D10.2', N'Benign neoplasm of floor of mouth', NULL, NULL, 0),
('C32.0', N'Malignant neoplasm of glottis', NULL, NULL, 0),
('D10.1', N'Benign neoplasm of tongue', NULL, NULL, 0),
('C41.9', N'Malignant neoplasm of bone and articular cartilage  unspecified', NULL, NULL, 0),
('K04.6', N'Periapical abscess with sinus', NULL, NULL, 0),
('9733100011', N'Macular edema and retinopathy due to type 2 diabetes mellitus (disorder)', NULL, NULL, 0),
('94503003', N'Metastatic malignant neoplasm to prostate (disorder)', NULL, NULL, 0),
('K05.30', N'Chronic periodontitis  unspecified', NULL, NULL, 0),
('K05.329', N'Chronic periodontitis  generalized  unspecified severity', NULL, NULL, 0),
('1501000119', N'Proliferative diabetic retinopathy due to type II diabetes mellitus', NULL, NULL, 0),
('47505003', N'Posttraumatic stress disorder (disorder)', NULL, NULL, 0),
('Z01.818', N'Encounter for other preprocedural examination', NULL, NULL, 0),
('M26.633', N'Articular disc disorder of bilateral temporomandibular joint', NULL, NULL, 0),
('Z41.8', N'Encounter for other procedures for purposes other than remedying health state', NULL, NULL, 0),
('267253006', N'Fetus with chromosomal abnormality (disorder)', NULL, NULL, 0),
('K08.401', N'Partial loss of teeth  unspecified cause  class I', NULL, NULL, 0),
('D48.5', N'Neoplasm of uncertain behavior of skin', NULL, NULL, 0),
('C15.9', N'Malignant neoplasm of esophagus  unspecified', NULL, NULL, 0),
('81629009', N'Traumatic dislocation of temporomandibular joint', NULL, NULL, 0),
('M26.603', N'Bilateral temporomandibular joint disorder  unspecified', NULL, NULL, 0),
('K05.21', N'Aggressive periodontitis  localized', NULL, NULL, 0),
('C10.2', N'Malignant neoplasm of lateral wall of oropharynx', NULL, NULL, 0),
('40095003', N'Injury of kidney (disorder)', NULL, NULL, 0),
('43724002', N'Chill (finding)', NULL, NULL, 0),
('4557003', N'Preinfarction syndrome (disorder)', NULL, NULL, 0),
('K08.25', N'Moderate atrophy of the maxilla', NULL, NULL, 0),
('C4A.39', N'Merkel cell carcinoma of other parts of face', NULL, NULL, 0),
('K05.322', N'Chronic periodontitis  generalized  moderate', NULL, NULL, 0),
('267060006', N'Diarrhea symptom (finding)', NULL, NULL, 0),
('68235000', N'Nasal congestion (finding)', NULL, NULL, 0),
('K05.32', N'Chronic periodontitis  generalized', NULL, NULL, 0),
('C32.1', N'Malignant neoplasm of supraglottis', NULL, NULL, 0),
('D00.0', N'Carcinoma in situ of lip  oral cavity and pharynx', NULL, NULL, 0),
('K05.5', N'Other periodontal diseases', NULL, NULL, 0),
('D10.3', N'Benign neoplasm of other and unspecified parts of mouth', NULL, NULL, 0),
('M27.49', N'Other cysts of jaw', NULL, NULL, 0),
('C05.9', N'Malignant neoplasm of palate  unspecified', NULL, NULL, 0),
('D16.5', N'Benign neoplasm of lower jaw bone', NULL, NULL, 0),
('M27.9', N'Disease of jaws  unspecified', NULL, NULL, 0),
('K05.222', N'Aggressive periodontitis  generalized  moderate', NULL, NULL, 0),
('161679004', N'History of artificial joint (situation)', NULL, NULL, 0),
('6072007', N'Bleeding from anus (disorder)', NULL, NULL, 0),
('236077008', N'Protracted diarrhea (finding)', NULL, NULL, 0),
('M27.63', N'Post-osseointegration mechanical failure of dental implant', NULL, NULL, 0),
('47693006', N'Rupture of appendix (disorder)', NULL, NULL, 0),
('60573004', N'Aortic valve stenosis (disorder)', NULL, NULL, 0),
('47222000', N'Abrasion of tooth', NULL, NULL, 0),
('266421008', N'Dental arch relationship anomaly', NULL, NULL, 0),
('K13.5', N'Oral submucous fibrosis', NULL, NULL, 0),
('C77.9', N'Secondary and unspecified malignant neoplasm of lymph node  unspecified', NULL, NULL, 0),
('609496007', N'Complication occurring during pregnancy (disorder)', NULL, NULL, 0),
('K08.24', N'Minimal atrophy of maxilla', NULL, NULL, 0),
('C11.0', N'Malignant neoplasm of superior wall of nasopharynx', NULL, NULL, 0),
('263172003', N'Fracture of mandible (disorder)', NULL, NULL, 0),
('403192003', N'Full thickness burn (disorder)', NULL, NULL, 0),
('195967001', N'Asthma (disorder)', NULL, NULL, 0),
('M26.4', N'Malocclusion  unspecified', NULL, NULL, 0),
('M26.622', N'Arthralgia of left temporomandibular joint', NULL, NULL, 0),
('K08.21', N'Minimal atrophy of the mandible', NULL, NULL, 0),
('K04.9', N'Other and unspecified diseases of pulp and periapical tissues', NULL, NULL, 0),
('371136004', N'Disorder of tooth development', NULL, NULL, 0),
('718062006', N'Generalised moderate chronic periodontitis', NULL, NULL, 0),
('C01', N'Malignant neoplasm of base of tongue', NULL, NULL, 0),
('K12.32', N'Oral mucositis (ulcerative) due to other drugs', NULL, NULL, 0),
('M26.63', N'Articular disc disorder of temporomandibular joint', NULL, NULL, 0),
('K08.81', N'Primary occlusal trauma', NULL, NULL, 0);
GO

USE QL_KhamBenhNgoaiTru;
GO

-- ===========================================
-- INSERT DANH MỤC HOẠT CHẤT
-- ===========================================
INSERT INTO DANHMUC_HOATCHAT (MaHoatChat, TenHoatChat) VALUES 
('HC00000001', N'Acetaminophen'),
('HC00000002', N'{24 (drospirenone'),
('HC00000003', N'ethinyl estradiol'),
('HC00000004', N'4 (inert ingredients'),
('HC00000005', N'Ibuprofen'),
('HC00000006', N'Meperidine Hydrochloride'),
('HC00000007', N'Fexofenadine hydrochloride'),
('HC00000008', N'NDA020800'),
('HC00000009', N'{7 (ethinyl estradiol'),
('HC00000010', N'norgestimate'),
('HC00000011', N'7 (ethinyl estradiol'),
('HC00000012', N'7 (inert ingredients'),
('HC00000013', N'budesonide'),
('HC00000014', N'albuterol'),
('HC00000015', N'Amoxicillin'),
('HC00000016', N'Etonogestrel'),
('HC00000017', N'Naproxen sodium'),
('HC00000018', N'21 DAY ethinyl estradiol'),
('HC00000019', N'Vitamin B12'),
('HC00000020', N'diphenhydrAMINE Hydrochloride'),
('HC00000021', N'cephalexin'),
('HC00000022', N'lisinopril'),
('HC00000023', N'{28 (norethindrone'),
('HC00000024', N'Hydrochlorothiazide'),
('HC00000025', N'Dextromethorphan Hydrobromide'),
('HC00000026', N'doxylamine succinate'),
('HC00000027', N'Chưa xác định'),
('HC00000028', N'sulfamethoxazole'),
('HC00000029', N'trimethoprim'),
('HC00000030', N'tropicamide'),
('HC00000031', N'Penicillin V Potassium'),
('HC00000032', N'levonorgestrel'),
('HC00000033', N'Clopidogrel'),
('HC00000034', N'Simvastatin'),
('HC00000035', N'Nitroglycerin'),
('HC00000036', N'hydrocodone bitartrate'),
('HC00000037', N'Sertraline'),
('HC00000038', N'Oxycodone Hydrochloride'),
('HC00000039', N'amLODIPine'),
('HC00000040', N'insulin isophane  human'),
('HC00000041', N'insulin  regular  human'),
('HC00000042', N'Codeine Phosphate'),
('HC00000043', N'Clavulanate'),
('HC00000044', N'84 (ethinyl estradiol'),
('HC00000045', N'ferrous sulfate'),
('HC00000046', N'{21 (ethinyl estradiol'),
('HC00000047', N'{5 (dienogest'),
('HC00000048', N'estradiol valerate'),
('HC00000049', N'17 (dienogest'),
('HC00000050', N'2 (estradiol valerate'),
('HC00000051', N'2 (inert ingredients'),
('HC00000052', N'Furosemide'),
('HC00000053', N'cefazolin'),
('HC00000054', N'Midazolam'),
('HC00000055', N'Rocuronium bromide'),
('HC00000056', N'isoflurane'),
('HC00000057', N'Galantamine'),
('HC00000058', N'buprenorphine'),
('HC00000059', N'naloxone'),
('HC00000060', N'Cefuroxime'),
('HC00000061', N'Alendronic acid'),
('HC00000062', N'ciprofloxacin'),
('HC00000063', N'desflurane'),
('HC00000064', N'Naproxen'),
('HC00000065', N'Allopurinol'),
('HC00000066', N'Doxycycline Monohydrate'),
('HC00000067', N'Abuse-Deterrent'),
('HC00000068', N'Levothyroxine Sodium'),
('HC00000069', N'norelgestromin'),
('HC00000070', N'proparacaine hydrochloride'),
('HC00000071', N'carvedilol'),
('HC00000072', N'atorvastatin'),
('HC00000073', N'losartan potassium'),
('HC00000074', N'prasugrel'),
('HC00000075', N'NDA020503'),
('HC00000076', N'vancomycin'),
('HC00000077', N'piperacillin'),
('HC00000078', N'tazobactam'),
('HC00000079', N'nitrofurantoin  macrocrystals'),
('HC00000080', N'nitrofurantoin  monohydrate'),
('HC00000081', N'Loratadine'),
('HC00000082', N'remifentanil'),
('HC00000083', N'Hydrocortisone'),
('HC00000084', N'cetirizine hydrochloride'),
('HC00000085', N'doxycycline hyclate'),
('HC00000086', N'heparin sodium  porcine'),
('HC00000087', N'predniSONE'),
('HC00000088', N'aspirin'),
('HC00000089', N'Diazepam'),
('HC00000090', N'sacubitril'),
('HC00000091', N'valsartan'),
('HC00000092', N'verapamil hydrochloride'),
('HC00000093', N'Warfarin Sodium'),
('HC00000094', N'Digoxin'),
('HC00000095', N'atenolol'),
('HC00000096', N'Chlorpheniramine Maleate'),
('HC00000097', N'Astemizole'),
('HC00000098', N'Donepezil hydrochloride'),
('HC00000099', N'tramadol hydrochloride'),
('HC00000100', N'Alteplase'),
('HC00000101', N'salmeterol'),
('HC00000102', N'Tamoxifen'),
('HC00000103', N'ribociclib'),
('HC00000104', N'ticagrelor'),
('HC00000105', N'metoprolol tartrate'),
('HC00000106', N'palbociclib'),
('HC00000107', N'clonazePAM'),
('HC00000108', N'Methotrexate'),
('HC00000109', N'cycloSPORINE  modified'),
('HC00000110', N'NDA021457'),
('HC00000111', N'Cisplatin'),
('HC00000112', N'PACLitaxel'),
('HC00000113', N'etoposide'),
('HC00000114', N'trastuzumab-oysk'),
('HC00000115', N'FLUoxetine'),
('HC00000116', N'Leucovorin'),
('HC00000117', N'cefpodoxime'),
('HC00000118', N'Memantine hydrochloride'),
('HC00000119', N'Methylphenidate Hydrochloride'),
('HC00000120', N'Naltrexone hydrochloride'),
('HC00000121', N'NDA020983'),
('HC00000122', N'Azithromycin'),
('HC00000123', N'enalapril maleate'),
('HC00000124', N'Terfenadine'),
('HC00000125', N'carbamazepine'),
('HC00000126', N'rosuvastatin calcium'),
('HC00000127', N'Breath-Actuated'),
('HC00000128', N'ezetimibe'),
('HC00000129', N'clindamycin'),
('HC00000130', N'atomoxetine'),
('HC00000131', N'fosfomycin'),
('HC00000132', N'Penicillin G'),
('HC00000133', N'sevoflurane');
GO

-- ===========================================
-- INSERT THUỐC
-- ===========================================
INSERT INTO THUOC (MaThuoc, TenThuoc, QuyCach, DonViCoBan, MaLoaiThuoc, DuongDung, GiaBan, CoBHYT, MaNSX, TrangThai) VALUES 
('TH00000001', N'Thuốc Acetaminophen 325 mg (Viên nén uống)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 1200, 0, 6, 1),
('TH00000002', N'Thuốc tránh thai Drospirenone 3mg / Ethinyl Estradiol 0.02mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 11500, 0, 4, 1),
('TH00000003', N'Thuốc giảm đau Ibuprofen 100 mg (Viên nén uống)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 2200, 1, 5, 1),
('TH00000004', N'Thuốc Meperidine Hydrochloride 50 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 8500, 0, 6, 1),
('TH00000005', N'Thuốc Ibuprofen 200 mg (Viên nén uống)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 3200, 0, 4, 1),
('TH00000006', N'Thuốc Fexofenadine hydrochloride 30 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 5500, 0, 10, 1),
('TH00000007', N'Bút tiêm tự động Epinephrine 1 mg/ml (0.3 ml)', N'Chai 100ml', N'Chai', 'DM010', N'Uống', 155000, 0, 4, 1),
('TH00000008', N'Thuốc tránh thai Ethinyl Estradiol / Norgestimate', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 9500, 0, 4, 1),
('TH00000009', N'Hỗn dịch hít Budesonide 0.125 mg/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 185000, 1, 1, 1),
('TH00000010', N'Dung dịch hít Albuterol 0.83 mg/ml', N'Chai 100ml', N'Chai', 'DM004', N'Uống', 125000, 1, 10, 1),
('TH00000011', N'Thuốc kháng sinh Amoxicillin 500 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 2500, 0, 6, 1),
('TH00000012', N'Thuốc Acetaminophen 160 mg (Viên nhai)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 3000, 1, 2, 1),
('TH00000013', N'Que cấy tránh thai Etonogestrel 68 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 550000, 0, 8, 1),
('TH00000014', N'Thuốc Naproxen sodium 220 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 4500, 0, 10, 1),
('TH00000015', N'Vòng đặt âm đạo Ethinyl Estradiol / Etonogestrel', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 450000, 1, 2, 1),
('TH00000016', N'Dung dịch tiêm Vitamin B12 5 mg/ml', N'Chai 100ml', N'Chai', 'DM006', N'Uống', 15000, 0, 2, 1),
('TH00000017', N'Thuốc Diphenhydramine Hydrochloride 25 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 2500, 1, 7, 1),
('TH00000018', N'Thuốc kháng sinh Cephalexin 500 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 3500, 0, 4, 1),
('TH00000019', N'Thuốc lisinopril 10 mg (Viên nén uống)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 1500, 0, 9, 1),
('TH00000020', N'Thuốc Norethindrone 0.35 mg (Gói 28 viên)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 8500, 0, 4, 1),
('TH00000021', N'Thuốc Hydrochlorothiazide 25 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 1200, 0, 10, 1),
('TH00000022', N'Hỗn hợp Acetaminophen / Dextromethorphan / Doxylamine', N'Chai 100ml', N'Chai', 'DM006', N'Uống', 65000, 1, 5, 1),
('TH00000023', N'Thuốc tiêm Medroxyprogesterone Acetate 150 mg/ml', N'Chai 100ml', N'Chai', 'DM004', N'Uống', 185000, 0, 4, 1),
('TH00000024', N'Thuốc Sulfamethoxazole 800 mg / Trimethoprim 160 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 4500, 0, 9, 1),
('TH00000025', N'Dung dịch nhỏ mắt Tropicamide 5 mg/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 85000, 1, 10, 1),
('TH00000026', N'Thuốc kháng sinh Penicillin V Potassium 500 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 2200, 0, 6, 1),
('TH00000027', N'Dụng cụ tử cung Levonorgestrel [Mirena]', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 3500000, 0, 2, 1),
('TH00000028', N'Thuốc chống đông máu Clopidogrel 75 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 15500, 1, 7, 1),
('TH00000029', N'Thuốc Simvastatin 20 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 4500, 1, 1, 1),
('TH00000030', N'Thuốc Metoprolol succinate 100 mg (Giải phóng kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 8500, 1, 8, 1),
('TH00000031', N'Thuốc xịt niêm mạc Nitroglycerin 0.4 mg/liều', N'Lọ 120 liều', N'Lọ', 'DM001', N'Xịt/Hít', 320000, 1, 7, 1),
('TH00000032', N'Thuốc Acetaminophen 300 mg / Hydrocodone 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 18500, 1, 3, 1),
('TH00000033', N'Thuốc Sertraline 100 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 12500, 1, 2, 1),
('TH00000034', N'Thuốc Acetaminophen 325 mg / Oxycodone 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 25000, 1, 4, 1),
('TH00000035', N'Thuốc Acetaminophen 325 mg (Tylenol)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 2500, 1, 7, 1),
('TH00000036', N'Thuốc Amlodipine 2.5 mg (Huyết áp)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 1500, 1, 1, 1),
('TH00000037', N'Thuốc tiêm DOCEtaxel 20 mg/ml', N'Chai 100ml', N'Chai', 'DM009', N'Uống', 1250000, 1, 1, 1),
('TH00000038', N'Bơm tiêm Leuprolide Acetate 30 mg/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 3500000, 1, 7, 1),
('TH00000039', N'Hỗn dịch tiêm Insulin Isophane (Humulin)', N'Chai 100ml', N'Chai', 'DM008', N'Uống', 285000, 1, 6, 1),
('TH00000040', N'Thuốc Acetaminophen 300 mg / Codeine 15 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 12500, 0, 6, 1),
('TH00000041', N'Miếng dán thẩm thấu Fentanyl 0.025 mg/h', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 195000, 0, 2, 1),
('TH00000042', N'Thuốc kháng sinh Amoxicillin / Clavulanate 125mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 14500, 1, 4, 1),
('TH00000043', N'Thuốc giảm đau Acetaminophen 325 mg / Oxycodone 5 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 22000, 0, 1, 1),
('TH00000044', N'Dụng cụ tử cung Levonorgestrel [Kyleena]', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 4500000, 0, 9, 1),
('TH00000045', N'Liệu trình tránh thai Ethinyl Estradiol / Levonorgestrel', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 15500, 1, 9, 1),
('TH00000046', N'Thuốc sắt Ferrous Sulfate 325 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 1000, 1, 10, 1),
('TH00000047', N'Thuốc Tacrolimus 1 mg (Giải phóng kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 65000, 1, 7, 1),
('TH00000048', N'Thuốc tiểu đường Metformin 500 mg (Hỗ trợ kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 3500, 1, 7, 1),
('TH00000049', N'Thuốc kháng sinh Amoxicillin 250 mg (Viên nang)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 2000, 0, 5, 1),
('TH00000050', N'Thuốc tránh thai Ethinyl Estradiol / Levonorgestrel', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 12500, 1, 4, 1),
('TH00000051', N'Hệ thống giải phóng tử cung Levonorgestrel [Liletta]', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 4200000, 1, 3, 1),
('TH00000052', N'Thuốc Simvastatin 10 mg (Hạ mỡ máu)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 2500, 1, 7, 1),
('TH00000053', N'Liệu trình Estradiol Valerate / Dienogest', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 18500, 1, 8, 1),
('TH00000054', N'Thuốc lợi tiểu Furosemide 40 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 1200, 0, 7, 1),
('TH00000055', N'Dung dịch tiêm Furosemide 10 mg/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 25000, 0, 8, 1),
('TH00000056', N'Thuốc tiêm kháng sinh Cefazolin 2000 mg', N'Hộp 10 ống', N'Ống', 'DM010', N'Tiêm', 65000, 1, 6, 1),
('TH00000057', N'Thuốc tiêm Heparin Sodium 5000 đơn vị/ml', N'Chai 100ml', N'Chai', 'DM004', N'Uống', 45000, 1, 7, 1),
('TH00000058', N'Thuốc tiêm Protamine Sulfate 10 mg/ml', N'Chai 100ml', N'Chai', 'DM009', N'Uống', 155000, 1, 10, 1),
('TH00000059', N'Dung dịch tiêm Insulin Regular 100 đơn vị/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 215000, 1, 7, 1),
('TH00000060', N'Dung dịch tiêm Ondansetron 2 mg/ml', N'Chai 100ml', N'Chai', 'DM001', N'Uống', 35000, 1, 10, 1),
('TH00000061', N'Dung dịch tiêm Midazolam 1 mg/ml', N'Chai 100ml', N'Chai', 'DM001', N'Uống', 45000, 1, 10, 1),
('TH00000062', N'Dung dịch tiêm Propofol 10 mg/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 225000, 1, 6, 1),
('TH00000063', N'Dung dịch tiêm Rocuronium bromide 10 mg/ml', N'Chai 100ml', N'Chai', 'DM006', N'Uống', 185000, 1, 2, 1),
('TH00000064', N'Dung dịch hít Isoflurane 999 mg/ml', N'Chai 100ml', N'Chai', 'DM006', N'Uống', 245000, 1, 10, 1),
('TH00000065', N'Dung dịch tiêm SUFentanil 0.05 mg/ml', N'Chai 100ml', N'Chai', 'DM001', N'Uống', 155000, 1, 2, 1),
('TH00000066', N'Thuốc Galantamine 4 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 12500, 1, 4, 1),
('TH00000067', N'Thuốc Acetaminophen 300 mg / Hydrocodone 5 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 14500, 0, 3, 1),
('TH00000068', N'Thuốc đặt dưới lưỡi Buprenorphine / Naloxone', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 35000, 1, 1, 1),
('TH00000069', N'Thuốc kháng sinh Cefuroxime 250 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 8500, 1, 2, 1),
('TH00000070', N'Thuốc điều trị loãng xương Alendronic acid 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 22000, 0, 1, 1),
('TH00000071', N'Thuốc kháng sinh Ciprofloxacin 500 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 7500, 0, 7, 1),
('TH00000072', N'Dung dịch hít Desflurane 1000 mg/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 450000, 1, 1, 1),
('TH00000073', N'Dung dịch tiêm Fentanyl 0.05 mg/ml', N'Chai 100ml', N'Chai', 'DM004', N'Uống', 55000, 1, 8, 1),
('TH00000074', N'Thuốc kháng viêm Ibuprofen 400 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 4500, 0, 4, 1),
('TH00000075', N'Thuốc giảm đau Naproxen 500 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 6500, 1, 4, 1),
('TH00000076', N'Thuốc Gout Allopurinol 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 2500, 1, 5, 1),
('TH00000077', N'Thuốc kháng sinh Doxycycline Monohydrate 50 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 8500, 0, 3, 1),
('TH00000078', N'Thuốc kháng sinh Penicillin V Potassium 250 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 2500, 0, 9, 1),
('TH00000079', N'Thuốc giảm đau Oxycodone Hydrochloride 15 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 45000, 0, 10, 1),
('TH00000080', N'Thuốc tuyến giáp Levothyroxine Sodium 0.075 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 3500, 1, 7, 1),
('TH00000081', N'Dung dịch tiêm Epoetin Alfa 4000 đơn vị/ml', N'Chai 100ml', N'Chai', 'DM008', N'Uống', 650000, 1, 1, 1),
('TH00000082', N'Gói tránh thai Norethindrone 0.35 mg [Errin]', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 12500, 0, 3, 1),
('TH00000083', N'Miếng dán tránh thai Ethinyl Estradiol / Norelgestromin', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 225000, 1, 1, 1),
('TH00000084', N'Bơm tiêm Enoxaparin sodium 100 mg/ml', N'Chai 100ml', N'Chai', 'DM006', N'Uống', 145000, 0, 3, 1),
('TH00000085', N'Thuốc giảm đau Acetaminophen 500 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 1500, 0, 9, 1),
('TH00000086', N'Bơm tiêm Enoxaparin sodium 150 mg/ml', N'Chai 100ml', N'Chai', 'DM007', N'Uống', 185000, 0, 6, 1),
('TH00000087', N'Dung dịch tiêm Tacrolimus 5 mg/ml [Prograf]', N'Chai 100ml', N'Chai', 'DM008', N'Uống', 355000, 1, 5, 1),
('TH00000088', N'Dung dịch nhỏ mắt Proparacaine hydrochloride', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 65000, 1, 9, 1),
('TH00000089', N'Thuốc Carvedilol 12.5 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 3500, 1, 5, 1),
('TH00000090', N'Thuốc hạ mỡ máu Atorvastatin 20 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 7500, 1, 9, 1),
('TH00000091', N'Thuốc Losartan potassium 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 6500, 1, 4, 1),
('TH00000092', N'Thuốc Prasugrel 10 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 18500, 1, 5, 1),
('TH00000093', N'Ống hít Albuterol 0.09 mg/liều (200 liều)', N'Lọ 120 liều', N'Lọ', 'DM001', N'Xịt/Hít', 125000, 1, 10, 1),
('TH00000094', N'Thuốc tiêm kháng sinh Vancomycin 1000 mg', N'Hộp 10 ống', N'Ống', 'DM002', N'Tiêm', 145000, 1, 5, 1),
('TH00000095', N'Thuốc tiêm Piperacillin / Tazobactam', N'Hộp 10 ống', N'Ống', 'DM004', N'Tiêm', 165000, 1, 1, 1),
('TH00000096', N'Viên nang Nitrofurantoin 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 8500, 0, 6, 1),
('TH00000097', N'Thuốc dị ứng Loratadine 5 mg (Viên nhai)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 4500, 0, 8, 1),
('TH00000098', N'Thuốc tiêm Remifentanil 2 mg', N'Hộp 10 ống', N'Ống', 'DM006', N'Tiêm', 255000, 1, 2, 1),
('TH00000099', N'Kem bôi ngoài da Hydrocortisone 10 mg/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 55000, 1, 8, 1),
('TH00000100', N'Thuốc Loratadine 10 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 3500, 1, 2, 1),
('TH00000101', N'Thuốc kháng sinh Ciprofloxacin 250 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 6500, 0, 10, 1),
('TH00000102', N'Thuốc Cetirizine hydrochloride 5 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 4500, 1, 3, 1),
('TH00000103', N'Thuốc kháng sinh Doxycycline Hyclate 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 7500, 0, 2, 1),
('TH00000104', N'Dung dịch tiêm Heparin Sodium 1000 đơn vị/ml', N'Chai 100ml', N'Chai', 'DM008', N'Uống', 85000, 1, 10, 1),
('TH00000105', N'Dung dịch tiêm Alfentanil 0.5 mg/ml', N'Chai 100ml', N'Chai', 'DM007', N'Uống', 125000, 0, 8, 1),
('TH00000106', N'Thuốc kháng viêm Prednisone 5 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 1500, 1, 1, 1),
('TH00000107', N'Viên nang Aspirin 81 mg [Vazalore]', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 2500, 1, 5, 1),
('TH00000108', N'Dung dịch tiêm Diazepam 5 mg/ml', N'Chai 100ml', N'Chai', 'DM009', N'Uống', 45000, 0, 8, 1),
('TH00000109', N'Thuốc Carvedilol 25 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 4500, 0, 4, 1),
('TH00000110', N'Thuốc Lisinopril 20 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 3500, 0, 2, 1),
('TH00000111', N'Thuốc Sacubitril 97 mg / Valsartan 103 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 45000, 1, 8, 1),
('TH00000112', N'Viên nang Hydrocodone Bitartrate 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 35000, 0, 2, 1),
('TH00000113', N'Thuốc Cetirizine hydrochloride 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 6500, 1, 8, 1),
('TH00000114', N'Ống hít Fluticasone propionate 0.044 mg/liều', N'Lọ 120 liều', N'Lọ', 'DM004', N'Xịt/Hít', 185000, 0, 9, 1),
('TH00000115', N'Thuốc tiêm Piperacillin / Tazobactam 250mg', N'Hộp 10 ống', N'Ống', 'DM007', N'Tiêm', 115000, 0, 2, 1),
('TH00000116', N'Dung dịch tiêm Vancomycin 5 mg/ml', N'Chai 100ml', N'Chai', 'DM008', N'Uống', 195000, 0, 4, 1),
('TH00000117', N'Thuốc Losartan potassium 50 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 4500, 1, 3, 1),
('TH00000118', N'Thuốc Aspirin 81 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 1200, 0, 1, 1),
('TH00000119', N'Thuốc Metoprolol succinate 25 mg (Kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 6500, 0, 6, 1),
('TH00000120', N'Thuốc hạ mỡ máu Atorvastatin 40 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 8500, 0, 5, 1),
('TH00000121', N'Thuốc Lisinopril 40 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 5500, 0, 9, 1),
('TH00000122', N'Thuốc Amoxicillin 875 mg / Clavulanate 125 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 18500, 1, 9, 1),
('TH00000123', N'Thuốc Verapamil hydrochloride 80 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 7500, 0, 5, 1),
('TH00000124', N'Thuốc chống đông máu Warfarin Sodium 5 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 3500, 0, 5, 1),
('TH00000125', N'Thuốc trợ tim Digoxin 0.125 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 4500, 1, 4, 1),
('TH00000126', N'Hỗn dịch hít Budesonide 0.25 mg/ml', N'Chai 100ml', N'Chai', 'DM007', N'Uống', 95000, 1, 10, 1),
('TH00000127', N'Gói tránh thai Norethindrone 0.35 mg [Jolivette]', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 15500, 1, 3, 1),
('TH00000128', N'Thuốc Atenolol 25 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 3500, 0, 3, 1),
('TH00000129', N'Thuốc Simvastatin 40 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 6500, 0, 4, 1),
('TH00000130', N'Thuốc Metoprolol succinate 50 mg (Kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 8500, 0, 2, 1),
('TH00000131', N'Thuốc hạ mỡ máu Atorvastatin 80 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 12500, 0, 7, 1),
('TH00000132', N'Siro Chlorpheniramine Maleate 2 mg/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 35000, 1, 4, 1),
('TH00000133', N'Thuốc Astemizole 10 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 4500, 0, 6, 1),
('TH00000134', N'Thuốc Donepezil hydrochloride 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 18500, 1, 10, 1),
('TH00000135', N'Thuốc Donepezil hydrochloride 23 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 22500, 1, 4, 1),
('TH00000136', N'Thuốc giảm đau Tramadol 50 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 12500, 1, 9, 1),
('TH00000137', N'Thuốc Amoxicillin 500 mg / Clavulanate 125 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 14500, 0, 8, 1),
('TH00000138', N'Dung dịch tiêm Norepinephrine 1 mg/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 115000, 0, 4, 1),
('TH00000139', N'Thuốc Acetaminophen 325 mg / Hydrocodone 7.5 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 18500, 0, 9, 1),
('TH00000140', N'Thuốc tiêm Alteplase 100 mg (Tiêu sợi huyết)', N'Hộp 10 ống', N'Ống', 'DM001', N'Tiêm', 2500000, 1, 4, 1),
('TH00000141', N'Dung dịch hít Albuterol 5 mg/ml', N'Chai 100ml', N'Chai', 'DM010', N'Uống', 95000, 0, 1, 1),
('TH00000142', N'Bình xịt Fluticasone / Salmeterol (Khô)', N'Lọ 120 liều', N'Lọ', 'DM001', N'Xịt/Hít', 355000, 1, 1, 1),
('TH00000143', N'Thuốc Tamoxifen 10 mg (Điều trị ung thư)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 22500, 1, 10, 1),
('TH00000144', N'Thuốc Ribociclib 200 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 1250000, 1, 3, 1),
('TH00000145', N'Thuốc Aspirin 325 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 1200, 1, 4, 1),
('TH00000146', N'Dung dịch tiêm Morphine Sulfate 1 mg/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 55000, 1, 4, 1),
('TH00000147', N'Dung dịch tiêm Heparin Sodium 100 đơn vị/ml', N'Chai 100ml', N'Chai', 'DM003', N'Uống', 35000, 1, 7, 1),
('TH00000148', N'Thuốc Ticagrelor 90 mg (Chống kết tập tiểu cầu)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 25500, 1, 5, 1),
('TH00000149', N'Thuốc Metoprolol tartrate 25 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 3500, 1, 2, 1),
('TH00000150', N'Thuốc Lisinopril 2.5 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 1200, 1, 8, 1),
('TH00000151', N'Dung dịch tiêm Epirubicin Hydrochloride 2 mg/ml', N'Chai 100ml', N'Chai', 'DM004', N'Uống', 1450000, 0, 1, 1),
('TH00000152', N'Thuốc Metoprolol tartrate 50 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 5500, 1, 7, 1),
('TH00000153', N'Bơm tiêm Fulvestrant 50 mg/ml', N'Chai 100ml', N'Chai', 'DM001', N'Uống', 850000, 1, 8, 1),
('TH00000154', N'Viên nang Palbociclib 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 1150000, 1, 2, 1),
('TH00000155', N'Thuốc Clonazepam 0.25 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 6500, 0, 9, 1),
('TH00000156', N'Dung dịch hít Albuterol 0.417 mg/ml', N'Chai 100ml', N'Chai', 'DM007', N'Uống', 75000, 0, 4, 1),
('TH00000157', N'Thuốc Methotrexate 2.5 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM001', N'Uống', 12500, 0, 4, 1),
('TH00000158', N'Dung dịch hít Albuterol 0.21 mg/ml', N'Chai 100ml', N'Chai', 'DM004', N'Uống', 75000, 1, 3, 1),
('TH00000159', N'Viên nang Cyclosporine 100 mg (Cải tiến)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 55000, 1, 4, 1),
('TH00000160', N'Thuốc Valsartan 160 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 14500, 1, 4, 1),
('TH00000161', N'Miếng dán cai thuốc Nicotine 0.292 mg/h', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 45000, 0, 9, 1),
('TH00000162', N'Thuốc Losartan potassium 25 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 3500, 1, 6, 1),
('TH00000163', N'Hỗn dịch hít Budesonide (Pulmicort)', N'Chai 100ml', N'Chai', 'DM006', N'Uống', 95000, 0, 1, 1),
('TH00000164', N'Thuốc Diazepam 5 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 4500, 1, 10, 1),
('TH00000165', N'Bình xịt Fluticasone 0.11 mg/liều (120 liều)', N'Lọ 120 liều', N'Lọ', 'DM003', N'Xịt/Hít', 185000, 1, 4, 1),
('TH00000166', N'Bình xịt Albuterol 0.09 mg/liều (ProAir)', N'Lọ 120 liều', N'Lọ', 'DM001', N'Xịt/Hít', 175000, 1, 7, 1),
('TH00000167', N'Thuốc đặt dưới lưỡi Nitroglycerin 0.4 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 6500, 0, 7, 1),
('TH00000168', N'Thuốc tiêm Cisplatin 50 mg', N'Hộp 10 ống', N'Ống', 'DM010', N'Tiêm', 350000, 1, 7, 1),
('TH00000169', N'Thuốc tiêm PACLitaxel 100 mg', N'Hộp 10 ống', N'Ống', 'DM001', N'Tiêm', 450000, 1, 5, 1),
('TH00000170', N'Thuốc Cefaclor 500 mg (Giải phóng kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 18500, 1, 4, 1),
('TH00000171', N'Thuốc tiêm Etoposide 100 mg [Etopophos]', N'Hộp 10 ống', N'Ống', 'DM001', N'Tiêm', 155000, 1, 8, 1),
('TH00000172', N'Thuốc tiêm Hyaluronidase / Trastuzumab', N'Chai 100ml', N'Chai', 'DM001', N'Uống', 1250000, 1, 8, 1),
('TH00000173', N'Viên nang Fluoxetine 20 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 9500, 0, 2, 1),
('TH00000174', N'Thuốc tiêm Leucovorin 100 mg', N'Hộp 10 ống', N'Ống', 'DM007', N'Tiêm', 125000, 1, 7, 1),
('TH00000175', N'Dung dịch tiêm Oxaliplatin 5 mg/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 255000, 1, 3, 1),
('TH00000176', N'Thuốc kháng sinh Cefpodoxime 200 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 15500, 0, 8, 1),
('TH00000177', N'Thuốc Oxycodone Hydrochloride 10 mg (Kéo dài)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM004', N'Uống', 22500, 0, 5, 1),
('TH00000178', N'Siro Memantine hydrochloride 2 mg/ml', N'Chai 100ml', N'Chai', 'DM009', N'Uống', 185000, 1, 8, 1),
('TH00000179', N'Thuốc Methylphenidate Hydrochloride 20 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 3500, 0, 6, 1),
('TH00000180', N'Viên nang Donepezil 10 mg / Memantine 28 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM010', N'Uống', 45000, 0, 4, 1),
('TH00000181', N'Thuốc Naltrexone hydrochloride 50 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 25500, 1, 3, 1),
('TH00000182', N'Viên nang Tacrolimus 5 mg (Astagraf)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM009', N'Uống', 65000, 1, 9, 1),
('TH00000183', N'Bình xịt Albuterol 0.09 mg/liều (Ventolin)', N'Lọ 120 liều', N'Lọ', 'DM002', N'Xịt/Hít', 175000, 1, 2, 1),
('TH00000184', N'Viên nang Azithromycin 250 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 12500, 1, 10, 1),
('TH00000185', N'Thuốc Enalapril maleate 20 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 3500, 1, 6, 1),
('TH00000186', N'Thuốc Nitrofurantoin Macrocrystals 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 9500, 1, 5, 1),
('TH00000187', N'Dung dịch tiêm Bevacizumab 25 mg/ml', N'Chai 100ml', N'Chai', 'DM009', N'Uống', 2500000, 1, 6, 1),
('TH00000188', N'Thuốc Terfenadine 60 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 3500, 0, 3, 1),
('TH00000189', N'Hỗn dịch uống Carbamazepine 20 mg/ml', N'Chai 100ml', N'Chai', 'DM002', N'Uống', 125000, 1, 2, 1),
('TH00000190', N'Thuốc hạ mỡ máu Rosuvastatin 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM006', N'Uống', 12500, 1, 6, 1),
('TH00000191', N'Bình xịt Beclomethasone 0.04 mg/liều (Qvar)', N'Lọ 120 liều', N'Lọ', 'DM005', N'Xịt/Hít', 225000, 1, 10, 1),
('TH00000192', N'Bơm tiêm Fondaparinux sodium 12.5 mg/ml', N'Chai 100ml', N'Chai', 'DM005', N'Uống', 185000, 1, 4, 1),
('TH00000193', N'Thuốc Ezetimibe 10 mg / Simvastatin 40 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM005', N'Uống', 18500, 1, 5, 1),
('TH00000194', N'Viên nang kháng sinh Clindamycin 300 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 9500, 1, 7, 1),
('TH00000195', N'Viên nang Atomoxetine 100 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM007', N'Uống', 45000, 0, 1, 1),
('TH00000196', N'Thuốc Chlorpheniramine Maleate 4 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 1500, 0, 2, 1),
('TH00000197', N'Cốm pha dung dịch uống Fosfomycin 3000 mg', N'Chai 100ml', N'Chai', 'DM005', N'Uống', 125000, 0, 6, 1),
('TH00000198', N'Dung dịch tiêm Bivalirudin 5 mg/ml', N'Chai 100ml', N'Chai', 'DM007', N'Uống', 850000, 0, 10, 1),
('TH00000199', N'Dung dịch tiêm Penicillin G 375 mg/ml', N'Chai 100ml', N'Chai', 'DM010', N'Uống', 55000, 1, 1, 1),
('TH00000200', N'Dung dịch tiêm Vasopressin 20 đơn vị/ml', N'Chai 100ml', N'Chai', 'DM001', N'Uống', 65000, 1, 1, 1),
('TH00000201', N'Dung dịch hít Sevoflurane 1000 mg/ml', N'Chai 100ml', N'Chai', 'DM005', N'Uống', 145000, 0, 1, 1),
('TH00000202', N'Thuốc Sacubitril 24 mg / Valsartan 26 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM003', N'Uống', 25000, 1, 8, 1),
('TH00000203', N'Bình xịt Fluticasone 0.22 mg/liều (Flovent)', N'Lọ 120 liều', N'Lọ', 'DM003', N'Xịt/Hít', 185000, 1, 7, 1),
('TH00000204', N'Thuốc Atenolol 50 mg (Viên nén)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 4500, 1, 1, 1),
('TH00000205', N'Thuốc Hydrochlorothiazide 12.5 mg / Lisinopril 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM002', N'Uống', 3500, 1, 6, 1),
('TH00000206', N'Thuốc hạ mỡ máu Atorvastatin 10 mg', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 6500, 1, 8, 1),
('TH00000207', N'Thuốc hạ mỡ máu Rosuvastatin 10 mg (Crestor)', N'Hộp 10 vỉ x 10 viên', N'Viên', 'DM008', N'Uống', 12500, 1, 10, 1);
GO

-- ===========================================
-- INSERT THÀNH PHẦN THUỐC
-- ===========================================
INSERT INTO THANHPHAN_THUOC (MaThanhPhan, MaThuoc, MaHoatChat, HamLuong) VALUES 
('TP00000001', 'TH00000001', 'HC00000001', N'325 MG'),
('TP00000002', 'TH00000002', 'HC00000002', N'3 MG'),
('TP00000003', 'TH00000002', 'HC00000003', N'0.02 MG'),
('TP00000004', 'TH00000002', 'HC00000004', N'1 MG'),
('TP00000005', 'TH00000003', 'HC00000005', N'100 MG'),
('TP00000006', 'TH00000004', 'HC00000006', N'50 MG'),
('TP00000007', 'TH00000005', 'HC00000005', N'200 MG'),
('TP00000008', 'TH00000006', 'HC00000007', N'30 MG'),
('TP00000009', 'TH00000007', 'HC00000008', N'0.3 ML'),
('TP00000010', 'TH00000008', 'HC00000009', N'0.035 MG'),
('TP00000011', 'TH00000008', 'HC00000010', N'0.18 MG'),
('TP00000012', 'TH00000008', 'HC00000011', N'0.035 MG'),
('TP00000013', 'TH00000008', 'HC00000010', N'0.215 MG'),
('TP00000014', 'TH00000008', 'HC00000011', N'0.035 MG'),
('TP00000015', 'TH00000008', 'HC00000010', N'0.25 MG'),
('TP00000016', 'TH00000008', 'HC00000012', N'1 MG'),
('TP00000017', 'TH00000009', 'HC00000013', N'0.125 MG'),
('TP00000018', 'TH00000010', 'HC00000014', N'0.83 MG'),
('TP00000019', 'TH00000011', 'HC00000015', N'500 MG'),
('TP00000020', 'TH00000012', 'HC00000001', N'160 MG'),
('TP00000021', 'TH00000013', 'HC00000016', N'68 MG'),
('TP00000022', 'TH00000014', 'HC00000017', N'220 MG'),
('TP00000023', 'TH00000015', 'HC00000018', N'0.000625 MG'),
('TP00000024', 'TH00000015', 'HC00000016', N'0.005 MG'),
('TP00000025', 'TH00000016', 'HC00000019', N'5 MG'),
('TP00000026', 'TH00000017', 'HC00000020', N'25 MG'),
('TP00000027', 'TH00000018', 'HC00000021', N'500 MG'),
('TP00000028', 'TH00000019', 'HC00000022', N'10 MG'),
('TP00000029', 'TH00000020', 'HC00000023', N'0.35 MG'),
('TP00000030', 'TH00000021', 'HC00000024', N'25 MG'),
('TP00000031', 'TH00000022', 'HC00000001', N'21.7 MG'),
('TP00000032', 'TH00000022', 'HC00000025', N'1 MG'),
('TP00000033', 'TH00000022', 'HC00000026', N'0.417 MG'),
('TP00000034', 'TH00000023', 'HC00000027', N'1 ML'),
('TP00000035', 'TH00000024', 'HC00000028', N'800 MG'),
('TP00000036', 'TH00000024', 'HC00000029', N'160 MG'),
('TP00000037', 'TH00000025', 'HC00000030', N'5 MG'),
('TP00000038', 'TH00000026', 'HC00000031', N'500 MG'),
('TP00000039', 'TH00000027', 'HC00000032', N'0.000833 MG'),
('TP00000040', 'TH00000028', 'HC00000033', N'75 MG'),
('TP00000041', 'TH00000029', 'HC00000034', N'20 MG'),
('TP00000042', 'TH00000030', 'HC00000027', N'24 HR'),
('TP00000043', 'TH00000031', 'HC00000035', N'0.4 MG'),
('TP00000044', 'TH00000032', 'HC00000001', N'300 MG'),
('TP00000045', 'TH00000032', 'HC00000036', N'10 MG'),
('TP00000046', 'TH00000033', 'HC00000037', N'100 MG'),
('TP00000047', 'TH00000034', 'HC00000001', N'325 MG'),
('TP00000048', 'TH00000034', 'HC00000038', N'10 MG'),
('TP00000049', 'TH00000035', 'HC00000001', N'325 MG'),
('TP00000050', 'TH00000036', 'HC00000039', N'2.5 MG'),
('TP00000051', 'TH00000037', 'HC00000027', N'1 ML'),
('TP00000052', 'TH00000038', 'HC00000027', N'0.25 ML'),
('TP00000053', 'TH00000039', 'HC00000040', N'70 UNT'),
('TP00000054', 'TH00000039', 'HC00000041', N'30 UNT'),
('TP00000055', 'TH00000040', 'HC00000001', N'300 MG'),
('TP00000056', 'TH00000040', 'HC00000042', N'15 MG'),
('TP00000057', 'TH00000041', 'HC00000027', N'72 HR'),
('TP00000058', 'TH00000042', 'HC00000015', N'250 MG'),
('TP00000059', 'TH00000042', 'HC00000043', N'125 MG'),
('TP00000060', 'TH00000043', 'HC00000001', N'325 MG'),
('TP00000061', 'TH00000043', 'HC00000038', N'5 MG'),
('TP00000062', 'TH00000044', 'HC00000032', N'0.000729 MG'),
('TP00000063', 'TH00000045', 'HC00000009', N'0.01 MG'),
('TP00000064', 'TH00000045', 'HC00000044', N'0.03 MG'),
('TP00000065', 'TH00000045', 'HC00000032', N'0.15 MG'),
('TP00000066', 'TH00000046', 'HC00000045', N'325 MG'),
('TP00000067', 'TH00000047', 'HC00000027', N'24 HR'),
('TP00000068', 'TH00000048', 'HC00000027', N'24 HR'),
('TP00000069', 'TH00000049', 'HC00000015', N'250 MG'),
('TP00000070', 'TH00000050', 'HC00000046', N'0.03 MG'),
('TP00000071', 'TH00000050', 'HC00000032', N'0.15 MG'),
('TP00000072', 'TH00000050', 'HC00000012', N'1 MG'),
('TP00000073', 'TH00000051', 'HC00000032', N'0.000813 MG'),
('TP00000074', 'TH00000052', 'HC00000034', N'10 MG'),
('TP00000075', 'TH00000053', 'HC00000047', N'2 MG'),
('TP00000076', 'TH00000053', 'HC00000048', N'2 MG'),
('TP00000077', 'TH00000053', 'HC00000049', N'3 MG'),
('TP00000078', 'TH00000053', 'HC00000048', N'2 MG'),
('TP00000079', 'TH00000053', 'HC00000050', N'1 MG'),
('TP00000080', 'TH00000053', 'HC00000050', N'3 MG'),
('TP00000081', 'TH00000053', 'HC00000051', N'1 MG'),
('TP00000082', 'TH00000054', 'HC00000052', N'40 MG'),
('TP00000083', 'TH00000055', 'HC00000027', N'10 ML'),
('TP00000084', 'TH00000056', 'HC00000053', N'2000 MG'),
('TP00000085', 'TH00000057', 'HC00000027', N'1 ML'),
('TP00000086', 'TH00000058', 'HC00000027', N'25 ML'),
('TP00000087', 'TH00000059', 'HC00000041', N'100 UNT'),
('TP00000088', 'TH00000060', 'HC00000027', N'2 ML'),
('TP00000089', 'TH00000061', 'HC00000054', N'1 MG'),
('TP00000090', 'TH00000062', 'HC00000027', N'100 ML'),
('TP00000091', 'TH00000063', 'HC00000055', N'10 MG'),
('TP00000092', 'TH00000064', 'HC00000056', N'999 MG'),
('TP00000093', 'TH00000065', 'HC00000027', N'5 ML'),
('TP00000094', 'TH00000066', 'HC00000057', N'4 MG'),
('TP00000095', 'TH00000067', 'HC00000001', N'300 MG'),
('TP00000096', 'TH00000067', 'HC00000036', N'5 MG'),
('TP00000097', 'TH00000068', 'HC00000058', N'2 MG'),
('TP00000098', 'TH00000068', 'HC00000059', N'0.5 MG'),
('TP00000099', 'TH00000069', 'HC00000060', N'250 MG'),
('TP00000100', 'TH00000070', 'HC00000061', N'10 MG'),
('TP00000101', 'TH00000071', 'HC00000062', N'500 MG'),
('TP00000102', 'TH00000072', 'HC00000063', N'1000 MG'),
('TP00000103', 'TH00000073', 'HC00000027', N'10 ML'),
('TP00000104', 'TH00000074', 'HC00000005', N'400 MG'),
('TP00000105', 'TH00000075', 'HC00000064', N'500 MG'),
('TP00000106', 'TH00000076', 'HC00000065', N'100 MG'),
('TP00000107', 'TH00000077', 'HC00000066', N'50 MG'),
('TP00000108', 'TH00000078', 'HC00000031', N'250 MG'),
('TP00000109', 'TH00000079', 'HC00000067', N'12 HR'),
('TP00000110', 'TH00000080', 'HC00000068', N'0.075 MG'),
('TP00000111', 'TH00000081', 'HC00000027', N'1 ML'),
('TP00000112', 'TH00000082', 'HC00000023', N'0.35 MG'),
('TP00000113', 'TH00000083', 'HC00000027', N'168 HR'),
('TP00000114', 'TH00000083', 'HC00000069', N'0.00625 MG'),
('TP00000115', 'TH00000084', 'HC00000027', N'0.4 ML'),
('TP00000116', 'TH00000085', 'HC00000001', N'500 MG'),
('TP00000117', 'TH00000086', 'HC00000027', N'1 ML'),
('TP00000118', 'TH00000087', 'HC00000027', N'1 ML'),
('TP00000119', 'TH00000088', 'HC00000070', N'5 MG'),
('TP00000120', 'TH00000089', 'HC00000071', N'12.5 MG'),
('TP00000121', 'TH00000090', 'HC00000072', N'20 MG'),
('TP00000122', 'TH00000091', 'HC00000073', N'100 MG'),
('TP00000123', 'TH00000092', 'HC00000074', N'10 MG'),
('TP00000124', 'TH00000093', 'HC00000075', N'200 ACTUAT'),
('TP00000125', 'TH00000094', 'HC00000076', N'1000 MG'),
('TP00000126', 'TH00000095', 'HC00000077', N'4000 MG'),
('TP00000127', 'TH00000095', 'HC00000078', N'500 MG'),
('TP00000128', 'TH00000096', 'HC00000079', N'25 MG'),
('TP00000129', 'TH00000096', 'HC00000080', N'75 MG'),
('TP00000130', 'TH00000097', 'HC00000081', N'5 MG'),
('TP00000131', 'TH00000098', 'HC00000082', N'2 MG'),
('TP00000132', 'TH00000099', 'HC00000083', N'10 MG'),
('TP00000133', 'TH00000100', 'HC00000081', N'10 MG'),
('TP00000134', 'TH00000101', 'HC00000062', N'250 MG'),
('TP00000135', 'TH00000102', 'HC00000084', N'5 MG'),
('TP00000136', 'TH00000103', 'HC00000085', N'100 MG'),
('TP00000137', 'TH00000104', 'HC00000086', N'1000 UNT'),
('TP00000138', 'TH00000105', 'HC00000027', N'10 ML'),
('TP00000139', 'TH00000106', 'HC00000087', N'5 MG'),
('TP00000140', 'TH00000107', 'HC00000088', N'81 MG'),
('TP00000141', 'TH00000108', 'HC00000089', N'5 MG'),
('TP00000142', 'TH00000109', 'HC00000071', N'25 MG'),
('TP00000143', 'TH00000110', 'HC00000022', N'20 MG'),
('TP00000144', 'TH00000111', 'HC00000090', N'97 MG'),
('TP00000145', 'TH00000111', 'HC00000091', N'103 MG'),
('TP00000146', 'TH00000112', 'HC00000027', N'12 HR'),
('TP00000147', 'TH00000113', 'HC00000084', N'10 MG'),
('TP00000148', 'TH00000114', 'HC00000027', N'120 ACTUAT'),
('TP00000149', 'TH00000115', 'HC00000077', N'2000 MG'),
('TP00000150', 'TH00000115', 'HC00000078', N'250 MG'),
('TP00000151', 'TH00000116', 'HC00000027', N'150 ML'),
('TP00000152', 'TH00000117', 'HC00000073', N'50 MG'),
('TP00000153', 'TH00000118', 'HC00000088', N'81 MG'),
('TP00000154', 'TH00000119', 'HC00000027', N'24 HR'),
('TP00000155', 'TH00000120', 'HC00000072', N'40 MG'),
('TP00000156', 'TH00000121', 'HC00000022', N'40 MG'),
('TP00000157', 'TH00000122', 'HC00000015', N'875 MG'),
('TP00000158', 'TH00000122', 'HC00000043', N'125 MG'),
('TP00000159', 'TH00000123', 'HC00000092', N'80 MG'),
('TP00000160', 'TH00000124', 'HC00000093', N'5 MG'),
('TP00000161', 'TH00000125', 'HC00000094', N'0.125 MG'),
('TP00000162', 'TH00000126', 'HC00000013', N'0.25 MG'),
('TP00000163', 'TH00000127', 'HC00000023', N'0.35 MG'),
('TP00000164', 'TH00000128', 'HC00000095', N'25 MG'),
('TP00000165', 'TH00000129', 'HC00000034', N'40 MG'),
('TP00000166', 'TH00000130', 'HC00000027', N'24 HR'),
('TP00000167', 'TH00000131', 'HC00000072', N'80 MG'),
('TP00000168', 'TH00000132', 'HC00000096', N'2 MG'),
('TP00000169', 'TH00000133', 'HC00000097', N'10 MG'),
('TP00000170', 'TH00000134', 'HC00000098', N'10 MG'),
('TP00000171', 'TH00000135', 'HC00000098', N'23 MG'),
('TP00000172', 'TH00000136', 'HC00000099', N'50 MG'),
('TP00000173', 'TH00000137', 'HC00000015', N'500 MG'),
('TP00000174', 'TH00000137', 'HC00000043', N'125 MG'),
('TP00000175', 'TH00000138', 'HC00000027', N'4 ML'),
('TP00000176', 'TH00000139', 'HC00000001', N'325 MG'),
('TP00000177', 'TH00000139', 'HC00000036', N'7.5 MG'),
('TP00000178', 'TH00000140', 'HC00000100', N'100 MG'),
('TP00000179', 'TH00000141', 'HC00000014', N'5 MG'),
('TP00000180', 'TH00000142', 'HC00000027', N'60 ACTUAT'),
('TP00000181', 'TH00000142', 'HC00000101', N'0.05 MG'),
('TP00000182', 'TH00000143', 'HC00000102', N'10 MG'),
('TP00000183', 'TH00000144', 'HC00000103', N'200 MG'),
('TP00000184', 'TH00000145', 'HC00000088', N'325 MG'),
('TP00000185', 'TH00000146', 'HC00000027', N'2 ML'),
('TP00000186', 'TH00000147', 'HC00000086', N'100 UNT'),
('TP00000187', 'TH00000148', 'HC00000104', N'90 MG'),
('TP00000188', 'TH00000149', 'HC00000105', N'25 MG'),
('TP00000189', 'TH00000150', 'HC00000022', N'2.5 MG'),
('TP00000190', 'TH00000151', 'HC00000027', N'100 ML'),
('TP00000191', 'TH00000152', 'HC00000105', N'50 MG'),
('TP00000192', 'TH00000153', 'HC00000027', N'5 ML'),
('TP00000193', 'TH00000154', 'HC00000106', N'100 MG'),
('TP00000194', 'TH00000155', 'HC00000107', N'0.25 MG'),
('TP00000195', 'TH00000156', 'HC00000014', N'0.417 MG'),
('TP00000196', 'TH00000157', 'HC00000108', N'2.5 MG'),
('TP00000197', 'TH00000158', 'HC00000014', N'0.21 MG'),
('TP00000198', 'TH00000159', 'HC00000109', N'100 MG'),
('TP00000199', 'TH00000160', 'HC00000091', N'160 MG'),
('TP00000200', 'TH00000161', 'HC00000027', N'24 HR'),
('TP00000201', 'TH00000162', 'HC00000073', N'25 MG'),
('TP00000202', 'TH00000163', 'HC00000013', N'0.125 MG'),
('TP00000203', 'TH00000164', 'HC00000089', N'5 MG'),
('TP00000204', 'TH00000165', 'HC00000027', N'120 ACTUAT'),
('TP00000205', 'TH00000166', 'HC00000110', N'200 ACTUAT'),
('TP00000206', 'TH00000167', 'HC00000035', N'0.4 MG'),
('TP00000207', 'TH00000168', 'HC00000111', N'50 MG'),
('TP00000208', 'TH00000169', 'HC00000112', N'100 MG'),
('TP00000209', 'TH00000170', 'HC00000027', N'12 HR'),
('TP00000210', 'TH00000171', 'HC00000113', N'100 MG'),
('TP00000211', 'TH00000172', 'HC00000027', N'5 ML'),
('TP00000212', 'TH00000172', 'HC00000114', N'120 MG'),
('TP00000213', 'TH00000173', 'HC00000115', N'20 MG'),
('TP00000214', 'TH00000174', 'HC00000116', N'100 MG'),
('TP00000215', 'TH00000175', 'HC00000027', N'10 ML'),
('TP00000216', 'TH00000176', 'HC00000117', N'200 MG'),
('TP00000217', 'TH00000177', 'HC00000067', N'12 HR'),
('TP00000218', 'TH00000178', 'HC00000118', N'2 MG'),
('TP00000219', 'TH00000179', 'HC00000119', N'20 MG'),
('TP00000220', 'TH00000180', 'HC00000027', N'24 HR'),
('TP00000221', 'TH00000180', 'HC00000118', N'28 MG'),
('TP00000222', 'TH00000181', 'HC00000120', N'50 MG'),
('TP00000223', 'TH00000182', 'HC00000027', N'24 HR'),
('TP00000224', 'TH00000183', 'HC00000121', N'200 ACTUAT'),
('TP00000225', 'TH00000184', 'HC00000122', N'250 MG'),
('TP00000226', 'TH00000185', 'HC00000123', N'20 MG'),
('TP00000227', 'TH00000186', 'HC00000079', N'25 MG'),
('TP00000228', 'TH00000186', 'HC00000080', N'75 MG'),
('TP00000229', 'TH00000187', 'HC00000027', N'4 ML'),
('TP00000230', 'TH00000188', 'HC00000124', N'60 MG'),
('TP00000231', 'TH00000189', 'HC00000125', N'20 MG'),
('TP00000232', 'TH00000190', 'HC00000126', N'10 MG'),
('TP00000233', 'TH00000191', 'HC00000127', N'120 ACTUAT'),
('TP00000234', 'TH00000192', 'HC00000027', N'0.8 ML'),
('TP00000235', 'TH00000193', 'HC00000128', N'10 MG'),
('TP00000236', 'TH00000193', 'HC00000034', N'40 MG'),
('TP00000237', 'TH00000194', 'HC00000129', N'300 MG'),
('TP00000238', 'TH00000195', 'HC00000130', N'100 MG'),
('TP00000239', 'TH00000196', 'HC00000096', N'4 MG'),
('TP00000240', 'TH00000197', 'HC00000131', N'3000 MG'),
('TP00000241', 'TH00000198', 'HC00000027', N'50 ML'),
('TP00000242', 'TH00000199', 'HC00000132', N'375 MG'),
('TP00000243', 'TH00000200', 'HC00000027', N'1 ML'),
('TP00000244', 'TH00000201', 'HC00000133', N'1000 MG'),
('TP00000245', 'TH00000202', 'HC00000090', N'24 MG'),
('TP00000246', 'TH00000202', 'HC00000091', N'26 MG'),
('TP00000247', 'TH00000203', 'HC00000027', N'120 ACTUAT'),
('TP00000248', 'TH00000204', 'HC00000095', N'50 MG'),
('TP00000249', 'TH00000205', 'HC00000024', N'12.5 MG'),
('TP00000250', 'TH00000205', 'HC00000022', N'10 MG'),
('TP00000251', 'TH00000206', 'HC00000072', N'10 MG'),
('TP00000252', 'TH00000207', 'HC00000126', N'10 MG');
GO


USE QL_KhamBenhNgoaiTru;
GO

INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 1, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 2, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 3, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 4, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 5, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 6, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 7, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 8, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 9, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
INSERT INTO PHIEUNHAP (MaNV_LapPhieu, MaNSX, MaKho, NgayLap, TrangThai, GhiChu, MaNV_Duyet, NgayDuyet, TongTienNhap) VALUES ('NV021', 10, 1, '2025-01-10', N'Đã duyệt', N'Nhập hàng Bulk', 'NV_BAO', '2025-01-11', 0);
GO

INSERT INTO CT_PHIEUNHAP (MaPhieuNhap, MaThuoc, MaLo, NgaySanXuat, HanSuDung, SoLuongNhap, DonGiaNhap, ThanhTien) VALUES 
(1, 'TH00000001', 'L639_0125', '2024-12-01', '2027-12-31', 89585, 825480, 73950625800),
(10, 'TH00000002', 'L842_0125', '2024-12-01', '2027-12-31', 72779, 4152780, 302235175620),
(4, 'TH00000003', 'L825_0125', '2024-12-01', '2027-12-31', 148409, 11994300, 1780062068700),
(1, 'TH00000004', 'L729_0125', '2024-12-01', '2027-12-31', 69118, 624600, 43171102800),
(4, 'TH00000005', 'L156_0125', '2024-12-01', '2027-12-31', 95392, 2294820, 218907469440),
(6, 'TH00000006', 'L631_0125', '2024-12-01', '2027-12-31', 126942, 3852540, 489049132680),
(7, 'TH00000007', 'L844_0125', '2024-12-01', '2027-12-31', 136484, 679680, 92765445120),
(9, 'TH00000008', 'L273_0125', '2024-12-01', '2027-12-31', 126751, 4127760, 523197707760),
(5, 'TH00000009', 'L370_0125', '2024-12-01', '2027-12-31', 51671, 2338920, 120854335320),
(7, 'TH00000010', 'L255_0125', '2024-12-01', '2027-12-31', 148564, 2338920, 347479310880),
(3, 'TH00000011', 'L121_0125', '2024-12-01', '2027-12-31', 79849, 3399300, 271430705700),
(10, 'TH00000012', 'L859_0125', '2024-12-01', '2027-12-31', 52424, 825480, 43274963520),
(9, 'TH00000013', 'L313_0125', '2024-12-01', '2027-12-31', 60970, 236160, 14398675200),
(6, 'TH00000014', 'L869_0125', '2024-12-01', '2027-12-31', 64313, 3026880, 194667733440),
(7, 'TH00000015', 'L604_0125', '2024-12-01', '2027-12-31', 130570, 13759200, 1796538744000),
(2, 'TH00000016', 'L437_0125', '2024-12-01', '2027-12-31', 103743, 410400, 42576127200),
(1, 'TH00000017', 'L817_0125', '2024-12-01', '2027-12-31', 94989, 1618560, 153745395840),
(6, 'TH00000018', 'L248_0125', '2024-12-01', '2027-12-31', 117760, 2338920, 275431219200),
(3, 'TH00000019', 'L342_0125', '2024-12-01', '2027-12-31', 133472, 16380, 2186271360),
(8, 'TH00000020', 'L821_0125', '2024-12-01', '2027-12-31', 147892, 4127760, 610462681920),
(9, 'TH00000021', 'L723_0125', '2024-12-01', '2027-12-31', 105080, 9360, 983548800),
(2, 'TH00000022', 'L431_0125', '2024-12-01', '2027-12-31', 58695, 1382400, 81139968000),
(5, 'TH00000023', 'L744_0125', '2024-12-01', '2027-12-31', 137793, 325980, 44917762140),
(7, 'TH00000024', 'L240_0125', '2024-12-01', '2027-12-31', 66348, 2338920, 155182664160),
(9, 'TH00000025', 'L381_0125', '2024-12-01', '2027-12-31', 132718, 2338920, 310416784560),
(4, 'TH00000026', 'L425_0125', '2024-12-01', '2027-12-31', 99445, 94680, 9415452600),
(5, 'TH00000027', 'L675_0125', '2024-12-01', '2027-12-31', 148203, 221580, 32838820740),
(9, 'TH00000028', 'L132_0125', '2024-12-01', '2027-12-31', 136567, 2154420, 294222676140),
(3, 'TH00000029', 'L848_0125', '2024-12-01', '2027-12-31', 93103, 550260, 51230856780),
(4, 'TH00000030', 'L104_0125', '2024-12-01', '2027-12-31', 103999, 2338920, 243245341080),
(10, 'TH00000031', 'L594_0125', '2024-12-01', '2027-12-31', 69566, 1923840, 133833853440),
(6, 'TH00000032', 'L555_0125', '2024-12-01', '2027-12-31', 97184, 275580, 26781966720),
(4, 'TH00000033', 'L550_0125', '2024-12-01', '2027-12-31', 99933, 13680, 1367083440),
(2, 'TH00000034', 'L417_0125', '2024-12-01', '2027-12-31', 90333, 2338920, 211281660360),
(3, 'TH00000035', 'L241_0125', '2024-12-01', '2027-12-31', 91168, 2338920, 213234658560),
(7, 'TH00000036', 'L522_0125', '2024-12-01', '2027-12-31', 130741, 16380, 2141537580),
(9, 'TH00000037', 'L874_0125', '2024-12-01', '2027-12-31', 113017, 256500, 28988860500),
(5, 'TH00000038', 'L523_0125', '2024-12-01', '2027-12-31', 131101, 5808060, 761442474060),
(8, 'TH00000039', 'L752_0125', '2024-12-01', '2027-12-31', 55790, 2284020, 127425475800),
(9, 'TH00000040', 'L869_0125', '2024-12-01', '2027-12-31', 138058, 2338920, 322906617360),
(3, 'TH00000041', 'L183_0125', '2024-12-01', '2027-12-31', 93152, 2338920, 217875075840),
(1, 'TH00000042', 'L441_0125', '2024-12-01', '2027-12-31', 99958, 825480, 82513329840),
(3, 'TH00000043', 'L326_0125', '2024-12-01', '2027-12-31', 50728, 133380, 6766100640),
(5, 'TH00000044', 'L360_0125', '2024-12-01', '2027-12-31', 66323, 201420, 13358778660),
(3, 'TH00000045', 'L495_0125', '2024-12-01', '2027-12-31', 79594, 4127760, 328544929440),
(9, 'TH00000046', 'L515_0125', '2024-12-01', '2027-12-31', 71229, 2700, 192318300),
(7, 'TH00000047', 'L921_0125', '2024-12-01', '2027-12-31', 85639, 2338920, 200302769880),
(1, 'TH00000048', 'L200_0125', '2024-12-01', '2027-12-31', 80971, 31140, 2521436940),
(3, 'TH00000049', 'L305_0125', '2024-12-01', '2027-12-31', 128761, 464040, 59750254440),
(2, 'TH00000050', 'L795_0125', '2024-12-01', '2027-12-31', 131256, 5297940, 695386412640),
(4, 'TH00000051', 'L799_0125', '2024-12-01', '2027-12-31', 85629, 10594260, 907175889540),
(1, 'TH00000052', 'L401_0125', '2024-12-01', '2027-12-31', 115407, 8100, 934796700),
(1, 'TH00000053', 'L379_0125', '2024-12-01', '2027-12-31', 117789, 6408000, 754791912000),
(7, 'TH00000054', 'L468_0125', '2024-12-01', '2027-12-31', 133856, 21960, 2939477760),
(6, 'TH00000055', 'L169_0125', '2024-12-01', '2027-12-31', 145095, 66000, 9576270000),
(7, 'TH00000056', 'L500_0125', '2024-12-01', '2027-12-31', 63577, 2338920, 148701516840),
(3, 'TH00000057', 'L235_0125', '2024-12-01', '2027-12-31', 141930, 9180, 1302917400),
(4, 'TH00000058', 'L598_0125', '2024-12-01', '2027-12-31', 107485, 2338920, 251398816200),
(5, 'TH00000059', 'L969_0125', '2024-12-01', '2027-12-31', 144980, 2338920, 339096621600),
(3, 'TH00000060', 'L172_0125', '2024-12-01', '2027-12-31', 68809, 3780, 260098020),
(2, 'TH00000061', 'L848_0125', '2024-12-01', '2027-12-31', 112823, 1980, 223389540),
(9, 'TH00000062', 'L368_0125', '2024-12-01', '2027-12-31', 53185, 2340, 124452900),
(3, 'TH00000063', 'L981_0125', '2024-12-01', '2027-12-31', 118537, 16560, 1962972720),
(7, 'TH00000064', 'L400_0125', '2024-12-01', '2027-12-31', 71236, 1980, 141047280),
(9, 'TH00000065', 'L701_0125', '2024-12-01', '2027-12-31', 122928, 53460, 6571730880),
(2, 'TH00000066', 'L122_0125', '2024-12-01', '2027-12-31', 131121, 352800, 46259488800),
(9, 'TH00000067', 'L525_0125', '2024-12-01', '2027-12-31', 51549, 738180, 38052440820),
(1, 'TH00000068', 'L120_0125', '2024-12-01', '2027-12-31', 74054, 2338920, 173206381680),
(5, 'TH00000069', 'L250_0125', '2024-12-01', '2027-12-31', 110633, 7383420, 816849904860),
(9, 'TH00000070', 'L712_0125', '2024-12-01', '2027-12-31', 57776, 92160, 5324636160),
(9, 'TH00000071', 'L844_0125', '2024-12-01', '2027-12-31', 51340, 2338920, 120080152800),
(9, 'TH00000072', 'L542_0125', '2024-12-01', '2027-12-31', 106232, 6300, 669261600),
(8, 'TH00000073', 'L240_0125', '2024-12-01', '2027-12-31', 123655, 3240, 400642200),
(3, 'TH00000074', 'L641_0125', '2024-12-01', '2027-12-31', 126591, 2338920, 296086221720),
(1, 'TH00000075', 'L565_0125', '2024-12-01', '2027-12-31', 137361, 4528440, 622031046840),
(1, 'TH00000076', 'L117_0125', '2024-12-01', '2027-12-31', 66109, 7033860, 465001450740),
(6, 'TH00000077', 'L891_0125', '2024-12-01', '2027-12-31', 73044, 23583240, 1722614182560),
(7, 'TH00000078', 'L734_0125', '2024-12-01', '2027-12-31', 61629, 135180, 8331008220),
(5, 'TH00000079', 'L196_0125', '2024-12-01', '2027-12-31', 56257, 825480, 46439028360),
(9, 'TH00000080', 'L568_0125', '2024-12-01', '2027-12-31', 137227, 2160, 296410320),
(7, 'TH00000081', 'L829_0125', '2024-12-01', '2027-12-31', 119450, 525780, 62804421000),
(3, 'TH00000082', 'L561_0125', '2024-12-01', '2027-12-31', 86299, 5221440, 450605050560),
(10, 'TH00000083', 'L169_0125', '2024-12-01', '2027-12-31', 141154, 9597240, 1354688814960),
(8, 'TH00000084', 'L703_0125', '2024-12-01', '2027-12-31', 68759, 83340, 5730375060),
(10, 'TH00000085', 'L735_0125', '2024-12-01', '2027-12-31', 60215, 72000, 4335480000),
(6, 'TH00000086', 'L846_0125', '2024-12-01', '2027-12-31', 96205, 106020, 10199654100),
(8, 'TH00000087', 'L667_0125', '2024-12-01', '2027-12-31', 145188, 2338920, 339583116960),
(5, 'TH00000088', 'L236_0125', '2024-12-01', '2027-12-31', 70876, 2338920, 165773293920),
(3, 'TH00000089', 'L602_0125', '2024-12-01', '2027-12-31', 131394, 2338920, 307320054480),
(8, 'TH00000090', 'L356_0125', '2024-12-01', '2027-12-31', 93263, 2338920, 218134695960),
(1, 'TH00000091', 'L507_0125', '2024-12-01', '2027-12-31', 107202, 2338920, 250736901840),
(2, 'TH00000092', 'L438_0125', '2024-12-01', '2027-12-31', 91183, 2338920, 213269742360),
(1, 'TH00000093', 'L920_0125', '2024-12-01', '2027-12-31', 78387, 17929080, 1405406793960),
(3, 'TH00000094', 'L307_0125', '2024-12-01', '2027-12-31', 60398, 44640, 2696166720),
(5, 'TH00000095', 'L420_0125', '2024-12-01', '2027-12-31', 109958, 63540, 6986731320),
(9, 'TH00000096', 'L772_0125', '2024-12-01', '2027-12-31', 108423, 2338920, 253592723160),
(5, 'TH00000097', 'L288_0125', '2024-12-01', '2027-12-31', 98233, 1375920, 135160749360),
(10, 'TH00000098', 'L561_0125', '2024-12-01', '2027-12-31', 63104, 999720, 63086330880),
(6, 'TH00000099', 'L779_0125', '2024-12-01', '2027-12-31', 51864, 825480, 42812694720),
(8, 'TH00000100', 'L247_0125', '2024-12-01', '2027-12-31', 53937, 1375920, 74212997040),
(5, 'TH00000101', 'L836_0125', '2024-12-01', '2027-12-31', 58080, 2338920, 135844473600),
(4, 'TH00000102', 'L259_0125', '2024-12-01', '2027-12-31', 141048, 5674500, 800376876000),
(6, 'TH00000103', 'L324_0125', '2024-12-01', '2027-12-31', 146065, 2338920, 341634349800),
(8, 'TH00000104', 'L531_0125', '2024-12-01', '2027-12-31', 50851, 2338920, 118936420920),
(3, 'TH00000105', 'L798_0125', '2024-12-01', '2027-12-31', 99880, 18180, 1815818400),
(1, 'TH00000106', 'L832_0125', '2024-12-01', '2027-12-31', 149930, 68580, 10282199400),
(9, 'TH00000107', 'L151_0125', '2024-12-01', '2027-12-31', 81359, 2338920, 190292192280),
(2, 'TH00000108', 'L493_0125', '2024-12-01', '2027-12-31', 101335, 14000, 1418690000),
(8, 'TH00000109', 'L187_0125', '2024-12-01', '2027-12-31', 120909, 2338920, 282796478280),
(10, 'TH00000110', 'L485_0125', '2024-12-01', '2027-12-31', 138448, 2338920, 323818796160),
(6, 'TH00000111', 'L696_0125', '2024-12-01', '2027-12-31', 114406, 2338920, 267586481520),
(10, 'TH00000112', 'L136_0125', '2024-12-01', '2027-12-31', 125517, 2338920, 293574221640),
(8, 'TH00000113', 'L269_0125', '2024-12-01', '2027-12-31', 101433, 4127760, 418691080080),
(9, 'TH00000114', 'L731_0125', '2024-12-01', '2027-12-31', 113258, 2338920, 264901401360),
(8, 'TH00000115', 'L539_0125', '2024-12-01', '2027-12-31', 95622, 2338920, 223652208240),
(5, 'TH00000116', 'L409_0125', '2024-12-01', '2027-12-31', 74682, 2338920, 174675223440),
(5, 'TH00000117', 'L454_0125', '2024-12-01', '2027-12-31', 60453, 2338920, 141394730760),
(9, 'TH00000118', 'L576_0125', '2024-12-01', '2027-12-31', 140758, 1100700, 154932330600),
(7, 'TH00000119', 'L350_0125', '2024-12-01', '2027-12-31', 70496, 2338920, 164884504320),
(6, 'TH00000120', 'L886_0125', '2024-12-01', '2027-12-31', 112750, 2338920, 263713230000),
(9, 'TH00000121', 'L387_0125', '2024-12-01', '2027-12-31', 53429, 2338920, 124966156680),
(4, 'TH00000122', 'L397_0125', '2024-12-01', '2027-12-31', 118938, 2338920, 278186466960),
(10, 'TH00000123', 'L886_0125', '2024-12-01', '2027-12-31', 133252, 2338920, 311665767840),
(1, 'TH00000124', 'L717_0125', '2024-12-01', '2027-12-31', 144191, 16740, 2413757340),
(7, 'TH00000125', 'L979_0125', '2024-12-01', '2027-12-31', 127108, 41277600, 5246713180800),
(7, 'TH00000126', 'L672_0125', '2024-12-01', '2027-12-31', 70339, 2338920, 164517293880),
(5, 'TH00000127', 'L127_0125', '2024-12-01', '2027-12-31', 97666, 4300740, 420036072840),
(7, 'TH00000128', 'L906_0125', '2024-12-01', '2027-12-31', 92656, 2338920, 216714971520),
(2, 'TH00000129', 'L312_0125', '2024-12-01', '2027-12-31', 123431, 2338920, 288695234520),
(4, 'TH00000130', 'L443_0125', '2024-12-01', '2027-12-31', 86443, 2338920, 202183261560),
(1, 'TH00000131', 'L714_0125', '2024-12-01', '2027-12-31', 85101, 20340, 1730954340),
(5, 'TH00000132', 'L910_0125', '2024-12-01', '2027-12-31', 120143, 2751840, 330614313120),
(10, 'TH00000133', 'L534_0125', '2024-12-01', '2027-12-31', 60708, 825480, 50113239840),
(6, 'TH00000134', 'L294_0125', '2024-12-01', '2027-12-31', 114729, 5879700, 674572101300),
(2, 'TH00000135', 'L843_0125', '2024-12-01', '2027-12-31', 119672, 2105820, 252007691040),
(6, 'TH00000136', 'L388_0125', '2024-12-01', '2027-12-31', 120260, 2338920, 281278519200),
(2, 'TH00000137', 'L187_0125', '2024-12-01', '2027-12-31', 72278, 2338920, 169052459760),
(7, 'TH00000138', 'L729_0125', '2024-12-01', '2027-12-31', 93854, 18900, 1773840600),
(9, 'TH00000139', 'L226_0125', '2024-12-01', '2027-12-31', 82894, 360180, 29856760920),
(9, 'TH00000140', 'L884_0125', '2024-12-01', '2027-12-31', 86753, 1651104000, 143238225312000),
(7, 'TH00000141', 'L833_0125', '2024-12-01', '2027-12-31', 114205, 371340, 42408884700),
(5, 'TH00000142', 'L899_0125', '2024-12-01', '2027-12-31', 80608, 4330260, 349053598080),
(4, 'TH00000143', 'L441_0125', '2024-12-01', '2027-12-31', 104354, 6361200, 663816664800),
(8, 'TH00000144', 'L604_0125', '2024-12-01', '2027-12-31', 115008, 2014380, 231669815040),
(9, 'TH00000145', 'L975_0125', '2024-12-01', '2027-12-31', 67113, 2338920, 156971937960),
(1, 'TH00000146', 'L392_0125', '2024-12-01', '2027-12-31', 102148, 2338920, 238916000160),
(2, 'TH00000147', 'L955_0125', '2024-12-01', '2027-12-31', 52615, 2338920, 123062275800),
(10, 'TH00000148', 'L240_0125', '2024-12-01', '2027-12-31', 85118, 2338920, 199084192560),
(4, 'TH00000149', 'L214_0125', '2024-12-01', '2027-12-31', 90908, 2338920, 212626539360),
(2, 'TH00000150', 'L728_0125', '2024-12-01', '2027-12-31', 87980, 2338920, 205778181600),
(4, 'TH00000151', 'L960_0125', '2024-12-01', '2027-12-31', 118525, 16020, 1898770500),
(6, 'TH00000152', 'L178_0125', '2024-12-01', '2027-12-31', 101046, 2338920, 236338510320),
(8, 'TH00000153', 'L339_0125', '2024-12-01', '2027-12-31', 133990, 1865520, 249961024800),
(10, 'TH00000154', 'L553_0125', '2024-12-01', '2027-12-31', 52781, 5681160, 299857305960),
(7, 'TH00000155', 'L686_0125', '2024-12-01', '2027-12-31', 117797, 246060, 28985129820),
(4, 'TH00000156', 'L951_0125', '2024-12-01', '2027-12-31', 138878, 2338920, 324824531760),
(3, 'TH00000157', 'L474_0125', '2024-12-01', '2027-12-31', 147332, 447120, 65875083840),
(2, 'TH00000158', 'L523_0125', '2024-12-01', '2027-12-31', 85787, 2338920, 200648930040),
(5, 'TH00000159', 'L832_0125', '2024-12-01', '2027-12-31', 51332, 152280, 7816836960),
(4, 'TH00000160', 'L583_0125', '2024-12-01', '2027-12-31', 53237, 2338920, 124517084040),
(3, 'TH00000161', 'L753_0125', '2024-12-01', '2027-12-31', 58249, 464940, 27082290060),
(4, 'TH00000162', 'L283_0125', '2024-12-01', '2027-12-31', 72382, 57240, 4143145680),
(1, 'TH00000163', 'L862_0125', '2024-12-01', '2027-12-31', 113034, 2338920, 264377483280),
(1, 'TH00000164', 'L786_0125', '2024-12-01', '2027-12-31', 129532, 102420, 13266667440),
(3, 'TH00000165', 'L483_0125', '2024-12-01', '2027-12-31', 106328, 2338920, 248692685760),
(10, 'TH00000166', 'L369_0125', '2024-12-01', '2027-12-31', 113573, 2338920, 265638161160),
(7, 'TH00000167', 'L192_0125', '2024-12-01', '2027-12-31', 123490, 2338920, 288833230800),
(3, 'TH00000168', 'L531_0125', '2024-12-01', '2027-12-31', 123168, 3669120, 451918172160),
(3, 'TH00000169', 'L869_0125', '2024-12-01', '2027-12-31', 116961, 32040, 3747430440),
(5, 'TH00000170', 'L361_0125', '2024-12-01', '2027-12-31', 119666, 13782060, 1649243991960),
(3, 'TH00000171', 'L858_0125', '2024-12-01', '2027-12-31', 148404, 2338920, 347105083680),
(1, 'TH00000172', 'L472_0125', '2024-12-01', '2027-12-31', 93118, 8971740, 835430485320),
(8, 'TH00000173', 'L947_0125', '2024-12-01', '2027-12-31', 128216, 21960, 2815623360),
(9, 'TH00000174', 'L540_0125', '2024-12-01', '2027-12-31', 147885, 250920, 37107304200),
(6, 'TH00000175', 'L179_0125', '2024-12-01', '2027-12-31', 128406, 18345600, 2355685113600),
(1, 'TH00000176', 'L631_0125', '2024-12-01', '2027-12-31', 64142, 2338920, 150023006640),
(8, 'TH00000177', 'L143_0125', '2024-12-01', '2027-12-31', 138784, 2338920, 324604673280),
(6, 'TH00000178', 'L414_0125', '2024-12-01', '2027-12-31', 149138, 12671460, 1889796201480),
(7, 'TH00000179', 'L861_0125', '2024-12-01', '2027-12-31', 126221, 25844400, 3262106012400),
(1, 'TH00000180', 'L543_0125', '2024-12-01', '2027-12-31', 83492, 2759940, 230432910480),
(2, 'TH00000181', 'L981_0125', '2024-12-01', '2027-12-31', 147497, 2338920, 344983683240),
(5, 'TH00000182', 'L197_0125', '2024-12-01', '2027-12-31', 99724, 2338920, 233246458080),
(6, 'TH00000183', 'L243_0125', '2024-12-01', '2027-12-31', 145201, 2338920, 339613522920),
(10, 'TH00000184', 'L934_0125', '2024-12-01', '2027-12-31', 80527, 2338920, 188346210840),
(7, 'TH00000185', 'L819_0125', '2024-12-01', '2027-12-31', 75872, 2338920, 177458538240),
(2, 'TH00000186', 'L357_0125', '2024-12-01', '2027-12-31', 70725, 2338920, 165420117000),
(10, 'TH00000187', 'L848_0125', '2024-12-01', '2027-12-31', 108727, 2338920, 254303754840),
(2, 'TH00000188', 'L130_0125', '2024-12-01', '2027-12-31', 59801, 1650960, 98729058960),
(7, 'TH00000189', 'L581_0125', '2024-12-01', '2027-12-31', 89606, 8055720, 721840846320),
(9, 'TH00000190', 'L592_0125', '2024-12-01', '2027-12-31', 88797, 2338920, 207689079240),
(3, 'TH00000191', 'L978_0125', '2024-12-01', '2027-12-31', 129539, 2338920, 302981357880),
(9, 'TH00000192', 'L892_0125', '2024-12-01', '2027-12-31', 78635, 2338920, 183920974200),
(5, 'TH00000193', 'L343_0125', '2024-12-01', '2027-12-31', 132754, 2338920, 310500985680),
(6, 'TH00000194', 'L161_0125', '2024-12-01', '2027-12-31', 52251, 2338920, 122210908920),
(9, 'TH00000195', 'L476_0125', '2024-12-01', '2027-12-31', 92640, 2322540, 215160105600),
(9, 'TH00000196', 'L345_0125', '2024-12-01', '2027-12-31', 66399, 1100700, 73085379300),
(1, 'TH00000197', 'L464_0125', '2024-12-01', '2027-12-31', 120584, 2338920, 282036329280),
(4, 'TH00000198', 'L673_0125', '2024-12-01', '2027-12-31', 52436, 2338920, 122643609120),
(4, 'TH00000199', 'L807_0125', '2024-12-01', '2027-12-31', 60567, 7375140, 446690104380),
(8, 'TH00000200', 'L746_0125', '2024-12-01', '2027-12-31', 105678, 48600, 5135950800),
(7, 'TH00000201', 'L773_0125', '2024-12-01', '2027-12-31', 122686, 4140, 507920040),
(2, 'TH00000202', 'L629_0125', '2024-12-01', '2027-12-31', 63227, 2338920, 147882894840),
(2, 'TH00000203', 'L953_0125', '2024-12-01', '2027-12-31', 75128, 2338920, 175718381760),
(9, 'TH00000204', 'L533_0125', '2024-12-01', '2027-12-31', 91726, 2338920, 214539775920),
(4, 'TH00000205', 'L830_0125', '2024-12-01', '2027-12-31', 107763, 2338920, 252049035960),
(1, 'TH00000206', 'L532_0125', '2024-12-01', '2027-12-31', 87495, 2338920, 204643805400),
(2, 'TH00000207', 'L371_0125', '2024-12-01', '2027-12-31', 75380, 2338920, 176307789600);
GO

INSERT INTO TONKHO (MaKho, MaThuoc, MaLo, HanSuDung, NgaySanXuat, GiaNhap, SoLuongTon) VALUES 
(1, 'TH00000001', 'L639_0125', '2027-12-31', '2024-12-01', 825480, 89585),
(1, 'TH00000002', 'L842_0125', '2027-12-31', '2024-12-01', 4152780, 72779),
(1, 'TH00000003', 'L825_0125', '2027-12-31', '2024-12-01', 11994300, 148409),
(1, 'TH00000004', 'L729_0125', '2027-12-31', '2024-12-01', 624600, 69118),
(1, 'TH00000005', 'L156_0125', '2027-12-31', '2024-12-01', 2294820, 95392),
(1, 'TH00000006', 'L631_0125', '2027-12-31', '2024-12-01', 3852540, 126942),
(1, 'TH00000007', 'L844_0125', '2027-12-31', '2024-12-01', 679680, 136484),
(1, 'TH00000008', 'L273_0125', '2027-12-31', '2024-12-01', 4127760, 126751),
(1, 'TH00000009', 'L370_0125', '2027-12-31', '2024-12-01', 2338920, 51671),
(1, 'TH00000010', 'L255_0125', '2027-12-31', '2024-12-01', 2338920, 148564),
(1, 'TH00000011', 'L121_0125', '2027-12-31', '2024-12-01', 3399300, 79849),
(1, 'TH00000012', 'L859_0125', '2027-12-31', '2024-12-01', 825480, 52424),
(1, 'TH00000013', 'L313_0125', '2027-12-31', '2024-12-01', 236160, 60970),
(1, 'TH00000014', 'L869_0125', '2027-12-31', '2024-12-01', 3026880, 64313),
(1, 'TH00000015', 'L604_0125', '2027-12-31', '2024-12-01', 13759200, 130570),
(1, 'TH00000016', 'L437_0125', '2027-12-31', '2024-12-01', 410400, 103743),
(1, 'TH00000017', 'L817_0125', '2027-12-31', '2024-12-01', 1618560, 94989),
(1, 'TH00000018', 'L248_0125', '2027-12-31', '2024-12-01', 2338920, 117760),
(1, 'TH00000019', 'L342_0125', '2027-12-31', '2024-12-01', 16380, 133472),
(1, 'TH00000020', 'L821_0125', '2027-12-31', '2024-12-01', 4127760, 147892),
(1, 'TH00000021', 'L723_0125', '2027-12-31', '2024-12-01', 9360, 105080),
(1, 'TH00000022', 'L431_0125', '2027-12-31', '2024-12-01', 1382400, 58695),
(1, 'TH00000023', 'L744_0125', '2027-12-31', '2024-12-01', 325980, 137793),
(1, 'TH00000024', 'L240_0125', '2027-12-31', '2024-12-01', 2338920, 66348),
(1, 'TH00000025', 'L381_0125', '2027-12-31', '2024-12-01', 2338920, 132718),
(1, 'TH00000026', 'L425_0125', '2027-12-31', '2024-12-01', 94680, 99445),
(1, 'TH00000027', 'L675_0125', '2027-12-31', '2024-12-01', 221580, 148203),
(1, 'TH00000028', 'L132_0125', '2027-12-31', '2024-12-01', 2154420, 136567),
(1, 'TH00000029', 'L848_0125', '2027-12-31', '2024-12-01', 550260, 93103),
(1, 'TH00000030', 'L104_0125', '2027-12-31', '2024-12-01', 2338920, 103999),
(1, 'TH00000031', 'L594_0125', '2027-12-31', '2024-12-01', 1923840, 69566),
(1, 'TH00000032', 'L555_0125', '2027-12-31', '2024-12-01', 275580, 97184),
(1, 'TH00000033', 'L550_0125', '2027-12-31', '2024-12-01', 13680, 99933),
(1, 'TH00000034', 'L417_0125', '2027-12-31', '2024-12-01', 2338920, 90333),
(1, 'TH00000035', 'L241_0125', '2027-12-31', '2024-12-01', 2338920, 91168),
(1, 'TH00000036', 'L522_0125', '2027-12-31', '2024-12-01', 16380, 130741),
(1, 'TH00000037', 'L874_0125', '2027-12-31', '2024-12-01', 256500, 113017),
(1, 'TH00000038', 'L523_0125', '2027-12-31', '2024-12-01', 5808060, 131101),
(1, 'TH00000039', 'L752_0125', '2027-12-31', '2024-12-01', 2284020, 55790),
(1, 'TH00000040', 'L869_0125', '2027-12-31', '2024-12-01', 2338920, 138058),
(1, 'TH00000041', 'L183_0125', '2027-12-31', '2024-12-01', 2338920, 93152),
(1, 'TH00000042', 'L441_0125', '2027-12-31', '2024-12-01', 825480, 99958),
(1, 'TH00000043', 'L326_0125', '2027-12-31', '2024-12-01', 133380, 50728),
(1, 'TH00000044', 'L360_0125', '2027-12-31', '2024-12-01', 201420, 66323),
(1, 'TH00000045', 'L495_0125', '2027-12-31', '2024-12-01', 4127760, 79594),
(1, 'TH00000046', 'L515_0125', '2027-12-31', '2024-12-01', 2700, 71229),
(1, 'TH00000047', 'L921_0125', '2027-12-31', '2024-12-01', 2338920, 85639),
(1, 'TH00000048', 'L200_0125', '2027-12-31', '2024-12-01', 31140, 80971),
(1, 'TH00000049', 'L305_0125', '2027-12-31', '2024-12-01', 464040, 128761),
(1, 'TH00000050', 'L795_0125', '2027-12-31', '2024-12-01', 5297940, 131256),
(1, 'TH00000051', 'L799_0125', '2027-12-31', '2024-12-01', 10594260, 85629),
(1, 'TH00000052', 'L401_0125', '2027-12-31', '2024-12-01', 8100, 115407),
(1, 'TH00000053', 'L379_0125', '2027-12-31', '2024-12-01', 6408000, 117789),
(1, 'TH00000054', 'L468_0125', '2027-12-31', '2024-12-01', 21960, 133856),
(1, 'TH00000055', 'L169_0125', '2027-12-31', '2024-12-01', 66000, 145095),
(1, 'TH00000056', 'L500_0125', '2027-12-31', '2024-12-01', 2338920, 63577),
(1, 'TH00000057', 'L235_0125', '2027-12-31', '2024-12-01', 9180, 141930),
(1, 'TH00000058', 'L598_0125', '2027-12-31', '2024-12-01', 2338920, 107485),
(1, 'TH00000059', 'L969_0125', '2027-12-31', '2024-12-01', 2338920, 144980),
(1, 'TH00000060', 'L172_0125', '2027-12-31', '2024-12-01', 3780, 68809),
(1, 'TH00000061', 'L848_0125', '2027-12-31', '2024-12-01', 1980, 112823),
(1, 'TH00000062', 'L368_0125', '2027-12-31', '2024-12-01', 2340, 53185),
(1, 'TH00000063', 'L981_0125', '2027-12-31', '2024-12-01', 16560, 118537),
(1, 'TH00000064', 'L400_0125', '2027-12-31', '2024-12-01', 1980, 71236),
(1, 'TH00000065', 'L701_0125', '2027-12-31', '2024-12-01', 53460, 122928),
(1, 'TH00000066', 'L122_0125', '2027-12-31', '2024-12-01', 352800, 131121),
(1, 'TH00000067', 'L525_0125', '2027-12-31', '2024-12-01', 738180, 51549),
(1, 'TH00000068', 'L120_0125', '2027-12-31', '2024-12-01', 2338920, 74054),
(1, 'TH00000069', 'L250_0125', '2027-12-31', '2024-12-01', 7383420, 110633),
(1, 'TH00000070', 'L712_0125', '2027-12-31', '2024-12-01', 92160, 57776),
(1, 'TH00000071', 'L844_0125', '2027-12-31', '2024-12-01', 2338920, 51340),
(1, 'TH00000072', 'L542_0125', '2027-12-31', '2024-12-01', 6300, 106232),
(1, 'TH00000073', 'L240_0125', '2027-12-31', '2024-12-01', 3240, 123655),
(1, 'TH00000074', 'L641_0125', '2027-12-31', '2024-12-01', 2338920, 126591),
(1, 'TH00000075', 'L565_0125', '2027-12-31', '2024-12-01', 4528440, 137361),
(1, 'TH00000076', 'L117_0125', '2027-12-31', '2024-12-01', 7033860, 66109),
(1, 'TH00000077', 'L891_0125', '2027-12-31', '2024-12-01', 23583240, 73044),
(1, 'TH00000078', 'L734_0125', '2027-12-31', '2024-12-01', 135180, 61629),
(1, 'TH00000079', 'L196_0125', '2027-12-31', '2024-12-01', 825480, 56257),
(1, 'TH00000080', 'L568_0125', '2027-12-31', '2024-12-01', 2160, 137227),
(1, 'TH00000081', 'L829_0125', '2027-12-31', '2024-12-01', 525780, 119450),
(1, 'TH00000082', 'L561_0125', '2027-12-31', '2024-12-01', 5221440, 86299),
(1, 'TH00000083', 'L169_0125', '2027-12-31', '2024-12-01', 9597240, 141154),
(1, 'TH00000084', 'L703_0125', '2027-12-31', '2024-12-01', 83340, 68759),
(1, 'TH00000085', 'L735_0125', '2027-12-31', '2024-12-01', 72000, 60215),
(1, 'TH00000086', 'L846_0125', '2027-12-31', '2024-12-01', 106020, 96205),
(1, 'TH00000087', 'L667_0125', '2027-12-31', '2024-12-01', 2338920, 145188),
(1, 'TH00000088', 'L236_0125', '2027-12-31', '2024-12-01', 2338920, 70876),
(1, 'TH00000089', 'L602_0125', '2027-12-31', '2024-12-01', 2338920, 131394),
(1, 'TH00000090', 'L356_0125', '2027-12-31', '2024-12-01', 2338920, 93263),
(1, 'TH00000091', 'L507_0125', '2027-12-31', '2024-12-01', 2338920, 107202),
(1, 'TH00000092', 'L438_0125', '2027-12-31', '2024-12-01', 2338920, 91183),
(1, 'TH00000093', 'L920_0125', '2027-12-31', '2024-12-01', 17929080, 78387),
(1, 'TH00000094', 'L307_0125', '2027-12-31', '2024-12-01', 44640, 60398),
(1, 'TH00000095', 'L420_0125', '2027-12-31', '2024-12-01', 63540, 109958),
(1, 'TH00000096', 'L772_0125', '2027-12-31', '2024-12-01', 2338920, 108423),
(1, 'TH00000097', 'L288_0125', '2027-12-31', '2024-12-01', 1375920, 98233),
(1, 'TH00000098', 'L561_0125', '2027-12-31', '2024-12-01', 999720, 63104),
(1, 'TH00000099', 'L779_0125', '2027-12-31', '2024-12-01', 825480, 51864),
(1, 'TH00000100', 'L247_0125', '2027-12-31', '2024-12-01', 1375920, 53937),
(1, 'TH00000101', 'L836_0125', '2027-12-31', '2024-12-01', 2338920, 58080),
(1, 'TH00000102', 'L259_0125', '2027-12-31', '2024-12-01', 5674500, 141048),
(1, 'TH00000103', 'L324_0125', '2027-12-31', '2024-12-01', 2338920, 146065),
(1, 'TH00000104', 'L531_0125', '2027-12-31', '2024-12-01', 2338920, 50851),
(1, 'TH00000105', 'L798_0125', '2027-12-31', '2024-12-01', 18180, 99880),
(1, 'TH00000106', 'L832_0125', '2027-12-31', '2024-12-01', 68580, 149930),
(1, 'TH00000107', 'L151_0125', '2027-12-31', '2024-12-01', 2338920, 81359),
(1, 'TH00000108', 'L493_0125', '2027-12-31', '2024-12-01', 14000, 101335),
(1, 'TH00000109', 'L187_0125', '2027-12-31', '2024-12-01', 2338920, 120909),
(1, 'TH00000110', 'L485_0125', '2027-12-31', '2024-12-01', 2338920, 138448),
(1, 'TH00000111', 'L696_0125', '2027-12-31', '2024-12-01', 2338920, 114406),
(1, 'TH00000112', 'L136_0125', '2027-12-31', '2024-12-01', 2338920, 125517),
(1, 'TH00000113', 'L269_0125', '2027-12-31', '2024-12-01', 4127760, 101433),
(1, 'TH00000114', 'L731_0125', '2027-12-31', '2024-12-01', 2338920, 113258),
(1, 'TH00000115', 'L539_0125', '2027-12-31', '2024-12-01', 2338920, 95622),
(1, 'TH00000116', 'L409_0125', '2027-12-31', '2024-12-01', 2338920, 74682),
(1, 'TH00000117', 'L454_0125', '2027-12-31', '2024-12-01', 2338920, 60453),
(1, 'TH00000118', 'L576_0125', '2027-12-31', '2024-12-01', 1100700, 140758),
(1, 'TH00000119', 'L350_0125', '2027-12-31', '2024-12-01', 2338920, 70496),
(1, 'TH00000120', 'L886_0125', '2027-12-31', '2024-12-01', 2338920, 112750),
(1, 'TH00000121', 'L387_0125', '2027-12-31', '2024-12-01', 2338920, 53429),
(1, 'TH00000122', 'L397_0125', '2027-12-31', '2024-12-01', 2338920, 118938),
(1, 'TH00000123', 'L886_0125', '2027-12-31', '2024-12-01', 2338920, 133252),
(1, 'TH00000124', 'L717_0125', '2027-12-31', '2024-12-01', 16740, 144191),
(1, 'TH00000125', 'L979_0125', '2027-12-31', '2024-12-01', 41277600, 127108),
(1, 'TH00000126', 'L672_0125', '2027-12-31', '2024-12-01', 2338920, 70339),
(1, 'TH00000127', 'L127_0125', '2027-12-31', '2024-12-01', 4300740, 97666),
(1, 'TH00000128', 'L906_0125', '2027-12-31', '2024-12-01', 2338920, 92656),
(1, 'TH00000129', 'L312_0125', '2027-12-31', '2024-12-01', 2338920, 123431),
(1, 'TH00000130', 'L443_0125', '2027-12-31', '2024-12-01', 2338920, 86443),
(1, 'TH00000131', 'L714_0125', '2027-12-31', '2024-12-01', 20340, 85101),
(1, 'TH00000132', 'L910_0125', '2027-12-31', '2024-12-01', 2751840, 120143),
(1, 'TH00000133', 'L534_0125', '2027-12-31', '2024-12-01', 825480, 60708),
(1, 'TH00000134', 'L294_0125', '2027-12-31', '2024-12-01', 5879700, 114729),
(1, 'TH00000135', 'L843_0125', '2027-12-31', '2024-12-01', 2105820, 119672),
(1, 'TH00000136', 'L388_0125', '2027-12-31', '2024-12-01', 2338920, 120260),
(1, 'TH00000137', 'L187_0125', '2027-12-31', '2024-12-01', 2338920, 72278),
(1, 'TH00000138', 'L729_0125', '2027-12-31', '2024-12-01', 18900, 93854),
(1, 'TH00000139', 'L226_0125', '2027-12-31', '2024-12-01', 360180, 82894),
(1, 'TH00000140', 'L884_0125', '2027-12-31', '2024-12-01', 1651104000, 86753),
(1, 'TH00000141', 'L833_0125', '2027-12-31', '2024-12-01', 371340, 114205),
(1, 'TH00000142', 'L899_0125', '2027-12-31', '2024-12-01', 4330260, 80608),
(1, 'TH00000143', 'L441_0125', '2027-12-31', '2024-12-01', 6361200, 104354),
(1, 'TH00000144', 'L604_0125', '2027-12-31', '2024-12-01', 2014380, 115008),
(1, 'TH00000145', 'L975_0125', '2027-12-31', '2024-12-01', 2338920, 67113),
(1, 'TH00000146', 'L392_0125', '2027-12-31', '2024-12-01', 2338920, 102148),
(1, 'TH00000147', 'L955_0125', '2027-12-31', '2024-12-01', 2338920, 52615),
(1, 'TH00000148', 'L240_0125', '2027-12-31', '2024-12-01', 2338920, 85118),
(1, 'TH00000149', 'L214_0125', '2027-12-31', '2024-12-01', 2338920, 90908),
(1, 'TH00000150', 'L728_0125', '2027-12-31', '2024-12-01', 2338920, 87980),
(1, 'TH00000151', 'L960_0125', '2027-12-31', '2024-12-01', 16020, 118525),
(1, 'TH00000152', 'L178_0125', '2027-12-31', '2024-12-01', 2338920, 101046),
(1, 'TH00000153', 'L339_0125', '2027-12-31', '2024-12-01', 1865520, 133990),
(1, 'TH00000154', 'L553_0125', '2027-12-31', '2024-12-01', 5681160, 52781),
(1, 'TH00000155', 'L686_0125', '2027-12-31', '2024-12-01', 246060, 117797),
(1, 'TH00000156', 'L951_0125', '2027-12-31', '2024-12-01', 2338920, 138878),
(1, 'TH00000157', 'L474_0125', '2027-12-31', '2024-12-01', 447120, 147332),
(1, 'TH00000158', 'L523_0125', '2027-12-31', '2024-12-01', 2338920, 85787),
(1, 'TH00000159', 'L832_0125', '2027-12-31', '2024-12-01', 152280, 51332),
(1, 'TH00000160', 'L583_0125', '2027-12-31', '2024-12-01', 2338920, 53237),
(1, 'TH00000161', 'L753_0125', '2027-12-31', '2024-12-01', 464940, 58249),
(1, 'TH00000162', 'L283_0125', '2027-12-31', '2024-12-01', 57240, 72382),
(1, 'TH00000163', 'L862_0125', '2027-12-31', '2024-12-01', 2338920, 113034),
(1, 'TH00000164', 'L786_0125', '2027-12-31', '2024-12-01', 102420, 129532),
(1, 'TH00000165', 'L483_0125', '2027-12-31', '2024-12-01', 2338920, 106328),
(1, 'TH00000166', 'L369_0125', '2027-12-31', '2024-12-01', 2338920, 113573),
(1, 'TH00000167', 'L192_0125', '2027-12-31', '2024-12-01', 2338920, 123490),
(1, 'TH00000168', 'L531_0125', '2027-12-31', '2024-12-01', 3669120, 123168),
(1, 'TH00000169', 'L869_0125', '2027-12-31', '2024-12-01', 32040, 116961),
(1, 'TH00000170', 'L361_0125', '2027-12-31', '2024-12-01', 13782060, 119666),
(1, 'TH00000171', 'L858_0125', '2027-12-31', '2024-12-01', 2338920, 148404),
(1, 'TH00000172', 'L472_0125', '2027-12-31', '2024-12-01', 8971740, 93118),
(1, 'TH00000173', 'L947_0125', '2027-12-31', '2024-12-01', 21960, 128216),
(1, 'TH00000174', 'L540_0125', '2027-12-31', '2024-12-01', 250920, 147885),
(1, 'TH00000175', 'L179_0125', '2027-12-31', '2024-12-01', 18345600, 128406),
(1, 'TH00000176', 'L631_0125', '2027-12-31', '2024-12-01', 2338920, 64142),
(1, 'TH00000177', 'L143_0125', '2027-12-31', '2024-12-01', 2338920, 138784),
(1, 'TH00000178', 'L414_0125', '2027-12-31', '2024-12-01', 12671460, 149138),
(1, 'TH00000179', 'L861_0125', '2027-12-31', '2024-12-01', 25844400, 126221),
(1, 'TH00000180', 'L543_0125', '2027-12-31', '2024-12-01', 2759940, 83492),
(1, 'TH00000181', 'L981_0125', '2027-12-31', '2024-12-01', 2338920, 147497),
(1, 'TH00000182', 'L197_0125', '2027-12-31', '2024-12-01', 2338920, 99724),
(1, 'TH00000183', 'L243_0125', '2027-12-31', '2024-12-01', 2338920, 145201),
(1, 'TH00000184', 'L934_0125', '2027-12-31', '2024-12-01', 2338920, 80527),
(1, 'TH00000185', 'L819_0125', '2027-12-31', '2024-12-01', 2338920, 75872),
(1, 'TH00000186', 'L357_0125', '2027-12-31', '2024-12-01', 2338920, 70725),
(1, 'TH00000187', 'L848_0125', '2027-12-31', '2024-12-01', 2338920, 108727),
(1, 'TH00000188', 'L130_0125', '2027-12-31', '2024-12-01', 1650960, 59801),
(1, 'TH00000189', 'L581_0125', '2027-12-31', '2024-12-01', 8055720, 89606),
(1, 'TH00000190', 'L592_0125', '2027-12-31', '2024-12-01', 2338920, 88797),
(1, 'TH00000191', 'L978_0125', '2027-12-31', '2024-12-01', 2338920, 129539),
(1, 'TH00000192', 'L892_0125', '2027-12-31', '2024-12-01', 2338920, 78635),
(1, 'TH00000193', 'L343_0125', '2027-12-31', '2024-12-01', 2338920, 132754),
(1, 'TH00000194', 'L161_0125', '2027-12-31', '2024-12-01', 2338920, 52251),
(1, 'TH00000195', 'L476_0125', '2027-12-31', '2024-12-01', 2322540, 92640),
(1, 'TH00000196', 'L345_0125', '2027-12-31', '2024-12-01', 1100700, 66399),
(1, 'TH00000197', 'L464_0125', '2027-12-31', '2024-12-01', 2338920, 120584),
(1, 'TH00000198', 'L673_0125', '2027-12-31', '2024-12-01', 2338920, 52436),
(1, 'TH00000199', 'L807_0125', '2027-12-31', '2024-12-01', 7375140, 60567),
(1, 'TH00000200', 'L746_0125', '2027-12-31', '2024-12-01', 48600, 105678),
(1, 'TH00000201', 'L773_0125', '2027-12-31', '2024-12-01', 4140, 122686),
(1, 'TH00000202', 'L629_0125', '2027-12-31', '2024-12-01', 2338920, 63227),
(1, 'TH00000203', 'L953_0125', '2027-12-31', '2024-12-01', 2338920, 75128),
(1, 'TH00000204', 'L533_0125', '2027-12-31', '2024-12-01', 2338920, 91726),
(1, 'TH00000205', 'L830_0125', '2027-12-31', '2024-12-01', 2338920, 107763),
(1, 'TH00000206', 'L532_0125', '2027-12-31', '2024-12-01', 2338920, 87495),
(1, 'TH00000207', 'L371_0125', '2027-12-31', '2024-12-01', 2338920, 75380);
GO

UPDATE PHIEUNHAP SET TongTienNhap = (SELECT SUM(ThanhTien) FROM CT_PHIEUNHAP WHERE CT_PHIEUNHAP.MaPhieuNhap = PHIEUNHAP.MaPhieuNhap);
GO
