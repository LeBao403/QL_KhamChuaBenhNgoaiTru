USE QL_KhamBenhNgoaiTru;
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
INSERT INTO BENHNHAN (MaBN, HoTen, NgaySinh, GioiTinh, CCCD, SDT, Email, DiaChi, BHYT, MaTK)
VALUES
-- NGƯỜI LỚN
('KH0001', N'Nguyễn Văn Hùng', '1985-03-12', N'Nam', '012345678901', '0908000001', 'hung.nguyen@vnvc.vn', N'Quận 1, TP.HCM', 1, 87),
('KH0002', N'Trần Thị Lan', '1990-07-22', N'Nữ', '012345678902', '0908000002', 'lan.tran@vnvc.vn', N'Quận Hai Bà Trưng, Hà Nội', 1, 88),
('KH0003', N'Lê Văn Minh', '1982-11-10', N'Nam', '012345678903', '0908000003', 'minh.le@vnvc.vn', N'Hải Châu, Đà Nẵng', 0, 89),
('KH0004', N'Phạm Thị Hoa', '1995-05-09', N'Nữ', '012345678904', '0908000004', 'hoa.pham@vnvc.vn', N'Bình Thủy, Cần Thơ', 1, 90),
('KH0005', N'Hoàng Văn Tuấn', '1989-09-15', N'Nam', '012345678905', '0908000005', 'tuan.hoang@vnvc.vn', N'Ngô Quyền, Hải Phòng', 0, 91),
('KH0006', N'Võ Thị Hạnh', '1987-12-01', N'Nữ', '012345678906', '0908000006', 'hanh.vo@vnvc.vn', N'TP. Vinh, Nghệ An', 1, 92),
('KH0007', N'Đặng Văn Long', '1984-04-18', N'Nam', '012345678907', '0908000007', 'long.dang@vnvc.vn', N'Thủ Dầu Một, Bình Dương', 0, 93),
('KH0008', N'Ngô Thị Mai', '1991-06-25', N'Nữ', '012345678908', '0908000008', 'mai.ngo@vnvc.vn', N'TP. Nha Trang, Khánh Hòa', 1, 94),
('KH0009', N'Bùi Văn Khánh', '1986-08-30', N'Nam', '012345678909', '0908000009', 'khanh.bui@vnvc.vn', N'TP. Huế, Thừa Thiên Huế', 0, 95),
('KH0010', N'Đỗ Thị Yến', '1993-01-19', N'Nữ', '012345678910', '0908000010', 'yen.do@vnvc.vn', N'TP. Biên Hòa, Đồng Nai', 1, 96),

('KH0011', N'Nguyễn Thị Hương', '1988-06-14', N'Nữ', '012345678911', '0908000011', 'huong.nguyen@vnvc.vn', N'Nam Từ Liêm, Hà Nội', 1, 97),
('KH0012', N'Trần Văn Nam', '1981-02-21', N'Nam', '012345678912', '0908000012', 'nam.tran@vnvc.vn', N'Quận 3, TP.HCM', 0, 98),
('KH0013', N'Lê Thị Thu', '1996-08-09', N'Nữ', '012345678913', '0908000013', 'thu.le@vnvc.vn', N'Quận 7, TP.HCM', 1, 99),
('KH0014', N'Phạm Văn Hoàng', '1983-04-27', N'Nam', '012345678914', '0908000014', 'hoang.pham@vnvc.vn', N'Hồng Bàng, Hải Phòng', 1, 100),
('KH0015', N'Hoàng Thị Mai', '1989-10-13', N'Nữ', '012345678915', '0908000015', 'mai.hoang@vnvc.vn', N'Thanh Khê, Đà Nẵng', 0, 101),
('KH0016', N'Vũ Văn Dũng', '1986-07-19', N'Nam', '012345678916', '0908000016', 'dung.vu@vnvc.vn', N'Thủ Đức, TP.HCM', 1, 102),
('KH0017', N'Đinh Thị Hòa', '1992-03-05', N'Nữ', '012345678917', '0908000017', 'hoa.dinh@vnvc.vn', N'Hồng Bàng, Hải Phòng', 0, 103),
('KH0018', N'Nguyễn Văn Lâm', '1987-09-11', N'Nam', '012345678918', '0908000018', 'lam.nguyen@vnvc.vn', N'Cẩm Lệ, Đà Nẵng', 1, 104),
('KH0019', N'Trần Thị Ngọc', '1994-05-24', N'Nữ', '012345678919', '0908000019', 'ngoc.tran@vnvc.vn', N'Cầu Giấy, Hà Nội', 0, 105),
('KH0020', N'Lê Văn Thành', '1985-12-30', N'Nam', '012345678920', '0908000020', 'thanh.le@vnvc.vn', N'Huế', 1, 106),

