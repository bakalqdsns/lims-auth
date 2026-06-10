# LimsAuth Flutter 移动端完整实现计划

**项目目标：** 完善 `mobile/` (Flutter) 移动端，实现与后端 API (150+ 接口) 完整对接的实验室管理系统。

**后端地址：** `http://localhost:5047/api/v1`
**参考项目：** `frontend/` (Vue 3，已完整实现所有 API 调用)
**现状：** 已有登录页面骨架 + `auth_service.dart`，其余模块均为占位页面。

---

## 一、技术架构

### 1.1 技术栈选型

| 层级 | 技术选型 | 说明 |
|------|---------|------|
| 框架 | Flutter 3.x / Dart | 现有项目基础 |
| HTTP 客户端 | `dio` ^5.4.0 | 替代 `http` 包，支持拦截器 |
| 状态管理 | `get` (GetX) ^4.6.6 | 路由+状态+DI一体化，与 Pinia 风格对齐 |
| 本地存储 | `shared_preferences` ^2.2.2 | Token 持久化（已有）|
| 日期处理 | `intl` ^0.19.0 | 日期格式化 |
| Excel | `excel` ^4.0.3 + `file_picker` ^6.1.1 | 设备导入导出 |
| 图表 | `fl_chart` ^0.66.2 | 统计图表 |
| 图片缓存 | `cached_network_image` ^3.3.1 | 设备图片 |
| 列表骨架 | `shimmer` ^3.0.0 | 加载效果 |
| 抽屉组件 | `flutter_slidable` ^3.0.1 | 列表滑动操作 |

### 1.2 目标目录结构

```
mobile/lib/
├── main.dart                        # 入口：GetX 配置、路由、依赖注入
├── config/
│   └── api_config.dart              # API 配置（已有，调整为 Dio）
├── models/
│   ├── api_response.dart            # API 统一响应封装
│   ├── user.dart                    # 用户/角色/权限/部门模型
│   ├── semester.dart                # 学期/课程/专业/班级模型
│   ├── lab.dart                     # 实验室/设备模型
│   ├── schedule.dart                # 排课/预约/使用登记模型
│   ├── borrow_record.dart           # 借用记录模型
│   ├── experiment.dart              # 实验任务/项目/培养计划模型
│   └── statistics.dart             # 统计数据模型
├── services/
│   ├── api_service.dart             # Dio 基础服务（含拦截器）
│   ├── auth_service.dart            # 认证（替换现有）
│   ├── user_service.dart            # 用户管理
│   ├── role_service.dart            # 角色管理
│   ├── permission_service.dart      # 权限管理
│   ├── department_service.dart      # 部门管理
│   ├── campus_service.dart          # 校区管理
│   ├── building_service.dart        # 建筑管理
│   ├── lab_service.dart             # 实验室管理
│   ├── equipment_service.dart       # 设备管理（含导入导出）
│   ├── borrow_record_service.dart   # 借用记录
│   ├── semester_service.dart        # 学期管理
│   ├── calendar_service.dart        # 校历管理
│   ├── course_service.dart          # 课程管理
│   ├── major_service.dart           # 专业管理
│   ├── class_service.dart           # 班级管理
│   ├── teaching_task_service.dart   # 授课任务
│   ├── schedule_service.dart        # 排课管理
│   ├── reservation_service.dart     # 预约管理
│   ├── teaching_application_service.dart  # 授课申请
│   ├── usage_registration_service.dart   # 使用登记
│   └── statistics_service.dart      # 统计报表
├── controllers/
│   ├── auth_controller.dart         # 认证状态（替换现有 auth_service）
│   ├── user_controller.dart         # 用户列表状态
│   ├── semester_controller.dart     # 学期状态
│   ├── lab_controller.dart          # 实验室状态
│   ├── equipment_controller.dart    # 设备状态
│   ├── borrow_controller.dart       # 借用记录状态
│   ├── schedule_controller.dart     # 排课状态
│   ├── statistics_controller.dart   # 统计数据状态
│   └── app_controller.dart          # 全局 app 状态（当前学期等）
├── screens/
│   ├── login_screen.dart            # 登录页（已有，优化）
│   ├── home_screen.dart             # 首页骨架（已有，替换为完整实现）
│   ├── dashboard_screen.dart        # 仪表盘
│   ├── users_screen.dart            # 用户管理
│   ├── roles_screen.dart            # 角色管理
│   ├── departments_screen.dart      # 部门管理（树形）
│   ├── semesters_screen.dart        # 学期管理
│   ├── courses_screen.dart          # 课程管理
│   ├── majors_screen.dart           # 专业管理
│   ├── classes_screen.dart          # 班级管理
│   ├── teaching_tasks_screen.dart    # 授课任务
│   ├── labs_screen.dart             # 实验室管理
│   ├── equipments_screen.dart       # 设备管理
│   ├── borrow_records_screen.dart    # 借用记录
│   ├── schedules_screen.dart        # 排课管理
│   ├── reservations_screen.dart     # 预约管理
│   ├── teaching_applications_screen.dart  # 授课申请
│   ├── usage_registrations_screen.dart   # 使用登记
│   ├── statistics_screen.dart       # 统计报表
│   └── profile_screen.dart          # 个人中心
├── widgets/
│   ├── api_list_view.dart           # 通用分页列表组件
│   ├── stat_card.dart               # 统计卡片
│   ├── status_badge.dart            # 状态徽章
│   ├── search_bar.dart              # 搜索栏
│   ├── confirm_dialog.dart           # 确认对话框
│   ├── loading_overlay.dart          # 加载遮罩
│   ├── empty_state.dart             # 空状态占位
│   ├── form_field.dart              # 统一表单输入框
│   └── timeline_item.dart           # 时间线（借用记录用）
└── utils/
    ├── date_utils.dart              # 日期工具（周计算等）
    ├── permission_utils.dart        # 权限判断工具
    ├── excel_utils.dart              # Excel 导入导出工具
    └── theme/
        └── app_theme.dart           # 主题配置（紫渐变色系）
```

