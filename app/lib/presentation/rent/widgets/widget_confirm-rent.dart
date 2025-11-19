import 'package:flutter/material.dart';

class ConfirmRentButton extends StatelessWidget {
  final VoidCallback onConfirm;
  const ConfirmRentButton({super.key, required this.onConfirm});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      width: double.infinity,
      child: ElevatedButton.icon(
        onPressed: onConfirm,
        icon: const Icon(Icons.check),
        label: const Text('Xác nhận thuê xe'),
      ),
    );
  }
}
