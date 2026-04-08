import 'package:flutter/material.dart';

// ─── User model ──────────────────────────────────────────────────────────────
class UserModel {
  final int maTK;
  final String username;
  final String maBN;
  final String hoTen;
  final String? cccd;
  final String? sdt;
  final String? email;
  final DateTime? ngaySinh;
  final String? gioiTinh;
  final String? diaChi;
  final bool bhyt;
  final String? soTheBHYT;
  final DateTime? hanSuDungBHYT;
  final String? tuyenKham;
  final int? mucHuongBHYT;
  final String? avatarPath;

  const UserModel({
    required this.maTK,
    required this.username,
    required this.maBN,
    required this.hoTen,
    this.cccd,
    this.sdt,
    this.email,
    this.ngaySinh,
    this.gioiTinh,
    this.diaChi,
    this.bhyt = false,
    this.soTheBHYT,
    this.hanSuDungBHYT,
    this.tuyenKham,
    this.mucHuongBHYT,
    this.avatarPath,
  });

  String get avatarLetter =>
      hoTen.trim().isNotEmpty ? hoTen.trim()[0].toUpperCase() : 'B';

  factory UserModel.fromJson(Map<String, dynamic> json) {
    return UserModel(
      maTK: json['MaTK'] ?? 0,
      username: json['Username'] ?? '',
      maBN: json['MaBN'] ?? '',
      hoTen: json['HoTen'] ?? '',
      cccd: json['CCCD'],
      sdt: json['SDT'],
      email: json['Email'],
      ngaySinh: json['NgaySinh'] != null
          ? DateTime.tryParse(json['NgaySinh'])
          : null,
      gioiTinh: json['GioiTinh'],
      diaChi: json['DiaChi'],
      bhyt: json['BHYT'] ?? false,
      soTheBHYT: json['SoTheBHYT'],
      hanSuDungBHYT: json['HanSuDungBHYT'] != null
          ? DateTime.tryParse(json['HanSuDungBHYT'])
          : null,
      tuyenKham: json['TuyenKham'],
      mucHuongBHYT: json['MucHuongBHYT'],
      avatarPath: json['AvatarPath'],
    );
  }

  Map<String, dynamic> toJson() => {
        'MaTK': maTK,
        'Username': username,
        'MaBN': maBN,
        'HoTen': hoTen,
        'CCCD': cccd,
        'SDT': sdt,
        'Email': email,
        'NgaySinh': ngaySinh?.toIso8601String(),
        'GioiTinh': gioiTinh,
        'DiaChi': diaChi,
        'BHYT': bhyt,
        'SoTheBHYT': soTheBHYT,
        'HanSuDungBHYT': hanSuDungBHYT?.toIso8601String(),
        'TuyenKham': tuyenKham,
        'MucHuongBHYT': mucHuongBHYT,
      };

  UserModel copyWith({
    String? hoTen,
    String? cccd,
    String? sdt,
    String? email,
    DateTime? ngaySinh,
    String? gioiTinh,
    String? diaChi,
    bool? bhyt,
    String? soTheBHYT,
    DateTime? hanSuDungBHYT,
    String? tuyenKham,
    int? mucHuongBHYT,
    String? avatarPath,
  }) {
    return UserModel(
      maTK: maTK,
      username: username,
      maBN: maBN,
      hoTen: hoTen ?? this.hoTen,
      cccd: cccd ?? this.cccd,
      sdt: sdt ?? this.sdt,
      email: email ?? this.email,
      ngaySinh: ngaySinh ?? this.ngaySinh,
      gioiTinh: gioiTinh ?? this.gioiTinh,
      diaChi: diaChi ?? this.diaChi,
      bhyt: bhyt ?? this.bhyt,
      soTheBHYT: soTheBHYT ?? this.soTheBHYT,
      hanSuDungBHYT: hanSuDungBHYT ?? this.hanSuDungBHYT,
      tuyenKham: tuyenKham ?? this.tuyenKham,
      mucHuongBHYT: mucHuongBHYT ?? this.mucHuongBHYT,
      avatarPath: avatarPath ?? this.avatarPath,
    );
  }
}

// ─── Appointment / Booking models ────────────────────────────────────────────
class TimeSlotModel {
  final int maKhungGio;
  final String tenKhungGio;
  final int conTrong;
  final bool isFull;

  const TimeSlotModel({
    required this.maKhungGio,
    required this.tenKhungGio,
    required this.conTrong,
    required this.isFull,
  });

