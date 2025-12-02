import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:geolocator/geolocator.dart';
import 'package:hola_bike_app/application/usecases/usecase_rental-start.dart';
import 'package:hola_bike_app/application/usecases/usecase_scan-vehicle.dart';
import 'package:hola_bike_app/application/usecases/usecase_ticket-active.dart';
import 'package:hola_bike_app/domain/models/info_scan-vehicle.dart';
import 'package:hola_bike_app/domain/models/info_ticket-active.dart';
import 'package:hola_bike_app/presentation/rent/widgets/widget_confirm-rent.dart';
import 'package:hola_bike_app/presentation/rent/widgets/widget_ticket-list.dart';
import 'package:hola_bike_app/presentation/rent/widgets/widget_vehicle-info.dart';
import 'package:hola_bike_app/presentation/trip/trip_tracking_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class RentBikePage extends StatefulWidget {
  final String bikeId;
  const RentBikePage({super.key, required this.bikeId});

  @override
  State<RentBikePage> createState() => _RentBikePageState();
}

class _RentBikePageState extends State<RentBikePage> {
  final _scanVehicleUsecase = ScanVehicleUsecase();
  final _ticketActiveUsecase = TicketActiveUsecase();
  final secureStorage = const FlutterSecureStorage();
  final _rentalStartUsecase = RentalStartUsecase();

  InfoScanVehicle? vehicleInfo;
  List<TicketInfo> tickets = [];
  int? selectedTicketId; // để nullable
  DateTime? startTime; // thêm biến lưu thời gian bắt đầu

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    EasyLoading.show();
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) throw Exception('Không tìm thấy access token');

      final vehicleId = int.parse(widget.bikeId);

      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
      }
      if (permission == LocationPermission.deniedForever) return;

      final pos = await Geolocator.getCurrentPosition(
        desiredAccuracy: LocationAccuracy.high,
      );

      // gọi API scan xe
      final info = await _scanVehicleUsecase.execute(
        token: token,
        vehicleId: vehicleId,
        currentLatitude: pos.latitude,
        currentLongitude: pos.longitude,
      );

      // gọi API lấy vé
      final activeTickets = await _ticketActiveUsecase.execute(token);

      setState(() {
        vehicleInfo = info;
        tickets = activeTickets;
        // gán selectedTicketId bằng id của vé đầu tiên nếu có
        if (tickets.isNotEmpty) {
          selectedTicketId = tickets.first.id;
        }
      });
    } catch (e) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(const SnackBar(content: Text('Lỗi tải dữ liệu')));
      });
    } finally {
      EasyLoading.dismiss();
    }
  }

  Future<void> _confirmRent() async {
    if (selectedTicketId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn vé trước khi thuê')),
      );
      return;
    }

    // gán thời gian bắt đầu khi user bấm nút
    startTime = DateTime.now();

    EasyLoading.show(status: 'Đang xác nhận thuê xe...');
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) throw Exception('Không tìm thấy access token');

      final vehicleId = int.parse(widget.bikeId);

      // gọi API RentalStart để xác nhận thuê
      final rentalResult = await _rentalStartUsecase.execute(
        token: token,
        vehicleId: vehicleId,
        userTicketId: selectedTicketId!,
        startTime: startTime!, // truyền thời gian bắt đầu
      );

      EasyLoading.dismiss();

      if (rentalResult.success) {
        EasyLoading.showSuccess('Thuê xe thành công!');
        Future.delayed(const Duration(seconds: 1), () {
          Navigator.pushReplacement(
            context,
            MaterialPageRoute(
              builder: (_) => TripTrackingPage(
                rentalId: rentalResult.retaalId,
                // có thể truyền startTime nếu TripTrackingPage cần
              ),
            ),
          );
        });
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Xác nhận thuê xe thất bại')),
        );
      }
    } catch (e) {
      EasyLoading.dismiss();
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Lỗi xác nhận thuê xe: $e')));
    }
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
              padding: const EdgeInsets.all(20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  if (vehicleInfo != null)
                    VehicleInfoWidget(info: vehicleInfo!),
                  const SizedBox(height: 24),
                  const Text(
                    'Danh sách vé khả dụng',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                      color: AppColors.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 12),
                  TicketListWidget(
                    tickets: tickets,
                    selectedTicketId: selectedTicketId,
                    onSelect: (id) {
                      setState(() {
                        selectedTicketId = id;
                      });
                    },
                  ),
                ],
              ),
            ),
          ),
          ConfirmRentButton(onConfirm: _confirmRent),
        ],
      ),
    );
  }
}
