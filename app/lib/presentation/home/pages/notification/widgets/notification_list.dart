import 'package:flutter/material.dart';

class NotificationList extends StatelessWidget {
  const NotificationList({super.key});

  @override
  Widget build(BuildContext context) {
    final List<Map<String, String>> notifications = [
      {
        "title": "Bạn vừa nhận được ưu đãi mới",
        "subtitle": "Giảm 20% khi thuê xe điện trong hôm nay",
        "time": "2 phút trước",
      },
      {
        "title": "Trạm xe gần bạn đã cập nhật",
        "subtitle": "Trạm Yên Hòa hiện có 5 xe đạp mới",
        "time": "1 giờ trước",
      },
      {
        "title": "Tài khoản đã được xác thực",
        "subtitle": "Chúc mừng! Tài khoản của bạn đã được xác thực thành công",
        "time": "Hôm qua",
      },
    ];

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: notifications.length,
      separatorBuilder: (_, __) => const Divider(height: 1),
      itemBuilder: (context, index) {
        final item = notifications[index];
        return ListTile(
          leading: const Icon(Icons.notifications, color: Colors.green),
          title: Text(item['title'] ?? ''),
          subtitle: Text(item['subtitle'] ?? ''),
          trailing: Text(
            item['time'] ?? '',
            style: const TextStyle(fontSize: 12),
          ),
          onTap: () {
            // TODO: xử lý khi nhấn vào thông báo
          },
        );
      },
    );
  }
}
