import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../utils/theme/app_theme.dart';
import '../widgets/widgets.dart';

class DashboardScreen extends GetView<StatisticsController> {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context) {
    controller.loadDashboard();

    return Scaffold(
      appBar: AppBar(
        title: const Text('仪表盘'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: controller.loadDashboard,
          ),
        ],
      ),
      body: Obx(() {
        if (controller.isLoading.value) {
          return const LoadingShimmer(count: 6);
        }

        final d = controller.dashboard.value;
        return RefreshIndicator(
          onRefresh: controller.loadDashboard,
          child: SingleChildScrollView(
            physics: const AlwaysScrollableScrollPhysics(),
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // 统计卡片网格
                GridView.count(
                  crossAxisCount: 2,
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  mainAxisSpacing: 12,
                  crossAxisSpacing: 12,
                  childAspectRatio: 1.3,
                  children: [
                    StatCard(
                      title: '实验室',
                      value: '${d?.totalLabs ?? 0}',
                      icon: Icons.science,
                      color: AppTheme.primary,
                    ),
                    StatCard(
                      title: '设备',
                      value: '${d?.totalEquipments ?? 0}',
                      icon: Icons.devices,
                      color: AppTheme.success,
                    ),
                    StatCard(
                      title: '活跃排课',
                      value: '${d?.activeSchedules ?? 0}',
                      icon: Icons.calendar_today,
                      color: AppTheme.info,
                    ),
                    StatCard(
                      title: '待审批',
                      value: '${d?.pendingApprovals ?? 0}',
                      icon: Icons.pending_actions,
                      color: AppTheme.warning,
                    ),
                  ],
                ),
                const SizedBox(height: 24),

                // 快捷操作
                _buildSectionTitle('快捷操作'),
                const SizedBox(height: 12),
                _buildQuickActions(context),
                const SizedBox(height: 24),

                // 周预约趋势
                _buildSectionTitle('本周数据'),
                const SizedBox(height: 12),
                _buildTodayCard(d),
              ],
            ),
          ),
        );
      }),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 18,
        fontWeight: FontWeight.bold,
        color: AppTheme.textPrimary,
      ),
    );
  }

  Widget _buildQuickActions(BuildContext context) {
    final auth = Get.find<AuthController>();

    final actions = <Map<String, dynamic>>[];
    if (auth.isAdmin) {
      actions.addAll([
        {'icon': Icons.people, 'label': '用户管理', 'color': AppTheme.primary},
        {'icon': Icons.school, 'label': '学期管理', 'color': AppTheme.info},
        {'icon': Icons.science, 'label': '实验室', 'color': AppTheme.success},
        {'icon': Icons.devices, 'label': '设备管理', 'color': AppTheme.warning},
      ]);
    }
    if (auth.isTeacher || auth.isAdmin) {
      actions.addAll([
        {'icon': Icons.assignment, 'label': '授课任务', 'color': Colors.orange},
        {'icon': Icons.book_online, 'label': '排课管理', 'color': Colors.teal},
        {'icon': Icons.event_note, 'label': '使用登记', 'color': Colors.indigo},
      ]);
    }
    actions.addAll([
      {'icon': Icons.calendar_month, 'label': '预约管理', 'color': Colors.purple},
      {'icon': Icons.bar_chart, 'label': '统计报表', 'color': Colors.cyan},
    ]);

    return Wrap(
      spacing: 12,
      runSpacing: 12,
      children: actions.map((a) {
        return _QuickActionChip(
          icon: a['icon'] as IconData,
          label: a['label'] as String,
          color: a['color'] as Color,
        );
      }).toList(),
    );
  }

  Widget _buildTodayCard(dynamic d) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                _InfoItem(
                  label: '今日预约',
                  value: '${d?.todayReservations ?? 0}',
                  icon: Icons.event_available,
                ),
                _InfoItem(
                  label: '逾期记录',
                  value: '${d?.overdueRecords ?? 0}',
                  icon: Icons.warning_amber,
                  isWarning: (d?.overdueRecords ?? 0) > 0,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _QuickActionChip extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;

  const _QuickActionChip({
    required this.icon,
    required this.label,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () {},
      borderRadius: BorderRadius.circular(12),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        decoration: BoxDecoration(
          color: color.withOpacity(0.1),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, color: color, size: 18),
            const SizedBox(width: 6),
            Text(
              label,
              style: TextStyle(
                color: color,
                fontWeight: FontWeight.w500,
                fontSize: 13,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _InfoItem extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final bool isWarning;

  const _InfoItem({
    required this.label,
    required this.value,
    required this.icon,
    this.isWarning = false,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Icon(icon, color: isWarning ? AppTheme.warning : AppTheme.primary, size: 28),
        const SizedBox(height: 6),
        Text(
          value,
          style: TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: isWarning ? AppTheme.warning : AppTheme.textPrimary,
          ),
        ),
        Text(
          label,
          style: const TextStyle(
            fontSize: 12,
            color: AppTheme.textSecondary,
          ),
        ),
      ],
    );
  }
}
