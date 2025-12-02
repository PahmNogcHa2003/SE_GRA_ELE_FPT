import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_payment.dart';
import 'package:http/http.dart' as http;

class PaymentApi {
  Future<PaymentInfo> createPaymentSession({
    required double amount,
    required String token,
  }) async {
    // API endpoint to create a VNPay payment session

    final url = Uri.parse('$baseUrl/payments/vnpay/create-url');

    try {
      // Log request
      print('--- API REQUEST ---');
      print('GET $url');
      final response = await http.post(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: '{"amount": $amount}',
      );
      // Log response
      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        return PaymentInfo.fromJson(data);
      } else {
        throw Exception('Failed to create payment session');
      }
    } catch (e) {
      throw Exception('Failed to create payment session: $e');
    }
  }

  Future<PaymentResult> verifyPayment({
    required String returnUrl,
    required String token,
  }) async {
    final url = Uri.parse('$baseUrl/Payments/vnpay-return?$returnUrl');
    try {
      print('--- API REQUEST ---');
      print('GET $url');
      final response = await http.get(
        url,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );
      // Log response
      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');
      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        return PaymentResult.fromJson(data);
      } else {
        throw Exception('Failed to verify payment');
      }
    } catch (e) {
      throw Exception('Failed to verify payment: $e');
    }
  }
}
