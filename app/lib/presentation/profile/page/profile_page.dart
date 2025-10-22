import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/profile/page/edit_profile_page.dart';

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Hồ sơ cá nhân'),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => const EditProfilePage(),
                ),
              );
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            // Avatar và tên
            Center(
              child: Column(
                children: const [
                  CircleAvatar(radius: 50, child: Icon(Icons.person, size: 50)),
                  SizedBox(height: 12),
                  Text(
                    'EcoJourney',
                    style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // Card thống kê
            Card(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              elevation: 4,
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  vertical: 16,
                  horizontal: 8,
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: const [
                    StatBox(label: 'Km đã đạp', value: '0 km'),
                    StatBox(label: 'Thời gian', value: '0 giờ'),
                    StatBox(label: 'Chuyến đi', value: '0 chuyến'),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),

            // Thông tin cá nhân
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.grey.shade100,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: const [
                  InfoRow(
                    icon: Icons.email,
                    label: 'Email',
                    value: 'ecojourney@gmail.com',
                  ),
                  SizedBox(height: 12),
                  InfoRow(
                    icon: Icons.cake,
                    label: 'Ngày sinh',
                    value: '01/01/2025',
                  ),
                  SizedBox(height: 12),
                  InfoRow(icon: Icons.home, label: 'Địa chỉ', value: 'FPT'),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class StatBox extends StatelessWidget {
  final String label;
  final String value;

  const StatBox({required this.label, required this.value, super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          value,
          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 4),
        Text(label, style: const TextStyle(fontSize: 14, color: Colors.grey)),
      ],
    );
  }
}

class InfoRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const InfoRow({
    required this.icon,
    required this.label,
    required this.value,
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, size: 20, color: Colors.blueGrey),
        const SizedBox(width: 8),
        Text('$label:', style: const TextStyle(fontWeight: FontWeight.bold)),
        const SizedBox(width: 8),
        Expanded(child: Text(value, overflow: TextOverflow.ellipsis)),
      ],
    );
  }
}