('KH0021', N'Đoàn Minh Phúc', '1985-11-20', N'Nam', '026985001234', '0915789456', 'minhphuc.doan@example.com', N'25 Lê Lợi, Hà Nội', 1, 107),
('KH0022', N'Lê Hải Yến', '1991-04-10', N'Nữ', '034896752134', '0902349876', 'haiyen.le@example.com', N'88 Nguyễn Văn Cừ, Bắc Ninh', 0, 108),
('KH0023', N'Trần Nhật Nam', '1978-07-05', N'Nam', '021548963215', '0984563217', 'nhatnam.tran@example.com', N'15 Lạch Tray, Hải Phòng', 1, 109),
('KH0024', N'Nguyễn Thị Mỹ Duyên', '1989-12-12', N'Nữ', '025489631254', '0932145896', 'myduyen.nguyen@example.com', N'42 Trần Quang Diệu, Hà Nội', 0, 110),
('KH0025', N'Phạm Văn Khánh', '1993-09-14', N'Nam', '028945632541', '0916325478', 'vankhanh.pham@example.com', N'19 Hùng Vương, Đà Nẵng', 1, 111),
('KH0026', N'Hoàng Lan Chi', '1986-06-18', N'Nữ', '037894562315', '0978456321', 'lanchi.hoang@example.com', N'67 Nguyễn Chí Thanh, Hà Nội', 0, 112),
('KH0027', N'Nguyễn Quang Huy', '1990-02-27', N'Nam', '022145896321', '0931458796', 'quanghuy.nguyen@example.com', N'8 Đường 30/4, Cần Thơ', 1, 113),
('KH0028', N'Lý Thị Bảo Ngọc', '1992-05-22', N'Nữ', '029875463215', '0907856321', 'baongoc.ly@example.com', N'33 Võ Thị Sáu, TP.HCM', 0, 114),
('KH0029', N'Phan Hoàng Long', '1981-03-03', N'Nam', '027894562317', '0917458963', 'hoanglong.phan@example.com', N'12 Phan Bội Châu, Đà Nẵng', 1, 115),
('KH0030', N'Vũ Thu Trang', '1995-08-28', N'Nữ', '024785693214', '0932147856', 'thutrang.vu@example.com', N'55 Nguyễn Du, Hà Nội', 0, 116),

('KH0031', N'Nguyễn Hữu Toàn', '1979-01-10', N'Nam', '023654987521', '0987456321', 'huutoan.nguyen@example.com', N'9 Phạm Văn Đồng, TP.HCM', 1, 117),
('KH0032', N'Trịnh Mai Phương', '1987-10-17', N'Nữ', '035214789652', '0978562143', 'maiphuong.trinh@example.com', N'21 Nguyễn Trãi, Hà Nội', 0, 118),
('KH0033', N'Tạ Văn Cường', '1984-04-21', N'Nam', '038965214785', '0912348756', 'vancuong.ta@example.com', N'18 Lê Lợi, Thanh Hóa', 1, 119),
('KH0034', N'Nguyễn Kim Oanh', '1991-09-19', N'Nữ', '022154789632', '0907854213', 'kimoanh.nguyen@example.com', N'70 Cách Mạng Tháng 8, TP.HCM', 0, 120),
('KH0035', N'Đỗ Quang Minh', '1983-06-25', N'Nam', '021478965321', '0914789562', 'quangminh.do@example.com', N'32 Hoàng Hoa Thám, Hà Nội', 1, 121),
('KH0036', N'Lưu Thanh Hà', '1988-07-07', N'Nữ', '026321547896', '0932654789', 'thanhha.luu@example.com', N'11 Ngô Quyền, Hải Phòng', 0, 122),
('KH0037', N'Ngô Văn An', '1990-05-05', N'Nam', '024789632154', '0907896541', 'vann.an@example.com', N'23 Nguyễn Văn Linh, Đà Nẵng', 1, 123),
('KH0038', N'Trần Thị Thu Hằng', '1992-11-15', N'Nữ', '027896321547', '0978456213', 'thuhang.tran@example.com', N'101 Hai Bà Trưng, Hà Nội', 0, 124),
('KH0039', N'Nguyễn Đức Mạnh', '1985-12-30', N'Nam', '025478963215', '0912354789', 'ducmanh.nguyen@example.com', N'14 Điện Biên Phủ, TP.HCM', 1, 125),
('KH0040', N'Phạm Thị Hòa', '1989-03-08', N'Nữ', '021547896325', '0932145879', 'thihoa.pham@example.com', N'7 Nguyễn Huệ, Huế', 0, 126),

