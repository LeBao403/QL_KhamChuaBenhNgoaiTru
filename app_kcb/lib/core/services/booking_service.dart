import 'dart:async';

import '../models/user_model.dart';
import 'api_service.dart';

class BookingService {
  static final BookingService _instance = BookingService._internal();
  factory BookingService() => _instance;
  BookingService._internal();

  final ApiService _api = ApiService();

  static const String _bookingPagePath = '/BenhNhanPortal/DatLichKham';

  String? _bookingFormToken;
  List<ServiceModel>? _cachedServices;

  Future<void> _loadBookingForm({bool forceRefresh = false}) async {
    if (!forceRefresh &&
        _bookingFormToken != null &&
        _cachedServices != null &&
        _cachedServices!.isNotEmpty) {
      return;
    }

    final resp = await _api.get(_bookingPagePath);
    if (resp.statusCode != 200) {
      throw Exception('Không tải được trang đặt lịch.');
    }

    final body = resp.body;

    final tokenMatch = RegExp(
      r'name="__RequestVerificationToken"[^>]*value="([^"]+)"',
      caseSensitive: false,
    ).firstMatch(body);
    _bookingFormToken = tokenMatch?.group(1);

    final services = <ServiceModel>[];
    final optionPattern = RegExp(
      r'<option\s+value="(DV[^"]+)"[^>]*>([^<]+)</option>',
      caseSensitive: false,
    );

    for (final match in optionPattern.allMatches(body)) {
      services.add(
        ServiceModel.fromSelectItem(match.group(1)!, match.group(2)!),
      );
    }

    _cachedServices = services;
  }

