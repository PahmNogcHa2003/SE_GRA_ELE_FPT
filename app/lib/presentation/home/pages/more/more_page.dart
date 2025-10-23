import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/more/page/edit_profile_page.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class MorePage extends StatelessWidget {
  const MorePage({super.key});

  @override
  Widget build(BuildContext context) {
    final List<Map<String, dynamic>> accountItems = [
      {
        'icon': Icons.verified_user,
        'label': 'Xác thực tài khoản',
        'router': '/verify',
      },
      {
        'icon': Icons.password,
        'label': 'Đổi mật khẩu',
        'router': '/change-password',
      },
      {
        'icon': Icons.delete_forever,
        'label': 'Xoá tài khoản',
        'router': '/delete-account',
      },
      {'icon': Icons.logout, 'label': 'Đăng xuất', 'router': '/logout'},
    ];

    final List<Map<String, dynamic>> infoItems = [
      {'icon': Icons.price_change, 'label': 'Bảng giá', 'router': '/pricing'},
      {
        'icon': Icons.help_outline,
        'label': 'Hướng dẫn sử dụng',
        'router': '/guide',
      },
      {
        'icon': Icons.policy,
        'label': 'Quy định chính sách',
        'router': '/policy',
      },
      {
        'icon': Icons.language,
        'label': 'Website EcoJourney',
        'router': '/website',
      },
      {'icon': Icons.support_agent, 'label': 'Hỗ trợ', 'router': '/support'},
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
                          "EcoJourney",
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
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => const EditProfilePage(),
                        ),
                      );
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
                  ...accountItems.map(
                    (item) => _buildRouterItem(context, item),
                  ),

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
                        "Về EcoJourney",
                        style: TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w600,
                          color: AppColors.primary,
                        ),
                      ),
                    ),
                  ),

                  // 🔷 Danh sách thông tin ứng dụng
                  ...infoItems.map((item) => _buildRouterItem(context, item)),
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

  Widget _buildRouterItem(BuildContext context, Map<String, dynamic> item) {
    return Column(
      children: [
        ListTile(
          leading: Icon(item['icon'], color: AppColors.primary),
          title: Text(item['label'], style: const TextStyle(fontSize: 14)),
          trailing: const Icon(Icons.chevron_right),
          onTap: () {
            final route = item['router'] as String;
            if (route == '/logout') {
              _handleLogout(context);
            } else {
              Navigator.pushNamed(context, route);
            }
          },
        ),
        const Divider(height: 1),
      ],
    );
  }

  void _handleLogout(BuildContext context) {
    // TODO: xử lý đăng xuất, ví dụ: xóa token, clear session, chuyển về màn đăng nhập
    Navigator.pushNamedAndRemoveUntil(context, '/login', (route) => false);
  }
}