---

## 二、API 基础设施（Phase 1）

### 2.1 `pubspec.yaml` 依赖更新

```yaml
dependencies:
  flutter:
    sdk: flutter
  dio: ^5.4.0
  get: ^4.6.6
  shared_preferences: ^2.2.2
  intl: ^0.19.0
  excel: ^4.0.3
  file_picker: ^6.1.1
  fl_chart: ^0.66.2
  cached_network_image: ^3.3.1
  shimmer: ^3.0.0
  flutter_slidable: ^3.0.1
```

### 2.2 统一 API 响应模型

后端所有接口返回格式：
```json
{ "code": 200, "message": "操作成功", "data": {...} }
```

```dart
// lib/models/api_response.dart
class ApiResponse<T> {
  final int code;
  final String message;
  final T? data;
  bool get isSuccess => code == 200;
}

class PagedResponse<T> {
  final List<T> items;
  final int total;
  final int page;
  final int pageSize;
}
```

### 2.3 Dio 基础服务 (`services/api_service.dart`)

- Base URL：从 `ApiConfig.effectiveBaseUrl` 读取
- 请求拦截器：自动注入 `Authorization: Bearer {token}`
- 响应拦截器：统一解析 `ApiResponse<T>`，错误码 401 自动跳转登录
- 超时配置：连接 15s，接收 30s

---

## 三、服务层（Phase 2）

每个 Service 类对应 Vue `src/api/` 中的一个文件，采用 `ApiService` 作为 Dio 实例，调用 `get/post/put/patch/delete` 封装方法。

### 3.1 认证服务 (`services/auth_service.dart`)

| 方法 | API | 说明 |
|------|-----|------|
| `login` | `POST /api/v1/Auth/login` | 登录 |
| `refreshToken` | `POST /api/v1/Auth/refresh` | 刷新 Token |
| `getCurrentUser` | `GET /api/v1/Auth/me` | 获取当前用户 |
| `updateProfile` | `PUT /api/v1/Auth/profile` | 更新个人资料 |
| `changePassword` | `POST /api/v1/Users/change-password` | 修改密码 |

### 3.2 用户服务 (`services/user_service.dart`)

| 方法 | API | 说明 |
|------|-----|------|
| `getUsers` | `GET /api/v1/Users` | 分页获取用户 |
| `getUserById` | `GET /api/v1/Users/{id}` | 获取用户详情 |
| `createUser` | `POST /api/v1/Users` | 创建用户 |
| `updateUser` | `PUT /api/v1/Users/{id}` | 更新用户 |
| `deleteUser` | `DELETE /api/v1/Users/{id}` | 删除用户 |
| `updateUserStatus` | `PATCH /api/v1/Users/{id}/status` | 修改状态 |
| `updateUserRoles` | `PUT /api/v1/Users/{id}/roles` | 分配角色 |
| `resetPassword` | `PUT /api/v1/Users/{id}/password` | 重置密码 |
| `getMyPermissions` | `GET /api/v1/Users/permissions/my` | 获取我的权限 |

