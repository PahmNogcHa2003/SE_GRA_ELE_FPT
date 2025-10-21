import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/wallet/page/pricing/pricing_page.dart';
import 'package:hola_bike_app/presentation/wallet/page/topUp/top_up_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class WalletScreen extends StatelessWidget {
  const WalletScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Ví của tôi"),
        backgroundColor: AppColors.primary,
        elevation: 0,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 🔷 Số dư tài khoản
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: AppColors.primary,
                borderRadius: BorderRadius.circular(16),
              ),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: _buildBalanceRow(
                      label: "Tài khoản chính",
                      point: 0,
                      icon: Icons.account_balance_wallet,
                    ),
                  ),
                  Container(
                    width: 1,
                    height: 60,
                    color: Colors.white,
                    margin: const EdgeInsets.symmetric(horizontal: 12),
                  ),
                  Expanded(
                    child: _buildBalanceRow(
                      label: "Nợ cước",
                      point: 0,
                      icon: Icons.receipt_long,
                      subtitle: "Thanh toán trước 11/11/2025",
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 24),

            // 🔘 Các nút chức năng
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                _buildActionButton(
                  icon: Icons.attach_money,
                  label: "Nạp điểm",
                  onTap: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(builder: (_) => const TopUpPage()),
                    );
                  },
                ),
                _buildActionButton(
                  icon: Icons.info_outline,
                  label: "Bảng giá",
                  onTap: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(builder: (_) => const PricingPage()),
                    );
                  },
                ),
              ],
            ),

            const SizedBox(height: 32),

            // 📁 Giao dịch gần đây
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: const [
                Text(
                  "Giao dịch gần đây",
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                ),
                Text(
                  "Xem tất cả",
                  style: TextStyle(
                    fontSize: 13,
                    color: AppColors.primary,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            _buildEmptyTransaction(),
          ],
        ),
      ),
    );
  }

  Widget _buildBalanceRow({
    required String label,
    required int point,
    required IconData icon,
    String? subtitle,
  }) {
    return Row(
      children: [
        Icon(icon, color: Colors.white, size: 20),
        const SizedBox(width: 8),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: const TextStyle(color: Colors.white70, fontSize: 13),
              ),
              Text(
                "$point điểm",
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                ),
              ),
              if (subtitle != null)
                Text(
                  subtitle,
                  style: const TextStyle(color: Colors.white60, fontSize: 11),
                ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required String label,
    required VoidCallback onTap,
  }) {
    return Expanded(
      child: GestureDetector(
        onTap: onTap,
        child: Column(
          children: [
            CircleAvatar(
              radius: 24,
              backgroundColor: AppColors.primary.withOpacity(0.1),
              child: Icon(icon, color: AppColors.primary),
            ),
            const SizedBox(height: 6),
            Text(
              label,
              style: const TextStyle(fontSize: 12),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildEmptyTransaction() {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 32),
      alignment: Alignment.center,
      child: Column(
        children: const [
          Icon(Icons.folder_open, size: 48, color: Colors.grey),
          SizedBox(height: 12),
          Text("Chưa có dữ liệu", style: TextStyle(color: Colors.grey)),
        ],
      ),
    );
  }
}
