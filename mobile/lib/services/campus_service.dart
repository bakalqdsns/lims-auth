import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class CampusService {
  ApiService get _api => ApiService.to;

  Future<List<Campus>> getCampuses() async {
    final resp = await _api.get('/campuses');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Campus.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Campus?> getCampusById(String id) async {
    final resp = await _api.get('/campuses/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Campus.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Campus?> createCampus(Campus req) async {
    final resp = await _api.post('/campuses', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Campus.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建校区失败');
  }

  Future<bool> updateCampus(String id, Campus req) async {
    final resp = await _api.put('/campuses/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新校区失败');
  }

  Future<bool> deleteCampus(String id) async {
    final resp = await _api.delete('/campuses/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除校区失败');
  }

  Future<bool> toggleCampusStatus(String id, bool isActive) async {
    final resp = await _api.patch('/campuses/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }
}

class BuildingService {
  ApiService get _api => ApiService.to;

  Future<List<Building>> getBuildings({String? campusId}) async {
    final params = <String, dynamic>{};
    if (campusId != null) params['campusId'] = campusId;
    final resp = await _api.get('/buildings', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Building.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<Building>> getBuildingsByCampus(String campusId) async {
    final resp = await _api.get('/buildings/by-campus/$campusId');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Building.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Building?> getBuildingById(String id) async {
    final resp = await _api.get('/buildings/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Building.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Building?> createBuilding(Building req) async {
    final resp = await _api.post('/buildings', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Building.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建建筑失败');
  }

  Future<bool> updateBuilding(String id, Building req) async {
    final resp = await _api.put('/buildings/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新建筑失败');
  }

  Future<bool> deleteBuilding(String id) async {
    final resp = await _api.delete('/buildings/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除建筑失败');
  }

  Future<bool> toggleBuildingStatus(String id, bool isActive) async {
    final resp = await _api.patch('/buildings/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }
}
