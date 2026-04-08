import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/user_model.dart';
import 'api_service.dart';

class AuthService {
  static final AuthService _instance = AuthService._internal();
  factory AuthService() => _instance;
  AuthService._internal();

  final ApiService _api = ApiService();

  static const String _userKey = 'current_user';
  UserModel? _currentUser;

  UserModel? get currentUser => _currentUser;
  bool get isLoggedIn => _currentUser != null;

  Future<void> init() async {
    await _api.loadCookies();
    await _loadUserFromPrefs();
  }

  Future<void> _loadUserFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    final userJson = prefs.getString(_userKey);
    if (userJson != null) {
      try {
        _currentUser = UserModel.fromJson(jsonDecode(userJson));
      } catch (_) {
        _currentUser = null;
      }
    }
  }

  Future<void> _saveUserToPrefs(UserModel user) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_userKey, jsonEncode(user.toJson()));
  }

  Future<void> _clearUserFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_userKey);
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
    String password,
    String confirmPassword,
  ) async {
    if (password != confirmPassword) {
      return ApiResult.fail('Mật khẩu xác nhận không khớp!');
    }

    try {
      final response = await _api.post('/MobileApi/Register', {
        'username': username,
        'password': password,
        'confirmPassword': confirmPassword,
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

  Future<void> logout() async {
    try {
      await _api.get('/TaiKhoan/Logout');
    } catch (_) {}
    _currentUser = null;
    await _clearUserFromPrefs();
    await _api.clearCookies();
  }

  Future<void> updateLocalUser(UserModel updated) async {
    _currentUser = updated;
    await _saveUserToPrefs(updated);
  }
}
