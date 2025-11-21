import 'package:flutter/material.dart';
import 'package:hola_bike_app/domain/models/info_wallet.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class WalletBalanceCard extends StatelessWidget {
  final WalletInfo? walletInfo;

  const WalletBalanceCard({super.key, required this.walletInfo});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.primary,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: _BalanceRow(
              label: "Tài khoản chính",
              point: walletInfo?.balance ?? 0,
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
            child: _BalanceRow(
              label: "Nợ cước",
              point: walletInfo?.totalDebt ?? 0,
              icon: Icons.receipt_long,
              subtitle: "Thanh toán trước 11/11/2025",
            ),
          ),
        ],
      ),
    );
  }
}

class _BalanceRow extends StatelessWidget {
  final String label;
  final double point;
  final IconData icon;
  final String? subtitle;

  const _BalanceRow({
    required this.label,
    required this.point,
    required this.icon,
    this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
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
                "${point.toStringAsFixed(0)} điểm",
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                ),
              ),
              if (subtitle != null)
                Text(
                  subtitle!,
                  style: const TextStyle(color: Colors.white60, fontSize: 11),
                ),
            ],
          ),
        ),
      ],
    );
  }
}
