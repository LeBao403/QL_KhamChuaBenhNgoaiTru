# Bối cảnh Dự án (Project Context)
- **Tên Đề tài:** Xây dựng Website Quản lý Phòng khám Ngoại trú.
- **Tên dự án:** `QL_KhamChuaBenhNgoaiTru`.
- **Nền tảng công nghệ:** ASP.NET MVC (C# / .NET Framework), Frontend (HTML/CSS/JS, jQuery, Bootstrap), SQL Server.
- **Mục tiêu:** Số hóa toàn diện quy trình khám chữa bệnh ngoại trú, từ khâu đặt lịch, tiếp nhận bằng CCCD/BHYT, đến khám bệnh và thanh toán.

# Kiến trúc Hệ thống & Phân quyền (Architecture & Roles)
Dự án áp dụng mô hình **MVC** với cấu trúc thư mục rõ ràng:
1. **Public & Patient (Thư mục gốc Hệ thống chia làm 2 nhánh rõ rệt đối với người dùng cuối (End-User):
* **Nhánh Public & Shared (Khách vãng lai + Bệnh nhân):**
    * **`Controllers/HomeController.cs`**: Xử lý Landing Page và các nghiệp vụ không yêu cầu đăng nhập bắt buộc.
    * **`Views/Home/`**: Chứa giao diện trang chủ và các View chức năng dùng chung như: `DatLich.cshtml` (Guest và User đều dùng được), `TraCuuKQ.cshtml` (Tra cứu kết quả bằng mã phiếu), danh sách bác sĩ, bảng giá.
* **Nhánh Private (Chỉ dành cho Bệnh nhân đã đăng nhập):**
    * **`Controllers/BenhNhanController.cs`**: BẮT BUỘC phải có attribute `[Authorize]` hoặc logic kiểm tra Session ở mọi Action. Chỉ xử lý dữ liệu cá nhân của user đang đăng nhập.
    * **`Views/BenhNhan/`**: Chứa các giao diện quản lý hồ sơ cá nhân (Portal riêng): `LichSuKham.cshtml`, `ChiTietDonThuoc.cshtml`, `HoaDon.cshtml`, `ThongTinCaNhan.cshtml`.

### 2. Phân hệ Quản trị & Vận hành (Sử dụng Areas)
* **Area `Admin`:** Quản trị danh mục hệ thống (Khoa, Phòng, Dịch vụ, Nhân viên, Bệnh nhân...).
* **Area `Staff` (Nhân viên Y tế):**
    * `TiepTanController`: Quản lý "Hàng chờ Tiếp nhận" (sảnh chờ), check-in bằng CCCD/BHYT, gán Dịch vụ khám và phân luồng vào Phòng khám.
    * `BacSiController`: Quản lý "Hàng chờ Phòng khám" cá nhân, khám lâm sàng, kê đơn, hoàn tất ca khám.
* **Area `NhanVienKho`:** Quản lý xuất/nhập tồn kho thuốc và vật tư.

# Luồng Nghiệp vụ Cốt lõi (Core Business Workflow)
*Chú ý: AI phải tuân thủ nghiêm ngặt luồng dữ liệu và chuyển đổi trạng thái dưới đây khi viết code xử lý Controller và DBContext.*

### Bước 0: Đặt lịch khám Online (Landing Page)
- **Actor:** Khách vãng lai (Guest) hoặc Bệnh nhân đã đăng nhập.
- **Logic hiển thị:** - Nếu là Guest: Form yêu cầu nhập đầy đủ thông tin cá nhân (Họ tên, Tuổi, CCCD, BHYT...).
  - Nếu đã đăng nhập: Tự động fill thông tin cá nhân từ User Profile.
- **Dữ liệu bắt buộc:** Ngày/Giờ khám.
- **Dữ liệu tùy chọn:** Dịch vụ khám, Lý do khám.
- **Hành vi hệ thống:** Tạo bản ghi `PhieuDangKy` với trạng thái `DaDatLichOnline`.

### Bước 1 & 2: Tiếp nhận tại sảnh & Phân luồng (Area `Staff` - `TiepTanController`)
Hệ thống chia làm 2 luồng rõ rệt tại sảnh:

**Luồng A: Bệnh nhân đến trực tiếp (Walk-in)**
- **Hành động:** Quét mã CCCD hoặc mã BHYT (Hardware/Barcode Scanner).
- **Hành vi hệ thống:** 1. Auto-fill thông tin bệnh nhân từ mã vạch.
  2. Đẩy bệnh nhân vào **"Hàng chờ Tiếp nhận"** (Trạng thái: `ChoTiepNhan`).
- **Nghiệp vụ Tiếp tân:** Lấy bệnh nhân từ "Hàng chờ Tiếp nhận" -> Chọn *Dịch vụ khám* -> Nhập *Lý do khám*.

**Luồng B: Bệnh nhân đã đặt lịch online từ Bước 0**
- **Hành động:** Bệnh nhân đến sảnh báo danh.
- **Hành vi hệ thống:** Bỏ qua "Hàng chờ Tiếp nhận". Tiếp tân thực hiện **Check-in** trực tiếp từ danh sách hẹn. Nếu bệnh nhân chưa chọn *Dịch vụ khám* lúc đặt online, Tiếp tân sẽ chọn bổ sung.

**Điểm chung khi Phân phòng (Routing):**
- Khi *Dịch vụ khám* được xác định (cho cả Luồng A và Luồng B), hệ thống **phải tự động lọc và gợi ý** danh sách các `PhongKham` có chuyên môn tương ứng với dịch vụ đó.
- Tiếp tân chọn Phòng khám -> Hệ thống tạo `PhieuKhamBenh` -> Chuyển trạng thái bệnh nhân sang **`ChoKham`** và đẩy vào **"Hàng chờ Phòng khám"** cụ thể đó.

### Bước 3: Khám bệnh (Area `Staff` - `BacSiController`)
- **Actor:** Bác sĩ tại phòng khám.
- **Hành vi hệ thống:**
  1. Hiển thị **"Hàng chờ Phòng khám"**: Bác sĩ chỉ nhìn thấy danh sách bệnh nhân đang ở trạng thái `ChoKham` được gán định danh vào đúng ID Phòng khám của mình.
  2. Bác sĩ gọi bệnh nhân -> Trạng thái đổi thành `DangKham`.
  3. Thực hiện khám lâm sàng, chỉ định thêm Cận lâm sàng (nếu cần), kê `DonThuoc` (trừ tồn kho logic).
  4. Hoàn tất -> Trạng thái đổi thành `DaKham` -> Chuyển dữ liệu sang bộ phận Kế toán/Thu ngân sinh `HoaDon`.

# Quy tắc Lập trình (Strict AI Coding Rules)
1. **Truy xuất CSDL (Data Access):** - Tuyệt đối KHÔNG viết trực tiếp câu lệnh Entity Framework/LINQ hoặc ADO.NET trong file `Controller`.
   - Mọi thao tác Database phải được gọi qua thư mục `DBContext/` (Ví dụ: `TiepTanDB.cs`, `BacSiDB.cs`, `BenhNhanDB.cs`).
2. **Xử lý Hàng chờ (Queues):**
   - Phân biệt rõ tham số khi truy vấn: "Hàng chờ tiếp nhận" lọc theo `TrangThai = ChoTiepNhan`. "Hàng chờ phòng khám" lọc theo `TrangThai = ChoKham` VÀ `PhongID = @CurrentPhongID`.
3. **Tính toàn vẹn Dữ liệu (Transactions):**
   - Khi chuyển trạng thái bệnh nhân từ Check-in sang Hàng chờ phòng khám (tạo `PhieuKhamBenh`), phải dùng `SqlTransaction` để đảm bảo không bị mất dữ liệu giữa chừng.
4. **Trải nghiệm người dùng (UX):**
   - Khi Tiếp tân chọn "Dịch vụ khám" trong View (Dropdown), hãy viết mã AJAX (jQuery) để tự động load danh sách "Phòng khám" tương ứng ra một Dropdown khác mà không cần reload trang.