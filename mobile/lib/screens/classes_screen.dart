import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class ClassesScreen extends StatefulWidget {
  const ClassesScreen({super.key});

  @override
  State<ClassesScreen> createState() => _ClassesScreenState();
}

class _ClassesScreenState extends State<ClassesScreen> {
  final _authController = Get.find<AuthController>();
  final _service = ClassService();

  int _currentPage = 1;
  int _pageSize = 10;
  bool _isLoading = false;
  List<ClassModel> _classes = [];

  @override
  void initState() {
    super.initState();
    _loadClasses();
  }

  Future<void> _loadClasses() async {
    setState(() => _isLoading = true);
    try {
      _classes = await _service.getClasses();
      setState(() {
        _currentPage = 1;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      Get.snackbar('错误', '加载班级列表失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
    }
  }

  List<ClassModel> get _paginatedClasses {
    final start = (_currentPage - 1) * _pageSize;
    final end = start + _pageSize;
    if (start >= _classes.length) return [];
    return _classes.sublist(start, end > _classes.length ? _classes.length : end);
  }

  int get _totalPages => (_classes.length / _pageSize).ceil();

  void _showFormDialog({ClassModel? classModel}) {
    showDialog(
      context: context,
      builder: (context) => _ClassFormDialog(
        classModel: classModel,
        onSaved: _loadClasses,
      ),
    );
  }

  void _handleManageStudents(ClassModel classModel) {
    Get.snackbar('提示', '学生管理功能开发中', snackPosition: SnackPosition.BOTTOM,
        backgroundColor: AppTheme.info, colorText: Colors.white);
  }

  Future<void> _handleDelete(ClassModel classModel) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认删除'),
        content: Text('确定要删除班级 "${classModel.name}" 吗？此操作不可恢复！'),
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
        await _service.deleteClass(classModel.id);
        Get.snackbar('成功', '删除成功', snackPosition: SnackPosition.BOTTOM);
        _loadClasses();
      } catch (e) {
        Get.snackbar('错误', '删除失败', snackPosition: SnackPosition.BOTTOM,
            backgroundColor: AppTheme.error);
      }
    }
  }

