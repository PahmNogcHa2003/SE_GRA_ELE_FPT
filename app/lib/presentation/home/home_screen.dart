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

  final List<Widget> mainPages = [];

  @override
  void initState() {
    super.initState();
    mainPages.addAll([
      HomeContent(onItemSelected: _handleMenuSelection),
      const StationPage(),
      const NotificationPage(),
      const MorePage(),
    ]);
  }

  void _handleMenuSelection(int index) {
    setState(() => _index = index);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: AnimatedSwitcher(
        duration: const Duration(milliseconds: 300),
        child: mainPages[_index],
      ),
      bottomNavigationBar: BottomNavBar(
        currentIndex: _index,
        onTap: (i) => setState(() => _index = i),
      ),
    );
  }
}
