import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class CourseService {
  ApiService get _api => ApiService.to;

  Future<List<Course>> getCourses({String? keyword, bool? isActive}) async {
    final params = <String, dynamic>{};
    if (keyword != null) params['keyword'] = keyword;
    if (isActive != null) params['isActive'] = isActive;
    final resp = await _api.get('/courses', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Course.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Course?> getCourseById(String id) async {
    final resp = await _api.get('/courses/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Course.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Course?> createCourse(Course req) async {
    final resp = await _api.post('/courses', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Course.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建课程失败');
  }

  Future<bool> updateCourse(String id, Course req) async {
    final resp = await _api.put('/courses/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新课程失败');
  }

  Future<bool> deleteCourse(String id) async {
    final resp = await _api.delete('/courses/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除课程失败');
  }

  Future<bool> toggleCourseStatus(String id, bool isActive) async {
    final resp = await _api.patch('/courses/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }
}

class MajorService {
  ApiService get _api => ApiService.to;

  Future<List<Major>> getMajors({String? keyword}) async {
    final params = <String, dynamic>{};
    if (keyword != null) params['keyword'] = keyword;
    final resp = await _api.get('/majors', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Major.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<Major>> getAllMajors() async {
    final resp = await _api.get('/majors/all');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Major.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Major?> createMajor(Major req) async {
    final resp = await _api.post('/majors', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Major.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建专业失败');
  }

  Future<bool> updateMajor(String id, Major req) async {
    final resp = await _api.put('/majors/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新专业失败');
  }

  Future<bool> deleteMajor(String id) async {
    final resp = await _api.delete('/majors/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除专业失败');
  }

  Future<bool> toggleMajorStatus(String id, bool isActive) async {
    final resp = await _api.patch('/majors/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }
}

class ClassService {
  ApiService get _api => ApiService.to;

  Future<List<ClassModel>> getClasses({String? keyword, String? majorId}) async {
    final params = <String, dynamic>{};
    if (keyword != null) params['keyword'] = keyword;
    if (majorId != null) params['majorId'] = majorId;
    final resp = await _api.get('/classes', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ClassModel.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<ClassModel?> getClassById(String id) async {
    final resp = await _api.get('/classes/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return ClassModel.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<ClassModel?> createClass(ClassModel req) async {
    final resp = await _api.post('/classes', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return ClassModel.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建班级失败');
  }

  Future<bool> updateClass(String id, ClassModel req) async {
    final resp = await _api.put('/classes/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新班级失败');
  }

  Future<bool> deleteClass(String id) async {
    final resp = await _api.delete('/classes/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除班级失败');
  }

  Future<bool> toggleClassStatus(String id, bool isActive) async {
    final resp = await _api.patch('/classes/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }

  Future<List<ClassStudent>> getClassStudents(String classId) async {
    final resp = await _api.get('/classes/$classId/students');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data'] ?? [];
      if (items is List) {
        return items.map((e) => ClassStudent.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<bool> addStudents(String classId, List<String> studentIds) async {
    final resp = await _api.post('/classes/$classId/students', data: {'studentIds': studentIds});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '添加学生失败');
  }

  Future<bool> removeStudent(String classId, String studentId) async {
    final resp = await _api.delete('/classes/$classId/students/$studentId');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '移除学生失败');
  }
}
