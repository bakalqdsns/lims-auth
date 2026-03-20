import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../config/api_config.dart';

class AuthService {
  static String get _baseUrl => ApiConfig.effectiveBaseUrl;
  static String? _token;

  static String? get token => _token;

  static Future<Map<String, dynamic>> login(String username, String password) async {
    try {
      final url = Uri.parse('$_baseUrl/auth/login');
      print('正在连接: $url');
      
      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'username': username,
          'password': password,
        }),
      ).timeout(
        const Duration(seconds: 10),
        onTimeout: () {
          throw Exception('连接超时，请检查服务器地址和网络');
        },
      );

      final data = jsonDecode(response.body);

      if (data['code'] == 200 && data['data'] != null) {
        _token = data['data']['token'];
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('token', _token!);
        await prefs.setString('user', jsonEncode(data['data']['user']));
        return {'success': true, 'data': data['data']};
      } else {
        return {'success': false, 'message': data['message'] ?? '登录失败'};
      }
    } on FormatException catch (e) {
      return {
        'success': false, 
        'message': '服务器响应格式错误，请检查服务器地址是否正确'
      };
    } on http.ClientException catch (_) {
      return {
        'success': false, 
        'message': '无法连接到服务器 ($_baseUrl)\n请检查：\n1. 服务器是否运行\n2. 服务器地址是否正确\n3. 手机和服务器是否在同一网络'
      };
    } catch (e) {
      return {
        'success': false, 
        'message': '网络错误: ${e.toString()}'
      };
    }
  }

  static Future<Map<String, dynamic>?> getCurrentUser() async {
    final prefs = await SharedPreferences.getInstance();
    final userStr = prefs.getString('user');
    if (userStr != null) {
      return jsonDecode(userStr);
    }
    return null;
  }

  static Future<bool> isLoggedIn() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('token');
    return _token != null;
  }

  static Future<void> logout() async {
    _token = null;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('token');
    await prefs.remove('user');
  }
}
