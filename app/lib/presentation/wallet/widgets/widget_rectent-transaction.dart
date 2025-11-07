import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hola_bike_app/application/usecases/usecase_get-wallet-transaction.dart';
import 'package:hola_bike_app/theme/app_colors.dart';
import 'package:hola_bike_app/domain/models/info_wallet-transaction.dart';

class RecentTransactions extends StatefulWidget {
  const RecentTransactions({super.key});

  @override
  State<RecentTransactions> createState() => _RecentTransactionsState();
}

class _RecentTransactionsState extends State<RecentTransactions> {
  final WalletTransactionUseCase _useCase = WalletTransactionUseCase();
  final FlutterSecureStorage secureStorage = const FlutterSecureStorage();

  List<WalletTransactionInfo> _transactions = [];
  bool _isLoaded = false;
  bool _showAll = false; // trạng thái hiển thị tất cả hay chỉ 10 cái

  @override
  void initState() {
    super.initState();
    _fetchTransactions(limit: 10);
  }

  Future<void> _fetchTransactions({int limit = 10}) async {
    try {
      EasyLoading.show();
      final token = await secureStorage.read(key: 'access_token');
      if (token == null) throw Exception('Không tìm thấy token');

      final data = await _useCase.execute(token: token, pageSize: limit);

      setState(() {
        _transactions = data;
        _isLoaded = true;
      });
    } catch (e) {
      EasyLoading.showError('Lỗi khi tải giao dịch');
      debugPrint('Error fetching transactions: $e');
    } finally {
      EasyLoading.dismiss();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // --- Header ---
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              "Giao dịch gần đây",
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
            ),
            GestureDetector(
              onTap: () async {
                if (!_showAll) {
                  setState(() => _showAll = true);
                  await _fetchTransactions(limit: 200);
                } else {
                  setState(() => _showAll = false);
                  await _fetchTransactions(limit: 10);
                }
              },
              child: Text(
                _showAll ? "Thu gọn" : "Xem tất cả",
                style: const TextStyle(
                  fontSize: 13,
                  color: AppColors.primary,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),

        if (!_isLoaded)
          const Center(child: CircularProgressIndicator())
        else if (_transactions.isEmpty)
          _buildEmptyState()
        else
          Container(
            height: 300, // Giới hạn chiều cao để có thể scroll trong box
            padding: const EdgeInsets.only(right: 4),
            child: Scrollbar(
              thumbVisibility: true,
              child: SingleChildScrollView(
                child: Column(
                  children: _transactions
                      .map((tx) => _buildTransactionItem(tx))
                      .toList(),
                ),
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildTransactionItem(WalletTransactionInfo tx) {
    final isIncome = tx.direction.toLowerCase() == 'in';
    final color = isIncome ? Colors.green : Colors.redAccent;
    final icon = isIncome ? Icons.arrow_downward : Icons.arrow_upward;

    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 12),
      decoration: BoxDecoration(
        color: Colors.grey[100],
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          CircleAvatar(
            backgroundColor: color.withOpacity(0.15),
            child: Icon(icon, color: color, size: 20),
          ),
          const SizedBox(width: 12),

          // --- Thông tin giao dịch ---
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  tx.source, // sửa typo: tx.source
                  style: const TextStyle(
                    fontWeight: FontWeight.w600,
                    fontSize: 14,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  _formatDate(tx.createdAt),
                  style: const TextStyle(color: Colors.grey, fontSize: 12),
                ),
              ],
            ),
          ),

          // --- Số tiền ---
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                "${isIncome ? '+' : '-'}${tx.amount.toStringAsFixed(0)} đ",
                style: TextStyle(
                  color: color,
                  fontWeight: FontWeight.w600,
                  fontSize: 14,
                ),
              ),
              Text(
                "Còn lại: ${tx.balanceAfter.toStringAsFixed(0)} đ",
                style: const TextStyle(color: Colors.grey, fontSize: 11),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildEmptyState() {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 32),
      alignment: Alignment.center,
      child: Column(
        children: const [
          Icon(Icons.folder_open, size: 48, color: Colors.grey),
          SizedBox(height: 12),
          Text("Chưa có dữ liệu", style: TextStyle(color: Colors.grey)),
        ],
      ),
    );
  }

  String _formatDate(DateTime date) {
    return "${date.day}/${date.month}/${date.year} ${date.hour.toString().padLeft(2, '0')}:${date.minute.toString().padLeft(2, '0')}";
  }
}
