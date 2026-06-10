import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../widgets/widgets.dart';
import '../utils/theme/app_theme.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final auth = Get.find<AuthController>();
    final user = auth.currentUser.value;

    return Scaffold(
      appBar: AppBar(title: const Text('个人中心')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          // 头像卡片
          Card(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                children: [
                  CircleAvatar(
                    radius: 48,
                    backgroundColor: AppTheme.primary.withOpacity(0.1),
                    child: Text(
                      (user?.fullName.isNotEmpty == true ? user!.fullName[0] : '?').toUpperCase(),
                      style: const TextStyle(fontSize: 32, color: AppTheme.primary, fontWeight: FontWeight.bold),
                    ),
                  ),
                  const SizedBox(height: 12),
                  Text(
                    user?.fullName ?? '未知用户',
                    style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    user?.email ?? '',
                    style: const TextStyle(color: AppTheme.textSecondary),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    children: (user?.roles ?? []).map((r) => Chip(
                      label: Text(r),
                      backgroundColor: RoleColors.getColor(r).withOpacity(0.15),
                      labelStyle: TextStyle(color: RoleColors.getColor(r), fontSize: 12),
                    )).toList(),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          // 功能列表
          _ProfileMenuItem(
            icon: Icons.person_outline,
            title: '编辑资料',
            onTap: () {},
          ),
          _ProfileMenuItem(
            icon: Icons.lock_outline,
            title: '修改密码',
            onTap: () => _showChangePassword(context),
          ),
          _ProfileMenuItem(
            icon: Icons.settings_outlined,
            title: '设置',
            onTap: () {},
          ),
          _ProfileMenuItem(
            icon: Icons.info_outline,
            title: '关于',
            onTap: () {},
          ),
          _ProfileMenuItem(
            icon: Icons.logout,
            title: '退出登录',
            isDanger: true,
            onTap: () => _logout(context),
          ),
        ],
      ),
    );
  }

  void _showChangePassword(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (_) => const _ChangePasswordSheet(),
    );
  }

  void _logout(BuildContext context) async {
    final ok = await ConfirmDialog.show(
      context,
      title: '退出登录',
      message: '确定要退出登录吗？',
      confirmLabel: '退出',
      isDanger: true,
    );
    if (ok) {
      await Get.find<AuthController>().logout();
      Get.offAllNamed('/');
    }
  }
}

class _ProfileMenuItem extends StatelessWidget {
  final IconData icon;
  final String title;
  final VoidCallback onTap;
  final bool isDanger;

  const _ProfileMenuItem({
    required this.icon,
    required this.title,
    required this.onTap,
    this.isDanger = false,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: Icon(icon, color: isDanger ? AppTheme.error : AppTheme.primary),
        title: Text(
          title,
          style: TextStyle(color: isDanger ? AppTheme.error : null),
        ),
        trailing: const Icon(Icons.chevron_right, color: AppTheme.textHint),
        onTap: onTap,
      ),
    );
  }
}

class _ChangePasswordSheet extends StatefulWidget {
  const _ChangePasswordSheet();

  @override
  State<_ChangePasswordSheet> createState() => _ChangePasswordSheetState();
}

class _ChangePasswordSheetState extends State<_ChangePasswordSheet> {
  final _oldPwd = TextEditingController();
  final _newPwd = TextEditingController();
  final _confirmPwd = TextEditingController();
  bool _isLoading = false;
  String? _error;

  @override
  void dispose() {
    _oldPwd.dispose();
    _newPwd.dispose();
    _confirmPwd.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(
        left: 16, right: 16, top: 16,
        bottom: MediaQuery.of(context).viewInsets.bottom + 16,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const Text('修改密码', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
          const SizedBox(height: 16),
          TextField(
            controller: _oldPwd,
            obscureText: true,
            decoration: const InputDecoration(labelText: '旧密码'),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _newPwd,
            obscureText: true,
            decoration: const InputDecoration(labelText: '新密码'),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _confirmPwd,
            obscureText: true,
            decoration: const InputDecoration(labelText: '确认新密码'),
          ),
          if (_error != null) ...[
            const SizedBox(height: 8),
            Text(_error!, style: const TextStyle(color: AppTheme.error, fontSize: 12)),
          ],
          const SizedBox(height: 20),
          SizedBox(
            height: 48,
            child: ElevatedButton(
              onPressed: _isLoading ? null : _submit,
              child: _isLoading
                  ? const CircularProgressIndicator(strokeWidth: 2, color: Colors.white)
                  : const Text('确认修改'),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _submit() async {
    if (_newPwd.text != _confirmPwd.text) {
      setState(() => _error = '两次输入的新密码不一致');
      return;
    }
    if (_newPwd.text.length < 6) {
      setState(() => _error = '新密码长度不能少于6位');
      return;
    }
    setState(() { _error = null; _isLoading = true; });
    final ok = await Get.find<AuthController>().changePassword(_oldPwd.text, _newPwd.text);
    setState(() => _isLoading = false);
    if (ok) {
      Navigator.pop(context);
      Get.snackbar('成功', '密码已修改');
    } else {
      setState(() => _error = Get.find<AuthController>().error.value ?? '修改失败');
    }
  }
}
