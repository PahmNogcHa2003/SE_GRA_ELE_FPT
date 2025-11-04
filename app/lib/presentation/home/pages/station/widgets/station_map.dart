import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:geolocator/geolocator.dart';
import 'package:hola_bike_app/core/constants/locations.dart';
import 'package:hola_bike_app/presentation/home/pages/station/widgets/station_card.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:latlong2/latlong.dart';

class StationMap extends StatefulWidget {
  const StationMap({super.key});

  @override
  State<StationMap> createState() => _StationMapState();
}

class _StationMapState extends State<StationMap> {
  final MapController _mapController = MapController();
  final Set<VehicleType> selectedTypes = {
    VehicleType.bike,
    VehicleType.electric,
    VehicleType.car,
  };

  OverlayEntry? _stationOverlay;

  void _showStationOverlay(BuildContext context, Station station) {
    _stationOverlay?.remove();

    _stationOverlay = OverlayEntry(
      builder: (_) => Positioned(
        bottom: 70,
        left: 16,
        right: 16,
        child: Material(
          color: Colors.transparent,
          child: StationCard(
            station: station,
            onClose: () {
              _stationOverlay?.remove();
              _stationOverlay = null;
            },
          ),
        ),
      ),
    );

    Overlay.of(context).insert(_stationOverlay!);
  }

  @override
  Widget build(BuildContext context) {
    final center = AppLocations.stations.first.latLng;
    final filteredStations = AppLocations.stations
        .where((s) => selectedTypes.contains(s.type))
        .toList();

    return Stack(
      children: [
        FlutterMap(
          mapController: _mapController,
          options: MapOptions(
            center: center,
            zoom: 14,
            interactiveFlags: InteractiveFlag.all,
          ),
          children: [
            TileLayer(
              urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
              userAgentPackageName: 'com.example.hola_bike_app',
            ),
            MarkerLayer(
              markers: filteredStations.map((s) {
                return Marker(
                  point: s.latLng,
                  width: 40,
                  height: 40,
                  child: GestureDetector(
                    onTap: () {
                      showDialog(
                        context: context,
                        builder: (_) => StationCard(station: s),
                      );
                    },

                    child: const Icon(
                      Icons.location_pin,
                      color: Colors.red,
                      size: 40,
                    ),
                  ),
                );
              }).toList(),
            ),
          ],
        ),

        // 🔍 Thanh tìm kiếm
        Positioned(
          top: MediaQuery.of(context).padding.top + 12,
          left: 16,
          right: 16,
          child: Container(
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
              boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
            ),
            child: TextField(
              decoration: InputDecoration(
                hintText: 'Tìm kiếm trạm xe...',
                prefixIcon: const Icon(Icons.search, color: AppColors.primary),
                border: InputBorder.none,
                contentPadding: const EdgeInsets.symmetric(vertical: 12),
              ),
              onChanged: (query) {
                // TODO: xử lý tìm kiếm theo tên trạm hoặc vị trí
              },
            ),
          ),
        ),

        // ✅ Bộ lọc loại xe
        Positioned(
          bottom: 16,
          left: 16,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.95),
              borderRadius: BorderRadius.circular(10),
              boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildCheckbox('Xe đạp', VehicleType.bike),
                const SizedBox(height: 6),
                _buildCheckbox('Xe điện', VehicleType.electric),
                const SizedBox(height: 6),
              ],
            ),
          ),
        ),

        // ✅ Nút hỗ trợ
        Positioned(
          bottom: 16,
          right: 16,
          child: Column(
            children: [
              _buildCircleButton(
                Icons.my_location,
                'Vị trí hiện tại',
                () async {
                  LocationPermission permission =
                      await Geolocator.checkPermission();
                  if (permission == LocationPermission.denied) {
                    permission = await Geolocator.requestPermission();
                    if (permission == LocationPermission.denied) return;
                  }

                  final position = await Geolocator.getCurrentPosition();
                  final currentLatLng = LatLng(
                    position.latitude,
                    position.longitude,
                  );

                  _mapController.move(currentLatLng, 16);
                },
              ),
              const SizedBox(height: 12),
              _buildCircleButton(Icons.refresh, 'Làm mới', () {
                setState(() {});
              }),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildCheckbox(String label, VehicleType type) {
    final isChecked = selectedTypes.contains(type);
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        SizedBox(
          width: 18,
          height: 18,
          child: Checkbox(
            value: isChecked,
            onChanged: (_) {
              setState(() {
                if (isChecked) {
                  selectedTypes.remove(type);
                } else {
                  selectedTypes.add(type);
                }
              });
            },
            activeColor: AppColors.primary,
            materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
            visualDensity: VisualDensity.compact,
          ),
        ),
        const SizedBox(width: 6),
        Text(label, style: const TextStyle(fontSize: 12)),
      ],
    );
  }

  Widget _buildCircleButton(IconData icon, String tooltip, VoidCallback onTap) {
    return Tooltip(
      message: tooltip,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(30),
        child: Container(
          width: 48,
          height: 48,
          decoration: BoxDecoration(
            color: AppColors.primary,
            shape: BoxShape.circle,
            boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 6)],
          ),
          child: Icon(icon, color: Colors.white),
        ),
      ),
    );
  }
}
