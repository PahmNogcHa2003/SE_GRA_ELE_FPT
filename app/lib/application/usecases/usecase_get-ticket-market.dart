import 'package:hola_bike_app/data/sources/remote/api_ticket-market.dart';
import 'package:hola_bike_app/domain/models/info_ticket-market.dart';

class MarketTicketUsecase {
  final MarketTicketApi _api = MarketTicketApi();

  /// Lấy danh sách vé market theo loại phương tiện
  Future<List<Ticket>> execute(String token, String vehicleType) {
    return _api.getMarketTickets(token, vehicleType);
  }
}
