import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class UserController extends GetxController {
  final _service = UserService();

  final users = <User>[].obs;
  final isLoading = false.obs;
  final error = RxnString();
  final keyword = ''.obs;
  final selectedDepartmentId = RxnString();

  Future<void> loadUsers({bool refresh = false}) async {
    if (refresh) users.clear();
    isLoading.value = true;
    error.value = null;
    try {
      final result = await _service.getUsers(
        keyword: keyword.value.isEmpty ? null : keyword.value,
        departmentId: selectedDepartmentId.value,
      );
      if (refresh) {
        users.value = result;
      } else {
        users.addAll(result);
      }
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<User?> getUserById(String id) async {
    try {
      return await _service.getUserById(id);
    } on ApiException catch (e) {
      error.value = e.message;
      return null;
    }
  }

  Future<bool> createUser(CreateUserRequest req) async {
    try {
      final user = await _service.createUser(req);
      if (user != null) {
        users.insert(0, user);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> updateUser(String id, UpdateUserRequest req) async {
    try {
      final ok = await _service.updateUser(id, req);
      if (ok) {
        await loadUsers(refresh: true);
      }
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> deleteUser(String id) async {
    try {
      final ok = await _service.deleteUser(id);
      if (ok) {
        users.removeWhere((u) => u.id == id);
      }
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> toggleStatus(String id, bool isActive) async {
    try {
      final ok = await _service.updateUserStatus(id, isActive);
      if (ok) {
        final idx = users.indexWhere((u) => u.id == id);
        if (idx >= 0) {
          users[idx] = User(
            id: users[idx].id,
            username: users[idx].username,
            email: users[idx].email,
            phone: users[idx].phone,
            fullName: users[idx].fullName,
            departmentId: users[idx].departmentId,
            isActive: isActive,
            avatarUrl: users[idx].avatarUrl,
            userRoles: users[idx].userRoles,
            department: users[idx].department,
          );
        }
      }
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> updateRoles(String id, List<String> roleIds) async {
    try {
      return await _service.updateUserRoles(id, roleIds);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> resetPassword(String id) async {
    try {
      return await _service.resetPassword(id);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }
}
