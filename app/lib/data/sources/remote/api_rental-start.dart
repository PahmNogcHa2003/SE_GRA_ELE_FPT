import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_rental-start.dart';
import 'package:http/http.dart' as http;

class RentalStartApi {
  /// Bắt đầu thuê xe với vehicleId, userTicketId và thời gian bắt đầu
  Future<RentalStartInfo> startRental({
    required String token,
    required int vehicleId,
    required int userTicketId,
    required DateTime startTime,
  }) async {
    final url = Uri.parse('$baseUrl/Rentals/start');

    try {
      print('--- API REQUEST ---');
      print('POST $url');
      print('vehicleId: $vehicleId');
      print('userTicketId: $userTicketId');
      print('startTime: $startTime');

      final response = await http
          .post(
            url,
            headers: {
              'Authorization': 'Bearer $token',
              'Content-Type': 'application/json',
            },
            body: jsonEncode({
              'vehicleId': vehicleId,
              'userTicketId': userTicketId,
              'startTime': startTime.toIso8601String(),
            }),
          )
          .timeout(const Duration(seconds: 15));

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        return RentalStartInfo.fromJson(jsonDecode(response.body));
      } else {
        print('❌ Bắt đầu thuê xe thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Bắt đầu thuê xe thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API startRental: $e');
      rethrow;
    }
  }
}
