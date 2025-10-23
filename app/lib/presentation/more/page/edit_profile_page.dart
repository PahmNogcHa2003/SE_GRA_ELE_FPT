import 'package:flutter/material.dart';

class EditProfilePage extends StatefulWidget {
  const EditProfilePage({super.key});

  @override
  State<EditProfilePage> createState() => _EditProfilePageState();
}

class _EditProfilePageState extends State<EditProfilePage> {
  final _formKey = GlobalKey<FormState>();

  final nameController = TextEditingController(text: 'EcoJourney');
  final idController = TextEditingController(text: '111111111111');
  final cityController = TextEditingController(text: 'Hà Nội');
  final wardController = TextEditingController(text: 'Thạch Thất');
  final addressController = TextEditingController(text: 'FPT');
  final emailController = TextEditingController(text: 'ecojourney@gmail.com');
  final dobController = TextEditingController(text: '01/01/2025');

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Chỉnh sửa hồ sơ')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              buildTextField('Tên của bạn', nameController),
              buildTextField('CCCD/Passport', idController),
              buildTextField('Tỉnh/TP', cityController),
              buildTextField('Xã/Phường', wardController),
              buildTextField('Địa chỉ', addressController),
              buildTextField('Email', emailController),
              buildTextField('Ngày sinh', dobController),
              const SizedBox(height: 24),
              ElevatedButton(
                onPressed: () {
                  if (_formKey.currentState!.validate()) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Thông tin đã được cập nhật'),
                      ),
                    );
                    Navigator.pop(context);
                  }
                },
                child: const Text('Cập nhật'),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget buildTextField(String label, TextEditingController controller) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        controller: controller,
        decoration: InputDecoration(
          labelText: label,
          border: const OutlineInputBorder(),
        ),
        validator: (value) =>
            value == null || value.isEmpty ? 'Vui lòng nhập $label' : null,
      ),
    );
  }
}
