import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class ReservationService {
  ApiService get _api => ApiService.to;

  Future<List<Reservation>> getReservations({
    String? status,
    int page = 1,
    int pageSize = 20,
  }) async {
    final params = <String, dynamic>{'page': page, 'pageSize': pageSize};
    if (status != null) params['status'] = status;
    final resp = await _api.get('/reservations', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Reservation.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<Reservation>> getPending() async {
    final resp = await _api.get('/reservations/pending');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Reservation.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Reservation?> getById(String id) async {
    final resp = await _api.get('/reservations/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Reservation.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Reservation?> create(CreateReservationRequest req) async {
    final resp = await _api.post('/reservations', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Reservation.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建预约失败');
  }

  Future<bool> approve(String id, ApprovalRequest req) async {
    final resp = await _api.put('/reservations/$id/approve', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '审批失败');
  }

  Future<bool> reject(String id, ApprovalRequest req) async {
    final resp = await _api.put('/reservations/$id/reject', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '驳回失败');
  }

  Future<bool> cancel(String id) async {
    final resp = await _api.put('/reservations/$id/cancel');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '取消失败');
  }
}

class TeachingApplicationService {
  ApiService get _api => ApiService.to;

  Future<List<TeachingApplication>> getApplications({String? status}) async {
    final params = <String, dynamic>{};
    if (status != null) params['status'] = status;
    final resp = await _api.get('/teaching-applications', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => TeachingApplication.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<TeachingApplication>> getMyApplications() async {
    final resp = await _api.get('/teaching-applications/my');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => TeachingApplication.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<TeachingApplication>> getPending() async {
    final resp = await _api.get('/teaching-applications/pending');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => TeachingApplication.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<TeachingApplication?> getById(String id) async {
    final resp = await _api.get('/teaching-applications/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return TeachingApplication.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<TeachingApplication?> create(CreateTeachingApplicationRequest req) async {
    final resp = await _api.post('/teaching-applications', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return TeachingApplication.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建申请失败');
  }

  Future<bool> approve(String id, ApprovalRequest req) async {
    final resp = await _api.put('/teaching-applications/$id/approve', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '审批失败');
  }

  Future<bool> reject(String id) async {
    final resp = await _api.put('/teaching-applications/$id/reject');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '驳回失败');
  }

  Future<bool> cancel(String id) async {
    final resp = await _api.put('/teaching-applications/$id/cancel');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '取消失败');
  }
}

class UsageRegistrationService {
  ApiService get _api => ApiService.to;

  Future<List<UsageRegistration>> getRegistrations({String? status}) async {
    final params = <String, dynamic>{};
    if (status != null) params['status'] = status;
    final resp = await _api.get('/usage-registrations', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => UsageRegistration.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<UsageRegistration>> getPending() async {
    final resp = await _api.get('/usage-registrations/pending');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => UsageRegistration.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<UsageRegistration>> getOverdue() async {
    final resp = await _api.get('/usage-registrations/overdue');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => UsageRegistration.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<UsageRegistration?> getById(String id) async {
    final resp = await _api.get('/usage-registrations/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return UsageRegistration.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<UsageRegistration?> create(UsageRegistration req) async {
    final resp = await _api.post('/usage-registrations', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return UsageRegistration.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建登记失败');
  }

  Future<bool> remind(String id) async {
    final resp = await _api.put('/usage-registrations/$id/remind');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '催办失败');
  }

  Future<CompletionRate> getCompletionRate() async {
    final resp = await _api.get('/usage-registrations/statistics/completion');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return CompletionRate.fromJson(data['data'] as Map<String, dynamic>);
    }
    return CompletionRate();
  }
}
