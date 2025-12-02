import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class PasswordFieldWithEye extends StatefulWidget {
  final TextEditingController controller;
  final String label;
  final String? hint;
  final String? Function(String?)? validator;

  const PasswordFieldWithEye({
    Key? key,
    required this.controller,
    required this.label,
    this.hint,
    this.validator,
  }) : super(key: key);

  @override
  State<PasswordFieldWithEye> createState() => _PasswordFieldWithEyeState();
}

class _PasswordFieldWithEyeState extends State<PasswordFieldWithEye> {
  bool _obscureText = true;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        controller: widget.controller,
        obscureText: _obscureText,
        decoration: InputDecoration(
          labelText: widget.label,
          hintText: widget.hint,
          filled: true,
          fillColor: AppColors.card,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
          suffixIcon: IconButton(
            icon: Icon(_obscureText ? Icons.visibility_off : Icons.visibility),
            onPressed: () {
              setState(() {
                _obscureText = !_obscureText;
              });
            },
          ),
        ),
        validator: widget.validator,
      ),
    );
  }
}
