import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

/// HTTP service với cookie-based session (giống browser).
/// Backend ASP.NET MVC dùng Session qua cookie "ASP.NET_SessionId".
class ApiService {
  // ── Cấu hình base URL ──────────────────────────────────────────────────────
  // Thay đổi IP/port cho phù hợp với môi trường của bạn:
  //  - Android Emulator → http://10.0.2.2:<port>   (10.0.2.2 = host machine)
  //  - iOS Simulator    → http://localhost:<port>
  //  - Thiết bị thật   → http://<IP_LAN_máy_tính>:<port>
  //
  // Port của IIS Express khi chạy qua Visual Studio: 57595 (HTTP)
  // Port khi chạy IIS Express thủ công với port 8080: 8080
  static String get baseUrl {
    if (Platform.isAndroid) {
      return 'https://10.0.2.2:44326';
    }
    return 'https://localhost:44326';
  }

  // IIS Express chỉ chấp nhận Host: localhost → ép header này vào mọi request
  // để tránh bị từ chối khi chạy từ Android Emulator (Host: 10.0.2.2)
  static const String _iisHostHeader = 'localhost';

  static const String _cookieKey = 'session_cookies';

  // Singleton
  static final ApiService _instance = ApiService._internal();
  factory ApiService() => _instance;
  ApiService._internal();

  // Lưu cookie nhận từ server
  Map<String, String> _cookies = {};

  // ── Cookie persistence ─────────────────────────────────────────────────────
  Future<void> loadCookies() async {
    final prefs = await SharedPreferences.getInstance();
    final cookieStr = prefs.getString(_cookieKey) ?? '{}';
    try {
      final map = jsonDecode(cookieStr) as Map<String, dynamic>;
      _cookies = map.cast<String, String>();
    } catch (_) {
      _cookies = {};
    }
  }

  Future<void> _saveCookies() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_cookieKey, jsonEncode(_cookies));
  }

  Future<void> clearCookies() async {
    _cookies = {};
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_cookieKey);
  }

  void _updateCookies(http.Response response) {
    final setCookie = response.headers['set-cookie'];
    if (setCookie != null) {
      // Giải phân tích cookie đơn giản – lấy mỗi name=value
      for (final part in setCookie.split(',')) {
        final segments = part.trim().split(';');
        if (segments.isNotEmpty) {
          final kv = segments[0].trim().split('=');
          if (kv.length >= 2) {
            _cookies[kv[0].trim()] = kv.sublist(1).join('=').trim();
          }
        }
      }
      _saveCookies();
    }
  }

  String _buildCookieHeader() {
    return _cookies.entries.map((e) => '${e.key}=${e.value}').join('; ');
  }

  Map<String, String> _headers({Map<String, String>? extra}) {
    final h = <String, String>{
      'Content-Type': 'application/x-www-form-urlencoded',
      'Accept': 'application/json, text/html',
      // IIS Express chỉ chấp nhận Host: localhost.
      // Android Emulator gửi Host: 10.0.2.2 → bị từ chối.
      // Header này ép IIS Express nhận request như từ trình duyệt.
      'Host': _iisHostHeader,
    };
    if (_cookies.isNotEmpty) {
      h['Cookie'] = _buildCookieHeader();
    }
    if (extra != null) h.addAll(extra);
    return h;
  }

  // Backend dùng HTTPS với cert tự ký (localhost dev) → cần bypass SSL verify
  http.Client _buildClient() {
    // Trong môi trường production, bỏ phần override HttpClient này
    final ioClient = HttpClient()
      ..badCertificateCallback = (_, __, ___) => true;
    return _IOClient(ioClient);
  }

  // ── HTTP methods ───────────────────────────────────────────────────────────
  Future<http.Response> get(String path,
      {Map<String, String>? queryParams}) async {
    await loadCookies();
    Uri uri = Uri.parse('$baseUrl$path');
    if (queryParams != null && queryParams.isNotEmpty) {
      uri = uri.replace(queryParameters: queryParams);
    }
    final client = _buildClient();
    try {
      final response = await client
          .get(uri, headers: _headers())
          .timeout(const Duration(seconds: 30));
      _updateCookies(response);
      return response;
    } finally {
      client.close();
    }
  }

  Future<http.Response> post(String path, Map<String, String> body) async {
    await loadCookies();
    final uri = Uri.parse('$baseUrl$path');
    final client = _buildClient();
    try {
      final response = await client
          .post(uri, headers: _headers(), body: body)
          .timeout(const Duration(seconds: 30));
      _updateCookies(response);
      return response;
    } finally {
      client.close();
    }
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
}

// ── Minimal IOClient wrapper để dùng HttpClient tùy chỉnh ─────────────────────
class _IOClient extends http.BaseClient {
  final HttpClient _inner;
  _IOClient(this._inner);

  @override
  Future<http.StreamedResponse> send(http.BaseRequest request) async {
    final ioRequest = await _inner.openUrl(
      request.method,
      request.url,
    );
    request.headers.forEach((name, value) {
      ioRequest.headers.set(name, value);
    });
    ioRequest.contentLength = request.contentLength ?? -1;
    await ioRequest.addStream(request.finalize());
    final response = await ioRequest.close();
    final headers = <String, String>{};
    response.headers.forEach((name, values) {
      headers[name] = values.join(',');
    });
    return http.StreamedResponse(
      response.cast<List<int>>(),
      response.statusCode,
      headers: headers,
      request: request,
    );
  }

  @override
  void close() => _inner.close(force: true);
}
