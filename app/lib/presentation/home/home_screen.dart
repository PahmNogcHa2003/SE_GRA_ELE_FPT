import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/domain/models/info_user.dart';
import 'package:hola_bike_app/data/sources/remote/api_user.dart';
import 'package:hola_bike_app/presentation/home/pages/qr/qr_page.dart';
import 'package:hola_bike_app/presentation/navigation/bottom_nav.dart';
import 'pages/home_content.dart';
import 'pages/more/more_page.dart';
import 'pages/notification/notification_page.dart';
import 'pages/station/station_page.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final secureStorage = const FlutterSecureStorage();
  int _index = 0;
  UserInfo? userInfo;
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadUserInfo();
  }

  Future<void> _loadUserInfo() async {
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) {
        throw Exception('Không tìm thấy access token');
      }

      print('🔹 Gọi getUserInfo với token: $token');
      final api = UserApi();
      final info = await api.getUserInfo(token);

      setState(() {
        userInfo = info;
        isLoading = false;
      });
    } catch (e) {
      print('❌ Lỗi khi gọi getUserInfo: $e');
      setState(() => isLoading = false);
    }
  }

  void _handleMenuSelection(int index) {
    setState(() => _index = index);
  }

  void _handleQrTap() {
    Navigator.push(context, MaterialPageRoute(builder: (_) => const QrPage()));
  }

  @override
  Widget build(BuildContext context) {
    if (isLoading) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    if (userInfo == null) {
      return const Scaffold(
        body: Center(child: Text('Không thể tải thông tin người dùng')),
      );
    }

    final mainPages = [
      HomeContent(onItemSelected: _handleMenuSelection, userInfo: userInfo!),
      const StationPage(),
      const NotificationPage(),
      const MorePage(),
    ];

    return Scaffold(
      body: AnimatedSwitcher(
        duration: const Duration(milliseconds: 300),
        child: (_index >= 0 && _index < mainPages.length)
            ? mainPages[_index]
            : const SizedBox.shrink(),
      ),
      bottomNavigationBar: BottomNavBar(
        currentIndex: _index,
        onTap: (i) => setState(() => _index = i),
        onQrTap: _handleQrTap,
      ),
    );
  }
}
