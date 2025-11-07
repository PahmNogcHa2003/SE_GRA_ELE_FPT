import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_payment.dart';
import 'package:http/http.dart' as http;

class PaymentApi {
  Future<PaymentInfo> createPaymentSession({
    required double amount,
    required String token,
  }) async {
    final url = Uri.parse('$baseUrl/payments/vnpay/create-url');

    final response = await http.post(
      url,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
      body: '{"amount": $amount}',
    );

    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      return PaymentInfo.fromJson(data);
    } else {
      throw Exception('Failed to create payment session');
    }
  }

  Future<PaymentResult> verifyPayment({
    required String returnUrl,
    required String token,
  }) async {
    final url = Uri.parse('$baseUrl/Payments/vnpay-return?$returnUrl');

    final response = await http.get(
      url,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer $token',
      },
    );

    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      return PaymentResult.fromJson(data);
    } else {
      throw Exception('Failed to verify payment');
    }
  }
}
