import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../controllers/semester_controller.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class SemestersScreen extends StatefulWidget {
  const SemestersScreen({super.key});

  @override
  State<SemestersScreen> createState() => _SemestersScreenState();
}

class _SemestersScreenState extends State<SemestersScreen> {
  final _controller = Get.find<SemesterController>();
  final _authController = Get.find<AuthController>();

  final _searchController = TextEditingController();
  bool? _isCurrentFilter;
  int _currentPage = 1;
  int _pageSize = 10;

  @override
  void initState() {
    super.initState();
    _controller.loadSemesters();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  List<Semester> get _filteredSemesters {
    var list = _controller.semesters.toList();
    if (_searchController.text.isNotEmpty) {
      list = list.where((s) =>
        s.name.toLowerCase().contains(_searchController.text.toLowerCase()) ||
        s.code.toLowerCase().contains(_searchController.text.toLowerCase())
      ).toList();
    }
    if (_isCurrentFilter != null) {
      list = list.where((s) => s.isCurrent == _isCurrentFilter).toList();
    }
    return list;
  }

  List<Semester> get _paginatedSemesters {
    final start = (_currentPage - 1) * _pageSize;
    final end = start + _pageSize;
    final filtered = _filteredSemesters;
    if (start >= filtered.length) return [];
    return filtered.sublist(start, end > filtered.length ? filtered.length : end);
  }

  int get _totalPages => (_filteredSemesters.length / _pageSize).ceil();

  void _onSearch() {
    setState(() {
      _currentPage = 1;
    });
  }

  void _onReset() {
    _searchController.clear();
    setState(() {
      _isCurrentFilter = null;
      _currentPage = 1;
    });
  }

  void _showCalendarSheet(Semester semester) {
    _controller.loadCalendar(semesterId: semester.id);
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (context) => _CalendarBottomSheet(semester: semester),
    );
  }

  void _showFormDialog({Semester? semester}) {
    showDialog(
      context: context,
      builder: (context) => _SemesterFormDialog(
        semester: semester,
        onSaved: () {
          _controller.loadSemesters();
        },
      ),
    );
  }

