import 'package:hola_bike_app/data/sources/remote/api_ticket-active.dart';
import 'package:hola_bike_app/domain/models/info_ticket-active.dart';

class TicketActiveUsecase {
  final UserTicketApi _api = UserTicketApi();

  Future<List<TicketInfo>> execute(String token) {
    return _api.getActiveTickets(token);
  }
}
