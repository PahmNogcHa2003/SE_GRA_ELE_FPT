import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:http/http.dart' as http;
import 'package:hola_bike_app/domain/models/info_register.dart';

class RegisterApi {
  Future<RegisterResponse> register({
    required String phoneNumber,
    required String fullName,
    required String identityNumber,
    required String emergencyName,
    required String emergencyPhone,
    required int provinceId,
    required String provinceName,
    required int wardId,
    required String wardName,
    required String address,
    required String email,
    required String gender,
    required DateTime dateOfBirth,
    required String password,
    required String confirmPassword,
  }) async {
    final url = Uri.parse('$baseUrl/Auth/register');

    final body = jsonEncode({
      "phoneNumber": phoneNumber,
      "fullName": fullName,
      "identityNumber": identityNumber,
      "emergencyName": emergencyName,
      "emergencyPhone": emergencyPhone,
      "provinceId": provinceId,
      "provinceName": provinceName,
      "wardId": wardId,
      "wardName": wardName,
      "address": address,
      "email": email,
      "gender": gender,
      "dateOfBirth": dateOfBirth.toIso8601String(),
      "password": password,
      "confirmPassword": confirmPassword,
    });

    try {
      // Log request
      print('--- API REQUEST ---');
      print('POST $url');
      print('BODY: $body');

      // Gửi request với timeout
      final response = await http
          .post(url, headers: {'Content-Type': 'application/json'}, body: body)
          .timeout(const Duration(seconds: 10));

      // Log response
      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      // Kiểm tra kết quả
      if (response.statusCode == 200 || response.statusCode == 201) {
        return RegisterResponse.fromJson(jsonDecode(response.body));
      } else {
        throw Exception(
          'Đăng ký thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('Error calling register API: $e');
      rethrow;
    }
  }
}