('KH0041', N'Nguyễn Văn Thắng', '1986-09-12', N'Nam', '023654789521', '0914785963', 'vanthang.nguyen@example.com', N'5 Hoàng Văn Thụ, Hà Nội', 1, 127),
('KH0042', N'Trần Ngọc Ánh', '1993-01-29', N'Nữ', '029875641325', '0901452369', 'ngocanh.tran@example.com', N'77 Bạch Đằng, Đà Nẵng', 0, 128),
('KH0043', N'Lê Minh Quân', '1980-02-02', N'Nam', '028974563215', '0987451236', 'minhquan.le@example.com', N'28 Trần Hưng Đạo, TP.HCM', 1, 129),
('KH0044', N'Nguyễn Thanh Mai', '1987-08-18', N'Nữ', '025478963214', '0932147852', 'thanhmai.nguyen@example.com', N'44 Nguyễn Khuyến, Hà Nội', 0, 130),
('KH0045', N'Phan Văn Đức', '1982-05-09', N'Nam', '026325478965', '0912564789', 'vanduc.phan@example.com', N'35 Nguyễn Thị Minh Khai, Huế', 1, 131),
('KH0046', N'Hồ Thị Yến Nhi', '1994-04-14', N'Nữ', '037854621354', '0903248756', 'yennhi.ho@example.com', N'10 Bùi Thị Xuân, Đà Nẵng', 0, 132),
('KH0047', N'Nguyễn Văn Hải', '1983-03-30', N'Nam', '021478563219', '0978563214', 'vanhai.nguyen@example.com', N'60 Lý Thường Kiệt, Hà Nội', 1, 133),
('KH0048', N'Đinh Thị Thảo', '1988-10-22', N'Nữ', '024789653214', '0914785632', 'thithao.dinh@example.com', N'17 Trần Cao Vân, TP.HCM', 0, 134),
('KH0049', N'Võ Quang Khải', '1981-06-06', N'Nam', '023698745123', '0902147856', 'quangkhai.vo@example.com', N'19 Nguyễn Công Trứ, Đà Nẵng', 1, 135),
('KH0050', N'Lê Mỹ Linh', '1990-12-19', N'Nữ', '022541236985', '0987456325', 'mylinh.le@example.com', N'82 Nguyễn Trường Tộ, Hà Nội', 0, 136),

-- 20 TRẺ EM
('KH0051', N'Nguyễn Anh Tuấn', '2015-04-10', N'Nam', NULL, NULL, NULL, N'Quận 1, TP.HCM', 1, NULL),
('KH0052', N'Trần Ngọc Bích', '2017-08-22', N'Nữ', NULL, NULL, NULL, N'Hà Đông, Hà Nội', 0, NULL),
('KH0053', N'Lê Đức Huy', '2014-12-30', N'Nam', NULL, NULL, NULL, N'Hải Châu, Đà Nẵng', 1, NULL),
('KH0054', N'Phạm Thảo Nhi', '2016-03-05', N'Nữ', NULL, NULL, NULL, N'Ngô Quyền, Hải Phòng', 0, NULL),
('KH0055', N'Hoàng Gia Bảo', '2013-11-15', N'Nam', NULL, NULL, NULL, N'Bình Thủy, Cần Thơ', 1, NULL),
('KH0056', N'Võ Minh Quân', '2018-01-25', N'Nam', NULL, NULL, NULL, N'TP. Vinh, Nghệ An', 0, NULL),
('KH0057', N'Đặng Khánh Linh', '2019-09-07', N'Nữ', NULL, NULL, NULL, N'Thủ Dầu Một, Bình Dương', 1, NULL),
('KH0058', N'Ngô Hoài Nam', '2014-02-18', N'Nam', NULL, NULL, NULL, N'TP. Nha Trang, Khánh Hòa', 0, NULL),
('KH0059', N'Bùi Thanh Thảo', '2016-10-21', N'Nữ', NULL, NULL, NULL, N'TP. Huế, Thừa Thiên Huế', 1, NULL),
('KH0060', N'Đỗ Minh Anh', '2012-07-14', N'Nam', NULL, NULL, NULL, N'TP. Biên Hòa, Đồng Nai', 0, NULL),

