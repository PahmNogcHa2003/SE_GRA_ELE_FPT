import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:hola_bike_app/domain/models/info_ticket-active.dart';
import 'package:hola_bike_app/data/sources/remote/api_ticket-active.dart';

class MyTicketPage extends StatefulWidget {
  const MyTicketPage({Key? key}) : super(key: key);

  @override
  State<MyTicketPage> createState() => _MyTicketPageState();
}

class _MyTicketPageState extends State<MyTicketPage>
    with SingleTickerProviderStateMixin {
  final UserTicketApi _api = UserTicketApi();
  final secureStorage = const FlutterSecureStorage();

  late Future<List<TicketInfo>> _ticketsFuture;
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _loadTickets();
  }

  Future<void> _loadTickets() async {
    final token = await secureStorage.read(key: 'access_token');
    if (token == null) throw Exception('Token không tồn tại');
    setState(() {
      _ticketsFuture = _api.getActiveTickets(token);
    });
  }

  List<TicketInfo> _filterTickets(List<TicketInfo> tickets, String keyword) {
    return tickets
        .where((t) => t.planName.toLowerCase().contains(keyword))
        .toList();
  }

  Color _getCardColor(String name) {
    final lower = name.toLowerCase();
    if (lower.contains("1 lượt")) return Colors.yellow[100]!;
    if (lower.contains("ngày")) return Colors.green[100]!;
    if (lower.contains("tháng")) return Colors.lightBlue[100]!;
    return AppColors.card;
  }

  Widget _buildTicketCard(TicketInfo ticket) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      constraints: const BoxConstraints(minHeight: 180),
      child: Card(
        color: _getCardColor(ticket.planName),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        elevation: 3,
        child: SizedBox(
          width: double.infinity, // đảm bảo chiều ngang full
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Align(
              alignment: Alignment.centerLeft, // căn trái nội dung
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    ticket.planName,
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: AppColors.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    "Phương tiện: ${ticket.vehicleType}\n"
                    "Trạng thái: ${ticket.status}\n"
                    "Giá mua: ${ticket.purchasedPrice ?? 0} VNĐ",
                    style: const TextStyle(
                      fontSize: 14,
                      color: AppColors.textSecondary,
                    ),
                  ),
                  const SizedBox(height: 8),
                  if (ticket.validFrom != null && ticket.validTo != null)
                    Text(
                      "Hiệu lực: ${ticket.validFrom} → ${ticket.validTo}",
                      style: const TextStyle(
                        fontSize: 14,
                        color: AppColors.textSecondary,
                      ),
                    ),
                  if (ticket.remainingMinutes != null)
                    Text(
                      "Thời gian còn lại: ${ticket.remainingMinutes} phút",
                      style: const TextStyle(
                        fontSize: 14,
                        color: AppColors.textSecondary,
                      ),
                    ),
                  if (ticket.remainingRides != null)
                    Text(
                      "Số lượt còn lại: ${ticket.remainingRides}",
                      style: const TextStyle(
                        fontSize: 14,
                        color: AppColors.textSecondary,
                      ),
                    ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildTabContent(List<TicketInfo> tickets) {
    if (tickets.isEmpty) {
      return const Center(child: Text("Không có vé loại này"));
    }
    return SingleChildScrollView(
      child: Column(children: tickets.map(_buildTicketCard).toList()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text("Vé của tôi"),
        backgroundColor: AppColors.primary,
        bottom: TabBar(
          controller: _tabController,
          indicatorColor: AppColors.card,
          labelColor: Colors.white,
          unselectedLabelColor: AppColors.textPrimary,
          tabs: const [
            Tab(text: "Vé 1 lượt"),
            Tab(text: "Vé ngày"),
            Tab(text: "Vé tháng"),
          ],
        ),
      ),
      body: FutureBuilder<List<TicketInfo>>(
        future: _ticketsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(child: Text("Lỗi: ${snapshot.error}"));
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text("Bạn chưa có vé nào"));
          }

          final tickets = snapshot.data!;
          final tabViews = [
            _filterTickets(tickets, "1 lượt"),
            _filterTickets(tickets, "ngày"),
            _filterTickets(tickets, "tháng"),
          ];

          return TabBarView(
            controller: _tabController,
            children: tabViews.map(_buildTabContent).toList(),
          );
        },
      ),
    );
  }
}

Color _getCardColor(String name) {
  final lower = name.toLowerCase();
  if (lower.contains("1 lượt")) {
    return Colors.yellow[100]!; // màu vàng nhạt
  } else if (lower.contains("ngày")) {
    return Colors.green[100]!; // màu xanh lá nhạt
  } else if (lower.contains("tháng")) {
    return Colors.lightBlue[100]!; // màu xanh da trời nhạt
  }
  return AppColors.card; // mặc định
}
