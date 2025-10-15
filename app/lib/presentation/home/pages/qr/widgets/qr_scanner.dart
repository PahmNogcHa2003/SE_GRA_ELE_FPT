import 'package:flutter/material.dart';

class QrScanner extends StatelessWidget {
  const QrScanner({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: const [
          Icon(Icons.qr_code_scanner, size: 80),
          SizedBox(height: 16),
          Text("Quét mã QR để bắt đầu chuyến đi"),
        ],
      ),
    );
  }
}
