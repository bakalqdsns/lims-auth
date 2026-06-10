import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/api_service.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class TeachingTasksScreen extends StatefulWidget {
  const TeachingTasksScreen({super.key});

  @override
  State<TeachingTasksScreen> createState() => _TeachingTasksScreenState();
}

class _TeachingTasksScreenState extends State<TeachingTasksScreen> {
  final _authController = Get.find<AuthController>();
  final _api = ApiService.to;

  int _currentPage = 1;
  int _pageSize = 10;
  bool _isLoading = false;
  List<TeachingTask> _tasks = [];
  List<Semester> _semesters = [];
  List<Course> _courses = [];
  List<ClassModel> _classes = [];

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    try {
      final futures = await Future.wait([
        _api.get('/teaching-tasks'),
        _api.get('/semesters'),
        _api.get('/courses'),
        _api.get('/classes'),
      ]);

      _tasks = _parseTasks(futures[0].data);
      _semesters = _parseSemesters(futures[1].data);
      _courses = _parseCourses(futures[2].data);
      _classes = _parseClasses(futures[3].data);

      setState(() {
        _currentPage = 1;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      Get.snackbar('错误', '加载教学任务列表失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
    }
  }

  List<TeachingTask> _parseTasks(dynamic data) {
    if (data['code'] != 200) return [];
    final items = data['data']?['items'] ?? data['data'] ?? [];
    if (items is List) {
      return items.map((e) => TeachingTask.fromJson(e as Map<String, dynamic>)).toList();
    }
    return [];
  }

  List<Semester> _parseSemesters(dynamic data) {
    if (data['code'] != 200) return [];
    final items = data['data']?['items'] ?? data['data'] ?? [];
    if (items is List) {
      return items.map((e) => Semester.fromJson(e as Map<String, dynamic>)).toList();
    }
    return [];
  }

  List<Course> _parseCourses(dynamic data) {
    if (data['code'] != 200) return [];
    final items = data['data']?['items'] ?? data['data'] ?? [];
    if (items is List) {
      return items.map((e) => Course.fromJson(e as Map<String, dynamic>)).toList();
    }
    return [];
  }

  List<ClassModel> _parseClasses(dynamic data) {
    if (data['code'] != 200) return [];
    final items = data['data']?['items'] ?? data['data'] ?? [];
    if (items is List) {
      return items.map((e) => ClassModel.fromJson(e as Map<String, dynamic>)).toList();
    }
    return [];
  }

  List<TeachingTask> get _paginatedTasks {
    final start = (_currentPage - 1) * _pageSize;
    final end = start + _pageSize;
    if (start >= _tasks.length) return [];
    return _tasks.sublist(start, end > _tasks.length ? _tasks.length : end);
  }

  int get _totalPages => (_tasks.length / _pageSize).ceil();

  void _showFormDialog({TeachingTask? task}) {
    showDialog(
      context: context,
      builder: (context) => _TeachingTaskFormDialog(
        task: task,
        semesters: _semesters,
        courses: _courses,
        classes: _classes,
        onSaved: _loadData,
      ),
    );
  }

  Future<void> _handleToggleStatus(TeachingTask task) async {
    final newStatus = !task.isActive;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认操作'),
        content: Text('确定要${newStatus ? '启用' : '禁用'}该教学任务吗？'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('取消'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            style: TextButton.styleFrom(foregroundColor: newStatus ? AppTheme.success : AppTheme.warning),
            child: Text(newStatus ? '启用' : '禁用'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      try {
        final res = await _api.patch('/teaching-tasks/${task.id}/status', data: {'isActive': newStatus});
        if (res.data['code'] == 200) {
          Get.snackbar('成功', newStatus ? '教学任务已启用' : '教学任务已禁用',
              snackPosition: SnackPosition.BOTTOM);
          _loadData();
        } else {
          Get.snackbar('错误', res.data['message'] ?? '操作失败',
              snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
        }
      } catch (e) {
        Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM,
            backgroundColor: AppTheme.error);
      }
    }
  }

  Future<void> _handleDelete(TeachingTask task) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认删除'),
        content: const Text('确定要删除该教学任务吗？此操作不可恢复！'),
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
      try {
        final res = await _api.delete('/teaching-tasks/${task.id}');
        if (res.data['code'] == 200) {
          Get.snackbar('成功', '删除成功', snackPosition: SnackPosition.BOTTOM);
          _loadData();
        } else {
          Get.snackbar('错误', res.data['message'] ?? '删除失败',
              snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
        }
      } catch (e) {
        Get.snackbar('错误', '删除失败', snackPosition: SnackPosition.BOTTOM,
            backgroundColor: AppTheme.error);
      }
    }
  }

  String _getSemesterName(String? semesterId) {
    if (semesterId == null) return '-';
    final semester = _semesters.firstWhereOrNull((s) => s.id == semesterId);
    return semester?.name ?? '-';
  }

  String _getCourseName(String? courseId) {
    if (courseId == null) return '-';
    final course = _courses.firstWhereOrNull((c) => c.id == courseId);
    return course?.name ?? '-';
  }

  String _getCourseCode(String? courseId) {
    if (courseId == null) return '-';
    final course = _courses.firstWhereOrNull((c) => c.id == courseId);
    return course?.code ?? '-';
  }

  String _getClassName(String? classId) {
    if (classId == null) return '-';
    final classModel = _classes.firstWhereOrNull((c) => c.id == classId);
    return classModel?.name ?? '-';
  }

  int _getStudentCount(String? classId) {
    if (classId == null) return 0;
    final classModel = _classes.firstWhereOrNull((c) => c.id == classId);
    return classModel?.studentCount ?? 0;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('教学任务管理'),
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
          _buildHeader(),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _paginatedTasks.isEmpty
                    ? const Center(child: Text('暂无数据', style: TextStyle(color: AppTheme.textSecondary)))
                    : ListView.builder(
                        padding: const EdgeInsets.all(16),
                        itemCount: _paginatedTasks.length,
                        itemBuilder: (context, index) => _buildTaskCard(_paginatedTasks[index], index),
                      ),
          ),
          _buildPagination(),
        ],
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Row(
        children: [
          const Icon(Icons.assignment, color: AppTheme.primary),
          const SizedBox(width: 8),
          Text(
            '共 ${_tasks.length} 个教学任务',
            style: const TextStyle(color: AppTheme.textSecondary),
          ),
        ],
      ),
    );
  }

