import 'user_model.dart';

class HomeStatsModel {
  final int tongSoKhoa;
  final int tongSoPhong;
  final int tongSoNhanVien;
  final int tongLuotKham;

  const HomeStatsModel({
    required this.tongSoKhoa,
    required this.tongSoPhong,
    required this.tongSoNhanVien,
    required this.tongLuotKham,
  });

  factory HomeStatsModel.fromJson(Map<String, dynamic> json) {
    return HomeStatsModel(
      tongSoKhoa: json['TongSoKhoa'] ?? 0,
      tongSoPhong: json['TongSoPhong'] ?? 0,
      tongSoNhanVien: json['TongSoNhanVien'] ?? 0,
      tongLuotKham: json['TongLuotKham'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'TongSoKhoa': tongSoKhoa,
      'TongSoPhong': tongSoPhong,
      'TongSoNhanVien': tongSoNhanVien,
      'TongLuotKham': tongLuotKham,
    };
  }
}

class DoctorModel {
  final String maNV;
  final String hoTen;
  final String chucDanh;
  final String chuyenKhoa;
  final String? hinhAnh;

  const DoctorModel({
    required this.maNV,
    required this.hoTen,
    required this.chucDanh,
    required this.chuyenKhoa,
    this.hinhAnh,
  });

  factory DoctorModel.fromJson(Map<String, dynamic> json) {
    return DoctorModel(
      maNV: (json['MaNV'] ?? '').toString(),
      hoTen: ServiceModel.fromJson({
        'MaDV': '',
        'TenDV': json['HoTen'] ?? '',
        'GiaDichVu': 0,
      }).tenDV,
      chucDanh: ServiceModel.fromJson({
        'MaDV': '',
        'TenDV': json['ChucDanh'] ?? '',
        'GiaDichVu': 0,
      }).tenDV,
      chuyenKhoa: ServiceModel.fromJson({
        'MaDV': '',
        'TenDV': json['ChuyenKhoa'] ?? '',
        'GiaDichVu': 0,
      }).tenDV,
      hinhAnh: json['HinhAnh']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'MaNV': maNV,
      'HoTen': hoTen,
      'ChucDanh': chucDanh,
      'ChuyenKhoa': chuyenKhoa,
      'HinhAnh': hinhAnh,
    };
  }

  String get tenRutGon {
    final parts = hoTen.trim().split(RegExp(r'\s+'));
    if (parts.length <= 2) return hoTen;
    return parts.sublist(parts.length - 2).join(' ');
  }

  String get initials {
    final parts = hoTen.trim().split(RegExp(r'\s+')).where((e) => e.isNotEmpty);
    final values = parts.take(2).map((e) => e[0].toUpperCase()).join();
    return values.isEmpty ? 'BS' : values;
  }
}

class SpecialtyModel {
  final int maKhoa;
  final String tenKhoa;
  final String moTa;

  const SpecialtyModel({
    required this.maKhoa,
    required this.tenKhoa,
    required this.moTa,
  });

  factory SpecialtyModel.fromJson(Map<String, dynamic> json) {
    return SpecialtyModel(
      maKhoa: json['MaKhoa'] ?? 0,
      tenKhoa: ServiceModel.fromJson({
        'MaDV': '',
        'TenDV': json['TenKhoa'] ?? '',
        'GiaDichVu': 0,
      }).tenDV,
      moTa: ServiceModel.fromJson({
        'MaDV': '',
        'TenDV': json['MoTa'] ?? '',
        'GiaDichVu': 0,
      }).tenDV,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'MaKhoa': maKhoa,
      'TenKhoa': tenKhoa,
      'MoTa': moTa,
    };
  }

  String get slug => tenKhoa.toLowerCase();
}

class NewsModel {
  final String tieuDe;
  final String tomTat;
  final DateTime? ngayDang;
  final String chuyenMuc;

  const NewsModel({
    required this.tieuDe,
    required this.tomTat,
    this.ngayDang,
    required this.chuyenMuc,
  });

  factory NewsModel.fromJson(Map<String, dynamic> json) {
    return NewsModel(
      tieuDe: (json['TieuDe'] ?? '').toString(),
      tomTat: (json['TomTat'] ?? '').toString(),
      ngayDang: json['NgayDang'] != null
          ? DateTime.tryParse(json['NgayDang'].toString())
          : null,
      chuyenMuc: (json['ChuyenMuc'] ?? '').toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'TieuDe': tieuDe,
      'TomTat': tomTat,
      'NgayDang': ngayDang?.toIso8601String(),
      'ChuyenMuc': chuyenMuc,
    };
  }
}

class HomeLandingModel {
  final List<SpecialtyModel> specialties;
  final List<DoctorModel> doctors;
  final List<NewsModel> news;
  final HomeStatsModel stats;

  const HomeLandingModel({
    required this.specialties,
    required this.doctors,
    required this.news,
    required this.stats,
  });

  factory HomeLandingModel.fromJson(Map<String, dynamic> json) {
    final specialtiesJson = (json['specialties'] as List?) ?? const [];
    final doctorsJson = (json['doctors'] as List?) ?? const [];
    final newsJson = (json['news'] as List?) ?? const [];

    return HomeLandingModel(
      specialties: specialtiesJson
          .map((item) => SpecialtyModel.fromJson(item as Map<String, dynamic>))
          .toList(),
      doctors: doctorsJson
          .map((item) => DoctorModel.fromJson(item as Map<String, dynamic>))
          .toList(),
      news: newsJson
          .map((item) => NewsModel.fromJson(item as Map<String, dynamic>))
          .toList(),
      stats: HomeStatsModel.fromJson(
        (json['stats'] as Map<String, dynamic>?) ?? const {},
      ),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'specialties': specialties.map((item) => item.toJson()).toList(),
      'doctors': doctors.map((item) => item.toJson()).toList(),
      'news': news.map((item) => item.toJson()).toList(),
      'stats': stats.toJson(),
    };
  }
}

class DoctorDirectoryModel {
  final List<DoctorModel> doctors;
  final List<String> specialties;
  final int page;
  final int pageSize;
  final int totalItems;
  final int totalPages;

  const DoctorDirectoryModel({
    required this.doctors,
    required this.specialties,
    required this.page,
    required this.pageSize,
    required this.totalItems,
    required this.totalPages,
  });

  factory DoctorDirectoryModel.fromJson(Map<String, dynamic> json) {
    final doctorsJson = (json['data'] as List?) ?? const [];
    final filters = (json['filters'] as Map<String, dynamic>?) ?? const {};
    final paging = (json['paging'] as Map<String, dynamic>?) ?? const {};

    return DoctorDirectoryModel(
      doctors: doctorsJson
          .map((item) => DoctorModel.fromJson(item as Map<String, dynamic>))
          .toList(),
      specialties: ((filters['specialties'] as List?) ?? const [])
          .map((item) => item.toString())
          .toList(),
      page: paging['page'] ?? 1,
      pageSize: paging['pageSize'] ?? 20,
      totalItems: paging['totalItems'] ?? 0,
      totalPages: paging['totalPages'] ?? 0,
    );
  }
}
