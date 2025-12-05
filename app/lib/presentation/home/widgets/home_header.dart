import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/data/sources/remote/api_user.dart';
import 'package:hola_bike_app/presentation/wallet/walet_page.dart';
import '../../../theme/app_colors.dart';

class HomeHeader extends StatefulWidget {
  const HomeHeader({super.key});

  @override
  State<HomeHeader> createState() => _HomeHeaderState();
}

class _HomeHeaderState extends State<HomeHeader> {
  final secureStorage = const FlutterSecureStorage();
  final userApi = UserApi();

  String name = "";
  double point = 0;
  bool isLoading = true;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _loadUserInfo(); // ✅ Tự động reload mỗi khi widget được gọi lại
  }

  Future<void> _loadUserInfo() async {
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) return;

      final info = await userApi.getUserInfo(token);

      setState(() {
        name = info.fullName;
        point = info.walletBalance;
        isLoading = false;
      });
    } catch (e) {
      print("❌ Lỗi load user info: $e");
      setState(() => isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.primary,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  "Xin chào 👋",
                  style: TextStyle(color: Colors.white, fontSize: 20),
                ),
                const SizedBox(height: 8),
                Text(
                  isLoading ? "Đang tải..." : name,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
          ),
          GestureDetector(
            onTap: () async {
              await Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const WalletScreen()),
              );
              _loadUserInfo(); // ✅ Reload sau khi quay lại từ Wallet
            },
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.15),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Row(
                children: [
                  const Icon(
                    Icons.account_balance_wallet,
                    color: Colors.white,
                    size: 20,
                  ),
                  const SizedBox(width: 6),
                  Text(
                    isLoading ? "..." : "$point điểm",
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
