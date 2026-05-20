# CHƯƠNG 4: CÀI ĐẶT HỆ THỐNG

## 4.1. Môi trường cài đặt

Hệ thống quản lý khám chữa bệnh ngoại trú được cài đặt theo kiến trúc gồm ba thành phần chính: ứng dụng web ASP.NET MVC, dịch vụ AI FastAPI và ứng dụng di động Flutter. Trong phạm vi khóa luận, các thành phần được phát triển và kiểm thử trên môi trường local, sử dụng chung cơ sở dữ liệu SQL Server `QL_KhamBenhNgoaiTru`. Cách triển khai này giúp nhóm chủ động kiểm soát toàn bộ quy trình từ thiết kế dữ liệu, xử lý nghiệp vụ, huấn luyện mô hình gợi ý thuốc đến xây dựng giao diện cho người dùng cuối.

Ứng dụng web được phát triển bằng Visual Studio, .NET Framework 4.8, ASP.NET MVC 5.2.9, Razor View, Bootstrap, jQuery và SignalR. Tầng truy cập dữ liệu sử dụng ADO.NET thông qua các lớp `SqlConnection`, `SqlCommand`, `SqlDataReader`, `DataTable` và `SqlTransaction`. Hệ thống không sử dụng ORM nhằm giúp nhóm kiểm soát trực tiếp câu lệnh SQL, transaction và các stored procedure trong những nghiệp vụ có yêu cầu cao về tính toàn vẹn dữ liệu.

Dịch vụ AI được xây dựng bằng Python với FastAPI, Uvicorn, NumPy, pyodbc, Pydantic và python-dotenv. FastAPI đảm nhiệm việc cung cấp API huấn luyện và gợi ý thuốc; pyodbc kết nối đến SQL Server; NumPy được dùng cho các phép tính vector trong thuật toán Node2Vec do nhóm tự cài đặt. Ứng dụng di động được phát triển bằng Flutter và Dart, giao tiếp với Web App thông qua các API JSON để phục vụ bệnh nhân đặt lịch, xem lịch khám, lịch sử khám, đơn thuốc và hóa đơn.

## 4.2. Cài đặt cơ sở dữ liệu

### 4.2.1. Khởi tạo database

Cơ sở dữ liệu được khởi tạo trên SQL Server với tên `QL_KhamBenhNgoaiTru`. File `Create_QLKCB.sql` đảm nhiệm việc tạo database, tạo các bảng chính, khai báo khóa chính, khóa ngoại, ràng buộc `CHECK`, `UNIQUE` và các ràng buộc dữ liệu cần thiết. Sau khi tạo cấu trúc, các file dữ liệu mẫu trong thư mục `Database` được sử dụng để nạp dữ liệu ban đầu phục vụ kiểm thử nghiệp vụ và huấn luyện mô hình gợi ý thuốc.

Các bảng trong cơ sở dữ liệu được tổ chức theo nhóm nghiệp vụ gồm: quản lý tài khoản và nhân sự; quản lý bệnh nhân và tiền sử y tế; danh mục khoa, phòng, dịch vụ và bệnh lý; quản lý thuốc, nhà sản xuất, kho và tồn kho; quy trình đăng ký, tiếp nhận, khám bệnh, cận lâm sàng; kê đơn, phát thuốc; hóa đơn và thanh toán. Việc nạp dữ liệu mẫu được thực hiện theo thứ tự phụ thuộc khóa ngoại, bảo đảm dữ liệu danh mục tồn tại trước khi tạo dữ liệu phát sinh như phiếu đăng ký, phiếu khám, đơn thuốc, phiếu phát thuốc và hóa đơn.

### 4.2.2. Cấu hình kết nối cơ sở dữ liệu

Trong Web App, chuỗi kết nối được khai báo trong file `Web.config` với tên `dbcs` và sử dụng provider `System.Data.SqlClient`. Các lớp trong thư mục `DBContext` đọc chuỗi kết nối thông qua `ConfigurationManager.ConnectionStrings`, sau đó mở kết nối đến SQL Server để thực hiện truy vấn hoặc cập nhật dữ liệu. Cách cấu hình tập trung giúp hệ thống dễ thay đổi server hoặc database khi chuyển môi trường chạy mà không cần sửa từng lớp xử lý dữ liệu.

