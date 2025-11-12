import 'package:google_mlkit_text_recognition/google_mlkit_text_recognition.dart';
import 'package:image_picker/image_picker.dart';

class KycOcr {
  /// Nhận dạng văn bản từ ảnh CCCD bằng ML Kit
  static Future<Map<String, String>> extractInfo(
    XFile front,
    XFile? back,
  ) async {
    // Lấy text từ ảnh
    final frontText = await _extractTextFromImage(front.path);
    final backText = back != null ? await _extractTextFromImage(back.path) : '';

    // Debug: in ra số dòng => nội dung
    final frontLines = frontText.split('\n');
    for (int i = 0; i < frontLines.length; i++) {
      print('${i + 1} => ${frontLines[i]}');
    }

    if (back != null) {
      final backLines = backText.split('\n');
      for (int i = 0; i < backLines.length; i++) {
        print('${i + 1} => ${backLines[i]}');
      }
    }

    // Xử lý tách các trường dựa theo số dòng
    final fields = _extractFieldsByLines(frontLines);

    return fields;
  }

  /// Nhận dạng văn bản từ đường dẫn ảnh
  static Future<String> _extractTextFromImage(String path) async {
    final inputImage = InputImage.fromFilePath(path);
    final textRecognizer = TextRecognizer(script: TextRecognitionScript.latin);
    final RecognizedText recognizedText = await textRecognizer.processImage(
      inputImage,
    );
    await textRecognizer.close();
    return recognizedText.text;
  }

  /// Tách thông tin từ OCR dựa trên số dòng
  static Map<String, String> _extractFieldsByLines(List<String> lines) {
    final result = <String, String>{};

    for (int i = 0; i < lines.length; i++) {
      final line = lines[i].trim();

      // --- ID number ---
      if (line.toLowerCase().contains('no')) {
        final parts = line.split(':');
        if (parts.length > 1) {
          // chỉ lấy số sau "No"
          final idMatch = RegExp(r'\d{9,12}').firstMatch(parts[1]);
          result['idNumber'] = idMatch?.group(0)?.trim() ?? '';
        }
      }

      // --- Full name ---
      if (line.toLowerCase().contains('full name')) {
        // dòng kế tiếp thường là tên
        if (i + 1 < lines.length) {
          result['fullName'] = lines[i + 1].trim();
        }
      }

      // --- Date of birth ---
      if (line.toLowerCase().contains('date of birth')) {
        final match = RegExp(r'\d{2}[/-]\d{2}[/-]\d{4}').firstMatch(line);
        if (match != null) {
          result['dob'] = match.group(0)!;
        } else if (i + 1 < lines.length) {
          // fallback dòng dưới
          result['dob'] = lines[i + 1].trim();
        }
      }

      // --- Gender & Nationality cùng dòng ---
      if (line.toLowerCase().contains('sex')) {
        // Lấy gender
        final genderMatch = RegExp(
          r'(Nam|Nữ|Male|Female)',
          caseSensitive: false,
        ).firstMatch(line);
        result['gender'] = genderMatch?.group(0)?.trim() ?? '';

        // Lấy nationality
        final nationalityMatch = RegExp(
          r'(?:nationality)[:\s]*([A-Za-zÀ-ỹ\s]+)',
          caseSensitive: false,
        ).firstMatch(line);
        result['nationality'] = nationalityMatch?.group(1)?.trim() ?? '';
      }

      // --- Place of origin ---
      if (line.toLowerCase().contains('place of origin')) {
        final buffer = StringBuffer();
        int j = i + 1;
        while (j < lines.length &&
            !lines[j].toLowerCase().contains('place of residence') &&
            lines[j].isNotEmpty) {
          buffer.write(lines[j].trim() + ' ');
          j++;
        }
        result['origin'] = buffer.toString().trim();
      }

      // --- Address / Place of residence ---
      if (line.toLowerCase().contains('place of residence')) {
        final buffer = StringBuffer();
        int j = i + 1;
        while (j < lines.length && lines[j].isNotEmpty) {
          buffer.write(lines[j].trim() + ' ');
          j++;
        }
        result['address'] = buffer.toString().trim();
      }
    }

    print(result);
    return result;
  }
}
