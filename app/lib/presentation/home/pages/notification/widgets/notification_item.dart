import 'package:flutter/material.dart';

class NotificationItem extends StatelessWidget {
  final String title;

  const NotificationItem({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: const Icon(Icons.notifications),
      title: Text(title),
    );
  }
}
