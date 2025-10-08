import 'package:flutter/material.dart';

enum TicketType { luot, ngay, thang }

class TicketSelectionSheet extends StatefulWidget {
  final Duration rentalDuration;
  final TicketType? initialSelection;

  const TicketSelectionSheet({
    Key? key,
    required this.rentalDuration,
    this.initialSelection,
  }) : super(key: key);

  @override
  State<TicketSelectionSheet> createState() => _TicketSelectionSheetState();
}

class _TicketSelectionSheetState extends State<TicketSelectionSheet> {
  TicketType? _selected;

  @override
  void initState() {
    super.initState();
    _selected = widget.initialSelection;
  }

  @override
  Widget build(BuildContext context) {
    final ticketOptions = [TicketType.luot, TicketType.ngay, TicketType.thang];

    return DraggableScrollableSheet(
      expand: false,
      initialChildSize: 0.66,
      maxChildSize: 0.9,
      builder: (_, controller) => Container(
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        child: ListView(
          controller: controller,
          padding: const EdgeInsets.all(16),
          children: [
            Center(
              child: Container(
                width: 40,
                height: 5,
                decoration: BoxDecoration(
                  color: Colors.grey[400],
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
            ),
            const SizedBox(height: 20),
            const Text(
              "🎟 Chọn loại vé",
              style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),

            // ✅ Danh sách vé
            ...ticketOptions.map((t) {
              final isSelected = _selected == t;
              final color = {
                TicketType.luot: Colors.orange,
                TicketType.ngay: Colors.green,
                TicketType.thang: Colors.blue,
              }[t]!;

              final icon = {
                TicketType.luot: Icons.confirmation_number,
                TicketType.ngay: Icons.calendar_today,
                TicketType.thang: Icons.card_membership,
              }[t]!;

              return InkWell(
                borderRadius: BorderRadius.circular(16),
                onTap: () {
                  setState(() => _selected = t);
                  Future.delayed(const Duration(milliseconds: 250), () {
                    Navigator.pop(context, t);
                  });
                },
                child: AnimatedContainer(
                  duration: const Duration(milliseconds: 300),
                  curve: Curves.easeInOut,
                  margin: const EdgeInsets.symmetric(vertical: 10),
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    color: isSelected
                        ? color.withOpacity(0.9)
                        : color.withOpacity(0.25),
                    borderRadius: BorderRadius.circular(16),
                    boxShadow: isSelected
                        ? [
                            BoxShadow(
                              color: color.withOpacity(0.5),
                              blurRadius: 12,
                              offset: const Offset(0, 6),
                            ),
                          ]
                        : [],
                    border: Border.all(
                      color: isSelected ? Colors.black87 : Colors.transparent,
                      width: 2,
                    ),
                  ),
                  child: Row(
                    children: [
                      Icon(icon, size: 32, color: Colors.white),
                      const SizedBox(width: 16),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              _getTicketLabel(t),
                              style: const TextStyle(
                                fontSize: 18,
                                fontWeight: FontWeight.bold,
                                color: Colors.white,
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              _getTicketPrice(t),
                              style: const TextStyle(
                                fontSize: 15,
                                color: Colors.white70,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                          ],
                        ),
                      ),
                      if (isSelected)
                        const Icon(
                          Icons.check_circle,
                          color: Colors.white,
                          size: 28,
                        ),
                    ],
                  ),
                ),
              );
            }).toList(),
          ],
        ),
      ),
    );
  }

  String _getTicketLabel(TicketType t) {
    switch (t) {
      case TicketType.luot:
        return "Vé Lượt";
      case TicketType.ngay:
        return "Vé Ngày";
      case TicketType.thang:
        return "Vé Tháng";
    }
  }

  String _getTicketPrice(TicketType t) {
    switch (t) {
      case TicketType.luot:
        return "10.000đ / lượt";
      case TicketType.ngay:
        return "50.000đ / ngày";
      case TicketType.thang:
        return "Không giới hạn (theo tháng)";
    }
  }
}