  Future<void> _handleDelete(Semester semester) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认删除'),
        content: Text('确定要删除学期 "${semester.name}" 吗？'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('取消'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            style: TextButton.styleFrom(foregroundColor: AppTheme.error),
            child: const Text('删除'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      final success = await _controller.deleteSemester(semester.id);
      if (success) {
        Get.snackbar('成功', '删除成功', snackPosition: SnackPosition.BOTTOM);
      } else {
        Get.snackbar('错误', _controller.error.value ?? '删除失败',
            snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
      }
    }
  }

  Future<void> _handleSetCurrent(Semester semester) async {
    final success = await _controller.setCurrent(semester.id);
    if (success) {
      Get.snackbar('成功', '已设为当前学期', snackPosition: SnackPosition.BOTTOM);
    } else {
      Get.snackbar('错误', '设置失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('学期管理'),
        actions: [
          if (_authController.isAdmin)
            IconButton(
              icon: const Icon(Icons.add),
              onPressed: () => _showFormDialog(),
            ),
        ],
      ),
      body: Column(
        children: [
          _buildSearchBar(),
          Expanded(
            child: Obx(() {
              if (_controller.isLoading.value) {
                return const Center(child: CircularProgressIndicator());
              }
              final list = _paginatedSemesters;
              if (list.isEmpty) {
                return const Center(child: Text('暂无数据', style: TextStyle(color: AppTheme.textSecondary)));
              }
              return ListView.builder(
                padding: const EdgeInsets.all(16),
                itemCount: list.length,
                itemBuilder: (context, index) => _buildSemesterCard(list[index]),
              );
            }),
          ),
          _buildPagination(),
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Card(
      margin: const EdgeInsets.all(16),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(
              controller: _searchController,
              decoration: InputDecoration(
                hintText: '搜索学期名称',
                prefixIcon: const Icon(Icons.search),
                suffixIcon: _searchController.text.isNotEmpty
                    ? IconButton(
                        icon: const Icon(Icons.clear),
                        onPressed: _onReset,
                      )
                    : null,
              ),
              onSubmitted: (_) => _onSearch(),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                const Text('当前学期: ', style: TextStyle(color: AppTheme.textSecondary)),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('全部'),
                  selected: _isCurrentFilter == null,
                  onSelected: (selected) {
                    setState(() {
                      _isCurrentFilter = null;
                      _currentPage = 1;
                    });
                  },
                ),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('是'),
                  selected: _isCurrentFilter == true,
                  onSelected: (selected) {
                    setState(() {
                      _isCurrentFilter = selected ? true : null;
                      _currentPage = 1;
                    });
                  },
                ),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('否'),
                  selected: _isCurrentFilter == false,
                  onSelected: (selected) {
                    setState(() {
                      _isCurrentFilter = selected ? false : null;
                      _currentPage = 1;
                    });
                  },
                ),
                const Spacer(),
                ElevatedButton(
                  onPressed: _onSearch,
                  child: const Text('搜索'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSemesterCard(Semester semester) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    semester.name,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.textPrimary,
                    ),
                  ),
                ),
                if (semester.isCurrent)
                  Chip(
                    label: const Text('当前'),
                    backgroundColor: AppTheme.success.withOpacity(0.1),
                    labelStyle: const TextStyle(color: AppTheme.success, fontSize: 12),
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                  )
                else
                  Chip(
                    label: const Text('否'),
                    backgroundColor: AppTheme.textSecondary.withOpacity(0.1),
                    labelStyle: const TextStyle(color: AppTheme.textSecondary, fontSize: 12),
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                _InfoChip(label: '代码: ${semester.code}'),
                const SizedBox(width: 8),
                _InfoChip(label: '学年度: ${semester.academicYear}'),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Icon(Icons.calendar_today, size: 14, color: AppTheme.textSecondary),
                const SizedBox(width: 4),
                Text(
                  '${semester.startDate ?? '-'} ~ ${semester.endDate ?? '-'}',
                  style: const TextStyle(color: AppTheme.textSecondary, fontSize: 13),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Text('状态: ', style: TextStyle(color: AppTheme.textSecondary, fontSize: 13)),
                Switch(
                  value: semester.isActive,
                  onChanged: _authController.isAdmin
                      ? (value) async {
                          // TODO: toggle status
                        }
                      : null,
                  activeColor: AppTheme.success,
                ),
                const Spacer(),
                if (_authController.isAdmin) ...[
                  TextButton(
                    onPressed: () => _showCalendarSheet(semester),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.info,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('校历'),
                  ),
                  if (!semester.isCurrent)
                    TextButton(
                      onPressed: () => _handleSetCurrent(semester),
                      style: TextButton.styleFrom(
                        foregroundColor: AppTheme.success,
                        padding: const EdgeInsets.symmetric(horizontal: 8),
                      ),
                      child: const Text('设为当前'),
                    ),
                  TextButton(
                    onPressed: () => _showFormDialog(semester: semester),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.primary,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('编辑'),
                  ),
                  TextButton(
                    onPressed: () => _handleDelete(semester),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.error,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('删除'),
                  ),
                ],
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPagination() {
    if (_totalPages <= 1) return const SizedBox.shrink();
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.chevron_left),
            onPressed: _currentPage > 1
                ? () => setState(() => _currentPage--)
                : null,
          ),
          for (int i = 1; i <= _totalPages; i++)
            if (i == 1 || i == _totalPages || (i - _currentPage).abs() <= 2)
              Container(
                margin: const EdgeInsets.symmetric(horizontal: 2),
                child: TextButton(
                  onPressed: i != _currentPage ? () => setState(() => _currentPage = i) : null,
                  style: TextButton.styleFrom(
                    backgroundColor: i == _currentPage ? AppTheme.primary : null,
                    foregroundColor: i == _currentPage ? Colors.white : AppTheme.textPrimary,
                    minimumSize: const Size(36, 36),
                    padding: EdgeInsets.zero,
                  ),
                  child: Text('$i'),
                ),
              )
            else if (i == 2 || i == _totalPages - 1)
              const Text('...'),
          IconButton(
            icon: const Icon(Icons.chevron_right),
            onPressed: _currentPage < _totalPages
                ? () => setState(() => _currentPage++)
                : null,
          ),
        ],
      ),
    );
  }
}

class _InfoChip extends StatelessWidget {
  final String label;
  const _InfoChip({required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: AppTheme.background,
        borderRadius: BorderRadius.circular(4),
      ),
      child: Text(label, style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary)),
    );
  }
}

