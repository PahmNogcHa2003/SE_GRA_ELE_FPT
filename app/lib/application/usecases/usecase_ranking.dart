import 'package:hola_bike_app/data/sources/remote/api_ranking.dart';
import 'package:hola_bike_app/domain/models/info_ranking.dart';

class RankingUsecase {
  final RankingApi _api = RankingApi();

  Future<RankingEnd> execute({required String token}) async {
    return _api.getRanking(token: token);
  }
}