Trong AI Service, thông tin kết nối được khai báo bằng biến môi trường hoặc file `.env`, gồm các thông tin như server, database, chế độ xác thực và tài khoản kết nối. File `database.py` đọc cấu hình này, tạo connection string cho ODBC Driver SQL Server và trích xuất dữ liệu huấn luyện từ các bảng đơn thuốc, chi tiết đơn thuốc và chẩn đoán.

### 4.2.3. Cài đặt mã định danh và transaction

Các mã nghiệp vụ như phiếu đăng ký, phiếu khám, phiếu sàng lọc, chẩn đoán, phiếu chỉ định, chi tiết chỉ định, đơn thuốc, chi tiết đơn thuốc, phiếu phát thuốc, chi tiết phiếu phát và hóa đơn được sinh theo quy tắc mã định danh thông minh. Lớp `Utilities` cung cấp hàm sinh mã dựa trên tiền tố, ngày phát sinh và số thứ tự trong ngày. Cách sinh mã này giúp dữ liệu dễ đọc, dễ tra cứu và phù hợp với đặc thù quản lý chứng từ trong phòng khám.

Những nghiệp vụ ghi dữ liệu qua nhiều bảng như lưu khám bệnh, thanh toán, phát thuốc và cập nhật tồn kho được xử lý bằng transaction. Khi tất cả bước xử lý thành công, transaction được commit; nếu một bước thất bại, transaction được rollback để tránh phát sinh dữ liệu dở dang. Đặc biệt, nghiệp vụ phát thuốc có sử dụng cơ chế khóa dòng tồn kho để hạn chế tình huống nhiều người dùng cùng trừ số lượng trên một lô thuốc tại cùng thời điểm.

## 4.3. Cài đặt ứng dụng web ASP.NET MVC

### 4.3.1. Cấu trúc dự án Web App

Ứng dụng web được triển khai trong project `QL_KhamChuaBenhNgoaiTru` theo mô hình ASP.NET MVC. Thư mục `Controllers` chứa các controller public như `HomeController`, `TaiKhoanController`, `BenhNhanPortalController`, `MobileApiController`, `ThanhToanController` và `MockLISController`. Thư mục `Areas` chia hệ thống nội bộ thành các khu vực `Admin`, `Staff` và `NhanVienKho`, tương ứng với nhóm chức năng quản trị, nghiệp vụ khám chữa bệnh và nghiệp vụ kho dược.

Thư mục `Models` chứa các lớp dữ liệu và ViewModel; thư mục `DBContext` chứa các lớp thao tác trực tiếp với SQL Server; thư mục `Views` chứa giao diện Razor; thư mục `Helpers` chứa các lớp hỗ trợ như gửi email, tạo hóa đơn PDF, xử lý route và sinh mã. Cách tổ chức này giúp tách rõ trách nhiệm giữa tầng điều khiển, tầng dữ liệu, tầng giao diện và các tiện ích dùng chung.

### 4.3.2. Cài đặt tầng Controller

Controller là tầng tiếp nhận yêu cầu HTTP từ trình duyệt hoặc ứng dụng di động. Với giao diện web, controller trả về Razor View hoặc Partial View; với thao tác AJAX và mobile, controller trả về `JsonResult`. Ví dụ, `TiepTanController` xử lý xác nhận dịch vụ khám, lấy danh sách phòng và chốt cấp số; `BacSiController` xử lý tiếp nhận bệnh nhân, lưu kết quả khám, chỉ định cận lâm sàng, kê đơn và gọi gợi ý thuốc; `ThuNganController` xử lý chi tiết hóa đơn, thanh toán, tạo QR và gửi hóa đơn; `PhatThuocController` xử lý xem đơn thuốc và xác nhận phát thuốc.

Các controller nền như `BaseAdminController`, `BaseStaffController` và `BaseNhanVienKhoController` được dùng để kiểm tra phiên đăng nhập và quyền truy cập trước khi cho phép người dùng vào từng khu vực chức năng. Việc gom logic kiểm tra quyền tại controller nền giúp giảm lặp mã và hạn chế việc người dùng truy cập sai phạm vi nghiệp vụ.

