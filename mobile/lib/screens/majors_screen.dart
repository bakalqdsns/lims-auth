import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class MajorsScreen extends StatefulWidget {
  const MajorsScreen({super.key});

  @override
  State<MajorsScreen> createState() => _MajorsScreenState();
}

class _MajorsScreenState extends State<MajorsScreen> {
  final _authController = Get.find<AuthController>();
  final _service = MajorService();

  int _currentPage = 1;
  int _pageSize = 10;
  bool _isLoading = false;
  List<Major> _majors = [];

  @override
  void initState() {
    super.initState();
    _loadMajors();
  }

  Future<void> _loadMajors() async {
    setState(() => _isLoading = true);
    try {
      _majors = await _service.getMajors();
      setState(() {
        _currentPage = 1;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
      Get.snackbar('错误', '加载专业列表失败', snackPosition: SnackPosition.BOTTOM,
          backgroundColor: AppTheme.error);
    }
  }

  List<Major> get _paginatedMajors {
    final start = (_currentPage - 1) * _pageSize;
    final end = start + _pageSize;
    if (start >= _majors.length) return [];
    return _majors.sublist(start, end > _majors.length ? _majors.length : end);
  }

  int get _totalPages => (_majors.length / _pageSize).ceil();

  void _showFormDialog({Major? major}) {
    showDialog(
      context: context,
      builder: (context) => _MajorFormDialog(
        major: major,
        onSaved: _loadMajors,
      ),
    );
  }

  Future<void> _handleToggleStatus(Major major) async {
    final newStatus = !major.isActive;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认操作'),
        content: Text('确定要${newStatus ? '启用' : '禁用'}专业 "${major.name}" 吗？'),
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
        await _service.toggleMajorStatus(major.id, newStatus);
        Get.snackbar('成功', newStatus ? '专业已启用' : '专业已禁用',
            snackPosition: SnackPosition.BOTTOM);
        _loadMajors();
      } catch (e) {
        Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM,
            backgroundColor: AppTheme.error);
      }
    }
  }

  Future<void> _handleDelete(Major major) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('确认删除'),
        content: Text('确定要删除专业 "${major.name}" 吗？此操作不可恢复！'),
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
        await _service.deleteMajor(major.id);
        Get.snackbar('成功', '删除成功', snackPosition: SnackPosition.BOTTOM);
        _loadMajors();
      } catch (e) {
        Get.snackbar('错误', '删除失败', snackPosition: SnackPosition.BOTTOM,
            backgroundColor: AppTheme.error);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('专业管理'),
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
                : _paginatedMajors.isEmpty
                    ? const Center(child: Text('暂无数据', style: TextStyle(color: AppTheme.textSecondary)))
                    : ListView.builder(
                        padding: const EdgeInsets.all(16),
                        itemCount: _paginatedMajors.length,
                        itemBuilder: (context, index) => _buildMajorCard(_paginatedMajors[index], index),
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
          const Icon(Icons.school, color: AppTheme.primary),
          const SizedBox(width: 8),
          Text(
            '共 ${_majors.length} 个专业',
            style: const TextStyle(color: AppTheme.textSecondary),
          ),
        ],
      ),
    );
  }

  Widget _buildMajorCard(Major major, int index) {
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
                        major.name,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.textPrimary,
                        ),
                      ),
                      if (major.englishName != null && major.englishName!.isNotEmpty)
                        Text(
                          major.englishName!,
                          style: const TextStyle(color: AppTheme.textSecondary, fontSize: 13),
                        ),
                    ],
                  ),
                ),
                Chip(
                  label: Text(major.isActive ? '启用' : '禁用'),
                  backgroundColor: major.isActive
                      ? AppTheme.success.withOpacity(0.1)
                      : AppTheme.textSecondary.withOpacity(0.1),
                  labelStyle: TextStyle(
                    color: major.isActive ? AppTheme.success : AppTheme.textSecondary,
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
                _InfoChip(label: '代码: ${major.code}'),
                if (major.department != null) ...[
                  const SizedBox(width: 8),
                  _InfoChip(label: '院系: ${major.department!.name}'),
                ],
                if (major.duration > 0) ...[
                  const SizedBox(width: 8),
                  _InfoChip(label: '学制: ${major.duration}年'),
                ],
              ],
            ),
            if (major.description != null && major.description!.isNotEmpty) ...[
              const SizedBox(height: 8),
              Text(
                major.description!,
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
                    onPressed: () => _showFormDialog(major: major),
                    style: TextButton.styleFrom(
                      foregroundColor: AppTheme.primary,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: const Text('编辑'),
                  ),
                  TextButton(
                    onPressed: () => _handleToggleStatus(major),
                    style: TextButton.styleFrom(
                      foregroundColor: major.isActive ? AppTheme.warning : AppTheme.success,
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: Text(major.isActive ? '禁用' : '启用'),
                  ),
                  TextButton(
                    onPressed: () => _handleDelete(major),
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

class _MajorFormDialog extends StatefulWidget {
  final Major? major;
  final VoidCallback onSaved;
  const _MajorFormDialog({this.major, required this.onSaved});

  @override
  State<_MajorFormDialog> createState() => _MajorFormDialogState();
}

class _MajorFormDialogState extends State<_MajorFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _codeController;
  late final TextEditingController _nameController;
  late final TextEditingController _englishNameController;
  late final TextEditingController _durationController;
  late final TextEditingController _degreeTypeController;
  late final TextEditingController _descriptionController;

  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _codeController = TextEditingController(text: widget.major?.code ?? '');
    _nameController = TextEditingController(text: widget.major?.name ?? '');
    _englishNameController = TextEditingController(text: widget.major?.englishName ?? '');
    _durationController = TextEditingController(text: widget.major?.duration.toString() ?? '4');
    _degreeTypeController = TextEditingController(text: widget.major?.degreeType ?? '学士');
    _descriptionController = TextEditingController(text: widget.major?.description ?? '');
  }

  @override
  void dispose() {
    _codeController.dispose();
    _nameController.dispose();
    _englishNameController.dispose();
    _durationController.dispose();
    _degreeTypeController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _onSave() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final service = MajorService();
    final major = Major(
      id: widget.major?.id ?? '',
      code: _codeController.text.trim(),
      name: _nameController.text.trim(),
      englishName: _englishNameController.text.trim().isNotEmpty ? _englishNameController.text.trim() : null,
      departmentId: widget.major?.departmentId,
      duration: int.tryParse(_durationController.text) ?? 4,
      degreeType: _degreeTypeController.text.trim().isNotEmpty ? _degreeTypeController.text.trim() : null,
      description: _descriptionController.text.trim().isNotEmpty ? _descriptionController.text.trim() : null,
    );

    bool success = false;
    try {
      if (widget.major != null) {
        success = await service.updateMajor(widget.major!.id, major);
      } else {
        await service.createMajor(major);
        success = true;
      }
    } catch (e) {
      Get.snackbar('错误', '操作失败', snackPosition: SnackPosition.BOTTOM, backgroundColor: AppTheme.error);
    }

    setState(() => _isLoading = false);

    if (success) {
      widget.onSaved();
      if (mounted) Navigator.pop(context);
      Get.snackbar('成功', widget.major != null ? '更新成功' : '创建成功', snackPosition: SnackPosition.BOTTOM);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.major != null ? '编辑专业' : '新增专业'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _codeController,
                decoration: const InputDecoration(labelText: '专业代码 *'),
                validator: (v) => v?.isEmpty == true ? '请输入专业代码' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(labelText: '专业名称 *'),
                validator: (v) => v?.isEmpty == true ? '请输入专业名称' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _englishNameController,
                decoration: const InputDecoration(labelText: '英文名称'),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _durationController,
                      decoration: const InputDecoration(labelText: '学制(年)'),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: DropdownButtonFormField<String>(
                      value: _degreeTypeController.text.isEmpty ? '学士' : _degreeTypeController.text,
                      decoration: const InputDecoration(labelText: '学位类型'),
                      items: const [
                        DropdownMenuItem(value: '学士', child: Text('学士')),
                        DropdownMenuItem(value: '硕士', child: Text('硕士')),
                        DropdownMenuItem(value: '博士', child: Text('博士')),
                      ],
                      onChanged: (v) => _degreeTypeController.text = v ?? '学士',
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
