import 'package:flutter/material.dart';
import 'widgets/notification_list.dart';
import 'widgets/notification_news.dart';

class NotificationPage extends StatelessWidget {
  const NotificationPage({super.key});

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text("Thông báo & Tin tức"),
          centerTitle: true,
          backgroundColor: Colors.green,
          bottom: const TabBar(
            indicatorColor: Colors.white,
            labelColor: Colors.white,
            unselectedLabelColor: Colors.white70,
            tabs: [
              Tab(text: "Thông báo"),
              Tab(text: "Tin tức"),
            ],
          ),
        ),
        body: const TabBarView(
          children: [NotificationList(), NotificationNews()],
        ),
      ),
    );
  }
}
