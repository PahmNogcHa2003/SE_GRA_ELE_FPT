import '../../data/datasources/bike_rental_api.dart';
import '../../data/models/rental_request.dart';

class BikeRentalService {
  final BikeRentalApi _api = BikeRentalApi();

  Future<String> rentBike(RentalRequest request) async {
    final success = await _api.submitRental(request);
    return success ? 'Đặt xe thành công!' : 'Có lỗi xảy ra, vui lòng thử lại.';
  }
}
