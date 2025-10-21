import 'package:flutter/material.dart';

class MoreHeader extends StatelessWidget {
  const MoreHeader({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Text(
        "Cài đặt & chức năng mở rộng",
        style: Theme.of(context).textTheme.titleLarge,
      ),
    );
  }
}
