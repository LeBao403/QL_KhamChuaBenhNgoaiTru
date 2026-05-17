import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'core/services/auth_service.dart';
import 'core/theme/app_theme.dart';
import 'features/auth/login_screen.dart';
import 'features/shell/main_shell.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

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

  runApp(const MedicHubApp());
}

class MedicHubApp extends StatefulWidget {
  const MedicHubApp({super.key});

  @override
  State<MedicHubApp> createState() => _MedicHubAppState();
}

class _MedicHubAppState extends State<MedicHubApp> {
  late final Future<void> _bootstrap = _initApp();

  Future<void> _initApp() async {
    await initializeDateFormatting('vi_VN', null);
    await AuthService().init().timeout(const Duration(seconds: 4));
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'DigiMed Clinic',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.theme,
      routes: {
        '/home': (_) => const MainShell(),
        '/login': (_) => const LoginScreen(),
      },
      home: FutureBuilder<void>(
        future: _bootstrap,
        builder: (context, snapshot) {
          if (snapshot.connectionState != ConnectionState.done) {
            return const _AppStartupScreen();
          }

          return AuthService().isLoggedIn
              ? const MainShell()
              : const LoginScreen();
        },
      ),
    );
  }
}

class _AppStartupScreen extends StatelessWidget {
  const _AppStartupScreen();

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      backgroundColor: AppTheme.bgLight,
      body: Center(
        child: CircularProgressIndicator(),
      ),
    );
  }
}
