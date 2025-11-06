import 'dart:async';
import 'dart:math' as math;
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:geolocator/geolocator.dart';
import 'package:latlong2/latlong.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class TripTrackingPage extends StatefulWidget {
  final String? bikeId;
  final bool isRenting;

  const TripTrackingPage({super.key, this.bikeId, required this.isRenting});

  @override
  State<TripTrackingPage> createState() => _TripTrackingPageState();
}

class _TripTrackingPageState extends State<TripTrackingPage> {
  final MapController _mapController = MapController();
  StreamSubscription<Position>? _positionSub;

  final List<LatLng> _routePoints = [];
  final List<Map<String, dynamic>> _tripHistory = [];
  LatLng? _currentPos;
  double _totalMeters = 0;
  DateTime? _startTime;

  @override
  void initState() {
    super.initState();
    if (widget.isRenting && widget.bikeId != null) _startTracking();
    _loadHistory();
  }

  @override
  void dispose() {
    _positionSub?.cancel();
    super.dispose();
  }

  Future<void> _startTracking() async {
    final status = await Permission.location.request();
    if (status.isDenied || status.isPermanentlyDenied) {
      EasyLoading.showError('Không có quyền truy cập vị trí');
      return;
    }

    setState(() {
      _startTime = DateTime.now();
    });

    EasyLoading.show(status: 'Đang bắt đầu hành trình...');

    final settings = const LocationSettings(
      accuracy: LocationAccuracy.bestForNavigation,
      distanceFilter: 5,
    );

    bool firstPositionReceived = false;

    _positionSub = Geolocator.getPositionStream(locationSettings: settings)
        .listen((Position pos) {
          final LatLng newPoint = LatLng(pos.latitude, pos.longitude);

          setState(() {
            if (_routePoints.isNotEmpty) {
              final prev = _routePoints.last;
              final dist = _calcDistance(prev, newPoint);
              if (dist > 0.5) _totalMeters += dist;
            }
            _routePoints.add(newPoint);
            _currentPos = newPoint;
          });

          _mapController.move(newPoint, 17);

          if (!firstPositionReceived) {
            EasyLoading.dismiss();
            firstPositionReceived = true;
          }
        });
  }

  void _stopTracking() {
    _positionSub?.cancel();

    EasyLoading.showSuccess('Hành trình đã kết thúc');
    Navigator.pop(context);
  }

  double _calcDistance(LatLng a, LatLng b) {
    const R = 6371000;
    final dLat = (b.latitude - a.latitude) * math.pi / 180;
    final dLon = (b.longitude - a.longitude) * math.pi / 180;
    final lat1 = a.latitude * math.pi / 180;
    final lat2 = b.latitude * math.pi / 180;

    final h =
        math.sin(dLat / 2) * math.sin(dLat / 2) +
        math.cos(lat1) *
            math.cos(lat2) *
            math.sin(dLon / 2) *
            math.sin(dLon / 2);
    return R * 2 * math.atan2(math.sqrt(h), math.sqrt(1 - h));
  }

  void _loadHistory() {
    _tripHistory.addAll([
      {'bikeId': 'HB001', 'distance': 3.2, 'duration': '15 phút'},
      {'bikeId': 'HB002', 'distance': 5.8, 'duration': '28 phút'},
    ]);
  }

  Widget _buildTripInfo() {
    if (widget.bikeId == null || widget.bikeId!.isEmpty)
      return const SizedBox();

    final km = (_totalMeters / 1000).toStringAsFixed(2);
    final durationMinutes = _startTime != null
        ? DateTime.now().difference(_startTime!).inMinutes
        : 0;
    final duration = '$durationMinutes phút';

    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.15),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            'Xe: ${widget.bikeId}',
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w600,
              color: AppColors.textPrimary,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Quãng đường: $km km',
            style: const TextStyle(
              fontSize: 15,
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Thời gian: $duration',
            style: const TextStyle(
              fontSize: 15,
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: 16),
          ElevatedButton.icon(
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.primary,
              minimumSize: const Size.fromHeight(45),
            ),
            onPressed: _stopTracking,
            icon: const Icon(Icons.stop_circle_outlined),
            label: const Text('Kết thúc hành trình'),
          ),
        ],
      ),
    );
  }

  Widget _buildHistoryList() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.15),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Text(
            'Lịch sử hành trình',
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w600,
              color: AppColors.textPrimary,
            ),
          ),
          const SizedBox(height: 8),
          ..._tripHistory.map(
            (trip) => ListTile(
              title: Text('Xe: ${trip['bikeId']}'),
              subtitle: Text(
                'Quãng đường: ${trip['distance']} km - Thời gian: ${trip['duration']}',
              ),
            ),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Hành trình thuê xe'),
        backgroundColor: AppColors.primary,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Stack(
        children: [
          FlutterMap(
            mapController: _mapController,
            options: MapOptions(
              initialCenter: _currentPos ?? const LatLng(21.0285, 105.8048),
              initialZoom: 15,
              maxZoom: 18,
              minZoom: 5,
            ),
            children: [
              TileLayer(
                urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                userAgentPackageName: 'com.example.hola_bike_app',
              ),
              PolylineLayer(
                polylines: [
                  Polyline(
                    points: _routePoints,
                    color: AppColors.primary,
                    strokeWidth: 5,
                  ),
                ],
              ),
              if (_currentPos != null)
                MarkerLayer(
                  markers: [
                    Marker(
                      point: _currentPos!,
                      width: 60,
                      height: 60,
                      child: const Icon(
                        Icons.location_pin,
                        color: Colors.red,
                        size: 40,
                      ),
                    ),
                  ],
                ),
            ],
          ),
          Positioned(
            left: 16,
            right: 16,
            bottom: 24,
            child: widget.isRenting ? _buildTripInfo() : _buildHistoryList(),
          ),
        ],
      ),
    );
  }
}
