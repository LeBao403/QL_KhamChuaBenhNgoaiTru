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
          json?['message'] ?? 'Khong tai duoc lich su kham!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tai lich su kham qua lau. Vui long thu lai sau it phut.',
      );
    } catch (e) {
      return ApiResult.fail('Loi ket noi: ${e.toString()}');
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
          json?['message'] ?? 'Khong tai duoc don thuoc!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tai don thuoc qua lau. Vui long thu lai sau it phut.',
      );
    } catch (e) {
      return ApiResult.fail('Loi ket noi: ${e.toString()}');
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
          json?['message'] ?? 'Khong tai duoc chi tiet!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tai chi tiet don thuoc qua lau. Vui long thu lai sau it phut.',
      );
    } catch (e) {
      return ApiResult.fail('Loi ket noi: ${e.toString()}');
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
          json?['message'] ?? 'Khong tai duoc hoa don!',
        );
      }
      return ApiResult.fail('Loi server!');
    } on TimeoutException {
      return ApiResult.fail(
        'Tai hoa don qua lau. Vui long thu lai sau it phut.',
      );
    } catch (e) {
      return ApiResult.fail('Loi ket noi: ${e.toString()}');
    }
  }
}
