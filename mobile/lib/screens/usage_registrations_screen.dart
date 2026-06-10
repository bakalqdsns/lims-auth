import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../utils/theme/app_theme.dart';

class UsageRegistrationsScreen extends StatefulWidget {
  const UsageRegistrationsScreen({super.key});

  @override
  State<UsageRegistrationsScreen> createState() =>
      _UsageRegistrationsScreenState();
}

class _UsageRegistrationsScreenState extends State<UsageRegistrationsScreen> {
  final _service = UsageRegistrationService();
  final _semesterController = Get.find<SemesterController>();

  bool _loading = false;
  bool _submitting = false;
  String? _selectedSemesterId;
  String? _selectedStatus;

  List<UsageRegistration> _listData = [];

  UsageRegistration? _currentRow;
  int _formActualStudentCount = 0;
  double _formActualHours = 0;
  String _formAttendanceRecord = '无';
  String _formTeachingCondition = '正常';
  String _formEquipmentCondition = '正常';

  final List<String> _attendanceOptions = [
    '无',
    '有迟到',
    '有旷课',
    '迟到+旷课',
  ];

  final List<String> _teachingConditionOptions = [
    '正常',
    '设备故障',
    '其他异常',
  ];

  final List<String> _equipmentConditionOptions = [
    '正常',
    '部分损坏',
    '严重损坏',
  ];

  @override
  void initState() {
    super.initState();
    _initData();
  }

  Future<void> _initData() async {
    final semester = _semesterController.currentSemester.value;
    if (semester != null) {
      _selectedSemesterId = semester.id;
      await _fetchData();
    }
  }

