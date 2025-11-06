import 'package:flutter/material.dart';

class MoreMenu extends StatelessWidget {
  const MoreMenu({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      children: const [
        ListTile(leading: Icon(Icons.settings), title: Text("Cài đặt")),
        ListTile(leading: Icon(Icons.info), title: Text("Thông tin ứng dụng")),
      ],
    );
  }
}
