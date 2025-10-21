import 'package:flutter/material.dart';
import '../../../theme/app_colors.dart';

class HomeMenu extends StatelessWidget {
  final Function(int) onItemSelected;

  const HomeMenu({super.key, required this.onItemSelected});

  @override
  Widget build(BuildContext context) {
    final items = [
      {'icon': Icons.pedal_bike, 'label': 'Trạm xe', 'index': 1},
      {
        'icon': Icons.confirmation_num_outlined,
        'label': 'Mua vé',
        'route': '/tickets',
      },
      {'icon': Icons.map_outlined, 'label': 'Chuyến đi', 'route': '/jouney'},
      {'icon': Icons.help_outline, 'label': 'Hướng dẫn', 'route': '/guide'},
      {
        'icon': Icons.message_outlined,
        'label': 'Tin nhắn',
        'route': '/messages',
      },
      {'icon': Icons.newspaper, 'label': 'Tin tức', 'route': '/news'},
      {
        'icon': Icons.personal_injury_outlined,
        'label': 'Hồ sơ',
        'route': '/profile',
      },
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
          return InkWell(
            onTap: () {
              if (item.containsKey('index')) {
                onItemSelected(item['index'] as int);
              } else if (item.containsKey('route')) {
                Navigator.pushNamed(context, item['route'] as String);
              }
            },
            child: Column(
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
            ),
          );
        },
      ),
    );
  }
}
