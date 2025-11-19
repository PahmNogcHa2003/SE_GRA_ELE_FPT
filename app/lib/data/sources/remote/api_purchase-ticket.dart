import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:hola_bike_app/core/constants/api_constants.dart';

class PurchaseTicketApi {
  Future<void> purchaseTicket(String token, int planPriceId) async {
    final url = Uri.parse('$baseUrl/UserTicket/purchase');

    final response = await http.post(
      url,
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
      body: jsonEncode({'planPriceId': planPriceId}),
    );

    if (response.statusCode == 200 || response.statusCode == 201) {
      // Thành công
      return;
    } else {
      // Thất bại
      throw Exception('Lỗi mua vé: ${response.body}');
    }
  }
}