  Widget _buildTaskCard(TeachingTask task, int index) {
    final teachers = task.teachers ?? [];
    final studentCount = _getStudentCount(task.classId);

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  backgroundColor: AppTheme.primary.withOpacity(0.1),
                  child: Text(
                    '${index + 1 + (_currentPage - 1) * _pageSize}',
                    style: const TextStyle(color: AppTheme.primary, fontWeight: FontWeight.bold),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        _getCourseName(task.courseId),
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.textPrimary,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: AppTheme.primary.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(
                              _getCourseCode(task.courseId),
                              style: const TextStyle(fontSize: 11, color: AppTheme.primary),
                            ),
                          ),
                          const SizedBox(width: 8),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: AppTheme.info.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(
                              _getSemesterName(task.semesterId),
                              style: const TextStyle(fontSize: 11, color: AppTheme.info),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
                Chip(
                  label: Text(task.isActive ? '启用' : '禁用'),
                  backgroundColor: task.isActive
                      ? AppTheme.success.withOpacity(0.1)
                      : AppTheme.textSecondary.withOpacity(0.1),
                  labelStyle: TextStyle(
                    color: task.isActive ? AppTheme.success : AppTheme.textSecondary,
                    fontSize: 12,
                  ),
                  padding: EdgeInsets.zero,
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                _InfoChip(label: '班级: ${_getClassName(task.classId)}'),
                const SizedBox(width: 8),
                _InfoChip(label: '学生: ${studentCount}人'),
              ],
            ),
            const SizedBox(height: 8),
            const Text(
              '任课教师:',
              style: TextStyle(color: AppTheme.textSecondary, fontSize: 12),
            ),
            const SizedBox(height: 4),
            if (teachers.isEmpty)
              const Text(
                '暂无教师',
                style: TextStyle(color: AppTheme.textHint, fontSize: 13),
              )
            else
              Wrap(
                spacing: 4,
                runSpacing: 4,
                children: teachers.map((t) {
                  final isMain = t.isMainTeacher;
                  return Chip(
                    avatar: isMain
                        ? const Icon(Icons.star, size: 14, color: AppTheme.success)
                        : null,
                    label: Text(
                      t.teacher?.fullName ?? '未知',
                      style: TextStyle(
                        fontSize: 12,
                        color: isMain ? AppTheme.success : AppTheme.textSecondary,
                      ),
                    ),
                    backgroundColor: isMain
                        ? AppTheme.success.withOpacity(0.1)
                        : AppTheme.background,
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    visualDensity: VisualDensity.compact,
                  );
                }).toList(),
              ),
            if (task.description != null && task.description!.isNotEmpty) ...[
              const SizedBox(height: 8),
              Text(
                task.description!,
                style: const TextStyle(color: AppTheme.textSecondary, fontSize: 13),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
            const SizedBox(height: 12),
            Row(
              children: [
                const Spacer(),
                if (_authController.isAdmin) ...[
                  TextButton(
                    onPressed: () => _showFormDialog(task: task),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.primary,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('编辑'),
                  ),
                  TextButton(
                    onPressed: () => _handleToggleStatus(task),
                    style: TextButton.styleFrom(
                      foregroundColor: task.isActive ? AppTheme.warning : AppTheme.success,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: Text(task.isActive ? '禁用' : '启用'),
                  ),
                  TextButton(
                    onPressed: () => _handleDelete(task),
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

class _TeachingTaskFormDialog extends StatefulWidget {
  final TeachingTask? task;
  final List<Semester> semesters;
  final List<Course> courses;
  final List<ClassModel> classes;
  final VoidCallback onSaved;
  const _TeachingTaskFormDialog({
    this.task,
    required this.semesters,
    required this.courses,
    required this.classes,
    required this.onSaved,
  });

  @override
  State<_TeachingTaskFormDialog> createState() => _TeachingTaskFormDialogState();
}

class _TeachingTaskFormDialogState extends State<_TeachingTaskFormDialog> {
  final _formKey = GlobalKey<FormState>();
  final _api = ApiService.to;

  String? _selectedSemesterId;
  String? _selectedCourseId;
  String? _selectedClassId;
  late final TextEditingController _descriptionController;

  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _selectedSemesterId = widget.task?.semesterId;
    _selectedCourseId = widget.task?.courseId;
    _selectedClassId = widget.task?.classId;
    _descriptionController = TextEditingController(text: widget.task?.description ?? '');
  }

  @override
  void dispose() {
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _onSave() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedSemesterId == null || _selectedCourseId == null || _selectedClassId == null) {
      Get.snackbar('错误', '请填写所有必填项', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
      return;
    }

    setState(() => _isLoading = true);

    final data = {
      'semesterId': _selectedSemesterId,
      'courseId': _selectedCourseId,
      'classId': _selectedClassId,
      'description': _descriptionController.text.trim(),
    };

    try {
      if (widget.task != null) {
        await _api.put('/teaching-tasks/${widget.task!.id}', data: data);
      } else {
        await _api.post('/teaching-tasks', data: data);
      }
      widget.onSaved();
      if (mounted) Navigator.pop(context);
      Get.snackbar('成功', widget.task != null ? '更新成功' : '创建成功', snackPosition: SnackPosition.BOTTOM);
    } catch (e) {
      Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
    }

    setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.task != null ? '编辑教学任务' : '新增教学任务'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              DropdownButtonFormField<String>(
                value: _selectedSemesterId,
                decoration: const InputDecoration(labelText: '学期 *'),
                items: widget.semesters.map((s) {
                  return DropdownMenuItem(value: s.id, child: Text(s.name));
                }).toList(),
                onChanged: (v) => setState(() => _selectedSemesterId = v),
                validator: (v) => v == null ? '请选择学期' : null,
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                value: _selectedCourseId,
                decoration: const InputDecoration(labelText: '课程 *'),
                items: widget.courses.map((c) {
                  return DropdownMenuItem(value: c.id, child: Text('${c.code} - ${c.name}'));
                }).toList(),
                onChanged: (v) => setState(() => _selectedCourseId = v),
                validator: (v) => v == null ? '请选择课程' : null,
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                value: _selectedClassId,
                decoration: const InputDecoration(labelText: '班级 *'),
                items: widget.classes.map((c) {
                  return DropdownMenuItem(value: c.id, child: Text(c.name));
                }).toList(),
                onChanged: (v) => setState(() => _selectedClassId = v),
                validator: (v) => v == null ? '请选择班级' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _descriptionController,
                decoration: const InputDecoration(labelText: '描述'),
                maxLines: 3,
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
