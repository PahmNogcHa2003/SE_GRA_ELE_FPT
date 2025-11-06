import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:hola_bike_app/presentation/auth/login/login_controller.dart';
import 'package:hola_bike_app/presentation/auth/login/login_form.dart';
import 'package:hola_bike_app/presentation/auth/register/register_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class LoginPage extends StatelessWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => LoginController(),
      child: Scaffold(
        // Không dùng AppBar, full screen
        body: Container(
          width: double.infinity,
          height: double.infinity,

          child: Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 24),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  // Logo
                  Image.asset(
                    'assets/images/logo_green.png',
                    width: 250,
                    height: 250,
                  ),

                  const SizedBox(height: 8),

                  Text(
                    "Đăng nhập để tiếp tục hành trình EcoJourney 🌿",
                    textAlign: TextAlign.center,
                    style: TextStyle(fontSize: 14, color: AppColors.primary),
                  ),
                  const SizedBox(height: 32),

                  // Form đăng nhập
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: Colors.white.withOpacity(0.15),
                      borderRadius: BorderRadius.circular(16),
                    ),
                    child: const LoginForm(),
                  ),

                  const SizedBox(height: 24),

                  // Link đăng ký
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        'Chưa có tài khoản?',
                        style: TextStyle(color: AppColors.textSecondary),
                      ),
                      TextButton(
                        onPressed: () {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => const RegisterPage(),
                            ),
                          );
                        },
                        child: const Text(
                          'Đăng ký ngay',
                          style: TextStyle(
                            color: AppColors.primary,
                            fontWeight: FontWeight.bold,
                            decoration: TextDecoration.underline,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
