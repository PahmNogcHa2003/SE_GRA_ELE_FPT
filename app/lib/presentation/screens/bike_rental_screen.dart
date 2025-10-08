import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/screens/ticket_selection_sheet.dart';
import '../../data/models/rental_request.dart';
import '../../application/services/bike_rental_service.dart';
import '../widgets/rental_form.dart';
import 'map_bottom_sheet.dart';

class BikeRentalScreen extends StatefulWidget {
  const BikeRentalScreen({Key? key}) : super(key: key);

  @override
  State<BikeRentalScreen> createState() => _BikeRentalScreenState();
}

class _BikeRentalScreenState extends State<BikeRentalScreen> {
  final BikeRentalService _service = BikeRentalService();

  int? _selectedStartId;
  int? _selectedEndId;
  DateTime? _startDate;
  DateTime? _endDate;

  String _distanceTxt = '';
  String _durationTxt = '';
  String _avgSpeedTxt = '';
  Duration? _actualDuration;
  int? _actualDurationInMinutes;
  String? _estimatedPrice;

  TicketType? _selectedTicket; // 👈 loại vé đã chọn

  // Khi thay đổi bến
  void _onStationsChanged(int? startId, int? endId) {
    setState(() {
      _selectedStartId = startId;
      _selectedEndId = endId;
      _distanceTxt = '';
      _durationTxt = '';
      _avgSpeedTxt = '';
      _actualDuration = null;
      _actualDurationInMinutes = null;
      _estimatedPrice = null;
    });
  }

  // Khi thay đổi ngày thuê
  void _onDatesChanged(DateTime? start, DateTime? end) {
    setState(() {
      _startDate = start;
      _endDate = end;
    });
  }

  // Mở bản đồ
  void _openMap() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.black.withOpacity(0.4),
      builder: (_) => MapBottomSheet(
        startId: _selectedStartId,
        endId: _selectedEndId,
        onRouteCalculated: (distance, duration, speed) {
          setState(() {
            _distanceTxt = distance;
            _durationTxt = duration;
            _avgSpeedTxt = speed;
          });
        },
      ),
    );
  }

  // Mở chọn loại vé
  void _openTicketSheet(Duration rentalDuration) async {
    final result = await showModalBottomSheet<TicketType>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => TicketSelectionSheet(
        rentalDuration: rentalDuration,
        initialSelection: _selectedTicket,
      ),
    );

    if (result != null) {
      setState(() {
        _selectedTicket = result;
      });
    }
  }

  // Submit đặt xe
  Future<void> _submit(BuildContext ctx, RentalRequest request) async {
    try {
      final message = await _service.rentBike(request);

      final duration = request.endDate.difference(request.startDate);
      final minutes = duration.inMinutes;
      final price = calculateEstimatedPrice(duration);

      setState(() {
        _actualDuration = duration;
        _actualDurationInMinutes = minutes;
        _estimatedPrice = price;
      });

      if (!mounted) return;
      showDialog(
        context: ctx,
        builder: (_) => AlertDialog(
          title: const Text('✅ Thành công'),
          content: Text(message),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('Đóng'),
            ),
          ],
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(
        ctx,
      ).showSnackBar(SnackBar(content: Text('Lỗi khi gửi yêu cầu: $e')));
    }
  }

  @override
  Widget build(BuildContext context) {
    final showRentalDuration = _startDate != null && _endDate != null;
    final rentalDuration = showRentalDuration
        ? _endDate!.difference(_startDate!)
        : null;

    return Scaffold(
      appBar: AppBar(
        title: const Text('🚲 Cho Thuê Xe HolaGo'),
        backgroundColor: Colors.teal,
        centerTitle: true,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            /// Form đặt xe
            RentalForm(
              onStationsChanged: _onStationsChanged,
              onSubmit: (request) async {
                await _submit(context, request);
              },
              extraWidget: Column(
                children: [
                  GestureDetector(
                    onTap: () =>
                        _openTicketSheet(rentalDuration ?? Duration.zero),
                    child: Container(
                      width: double.infinity,
                      padding: const EdgeInsets.all(14),
                      decoration: BoxDecoration(
                        color: Colors.purple.withOpacity(0.08),
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: Colors.purple, width: 1.2),
                      ),
                      child: Row(
                        children: [
                          const Icon(
                            Icons.confirmation_number,
                            color: Colors.purple,
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Text(
                              _selectedTicket == null
                                  ? '👉 Chưa chọn loại vé'
                                  : () {
                                      final name = _selectedTicket
                                          .toString()
                                          .split('.')
                                          .last
                                          .toUpperCase();

                                      String priceText = '';
                                      switch (_selectedTicket) {
                                        case TicketType.luot:
                                          priceText = ' - 10.000đ/lượt';
                                          break;
                                        case TicketType.ngay:
                                          priceText = ' - 50.000đ/ngày';
                                          break;
                                        case TicketType.thang:
                                          priceText = ''; // Không hiển thị giá
                                          break;
                                        default:
                                          priceText = '';
                                      }

                                      return '🎫 Vé: $name$priceText';
                                    }(),
                              style: const TextStyle(
                                fontWeight: FontWeight.w600,
                                color: Colors.purple,
                              ),
                            ),
                          ),
                          const Icon(Icons.edit, color: Colors.purple),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 16),
                ],
              ),
            ),

            const SizedBox(height: 24),

            /// Hiển thị thời gian thuê dự kiến
            if (rentalDuration != null)
              Card(
                elevation: 3,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Text(
                    '🕒 Thời gian thuê dự kiến: ${rentalDuration.inMinutes} phút',
                    style: const TextStyle(
                      color: Colors.indigo,
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                ),
              ),

            const SizedBox(height: 32),

            /// ✅ Nút bản đồ ở dưới cùng
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: _openMap,
                icon: const Icon(Icons.map),
                label: const Text(
                  'Xem bản đồ',
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.teal,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(14),
                  ),
                  elevation: 4,
                ),
              ),
            ),

            const SizedBox(height: 16),
          ],
        ),
      ),
    );
  }

  /// Hàm tính giá tạm tính
  String calculateEstimatedPrice(Duration duration) {
    final hours = duration.inHours;
    final days = duration.inDays;
    final isOvernight = days >= 1;
    int price = 0;

    if (hours <= 3) {
      price = 50000;
    } else {
      price = 80000;
    }

    if (isOvernight) {
      price += 20000;
    }

    if (days < 1) {
      return '❌ Thời gian thuê phải tối thiểu 1 ngày';
    } else if (days > 7) {
      return '❌ Thời gian thuê tối đa là 7 ngày';
    }

    return '$price VNĐ/xe';
  }
}
