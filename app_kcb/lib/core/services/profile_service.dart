import '../models/user_model.dart';
import 'api_service.dart';

/// Profile service – lấy và cập nhật thông tin bệnh nhân
class ProfileService {
  static final ProfileService _instance = ProfileService._internal();
  factory ProfileService() => _instance;
  ProfileService._internal();

  final ApiService _api = ApiService();

  /// Lấy thông tin đầy đủ của bệnh nhân hiện tại từ server.
  /// Endpoint: GET /BenhNhanPortal/GetProfileJson (cần thêm vào backend)
  /// Fallback: scrape HTML từ /BenhNhanPortal/ThongTinCaNhan
  Future<ApiResult<UserModel>> getProfile() async {
    try {
      final resp = await _api.get('/MobileApi/GetProfile');
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null && json['success'] == true) {
          return ApiResult.ok(UserModel.fromJson(json['profile']));
        }
      }
      return ApiResult.fail('Không thể tải thông tin. Vui lòng thử lại!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }

  /// Cập nhật thông tin bệnh nhân.
  /// Endpoint: POST /BenhNhanPortal/UpdateProfileJson
  Future<ApiResult<void>> updateProfile(UserModel user) async {
    try {
      final body = <String, String>{
        'HoTen': user.hoTen,
        'NgaySinh': user.ngaySinh?.toIso8601String().split('T')[0] ?? '',
        'GioiTinh': user.gioiTinh ?? '',
        'SDT': user.sdt ?? '',
        'Email': user.email ?? '',
        'DiaChi': user.diaChi ?? '',
        'CCCD': user.cccd ?? '',
        'BHYT': user.bhyt ? '1' : '0',
        'SoTheBHYT': user.soTheBHYT ?? '',
        'HanSuDungBHYT':
            user.hanSuDungBHYT?.toIso8601String().split('T')[0] ?? '',
        'TuyenKham': user.tuyenKham ?? '',
        'MucHuongBHYT': user.mucHuongBHYT?.toString() ?? '',
      };

      // Thử endpoint JSON
      final resp = await _api.post('/MobileApi/UpdateProfile', body);
      if (resp.statusCode == 200) {
        final json = _api.parseJson(resp);
        if (json != null) {
          if (json['success'] == true) return const ApiResult(success: true);
          return ApiResult.fail(json['message'] ?? 'Cập nhật thất bại!');
        }
      }

      // Fallback: HTML form (dùng antiforgery token trống)
      final htmlBody = {
        ...body,
        '__RequestVerificationToken': '',
        'MaBN': user.maBN,
      };
      final htmlResp =
          await _api.post('/BenhNhanPortal/ThongTinCaNhan', htmlBody);
      if (htmlResp.statusCode == 200 || htmlResp.statusCode == 302) {
        if (htmlResp.body.contains('thành công') ||
            htmlResp.statusCode == 302) {
          return const ApiResult(success: true);
        }
      }
      return ApiResult.fail('Cập nhật thất bại. Vui lòng thử lại!');
    } catch (e) {
      return ApiResult.fail('Lỗi kết nối: ${e.toString()}');
    }
  }
}
