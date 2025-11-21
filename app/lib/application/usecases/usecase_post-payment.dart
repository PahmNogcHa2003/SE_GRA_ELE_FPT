import 'package:hola_bike_app/data/sources/remote/api_payment.dart';
import 'package:hola_bike_app/domain/models/info_payment.dart';

class PostPaymentUseCase {
  final PaymentApi _api = PaymentApi();

  Future<PaymentInfo> execute({required double amount, required String token}) {
    return _api.createPaymentSession(amount: amount, token: token);
  }
}

class VerifyPaymentUseCase {
  final PaymentApi _api = PaymentApi();

  Future<PaymentResult> execute({
    required String returnUrl,
    required String token,
  }) {
    return _api.verifyPayment(returnUrl: returnUrl, token: token);
  }
}
