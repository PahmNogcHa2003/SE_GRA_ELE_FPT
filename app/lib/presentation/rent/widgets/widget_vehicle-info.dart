import 'package:flutter/material.dart';
import 'package:hola_bike_app/domain/models/info_scan-vehicle.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class VehicleInfoWidget extends StatelessWidget {
  final InfoScanVehicle info;
  const VehicleInfoWidget({super.key, required this.info});

  Widget _infoRow(IconData icon, String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        children: [
          Icon(icon, color: AppColors.accent),
          const SizedBox(width: 6),
          Text(
            '$label: $value',
            style: const TextStyle(color: AppColors.textSecondary),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _infoRow(Icons.confirmation_number, 'Mã xe', info.bikeCode),
        _infoRow(Icons.directions_bike, 'Loại xe', info.categoryName),
        _infoRow(Icons.electric_bike, 'Tình trạng', info.vehicleStatus),
        _infoRow(Icons.location_on, 'Trạm', info.stationName),
      ],
    );
  }
}
