import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../core/models/user_model.dart';
import '../../core/services/auth_service.dart';
import '../../core/services/profile_service.dart';
import '../../core/theme/app_theme.dart';
import '../../shared/widgets/common_widgets.dart';
import '../auth/login_screen.dart';
import '../history/lich_su_kham_screen.dart';
import '../medical/don_thuoc_screen.dart';
import '../medical/hoa_don_screen.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  final _profileService = ProfileService();
  final _authService = AuthService();

  UserModel? _user;
  bool _isLoading = true;
  bool _isEditing = false;
  bool _isSaving = false;
  String? _loadError;

  // Form controllers
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _hoTenCtrl;
  late TextEditingController _sdtCtrl;
  late TextEditingController _emailCtrl;
  late TextEditingController _cccdCtrl;
  late TextEditingController _diaChiCtrl;
  late TextEditingController _soTheBHYTCtrl;

  String? _gioiTinh;
  DateTime? _ngaySinh;
  DateTime? _hanSuDungBHYT;
  bool _bhyt = false;
  String? _tuyenKham;
  int? _mucHuongBHYT;

  @override
  void initState() {
    super.initState();
    _loadProfile();
  }

  @override
  void dispose() {
    _disposeControllers();
    super.dispose();
  }

  void _disposeControllers() {
    _hoTenCtrl.dispose();
    _sdtCtrl.dispose();
    _emailCtrl.dispose();
    _cccdCtrl.dispose();
    _diaChiCtrl.dispose();
    _soTheBHYTCtrl.dispose();
  }

  void _initControllers(UserModel user) {
    _hoTenCtrl = TextEditingController(text: user.hoTen);
    _sdtCtrl = TextEditingController(text: user.sdt ?? '');
    _emailCtrl = TextEditingController(text: user.email ?? '');
    _cccdCtrl = TextEditingController(text: user.cccd ?? '');
    _diaChiCtrl = TextEditingController(text: user.diaChi ?? '');
    _soTheBHYTCtrl = TextEditingController(text: user.soTheBHYT ?? '');
    _gioiTinh = user.gioiTinh;
    _ngaySinh = user.ngaySinh;
    _hanSuDungBHYT = user.hanSuDungBHYT;
    _bhyt = user.bhyt;
    _tuyenKham = user.tuyenKham;
    _mucHuongBHYT = user.mucHuongBHYT;
  }

  // ─── Load ──────────────────────────────────────────────────────────────────
  Future<void> _loadProfile() async {
    setState(() {
      _isLoading = true;
      _loadError = null;
    });

    // Nếu chưa đăng nhập, dùng user từ auth service
    final localUser = _authService.currentUser;
    if (localUser != null) {
      setState(() {
        _user = localUser;
        _isLoading = false;
      });
      _initControllers(localUser);
    }

    // Thử load từ API để có dữ liệu đầy đủ
    final result = await _profileService.getProfile();
    if (mounted && result.success && result.data != null) {
      setState(() {
        _user = result.data;
        _isLoading = false;
      });
      _initControllers(result.data!);
      _authService.updateLocalUser(result.data!);
    } else if (mounted && localUser == null) {
      setState(() {
        _isLoading = false;
        _loadError = result.message;
      });
    }
  }

  // ─── Save ──────────────────────────────────────────────────────────────────
  Future<void> _saveProfile() async {
    if (!_formKey.currentState!.validate()) return;
    if (_user == null) return;

    setState(() => _isSaving = true);

    final updated = _user!.copyWith(
      hoTen: _hoTenCtrl.text.trim(),
      sdt: _sdtCtrl.text.trim(),
      email: _emailCtrl.text.trim(),
      cccd: _cccdCtrl.text.trim(),
      diaChi: _diaChiCtrl.text.trim(),
      gioiTinh: _gioiTinh,
      ngaySinh: _ngaySinh,
      bhyt: _bhyt,
      soTheBHYT: _soTheBHYTCtrl.text.trim(),
      hanSuDungBHYT: _hanSuDungBHYT,
      tuyenKham: _tuyenKham,
      mucHuongBHYT: _mucHuongBHYT,
    );

    final result = await _profileService.updateProfile(updated);
    if (!mounted) return;
    setState(() => _isSaving = false);

    if (result.success) {
      setState(() {
        _user = updated;
        _isEditing = false;
      });
      _authService.updateLocalUser(updated);
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: const Row(
          children: [
            Icon(Icons.check_circle_rounded, color: Colors.white, size: 18),
            SizedBox(width: 8),
            Text('Cập nhật thông tin thành công!',
                style: TextStyle(fontWeight: FontWeight.w600)),
          ],
        ),
        backgroundColor: AppTheme.secondary,
        behavior: SnackBarBehavior.floating,
        shape:
            RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      ));
    } else {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        content: Text(result.message ?? 'Cập nhật thất bại!'),
        backgroundColor: AppTheme.danger,
        behavior: SnackBarBehavior.floating,
        shape:
            RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      ));
    }
  }

  void _cancelEdit() {
    if (_user != null) {
      _initControllers(_user!);
    }
    setState(() => _isEditing = false);
  }

  Future<void> _pickDate(bool isBirthday) async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: isBirthday
          ? (_ngaySinh ?? DateTime(1990))
          : (_hanSuDungBHYT ?? now),
      firstDate: isBirthday ? DateTime(1940) : now,
      lastDate: isBirthday ? now : DateTime(now.year + 10),
      builder: (ctx, child) => Theme(
        data: Theme.of(ctx).copyWith(
          colorScheme: const ColorScheme.light(
            primary: AppTheme.primary,
            onPrimary: Colors.white,
          ),
        ),
        child: child!,
      ),
    );
    if (picked != null) {
      setState(() {
        if (isBirthday) {
          _ngaySinh = picked;
        } else {
          _hanSuDungBHYT = picked;
        }
      });
    }
  }

  void _logout() async {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        shape:
            RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Đăng xuất',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
        content: const Text(
            'Bạn có chắc muốn đăng xuất khỏi ứng dụng MedicHub?'),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Hủy')),
          ElevatedButton(
            onPressed: () async {
              Navigator.pop(context);
              await _authService.logout();
              if (mounted) {
                Navigator.pushAndRemoveUntil(
                  context,
                  MaterialPageRoute(builder: (_) => const LoginScreen()),
                  (_) => false,
                );
              }
            },
            style:
                ElevatedButton.styleFrom(backgroundColor: AppTheme.danger),
            child: const Text('Đăng xuất'),
          ),
        ],
      ),
    );
  }

  String _fmtDate(DateTime? date) =>
      date != null ? DateFormat('dd/MM/yyyy').format(date) : '—';

  // ─── BUILD ─────────────────────────────────────────────────────────────────
  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return const Scaffold(
        backgroundColor: AppTheme.bgLight,
        body: Center(child: CircularProgressIndicator()),
      );
    }

    if (_loadError != null && _user == null) {
      return Scaffold(
        backgroundColor: AppTheme.bgLight,
        appBar: AppBar(title: const Text('Hồ sơ cá nhân')),
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(32),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.error_outline_rounded,
                    size: 64, color: AppTheme.textLight),
                const SizedBox(height: 16),
                Text(_loadError!,
                    textAlign: TextAlign.center,
                    style: const TextStyle(color: AppTheme.textMuted)),
                const SizedBox(height: 16),
                ElevatedButton.icon(
                  onPressed: _loadProfile,
                  icon: const Icon(Icons.refresh_rounded),
                  label: const Text('Thử lại'),
                ),
              ],
            ),
          ),
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      body: CustomScrollView(
        slivers: [
          _buildSliverHeader(),
          SliverToBoxAdapter(
            child: _isEditing && _user != null
                ? _buildEditForm()
                : _buildViewMode(),
          ),
        ],
      ),
    );
  }

  // ─── SliverAppBar header ───────────────────────────────────────────────────
  SliverAppBar _buildSliverHeader() {
    final user = _user;
    return SliverAppBar(
      expandedHeight: 230,
      pinned: true,
      backgroundColor: AppTheme.primary,
      flexibleSpace: FlexibleSpaceBar(
        background: Container(
          decoration: const BoxDecoration(gradient: AppTheme.heroGradient),
          child: SafeArea(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const SizedBox(height: 16),
                Stack(
                  children: [
                    Container(
                      width: 88,
                      height: 88,
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        gradient: AppTheme.statsGradient,
                        border:
                            Border.all(color: Colors.white, width: 3),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.2),
                            blurRadius: 12,
                            offset: const Offset(0, 4),
                          ),
                        ],
                      ),
                      child: Center(
                        child: Text(
                          user?.avatarLetter ?? 'B',
                          style: const TextStyle(
                            color: Colors.white,
                            fontSize: 34,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                      ),
                    ),
                    if (!_isEditing)
                      Positioned(
                        bottom: 0,
                        right: 0,
                        child: GestureDetector(
                          onTap: () => setState(() => _isEditing = true),
                          child: Container(
                            width: 28,
                            height: 28,
                            decoration: const BoxDecoration(
                                color: Colors.white,
                                shape: BoxShape.circle),
                            child: const Icon(Icons.camera_alt_rounded,
                                size: 16, color: AppTheme.primary),
                          ),
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 10),
                Text(
                  user?.hoTen ?? '—',
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 20,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 4),
                if (user?.username.isNotEmpty == true)
                  Container(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 12, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.white.withOpacity(0.2),
                      borderRadius: BorderRadius.circular(50),
                    ),
                    child: Text(
                      '@${user!.username}',
                      style: const TextStyle(
                          color: Colors.white,
                          fontSize: 12,
                          fontWeight: FontWeight.w500),
                    ),
                  ),
                if (user?.maBN.isNotEmpty == true)
                  Text(
                    'Mã BN: ${user!.maBN}',
                    style: const TextStyle(
                        color: Colors.white70, fontSize: 11),
                  ),
              ],
            ),
          ),
        ),
      ),
      actions: [
        if (!_isEditing)
          IconButton(
            tooltip: 'Chỉnh sửa thông tin',
            icon: const Icon(Icons.edit_rounded, color: Colors.white),
            onPressed: () => setState(() => _isEditing = true),
          ),
      ],
    );
  }

  // ─── VIEW MODE ─────────────────────────────────────────────────────────────
  Widget _buildViewMode() {
    final user = _user!;
    return Column(
      children: [
        const SizedBox(height: 20),

        // Quick stats (BHYT)
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: Row(
            children: [
              Expanded(
                child: _statCard(
                  Icons.shield_rounded,
                  'BHYT',
                  user.bhyt ? 'Có thẻ' : 'Không có',
                  user.bhyt ? AppTheme.secondary : AppTheme.textMuted,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _statCard(
                  Icons.percent_rounded,
                  'Mức hưởng',
                  user.mucHuongBHYT != null
                      ? '${user.mucHuongBHYT}%'
                      : '—',
                  AppTheme.primary,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _statCard(
                  Icons.route_rounded,
                  'Tuyến khám',
                  user.tuyenKham == 'Đúng tuyến'
                      ? 'Đúng'
                      : user.tuyenKham == 'Trái tuyến'
                          ? 'Trái'
                          : '—',
                  AppTheme.accent,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 20),

        // Personal info section
        _infoSection(
          'Thông tin cá nhân',
          Icons.person_rounded,
          [
            _infoRow('Họ và tên', user.hoTen),
            _infoRow('Ngày sinh', _fmtDate(user.ngaySinh)),
            _infoRow('Giới tính', user.gioiTinh ?? '—'),
            _infoRow('Số điện thoại', user.sdt ?? '—'),
            _infoRow('Email', user.email ?? '—'),
            _infoRow('CCCD / CMND', user.cccd ?? '—'),
            _infoRow('Địa chỉ', user.diaChi ?? '—', isLast: true),
          ],
        ),

        const SizedBox(height: 12),

        // BHYT section
        _infoSection(
          'Thông tin BHYT',
          Icons.health_and_safety_rounded,
          [
            _infoRowWithWidget(
              'Có thẻ BHYT',
              InfoBadge(
                label: user.bhyt ? 'Có' : 'Không',
                color: user.bhyt ? AppTheme.secondary : AppTheme.textMuted,
                icon: user.bhyt
                    ? Icons.check_circle_rounded
                    : Icons.cancel_rounded,
              ),
            ),
            if (user.bhyt) ...[
              _infoRow('Số thẻ BHYT', user.soTheBHYT ?? '—'),
              _infoRow('Hạn sử dụng', _fmtDate(user.hanSuDungBHYT)),
              _infoRow('Tuyến khám', user.tuyenKham ?? '—'),
              _infoRow('Mức hưởng',
                  user.mucHuongBHYT != null ? '${user.mucHuongBHYT}%' : '—',
                  isLast: true),
            ],
          ],
          iconColor: AppTheme.secondary,
        ),

        const SizedBox(height: 12),

      // Quick activities grid
        _buildActivitiesSection(),

        const SizedBox(height: 12),

        // Settings
        _infoSection(
          'Cài đặt',
          Icons.settings_rounded,
          [
            _settingRow(Icons.edit_rounded, 'Chỉnh sửa thông tin',
                onTap: () => setState(() => _isEditing = true)),
            _settingRow(Icons.lock_outline_rounded, 'Đổi mật khẩu'),
            _settingRow(
                Icons.notifications_outlined, 'Thông báo'),
            _settingRow(Icons.help_outline_rounded,
                'Trợ giúp & Hỗ trợ'),
            _settingRow(Icons.logout_rounded, 'Đăng xuất',
                color: AppTheme.danger,
                onTap: _logout,
                isLast: true),
          ],
          iconColor: AppTheme.textMuted,
        ),

        const SizedBox(height: 32),
      ],
    );
  }

  // ─── Activities quick access ───────────────────────────────────────────────
  Widget _buildActivitiesSection() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Padding(
            padding: EdgeInsets.only(left: 4, bottom: 12),
            child: Text(
              'Hoạt động của tôi',
              style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w700,
                  color: AppTheme.textDark),
            ),
          ),
          Row(
            children: [
              Expanded(
                child: _activityCard(
                  icon: Icons.medical_information_rounded,
                  label: 'Lịch sử\nkhám bệnh',
                  color: const Color(0xFF0EA5E9),
                  bgColor: const Color(0xFFE0F2FE),
                  onTap: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const LichSuKhamScreen(),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _activityCard(
                  icon: Icons.medication_rounded,
                  label: 'Đơn\nthuốc',
                  color: const Color(0xFF10B981),
                  bgColor: const Color(0xFFD1FAE5),
                  onTap: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const DonThuocScreen(),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _activityCard(
                  icon: Icons.receipt_long_rounded,
                  label: 'Hóa\nđơn',
                  color: const Color(0xFFF59E0B),
                  bgColor: const Color(0xFFFEF3C7),
                  onTap: () => Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const HoaDonScreen(),
                    ),
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _activityCard({
    required IconData icon,
    required String label,
    required Color color,
    required Color bgColor,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 16),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: AppTheme.borderLight),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 6,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          children: [
            Container(
              width: 44,
              height: 44,
              decoration: BoxDecoration(
                color: bgColor,
                shape: BoxShape.circle,
              ),
              child: Icon(icon, color: color, size: 22),
            ),
            const SizedBox(height: 8),
            Text(
              label,
              textAlign: TextAlign.center,
              style: const TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.w600,
                  color: AppTheme.textBody,
                  height: 1.3),
            ),
          ],
        ),
      ),
    );
  }

  Widget _statCard(
      IconData icon, String label, String value, Color color) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        children: [
          Icon(icon, color: color, size: 22),
          const SizedBox(height: 6),
          Text(value,
              style: TextStyle(
                  color: color,
                  fontSize: 13,
                  fontWeight: FontWeight.w700)),
          Text(label,
              style: const TextStyle(
                  color: AppTheme.textMuted, fontSize: 10)),
        ],
      ),
    );
  }

  Widget _infoSection(String title, IconData icon, List<Widget> children,
      {Color? iconColor}) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: AppTheme.borderLight),
        ),
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 14, 16, 12),
              child: Row(
                children: [
                  Icon(icon,
                      color: iconColor ?? AppTheme.primary, size: 18),
                  const SizedBox(width: 8),
                  Text(title,
                      style: const TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w700,
                          color: AppTheme.textDark)),
                ],
              ),
            ),
            const Divider(height: 0),
            ...children,
          ],
        ),
      ),
    );
  }

  Widget _infoRow(String label, String value, {bool isLast = false}) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(
              horizontal: 16, vertical: 12),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              SizedBox(
                width: 120,
                child: Text(label,
                    style: const TextStyle(
                        color: AppTheme.textMuted, fontSize: 13)),
              ),
              Expanded(
                child: Text(value,
                    style: const TextStyle(
                        color: AppTheme.textDark,
                        fontSize: 13,
                        fontWeight: FontWeight.w600)),
              ),
            ],
          ),
        ),
        if (!isLast) const Divider(height: 0, indent: 16),
      ],
    );
  }

  Widget _infoRowWithWidget(String label, Widget widget,
      {bool isLast = false}) {
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(
              horizontal: 16, vertical: 12),
          child: Row(
            children: [
              SizedBox(
                width: 120,
                child: Text(label,
                    style: const TextStyle(
                        color: AppTheme.textMuted, fontSize: 13)),
              ),
              widget,
            ],
          ),
        ),
        if (!isLast) const Divider(height: 0, indent: 16),
      ],
    );
  }

  Widget _settingRow(IconData icon, String label,
      {Color? color, bool isLast = false, VoidCallback? onTap}) {
    return Column(
      children: [
        InkWell(
          onTap: onTap ?? () {},
          child: Padding(
            padding: const EdgeInsets.symmetric(
                horizontal: 16, vertical: 14),
            child: Row(
              children: [
                Icon(icon, size: 18, color: color ?? AppTheme.textBody),
                const SizedBox(width: 14),
                Expanded(
                  child: Text(label,
                      style: TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w500,
                          color: color ?? AppTheme.textBody)),
                ),
                Icon(Icons.chevron_right_rounded,
                    color: color?.withOpacity(0.5) ?? AppTheme.textLight,
                    size: 18),
              ],
            ),
          ),
        ),
        if (!isLast)
          const Divider(height: 0, indent: 16 + 18 + 14),
      ],
    );
  }

  // ─── EDIT FORM ──────────────────────────────────────────────────────────────
  Widget _buildEditForm() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Edit header
            Container(
              padding: const EdgeInsets.all(14),
              margin: const EdgeInsets.only(bottom: 20),
              decoration: BoxDecoration(
                color: const Color(0xFFEFF6FF),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: const Color(0xFFBFDBFE)),
              ),
              child: const Row(
                children: [
                  Icon(Icons.edit_note_rounded,
                      color: AppTheme.primary, size: 20),
                  SizedBox(width: 10),
                  Text('Đang chỉnh sửa thông tin hồ sơ',
                      style: TextStyle(
                          color: AppTheme.primary,
                          fontWeight: FontWeight.w600,
                          fontSize: 13)),
                ],
              ),
            ),

            // ── Thông tin cơ bản ─────────────────────────────────────────
            _sectionHeader('Thông tin cơ bản', Icons.person_rounded),
            const SizedBox(height: 14),

            _buildField(
              controller: _hoTenCtrl,
              label: 'Họ và tên *',
              hint: 'Nhập họ tên đầy đủ',
              icon: Icons.person_outline_rounded,
              validator: (v) => (v == null || v.trim().isEmpty)
                  ? 'Vui lòng nhập họ tên'
                  : null,
            ),
            const SizedBox(height: 12),

            Row(
              children: [
                Expanded(
                  child: _buildDropdown(
                    label: 'Giới tính',
                    value: _gioiTinh,
                    items: const ['Nam', 'Nữ'],
                    onChanged: (v) => setState(() => _gioiTinh = v),
                    icon: Icons.wc_rounded,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _buildDateTap(
                    label: 'Ngày sinh',
                    value: _ngaySinh,
                    onTap: () => _pickDate(true),
                    icon: Icons.cake_rounded,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            _buildField(
              controller: _sdtCtrl,
              label: 'Số điện thoại',
              hint: 'Nhập số điện thoại',
              icon: Icons.phone_outlined,
              keyboardType: TextInputType.phone,
            ),
            const SizedBox(height: 12),

            _buildField(
              controller: _emailCtrl,
              label: 'Email',
              hint: 'Nhập địa chỉ email',
              icon: Icons.email_outlined,
              keyboardType: TextInputType.emailAddress,
            ),
            const SizedBox(height: 12),

            _buildField(
              controller: _cccdCtrl,
              label: 'CCCD / CMND',
              hint: '12 số',
              icon: Icons.badge_outlined,
              keyboardType: TextInputType.number,
            ),
            const SizedBox(height: 12),

            _buildField(
              controller: _diaChiCtrl,
              label: 'Địa chỉ',
              hint: 'Địa chỉ thường trú',
              icon: Icons.location_on_outlined,
              maxLines: 2,
            ),

            const SizedBox(height: 24),
            const Divider(),
            const SizedBox(height: 20),

            // ── BHYT ───────────────────────────────────────────────────
            _sectionHeader(
                'Thông tin BHYT',
                Icons.health_and_safety_rounded,
                color: AppTheme.secondary),
            const SizedBox(height: 14),

            Container(
              padding: const EdgeInsets.all(14),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppTheme.borderLight),
              ),
              child: Row(
                children: [
                  const Icon(Icons.shield_rounded,
                      color: AppTheme.secondary, size: 20),
                  const SizedBox(width: 12),
                  const Expanded(
                    child: Text('Tôi có thẻ BHYT',
                        style: TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.w600,
                            color: AppTheme.textDark)),
                  ),
                  Switch.adaptive(
                    value: _bhyt,
                    onChanged: (v) => setState(() => _bhyt = v),
                    activeColor: AppTheme.secondary,
                  ),
                ],
              ),
            ),

            if (_bhyt) ...[
              const SizedBox(height: 12),
              _buildField(
                controller: _soTheBHYTCtrl,
                label: 'Số thẻ BHYT',
                hint: 'VD: DN4030123456789',
                icon: Icons.credit_card_rounded,
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: _buildDateTap(
                      label: 'Hạn sử dụng',
                      value: _hanSuDungBHYT,
                      onTap: () => _pickDate(false),
                      icon: Icons.event_available_rounded,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _buildDropdown(
                      label: 'Tuyến khám',
                      value: _tuyenKham,
                      items: const ['Đúng tuyến', 'Trái tuyến'],
                      onChanged: (v) => setState(() => _tuyenKham = v),
                      icon: Icons.route_rounded,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              _buildDropdown(
                label: 'Mức hưởng BHYT',
                value: _mucHuongBHYT?.toString(),
                items: const ['80', '95', '100'],
                onChanged: (v) => setState(
                    () => _mucHuongBHYT = v != null ? int.tryParse(v) : null),
                icon: Icons.percent_rounded,
              ),
            ],

            const SizedBox(height: 32),

            // Action buttons
            Row(
              children: [
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: _isSaving ? null : _cancelEdit,
                    icon: const Icon(Icons.close_rounded),
                    label: const Text('Hủy'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  flex: 2,
                  child: GradientButton(
                    label: 'Lưu thông tin',
                    icon: Icons.save_rounded,
                    isLoading: _isSaving,
                    onPressed: _isSaving ? null : _saveProfile,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 32),
          ],
        ),
      ),
    );
  }

  Widget _sectionHeader(String title, IconData icon, {Color? color}) {
    return Row(
      children: [
        Icon(icon, color: color ?? AppTheme.primary, size: 18),
        const SizedBox(width: 8),
        Text(title,
            style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w700,
                color: color ?? AppTheme.primary)),
      ],
    );
  }

  Widget _buildField({
    required TextEditingController controller,
    required String label,
    required String hint,
    required IconData icon,
    TextInputType? keyboardType,
    int maxLines = 1,
    String? Function(String?)? validator,
  }) {
    return TextFormField(
      controller: controller,
      keyboardType: keyboardType,
      maxLines: maxLines,
      validator: validator,
      decoration: InputDecoration(
        labelText: label,
        hintText: hint,
        prefixIcon: Icon(icon, size: 18, color: AppTheme.textMuted),
      ),
    );
  }

  Widget _buildDropdown({
    required String label,
    required String? value,
    required List<String> items,
    required ValueChanged<String?> onChanged,
    required IconData icon,
  }) {
    return DropdownButtonFormField<String>(
      value: value,
      onChanged: onChanged,
      decoration: InputDecoration(
        labelText: label,
        prefixIcon: Icon(icon, size: 18, color: AppTheme.textMuted),
      ),
      items: items
          .map((e) => DropdownMenuItem(value: e, child: Text(e)))
          .toList(),
      style:
          const TextStyle(color: AppTheme.textDark, fontSize: 14),
    );
  }

  Widget _buildDateTap({
    required String label,
    required DateTime? value,
    required VoidCallback onTap,
    required IconData icon,
  }) {
    return GestureDetector(
      onTap: onTap,
      child: AbsorbPointer(
        child: TextFormField(
          readOnly: true,
          decoration: InputDecoration(
            labelText: label,
            hintText: 'Chọn ngày',
            prefixIcon: Icon(icon, size: 18, color: AppTheme.textMuted),
            suffixIcon: const Icon(Icons.arrow_drop_down_rounded,
                color: AppTheme.textMuted),
          ),
          controller: TextEditingController(
              text: value != null ? _fmtDate(value) : ''),
        ),
      ),
    );
  }
}
