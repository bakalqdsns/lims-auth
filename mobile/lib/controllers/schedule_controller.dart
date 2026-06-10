import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class ScheduleController extends GetxController {
  final _scheduleService = ScheduleService();
  final _reservationService = ReservationService();
  final _teachingAppService = TeachingApplicationService();
  final _usageService = UsageRegistrationService();

  final schedules = <ScheduleEntry>[].obs;
  final tableView = <ScheduleTableRow>[].obs;
  final reservations = <Reservation>[].obs;
  final pendingReservations = <Reservation>[].obs;
  final teachingApps = <TeachingApplication>[].obs;
  final pendingApps = <TeachingApplication>[].obs;
  final usageRecords = <UsageRegistration>[].obs;
  final pendingUsage = <UsageRegistration>[].obs;
  final overdueUsage = <UsageRegistration>[].obs;
  final selectedSemesterId = RxnString();
  final selectedWeek = 0.obs;
  final isLoading = false.obs;
  final error = RxnString();

  Future<void> loadSchedules({String? labId, String? classId, String? teacherId}) async {
    isLoading.value = true;
    error.value = null;
    try {
      schedules.value = await _scheduleService.getSchedules(
        semesterId: selectedSemesterId.value,
        labId: labId,
        classId: classId,
        teacherId: teacherId,
        weekNumber: selectedWeek.value > 0 ? selectedWeek.value : null,
      );
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadTableView({String? labId, String? classId}) async {
    isLoading.value = true;
    try {
      tableView.value = await _scheduleService.getTableView(
        semesterId: selectedSemesterId.value,
        labId: labId,
        classId: classId,
        weekNumber: selectedWeek.value > 0 ? selectedWeek.value : null,
      );
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<ConflictCheckResult> checkConflicts({
    required String labId,
    required int weekNumber,
    required int dayOfWeek,
    required int periodNumber,
  }) async {
    return await _scheduleService.checkConflicts(
      labId: labId,
      semesterId: selectedSemesterId.value ?? '',
      weekNumber: weekNumber,
      dayOfWeek: dayOfWeek,
      periodNumber: periodNumber,
    );
  }

  Future<bool> createSchedule(ScheduleEntry req) async {
    try {
      final s = await _scheduleService.createSchedule(req);
      if (s != null) {
        schedules.insert(0, s);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> deleteSchedule(String id) async {
    try {
      final ok = await _scheduleService.deleteSchedule(id);
      if (ok) schedules.removeWhere((s) => s.id == id);
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<void> loadReservations() async {
    try {
      reservations.value = await _reservationService.getReservations();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadPendingReservations() async {
    try {
      pendingReservations.value = await _reservationService.getPending();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<bool> createReservation(CreateReservationRequest req) async {
    try {
      final r = await _reservationService.create(req);
      if (r != null) {
        reservations.insert(0, r);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> approveReservation(String id, {String? comment}) async {
    try {
      return await _reservationService.approve(id, ApprovalRequest(comment: comment));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> rejectReservation(String id, {String? comment}) async {
    try {
      return await _reservationService.reject(id, ApprovalRequest(comment: comment));
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<void> loadTeachingApplications() async {
    try {
      teachingApps.value = await _teachingAppService.getApplications();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadPendingApplications() async {
    try {
      pendingApps.value = await _teachingAppService.getPending();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadUsageRegistrations() async {
    try {
      usageRecords.value = await _usageService.getRegistrations();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadPendingUsage() async {
    try {
      pendingUsage.value = await _usageService.getPending();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadOverdueUsage() async {
    try {
      overdueUsage.value = await _usageService.getOverdue();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<bool> remindUsage(String id) async {
    try {
      return await _usageService.remind(id);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }
}
