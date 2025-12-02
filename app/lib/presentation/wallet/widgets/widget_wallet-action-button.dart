import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/wallet/page/pricing/pricing_page.dart';
import 'package:hola_bike_app/presentation/wallet/page/topUp/top_up_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class WalletActionButtons extends StatelessWidget {
  const WalletActionButtons({super.key});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        _ActionButton(
          icon: Icons.attach_money,
          label: "Nạp điểm",
          onTap: () {
            Navigator.push(
              context,
              MaterialPageRoute(builder: (_) => const TopUpPage()),
            );
          },
        ),
        _ActionButton(
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
    );
  }
}

class _ActionButton extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;

  const _ActionButton({
    required this.icon,
    required this.label,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
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
}
