import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_provice.dart';
import 'package:http/http.dart' as http;

class ProvinceApi {
  /// Lấy danh sách tỉnh/thành từ API
  Future<List<Province>> getProvinces() async {
    final url = Uri.parse('$baseUrl/Locations/provinces');

    try {
      // Log request
      print('--- API REQUEST ---');
      print('GET $url');

      // Gửi request
      final response = await http
          .get(url, headers: {'Content-Type': 'application/json'})
          .timeout(const Duration(seconds: 10));

      // Log response
      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      // Xử lý phản hồi
      if (response.statusCode == 200) {
        try {
          final List<dynamic> jsonList = jsonDecode(response.body);
          final List<Province> provinces = jsonList
              .map((e) => Province.fromJson(e))
              .toList();
          return provinces;
        } catch (e) {
          print('JSON decode error: $e');
          throw Exception('Không thể parse dữ liệu từ server');
        }
      } else {
        print('Get provinces failed with status: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lấy danh sách tỉnh thất bại: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('Error calling getProvinces API: $e');
      rethrow; // Cho phép hàm gọi bên ngoài xử lý tiếp
    }
  }
}
