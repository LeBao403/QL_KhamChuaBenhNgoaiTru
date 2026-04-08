import 'package:flutter/material.dart';

import '../../core/services/auth_service.dart';
import '../../core/theme/app_theme.dart';
import '../auth/login_screen.dart';
import '../guide/guide_screen.dart';

class SettingsScreen extends StatelessWidget {
  const SettingsScreen({super.key});

  Future<void> _logout(BuildContext context) async {
    final shouldLogout = await showDialog<bool>(
          context: context,
          builder: (context) => AlertDialog(
            title: const Text('Đăng xuất'),
            content: const Text('Bạn có chắc muốn đăng xuất khỏi ứng dụng?'),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context, false),
                child: const Text('Hủy'),
              ),
              ElevatedButton(
                onPressed: () => Navigator.pop(context, true),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.danger,
                ),
                child: const Text('Đăng xuất'),
              ),
            ],
          ),
        ) ??
        false;

    if (!shouldLogout || !context.mounted) return;

    await AuthService().logout();
    if (!context.mounted) return;

    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (_) => const LoginScreen()),
      (_) => false,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      appBar: AppBar(title: const Text('Cài đặt')),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
        children: [
          Container(
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              gradient: AppTheme.heroGradient,
              borderRadius: BorderRadius.circular(24),
            ),
            child: const Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Thiết lập ứng dụng',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 24,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                SizedBox(height: 10),
                Text(
                  'Quản lý các tùy chọn hỗ trợ, thông tin liên hệ và thao tác tài khoản ngay trên điện thoại.',
                  style: TextStyle(color: Colors.white, height: 1.5),
                ),
              ],
            ),
          ),
          const SizedBox(height: 18),
          _group(
            title: 'Tài khoản',
            children: [
              _tile(
                icon: Icons.lock_outline_rounded,
                title: 'Đổi mật khẩu',
                subtitle: 'Sẽ mở rộng theo luồng web ở bước tiếp theo',
              ),
              _tile(
                icon: Icons.notifications_none_rounded,
                title: 'Thông báo',
                subtitle: 'Quản lý nhắc lịch, cập nhật trạng thái khám',
              ),
            ],
          ),
          const SizedBox(height: 16),
          _group(
            title: 'Hỗ trợ',
            children: [
              _tile(
                icon: Icons.menu_book_rounded,
                title: 'Hướng dẫn sử dụng',
                subtitle: 'Xem quy trình khám và câu hỏi thường gặp',
                onTap: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (_) => const GuideScreen()),
                  );
                },
              ),
              _tile(
                icon: Icons.phone_in_talk_rounded,
                title: 'Liên hệ tư vấn',
                subtitle: 'Tổng đài hỗ trợ: 1900 6789',
              ),
              _tile(
                icon: Icons.info_outline_rounded,
                title: 'Về ứng dụng',
                subtitle: 'MedicHub dành cho đặt lịch và theo dõi hồ sơ khám',
              ),
            ],
          ),
          const SizedBox(height: 16),
          _group(
            title: 'Phiên đăng nhập',
            children: [
              _tile(
                icon: Icons.logout_rounded,
                title: 'Đăng xuất',
                subtitle: 'Kết thúc phiên đăng nhập hiện tại',
                iconColor: AppTheme.danger,
                textColor: AppTheme.danger,
                onTap: () => _logout(context),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _group({required String title, required List<Widget> children}) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.only(left: 4, bottom: 10),
          child: Text(
            title,
            style: const TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w800,
              color: AppTheme.textDark,
            ),
          ),
        ),
        Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            border: Border.all(color: AppTheme.borderLight),
          ),
          child: Column(children: children),
        ),
      ],
    );
  }

  Widget _tile({
    required IconData icon,
    required String title,
    required String subtitle,
    Color iconColor = AppTheme.primary,
    Color textColor = AppTheme.textDark,
    VoidCallback? onTap,
  }) {
    return ListTile(
      onTap: onTap,
      leading: Container(
        width: 42,
        height: 42,
        decoration: BoxDecoration(
          color: iconColor.withOpacity(0.12),
          borderRadius: BorderRadius.circular(14),
        ),
        child: Icon(icon, color: iconColor),
      ),
      title: Text(
        title,
        style: TextStyle(
          fontWeight: FontWeight.w700,
          color: textColor,
        ),
      ),
      subtitle: Text(subtitle),
      trailing: const Icon(Icons.chevron_right_rounded),
    );
  }
}
