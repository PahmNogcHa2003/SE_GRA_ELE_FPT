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
import 'package:hola_bike_app/presentation/splash/splash_page.dart';
import 'theme/app_colors.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Elegant App',
      theme: ThemeData(
        fontFamily: 'Arial',
        scaffoldBackgroundColor: AppColors.background,
        colorScheme: ColorScheme.fromSeed(
          seedColor: AppColors.primary,
          primary: AppColors.primary,
          background: AppColors.background,
        ),
      ),
      home: const SplashScreen(),
    );
  }
}
