import 'package:flutter/material.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:hola_bike_app/presentation/rent/rent_bike_page.dart';

class QrScanner extends StatefulWidget {
  const QrScanner({super.key});

  @override
  State<QrScanner> createState() => _QrScannerState();
}

class _QrScannerState extends State<QrScanner> {
  bool isProcessing = false;

  void _handleDetection(String? code) {
    if (code == null || isProcessing) return;

    setState(() => isProcessing = true);

    Navigator.push(
      context,
      MaterialPageRoute(builder: (_) => RentBikePage(bikeId: code)),
    );
  }

  @override
  Widget build(BuildContext context) {
    final screenSize = MediaQuery.of(context).size;

    return Scaffold(
      extendBodyBehindAppBar: true,
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Stack(
        children: [
          // Camera toàn màn hình
          Positioned.fill(
            child: MobileScanner(
              fit: BoxFit.cover,
              onDetect: (capture) {
                final barcode = capture.barcodes.first;
                _handleDetection(barcode.rawValue);
              },
            ),
          ),

          // Overlay tối có lỗ ở giữa
          CustomPaint(size: screenSize, painter: _OverlayPainter()),

          // Viền khung quét
          Center(
            child: CustomPaint(
              size: const Size(250, 250),
              painter: _CornerBorderPainter(),
            ),
          ),

          // Hướng dẫn người dùng
          Positioned(
            bottom: 40,
            left: 0,
            right: 0,
            child: const Center(
              child: Text(
                'Đưa mã QR vào khung để quét',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

// Vẽ overlay tối có lỗ ở giữa
class _OverlayPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final holeSize = 250.0;
    final holeOffset = Offset(
      (size.width - holeSize) / 2,
      (size.height - holeSize) / 2,
    );

    final overlayPaint = Paint()..color = Colors.black.withOpacity(0.5);

    final holeRect = Rect.fromLTWH(
      holeOffset.dx,
      holeOffset.dy,
      holeSize,
      holeSize,
    );
    final holeRRect = RRect.fromRectAndRadius(
      holeRect,
      const Radius.circular(16),
    );

    final fullRect = Path()
      ..addRect(Rect.fromLTWH(0, 0, size.width, size.height));
    final holePath = Path()..addRRect(holeRRect);

    final overlayPath = Path.combine(
      PathOperation.difference,
      fullRect,
      holePath,
    );

    canvas.drawPath(overlayPath, overlayPaint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

// Vẽ viền khung quét
class _CornerBorderPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final cornerLength = 20.0;
    final strokeWidth = 4.0;
    final paint = Paint()
      ..color = Colors.white
      ..strokeWidth = strokeWidth
      ..style = PaintingStyle.stroke;

    // Top-left
    canvas.drawLine(Offset(0, 0), Offset(cornerLength, 0), paint);
    canvas.drawLine(Offset(0, 0), Offset(0, cornerLength), paint);

    // Top-right
    canvas.drawLine(
      Offset(size.width, 0),
      Offset(size.width - cornerLength, 0),
      paint,
    );
    canvas.drawLine(
      Offset(size.width, 0),
      Offset(size.width, cornerLength),
      paint,
    );

    // Bottom-left
    canvas.drawLine(
      Offset(0, size.height),
      Offset(0, size.height - cornerLength),
      paint,
    );
    canvas.drawLine(
      Offset(0, size.height),
      Offset(cornerLength, size.height),
      paint,
    );

    // Bottom-right
    canvas.drawLine(
      Offset(size.width, size.height),
      Offset(size.width - cornerLength, size.height),
      paint,
    );
    canvas.drawLine(
      Offset(size.width, size.height),
      Offset(size.width, size.height - cornerLength),
      paint,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
