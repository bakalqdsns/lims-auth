import 'dart:io';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../utils/theme/app_theme.dart';

class StatisticsScreen extends StatefulWidget {
  const StatisticsScreen({super.key});

  @override
  State<StatisticsScreen> createState() => _StatisticsScreenState();
}

class _StatisticsScreenState extends State<StatisticsScreen> {
  final _statCtrl = Get.find<StatisticsController>();
  final _semCtrl = Get.find<SemesterController>();

  // UI 状态
  final _activeReport = 'lab-usage'.obs;
  final _selectedSemesterId = Rxn<String>();
  final _startWeek = 1.obs;
  final _endWeek = 20.obs;

  @override
  void initState() {
    super.initState();
    // 初始化默认学期
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final current = _semCtrl.semesters
          .cast<Semester?>()
          .firstWhere((s) => s?.isCurrent == true, orElse: () => null);
      if (current != null) {
        _selectedSemesterId.value = current.id;
      }
      _loadCurrentReport();
    });
  }

  void _setActiveReport(String report) {
    _activeReport.value = report;
    _loadCurrentReport();
  }

  Future<void> _loadCurrentReport() async {
    final semId = _selectedSemesterId.value;
    _statCtrl.isLoading.value = true;
    _statCtrl.error.value = null;

    try {
      switch (_activeReport.value) {
        case 'lab-usage':
          await _statCtrl.loadLabUsage(semesterId: semId);
          break;
        case 'by-major':
          await _statCtrl.loadByMajor(semesterId: semId);
          break;
        case 'by-class':
          await _statCtrl.loadByClass(semesterId: semId);
          break;
        case 'reservation':
          await _statCtrl.loadReservationStats();
          break;
        case 'completion':
          await _statCtrl.loadCompletionRate();
          break;
      }
    } catch (e) {
      _statCtrl.error.value = e.toString();
    } finally {
      _statCtrl.isLoading.value = false;
    }
  }

  Future<void> _handleExport() async {
    try {
      final file = await _statCtrl.exportExcel();
      if (file != null) {
        Get.snackbar(
          '导出成功',
          '文件已保存',
          snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.success.withOpacity(0.9),
          colorText: Colors.white,
          duration: const Duration(seconds: 3),
          margin: const EdgeInsets.all(16),
        );
      } else {
        Get.snackbar(
          '导出失败',
          '服务器未返回文件',
          snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error.withOpacity(0.9),
          colorText: Colors.white,
          margin: const EdgeInsets.all(16),
        );
      }
    } catch (e) {
      Get.snackbar(
        '导出异常',
        e.toString(),
        snackPosition: SnackPosition.BOTTOM,
        backgroundColor: AppTheme.error.withOpacity(0.9),
        colorText: Colors.white,
        margin: const EdgeInsets.all(16),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('统计分析'),
        actions: [
          IconButton(
            icon: const Icon(Icons.file_download_outlined),
            tooltip: '导出',
            onPressed: _handleExport,
          ),
        ],
      ),
      body: Column(
        children: [
          _buildSearchBar(),
          _buildReportTabs(),
          Expanded(child: _buildContent()),
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Container(
      color: Colors.white,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      child: Row(
        children: [
          Expanded(
            flex: 2,
            child: Obx(() {
              final semesters = _semCtrl.semesters;
              return DropdownButtonFormField<String>(
                value: _selectedSemesterId.value,
                decoration: _inputDecoration('学期'),
                hint: const Text('请选择学期'),
                items: semesters.map((s) {
                  return DropdownMenuItem(
                    value: s.id,
                    child: Text(s.name, overflow: TextOverflow.ellipsis),
                  );
                }).toList(),
                onChanged: (v) {
                  _selectedSemesterId.value = v;
                  if (v != null) _loadCurrentReport();
                },
              );
            }),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Obx(() => DropdownButtonFormField<int>(
                  value: _startWeek.value,
                  decoration: _inputDecoration('起始周'),
                  items: List.generate(20, (i) => i + 1).map((w) {
                    return DropdownMenuItem(value: w, child: Text('第$w周'));
                  }).toList(),
                  onChanged: (v) {
                    if (v != null) _startWeek.value = v;
                  },
                )),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Obx(() => DropdownButtonFormField<int>(
                  value: _endWeek.value,
                  decoration: _inputDecoration('结束周'),
                  items: List.generate(20, (i) => i + 1).map((w) {
                    return DropdownMenuItem(value: w, child: Text('第$w周'));
                  }).toList(),
                  onChanged: (v) {
                    if (v != null) _endWeek.value = v;
                  },
                )),
          ),
          const SizedBox(width: 10),
          FilledButton.icon(
            onPressed: _loadCurrentReport,
            icon: const Icon(Icons.search, size: 18),
            label: const Text('查询'),
            style: FilledButton.styleFrom(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            ),
          ),
        ],
      ),
    );
  }

  InputDecoration _inputDecoration(String label) {
    return InputDecoration(
      labelText: label,
      contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
    );
  }

  Widget _buildReportTabs() {
    const tabs = [
      ('lab-usage', '实验室使用人次'),
      ('by-major', '分专业统计'),
      ('by-class', '分班级统计'),
      ('reservation', '预约统计'),
      ('completion', '登记完成率'),
    ];

    return Container(
      color: Colors.white,
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Obx(() => Wrap(
              spacing: 8,
              children: tabs.map((t) {
                final selected = t.$1 == _activeReport.value;
                return ChoiceChip(
                  label: Text(t.$2),
                  selected: selected,
                  selectedColor: AppTheme.primary.withOpacity(0.15),
                  labelStyle: TextStyle(
                    color:
                        selected ? AppTheme.primary : AppTheme.textSecondary,
                    fontWeight:
                        selected ? FontWeight.w600 : FontWeight.normal,
                    fontSize: 13,
                  ),
                  side: BorderSide(
                    color: selected
                        ? AppTheme.primary.withOpacity(0.4)
                        : AppTheme.textHint.withOpacity(0.3),
                  ),
                  onSelected: (_) => _setActiveReport(t.$1),
                );
              }).toList(),
            )),
      ),
    );
  }

  Widget _buildContent() {
    return Obx(() {
      if (_statCtrl.isLoading.value) {
        return const Center(child: CircularProgressIndicator());
      }

      if (_statCtrl.error.value != null) {
        return Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.error_outline,
                  size: 48, color: AppTheme.error.withOpacity(0.6)),
              const SizedBox(height: 12),
              Text(
                _statCtrl.error.value!,
                style: const TextStyle(color: AppTheme.textSecondary),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 12),
              ElevatedButton(
                onPressed: _loadCurrentReport,
                child: const Text('重试'),
              ),
            ],
          ),
        );
      }

      return _buildReportContent(_activeReport.value);
    });
  }

  Widget _buildReportContent(String report) {
    switch (report) {
      case 'lab-usage':
        return _LabUsageReport(data: _statCtrl.labUsage);
      case 'by-major':
        return _CategoryReport(
          title: '分专业使用统计',
          data: _statCtrl.byMajor,
        );
      case 'by-class':
        return _CategoryReport(
          title: '分班级使用统计',
          data: _statCtrl.byClass,
        );
      case 'reservation':
        return _ReservationReport(data: _statCtrl.reservationStats.value);
      case 'completion':
        return _CompletionReport(data: _statCtrl.completionRate.value);
      default:
        return const Center(child: Text('未知报表类型'));
    }
  }
}

