import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../core/models/discovery_models.dart';
import '../../core/services/auth_service.dart';
import '../../core/services/discovery_service.dart';
import '../../core/theme/app_theme.dart';
import '../../shared/widgets/common_widgets.dart';
import '../booking/booking_screen.dart';
import '../doctors/doctors_screen.dart';
import '../guide/guide_screen.dart';
import '../specialties/specialties_screen.dart';

class HomeTab extends StatefulWidget {
  const HomeTab({super.key});

  @override
  State<HomeTab> createState() => _HomeTabState();
}

class _HomeTabState extends State<HomeTab> {
  final DiscoveryService _service = DiscoveryService();
  final PageController _pageController = PageController(viewportFraction: 0.92);

  HomeLandingModel? _homeData;
  bool _isLoading = true;
  String? _error;
  int _currentSlide = 0;

  final List<_BannerData> _banners = const [
    _BannerData(
      title: 'Đặt lịch nhanh\nkhông cần chờ lâu',
      subtitle:
          'Chọn ngày, khung giờ và thanh toán QR ngay trên điện thoại để chủ động thời gian khám.',
      buttonLabel: 'Đặt lịch ngay',
      icon: Icons.calendar_month_rounded,
      gradient: LinearGradient(
        colors: [Color(0xFF0A58CA), Color(0xFF0D6EFD)],
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
      ),
    ),
    _BannerData(
      title: 'Tìm bác sĩ theo\nđúng chuyên khoa',
      subtitle:
          'Xem nhanh đội ngũ bác sĩ, lọc theo chuyên khoa và chọn hướng khám phù hợp.',
      buttonLabel: 'Tìm bác sĩ',
      icon: Icons.person_search_rounded,
      gradient: LinearGradient(
        colors: [Color(0xFF145A32), Color(0xFF198754)],
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
      ),
    ),
    _BannerData(
      title: 'Tra cứu hướng dẫn\ntrước khi đến khám',
      subtitle:
          'Xem quy trình khám, lưu ý xét nghiệm và các câu hỏi thường gặp trên ứng dụng.',
      buttonLabel: 'Xem hướng dẫn',
      icon: Icons.menu_book_rounded,
      gradient: LinearGradient(
        colors: [Color(0xFF052C65), Color(0xFF0D6EFD)],
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
      ),
    ),
  ];

