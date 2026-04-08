import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'core/services/auth_service.dart';
import 'core/theme/app_theme.dart';
import 'features/auth/login_screen.dart';
import 'features/shell/main_shell.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await initializeDateFormatting('vi_VN', null);

  SystemChrome.setSystemUIOverlayStyle(
    const SystemUiOverlayStyle(
      statusBarColor: Colors.transparent,
      statusBarIconBrightness: Brightness.dark,
    ),
  );
  await SystemChrome.setPreferredOrientations([
    DeviceOrientation.portraitUp,
    DeviceOrientation.portraitDown,
  ]);

  // Khởi tạo AuthService (load cookie + user từ SharedPreferences)
  await AuthService().init();

  runApp(const MedicHubApp());
}

class MedicHubApp extends StatelessWidget {
  const MedicHubApp({super.key});

  @override
  Widget build(BuildContext context) {
    final isLoggedIn = AuthService().isLoggedIn;

    return MaterialApp(
      title: 'MedicHub',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.theme,
      routes: {
        '/home': (_) => const MainShell(),
        '/login': (_) => const LoginScreen(),
      },
      // Nếu đã đăng nhập → vào MainShell, chưa → LoginScreen
      home: isLoggedIn ? const MainShell() : const LoginScreen(),
    );
  }
}
