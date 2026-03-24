ALTER TABLE PHIEUDANGKY ADD LyDo NVARCHAR(500);
-- ================================================================
-- 1. Thêm cột AvatarPath vào bảng BENHNHAN
-- ================================================================
ALTER TABLE BENHNHAN
ADD AvatarPath NVARCHAR(500) NULL;

-- ================================================================
-- 2. Thêm cột cho đặt lịch khách chưa đăng nhập (PHIEUDANGKY)
--    Kiểm tra trước xem đã có chưa
-- ================================================================
-- Nếu chưa có cột HoTen trong PHIEUDANGKY, chạy:
ALTER TABLE PHIEUDANGKY ADD HoTen NVARCHAR(100) NULL;
ALTER TABLE PHIEUDANGKY ADD SDT NVARCHAR(15) NULL;
ALTER TABLE PHIEUDANGKY ADD CCCD NVARCHAR(20) NULL;

-- ================================================================
-- 3. Tạo folder lưu ảnh avatar (tùy chọn - app sẽ tự tạo)
-- ================================================================
-- Ảnh sẽ được lưu vào: ~/Images/benhnhan/
-- App sẽ tự tạo thư mục này khi upload lần đầu.
ALTER TABLE PHIEUKHAMBENH ADD ChanDoan NVARCHAR(500) NULL;