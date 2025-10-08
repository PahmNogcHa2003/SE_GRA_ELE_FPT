import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:http/http.dart' as http;
import 'package:latlong2/latlong.dart';
import '../../core/constants/locations.dart';
import '../widgets/station_info_dialog.dart';

class MapBottomSheet extends StatefulWidget {
  final int? startId;
  final int? endId;
  final void Function(String, String, String) onRouteCalculated;

  const MapBottomSheet({
    super.key,
    required this.startId,
    required this.endId,
    required this.onRouteCalculated,
  });

  @override
  State<MapBottomSheet> createState() => _MapBottomSheetState();
}

class _MapBottomSheetState extends State<MapBottomSheet> {
  List<Polyline> _polylines = [];
  String distance = '';
  String duration = '';
  String speed = '';

  @override
  void initState() {
    super.initState();
    if (widget.startId != null && widget.endId != null) {
      final start = AppLocations.getById(widget.startId!)!;
      final end = AppLocations.getById(widget.endId!)!;
      _getRoute(start.latLng, end.latLng);
    }
  }

  Future<void> _getRoute(LatLng start, LatLng end) async {
    final url =
        "https://router.project-osrm.org/route/v1/bicycle/${start.longitude},${start.latitude};${end.longitude},${end.latitude}?overview=full&geometries=geojson";

    final resp = await http.get(Uri.parse(url));
    if (resp.statusCode != 200) return;

    final data = jsonDecode(resp.body);
    final route = data['routes'][0];
    final distKm = (route['distance'] as num) / 1000.0;

    const averageBikeSpeed = 15.0; // km/h – tốc độ trung bình xe đạp
    final durMin = distKm / averageBikeSpeed * 60.0; // thời gian dự kiến (phút)
    final avgKmH = averageBikeSpeed; // tốc độ cố định

    final coords = route['geometry']['coordinates'] as List;
    final polyPoints = coords
        .map((c) => LatLng((c[1] as num).toDouble(), (c[0] as num).toDouble()))
        .toList();

    setState(() {
      _polylines = [
        Polyline(points: polyPoints, strokeWidth: 4, color: Colors.blue),
      ];
      distance = '${distKm.toStringAsFixed(2)} km';
      duration = '${durMin.toStringAsFixed(0)} phút';
      speed = '${avgKmH.toStringAsFixed(2)} km/h';
    });

    widget.onRouteCalculated(distance, duration, speed);
  }

  @override
  Widget build(BuildContext context) {
    final center = AppLocations.stations.first.latLng;

    return Scaffold(
      body: Stack(
        children: [
          FlutterMap(
            options: MapOptions(
              center: center,
              zoom: 14,
              minZoom: 3,
              maxZoom: 18,
              interactiveFlags: InteractiveFlag.all,
            ),
            children: [
              TileLayer(
                urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                userAgentPackageName: 'com.example.hola_bike_app',
              ),
              PolylineLayer(polylines: _polylines),
              MarkerLayer(
                markers: AppLocations.stations.map((s) {
                  return Marker(
                    point: s.latLng,
                    width: 40,
                    height: 40,
                    child: GestureDetector(
                      onTap: () {
                        showDialog(
                          context: context,
                          builder: (_) => StationInfoDialog(station: s),
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

          // Nút đóng hoặc quay lại
          Positioned(
            top: MediaQuery.of(context).padding.top + 8,
            left: 8,
            child: IconButton(
              icon: const Icon(Icons.close, size: 30, color: Colors.black),
              onPressed: () => Navigator.of(context).pop(),
            ),
          ),

          // Thông tin khoảng cách, thời gian, tốc độ
          Positioned(
            top: MediaQuery.of(context).padding.top + 60,
            left: 16,
            child: Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.9),
                borderRadius: BorderRadius.circular(8),
                boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 4)],
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '📍 Khoảng cách: $distance',
                    style: const TextStyle(
                      color: Colors.green,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    '⏱ Thời gian dự tính: $duration',
                    style: const TextStyle(
                      color: Colors.orange,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    '🚲 Tốc độ xe đạp: $speed',
                    style: const TextStyle(
                      color: Colors.blue,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
