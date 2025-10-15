import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class MorePage extends StatelessWidget {
  const MorePage({super.key});

  @override
  Widget build(BuildContext context) {
    final List<(String, IconData)> items = [
      ("Xác thực tài khoản", Icons.verified_user),
      ("Bảo mật", Icons.lock),
      ("Đổi mật khẩu", Icons.password),
      ("Thông báo", Icons.notifications),
      ("Vô hiệu hoá tài khoản", Icons.block),
      ("Xoá tài khoản", Icons.delete_forever),
      ("Đăng xuất", Icons.logout),
      ("Bảng giá", Icons.price_change),
      ("Về Hola Bike", Icons.info_outline),
      ("Hướng dẫn sử dụng", Icons.help_outline),
      ("Điều khoản sử dụng", Icons.article),
      ("Quy định chính sách", Icons.policy),
      ("Website Hola Bike", Icons.language),
      ("Hỗ trợ", Icons.support_agent),
    ];

    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // 🔷 Header
            Container(
              color: AppColors.primary.withOpacity(0.1),
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 20),
              child: Row(
                children: [
                  const CircleAvatar(
                    radius: 24,
                    backgroundColor: AppColors.primary,
                    child: Icon(Icons.person, color: Colors.white),
                  ),
                  const SizedBox(width: 12),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: const [
                      Text(
                        "Nguyễn Quang Bích",
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      SizedBox(height: 4),
                      Text(
                        "0944254867",
                        style: TextStyle(color: AppColors.textSecondary),
                      ),
                    ],
                  ),
                ],
              ),
            ),

            // 🔷 Menu
            Expanded(
              child: ListView.separated(
                padding: const EdgeInsets.symmetric(vertical: 8),
                itemCount: items.length,
                separatorBuilder: (_, __) => const Divider(height: 1),
                itemBuilder: (context, index) {
                  final (label, icon) = items[index];
                  return ListTile(
                    leading: Icon(icon, color: AppColors.primary),
                    title: Text(label),
                    trailing: const Icon(Icons.chevron_right),
                    onTap: () {
                      // TODO: xử lý chức năng khi nhấn
                    },
                  );
                },
              ),
            ),

            // 🔷 Phiên bản
            const SizedBox(height: 8),

            const SizedBox(height: 8),
          ],
        ),
      ),
    );
  }
}
