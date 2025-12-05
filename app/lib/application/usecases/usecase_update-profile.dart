import 'dart:io';

import 'package:hola_bike_app/data/sources/remote/api_update-profile.dart';

class UpdateProfile {
  final UserProfileApi _api = UserProfileApi();

  Future<bool> updateProfile({
    required String token,
    required String fullName,
    required String emergencyName,
    required String emergencyPhone,
    required String addressDetail,
    required String phoneNumber,
    required String gender,
    required String dob,
  }) {
    return _api.updateUserProfile(
      token: token,
      fullName: fullName,
      emergencyName: emergencyName,
      emergencyPhone: emergencyPhone,
      addressDetail: addressDetail,
      phoneNumber: phoneNumber,
      gender: gender,
      dob: dob,
    );
  }

  Future<bool> updateAvatar({required String token, required File avatarFile}) {
    return _api.uploadAvatar(token: token, avatarFile: avatarFile);
  }
}
