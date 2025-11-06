import 'package:flutter/material.dart';
import 'package:hola_bike_app/domain/models/info_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_provice.dart';
import 'package:hola_bike_app/data/sources/remote/api_wards.dart';

class RegisterController extends ChangeNotifier {
  final formKey = GlobalKey<FormState>();

  // Các controller nhập liệu
  final fullNameController = TextEditingController();
  final phoneController = TextEditingController();
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  final confirmPasswordController = TextEditingController();

  Province? selectedProvince;
  Province? selectedWard;

  List<Province> provinces = [];
  List<Province> wards = [];

  bool isLoadingProvinces = false;
  bool isLoadingWards = false;

  final _provinceApi = ProvinceApi();
  final _wardsApi = WardsApi();

  Future<void> loadProvinces() async {
    isLoadingProvinces = true;
    notifyListeners();
    provinces = await _provinceApi.getProvinces();
    isLoadingProvinces = false;
    notifyListeners();
  }

  Future<void> loadWards(String provinceCode) async {
    isLoadingWards = true;
    notifyListeners();
    wards = await _wardsApi.GetWardByProvice(provinceCode);
    isLoadingWards = false;
    notifyListeners();
  }

  bool validateAndSubmit() {
    if (!formKey.currentState!.validate()) return false;

    if (selectedProvince == null) {
      debugPrint("Chưa chọn tỉnh/thành phố");
      return false;
    }
    if (selectedWard == null) {
      debugPrint("Chưa chọn xã/phường");
      return false;
    }

    // TODO: Gọi API đăng ký tại đây
    debugPrint("✅ Form hợp lệ, gửi dữ liệu đăng ký...");
    return true;
  }

  @override
  void dispose() {
    fullNameController.dispose();
    phoneController.dispose();
    emailController.dispose();
    passwordController.dispose();
    confirmPasswordController.dispose();
    super.dispose();
  }
}
