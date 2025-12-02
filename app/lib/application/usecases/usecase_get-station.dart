import 'package:hola_bike_app/data/sources/remote/api_station.dart';
import 'package:hola_bike_app/domain/models/info_station.dart';

class GetStationInfoUsecase {
  final StationApi _api = StationApi();
  Future<List<StationInfo>> execute(String token) {
    return _api.getStationInfo(token);
  }
}
