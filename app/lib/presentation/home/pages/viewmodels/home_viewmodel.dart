// lib/presentation/home/home_viewmodel.dart
import 'package:flutter/material.dart';
import 'package:hola_bike_app/application/usecases/get_user_info_usecase.dart';
import 'package:hola_bike_app/domain/models/user_info.dart';

class HomeViewModel extends ChangeNotifier {
  final GetUserInfoUseCase _useCase = GetUserInfoUseCase();

  UserInfo? userInfo;
  bool isLoading = false;
  String? error;

  Future<void> fetchUserInfo(String token) async {
    try {
      isLoading = true;
      notifyListeners();

      userInfo = await _useCase.execute(token);
      error = null;
    } catch (e) {
      error = e.toString();
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }
}