### 3.3 角色服务 (`services/role_service.dart`)

| 方法 | API |
|------|-----|
| `getRoles` | `GET /api/v1/Roles` |
| `getAllRoles` | `GET /api/v1/Roles/all` |
| `getRoleById` | `GET /api/v1/Roles/{id}` |
| `createRole` | `POST /api/v1/Roles` |
| `updateRole` | `PUT /api/v1/Roles/{id}` |
| `deleteRole` | `DELETE /api/v1/Roles/{id}` |
| `updateRolePermissions` | `PUT /api/v1/Roles/{id}/permissions` |
| `getRoleUsers` | `GET /api/v1/Roles/{id}/users` |

### 3.4 权限服务 (`services/permission_service.dart`)

| 方法 | API |
|------|-----|
| `getAllPermissions` | `GET /api/v1/Permissions` |
| `getPermissionsByModule` | `GET /api/v1/Permissions/by-module` |
| `getPermissionModules` | `GET /api/v1/Permissions/modules` |

### 3.5 部门服务 (`services/department_service.dart`)

| 方法 | API |
|------|-----|
| `getDepartmentTree` | `GET /api/v1/Departments` |
| `getAllDepartments` | `GET /api/v1/Departments/all` |
| `getDepartmentById` | `GET /api/v1/Departments/{id}` |
| `createDepartment` | `POST /api/v1/Departments` |
| `updateDepartment` | `PUT /api/v1/Departments/{id}` |
| `deleteDepartment` | `DELETE /api/v1/Departments/{id}` |

### 3.6 校区服务 + 建筑服务

`campus_service.dart` 和 `building_service.dart`，实现 CRUD + 状态切换 + 按校区查建筑。

### 3.7 实验室服务 (`services/lab_service.dart`)

| 方法 | API |
|------|-----|
| `getLabs` | `GET /api/v1/labs` |
| `getLabById` | `GET /api/v1/labs/{id}` |
| `createLab` | `POST /api/v1/labs` |
| `updateLab` | `PUT /api/v1/labs/{id}` |
| `deleteLab` | `DELETE /api/v1/labs/{id}` |
| `toggleLabStatus` | `PATCH /api/v1/labs/{id}/status` |

### 3.8 设备服务 (`services/equipment_service.dart`)

- CRUD + 状态切换 + 统计
- `importExcel(File)` — `POST /api/v1/equipments/import`
- `exportExcel()` — `GET /api/v1/equipments/export`，返回 blob
- `downloadTemplate()` — `GET /api/v1/equipments/import-template`

### 3.9 借用记录服务 (`services/borrow_record_service.dart`)

覆盖完整借用生命周期所有 API：

| 方法 | API |
|------|-----|
| `getRecords` | `GET /api/v1/borrow-records` |
| `getMyRecords` | `GET /api/v1/borrow-records/my` |
| `getPending` | `GET /api/v1/borrow-records/pending` |
| `getExpiring` | `GET /api/v1/borrow-records/expiring` |
| `getOverdue` | `GET /api/v1/borrow-records/overdue` |
| `getByRecordNo` | `GET /api/v1/borrow-records/no/{recordNo}` |
| `getById` | `GET /api/v1/borrow-records/{id}` |
| `createRecord` | `POST /api/v1/borrow-records` |
| `confirmBorrow` | `POST /api/v1/borrow-records/{id}/confirm-borrow` |
| `submitReturn` | `POST /api/v1/borrow-records/{id}/submit-return` |
| `confirmReturn` | `POST /api/v1/borrow-records/{id}/confirm-return` |
| `renew` | `POST /api/v1/borrow-records/{id}/renew` |
| `approveRenew` | `POST /api/v1/borrow-records/{id}/approve-renew` |
| `supervisorApprove` | `POST /api/v1/borrow-records/{id}/supervisor-approve` |
| `adminApprove` | `POST /api/v1/borrow-records/{id}/admin-approve` |
| `approveReturn` | `POST /api/v1/borrow-records/{id}/approve-return` |
| `deleteRecord` | `DELETE /api/v1/borrow-records/{id}` |

### 3.10 学期服务 (`services/semester_service.dart`)

