import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_station.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class StationApi {
  Future<List<StationInfo>> getStationInfo(String token) async {
    final url = Uri.parse('$baseUrl/Stations/all');

    try {
      print('--- API REQUEST ---');
      print('GET $url');
      print('Token: $token');

      final response = await http
          .get(
            url,
            headers: {
              'Authorization': 'Bearer $token',
              'Content-Type': 'application/json',
            },
          )
          .timeout(const Duration(seconds: 10));

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        final decoded = jsonDecode(response.body);

        // ✅ JSON có cấu trúc: { "count": ..., "data": [ ... ] }
        if (decoded is Map && decoded.containsKey('data')) {
          final List<dynamic> data = decoded['data'];
          return data.map((e) => StationInfo.fromJson(e)).toList();
        } else {
          throw Exception('Phản hồi API không đúng định dạng mong đợi');
        }
      } else {
        print('❌ API error: ${response.statusCode}');
        throw Exception(
          'Lỗi khi lấy thông tin trạm: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('❌ Error calling getStationInfo API: $e');
      rethrow;
    }
  }
}
