import 'package:latlong2/latlong.dart';

class TrackingSession {
  final int rentalId;
  final DateTime startTime;
  final double totalMeters;
  final List<LatLng> routePoints;

  TrackingSession({
    required this.rentalId,
    required this.startTime,
    required this.totalMeters,
    required this.routePoints,
  });

  Map<String, dynamic> toJson() => {
    "rentalId": rentalId,
    "startTime": startTime.toIso8601String(),
    "totalMeters": totalMeters,
    "routePoints": routePoints
        .map((p) => {"lat": p.latitude, "lng": p.longitude})
        .toList(),
  };

  static TrackingSession fromJson(Map<String, dynamic> json) {
    return TrackingSession(
      rentalId: json["rentalId"],
      startTime: DateTime.parse(json["startTime"]),
      totalMeters: (json["totalMeters"] as num).toDouble(),
      routePoints: (json["routePoints"] as List)
          .map((p) => LatLng(p["lat"], p["lng"]))
          .toList(),
    );
  }
}
