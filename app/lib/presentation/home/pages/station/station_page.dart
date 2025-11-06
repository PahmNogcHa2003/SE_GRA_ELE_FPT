import 'package:flutter/material.dart';
import 'widgets/station_map.dart';

class StationPage extends StatelessWidget {
  final bool showStatusCard;

  const StationPage({super.key, this.showStatusCard = true});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          const Expanded(child: StationMap()),

          // const StationCard(),
          // if (showStatusCard) ...[
          //   const SizedBox(height: 12),
          //   const StationStatusCard(),
          // ],
        ],
      ),
    );
  }
}
