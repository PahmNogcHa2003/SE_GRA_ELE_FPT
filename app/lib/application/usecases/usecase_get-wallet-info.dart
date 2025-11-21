import 'package:hola_bike_app/data/sources/remote/api_wallet.dart';
import 'package:hola_bike_app/domain/models/info_wallet.dart';

class GetWalletInfoUseCase {
  final WalletApi _api = WalletApi();

  Future<WalletInfo> execute(String token) {
    return _api.getWalletInfo(token);
  }
}
