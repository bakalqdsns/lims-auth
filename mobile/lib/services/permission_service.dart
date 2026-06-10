import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

/// 权限服务
class PermissionService {
  ApiService get _api => ApiService.to;

  Future<List<Permission>> getAllPermissions() async {
    final resp = await _api.get('/permissions');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Permission.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<PermissionModule>> getPermissionsByModule() async {
    final resp = await _api.get('/permissions/by-module');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => PermissionModule.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<String>> getModules() async {
    final resp = await _api.get('/permissions/modules');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data'] ?? [];
      if (items is List) return List<String>.from(items);
    }
    return [];
  }
}
