// lib/data/sources/remote/user_api.dart
import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:http/http.dart' as http;
import '../../../domain/models/info_user.dart';

class UserApi {
  Future<UserInfo> getUserInfo(String token) async {
    final url = Uri.parse('$baseUrl/Auth/me');
    try {
      // --- Log request ---
      print('--- API REQUEST ---');
      print('GET $url');
      print('Token: $token');

      // Gửi request với timeout
      final response = await http
          .get(
            url,
            headers: {
              'Authorization': 'Bearer $token',
              'Content-Type': 'application/json',
            },
          )
          .timeout(const Duration(seconds: 10));

      // --- Log response ---
      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 401) {
        return {'Unauthorized', 401} as UserInfo;
      }
      // --- Xử lý kết quả ---
      if (response.statusCode == 200) {
        try {
          final json = jsonDecode(response.body);
          final data = json['data'];
          return UserInfo.fromJson(data);
        } catch (e) {
          print('JSON decode error: $e');
          throw Exception('Không thể parse dữ liệu người dùng từ server');
        }
      } else {
        print(
          'Lấy thông tin người dùng thất bại với status: ${response.statusCode}',
        );
        print('Response body: ${response.body}');
        throw Exception(
          'Lỗi khi lấy thông tin người dùng: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('Error calling getUserInfo API: $e');
      rethrow; // cho phép hàm gọi bên ngoài xử lý tiếp
    }
  }
}
