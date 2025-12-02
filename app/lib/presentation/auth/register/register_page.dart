import 'package:flutter/material.dart';
import 'package:hola_bike_app/data/sources/remote/api_register.dart';
import 'package:hola_bike_app/presentation/auth/login/login_page.dart';
import 'package:hola_bike_app/presentation/auth/register/register_form.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  bool _formValid = false;
  bool _agreePolicy = false;
  Map<String, dynamic>? _formData;

  final _registerApi = RegisterApi();

  Future<void> _handleRegister() async {
    if (_formData == null) return;

    try {
      final response = await _registerApi.register(
        phoneNumber: _formData!['phoneNumber'],
        fullName: _formData!['fullName'],
        identityNumber: _formData!['identityNumber'],
        emergencyName: _formData!['emergencyName'],
        emergencyPhone: _formData!['emergencyPhone'],
        provinceId: _formData!['provinceId'],
        provinceName: _formData!['provinceName'],
        wardId: _formData!['wardId'],
        wardName: _formData!['wardName'],
        address: _formData!['address'],
        email: _formData!['email'],
        gender: _formData!['gender'],
        dateOfBirth: _formData!['dateOfBirth'],
        password: _formData!['password'],
        confirmPassword: _formData!['confirmPassword'],
      );

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Đăng ký thành công! ${response.message}'),
            backgroundColor: Colors.green,
          ),
        );
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => const LoginPage()),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Đăng ký thất bại: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isRegisterEnabled = _formValid && _agreePolicy;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Đăng ký'),
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            RegisterForm(
              onFormValidChanged: (valid, data) {
                setState(() {
                  _formValid = valid;
                  _formData = data;
                });
              },
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Checkbox(
                  value: _agreePolicy,
                  onChanged: (value) =>
                      setState(() => _agreePolicy = value ?? false),
                ),
                const Expanded(
                  child: Text(
                    'Tôi đồng ý với Điều khoản sử dụng và Quy định chính sách',
                    style: TextStyle(fontSize: 14),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                style: ElevatedButton.styleFrom(
                  backgroundColor: isRegisterEnabled
                      ? AppColors.primary
                      : Colors.grey,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                ),
                onPressed: isRegisterEnabled ? _handleRegister : null,
                child: const Text('Đăng ký'),
              ),
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Text('Đã có tài khoản? '),
                TextButton(
                  onPressed: () {
                    Navigator.pushReplacement(
                      context,
                      MaterialPageRoute(builder: (_) => const LoginPage()),
                    );
                  },
                  child: const Text(
                    'Đăng nhập',
                    style: TextStyle(
                      color: AppColors.primary,
                      decoration: TextDecoration.underline,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
