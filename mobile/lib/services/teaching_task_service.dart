import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class TeachingTaskService {
  ApiService get _api => ApiService.to;

  Future<List<TeachingTask>> getTasks({String? semesterId, String? courseId, String? classId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    if (courseId != null) params['courseId'] = courseId;
    if (classId != null) params['classId'] = classId;
    final resp = await _api.get('/teaching-tasks', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => TeachingTask.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<TeachingTask?> getTaskById(String id) async {
    final resp = await _api.get('/teaching-tasks/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return TeachingTask.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<TeachingTask?> createTask(TeachingTask req) async {
    final resp = await _api.post('/teaching-tasks', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return TeachingTask.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建授课任务失败');
  }

  Future<bool> updateTask(String id, TeachingTask req) async {
    final resp = await _api.put('/teaching-tasks/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新授课任务失败');
  }

  Future<bool> deleteTask(String id) async {
    final resp = await _api.delete('/teaching-tasks/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除授课任务失败');
  }

  Future<bool> toggleTaskStatus(String id, bool isActive) async {
    final resp = await _api.patch('/teaching-tasks/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }

  Future<bool> addTeacher(String taskId, String teacherId, {bool isMainTeacher = false}) async {
    final resp = await _api.post('/teaching-tasks/$taskId/teachers', data: {
      'teacherId': teacherId,
      'isMainTeacher': isMainTeacher,
    });
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '添加教师失败');
  }

  Future<bool> removeTeacher(String taskId, String teacherId) async {
    final resp = await _api.delete('/teaching-tasks/$taskId/teachers/$teacherId');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '移除教师失败');
  }
}
