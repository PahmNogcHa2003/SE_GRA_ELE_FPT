import 'package:flutter/material.dart';

class NotificationNews extends StatelessWidget {
  const NotificationNews({super.key});

  @override
  Widget build(BuildContext context) {
    final List<Map<String, String>> news = [
      {
        "title": "EcoJourney ra mắt dịch vụ thuê xe ô tô",
        "summary":
            "Từ tháng 11, người dùng có thể thuê xe ô tô trực tiếp qua ứng dụng EcoJourney.",
        "date": "15/10/2025",
      },
      {
        "title": "Cập nhật bảng giá mới",
        "summary":
            "Giá thuê xe đạp và xe điện được điều chỉnh nhẹ để phù hợp với nhu cầu.",
        "date": "14/10/2025",
      },
      {
        "title": "EcoJourney hợp tác cùng Đại học Quốc gia",
        "summary":
            "Sinh viên được ưu đãi 30% khi sử dụng dịch vụ tại các trạm trong khuôn viên trường.",
        "date": "12/10/2025",
      },
    ];

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: news.length,
      separatorBuilder: (_, __) => const Divider(height: 1),
      itemBuilder: (context, index) {
        final item = news[index];
        return ListTile(
          leading: const Icon(Icons.article_outlined, color: Colors.green),
          title: Text(item['title'] ?? ''),
          subtitle: Text(item['summary'] ?? ''),
          trailing: Text(
            item['date'] ?? '',
            style: const TextStyle(fontSize: 12),
          ),
          onTap: () {
            // TODO: xử lý khi nhấn vào tin tức
          },
        );
      },
    );
  }
}
