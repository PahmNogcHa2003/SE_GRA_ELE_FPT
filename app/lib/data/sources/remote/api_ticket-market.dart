import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_ticket-market.dart';
import 'package:http/http.dart' as http;

class MarketTicketApi {
  /// Lấy danh sách vé market theo loại phương tiện (bike/electric)
  Future<List<Ticket>> getMarketTickets(
    String token,
    String vehicleType,
  ) async {
    final url = Uri.parse(
      '$baseUrl/UserTicket/market?vehicleType=$vehicleType',
    );

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
          return items.map((e) => Ticket.fromJson(e)).toList();
        } catch (e) {
          print('❌ JSON decode error: $e');
          throw Exception('Không thể parse dữ liệu vé market từ server');
        }
      } else {
        print('❌ Lấy vé market thất bại với status: ${response.statusCode}');
        print('Response body: ${response.body}');
        throw Exception(
          'Lỗi khi lấy vé market: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('🔥 Lỗi khi gọi API getMarketTickets: $e');
      rethrow;
    }
  }
}
