# Web ASP.NET MVC

Ứng dụng Web chính của hệ thống quản lý khám chữa bệnh ngoại trú. Project cung cấp cổng bệnh nhân, khu vực nhân viên, khu vực quản trị, API mobile cho Flutter app, tích hợp thanh toán, SignalR realtime và quản lý dịch vụ AI gợi ý thuốc.

## Công nghệ

- ASP.NET MVC 5 trên .NET Framework 4.8.
- SQL Server.
- SignalR 2.4.3 cho realtime.
- OWIN startup.
- MSTest ở project `../QL_KhamChuaBenh.Tests`.
- NuGet `packages.config`.

## Cấu trúc chính

- `Controllers/`: controller public, tài khoản, thanh toán, API mobile và cổng bệnh nhân.
- `Areas/Admin/`: module quản trị bệnh nhân, nhân viên, khoa/phòng, thuốc, kho, hóa đơn, dashboard, backup và AI.
- `Areas/Staff/`: module nhân viên tiếp tân, bác sĩ, thu ngân, kho, CLS, phát thuốc và kiosk.
- `DBContext/`: lớp truy cập dữ liệu SQL Server.
- `Models/`: model và view model.
- `Services/`: scheduler backup và scheduler train AI.
- `Helpers/`: tiện ích bảo mật, JWT, email, PDF hóa đơn và route.
- `Hubs/`: SignalR hub.
- `Views/`, `Content/`, `Scripts/`, `Styles/`, `Images/`: giao diện và static assets.
- `App_Data/ai_management_config.json`: cấu hình quản lý AI.
- `backup/`: file cấu hình và bản sao lưu database.

## Thiết lập

1. Mở solution `../QL_KhamChuaBenhNgoaiTru.sln` bằng Visual Studio.
2. Restore NuGet packages.
3. Tạo database bằng script trong `../Database`.
4. Cập nhật connection string trong `Web.config`.
5. Chọn `QL_KhamChuaBenhNgoaiTru` làm startup project.
6. Chạy bằng IIS Express hoặc IIS local.

## Tích hợp Flutter

Flutter app gọi các API trong `MobileApiController`. Khi chạy local bằng Android Emulator, app thường dùng:

```text
http://10.0.2.2:8080
```

Nếu backend chạy bằng IIS Express và cần ép host, cấu hình `API_HOST_HEADER=localhost` ở Flutter app.

## Tích hợp AI

Module `Areas/Admin/Controllers/AiManagementController.cs` và `Services/AiTrainingScheduler.cs` dùng để quản lý dịch vụ AI. Trước khi dùng chức năng này, chạy FastAPI trong thư mục `../AI`:

```powershell
cd ..\AI
uvicorn main:app --reload
```

## Kiểm thử

Project test nằm ở `../QL_KhamChuaBenh.Tests` và dùng .NET Framework 4.8/MSTest. Có thể chạy bằng Test Explorer trong Visual Studio sau khi restore packages và cấu hình database test trong `App.config`.

## File local đã loại bỏ

Các file sinh ra bởi Visual Studio như `.vs/`, `bin/`, `obj/` và `.csproj.user` không cần commit. NuGet packages có thể restore lại từ `packages.config`.
