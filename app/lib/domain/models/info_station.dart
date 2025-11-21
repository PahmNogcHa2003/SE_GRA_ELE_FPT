class StationInfo {
  final String name;
  final String location;
  final int capacity;
  final double lat;
  final double lng;
  final bool isActive;
  final String image;
  final int vehicleAvailable;
  final double? distanceKm;
  final int id;

  StationInfo({
    required this.name,
    required this.location,
    required this.capacity,
    required this.lat,
    required this.lng,
    required this.isActive,
    required this.image,
    required this.vehicleAvailable,
    this.distanceKm,
    required this.id,
  });

  factory StationInfo.fromJson(Map<String, dynamic> json) {
    return StationInfo(
      name: json['name'],
      location: json['location'],
      capacity: json['capacity'],
      lat: json['lat'].toDouble(),
      lng: json['lng'].toDouble(),
      isActive: json['isActive'],
      image: json['image'],
      vehicleAvailable: json['vehicleAvailable'],
      distanceKm: json['distanceKm'] != null
          ? json['distanceKm'].toDouble()
          : null,
      id: json['id'],
    );
  }
}