  @override
  void initState() {
    super.initState();
    _bootstrapHome();
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  Future<void> _bootstrapHome() async {
    final cached = await _service.getCachedHomeData();
    if (!mounted) return;

    if (cached != null) {
      setState(() {
        _homeData = cached;
        _isLoading = false;
      });
    }

    await _loadHome(showLoader: cached == null);
  }

  Future<void> _loadHome({bool showLoader = true}) async {
    if (showLoader) {
      setState(() {
        _isLoading = true;
        _error = null;
      });
    } else {
      setState(() => _error = null);
    }

    final result = await _service.getHomeData();
    if (!mounted) return;

    setState(() {
      _isLoading = false;
      if (result.success && result.data != null) {
        _homeData = result.data;
      } else if (_homeData == null) {
        _error = result.message;
      }
    });
  }

  void _openBooking() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => const BookingScreen()),
    );
  }

  void _openDoctors([String? specialty]) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => DoctorsScreen(initialSpecialty: specialty),
      ),
    );
  }

  void _openSpecialties() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => const SpecialtiesScreen()),
    );
  }

  void _openGuide() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => const GuideScreen()),
    );
  }

  @override
  Widget build(BuildContext context) {
    final userName = AuthService().currentUser?.hoTen ?? 'Bạn';

    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      body: RefreshIndicator(
        onRefresh: _loadHome,
        child: CustomScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            SliverAppBar(
              pinned: true,
              backgroundColor: AppTheme.bgLight,
              titleSpacing: 16,
              title: Row(
                children: [
                  const ClinicBrandLogo(
                    size: 52,
                    imagePadding: 4,
                    borderRadius: 18,
                    borderColor: Color(0xFFBFDBFE),
                  ),
                  const SizedBox(width: 12),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'DigiMed Clinic',
                        style: TextStyle(
                          fontSize: 17,
                          color: AppTheme.textDark,
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                      Text(
                        'Xin chao, $userName',
                        style: const TextStyle(
                          fontSize: 12,
                          color: AppTheme.textMuted,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
              actions: [
                IconButton(
                  onPressed: _openGuide,
                  icon: const Icon(Icons.help_outline_rounded),
                ),
              ],
            ),
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 12, 16, 32),
                child: _isLoading
                    ? const Padding(
                        padding: EdgeInsets.only(top: 120),
                        child: Center(child: CircularProgressIndicator()),
                      )
                    : _error != null
                        ? _buildErrorState()
                        : _buildContent(),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildErrorState() {
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(24),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        children: [
          const Icon(Icons.cloud_off_rounded, size: 48, color: AppTheme.danger),
          const SizedBox(height: 12),
          Text(
            _error ?? 'Không tải được dữ liệu trang chủ.',
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 16),
          ElevatedButton(
            onPressed: _loadHome,
            child: const Text('Tải lại'),
          ),
        ],
      ),
    );
  }

  Widget _buildContent() {
    final homeData = _homeData!;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildHeroCarousel(),
        const SizedBox(height: 18),
        _buildQuickActions(),
        const SizedBox(height: 24),
        _buildInfoBanner(),
        const SizedBox(height: 24),
        _sectionHeader(
            'Chuyên khoa nổi bật', 'Khám phá các chuyên khoa đang hoạt động'),
        const SizedBox(height: 12),
        _buildSpecialties(homeData.specialties),
        const SizedBox(height: 24),
        _sectionHeader('Thống kê bệnh viện', 'Dữ liệu tổng quan từ hệ thống'),
        const SizedBox(height: 12),
        _buildStats(homeData.stats),
        const SizedBox(height: 24),
        _sectionHeader('Bác sĩ tiêu biểu',
            'Đội ngũ chuyên gia đang đồng hành cùng người bệnh'),
        const SizedBox(height: 12),
        _buildDoctors(homeData.doctors),
        const SizedBox(height: 24),
        _sectionHeader('Tin tức & cẩm nang',
            'Thông tin mới và nội dung hữu ích trước khi đi khám'),
        const SizedBox(height: 12),
        _buildNews(homeData.news),
        const SizedBox(height: 24),
        _buildBottomCta(),
      ],
    );
  }

  Widget _buildHeroCarousel() {
    return Column(
      children: [
        SizedBox(
          height: 280,
          child: PageView.builder(
            controller: _pageController,
            itemCount: _banners.length,
            onPageChanged: (value) => setState(() => _currentSlide = value),
            itemBuilder: (context, index) {
              final banner = _banners[index];

              return Container(
                margin: const EdgeInsets.symmetric(horizontal: 6),
                padding: const EdgeInsets.all(22),
                decoration: BoxDecoration(
                  gradient: banner.gradient,
                  borderRadius: BorderRadius.circular(28),
                  boxShadow: const [
                    BoxShadow(
                      color: Color(0x1A0F172A),
                      blurRadius: 24,
                      offset: Offset(0, 10),
                    ),
                  ],
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 12, vertical: 8),
                      decoration: BoxDecoration(
                        color: Colors.white.withOpacity(0.16),
                        borderRadius: BorderRadius.circular(999),
                      ),
                      child: const Text(
                        'Bệnh viện đa khoa hiện đại',
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: 12,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      banner.title,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 28,
                        fontWeight: FontWeight.w800,
                        height: 1.15,
                      ),
                    ),
                    const SizedBox(height: 10),
                    Text(
                      banner.subtitle,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 14,
                        height: 1.5,
                      ),
                    ),
                    const Spacer(),
                    ElevatedButton.icon(
                      onPressed: () {
                        switch (index) {
                          case 0:
                            _openBooking();
                            break;
                          case 1:
                            _openDoctors();
                            break;
                          default:
                            _openGuide();
                        }
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.white,
                        foregroundColor: AppTheme.primary,
                      ),
                      icon: Icon(banner.icon),
                      label: Text(banner.buttonLabel),
                    ),
                  ],
                ),
              );
            },
          ),
        ),
        const SizedBox(height: 12),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(
            _banners.length,
            (index) => AnimatedContainer(
              duration: const Duration(milliseconds: 220),
              margin: const EdgeInsets.symmetric(horizontal: 4),
              width: _currentSlide == index ? 24 : 8,
              height: 8,
              decoration: BoxDecoration(
                color: _currentSlide == index
                    ? AppTheme.primary
                    : AppTheme.borderMedium,
                borderRadius: BorderRadius.circular(999),
              ),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildQuickActions() {
    final actions = [
      _QuickAction(
        title: 'Đặt lịch',
        icon: Icons.calendar_month_rounded,
        color: AppTheme.primary,
        onTap: _openBooking,
      ),
      _QuickAction(
        title: 'Tìm bác sĩ',
        icon: Icons.person_search_rounded,
        color: AppTheme.secondary,
        onTap: _openDoctors,
      ),
      _QuickAction(
        title: 'Chuyên khoa',
        icon: Icons.local_hospital_rounded,
        color: const Color(0xFFB45309),
        onTap: _openSpecialties,
      ),
      _QuickAction(
        title: 'Hướng dẫn',
        icon: Icons.menu_book_rounded,
        color: const Color(0xFF7C3AED),
        onTap: _openGuide,
      ),
    ];

    return LayoutBuilder(
      builder: (context, constraints) {
        final isCompactPhone = constraints.maxWidth < 420;
        final crossAxisCount = isCompactPhone ? 2 : 4;
        final mainAxisExtent = isCompactPhone ? 112.0 : 102.0;

        return GridView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          itemCount: actions.length,
          gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: crossAxisCount,
            mainAxisExtent: mainAxisExtent,
            crossAxisSpacing: 12,
            mainAxisSpacing: 12,
          ),
          itemBuilder: (context, index) {
            final action = actions[index];
            return GestureDetector(
              onTap: action.onTap,
              child: Container(
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(22),
                  border: Border.all(color: AppTheme.borderLight),
                ),
                padding:
                    const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Container(
                      width: 44,
                      height: 44,
                      decoration: BoxDecoration(
                        color: action.color.withOpacity(0.12),
                        borderRadius: BorderRadius.circular(14),
                      ),
                      child: Icon(action.icon, color: action.color),
                    ),
                    const SizedBox(height: 10),
                    Text(
                      action.title,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      textAlign: TextAlign.center,
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w700,
                        color: AppTheme.textDark,
                        height: 1.2,
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  Widget _buildInfoBanner() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFFF0FDF4),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: const Color(0xFF86EFAC)),
      ),
      child: const Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(Icons.info_outline_rounded, color: AppTheme.secondary),
          SizedBox(width: 12),
          Expanded(
            child: Text.rich(
              TextSpan(
                style: TextStyle(color: Color(0xFF166534), height: 1.5),
                children: [
                  TextSpan(text: 'Phí đặt lịch '),
                  TextSpan(
                    text: '100.000 VNĐ',
                    style: TextStyle(fontWeight: FontWeight.w800),
                  ),
                  TextSpan(
                    text:
                        ' để giữ chỗ. Khoản phí này tách biệt với viện phí khi đến khám.',
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _sectionHeader(String title, String subtitle) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: const TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.w800,
            color: AppTheme.textDark,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          subtitle,
          style: const TextStyle(
            color: AppTheme.textMuted,
            height: 1.4,
          ),
        ),
      ],
    );
  }

  Widget _buildSpecialties(List<SpecialtyModel> specialties) {
    final displayItems = specialties.take(4).toList();
    final children = <Widget>[
      ...displayItems.map((item) {
        return Container(
          margin: const EdgeInsets.only(bottom: 12),
          padding: const EdgeInsets.all(18),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(22),
            border: Border.all(color: AppTheme.borderLight),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                item.tenKhoa,
                style: const TextStyle(
                  fontSize: 17,
                  fontWeight: FontWeight.w700,
                  color: AppTheme.textDark,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                item.moTa,
                maxLines: 3,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 14),
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => _openDoctors(item.tenKhoa),
                      child: const Text('Xem bác sĩ'),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: ElevatedButton(
                      onPressed: _openBooking,
                      child: const Text('Đặt khám'),
                    ),
                  ),
                ],
              ),
            ],
          ),
        );
      }),
      Align(
        alignment: Alignment.centerLeft,
        child: TextButton.icon(
          onPressed: _openSpecialties,
          icon: const Icon(Icons.arrow_forward_rounded),
          label: const Text('Xem tất cả chuyên khoa'),
        ),
      ),
    ];

    return Column(
      children: children,
    );
  }

  Widget _buildStats(HomeStatsModel stats) {
    final statItems = [
      _StatCardData(
        label: 'Khoa đang hoạt động',
        value: stats.tongSoKhoa.toString(),
        icon: Icons.apartment_rounded,
      ),
      _StatCardData(
        label: 'Phòng chức năng',
        value: stats.tongSoPhong.toString(),
        icon: Icons.meeting_room_rounded,
      ),
      _StatCardData(
        label: 'Nhân viên y tế',
        value: stats.tongSoNhanVien.toString(),
        icon: Icons.groups_rounded,
      ),
      _StatCardData(
        label: 'Lượt khám',
        value: NumberFormat.compact(locale: 'vi').format(stats.tongLuotKham),
        icon: Icons.favorite_rounded,
      ),
    ];

    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: statItems.length,
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        mainAxisExtent: 136,
        mainAxisSpacing: 12,
        crossAxisSpacing: 12,
      ),
      itemBuilder: (context, index) {
        final item = statItems[index];
        return Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            gradient: AppTheme.statsGradient,
            borderRadius: BorderRadius.circular(22),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(item.icon, color: Colors.white),
              const Spacer(),
              Text(
                item.value,
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: 4),
              Text(
                item.label,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 13,
                  height: 1.4,
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildDoctors(List<DoctorModel> doctors) {
    final displayItems = doctors.take(5).toList();
    final children = <Widget>[
      ...displayItems.map((doctor) {
        return Container(
          margin: const EdgeInsets.only(bottom: 12),
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(22),
            border: Border.all(color: AppTheme.borderLight),
          ),
          child: Row(
            children: [
              DoctorAvatar(
                name: doctor.hoTen,
                imagePath: doctor.hinhAnh,
                radius: 30,
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      doctor.hoTen,
                      style: const TextStyle(
                        fontWeight: FontWeight.w700,
                        color: AppTheme.textDark,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      doctor.chucDanh,
                      style: const TextStyle(color: AppTheme.textMuted),
                    ),
                    const SizedBox(height: 8),
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 10, vertical: 6),
                      decoration: BoxDecoration(
                        color: AppTheme.secondary.withOpacity(0.12),
                        borderRadius: BorderRadius.circular(999),
                      ),
                      child: Text(
                        doctor.chuyenKhoa,
                        style: const TextStyle(
                          color: AppTheme.secondary,
                          fontWeight: FontWeight.w700,
                          fontSize: 12,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      }),
      Align(
        alignment: Alignment.centerLeft,
        child: TextButton.icon(
          onPressed: _openDoctors,
          icon: const Icon(Icons.arrow_forward_rounded),
          label: const Text('Xem toàn bộ bác sĩ'),
        ),
      ),
    ];

    return Column(
      children: children,
    );
  }

  Widget _buildNews(List<NewsModel> news) {
    return Column(
      children: news.map((item) {
        final date = item.ngayDang != null
            ? DateFormat('dd/MM/yyyy').format(item.ngayDang!)
            : '';

        return Container(
          margin: const EdgeInsets.only(bottom: 12),
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(22),
            border: Border.all(color: AppTheme.borderLight),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                    decoration: BoxDecoration(
                      color: AppTheme.info.withOpacity(0.12),
                      borderRadius: BorderRadius.circular(999),
                    ),
                    child: Text(
                      item.chuyenMuc,
                      style: const TextStyle(
                        color: AppTheme.primary,
                        fontSize: 12,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  const Spacer(),
                  Text(
                    date,
                    style: const TextStyle(color: AppTheme.textMuted),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Text(
                item.tieuDe,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w700,
                  color: AppTheme.textDark,
                ),
              ),
              const SizedBox(height: 6),
              Text(item.tomTat),
            ],
          ),
        );
      }).toList(),
    );
  }

  Widget _buildBottomCta() {
    return Container(
      padding: const EdgeInsets.all(22),
      decoration: BoxDecoration(
        gradient: AppTheme.heroGradient,
        borderRadius: BorderRadius.circular(28),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Sẵn sàng đặt lịch khám?',
            style: TextStyle(
              color: Colors.white,
              fontSize: 24,
              fontWeight: FontWeight.w800,
            ),
          ),
          const SizedBox(height: 10),
          const Text(
            'Hệ thống đặt lịch trên app giúp bạn chọn ngày khám, khung giờ và theo dõi thanh toán QR ngân hàng thuận tiện hơn.',
            style: TextStyle(color: Colors.white, height: 1.5),
          ),
          const SizedBox(height: 16),
          ElevatedButton.icon(
            onPressed: _openBooking,
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.white,
              foregroundColor: AppTheme.primary,
            ),
            icon: const Icon(Icons.calendar_month_rounded),
            label: const Text('Đặt lịch khám ngay'),
          ),
        ],
      ),
    );
  }
}

class _QuickAction {
  const _QuickAction({
    required this.title,
    required this.icon,
    required this.color,
    required this.onTap,
  });

  final String title;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;
}

class _BannerData {
  const _BannerData({
    required this.title,
    required this.subtitle,
    required this.buttonLabel,
    required this.icon,
    required this.gradient,
  });

  final String title;
  final String subtitle;
  final String buttonLabel;
  final IconData icon;
  final Gradient gradient;
}

class _StatCardData {
  const _StatCardData({
    required this.label,
    required this.value,
    required this.icon,
  });

  final String label;
  final String value;
  final IconData icon;
}
