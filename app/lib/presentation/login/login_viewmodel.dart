import 'package:flutter/material.dart';
import '../../../application/usecases/login_usecase.dart';
import '../../../domain/models/login_response.dart';

class LoginViewModel extends ChangeNotifier {
  final LoginUseCase _useCase = LoginUseCase();

  bool isLoading = false;
  String? errorMessage;

  Future<LoginResponse?> login(String email, String password) async {
    try {
      isLoading = true;
      notifyListeners();

      final response = await _useCase.execute(
        email: email,
        password: password,
        deviceId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        pushToken: 'your-push-token',
        platform: 'android',
      );

      isLoading = false;
      notifyListeners();

      return response;
    } catch (e) {
      isLoading = false;
      errorMessage = e.toString();
      notifyListeners();
      return null;
    }
  }
}
