import 'dart:async';

import '../models/user_model.dart';
import 'api_service.dart';

/// Medical service - lich su kham, don thuoc, hoa don.
class MedicalService {
  static final MedicalService _instance = MedicalService._internal();
  factory MedicalService() => _instance;
  MedicalService._internal();

  final ApiService _api = ApiService();

  Future<ApiResult<List<LichSuKhamModel>>> getLichSuKham() async {
    try {
      final resp = await _api.get(
        '/MobileApi/GetLichSuKham',
        timeout: const Duration(seconds: 60),
        retries: 1,
      );
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map((e) => LichSuKhamModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(
          json?['message'] ?? 'Không tải được lịch sử khám!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tải lịch sử khám quá lâu. Vui lòng thử lại sau ít phút.',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  Future<ApiResult<List<DonThuocModel>>> getDonThuoc() async {
    try {
      final resp = await _api.get(
        '/MobileApi/GetDonThuoc',
        timeout: const Duration(seconds: 45),
        retries: 1,
      );
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map((e) => DonThuocModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(
          json?['message'] ?? 'Không tải được đơn thuốc!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tải đơn thuốc quá lâu. Vui lòng thử lại sau ít phút.',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  Future<ApiResult<List<ChiTietThuocModel>>> getChiTietDonThuoc(
    String maDonThuoc,
  ) async {
    try {
      final resp = await _api.get(
        '/MobileApi/GetChiTietDonThuoc',
        queryParams: {'id': maDonThuoc},
        timeout: const Duration(seconds: 45),
        retries: 1,
      );
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map(
                (e) => ChiTietThuocModel.fromJson(e as Map<String, dynamic>),
              )
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(
          json?['message'] ?? 'Không tải được chi tiết!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tải chi tiết đơn thuốc quá lâu. Vui lòng thử lại sau ít phút.',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  Future<ApiResult<List<HoaDonModel>>> getHoaDon() async {
    try {
      final resp = await _api.get(
        '/MobileApi/GetHoaDon',
        timeout: const Duration(seconds: 45),
        retries: 1,
      );
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          final list = (json['data'] as List)
              .map((e) => HoaDonModel.fromJson(e as Map<String, dynamic>))
              .toList();
          return ApiResult.ok(list);
        }
        return ApiResult.fail(
          json?['message'] ?? 'Không tải được hóa đơn!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tải hóa đơn quá lâu. Vui lòng thử lại sau ít phút.',
      );
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }
}