  Future<ApiResult<List<ServiceModel>>> getDanhSachDichVu() async {
    // 1. Dùng cache nếu có
    if (_cachedServices != null && _cachedServices!.isNotEmpty) {
      return ApiResult.ok(_cachedServices!);
    }

    // 2. Gọi MobileApi/GetDichVu trước (không cần parse HTML)
    try {
      final resp = await _api.get('/MobileApi/GetDichVu');
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = _parseList(json['data'], ServiceModel.fromJson);
          if (list.isNotEmpty) {
            _cachedServices = list;
            return ApiResult.ok(list);
          }
        }
      }
    } catch (_) {
      // MobileApi thất bại -> thử fallback HTML
    }

    // 3. Fallback: parse HTML từ web portal
    try {
      await _loadBookingForm();
      if (_cachedServices != null && _cachedServices!.isNotEmpty) {
        return ApiResult.ok(_cachedServices!);
      }
    } catch (_) {
      // HTML parse cũng thất bại
    }

    return ApiResult.fail('Không tải được danh sách dịch vụ!');
  }

  Future<ApiResult<List<TimeSlotModel>>> getKhungGio(DateTime ngayKham) async {
    try {
      final dateStr =
          '${ngayKham.year}-${ngayKham.month.toString().padLeft(2, '0')}-${ngayKham.day.toString().padLeft(2, '0')}';

      final mobileResp = await _api.post('/MobileApi/LoadKhungGio', {
        'ngayKham': dateStr,
      });
      final mobileResult = _parseTimeSlotResponse(mobileResp);
      if (mobileResult.success) {
        return mobileResult;
      }

      final webResp = await _api.post('/BenhNhanPortal/LoadKhungGio', {
        'ngayKham': dateStr,
      });
      return _parseTimeSlotResponse(
        webResp,
        fallbackMessage: mobileResult.message,
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<ApiResult<Map<String, dynamic>>> datLichKham({
    required DateTime ngayKham,
    required int maKhungGio,
    required String maDV,
    required String lyDo,
  }) async {
    try {
      final dateStr =
          '${ngayKham.year}-${ngayKham.month.toString().padLeft(2, '0')}-${ngayKham.day.toString().padLeft(2, '0')}';

      final mobileResp = await _api.post('/MobileApi/DatLichKham', {
        'ngayKham': dateStr,
        'maKhungGio': maKhungGio.toString(),
        'maDV': maDV,
        'lyDo': lyDo,
      });

      final mobileResult = _parseBookingResponse(mobileResp);
      if (mobileResult.success) {
        return mobileResult;
      }

      await _loadBookingForm(forceRefresh: true);

      if ((_bookingFormToken ?? '').isNotEmpty) {
        final webResp = await _api.post('/BenhNhanPortal/DatLichKham', {
          '__RequestVerificationToken': _bookingFormToken!,
          'ngayKham': dateStr,
          'maKhungGio': maKhungGio.toString(),
          'maDV': maDV,
          'lyDo': lyDo,
        });

        final webResult = _parseBookingResponse(webResp);
        if (webResult.success) {
          return webResult;
        }
      }

      return mobileResult;
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<ApiResult<Map<String, dynamic>>> taoMaQR(
    String maHD,
    String maPhieuDK,
  ) async {
    try {
      final resp = await _api.post('/BenhNhanPortal/TaoMaQROnline', {
        'maHD': maHD,
        'maPhieuDK': maPhieuDK,
      });
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null) {
          if (json['success'] == true) {
            return ApiResult.ok({
              'qrString': json['qrString'],
              'orderCode': json['orderCode'],
            });
          }
          return ApiResult.fail(json['message'] ?? 'Tạo QR thất bại!');
        }
      }
      return ApiResult.fail('Loi tao ma QR!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<bool> kiemTraThanhToan(
      int orderCode, String maPhieuDK, String maHD) async {
    try {
      final resp = await _api.post('/BenhNhanPortal/KiemTraThanhToanOnline', {
        'orderCode': orderCode.toString(),
        'maPhieuDK': maPhieuDK,
        'maHD': maHD,
      });
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        return json != null &&
            json['success'] == true &&
            json['isPaid'] == true;
      }
    } catch (_) {}
    return false;
  }

  Future<ApiResult<String>> taoThanhToanTheUrl(
    String maHD,
    String maPhieuDK,
  ) async {
    try {
      final resp = await _api.post('/MobileApi/TaoThanhToanTheUrl', {
        'maHD': maHD,
        'maPhieuDK': maPhieuDK,
        'returnUrl': ApiService.resolveUrl('/ThanhToan/KetQua'),
      });
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final paymentUrl = json['paymentUrl']?.toString() ?? '';
          if (paymentUrl.isNotEmpty) return ApiResult.ok(paymentUrl);
        }
        return ApiResult.fail(
          json?['message']?.toString() ?? 'Không tạo được liên kết VNPAY!',
        );
      }
      return ApiResult.fail('Không tạo được liên kết VNPAY!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<ApiResult<Map<String, dynamic>>> kiemTraThanhToanTheOnline(
    String maHD,
    String maPhieuDK,
  ) async {
    try {
      final resp = await _api.post('/MobileApi/KiemTraThanhToanThe', {
        'maHD': maHD,
        'maPhieuDK': maPhieuDK,
      });
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          return ApiResult.ok({
            'isPaid': json['isPaid'] == true,
            'isCanceled': json['isCanceled'] == true,
            'trangThaiThanhToan': json['trangThaiThanhToan']?.toString() ?? '',
            'trangThaiPhieu': json['trangThaiPhieu']?.toString() ?? '',
          });
        }
        return ApiResult.fail(
          json?['message']?.toString() ?? 'Không kiểm tra được thanh toán!',
        );
      }
      return ApiResult.fail('Không kiểm tra được thanh toán!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<ApiResult<void>> huyDatLichKhiKhongThanhToan(String maPhieuDK) async {
    try {
      final resp = await _api.post('/BenhNhanPortal/HuyDatLichOnline', {
        'maPhieuDK': maPhieuDK,
      });
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          return const ApiResult(success: true);
        }
        return ApiResult.fail(json?['message']?.toString() ?? 'Hủy thất bại!');
      }
      return ApiResult.fail('Hủy thất bại!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<ApiResult<List<LichKhamModel>>> getLichKham() async {
    try {
      final resp = await _api.get(
        '/MobileApi/GetLichKham',
        timeout: const Duration(seconds: 60),
        retries: 1,
      );
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = _parseList(json['data'], LichKhamModel.fromJson);
          return ApiResult.ok(list);
        }
      }
      return ApiResult.fail('Không tải được lịch khám!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tải lịch đặt khám quá lâu. Vui lòng thử lại sau ít phút.',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  Future<ApiResult<void>> huyLich(String maPhieuDK) async {
    try {
      final resp = await _api.post('/MobileApi/HuyLich', {
        'maPhieuDK': maPhieuDK,
      });
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          return const ApiResult(success: true);
        }
        return ApiResult.fail(
          json?['message']?.toString() ?? 'Hủy lịch thất bại!',
        );
      }
      return ApiResult.fail('Hủy lịch thất bại!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: $e');
    }
  }

  ApiResult<List<TimeSlotModel>> _parseTimeSlotResponse(
    dynamic resp, {
    String? fallbackMessage,
  }) {
    if (resp.statusCode == 200) {
      final json = _api.parseJson(resp);
      if (json != null && json['success'] == true) {
        final list = _parseList(json['data'], TimeSlotModel.fromJson);
        return ApiResult.ok(list);
      }

      return ApiResult.fail(
        json?['message']?.toString() ??
            fallbackMessage ??
            'Không tải được khung giờ!',
      );
    }

    return ApiResult.fail(
      fallbackMessage ?? 'Loi server khi tai khung gio! (${resp.statusCode})',
    );
  }

  ApiResult<Map<String, dynamic>> _parseBookingResponse(dynamic resp) {
    if (resp.statusCode == 200) {
      final json = _api.parseJson(resp);
      if (json != null) {
        if (json['success'] == true) {
          return ApiResult.ok({
            'maPhieuDK': json['maPhieuDK'],
            'maHD': json['maHD'],
            'tenQuay': json['tenQuay'] ?? 'Quay Tiep Tan',
          });
        }

        return ApiResult.fail(
          json['message']?.toString() ?? 'Đặt lịch thất bại!',
        );
      }
    }

    return ApiResult.fail('Lỗi máy chủ! Vui lòng thử lại.');
  }

  List<T> _parseList<T>(
    dynamic value,
    T Function(Map<String, dynamic> json) fromJson,
  ) {
    if (value is! List) return <T>[];

    return value
        .whereType<Map>()
        .map((item) => fromJson(Map<String, dynamic>.from(item)))
        .toList();
  }
}
