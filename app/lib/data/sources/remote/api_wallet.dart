import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_wallet.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class WalletApi {
  Future<WalletInfo> getWalletInfo(String token) async {
    final url = Uri.parse('$baseUrl/api/wallets');
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

      // --- Xử lý kết quả ---
      if (response.statusCode == 200) {
        try {
          final json = jsonDecode(response.body);
          final data = json['data'];
          return WalletInfo.fromJson(data);
        } catch (e) {
          print('JSON decode error: $e');
          throw Exception('Không thể parse dữ liệu ví từ server');
        }
      } else {
        print('Lấy thông tin ví thất bại với status: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lỗi khi lấy thông tin ví: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('Error calling getWalletInfo API: $e');
      rethrow; // cho phép hàm gọi bên ngoài xử lý tiếp
    }
  }
}
