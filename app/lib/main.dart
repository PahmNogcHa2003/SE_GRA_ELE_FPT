import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:hola_bike_app/presentation/home/pages/notification/notification_page.dart';
import 'package:hola_bike_app/presentation/home/pages/station/station_page.dart';
import 'package:hola_bike_app/presentation/login/login_page.dart';
import 'package:hola_bike_app/presentation/more/page/change_password_page.dart';
import 'package:hola_bike_app/presentation/more/page/profile_page.dart';
import 'package:hola_bike_app/presentation/splash/splash_page.dart';
import 'package:hola_bike_app/presentation/wallet/page/pricing/pricing_page.dart';
import 'package:hola_bike_app/presentation/wallet/walet_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

void main() {
  _setupLoading();
  runApp(const MyApp());
}

void _setupLoading() {
  EasyLoading.instance
    ..indicatorType = EasyLoadingIndicatorType.fadingCircle
    ..animationStyle = EasyLoadingAnimationStyle.scale
    ..loadingStyle = EasyLoadingStyle.custom
    ..backgroundColor = Colors.black87
    ..indicatorColor = Colors.white
    ..textColor = Colors.white
    ..maskColor = Colors.blueGrey
    ..userInteractions = false
    ..dismissOnTap = false
    ..displayDuration = const Duration(milliseconds: 2000);
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'EcoJourney',
      debugShowCheckedModeBanner: false,
      builder: EasyLoading.init(),
      theme: ThemeData(
        fontFamily: 'Arial',
        scaffoldBackgroundColor: AppColors.background,
        colorScheme: ColorScheme.fromSeed(
          seedColor: AppColors.primary,
          primary: AppColors.primary,
          background: AppColors.background,
        ),
        appBarTheme: const AppBarTheme(
          backgroundColor: AppColors.primary,
          foregroundColor: Colors.white,
          elevation: 0,
        ),
        elevatedButtonTheme: ElevatedButtonThemeData(
          style: ElevatedButton.styleFrom(
            backgroundColor: AppColors.primary,
            foregroundColor: Colors.white,
            textStyle: const TextStyle(fontSize: 16),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
          ),
        ),
      ),
      home: const SplashScreen(),
      routes: {
        '/stations': (context) => const StationPage(),
        '/notifications': (context) => const NotificationPage(),
        '/wallet': (context) => const WalletScreen(),
        '/profile': (context) => const ProfilePage(),

        // // Các chức năng tài khoản
        // '/verify': (context) => const VerifyAccountPage(),
        '/change-password': (context) => const ChangePasswordPage(),
        // '/delete-account': (context) => const DeleteAccountPage(),
        // // Các trang thông tin
        '/pricing': (context) => const PricingPage(),
        // '/guide': (context) => const GuidePage(),
        // '/policy': (context) => const PolicyPage(),
        // '/website': (context) => const WebsitePage(),
        // '/support': (context) => const SupportPage(),

        // Trang đăng nhập (nếu có)
        '/login': (context) => const LoginScreen(),
      },
    );
  }
}
