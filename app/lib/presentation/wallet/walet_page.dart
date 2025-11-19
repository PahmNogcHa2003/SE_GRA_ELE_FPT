import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_get-wallet-info.dart';
import 'package:hola_bike_app/presentation/wallet/widgets/widget_rectent-transaction.dart';
import 'package:hola_bike_app/presentation/wallet/widgets/widget_wallet-action-button.dart';
import 'package:hola_bike_app/presentation/wallet/widgets/widget_wallet-balance-card.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:hola_bike_app/domain/models/info_wallet.dart';

class WalletScreen extends StatefulWidget {
  const WalletScreen({super.key});

  @override
  State<WalletScreen> createState() => _WalletScreenState();
}

class _WalletScreenState extends State<WalletScreen> {
  WalletInfo? _walletInfo;
  final secureStorage = const FlutterSecureStorage();
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _fetchWalletInfo();
  }

  Future<void> _fetchWalletInfo() async {
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) {
        throw Exception('Không tìm thấy access token');
      }
      WalletInfo info = await GetWalletInfoUseCase().execute(token);
      setState(() {
        _walletInfo = info;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      debugPrint("Lỗi lấy thông tin ví: $e");
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Ví của tôi"),
        backgroundColor: AppColors.primary,
        elevation: 0,
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  WalletBalanceCard(walletInfo: _walletInfo),
                  const SizedBox(height: 24),
                  WalletActionButtons(),
                  const SizedBox(height: 32),
                  const RecentTransactions(),
                ],
              ),
            ),
    );
  }
}
