import 'package:flutter/material.dart';
import '../../theme/app_colors.dart';
import 'widgets/home_header.dart';
import 'widgets/home_menu.dart';
import 'widgets/home_promo.dart';
import 'widgets/home_news.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int _index = 0;

  final pages = [
    const HomeContent(),
    const Center(child: Text("🚲 Trạm xe")),
    const Center(child: Text("🔔 Thông báo")),
    const Center(child: Text("☰ Mở rộng")),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: AnimatedSwitcher(
        duration: const Duration(milliseconds: 300),
        child: pages[_index],
      ),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _index,
        onTap: (i) => setState(() => _index = i),
        selectedItemColor: AppColors.primary,
        unselectedItemColor: AppColors.textSecondary,
        type: BottomNavigationBarType.fixed,
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.home), label: "Trang chủ"),
          BottomNavigationBarItem(
            icon: Icon(Icons.pedal_bike),
            label: "Trạm xe",
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.notifications),
            label: "Thông báo",
          ),
          BottomNavigationBarItem(icon: Icon(Icons.menu), label: "Mở rộng"),
        ],
      ),
    );
  }
}

class HomeContent extends StatelessWidget {
  const HomeContent({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.all(16),
      children: const [
        HomeHeader(),
        SizedBox(height: 16),
        HomeMenu(),
        SizedBox(height: 16),
        HomePromo(),
        SizedBox(height: 16),
        HomeNews(),
      ],
    );
  }
}
