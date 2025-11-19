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
      id: json['id'] as int,
      code: json['code'] as String,
      name: json['name'] as String,
      type: json['type'] as String,
      description: json['description'] as String,
      prices: (json['prices'] as List<dynamic>)
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
      id: json['id'] as int,
      vehicleType: json['vehicleType'] as String,
      price: json['price'] as double,
      validityDays: json['validityDays'] as int?,
      durationLimitMinutes: json['durationLimitMinutes'] as int?,
      dailyFreeDurationMinutes: json['dailyFreeDurationMinutes'] as int,
      overageFeePer15Min: json['overageFeePer15Min'] as double,
      activationMode: json['activationMode'] as int,
      activationWindowDays: json['activationWindowDays'] as int?,
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
