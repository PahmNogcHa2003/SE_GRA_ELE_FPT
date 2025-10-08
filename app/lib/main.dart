import 'package:flutter/material.dart';
import 'presentation/screens/bike_rental_screen.dart';

void main() {
  runApp(BikeRentalApp());
}

class BikeRentalApp extends StatelessWidget {
  const BikeRentalApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Bike Rental',
      theme: ThemeData(primarySwatch: Colors.blue),
      home: BikeRentalScreen(),
    );
  }
}
