class RentalRequest {
  final String fullName;
  final String phoneNumber;
  final String city;
  final int quantity;
  final DateTime startDate;
  final DateTime endDate;
  final int stationStartId;
  final int stationEndId;

  RentalRequest({
    required this.fullName,
    required this.phoneNumber,
    required this.city,
    required this.quantity,
    required this.startDate,
    required this.endDate,
    required this.stationStartId,
    required this.stationEndId,
  });

  factory RentalRequest.fromJson(Map<String, dynamic> json) {
    return RentalRequest(
      fullName: json['fullName'] as String,
      phoneNumber: json['phoneNumber'] as String,
      city: json['city'] as String,
      quantity: json['quantity'] as int,
      startDate: DateTime.parse(json['startDate'] as String),
      endDate: DateTime.parse(json['endDate'] as String),
      stationStartId: json['station_start_id'] as int,
      stationEndId: json['station_end_id'] as int,
    );
  }

  Map<String, dynamic> toJson() => {
    'fullName': fullName,
    'phoneNumber': phoneNumber,
    'city': city,
    'quantity': quantity,
    'startDate': startDate.toIso8601String(),
    'endDate': endDate.toIso8601String(),
    'station_start_id': stationStartId,
    'station_end_id': stationEndId,
  };
}
