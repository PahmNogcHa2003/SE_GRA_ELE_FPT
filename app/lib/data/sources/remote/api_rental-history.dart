import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_rental-history.dart';
import 'package:http/http.dart' as http;

class RentalHistoryApi {
  Future<List<InfoRentalHistory>> getRentalHistory({
    required String token,
  }) async {
    final url = Uri.parse('$baseUrl/Rentals/history');

    try {
      print('--- API REQUEST ---');
      print('GET $url');

      final response = await http
          .get(
            url,
            headers: {
              'Authorization': 'Bearer $token',
              'Content-Type': 'application/json',
            },
          )
          .timeout(const Duration(seconds: 15));

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        print('✅ Lấy lịch sử thuê xe thành công');

        final Map<String, dynamic> res = jsonDecode(response.body);
        final List<dynamic> data = res['data'];

        return data.map((e) => InfoRentalHistory.fromJson(e)).toList();
      } else {
        print('❌ Lấy lịch sử thuê xe thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lấy lịch sử thuê xe thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API getRentalHistory: $e');
      rethrow;
    }
  }
}
