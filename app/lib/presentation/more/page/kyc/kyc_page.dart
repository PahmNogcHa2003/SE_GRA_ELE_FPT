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

  String gender = '';
  String nationality = '';
  String origin = '';

  XFile? frontImage;
  XFile? backImage;
  final ImagePicker picker = ImagePicker();

  Future<void> _processOcr() async {
    if (frontImage != null && backImage != null) {
      final result = await KycOcr.extractInfo(frontImage!, backImage);
      setState(() {
        fullNameController.text = result['fullName'] ?? '';
        idNumberController.text = result['idNumber'] ?? '';
        dobController.text = result['dob'] ?? '';
        addressController.text = result['address'] ?? '';
        gender = result['gender'] ?? '';
        nationality = result['nationality'] ?? '';
        origin = result['origin'] ?? '';
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
  @override
  Widget build(BuildContext context) {
    final hasBothImages = frontImage != null && backImage != null;

    return Scaffold(
      appBar: AppBar(title: const Text('Xác thực KYC')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              // Hàng chứa 2 ảnh
              Row(
                children: [
                  Expanded(
                    child: buildImagePicker(
                      label: 'Ảnh mặt trước',
                      pickedImage: frontImage,
                      onTap: () => _pickImage(true),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: buildImagePicker(
                      label: 'Ảnh mặt sau',
                      pickedImage: backImage,
                      onTap: () => _pickImage(false),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),

              // Thông tin chỉ hiển thị khi có đủ 2 ảnh
              if (hasBothImages) ...[
                buildTextField(
                  'Họ và tên',
                  fullNameController,
                  editable: false,
                ),
                buildTextField('Số CCCD', idNumberController, editable: false),
                buildTextField('Ngày sinh', dobController, editable: false),
                buildTextField(
                  'Giới tính',
                  TextEditingController(text: gender),
                  editable: false,
                ),
                buildTextField(
                  'Quốc tịch',
                  TextEditingController(text: nationality),
                  editable: false,
                ),
                buildTextField(
                  'Quê quán',
                  TextEditingController(text: origin),
                  editable: false,
                ),
                buildTextField('Địa chỉ', addressController, editable: true),
                const SizedBox(height: 24),
              ],

              // Button luôn hiển thị nhưng disable nếu chưa đủ ảnh
              ElevatedButton(
                onPressed:
                    (frontImage != null &&
                        backImage != null &&
                        fullNameController.text.isNotEmpty)
                    ? () {
                        if (_formKey.currentState!.validate()) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Đã gửi thông tin KYC'),
                            ),
                          );
                          Navigator.pop(context);
                        }
                      }
                    : null, // disable khi chưa có dữ liệu OCR
                child: const Text('Gửi thông tin KYC'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
