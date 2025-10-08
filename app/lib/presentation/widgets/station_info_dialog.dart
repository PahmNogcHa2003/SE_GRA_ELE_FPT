import 'package:flutter/material.dart';
import '../../core/constants/locations.dart';

class StationInfoDialog extends StatelessWidget {
  final Station station;

  const StationInfoDialog({super.key, required this.station});

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(station.name),
      content: Text('${station.name}'),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Đóng'),
        ),
      ],
    );
  }
}