// ==================== 实验室使用人次 ====================

class _LabUsageReport extends StatelessWidget {
  final List<LabUsage> data;

  const _LabUsageReport({required this.data});

  @override
  Widget build(BuildContext context) {
    if (data.isEmpty) {
      return const Center(child: Text('暂无数据'));
    }

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: data.length,
      separatorBuilder: (_, __) => const SizedBox(height: 10),
      itemBuilder: (context, index) {
        final item = data[index];
        final pct = item.utilizationRate.clamp(0.0, 100.0);
        return Card(
          margin: EdgeInsets.zero,
          child: Padding(
            padding: const EdgeInsets.all(14),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: AppTheme.primary.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: const Icon(Icons.science,
                          color: AppTheme.primary, size: 20),
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: Text(
                        item.labName ?? '未知实验室',
                        style: const TextStyle(
                          fontWeight: FontWeight.w600,
                          fontSize: 15,
                          color: AppTheme.textPrimary,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                Row(
                  children: [
                    _StatPill(
                      label: '已用节次数',
                      value: '${item.completedBookings}',
                      color: AppTheme.info,
                    ),
                    const SizedBox(width: 8),
                    _StatPill(
                      label: '总节次数',
                      value: '${item.totalBookings}',
                      color: AppTheme.textSecondary,
                    ),
                    const Spacer(),
                    Text(
                      '${pct.toStringAsFixed(1)}%',
                      style: TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: _progressColor(pct),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 10),
                ClipRRect(
                  borderRadius: BorderRadius.circular(4),
                  child: LinearProgressIndicator(
                    value: pct / 100,
                    minHeight: 6,
                    backgroundColor: AppTheme.textHint.withOpacity(0.2),
                    valueColor: AlwaysStoppedAnimation(_progressColor(pct)),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  Color _progressColor(double pct) {
    if (pct >= 80) return AppTheme.success;
    if (pct >= 50) return AppTheme.warning;
    return AppTheme.error;
  }
}

// ==================== 分专业 / 分班级 报表 ====================

class _CategoryReport extends StatelessWidget {
  final String title;
  final List<StatisticsByCategory> data;

  const _CategoryReport({required this.title, required this.data});

  @override
  Widget build(BuildContext context) {
    if (data.isEmpty) {
      return const Center(child: Text('暂无数据'));
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
          child: Text(
            title,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: AppTheme.textPrimary,
            ),
          ),
        ),
        Expanded(
          child: ListView.separated(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
            itemCount: data.length,
            separatorBuilder: (_, __) => const SizedBox(height: 8),
            itemBuilder: (context, index) {
              final item = data[index];
              final pct = item.percentage.clamp(0.0, 100.0);
              return Card(
                margin: EdgeInsets.zero,
                child: Padding(
                  padding: const EdgeInsets.all(14),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Container(
                            width: 28,
                            height: 28,
                            decoration: BoxDecoration(
                              color: AppTheme.primary.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(6),
                            ),
                            alignment: Alignment.center,
                            child: Text(
                              '${index + 1}',
                              style: const TextStyle(
                                color: AppTheme.primary,
                                fontWeight: FontWeight.bold,
                                fontSize: 13,
                              ),
                            ),
                          ),
                          const SizedBox(width: 10),
                          Expanded(
                            child: Text(
                              item.category,
                              style: const TextStyle(
                                fontWeight: FontWeight.w600,
                                fontSize: 14,
                                color: AppTheme.textPrimary,
                              ),
                            ),
                          ),
                          Text(
                            '${item.count}次',
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 14,
                              color: AppTheme.textPrimary,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 10),
                      ClipRRect(
                        borderRadius: BorderRadius.circular(4),
                        child: LinearProgressIndicator(
                          value: pct / 100,
                          minHeight: 6,
                          backgroundColor: AppTheme.textHint.withOpacity(0.2),
                          valueColor:
                              const AlwaysStoppedAnimation(AppTheme.primary),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Align(
                        alignment: Alignment.centerRight,
                        child: Text(
                          '${pct.toStringAsFixed(1)}%',
                          style: const TextStyle(
                            fontSize: 12,
                            color: AppTheme.textSecondary,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

// ==================== 预约统计 ====================

class _ReservationReport extends StatelessWidget {
  final ReservationStats? data;

  const _ReservationReport({required this.data});

  @override
  Widget build(BuildContext context) {
    if (data == null) {
      return const Center(child: Text('暂无数据'));
    }

    final items = <_ResStatItem>[
      _ResStatItem('待审核', data!.pending, AppTheme.warning),
      _ResStatItem('已通过', data!.approved, AppTheme.success),
      _ResStatItem('已完成', data!.completed, AppTheme.info),
      _ResStatItem('已驳回', data!.rejected, AppTheme.error),
      _ResStatItem('已取消', data!.cancelled, AppTheme.textSecondary),
    ];

    return Column(
      children: [
        // 总计卡片
        Container(
          margin: const EdgeInsets.all(16),
          padding: const EdgeInsets.all(20),
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [AppTheme.primary, AppTheme.primaryDark],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(16),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.event_note, color: Colors.white, size: 32),
              const SizedBox(width: 16),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    '预约总数',
                    style: TextStyle(color: Colors.white70, fontSize: 13),
                  ),
                  Text(
                    '${data!.total}',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 32,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
        // 状态列表
        Expanded(
          child: ListView.separated(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
            itemCount: items.length,
            separatorBuilder: (_, __) => const SizedBox(height: 8),
            itemBuilder: (context, index) {
              final item = items[index];
              final pct = data!.total > 0
                  ? (item.count / data!.total * 100).clamp(0.0, 100.0)
                  : 0.0;
              return Card(
                margin: EdgeInsets.zero,
                child: Padding(
                  padding: const EdgeInsets.all(14),
                  child: Row(
                    children: [
                      Container(
                        width: 10,
                        height: 10,
                        decoration: BoxDecoration(
                          color: item.color,
                          shape: BoxShape.circle,
                        ),
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          item.label,
                          style: const TextStyle(
                            fontSize: 14,
                            color: AppTheme.textPrimary,
                          ),
                        ),
                      ),
                      Text(
                        '${item.count}',
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: item.color,
                        ),
                      ),
                      const SizedBox(width: 12),
                      SizedBox(
                        width: 80,
                        child: ClipRRect(
                          borderRadius: BorderRadius.circular(4),
                          child: LinearProgressIndicator(
                            value: pct / 100,
                            minHeight: 6,
                            backgroundColor:
                                AppTheme.textHint.withOpacity(0.2),
                            valueColor: AlwaysStoppedAnimation(item.color),
                          ),
                        ),
                      ),
                      const SizedBox(width: 8),
                      SizedBox(
                        width: 48,
                        child: Text(
                          '${pct.toStringAsFixed(1)}%',
                          style: const TextStyle(
                            fontSize: 12,
                            color: AppTheme.textSecondary,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

class _ResStatItem {
  final String label;
  final int count;
  final Color color;

  _ResStatItem(this.label, this.count, this.color);
}

// ==================== 登记完成率 ====================

class _CompletionReport extends StatelessWidget {
  final CompletionRate? data;

  const _CompletionReport({required this.data});

  @override
  Widget build(BuildContext context) {
    if (data == null) {
      return const Center(child: Text('暂无数据'));
    }

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // 统计数字卡片
          Row(
            children: [
              Expanded(
                child: _CountCard(
                  title: '总记录数',
                  value: data!.total,
                  icon: Icons.assignment,
                  color: AppTheme.primary,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _CountCard(
                  title: '已完成',
                  value: data!.completed,
                  icon: Icons.check_circle,
                  color: AppTheme.success,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          Row(
            children: [
              Expanded(
                child: _CountCard(
                  title: '待登记',
                  value: data!.pending,
                  icon: Icons.pending,
                  color: AppTheme.warning,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _CountCard(
                  title: '已逾期',
                  value: data!.overdue,
                  icon: Icons.warning_amber,
                  color: AppTheme.error,
                  isOverdue: true,
                ),
              ),
            ],
          ),
          const SizedBox(height: 28),
          // 圆形进度
          _CircleProgress(rate: data!.rate),
        ],
      ),
    );
  }
}

class _CountCard extends StatelessWidget {
  final String title;
  final int value;
  final IconData icon;
  final Color color;
  final bool isOverdue;

  const _CountCard({
    required this.title,
    required this.value,
    required this.icon,
    required this.color,
    this.isOverdue = false,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(icon, color: color, size: 22),
                const SizedBox(width: 6),
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 13,
                    color: AppTheme.textSecondary,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Text(
                  '$value',
                  style: TextStyle(
                    fontSize: 26,
                    fontWeight: FontWeight.bold,
                    color: color,
                  ),
                ),
                if (isOverdue && value > 0) ...[
                  const SizedBox(width: 6),
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                    decoration: BoxDecoration(
                      color: AppTheme.error.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: const Text(
                      '逾期',
                      style: TextStyle(
                        fontSize: 11,
                        color: AppTheme.error,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _CircleProgress extends StatelessWidget {
  final double rate;

  const _CircleProgress({required this.rate});

  @override
  Widget build(BuildContext context) {
    final pct = rate.clamp(0.0, 100.0);
    final color = _progressColor(pct);

    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          children: [
            const Text(
              '使用登记完成率',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w600,
                color: AppTheme.textPrimary,
              ),
            ),
            const SizedBox(height: 24),
            SizedBox(
              width: 160,
              height: 160,
              child: Stack(
                alignment: Alignment.center,
                children: [
                  SizedBox(
                    width: 160,
                    height: 160,
                    child: CircularProgressIndicator(
                      value: pct / 100,
                      strokeWidth: 12,
                      backgroundColor: AppTheme.textHint.withOpacity(0.15),
                      valueColor: AlwaysStoppedAnimation(color),
                    ),
                  ),
                  Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        '${pct.toStringAsFixed(0)}%',
                        style: TextStyle(
                          fontSize: 36,
                          fontWeight: FontWeight.bold,
                          color: color,
                        ),
                      ),
                      const Text(
                        '完成率',
                        style: TextStyle(
                          fontSize: 13,
                          color: AppTheme.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Color _progressColor(double pct) {
    if (pct >= 80) return AppTheme.success;
    if (pct >= 50) return AppTheme.warning;
    return AppTheme.error;
  }
}

// ==================== 通用组件 ====================

class _StatPill extends StatelessWidget {
  final String label;
  final String value;
  final Color color;

  const _StatPill({
    required this.label,
    required this.value,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Column(
        children: [
          Text(
            value,
            style: TextStyle(
              fontWeight: FontWeight.bold,
              fontSize: 14,
              color: color,
            ),
          ),
          Text(
            label,
            style: TextStyle(
              fontSize: 10,
              color: color.withOpacity(0.8),
            ),
          ),
        ],
      ),
    );
  }
}

// ==================== Controller 私有扩展 ====================
// 仅本文件可见，用于暴露 StatisticsService.exportExcel()

extension _StatisticsControllerExport on StatisticsController {
  Future<File?> exportExcel() {
    return StatisticsService().exportExcel();
  }
}
