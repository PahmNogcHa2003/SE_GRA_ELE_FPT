import '../../data/sources/remote/api_login.dart';
import '../../domain/models/info_login.dart';

class LoginUseCase {
  final LoginApi _api = LoginApi();

  Future<LoginResponse> execute({
    required String email,
    required String password,
    required String deviceId,
    required String pushToken,
    required String platform,
  }) {
    return _api.login(
      email: email,
      password: password,
      deviceId: deviceId,
      pushToken: pushToken,
      platform: platform,
    );
  }
}
