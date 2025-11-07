import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_post-payment.dart';
import 'package:hola_bike_app/domain/models/info_payment.dart';
import 'package:hola_bike_app/presentation/wallet/walet_page.dart';
import 'package:webview_flutter/webview_flutter.dart';

class PaymentWebViewPage extends StatefulWidget {
  final String url;
  const PaymentWebViewPage({super.key, required this.url});

  @override
  State<PaymentWebViewPage> createState() => _PaymentWebViewPageState();
}

class _PaymentWebViewPageState extends State<PaymentWebViewPage> {
  late WebViewController _controller;
  final _storage = const FlutterSecureStorage();
  final _verifyPaymentUseCase = VerifyPaymentUseCase();
  bool isLoading = true;

  Future<void> _callBack(String url) async {
    EasyLoading.show();
    final token = await _storage.read(key: 'access_token');
    if (token == null) {
      EasyLoading.dismiss();
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(('Vui lòng đăng nhập lại'))));
      return;
    }

    PaymentResult paymentResult = await _verifyPaymentUseCase.execute(
      returnUrl: url,
      token: token,
    );
    EasyLoading.dismiss();
    if (paymentResult.isSuccess) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Thanh toán thành công")));
      await Future.delayed(const Duration(seconds: 2));
      Navigator.push(
        context,
        MaterialPageRoute(builder: (_) => WalletScreen()),
      );
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text("Thanh toán thất bại")));
      await Future.delayed(const Duration(seconds: 2));
      Navigator.push(
        context,
        MaterialPageRoute(builder: (_) => WalletScreen()),
      );
    }
  }

  @override
  void initState() {
    super.initState();

    String finalUrl = widget.url;
    if (!finalUrl.startsWith('http')) {
      finalUrl = 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html$finalUrl';
    }
    _controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..loadRequest(Uri.parse(finalUrl))
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageFinished: (_) {
            setState(() => isLoading = false);
          },
          onNavigationRequest: (request) async {
            final url = request.url;
            await _callBack(url);

            return NavigationDecision.navigate;
          },
        ),
      );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Thanh toán VNPay")),
      body: Stack(
        children: [
          WebViewWidget(controller: _controller),
          if (isLoading) const Center(child: CircularProgressIndicator()),
        ],
      ),
    );
  }
}