class _CalendarBottomSheet extends StatelessWidget {
  final Semester semester;
  const _CalendarBottomSheet({required this.semester});

  @override
  Widget build(BuildContext context) {
    final controller = Get.find<SemesterController>();
    return DraggableScrollableSheet(
      initialChildSize: 0.7,
      minChildSize: 0.5,
      maxChildSize: 0.95,
      expand: false,
      builder: (context, scrollController) {
        return Column(
          children: [
            Container(
              padding: const EdgeInsets.all(16),
              decoration: const BoxDecoration(
                border: Border(bottom: BorderSide(color: AppTheme.background)),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      '${semester.name} - 校历',
                      style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.pop(context),
                  ),
                ],
              ),
            ),
            Expanded(
              child: Obx(() {
                if (controller.calendar.isEmpty) {
                  return const Center(child: Text('暂无校历数据'));
                }
                return ListView.builder(
                  controller: scrollController,
                  padding: const EdgeInsets.all(16),
                  itemCount: controller.calendar.length,
                  itemBuilder: (context, index) {
                    final item = controller.calendar[index];
                    return Card(
                      margin: const EdgeInsets.only(bottom: 8),
                      child: ListTile(
                        leading: CircleAvatar(
                          backgroundColor: item.isHoliday
                              ? AppTheme.error.withOpacity(0.1)
                              : item.isTeachingDay
                                  ? AppTheme.success.withOpacity(0.1)
                                  : AppTheme.info.withOpacity(0.1),
                          child: Text(
                            '${item.weekNumber}',
                            style: TextStyle(
                              color: item.isHoliday
                                  ? AppTheme.error
                                  : item.isTeachingDay
                                      ? AppTheme.success
                                      : AppTheme.info,
                            ),
                          ),
                        ),
                        title: Text(item.eventName ?? '第${item.weekNumber}周'),
                        subtitle: Text('${item.date ?? "-"} | 周${_weekdayName(item.dayOfWeek)}'),
                        trailing: item.isHoliday
                            ? const Chip(
                                label: Text('假期'),
                                backgroundColor: AppTheme.error,
                                labelStyle: TextStyle(color: Colors.white, fontSize: 11),
                                padding: EdgeInsets.zero,
                                materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                              )
                            : item.isTeachingDay
                                ? const Chip(
                                    label: Text('教学'),
                                    backgroundColor: AppTheme.success,
                                    labelStyle: TextStyle(color: Colors.white, fontSize: 11),
                                    padding: EdgeInsets.zero,
                                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                                  )
                                : null,
                      ),
                    );
                  },
                );
              }),
            ),
          ],
        );
      },
    );
  }

  String _weekdayName(int day) {
    const days = ['', '一', '二', '三', '四', '五', '六', '日'];
    return days[day.clamp(0, 7)];
  }
}

class _SemesterFormDialog extends StatefulWidget {
  final Semester? semester;
  final VoidCallback onSaved;
  const _SemesterFormDialog({this.semester, required this.onSaved});

  @override
  State<_SemesterFormDialog> createState() => _SemesterFormDialogState();
}