  factory TimeSlotModel.fromJson(Map<String, dynamic> json) => TimeSlotModel(
        maKhungGio: json['MaKhungGio'] ?? 0,
        tenKhungGio: json['TenKhungGio'] ?? '',
        conTrong: json['ConTrong'] ?? 0,
        isFull: json['IsFull'] ?? false,
      );

  bool isPastForDate(DateTime selectedDate) {
    final now = DateTime.now();
    if (selectedDate.day != now.day ||
        selectedDate.month != now.month ||
        selectedDate.year != now.year) return false;
    // Parse giờ bắt đầu từ "HH:mm - HH:mm"
    try {
      final startStr = tenKhungGio.split('-')[0].trim();
      final parts = startStr.split(':');
      final h = int.parse(parts[0]);
      final m = int.parse(parts[1]);
      return now.hour > h || (now.hour == h && now.minute >= m);
    } catch (_) {
      return false;
    }
  }

  bool isDisabledForDate(DateTime date) => isFull || isPastForDate(date);

  String statusForDate(DateTime date) {
    if (isPastForDate(date)) return 'Đã qua giờ';
    if (isFull) return 'Kín lịch';
    return 'Khả dụng';
  }
}

class ServiceModel {
  final String maDV;
  final String tenDV;
  final double giaDichVu;

  const ServiceModel({
    required this.maDV,
    required this.tenDV,
    required this.giaDichVu,
  });

  factory ServiceModel.fromJson(Map<String, dynamic> json) => ServiceModel(
        maDV: json['MaDV'] ?? '',
        tenDV: _decodeHtmlEntities((json['TenDV'] ?? '').toString()),
        giaDichVu: (json['GiaDichVu'] ?? 0).toDouble(),
      );

  // Parse từ SelectListItem text: "Tên DV - 150,000 VNĐ"
  factory ServiceModel.fromSelectItem(String value, String text) {
    final parts = text.split(' - ');
    final tenDV = _decodeHtmlEntities(
      parts.isNotEmpty ? parts[0].trim() : text,
    );
    double gia = 0;
    if (parts.length > 1) {
      final giaStr = parts[1].replaceAll(RegExp(r'[^\d]'), '');
      gia = double.tryParse(giaStr) ?? 0;
    }
    return ServiceModel(maDV: value, tenDV: tenDV, giaDichVu: gia);
  }

  static String _decodeHtmlEntities(String input) {
    const namedEntities = {
      '&amp;': '&',
      '&lt;': '<',
      '&gt;': '>',
      '&quot;': '"',
      '&#39;': "'",
      '&nbsp;': ' ',
    };

    var output = input;
    namedEntities.forEach((key, value) {
      output = output.replaceAll(key, value);
    });

    output = output.replaceAllMapped(RegExp(r'&#(\d+);'), (match) {
      final codePoint = int.tryParse(match.group(1)!);
      return codePoint == null ? match.group(0)! : String.fromCharCode(codePoint);
    });

    output = output.replaceAllMapped(RegExp(r'&#x([0-9a-fA-F]+);'), (match) {
      final codePoint = int.tryParse(match.group(1)!, radix: 16);
      return codePoint == null ? match.group(0)! : String.fromCharCode(codePoint);
    });

    return output;
  }
}

class LichKhamModel {
  final int maPhieuDK;
  final DateTime? ngayDangKy;
  final String hinhThucDangKy;
  final String trangThai;
  final int? maPhieuKhamBenh;
  final int? stt;
  final String trangThaiKham;
  final DateTime? ngayKham;
  final String lyDoDenKham;
  final String tenPhong;
  final String tenKhoa;
  final String tenBacSi;

  const LichKhamModel({
    required this.maPhieuDK,
    this.ngayDangKy,
    required this.hinhThucDangKy,
    required this.trangThai,
    this.maPhieuKhamBenh,
    this.stt,
    required this.trangThaiKham,
    this.ngayKham,
    required this.lyDoDenKham,
    required this.tenPhong,
    required this.tenKhoa,
    required this.tenBacSi,
  });

