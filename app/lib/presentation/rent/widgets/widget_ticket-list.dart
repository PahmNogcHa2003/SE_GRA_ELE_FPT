import 'package:flutter/material.dart';
import 'package:hola_bike_app/domain/models/info_ticket-active.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:intl/intl.dart';

class TicketListWidget extends StatelessWidget {
  final List<TicketInfo> tickets;
  final int? selectedTicketId;
  final Function(int) onSelect;

  const TicketListWidget({
    super.key,
    required this.tickets,
    required this.selectedTicketId,
    required this.onSelect,
  });

  String _formatDate(DateTime? date) {
    if (date == null) return '---';
    return DateFormat('dd/MM/yyyy HH:mm').format(date);
  }

  String _formatPrice(double? price) {
    if (price == null) return '---';
    final formatter = NumberFormat.currency(locale: 'vi_VN', symbol: '₫');
    return formatter.format(price);
  }

  @override
  Widget build(BuildContext context) {
    if (tickets.isEmpty) {
      return const Text(
        'Không có vé khả dụng',
        style: TextStyle(color: AppColors.textSecondary),
      );
    }

    return ListView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: tickets.length,
      itemBuilder: (context, index) {
        final ticket = tickets[index];
        return Card(
          margin: const EdgeInsets.symmetric(vertical: 8),
          child: ListTile(
            leading: const Icon(
              Icons.confirmation_number,
              color: AppColors.primary,
            ),
            title: Text(ticket.planName),
            subtitle: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Loại xe: ${ticket.vehicleType}'),
                Text('Mã vé: ${ticket.serialCode ?? '---'}'),
                Text('Giá: ${_formatPrice(ticket.purchasedPrice)}'),
                Text('Trạng thái: ${ticket.status}'),
                if (ticket.validFrom != null && ticket.validTo != null)
                  Text(
                    'Hiệu lực: ${_formatDate(ticket.validFrom)} → ${_formatDate(ticket.validTo)}',
                  ),
                if ((ticket.remainingMinutes ?? 0) > 0)
                  Text('Còn lại: ${ticket.remainingMinutes} phút'),
                if ((ticket.remainingRides ?? 0) > 0)
                  Text('Số lượt còn lại: ${ticket.remainingRides}'),
              ],
            ),
            trailing: Radio<int>(
              value: ticket.id,
              groupValue: selectedTicketId,
              onChanged: (val) => onSelect(val!),
            ),
          ),
        );
      },
    );
  }
}
