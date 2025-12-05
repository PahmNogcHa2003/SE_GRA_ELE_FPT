import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_profile.dart';
import 'package:hola_bike_app/application/usecases/usecase_update-profile.dart';
import 'package:intl/intl.dart';
import 'package:hola_bike_app/domain/models/info_profile.dart';

class EditProfilePage extends StatefulWidget {
  const EditProfilePage({super.key});

  @override
  State<EditProfilePage> createState() => _EditProfilePageState();
}

class _EditProfilePageState extends State<EditProfilePage> {
  final _formKey = GlobalKey<FormState>();
  final secureStorage = const FlutterSecureStorage();
  final ProfileUsecase _profileUsecase = ProfileUsecase();
  final UpdateProfile _updateUsecase = UpdateProfile();

  bool isLoading = true;
  bool isUpdating = false;
  String? _gender;
  String? _emergencyName;
  String? _emergencyPhone;
  String? _dob;

  final nameController = TextEditingController();
  final phoneController = TextEditingController();
  final addressController = TextEditingController();
  final genderController = TextEditingController();
  final dobController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadUserProfile();
  }

  Future<void> _loadUserProfile() async {
    final token = await secureStorage.read(key: 'access_token');
    if (token == null) throw Exception('Không tìm thấy access token');

    try {
      InfoProfile profile = await _profileUsecase.execute(token: token);

      nameController.text = profile.fullName;
      phoneController.text = profile.phoneNumber;
      addressController.text = profile.addressDetail;
      if (profile.gender.toLowerCase() == "male") {
        _gender = "Nam";
      } else if (profile.gender.toLowerCase() == "female") {
        _gender = "Nữ";
      } else {
        _gender = "Khác";
      }
      _emergencyName = profile.emergencyName;
      _emergencyPhone = profile.emergencyPhone;
      final parsedDob = DateTime.parse(profile.dob);
      dobController.text = DateFormat('dd/MM/yyyy').format(parsedDob);
      _dob = parsedDob.toIso8601String();
    } catch (e) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("Lỗi tải thông tin: $e")));
    }

    setState(() => isLoading = false);
  }

  Future<void> _onUpdatePressed() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => isUpdating = true);
    final token = await secureStorage.read(key: 'access_token');
    if (token == null) throw Exception('Không tìm thấy access token');

    final success = await _updateUsecase.updateProfile(
      token: token,
      fullName: nameController.text,
      emergencyName: _emergencyName ?? "nguoi than",
      emergencyPhone: _emergencyPhone ?? "0987654567",
      addressDetail: addressController.text,
      phoneNumber: phoneController.text,
      gender: _gender ?? "",
      dob: _dob ?? "2003-06-29T00:00:00.000",
    );

    setState(() => isUpdating = false);

    if (success) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Cập nhật thành công')));
      Navigator.pop(context, true);
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Cập nhật thất bại')));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Chỉnh sửa hồ sơ')),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(16),
              child: Form(
                key: _formKey,
                child: ListView(
                  children: [
                    _buildTextField('Tên của bạn', nameController),
                    _buildTextField('Số điện thoại', phoneController),
                    _buildTextField('Địa chỉ', addressController),
                    _dropdownGender(),
                    _buildDatePickerField(dobController),
                    const SizedBox(height: 24),

                    ElevatedButton(
                      onPressed: isUpdating ? null : _onUpdatePressed,
                      child: isUpdating
                          ? const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(
                                color: Colors.white,
                                strokeWidth: 2,
                              ),
                            )
                          : const Text('Cập nhật'),
                    ),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _dropdownGender() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        value: _gender,
        decoration: InputDecoration(
          labelText: 'Giới tính',
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
        ),
        items: [
          'Nam',
          'Nữ',
          'Khác',
        ].map((e) => DropdownMenuItem(value: e, child: Text(e))).toList(),
        onChanged: (v) {
          setState(() => _gender = v);
        },
        validator: (v) => v == null ? 'Vui lòng chọn giới tính' : null,
      ),
    );
  }

  Widget _buildTextField(String label, TextEditingController controller) {
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

  Widget _buildDatePickerField(TextEditingController controller) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        controller: controller,
        readOnly: true,
        decoration: InputDecoration(
          labelText: 'Ngày sinh *',
          hintText: 'DD/MM/YYYY',
          suffixIcon: const Icon(Icons.calendar_today),
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
        ),
        validator: (v) => v!.isEmpty ? 'Vui lòng chọn ngày sinh' : null,
        onTap: () async {
          FocusScope.of(context).unfocus();
          final now = DateTime.now();

          final picked = await showDatePicker(
            context: context,
            initialDate: DateTime(now.year - 18),
            firstDate: DateTime(1900),
            lastDate: now,
          );

          if (picked != null) {
            controller.text = DateFormat('dd/MM/yyyy').format(picked);
          }
        },
      ),
    );
  }
}
