import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

/// TextField helper
Widget buildTextField(
  String label,
  TextEditingController controller, {
  bool editable = true,
}) {
  return Padding(
    padding: const EdgeInsets.symmetric(vertical: 8),
    child: TextFormField(
      controller: controller,
      enabled: editable,
      decoration: InputDecoration(
        labelText: label,
        border: const OutlineInputBorder(),
      ),
      validator: (value) {
        if (value == null || value.isEmpty) return 'Vui lòng nhập $label';
        return null;
      },
    ),
  );
}

/// Image picker helper
Widget buildImagePicker({
  required String label,
  required XFile? pickedImage,
  required VoidCallback onTap,
}) {
  return Padding(
    padding: const EdgeInsets.symmetric(vertical: 8),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: const TextStyle(fontWeight: FontWeight.bold)),
        const SizedBox(height: 8),
        GestureDetector(
          onTap: onTap,
          child: Container(
            height: 200,
            decoration: BoxDecoration(
              border: Border.all(color: Colors.grey),
              color: Colors.grey[200],
            ),
            child: pickedImage != null
                ? Image.file(File(pickedImage.path), fit: BoxFit.cover)
                : const Center(child: Text('Chạm để chụp ảnh')),
          ),
        ),
      ],
    ),
  );
}
