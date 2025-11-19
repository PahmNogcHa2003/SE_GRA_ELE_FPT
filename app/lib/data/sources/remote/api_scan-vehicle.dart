import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_scan-vehicle.dart';
import 'package:http/http.dart' as http;

class ScanVehicleApi {
  Future<InfoScanVehicle> scanVehicle({
    required String token,
    required int vehicleId,
    required double currentLatitude,
    required double currentLongitude,
  }) async {
    final url = Uri.parse('$baseUrl/Rentals/scan-vehicle');

    try {
      print('--- API REQUEST ---');
      print('POST $url');
      print('vehicleId $vehicleId');
      print('currentLatitude $currentLatitude');
      print('currentLongitude $currentLongitude');

      final response = await http
          .post(
            url,
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer $token',
            },
            body: jsonEncode({
              'vehicleId': vehicleId,
              'currentLatitude': currentLatitude,
              'currentLongitude': currentLongitude,
            }),
          )
          .timeout(const Duration(seconds: 15));

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        final data = json['data'];
        return InfoScanVehicle.fromJson(data);
      } else {
        print('❌ Scan vehicle thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Scan vehicle thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API ScanVehicle: $e');
      rethrow;
    }
  }
}
