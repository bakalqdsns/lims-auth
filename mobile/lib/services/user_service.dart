import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

/// 用户服务
class UserService {
  ApiService get _api => ApiService.to;

  /// 获取用户列表
  Future<List<User>> getUsers({
    String? keyword,
    String? departmentId,
    bool? isActive,
    int page = 1,
    int pageSize = 20,
  }) async {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };
    if (keyword != null && keyword.isNotEmpty) params['keyword'] = keyword;
    if (departmentId != null) params['departmentId'] = departmentId;
    if (isActive != null) params['isActive'] = isActive;

    final resp = await _api.get('/users', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => User.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  /// 获取用户详情
  Future<User?> getUserById(String id) async {
    final resp = await _api.get('/users/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return User.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  /// 创建用户
  Future<User?> createUser(CreateUserRequest req) async {
    final resp = await _api.post('/users', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return User.fromJson(data['data'] as Map<String, dynamic>);
    } else {
      throw ApiException(
        code: data['code'] ?? -1,
        message: data['message'] ?? '创建用户失败',
      );
    }
  }

  /// 更新用户
  Future<bool> updateUser(String id, UpdateUserRequest req) async {
    final resp = await _api.put('/users/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '更新用户失败',
    );
  }

  /// 删除用户
  Future<bool> deleteUser(String id) async {
    final resp = await _api.delete('/users/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '删除用户失败',
    );
  }

  /// 修改用户状态
  Future<bool> updateUserStatus(String id, bool isActive) async {
    final resp = await _api.patch('/users/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '修改状态失败',
    );
  }

  /// 分配用户角色
  Future<bool> updateUserRoles(String id, List<String> roleIds) async {
    final resp = await _api.put('/users/$id/roles', data: {'roleIds': roleIds});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '分配角色失败',
    );
  }

  /// 重置密码
  Future<bool> resetPassword(String id) async {
    final resp = await _api.put('/users/$id/password');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '重置密码失败',
    );
  }

  /// 获取我的权限列表
  Future<List<String>> getMyPermissions() async {
    final resp = await _api.get('/users/permissions/my');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      if (data['data'] is List) {
        return List<String>.from(data['data']);
      }
    }
    return [];
  }
}
