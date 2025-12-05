import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/core/utils/toast_util.dart';
import 'package:hola_bike_app/data/sources/remote/api_rental-history.dart';
import 'package:hola_bike_app/domain/models/info_rental-history.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:intl/intl.dart';

class TripTrackingHistoryPage extends StatefulWidget {
  const TripTrackingHistoryPage({Key? key}) : super(key: key);

  @override
  State<TripTrackingHistoryPage> createState() =>
      _TripTrackingHistoryPageState();
}

class _TripTrackingHistoryPageState extends State<TripTrackingHistoryPage> {
  final RentalHistoryApi _api = RentalHistoryApi();
  final secureStorage = const FlutterSecureStorage();

  List<InfoRentalHistory> tripHistory = [];
  bool isLoading = true;
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    _loadTrips();
  }

  Future<void> _loadTrips() async {
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) {
        ToastUtil.showError("Không tìm thấy token");
        return;
      }

      final history = await _api.getRentalHistory(token: token);

      setState(() {
        tripHistory = history;
        isLoading = false;
      });
    } catch (e) {
      setState(() {
        errorMessage = e.toString();
        isLoading = false;
      });
    }
  }

  String _formatDateTime(String dateTime) {
    try {
      final dt = DateTime.parse(dateTime);
      return DateFormat('dd/MM/yyyy HH:mm').format(dt);
    } catch (e) {
      return dateTime;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Chuyến đi của tôi'),
        backgroundColor: AppColors.primary,
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : errorMessage != null
          ? Center(
              child: Text(
                'Lỗi: $errorMessage',
                style: const TextStyle(color: Colors.red),
              ),
            )
          : tripHistory.isEmpty
          ? const Center(child: Text('Không có dữ liệu chuyến đi.'))
          : ListView.builder(
              padding: const EdgeInsets.symmetric(vertical: 8),
              itemCount: tripHistory.length,
              itemBuilder: (context, index) {
                final trip = tripHistory[index];
                return Card(
                  margin: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 6,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  elevation: 3,
                  child: Padding(
                    padding: const EdgeInsets.all(12),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // Header: Xe + Trạng thái
                        Row(
                          children: [
                            const Icon(
                              Icons.directions_bike,
                              color: AppColors.primary,
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              child: Text(
                                '${trip.vehicleCode} (${trip.vehicleType})',
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 16,
                                ),
                              ),
                            ),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: trip.status == 'End'
                                    ? Colors.green[100]
                                    : Colors.orange[100],
                                borderRadius: BorderRadius.circular(8),
                              ),
                              child: Text(
                                trip.status,
                                style: TextStyle(
                                  color: trip.status == 'End'
                                      ? Colors.green[800]
                                      : Colors.orange[800],
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 8),

                        // Thời gian
                        Text('Bắt đầu: ${_formatDateTime(trip.startTimeVn)}'),
                        Text('Kết thúc: ${_formatDateTime(trip.endTimeVn)}'),
                        const SizedBox(height: 4),

                        // Trạm
                        Text(
                          'Trạm: ${trip.startStationName} → ${trip.endStationName}',
                        ),
                        const SizedBox(height: 4),

                        // Quãng đường & thời lượng
                        Row(
                          children: [
                            Text(
                              'Quãng đường: ${trip.distanceKm.toStringAsFixed(2)} km',
                            ),
                            const SizedBox(width: 16),
                            Text('Thời lượng: ${trip.durationMinutes} phút'),
                          ],
                        ),
                        const SizedBox(height: 4),

                        // Carbon & Calo
                        Row(
                          children: [
                            Text(
                              'Carbon giảm: ${trip.co2SavedKg.toStringAsFixed(2)} kg',
                            ),
                            const SizedBox(width: 16),
                            Text(
                              'Calo tiêu thụ: ${trip.caloriesBurned.toStringAsFixed(2)}',
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
    );
  }
}
