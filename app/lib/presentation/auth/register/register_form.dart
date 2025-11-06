import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:hola_bike_app/domain/models/info_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_wards.dart';

class RegisterForm extends StatefulWidget {
  final void Function(bool isValid, Map<String, dynamic> formData)
  onFormValidChanged;

  const RegisterForm({super.key, required this.onFormValidChanged});

  @override
  State<RegisterForm> createState() => _RegisterFormState();
}

class _RegisterFormState extends State<RegisterForm> {
  final _formKey = GlobalKey<FormState>();

  // Controllers
  final _fullNameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _identityController = TextEditingController();
  final _addressController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _birthController = TextEditingController();
  final _emergencyNameController = TextEditingController();
  final _emergencyPhoneController = TextEditingController();

  String? _gender;
  String? _selectedProvinceCode;
  String? _selectedWardCode;

  List<Province> _provinces = [];
  List<Province> _wards = [];

  final _provinceApi = ProvinceApi();
  final _wardsApi = WardsApi();

  @override
  void initState() {
    super.initState();
    _loadProvinces();
  }

  Future<void> _loadProvinces() async {
    try {
      final data = await _provinceApi.getProvinces();
      if (mounted) setState(() => _provinces = data);
    } finally {}
  }

  Future<void> _loadWards(String provinceCode) async {
    setState(() {
      _wards = [];
      _selectedWardCode = null;
    });
    try {
      final data = await _wardsApi.GetWardByProvice(provinceCode);
      if (mounted) setState(() => _wards = data);
    } finally {}
  }

  void _checkFormValid() {
    final isValid = _formKey.currentState?.validate() ?? false;

    if (!isValid) {
      widget.onFormValidChanged(false, {});
      return;
    }

    final province = _provinces.firstWhere(
      (p) => p.code == _selectedProvinceCode,
      orElse: () => Province(code: '', name: ''),
    );
    final ward = _wards.firstWhere(
      (w) => w.code == _selectedWardCode,
      orElse: () => Province(code: '', name: ''),
    );

    DateTime? dob;
    try {
      dob = DateFormat('dd/MM/yyyy').parse(_birthController.text);
    } catch (_) {}

    widget.onFormValidChanged(true, {
      'phoneNumber': _phoneController.text.trim(),
      'fullName': _fullNameController.text.trim(),
      'identityNumber': _identityController.text.trim(),
      'emergencyName': _emergencyNameController.text.trim(),
      'emergencyPhone': _emergencyPhoneController.text.trim(),
      'provinceId': int.tryParse(province.code) ?? 0,
      'provinceName': province.name,
      'wardId': int.tryParse(ward.code) ?? 0,
      'wardName': ward.name,
      'address': _addressController.text.trim(),
      'email': _emailController.text.trim(),
      'gender': _gender ?? '',
      'dateOfBirth': dob ?? DateTime.now(),
      'password': _passwordController.text,
      'confirmPassword': _confirmPasswordController.text,
    });
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      onChanged: _checkFormValid,
      child: Column(
        children: [
          _buildField(
            _fullNameController,
            'Họ và tên *',
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập họ tên' : null,
          ),
          _buildField(
            _phoneController,
            'Số điện thoại *',
            keyboardType: TextInputType.phone,
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập số điện thoại' : null,
          ),
          _buildField(
            _identityController,
            'CCCD/Passport *',
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập CCCD/Passport' : null,
          ),
          _buildField(
            _emergencyNameController,
            'Tên người thân liên hệ khẩn *',
            validator: (v) =>
                v!.isEmpty ? 'Vui lòng nhập tên người thân' : null,
          ),
          _buildField(
            _emergencyPhoneController,
            'SĐT người thân *',
            keyboardType: TextInputType.phone,
            validator: (v) =>
                v!.isEmpty ? 'Vui lòng nhập SĐT người thân' : null,
          ),

          // Dropdown tỉnh/thành
          _dropdownProvince(),
          _dropdownWard(),
          _buildField(
            _addressController,
            'Địa chỉ *',
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập địa chỉ' : null,
          ),
          _buildField(
            _emailController,
            'Email',
            keyboardType: TextInputType.emailAddress,
          ),
          _dropdownGender(),
          _buildDatePickerField(_birthController),
          _buildField(
            _passwordController,
            'Mật khẩu *',
            obscureText: true,
            validator: (v) {
              if (v == null || v.isEmpty) return 'Vui lòng nhập mật khẩu';
              if (v.length < 6) return 'Mật khẩu phải ít nhất 6 ký tự';
              if (!RegExp(r'[0-9]').hasMatch(v))
                return 'Phải có ít nhất 1 chữ số';
              return null;
            },
          ),
          _buildField(
            _confirmPasswordController,
            'Nhập lại mật khẩu',
            obscureText: true,
            validator: (v) =>
                v != _passwordController.text ? 'Mật khẩu không khớp' : null,
          ),
        ],
      ),
    );
  }

  Widget _dropdownProvince() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        value: _selectedProvinceCode,
        isExpanded: true,
        decoration: _dropdownDecoration('Tỉnh/TP *'),
        items: _provinces
            .map((p) => DropdownMenuItem(value: p.code, child: Text(p.name)))
            .toList(),
        onChanged: (v) {
          setState(() => _selectedProvinceCode = v);
          if (v != null) _loadWards(v);
          _checkFormValid();
        },
        validator: (v) => v == null ? 'Chọn tỉnh/thành phố' : null,
      ),
    );
  }

  Widget _dropdownWard() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        value: _selectedWardCode,
        isExpanded: true,
        decoration: _dropdownDecoration('Xã/Phường *'),
        items: _wards
            .map((w) => DropdownMenuItem(value: w.code, child: Text(w.name)))
            .toList(),
        onChanged: (v) {
          setState(() => _selectedWardCode = v);
          _checkFormValid();
        },
        validator: (v) => v == null ? 'Chọn xã/phường' : null,
      ),
    );
  }

  Widget _dropdownGender() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        value: _gender,
        decoration: _dropdownDecoration('Giới tính'),
        items: [
          'Nam',
          'Nữ',
          'Khác',
        ].map((e) => DropdownMenuItem(value: e, child: Text(e))).toList(),
        onChanged: (v) {
          setState(() => _gender = v);
          _checkFormValid();
        },
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
          filled: true,
          fillColor: AppColors.card,
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
            _checkFormValid();
          }
        },
      ),
    );
  }

  InputDecoration _dropdownDecoration(String label) => InputDecoration(
    labelText: label,
    filled: true,
    fillColor: AppColors.card,
    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
  );

  Widget _buildField(
    TextEditingController c,
    String label, {
    String? hint,
    TextInputType keyboardType = TextInputType.text,
    bool obscureText = false,
    String? Function(String?)? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        controller: c,
        obscureText: obscureText,
        keyboardType: keyboardType,
        decoration: InputDecoration(
          labelText: label,
          hintText: hint,
          filled: true,
          fillColor: AppColors.card,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
        ),
        validator: validator,
      ),
    );
  }
}