| 方法 | API |
|------|-----|
| `getSemesters` | `GET /api/v1/semesters` |
| `getCurrentSemester` | `GET /api/v1/semesters/current` |
| `getSemesterById` | `GET /api/v1/semesters/{id}` |
| `createSemester` | `POST /api/v1/semesters` |
| `updateSemester` | `PUT /api/v1/semesters/{id}` |
| `deleteSemester` | `DELETE /api/v1/semesters/{id}` |
| `setCurrent` | `POST /api/v1/semesters/{id}/set-current` |
| `generateCalendar` | `POST /api/v1/semesters/{id}/generate-calendar` |
| `getSemesterCalendar` | `GET /api/v1/semesters/{id}/calendar` |
| `getWeekInfo` | `GET /api/v1/semesters/{id}/week-info` |
| `archive` | `POST /api/v1/semesters/{id}/archive` |
| `copyFromSemester` | `POST /api/v1/semesters/copy-from-semester` |
| `copyFromTemplate` | `POST /api/v1/semesters/copy-from-template` |
| `createSandbox` | `POST /api/v1/semesters/{id}/sandbox` |
| `applySandbox` | `POST /api/v1/semesters/sandbox/{sandboxId}/apply` |
| `deleteSandbox` | `DELETE /api/v1/semesters/sandbox/{sandboxId}` |

### 3.11 校历服务 (`services/calendar_service.dart`)

| 方法 | API |
|------|-----|
| `getCalendar` | `GET /api/v1/calendar` |
| `getToday` | `GET /api/v1/calendar/today` |
| `getWeekInfo` | `GET /api/v1/calendar/week-info` |
| `getByDate` | `GET /api/v1/calendar/date/{date}` |
| `getByEventType` | `GET /api/v1/calendar/by-event-type` |
| `getEventTypes` | `GET /api/v1/calendar/event-types` |
| `getHolidays` | `GET /api/v1/calendar/holidays` |
| `addHoliday` | `POST /api/v1/calendar/holidays` |
| `adjustWorkday` | `POST /api/v1/calendar/adjust-workday` |
| `updateCalendar` | `PUT /api/v1/calendar/{id}` |

### 3.12 课程/专业/班级服务

`course_service.dart`、`major_service.dart`、`class_service.dart`，各自实现 CRUD + 状态切换。

班级服务额外包含：
- `getStudents(classId)` — `GET /api/v1/classes/{id}/students`
- `addStudents(classId, studentIds)` — `POST /api/v1/classes/{id}/students`
- `removeStudent(classId, studentId)` — `DELETE /api/v1/classes/{id}/students/{studentId}`

### 3.13 授课任务服务 (`services/teaching_task_service.dart`)

| 方法 | API |
|------|-----|
| `getTasks` | `GET /api/v1/teaching-tasks` |
| `getTaskById` | `GET /api/v1/teaching-tasks/{id}` |
| `createTask` | `POST /api/v1/teaching-tasks` |
| `updateTask` | `PUT /api/v1/teaching-tasks/{id}` |
| `deleteTask` | `DELETE /api/v1/teaching-tasks/{id}` |
| `toggleTaskStatus` | `PATCH /api/v1/teaching-tasks/{id}/status` |
| `addTeacher` | `POST /api/v1/teaching-tasks/{id}/teachers` |
| `removeTeacher` | `DELETE /api/v1/teaching-tasks/{id}/teachers/{teacherId}` |

### 3.14 排课服务 (`services/schedule_service.dart`)

| 方法 | API |
|------|-----|
| `getSchedules` | `GET /api/v1/schedules` |
| `getScheduleById` | `GET /api/v1/schedules/{id}` |
| `getTableView` | `GET /api/v1/schedules/table-view` |
| `getAvailableLabs` | `GET /api/v1/schedules/available-labs` |
| `checkConflicts` | `POST /api/v1/schedules/check-conflicts` |
| `createSchedule` | `POST /api/v1/schedules` |
| `updateSchedule` | `PUT /api/v1/schedules/{id}` |
| `deleteSchedule` | `DELETE /api/v1/schedules/{id}` |
| `getByLab` | `GET /api/v1/schedules/by-lab/{labId}` |
| `getByTeacher` | `GET /api/v1/schedules/by-teacher/{teacherId}` |
| `getByClass` | `GET /api/v1/schedules/by-class/{classId}` |
| `getImportableTasks` | `GET /api/v1/semesters/importable-tasks` |
| `importFromTasks` | `POST /api/v1/schedules/import-from-tasks` |

### 3.15 预约服务 (`services/reservation_service.dart`)

