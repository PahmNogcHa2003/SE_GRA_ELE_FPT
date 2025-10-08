import 'package:flutter/material.dart';
import '../../../theme/app_colors.dart';

class HomeMenu extends StatelessWidget {
  const HomeMenu({super.key});

  @override
  Widget build(BuildContext context) {
    final items = [
      {'icon': Icons.pedal_bike, 'label': 'Trạm xe'},
      {'icon': Icons.confirmation_num_outlined, 'label': 'Mua vé'},
      {'icon': Icons.map_outlined, 'label': 'Chuyến đi'},
      {'icon': Icons.credit_card_outlined, 'label': 'Thẻ của tôi'},
      {'icon': Icons.event_outlined, 'label': 'Thuê xe sự kiện'},
      {'icon': Icons.help_outline, 'label': 'Hướng dẫn'},
      {'icon': Icons.message_outlined, 'label': 'Tin nhắn'},
      {'icon': Icons.leaderboard_outlined, 'label': 'Bảng xếp hạng'},
    ];

    return Container(
      padding: const EdgeInsets.symmetric(vertical: 12),
      decoration: BoxDecoration(
        color: AppColors.card,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withOpacity(0.08),
            blurRadius: 6,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: GridView.builder(
        physics: const NeverScrollableScrollPhysics(),
        shrinkWrap: true,
        padding: const EdgeInsets.symmetric(horizontal: 8),
        itemCount: items.length,
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 4,
          mainAxisSpacing: 8,
          crossAxisSpacing: 8,
          childAspectRatio: 0.85,
        ),
        itemBuilder: (_, i) {
          final item = items[i];
          return Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              CircleAvatar(
                radius: 22,
                backgroundColor: AppColors.primary.withOpacity(0.1),
                child: Icon(
                  item['icon'] as IconData,
                  color: AppColors.primary,
                  size: 22,
                ),
              ),
              const SizedBox(height: 6),
              Text(
                item['label'] as String,
                textAlign: TextAlign.center,
                style: const TextStyle(
                  fontSize: 12,
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          );
        },
      ),
    );
  }
}
