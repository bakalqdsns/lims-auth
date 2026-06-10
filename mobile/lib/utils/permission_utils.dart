import 'package:get/get.dart';
import '../controllers/auth_controller.dart';

class PermissionUtils {
  /// 检查是否有指定权限
  static bool hasPermission(String permission) {
    try {
      final auth = Get.find<AuthController>();
      if (!auth.isLoggedIn) return false;
      return auth.hasPermission(permission);
    } catch (_) {
      return false;
    }
  }

  /// 检查是否有指定角色
  static bool hasRole(String role) {
    try {
      final auth = Get.find<AuthController>();
      if (!auth.isLoggedIn) return false;
      return auth.hasRole(role);
    } catch (_) {
      return false;
    }
  }

  /// 是否是管理员
  static bool get isAdmin {
    try {
      return Get.find<AuthController>().isAdmin;
    } catch (_) {
      return false;
    }
  }

  /// 是否是教师
  static bool get isTeacher {
    try {
      return Get.find<AuthController>().isTeacher;
    } catch (_) {
      return false;
    }
  }

  /// 是否是学生
  static bool get isStudent {
    try {
      return Get.find<AuthController>().isStudent;
    } catch (_) {
      return false;
    }
  }

  /// 常用权限检查
  static bool get canManageUsers => isAdmin;
  static bool get canManageRoles => isAdmin;
  static bool get canManageDepartments => isAdmin;
  static bool get canManageSemesters => isAdmin;
  static bool get canManageCourses => isAdmin;
  static bool get canManageLabs => isAdmin;
  static bool get canManageEquipments => isAdmin;
  static bool get canManageSchedule => isAdmin || isTeacher;
  static bool get canApproveReservation => isAdmin;
  static bool get canReadStatistics => isAdmin || isTeacher;
}
