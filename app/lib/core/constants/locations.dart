import 'package:latlong2/latlong.dart';

enum VehicleType { bike, electric, car }

class Station {
  final int id;
  final String name;
  final LatLng latLng;
  final VehicleType type;
  final int available;
  final String batteryStatus;

  const Station({
    required this.id,
    required this.name,
    required this.latLng,
    required this.type,
    required this.available,
    required this.batteryStatus,
  });
}

class AppLocations {
  static final List<Station> stations = [
    Station(
      id: 1,
      name: 'Đại học FPT',
      latLng: LatLng(21.012606973067452, 105.52535356875333),
      type: VehicleType.bike,
      available: 12,
      batteryStatus: 'Không dùng pin',
    ),
    Station(
      id: 2,
      name: 'FPT Software',
      latLng: LatLng(21.010148850603212, 105.53638922582147),
      type: VehicleType.electric,
      available: 5,
      batteryStatus: 'Pin 80%',
    ),
    Station(
      id: 3,
      name: 'Học viện tài chính',
      latLng: LatLng(21.021988665518933, 105.52929914806127),
      type: VehicleType.car,
      available: 3,
      batteryStatus: 'Pin 65%',
    ),
    Station(
      id: 4,
      name: 'Trung tâm dữ liệu quốc gia',
      latLng: LatLng(21.018114243280007, 105.53435544683929),
      type: VehicleType.electric,
      available: 7,
      batteryStatus: 'Pin 90%',
    ),
    Station(
      id: 5,
      name: 'Viện Hàng Không Vũ Trụ Viettel',
      latLng: LatLng(21.009267479899677, 105.53127737917642),
      type: VehicleType.bike,
      available: 9,
      batteryStatus: 'Không dùng pin',
    ),
    Station(
      id: 6,
      name: 'Trung Tâm Vũ Trụ Việt Nam (VNSC)',
      latLng: LatLng(21.02265409089084, 105.54490699248427),
      type: VehicleType.car,
      available: 2,
      batteryStatus: 'Pin 50%',
    ),
    Station(
      id: 7,
      name: 'Trường ĐH Sĩ Quan Chính Trị',
      latLng: LatLng(20.987998822569775, 105.51444356049518),
      type: VehicleType.electric,
      available: 4,
      batteryStatus: 'Pin 70%',
    ),
    Station(
      id: 8,
      name: 'Đại Học Quốc Gia Hà Nội (VNU)',
      latLng: LatLng(20.991691989620335, 105.51904677666448),
      type: VehicleType.bike,
      available: 10,
      batteryStatus: 'Không dùng pin',
    ),
    Station(
      id: 9,
      name: 'VNPT IDC Láng Hoà Lạc',
      latLng: LatLng(20.99997019702437, 105.53309087599071),
      type: VehicleType.car,
      available: 1,
      batteryStatus: 'Pin 40%',
    ),
    Station(
      id: 10,
      name: 'MB Bank',
      latLng: LatLng(21.004928532189023, 105.53429918280271),
      type: VehicleType.electric,
      available: 6,
      batteryStatus: 'Pin 85%',
    ),
    Station(
      id: 11,
      name: 'Trung Tâm DV Tài Chính – Bộ Tài Chính',
      latLng: LatLng(21.004955301645232, 105.53170888201495),
      type: VehicleType.bike,
      available: 8,
      batteryStatus: 'Không dùng pin',
    ),
  ];

  static Station? getById(int id) {
    try {
      return stations.firstWhere((s) => s.id == id);
    } catch (e) {
      return null;
    }
  }
}
