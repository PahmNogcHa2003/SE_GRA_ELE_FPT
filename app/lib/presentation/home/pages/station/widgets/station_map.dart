import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:geolocator/geolocator.dart';
import 'package:hola_bike_app/application/usecases/usecase_get-station.dart';
import 'package:hola_bike_app/domain/models/info_station.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:latlong2/latlong.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'station_card.dart';

class StationMap extends StatefulWidget {
  const StationMap({super.key});

  @override
  State<StationMap> createState() => _StationMapState();
}

class _StationMapState extends State<StationMap> {
  final MapController _mapController = MapController();
  final GetStationInfoUsecase _usecase = GetStationInfoUsecase();
  final FlutterSecureStorage secureStorage = const FlutterSecureStorage();

  List<StationInfo> _stations = [];
  LatLng? _currentLocation;
  String? _error;

  List<StationInfo> _searchResults = [];
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();

    _initializeData();
  }

  Future<void> _initializeData() async {
    EasyLoading.show();
    await _getCurrentLocation();
    await _fetchStations();
  }

  Future<void> _getCurrentLocation() async {
    try {
      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
      }
      if (permission == LocationPermission.deniedForever) return;

      final pos = await Geolocator.getCurrentPosition(
        desiredAccuracy: LocationAccuracy.high,
      );
      setState(() {
        _currentLocation = LatLng(pos.latitude, pos.longitude);
      });
    } catch (e) {
      debugPrint('Lỗi lấy vị trí: $e');
    }
  }

  Future<void> _fetchStations() async {
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) throw Exception('Không tìm thấy token');
      final stations = await _usecase.execute(token);
      setState(() {
        _stations = stations;
        _searchResults = stations;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
      });
    } finally {
      EasyLoading.dismiss();
    }
  }

  void _onSearchChanged(String query) {
    final results = _stations.where((s) {
      return s.name.toLowerCase().contains(query.toLowerCase());
    }).toList();
    setState(() => _searchResults = results);
  }

  void _moveToStation(StationInfo station) {
    _mapController.move(LatLng(station.lat, station.lng), 17);
    FocusScope.of(context).unfocus();
    setState(() {
      _searchController.text = station.name;
      _searchResults = [];
    });
  }

  @override
  Widget build(BuildContext context) {
    if (_error != null) return Center(child: Text('Lỗi: $_error'));

    return Stack(
      children: [
        FlutterMap(
          mapController: _mapController,
          options: MapOptions(
            center: _currentLocation ?? LatLng(21.012516, 105.525241),
            zoom: 14,
          ),
          children: [
            TileLayer(
              urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
              userAgentPackageName: 'com.example.hola_bike_app',
            ),

            MarkerLayer(
              markers: [
                // marker trạm xe
                ..._stations.map(
                  (s) => Marker(
                    point: LatLng(s.lat, s.lng),
                    width: 40,
                    height: 40,
                    child: GestureDetector(
                      onTap: () {
                        showDialog(
                          context: context,
                          builder: (_) => StationCard(
                            station: s,
                            onClose: () => Navigator.of(context).pop(),
                          ),
                        );
                      },
                      child: const Icon(
                        Icons.location_pin,
                        color: Colors.red,
                        size: 38,
                      ),
                    ),
                  ),
                ),
                // marker vị trí hiện tại
                if (_currentLocation != null)
                  Marker(
                    point: _currentLocation!,
                    width: 40,
                    height: 40,
                    child: const Icon(
                      Icons.my_location,
                      color: Colors.blue,
                      size: 30,
                    ),
                  ),
              ],
            ),
          ],
        ),

        // 🔍 Thanh tìm kiếm
        Positioned(
          top: MediaQuery.of(context).padding.top + 12,
          left: 16,
          right: 16,
          child: Column(
            children: [
              Container(
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: [BoxShadow(color: Colors.black12, blurRadius: 4)],
                ),
                child: TextField(
                  controller: _searchController,
                  decoration: InputDecoration(
                    hintText: 'Tìm kiếm trạm xe...',
                    prefixIcon: const Icon(
                      Icons.search,
                      color: AppColors.primary,
                    ),
                    border: InputBorder.none,
                    contentPadding: const EdgeInsets.symmetric(
                      vertical: 12,
                      horizontal: 8,
                    ),
                  ),
                  onChanged: _onSearchChanged,
                ),
              ),

              // danh sách gợi ý
              if (_searchController.text.isNotEmpty &&
                  _searchResults.isNotEmpty)
                Container(
                  margin: const EdgeInsets.only(top: 4),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(8),
                    boxShadow: [
                      BoxShadow(color: Colors.black12, blurRadius: 4),
                    ],
                  ),
                  child: ListView.builder(
                    shrinkWrap: true,
                    itemCount: _searchResults.length,
                    itemBuilder: (context, index) {
                      final station = _searchResults[index];
                      return ListTile(
                        title: Text(station.name),
                        subtitle: Text(station.location),
                        onTap: () => _moveToStation(station),
                      );
                    },
                  ),
                ),
            ],
          ),
        ),

        // 🧭 Nút vị trí hiện tại & refresh
        Positioned(
          bottom: 16,
          right: 16,
          child: Column(
            children: [
              _buildCircleButton(Icons.my_location, 'Vị trí hiện tại', () {
                if (_currentLocation != null) {
                  _mapController.move(_currentLocation!, 16);
                }
              }),
              const SizedBox(height: 12),
              _buildCircleButton(Icons.refresh, 'Làm mới', () {
                _initializeData();
              }),
            ],
          ),
        ),
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
