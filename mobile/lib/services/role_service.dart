import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

/// 角色服务
class RoleService {
  ApiService get _api => ApiService.to;

  Future<List<Role>> getRoles() async {
    final resp = await _api.get('/roles');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Role.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<Role>> getAllRoles() async {
    final resp = await _api.get('/roles/all');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Role.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Role?> getRoleById(String id) async {
    final resp = await _api.get('/roles/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Role.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Role?> createRole(CreateRoleRequest req) async {
    final resp = await _api.post('/roles', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Role.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '创建角色失败',
    );
  }

  Future<bool> updateRole(String id, UpdateRoleRequest req) async {
    final resp = await _api.put('/roles/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '更新角色失败',
    );
  }

  Future<bool> deleteRole(String id) async {
    final resp = await _api.delete('/roles/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '删除角色失败',
    );
  }

  Future<bool> updateRolePermissions(String id, List<String> permissionIds) async {
    final resp = await _api.put('/roles/$id/permissions', data: {'permissionIds': permissionIds});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '分配权限失败',
    );
  }

  Future<List<User>> getRoleUsers(String id) async {
    final resp = await _api.get('/roles/$id/users');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data'] ?? [];
      if (items is List) {
        return items.map((e) => User.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }
}