  Future<void> _fetchData() async {
    if (_selectedSemesterId == null) return;
    setState(() => _loading = true);
    try {
      _listData = await _service.getRegistrations(status: _selectedStatus);
    } catch (e) {
      Get.snackbar('错误', '获取使用登记数据失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      setState(() => _loading = false);
    }
  }

  String _formatDayOfWeek(int? val) {
    const days = ['周一', '周二', '周三', '周四', '周五', '周六', '周日'];
    if (val == null) return '-';
    return days[val - 1] ?? '$val';
  }

  String _formatStatus(String? val) {
    const map = {'Pending': '待登记', 'Registered': '已登记', 'Overdue': '已逾期'};
    return map[val] ?? val ?? '-';
  }

  Color _statusColor(String? status) {
    switch (status) {
      case 'Registered':
        return AppTheme.success;
      case 'Overdue':
        return AppTheme.error;
      case 'Pending':
        return AppTheme.warning;
      default:
        return AppTheme.info;
    }
  }

  String _formatDate(String? date) {
    if (date == null || date.isEmpty) return '-';
    if (date.length >= 10) {
      return date.substring(0, 10);
    }
    return date;
  }

  void _handleReset() {
    setState(() {
      _selectedStatus = null;
    });
    _fetchData();
  }

  void _showFillDialog(UsageRegistration item) {
    _currentRow = item;
    setState(() {
      _formActualStudentCount =
          item.actualStudentCount > 0 ? item.actualStudentCount : item.expectedStudentCount;
      _formActualHours =
          item.actualHours > 0 ? item.actualHours : item.plannedHours.toDouble();
      _formAttendanceRecord = item.attendanceRecord ?? '无';
      _formTeachingCondition = item.teachingCondition ?? '正常';
      _formEquipmentCondition = item.equipmentCondition ?? '正常';
    });
    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('填写使用登记'),
          content: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    const Icon(Icons.meeting_room, size: 18,
                        color: AppTheme.textSecondary),
                    const SizedBox(width: 6),
                    Expanded(
                      child: Text(
                        item.labName ?? '-',
                        style: const TextStyle(fontWeight: FontWeight.w500),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Row(
                  children: [
                    const Icon(Icons.book, size: 18,
                        color: AppTheme.textSecondary),
                    const SizedBox(width: 6),
                    Expanded(
                      child: Text(
                        item.courseName ?? item.projectName ?? '-',
                        style: const TextStyle(fontWeight: FontWeight.w500),
                      ),
                    ),
                  ],
                ),
                const Divider(height: 24),
                Row(
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text('应到人数',
                              style: TextStyle(fontWeight: FontWeight.w500)),
                          const SizedBox(height: 4),
                          Text('${item.expectedStudentCount} 人',
                              style: const TextStyle(
                                  fontSize: 16, color: AppTheme.primary)),
                        ],
                      ),
                    ),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text('计划学时',
                              style: TextStyle(fontWeight: FontWeight.w500)),
                          const SizedBox(height: 4),
                          Text('${item.plannedHours} 学时',
                              style: const TextStyle(
                                  fontSize: 16, color: AppTheme.primary)),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 16),
                Row(
                  children: [
                    Expanded(
                      child: TextField(
                        decoration: const InputDecoration(
                          labelText: '实到人数',
                        ),
                        keyboardType: TextInputType.number,
                        controller: TextEditingController(
                          text: '$_formActualStudentCount',
                        ),
                        onChanged: (v) => setDialogState(
                            () => _formActualStudentCount = int.tryParse(v) ?? 0),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: TextField(
                        decoration: const InputDecoration(
                          labelText: '实际学时',
                        ),
                        keyboardType: const TextInputType.numberWithOptions(
                            decimal: true),
                        controller: TextEditingController(
                          text: _formActualHours.toString(),
                        ),
                        onChanged: (v) => setDialogState(() =>
                            _formActualHours =
                                double.tryParse(v) ?? 0),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  value: _formAttendanceRecord,
                  decoration: const InputDecoration(
                    labelText: '考勤情况',
                  ),
                  items: _attendanceOptions
                      .map((o) => DropdownMenuItem(value: o, child: Text(o)))
                      .toList(),
                  onChanged: (v) =>
                      setDialogState(() => _formAttendanceRecord = v ?? '无'),
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  value: _formTeachingCondition,
                  decoration: const InputDecoration(
                    labelText: '教学情况',
                  ),
                  items: _teachingConditionOptions
                      .map((o) => DropdownMenuItem(value: o, child: Text(o)))
                      .toList(),
                  onChanged: (v) =>
                      setDialogState(() => _formTeachingCondition = v ?? '正常'),
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  value: _formEquipmentCondition,
                  decoration: const InputDecoration(
                    labelText: '设备状况',
                  ),
                  items: _equipmentConditionOptions
                      .map((o) => DropdownMenuItem(value: o, child: Text(o)))
                      .toList(),
                  onChanged: (v) =>
                      setDialogState(() => _formEquipmentCondition = v ?? '正常'),
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('取消'),
            ),
            ElevatedButton(
              onPressed: _submitting ? null : () => _handleSubmit(ctx),
              child: _submitting
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('提交登记'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _handleSubmit(BuildContext dialogContext) async {
    if (_currentRow == null) return;
    setState(() => _submitting = true);
    try {
      final req = UsageRegistration(
        id: '',
        semesterId: _currentRow!.semesterId,
        labId: _currentRow!.labId,
        labName: _currentRow!.labName,
        useDate: _currentRow!.useDate,
        weekNumber: _currentRow!.weekNumber,
        dayOfWeek: _currentRow!.dayOfWeek,
        periodNumber: _currentRow!.periodNumber,
        source: _currentRow!.source,
        scheduleEntryId: _currentRow!.scheduleEntryId,
        reservationId: _currentRow!.reservationId,
        teachingApplicationId: _currentRow!.teachingApplicationId,
        courseName: _currentRow!.courseName,
        projectName: _currentRow!.projectName,
        className: _currentRow!.className,
        expectedStudentCount: _currentRow!.expectedStudentCount,
        actualStudentCount: _formActualStudentCount,
        plannedHours: _currentRow!.plannedHours,
        actualHours: _formActualHours,
        attendanceRecord: _formAttendanceRecord,
        teachingCondition: _formTeachingCondition,
        equipmentCondition: _formEquipmentCondition,
      );
      await _service.create(req);
      Get.snackbar('成功', '登记成功',
          backgroundColor: AppTheme.success, colorText: Colors.white);
      Navigator.pop(dialogContext);
      _fetchData();
    } catch (e) {
      Get.snackbar('错误', '提交失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      setState(() => _submitting = false);
    }
  }

  void _showDetailDialog(UsageRegistration item) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('使用登记详情'),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              _detailRow('实验室', item.labName ?? '-'),
              _detailRow('地点',
                  '${item.lab?.building?.name ?? ''} ${item.lab?.roomNumber ?? ''}'.trim()),
              _detailRow(
                  '使用时间',
                  '${_formatDate(item.useDate)} 第${item.weekNumber}周 ${_formatDayOfWeek(item.dayOfWeek)} 第${item.periodNumber}节'),
              _detailRow(
                  '课程/项目', item.courseName ?? item.projectName ?? '-'),
              _detailRow('班级', item.className ?? '-'),
              _detailRow('来源', item.source ?? '-'),
              _detailRow('应到人数',
                  item.expectedStudentCount > 0 ? '${item.expectedStudentCount}' : '-'),
              _detailRow('实到人数',
                  item.actualStudentCount > 0 ? '${item.actualStudentCount}' : '-'),
              _detailRow('计划学时', '${item.plannedHours}'),
              _detailRow(
                  '实际学时', item.actualHours > 0 ? '${item.actualHours}' : '-'),
              _detailRow('考勤情况', item.attendanceRecord ?? '-'),
              _detailRow('教学情况', item.teachingCondition ?? '-'),
              _detailRow('设备状况', item.equipmentCondition ?? '-'),
              _detailRow('登记人', item.filledByName ?? '-'),
              _detailRow('登记时间',
                  item.filledAt != null && item.filledAt!.isNotEmpty
                      ? _formatDateTime(item.filledAt!)
                      : '-'),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('关闭'),
          ),
        ],
      ),
    );
  }

  String _formatDateTime(String dateTime) {
    if (dateTime.isEmpty) return '-';
    try {
      final dt = DateTime.parse(dateTime);
      return '${dt.year}-${dt.month.toString().padLeft(2, '0')}-${dt.day.toString().padLeft(2, '0')} '
          '${dt.hour.toString().padLeft(2, '0')}:${dt.minute.toString().padLeft(2, '0')}';
    } catch (_) {
      return dateTime;
    }
  }

  Widget _detailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 80,
            child: Text('$label:',
                style: const TextStyle(fontWeight: FontWeight.w500)),
          ),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('使用登记'),
      ),
      body: Column(
        children: [
          Container(
            color: Colors.white,
            padding: const EdgeInsets.all(12),
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: [
                  Obx(() => DropdownButton<String>(
                        hint: const Text('选择学期'),
                        value: _semesterController.currentSemester.value?.id,
                        items: _semesterController.semesters
                            .map((s) => DropdownMenuItem(
                                  value: s.id,
                                  child: Text(s.name),
                                ))
                            .toList(),
                        onChanged: (v) {
                          setState(() => _selectedSemesterId = v);
                          _fetchData();
                        },
                      )),
                  const SizedBox(width: 8),
                  DropdownButton<String>(
                    hint: const Text('状态'),
                    value: _selectedStatus,
                    items: const [
                      DropdownMenuItem(value: null, child: Text('全部')),
                      DropdownMenuItem(value: 'Pending', child: Text('待登记')),
                      DropdownMenuItem(value: 'Registered', child: Text('已登记')),
                      DropdownMenuItem(value: 'Overdue', child: Text('已逾期')),
                    ],
                    onChanged: (v) {
                      setState(() => _selectedStatus = v);
                      _fetchData();
                    },
                  ),
                  const SizedBox(width: 8),
                  ElevatedButton.icon(
                    onPressed: _fetchData,
                    icon: const Icon(Icons.search, size: 18),
                    label: const Text('搜索'),
                  ),
                  const SizedBox(width: 4),
                  OutlinedButton(
                    onPressed: _handleReset,
                    child: const Text('重置'),
                  ),
                ],
              ),
            ),
          ),
          Expanded(
            child: _loading
                ? const Center(child: CircularProgressIndicator())
                : _selectedSemesterId == null
                    ? const Center(
                        child: Text('请选择学期',
                            style:
                                TextStyle(color: AppTheme.textSecondary)),
                      )
                    : _listData.isEmpty
                        ? const Center(
                            child: Text('暂无数据',
                                style:
                                    TextStyle(color: AppTheme.textSecondary)),
                          )
                        : ListView.builder(
                            padding: const EdgeInsets.all(12),
                            itemCount: _listData.length,
                            itemBuilder: (ctx, idx) {
                              final item = _listData[idx];
                              return Card(
                                margin: const EdgeInsets.only(bottom: 8),
                                child: InkWell(
                                  onTap: () => _showDetailDialog(item),
                                  borderRadius: BorderRadius.circular(12),
                                  child: Padding(
                                    padding: const EdgeInsets.all(12),
                                    child: Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      children: [
                                        Row(
                                          children: [
                                            Expanded(
                                              child: Text(
                                                item.labName ?? '-',
                                                style: const TextStyle(
                                                  fontWeight: FontWeight.w600,
                                                  fontSize: 15,
                                                ),
                                              ),
                                            ),
                                            Container(
                                              padding:
                                                  const EdgeInsets.symmetric(
                                                      horizontal: 8,
                                                      vertical: 2),
                                              decoration: BoxDecoration(
                                                color: _statusColor(
                                                        item.status)
                                                    .withOpacity(0.1),
                                                borderRadius:
                                                    BorderRadius.circular(4),
                                              ),
                                              child: Text(
                                                _formatStatus(item.status),
                                                style: TextStyle(
                                                  fontSize: 12,
                                                  color: _statusColor(
                                                      item.status),
                                                ),
                                              ),
                                            ),
                                          ],
                                        ),
                                        const SizedBox(height: 6),
                                        Wrap(
                                          spacing: 12,
                                          runSpacing: 4,
                                          children: [
                                            _infoChip(
                                              Icons.schedule,
                                              '${_formatDate(item.useDate)} 第${item.weekNumber}周 ${_formatDayOfWeek(item.dayOfWeek)} 第${item.periodNumber}节',
                                            ),
                                            _infoChip(Icons.book,
                                                item.courseName ?? item.projectName ?? '-'),
                                            _infoChip(Icons.class_,
                                                item.className ?? '-'),
                                            _infoChip(Icons.group,
                                                '${item.expectedStudentCount}人'),
                                            if (item.actualStudentCount > 0)
                                              _infoChip(Icons.person,
                                                  '实到${item.actualStudentCount}人'),
                                            _infoChip(Icons.timer,
                                                '${item.plannedHours}h'),
                                            if (item.actualHours > 0)
                                              _infoChip(Icons.access_time,
                                                  '实际${item.actualHours}h'),
                                          ],
                                        ),
                                        const SizedBox(height: 8),
                                        Row(
                                          mainAxisAlignment:
                                              MainAxisAlignment.end,
                                          children: [
                                            if (item.status == 'Pending' ||
                                                item.status == 'Overdue')
                                              TextButton(
                                                onPressed: () =>
                                                    _showFillDialog(item),
                                                child: const Text('登记'),
                                              ),
                                            TextButton(
                                              onPressed: () =>
                                                  _showDetailDialog(item),
                                              child: const Text('详情'),
                                            ),
                                          ],
                                        ),
                                      ],
                                    ),
                                  ),
                                ),
                              );
                            },
                          ),
          ),
        ],
      ),
    );
  }

  Widget _infoChip(IconData icon, String label) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 14, color: AppTheme.textSecondary),
        const SizedBox(width: 4),
        Text(label,
            style:
                const TextStyle(fontSize: 12, color: AppTheme.textSecondary)),
      ],
    );
  }
}
