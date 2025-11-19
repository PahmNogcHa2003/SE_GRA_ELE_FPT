import 'package:hola_bike_app/data/sources/remote/api_purchase-ticket.dart';

class PurchaseTicketUsecase {
  final PurchaseTicketApi _api = PurchaseTicketApi();

  /// Thực hiện mua vé với planPriceId và token
  Future<void> execute(String token, int planPriceId) {
    return _api.purchaseTicket(token, planPriceId);
  }
}
