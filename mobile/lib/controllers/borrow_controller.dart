import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class BorrowController extends GetxController {
  final _service = BorrowRecordService();

  final records = <BorrowRecord>[].obs;
  final myRecords = <BorrowRecord>[].obs;
  final pendingRecords = <BorrowRecord>[].obs;
  final selectedRecord = Rxn<BorrowRecord>();
  final isLoading = false.obs;
  final error = RxnString();
  final currentTab = 0.obs;

  Future<void> loadRecords({String? status, bool refresh = false}) async {
    if (refresh) records.clear();
    isLoading.value = true;
    error.value = null;
    try {
      records.value = await _service.getRecords(status: status);
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadMyRecords({String? status}) async {
    isLoading.value = true;
    try {
      myRecords.value = await _service.getMyRecords(status: status);
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadPending() async {
    isLoading.value = true;
    try {
      pendingRecords.value = await _service.getPending();
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadExpiring() async {
    try {
      records.value = await _service.getExpiring();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadOverdue() async {
    try {
      records.value = await _service.getOverdue();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<BorrowRecord?> getById(String id) async {
    try {
      selectedRecord.value = await _service.getById(id);
      return selectedRecord.value;
    } on ApiException catch (e) {
      error.value = e.message;
      return null;
    }
  }

  Future<bool> createRecord(CreateBorrowRequest req) async {
    try {
      final record = await _service.createRecord(req);
      if (record != null) {
        myRecords.insert(0, record);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> confirmBorrow(String id) async {
    try {
      return await _service.confirmBorrow(id);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> submitReturn(String id) async {
    try {
      return await _service.submitReturn(id);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> confirmReturn(String id, {String? condition, String? remarks}) async {
    try {
      return await _service.confirmReturn(id, ReturnConfirmRequest(
        condition: condition,
        remarks: remarks,
      ));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> renew(String id, String newReturnDate) async {
    try {
      return await _service.renew(id, RenewRequest(newReturnDate: newReturnDate));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> approveRenew(String id, bool approved, {String? remark}) async {
    try {
      return await _service.approveRenew(id, BorrowApprovalRequest(
        approved: approved,
        remark: remark,
      ));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> supervisorApprove(String id, bool approved, {String? remark}) async {
    try {
      return await _service.supervisorApprove(id, BorrowApprovalRequest(
        approved: approved,
        remark: remark,
      ));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> adminApprove(String id, bool approved, {String? remark}) async {
    try {
      return await _service.adminApprove(id, BorrowApprovalRequest(
        approved: approved,
        remark: remark,
      ));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> deleteRecord(String id) async {
    try {
      final ok = await _service.deleteRecord(id);
      if (ok) {
        records.removeWhere((r) => r.id == id);
        myRecords.removeWhere((r) => r.id == id);
        pendingRecords.removeWhere((r) => r.id == id);
      }
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }
}
