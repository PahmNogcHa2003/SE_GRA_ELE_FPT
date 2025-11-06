class UserInfo {
  final int userId;
  final String email;
  final String fullName;
  final String? avatarUrl;
  final DateTime createdDate;
  final DateTime dob;
  final String gender;
  final String addressDetail;
  final List<String> roles;

  UserInfo({
    required this.userId,
    required this.email,
    required this.fullName,
    required this.avatarUrl,
    required this.createdDate,
    required this.dob,
    required this.gender,
    required this.addressDetail,
    required this.roles,
  });

  factory UserInfo.fromJson(Map<String, dynamic> json) {
    return UserInfo(
      userId: json['userId'],
      email: json['email'],
      fullName: json['fullName'],
      avatarUrl: json['avatarUrl'],
      createdDate: DateTime.parse(json['createdDate']),
      dob: DateTime.parse(json['dob']),
      gender: json['gender'],
      addressDetail: json['addressDetail'],
      roles: List<String>.from(json['roles']),
    );
  }
}
