import 'package:hola_bike_app/data/sources/remote/api_rental-end.dart';
import 'package:hola_bike_app/domain/models/info_rental-end.dart';

class RentalEndUsecase {
  final RentalEndApi _api = RentalEndApi();

  Future<InfoRentalEnd> execute({
    required String token,
    required int rentalId,
    required double currentLatitude,
    required double currentLongitude,
    required double distanceMeters,
  }) async {
    return _api.endRental(
      token: token,
      rentalId: rentalId,
      currentLatitude: currentLatitude,
      currentLongitude: currentLongitude,
      distanceMeters: distanceMeters,
    );
  }
}
