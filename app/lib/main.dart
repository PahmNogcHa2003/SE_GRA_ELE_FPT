import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:hola_bike_app/presentation/home/pages/notification/notification_page.dart';
import 'package:hola_bike_app/presentation/home/pages/station/station_page.dart';
import 'package:hola_bike_app/presentation/auth/login/login_page.dart';
import 'package:hola_bike_app/presentation/more/page/change_password_page.dart';
import 'package:hola_bike_app/presentation/more/page/profile_page.dart';
import 'package:hola_bike_app/presentation/splash/splash_page.dart';
import 'package:hola_bike_app/presentation/trip/trip_tracking_history_page.dart';
import 'package:hola_bike_app/presentation/trip/trip_tracking_page.dart';
import 'package:hola_bike_app/presentation/wallet/page/pricing/pricing_page.dart';
import 'package:hola_bike_app/presentation/wallet/walet_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

void main() {
  _setupLoading();
  runApp(const MyApp());
}

void _setupLoading() {
  EasyLoading.instance
    ..indicatorType = EasyLoadingIndicatorType.wave
    ..animationStyle = EasyLoadingAnimationStyle.scale
    ..loadingStyle = EasyLoadingStyle.custom
    ..indicatorSize = 60
    ..radius = 12
    ..backgroundColor = Colors
        .transparent // bỏ nền đen của hộp
    ..indicatorColor = Colors.greenAccent
    ..textColor = Colors
        .transparent // ẩn chữ
    // dùng Color.fromRGBO để chắc chắn không lỗi
    ..maskColor =
        const Color.fromRGBO(0, 0, 0, 0.7) // nền mờ xung quanh
    ..maskType = EasyLoadingMaskType
        .custom // rất quan trọng!
    ..userInteractions = false
    ..dismissOnTap = false
    ..boxShadow = []
    ..contentPadding = EdgeInsets.zero
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
        '/trip-tracking': (context) => const TripTrackingPage(isRenting: false),
        '/trip-tracking-history': (context) => const TripTrackingHistoryPage(),
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
        '/login': (context) => const LoginPage(),
      },
    );
  }
}