('KH0061', N'Nguyễn Phương Thảo', '2015-06-02', N'Nữ', NULL, NULL, NULL, N'Quận 5, TP.HCM', 1, NULL),
('KH0062', N'Trần Hoàng Nam', '2013-11-09', N'Nam', NULL, NULL, NULL, N'Nam Từ Liêm, Hà Nội', 0, NULL),
('KH0063', N'Lê Khánh Vy', '2017-02-25', N'Nữ', NULL, NULL, NULL, N'Hải Châu, Đà Nẵng', 1, NULL),
('KH0064', N'Phạm Anh Dũng', '2014-08-19', N'Nam', NULL, NULL, NULL, N'Ngô Quyền, Hải Phòng', 0, NULL),
('KH0065', N'Hoàng Bảo Ngọc', '2016-12-28', N'Nữ', NULL, NULL, NULL, N'Bình Thủy, Cần Thơ', 1, NULL),
('KH0066', N'Võ Hữu Tài', '2015-10-10', N'Nam', NULL, NULL, NULL, N'TP. Vinh, Nghệ An', 0, NULL),
('KH0067', N'Đặng Ngọc Hân', '2018-04-23', N'Nữ', NULL, NULL, NULL, N'Thủ Dầu Một, Bình Dương', 1, NULL),
('KH0068', N'Ngô Minh Đức', '2013-01-07', N'Nam', NULL, NULL, NULL, N'TP. Nha Trang, Khánh Hòa', 0, NULL),
('KH0069', N'Bùi Hồng Anh', '2017-09-17', N'Nữ', NULL, NULL, NULL, N'TP. Huế, Thừa Thiên Huế', 1, NULL),
('KH0070', N'Đỗ Quốc Khánh', '2014-05-12', N'Nam', NULL, NULL, NULL, N'TP. Biên Hòa, Đồng Nai', 0, NULL),

('KH0071', N'Lê Quân Bảo', '2004-03-04', N'Nam', NULL, '0383156689', 'baocenzo@gmail.com', N'TP. Huế, Thừa Thiên Huế', 1, 137),
('KH0072', N'Đinh Đỗ Quỳnh Như', '2004-09-01', N'Nữ', NULL, '0976865715', 'dinhdoquynhnhuss@gmail.com', N'TP. Biên Hòa, Đồng Nai', 0, 138),
('KH0073', N'Lại Phước Thịnh', '2004-09-01', N'Nam', NULL, '0978610519', 'laiphuocthinh.bp@gmail.com', N'TP. Biên Hòa, Đồng Nai', 1, 139);



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
	(N'Nhân viên tạp vụ');
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
    (N'Phòng khám'), 
    (N'Phòng xét nghiệm'), 
    (N'Phòng X-Quang'), 
    (N'Phòng siêu âm'), 
    (N'Nhà thuốc'), 
    (N'Thu ngân'), 
    (N'Kho');
GO

INSERT INTO PHONG (TenPhong, MaLoaiPhong, TrangThai, MaKhoa) VALUES 
-- Khu vực hành chính & Tiếp nhận
(N'Sảnh tiếp đón & Đăng ký', 1, 1, 1), -- Thuộc Khoa Khám bệnh (ID 1)
(N'Quầy thu ngân & Thanh toán', 6, 1, NULL), -- Thu ngân tách riêng (Phòng kế toán) nên để NULL

-- Khu vực khám chuyên khoa
(N'Phòng khám Nội tổng quát 01', 1, 1, 2),
(N'Phòng khám Nội tổng quát 02', 1, 1, 2),
(N'Phòng khám Ngoại tổng hợp 01', 1, 1, 3),
(N'Phòng khám Ngoại tổng hợp 02', 1, 1, 3),
(N'Phòng khám Nhi khoa 01', 1, 1, 4),
(N'Phòng khám Nhi khoa 02', 1, 1, 4),
(N'Phòng khám Sản phụ khoa 01', 1, 1, 5),
(N'Phòng khám Sản phụ khoa 02', 1, 1, 5),
(N'Phòng khám Tai Mũi Họng 01', 1, 1, 6),
(N'Phòng khám Tai Mũi Họng 02', 1, 1, 6),
(N'Phòng khám Răng Hàm Mặt 01', 1, 1, 7),
(N'Phòng khám Răng Hàm Mặt 02', 1, 1, 7),

-- [Bổ sung] Phòng khám cho Khoa Mắt (ID 8) và Da Liễu (ID 9)
(N'Phòng khám Mắt 01', 1, 1, 8),
(N'Phòng khám Da liễu 01', 1, 1, 9),

