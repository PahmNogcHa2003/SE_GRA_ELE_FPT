import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:hola_bike_app/domain/models/info_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_wards.dart';

class RegisterForm extends StatefulWidget {
  final void Function(bool isValid) onFormValidChanged;

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

  String? _gender;
  String? _selectedProvinceCode;
  String? _selectedWardCode;

  List<Province> _provinces = [];
  List<Province> _wards = [];

  bool _isLoadingProvince = false;
  bool _isLoadingWard = false;

  final _provinceApi = ProvinceApi();
  final _wardsApi = WardsApi();

  @override
  void initState() {
    super.initState();
    _loadProvinces();
  }

  @override
  void dispose() {
    _fullNameController.dispose();
    _phoneController.dispose();
    _identityController.dispose();
    _addressController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    _birthController.dispose();
    super.dispose();
  }

  Future<void> _loadProvinces() async {
    setState(() => _isLoadingProvince = true);
    try {
      final data = await _provinceApi.getProvinces();
      if (mounted) {
        setState(() => _provinces = data);
      }
    } finally {
      if (mounted) setState(() => _isLoadingProvince = false);
    }
  }

  Future<void> _loadWards(String provinceCode) async {
    setState(() {
      _isLoadingWard = true;
      _wards = [];
      _selectedWardCode = null;
    });
    try {
      final data = await _wardsApi.GetWardByProvice(provinceCode);
      if (mounted) {
        setState(() => _wards = data);
      }
    } finally {
      if (mounted) setState(() => _isLoadingWard = false);
    }
  }

  void _checkFormValid() {
    final isValid = _formKey.currentState?.validate() ?? false;
    widget.onFormValidChanged(isValid);
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      onChanged: _checkFormValid,
      child: Column(
        children: [
          _buildField(
            controller: _fullNameController,
            label: 'Họ và tên *',
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập họ tên' : null,
          ),
          _buildField(
            controller: _phoneController,
            label: 'Số điện thoại *',
            keyboardType: TextInputType.phone,
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập số điện thoại' : null,
          ),
          _buildField(
            controller: _identityController,
            label: 'CCCD/Passport *',
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập CCCD/Passport' : null,
          ),

          // --- Dropdown Tỉnh/TP ---
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: DropdownButtonFormField<String>(
              value: _selectedProvinceCode,
              isExpanded: true,
              decoration: _dropdownDecoration('Tỉnh/TP (Nơi ở hiện tại) *'),
              items: _provinces
                  .map(
                    (p) => DropdownMenuItem(value: p.code, child: Text(p.name)),
                  )
                  .toList(),
              onChanged: (value) {
                setState(() => _selectedProvinceCode = value);
                if (value != null) _loadWards(value);
                _checkFormValid();
              },
              validator: (v) =>
                  v == null ? 'Vui lòng chọn Tỉnh/Thành phố' : null,
            ),
          ),
          if (_isLoadingProvince)
            const Padding(
              padding: EdgeInsets.all(8),
              child: CircularProgressIndicator(),
            ),

          // --- Dropdown Xã/Phường ---
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: DropdownButtonFormField<String>(
              value: _selectedWardCode,
              isExpanded: true,
              decoration: _dropdownDecoration('Xã/Phường (Nơi ở hiện tại) *'),
              items: _wards
                  .map(
                    (w) => DropdownMenuItem(value: w.code, child: Text(w.name)),
                  )
                  .toList(),
              onChanged: (value) {
                setState(() => _selectedWardCode = value);
                _checkFormValid();
              },
              validator: (v) => v == null ? 'Vui lòng chọn xã/phường' : null,
            ),
          ),
          if (_isLoadingWard)
            const Padding(
              padding: EdgeInsets.all(8),
              child: CircularProgressIndicator(),
            ),

          _buildField(
            controller: _addressController,
            label: 'Địa chỉ *',
            validator: (v) => v!.isEmpty ? 'Vui lòng nhập địa chỉ' : null,
          ),
          _buildField(
            controller: _emailController,
            label: 'Email',
            keyboardType: TextInputType.emailAddress,
            validator: (v) {
              if (v!.isEmpty) return null;
              final emailReg = RegExp(r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$');
              return emailReg.hasMatch(v) ? null : 'Email không hợp lệ';
            },
          ),

          // --- Dropdown giới tính ---
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: DropdownButtonFormField<String>(
              value: _gender,
              decoration: _dropdownDecoration('Giới tính'),
              items: ['Nam', 'Nữ', 'Khác']
                  .map(
                    (item) => DropdownMenuItem(value: item, child: Text(item)),
                  )
                  .toList(),
              onChanged: (v) {
                setState(() => _gender = v);
                _checkFormValid();
              },
            ),
          ),

          // --- Chọn ngày sinh ---
          _buildDatePickerField(_birthController),

          // --- Mật khẩu ---
          _buildField(
            controller: _passwordController,
            label: 'Mật khẩu *',
            obscureText: true,
            validator: (v) {
              if (v == null || v.isEmpty) {
                return 'Vui lòng nhập mật khẩu';
              }
              if (v.length < 6) {
                return 'Mật khẩu phải ít nhất 6 ký tự';
              }
              if (!RegExp(r'[0-9]').hasMatch(v)) {
                return 'Mật khẩu phải chứa ít nhất 1 chữ số';
              }
              return null;
            },
          ),

          _buildField(
            controller: _confirmPasswordController,
            label: 'Nhập lại mật khẩu',
            obscureText: true,
            validator: (v) =>
                v != _passwordController.text ? 'Mật khẩu không khớp' : null,
          ),
        ],
      ),
    );
  }

  // ---------- Tiện ích ----------
  InputDecoration _dropdownDecoration(String label) {
    return InputDecoration(
      labelText: label,
      filled: true,
      fillColor: AppColors.card,
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
    );
  }

  Widget _buildField({
    required TextEditingController controller,
    required String label,
    String? hint,
    TextInputType keyboardType = TextInputType.text,
    bool obscureText = false,
    String? Function(String?)? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        controller: controller,
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

  // --- Field chọn ngày sinh ---
  Widget _buildDatePickerField(TextEditingController controller) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        controller: controller,
        readOnly: true,
        decoration: InputDecoration(
          labelText: 'Ngày sinh *',
          hintText: 'DD/MM/YYYY',
          suffixIcon: const Icon(Icons.calendar_today, color: Colors.grey),
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
            builder: (context, child) => Theme(
              data: Theme.of(context).copyWith(
                colorScheme: const ColorScheme.light(primary: Colors.green),
              ),
              child: child!,
            ),
          );

          if (picked != null && mounted) {
            setState(() {
              controller.text = DateFormat('dd/MM/yyyy').format(picked);
            });
          }
        },
      ),
    );
  }
}
