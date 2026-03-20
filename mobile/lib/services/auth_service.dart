import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class AuthService {
  // 后端 API 地址 - 使用 10.0.2.2 访问本机服务器 (Android 模拟器)
  static const String baseUrl = 'http://10.0.2.2:5000/api/v1';
  
  static String? _token;
  static Map<String, dynamic>? _user;

  // 获取 Token
  static String? get token => _token;
  
  // 获取当前用户
  static Map<String, dynamic>? get user => _user;

  // 是否已登录
  static bool get isLoggedIn => _token != null;

  // 初始化 - 从本地存储读取 Token
  static Future<void> init() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('token');
    final userJson = prefs.getString('user');
    if (userJson != null) {
      _user = jsonDecode(userJson);
    }
  }

  // 登录
  static Future<LoginResult> login(String username, String password) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'username': username,
          'password': password,
        }),
      );

      final data = jsonDecode(response.body);

      if (data['code'] == 200 && data['data'] != null) {
        _token = data['data']['token'];
        _user = data['data']['user'];
        
        // 保存到本地存储
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('token', _token!);
        await prefs.setString('user', jsonEncode(_user));
        
        return LoginResult.success(
          message: data['message'] ?? '登录成功',
          user: _user!,
        );
      } else {
        return LoginResult.failure(
          message: data['message'] ?? '登录失败',
        );
      }
    } catch (e) {
      return LoginResult.failure(
        message: '网络错误，请检查连接',
      );
    }
  }

  // 退出登录
  static Future<void> logout() async {
    _token = null;
    _user = null;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('token');
    await prefs.remove('user');
  }

  // 获取当前用户信息
  static Future<Map<String, dynamic>?> getCurrentUser() async {
    if (_token == null) return null;
    
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/auth/me'),
        headers: {
          'Authorization': 'Bearer $_token',
        },
      );

      final data = jsonDecode(response.body);
      
      if (data['code'] == 200) {
        return data['data'];
      }
      return null;
    } catch (e) {
      return null;
    }
  }
}

// 登录结果类
class LoginResult {
  final bool success;
  final String message;
  final Map<String, dynamic>? user;

  LoginResult._({
    required this.success,
    required this.message,
    this.user,
  });

  factory LoginResult.success({
    required String message,
    required Map<String, dynamic> user,
  }) {
    return LoginResult._(
      success: true,
      message: message,
      user: user,
    );
  }

  factory LoginResult.failure({
    required String message,
  }) {
    return LoginResult._(
      success: false,
      message: message,
    );
  }
}
