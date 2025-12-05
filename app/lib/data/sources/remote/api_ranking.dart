import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_ranking.dart';
import 'package:http/http.dart' as http;

class RankingApi {
  Future<RankingEnd> getRanking({
    required String token,
    String period = 'lifetime',
  }) async {
    final url = Uri.parse('$baseUrl/Leaderboard?period=$period&topN=10');

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
        print('✅ Lấy bảng xếp hạng thành công');

        final Map<String, dynamic> res = jsonDecode(response.body);
        return RankingEnd.fromJson(res);
      } else {
        print('❌ Lấy bảng xếp hạng thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lấy bảng xếp hạng thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API getRanking: $e');
      rethrow;
    }
  }
}
