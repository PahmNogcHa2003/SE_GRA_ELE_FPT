// import 'package:flutter/material.dart';
// import 'presentation/screens/bike_rental_screen.dart';

// void main() {
//   runApp(BikeRentalApp());
// }

// class BikeRentalApp extends StatelessWidget {
//   const BikeRentalApp({super.key});

//   @override
//   Widget build(BuildContext context) {
//     return MaterialApp(
//       debugShowCheckedModeBanner: false,
//       title: 'Bike Rental',
//       theme: ThemeData(primarySwatch: Colors.blue),
//       home: BikeRentalScreen(),
//     );
//   }
// }

import 'package:flutter/material.dart';
import 'presentation/home/home_page.dart';
import 'presentation/login/login_page.dart';
import 'presentation/splash/splash_page.dart';

void main() {
  runApp(const SmartBikeApp());
}

class SmartBikeApp extends StatelessWidget {
  const SmartBikeApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Smart Bike',
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      routes: {
        '/': (context) => const SplashPage(),
        '/login': (context) => const LoginPage(),
        '/home': (context) => const HomePage(),
      },
      theme: ThemeData(
        primarySwatch: Colors.green,
        scaffoldBackgroundColor: Colors.grey[100],
      ),
    );
  }
}
