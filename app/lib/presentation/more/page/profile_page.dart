import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_update-profile.dart';
import 'package:image_picker/image_picker.dart';

import 'package:hola_bike_app/application/usecases/usecase_profile.dart';
import 'package:hola_bike_app/domain/models/info_profile.dart';
import 'package:hola_bike_app/presentation/more/page/edit_profile_page.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final secureStorage = const FlutterSecureStorage();
  final ProfileUsecase _usecase = ProfileUsecase();
  final UpdateProfile _updateProfile = UpdateProfile();

  InfoProfile? profile;

  @override
  void initState() {
    super.initState();
    _loadProfile();
  }

  Future<void> _loadProfile() async {
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) throw Exception('Không tìm thấy access token');

      final info = await _usecase.execute(token: token);

      setState(() {
        profile = info;
      });
    } catch (e) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(const SnackBar(content: Text('Lỗi tải hồ sơ cá nhân')));
      });
    } finally {}
  }

  // ✅ Hàm chọn ảnh hoặc chụp ảnh
  Future<File?> _pickImage() async {
    final picker = ImagePicker();

    return showModalBottomSheet<File?>(
      context: context,
      builder: (context) {
        return SafeArea(
          child: Wrap(
            children: [
              ListTile(
                leading: const Icon(Icons.photo_library),
                title: const Text('Chọn từ thư viện'),
                onTap: () async {
                  final picked = await picker.pickImage(
                    source: ImageSource.gallery,
                  );
                  Navigator.pop(
                    context,
                    picked != null ? File(picked.path) : null,
                  );
                },
              ),
              ListTile(
                leading: const Icon(Icons.camera_alt),
                title: const Text('Chụp ảnh'),
                onTap: () async {
                  final picked = await picker.pickImage(
                    source: ImageSource.camera,
                  );
                  Navigator.pop(
                    context,
                    picked != null ? File(picked.path) : null,
                  );
                },
              ),
            ],
          ),
        );
      },
    );
  }

  // ✅ Upload avatar
  Future<void> _updateAvatar() async {
    final file = await _pickImage();
    if (file == null) return;

    final token = await secureStorage.read(key: 'access_token');
    if (token == null) return;

    EasyLoading.show(status: "Đang cập nhật...");

    final success = await _updateProfile.updateAvatar(
      token: token,
      avatarFile: file,
    );

    EasyLoading.dismiss();

    if (success) {
      _loadProfile();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Cập nhật avatar thành công")),
      );
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Cập nhật avatar thất bại")));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Hồ sơ cá nhân'),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings),
            onPressed: () async {
              final result = await Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => const EditProfilePage(),
                ),
              );
              if (result == true) {
                _loadProfile(); // ✅ Reload lại profile
              }
            },
          ),
        ],
      ),
      body: profile == null
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                children: [
                  // ✅ Avatar + click để đổi ảnh
                  Center(
                    child: Column(
                      children: [
                        GestureDetector(
                          onTap: _updateAvatar,
                          child: CircleAvatar(
                            radius: 50,
                            backgroundImage: NetworkImage(profile!.avatarUrl),
                          ),
                        ),
                        const SizedBox(height: 12),
                        Text(
                          profile!.fullName,
                          style: const TextStyle(
                            fontSize: 22,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 24),

                  // ✅ Card thống kê
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
                        children: [
                          StatBox(
                            label: 'Km đã đạp',
                            value: '${profile!.totalDistanceKm} km',
                          ),
                          StatBox(
                            label: 'Thời gian',
                            value: '${profile!.totalDurationMinutes} phút',
                          ),
                          StatBox(
                            label: 'Chuyến đi',
                            value: '${profile!.totalTrips} chuyến',
                          ),
                        ],
                      ),
                    ),
                  ),

                  const SizedBox(height: 24),

                  // ✅ Thông tin cá nhân
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: Colors.grey.shade100,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        InfoRow(
                          icon: Icons.email,
                          label: 'Email',
                          value: profile!.email,
                        ),
                        const SizedBox(height: 12),
                        InfoRow(
                          icon: Icons.cake,
                          label: 'Ngày sinh',
                          value: profile!.dob,
                        ),
                        const SizedBox(height: 12),
                        InfoRow(
                          icon: Icons.home,
                          label: 'Địa chỉ',
                          value: profile!.addressDetail,
                        ),
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
