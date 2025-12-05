import 'package:hola_bike_app/data/sources/remote/api_profile.dart';
import 'package:hola_bike_app/domain/models/info_profile.dart';

class ProfileUsecase {
  final ProfileApi _api = ProfileApi();

  Future<InfoProfile> execute({required String token}) async {
    return _api.getProfile(token: token);
  }
}
