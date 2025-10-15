import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class StationStatusCard extends StatelessWidget {
  const StationStatusCard({super.key});

  @override
  Widget build(BuildContext context) {
    final stations = [
      {
        'name': 'Trạm Nguyễn Trãi',
        'type': 'Xe đạp',
        'available': 12,
        'battery': 'Không dùng pin',
        'icon': Icons.pedal_bike,
      },
      {
        'name': 'Trạm Thanh Xuân',
        'type': 'Xe điện',
        'available': 5,
        'battery': 'Pin 80%',
        'icon': Icons.electric_bike,
      },
      {
        'name': 'Trạm Hà Đông',
        'type': 'Xe ô tô',
        'available': 3,
        'battery': 'Pin 65%',
        'icon': Icons.directions_car,
      },
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Padding(
          padding: EdgeInsets.symmetric(horizontal: 16),
          child: Text(
            "🔋 Trạng thái trạm sạc",
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimary,
            ),
          ),
        ),
        const SizedBox(height: 12),
        SizedBox(
          height: 140,
          child: ListView.separated(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            scrollDirection: Axis.horizontal,
            itemCount: stations.length,
            separatorBuilder: (_, __) => const SizedBox(width: 12),
            itemBuilder: (_, i) {
              final item = stations[i];
              return Container(
                width: 220,
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
                child: Padding(
                  padding: const EdgeInsets.all(12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Icon(
                            item['icon'] as IconData,
                            color: AppColors.primary,
                          ),
                          const SizedBox(width: 8),
                          Expanded(
                            child: Text(
                              item['name'] as String? ?? '',
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.bold,
                              ),
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 8),
                      Text("Loại xe: ${item['type']}"),
                      Text("Sẵn có: ${item['available']} xe"),
                      Text("Pin: ${item['battery']}"),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}