### 4.3.3. Cài đặt tầng dữ liệu bằng ADO.NET

Tầng dữ liệu được cài đặt bằng các lớp DBContext chuyên trách cho từng nhóm nghiệp vụ như `BenhNhanDB`, `NhanVienDB`, `KhoaDB`, `PhongDB`, `ThuocDB`, `TiepTanDB`, `BacSiDB`, `CLSDB`, `ThuNganDB`, `PhatThuocDB`, `KhoDB`, `KhoNhapDB`, `DashboardDB` và `BackupDB`. Các lớp này chịu trách nhiệm mở kết nối, tạo command, gán tham số, đọc dữ liệu và chuyển đổi kết quả sang Model hoặc ViewModel cho tầng controller sử dụng.

Hệ thống sử dụng tham số SQL trong các truy vấn để giảm rủi ro lỗi nhập liệu và tránh ghép chuỗi SQL tùy tiện. Với các màn hình danh sách, tầng dữ liệu hỗ trợ tìm kiếm, lọc, sắp xếp và phân trang. Với các nghiệp vụ cần ghi đồng thời nhiều bảng, DBContext sử dụng `SqlTransaction` để bảo đảm dữ liệu luôn nhất quán giữa phiếu khám, đơn thuốc, chỉ định cận lâm sàng, hóa đơn, phiếu phát thuốc và tồn kho.

### 4.3.4. Cài đặt giao diện Razor, Bootstrap và jQuery

Giao diện web được xây dựng bằng Razor View kết hợp Bootstrap, CSS riêng và jQuery. Các layout chính gồm `_Layout_Client`, `_Layout_Admin`, `_Layout_Staff`, `_Layout_NhanVienKho` và `_AuthLayout`. Việc tách layout theo nhóm người dùng giúp mỗi khu vực có menu, bố cục và luồng thao tác phù hợp với vai trò sử dụng.

Một số màn hình sử dụng partial view để tách các bảng dữ liệu hoặc thành phần có thể tải lại bằng AJAX, ví dụ danh sách bệnh nhân, danh sách thuốc, danh sách phòng, bảng dịch vụ, danh sách bác sĩ và các khối dashboard. Cách cài đặt này giúp giao diện phản hồi nhanh hơn khi người dùng lọc, tìm kiếm hoặc phân trang mà không cần tải lại toàn bộ trang.

### 4.3.5. Cài đặt cập nhật thời gian thực bằng SignalR

SignalR được tích hợp thông qua OWIN Startup và `ClinicHub`. Khi ứng dụng khởi động, `app.MapSignalR()` đăng ký endpoint SignalR cho Web App. Các màn hình theo dõi hàng đợi, tiếp nhận, bác sĩ và kiosk có thể nhận thông báo khi trạng thái phiếu khám thay đổi. Nhờ đó, người dùng ở nhiều vai trò khác nhau có thể nhìn thấy dữ liệu mới gần như tức thời mà không phải thao tác tải lại trang.

## 4.4. Cài đặt các module nghiệp vụ chính

### 4.4.1. Module tài khoản, đăng nhập và phân quyền

Module tài khoản được cài đặt bằng `TaiKhoanController` và `TaiKhoanDB`. Người dùng nhập tên đăng nhập và mật khẩu từ form đăng nhập, controller kiểm tra dữ liệu đầu vào, gọi tầng dữ liệu để xác thực, sau đó lưu thông tin phiên làm việc vào `Session`. Từ tài khoản đăng nhập, hệ thống xác định người dùng là nhân viên hay bệnh nhân, tiếp tục lấy thông tin chi tiết tương ứng và điều hướng đến khu vực phù hợp.

Với nhân viên, hệ thống lưu các thông tin như mã nhân viên, họ tên và mã chức vụ để các controller nền kiểm tra quyền truy cập. Với bệnh nhân, hệ thống lưu mã bệnh nhân và họ tên để truy cập portal bệnh nhân. Quy trình đăng ký tài khoản bệnh nhân có thêm bước xác thực OTP qua email. Dữ liệu đăng ký được lưu tạm trong `Session`, sau đó hệ thống gửi mã OTP; chỉ khi người dùng nhập đúng OTP trong thời gian hiệu lực, tài khoản và hồ sơ bệnh nhân mới được tạo trong database.

