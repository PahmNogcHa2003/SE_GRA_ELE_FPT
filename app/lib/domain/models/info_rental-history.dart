class InfoRentalHistory {
  final int rentalId;
  final String startTimeUtc;
  final String endTimeUtc;
  final String startTimeVn;
  final String endTimeVn;
  final String startStationName;
  final String endStationName;
  final String vehicleCode;
  final String vehicleType;
  final int userTicketId;
  final String ticketPlanName;
  final String ticketType;
  final String ticketVehicleType;
  final int durationMinutes;
  final double distanceKm;
  final double co2SavedKg;
  final double caloriesBurned;
  final int? overusedMinutes;
  final double? overusedFee;
  final bool isOvertime;
  final String status;

  InfoRentalHistory({
    required this.rentalId,
    required this.startTimeUtc,
    required this.endTimeUtc,
    required this.startTimeVn,
    required this.endTimeVn,
    required this.startStationName,
    required this.endStationName,
    required this.vehicleCode,
    required this.vehicleType,
    required this.userTicketId,
    required this.ticketPlanName,
    required this.ticketType,
    required this.ticketVehicleType,
    required this.durationMinutes,
    required this.distanceKm,
    required this.co2SavedKg,
    required this.caloriesBurned,
    this.overusedMinutes,
    this.overusedFee,
    required this.isOvertime,
    required this.status,
  });

  factory InfoRentalHistory.fromJson(Map<String, dynamic> json) {
    return InfoRentalHistory(
      rentalId: json['rentalId'] ?? 0,
      startTimeUtc: json['startTimeUtc'] ?? '',
      endTimeUtc: json['endTimeUtc'] ?? '',
      startTimeVn: json['startTimeVn'] ?? '',
      endTimeVn: json['endTimeVn'] ?? '',
      startStationName: json['startStationName'] ?? '',
      endStationName: json['endStationName'] ?? '',
      vehicleCode: json['vehicleCode'] ?? '',
      vehicleType: json['vehicleType'] ?? '',
      userTicketId: json['userTicketId'] ?? 0,
      ticketPlanName: json['ticketPlanName'] ?? '',
      ticketType: json['ticketType'] ?? '',
      ticketVehicleType: json['ticketVehicleType'] ?? '',
      durationMinutes: json['durationMinutes'] ?? 0,
      distanceKm: (json['distanceKm'] as num?)?.toDouble() ?? 0.0,
      co2SavedKg: (json['co2SavedKg'] as num?)?.toDouble() ?? 0.0,
      caloriesBurned: (json['caloriesBurned'] as num?)?.toDouble() ?? 0.0,
      overusedMinutes: (json['overusedMinutes'] as num?)?.toInt(),
      overusedFee: (json['overusedFee'] as num?)?.toDouble(),
      isOvertime: json['isOvertime'] ?? false,
      status: json['status'] ?? '',
    );
  }
}
