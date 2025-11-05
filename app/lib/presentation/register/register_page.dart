import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/login/login_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class RegisterPage extends StatelessWidget {
  const RegisterPage({super.key});

  @override
  Widget build(BuildContext context) {
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
            _buildTextField(label: 'Họ và tên *'),
            _buildTextField(
              label: 'Số điện thoại *',
              keyboardType: TextInputType.phone,
            ),
            _buildTextField(label: 'CCCD/Passport *'),
            _buildTextField(label: 'Tỉnh/TP (Nơi ở hiện tại) *'),
            _buildTextField(label: 'Xã/Phường (Nơi ở hiện tại) *'),
            _buildTextField(label: 'Địa chỉ *'),
            _buildTextField(
              label: 'Email',
              keyboardType: TextInputType.emailAddress,
            ),
            _buildDropdown(label: 'Giới tính', items: ['Nam', 'Nữ', 'Khác']),
            _buildTextField(label: 'Ngày sinh', hint: 'DD/MM/YYYY'),
            _buildTextField(label: 'Mật khẩu', obscureText: true),
            _buildTextField(label: 'Nhập lại mật khẩu', obscureText: true),
            const SizedBox(height: 16),
            Row(
              children: [
                Checkbox(value: true, onChanged: (_) {}),
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
                  backgroundColor: AppColors.primary,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                ),
                onPressed: () {
                  // Xử lý đăng ký
                },
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
                  style: TextButton.styleFrom(
                    padding: EdgeInsets.zero, // bỏ khoảng cách
                    minimumSize: Size(0, 0), // bỏ kích thước tối thiểu
                    tapTargetSize:
                        MaterialTapTargetSize.shrinkWrap, // thu gọn vùng chạm
                  ),
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
                      decoration: TextDecoration.underline, // giống link web
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

  Widget _buildTextField({
    required String label,
    String? hint,
    TextInputType keyboardType = TextInputType.text,
    bool obscureText = false,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextField(
        keyboardType: keyboardType,
        obscureText: obscureText,
        decoration: InputDecoration(
          labelText: label,
          hintText: hint,
          filled: true,
          fillColor: AppColors.card,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
        ),
      ),
    );
  }

  Widget _buildDropdown({required String label, required List<String> items}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        decoration: InputDecoration(
          labelText: label,
          filled: true,
          fillColor: AppColors.card,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
        ),
        items: items.map((item) {
          return DropdownMenuItem(value: item, child: Text(item));
        }).toList(),
        onChanged: (value) {},
      ),
    );
  }
}