### 4.4.2. Module quản trị danh mục và dữ liệu nền

Module quản trị được đặt trong Area `Admin`, gồm các chức năng quản lý bệnh nhân, nhân viên, thuốc, loại thuốc, dịch vụ, khoa, phòng, kho và hóa đơn. Các màn hình danh sách hỗ trợ tìm kiếm, lọc và phân trang; các thao tác thêm, sửa, khóa/mở khóa hoặc đổi trạng thái được tách thành các action riêng để dễ kiểm soát.

Khi thêm hoặc sửa nhân viên, controller sử dụng ViewModel để gom thông tin tài khoản, thông tin cá nhân, chức vụ, khoa, phòng, trạng thái và ảnh đại diện. Khi quản lý thuốc, hệ thống cho phép lọc theo từ khóa, loại thuốc, đường dùng, bảo hiểm y tế và trạng thái. Dữ liệu danh mục thuốc là dữ liệu nền quan trọng vì được sử dụng đồng thời trong kê đơn, thanh toán, quản lý kho, phát thuốc và mô-đun gợi ý thuốc.

### 4.4.3. Module tiếp nhận và cấp số khám

Module tiếp nhận thuộc Area `Staff` và được sử dụng bởi lễ tân. Dữ liệu đầu vào có thể đến từ bệnh nhân đặt lịch online hoặc bệnh nhân đăng ký trực tiếp tại quầy. Lễ tân xác nhận dịch vụ, khoa/phòng khám, lý do đến khám, sau đó hệ thống tạo hoặc cập nhật phiếu khám bệnh, gán số thứ tự và chuyển bệnh nhân vào hàng đợi khám.

Các action như xác nhận dịch vụ và chốt cấp số trả JSON để giao diện cập nhật nhanh mà không cần tải lại toàn bộ trang. Tầng dữ liệu kiểm tra phiếu đăng ký, lấy thông tin bệnh nhân, sinh mã phiếu khám, tính số thứ tự theo phòng và ngày khám, đồng thời cập nhật trạng thái phiếu. Quy trình này bảo đảm bệnh nhân chỉ được đưa vào hàng đợi khi đã có thông tin đăng ký hợp lệ và đã được xác nhận đúng phòng khám.

### 4.4.4. Module bác sĩ khám bệnh và kê đơn

Module bác sĩ hiển thị danh sách bệnh nhân chờ khám, cho phép bác sĩ tiếp nhận bệnh nhân, xem thông tin sàng lọc, nhập triệu chứng, kết luận, chẩn đoán, chỉ định cận lâm sàng và kê đơn thuốc. Dữ liệu của một lượt khám được gom vào `KhamBenhViewModel` để controller chuyển xuống tầng dữ liệu xử lý đồng bộ.

Khi lưu khám, `BacSiDB` mở transaction để cập nhật phiếu khám, tạo chẩn đoán, tạo phiếu chỉ định nếu có cận lâm sàng, tạo đơn thuốc và chi tiết đơn thuốc nếu có kê thuốc, đồng thời phát sinh dữ liệu hóa đơn tương ứng. Nếu một bước lỗi, transaction được rollback để tránh tình trạng dữ liệu bị lệch giữa phiếu khám, đơn thuốc, chỉ định và hóa đơn.

Mô-đun gợi ý thuốc được tích hợp vào màn hình kê đơn. Controller gửi mã bệnh hoặc mã thuốc hiện có sang AI Service thông qua endpoint `/api/recommend`, nhận danh sách thuốc gợi ý kèm điểm tương đồng và hiển thị cho bác sĩ tham khảo. Kết quả gợi ý không tự động thay thế quyết định chuyên môn; bác sĩ vẫn là người kiểm tra chỉ định, chống chỉ định, liều dùng và tình trạng cụ thể của bệnh nhân trước khi đưa thuốc vào đơn.

### 4.4.5. Module cận lâm sàng

Module cận lâm sàng nhận các dịch vụ đã được bác sĩ chỉ định. Nhân viên CLS xem danh sách chỉ định chờ thực hiện, mở chi tiết, nhập kết quả, ghi nhận thông tin mẫu xét nghiệm, chất lượng mẫu, file kết quả nếu có và xác nhận hoàn tất. Khi toàn bộ chỉ định của một phiếu đã có kết quả, hệ thống cập nhật trạng thái phiếu chỉ định và phiếu khám để bác sĩ tiếp tục xử lý.

