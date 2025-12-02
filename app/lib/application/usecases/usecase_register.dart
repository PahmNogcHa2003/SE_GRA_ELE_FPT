import 'package:hola_bike_app/data/sources/remote/api_register.dart';
import 'package:hola_bike_app/domain/models/info_register.dart';

class ResgisterUsecase {
  final RegisterApi _api = RegisterApi();

  Future<RegisterResponse> execute({
    required String phoneNumber,
    required String fullName,
    required String identityNumber,
    required String emergencyName,
    required String emergencyPhone,
    required int provinceId,
    required String provinceName,
    required int wardId,
    required String wardName,
    required String address,
    required String email,
    required String gender,
    required DateTime dateOfBirth,
    required String password,
    required String confirmPassword,
  }) {
    return _api.register(
      phoneNumber: phoneNumber,
      fullName: fullName,
      identityNumber: identityNumber,
      emergencyName: emergencyName,
      emergencyPhone: emergencyPhone,
      provinceId: provinceId,
      provinceName: provinceName,
      wardId: wardId,
      wardName: wardName,
      address: address,
      email: email,
      gender: gender,
      dateOfBirth: dateOfBirth,
      password: password,
      confirmPassword: confirmPassword,
    );
  }
}