| 方法 | API |
|------|-----|
| `getReservations` | `GET /api/v1/reservations` |
| `getPending` | `GET /api/v1/reservations/pending` |
| `getById` | `GET /api/v1/reservations/{id}` |
| `create` | `POST /api/v1/reservations` |
| `approve` | `PUT /api/v1/reservations/{id}/approve` |
| `reject` | `PUT /api/v1/reservations/{id}/reject` |
| `cancel` | `PUT /api/v1/reservations/{id}/cancel` |

### 3.16 授课申请服务 (`services/teaching_application_service.dart`)

| 方法 | API |
|------|-----|
| `getApplications` | `GET /api/v1/teaching-applications` |
| `getMyApplications` | `GET /api/v1/teaching-applications/my` |
| `getPending` | `GET /api/v1/teaching-applications/pending` |
| `getById` | `GET /api/v1/teaching-applications/{id}` |
| `create` | `POST /api/v1/teaching-applications` |
| `approve` | `PUT /api/v1/teaching-applications/{id}/approve` |
| `reject` | `PUT /api/v1/teaching-applications/{id}/reject` |
| `cancel` | `PUT /api/v1/teaching-applications/{id}/cancel` |

### 3.17 使用登记服务 (`services/usage_registration_service.dart`)

| 方法 | API |
|------|-----|
| `getRegistrations` | `GET /api/v1/usage-registrations` |
| `getPending` | `GET /api/v1/usage-registrations/pending` |
| `getOverdue` | `GET /api/v1/usage-registrations/overdue` |
| `getById` | `GET /api/v1/usage-registrations/{id}` |
| `create` | `POST /api/v1/usage-registrations` |
| `remind` | `PUT /api/v1/usage-registrations/{id}/remind` |
| `getCompletionRate` | `GET /api/v1/usage-registrations/statistics/completion` |

### 3.18 统计服务 (`services/statistics_service.dart`)

| 方法 | API |
|------|-----|
| `getDashboard` | `GET /api/v1/statistics/dashboard` |
| `getByClass` | `GET /api/v1/statistics/by-class` |
| `getByCourse` | `GET /api/v1/statistics/by-course` |
| `getByGrade` | `GET /api/v1/statistics/by-grade` |
| `getByMajor` | `GET /api/v1/statistics/by-major` |
| `getLabUsage` | `GET /api/v1/statistics/lab-usage` |
| `getReservationStats` | `GET /api/v1/statistics/reservation` |
| `getCompletionRate` | `GET /api/v1/statistics/completion-rate` |
| `getWeeklySummary` | `GET /api/v1/statistics/weekly-summary` |
| `exportExcel` | `GET /api/v1/statistics/export` |

---

## 四、状态管理（Phase 3）

使用 GetX，对标 Vue Pinia 的 Composition API 风格（`ref` + `computed` + 函数）。

### 4.1 认证控制器 (`controllers/auth_controller.dart`)

```dart
class AuthController extends GetxController {
  final token = RxnString();
  final currentUser = Rxn<User>();
  final isLoading = false.obs;
  final error = RxnString();

  bool get isLoggedIn => token.value != null;
  bool get isAdmin => [...];
  bool get isTeacher => [...];
  bool get isStudent => [...];

  Future<bool> login(String username, String password);
  Future<void> logout();
  Future<void> fetchCurrentUser();
  Future<bool> changePassword(String oldPwd, String newPwd);
  bool hasPermission(String permission);
  bool hasRole(String role);
}
```

### 4.2 其他控制器

每个域一个 Controller：`UserController`、`SemesterController`、`LabController`、`EquipmentController`、`BorrowRecordController`、`ScheduleController`、`StatisticsController`。每个 Controller 包含：响应式列表、分页状态、加载状态、CRUD 方法。

---

## 五、路由导航（Phase 4）

### 5.1 `main.dart` 重构

```dart
void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await initServices(); // SharedPreferences + Dio
  runApp(const LimsApp());
}

class LimsApp extends StatelessWidget {
  const LimsApp({super.key});

  @override
  Widget build(BuildContext context) {
    return GetMaterialApp(
      title: '实验室管理系统',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light,
      initialRoute: AppRoutes.login,
      getPages: AppPages.routes,
      routingCallback: AppPages.onRouting, // 权限守卫
    );
  }
}
```

### 5.2 路由表

