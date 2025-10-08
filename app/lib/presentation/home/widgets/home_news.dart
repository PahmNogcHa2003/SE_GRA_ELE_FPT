import 'package:flutter/material.dart';
import '../../../theme/app_colors.dart';

class HomeNews extends StatelessWidget {
  const HomeNews({super.key});

  @override
  Widget build(BuildContext context) {
    final news = [
      {
        'title': '🎉 Sinh nhật Hola Bike rộn ràng',
        'img':
            'https://img.freepik.com/free-vector/realistic-golden-wheel-fortune-background_23-2149639949.jpg',
      },
      {
        'title': '🚴 Ưu đãi khi nạp điểm',
        'img':
            'https://img.freepik.com/free-vector/people-riding-bike-city_23-2148444190.jpg',
      },
      {
        'title': '🌿 Đi xe xanh - Sống xanh',
        'img':
            'https://img.freepik.com/free-vector/eco-bike-illustration_23-2148482403.jpg',
      },
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          "📰 Tin tức",
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: AppColors.textPrimary,
          ),
        ),
        const SizedBox(height: 12),
        SizedBox(
          height: 150,
          child: ListView.builder(
            scrollDirection: Axis.horizontal,
            itemCount: news.length,
            itemBuilder: (_, i) {
              final item = news[i];
              return Container(
                width: 220,
                margin: EdgeInsets.only(right: i == news.length - 1 ? 0 : 12),
                decoration: BoxDecoration(
                  color: AppColors.card,
                  borderRadius: BorderRadius.circular(16),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.grey.withOpacity(0.08),
                      blurRadius: 6,
                      offset: const Offset(0, 3),
                    ),
                  ],
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    ClipRRect(
                      borderRadius: const BorderRadius.vertical(
                        top: Radius.circular(16),
                      ),
                      child: Image.network(
                        item['img']!,
                        height: 90,
                        width: double.infinity,
                        fit: BoxFit.cover,
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: Text(
                        item['title']!,
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w500,
                        ),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ],
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}
