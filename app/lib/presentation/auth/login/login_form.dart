import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/auth/login/widgets/widget_password-field.dart';
import 'package:provider/provider.dart';
import 'package:hola_bike_app/presentation/auth/login/login_controller.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class LoginForm extends StatelessWidget {
  const LoginForm({super.key});

  @override
  Widget build(BuildContext context) {
    final controller = context.watch<LoginController>();

    return Form(
      key: controller.formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          TextFormField(
            controller: controller.emailController,
            decoration: const InputDecoration(labelText: "Email"),
            validator: (value) {
              if (value == null || value.isEmpty) return "Vui lòng nhập email";
              final emailRegex = RegExp(r'^[^@]+@[^@]+\.[^@]+$');
              if (!emailRegex.hasMatch(value)) return "Email không hợp lệ";
              return null;
            },
          ),
          const SizedBox(height: 16),
          PasswordField(controller: controller.passwordController),
          const SizedBox(height: 30),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.primary,
              minimumSize: const Size(double.infinity, 48),
            ),
            onPressed: controller.isLoading
                ? null
                : () => controller.handleLogin(context),
            child: controller.isLoading
                ? const CircularProgressIndicator(color: Colors.white)
                : const Text(
                    "Đăng nhập",
                    style: TextStyle(fontSize: 16, color: Colors.white),
                  ),
          ),
        ],
      ),
    );
  }
}
