class InfoRentalEnd {
  final bool success;

  InfoRentalEnd({required this.success});

  factory InfoRentalEnd.fromJson(Map<String, dynamic> json) {
    return InfoRentalEnd(success: json['success']);
  }
}
