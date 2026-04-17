import 'dart:async';
import 'dart:convert';
import 'dart:io';
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
    if (Platform.isAndroid) {
      return 'http://10.0.2.2:8080';
    }
    return 'http://localhost:8080';
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

  // Dùng http.Client bình thường (HTTP không cần SSL bypass)
  http.Client _buildClient() {
    return http.Client();
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
      final client = _buildClient();
      try {
        final response =
            await client.get(uri, headers: _headers()).timeout(timeout);
        _updateCookies(response);
        return response;
      } on SocketException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      } on TimeoutException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      } finally {
        client.close();
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
      final client = _buildClient();
      try {
        final response = await client
            .post(uri, headers: _headers(), body: body)
            .timeout(timeout);
        _updateCookies(response);
        return response;
      } on SocketException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      } on TimeoutException catch (e) {
        lastError = e;
        if (attempt >= retries) rethrow;
      } finally {
        client.close();
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
