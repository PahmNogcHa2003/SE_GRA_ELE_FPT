import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_rental-end.dart';
import 'package:http/http.dart' as http;

class RentalEndApi {
  /// Kết thúc phiên thuê xe với rentalId và vị trí hiện tại
  Future<InfoRentalEnd> endRental({
    required String token,
    required int rentalId,
    required double currentLatitude,
    required double currentLongitude,
    required double distanceMeters,
  }) async {
    final url = Uri.parse('$baseUrl/Rentals/$rentalId/end');

    try {
      print('--- API REQUEST ---');
      print('PUT $url');
      print('rentalId: $rentalId');
      print('currentLatitude: $currentLatitude');
      print('currentLongitude: $currentLongitude');

      final response = await http
          .put(
            url,
            headers: {
              'Authorization': 'Bearer $token',
              'Content-Type': 'application/json',
            },
            body: jsonEncode({
              "rentalId": rentalId,
              "currentLatitude": currentLatitude,
              "currentLongitude": currentLongitude,
              "distanceMeters": distanceMeters,
            }),
          )
          .timeout(const Duration(seconds: 15));

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        print('✅ Kết thúc thuê xe thành công');
        return InfoRentalEnd.fromJson(jsonDecode(response.body));
      } else {
        print('❌ Kết thúc thuê xe thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Kết thúc thuê xe thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API endRental: $e');
      rethrow;
    }
  }
}
