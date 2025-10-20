import 'package:flutter/material.dart';
import '../widgets/home_header.dart';
import '../widgets/home_menu.dart';
import '../widgets/home_promo.dart';
import '../widgets/home_news.dart';

class HomeContent extends StatelessWidget {
  final Function(int) onItemSelected;

  const HomeContent({super.key, required this.onItemSelected});

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          HomeHeader(point: 2000),
          const SizedBox(height: 16),
          HomeMenu(onItemSelected: onItemSelected),
          const SizedBox(height: 16),
          const HomePromo(),
          const SizedBox(height: 16),
          const HomeNews(),
        ],
      ),
    );
  }
}
