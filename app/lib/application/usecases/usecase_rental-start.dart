import 'package:hola_bike_app/data/sources/remote/api_rental-start.dart';
import 'package:hola_bike_app/domain/models/info_rental-start.dart';

class RentalStartUsecase {
  final RentalStartApi _api = RentalStartApi();

  Future<RentalStartInfo> execute({
    required String token,
    required int vehicleId,
    required int userTicketId,
    required DateTime startTime,
  }) {
    return _api.startRental(
      token: token,
      vehicleId: vehicleId,
      userTicketId: userTicketId,
      startTime: startTime,
    );
  }
}