`CLSController` lấy danh sách chờ thực hiện, trả chi tiết kết quả bằng JSON, nhận nội dung kết quả từ giao diện và gọi `CLSDB` để cập nhật dữ liệu. Ngoài ra, `MockLISController` được xây dựng để mô phỏng việc hệ thống xét nghiệm bên ngoài gửi kết quả về hệ thống. Thành phần này giúp kiểm thử luồng tích hợp cận lâm sàng trong môi trường demo mà chưa cần kết nối thiết bị hoặc hệ thống LIS thật.

### 4.4.6. Module thu ngân, thanh toán và hóa đơn

Module thu ngân hiển thị danh sách hóa đơn chưa thanh toán hoặc thanh toán một phần. Thu ngân mở chi tiết hóa đơn, kiểm tra các khoản dịch vụ và thuốc, cập nhật hoặc hủy các dòng cần xử lý, chọn phương thức thanh toán và xác nhận thanh toán. Hệ thống hỗ trợ tính phần bảo hiểm y tế chi trả, phần bệnh nhân trả, tạo mã QR thanh toán và gửi hóa đơn cho bệnh nhân.

Tầng dữ liệu sử dụng transaction để cập nhật chi tiết hóa đơn dịch vụ, chi tiết hóa đơn thuốc, tính lại tổng tiền gốc, tổng bảo hiểm chi trả và tổng bệnh nhân cần thanh toán. Sau khi thanh toán thành công, `InvoicePdfHelper` tạo file PDF hóa đơn và `InvoiceEmailService` gửi hóa đơn qua email nếu bệnh nhân có thông tin email hợp lệ. Việc tách logic tạo PDF và gửi email sang lớp helper giúp controller gọn hơn và dễ tái sử dụng.

### 4.4.7. Module phát thuốc và quản lý tồn kho

Module phát thuốc chỉ xử lý các đơn thuốc đã thanh toán. Dược sĩ xem chi tiết đơn, kiểm tra tồn kho dự kiến và xác nhận phát thuốc. Khi phát thuốc, hệ thống tạo phiếu phát, ghi chi tiết từng lô thuốc đã phát, cập nhật số lượng đã phát trong chi tiết đơn thuốc và trừ tồn kho theo lô còn hạn sử dụng.

Trong `PhatThuocDB`, nghiệp vụ phát thuốc được xử lý bằng transaction. Khi chọn tồn kho, hệ thống sử dụng khóa dòng ở bảng tồn kho để hạn chế việc hai người dùng cùng trừ một lô thuốc tại cùng thời điểm. Bên cạnh đó, Area `NhanVienKho` cung cấp các chức năng quản lý nhà cung cấp, tạo phiếu nhập, xem chi tiết phiếu nhập, quản lý thuốc và tồn kho. Dữ liệu nhập kho là nguồn dữ liệu đầu vào để module phát thuốc kiểm tra số lượng có thể cấp phát.

### 4.4.8. Module bệnh nhân trên web portal

Web portal bệnh nhân cho phép bệnh nhân cập nhật thông tin cá nhân, đặt lịch khám, thanh toán online, xem lịch khám, hủy lịch, xem lịch sử khám, đơn thuốc và hóa đơn. Các action trong `BenhNhanPortalController` luôn lấy mã bệnh nhân từ `Session`, từ đó bảo đảm bệnh nhân chỉ thao tác trên dữ liệu của chính mình.

Khi đặt lịch, hệ thống chuẩn bị danh sách dịch vụ và khung giờ khả dụng cho giao diện. Sau khi bệnh nhân chọn ngày khám, khung giờ, dịch vụ và nhập lý do khám, controller gọi `BenhNhanPortalDB` để tạo phiếu đăng ký. Dữ liệu đặt lịch được ghi vào bảng phiếu đăng ký và chờ lễ tân tiếp nhận. Các chức năng xem đơn thuốc, chi tiết đơn thuốc, hóa đơn và chi tiết hóa đơn đều được lọc theo mã bệnh nhân hiện tại.

