import 'package:flutter/material.dart';
import '../../theme/app_colors.dart';
import '../navigation/bottom_nav.dart';
import 'pages/home_content.dart';
import 'pages/more/more_page.dart';
import 'pages/notification/notification_page.dart';
import 'pages/qr/qr_page.dart';
import 'pages/station/station_page.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int _index = 0;

  final List<Widget> pages = const [
    HomeContent(), // index 0
    StationPage(), // index 1
    NotificationPage(), // index 2
    MorePage(), // index 3
  ];

  final Widget qrPage = const QrPage(); // index đặc biệt

  @override
  Widget build(BuildContext context) {
    final bool isQrPage = _index == 99;

    return Scaffold(
      body: AnimatedSwitcher(
        duration: const Duration(milliseconds: 300),
        child: isQrPage ? qrPage : pages[_index],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => setState(() => _index = 99),
        backgroundColor: AppColors.primary,
        elevation: 4,
        child: const Icon(Icons.qr_code_scanner, size: 28, color: Colors.white),
      ),
      floatingActionButtonLocation: FloatingActionButtonLocation.centerDocked,
      bottomNavigationBar: BottomNavBar(
        currentIndex: isQrPage ? -1 : _index,
        onTap: (i) => setState(() => _index = i),
      ),
    );
  }
}
