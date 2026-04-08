import '../models/user_model.dart';
import 'api_service.dart';

/// Medical service – lịch sử khám, đơn thuốc, hóa đơn
class MedicalService {
  static final MedicalService _instance = MedicalService._internal();
  factory MedicalService() => _instance;
  MedicalService._internal();

  final ApiService _api = ApiService();

  // ── Lịch sử khám ──────────────────────────────────────────────────────────
  Future<ApiResult<List<LichSuKhamModel>>> getLichSuKham() async {
    try {
      final resp = await _api.get('/MobileApi/GetLichSuKham');
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map((e) => LichSuKhamModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(json?['message'] ?? 'Không tải được lịch sử khám!');
      }
      return ApiResult.fail('Lỗi server!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  // ── Đơn thuốc ─────────────────────────────────────────────────────────────
  Future<ApiResult<List<DonThuocModel>>> getDonThuoc() async {
    try {
      final resp = await _api.get('/MobileApi/GetDonThuoc');
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map((e) => DonThuocModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(json?['message'] ?? 'Không tải được đơn thuốc!');
      }
      return ApiResult.fail('Lỗi server!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  Future<ApiResult<List<ChiTietThuocModel>>> getChiTietDonThuoc(
      int maDonThuoc) async {
    try {
      final resp = await _api.get('/MobileApi/GetChiTietDonThuoc',
          queryParams: {'id': maDonThuoc.toString()});
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map(
                  (e) => ChiTietThuocModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(json?['message'] ?? 'Không tải được chi tiết!');
      }
      return ApiResult.fail('Lỗi server!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  // ── Hóa đơn ───────────────────────────────────────────────────────────────
  Future<ApiResult<List<HoaDonModel>>> getHoaDon() async {
    try {
      final resp = await _api.get('/MobileApi/GetHoaDon');
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map((e) => HoaDonModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(json?['message'] ?? 'Không tải được hóa đơn!');
      }
      return ApiResult.fail('Lỗi server!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }
}