  factory LichKhamModel.fromJson(Map<String, dynamic> json) => LichKhamModel(
        maPhieuDK: json['MaPhieuDK'] ?? 0,
        ngayDangKy: json['NgayDangKy'] != null
            ? DateTime.tryParse(json['NgayDangKy'])
            : null,
        hinhThucDangKy: json['HinhThucDangKy'] ?? '',
        trangThai: json['TrangThai'] ?? '',
        maPhieuKhamBenh: json['MaPhieuKhamBenh'],
        stt: json['STT'],
        trangThaiKham: json['TrangThaiKham'] ?? '',
        ngayKham: json['NgayKham'] != null
            ? DateTime.tryParse(json['NgayKham'])
            : null,
        lyDoDenKham: json['LyDoDenKham'] ?? '',
        tenPhong: json['TenPhong'] ?? '',
        tenKhoa: json['TenKhoa'] ?? '',
        tenBacSi: json['TenBacSi'] ?? '',
      );

  Color get statusColor {
    switch (trangThai) {
      case 'Đã xác nhận':
        return const Color(0xFF198754);
      case 'Chờ xử lý':
        return const Color(0xFFFF8C00);
      case 'Hủy':
        return const Color(0xFFDC3545);
      case 'Chờ thanh toán':
        return const Color(0xFF0D6EFD);
      default:
        return const Color(0xFF64748B);
    }
  }
}

// ─── Lịch sử khám ────────────────────────────────────────────────────────────
class LichSuKhamModel {
  final int maPhieuKhamBenh;
  final DateTime ngayKham;
  final String lyDoDenKham;
  final String trieuChung;
  final String ketLuan;
  final String trangThai;
  final String tenPhong;
  final String tenKhoa;
  final String tenBacSi;

  const LichSuKhamModel({
    required this.maPhieuKhamBenh,
    required this.ngayKham,
    required this.lyDoDenKham,
    required this.trieuChung,
    required this.ketLuan,
    required this.trangThai,
    required this.tenPhong,
    required this.tenKhoa,
    required this.tenBacSi,
  });

  factory LichSuKhamModel.fromJson(Map<String, dynamic> json) =>
      LichSuKhamModel(
        maPhieuKhamBenh: json['MaPhieuKhamBenh'] ?? 0,
        ngayKham: json['NgayKham'] != null
            ? DateTime.parse(json['NgayKham'])
            : DateTime.now(),
        lyDoDenKham: json['LyDoDenKham'] ?? '',
        trieuChung: json['TrieuChung'] ?? '',
        ketLuan: json['KetLuan'] ?? '',
        trangThai: json['TrangThai'] ?? '',
        tenPhong: json['TenPhong'] ?? '',
        tenKhoa: json['TenKhoa'] ?? '',
        tenBacSi: json['TenBacSi'] ?? '',
      );
}

// ─── Đơn thuốc ───────────────────────────────────────────────────────────────
class DonThuocModel {
  final int maDonThuoc;
  final DateTime ngayKe;
  final String loiDanBS;
  final String trangThai;
  final DateTime ngayKham;
  final int maPhieuKhamBenh;
  final String tenBacSi;

  const DonThuocModel({
    required this.maDonThuoc,
    required this.ngayKe,
    required this.loiDanBS,
    required this.trangThai,
    required this.ngayKham,
    required this.maPhieuKhamBenh,
    required this.tenBacSi,
  });

  factory DonThuocModel.fromJson(Map<String, dynamic> json) => DonThuocModel(
        maDonThuoc: json['MaDonThuoc'] ?? 0,
        ngayKe: json['NgayKe'] != null
            ? DateTime.parse(json['NgayKe'])
            : DateTime.now(),
        loiDanBS: json['LoiDanBS'] ?? '',
        trangThai: json['TrangThai'] ?? '',
        ngayKham: json['NgayKham'] != null
            ? DateTime.parse(json['NgayKham'])
            : DateTime.now(),
        maPhieuKhamBenh: json['MaPhieuKhamBenh'] ?? 0,
        tenBacSi: json['TenBacSi'] ?? '',
      );

  Color get statusColor {
    switch (trangThai) {
      case 'Đã phát thuốc':
        return const Color(0xFF198754);
      case 'Chưa phát':
        return const Color(0xFFFF8C00);
      case 'Đã phát 1 phần':
        return const Color(0xFF0D6EFD);
      case 'Đã hủy':
        return const Color(0xFFDC3545);
      default:
        return const Color(0xFF64748B);
    }
  }
}

class ChiTietThuocModel {
  final int maCTDonThuoc;
  final String maThuoc;
  final String tenThuoc;
  final double soLuongSang;
  final double soLuongTrua;
  final double soLuongChieu;
  final double soLuongToi;
  final int soNgayDung;
  final int soLuong;
  final String donViTinh;
  final double donGia;
  final String ghiChu;
  final String donViCoBan;

