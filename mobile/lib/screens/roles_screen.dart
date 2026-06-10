import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class RolesScreen extends StatefulWidget {
  const RolesScreen({super.key});

  @override
  State<RolesScreen> createState() => _RolesScreenState();
}

class _RolesScreenState extends State<RolesScreen> {
  final _auth = Get.find<AuthController>();
  final _roleService = Get.find<RoleService>();

  bool _isLoading = false;
  List<Role> _roles = [];
  int _currentPage = 1;
  int _pageSize = 20;

  final _keywordController = TextEditingController();
  bool? _selectedIsActive;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  @override
  void dispose() {
    _keywordController.dispose();
    super.dispose();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    try {
      final roles = await _roleService.getRoles();
      setState(() {
        _roles = roles;
      });
    } catch (e) {
      Get.snackbar('错误', '获取角色列表失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _handleCreate() async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => _RoleFormDialog(),
    );
    if (result == true) {
      _loadData();
    }
  }

  Future<void> _handleEdit(Role role) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => _RoleFormDialog(role: role),
    );
    if (result == true) {
      _loadData();
    }
  }

  Future<void> _handleView(Role role) async {
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('角色详情 - ${role.name}'),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              _detailRow('角色编码', role.code),
              _detailRow('角色名称', role.name),
              _detailRow('描述', role.description ?? '-'),
              _detailRow('类型', role.isSystem ? '系统角色' : '自定义角色'),
              _detailRow('状态', role.isActive ? '启用' : '禁用'),
              _detailRow('创建时间', role.createdAt ?? '-'),
              const SizedBox(height: 16),
              const Text('权限:',
                  style: TextStyle(fontWeight: FontWeight.bold)),
              const SizedBox(height: 8),
              if (role.rolePermissions?.isEmpty ?? true)
                const Text('暂无权限', style: TextStyle(color: AppTheme.textSecondary))
              else
                Wrap(
                  spacing: 6,
                  runSpacing: 6,
                  children: role.rolePermissions!
                      .where((rp) => rp.permission != null)
                      .map((rp) => Chip(
                            label: Text(rp.permission!.name),
                            backgroundColor: AppTheme.primary.withOpacity(0.1),
                            labelStyle: const TextStyle(color: AppTheme.primary),
                          ))
                      .toList(),
                ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
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
            child: Text(label,
                style: const TextStyle(fontWeight: FontWeight.bold)),
          ),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }

  Future<void> _handleAssignPermissions(Role role) async {
    final allPermissions = await _roleService.getRoleById(role.id);
    final selectedIds = <String>{};
    for (var rp in role.rolePermissions ?? []) {
      if (rp.permissionId != null) selectedIds.add(rp.permissionId!);
    }

    final result = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Text('分配权限 - ${role.name}'),
          content: SizedBox(
            width: 350,
            child: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('当前权限列表:',
                      style: TextStyle(fontWeight: FontWeight.w500)),
                  const SizedBox(height: 8),
                  if (allPermissions?.rolePermissions?.isEmpty ?? true)
                    const Text('暂无权限',
                        style: TextStyle(color: AppTheme.textSecondary))
                  else
                    Wrap(
                      spacing: 6,
                      runSpacing: 6,
                      children: allPermissions!.rolePermissions!
                          .where((rp) => rp.permission != null)
                          .map((rp) => Chip(
                                label: Text(rp.permission!.name),
                                backgroundColor: AppTheme.success.withOpacity(0.1),
                                labelStyle:
                                    const TextStyle(color: AppTheme.success),
                                onDeleted: () {
                                  setDialogState(() {
                                    if (rp.permissionId != null) {
                                      selectedIds.remove(rp.permissionId);
                                    }
                                  });
                                },
                              ))
                          .toList(),
                    ),
                  const Divider(height: 32),
                  const Text('添加权限:',
                      style: TextStyle(fontWeight: FontWeight.w500)),
                  const SizedBox(height: 8),
                  const Text(
                    '可用的权限列表 (简化展示，实际应用中应从权限服务获取完整列表)',
                    style: TextStyle(
                        color: AppTheme.textSecondary, fontSize: 12),
                  ),
                  const SizedBox(height: 8),
                  ..._buildPermissionCheckboxes(role, selectedIds, setDialogState),
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
              onPressed: () async {
                try {
                  await _roleService.updateRolePermissions(role.id, selectedIds.toList());
                  Get.snackbar('成功', '分配权限成功',
                      backgroundColor: AppTheme.success, colorText: Colors.white);
                  Navigator.pop(context, true);
                } catch (e) {
                  Get.snackbar('错误', '分配权限失败',
                      backgroundColor: AppTheme.error, colorText: Colors.white);
                }
              },
              child: const Text('确定'),
            ),
          ],
        ),
      ),
    );
    if (result == true) {
      _loadData();
    }
  }

  List<Widget> _buildPermissionCheckboxes(
      Role role, Set<String> selectedIds, StateSetter setDialogState) {
    final allPerms = role.rolePermissions
            ?.where((rp) => rp.permission != null)
            .map((rp) => rp.permission!)
            .toList() ??
        [];

    if (allPerms.isEmpty) {
      return [
        const Text('暂无权限可分配', style: TextStyle(color: AppTheme.textSecondary)),
      ];
    }

    final modules = <String, List<Permission>>{};
    for (var p in allPerms) {
      modules.putIfAbsent(p.module, () => []).add(p);
    }

    return modules.entries.expand((entry) {
      return [
        Padding(
          padding: const EdgeInsets.only(top: 8, bottom: 4),
          child: Text(entry.key,
              style: const TextStyle(
                  fontWeight: FontWeight.bold, color: AppTheme.primary)),
        ),
        ...entry.value.map((p) => CheckboxListTile(
              title: Text(p.name),
              subtitle: Text(p.code, style: const TextStyle(fontSize: 11)),
              value: selectedIds.contains(p.id),
              onChanged: (val) {
                setDialogState(() {
                  if (val == true) {
                    selectedIds.add(p.id);
                  } else {
                    selectedIds.remove(p.id);
                  }
                });
              },
              dense: true,
            )),
      ];
    }).toList();
  }

  Future<void> _handleDelete(Role role) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('删除角色'),
        content: Text('确定要删除角色 "${role.name}" 吗？'),
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
        await _roleService.deleteRole(role.id);
        Get.snackbar('成功', '删除成功',
            backgroundColor: AppTheme.success, colorText: Colors.white);
        _loadData();
      } catch (e) {
        Get.snackbar('错误', '删除失败',
            backgroundColor: AppTheme.error, colorText: Colors.white);
      }
    }
  }

  Future<void> _handleStatusChange(Role role, bool isActive) async {
    try {
      await _roleService.updateRole(
        role.id,
        UpdateRoleRequest(
          name: role.name,
          description: role.description,
          isActive: isActive,
        ),
      );
      Get.snackbar('成功', isActive ? '角色已启用' : '角色已禁用',
          backgroundColor: AppTheme.success, colorText: Colors.white);
      _loadData();
    } catch (e) {
      Get.snackbar('错误', '操作失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('角色管理'),
        actions: [
          if (_auth.isAdmin)
            IconButton(
              icon: const Icon(Icons.add),
              onPressed: _handleCreate,
            ),
        ],
      ),
      body: Column(
        children: [
          _buildSearchBar(),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _roles.isEmpty
                    ? const Center(child: Text('暂无数据'))
                    : _buildRoleList(),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Card(
      margin: const EdgeInsets.all(12),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          children: [
            TextField(
              controller: _keywordController,
              decoration: InputDecoration(
                hintText: '搜索角色编码/名称',
                prefixIcon: const Icon(Icons.search),
                suffixIcon: IconButton(
                  icon: const Icon(Icons.clear),
                  onPressed: () {
                    _keywordController.clear();
                  },
                ),
              ),
              onSubmitted: (_) => _handleSearch(),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: DropdownButtonFormField<bool?>(
                    value: _selectedIsActive,
                    decoration: const InputDecoration(
                      hintText: '选择状态',
                      contentPadding:
                          EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                    ),
                    items: const [
                      DropdownMenuItem(value: null, child: Text('全部状态')),
                      DropdownMenuItem(value: true, child: Text('启用')),
                      DropdownMenuItem(value: false, child: Text('禁用')),
                    ],
                    onChanged: (val) {
                      setState(() => _selectedIsActive = val);
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: ElevatedButton(
                    onPressed: _handleSearch,
                    child: const Text('搜索'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: OutlinedButton(
                    onPressed: _handleReset,
                    child: const Text('重置'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  void _handleSearch() {
    _currentPage = 1;
    _loadData();
  }

  void _handleReset() {
    _keywordController.clear();
    setState(() {
      _selectedIsActive = null;
      _currentPage = 1;
    });
    _loadData();
  }

  Widget _buildRoleList() {
    var filteredRoles = _roles.where((role) {
      if (_keywordController.text.isNotEmpty) {
        final kw = _keywordController.text.toLowerCase();
        if (!role.code.toLowerCase().contains(kw) &&
            !role.name.toLowerCase().contains(kw)) {
          return false;
        }
      }
      if (_selectedIsActive != null && role.isActive != _selectedIsActive) {
        return false;
      }
      return true;
    }).toList();

    return ListView.builder(
      padding: const EdgeInsets.symmetric(horizontal: 12),
      itemCount: filteredRoles.length,
      itemBuilder: (context, index) {
        final role = filteredRoles[index];
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
                      width: 48,
                      height: 48,
                      decoration: BoxDecoration(
                        color: role.isSystem
                            ? AppTheme.error.withOpacity(0.1)
                            : AppTheme.textSecondary.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Icon(
                        Icons.admin_panel_settings,
                        color: role.isSystem
                            ? AppTheme.error
                            : AppTheme.textSecondary,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Text(
                                role.name,
                                style: const TextStyle(
                                    fontWeight: FontWeight.bold, fontSize: 16),
                              ),
                              const SizedBox(width: 8),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 6, vertical: 2),
                                decoration: BoxDecoration(
                                  color: role.isSystem
                                      ? AppTheme.error.withOpacity(0.1)
                                      : AppTheme.textSecondary.withOpacity(0.1),
                                  borderRadius: BorderRadius.circular(4),
                                ),
                                child: Text(
                                  role.isSystem ? '系统' : '自定义',
                                  style: TextStyle(
                                    fontSize: 11,
                                    color: role.isSystem
                                        ? AppTheme.error
                                        : AppTheme.textSecondary,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 4),
                          Text(
                            role.code,
                            style: const TextStyle(
                                color: AppTheme.textSecondary, fontSize: 13),
                          ),
                        ],
                      ),
                    ),
                    if (_auth.isAdmin && !role.isSystem)
                      Switch(
                        value: role.isActive,
                        onChanged: (val) => _handleStatusChange(role, val),
                        activeColor: AppTheme.success,
                      )
                    else if (role.isSystem)
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 8, vertical: 4),
                        decoration: BoxDecoration(
                          color: AppTheme.textSecondary.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Text(
                          role.isActive ? '启用' : '禁用',
                          style: const TextStyle(
                              fontSize: 12,
                              color: AppTheme.textSecondary),
                        ),
                      ),
                  ],
                ),
                if (role.description != null && role.description!.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    role.description!,
                    style: const TextStyle(color: AppTheme.textSecondary),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
                const SizedBox(height: 12),
                Row(
                  children: [
                    const Icon(Icons.people,
                        size: 16, color: AppTheme.textSecondary),
                    const SizedBox(width: 4),
                    Text(
                      '${role.rolePermissions?.length ?? 0} 个权限',
                      style: const TextStyle(
                          color: AppTheme.textSecondary, fontSize: 13),
                    ),
                    const SizedBox(width: 16),
                    const Icon(Icons.access_time,
                        size: 16, color: AppTheme.textSecondary),
                    const SizedBox(width: 4),
                    Text(
                      role.createdAt?.substring(0, 10) ?? '-',
                      style: const TextStyle(
                          color: AppTheme.textSecondary, fontSize: 13),
                    ),
                  ],
                ),
                if (_auth.isAdmin) ...[
                  const SizedBox(height: 12),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      TextButton.icon(
                        icon: const Icon(Icons.visibility, size: 18),
                        label: const Text('详情'),
                        onPressed: () => _handleView(role),
                      ),
                      TextButton.icon(
                        icon: const Icon(Icons.edit, size: 18),
                        label: const Text('编辑'),
                        onPressed: () => _handleEdit(role),
                      ),
                      TextButton.icon(
                        icon: const Icon(Icons.security, size: 18),
                        label: const Text('权限'),
                        onPressed: () => _handleAssignPermissions(role),
                      ),
                      if (!role.isSystem)
                        TextButton.icon(
                          icon: const Icon(Icons.delete, size: 18),
                          label: const Text('删除'),
                          style: TextButton.styleFrom(
                              foregroundColor: AppTheme.error),
                          onPressed: () => _handleDelete(role),
                        ),
                    ],
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }
}

class _RoleFormDialog extends StatefulWidget {
  final Role? role;

  const _RoleFormDialog({this.role});

  @override
  State<_RoleFormDialog> createState() => _RoleFormDialogState();
}

class _RoleFormDialogState extends State<_RoleFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _codeController;
  late TextEditingController _nameController;
  late TextEditingController _descriptionController;
  bool _isActive = true;
  bool _isLoading = false;

  bool get isEdit => widget.role != null;

  @override
  void initState() {
    super.initState();
    _codeController = TextEditingController(text: widget.role?.code);
    _nameController = TextEditingController(text: widget.role?.name);
    _descriptionController =
        TextEditingController(text: widget.role?.description);
    _isActive = widget.role?.isActive ?? true;
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
      final roleService = Get.find<RoleService>();
      if (isEdit) {
        await roleService.updateRole(
          widget.role!.id,
          UpdateRoleRequest(
            name: _nameController.text,
            description: _descriptionController.text.isEmpty
                ? null
                : _descriptionController.text,
            isActive: _isActive,
          ),
        );
        Get.snackbar('成功', '更新成功',
            backgroundColor: AppTheme.success, colorText: Colors.white);
      } else {
        await roleService.createRole(
          CreateRoleRequest(
            code: _codeController.text,
            name: _nameController.text,
            description: _descriptionController.text.isEmpty
                ? null
                : _descriptionController.text,
          ),
        );
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

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(isEdit ? '编辑角色' : '新增角色'),
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
                    labelText: '角色编码 *',
                    hintText: '如: admin, teacher',
                  ),
                  enabled: !isEdit,
                  validator: (v) =>
                      v?.isEmpty == true ? '请输入角色编码' : null,
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _nameController,
                  decoration: const InputDecoration(labelText: '角色名称 *'),
                  validator: (v) =>
                      v?.isEmpty == true ? '请输入角色名称' : null,
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