| 路由 | 页面 | 可见角色 |
|------|------|---------|
| `/` | LoginScreen | 公开 |
| `/home` | HomeScreen（主壳） | 登录用户 |
| `/home/dashboard` | DashboardScreen | 全部 |
| `/home/users` | UsersScreen | 管理员 |
| `/home/roles` | RolesScreen | 管理员 |
| `/home/departments` | DepartmentsScreen | 管理员 |
| `/home/semesters` | SemestersScreen | 管理员+教师 |
| `/home/courses` | CoursesScreen | 管理员+教师 |
| `/home/majors` | MajorsScreen | 管理员+教师 |
| `/home/classes` | ClassesScreen | 管理员+教师 |
| `/home/teaching-tasks` | TeachingTasksScreen | 管理员+教师 |
| `/home/labs` | LabsScreen | 全部 |
| `/home/equipments` | EquipmentsScreen | 管理员 |
| `/home/borrow-records` | BorrowRecordsScreen | 全部 |
| `/home/schedules` | SchedulesScreen | 全部 |
| `/home/reservations` | ReservationsScreen | 全部 |
| `/home/teaching-apps` | TeachingApplicationsScreen | 管理员+教师 |
| `/home/usage-records` | UsageRegistrationsScreen | 管理员+教师 |
| `/home/statistics` | StatisticsScreen | 管理员+教师 |
| `/home/profile` | ProfileScreen | 全部 |

### 5.3 权限守卫

路由跳转前检查：
1. 是否已登录（无 token → 跳转登录）
2. 角色权限（`requiresRole` → 无对应角色 → 提示无权限）
3. 功能权限（`requiresPermission` → 无权限 → 提示无权限，管理员和超级管理员跳过）

---

## 六、UI 界面（Phase 5）

### 6.1 登录页（优化现有）

- 保留现有紫渐变背景和表单布局
- 替换 `AuthService` 为 `AuthController`（GetX）
- 登录成功后根据角色跳转到对应首页
- 错误提示集成 GetX Snackbar

### 6.2 首页骨架（替换现有）

底部导航 4-5 个主 Tab：

| Tab | 图标 | 内容 |
|-----|------|------|
| 首页 | `Icons.dashboard` | 仪表盘 + 快捷操作 |
| 教学 | `Icons.school` | 学期/课程/班级/任务（抽屉菜单） |
| 实验室 | `Icons.science` | 实验室/设备/借用记录（抽屉菜单） |
| 排课 | `Icons.calendar_today` | 排课/预约/使用登记/统计 |
| 我的 | `Icons.person` | 个人中心/设置/退出 |

侧边抽屉：按角色动态显示功能菜单（`isAdmin`、`isTeacher` 等）。

### 6.3 仪表盘 (`screens/dashboard_screen.dart`)

- 统计卡片：实验室总数、在线设备、本周预约、待审批数
- 快捷操作按钮区（根据角色显示）
- 最近活动列表

### 6.4 用户管理 (`screens/users_screen.dart`)

- 分页表格（昵称/账号/角色/部门/状态）
- 搜索框（按姓名/账号搜索）
- FAB 添加用户 → `UserFormDialog`
- 列表操作：编辑、删除、重置密码、分配角色

### 6.5 角色管理 (`screens/roles_screen.dart`)

- 角色卡片列表（角色名/描述/权限数）
- 创建/编辑角色 → `RoleFormDialog`
- 权限分配 → `AssignPermissionsDialog`（CheckBox 矩阵，按模块分组）

### 6.6 部门管理 (`screens/departments_screen.dart`)

- 树形视图（`ExpansionTile` 或自定义 TreeView）
- 节点操作：新增子部门、编辑、删除
- 新增/编辑 → `DepartmentFormDialog`

### 6.7 学期管理 (`screens/semesters_screen.dart`)

- 列表 + 状态徽章（草稿/进行中/已完成/已归档）
- 当前学期高亮标记
- `SemesterFormDialog`：学期名称、学年、类型、开始/结束日期、教学日期、各类时间节点

### 6.8 课程/专业/班级管理

- 标准 CRUD 列表（搜索/筛选）
- `ClassStudentsDialog`：班级学生管理（添加/移除学生）

### 6.9 授课任务 (`screens/teaching_tasks_screen.dart`)

- 列表按学期分组
- 教师分配 Chip 显示
- 关联课程/班级信息展示

### 6.10 实验室管理 (`screens/labs_screen.dart`)

- 卡片网格布局（实验室图片、名称、位置、容量、状态）
- 筛选：按建筑、按状态
- 状态切换开关（管理员）
- `LabFormDialog`

### 6.11 设备管理 (`screens/equipments_screen.dart`)

