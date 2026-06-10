import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../controllers/app_controller.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../utils/theme/app_theme.dart';

class SchedulesScreen extends StatefulWidget {
  const SchedulesScreen({super.key});

  @override
  State<SchedulesScreen> createState() => _SchedulesScreenState();
}

class _SchedulesScreenState extends State<SchedulesScreen>
    with SingleTickerProviderStateMixin {
  final _scheduleService = ScheduleService();
  final _semesterController = Get.find<SemesterController>();
  final _appController = Get.find<AppController>();

  late TabController _tabController;

  bool _loading = false;
  bool _exporting = false;
  String? _selectedSemesterId;
  int _startWeek = 1;
  int _endWeek = 20;
  String? _selectedBuildingId;
  String? _selectedLabId;
  String? _selectedSource;
  int _currentWeek = 1;

  List<ScheduleEntry> _listData = [];
  List<ScheduleEntry> _timetableData = [];

  final List<Map<String, dynamic>> _weekDays = [
    {'label': '周一', 'value': 1},
    {'label': '周二', 'value': 2},
    {'label': '周三', 'value': 3},
    {'label': '周四', 'value': 4},
    {'label': '周五', 'value': 5},
    {'label': '周六', 'value': 6},
    {'label': '周日', 'value': 7},
  ];

  final List<Map<String, dynamic>> _periodTimes = [
    {'period': 1, 'time': '08:00-08:45'},
    {'period': 2, 'time': '08:55-09:40'},
    {'period': 3, 'time': '10:00-10:45'},
    {'period': 4, 'time': '10:55-11:40'},
    {'period': 5, 'time': '14:00-14:45'},
    {'period': 6, 'time': '14:55-15:40'},
    {'period': 7, 'time': '16:00-16:45'},
    {'period': 8, 'time': '16:55-17:40'},
    {'period': 9, 'time': '19:00-19:45'},
    {'period': 10, 'time': '19:55-20:40'},
    {'period': 11, 'time': '20:50-21:35'},
    {'period': 12, 'time': '21:45-22:30'},
  ];

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _initData();
  }

  Future<void> _initData() async {
    final semester = _semesterController.currentSemester.value;
    if (semester != null) {
      _selectedSemesterId = semester.id;
      await _appController.loadBuildings();
      await _appController.loadLabs();
      await _fetchData();
      await _loadTimetable();
    }
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _fetchData() async {
    if (_selectedSemesterId == null) return;
    setState(() => _loading = true);
    try {
      _listData = await _scheduleService.getSchedules(
        semesterId: _selectedSemesterId,
        labId: _selectedLabId,
        weekNumber: _startWeek,
      );
    } catch (e) {
      Get.snackbar('错误', '获取排课数据失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      setState(() => _loading = false);
    }
  }

  Future<void> _loadTimetable() async {
    if (_selectedSemesterId == null) return;
    try {
      _timetableData = await _scheduleService.getSchedules(
        semesterId: _selectedSemesterId,
        labId: _selectedLabId,
        weekNumber: _currentWeek,
      );
      setState(() {});
    } catch (e) {
      Get.snackbar('错误', '加载课程表失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    }
  }

  String _formatDayOfWeek(int? val) {
    const days = ['周日', '周一', '周二', '周三', '周四', '周五', '周六'];
    if (val == null) return '-';
    return days[val - 1] ?? '$val';
  }

  String _formatSource(String? val) {
    const map = {
      'CentralScheduling': '集中排课',
      'Reservation': '预约',
      'TeachingRequest': '授课申请',
    };
    return map[val] ?? val ?? '-';
  }

  void _handleReset() {
    setState(() {
      _startWeek = 1;
      _endWeek = 20;
      _selectedBuildingId = null;
      _selectedLabId = null;
      _selectedSource = null;
    });
    _fetchData();
    _loadTimetable();
  }

  Future<void> _handleExport() async {
    if (_listData.isEmpty) {
      Get.snackbar('提示', '没有可导出的数据',
          backgroundColor: AppTheme.warning, colorText: Colors.white);
      return;
    }
    setState(() => _exporting = true);
    await Future.delayed(const Duration(milliseconds: 500));
    Get.snackbar('成功', '导出功能开发中',
        backgroundColor: AppTheme.success, colorText: Colors.white);
    setState(() => _exporting = false);
  }

  void _showDetailDialog(ScheduleEntry item) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('排课详情'),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              _detailRow('学期', item.semesterName ?? '-'),
              _detailRow('周次',
                  '第${item.startWeek > 0 ? item.startWeek : item.weekNumber}-${item.endWeek > 0 ? item.endWeek : item.weekNumber}周'),
              _detailRow('星期', _formatDayOfWeek(item.dayOfWeek)),
              _detailRow('节次', '第${item.periodNumber}节'),
              _detailRow(
                  '课程/项目', item.courseName ?? item.projectName ?? '-'),
              _detailRow('来源', _formatSource(item.source)),
              _detailRow('教师', item.teacherName ?? '-'),
              _detailRow('班级', item.className ?? '-'),
              _detailRow('实验室', item.labName ?? '-'),
              _detailRow('人数', item.studentCount > 0 ? '${item.studentCount}' : '-'),
              _detailRow(
                  '地点', '${item.buildingName ?? ''} ${item.lab?.roomNumber ?? ''}'.trim()),
              _detailRow('冲突', item.hasConflict ? '有冲突' : '无'),
              if (item.remark != null && item.remark!.isNotEmpty)
                _detailRow('备注', item.remark!),
              if (item.conflictInfo != null && item.conflictInfo!.isNotEmpty)
                _detailRow('冲突信息', item.conflictInfo!, isAlert: true),
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

  Widget _detailRow(String label, String value, {bool isAlert = false}) {
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
          Expanded(
            child: Text(
              value,
              style: TextStyle(
                color: isAlert ? AppTheme.error : null,
              ),
            ),
          ),
        ],
      ),
    );
  }

  List<ScheduleEntry> _getTimetableItems(int dayOfWeek, int periodNumber) {
    return _timetableData.where((item) {
      final inDay = item.dayOfWeek == dayOfWeek;
      final inPeriod = item.periodNumber == periodNumber;
      final inWeek = _currentWeek >= (item.startWeek > 0 ? item.startWeek : item.weekNumber) &&
          _currentWeek <= (item.endWeek > 0 ? item.endWeek : item.weekNumber);
      return inDay && inPeriod && inWeek;
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    final labs = _appController.labs;
    final buildings = _appController.buildings;

    return Scaffold(
      appBar: AppBar(
        title: const Text('排课查询'),
        actions: [
          IconButton(
            icon: _exporting
                ? const SizedBox(
                    width: 20, height: 20,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: Colors.white,
                    ),
                  )
                : const Icon(Icons.download),
            onPressed: _exporting ? null : _handleExport,
            tooltip: '导出',
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          indicatorColor: Colors.white,
          tabs: const [
            Tab(text: '排课列表'),
            Tab(text: '课程表'),
          ],
        ),
      ),
      body: Column(
        children: [
          Container(
            color: Colors.white,
            padding: const EdgeInsets.all(12),
            child: Column(
              children: [
                SingleChildScrollView(
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
                              _loadTimetable();
                            },
                          )),
                      const SizedBox(width: 8),
                      const Text('周次:'),
                      const SizedBox(width: 4),
                      SizedBox(
                        width: 70,
                        child: DropdownButtonFormField<int>(
                          value: _startWeek,
                          decoration: const InputDecoration(
                            contentPadding:
                                EdgeInsets.symmetric(horizontal: 8, vertical: 8),
                            isDense: true,
                          ),
                          items: List.generate(
                            20,
                            (i) => DropdownMenuItem(
                              value: i + 1,
                              child: Text('${i + 1}周'),
                            ),
                          ),
                          onChanged: (v) =>
                              setState(() => _startWeek = v ?? 1),
                        ),
                      ),
                      const Text(' - '),
                      SizedBox(
                        width: 70,
                        child: DropdownButtonFormField<int>(
                          value: _endWeek,
                          decoration: const InputDecoration(
                            contentPadding:
                                EdgeInsets.symmetric(horizontal: 8, vertical: 8),
                            isDense: true,
                          ),
                          items: List.generate(
                            20,
                            (i) => DropdownMenuItem(
                              value: i + 1,
                              child: Text('${i + 1}周'),
                            ),
                          ),
                          onChanged: (v) =>
                              setState(() => _endWeek = v ?? 20),
                        ),
                      ),
                      const SizedBox(width: 8),
                      Obx(() => DropdownButton<String>(
                            hint: const Text('楼宇'),
                            value: _selectedBuildingId,
                            items: [
                              const DropdownMenuItem(value: null, child: Text('全部')),
                              ...buildings.map((b) => DropdownMenuItem(
                                    value: b.id,
                                    child: Text(b.name),
                                  )),
                            ],
                            onChanged: (v) {
                              setState(() {
                                _selectedBuildingId = v;
                                _selectedLabId = null;
                              });
                            },
                          )),
                      const SizedBox(width: 8),
                      Obx(() {
                        final filteredLabs = _selectedBuildingId == null
                            ? labs
                            : labs.where((l) => l.buildingId == _selectedBuildingId).toList();
                        return DropdownButton<String>(
                          hint: const Text('实验室'),
                          value: _selectedLabId,
                          items: [
                            const DropdownMenuItem(value: null, child: Text('全部')),
                            ...filteredLabs.map((l) => DropdownMenuItem(
                                  value: l.id,
                                  child: Text(l.name),
                                )),
                          ],
                          onChanged: (v) => setState(() => _selectedLabId = v),
                        );
                      }),
                      const SizedBox(width: 8),
                      DropdownButton<String>(
                        hint: const Text('来源'),
                        value: _selectedSource,
                        items: const [
                          DropdownMenuItem(value: null, child: Text('全部')),
                          DropdownMenuItem(
                              value: 'CentralScheduling', child: Text('集中排课')),
                          DropdownMenuItem(value: 'Reservation', child: Text('预约')),
                          DropdownMenuItem(
                              value: 'TeachingRequest', child: Text('授课申请')),
                        ],
                        onChanged: (v) => setState(() => _selectedSource = v),
                      ),
                      const SizedBox(width: 8),
                      ElevatedButton.icon(
                        onPressed: () {
                          _fetchData();
                          _loadTimetable();
                        },
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
              ],
            ),
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                _buildListView(),
                _buildTimetableView(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildListView() {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_selectedSemesterId == null) {
      return const Center(
        child: Text('请选择学期', style: TextStyle(color: AppTheme.textSecondary)),
      );
    }
    if (_listData.isEmpty) {
      return const Center(
        child: Text('暂无数据', style: TextStyle(color: AppTheme.textSecondary)),
      );
    }
    return ListView.builder(
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
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          item.courseName ?? item.projectName ?? '-',
                          style: const TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 15,
                          ),
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 8, vertical: 2),
                        decoration: BoxDecoration(
                          color: item.hasConflict
                              ? AppTheme.error.withOpacity(0.1)
                              : AppTheme.success.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Text(
                          item.hasConflict ? '有冲突' : '无冲突',
                          style: TextStyle(
                            fontSize: 12,
                            color: item.hasConflict
                                ? AppTheme.error
                                : AppTheme.success,
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
                      _infoChip(Icons.person, item.teacherName ?? '-'),
                      _infoChip(Icons.class_, item.className ?? '-'),
                      _infoChip(
                          Icons.meeting_room, item.labName ?? '-'),
                      _infoChip(
                        Icons.calendar_today,
                        '第${item.startWeek > 0 ? item.startWeek : item.weekNumber}-${item.endWeek > 0 ? item.endWeek : item.weekNumber}周',
                      ),
                      _infoChip(
                          Icons.schedule, _formatDayOfWeek(item.dayOfWeek)),
                      _infoChip(
                          Icons.access_time, '第${item.periodNumber}节'),
                      _infoChip(Icons.source, _formatSource(item.source)),
                    ],
                  ),
                  const SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      TextButton(
                        onPressed: () => _showDetailDialog(item),
                        child: const Text('查看'),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _infoChip(IconData icon, String label) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 14, color: AppTheme.textSecondary),
        const SizedBox(width: 4),
        Text(label,
            style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary)),
      ],
    );
  }

  Widget _buildTimetableView() {
    return Column(
      children: [
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          color: Colors.white,
          child: Row(
            children: [
              const Text('选择周次: '),
              SizedBox(
                width: 100,
                child: DropdownButtonFormField<int>(
                  value: _currentWeek,
                  decoration: const InputDecoration(
                    contentPadding: EdgeInsets.symmetric(horizontal: 8),
                    isDense: true,
                  ),
                  items: List.generate(
                    20,
                    (i) => DropdownMenuItem(
                      value: i + 1,
                      child: Text('第${i + 1}周'),
                    ),
                  ),
                  onChanged: (v) {
                    setState(() => _currentWeek = v ?? 1);
                    _loadTimetable();
                  },
                ),
              ),
            ],
          ),
        ),
        Expanded(
          child: SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: SingleChildScrollView(
              child: DataTable(
                headingRowColor: WidgetStateProperty.all(AppTheme.background),
                columns: [
                  const DataColumn(label: Text('节次/时间')),
                  ..._weekDays.map((d) => DataColumn(label: Text(d['label']))),
                ],
                rows: _periodTimes.map((period) {
                  return DataRow(
                    cells: [
                      DataCell(
                        Column(
                          mainAxisSize: MainAxisSize.min,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text('第${period['period']}节',
                                style: const TextStyle(
                                    fontWeight: FontWeight.w600, fontSize: 13)),
                            Text(period['time'],
                                style: const TextStyle(
                                    fontSize: 11, color: AppTheme.textSecondary)),
                          ],
                        ),
                      ),
                      ..._weekDays.map((d) {
                        final items =
                            _getTimetableItems(d['value'], period['period']);
                        return DataCell(
                          items.isEmpty
                              ? const SizedBox()
                              : Column(
                                  children: items.map((item) {
                                    return GestureDetector(
                                      onTap: () => _showDetailDialog(item),
                                      child: Container(
                                        width: double.infinity,
                                        margin:
                                            const EdgeInsets.symmetric(vertical: 2),
                                        padding: const EdgeInsets.all(4),
                                        decoration: BoxDecoration(
                                          color: item.hasConflict
                                              ? AppTheme.error.withOpacity(0.1)
                                              : const Color(0xFFECF5FF),
                                          borderRadius: BorderRadius.circular(4),
                                          border: Border(
                                            left: BorderSide(
                                              color: item.hasConflict
                                                  ? AppTheme.error
                                                  : AppTheme.primary,
                                              width: 3,
                                            ),
                                          ),
                                        ),
                                        child: Column(
                                          crossAxisAlignment:
                                              CrossAxisAlignment.start,
                                          children: [
                                            Text(
                                              item.courseName ??
                                                  item.projectName ??
                                                  '-',
                                              style: const TextStyle(
                                                fontSize: 12,
                                                fontWeight: FontWeight.w500,
                                              ),
                                              maxLines: 1,
                                              overflow: TextOverflow.ellipsis,
                                            ),
                                            if (item.teacherName != null)
                                              Text(
                                                item.teacherName!,
                                                style: const TextStyle(
                                                  fontSize: 10,
                                                  color: AppTheme.textSecondary,
                                                ),
                                                maxLines: 1,
                                                overflow: TextOverflow.ellipsis,
                                              ),
                                            if (item.labName != null)
                                              Text(
                                                item.labName!,
                                                style: const TextStyle(
                                                  fontSize: 10,
                                                  color: AppTheme.textSecondary,
                                                ),
                                                maxLines: 1,
                                                overflow: TextOverflow.ellipsis,
                                              ),
                                          ],
                                        ),
                                      ),
                                    );
                                  }).toList(),
                                ),
                        );
                      }),
                    ],
                  );
                }).toList(),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
