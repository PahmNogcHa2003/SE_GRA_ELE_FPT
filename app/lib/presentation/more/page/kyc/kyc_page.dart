import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:hola_bike_app/presentation/more/page/kyc/widgets/kyc_ocr.dart';
import 'package:hola_bike_app/presentation/more/page/kyc/widgets/kyc_widgets.dart';

class KycPage extends StatefulWidget {
  const KycPage({super.key});

  @override
  State<KycPage> createState() => _KycPageState();
}

class _KycPageState extends State<KycPage> {
  final _formKey = GlobalKey<FormState>();

  final fullNameController = TextEditingController();
  final idNumberController = TextEditingController();
  final dobController = TextEditingController();
  final addressController = TextEditingController();

  XFile? frontImage;
  XFile? backImage;
  final ImagePicker picker = ImagePicker();

  Future<void> _processOcr() async {
    if (frontImage != null) {
      final result = await KycOcr.extractInfo(frontImage!, backImage);
      setState(() {
        fullNameController.text = result['fullName'] ?? '';
        idNumberController.text = result['idNumber'] ?? '';
        dobController.text = result['dob'] ?? '';
        addressController.text = result['address'] ?? '';
      });
    }
  }

  Future<void> _pickImage(bool isFront) async {
    final XFile? picked = await picker.pickImage(source: ImageSource.camera);
    if (picked != null) {
      setState(() {
        if (isFront) {
          frontImage = picked;
        } else {
          backImage = picked;
        }
      });
      await _processOcr();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Xác thực KYC')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              buildTextField('Họ và tên', fullNameController, editable: false),
              buildTextField('Số CCCD', idNumberController, editable: false),
              buildTextField('Ngày sinh', dobController, editable: false),
              buildTextField('Địa chỉ', addressController, editable: true),
              const SizedBox(height: 16),
              buildImagePicker(
                label: 'Ảnh mặt trước CCCD',
                pickedImage: frontImage,
                onTap: () => _pickImage(true),
              ),
              buildImagePicker(
                label: 'Ảnh mặt sau CCCD',
                pickedImage: backImage,
                onTap: () => _pickImage(false),
              ),
              const SizedBox(height: 24),
              ElevatedButton(
                onPressed: () {
                  if (_formKey.currentState!.validate() &&
                      frontImage != null &&
                      backImage != null) {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(content: Text('Đã gửi thông tin KYC')),
                    );
                    Navigator.pop(context);
                  } else {
                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Vui lòng chụp đủ ảnh và nhập địa chỉ'),
                      ),
                    );
                  }
                },
                child: const Text('Gửi thông tin KYC'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
