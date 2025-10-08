import 'package:flutter/material.dart';

class RentalInfo extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 3,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '📋 Giá thuê xe đạp HolaGo',
              style: TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
                color: Colors.teal[700],
              ),
            ),
            SizedBox(height: 12),
            _buildBullet('Dưới 3 tiếng: 50,000 VNĐ/xe/ngày'),
            _buildBullet('Trên 3 tiếng: 80,000 VNĐ/xe/ngày'),
            _buildBullet('Phí qua đêm: 20,000 VNĐ/xe/đêm'),
            _buildBullet('Thời gian thuê: Tối thiểu 1 ngày, tối đa 7 ngày'),
          ],
        ),
      ),
    );
  }

  Widget _buildBullet(String text) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('• ', style: TextStyle(fontSize: 16)),
          Expanded(child: Text(text, style: TextStyle(fontSize: 16))),
        ],
      ),
    );
  }
}
