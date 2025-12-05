import 'dart:convert';
import 'dart:ffi';
import 'dart:io';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:http/http.dart' as http;

class UserProfileApi {
  Future<bool> updateUserProfile({
    required String token,
    required String fullName,
    required String emergencyName,
    required String emergencyPhone,
    required String addressDetail,
    required String phoneNumber,
    required String gender,
    required String dob,
  }) async {
    final url = Uri.parse('$baseUrl/UserProfiles/me');
    final body = jsonEncode({
      "fullName": fullName,
      "emergencyName": emergencyName,
      "emergencyPhone": emergencyPhone,
      "addressDetail": addressDetail,
      "phoneNumber": phoneNumber,
      "gender": gender,
      "dob": dob,
    });

    try {
      print('--- API REQUEST ---');
      print('PUT $url');

      final response = await http.put(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: body,
      );

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        return true;
      }
    } catch (e) {
      throw Exception('Error updating user profile: $e');
    }
    return false;
  }

  Future<bool> uploadAvatar({
    required String token,
    required File avatarFile,
  }) async {
    final url = Uri.parse('$baseUrl/UserProfiles/me/avatar');

    try {
      print('--- API REQUEST ---');
      print('POST $url');

      final request = http.MultipartRequest('POST', url)
        ..headers['Authorization'] = 'Bearer $token'
        ..files.add(await http.MultipartFile.fromPath('file', avatarFile.path));

      final response = await request.send();

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      final responseBody = await response.stream.bytesToString();
      print('Body: $responseBody');

      if (response.statusCode == 200) {
        return true;
      }
    } catch (e) {
      throw Exception('Error uploading avatar: $e');
    }
    return false;
  }
}
