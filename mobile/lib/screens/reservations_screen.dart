import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../utils/theme/app_theme.dart';

class ReservationsScreen extends StatefulWidget {
  const ReservationsScreen({super.key});

  @override
  State<ReservationsScreen> createState() => _ReservationsScreenState();
}

class _ReservationsScreenState extends State<ReservationsScreen> {
  final _service = ReservationService();
  final _semesterController = Get.find<SemesterController>();
  final _appController = Get.find<AppController>();

  bool _loading = false;
  bool _submitting = false;
  String? _selectedSemesterId;
  String? _selectedLabId;
  String? _selectedStatus;

  List<Reservation> _listData = [];

  final List<Map<String, dynamic>> _weekDays = [
    {'label': '周一', 'value': 1},
    {'label': '周二', 'value': 2},
    {'label': '周三', 'value': 3},
    {'label': '周四', 'value': 4},
    {'label': '周五', 'value': 5},
    {'label': '周六', 'value': 6},
    {'label': '周日', 'value': 7},
  ];

  final List<int> _periodOptions = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

  final Map<String, String> _projectCategoryLabels = {
    'CourseTeaching': '课程教学',
    'TeacherResearch': '教师科研',
    'StudentResearch': '学生科研',
    'InnovationEntrepreneurship': '创新创业',
    'GraduationThesis': '毕业论文',
    'StudentActivity': '学生课外活动',
    'InstitutionActivity': '机构活动',
    'Other': '其他',
  };

  String _formLabId = '';
  int _formWeekNumber = 1;
  int _formDayOfWeek = 1;
  List<int> _formPeriodNumbers = [];
  String _formProjectName = '';
  String _formProjectCategory = '';
  String _formApplicantPhone = '';
  int _formMemberCount = 0;
  String _formMemberGrade = '';
  String _formRemark = '';

  @override
  void initState() {
    super.initState();
    _initData();
  }

  Future<void> _initData() async {
    final semester = _semesterController.currentSemester.value;
    if (semester != null) {
      _selectedSemesterId = semester.id;
      await _appController.loadLabs();
      await _fetchData();
    }
  }

