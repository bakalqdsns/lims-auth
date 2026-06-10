import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class DepartmentsScreen extends StatefulWidget {
  const DepartmentsScreen({super.key});

  @override
  State<DepartmentsScreen> createState() => _DepartmentsScreenState();
}

class _DepartmentsScreenState extends State<DepartmentsScreen> {
  final _auth = Get.find<AuthController>();
  final _departmentService = Get.find<DepartmentService>();

  bool _isLoading = false;
  List<Department> _departments = [];

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    try {
      final departments = await _departmentService.getDepartments(tree: true);
      setState(() {
        _departments = departments;
      });
    } catch (e) {
      Get.snackbar('错误', '获取部门列表失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _handleCreate([Department? parent]) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => _DepartmentFormDialog(
        departments: _departments,
        parentDepartment: parent,
      ),
    );
    if (result == true) {
      _loadData();
    }
  }

  Future<void> _handleEdit(Department department) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => _DepartmentFormDialog(
        department: department,
        departments: _departments,
      ),
    );
    if (result == true) {
      _loadData();
    }
  }

  Future<void> _handleDelete(Department department) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('删除部门'),
        content: Text('确定要删除部门 "${department.name}" 吗？\n注意：子部门也会被删除。'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('取消'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            style: ElevatedButton.styleFrom(backgroundColor: AppTheme.error),
            child: const Text('删除'),
          ),
        ],
      ),
    );

    if (confirm == true) {
      try {
        await _departmentService.deleteDepartment(department.id);
        Get.snackbar('成功', '删除成功',
            backgroundColor: AppTheme.success, colorText: Colors.white);
        _loadData();
      } catch (e) {
        Get.snackbar('错误', '删除失败',
            backgroundColor: AppTheme.error, colorText: Colors.white);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('部门管理'),
        actions: [
          if (_auth.isAdmin)
            IconButton(
              icon: const Icon(Icons.add),
              onPressed: () => _handleCreate(),
            ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _departments.isEmpty
              ? const Center(child: Text('暂无部门数据'))
              : _buildDepartmentTree(),
    );
  }

  Widget _buildDepartmentTree() {
    return ListView.builder(
      padding: const EdgeInsets.all(12),
      itemCount: _departments.length,
      itemBuilder: (context, index) {
        return _buildDepartmentTile(_departments[index], 0);
      },
    );
  }

  Widget _buildDepartmentTile(Department department, int level) {
    final hasChildren = department.children?.isNotEmpty ?? false;
    final isSuperAdmin = _auth.isAdmin;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Card(
          margin: EdgeInsets.only(
            left: level * 16.0,
            bottom: 8,
          ),
          child: InkWell(
            onTap: hasChildren ? null : null,
            borderRadius: BorderRadius.circular(12),
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: Row(
                children: [
                  if (hasChildren)
                    ExpansionTile(
                      tilePadding: EdgeInsets.zero,
                      childrenPadding: EdgeInsets.zero,
                      title: Row(
                        children: [
                          Container(
                            width: 40,
                            height: 40,
                            decoration: BoxDecoration(
                              color: AppTheme.primary.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: const Icon(
                              Icons.business,
                              color: AppTheme.primary,
                              size: 22,
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Row(
                                  children: [
                                    Flexible(
                                      child: Text(
                                        department.name,
                                        style: const TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontSize: 15,
                                        ),
                                        overflow: TextOverflow.ellipsis,
                                      ),
                                    ),
                                    if (!department.isActive) ...[
                                      const SizedBox(width: 8),
                                      Container(
                                        padding: const EdgeInsets.symmetric(
                                            horizontal: 6, vertical: 2),
                                        decoration: BoxDecoration(
                                          color: AppTheme.error.withOpacity(0.1),
                                          borderRadius: BorderRadius.circular(4),
                                        ),
                                        child: const Text(
                                          '已禁用',
                                          style: TextStyle(
                                            fontSize: 10,
                                            color: AppTheme.error,
                                          ),
                                        ),
                                      ),
                                    ],
                                  ],
                                ),
                                const SizedBox(height: 4),
                                Row(
                                  children: [
                                    Container(
                                      padding: const EdgeInsets.symmetric(
                                          horizontal: 6, vertical: 2),
                                      decoration: BoxDecoration(
                                        color:
                                            AppTheme.textSecondary.withOpacity(0.1),
                                        borderRadius: BorderRadius.circular(4),
                                      ),
                                      child: Text(
                                        department.code,
                                        style: const TextStyle(
                                          fontSize: 11,
                                          color: AppTheme.textSecondary,
                                        ),
                                      ),
                                    ),
                                    if (department.manager != null) ...[
                                      const SizedBox(width: 8),
                                      const Icon(
                                        Icons.person,
                                        size: 14,
                                        color: AppTheme.textSecondary,
                                      ),
                                      const SizedBox(width: 4),
                                      Flexible(
                                        child: Text(
                                          department.manager is User
                                              ? (department.manager as User)
                                                  .fullName
                                              : department.manager.toString(),
                                          style: const TextStyle(
                                            fontSize: 12,
                                            color: AppTheme.textSecondary,
                                          ),
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                    ],
                                  ],
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                      trailing: isSuperAdmin
                          ? PopupMenuButton<String>(
                              icon: const Icon(Icons.more_vert, size: 20),
                              onSelected: (value) {
                                switch (value) {
                                  case 'add':
                                    _handleCreate(department);
                                    break;
                                  case 'edit':
                                    _handleEdit(department);
                                    break;
                                  case 'delete':
                                    _handleDelete(department);
                                    break;
                                }
                              },
                              itemBuilder: (context) => [
                                const PopupMenuItem(
                                  value: 'add',
                                  child: Row(
                                    children: [
                                      Icon(Icons.add, size: 18),
                                      SizedBox(width: 8),
                                      Text('添加子部门'),
                                    ],
                                  ),
                                ),
                                const PopupMenuItem(
                                  value: 'edit',
                                  child: Row(
                                    children: [
                                      Icon(Icons.edit, size: 18),
                                      SizedBox(width: 8),
                                      Text('编辑'),
                                    ],
                                  ),
                                ),
                                PopupMenuItem(
                                  value: 'delete',
                                  child: Row(
                                    children: [
                                      Icon(Icons.delete,
                                          size: 18, color: AppTheme.error),
                                      const SizedBox(width: 8),
                                      Text('删除',
                                          style:
                                              TextStyle(color: AppTheme.error)),
                                    ],
                                  ),
                                ),
                              ],
                            )
                          : null,
                      leading: const SizedBox.shrink(),
                      initiallyExpanded: level == 0,
                      children: (department.children ?? [])
                          .map((child) => _buildDepartmentTile(child, level + 1))
                          .toList(),
                    )
                  else
                    Expanded(
                      child: Row(
                        children: [
                          Container(
                            width: 40,
                            height: 40,
                            decoration: BoxDecoration(
                              color: AppTheme.primary.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: const Icon(
                              Icons.business,
                              color: AppTheme.primary,
                              size: 22,
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Row(
                                  children: [
                                    Flexible(
                                      child: Text(
                                        department.name,
                                        style: const TextStyle(
                                          fontWeight: FontWeight.bold,
                                          fontSize: 15,
                                        ),
                                        overflow: TextOverflow.ellipsis,
                                      ),
                                    ),
                                    if (!department.isActive) ...[
                                      const SizedBox(width: 8),
                                      Container(
                                        padding: const EdgeInsets.symmetric(
                                            horizontal: 6, vertical: 2),
                                        decoration: BoxDecoration(
                                          color: AppTheme.error.withOpacity(0.1),
                                          borderRadius: BorderRadius.circular(4),
                                        ),
                                        child: const Text(
                                          '已禁用',
                                          style: TextStyle(
                                            fontSize: 10,
                                            color: AppTheme.error,
                                          ),
                                        ),
                                      ),
                                    ],
                                  ],
                                ),
                                const SizedBox(height: 4),
                                Row(
                                  children: [
                                    Container(
                                      padding: const EdgeInsets.symmetric(
                                          horizontal: 6, vertical: 2),
                                      decoration: BoxDecoration(
                                        color:
                                            AppTheme.textSecondary.withOpacity(0.1),
                                        borderRadius: BorderRadius.circular(4),
                                      ),
                                      child: Text(
                                        department.code,
                                        style: const TextStyle(
                                          fontSize: 11,
                                          color: AppTheme.textSecondary,
                                        ),
                                      ),
                                    ),
                                    if (department.manager != null) ...[
                                      const SizedBox(width: 8),
                                      const Icon(
                                        Icons.person,
                                        size: 14,
                                        color: AppTheme.textSecondary,
                                      ),
                                      const SizedBox(width: 4),
                                      Flexible(
                                        child: Text(
                                          department.manager is User
                                              ? (department.manager as User)
                                                  .fullName
                                              : department.manager.toString(),
                                          style: const TextStyle(
                                            fontSize: 12,
                                            color: AppTheme.textSecondary,
                                          ),
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                    ],
                                  ],
                                ),
                              ],
                            ),
                          ),
                          if (isSuperAdmin)
                            PopupMenuButton<String>(
                              icon: const Icon(Icons.more_vert, size: 20),
                              onSelected: (value) {
                                switch (value) {
                                  case 'add':
                                    _handleCreate(department);
                                    break;
                                  case 'edit':
                                    _handleEdit(department);
                                    break;
                                  case 'delete':
                                    _handleDelete(department);
                                    break;
                                }
                              },
                              itemBuilder: (context) => [
                                const PopupMenuItem(
                                  value: 'add',
                                  child: Row(
                                    children: [
                                      Icon(Icons.add, size: 18),
                                      SizedBox(width: 8),
                                      Text('添加子部门'),
                                    ],
                                  ),
                                ),
                                const PopupMenuItem(
                                  value: 'edit',
                                  child: Row(
                                    children: [
                                      Icon(Icons.edit, size: 18),
                                      SizedBox(width: 8),
                                      Text('编辑'),
                                    ],
                                  ),
                                ),
                                PopupMenuItem(
                                  value: 'delete',
                                  child: Row(
                                    children: [
                                      Icon(Icons.delete,
                                          size: 18, color: AppTheme.error),
                                      const SizedBox(width: 8),
                                      Text('删除',
                                          style:
                                              TextStyle(color: AppTheme.error)),
                                    ],
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
          ),
        ),
      ],
    );
  }
}

class _DepartmentFormDialog extends StatefulWidget {
  final Department? department;
  final Department? parentDepartment;
  final List<Department> departments;

  const _DepartmentFormDialog({
    this.department,
    this.parentDepartment,
    required this.departments,
  });

  @override
  State<_DepartmentFormDialog> createState() => _DepartmentFormDialogState();
}

class _DepartmentFormDialogState extends State<_DepartmentFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _codeController;
  late TextEditingController _nameController;
  late TextEditingController _descriptionController;
  String? _selectedParentId;
  bool _isActive = true;
  bool _isLoading = false;

  bool get isEdit => widget.department != null;

  @override
  void initState() {
    super.initState();
    _codeController = TextEditingController(text: widget.department?.code);
    _nameController = TextEditingController(text: widget.department?.name);
    _descriptionController =
        TextEditingController(text: widget.department?.description);
    _selectedParentId = widget.parentDepartment?.id ?? widget.department?.parentId;
    _isActive = widget.department?.isActive ?? true;
  }

  @override
  void dispose() {
    _codeController.dispose();
    _nameController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _handleSubmit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);
    try {
      final deptService = Get.find<DepartmentService>();
      if (isEdit) {
        await deptService.updateDepartment(
          widget.department!.id,
          {
            'name': _nameController.text,
            'description': _descriptionController.text.isEmpty
                ? null
                : _descriptionController.text,
            'isActive': _isActive,
          },
        );
        Get.snackbar('成功', '更新成功',
            backgroundColor: AppTheme.success, colorText: Colors.white);
      } else {
        await deptService.createDepartment({
          'code': _codeController.text,
          'name': _nameController.text,
          'parentId': _selectedParentId,
          'description': _descriptionController.text.isEmpty
              ? null
              : _descriptionController.text,
        });
        Get.snackbar('成功', '创建成功',
            backgroundColor: AppTheme.success, colorText: Colors.white);
      }
      if (mounted) Navigator.pop(context, true);
    } catch (e) {
      Get.snackbar('错误', isEdit ? '更新失败' : '创建失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  List<DropdownMenuItem<String>> _buildDepartmentOptions() {
    final items = <DropdownMenuItem<String>>[
      const DropdownMenuItem(value: null, child: Text('无（顶级部门）')),
    ];

    void addDepts(List<Department> depts, int level) {
      for (var dept in depts) {
        if (isEdit && dept.id == widget.department!.id) continue;
        items.add(DropdownMenuItem(
          value: dept.id,
          child: Text('${'  ' * level}${dept.name}'),
        ));
        if (dept.children != null) {
          addDepts(dept.children!, level + 1);
        }
      }
    }

    addDepts(widget.departments, 0);
    return items;
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(isEdit
          ? '编辑部门'
          : (widget.parentDepartment != null
              ? '添加子部门'
              : '新增部门')),
      content: SizedBox(
        width: 400,
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextFormField(
                  controller: _codeController,
                  decoration: InputDecoration(
                    labelText: '部门代码 *',
                    hintText: '如: DEPT001',
                  ),
                  enabled: !isEdit,
                  validator: (v) =>
                      v?.isEmpty == true ? '请输入部门代码' : null,
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _nameController,
                  decoration: const InputDecoration(labelText: '部门名称 *'),
                  validator: (v) =>
                      v?.isEmpty == true ? '请输入部门名称' : null,
                ),
                const SizedBox(height: 16),
                DropdownButtonFormField<String?>(
                  value: _selectedParentId,
                  decoration: InputDecoration(
                    labelText: '上级部门',
                    hintText: _selectedParentId == null
                        ? '无（顶级部门）'
                        : null,
                  ),
                  items: _buildDepartmentOptions(),
                  onChanged: (val) {
                    setState(() => _selectedParentId = val);
                  },
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _descriptionController,
                  decoration: const InputDecoration(labelText: '描述'),
                  maxLines: 3,
                ),
                if (isEdit) ...[
                  const SizedBox(height: 16),
                  SwitchListTile(
                    title: const Text('启用状态'),
                    value: _isActive,
                    onChanged: (val) => setState(() => _isActive = val),
                    contentPadding: EdgeInsets.zero,
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: _isLoading ? null : () => Navigator.pop(context),
          child: const Text('取消'),
        ),
        ElevatedButton(
          onPressed: _isLoading ? null : _handleSubmit,
          child: _isLoading
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : Text(isEdit ? '保存' : '创建'),
        ),
      ],
    );
  }
}
