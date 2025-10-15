import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/navigation/bottom_nav.dart';
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

  final Map<int, Widget> pages = {
    0: const HomeContent(),
    1: const StationPage(),
    99: const QrPage(),
    2: const NotificationPage(),
    3: const MorePage(),
  };

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: AnimatedSwitcher(
        duration: const Duration(milliseconds: 300),
        child: pages[_index] ?? const SizedBox(),
      ),
      bottomNavigationBar: BottomNavBar(
        currentIndex: _index,
        onTap: (i) => setState(() => _index = i),
      ),
    );
  }
}
