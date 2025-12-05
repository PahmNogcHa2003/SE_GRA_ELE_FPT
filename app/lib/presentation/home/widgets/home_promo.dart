import 'package:flutter/material.dart';
import '../../../theme/app_colors.dart';

class HomePromo extends StatefulWidget {
  const HomePromo({super.key});

  @override
  State<HomePromo> createState() => _HomePromoState();
}

class _HomePromoState extends State<HomePromo> {
  final controller = PageController();
  int _index = 0;

  final promos = [
    'https://res.cloudinary.com/dt7e4s6mr/image/upload/v1763397692/holabike/news/th8r2e7fixddy0wo9201.jpg',
    'https://res.cloudinary.com/dt7e4s6mr/image/upload/v1763397093/holabike/news/mjqzqbgg16kyepxr7rsb.png',
    'https://res.cloudinary.com/dt7e4s6mr/image/upload/v1763397667/holabike/news/x4agn5qcqh3xz6vhdicn.jpg',
    'https://res.cloudinary.com/dt7e4s6mr/image/upload/v1763397637/holabike/news/haishhp4cei8xnejljht.png',
  ];

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          "🎁 Ưu đãi & Khuyến mãi",
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: AppColors.textPrimary,
          ),
        ),
        const SizedBox(height: 12),
        SizedBox(
          height: 150,
          child: PageView.builder(
            controller: controller,
            onPageChanged: (i) => setState(() => _index = i),
            itemCount: promos.length,
            itemBuilder: (_, i) {
              return AnimatedContainer(
                duration: const Duration(milliseconds: 300),
                margin: EdgeInsets.symmetric(horizontal: i == _index ? 4 : 12),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  image: DecorationImage(
                    image: NetworkImage(promos[i]),
                    fit: BoxFit.cover,
                  ),
                ),
              );
            },
          ),
        ),
        const SizedBox(height: 8),
        Center(
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: List.generate(promos.length, (i) {
              return AnimatedContainer(
                duration: const Duration(milliseconds: 300),
                margin: const EdgeInsets.symmetric(horizontal: 4),
                height: 6,
                width: i == _index ? 18 : 6,
                decoration: BoxDecoration(
                  color: i == _index
                      ? AppColors.primary
                      : AppColors.primary.withOpacity(0.3),
                  borderRadius: BorderRadius.circular(12),
                ),
              );
            }),
          ),
        ),
      ],
    );
  }
}
