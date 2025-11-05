import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class TripTrackingHistoryPage extends StatelessWidget {
  const TripTrackingHistoryPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final List<Map<String, dynamic>> tripHistory = [
      {
        'date': '27/10/2025',
        'distance': '5.2 km',
        'carbon': '0.8 kg',
        'calories': '120 cal',
      },
      {
        'date': '26/10/2025',
        'distance': '3.4 km',
        'carbon': '0.5 kg',
        'calories': '80 cal',
      },
      // Thêm dữ liệu giả lập khác nếu cần
    ];

    return Scaffold(
      appBar: AppBar(
        title: const Text('Chuyến đi của tôi'),
        backgroundColor: AppColors.primary,
      ),
      body: tripHistory.isEmpty
          ? const Center(child: Text('Không có dữ liệu chuyến đi.'))
          : ListView.builder(
              itemCount: tripHistory.length,
              itemBuilder: (context, index) {
                final trip = tripHistory[index];
                return Card(
                  margin: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 8,
                  ),
                  child: ListTile(
                    leading: const Icon(
                      Icons.directions_bike,
                      color: AppColors.primary,
                    ),
                    title: Text('Ngày: ${trip['date']}'),
                    subtitle: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('Quãng đường: ${trip['distance']}'),
                        Text('Carbon giảm: ${trip['carbon']}'),
                        Text('Calo tiêu thụ: ${trip['calories']}'),
                      ],
                    ),
                  ),
                );
              },
            ),
    );
  }
}
