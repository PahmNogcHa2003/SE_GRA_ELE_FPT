import 'package:flutter/material.dart';
import 'notification_item.dart';

class NotificationList extends StatelessWidget {
  const NotificationList({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      children: const [
        NotificationItem(title: "🎉 Sinh nhật Hola Bike rộn ràng"),
        NotificationItem(title: "🏍️ Ưu đãi khi nạp điểm"),
      ],
    );
  }
}
