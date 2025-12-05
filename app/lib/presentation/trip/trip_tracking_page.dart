import 'dart:async';
import 'dart:convert';
import 'dart:math' as math;
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:geolocator/geolocator.dart';
import 'package:hola_bike_app/application/usecases/usecase_rental-end.dart';
import 'package:hola_bike_app/core/utils/toast_util.dart';
import 'package:hola_bike_app/data/sources/local/trip_local_storage.dart';
import 'package:hola_bike_app/domain/models/tracking_session.dart';
import 'package:hola_bike_app/presentation/home/home_screen.dart';
import 'package:latlong2/latlong.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class TripTrackingPage extends StatefulWidget {
  final int rentalId;

  const TripTrackingPage({super.key, required this.rentalId});

  @override
  State<TripTrackingPage> createState() => _TripTrackingPageState();
}

class _TripTrackingPageState extends State<TripTrackingPage> {
  final MapController _mapController = MapController();
  StreamSubscription<Position>? _positionSub;
  Timer? _persistTimer;

  final secureStorage = const FlutterSecureStorage();
  final _rentalEndUsecase = RentalEndUsecase();
  final TripLocalStorage _tripStorage = TripLocalStorage();

  final List<LatLng> _routePoints = [];
  LatLng? _currentPos;
  double _totalMeters = 0;
  DateTime? _startTime;

  @override
  void initState() {
    super.initState();
    _loadSessionOrStartTracking();
  }

  @override
  void dispose() {
    _positionSub?.cancel();
    _persistTimer?.cancel();
    super.dispose();
  }

  Future<void> _loadSessionOrStartTracking() async {
    final session = await _tripStorage.loadSession();

    if (session != null) {
      setState(() {
        _startTime = session.startTime;
        _totalMeters = session.totalMeters;
        _routePoints.addAll(session.routePoints);
        if (_routePoints.isNotEmpty) {
          _currentPos = _routePoints.last;
          _mapController.move(_currentPos!, 17);
        }
      });
      _startTracking();
    } else {
      _startTracking();
    }

    _persistTimer = Timer.periodic(const Duration(seconds: 1), (_) {
      _persistSession();
    });
  }

  Future<void> _startTracking() async {
    final status = await Permission.location.request();
    if (status.isDenied || status.isPermanentlyDenied) {
      EasyLoading.showError('Không có quyền truy cập vị trí');
      return;
    }

    setState(() {
      _startTime ??= DateTime.now();
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

  Future<void> _persistSession() async {
    if (_startTime == null) return;

    final session = TrackingSession(
      rentalId: widget.rentalId,
      startTime: _startTime!,
      totalMeters: _totalMeters,
      routePoints: List<LatLng>.from(_routePoints),
    );

    await _tripStorage.saveSession(session);
  }

  Future<void> _returnBike() async {
    EasyLoading.show();

    final km = (_totalMeters / 1000).toStringAsFixed(2);
    final carbon = (double.parse(km) * 0.15).toStringAsFixed(2);
    final calories = (double.parse(km) * 25).toStringAsFixed(0);

    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) {
        ToastUtil.showError("Không tìm thấy token");
        return;
      }

      if (_currentPos == null) {
        ToastUtil.showError("Không xác định được vị trí hiện tại");
        return;
      }

      await _rentalEndUsecase.execute(
        token: token,
        rentalId: widget.rentalId,
        currentLatitude: _currentPos!.latitude,
        currentLongitude: _currentPos!.longitude,
        distanceMeters: (_totalMeters),
      );

      ToastUtil.showSuccess("✅ Trả xe thành công");

      await _tripStorage.clearSession();

      _stopTracking();
    } catch (e) {
      ToastUtil.showError("❌ Lỗi trả xe, vui lòng đến gần trạm");
    }
    EasyLoading.dismiss();
  }

  void _stopTracking() {
    _positionSub?.cancel();
    _persistTimer?.cancel();
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (_) => const HomeScreen()),
    );
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

  Widget _buildTripInfo() {
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
            'Xe: ${widget.rentalId}',
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
            onPressed: _returnBike,
            icon: const Icon(Icons.assignment_return),
            label: const Text('Trả xe'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return WillPopScope(
      onWillPop: () async => false, // luôn trả về false => không cho pop
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Hành trình thuê xe'),
          backgroundColor: AppColors.primary,
          automaticallyImplyLeading: false, // ẩn nút back trên AppBar
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
              child: _buildTripInfo(),
            ),
          ],
        ),
      ),
    );
  }
}
