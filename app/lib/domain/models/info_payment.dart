class PaymentInfo {
  final String url;

  PaymentInfo({required this.url});

  factory PaymentInfo.fromJson(Map<String, dynamic> json) {
    return PaymentInfo(url: json['paymentUrl']);
  }
}

class PaymentResult {
  final String rspCode;
  final String? message;
  final bool isSuccess;

  PaymentResult({required this.rspCode, this.message, required this.isSuccess});

  factory PaymentResult.fromJson(Map<String, dynamic> json) {
    return PaymentResult(
      rspCode: json['rspCode'],
      message: json['message'],
      isSuccess: json['isSuccess'],
    );
  }
}
