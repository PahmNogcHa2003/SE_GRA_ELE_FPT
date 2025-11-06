// lib/application/usecases/get_user_info_usecase.dart
import '../../data/sources/remote/user_api.dart';
import '../../domain/models/user_info.dart';

class GetUserInfoUseCase {
  final UserApi _api = UserApi();

  Future<UserInfo> execute(String token) {
    return _api.getUserInfo(token);
  }
}
