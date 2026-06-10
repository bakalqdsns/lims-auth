import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class LabService {
  ApiService get _api => ApiService.to;

  Future<List<Lab>> getLabs({String? keyword, String? buildingId, bool? isActive}) async {
    final params = <String, dynamic>{};
    if (keyword != null) params['keyword'] = keyword;
    if (buildingId != null) params['buildingId'] = buildingId;
    if (isActive != null) params['isActive'] = isActive;
    final resp = await _api.get('/labs', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Lab.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Lab?> getLabById(String id) async {
    final resp = await _api.get('/labs/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Lab.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Lab?> createLab(CreateLabRequest req) async {
    final resp = await _api.post('/labs', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Lab.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建实验室失败');
  }

  Future<bool> updateLab(String id, CreateLabRequest req) async {
    final resp = await _api.put('/labs/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新实验室失败');
  }

  Future<bool> deleteLab(String id) async {
    final resp = await _api.delete('/labs/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除实验室失败');
  }

  Future<bool> toggleLabStatus(String id, bool isActive) async {
    final resp = await _api.patch('/labs/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }
}
