import 'package:hola_bike_app/data/sources/remote/api_scan-vehicle.dart';
import 'package:hola_bike_app/domain/models/info_scan-vehicle.dart';

class ScanVehicleUsecase {
  final ScanVehicleApi _api = ScanVehicleApi();

  Future<InfoScanVehicle> execute({
    required String token,
    required int vehicleId,
    required double currentLatitude,
    required double currentLongitude,
  }) {
    return _api.scanVehicle(
      token: token,
      vehicleId: vehicleId,
      currentLatitude: currentLatitude,
      currentLongitude: currentLongitude,
    );
  }
}
