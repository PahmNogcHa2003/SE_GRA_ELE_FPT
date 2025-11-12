import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_post-payment.dart';
import 'package:hola_bike_app/presentation/wallet/walet_page.dart';
import 'package:webview_flutter/webview_flutter.dart';

class PaymentWebViewPage extends StatefulWidget {
  final String url;
  const PaymentWebViewPage({super.key, required this.url});

  @override
  State<PaymentWebViewPage> createState() => _PaymentWebViewPageState();
}

class _PaymentWebViewPageState extends State<PaymentWebViewPage> {
  late final WebViewController _controller;
  final _storage = const FlutterSecureStorage();
  final _verifyPaymentUseCase = VerifyPaymentUseCase();
  bool isLoading = true;
  bool _hasCalledBack = false;
  Future<void> _callBack(String url) async {
    EasyLoading.show();
    final token = await _storage.read(key: 'access_token');
    if (token == null) {
      EasyLoading.dismiss();
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Vui lòng đăng nhập lại')));
      return;
    }
    print(url);
    final paymentResult = await _verifyPaymentUseCase.execute(
      returnUrl: url,
      token: token,
    );
    EasyLoading.dismiss();

    final message = paymentResult.isSuccess
        ? "Thanh toán thành công"
        : "Thanh toán thất bại";

    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(message)));
    await Future.delayed(const Duration(seconds: 2));

    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (_) => const WalletScreen()),
    );
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
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageFinished: (_) {
            setState(() => isLoading = false);
          },
          onNavigationRequest: (request) {
            final url = request.url;

            if (!_hasCalledBack && url.contains("localhost")) {
              _hasCalledBack = true;

              final uri = Uri.parse(url);
              final query = uri.query;

              _callBack(query);

              return NavigationDecision.prevent;
            }

            return NavigationDecision.navigate;
          },
        ),
      )
      ..loadRequest(Uri.parse(finalUrl));
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
