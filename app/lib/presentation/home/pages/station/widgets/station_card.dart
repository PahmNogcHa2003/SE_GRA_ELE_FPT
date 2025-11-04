import 'package:flutter/material.dart';
import 'package:hola_bike_app/core/constants/locations.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class StationCard extends StatelessWidget {
  final Station station;
  final VoidCallback? onClose;

  const StationCard({super.key, required this.station, this.onClose});

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      insetPadding: const EdgeInsets.symmetric(horizontal: 24, vertical: 24),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 🔘 Nút đóng
            Align(
              alignment: Alignment.topRight,
              child: GestureDetector(
                onTap: onClose ?? () => Navigator.of(context).pop(),
                child: const Icon(Icons.close, size: 20),
              ),
            ),
            const SizedBox(height: 8),

            // 🏷️ Tên trạm
            Text(
              station.name,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppColors.textPrimary,
              ),
            ),
            const SizedBox(height: 12),

            // 🚲 Thông tin chi tiết
            Text("Loại xe: ${station.type.name}"),
            Text("Sẵn có: ${station.available} xe"),
            Text("Pin: ${station.batteryStatus}"),
          ],
        ),
      ),
    );
  }
}
