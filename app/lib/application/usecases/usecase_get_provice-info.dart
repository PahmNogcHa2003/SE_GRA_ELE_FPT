import 'package:hola_bike_app/data/sources/remote/api_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_wards.dart';
import 'package:hola_bike_app/domain/models/info_provice.dart';

class GetProviderInfoUseCase {
  final ProvinceApi _api = ProvinceApi();

  Future<List<Province>> execute() {
    return _api.getProvinces();
  }
}

class GetWardByProvice {
  final WardsApi _api = WardsApi();
  Future<List<Province>> execute(String code) {
    return _api.GetWardByProvice(code);
  }
}
