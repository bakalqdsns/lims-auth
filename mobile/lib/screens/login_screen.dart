import 'package:flutter/material.dart';
import '../services/auth_service.dart';
import '../config/api_config.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  final _serverUrlController = TextEditingController();
  bool _isLoading = false;
  bool _obscurePassword = true;
  bool _showServerConfig = false;
  String _currentServerUrl = '';

  @override
  void initState() {
    super.initState();
    _checkLoginStatus();
    _currentServerUrl = ApiConfig.effectiveBaseUrl;
    _serverUrlController.text = ApiConfig.baseUrl.replaceAll('/api/v1', '');
  }

  Future<void> _checkLoginStatus() async {
    if (await AuthService.isLoggedIn()) {
      if (mounted) {
        Navigator.pushReplacementNamed(context, '/home');
      }
    }
  }

  void _updateServerUrl() {
    final url = _serverUrlController.text.trim();
    if (url.isNotEmpty) {
      ApiConfig.customBaseUrl = url;
      setState(() {
        _currentServerUrl = ApiConfig.effectiveBaseUrl;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('服务器地址已更新: $url'),
          backgroundColor: Colors.green,
        ),
      );
    }
  }

  Future<void> _login() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final result = await AuthService.login(
      _usernameController.text.trim(),
      _passwordController.text.trim(),
    );

    setState(() => _isLoading = false);

    if (result['success']) {
      final user = result['data']['user'];
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('欢迎回来，${user['fullName'] ?? user['username']}'),
            backgroundColor: Colors.green,
          ),
        );
        Navigator.pushReplacementNamed(context, '/home');
      }
    } else {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(result['message']),
            backgroundColor: Colors.red,
            duration: const Duration(seconds: 5),
            action: SnackBarAction(
              label: '配置服务器',
              textColor: Colors.white,
              onPressed: () {
                setState(() => _showServerConfig = true);
              },
            ),
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [Color(0xFF667eea), Color(0xFF764ba2)],
          ),
        ),
        child: SafeArea(
          child: Center(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(24.0),
              child: Card(
                elevation: 8,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(32.0),
                  child: Form(
                    key: _formKey,
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        // Logo
                        const Icon(
                          Icons.school,
                          size: 64,
                          color: Color(0xFF667eea),
                        ),
                        const SizedBox(height: 16),
                        const Text(
                          '实验室管理系统',
                          style: TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          'Laboratory Information Management',
                          style: TextStyle(
                            fontSize: 14,
                            color: Colors.grey[600],
                          ),
                        ),
                        const SizedBox(height: 32),
                        // Username
                        TextFormField(
                          controller: _usernameController,
                          decoration: const InputDecoration(
                            labelText: '用户名',
                            prefixIcon: Icon(Icons.person),
                            border: OutlineInputBorder(),
                          ),
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return '请输入用户名';
                            }
                            return null;
                          },
                        ),
                        const SizedBox(height: 16),
                        // Password
                        TextFormField(
                          controller: _passwordController,
                          obscureText: _obscurePassword,
                          decoration: InputDecoration(
                            labelText: '密码',
                            prefixIcon: const Icon(Icons.lock),
                            suffixIcon: IconButton(
                              icon: Icon(
                                _obscurePassword
                                    ? Icons.visibility_off
                                    : Icons.visibility,
                              ),
                              onPressed: () {
                                setState(() {
                                  _obscurePassword = !_obscurePassword;
                                });
                              },
                            ),
                            border: const OutlineInputBorder(),
                          ),
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return '请输入密码';
                            }
                            return null;
                          },
                        ),
                        const SizedBox(height: 24),
                        // Login Button
                        SizedBox(
                          width: double.infinity,
                          height: 48,
                          child: ElevatedButton(
                            onPressed: _isLoading ? null : _login,
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFF667eea),
                              foregroundColor: Colors.white,
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8),
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
                                : const Text(
                                    '登 录',
                                    style: TextStyle(fontSize: 16),
                                  ),
                          ),
                        ),
                        const SizedBox(height: 16),
                        // Server Configuration
                        ExpansionTile(
                          title: const Text(
                            '服务器配置',
                            style: TextStyle(fontSize: 14, color: Colors.grey),
                          ),
                          initiallyExpanded: _showServerConfig,
                          onExpansionChanged: (expanded) {
                            setState(() => _showServerConfig = expanded);
                          },
                          children: [
                            Padding(
                              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    '当前: $_currentServerUrl',
                                    style: const TextStyle(fontSize: 12, color: Colors.grey),
                                  ),
                                  const SizedBox(height: 8),
                                  TextFormField(
                                    controller: _serverUrlController,
                                    decoration: InputDecoration(
                                      labelText: '服务器地址',
                                      hintText: 'http://192.168.1.100:5047',
                                      prefixIcon: const Icon(Icons.settings_ethernet),
                                      suffixIcon: IconButton(
                                        icon: const Icon(Icons.save),
                                        onPressed: _updateServerUrl,
                                        tooltip: '保存',
                                      ),
                                      border: const OutlineInputBorder(),
                                      helperText: '真机测试时使用实际IP地址',
                                    ),
                                  ),
                                  const SizedBox(height: 8),
                                  Wrap(
                                    spacing: 8,
                                    children: [
                                      ActionChip(
                                        label: const Text('模拟器'),
                                        onPressed: () {
                                          _serverUrlController.text = 'http://10.0.2.2:5047';
                                          _updateServerUrl();
                                        },
                                      ),
                                      ActionChip(
                                        label: const Text('本机'),
                                        onPressed: () {
                                          _serverUrlController.text = 'http://127.0.0.1:5047';
                                          _updateServerUrl();
                                        },
                                      ),
                                    ],
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 16),
                        // Test accounts
                        const Text(
                          '测试账号',
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.grey,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Wrap(
                          spacing: 8,
                          children: [
                            _buildTestChip('admin', 'admin123'),
                            _buildTestChip('teacher', 'teacher123'),
                            _buildTestChip('student', 'student123'),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildTestChip(String username, String password) {
    return ActionChip(
      label: Text('$username / $password'),
      onPressed: () {
        _usernameController.text = username;
        _passwordController.text = password;
      },
    );
  }

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    _serverUrlController.dispose();
    super.dispose();
  }
}
