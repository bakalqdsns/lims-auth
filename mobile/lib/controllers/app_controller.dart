import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class AppController extends GetxController {
  final semesters = <Semester>[].obs;
  final currentSemester = Rxn<Semester>();
  final departments = <Department>[].obs;
  final campuses = <Campus>[].obs;
  final buildings = <Building>[].obs;
  final roles = <Role>[].obs;
  final courses = <Course>[].obs;
  final majors = <Major>[].obs;
  final classes = <ClassModel>[].obs;
  final labs = <Lab>[].obs;

  final isLoading = false.obs;

  Future<void> loadSemesters() async {
    try {
      semesters.value = await SemesterService().getSemesters();
      currentSemester.value = await SemesterService().getCurrentSemester();
    } catch (_) {}
  }

  Future<void> loadDepartments() async {
    try {
      departments.value = await DepartmentService().getDepartments();
    } catch (_) {}
  }

  Future<void> loadCampuses() async {
    try {
      campuses.value = await CampusService().getCampuses();
    } catch (_) {}
  }

  Future<void> loadBuildings({String? campusId}) async {
    try {
      if (campusId != null) {
        buildings.value = await BuildingService().getBuildingsByCampus(campusId);
      } else {
        buildings.value = await BuildingService().getBuildings();
      }
    } catch (_) {}
  }

  Future<void> loadRoles() async {
    try {
      roles.value = await RoleService().getAllRoles();
    } catch (_) {}
  }

  Future<void> loadCourses() async {
    try {
      courses.value = await CourseService().getCourses();
    } catch (_) {}
  }

  Future<void> loadMajors() async {
    try {
      majors.value = await MajorService().getMajors();
    } catch (_) {}
  }

  Future<void> loadClasses() async {
    try {
      classes.value = await ClassService().getClasses();
    } catch (_) {}
  }

  Future<void> loadLabs() async {
    try {
      labs.value = await LabService().getLabs();
    } catch (_) {}
  }

  Future<void> loadAll() async {
    isLoading.value = true;
    await Future.wait([
      loadSemesters(),
      loadDepartments(),
      loadCampuses(),
      loadRoles(),
      loadCourses(),
      loadMajors(),
      loadClasses(),
      loadLabs(),
    ]);
    isLoading.value = false;
  }
}
