import 'package:flutter/material.dart';
import 'package:hola_bike_app/core/constants/locations.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class StationCard extends StatelessWidget {
  const StationCard({super.key});

  @override
  Widget build(BuildContext context) {
    final station = AppLocations.stations.first;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppColors.card,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.08),
            blurRadius: 6,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            station.name,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimary,
            ),
          ),
          const SizedBox(height: 4),
          Text("Loại xe: ${station.type}"),
          Text("Sẵn có: ${station.available} xe"),
          Text("Pin: ${station.batteryStatus}"),
        ],
      ),
    );
  }
}
