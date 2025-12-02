import 'package:hola_bike_app/data/sources/remote/api_wallet-transaction.dart';
import 'package:hola_bike_app/domain/models/info_wallet-transaction.dart';

class WalletTransactionUseCase {
  final WalletTransactionApi _api = WalletTransactionApi();

  Future<List<WalletTransactionInfo>> execute({
    int page = 1,
    int pageSize = 100,
    String sortOrder = 'createdAt_desc',
    String? token,
  }) {
    return _api.getWalletTransactions(
      page: page,
      pageSize: pageSize,
      sortOrder: sortOrder,
      token: token,
    );
  }
}
