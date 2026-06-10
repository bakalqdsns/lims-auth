import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class CoursesScreen extends StatefulWidget {
  const CoursesScreen({super.key});

  @override
  State<CoursesScreen> createState() => _CoursesScreenState();
}

class _CoursesScreenState extends State<CoursesScreen> {
  final _authController = Get.find<AuthController>();
  final _service = CourseService();

  final _searchController = TextEditingController();
  String? _courseTypeFilter;
  int _currentPage = 1;
  int _pageSize = 10;
  bool _isLoading = false;
  List<Course> _courses = [];

  @override
  void initState() {
    super.initState();
    _loadCourses();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadCourses() async {
    setState(() => _isLoading = true);
    try {
      _courses = await _service.getCourses(keyword: _searchController.text.isNotEmpty ? _searchController.text : null);
      setState(() {
        _currentPage = 1;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      Get.snackbar('错误', '加载课程列表失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
    }
  }

  List<Course> get _filteredCourses {
    var list = _courses;
    if (_courseTypeFilter != null && _courseTypeFilter!.isNotEmpty) {
      list = list.where((c) => c.courseType == _courseTypeFilter).toList();
    }
    return list;
  }

  List<Course> get _paginatedCourses {
    final start = (_currentPage - 1) * _pageSize;
    final end = start + _pageSize;
    final filtered = _filteredCourses;
    if (start >= filtered.length) return [];
    return filtered.sublist(start, end > filtered.length ? filtered.length : end);
  }

  int get _totalPages => (_filteredCourses.length / _pageSize).ceil();

  void _onSearch() => _loadCourses();

  void _onReset() {
    _searchController.clear();
    setState(() {
      _courseTypeFilter = null;
      _currentPage = 1;
    });
    _loadCourses();
  }

  void _showFormDialog({Course? course}) {
    showDialog(
      context: context,
      builder: (context) => _CourseFormDialog(
        course: course,
        onSaved: _loadCourses,
      ),
    );
  }

  Future<void> _handleDelete(Course course) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认删除'),
        content: Text('确定要删除课程 "${course.name}" 吗？'),
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
        await _service.deleteCourse(course.id);
        Get.snackbar('成功', '删除成功', snackPosition: SnackPosition.BOTTOM);
        _loadCourses();
      } catch (e) {
        Get.snackbar('错误', '删除失败', snackPosition: SnackPosition.BOTTOM,
            backgroundColor: AppTheme.error);
      }
    }
  }

  Future<void> _handleStatusChange(Course course, bool value) async {
    try {
      await _service.toggleCourseStatus(course.id, value);
      Get.snackbar('成功', value ? '课程已启用' : '课程已禁用',
          snackPosition: SnackPosition.BOTTOM);
      _loadCourses();
    } catch (e) {
      Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
      _loadCourses();
    }
  }

  Color _getCourseTypeColor(String? type) {
    switch (type) {
      case '必修':
        return AppTheme.error;
      case '选修':
        return AppTheme.success;
      case '限选':
        return AppTheme.warning;
      default:
        return AppTheme.info;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('课程管理'),
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
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _paginatedCourses.isEmpty
                    ? const Center(child: Text('暂无数据', style: TextStyle(color: AppTheme.textSecondary)))
                    : ListView.builder(
                        padding: const EdgeInsets.all(16),
                        itemCount: _paginatedCourses.length,
                        itemBuilder: (context, index) => _buildCourseCard(_paginatedCourses[index]),
                      ),
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
                hintText: '搜索课程代码/名称',
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
                const Text('修读性质: ', style: TextStyle(color: AppTheme.textSecondary)),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('全部'),
                  selected: _courseTypeFilter == null || _courseTypeFilter!.isEmpty,
                  onSelected: (selected) {
                    setState(() {
                      _courseTypeFilter = null;
                      _currentPage = 1;
                    });
                  },
                ),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('必修'),
                  selected: _courseTypeFilter == '必修',
                  backgroundColor: AppTheme.error.withOpacity(0.1),
                  selectedColor: AppTheme.error.withOpacity(0.3),
                  onSelected: (selected) {
                    setState(() {
                      _courseTypeFilter = selected ? '必修' : null;
                      _currentPage = 1;
                    });
                  },
                ),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('选修'),
                  selected: _courseTypeFilter == '选修',
                  backgroundColor: AppTheme.success.withOpacity(0.1),
                  selectedColor: AppTheme.success.withOpacity(0.3),
                  onSelected: (selected) {
                    setState(() {
                      _courseTypeFilter = selected ? '选修' : null;
                      _currentPage = 1;
                    });
                  },
                ),
                const SizedBox(width: 8),
                ChoiceChip(
                  label: const Text('限选'),
                  selected: _courseTypeFilter == '限选',
                  backgroundColor: AppTheme.warning.withOpacity(0.1),
                  selectedColor: AppTheme.warning.withOpacity(0.3),
                  onSelected: (selected) {
                    setState(() {
                      _courseTypeFilter = selected ? '限选' : null;
                      _currentPage = 1;
                    });
                  },
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                OutlinedButton(
                  onPressed: _onReset,
                  child: const Text('重置'),
                ),
                const SizedBox(width: 8),
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

  Widget _buildCourseCard(Course course) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: AppTheme.primary.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(4),
                  ),
                  child: Text(
                    course.code,
                    style: const TextStyle(
                      color: AppTheme.primary,
                      fontWeight: FontWeight.bold,
                      fontSize: 13,
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    course.name,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.textPrimary,
                    ),
                  ),
                ),
                if (course.courseType != null && course.courseType!.isNotEmpty)
                  Chip(
                    label: Text(course.courseType!),
                    backgroundColor: _getCourseTypeColor(course.courseType).withOpacity(0.1),
                    labelStyle: TextStyle(
                      color: _getCourseTypeColor(course.courseType),
                      fontSize: 12,
                    ),
                    padding: EdgeInsets.zero,
                    materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                  ),
              ],
            ),
            if (course.englishName != null && course.englishName!.isNotEmpty) ...[
              const SizedBox(height: 4),
              Text(
                course.englishName!,
                style: const TextStyle(color: AppTheme.textSecondary, fontSize: 13),
              ),
            ],
            const SizedBox(height: 12),
            Row(
              children: [
                _InfoChip(label: '学分: ${course.credits}'),
                const SizedBox(width: 8),
                _InfoChip(label: '学时: ${course.totalHours}'),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Icon(Icons.timer_outlined, size: 14, color: AppTheme.textSecondary),
                const SizedBox(width: 4),
                Text(
                  '讲授${course.theoryHours}/实践${course.practiceHours}/实验${course.experimentHours}',
                  style: const TextStyle(color: AppTheme.textSecondary, fontSize: 12),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                const Text('状态: ', style: TextStyle(color: AppTheme.textSecondary, fontSize: 13)),
                Switch(
                  value: course.isActive,
                  onChanged: _authController.isAdmin
                      ? (value) => _handleStatusChange(course, value)
                      : null,
                  activeColor: AppTheme.success,
                ),
                const Spacer(),
                if (_authController.isAdmin) ...[
                  TextButton(
                    onPressed: () => _showFormDialog(course: course),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.primary,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('编辑'),
                  ),
                  TextButton(
                    onPressed: () => _handleDelete(course),
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

class _CourseFormDialog extends StatefulWidget {
  final Course? course;
  final VoidCallback onSaved;
  const _CourseFormDialog({this.course, required this.onSaved});

  @override
  State<_CourseFormDialog> createState() => _CourseFormDialogState();
}

class _CourseFormDialogState extends State<_CourseFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _codeController;
  late final TextEditingController _nameController;
  late final TextEditingController _englishNameController;
  late final TextEditingController _courseTypeController;
  late final TextEditingController _creditsController;
  late final TextEditingController _totalHoursController;
  late final TextEditingController _theoryHoursController;
  late final TextEditingController _practiceHoursController;
  late final TextEditingController _experimentHoursController;
  late final TextEditingController _descriptionController;

  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _codeController = TextEditingController(text: widget.course?.code ?? '');
    _nameController = TextEditingController(text: widget.course?.name ?? '');
    _englishNameController = TextEditingController(text: widget.course?.englishName ?? '');
    _courseTypeController = TextEditingController(text: widget.course?.courseType ?? '必修');
    _creditsController = TextEditingController(text: widget.course?.credits.toString() ?? '2.0');
    _totalHoursController = TextEditingController(text: widget.course?.totalHours.toString() ?? '32');
    _theoryHoursController = TextEditingController(text: widget.course?.theoryHours.toString() ?? '24');
    _practiceHoursController = TextEditingController(text: widget.course?.practiceHours.toString() ?? '8');
    _experimentHoursController = TextEditingController(text: widget.course?.experimentHours.toString() ?? '0');
    _descriptionController = TextEditingController(text: widget.course?.description ?? '');
  }

  @override
  void dispose() {
    _codeController.dispose();
    _nameController.dispose();
    _englishNameController.dispose();
    _courseTypeController.dispose();
    _creditsController.dispose();
    _totalHoursController.dispose();
    _theoryHoursController.dispose();
    _practiceHoursController.dispose();
    _experimentHoursController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _onSave() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final service = CourseService();
    final course = Course(
      id: widget.course?.id ?? '',
      code: _codeController.text.trim(),
      name: _nameController.text.trim(),
      englishName: _englishNameController.text.trim().isNotEmpty ? _englishNameController.text.trim() : null,
      courseType: _courseTypeController.text.trim(),
      credits: double.tryParse(_creditsController.text) ?? 2.0,
      totalHours: int.tryParse(_totalHoursController.text) ?? 32,
      theoryHours: int.tryParse(_theoryHoursController.text) ?? 24,
      practiceHours: int.tryParse(_practiceHoursController.text) ?? 8,
      experimentHours: int.tryParse(_experimentHoursController.text) ?? 0,
      description: _descriptionController.text.trim().isNotEmpty ? _descriptionController.text.trim() : null,
    );

    bool success = false;
    try {
      if (widget.course != null) {
        success = await service.updateCourse(widget.course!.id, course);
      } else {
        await service.createCourse(course);
        success = true;
      }
    } catch (e) {
      Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
    }

    setState(() => _isLoading = false);

    if (success) {
      widget.onSaved();
      if (mounted) Navigator.pop(context);
      Get.snackbar('成功', widget.course != null ? '更新成功' : '创建成功', snackPosition: SnackPosition.BOTTOM);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.course != null ? '编辑课程' : '新增课程'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _codeController,
                decoration: const InputDecoration(labelText: '课程代码 *'),
                validator: (v) => v?.isEmpty == true ? '请输入课程代码' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(labelText: '课程名称 *'),
                validator: (v) => v?.isEmpty == true ? '请输入课程名称' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _englishNameController,
                decoration: const InputDecoration(labelText: '英文名称'),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<String>(
                value: _courseTypeController.text.isEmpty ? '必修' : _courseTypeController.text,
                decoration: const InputDecoration(labelText: '修读性质 *'),
                items: const [
                  DropdownMenuItem(value: '必修', child: Text('必修')),
                  DropdownMenuItem(value: '选修', child: Text('选修')),
                  DropdownMenuItem(value: '限选', child: Text('限选')),
                ],
                onChanged: (v) => _courseTypeController.text = v ?? '必修',
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _creditsController,
                      decoration: const InputDecoration(labelText: '学分'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _totalHoursController,
                      decoration: const InputDecoration(labelText: '总学时'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              const Text('学时分配', style: TextStyle(fontSize: 12, color: AppTheme.textSecondary)),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _theoryHoursController,
                      decoration: const InputDecoration(labelText: '讲授'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: TextFormField(
                      controller: _practiceHoursController,
                      decoration: const InputDecoration(labelText: '实践'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: TextFormField(
                      controller: _experimentHoursController,
                      decoration: const InputDecoration(labelText: '实验'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _descriptionController,
                decoration: const InputDecoration(labelText: '描述'),
                maxLines: 2,
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