### 4.4.9. Module Mobile API cho ứng dụng Flutter

`MobileApiController` là lớp API trung gian giữa Flutter App và Web App. Các endpoint trả về JSON thay vì Razor View. Ứng dụng Flutter gọi API để đăng nhập, đăng ký, cập nhật hồ sơ, lấy dữ liệu trang chủ, bác sĩ, chuyên khoa, dịch vụ, lịch khám, lịch sử khám, đơn thuốc, hóa đơn, đặt lịch và hủy lịch.

Các API đăng nhập và đăng ký trả về cấu trúc gồm trạng thái thành công, thông báo và thông tin người dùng. Các API hồ sơ, lịch khám, đơn thuốc và hóa đơn đóng gói dữ liệu bệnh nhân thành JSON để Flutter chuyển thành model và hiển thị bằng widget. Cách cài đặt này tách ứng dụng mobile khỏi cấu trúc cơ sở dữ liệu nội bộ, đồng thời giúp Web App đóng vai trò cổng nghiệp vụ trung tâm.

### 4.4.10. Module dashboard, thống kê và sao lưu

Module dashboard trong Area `Admin` tổng hợp dữ liệu vận hành như doanh thu, số lượng bệnh nhân, tình trạng kho dược và cảnh báo tồn kho. `DashboardController` chia thống kê thành nhiều action như tổng quan, doanh thu, bệnh nhân và kho dược; mỗi action gọi `DashboardDB` để lấy dữ liệu rồi trả về partial view hoặc JSON cho biểu đồ.

Module sao lưu hỗ trợ tạo bản backup, phục hồi database và cấu hình lịch sao lưu tự động. `BackupDB` thực thi các lệnh backup/restore trên SQL Server; `BackupScheduler` đọc cấu hình lịch sao lưu, kiểm tra thời điểm cần chạy và gọi `BackupDB` để tạo bản sao lưu phù hợp. Đây là phần quan trọng đối với hệ thống y tế vì dữ liệu hồ sơ khám chữa bệnh cần được bảo vệ và có khả năng khôi phục khi xảy ra sự cố.

## 4.5. Cài đặt mô-đun AI gợi ý thuốc

### 4.5.1. Cấu trúc AI Service

AI Service được đặt trong thư mục `AI_Recommender` hoặc `AI_Recommender_Quoc`, gồm các file chính như `requirements.txt`, `database.py`, `custom_node2vec.py`, `main.py` và thư mục `models`. File `requirements.txt` khai báo các thư viện cần cài đặt; `database.py` đảm nhiệm kết nối SQL Server và trích xuất dữ liệu kê đơn; `custom_node2vec.py` chứa thuật toán Node2Vec tự cài đặt; `main.py` khởi tạo FastAPI và định nghĩa các endpoint huấn luyện, gợi ý.

Dịch vụ được khởi chạy bằng lệnh `uvicorn main:app --reload`. Khi chạy, FastAPI cung cấp Swagger UI tại đường dẫn `/docs` để kiểm thử các API. Việc triển khai mô-đun AI dưới dạng microservice giúp thành phần gợi ý thuốc độc lập với Web App, thuận tiện thay đổi thuật toán, huấn luyện lại mô hình hoặc triển khai trên môi trường riêng.

### 4.5.2. Trích xuất dữ liệu huấn luyện

Hàm trích xuất dữ liệu trong `database.py` truy vấn các bảng liên quan đến đơn thuốc, chi tiết đơn thuốc và chẩn đoán để gom dữ liệu theo từng lượt khám. Mỗi mẫu huấn luyện là một chuỗi gồm mã bệnh và danh sách mã thuốc xuất hiện trong cùng đơn hoặc cùng lượt khám. Nếu một phiếu có nhiều bệnh hoặc nhiều thuốc, dữ liệu được gom và loại trùng trước khi đưa vào mô hình.

Cách đưa mã bệnh vào chuỗi học giúp mô hình học được cả quan hệ bệnh - thuốc và thuốc - thuốc. Trong trường hợp dữ liệu bệnh chưa đầy đủ, mô hình vẫn có thể học quan hệ đồng xuất hiện giữa các thuốc trong đơn. Đây là hướng tiếp cận phù hợp với phạm vi khóa luận vì tận dụng được dữ liệu kê đơn có sẵn mà không yêu cầu mô tả dược lý phức tạp cho từng thuốc.

