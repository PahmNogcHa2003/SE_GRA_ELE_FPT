import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_get-ticket-market.dart';
import 'package:hola_bike_app/application/usecases/usecase_purchase_ticket.dart';
import 'package:hola_bike_app/core/utils/toast_util.dart';
import 'package:hola_bike_app/domain/models/info_ticket-market.dart';
import 'package:hola_bike_app/theme/app_colors.dart';

class TicketPage extends StatefulWidget {
  const TicketPage({Key? key}) : super(key: key);

  @override
  State<TicketPage> createState() => _TicketPageState();
}

class _TicketPageState extends State<TicketPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  int selectedTicketIndex = 0;
  final secureStorage = const FlutterSecureStorage();
  final List<String> ticketTypes = ["Vé 1 lượt", "Vé ngày", "Vé tháng"];
  final MarketTicketUsecase _usecase = MarketTicketUsecase();

  late Future<List<Ticket>> _bikeTicketsFuture;
  late Future<List<Ticket>> _ebikeTicketsFuture;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _loadTickets(); // gọi hàm async riêng
  }

  Future<void> _loadTickets() async {
    final token = await secureStorage.read(key: 'access_token');
    if (token == null) {
      throw Exception('Token không tồn tại');
    }

    setState(() {
      _bikeTicketsFuture = _usecase.execute(token, "bike");
      _ebikeTicketsFuture = _usecase.execute(token, "Ebike");
    });
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Widget _buildTicketSelector() {
    return Wrap(
      spacing: 12,
      children: List.generate(ticketTypes.length, (index) {
        final isSelected = selectedTicketIndex == index;
        return ChoiceChip(
          label: Text(
            ticketTypes[index],
            style: TextStyle(
              color: isSelected ? Colors.white : AppColors.textPrimary,
              fontWeight: FontWeight.bold,
            ),
          ),
          selected: isSelected,
          selectedColor: AppColors.primary,
          backgroundColor: AppColors.card,
          onSelected: (_) {
            setState(() {
              selectedTicketIndex = index;
            });
          },
        );
      }),
    );
  }

  Widget _buildTicketList(List<Ticket> tickets, String vehicleType) {
    final currentTicketType = ticketTypes[selectedTicketIndex];
    final filtered = tickets
        .where((t) => t.name.contains(currentTicketType))
        .toList();

    if (filtered.isEmpty) {
      return const Center(child: Text("Không có vé phù hợp"));
    }

    return Column(
      children: filtered.map((ticket) {
        final price = ticket.prices.isNotEmpty ? ticket.prices.first.price : 0;
        return Card(
          color: AppColors.card,
          margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          elevation: 3,
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  ticket.name,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  "Phương tiện: $vehicleType\nGiá: $price VNĐ",
                  style: const TextStyle(
                    fontSize: 14,
                    color: AppColors.textSecondary,
                  ),
                ),
                const SizedBox(height: 12),
                Align(
                  alignment: Alignment.centerRight,
                  child: ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.accent,
                      foregroundColor: AppColors.textPrimary,
                    ),
                    onPressed: () async {
                      try {
                        final token = await secureStorage.read(
                          key: 'access_token',
                        );
                        if (token == null) throw Exception("Không có token");

                        await PurchaseTicketUsecase().execute(
                          token,
                          ticket.prices.first.id,
                        );

                        ToastUtil.showSuccess("🎉 Mua vé thành công!");
                      } catch (e) {
                        String errorMessage = "Mua vé thất bại";

                        try {
                          // Tìm đoạn JSON trong chuỗi lỗi
                          final errorString = e.toString();
                          final startIndex = errorString.indexOf('{');
                          final endIndex = errorString.lastIndexOf('}') + 1;

                          if (startIndex != -1 && endIndex != -1) {
                            final jsonPart = errorString.substring(
                              startIndex,
                              endIndex,
                            );
                            final parsed = jsonDecode(jsonPart);
                            if (parsed is Map && parsed['message'] != null) {
                              errorMessage = parsed['message'];
                            }
                          }
                        } catch (_) {
                          // fallback nếu không parse được
                          errorMessage = e.toString();
                        }

                        ToastUtil.showError("❌ $errorMessage");
                      }
                    },
                    icon: const Icon(Icons.shopping_cart),
                    label: const Text("Mua vé"),
                  ),
                ),
              ],
            ),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildFutureTickets(Future<List<Ticket>> future, String vehicleType) {
    return FutureBuilder<List<Ticket>>(
      future: future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        } else if (snapshot.hasError) {
          return Center(child: Text("Lỗi: ${snapshot.error}"));
        } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
          return const Center(child: Text("Không có dữ liệu vé"));
        }
        return SingleChildScrollView(
          child: _buildTicketList(snapshot.data!, vehicleType),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text("Mua vé"),
        backgroundColor: AppColors.primary,
        bottom: TabBar(
          controller: _tabController,
          indicatorColor: AppColors.card,
          labelColor: Colors.white,
          unselectedLabelColor: AppColors.textPrimary,
          tabs: const [
            Tab(text: "Xe đạp"),
            Tab(text: "Xe điện"),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          Column(
            children: [
              const SizedBox(height: 16),
              _buildTicketSelector(),
              Expanded(
                child: _buildFutureTickets(_bikeTicketsFuture, "Xe đạp"),
              ),
            ],
          ),
          Column(
            children: [
              const SizedBox(height: 16),
              _buildTicketSelector(),
              Expanded(
                child: _buildFutureTickets(_ebikeTicketsFuture, "Xe điện"),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
