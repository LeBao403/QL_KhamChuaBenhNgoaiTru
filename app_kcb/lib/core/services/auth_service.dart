import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/user_model.dart';
import 'api_service.dart';

class AuthService {
  static final AuthService _instance = AuthService._internal();
  factory AuthService() => _instance;
  AuthService._internal();

  final ApiService _api = ApiService();

  static const String _userKey = 'current_user';
  static const _secureStorage = FlutterSecureStorage();
  UserModel? _currentUser;

  UserModel? get currentUser => _currentUser;
  bool get isLoggedIn => _currentUser != null;

  Future<void> init() async {
    await _api.loadCookies();
    await _loadUserFromPrefs();
  }

  Future<void> _loadUserFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    var userJson = await _secureStorage.read(key: _userKey);
    final legacyUserJson = prefs.getString(_userKey);
    if ((userJson == null || userJson.isEmpty) && legacyUserJson != null) {
      userJson = legacyUserJson;
      await _secureStorage.write(key: _userKey, value: legacyUserJson);
      await prefs.remove(_userKey);
    }
    if (userJson != null) {
      try {
        _currentUser = UserModel.fromJson(jsonDecode(userJson));
      } catch (_) {
        _currentUser = null;
      }
    }
  }

  Future<void> _saveUserToPrefs(UserModel user) async {
    await _secureStorage.write(key: _userKey, value: jsonEncode(user.toJson()));
  }

  Future<void> _clearUserFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_userKey);
    await _secureStorage.delete(key: _userKey);
  }

  Future<ApiResult<UserModel>> login(String username, String password) async {
    try {
      final response = await _api.post('/MobileApi/Login', {
        'username': username,
        'password': password,
      });

      if (response.statusCode != 200) {
        return ApiResult.fail(
          'Đăng nhập thất bại. Mã phản hồi: ${response.statusCode}',
        );
      }

      final json = _api.parseJson(response);
      if (json == null) {
        return ApiResult.fail(
          'Máy chủ trả về dữ liệu không hợp lệ khi đăng nhập.',
        );
      }

      if (json['success'] == true) {
        final user = UserModel.fromJson(json['user']);
        _currentUser = user;
        await _saveUserToPrefs(user);

        // --- ĐOẠN CODE THÊM MỚI: LẤY VÀ LƯU JWT TOKEN ---
        final token = json['token'];
        if (token != null && token.toString().isNotEmpty) {
          // Lưu token vào Secure Storage với key là 'jwt_token' (khớp với api_service)
          await _secureStorage.write(key: 'jwt_token', value: token.toString());
          
          // In ra Terminal để kiểm tra
          print('==================================================');
          print('ĐĂNG NHẬP THÀNH CÔNG!');
          print('JWT TOKEN NHẬN ĐƯỢC: $token');
          print('==================================================');
        } else {
          print('LỖI: Server báo success nhưng KHÔNG CÓ TOKEN trả về!');
        }
        // --------------------------------------------------

        return ApiResult.ok(user);
      }

      return ApiResult.fail(
        json['message']?.toString() ?? 'Đăng nhập thất bại!',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối máy chủ: ${e.toString()}');
    }
  }

  Future<ApiResult<void>> register(
    String username,
    String email,
    String password,
    String confirmPassword,
    String hoTen,
  ) async {
    if (password != confirmPassword) {
      return ApiResult.fail('Mật khẩu xác nhận không khớp!');
    }

    try {
      final response = await _api.post('/MobileApi/RegisterRequestOtp', {
        'username': username,
        'email': email,
        'password': password,
        'confirmPassword': confirmPassword,
        'hoTen': hoTen,
      });

      if (response.statusCode != 200) {
        return ApiResult.fail(
          'Đăng ký thất bại. Mã phản hồi: ${response.statusCode}',
        );
      }

      final json = _api.parseJson(response);
      if (json == null) {
        return ApiResult.fail(
          'Máy chủ trả về dữ liệu không hợp lệ khi đăng ký.',
        );
      }

      if (json['success'] == true) {
        return const ApiResult(success: true);
      }

      return ApiResult.fail(
        json['message']?.toString() ?? 'Đăng ký thất bại!',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối máy chủ: ${e.toString()}');
    }
  }

  Future<ApiResult<void>> verifyRegisterOtp(String otp) async {
    try {
      final response = await _api.post('/MobileApi/RegisterVerifyOtp', {
        'otp': otp,
      });

      if (response.statusCode != 200) {
        return ApiResult.fail(
          'Xác thực OTP thất bại. Mã phản hồi: ${response.statusCode}',
        );
      }

      final json = _api.parseJson(response);
      if (json == null) {
        return ApiResult.fail(
          'Máy chủ trả về dữ liệu không hợp lệ khi xác thực OTP.',
        );
      }

      if (json['success'] == true) {
        return const ApiResult(success: true);
      }

      return ApiResult.fail(
        json['message']?.toString() ?? 'Xác thực OTP thất bại!',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối máy chủ: ${e.toString()}');
    }
  }

  Future<void> logout() async {
    try {
      await _api.get('/TaiKhoan/Logout');
    } catch (_) {}
    _currentUser = null;
    await _clearUserFromPrefs();
    await _api.clearCookies();
    
    // XÓA JWT TOKEN KHI ĐĂNG XUẤT
    await _secureStorage.delete(key: 'jwt_token');
    print('Đã xóa JWT Token khỏi thiết bị!');
  }
  
  Future<void> updateLocalUser(UserModel updated) async {
    _currentUser = updated;
    await _saveUserToPrefs(updated);
  }
}