- 表格视图（缩略图、编号、名称、型号、状态、所属实验室）
- 导入/导出按钮（Excel）
- 状态徽章颜色：正常(绿)、维修(黄)、报废(红)
- `EquipmentFormDialog`
- `EquipmentImportDialog`（选择文件上传）

### 6.12 借用记录 (`screens/borrow_records_screen.dart`)

- Tab 切换：全部 / 我的 / 待审批 / 逾期 / 即将到期
- 状态徽章颜色编码
- 详情页显示完整借用时间线（申请→主管审批→管理员审批→确认借用→提交归还→确认归还）
- 操作按钮根据状态和角色显示（申请、审批、确认、续借等）

### 6.13 排课管理 (`screens/schedules_screen.dart`)

- 周视图日历（横向滚动周，纵向节次）
- 色块区分不同来源（手动/预约/授课申请）
- 冲突标记（红色边框）
- 筛选：按实验室/班级/教师
- `ConflictAlert` 弹窗显示冲突详情

### 6.14 预约管理 (`screens/reservations_screen.dart`)

- Tab 切换：我的预约 / 待审批
- 详情页：预约信息 + 审批状态时间线
- 操作：申请、审批（通过/驳回）、取消

### 6.15 授课申请 (`screens/teaching_applications_screen.dart`)

结构同预约管理，额外显示：关联授课任务、期望实验室、周次/星期/节次。

### 6.16 使用登记 (`screens/usage_registrations_screen.dart`)

- 列表含逾期高亮显示
- 催办按钮（管理员）
- 登记表单：出勤记录、授课情况、设备状态

### 6.17 统计报表 (`screens/statistics_screen.dart`)

- 折线图：周预约趋势（`fl_chart`）
- 柱状图：各实验室使用率
- 饼图：设备状态分布
- 数据卡片：完成率、逾期率
- 筛选：学期、日期范围
- 导出按钮

### 6.18 个人中心 (`screens/profile_screen.dart`)

- 头像、用户名、角色徽章
- 编辑资料表单（姓名、邮箱、电话）
- 修改密码表单（旧密码 + 新密码）
- 退出登录

---

## 七、通用组件库（Phase 5 补充）

| 组件 | 说明 |
|------|------|
| `ApiListView` | 通用分页列表，支持下拉刷新、上拉加载、骨架屏、错误重试、空状态 |
| `StatCard` | 仪表盘统计卡片（图标+数字+标签） |
| `StatusBadge` | 颜色映射状态徽章（Pending绿/Processing蓝/Failed红 等） |
| `SearchBar` | 搜索框 + 清除按钮 + 回调防抖 |
| `ConfirmDialog` | 确认对话框（删除/审批等危险操作） |
| `LoadingOverlay` | 全屏加载遮罩 |
| `EmptyState` | 空数据占位（图标+文字） |
| `FormField` | 统一表单输入框样式 |
| `TimelineItem` | 借用记录时间线节点 |

---

## 八、工具层（Phase 6）

### 8.1 日期工具 (`utils/date_utils.dart`)

- `getWeekNumber(DateTime date, DateTime semesterStart)` — 计算学期第几周
- `getSemesterWeekInfo(int weekNumber)` — 获取周次信息
- `formatWeekRange(DateTime start, int weekNumber, int totalWeeks)` — 周范围格式化
- `isHoliday(DateTime date)` — 判断是否节假日

### 8.2 权限工具 (`utils/permission_utils.dart`)

```dart
bool hasPermission(String permission) {
  final user = AuthController.to.currentUser.value;
  if (user == null) return false;
  if (user.roles.contains('super_admin')) return true;
  return user.permissions.contains(permission);
}
```

### 8.3 Excel 工具 (`utils/excel_utils.dart`)

- `downloadTemplate(String type)` — 下载导入模板
- `parseEquipmentExcel(File file)` — 解析设备 Excel，返回设备列表
- `exportEquipmentsExcel(List<Equipment> data)` — 导出设备列表

### 8.4 主题 (`utils/theme/app_theme.dart`)

```dart
class AppTheme {
  static const Color primary = Color(0xFF667eea);
  static const Color primaryDark = Color(0xFF764ba2);
  static const Color surface = Colors.white;
  static const Color background = Color(0xFFF5F7FA);

  static ThemeData get light => ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.fromSeed(
      seedColor: primary,
      primary: primary,
      secondary: primaryDark,
    ),
    // ... appBarTheme, cardTheme, inputDecorationTheme
  );
}
```

---

## 九、错误处理

