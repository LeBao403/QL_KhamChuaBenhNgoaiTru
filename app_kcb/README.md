# MedicHub Flutter App

Ứng dụng Flutter cho bệnh nhân trong hệ thống khám chữa bệnh ngoại trú. App kết nối với Web ASP.NET qua các API mobile để đăng nhập, đăng ký, đặt lịch khám, xem lịch sử khám, đơn thuốc, hóa đơn và thông tin cá nhân.

## Cấu trúc chính

- `lib/main.dart`: điểm khởi động ứng dụng.
- `lib/core/services/`: lớp gọi API, đăng nhập, hồ sơ, đặt lịch, lịch sử khám.
- `lib/core/models/`: model dữ liệu dùng trong app.
- `lib/core/theme/`: cấu hình giao diện.
- `lib/features/`: các màn hình chức năng như auth, booking, doctors, history, medical, profile, settings.
- `assets/images/`: logo và hình ảnh dùng trong ứng dụng.

## Yêu cầu

- Flutter SDK 3.5 trở lên.
- Web backend `QL_KhamChuaBenhNgoaiTru` đang chạy.

## Chạy ở môi trường dev

Backend mặc định:

- Android Emulator: `http://10.0.2.2:8080`
- Desktop/iOS Simulator: `http://localhost:8080`
- Header dev IIS Express mặc định: `Host: localhost`

```powershell
cd app_kcb
flutter pub get
flutter run
```

## Cấu hình API

Không sửa trực tiếp `ApiService.baseUrl` khi đổi môi trường. Dùng `dart-define`:

```powershell
flutter run --dart-define=API_BASE_URL=https://your-api.example.com --dart-define=API_HOST_HEADER=
```

Với IIS Express local trên Android Emulator:

```powershell
flutter run --dart-define=API_BASE_URL=http://10.0.2.2:8080 --dart-define=API_HOST_HEADER=localhost
```

## Kiểm tra

```powershell
flutter analyze
flutter test
```

## Ghi chú bảo mật

- Cookie phiên và thông tin người dùng được lưu bằng `flutter_secure_storage`.
- QR thanh toán được render nội bộ bằng `qr_flutter`.
- Khi build production, nên dùng HTTPS và truyền `--dart-define=API_HOST_HEADER=` để tắt header dev.
