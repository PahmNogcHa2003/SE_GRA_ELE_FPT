import 'package:http/http.dart' as http;
import 'dart:convert';
import '../models/rental_request.dart';

class BikeRentalApi {
  final String baseUrl =
      'http://bookingbicycle.somee.com/api/Bookings'; // sửa lại đúng endpoint

  Future<bool> submitRental(RentalRequest request) async {
    try {
      final response = await http.post(
        Uri.parse(baseUrl),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(request.toJson()),
      );

      print('📡 Status: ${response.statusCode}');
      print('📦 Body: ${response.body}');

      if (response.statusCode == 200 || response.statusCode == 201) {
        return true;
      } else {
        print('⚠️ Server trả về lỗi: ${response.statusCode}');
        return false;
      }
    } catch (e) {
      print('❌ Lỗi khi gửi yêu cầu: $e');
      return false;
    }
  }
}
