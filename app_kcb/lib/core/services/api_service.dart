import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

/// HTTP service với cookie-based session (giống browser).
/// Backend ASP.NET MVC dùng Session qua cookie "ASP.NET_SessionId".
class ApiService {
  static const Duration defaultTimeout = Duration(seconds: 30);

  // ── Cấu hình base URL ──────────────────────────────────────────────────────
  // Thay đổi IP/port cho phù hợp với môi trường của bạn:
  //  - Android Emulator → http://10.0.2.2:<port>   (10.0.2.2 = host machine)
  //  - iOS Simulator    → http://localhost:<port>
  //  - Thiết bị thật   → http://<IP_LAN_máy_tính>:<port>
  //
  // IIS Express HTTP port: 8080 (dùng HTTP để tránh SSL cert tự ký trong dev)
  static String get baseUrl {
    const configured = String.fromEnvironment('API_BASE_URL');
    if (configured.isNotEmpty) return configured;

    if (Platform.isAndroid) {
      return 'http://10.0.2.2:8080';
    }
    return 'http://localhost:8080';
  }

  static String resolveUrl(String path) {
    final trimmed = path.trim();
    if (trimmed.isEmpty) return '';
    if (trimmed.startsWith('http://') || trimmed.startsWith('https://')) {
      return trimmed;
    }

    final normalized = trimmed.startsWith('/') ? trimmed : '/$trimmed';
    return '$baseUrl$normalized';
  }

  // Dev với IIS Express có thể cần ép Host: localhost khi chạy Android Emulator.
  // Khi build môi trường thật, truyền --dart-define=API_HOST_HEADER= để tắt.
  static const String _iisHostHeader = String.fromEnvironment(
    'API_HOST_HEADER',
    defaultValue: 'localhost',
  );

  static const String _cookieKey = 'session_cookies';
  static const _secureStorage = FlutterSecureStorage();

  // Singleton
  static final ApiService _instance = ApiService._internal();
  factory ApiService() => _instance;
  ApiService._internal();

  final http.Client _client = http.Client();
  bool _cookiesLoaded = false;

  // Lưu cookie nhận từ server
  Map<String, String> _cookies = {};

  // ── Cookie persistence ─────────────────────────────────────────────────────
  Future<void> loadCookies() async {
    if (_cookiesLoaded) return;
    final prefs = await SharedPreferences.getInstance();
    var cookieStr = await _secureStorage.read(key: _cookieKey);
    final legacyCookieStr = prefs.getString(_cookieKey);
    if ((cookieStr == null || cookieStr.isEmpty) && legacyCookieStr != null) {
      cookieStr = legacyCookieStr;
      await _secureStorage.write(key: _cookieKey, value: legacyCookieStr);
      await prefs.remove(_cookieKey);
    }
    cookieStr ??= '{}';
    try {
      final map = jsonDecode(cookieStr) as Map<String, dynamic>;
      _cookies = map.cast<String, String>();
    } catch (_) {
      _cookies = {};
    }
    _cookiesLoaded = true;
  }

  Future<void> _saveCookies() async {
    await _secureStorage.write(key: _cookieKey, value: jsonEncode(_cookies));
  }

  Future<void> clearCookies() async {
    _cookies = {};
    _cookiesLoaded = false;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_cookieKey);
    await _secureStorage.delete(key: _cookieKey);
  }

  Future<void> _updateCookies(http.Response response) async {
    final setCookie = response.headers['set-cookie'];
    if (setCookie != null) {
      for (final part in _splitSetCookieHeader(setCookie)) {
        final segments = part.trim().split(';');
        if (segments.isNotEmpty) {
          final kv = segments[0].trim().split('=');
          if (kv.length >= 2) {
            _cookies[kv[0].trim()] = kv.sublist(1).join('=').trim();
          }
        }
      }
      await _saveCookies();
    }
  }

  String _buildCookieHeader() {
    return _cookies.entries.map((e) => '${e.key}=${e.value}').join('; ');
  }

  Map<String, String> _headers({Map<String, String>? extra}) {
    final h = <String, String>{
      'Content-Type': 'application/x-www-form-urlencoded',
      'Accept': 'application/json, text/html',
    };
    if (_iisHostHeader.isNotEmpty) {
      h['Host'] = _iisHostHeader;
    }
    if (_cookies.isNotEmpty) {
      h['Cookie'] = _buildCookieHeader();
    }
    if (extra != null) h.addAll(extra);
    return h;
  }

  // ── HTTP methods ───────────────────────────────────────────────────────────
  Future<http.Response> get(
    String path, {
    Map<String, String>? queryParams,
    Duration timeout = defaultTimeout,
    int retries = 0,
  }) async {
    await loadCookies();
    Uri uri = Uri.parse('$baseUrl$path');
    if (queryParams != null && queryParams.isNotEmpty) {
      uri = uri.replace(queryParameters: queryParams);
    }

    Object? lastError;
    for (var attempt = 0; attempt <= retries; attempt++) {
      try {
        final response =
            await _client.get(uri, headers: _headers()).timeout(timeout);
        await _updateCookies(response);
        return response;
      } on SocketException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      } on TimeoutException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      }

      await Future<void>.delayed(Duration(milliseconds: 400 * (attempt + 1)));
    }

    throw lastError ?? Exception('Request failed');
  }

  Future<http.Response> post(
    String path,
    Map<String, String> body, {
    Duration timeout = defaultTimeout,
    int retries = 0,
  }) async {
    await loadCookies();
    final uri = Uri.parse('$baseUrl$path');

    Object? lastError;
    for (var attempt = 0; attempt <= retries; attempt++) {
      try {
        final response = await _client
            .post(uri, headers: _headers(), body: body)
            .timeout(timeout);
        await _updateCookies(response);
        return response;
      } on SocketException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      } on TimeoutException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      }

      await Future<void>.delayed(Duration(milliseconds: 400 * (attempt + 1)));
    }

    throw lastError ?? Exception('Request failed');
  }

  // ── Helpers ────────────────────────────────────────────────────────────────
  bool get hasSession =>
      _cookies.containsKey('ASP.NET_SessionId') &&
      _cookies['ASP.NET_SessionId']!.isNotEmpty;

  /// Parse JSON phản hồi. Trả null nếu không phải JSON.
  dynamic parseJson(http.Response response) {
    try {
      return jsonDecode(utf8.decode(response.bodyBytes));
    } catch (_) {
      return null;
    }
  }

  List<String> _splitSetCookieHeader(String header) {
    final cookies = <String>[];
    var start = 0;

    for (var i = 0; i < header.length; i++) {
      if (header.codeUnitAt(i) != 44) continue;

      final rest = header.substring(i + 1);
      if (RegExp(r'^\s*[^=;,\s]+=.+').hasMatch(rest)) {
        cookies.add(header.substring(start, i).trim());
        start = i + 1;
      }
    }

    cookies.add(header.substring(start).trim());
    return cookies.where((cookie) => cookie.isNotEmpty).toList();
  }
}
