import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:hola_bike_app/presentation/home/pages/notification/notification_page.dart';
import 'package:hola_bike_app/presentation/home/pages/station/station_page.dart';
import 'package:hola_bike_app/presentation/auth/login/login_page.dart';
import 'package:hola_bike_app/presentation/more/page/change_password_page.dart';
import 'package:hola_bike_app/presentation/more/page/kyc/kyc_page.dart';
import 'package:hola_bike_app/presentation/more/page/profile_page.dart';
import 'package:hola_bike_app/presentation/ranking/leaderboard_page.dart';
import 'package:hola_bike_app/presentation/splash/splash_page.dart';
import 'package:hola_bike_app/presentation/tickets/my_ticket_page.dart';
import 'package:hola_bike_app/presentation/tickets/ticket_page.dart';
import 'package:hola_bike_app/presentation/trip/trip_tracking_history_page.dart';
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
    ..backgroundColor = Colors.transparent
    ..indicatorColor = AppColors.primary
    ..textColor = Colors.transparent
    ..maskColor = const Color.fromRGBO(0, 0, 0, 0.7)
    ..maskType = EasyLoadingMaskType.custom
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
        '/tickets': (context) => const TicketPage(),
        '/my-ticket': (context) => const MyTicketPage(),
        '/ranking': (context) => const LeaderboardPage(),
        '/trip-tracking-history': (context) => const TripTrackingHistoryPage(),

        '/verify': (context) => const KycPage(),
        '/change-password': (context) => const ChangePasswordPage(),

        '/pricing': (context) => const PricingPage(),

        '/login': (context) => const LoginPage(),
      },
    );
  }
}
