import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_profile.dart';
import 'package:http/http.dart' as http;

class ProfileApi {
  Future<InfoProfile> getProfile({required String token}) async {
    final url = Uri.parse('$baseUrl/UserProfiles/me');

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
        print('✅ Lấy thông tin user profile thành công');

        final Map<String, dynamic> res = jsonDecode(response.body);

        // Vì API trả về {"success": true, "message": "...", "data": {...}}
        final Map<String, dynamic> data = res['data'];

        return InfoProfile.fromJson({'data': data});
      } else {
        print('❌ Lấy thông tin user profile thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lấy thông tin user profile thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API getProfile: $e');
      rethrow;
    }
  }
}
