# MedicHub Flutter App

Flutter client cho hệ thống khám chữa bệnh ngoại trú.

## Chạy ở môi trường dev

Backend mặc định:

- Android Emulator: `http://10.0.2.2:8080`
- Desktop/iOS Simulator: `http://localhost:8080`
- Header dev IIS Express mặc định: `Host: localhost`

Chạy app:

```powershell
flutter pub get
flutter run
```

## Cấu hình API theo môi trường

Không sửa trực tiếp `ApiService.baseUrl` khi đổi môi trường. Dùng `dart-define`:

```powershell
flutter run --dart-define=API_BASE_URL=https://your-api.example.com --dart-define=API_HOST_HEADER=
```

Với IIS Express local trên Android Emulator, có thể giữ mặc định hoặc truyền rõ:

```powershell
flutter run --dart-define=API_BASE_URL=http://10.0.2.2:8080 --dart-define=API_HOST_HEADER=localhost
```

## Ghi chú bảo mật

- Cookie phiên và thông tin người dùng hiện được lưu bằng `flutter_secure_storage`.
- QR thanh toán được render nội bộ bằng `qr_flutter`, không gửi nội dung QR sang dịch vụ tạo ảnh bên ngoài.
- Khi build production, nên dùng HTTPS và tắt `API_HOST_HEADER`.

## Kiểm tra

```powershell
flutter analyze
flutter test
```
