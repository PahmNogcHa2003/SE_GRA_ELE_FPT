import 'package:flutter/material.dart';
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

  @override
  Widget build(BuildContext context) {
    final isRegisterEnabled = _formValid && _agreePolicy;

    Future<void> _handleRegister() async {
      // Gọi API đăng ký ở đây
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Đăng ký thành công!'),
          backgroundColor: Colors.green,
        ),
      );
    }

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
              onFormValidChanged: (valid) => setState(() => _formValid = valid),
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
                onPressed: isRegisterEnabled
                    ? () {
                        _handleRegister();
                      }
                    : null,
                child: const Text('Đăng ký'),
              ),
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Text(
                  'Đã có tài khoản? ',
                  style: TextStyle(color: Colors.black),
                ),
                TextButton(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(builder: (_) => const LoginScreen()),
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
