import 'package:latlong2/latlong.dart';

class Station {
  final int id;
  final String name;
  final LatLng latLng;

  const Station({required this.id, required this.name, required this.latLng});
}

class AppLocations {
  static final List<Station> stations = [
    Station(
      id: 1,
      name: 'Đại học FPT',
      latLng: LatLng(21.012606973067452, 105.52535356875333),
    ),
    Station(
      id: 2,
      name: 'FPT Software',
      latLng: LatLng(21.010148850603212, 105.53638922582147),
    ),
    Station(
      id: 3,
      name: 'Học viện tài chính',
      latLng: LatLng(21.021988665518933, 105.52929914806127),
    ),
    Station(
      id: 4,
      name: 'Trung tâm dữ liệu quốc gia',
      latLng: LatLng(21.018114243280007, 105.53435544683929),
    ),
    Station(
      id: 5,
      name: 'Viện Hàng Không Vũ Trụ Viettel',
      latLng: LatLng(21.009267479899677, 105.53127737917642),
    ),
    Station(
      id: 6,
      name: 'Trung Tâm Vũ Trụ Việt Nam (VNSC)',
      latLng: LatLng(21.02265409089084, 105.54490699248427),
    ),
    Station(
      id: 7,
      name: 'Trường ĐH Sĩ Quan Chính Trị',
      latLng: LatLng(20.987998822569775, 105.51444356049518),
    ),
    Station(
      id: 8,
      name: 'Đại Học Quốc Gia Hà Nội (VNU)',
      latLng: LatLng(20.991691989620335, 105.51904677666448),
    ),
    Station(
      id: 9,
      name: 'VNPT IDC Láng Hoà Lạc',
      latLng: LatLng(20.99997019702437, 105.53309087599071),
    ),
    Station(
      id: 10,
      name: 'MB Bank',
      latLng: LatLng(21.004928532189023, 105.53429918280271),
    ),
    Station(
      id: 11,
      name: 'Trung Tâm DV Tài Chính – Bộ Tài Chính',
      latLng: LatLng(21.004955301645232, 105.53170888201495),
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