class _SemesterFormDialogState extends State<_SemesterFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _nameController;
  late final TextEditingController _codeController;
  late final TextEditingController _academicYearController;
  late final TextEditingController _semesterTypeController;
  late final TextEditingController _startDateController;
  late final TextEditingController _endDateController;
  late final TextEditingController _totalWeeksController;
  late final TextEditingController _teachingWeeksController;

  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.semester?.name ?? '');
    _codeController = TextEditingController(text: widget.semester?.code ?? '');
    _academicYearController = TextEditingController(text: widget.semester?.academicYear ?? '');
    _semesterTypeController = TextEditingController(text: widget.semester?.semesterType ?? '');
    _startDateController = TextEditingController(text: widget.semester?.startDate ?? '');
    _endDateController = TextEditingController(text: widget.semester?.endDate ?? '');
    _totalWeeksController = TextEditingController(text: widget.semester?.totalWeeks.toString() ?? '20');
    _teachingWeeksController = TextEditingController(text: widget.semester?.teachingWeeks.toString() ?? '18');
  }

  @override
  void dispose() {
    _nameController.dispose();
    _codeController.dispose();
    _academicYearController.dispose();
    _semesterTypeController.dispose();
    _startDateController.dispose();
    _endDateController.dispose();
    _totalWeeksController.dispose();
    _teachingWeeksController.dispose();
    super.dispose();
  }

  Future<void> _onSave() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final controller = Get.find<SemesterController>();
    final semester = Semester(
      id: widget.semester?.id ?? '',
      name: _nameController.text.trim(),
      code: _codeController.text.trim(),
      academicYear: _academicYearController.text.trim(),
      semesterType: _semesterTypeController.text.trim(),
      startDate: _startDateController.text.trim(),
      endDate: _endDateController.text.trim(),
      totalWeeks: int.tryParse(_totalWeeksController.text) ?? 20,
      teachingWeeks: int.tryParse(_teachingWeeksController.text) ?? 18,
    );

    bool success;
    if (widget.semester != null) {
      success = await controller.updateSemester(widget.semester!.id, semester);
    } else {
      success = await controller.createSemester(semester);
    }

    setState(() => _isLoading = false);

    if (success) {
      widget.onSaved();
      if (mounted) Navigator.pop(context);
      Get.snackbar('成功', widget.semester != null ? '更新成功' : '创建成功',
          snackPosition: SnackPosition.BOTTOM);
    } else {
      Get.snackbar('错误', controller.error.value ?? '操作失败',
          snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.semester != null ? '编辑学期' : '新增学期'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(labelText: '学期名称 *'),
                validator: (v) => v?.isEmpty == true ? '请输入学期名称' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _codeController,
                decoration: const InputDecoration(labelText: '学期代码 *'),
                validator: (v) => v?.isEmpty == true ? '请输入学期代码' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _academicYearController,
                decoration: const InputDecoration(labelText: '学年度 *', hintText: '如: 2024-2025'),
                validator: (v) => v?.isEmpty == true ? '请输入学年度' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _semesterTypeController,
                decoration: const InputDecoration(labelText: '学期类型 *', hintText: '如: 第一学期'),
                validator: (v) => v?.isEmpty == true ? '请输入学期类型' : null,
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _startDateController,
                      decoration: const InputDecoration(labelText: '开始日期', hintText: 'YYYY-MM-DD'),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _endDateController,
                      decoration: const InputDecoration(labelText: '结束日期', hintText: 'YYYY-MM-DD'),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _totalWeeksController,
                      decoration: const InputDecoration(labelText: '总周数'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _teachingWeeksController,
                      decoration: const InputDecoration(labelText: '教学周数'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('取消'),
        ),
        ElevatedButton(
          onPressed: _isLoading ? null : _onSave,
          child: _isLoading
              ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2))
              : const Text('保存'),
        ),
      ],
    );
  }
}
