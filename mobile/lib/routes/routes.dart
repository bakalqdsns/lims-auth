import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../screens/login_screen.dart';
import '../screens/home_screen.dart';
import '../screens/dashboard_screen.dart';
import '../screens/users_screen.dart';
import '../screens/profile_screen.dart';
import '../screens/roles_screen.dart';
import '../screens/departments_screen.dart';
import '../screens/semesters_screen.dart';
import '../screens/courses_screen.dart';
import '../screens/majors_screen.dart';
import '../screens/classes_screen.dart';
import '../screens/teaching_tasks_screen.dart';
import '../screens/labs_screen.dart';
import '../screens/equipments_screen.dart';
import '../screens/borrow_records_screen.dart';
import '../screens/schedules_screen.dart';
import '../screens/reservations_screen.dart';
import '../screens/teaching_applications_screen.dart';
import '../screens/usage_registrations_screen.dart';
import '../screens/statistics_screen.dart';

/// 路由名称常量
class AppRoutes {
  static const String login = '/';
  static const String home = '/home';
  static const String dashboard = '/home/dashboard';
  static const String users = '/home/users';
  static const String roles = '/home/roles';
  static const String departments = '/home/departments';
  static const String semesters = '/home/semesters';
  static const String courses = '/home/courses';
  static const String majors = '/home/majors';
  static const String classes = '/home/classes';
  static const String teachingTasks = '/home/teaching-tasks';
  static const String labs = '/home/labs';
  static const String equipments = '/home/equipments';
  static const String borrowRecords = '/home/borrow-records';
  static const String schedules = '/home/schedules';
  static const String reservations = '/home/reservations';
  static const String teachingApps = '/home/teaching-apps';
  static const String usageRecords = '/home/usage-records';
  static const String statistics = '/home/statistics';
  static const String profile = '/home/profile';
}

/// 扁平路由列表（无嵌套 children）
class AppPages {
  static final List<GetPage> routes = [
    GetPage(
      name: AppRoutes.login,
      page: () => const LoginScreen(),
    ),
    GetPage(
      name: AppRoutes.home,
      page: () => const HomeScreen(),
    ),
    // 仪表盘
    GetPage(name: AppRoutes.dashboard, page: () => const DashboardScreen()),
    // 教学 tab
    GetPage(name: AppRoutes.semesters, page: () => const SemestersScreen()),
    GetPage(name: AppRoutes.courses, page: () => const CoursesScreen()),
    GetPage(name: AppRoutes.majors, page: () => const MajorsScreen()),
    GetPage(name: AppRoutes.classes, page: () => const ClassesScreen()),
    GetPage(name: AppRoutes.teachingTasks, page: () => const TeachingTasksScreen()),
    GetPage(name: AppRoutes.users, page: () => const UsersScreen()),
    GetPage(name: AppRoutes.roles, page: () => const RolesScreen()),
    GetPage(name: AppRoutes.departments, page: () => const DepartmentsScreen()),
    // 实验室 tab
    GetPage(name: AppRoutes.labs, page: () => const LabsScreen()),
    GetPage(name: AppRoutes.equipments, page: () => const EquipmentsScreen()),
    GetPage(name: AppRoutes.borrowRecords, page: () => const BorrowRecordsScreen()),
    // 排课 tab
    GetPage(name: AppRoutes.schedules, page: () => const SchedulesScreen()),
    GetPage(name: AppRoutes.reservations, page: () => const ReservationsScreen()),
    GetPage(name: AppRoutes.teachingApps, page: () => const TeachingApplicationsScreen()),
    GetPage(name: AppRoutes.usageRecords, page: () => const UsageRegistrationsScreen()),
    GetPage(name: AppRoutes.statistics, page: () => const StatisticsScreen()),
    // 我的 tab
    GetPage(name: AppRoutes.profile, page: () => const ProfileScreen()),
  ];
}
