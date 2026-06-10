import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class SemesterController extends GetxController {
  final _service = SemesterService();
  final _calendarService = CalendarService();

  final semesters = <Semester>[].obs;
  final currentSemester = Rxn<Semester>();
  final calendar = <AcademicCalendar>[].obs;
  final todayCalendar = Rxn<AcademicCalendar>();
  final weekInfo = Rxn<WeekInfo>();
  final isLoading = false.obs;
  final error = RxnString();

  @override
  void onInit() {
    super.onInit();
    loadSemesters();
    loadCurrentSemester();
  }

  Future<void> loadSemesters() async {
    isLoading.value = true;
    error.value = null;
    try {
      semesters.value = await _service.getSemesters();
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadCurrentSemester() async {
    try {
      currentSemester.value = await _service.getCurrentSemester();
    } catch (_) {}
  }

  Future<void> loadCalendar({String? semesterId}) async {
    try {
      calendar.value = await _calendarService.getCalendar(semesterId: semesterId);
    } on ApiException catch (e) {
      error.value = e.message;
    }
  }

  Future<void> loadTodayCalendar() async {
    try {
      todayCalendar.value = await _calendarService.getTodayCalendar();
    } catch (_) {}
  }

  Future<void> loadWeekInfo() async {
    try {
      weekInfo.value = await _calendarService.getWeekInfo();
    } catch (_) {}
  }

  Future<bool> createSemester(Semester req) async {
    try {
      final s = await _service.createSemester(req);
      if (s != null) {
        semesters.insert(0, s);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> updateSemester(String id, Semester req) async {
    try {
      final ok = await _service.updateSemester(id, req);
      if (ok) await loadSemesters();
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> deleteSemester(String id) async {
    try {
      final ok = await _service.deleteSemester(id);
      if (ok) semesters.removeWhere((s) => s.id == id);
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> setCurrent(String id) async {
    try {
      final ok = await _service.setCurrentSemester(id);
      if (ok) await loadSemesters();
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> archiveSemester(String id) async {
    try {
      final ok = await _service.archiveSemester(id);
      if (ok) await loadSemesters();
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> generateCalendar(String semesterId) async {
    try {
      return await _service.generateCalendar(semesterId);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> addHoliday(AddHolidayRequest req) async {
    try {
      return await _calendarService.addHoliday(req);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> adjustWorkday(String date, bool isWorkday) async {
    try {
      return await _calendarService.adjustWorkday(date, isWorkday);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }
}
