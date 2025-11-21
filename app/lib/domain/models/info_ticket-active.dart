class TicketInfo {
  final int id;
  final int planPriceId;
  final String planName;
  final String vehicleType;
  final String? serialCode;
  final double? purchasedPrice;
  final String status;
  final int activationMode; // nếu ActivationModeDTO là enum thì map sang enum
  final DateTime? activatedAt;
  final DateTime? validFrom;
  final DateTime? validTo;
  final DateTime? expiresAt;
  final DateTime? activationDeadline;
  final int? remainingMinutes;
  final int? remainingRides;
  final DateTime createdAt;

  TicketInfo({
    required this.id,
    required this.planPriceId,
    required this.planName,
    required this.vehicleType,
    this.serialCode,
    this.purchasedPrice,
    required this.status,
    required this.activationMode,
    this.activatedAt,
    this.validFrom,
    this.validTo,
    this.expiresAt,
    this.activationDeadline,
    this.remainingMinutes,
    this.remainingRides,
    required this.createdAt,
  });

  factory TicketInfo.fromJson(Map<String, dynamic> json) {
    return TicketInfo(
      id: (json['id'] as num).toInt(),
      planPriceId: (json['planPriceId'] as num).toInt(),
      planName: json['planName'] as String,
      vehicleType: json['vehicleType'] as String,
      serialCode: json['serialCode'] as String?,
      purchasedPrice: json['purchasedPrice'] != null
          ? (json['purchasedPrice'] as num).toDouble()
          : null,
      status: json['status'] as String,
      activationMode: (json['activationMode'] as num).toInt(),
      activatedAt: json['activatedAt'] != null
          ? DateTime.parse(json['activatedAt'])
          : null,
      validFrom: json['validFrom'] != null
          ? DateTime.parse(json['validFrom'])
          : null,
      validTo: json['validTo'] != null ? DateTime.parse(json['validTo']) : null,
      expiresAt: json['expiresAt'] != null
          ? DateTime.parse(json['expiresAt'])
          : null,
      activationDeadline: json['activationDeadline'] != null
          ? DateTime.parse(json['activationDeadline'])
          : null,
      remainingMinutes: json['remainingMinutes'] != null
          ? (json['remainingMinutes'] as num).toInt()
          : null,
      remainingRides: json['remainingRides'] != null
          ? (json['remainingRides'] as num).toInt()
          : null,
      createdAt: DateTime.parse(json['createdAt']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'planPriceId': planPriceId,
      'planName': planName,
      'vehicleType': vehicleType,
      'serialCode': serialCode,
      'purchasedPrice': purchasedPrice,
      'status': status,
      'activationMode': activationMode,
      'activatedAt': activatedAt?.toIso8601String(),
      'validFrom': validFrom?.toIso8601String(),
      'validTo': validTo?.toIso8601String(),
      'expiresAt': expiresAt?.toIso8601String(),
      'activationDeadline': activationDeadline?.toIso8601String(),
      'remainingMinutes': remainingMinutes,
      'remainingRides': remainingRides,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}
