import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/controllers.dart';
import '../utils/theme/app_theme.dart';
import '../routes/routes.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _usernameCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();
  bool _obscure = true;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _checkLogin();
  }

  Future<void> _checkLogin() async {
    final auth = Get.find<AuthController>();
    await auth.fetchCurrentUser();
    if (auth.isLoggedIn) {
      Get.offAllNamed(AppRoutes.home);
    }
  }

  @override
  void dispose() {
    _usernameCtrl.dispose();
    _passwordCtrl.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    final username = _usernameCtrl.text.trim();
    final password = _passwordCtrl.text;
    if (username.isEmpty || password.isEmpty) {
      Get.snackbar('提示', '请输入用户名和密码', snackPosition: SnackPosition.TOP);
      return;
    }

    setState(() => _isLoading = true);
    final auth = Get.find<AuthController>();
    final ok = await auth.login(username, password);
    setState(() => _isLoading = false);

    if (ok) {
      Get.offAllNamed(AppRoutes.home);
    } else {
      Get.snackbar('登录失败', auth.error.value ?? '请检查用户名和密码',
          snackPosition: SnackPosition.TOP,
          backgroundColor: AppTheme.error.withOpacity(0.1),
          colorText: AppTheme.error);
    }
  }

  void _fillTestAccount(String username, String password) {
    _usernameCtrl.text = username;
    _passwordCtrl.text = password;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [AppTheme.primary, AppTheme.primaryDark],
          ),
        ),
        child: SafeArea(
          child: Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(24),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  // Logo
                  Container(
                    width: 80,
                    height: 80,
                    decoration: BoxDecoration(
                      color: Colors.white.withOpacity(0.2),
                      borderRadius: BorderRadius.circular(20),
                    ),
                    child: const Icon(Icons.science, color: Colors.white, size: 48),
                  ),
                  const SizedBox(height: 24),
                  const Text(
                    '实验室管理系统',
                    style: TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  ),
                  const SizedBox(height: 8),
                  const Text(
                    'Laboratory Information Management System',
                    style: TextStyle(fontSize: 12, color: Colors.white70),
                  ),
                  const SizedBox(height: 48),

                  // 表单卡片
                  Card(
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(16),
                    ),
                    elevation: 8,
                    child: Padding(
                      padding: const EdgeInsets.all(24),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          const Text(
                            '登录',
                            style: TextStyle(
                              fontSize: 22,
                              fontWeight: FontWeight.bold,
                              color: AppTheme.textPrimary,
                            ),
                          ),
                          const SizedBox(height: 24),

                          // 用户名
                          TextField(
                            controller: _usernameCtrl,
                            decoration: InputDecoration(
                              labelText: '用户名',
                              prefixIcon: const Icon(Icons.person_outline),
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                              ),
                            ),
                            textInputAction: TextInputAction.next,
                          ),
                          const SizedBox(height: 16),

                          // 密码
                          TextField(
                            controller: _passwordCtrl,
                            obscureText: _obscure,
                            decoration: InputDecoration(
                              labelText: '密码',
                              prefixIcon: const Icon(Icons.lock_outline),
                              suffixIcon: IconButton(
                                icon: Icon(_obscure ? Icons.visibility_off : Icons.visibility),
                                onPressed: () => setState(() => _obscure = !_obscure),
                              ),
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                              ),
                            ),
                            textInputAction: TextInputAction.done,
                            onSubmitted: (_) => _login(),
                          ),
                          const SizedBox(height: 24),

                          // 登录按钮
                          SizedBox(
                            height: 48,
                            child: ElevatedButton(
                              onPressed: _isLoading ? null : _login,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: AppTheme.primary,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(10),
                                ),
                              ),
                              child: _isLoading
                                  ? const SizedBox(
                                      width: 24,
                                      height: 24,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Colors.white,
                                      ),
                                    )
                                  : const Text('登 录', style: TextStyle(fontSize: 16)),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),

                  // 测试账号
                  const Text(
                    '快速填充测试账号',
                    style: TextStyle(color: Colors.white70, fontSize: 12),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    children: [
                      _TestChip(
                        label: '管理员',
                        onTap: () => _fillTestAccount('admin', 'admin123'),
                      ),
                      _TestChip(
                        label: '教师',
                        onTap: () => _fillTestAccount('teacher', 'teacher123'),
                      ),
                      _TestChip(
                        label: '学生',
                        onTap: () => _fillTestAccount('student', 'student123'),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _TestChip extends StatelessWidget {
  final String label;
  final VoidCallback onTap;

  const _TestChip({required this.label, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ActionChip(
      label: Text(label, style: const TextStyle(color: Colors.white, fontSize: 12)),
      backgroundColor: Colors.white.withOpacity(0.2),
      side: const BorderSide(color: Colors.white30),
      onPressed: onTap,
    );
  }
}
