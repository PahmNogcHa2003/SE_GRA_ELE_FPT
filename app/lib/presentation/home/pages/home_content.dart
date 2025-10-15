import 'package:flutter/material.dart';
import '../widgets/home_header.dart';
import '../widgets/home_menu.dart';
import '../widgets/home_promo.dart';
import '../widgets/home_news.dart';

class HomeContent extends StatelessWidget {
  const HomeContent({super.key});

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: ListView(
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
      ),
    );
  }
}
