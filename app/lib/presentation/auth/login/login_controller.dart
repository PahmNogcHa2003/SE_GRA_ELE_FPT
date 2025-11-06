import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_login.dart';
import 'package:hola_bike_app/presentation/home/home_screen.dart';

class LoginController extends ChangeNotifier {
  final formKey = GlobalKey<FormState>();
  final emailController = TextEditingController();
  final passwordController = TextEditingController();

  final loginUseCase = LoginUseCase();
  final secureStorage = const FlutterSecureStorage();

  bool isLoading = false;

  Future<void> handleLogin(BuildContext context) async {
    if (!formKey.currentState!.validate()) return;
    FocusScope.of(context).unfocus();
    isLoading = true;
    EasyLoading.show();
    notifyListeners();

    try {
      final response = await loginUseCase.execute(
        email: emailController.text.trim(),
        password: passwordController.text.trim(),
        deviceId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        pushToken: 'push-token-demo',
        platform: 'android',
      );

      if (response.success) {
        await secureStorage.write(key: 'access_token', value: response.token);
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => HomeScreen()),
        );
      } else {
        _showSnack(context, response.message);
      }
    } catch (e) {
      _showSnack(context, 'Đăng nhập thất bại. Vui lòng thử lại.');
    } finally {
      EasyLoading.dismiss();
    }

    isLoading = false;
    notifyListeners();
  }

  void _showSnack(BuildContext context, String msg) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(msg)));
  }

  @override
  void dispose() {
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }
}
