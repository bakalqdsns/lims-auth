import 'dart:convert';
import 'package:dio/dio.dart';
import 'package:get/get.dart' hide Response;
import 'package:shared_preferences/shared_preferences.dart';
import '../models/models.dart';
import 'api_service.dart';

/// 认证服务
class AuthService {
  ApiService get _api => ApiService.to;

  /// 登录
  Future<User?> login(String username, String password) async {
    try {
      final resp = await _api.post('/auth/login', data: {
        'username': username,
        'password': password,
      });

      final data = resp.data;
      if (data['code'] == 200 && data['data'] != null) {
        final token = data['data']['token']?.toString() ?? '';
        final userData = data['data']['user'];

        // 持久化存储
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('token', token);
        await prefs.setString('user', jsonEncode(userData));

        return User.fromJson(userData as Map<String, dynamic>);
      } else {
        throw ApiException(
          code: data['code'] ?? -1,
          message: data['message'] ?? '登录失败',
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDio(e);
    }
  }

  /// 获取当前用户（从服务器）
  Future<User?> getCurrentUser() async {
    try {
      final resp = await _api.get('/auth/me');
      final data = resp.data;
      if (data['code'] == 200 && data['data'] != null) {
        final user = User.fromJson(data['data'] as Map<String, dynamic>);
        // 缓存到本地
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('user', jsonEncode(data['data']));
        return user;
      }
      return null;
    } on DioException catch (e) {
      throw ApiException.fromDio(e);
    }
  }

  /// 从本地缓存获取用户
  Future<User?> getCachedUser() async {
    final prefs = await SharedPreferences.getInstance();
    final userStr = prefs.getString('user');
    if (userStr != null) {
      try {
        return User.fromJson(jsonDecode(userStr) as Map<String, dynamic>);
      } catch (_) {
        return null;
      }
    }
    return null;
  }

  /// 检查是否已登录
  Future<bool> isLoggedIn() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('token') != null;
  }

  /// 获取 Token
  Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('token');
  }

  /// 退出登录
  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('token');
    await prefs.remove('user');
  }

  /// 更新个人资料
  Future<bool> updateProfile(UpdateProfileRequest req) async {
    try {
      final resp = await _api.put('/auth/profile', data: req.toJson());
      final data = resp.data;
      if (data['code'] == 200) {
        // 更新本地缓存
        if (data['data'] != null) {
          final prefs = await SharedPreferences.getInstance();
          await prefs.setString('user', jsonEncode(data['data']));
        }
        return true;
      } else {
        throw ApiException(
          code: data['code'] ?? -1,
          message: data['message'] ?? '更新失败',
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDio(e);
    }
  }

  /// 修改密码
  Future<bool> changePassword(String oldPassword, String newPassword) async {
    try {
      final resp = await _api.post('/users/change-password', data: {
        'oldPassword': oldPassword,
        'newPassword': newPassword,
      });
      final data = resp.data;
      if (data['code'] == 200) {
        return true;
      } else {
        throw ApiException(
          code: data['code'] ?? -1,
          message: data['message'] ?? '修改密码失败',
        );
      }
    } on DioException catch (e) {
      throw ApiException.fromDio(e);
    }
  }
}
