import 'package:get/get.dart' hide Response;
import '../models/models.dart';
import 'api_service.dart';

class BorrowRecordService {
  ApiService get _api => ApiService.to;

  Future<List<BorrowRecord>> getRecords({
    String? status,
    String? keyword,
    int page = 1,
    int pageSize = 20,
  }) async {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };
    if (status != null && status.isNotEmpty) params['status'] = status;
    if (keyword != null && keyword.isNotEmpty) params['keyword'] = keyword;

    final resp = await _api.get('/borrow-records', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => BorrowRecord.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<BorrowRecord>> getMyRecords({String? status}) async {
    final params = <String, dynamic>{};
    if (status != null) params['status'] = status;
    final resp = await _api.get('/borrow-records/my', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => BorrowRecord.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<BorrowRecord>> getPending() async {
    final resp = await _api.get('/borrow-records/pending');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => BorrowRecord.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<BorrowRecord>> getExpiring() async {
    final resp = await _api.get('/borrow-records/expiring');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => BorrowRecord.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<BorrowRecord>> getOverdue() async {
    final resp = await _api.get('/borrow-records/overdue');
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => BorrowRecord.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<BorrowRecord?> getByRecordNo(String recordNo) async {
    final resp = await _api.get('/borrow-records/no/$recordNo');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return BorrowRecord.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<BorrowRecord?> getById(String id) async {
    final resp = await _api.get('/borrow-records/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return BorrowRecord.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<BorrowRecord?> createRecord(CreateBorrowRequest req) async {
    final resp = await _api.post('/borrow-records', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return BorrowRecord.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建借用记录失败');
  }

  Future<bool> confirmBorrow(String id) async {
    final resp = await _api.post('/borrow-records/$id/confirm-borrow');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '确认借用失败');
  }

  Future<bool> submitReturn(String id) async {
    final resp = await _api.post('/borrow-records/$id/submit-return');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '提交归还失败');
  }

  Future<bool> confirmReturn(String id, ReturnConfirmRequest req) async {
    final resp = await _api.post('/borrow-records/$id/confirm-return', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '确认归还失败');
  }

  Future<bool> renew(String id, RenewRequest req) async {
    final resp = await _api.post('/borrow-records/$id/renew', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '申请续借失败');
  }

  Future<bool> approveRenew(String id, BorrowApprovalRequest req) async {
    final resp = await _api.post('/borrow-records/$id/approve-renew', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '审批续借失败');
  }

  Future<bool> supervisorApprove(String id, BorrowApprovalRequest req) async {
    final resp = await _api.post('/borrow-records/$id/supervisor-approve', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '主管审批失败');
  }

  Future<bool> adminApprove(String id, BorrowApprovalRequest req) async {
    final resp = await _api.post('/borrow-records/$id/admin-approve', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '管理员审批失败');
  }

  Future<bool> approveReturn(String id, ReturnConfirmRequest req) async {
    final resp = await _api.post('/borrow-records/$id/approve-return', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '归还审批失败');
  }

  Future<bool> deleteRecord(String id) async {
    final resp = await _api.delete('/borrow-records/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除记录失败');
  }
}
