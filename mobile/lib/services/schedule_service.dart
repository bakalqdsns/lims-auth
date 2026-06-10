import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class ScheduleService {
  ApiService get _api => ApiService.to;

  Future<List<ScheduleEntry>> getSchedules({
    String? semesterId,
    String? labId,
    String? classId,
    String? teacherId,
    int? weekNumber,
  }) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    if (labId != null) params['labId'] = labId;
    if (classId != null) params['classId'] = classId;
    if (teacherId != null) params['teacherId'] = teacherId;
    if (weekNumber != null) params['weekNumber'] = weekNumber;

    final resp = await _api.get('/schedules', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ScheduleEntry.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<ScheduleTableRow>> getTableView({
    String? semesterId,
    String? labId,
    String? classId,
    int? weekNumber,
  }) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    if (labId != null) params['labId'] = labId;
    if (classId != null) params['classId'] = classId;
    if (weekNumber != null) params['weekNumber'] = weekNumber;

    final resp = await _api.get('/schedules/table-view', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ScheduleTableRow.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<ScheduleEntry?> getScheduleById(String id) async {
    final resp = await _api.get('/schedules/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return ScheduleEntry.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<List<Lab>> getAvailableLabs({
    required String semesterId,
    required int weekNumber,
    required int dayOfWeek,
    required int periodNumber,
    int? startWeek,
    int? endWeek,
  }) async {
    final params = <String, dynamic>{
      'semesterId': semesterId,
      'weekNumber': weekNumber,
      'dayOfWeek': dayOfWeek,
      'periodNumber': periodNumber,
    };
    if (startWeek != null) params['startWeek'] = startWeek;
    if (endWeek != null) params['endWeek'] = endWeek;

    final resp = await _api.get('/schedules/available-labs', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Lab.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<ConflictCheckResult> checkConflicts({
    required String labId,
    required String semesterId,
    required int weekNumber,
    required int dayOfWeek,
    required int periodNumber,
    int? startWeek,
    int? endWeek,
  }) async {
    final resp = await _api.post('/schedules/check-conflicts', data: {
      'labId': labId,
      'semesterId': semesterId,
      'weekNumber': weekNumber,
      'dayOfWeek': dayOfWeek,
      'periodNumber': periodNumber,
      'startWeek': startWeek,
      'endWeek': endWeek,
    });
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return ConflictCheckResult.fromJson(data['data'] as Map<String, dynamic>);
    }
    return ConflictCheckResult();
  }

  Future<ScheduleEntry?> createSchedule(ScheduleEntry req) async {
    final resp = await _api.post('/schedules', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return ScheduleEntry.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建排课失败');
  }

  Future<bool> updateSchedule(String id, ScheduleEntry req) async {
    final resp = await _api.put('/schedules/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新排课失败');
  }

  Future<bool> deleteSchedule(String id) async {
    final resp = await _api.delete('/schedules/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除排课失败');
  }

  Future<List<ScheduleEntry>> getByLab(String labId, {String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/schedules/by-lab/$labId', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ScheduleEntry.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<ScheduleEntry>> getByTeacher(String teacherId, {String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/schedules/by-teacher/$teacherId', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ScheduleEntry.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<ScheduleEntry>> getByClass(String classId, {String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/schedules/by-class/$classId', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ScheduleEntry.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<bool> importFromTasks(List<String> taskIds) async {
    final resp = await _api.post('/schedules/import-from-tasks', data: {'taskIds': taskIds});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '导入排课失败');
  }
}
