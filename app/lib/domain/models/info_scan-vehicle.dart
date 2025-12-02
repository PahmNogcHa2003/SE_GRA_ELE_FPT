class InfoScanVehicle {
  final String bikeCode;
  final String categoryName;
  final String vehicleStatus;
  final String stationName;

  InfoScanVehicle({
    required this.bikeCode,
    required this.categoryName,
    required this.stationName,
    required this.vehicleStatus,
  });

  factory InfoScanVehicle.fromJson(Map<String, dynamic> json) {
    return InfoScanVehicle(
      bikeCode: json['bikeCode'] ?? '',
      categoryName: json['categoryName'] ?? '',
      vehicleStatus: json['vehicleStatus'] ?? '',
      stationName: json['stationName'] ?? '',
    );
  }
}
