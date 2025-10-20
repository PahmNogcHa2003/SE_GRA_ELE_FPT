import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class MorePage extends StatelessWidget {
  const MorePage({super.key});

  @override
  Widget build(BuildContext context) {
    final List<(String, IconData)> accountItems = [
      ("Xác thực tài khoản", Icons.verified_user),
      ("Bảo mật", Icons.lock),
      ("Đổi mật khẩu", Icons.password),
      ("Thông báo", Icons.notifications),
      ("Vô hiệu hoá tài khoản", Icons.block),
      ("Xoá tài khoản", Icons.delete_forever),
      ("Đăng xuất", Icons.logout),
    ];

    final List<(String, IconData)> infoItems = [
      ("Bảng giá", Icons.price_change),
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
          children: [
            // 🔷 Header người dùng
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 28),
              decoration: BoxDecoration(
                color: AppColors.primary.withOpacity(0.1),
                border: const Border(bottom: BorderSide(color: Colors.black12)),
              ),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  const CircleAvatar(
                    radius: 32,
                    backgroundColor: AppColors.primary,
                    child: Icon(Icons.person, color: Colors.white, size: 32),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: const [
                        Text(
                          "Hola Bike",
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        SizedBox(height: 6),
                        Text(
                          "0987654321",
                          style: TextStyle(
                            fontSize: 14,
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.edit, color: AppColors.primary),
                    tooltip: "Chỉnh sửa thông tin",
                    onPressed: () {
                      // TODO: mở trang chỉnh sửa thông tin
                    },
                  ),
                ],
              ),
            ),

            // 🔷 Danh sách chức năng tài khoản
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(vertical: 12),
                children: [
                  ...accountItems.map((item) => _buildItem(item)),

                  // 🔷 Ngăn cách
                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    child: Container(
                      color: AppColors.primary.withOpacity(0.05),
                      padding: const EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 8,
                      ),
                      child: const Text(
                        "Về Hola Bike",
                        style: TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w600,
                          color: AppColors.primary,
                        ),
                      ),
                    ),
                  ),

                  // 🔷 Danh sách thông tin ứng dụng
                  ...infoItems.map((item) => _buildItem(item)),
                ],
              ),
            ),

            // 🔷 Phiên bản app
            Container(
              padding: const EdgeInsets.symmetric(vertical: 12),
              alignment: Alignment.center,
              child: const Text(
                "Phiên bản 1.0.0",
                style: TextStyle(fontSize: 12, color: AppColors.textSecondary),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildItem((String, IconData) item) {
    final (label, icon) = item;
    return Column(
      children: [
        ListTile(
          leading: Icon(icon, color: AppColors.primary),
          title: Text(label, style: const TextStyle(fontSize: 14)),
          trailing: const Icon(Icons.chevron_right),
          onTap: () {
            // TODO: xử lý chức năng khi nhấn
          },
        ),
        const Divider(height: 1),
      ],
    );
  }
}
