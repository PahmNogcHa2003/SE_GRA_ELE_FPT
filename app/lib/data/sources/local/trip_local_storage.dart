import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/domain/models/tracking_session.dart';

class TripLocalStorage {
  final FlutterSecureStorage storage = const FlutterSecureStorage();
  final String key = "currentTrip";

  Future<void> saveSession(TrackingSession session) async {
    await storage.write(key: key, value: jsonEncode(session.toJson()));
  }

  Future<TrackingSession?> loadSession() async {
    final raw = await storage.read(key: key);
    if (raw == null) return null;

    return TrackingSession.fromJson(jsonDecode(raw));
  }

  Future<void> clearSession() async {
    await storage.delete(key: key);
  }
}
