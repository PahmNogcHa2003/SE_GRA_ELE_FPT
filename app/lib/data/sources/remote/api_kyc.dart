import 'dart:convert';

import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_kyc.dart';
import 'package:http/http.dart' as http;
import 'package:image_picker/image_picker.dart';

class KycApi {
  /// Gửi thông tin KYC gồm chuỗi JsonData (phân tách bằng '|') và 2 ảnh
  Future<KycInfo> submitKyc({
    required String token,
    required String jsonDataString, // chuỗi có dạng "A|B|C|..."
    required XFile frontImage,
    required XFile backImage,
  }) async {
    final url = Uri.parse('$baseUrl/Kycs/submit-images');

    try {
      print('--- API REQUEST ---');
      print('POST $url');
      print('JsonData: $jsonDataString');
      print('FrontImage path: ${frontImage.path}');
      print('BackImage path: ${backImage.path}');

      final request = http.MultipartRequest('POST', url)
        ..headers['Authorization'] = 'Bearer $token'
        ..fields['JsonData'] = jsonDataString
        ..files.add(
          await http.MultipartFile.fromPath('FrontImage', frontImage.path),
        )
        ..files.add(
          await http.MultipartFile.fromPath('BackImage', backImage.path),
        );

      final streamedResponse = await request.send().timeout(
        const Duration(seconds: 15),
      );
      final response = await http.Response.fromStream(streamedResponse);

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        print('✅ Gửi KYC thành công');
        return KycInfo.fromJson(jsonDecode(response.body));
        ;
      } else {
        print('❌ Gửi KYC thất bại: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Gửi KYC thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API KYC: $e');
      rethrow;
    }
  }
}
