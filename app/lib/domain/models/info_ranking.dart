class RankingEnd {
  final bool success;
  final String message;
  final int count;
  final List<RankingItem> data;

  RankingEnd({
    required this.success,
    required this.message,
    required this.count,
    required this.data,
  });

  factory RankingEnd.fromJson(Map<String, dynamic> json) {
    return RankingEnd(
      success: json['success'] as bool,
      message: json['message'] as String,
      count: json['count'] as int,
      data: (json['data'] as List<dynamic>)
          .map((e) => RankingItem.fromJson(e))
          .toList(),
    );
  }
}

class RankingItem {
  final int userId;
  final String fullName;
  final String avatarUrl;
  final double totalDistanceKm;
  final int totalDurationMinutes;
  final int totalTrips;
  final int rank;

  RankingItem({
    required this.userId,
    required this.fullName,
    required this.avatarUrl,
    required this.totalDistanceKm,
    required this.totalDurationMinutes,
    required this.totalTrips,
    required this.rank,
  });

  factory RankingItem.fromJson(Map<String, dynamic> json) {
    return RankingItem(
      userId: json['userId'] as int,
      fullName: json['fullName'] as String,
      avatarUrl: json['avatarUrl'] as String,
      totalDistanceKm: (json['totalDistanceKm'] as num).toDouble(),
      totalDurationMinutes: json['totalDurationMinutes'] as int,
      totalTrips: json['totalTrips'] as int,
      rank: json['rank'] as int,
    );
  }
}
