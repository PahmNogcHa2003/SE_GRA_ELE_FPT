import 'package:flutter/material.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class PricingPage extends StatelessWidget {
  const PricingPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Bảng giá dịch vụ"),
        backgroundColor: AppColors.primary,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            _buildTicketCard(
              icon: Icons.confirmation_number,
              title: "🎫 Vé lượt",
              details: [
                "10.000 điểm TNGO/lượt",
                "Thời lượng: 60 phút",
                "Cước phí quá thời lượng: 3.000 điểm/15 phút",
                "Yêu cầu số dư tối thiểu: 20.000 điểm",
              ],
              color: Colors.blue.shade50,
            ),
            const SizedBox(height: 16),
            _buildTicketCard(
              icon: Icons.calendar_today,
              title: "📅 Vé ngày",
              details: [
                "50.000 điểm TNGO/ngày",
                "Thời lượng: 450 phút",
                "Hạn dùng: 24h từ lúc đăng ký",
                "Cước phí quá thời lượng: 3.000 điểm/15 phút",
              ],
              color: Colors.green.shade50,
            ),
            const SizedBox(height: 16),
            _buildTicketCard(
              icon: Icons.calendar_month,
              title: "📆 Vé tháng",
              details: [
                "79.000 điểm TNGO/tháng",
                "Miễn phí tất cả chuyến đi dưới 45 phút",
                "Hạn dùng: 30 ngày",
                "Cước phí quá thời lượng: 3.000 điểm/15 phút",
              ],
              color: Colors.purple.shade50,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTicketCard({
    required IconData icon,
    required String title,
    required List<String> details,
    required Color color,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey.shade300),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: AppColors.primary),
              const SizedBox(width: 8),
              Text(
                title,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          ...details.map(
            (text) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 2),
              child: Text("• $text", style: const TextStyle(fontSize: 14)),
            ),
          ),
        ],
      ),
    );
  }
}