### 4.5.3. Huấn luyện và lưu mô hình Node2Vec

Lớp `CustomNode2Vec` xây dựng đồ thị từ danh sách toa thuốc. Mỗi node là mã thuốc hoặc mã bệnh; cạnh giữa hai node được tăng trọng số khi chúng cùng xuất hiện trong một mẫu huấn luyện. Sau khi có đồ thị, mô hình sinh các random walk theo trọng số cạnh, tạo các cặp target-context và huấn luyện Skip-Gram với Negative Sampling.

Các vector nhúng được khởi tạo ngẫu nhiên và cập nhật qua nhiều epoch bằng gradient descent. Sau khi huấn luyện, mô hình lưu trọng số vào file `models/model_weights.json`. Khi service khởi động lại, mô hình có thể đọc lại file trọng số để phục vụ gợi ý mà không cần huấn luyện lại từ đầu. Cách lưu mô hình này phù hợp với giai đoạn demo, đồng thời giúp quá trình kiểm thử API nhanh hơn.

### 4.5.4. Gợi ý thuốc qua API

Endpoint `/api/train` dùng để huấn luyện lại mô hình từ dữ liệu hiện tại trong database. Endpoint `/api/recommend` nhận `node_id` và `top_k`, sau đó tính độ tương đồng cosine giữa vector đầu vào và các vector còn lại. Kết quả trả về gồm mã thuốc hoặc node gợi ý và điểm tương đồng, được sắp xếp theo thứ tự giảm dần.

Trong Web App, chức năng gợi ý thuốc được gọi tại màn hình bác sĩ kê đơn. Hệ thống gửi mã bệnh hoặc mã thuốc hiện có sang AI Service, nhận danh sách gợi ý, ánh xạ với danh mục thuốc trong cơ sở dữ liệu và hiển thị cho bác sĩ. Mô-đun này đóng vai trò hỗ trợ tham khảo, không thay thế quyết định chuyên môn của bác sĩ và chưa được sử dụng như một công cụ điều trị chính thức.

## 4.6. Cài đặt ứng dụng di động Flutter

### 4.6.1. Cấu trúc ứng dụng mobile

Ứng dụng di động được đặt trong thư mục `app_kcb`. File `pubspec.yaml` khai báo Flutter SDK, phiên bản ứng dụng, asset hình ảnh và các package như `http`, `shared_preferences`, `flutter_secure_storage`, `table_calendar`, `google_fonts`, `image_picker`, `cached_network_image`, `qr_flutter` và `url_launcher`. Thư mục `lib` được chia thành `core` và `features` để tách lớp dùng chung với các màn hình chức năng.

Tầng `core` gồm các service như `api_service`, `auth_service`, `booking_service`, `discovery_service`, `medical_service` và `profile_service`. Tầng `features` gồm các màn hình đăng nhập, đăng ký, xác thực OTP, trang chủ, đặt lịch, bác sĩ, chuyên khoa, hướng dẫn, lịch sử khám, đơn thuốc, hóa đơn, hồ sơ cá nhân và cài đặt. Cách chia này giúp UI không gọi API trực tiếp mà thông qua service, thuận tiện khi thay đổi endpoint hoặc xử lý lỗi tập trung.

### 4.6.2. Cài đặt xác thực và lưu phiên

Ứng dụng mobile gọi `MobileApiController` để đăng nhập, đăng ký và lấy thông tin người dùng hiện tại. Sau khi đăng nhập thành công, thông tin phiên và cookie được lưu bằng `flutter_secure_storage`; một số dữ liệu cấu hình hoặc cơ chế tương thích cũ có thể dùng `SharedPreferences`. Việc lưu thông tin nhạy cảm bằng secure storage giúp tăng mức độ an toàn so với lưu toàn bộ dữ liệu phiên bằng bộ nhớ thông thường.

Khi người dùng đăng xuất, ứng dụng xóa dữ liệu phiên và chuyển về màn hình đăng nhập. `ApiService` chịu trách nhiệm gắn cookie vào request, xử lý phản hồi từ server và chuẩn hóa lỗi để các màn hình chức năng có thể hiển thị thông báo phù hợp.

