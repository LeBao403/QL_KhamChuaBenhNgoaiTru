# QL_KhamChuaBenhNgoaiTru

Hệ thống quản lý khám chữa bệnh ngoại trú gồm 3 phần chính:

- `QL_KhamChuaBenhNgoaiTru/`: ứng dụng Web ASP.NET MVC cho quản trị, nhân viên phòng khám và cổng bệnh nhân.
- `app_kcb/`: ứng dụng Flutter cho bệnh nhân.
- `AI/`: dịch vụ FastAPI gợi ý thuốc bằng Node2Vec tự cài đặt.

Ngoài ra repo có:

- `QL_KhamChuaBenh.Tests/`: bộ kiểm thử MSTest cho các lớp truy cập dữ liệu.
- `Database/`: script tạo CSDL và dữ liệu mẫu SQL Server.

## Công nghệ sử dụng

- ASP.NET MVC 5, .NET Framework 4.8, SignalR, OWIN.
- SQL Server.
- Flutter/Dart cho ứng dụng mobile/desktop.
- Python FastAPI, NumPy, pandas, pyodbc cho dịch vụ AI.

## Cấu trúc nhanh

```text
.
├── AI/                         # API AI gợi ý thuốc
├── app_kcb/                    # Flutter client
├── Database/                   # Script SQL Server
├── QL_KhamChuaBenh.Tests/      # Unit/integration tests
├── QL_KhamChuaBenhNgoaiTru/    # Web ASP.NET MVC
└── QL_KhamChuaBenhNgoaiTru.sln
```

## Thiết lập cơ bản

1. Tạo database SQL Server bằng các script trong `Database/`.
2. Mở `QL_KhamChuaBenhNgoaiTru.sln` bằng Visual Studio.
3. Restore NuGet packages cho solution.
4. Cập nhật connection string trong `QL_KhamChuaBenhNgoaiTru/Web.config` và `QL_KhamChuaBenh.Tests/App.config` theo máy chạy.
5. Chạy Web project bằng IIS Express hoặc IIS local.
6. Chạy dịch vụ AI trong thư mục `AI/` nếu cần tính năng gợi ý thuốc.
7. Chạy Flutter app trong thư mục `app_kcb/` và trỏ API về Web backend.

## Chạy từng project

Xem hướng dẫn chi tiết tại:

- [AI/README.md](AI/README.md)
- [app_kcb/README.md](app_kcb/README.md)
- [QL_KhamChuaBenhNgoaiTru/README.md](QL_KhamChuaBenhNgoaiTru/README.md)

## Kiểm thử

- Web/tests: chạy bằng Test Explorer trong Visual Studio hoặc `vstest.console` sau khi restore package.
- Flutter: chạy `flutter analyze` và `flutter test` trong `app_kcb/`.
- AI: khởi động FastAPI rồi kiểm tra các endpoint `/api/status`, `/api/train`, `/api/recommend`.

## File không nên commit

Các file/thư mục sinh ra bởi IDE, build hoặc restore package đã được loại khỏi repo làm việc, ví dụ `.vs/`, `packages/`, `bin/`, `obj/`, Android `build/` và file `.csproj.user`.
