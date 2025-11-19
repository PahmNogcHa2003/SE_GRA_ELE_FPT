class UserInfo {
  final int userId;
  final String email;
  final String fullName;
  final String? avatarUrl;
  final DateTime createdDate;
  final String gender;
  final String addressDetail;
  final double walletBalance;
  final List<String> roles;

  UserInfo({
    required this.userId,
    required this.email,
    required this.fullName,
    required this.avatarUrl,
    required this.createdDate,
    required this.gender,
    required this.addressDetail,
    required this.walletBalance,
    required this.roles,
  });
  factory UserInfo.fromJson(Map<String, dynamic> json) {
    return UserInfo(
      userId: json['userId'],
      email: json['email'],
      fullName: json['fullName'],
      avatarUrl: json['avatarUrl'] ?? '',
      walletBalance: json['walletBalance'] ?? 0,
      createdDate: DateTime.parse(json['createdDate']),
      gender: json['gender'],
      addressDetail: json['addressDetail'],
      roles: List<String>.from(json['roles']),
    );
  }
}
