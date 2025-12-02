class RentalStartInfo {
  final bool success;
  final int retaalId;

  RentalStartInfo({required this.success, required this.retaalId});

  factory RentalStartInfo.fromJson(Map<String, dynamic> map) {
    return RentalStartInfo(
      success: map['success'] as bool,
      retaalId: map['data'] as int,
    );
  }
}