- 全局 Dio 拦截器：所有 HTTP 错误通过 `Get.snackbar` 提示
- 401 响应：自动清空 Token，跳转登录页
- 网络断开：检测到 `SocketException`，显示重试按钮
- Token 过期：后端返回特定码时自动刷新 Token（若刷新失败则跳转登录）

---

## 十、数据模型清单

| 模型文件 | 包含的 DTO |
|---------|-----------|
| `models/user.dart` | User, CreateUserRequest, UpdateUserRequest, LoginRequest, ChangePasswordRequest |
| `models/role.dart` | Role, RolePermission, CreateRoleRequest, UpdateRoleRequest, UpdateRolePermissionsRequest |
| `models/permission.dart` | Permission, PermissionModule |
| `models/department.dart` | Department, CreateDepartmentRequest, UpdateDepartmentRequest |
| `models/campus.dart` | Campus, CreateCampusRequest, UpdateCampusRequest |
| `models/building.dart` | Building, CreateBuildingRequest, UpdateBuildingRequest |
| `models/lab.dart` | Lab, CreateLabRequest, UpdateLabRequest |
| `models/equipment.dart` | Equipment, CreateEquipmentRequest, UpdateEquipmentRequest, UpdateEquipmentStatusRequest |
| `models/semester.dart` | Semester, CreateSemesterRequest, UpdateSemesterRequest, SemesterStatus, SemesterType |
| `models/calendar.dart` | AcademicCalendar, AddHolidayRequest, UpdateCalendarJsonRequest, CalendarEventType, CalendarEventPriority |
| `models/course.dart` | Course, CreateCourseRequest, UpdateCourseRequest |
| `models/major.dart` | Major, CreateMajorRequest, UpdateMajorRequest |
| `models/class.dart` | Class, CreateClassRequest, UpdateClassRequest, ClassStudent, AddStudentsRequest |
| `models/teaching_task.dart` | TeachingTask, CreateTeachingTaskRequest, UpdateTeachingTaskRequest, AddTeacherRequest, TeachingTaskTeacher |
| `models/schedule.dart` | ScheduleEntry, ScheduleEntryDto, ScheduleTableRow, ScheduleTableCell, CreateScheduleEntryRequest, ConflictCheckResult, ConflictItem |
| `models/reservation.dart` | ReservationDto, CreateReservationRequest |
| `models/teaching_application.dart` | TeachingApplicationDto, CreateTeachingApplicationRequest, ApprovalRequest |
| `models/usage_registration.dart` | UsageRegistrationDto, CreateUsageRegistrationRequest |
| `models/borrow_record.dart` | BorrowRecord, CreateBorrowRequestDto, BorrowApprovalRequest, RenewRequest, ReturnApprovalRequest, ReturnConfirmRequest |
| `models/statistics.dart` | DashboardData, CompletionRate, WeeklySummary, LabUsage, ReservationStats |

---

## 十一、实施顺序

```
Step 1:  pubspec.yaml + models + api_service.dart          （基础设施）
Step 2:  auth_service.dart + auth_controller.dart           （认证体系）
Step 3:  逐个实现 Service（16个）+ Controller（按依赖顺序）  （服务层+状态）
Step 4:  main.dart 路由 + 依赖注入                           （路由导航）
Step 5:  首页骨架 + 仪表盘                                    （首页）
Step 6:  用户/角色/部门管理                                   （管理员模块）
Step 7:  教学管理（学期/课程/专业/班级/授课任务）           （教学模块）
Step 8:  实验室/设备/借用记录                                 （实验室模块）
Step 9:  排课/预约/使用登记                                   （排课模块）
Step 10: 统计报表                                            （统计模块）
Step 11: 个人中心                                            （个人模块）
Step 12: 通用组件 + Excel 导入导出 + 细节打磨                 （收尾）
```

---

## 十二、文件清单汇总

**需要创建的文件夹：** `models/`、`services/`、`controllers/`、`screens/`、`widgets/`、`utils/theme/`

**需要创建的文件（约 80 个）：**

- `pubspec.yaml` — 1 个（修改）
- `lib/main.dart` — 1 个（修改）
- `lib/config/api_config.dart` — 1 个（修改）
- `lib/models/` — 18 个模型文件
- `lib/services/` — 17 个服务文件
- `lib/controllers/` — 9 个控制器文件
- `lib/screens/` — 18 个页面文件
- `lib/widgets/` — 9 个通用组件
- `lib/utils/` — 3 个工具文件 + 1 个主题文件
