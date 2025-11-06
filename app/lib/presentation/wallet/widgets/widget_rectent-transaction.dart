import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class RecentTransactions extends StatelessWidget {
  const RecentTransactions({super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
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
        Container(
          padding: const EdgeInsets.symmetric(vertical: 32),
          alignment: Alignment.center,
          child: Column(
            children: const [
              Icon(Icons.folder_open, size: 48, color: Colors.grey),
              SizedBox(height: 12),
              Text("Chưa có dữ liệu", style: TextStyle(color: Colors.grey)),
            ],
          ),
        ),
      ],
    );
  }
}