  Future<void> _handleStatusChange(ClassModel classModel, bool value) async {
    try {
      await _service.toggleClassStatus(classModel.id, value);
      Get.snackbar('成功', value ? '班级已启用' : '班级已禁用',
          snackPosition: SnackPosition.BOTTOM);
      _loadClasses();
    } catch (e) {
      Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
      _loadClasses();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('班级管理'),
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
                : _paginatedClasses.isEmpty
                    ? const Center(child: Text('暂无数据', style: TextStyle(color: AppTheme.textSecondary)))
                    : ListView.builder(
                        padding: const EdgeInsets.all(16),
                        itemCount: _paginatedClasses.length,
                        itemBuilder: (context, index) => _buildClassCard(_paginatedClasses[index], index),
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
          const Icon(Icons.groups, color: AppTheme.primary),
          const SizedBox(width: 8),
          Text(
            '共 ${_classes.length} 个班级',
            style: const TextStyle(color: AppTheme.textSecondary),
          ),
        ],
      ),
    );
  }

  Widget _buildClassCard(ClassModel classModel, int index) {
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
                        classModel.name,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.textPrimary,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Row(
                        children: [
                          if (classModel.code.isNotEmpty)
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                              decoration: BoxDecoration(
                                color: AppTheme.background,
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: Text(
                                classModel.code,
                                style: const TextStyle(fontSize: 11, color: AppTheme.textSecondary),
                              ),
                            ),
                          if (classModel.grade != null && classModel.grade!.isNotEmpty) ...[
                            const SizedBox(width: 8),
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                              decoration: BoxDecoration(
                                color: AppTheme.primary.withOpacity(0.1),
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: Text(
                                '${classModel.grade}级',
                                style: const TextStyle(fontSize: 11, color: AppTheme.primary),
                              ),
                            ),
                          ],
                        ],
                      ),
                    ],
                  ),
                ),
                Chip(
                  label: Text(classModel.isActive ? '启用' : '禁用'),
                  backgroundColor: classModel.isActive
                      ? AppTheme.success.withOpacity(0.1)
                      : AppTheme.textSecondary.withOpacity(0.1),
                  labelStyle: TextStyle(
                    color: classModel.isActive ? AppTheme.success : AppTheme.textSecondary,
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
                if (classModel.major != null) ...[
                  _InfoChip(label: '专业: ${classModel.major!.name}'),
                  const SizedBox(width: 8),
                ],
                _InfoChip(label: '学生: ${classModel.studentCount}人'),
                if (classModel.headTeacher != null) ...[
                  const SizedBox(width: 8),
                  _InfoChip(label: '班主任: ${classModel.headTeacher!.fullName}'),
                ],
              ],
            ),
            if (classModel.description != null && classModel.description!.isNotEmpty) ...[
              const SizedBox(height: 8),
              Text(
                classModel.description!,
                style: const TextStyle(color: AppTheme.textSecondary, fontSize: 13),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
            const SizedBox(height: 12),
            Row(
              children: [
                const Text('状态: ', style: TextStyle(color: AppTheme.textSecondary, fontSize: 13)),
                Switch(
                  value: classModel.isActive,
                  onChanged: _authController.isAdmin
                      ? (value) => _handleStatusChange(classModel, value)
                      : null,
                  activeColor: AppTheme.success,
                ),
                const Spacer(),
                if (_authController.isAdmin) ...[
                  TextButton(
                    onPressed: () => _showFormDialog(classModel: classModel),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.primary,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('编辑'),
                  ),
                  TextButton(
                    onPressed: () => _handleManageStudents(classModel),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.info,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('学生管理'),
                  ),
                  TextButton(
                    onPressed: () => _handleDelete(classModel),
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

class _ClassFormDialog extends StatefulWidget {
  final ClassModel? classModel;
  final VoidCallback onSaved;
  const _ClassFormDialog({this.classModel, required this.onSaved});

  @override
  State<_ClassFormDialog> createState() => _ClassFormDialogState();
}

class _ClassFormDialogState extends State<_ClassFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _codeController;
  late final TextEditingController _nameController;
  late final TextEditingController _gradeController;
  late final TextEditingController _studentCountController;
  late final TextEditingController _descriptionController;

  String? _selectedMajorId;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _codeController = TextEditingController(text: widget.classModel?.code ?? '');
    _nameController = TextEditingController(text: widget.classModel?.name ?? '');
    _gradeController = TextEditingController(text: widget.classModel?.grade ?? '');
    _studentCountController = TextEditingController(text: widget.classModel?.studentCount.toString() ?? '30');
    _descriptionController = TextEditingController(text: widget.classModel?.description ?? '');
    _selectedMajorId = widget.classModel?.majorId;
  }

  @override
  void dispose() {
    _codeController.dispose();
    _nameController.dispose();
    _gradeController.dispose();
    _studentCountController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _onSave() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final service = ClassService();
    final classModel = ClassModel(
      id: widget.classModel?.id ?? '',
      code: _codeController.text.trim(),
      name: _nameController.text.trim(),
      grade: _gradeController.text.trim().isNotEmpty ? _gradeController.text.trim() : null,
      majorId: _selectedMajorId,
      studentCount: int.tryParse(_studentCountController.text) ?? 30,
      description: _descriptionController.text.trim().isNotEmpty ? _descriptionController.text.trim() : null,
    );

    bool success = false;
    try {
      if (widget.classModel != null) {
        success = await service.updateClass(widget.classModel!.id, classModel);
      } else {
        await service.createClass(classModel);
        success = true;
      }
    } catch (e) {
      Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
    }

    setState(() => _isLoading = false);

    if (success) {
      widget.onSaved();
      if (mounted) Navigator.pop(context);
      Get.snackbar('成功', widget.classModel != null ? '更新成功' : '创建成功', snackPosition: SnackPosition.BOTTOM);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.classModel != null ? '编辑班级' : '新增班级'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _codeController,
                decoration: const InputDecoration(labelText: '班级代码 *'),
                validator: (v) => v?.isEmpty == true ? '请输入班级代码' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(labelText: '班级名称 *'),
                validator: (v) => v?.isEmpty == true ? '请输入班级名称' : null,
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _gradeController,
                      decoration: const InputDecoration(labelText: '年级', hintText: '如: 2024'),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _studentCountController,
                      decoration: const InputDecoration(labelText: '学生数'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                ],
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