### 4.6.3. Cài đặt đặt lịch và tra cứu hồ sơ

Màn hình đặt lịch sử dụng `TableCalendar` để chọn ngày khám, sau đó gọi API lấy danh sách khung giờ khả dụng. Khi bệnh nhân chọn dịch vụ, khung giờ và nhập lý do khám, ứng dụng gọi API đặt lịch để tạo phiếu đăng ký. Các màn hình lịch khám, lịch sử khám, đơn thuốc và hóa đơn gọi lần lượt các endpoint tương ứng của Web App để lấy dữ liệu.

Dữ liệu JSON trả về được chuyển thành model trong Flutter, sau đó hiển thị bằng các widget Material. Ứng dụng ưu tiên giao diện rõ ràng, thao tác ngắn và trạng thái dễ nhận biết để bệnh nhân có thể theo dõi quy trình sau khi đặt lịch. Bên cạnh đó, ứng dụng không truy cập trực tiếp SQL Server mà luôn thông qua Mobile API, giúp bảo vệ cơ sở dữ liệu nội bộ và giảm phụ thuộc giữa client với tầng dữ liệu.

## 4.7. Hướng dẫn chạy hệ thống

### 4.7.1. Chạy cơ sở dữ liệu

Bước đầu tiên là cài đặt SQL Server, tạo database `QL_KhamBenhNgoaiTru` bằng file `Create_QLKCB.sql`, sau đó chạy các file dữ liệu mẫu trong thư mục `Database`. Sau khi nạp dữ liệu, cần kiểm tra chuỗi kết nối `dbcs` trong `Web.config` và cấu hình kết nối trong AI Service để bảo đảm Web App và AI Service cùng sử dụng đúng database.

### 4.7.2. Chạy Web App

Web App được mở bằng file solution `QL_KhamChuaBenhNgoaiTru.sln` trong Visual Studio. Sau khi restore NuGet package và build thành công, chạy project bằng IIS Express hoặc IIS local. Khi ứng dụng khởi động, các route MVC, SignalR hub, controller, view và DBContext sẵn sàng phục vụ các vai trò người dùng.

### 4.7.3. Chạy AI Service

AI Service được chạy bằng cách mở terminal tại thư mục chứa service, cài thư viện bằng lệnh `pip install -r requirements.txt`, cấu hình database trong `.env` hoặc biến môi trường, sau đó chạy `uvicorn main:app --reload`. Sau khi service hoạt động, có thể truy cập `/docs` để kiểm thử endpoint `/api/train` và `/api/recommend`.

### 4.7.4. Chạy ứng dụng mobile

Ứng dụng Flutter được chạy bằng cách mở terminal tại thư mục `app_kcb`, thực hiện `flutter pub get` để tải package, sau đó chạy `flutter run` trên thiết bị ảo hoặc thiết bị thật. Trước khi chạy, cần kiểm tra cấu hình base URL trong `ApiService` hoặc truyền qua `dart-define` để ứng dụng mobile gọi đúng địa chỉ Web App đang hoạt động.

## 4.8. Kết luận chương

Chương 4 đã trình bày quá trình cài đặt hệ thống quản lý khám chữa bệnh ngoại trú tích hợp mô-đun gợi ý thuốc. Nội dung chương bao gồm môi trường phát triển, cách khởi tạo và cấu hình cơ sở dữ liệu, cài đặt ứng dụng web ASP.NET MVC, triển khai các module nghiệp vụ chính, tích hợp cập nhật thời gian thực bằng SignalR, xây dựng AI Service bằng FastAPI và Node2Vec, cũng như cài đặt ứng dụng di động Flutter cho bệnh nhân.

Kết quả cài đặt cho thấy hệ thống đã hiện thực được kiến trúc gồm Web App, AI Service và Mobile App. Web App đóng vai trò trung tâm xử lý nghiệp vụ phòng khám; AI Service cung cấp khả năng gợi ý thuốc dựa trên dữ liệu kê đơn; Mobile App mở rộng kênh tương tác cho bệnh nhân. Đây là nền tảng để tiếp tục kiểm thử, đánh giá và triển khai demo hệ thống trong Chương 5.
