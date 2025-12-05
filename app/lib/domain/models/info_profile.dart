class InfoProfile {
  final int userId;
  final String isVerify;
  final String email;
  final String phoneNumber;
  final String fullName;
  final String dob;
  final String gender;
  final String avatarUrl;
  final String? avatarPublicId;
  final String emergencyName;
  final String emergencyPhone;
  final int provinceCode;
  final String provinceName;
  final int wardCode;
  final String wardName;
  final String addressDetail;
  final double totalDistanceKm;
  final int totalTrips;
  final int totalDurationMinutes;
  final double totalCo2SavedKg;
  final double totalCaloriesBurned;
  final String numberCard;
  final String? placeOfOrigin;
  final String? placeOfResidence;
  final String? issuedDate;
  final String? expiryDate;
  final String? issuedBy;
  final String createdAt;
  final String updatedAt;

  InfoProfile({
    required this.userId,
    required this.isVerify,
    required this.email,
    required this.phoneNumber,
    required this.fullName,
    required this.dob,
    required this.gender,
    required this.avatarUrl,
    this.avatarPublicId,
    required this.emergencyName,
    required this.emergencyPhone,
    required this.provinceCode,
    required this.provinceName,
    required this.wardCode,
    required this.wardName,
    required this.addressDetail,
    required this.totalDistanceKm,
    required this.totalTrips,
    required this.totalDurationMinutes,
    required this.totalCo2SavedKg,
    required this.totalCaloriesBurned,
    required this.numberCard,
    this.placeOfOrigin,
    this.placeOfResidence,
    this.issuedDate,
    this.expiryDate,
    this.issuedBy,
    required this.createdAt,
    required this.updatedAt,
  });

  factory InfoProfile.fromJson(Map<String, dynamic> json) {
    final data = json['data'];
    return InfoProfile(
      userId: data['userId'] as int,
      isVerify: data['isVerify'] as String,
      email: data['email'] as String,
      phoneNumber: data['phoneNumber'] as String,
      fullName: data['fullName'] as String,
      dob: data['dob'] as String,
      gender: data['gender'] as String,
      avatarUrl: data['avatarUrl'] as String,
      avatarPublicId: data['avatarPublicId'],
      emergencyName: data['emergencyName'] as String,
      emergencyPhone: data['emergencyPhone'] as String,
      provinceCode: data['provinceCode'] as int,
      provinceName: data['provinceName'] as String,
      wardCode: data['wardCode'] as int,
      wardName: data['wardName'] as String,
      addressDetail: data['addressDetail'] as String,
      totalDistanceKm: (data['totalDistanceKm'] as num).toDouble(),
      totalTrips: data['totalTrips'] as int,
      totalDurationMinutes: data['totalDurationMinutes'] as int,
      totalCo2SavedKg: (data['totalCo2SavedKg'] as num).toDouble(),
      totalCaloriesBurned: (data['totalCaloriesBurned'] as num).toDouble(),
      numberCard: data['numberCard'] as String,
      placeOfOrigin: data['placeOfOrigin'],
      placeOfResidence: data['placeOfResidence'],
      issuedDate: data['issuedDate'],
      expiryDate: data['expiryDate'],
      issuedBy: data['issuedBy'],
      createdAt: data['createdAt'] as String,
      updatedAt: data['updatedAt'] as String,
    );
  }
}
