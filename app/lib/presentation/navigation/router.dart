import 'package:flutter/material.dart';
import 'package:hola_bike_app/presentation/home/pages/station/station_page.dart';
// import 'package:hola_bike_app/presentation/home/pages/ticket/ticket_page.dart';
// import 'package:hola_bike_app/presentation/home/pages/trip/trip_page.dart';
// import 'package:hola_bike_app/presentation/home/pages/card/card_page.dart';
// import 'package:hola_bike_app/presentation/home/pages/guide/guide_page.dart';
// import 'package:hola_bike_app/presentation/home/pages/message/message_page.dart';

final Map<String, WidgetBuilder> appRoutes = {
  '/stations': (context) => const StationPage(),
  // '/tickets': (context) => const TicketPage(),
  // '/trips': (context) => const TripPage(),
  // '/cards': (context) => const CardPage(),
  // '/guide': (context) => const GuidePage(),
  // '/messages': (context) => const MessagePage(),
};
