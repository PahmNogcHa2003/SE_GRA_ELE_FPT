class Ticket {
  final int id;
  final String code;
  final String name;
  final String type;
  final String description;
  final List<TicketPrice> prices;

  Ticket({
    required this.id,
    required this.code,
    required this.name,
    required this.type,
    required this.description,
    required this.prices,
  });

  factory Ticket.fromJson(Map<String, dynamic> json) {
    return Ticket(
      id: (json['id'] as num?)?.toInt() ?? 0,
      code: json['code'] ?? '',
      name: json['name'] ?? '',
      type: json['type'] ?? '',
      description: json['description'] ?? '',
      prices: (json['prices'] as List<dynamic>? ?? [])
          .map((e) => TicketPrice.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'code': code,
      'name': name,
      'type': type,
      'description': description,
      'prices': prices.map((e) => e.toJson()).toList(),
    };
  }
}

class TicketPrice {
  final int id;
  final String vehicleType;
  final double price;
  final int? validityDays;
  final int? durationLimitMinutes;
  final int dailyFreeDurationMinutes;
  final double overageFeePer15Min;
  final int activationMode;
  final int? activationWindowDays;

  TicketPrice({
    required this.id,
    required this.vehicleType,
    required this.price,
    this.validityDays,
    this.durationLimitMinutes,
    required this.dailyFreeDurationMinutes,
    required this.overageFeePer15Min,
    required this.activationMode,
    this.activationWindowDays,
  });

  factory TicketPrice.fromJson(Map<String, dynamic> json) {
    return TicketPrice(
      id: (json['id'] as num?)?.toInt() ?? 0,
      vehicleType: json['vehicleType'] ?? '',
      price: (json['price'] as num?)?.toDouble() ?? 0.0,
      validityDays: (json['validityDays'] as num?)?.toInt(),
      durationLimitMinutes: (json['durationLimitMinutes'] as num?)?.toInt(),
      dailyFreeDurationMinutes:
          (json['dailyFreeDurationMinutes'] as num?)?.toInt() ?? 0,
      overageFeePer15Min:
          (json['overageFeePer15Min'] as num?)?.toDouble() ?? 0.0,
      activationMode: (json['activationMode'] as num?)?.toInt() ?? 0,
      activationWindowDays: (json['activationWindowDays'] as num?)?.toInt(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'vehicleType': vehicleType,
      'price': price,
      'validityDays': validityDays,
      'durationLimitMinutes': durationLimitMinutes,
      'dailyFreeDurationMinutes': dailyFreeDurationMinutes,
      'overageFeePer15Min': overageFeePer15Min,
      'activationMode': activationMode,
      'activationWindowDays': activationWindowDays,
    };
  }
}
