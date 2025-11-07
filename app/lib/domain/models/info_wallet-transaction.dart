class WalletTransactionInfo {
  final String direction;
  final double amount;
  final String source;
  final double balanceAfter;
  final DateTime createdAt;

  WalletTransactionInfo({
    required this.direction,
    required this.amount,
    required this.source,
    required this.balanceAfter,
    required this.createdAt,
  });

  factory WalletTransactionInfo.fromJson(Map<String, dynamic> json) {
    return WalletTransactionInfo(
      direction: json['direction'],
      amount: json['amount'].toDouble(),
      source: json['source'],
      balanceAfter: json['balanceAfter'].toDouble(),
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}
