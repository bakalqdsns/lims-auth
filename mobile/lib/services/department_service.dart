import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

/// 部门服务
class DepartmentService {
  ApiService get _api => ApiService.to;

  Future<List<Department>> getDepartments({bool tree = false}) async {
    final resp = await _api.get(tree ? '/departments' : '/departments/all');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Department.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Department?> getDepartmentById(String id) async {
    final resp = await _api.get('/departments/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Department.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Department?> createDepartment(Map<String, dynamic> req) async {
    final resp = await _api.post('/departments', data: req);
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Department.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '创建部门失败',
    );
  }

  Future<bool> updateDepartment(String id, Map<String, dynamic> req) async {
    final resp = await _api.put('/departments/$id', data: req);
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '更新部门失败',
    );
  }

  Future<bool> deleteDepartment(String id) async {
    final resp = await _api.delete('/departments/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(
      code: data['code'] ?? -1,
      message: data['message'] ?? '删除部门失败',
    );
  }
}