-- Khu vực Cận lâm sàng
(N'Phòng Xét nghiệm máu & Sinh hóa', 2, 1, 11), -- Thuộc Khoa Xét nghiệm (ID 11)
(N'Phòng Siêu âm tổng quát', 4, 1, 10),            -- Thuộc Khoa CĐHA (ID 10)
(N'Phòng Chụp X-Quang kỹ thuật số', 3, 1, 10),      -- Thuộc Khoa CĐHA (ID 10)

-- Khu vực Dược & Kho 
(N'Nhà thuốc 01', 5, 1, 12), -- Thuộc Khoa Dược (ID 12)
(N'Nhà thuốc 02', 5, 1, 12),
(N'Nhà thuốc 03', 5, 1, 12),
(N'Nhà thuốc 04', 5, 1, 12),
(N'Nhà thuốc 05', 5, 1, 12),
(N'Kho tổng dược phẩm', 7, 1, 12);
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
('NV040', N'Nguyễn Khánh An', '1994-04-04', N'Nữ', '0903666000', 'an.nk@vnvc.vn', N'12 Điện Biên Phủ, Đà Nẵng', 5, 1, 40, 4, NULL, NULL), 
('NV041', N'Vũ Thị Thanh', '1996-05-28', N'Nữ', '0903777000', 'thanh.vt@vnvc.vn', N'80 Nguyễn Văn Linh, Đà Nẵng', 5, 1, 41, 5, NULL, NULL), 
('NV042', N'Nguyễn Minh Đức', '1987-06-16', N'Nam', '0903888000', 'duc.nm@vnvc.vn', N'40 Bạch Đằng, Đà Nẵng', 6, 1, 42, 11, NULL, NULL),
('NV043', N'Hồ Văn Tài', '1992-07-07', N'Nam', '0903999000', 'tai.hv@vnvc.vn', N'90 Trần Phú, Đà Nẵng', 6, 1, 43, 10, NULL, NULL), 
('NV044', N'Phạm Thị Thuỷ', '1990-08-19', N'Nữ', '0903111333', 'thuy.pt@vnvc.vn', N'15 Lê Duẩn, Đà Nẵng', 8, 1, 44, 1, 1, NULL), 
('NV045', N'Trần Quốc Việt', '1993-09-30', N'Nam', '0903222555', 'viet.tq@vnvc.vn', N'110 Nguyễn Văn Linh, Đà Nẵng', 8, 0, 45, 1, NULL, NULL);
GO

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