  Future<void> _fetchData() async {
    if (_selectedSemesterId == null) return;
    setState(() => _loading = true);
    try {
      _listData = await _service.getReservations(status: _selectedStatus);
    } catch (e) {
      Get.snackbar('错误', '获取预约数据失败',
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
    const map = {'Pending': '待审批', 'Approved': '已通过', 'Rejected': '已驳回', 'Cancelled': '已取消'};
    return map[val] ?? val ?? '-';
  }

  Color _statusColor(String? status) {
    switch (status) {
      case 'Approved':
        return AppTheme.success;
      case 'Rejected':
      case 'Cancelled':
        return AppTheme.error;
      case 'Pending':
        return AppTheme.warning;
      default:
        return AppTheme.info;
    }
  }

  String _formatProjectCategory(String? val) {
    return _projectCategoryLabels[val] ?? val ?? '-';
  }

  void _handleReset() {
    setState(() {
      _selectedLabId = null;
      _selectedStatus = null;
    });
    _fetchData();
  }

  void _showSubmitDialog() {
    setState(() {
      _formLabId = '';
      _formWeekNumber = 1;
      _formDayOfWeek = 1;
      _formPeriodNumbers = [];
      _formProjectName = '';
      _formProjectCategory = '';
      _formApplicantPhone = '';
      _formMemberCount = 0;
      _formMemberGrade = '';
      _formRemark = '';
    });
    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('提交预约申请'),
          content: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text('实验室', style: TextStyle(fontWeight: FontWeight.w500)),
                const SizedBox(height: 4),
                Obx(() => DropdownButtonFormField<String>(
                      value: _formLabId.isEmpty ? null : _formLabId,
                      decoration: const InputDecoration(
                        hintText: '请选择实验室',
                        isDense: true,
                      ),
                      items: _appController.labs
                          .map((l) => DropdownMenuItem(
                                value: l.id,
                                child: Text(l.name),
                              ))
                          .toList(),
                      onChanged: (v) =>
                          setDialogState(() => _formLabId = v ?? ''),
                    )),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text('教学周', style: TextStyle(fontWeight: FontWeight.w500)),
                          const SizedBox(height: 4),
                          DropdownButtonFormField<int>(
                            value: _formWeekNumber,
                            decoration: const InputDecoration(
                              isDense: true,
                            ),
                            items: List.generate(
                              20,
                              (i) => DropdownMenuItem(
                                value: i + 1,
                                child: Text('第${i + 1}周'),
                              ),
                            ),
                            onChanged: (v) =>
                                setDialogState(() => _formWeekNumber = v ?? 1),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text('星期', style: TextStyle(fontWeight: FontWeight.w500)),
                          const SizedBox(height: 4),
                          DropdownButtonFormField<int>(
                            value: _formDayOfWeek,
                            decoration: const InputDecoration(
                              isDense: true,
                            ),
                            items: _weekDays
                                .map((d) => DropdownMenuItem<int>(
                                      value: d['value'] as int,
                                      child: Text(d['label'] as String),
                                    ))
                                .toList(),
                            onChanged: (v) =>
                                setDialogState(() => _formDayOfWeek = (v as int?) ?? 1),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                const Text('节次（可多选）', style: TextStyle(fontWeight: FontWeight.w500)),
                const SizedBox(height: 4),
                Wrap(
                  spacing: 6,
                  runSpacing: 4,
                  children: _periodOptions.map((p) {
                    final selected = _formPeriodNumbers.contains(p);
                    return FilterChip(
                      label: Text('第$p节'),
                      selected: selected,
                      onSelected: (v) {
                        setDialogState(() {
                          if (v) {
                            _formPeriodNumbers = [..._formPeriodNumbers, p];
                          } else {
                            _formPeriodNumbers =
                                _formPeriodNumbers.where((e) => e != p).toList();
                          }
                        });
                      },
                      selectedColor: AppTheme.primary.withOpacity(0.2),
                      checkmarkColor: AppTheme.primary,
                      labelStyle: TextStyle(
                        color: selected ? AppTheme.primary : AppTheme.textSecondary,
                        fontSize: 12,
                      ),
                    );
                  }).toList(),
                ),
                const SizedBox(height: 12),
                TextField(
                  decoration: const InputDecoration(
                    labelText: '项目名称',
                    hintText: '请输入项目名称',
                  ),
                  onChanged: (v) => setDialogState(() => _formProjectName = v),
                ),
                const SizedBox(height: 12),
                DropdownButtonFormField<String>(
                  value: _formProjectCategory.isEmpty ? null : _formProjectCategory,
                  decoration: const InputDecoration(
                    labelText: '项目类别',
                    hintText: '请选择类别',
                  ),
                  items: _projectCategoryLabels.entries
                      .map((e) => DropdownMenuItem(
                            value: e.key,
                            child: Text(e.value),
                          ))
                      .toList(),
                  onChanged: (v) =>
                      setDialogState(() => _formProjectCategory = v ?? ''),
                ),
                const SizedBox(height: 12),
                TextField(
                  decoration: const InputDecoration(
                    labelText: '联系电话',
                    hintText: '请输入联系电话',
                  ),
                  keyboardType: TextInputType.phone,
                  onChanged: (v) =>
                      setDialogState(() => _formApplicantPhone = v),
                ),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Expanded(
                      child: TextField(
                        decoration: const InputDecoration(
                          labelText: '参与人数',
                        ),
                        keyboardType: TextInputType.number,
                        onChanged: (v) => setDialogState(
                            () => _formMemberCount = int.tryParse(v) ?? 0),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: TextField(
                        decoration: const InputDecoration(
                          labelText: '年级',
                          hintText: '如：2024级',
                        ),
                        onChanged: (v) =>
                            setDialogState(() => _formMemberGrade = v),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                TextField(
                  decoration: const InputDecoration(
                    labelText: '备注',
                    hintText: '选填',
                  ),
                  maxLines: 2,
                  onChanged: (v) => setDialogState(() => _formRemark = v),
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
                  : const Text('提交'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _handleSubmit(BuildContext dialogContext) async {
    if (_formLabId.isEmpty) {
      Get.snackbar('提示', '请选择实验室',
          backgroundColor: AppTheme.warning, colorText: Colors.white);
      return;
    }
    if (_formPeriodNumbers.isEmpty) {
      Get.snackbar('提示', '请选择节次',
          backgroundColor: AppTheme.warning, colorText: Colors.white);
      return;
    }
    if (_formProjectName.isEmpty) {
      Get.snackbar('提示', '请输入项目名称',
          backgroundColor: AppTheme.warning, colorText: Colors.white);
      return;
    }
    if (_formProjectCategory.isEmpty) {
      Get.snackbar('提示', '请选择项目类别',
          backgroundColor: AppTheme.warning, colorText: Colors.white);
      return;
    }

    setState(() => _submitting = true);
    try {
      final req = CreateReservationRequest(
        semesterId: _selectedSemesterId ?? '',
        labId: _formLabId,
        weekNumber: _formWeekNumber,
        dayOfWeek: _formDayOfWeek,
        periodNumbers: _formPeriodNumbers,
        projectName: _formProjectName,
        projectCategory: _formProjectCategory,
        projectLeaderPhone: _formApplicantPhone,
        memberCount: _formMemberCount,
        memberGrade: _formMemberGrade,
        remark: _formRemark,
      );
      await _service.create(req);
      Get.snackbar('成功', '提交成功',
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

  void _showDetailDialog(Reservation item) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('预约详情'),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              _detailRow('项目名称', item.projectName ?? '-'),
              _detailRow('项目类别', _formatProjectCategory(item.projectCategory)),
              _detailRow('实验室', item.labName ?? '-'),
              _detailRow('预约时间',
                  '第${item.weekNumber}周 ${_formatDayOfWeek(item.dayOfWeek)} 第${(item.periodNumbers ?? []).join(',')}节'),
              _detailRow('申请人', item.applicantName ?? '-'),
              _detailRow('联系电话', item.applicantPhone ?? '-'),
              _detailRow('参与人数', item.memberCount > 0 ? '${item.memberCount}' : '-'),
              _detailRow('年级', item.memberGrade ?? '-'),
              _detailRow('状态', _formatStatus(item.status)),
              _detailRow('审批意见', item.approvalComment ?? '-'),
              _detailRow('备注', item.remark ?? '-'),
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
        title: const Text('预约申请'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: _showSubmitDialog,
            tooltip: '提交预约',
          ),
        ],
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
                  Obx(() => DropdownButton<String>(
                        hint: const Text('实验室'),
                        value: _selectedLabId,
                        items: [
                          const DropdownMenuItem(value: null, child: Text('全部')),
                          ..._appController.labs.map((l) => DropdownMenuItem(
                                value: l.id,
                                child: Text(l.name),
                              )),
                        ],
                        onChanged: (v) {
                          setState(() => _selectedLabId = v);
                          _fetchData();
                        },
                      )),
                  const SizedBox(width: 8),
                  DropdownButton<String>(
                    hint: const Text('状态'),
                    value: _selectedStatus,
                    items: const [
                      DropdownMenuItem(value: null, child: Text('全部')),
                      DropdownMenuItem(value: 'Pending', child: Text('待审批')),
                      DropdownMenuItem(value: 'Approved', child: Text('已通过')),
                      DropdownMenuItem(value: 'Rejected', child: Text('已驳回')),
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
                                                item.projectName ?? '-',
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
                                            _infoChip(Icons.category,
                                                _formatProjectCategory(
                                                    item.projectCategory)),
                                            _infoChip(Icons.meeting_room,
                                                item.labName ?? '-'),
                                            _infoChip(
                                              Icons.schedule,
                                              '第${item.weekNumber}周 ${_formatDayOfWeek(item.dayOfWeek)} 第${(item.periodNumbers ?? []).join(',')}节',
                                            ),
                                            _infoChip(Icons.person,
                                                item.applicantName ?? '-'),
                                            _infoChip(Icons.group,
                                                '${item.memberCount}人'),
                                          ],
                                        ),
                                        const SizedBox(height: 8),
                                        Row(
                                          mainAxisAlignment:
                                              MainAxisAlignment.end,
                                          children: [
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
            style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary)),
      ],
    );
  }
}
