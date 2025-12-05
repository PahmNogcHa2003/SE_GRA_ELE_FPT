import 'package:hola_bike_app/data/sources/remote/api_rental-history.dart';
import 'package:hola_bike_app/domain/models/info_rental-history.dart';

class RentalHistoryUsecase {
  final RentalHistoryApi _api = RentalHistoryApi();

  Future<List<InfoRentalHistory>> execute({required String token}) async {
    return _api.getRentalHistory(token: token);
  }
}
