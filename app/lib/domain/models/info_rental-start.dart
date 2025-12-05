class RentalStartInfo {
  final bool success;
  final int rentalId;

  RentalStartInfo({required this.success, required this.rentalId});

  factory RentalStartInfo.fromJson(Map<String, dynamic> map) {
    return RentalStartInfo(
      success: map['success'] as bool,
      rentalId: map['data'] as int,
    );
  }
}
