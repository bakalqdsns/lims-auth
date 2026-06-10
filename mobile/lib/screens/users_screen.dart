import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';

class UsersScreen extends StatefulWidget {
  const UsersScreen({super.key});

  @override
  State<UsersScreen> createState() => _UsersScreenState();
}

class _UsersScreenState extends State<UsersScreen> {
  final _auth = Get.find<AuthController>();
  final _userService = Get.find<UserService>();
  final _roleService = Get.find<RoleService>();
  final _departmentService = Get.find<DepartmentService>();

  bool _isLoading = false;
  List<User> _users = [];
  List<Role> _roles = [];
  List<Department> _departments = [];
  int _currentPage = 1;
  int _pageSize = 20;
  int _total = 0;

  final _keywordController = TextEditingController();
  String? _selectedDepartmentId;
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
      final results = await Future.wait([
        _userService.getUsers(
          keyword: _keywordController.text.isEmpty ? null : _keywordController.text,
          departmentId: _selectedDepartmentId,
          isActive: _selectedIsActive,
          page: _currentPage,
          pageSize: _pageSize,
        ),
        _roleService.getAllRoles(),
        _departmentService.getDepartments(tree: false),
      ]);
      setState(() {
        _users = results[0] as List<User>;
        _roles = results[1] as List<Role>;
        _departments = results[2] as List<Department>;
        _total = _users.length;
      });
    } catch (e) {
      Get.snackbar('错误', '获取数据失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    } finally {
      setState(() => _isLoading = false);
    }
  }

  bool _isSuperAdminRow(User user) {
    return user.roles.any((r) => r == 'super_admin');
  }

  Future<void> _handleCreate() async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => _UserFormDialog(
        roles: _roles,
        departments: _departments,
      ),
    );
    if (result == true) {
      _loadData();
    }
  }

  Future<void> _handleEdit(User user) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => _UserFormDialog(
        user: user,
        roles: _roles,
        departments: _departments,
      ),
    );
    if (result == true) {
      _loadData();
    }
  }

  Future<void> _handleView(User user) async {
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('用户详情'),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              _detailRow('用户名', user.username),
              _detailRow('姓名', user.fullName),
              _detailRow('邮箱', user.email),
              _detailRow('电话', user.phone),
              _detailRow('部门', user.department?.name ?? '-'),
              _detailRow('工号', user.employeeId ?? '-'),
              _detailRow('学号', user.studentId ?? '-'),
              _detailRow('状态', user.isActive ? '启用' : '禁用'),
              _detailRow('创建时间', user.createdAt ?? '-'),
              const SizedBox(height: 12),
              const Text('角色:', style: TextStyle(fontWeight: FontWeight.bold)),
              const SizedBox(height: 8),
              Wrap(
                spacing: 6,
                runSpacing: 6,
                children: user.userRoles
                        ?.map((r) => Chip(
                              label: Text(r.roleName ?? r.roleCode ?? ''),
                              backgroundColor: RoleColors.getColor(r.roleCode)
                                  .withOpacity(0.2),
                              labelStyle: TextStyle(
                                  color: RoleColors.getColor(r.roleCode)),
                            ))
                        .toList() ??
                    [],
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

  Future<void> _handleAssignRoles(User user) async {
    final selectedIds = <String>{};
    for (var ur in user.userRoles ?? []) {
      if (ur.roleId != null) selectedIds.add(ur.roleId!);
    }

    final result = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Text('分配角色 - ${user.username}'),
          content: SizedBox(
            width: 300,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: _roles.map((role) {
                return CheckboxListTile(
                  title: Text(role.name),
                  subtitle: Text(role.code),
                  value: selectedIds.contains(role.id),
                  onChanged: (val) {
                    setDialogState(() {
                      if (val == true) {
                        selectedIds.add(role.id);
                      } else {
                        selectedIds.remove(role.id);
                      }
                    });
                  },
                );
              }).toList(),
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
                  await _userService.updateUserRoles(user.id, selectedIds.toList());
                  Get.snackbar('成功', '分配角色成功',
                      backgroundColor: AppTheme.success, colorText: Colors.white);
                  Navigator.pop(context, true);
                } catch (e) {
                  Get.snackbar('错误', '分配角色失败',
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

  Future<void> _handleResetPassword(User user) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('重置密码'),
        content: Text('确定要重置用户 ${user.username} 的密码吗？'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('取消'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('确定'),
          ),
        ],
      ),
    );

    if (confirm == true) {
      try {
        await _userService.resetPassword(user.id);
        Get.snackbar('成功', '密码已重置为默认密码',
            backgroundColor: AppTheme.success, colorText: Colors.white);
      } catch (e) {
        Get.snackbar('错误', '重置密码失败',
            backgroundColor: AppTheme.error, colorText: Colors.white);
      }
    }
  }

  Future<void> _handleDelete(User user) async {
    try {
      await _userService.deleteUser(user.id);
      Get.snackbar('成功', '删除成功',
          backgroundColor: AppTheme.success, colorText: Colors.white);
      _loadData();
    } catch (e) {
      Get.snackbar('错误', '删除失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
    }
  }

  Future<void> _handleStatusChange(User user, bool isActive) async {
    try {
      await _userService.updateUserStatus(user.id, isActive);
      Get.snackbar('成功', isActive ? '用户已启用' : '用户已禁用',
          backgroundColor: AppTheme.success, colorText: Colors.white);
    } catch (e) {
      Get.snackbar('错误', '操作失败',
          backgroundColor: AppTheme.error, colorText: Colors.white);
      _loadData();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('用户管理'),
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
                : _users.isEmpty
                    ? const Center(child: Text('暂无数据'))
                    : _buildUserList(),
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
                hintText: '搜索用户名/姓名/邮箱',
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
                  child: DropdownButtonFormField<String?>(
                    value: _selectedDepartmentId,
                    decoration: const InputDecoration(
                      hintText: '选择部门',
                      contentPadding:
                          EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                    ),
                    items: [
                      const DropdownMenuItem(value: null, child: Text('全部部门')),
                      ..._departments.map((d) => DropdownMenuItem(
                            value: d.id,
                            child: Text(d.name),
                          )),
                    ],
                    onChanged: (val) {
                      setState(() => _selectedDepartmentId = val);
                    },
                  ),
                ),
                const SizedBox(width: 12),
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
      _selectedDepartmentId = null;
      _selectedIsActive = null;
      _currentPage = 1;
    });
    _loadData();
  }

  Widget _buildUserList() {
    return ListView.builder(
      padding: const EdgeInsets.symmetric(horizontal: 12),
      itemCount: _users.length,
      itemBuilder: (context, index) {
        final user = _users[index];
        final isSuperAdmin = _isSuperAdminRow(user);
        return Card(
          margin: const EdgeInsets.only(bottom: 12),
          child: Padding(
            padding: const EdgeInsets.all(12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    CircleAvatar(
                      backgroundColor: AppTheme.primary.withOpacity(0.1),
                      child: Text(
                        user.fullName.isNotEmpty
                            ? user.fullName[0].toUpperCase()
                            : 'U',
                        style: const TextStyle(
                            color: AppTheme.primary,
                            fontWeight: FontWeight.bold),
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
                                user.fullName,
                                style: const TextStyle(
                                    fontWeight: FontWeight.bold, fontSize: 16),
                              ),
                              if (isSuperAdmin) ...[
                                const SizedBox(width: 8),
                                Container(
                                  padding: const EdgeInsets.symmetric(
                                      horizontal: 6, vertical: 2),
                                  decoration: BoxDecoration(
                                    color: AppTheme.error.withOpacity(0.1),
                                    borderRadius: BorderRadius.circular(4),
                                  ),
                                  child: const Text(
                                    '超级管理员',
                                    style: TextStyle(
                                        fontSize: 10, color: AppTheme.error),
                                  ),
                                ),
                              ],
                            ],
                          ),
                          Text(
                            '@${user.username}',
                            style: const TextStyle(
                                color: AppTheme.textSecondary, fontSize: 13),
                          ),
                        ],
                      ),
                    ),
                    if (_auth.isAdmin)
                      Switch(
                        value: user.isActive,
                        onChanged: isSuperAdmin
                            ? null
                            : (val) => _handleStatusChange(user, val),
                        activeColor: AppTheme.success,
                      ),
                  ],
                ),
                const Divider(height: 24),
                _infoRow(Icons.email, user.email),
                _infoRow(Icons.phone, user.phone),
                _infoRow(
                    Icons.business,
                    user.department?.name ?? '-'),
                const SizedBox(height: 8),
                const Text('角色:',
                    style: TextStyle(
                        fontWeight: FontWeight.w500,
                        color: AppTheme.textSecondary)),
                const SizedBox(height: 6),
                Wrap(
                  spacing: 6,
                  runSpacing: 6,
                  children: (user.userRoles ?? [])
                      .map((ur) => Chip(
                            avatar: CircleAvatar(
                              backgroundColor:
                                  RoleColors.getColor(ur.roleCode),
                              radius: 10,
                              child: Text(
                                (ur.roleName ?? ur.roleCode ?? '')[0]
                                    .toUpperCase(),
                                style: const TextStyle(
                                    fontSize: 10, color: Colors.white),
                              ),
                            ),
                            label: Text(ur.roleName ?? ur.roleCode ?? '-'),
                            backgroundColor: RoleColors.getColor(ur.roleCode)
                                .withOpacity(0.1),
                            labelStyle: TextStyle(
                                color: RoleColors.getColor(ur.roleCode)),
                          ))
                      .toList(),
                ),
                if (_auth.isAdmin && !isSuperAdmin) ...[
                  const SizedBox(height: 12),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      TextButton.icon(
                        icon: const Icon(Icons.visibility, size: 18),
                        label: const Text('查看'),
                        onPressed: () => _handleView(user),
                      ),
                      TextButton.icon(
                        icon: const Icon(Icons.edit, size: 18),
                        label: const Text('编辑'),
                        onPressed: () => _handleEdit(user),
                      ),
                      TextButton.icon(
                        icon: const Icon(Icons.admin_panel_settings, size: 18),
                        label: const Text('角色'),
                        onPressed: () => _handleAssignRoles(user),
                      ),
                      TextButton.icon(
                        icon: const Icon(Icons.key, size: 18),
                        label: const Text('重置密码'),
                        onPressed: () => _handleResetPassword(user),
                      ),
                      TextButton.icon(
                        icon: const Icon(Icons.delete, size: 18),
                        label: const Text('删除'),
                        style: TextButton.styleFrom(
                            foregroundColor: AppTheme.error),
                        onPressed: () => _handleDelete(user),
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

  Widget _infoRow(IconData icon, String text) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        children: [
          Icon(icon, size: 16, color: AppTheme.textSecondary),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              text,
              style: const TextStyle(color: AppTheme.textSecondary),
            ),
          ),
        ],
      ),
    );
  }
}

