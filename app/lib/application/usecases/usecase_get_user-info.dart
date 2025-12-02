// lib/application/usecases/get_user_info_usecase.dart
import '../../data/sources/remote/api_user.dart';
import '../../domain/models/info_user.dart';

class GetUserInfoUseCase {
  final UserApi _api = UserApi();

  Future<UserInfo> execute(String token) {
    return _api.getUserInfo(token);
  }
}
