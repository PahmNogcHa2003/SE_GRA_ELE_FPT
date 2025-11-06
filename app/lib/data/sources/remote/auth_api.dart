import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:http/http.dart' as http;
import '../../../domain/models/login_response.dart';

class AuthApi {
  /// Login API with detailed error handling and logging
  Future<LoginResponse> login({
    required String email,
    required String password,
    required String deviceId,
    required String pushToken,
    required String platform,
  }) async {
    final url = Uri.parse('$baseUrl/Auth/login');
    final body = jsonEncode({
      "email": email,
      "password": password,
      "deviceId": deviceId,
      "pushToken": pushToken,
      "platform": platform,
    });

    try {
      // Log request
      print('--- API REQUEST ---');
      print('POST $url');
      print('BODY: $body');

      // Send request with timeout
      final response = await http
          .post(url, headers: {'Content-Type': 'application/json'}, body: body)
          .timeout(const Duration(seconds: 10));

      // Log response
      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      // Check status code
      if (response.statusCode == 200) {
        try {
          return LoginResponse.fromJson(jsonDecode(response.body));
        } catch (e) {
          print('JSON decode error: $e');
          throw Exception('Không thể parse dữ liệu từ server');
        }
      } else {
        print('Login failed with status: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Đăng nhập thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('Error calling login API: $e');
      rethrow; // cho phép gọi hàm bên ngoài xử lý tiếp
    }
  }
}
