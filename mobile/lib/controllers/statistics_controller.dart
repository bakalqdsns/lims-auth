import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class StatisticsController extends GetxController {
  final _service = StatisticsService();

  final dashboard = Rxn<DashboardData>();
  final labUsage = <LabUsage>[].obs;
  final weeklySummary = <WeeklySummary>[].obs;
  final completionRate = Rxn<CompletionRate>();
  final reservationStats = Rxn<ReservationStats>();
  final byClass = <StatisticsByCategory>[].obs;
  final byMajor = <StatisticsByCategory>[].obs;
  final byCourse = <StatisticsByCategory>[].obs;
  final isLoading = false.obs;
  final error = RxnString();

  Future<void> loadDashboard() async {
    isLoading.value = true;
    try {
      dashboard.value = await _service.getDashboard();
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadLabUsage({String? semesterId}) async {
    try {
      labUsage.value = await _service.getLabUsage(semesterId: semesterId);
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadWeeklySummary({int weeks = 8}) async {
    try {
      weeklySummary.value = await _service.getWeeklySummary(weeks: weeks);
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadCompletionRate() async {
    try {
      completionRate.value = await _service.getCompletionRate();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadReservationStats() async {
    try {
      reservationStats.value = await _service.getReservationStats();
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadByClass({String? semesterId}) async {
    try {
      byClass.value = await _service.getByClass(semesterId: semesterId);
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadByMajor({String? semesterId}) async {
    try {
      byMajor.value = await _service.getByMajor(semesterId: semesterId);
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadByCourse({String? semesterId}) async {
    try {
      byCourse.value = await _service.getByCourse(semesterId: semesterId);
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }
}
