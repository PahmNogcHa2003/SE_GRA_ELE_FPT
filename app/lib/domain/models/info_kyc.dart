class KycInfo {
  final bool success;

  KycInfo({required this.success});

  factory KycInfo.fromJson(Map<String, dynamic> json) {
    return KycInfo(success: json['success']);
  }
}
