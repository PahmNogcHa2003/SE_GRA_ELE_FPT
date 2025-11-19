import 'dart:convert';
import 'package:hola_bike_app/core/constants/api_constants.dart';
import 'package:hola_bike_app/domain/models/info_wallet-transaction.dart';
import 'package:http/http.dart' as http;

class WalletTransactionApi {
  Future<List<WalletTransactionInfo>> getWalletTransactions({
    int page = 1,
    int pageSize = 20,
    String sortOrder = 'createdAt_desc',
    String? token,
  }) async {
    final url = Uri.parse(
      '$baseUrl/wallet/transactions?page=$page&pageSize=$pageSize&sortOrder=$sortOrder',
    );

    try {
      print('--- API REQUEST ---');
      print('GET $url');
      print('Token: $token');

      final response = await http
          .get(
            url,
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer $token',
            },
          )
          .timeout(const Duration(seconds: 10));

      print('--- API RESPONSE ---');
      print('Status code: ${response.statusCode}');
      print('Body: ${response.body}');

      if (response.statusCode == 200) {
        final Map<String, dynamic> jsonData = jsonDecode(response.body);

        if (jsonData['success'] == true &&
            jsonData['data'] != null &&
            jsonData['data']['items'] != null) {
          final List<dynamic> items = jsonData['data']['items'];
          return items.map((e) => WalletTransactionInfo.fromJson(e)).toList();
        } else {
          throw Exception('Dữ liệu phản hồi không hợp lệ.');
        }
      } else {
        throw Exception(
          'Lỗi khi gọi API: ${response.statusCode}, ${response.body}',
        );
      }
    } catch (e) {
      print('Error calling getWalletTransactions API: $e');
      rethrow;
    }
  }
}
