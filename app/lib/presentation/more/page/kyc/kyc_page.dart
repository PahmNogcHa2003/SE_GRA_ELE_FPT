import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_kyc.dart';
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
  final secureStorage = const FlutterSecureStorage();
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

  final KycUsecase _kycUsecase = KycUsecase();

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

  /// 🧩 Gửi dữ liệu KYC thực tế lên server
  Future<void> _handleSubmit() async {
    final token = await secureStorage.read(key: 'access_token');
    if (token == null) {
      throw Exception('Không tìm thấy access token');
    }
    if (!_formKey.currentState!.validate()) return;

    if (frontImage == null || backImage == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chụp đủ 2 mặt CCCD')),
      );
      return;
    }

    try {
      // Ghép chuỗi dữ liệu JsonData bằng dấu "|"
      final jsonDataString = [
        idNumberController.text,
        fullNameController.text,
        dobController.text,
        gender,
        nationality,
        origin,
        addressController.text,
      ].join('|');
      print(jsonDataString);
      EasyLoading.show();

      final response = await _kycUsecase.execute(
        token: token,
        jsonDataString: jsonDataString,
        frontImage: frontImage!,
        backImage: backImage!,
      );

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('✅ Gửi KYC thành công')));

      Navigator.pop(context);
    } catch (e) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('❌ Lỗi gửi KYC: $e')));
    } finally {
      EasyLoading.dismiss();
    }
  }

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

              ElevatedButton(
                onPressed:
                    (hasBothImages &&
                        fullNameController.text.isNotEmpty &&
                        idNumberController.text.isNotEmpty)
                    ? _handleSubmit
                    : null,
                child: const Text('Gửi thông tin KYC'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
