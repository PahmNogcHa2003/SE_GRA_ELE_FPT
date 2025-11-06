class WalletInfo {
  final int id;
  final int userId;
  final double balance;
  final double totalDebt;

  WalletInfo({
    required this.id,
    required this.userId,
    required this.balance,
    required this.totalDebt,
  });

  factory WalletInfo.fromJson(Map<String, dynamic> json) {
    return WalletInfo(
      id: json['id'],
      userId: json['userId'],
      balance: json['balance'].toDouble(),
      totalDebt: json['totalDebt'].toDouble(),
    );
  }
}
