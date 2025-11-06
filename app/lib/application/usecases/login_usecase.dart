import '../../data/sources/remote/auth_api.dart';
import '../../domain/models/login_response.dart';

class LoginUseCase {
  final AuthApi _api = AuthApi();

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
