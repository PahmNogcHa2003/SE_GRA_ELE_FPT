import 'package:flutter/material.dart';
import '../../core/constants/locations.dart';
import '../../data/models/rental_request.dart';

class RentalForm extends StatefulWidget {
  final Future<void> Function(RentalRequest request) onSubmit;
  final void Function(int? startId, int? endId)? onStationsChanged;
  final Widget? extraWidget; // 👈 thêm widget tuỳ chọn

  const RentalForm({
    Key? key,
    required this.onSubmit,
    this.onStationsChanged,
    this.extraWidget,
  }) : super(key: key);

  @override
  _RentalFormState createState() => _RentalFormState();
}

class _RentalFormState extends State<RentalForm> {
  final _formKey = GlobalKey<FormState>();

  String fullName = '';
  String phone = '';
  String city = '';
  int quantity = 1;

  DateTime? startDate;
  DateTime? endDate;

  int? startStationId;
  int? endStationId;

  bool isSubmitting = false;

  @override
  Widget build(BuildContext context) {
    final stationItems = AppLocations.stations.map((s) {
      return DropdownMenuItem<int>(
        value: s.id,
        child: LayoutBuilder(
          builder: (context, constraints) {
            return ConstrainedBox(
              constraints: BoxConstraints(maxWidth: constraints.maxWidth - 48),
              child: Text(s.name, overflow: TextOverflow.ellipsis, maxLines: 1),
            );
          },
        ),
      );
    }).toList();

    return LayoutBuilder(
      builder: (context, constraints) {
        return Center(
          child: ConstrainedBox(
            constraints: BoxConstraints(maxWidth: constraints.maxWidth),
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Card(
                elevation: 4,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          '📝 Thông tin thuê xe',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 12),

                        _buildTextField(
                          label: 'Họ và tên',
                          onChanged: (v) => fullName = v,
                          validator: (v) =>
                              v!.isEmpty ? 'Vui lòng nhập họ tên' : null,
                        ),
                        _buildTextField(
                          label: 'Số điện thoại',
                          keyboardType: TextInputType.phone,
                          onChanged: (v) => phone = v,
                          validator: (v) =>
                              v!.isEmpty ? 'Vui lòng nhập số điện thoại' : null,
                        ),
                        _buildTextField(
                          label: 'Thành phố',
                          onChanged: (v) => city = v,
                        ),
                        _buildTextField(
                          label: 'Số xe thuê',
                          keyboardType: TextInputType.number,
                          onChanged: (v) => quantity = int.tryParse(v) ?? 1,
                        ),

                        const SizedBox(height: 12),

                        // Dropdown chọn điểm bắt đầu
                        DropdownButtonFormField<int>(
                          decoration: const InputDecoration(
                            labelText: 'Chọn điểm bắt đầu',
                            border: OutlineInputBorder(),
                          ),
                          items: stationItems,
                          value: startStationId,
                          onChanged: (v) {
                            setState(() => startStationId = v);
                            widget.onStationsChanged?.call(
                              startStationId,
                              endStationId,
                            );
                          },
                          validator: (v) =>
                              v == null ? 'Chọn điểm bắt đầu' : null,
                          isExpanded: true,
                        ),

                        const SizedBox(height: 12),

                        // Dropdown chọn điểm kết thúc
                        DropdownButtonFormField<int>(
                          decoration: const InputDecoration(
                            labelText: 'Chọn điểm kết thúc',
                            border: OutlineInputBorder(),
                          ),
                          items: stationItems,
                          value: endStationId,
                          onChanged: (v) {
                            setState(() => endStationId = v);
                            widget.onStationsChanged?.call(
                              startStationId,
                              endStationId,
                            );
                          },
                          validator: (v) =>
                              v == null ? 'Chọn điểm kết thúc' : null,
                          isExpanded: true,
                        ),

                        const SizedBox(height: 12),

                        // Chọn ngày thuê và ngày trả
                        Row(
                          children: [
                            Expanded(
                              child: _buildDateTimeButton(
                                label: 'Ngày thuê',
                                dateTime: startDate,
                                onPressed: () async {
                                  final pickedDate = await showDatePicker(
                                    context: context,
                                    initialDate: DateTime.now(),
                                    firstDate: DateTime.now(),
                                    lastDate: DateTime.now().add(
                                      const Duration(days: 365),
                                    ),
                                  );
                                  if (pickedDate != null) {
                                    final pickedTime = await showTimePicker(
                                      context: context,
                                      initialTime: TimeOfDay.now(),
                                    );
                                    if (pickedTime != null) {
                                      final combined = DateTime(
                                        pickedDate.year,
                                        pickedDate.month,
                                        pickedDate.day,
                                        pickedTime.hour,
                                        pickedTime.minute,
                                      );
                                      setState(() => startDate = combined);
                                    }
                                  }
                                },
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: _buildDateTimeButton(
                                label: 'Ngày trả',
                                dateTime: endDate,
                                onPressed: () async {
                                  final pickedDate = await showDatePicker(
                                    context: context,
                                    initialDate: startDate ?? DateTime.now(),
                                    firstDate: startDate ?? DateTime.now(),
                                    lastDate: DateTime.now().add(
                                      const Duration(days: 365),
                                    ),
                                  );
                                  if (pickedDate != null) {
                                    final pickedTime = await showTimePicker(
                                      context: context,
                                      initialTime: TimeOfDay.now(),
                                    );
                                    if (pickedTime != null) {
                                      final combined = DateTime(
                                        pickedDate.year,
                                        pickedDate.month,
                                        pickedDate.day,
                                        pickedTime.hour,
                                        pickedTime.minute,
                                      );
                                      setState(() => endDate = combined);
                                    }
                                  }
                                },
                              ),
                            ),
                          ],
                        ),

                        const SizedBox(height: 16),

                        // 👇 widget bổ sung sẽ chèn ngay trước nút đặt xe
                        if (widget.extraWidget != null) ...[
                          widget.extraWidget!,
                          const SizedBox(height: 16),
                        ],

                        Center(
                          child: ElevatedButton.icon(
                            icon: isSubmitting
                                ? const SizedBox(
                                    width: 18,
                                    height: 18,
                                    child: CircularProgressIndicator(
                                      strokeWidth: 2,
                                      color: Colors.white,
                                    ),
                                  )
                                : const Icon(
                                    Icons.check_circle_outline,
                                    size: 22,
                                  ),
                            label: Text(
                              isSubmitting ? 'Đang gửi...' : 'Đặt xe',
                              style: const TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                letterSpacing: 0.5,
                              ),
                            ),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: Colors.teal,
                              foregroundColor: Colors.white,
                              elevation: 4,
                              padding: const EdgeInsets.symmetric(
                                horizontal: 32,
                                vertical: 14,
                              ),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(30),
                              ),
                              shadowColor: Colors.tealAccent,
                            ),
                            onPressed: isSubmitting ? null : _handleSubmit,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildTextField({
    required String label,
    TextInputType keyboardType = TextInputType.text,
    required Function(String) onChanged,
    String? Function(String?)? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: TextFormField(
        decoration: InputDecoration(
          labelText: label,
          border: const OutlineInputBorder(),
        ),
        keyboardType: keyboardType,
        onChanged: onChanged,
        validator: validator,
      ),
    );
  }

  Widget _buildDateTimeButton({
    required String label,
    required DateTime? dateTime,
    required VoidCallback onPressed,
  }) {
    final formatted = dateTime != null
        ? '${dateTime.day.toString().padLeft(2, '0')}/'
              '${dateTime.month.toString().padLeft(2, '0')}/'
              '${dateTime.year} '
              '${dateTime.hour.toString().padLeft(2, '0')}:'
              '${dateTime.minute.toString().padLeft(2, '0')}'
        : 'Chọn $label';

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: ElevatedButton(
        onPressed: onPressed,
        style: ElevatedButton.styleFrom(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
          backgroundColor: Colors.teal,
          foregroundColor: Colors.white,
          textStyle: const TextStyle(fontSize: 16),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        ),
        child: Text('$label: $formatted'),
      ),
    );
  }

  Future<void> _handleSubmit() async {
    FocusScope.of(context).unfocus();

    if (!_formKey.currentState!.validate()) return;

    if (startDate == null || endDate == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn ngày thuê và ngày trả')),
      );
      return;
    }

    if (endDate!.isBefore(startDate!)) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Ngày trả phải sau ngày thuê')),
      );
      return;
    }

    if (startStationId == null || endStationId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn điểm đi và điểm đến')),
      );
      return;
    }

    if (startStationId == endStationId) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Điểm bắt đầu và kết thúc phải khác nhau'),
        ),
      );
      return;
    }

    setState(() => isSubmitting = true);

    try {
      final request = RentalRequest(
        fullName: fullName,
        phoneNumber: phone,
        city: city,
        quantity: quantity,
        startDate: startDate!,
        endDate: endDate!,
        stationStartId: startStationId!,
        stationEndId: endStationId!,
      );

      await widget.onSubmit(request);
    } catch (e) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Lỗi: $e')));
    } finally {
      if (mounted) setState(() => isSubmitting = false);
    }
  }
}
