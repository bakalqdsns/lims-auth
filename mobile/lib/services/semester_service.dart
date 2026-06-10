import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class SemesterService {
  ApiService get _api => ApiService.to;

  Future<List<Semester>> getSemesters() async {
    final resp = await _api.get('/semesters');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Semester.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Semester?> getCurrentSemester() async {
    final resp = await _api.get('/semesters/current');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Semester.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Semester?> getSemesterById(String id) async {
    final resp = await _api.get('/semesters/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Semester.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Semester?> createSemester(Semester req) async {
    final resp = await _api.post('/semesters', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Semester.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建学期失败');
  }

  Future<bool> updateSemester(String id, Semester req) async {
    final resp = await _api.put('/semesters/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新学期失败');
  }

  Future<bool> deleteSemester(String id) async {
    final resp = await _api.delete('/semesters/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除学期失败');
  }

  Future<bool> setCurrentSemester(String id) async {
    final resp = await _api.post('/semesters/$id/set-current');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '设置当前学期失败');
  }

  Future<bool> archiveSemester(String id) async {
    final resp = await _api.post('/semesters/$id/archive');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '归档学期失败');
  }

  Future<WeekInfo?> getWeekInfo(String id) async {
    final resp = await _api.get('/semesters/$id/week-info');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return WeekInfo.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<bool> generateCalendar(String id, {List<Map<String, dynamic>>? holidays}) async {
    final resp = await _api.post('/semesters/$id/generate-calendar', data: {
      'holidays': holidays ?? [],
    });
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '生成校历失败');
  }
}

class CalendarService {
  ApiService get _api => ApiService.to;

  Future<List<AcademicCalendar>> getCalendar({String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/calendar', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => AcademicCalendar.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<AcademicCalendar?> getTodayCalendar() async {
    final resp = await _api.get('/calendar/today');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return AcademicCalendar.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<WeekInfo?> getWeekInfo() async {
    final resp = await _api.get('/calendar/week-info');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return WeekInfo.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<bool> addHoliday(AddHolidayRequest req) async {
    final resp = await _api.post('/calendar/holidays', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '添加假期失败');
  }

  Future<bool> adjustWorkday(String date, bool isWorkday) async {
    final resp = await _api.post('/calendar/adjust-workday', data: {
      'date': date,
      'isWorkday': isWorkday,
    });
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '调整工作日失败');
  }
}
