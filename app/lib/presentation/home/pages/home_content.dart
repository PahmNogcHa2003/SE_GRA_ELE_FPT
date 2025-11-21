import 'package:flutter/material.dart';
import 'package:hola_bike_app/domain/models/info_user.dart';
import '../widgets/home_header.dart';
import '../widgets/home_menu.dart';
import '../widgets/home_promo.dart';
import '../widgets/home_news.dart';

class HomeContent extends StatelessWidget {
  final Function(int) onItemSelected;
  final UserInfo userInfo;

  const HomeContent({
    super.key,
    required this.onItemSelected,
    required this.userInfo,
  });

  @override
  Widget build(BuildContext context) {
    print(userInfo.fullName); // ✅ In ở đây
    return SafeArea(
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          HomeHeader(name: userInfo.fullName, point: userInfo.walletBalance),
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
