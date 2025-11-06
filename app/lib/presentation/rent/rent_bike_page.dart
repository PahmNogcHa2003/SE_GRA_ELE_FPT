import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:hola_bike_app/presentation/trip/trip_tracking_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class RentBikePage extends StatefulWidget {
  final String bikeId;
  const RentBikePage({super.key, required this.bikeId});

  @override
  State<RentBikePage> createState() => _RentBikePageState();
}

class _RentBikePageState extends State<RentBikePage> {
  String selectedTicket = 'daily';

  final Map<String, dynamic> bikeInfo = {
    'code': 'ZJHDCD',
    'type': 'Xe đạp thường',
    'status': 'Sẵn sàng',
    'location': 'Trạm số 5 - Cổng trường',
  };

  void _confirmRent() {
    EasyLoading.showSuccess('Thuê xe thành công!');
    Future.delayed(const Duration(seconds: 1), () {
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => TripTrackingPage(
            bikeId: widget.bikeId,
            isRenting: true, // truyền trạng thái đang thuê
          ),
        ),
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Thuê xe'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Column(
        children: [
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _infoRow(
                    Icons.confirmation_number,
                    'Mã số',
                    bikeInfo['code'],
                  ),
                  _infoRow(Icons.directions_bike, 'Loại xe', bikeInfo['type']),
                  _infoRow(
                    Icons.electric_bike,
                    'Tình trạng',
                    bikeInfo['status'],
                  ),
                  _infoRow(Icons.location_on, 'Trạm', bikeInfo['location']),
                  const SizedBox(height: 24),

                  const Text(
                    'Chọn loại vé',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: AppColors.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 12),

                  _ticketOption('daily', 'Vé ngày', Colors.green.shade100),
                  _ticketOption('monthly', 'Vé tháng', Colors.orange.shade100),
                  _ticketOption('single', 'Vé lượt', Colors.blue.shade100),
                ],
              ),
            ),
          ),

          // Nút xác nhận cố định dưới cùng
          Container(
            padding: const EdgeInsets.all(20),
            width: double.infinity,
            child: ElevatedButton.icon(
              onPressed: _confirmRent,
              icon: const Icon(Icons.check),
              label: const Text('Xác nhận thuê xe'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _infoRow(IconData icon, String label, dynamic value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        children: [
          Icon(icon, color: AppColors.accent),
          const SizedBox(width: 6),
          Text(
            '$label: ${value ?? 'Không rõ'}',
            style: const TextStyle(color: AppColors.textSecondary),
          ),
        ],
      ),
    );
  }

  Widget _ticketOption(String value, String label, Color bgColor) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      decoration: BoxDecoration(
        color: bgColor,
        borderRadius: BorderRadius.circular(12),
      ),
      child: RadioListTile<String>(
        value: value,
        groupValue: selectedTicket,
        onChanged: (val) => setState(() => selectedTicket = val!),
        title: Text(label),
        activeColor: AppColors.primary,
        contentPadding: const EdgeInsets.symmetric(horizontal: 12),
      ),
    );
  }
}
