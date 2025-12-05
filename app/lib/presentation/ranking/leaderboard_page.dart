import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_ranking.dart';
import 'package:hola_bike_app/domain/models/info_ranking.dart';

class LeaderboardPage extends StatefulWidget {
  const LeaderboardPage({super.key});

  @override
  State<LeaderboardPage> createState() => _LeaderboardPageState();
}

class _LeaderboardPageState extends State<LeaderboardPage> {
  final secureStorage = const FlutterSecureStorage();
  final RankingUsecase _usecase = RankingUsecase();

  RankingEnd? ranking;

  @override
  void initState() {
    super.initState();
    _loadRanking();
  }

  Future<void> _loadRanking() async {
    EasyLoading.show();
    try {
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) throw Exception('Không tìm thấy access token');

      final res = await _usecase.execute(token: token);

      setState(() {
        ranking = res;
      });
    } catch (e) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(const SnackBar(content: Text('Lỗi tải bảng xếp hạng')));
      });
    } finally {
      EasyLoading.dismiss();
    }
  }

  @override
  Widget build(BuildContext context) {
    if (ranking == null) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    final data = ranking!.data;

    return Scaffold(
      appBar: AppBar(title: const Text('Bảng xếp hạng')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            if (data.length > 3)
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  PodiumItem(
                    rank: data[1].rank,
                    name: data[1].fullName,
                    avatarUrl: data[1].avatarUrl,
                    distance: data[1].totalDistanceKm,
                    color: Colors.grey.shade400,
                    height: 140,
                  ),
                  const SizedBox(width: 12),
                  PodiumItem(
                    rank: data[0].rank,
                    name: data[0].fullName,
                    avatarUrl: data[0].avatarUrl,
                    distance: data[0].totalDistanceKm,
                    color: Colors.amber,
                    height: 160,
                    crown: true,
                  ),
                  const SizedBox(width: 12),
                  PodiumItem(
                    rank: data[2].rank,
                    name: data[2].fullName,
                    avatarUrl: data[2].avatarUrl,
                    distance: data[2].totalDistanceKm,
                    color: const Color(0xFFCD7F32),
                    height: 120,
                  ),
                ],
              )
            else
              ListView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: data.length,
                itemBuilder: (context, index) {
                  final item = data[index];
                  return Container(
                    margin: const EdgeInsets.symmetric(vertical: 8),
                    padding: const EdgeInsets.symmetric(horizontal: 8),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(12),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.05),
                          blurRadius: 6,
                          offset: const Offset(0, 3),
                        ),
                      ],
                      border: Border.all(color: Colors.grey.shade300),
                    ),
                    child: ListTile(
                      leading: CircleAvatar(
                        backgroundImage: item.avatarUrl.isNotEmpty
                            ? NetworkImage(item.avatarUrl)
                            : null,
                        child: item.avatarUrl.isEmpty
                            ? const Icon(Icons.person)
                            : null,
                      ),
                      title: Text(item.fullName),
                      subtitle: Text(
                        '${item.totalDistanceKm.toStringAsFixed(1)} km',
                      ),
                      trailing: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text('Top ${item.rank}'),
                          if (item.rank == 1)
                            const Padding(
                              padding: EdgeInsets.only(left: 4),
                              child: Icon(
                                Icons.emoji_events,
                                color: Colors.amber,
                                size: 20,
                              ),
                            ),
                        ],
                      ),
                    ),
                  );
                },
              ),

            const SizedBox(height: 24),

            if (data.length > 3)
              ListView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: data.length - 3,
                itemBuilder: (context, index) {
                  final item = data[index + 3];
                  return Container(
                    margin: const EdgeInsets.symmetric(vertical: 8),
                    padding: const EdgeInsets.symmetric(horizontal: 8),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(12),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.05),
                          blurRadius: 6,
                          offset: const Offset(0, 3),
                        ),
                      ],
                      border: Border.all(color: Colors.grey.shade300),
                    ),
                    child: ListTile(
                      leading: CircleAvatar(
                        backgroundImage: item.avatarUrl.isNotEmpty
                            ? NetworkImage(item.avatarUrl)
                            : null,
                        child: item.avatarUrl.isEmpty
                            ? const Icon(Icons.person)
                            : null,
                      ),
                      title: Text(item.fullName),
                      subtitle: Text(
                        '${item.totalDistanceKm.toStringAsFixed(1)} km',
                      ),
                      trailing: Text('Top ${item.rank}'),
                    ),
                  );
                },
              ),
          ],
        ),
      ),
    );
  }
}

class PodiumItem extends StatelessWidget {
  final int rank;
  final String name;
  final String? avatarUrl;
  final double distance;
  final Color color;
  final double height;
  final bool crown;

  const PodiumItem({
    required this.rank,
    required this.name,
    required this.avatarUrl,
    required this.distance,
    required this.color,
    required this.height,
    this.crown = false,
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 4),
      padding: const EdgeInsets.all(8),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.08),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
        border: Border.all(color: Colors.grey.shade300),
      ),
      child: Column(
        children: [
          if (crown)
            const Icon(Icons.emoji_events, color: Colors.amber, size: 36),
          Container(
            padding: const EdgeInsets.all(3),
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(color: color, width: 3),
            ),
            child: CircleAvatar(
              radius: 32,
              backgroundImage: avatarUrl != null && avatarUrl!.isNotEmpty
                  ? NetworkImage(avatarUrl!)
                  : null,
              child: (avatarUrl == null || avatarUrl!.isEmpty)
                  ? const Icon(Icons.person, size: 32)
                  : null,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            name,
            style: const TextStyle(fontWeight: FontWeight.bold),
            textAlign: TextAlign.center,
          ),
          Text(
            '${distance.toStringAsFixed(1)} km',
            style: const TextStyle(color: Colors.grey),
          ),
          const SizedBox(height: 6),
          Container(
            width: 70,
            height: height,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [color.withOpacity(0.8), color],
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
              ),
              borderRadius: BorderRadius.circular(8),
            ),
            alignment: Alignment.center,
            child: Text(
              '$rank',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: Colors.white,
                fontSize: 18,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
