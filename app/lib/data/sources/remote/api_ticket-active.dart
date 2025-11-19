import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_ticket-active.dart';
import 'package:http/http.dart' as http;

class UserTicketApi {
  /// Lấy danh sách vé đang hoạt động của người dùng
  Future<List<TicketInfo>> getActiveTickets(String token) async {
    final url = Uri.parse('$baseUrl/UserTicket/active');

    try {
      // --- Log request ---
      print('--- API REQUEST ---');
      print('GET $url');
      print('Token: $token');

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

      if (response.statusCode == 200) {
        try {
          final json = jsonDecode(response.body);
          final List<dynamic> items = json['data'];
          return items.map((e) => TicketInfo.fromJson(e)).toList();
        } catch (e) {
          print('❌ JSON decode error: $e');
          throw Exception('Không thể parse dữ liệu vé từ server');
        }
      } else {
        print('❌ Lấy vé thất bại với status: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lỗi khi lấy vé: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API getActiveTickets: $e');
      rethrow;
    }
  }
}