  const ChiTietThuocModel({
    required this.maCTDonThuoc,
    required this.maThuoc,
    required this.tenThuoc,
    required this.soLuongSang,
    required this.soLuongTrua,
    required this.soLuongChieu,
    required this.soLuongToi,
    required this.soNgayDung,
    required this.soLuong,
    required this.donViTinh,
    required this.donGia,
    required this.ghiChu,
    required this.donViCoBan,
  });

  factory ChiTietThuocModel.fromJson(Map<String, dynamic> json) =>
      ChiTietThuocModel(
        maCTDonThuoc: json['MaCTDonThuoc'] ?? 0,
        maThuoc: json['MaThuoc'] ?? '',
        tenThuoc: json['TenThuoc'] ?? '',
        soLuongSang: (json['SoLuongSang'] ?? 0).toDouble(),
        soLuongTrua: (json['SoLuongTrua'] ?? 0).toDouble(),
        soLuongChieu: (json['SoLuongChieu'] ?? 0).toDouble(),
        soLuongToi: (json['SoLuongToi'] ?? 0).toDouble(),
        soNgayDung: json['SoNgayDung'] ?? 0,
        soLuong: json['SoLuong'] ?? 0,
        donViTinh: json['DonViTinh'] ?? '',
        donGia: (json['DonGia'] ?? 0).toDouble(),
        ghiChu: json['GhiChu'] ?? '',
        donViCoBan: json['DonViCoBan'] ?? '',
      );

  // Mô tả cách uống: "Sáng 1 - Trưa 1 - Chiều 0 - Tối 0"
  String get cachUong {
    final parts = <String>[];
    if (soLuongSang > 0) parts.add('Sáng ${_fmt(soLuongSang)}');
    if (soLuongTrua > 0) parts.add('Trưa ${_fmt(soLuongTrua)}');
    if (soLuongChieu > 0) parts.add('Chiều ${_fmt(soLuongChieu)}');
    if (soLuongToi > 0) parts.add('Tối ${_fmt(soLuongToi)}');
    return parts.isEmpty ? 'Theo chỉ định' : parts.join(' - ');
  }

  String _fmt(double v) => v == v.toInt() ? '${v.toInt()}' : '$v';
}

// ─── Hóa đơn ─────────────────────────────────────────────────────────────────
class HoaDonModel {
  final int maHD;
  final DateTime? ngayThanhToan;
  final double tongTien;
  final String trangThaiThanhToan;
  final String hinhThucThanhToan;
  final String ghiChu;
  final int? maPhieuKhamBenh;
  final DateTime? ngayKham;

  const HoaDonModel({
    required this.maHD,
    this.ngayThanhToan,
    required this.tongTien,
    required this.trangThaiThanhToan,
    required this.hinhThucThanhToan,
    required this.ghiChu,
    this.maPhieuKhamBenh,
    this.ngayKham,
  });

  factory HoaDonModel.fromJson(Map<String, dynamic> json) => HoaDonModel(
        maHD: json['MaHD'] ?? 0,
        ngayThanhToan: json['NgayThanhToan'] != null
            ? DateTime.tryParse(json['NgayThanhToan'])
            : null,
        tongTien: (json['TongTien'] ?? 0).toDouble(),
        trangThaiThanhToan: json['TrangThaiThanhToan'] ?? '',
        hinhThucThanhToan: json['HinhThucThanhToan'] ?? '',
        ghiChu: json['GhiChu'] ?? '',
        maPhieuKhamBenh: json['MaPhieuKhamBenh'],
        ngayKham: json['NgayKham'] != null
            ? DateTime.tryParse(json['NgayKham'])
            : null,
      );

  Color get statusColor {
    switch (trangThaiThanhToan) {
      case 'Đã thanh toán':
        return const Color(0xFF198754);
      case 'Chưa thanh toán':
        return const Color(0xFFFF8C00);
      case 'Đã hủy':
        return const Color(0xFFDC3545);
      default:
        return const Color(0xFF64748B);
    }
  }

  bool get isDatLich => ghiChu.contains('đặt lịch') || ghiChu.contains('tiện ích');
}

// ─── API Response wrapper ─────────────────────────────────────────────────────
class ApiResult<T> {
  final bool success;
  final T? data;
  final String? message;

  const ApiResult({required this.success, this.data, this.message});

  factory ApiResult.ok(T data) => ApiResult(success: true, data: data);
  factory ApiResult.fail(String message) =>
      ApiResult(success: false, message: message);
}