INSERT INTO DICHVU (MaDV, TenDV, MaLoaiDV, GiaDichVu, DonViTinh, CoBHYT, GiaBHYT, TrangThai, MoTa) VALUES
-- ================= NHÓM 1: KHÁM BỆNH (LDV01) =================
('DV001', N'Khám bệnh Nội tổng quát', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám lâm sàng đánh giá sức khỏe tổng quát, tim mạch, hô hấp cơ bản.'),
('DV002', N'Khám bệnh Ngoại tổng quát', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám chấn thương, đánh giá chỉ định phẫu thuật ngoại khoa.'),
('DV003', N'Khám chuyên khoa Nhi', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám, đánh giá thể chất và tư vấn dinh dưỡng cho trẻ dưới 16 tuổi.'),
('DV004', N'Khám chuyên khoa Phụ Sản', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám thai định kỳ, khám các bệnh lý phụ khoa thường gặp.'),
('DV005', N'Khám chuyên khoa Tai Mũi Họng', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám lâm sàng tai mũi họng, lấy ráy tai, rửa mũi viêm xoang.'),
('DV006', N'Khám chuyên khoa Răng Hàm Mặt', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám răng miệng tổng quát, tư vấn nhổ răng, chỉnh nha.'),
('DV007', N'Khám chuyên khoa Mắt', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám mắt tổng quát, đo nhãn áp, kiểm tra thị lực.'),
('DV008', N'Khám chuyên khoa Da liễu', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám các bệnh lý ngoài da, dị ứng, viêm da cơ địa.'),
('DV009', N'Khám bệnh yêu cầu (Chọn Giáo sư/Tiến sĩ)', 'LDV01', 500000, N'Lần', 0, NULL, 1, N'Khám trực tiếp với chuyên gia đầu ngành. Không áp dụng BHYT.'),
('DV010', N'Khám cấp cứu ngoài giờ', 'LDV01', 250000, N'Lần', 1, 38700, 1, N'Phí khám ban đầu tại phòng cấp cứu ngoài giờ hành chính.'),
('DV011', N'Khám chuyên khoa Tim mạch', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám chuyên sâu bệnh lý huyết áp, suy tim, thiếu máu cơ tim.'),
('DV012', N'Khám chuyên khoa Thần kinh', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám rối loạn tiền đình, đau nửa đầu, đau dây thần kinh.'),
('DV013', N'Khám chuyên khoa Cơ Xương Khớp', 'LDV01', 150000, N'Lần', 1, 38700, 1, N'Khám thoái hóa khớp, gout, loãng xương, viêm đa khớp.'),

-- ================= NHÓM 2: XÉT NGHIỆM (LDV02) =================
('DV014', N'Tổng phân tích tế bào máu ngoại vi (máy laser)', 'LDV02', 90000, N'Mẫu', 1, 41200, 1, N'Đánh giá tình trạng thiếu máu, nhiễm trùng. Lấy máu đường tĩnh mạch.'),
('DV015', N'Định lượng Glucose (Đường huyết)', 'LDV02', 40000, N'Mẫu', 1, 21500, 1, N'Bệnh nhân bắt buộc nhịn ăn sáng tối thiểu 8 tiếng trước khi lấy máu.'),
('DV016', N'Xét nghiệm HbA1c', 'LDV02', 120000, N'Mẫu', 1, 80000, 1, N'Đánh giá chỉ số đường huyết trung bình trong 3 tháng qua (Tầm soát Tiểu đường).'),
('DV017', N'Định lượng Cholesterol toàn phần', 'LDV02', 45000, N'Mẫu', 1, 26600, 1, N'Xét nghiệm mỡ máu cơ bản. Cần nhịn ăn sáng.'),
('DV018', N'Định lượng Triglyceride, HDL, LDL (Bộ mỡ máu)', 'LDV02', 120000, N'Mẫu', 1, 75000, 1, N'Đánh giá chi tiết nguy cơ xơ vữa động mạch.'),
('DV019', N'Đo hoạt độ AST (GOT) / ALT (GPT) (Chức năng gan)', 'LDV02', 45000, N'Mẫu', 1, 21500, 1, N'Đánh giá tổn thương tế bào gan.'),
('DV020', N'Định lượng Creatinin (Chức năng thận)', 'LDV02', 40000, N'Mẫu', 1, 21500, 1, N'Đánh giá chức năng lọc của thận, tầm soát suy thận.'),
('DV021', N'Định lượng Acid Uric', 'LDV02', 45000, N'Mẫu', 1, 21500, 1, N'Tầm soát bệnh Gout (Gút).'),
('DV022', N'Tổng phân tích nước tiểu', 'LDV02', 50000, N'Mẫu', 1, 27400, 1, N'Lấy mẫu nước tiểu giữa dòng vào buổi sáng.'),
('DV023', N'Xét nghiệm viêm gan B (HBsAg test nhanh)', 'LDV02', 80000, N'Mẫu', 1, 52000, 1, N'Tầm soát kháng nguyên virus viêm gan B.'),
('DV024', N'Định lượng TSH, FT3, FT4 (Chức năng tuyến giáp)', 'LDV02', 250000, N'Mẫu', 1, 150000, 1, N'Tầm soát basedow, suy giáp, cường giáp.'),
('DV025', N'Xét nghiệm tầm soát ung thư gan (AFP)', 'LDV02', 250000, N'Mẫu', 0, NULL, 1, N'Marker ung thư gan. Dịch vụ tự nguyện, không áp dụng BHYT.'),
('DV026', N'Xét nghiệm nhóm máu ABO, Rh', 'LDV02', 100000, N'Mẫu', 1, 65000, 1, N'Định nhóm máu cơ bản.'),
('DV027', N'Test nhanh Cúm A/B', 'LDV02', 150000, N'Mẫu', 0, NULL, 1, N'Lấy dịch tỵ hầu kiểm tra virus cúm mùa.'),
('DV028', N'Test nhanh Sốt xuất huyết (Dengue NS1)', 'LDV02', 180000, N'Mẫu', 1, 80000, 1, N'Phát hiện sớm virus sốt xuất huyết trong những ngày đầu.'),

-- ================= NHÓM 3: CHẨN ĐOÁN HÌNH ẢNH (LDV03) =================
('DV029', N'Siêu âm ổ bụng tổng quát (Màu 4D)', 'LDV03', 250000, N'Lần', 1, 43900, 1, N'Siêu âm đánh giá gan, mật, tụy, thận. Cần nhịn tiểu căng.'),
('DV030', N'Siêu âm tuyến giáp', 'LDV03', 150000, N'Lần', 1, 43900, 1, N'Siêu âm phát hiện nang, nhân tuyến giáp.'),
('DV031', N'Siêu âm tuyến vú 2 bên', 'LDV03', 200000, N'Lần', 1, 43900, 1, N'Tầm soát u xơ, nang vú ở nữ giới.'),
('DV032', N'Siêu âm doppler tim', 'LDV03', 350000, N'Lần', 1, 222000, 1, N'Siêu âm màu đánh giá cấu trúc và chức năng van tim.'),
('DV033', N'Chụp X-Quang ngực thẳng (Kỹ thuật số)', 'LDV03', 120000, N'Lần', 1, 65400, 1, N'Chụp tim phổi. Phụ nữ có thai KHÔNG được chụp.'),
('DV034', N'Chụp X-Quang cột sống thắt lưng (2 tư thế)', 'LDV03', 150000, N'Lần', 1, 65400, 1, N'Phát hiện gai cột sống, thoái hóa, xẹp đốt sống.'),
('DV035', N'Chụp X-Quang sọ não (thẳng, nghiêng)', 'LDV03', 150000, N'Lần', 1, 65400, 1, N'Đánh giá vỡ xương sọ sau chấn thương.'),
('DV036', N'Chụp cắt lớp vi tính (CT Scan) sọ não không tiêm thuốc', 'LDV03', 1200000, N'Lần', 1, 536000, 1, N'Chẩn đoán đột quỵ não, xuất huyết não cấp.'),
('DV037', N'Chụp cắt lớp vi tính (CT Scan) lồng ngực', 'LDV03', 1500000, N'Lần', 1, 536000, 1, N'Tầm soát ung thư phổi, tổn thương phổi phức tạp.'),
('DV038', N'Chụp Cộng hưởng từ (MRI) khớp gối', 'LDV03', 2500000, N'Lần', 1, 1311000, 1, N'Đánh giá rách sụn chêm, đứt dây chằng chéo.'),
('DV039', N'Chụp Cộng hưởng từ (MRI) cột sống cổ', 'LDV03', 2500000, N'Lần', 1, 1311000, 1, N'Đánh giá thoát vị đĩa đệm chèn ép rễ thần kinh.'),

-- ================= NHÓM 4: THĂM DÒ CHỨC NĂNG (LDV04) =================
('DV040', N'Đo Điện tâm đồ (ECG)', 'LDV04', 80000, N'Lần', 1, 30500, 1, N'Phát hiện rối loạn nhịp tim, nhồi máu cơ tim.'),
('DV041', N'Đo chức năng hô hấp', 'LDV04', 150000, N'Lần', 1, 75000, 1, N'Chẩn đoán hen suyễn, bệnh phổi tắc nghẽn mạn tính (COPD).'),
('DV042', N'Điện não đồ (EEG)', 'LDV04', 180000, N'Lần', 1, 65000, 1, N'Chẩn đoán động kinh, rối loạn giấc ngủ.'),
('DV043', N'Đo mật độ xương (DEXA) 2 vị trí', 'LDV04', 350000, N'Lần', 0, NULL, 1, N'Đo loãng xương cổ xương đùi và cột sống thắt lưng.'),

-- ================= NHÓM 5: NỘI SOI (LDV05) =================
('DV044', N'Nội soi Tai Mũi Họng ống mềm', 'LDV05', 200000, N'Lần', 1, 104000, 1, N'Ống nội soi siêu nhỏ không gây buồn nôn, soi rõ dây thanh quản.'),
('DV045', N'Nội soi dạ dày - tá tràng (không sinh thiết)', 'LDV05', 600000, N'Lần', 1, 244000, 1, N'Nội soi tươi. Bệnh nhân cần nhịn ăn 6 tiếng trước soi.'),
('DV046', N'Nội soi dạ dày - tá tràng (Gây mê không đau)', 'LDV05', 1800000, N'Lần', 0, NULL, 1, N'Ngủ êm ái trong 15 phút. Cần người nhà đi cùng đưa về.'),
('DV047', N'Nội soi đại trực tràng (Gây mê)', 'LDV05', 2500000, N'Lần', 0, NULL, 1, N'Bệnh nhân cần uống thuốc xổ làm sạch ruột trước khi soi.'),

-- ================= NHÓM 6: THỦ THUẬT NGOẠI KHOA (LDV06) =================
('DV048', N'Thay băng, rửa vết thương chiều dài < 15cm', 'LDV06', 80000, N'Lần', 1, 46500, 1, N'Chăm sóc vết thương nhiễm trùng, vết mổ cũ.'),
('DV049', N'Cắt chỉ vết thương phần mềm', 'LDV06', 50000, N'Lần', 1, 30000, 1, N'Tháo chỉ sau phẫu thuật.'),
('DV050', N'Khâu vết thương phần mềm tổn thương nông < 5cm', 'LDV06', 250000, N'Lần', 1, 126000, 1, N'Gây tê tại chỗ, khâu thẩm mỹ vết rách.'),
('DV051', N'Chích rạch áp xe phần mềm', 'LDV06', 300000, N'Lần', 1, 150000, 1, N'Tháo mủ ổ áp xe, đặt dẫn lưu.'),
('DV052', N'Bó bột cẳng tay/cẳng chân', 'LDV06', 400000, N'Lần', 1, 200000, 1, N'Cố định gãy xương bằng bột thạch cao hoặc bột thủy tinh.'),

-- ================= NHÓM 7: RĂNG HÀM MẶT (LDV07) =================
('DV053', N'Cạo vôi răng (Lấy cao răng) đánh bóng hai hàm', 'LDV07', 250000, N'Lần', 0, NULL, 1, N'Sử dụng sóng siêu âm làm sạch mảng bám, không tổn thương men răng.'),
('DV054', N'Nhổ răng vĩnh viễn (Răng 1 đến 7)', 'LDV07', 300000, N'Răng', 1, 150000, 1, N'Gây tê tại chỗ, nhổ răng lung lay, sâu hỏng nặng.'),
('DV055', N'Nhổ răng khôn (Răng số 8) mọc lệch ngầm', 'LDV07', 1500000, N'Răng', 1, 450000, 1, N'Tiểu phẫu nhổ răng bằng máy Piezotome giảm đau.'),
('DV056', N'Trám răng thẩm mỹ bằng Composite', 'LDV07', 300000, N'Răng', 1, 115000, 1, N'Hàn răng sâu bằng vật liệu cùng màu răng thật.'),
('DV057', N'Tẩy trắng răng tại phòng khám (Laser)', 'LDV07', 2500000, N'Lần', 0, NULL, 1, N'Bật từ 2-3 tone màu sau 60 phút thực hiện. Dịch vụ thẩm mỹ.'),

-- ================= NHÓM 8: SẢN PHỤ KHOA (LDV08) =================
('DV058', N'Siêu âm thai 4D (Ghi hình màu)', 'LDV08', 350000, N'Lần', 0, NULL, 1, N'Khảo sát dị tật thai nhi, thấy rõ khuôn mặt bé. Tặng kèm USB lưu video.'),
('DV059', N'Đo tim thai (Monitor sản khoa)', 'LDV08', 120000, N'Lần', 1, 55000, 1, N'Đo cơn gò tử cung và nhịp tim thai nhi trong 30 phút.'),
('DV060', N'Tầm soát ung thư cổ tử cung (Xét nghiệm Pap/HPV)', 'LDV08', 450000, N'Lần', 0, NULL, 1, N'Lấy tế bào cổ tử cung. Cần thực hiện định kỳ hàng năm cho phụ nữ đã quan hệ.'),
('DV061', N'Cấy que tránh thai Implanon', 'LDV08', 3000000, N'Lần', 0, NULL, 1, N'Cấy que dưới da tay, hiệu quả ngừa thai lên đến 3 năm.'),

-- ================= NHÓM 9: THỦ THUẬT MẮT - DA LIỄU (LDV09) =================
('DV062', N'Đo khúc xạ mắt bằng máy tự động', 'LDV09', 50000, N'Lần', 1, 25000, 1, N'Đo độ cận, viễn, loạn thị.'),
('DV063', N'Lấy dị vật giác mạc nông', 'LDV09', 200000, N'Lần', 1, 85000, 1, N'Gây tê bề mặt và gắp dị vật ở lớp nông của mắt.'),
('DV064', N'Chích lẹo/chắp mắt', 'LDV09', 150000, N'Lần', 1, 75000, 1, N'Rạch tháo mủ mụn lẹo mi mắt.'),
('DV065', N'Đốt nốt ruồi / mụn thịt bằng Laser CO2', 'LDV09', 300000, N'Nốt', 0, NULL, 1, N'Thẩm mỹ không sẹo. Tự nguyện, không áp dụng BHYT.'),
('DV066', N'Lấy nhân mụn chuẩn y khoa', 'LDV09', 400000, N'Lần', 0, NULL, 1, N'Làm sạch sâu, nặn mụn vô khuẩn, chiếu đèn sinh học giảm sưng.');
GO