import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class AuthController extends GetxController {
  final _authService = AuthService();

  final token = RxnString();
  final currentUser = Rxn<User>();
  final permissions = <String>[].obs;
  final isLoading = false.obs;
  final error = RxnString();

  @override
  void onInit() {
    super.onInit();
    _loadCachedUser();
  }

  bool get isLoggedIn => token.value != null && token.value!.isNotEmpty;

  bool get isAdmin => currentUser.value?.isAdmin ?? false;
  bool get isTeacher => currentUser.value?.isTeacher ?? false;
  bool get isStudent => currentUser.value?.isStudent ?? false;

  bool hasPermission(String permission) {
    if (currentUser.value == null) return false;
    if (isAdmin) return true;
    return permissions.contains(permission);
  }

  bool hasRole(String role) {
    return currentUser.value?.roles.contains(role) ?? false;
  }

  Future<void> _loadCachedUser() async {
    final user = await _authService.getCachedUser();
    if (user != null) {
      currentUser.value = user;
      token.value = await _authService.getToken();
    }
  }

  Future<bool> login(String username, String password) async {
    isLoading.value = true;
    error.value = null;
    try {
      final user = await _authService.login(username, password);
      if (user != null) {
        currentUser.value = user;
        token.value = await _authService.getToken();
        // 加载权限
        try {
          permissions.value = await UserService().getMyPermissions();
        } catch (_) {}
        return true;
      }
      error.value = '登录失败';
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    } catch (e) {
      error.value = '网络错误';
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> fetchCurrentUser() async {
    if (!isLoggedIn) return;
    try {
      final user = await _authService.getCurrentUser();
      if (user != null) {
        currentUser.value = user;
        permissions.value = await UserService().getMyPermissions();
      }
    } catch (_) {}
  }

  Future<bool> updateProfile(UpdateProfileRequest req) async {
    try {
      final ok = await _authService.updateProfile(req);
      if (ok) {
        await fetchCurrentUser();
      }
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> changePassword(String oldPassword, String newPassword) async {
    isLoading.value = true;
    try {
      return await _authService.changePassword(oldPassword, newPassword);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> logout() async {
    await _authService.logout();
    token.value = null;
    currentUser.value = null;
    permissions.clear();
    error.value = null;
  }
}