class _UserFormDialog extends StatefulWidget {
  final User? user;
  final List<Role> roles;
  final List<Department> departments;

  const _UserFormDialog({
    this.user,
    required this.roles,
    required this.departments,
  });

  @override
  State<_UserFormDialog> createState() => _UserFormDialogState();
}

class _UserFormDialogState extends State<_UserFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _usernameController;
  late TextEditingController _passwordController;
  late TextEditingController _emailController;
  late TextEditingController _phoneController;
  late TextEditingController _fullNameController;
  String? _selectedDepartmentId;
  List<String> _selectedRoleIds = [];
  bool _isActive = true;
  bool _isLoading = false;

  bool get isEdit => widget.user != null;

  @override
  void initState() {
    super.initState();
    _usernameController = TextEditingController(text: widget.user?.username);
    _passwordController = TextEditingController();
    _emailController = TextEditingController(text: widget.user?.email);
    _phoneController = TextEditingController(text: widget.user?.phone);
    _fullNameController = TextEditingController(text: widget.user?.fullName);
    _selectedDepartmentId = widget.user?.departmentId;
    _isActive = widget.user?.isActive ?? true;
    _selectedRoleIds = widget.user?.userRoles
            ?.map((ur) => ur.roleId)
            .where((id) => id != null)
            .cast<String>()
            .toList() ??
        [];
  }

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    _fullNameController.dispose();
    super.dispose();
  }

  Future<void> _handleSubmit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);
    try {
      final userService = Get.find<UserService>();
      if (isEdit) {
        await userService.updateUser(
          widget.user!.id,
          UpdateUserRequest(
            email: _emailController.text,
            phone: _phoneController.text,
            fullName: _fullNameController.text,
            departmentId: _selectedDepartmentId,
            isActive: _isActive,
          ),
        );
        if (_selectedRoleIds.isNotEmpty) {
          await userService.updateUserRoles(widget.user!.id, _selectedRoleIds);
        }
        Get.snackbar('成功', '更新成功',
            backgroundColor: AppTheme.success, colorText: Colors.white);
      } else {
        await userService.createUser(
          CreateUserRequest(
            username: _usernameController.text,
            password: _passwordController.text,
            email: _emailController.text,
            phone: _phoneController.text,
            fullName: _fullNameController.text,
            departmentId: _selectedDepartmentId,
            roleIds: _selectedRoleIds,
            isActive: _isActive,
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
      title: Text(isEdit ? '编辑用户' : '新增用户'),
      content: SizedBox(
        width: 400,
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (!isEdit)
                  TextFormField(
                    controller: _usernameController,
                    decoration: const InputDecoration(labelText: '用户名 *'),
                    validator: (v) =>
                        v?.isEmpty == true ? '请输入用户名' : null,
                  ),
                if (!isEdit) const SizedBox(height: 16),
                if (!isEdit)
                  TextFormField(
                    controller: _passwordController,
                    decoration: InputDecoration(
                        labelText: isEdit ? '新密码' : '密码 *'),
                    obscureText: true,
                    validator: isEdit
                        ? null
                        : (v) => v?.isEmpty == true ? '请输入密码' : null,
                  ),
                if (!isEdit) const SizedBox(height: 16),
                TextFormField(
                  controller: _fullNameController,
                  decoration: const InputDecoration(labelText: '姓名 *'),
                  validator: (v) =>
                      v?.isEmpty == true ? '请输入姓名' : null,
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _emailController,
                  decoration: const InputDecoration(labelText: '邮箱 *'),
                  keyboardType: TextInputType.emailAddress,
                  validator: (v) {
                    if (v?.isEmpty == true) return '请输入邮箱';
                    if (!GetUtils.isEmail(v!)) return '请输入有效邮箱';
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _phoneController,
                  decoration: const InputDecoration(labelText: '电话 *'),
                  keyboardType: TextInputType.phone,
                  validator: (v) =>
                      v?.isEmpty == true ? '请输入电话' : null,
                ),
                const SizedBox(height: 16),
                DropdownButtonFormField<String?>(
                  value: _selectedDepartmentId,
                  decoration: const InputDecoration(labelText: '部门'),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('请选择')),
                    ...widget.departments.map((d) => DropdownMenuItem(
                          value: d.id,
                          child: Text(d.name),
                        )),
                  ],
                  onChanged: (val) {
                    setState(() => _selectedDepartmentId = val);
                  },
                ),
                const SizedBox(height: 16),
                const Align(
                  alignment: Alignment.centerLeft,
                  child: Text('角色',
                      style: TextStyle(
                          fontSize: 12,
                          color: AppTheme.textSecondary)),
                ),
                const SizedBox(height: 8),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: widget.roles.map((role) {
                    final selected = _selectedRoleIds.contains(role.id);
                    return FilterChip(
                      label: Text(role.name),
                      selected: selected,
                      onSelected: (val) {
                        setState(() {
                          if (val) {
                            _selectedRoleIds.add(role.id);
                          } else {
                            _selectedRoleIds.remove(role.id);
                          }
                        });
                      },
                    );
                  }).toList(),
                ),
                const SizedBox(height: 16),
                SwitchListTile(
                  title: const Text('启用状态'),
                  value: _isActive,
                  onChanged: (val) => setState(() => _isActive = val),
                  contentPadding: EdgeInsets.zero,
                ),
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
