import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_post-payment.dart';
import 'package:hola_bike_app/domain/models/info_payment.dart';
import 'package:hola_bike_app/presentation/wallet/page/topUp/payment_web_view.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:webview_flutter/webview_flutter.dart';

class TopUpPage extends StatefulWidget {
  const TopUpPage({super.key});

  @override
  State<TopUpPage> createState() => _TopUpPageState();
}

class _TopUpPageState extends State<TopUpPage> {
  final List<int> amounts = [50000, 100000, 200000, 300000];
  int? selectedAmount;
  final _storage = const FlutterSecureStorage();
  final _paymentUseCase = PostPaymentUseCase();

  Future<void> _handleTopUp() async {
    if (selectedAmount == null) return;

    try {
      EasyLoading.show();

      final token = await _storage.read(key: 'access_token');
      if (token == null) {
        EasyLoading.dismiss();
        EasyLoading.showError('Vui lòng đăng nhập lại');
        return;
      }

      // 🔹 Gọi API tạo session thanh toán
      final PaymentInfo payment = await _paymentUseCase.execute(
        amount: selectedAmount!.toDouble(),
        token: token,
      );

      EasyLoading.dismiss();

      if (payment.url.isNotEmpty) {
        // 🔹 Mở trang thanh toán VNPay
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (_) => PaymentWebViewPage(url: payment.url),
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Không thể tạo giao dịch")),
        );
      }
    } catch (e) {
      EasyLoading.dismiss();
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("Lỗi khi tạo giao dịch: $e")));
      print(e);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey.shade100,
      appBar: AppBar(
        title: const Text("Nạp điểm"),
        backgroundColor: AppColors.primary,
        elevation: 0,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.primary.withOpacity(0.08),
            borderRadius: BorderRadius.circular(16),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // ✅ Hiển thị số tiền đã chọn
              Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(vertical: 20),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.grey.shade300),
                ),
                child: Center(
                  child: Text(
                    selectedAmount != null
                        ? "${selectedAmount!} VND"
                        : "Chọn số tiền bên dưới",
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: selectedAmount != null
                          ? AppColors.primary
                          : Colors.grey,
                    ),
                  ),
                ),
              ),

              const SizedBox(height: 24),

              // ✅ Chọn số tiền
              const Text(
                "Số tiền muốn nạp (VND)",
                style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
              ),
              const SizedBox(height: 12),
              SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: Row(
                  children: amounts.map((amount) {
                    final isSelected = selectedAmount == amount;
                    return GestureDetector(
                      onTap: () {
                        setState(() {
                          selectedAmount = amount;
                        });
                      },
                      child: AnimatedContainer(
                        duration: const Duration(milliseconds: 200),
                        margin: const EdgeInsets.only(right: 8),
                        padding: const EdgeInsets.symmetric(
                          horizontal: 16,
                          vertical: 10,
                        ),
                        decoration: BoxDecoration(
                          color: Colors.white,
                          border: Border.all(
                            color: isSelected
                                ? AppColors.primary
                                : Colors.grey.shade300,
                            width: isSelected ? 2 : 1,
                          ),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Text(
                          "$amount",
                          style: TextStyle(
                            fontSize: 13,
                            fontWeight: FontWeight.w500,
                            color: isSelected
                                ? AppColors.primary
                                : Colors.black87,
                          ),
                        ),
                      ),
                    );
                  }).toList(),
                ),
              ),

              const SizedBox(height: 24),

              // ✅ Nút xác nhận
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  onPressed: selectedAmount != null ? _handleTopUp : null,
                  child: const Text("Xác nhận nạp điểm"),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
