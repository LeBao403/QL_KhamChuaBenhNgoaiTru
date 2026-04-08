import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';

import '../models/discovery_models.dart';
import '../models/user_model.dart';
import 'api_service.dart';

class DiscoveryService {
  static final DiscoveryService _instance = DiscoveryService._internal();
  factory DiscoveryService() => _instance;
  DiscoveryService._internal();

  final ApiService _api = ApiService();

  static const String _homeCacheKey = 'home_landing_cache_v1';

  Future<HomeLandingModel?> getCachedHomeData() async {
    final prefs = await SharedPreferences.getInstance();
    final raw = prefs.getString(_homeCacheKey);
    if (raw == null || raw.isEmpty) return null;

    try {
      final json = jsonDecode(raw) as Map<String, dynamic>;
      return HomeLandingModel.fromJson(json);
    } catch (_) {
      return null;
    }
  }

  Future<void> _saveHomeCache(HomeLandingModel model) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_homeCacheKey, jsonEncode(model.toJson()));
  }

  Future<ApiResult<HomeLandingModel>> getHomeData() async {
    final cached = await getCachedHomeData();

    try {
      final response = await _api
          .get('/MobileApi/GetHomeData')
          .timeout(const Duration(seconds: 8));
      final json = _api.parseJson(response);

      if (response.statusCode == 200 &&
          json != null &&
          json['success'] == true) {
        final model =
            HomeLandingModel.fromJson(json['data'] as Map<String, dynamic>);
        await _saveHomeCache(model);
        return ApiResult.ok(model);
      }

      if (cached != null) {
        return ApiResult.ok(cached);
      }

      return ApiResult.fail(
        json?['message'] ?? 'Khong tai duoc du lieu trang chu.',
      );
    } catch (e) {
      if (cached != null) {
        return ApiResult.ok(cached);
      }
      return ApiResult.fail('Loi ket noi: $e');
    }
  }

  Future<ApiResult<DoctorDirectoryModel>> getDoctors({
    String searchQuery = '',
    String specialty = '',
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final response = await _api.get(
        '/MobileApi/GetDoctors',
        queryParams: {
          'searchString': searchQuery,
          'khoa': specialty,
          'page': '$page',
          'pageSize': '$pageSize',
        },
      );

      final json = _api.parseJson(response);
      if (response.statusCode == 200 &&
          json != null &&
          json['success'] == true) {
        return ApiResult.ok(
          DoctorDirectoryModel.fromJson(json as Map<String, dynamic>),
        );
      }

      return ApiResult.fail(
        json?['message'] ?? 'Khong tai duoc danh sach bac si.',
      );
    } catch (e) {
      return ApiResult.fail('Loi ket noi: $e');
    }
  }

  Future<ApiResult<List<SpecialtyModel>>> getSpecialties() async {
    try {
      final response = await _api.get('/MobileApi/GetSpecialties');
      final json = _api.parseJson(response);

      if (response.statusCode == 200 &&
          json != null &&
          json['success'] == true) {
        final items = ((json['data'] as List?) ?? const [])
            .map(
              (item) => SpecialtyModel.fromJson(item as Map<String, dynamic>),
            )
            .toList();
        return ApiResult.ok(items);
      }

      return ApiResult.fail(
        json?['message'] ?? 'Khong tai duoc danh sach chuyen khoa.',
      );
    } catch (e) {
      return ApiResult.fail('Loi ket noi: $e');
    }
  }
}
