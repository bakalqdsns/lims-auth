import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/auth_controller.dart';
import '../utils/theme/app_theme.dart';
import '../routes/routes.dart';
import '../screens/semesters_screen.dart';
import '../screens/courses_screen.dart';
import '../screens/majors_screen.dart';
import '../screens/classes_screen.dart';
import '../screens/teaching_tasks_screen.dart';
import '../screens/users_screen.dart';
import '../screens/roles_screen.dart';
import '../screens/departments_screen.dart';
import '../screens/labs_screen.dart';
import '../screens/equipments_screen.dart';
import '../screens/borrow_records_screen.dart';
import '../screens/schedules_screen.dart';
import '../screens/reservations_screen.dart';
import '../screens/teaching_applications_screen.dart';
import '../screens/usage_registrations_screen.dart';
import '../screens/statistics_screen.dart';
import '../screens/profile_screen.dart';

/// 路由到页面的映射（HomeScreen 内部导航用，不走 GetX）
Widget _pageFor(String route) {
  switch (route) {
    case AppRoutes.semesters:   return const SemestersScreen();
    case AppRoutes.courses:     return const CoursesScreen();
    case AppRoutes.majors:      return const MajorsScreen();
    case AppRoutes.classes:     return const ClassesScreen();
    case AppRoutes.teachingTasks: return const TeachingTasksScreen();
    case AppRoutes.users:       return const UsersScreen();
    case AppRoutes.roles:       return const RolesScreen();
    case AppRoutes.departments: return const DepartmentsScreen();
    case AppRoutes.labs:        return const LabsScreen();
    case AppRoutes.equipments:   return const EquipmentsScreen();
    case AppRoutes.borrowRecords: return const BorrowRecordsScreen();
    case AppRoutes.schedules:   return const SchedulesScreen();
    case AppRoutes.reservations: return const ReservationsScreen();
    case AppRoutes.teachingApps:  return const TeachingApplicationsScreen();
    case AppRoutes.usageRecords:  return const UsageRegistrationsScreen();
    case AppRoutes.statistics:   return const StatisticsScreen();
    case AppRoutes.profile:      return const ProfileScreen();
    default:                    return const SizedBox.shrink();
  }
}

/// HomeScreen：底部 5 tab，每个 tab 有独立的 Navigator。
/// 所有子页面由对应 tab 的 Navigator 管理，GetX 仅用于顶层页面切换（登录↔首页）。
class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => HomeScreenState();
}

class HomeScreenState extends State<HomeScreen> {
  int _currentIndex = 0;

  final _dashboardKey = GlobalKey<NavigatorState>();
  final _teachingKey = GlobalKey<NavigatorState>();
  final _labKey      = GlobalKey<NavigatorState>();
  final _scheduleKey = GlobalKey<NavigatorState>();
  final _profileKey  = GlobalKey<NavigatorState>();

  NavigatorState get _currentNav {
    switch (_currentIndex) {
      case 0: return _dashboardKey.currentState!;
      case 1: return _teachingKey.currentState!;
      case 2: return _labKey.currentState!;
      case 3: return _scheduleKey.currentState!;
      case 4: return _profileKey.currentState!;
      default: return _dashboardKey.currentState!;
    }
  }

  /// 导航到子页面（在当前 tab 的 Navigator 中 push）
  void push(String route) {
    _currentNav.push(MaterialPageRoute(builder: (_) => _pageFor(route)));
  }

  /// 切换 tab：如果已在当前 tab 则回退到根页
  void _onTabSelected(int idx) {
    if (idx == _currentIndex) {
      _currentNav.popUntil((route) => route.isFirst);
    } else {
      setState(() => _currentIndex = idx);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(
        index: _currentIndex,
        children: [
          Navigator(key: _dashboardKey, onGenerateRoute: (_) => MaterialPageRoute(builder: (_) => _buildDashboardNav())),
          Navigator(key: _teachingKey, onGenerateRoute: (_) => MaterialPageRoute(builder: (_) => _buildTeachingNav())),
          Navigator(key: _labKey,      onGenerateRoute: (_) => MaterialPageRoute(builder: (_) => _buildLabNav())),
          Navigator(key: _scheduleKey, onGenerateRoute: (_) => MaterialPageRoute(builder: (_) => _buildScheduleNav())),
          Navigator(key: _profileKey,  onGenerateRoute: (_) => MaterialPageRoute(builder: (_) => _buildProfileNav())),
        ],
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _currentIndex,
        onDestinationSelected: _onTabSelected,
        destinations: const [
          NavigationDestination(icon: Icon(Icons.dashboard_outlined),  selectedIcon: Icon(Icons.dashboard),       label: '首页'),
          NavigationDestination(icon: Icon(Icons.school_outlined),      selectedIcon: Icon(Icons.school),          label: '教学'),
          NavigationDestination(icon: Icon(Icons.science_outlined),     selectedIcon: Icon(Icons.science),         label: '实验室'),
          NavigationDestination(icon: Icon(Icons.calendar_today_outlined), selectedIcon: Icon(Icons.calendar_today), label: '排课'),
          NavigationDestination(icon: Icon(Icons.person_outline),       selectedIcon: Icon(Icons.person),           label: '我的'),
        ],
      ),
    );
  }

  Widget _buildDashboardNav() => const _NavPlaceholder('仪表盘', Icons.dashboard);
  Widget _buildProfileNav()   => const _NavPlaceholder('我的', Icons.person);

  Widget _buildTeachingNav() {
    final auth = Get.find<AuthController>();
    return Scaffold(
      appBar: AppBar(title: const Text('教学管理')),
      body: ListView(
        children: [
          _Tile(icon: Icons.calendar_month,  title: '学期管理', subtitle: '管理学期和校历', onTap: () => push(AppRoutes.semesters)),
          _Tile(icon: Icons.book,            title: '课程管理', subtitle: '管理课程信息',   onTap: () => push(AppRoutes.courses)),
          _Tile(icon: Icons.category,         title: '专业管理', subtitle: '管理专业信息',   onTap: () => push(AppRoutes.majors)),
          _Tile(icon: Icons.groups,           title: '班级管理', subtitle: '管理班级和学生', onTap: () => push(AppRoutes.classes)),
          _Tile(icon: Icons.assignment,       title: '授课任务', subtitle: '管理授课任务',   onTap: () => push(AppRoutes.teachingTasks)),
          if (auth.isAdmin) ...[
            const Divider(),
            _Tile(icon: Icons.people,              title: '用户管理',   subtitle: '管理系统用户',     onTap: () => push(AppRoutes.users)),
            _Tile(icon: Icons.admin_panel_settings, title: '角色管理',   subtitle: '管理角色和权限',   onTap: () => push(AppRoutes.roles)),
            _Tile(icon: Icons.account_tree,        title: '部门管理',   subtitle: '管理组织架构',     onTap: () => push(AppRoutes.departments)),
          ],
        ],
      ),
    );
  }

  Widget _buildLabNav() {
    final auth = Get.find<AuthController>();
    return Scaffold(
      appBar: AppBar(title: const Text('实验室')),
      body: ListView(
        children: [
          _Tile(icon: Icons.science,            title: '实验室',       subtitle: '管理实验室信息', onTap: () => push(AppRoutes.labs)),
          if (auth.isAdmin)
            _Tile(icon: Icons.devices,          title: '设备管理',   subtitle: '管理设备信息',   onTap: () => push(AppRoutes.equipments)),
          _Tile(icon: Icons.assignment_return,  title: '借用记录',   subtitle: '设备借用管理',   onTap: () => push(AppRoutes.borrowRecords)),
        ],
      ),
    );
  }

  Widget _buildScheduleNav() {
    return Scaffold(
      appBar: AppBar(title: const Text('排课预约')),
      body: ListView(
        children: [
          _Tile(icon: Icons.calendar_month, title: '排课管理',   subtitle: '排课和课表查看',     onTap: () => push(AppRoutes.schedules)),
          _Tile(icon: Icons.event_available, title: '预约管理',   subtitle: '实验室预约',         onTap: () => push(AppRoutes.reservations)),
          _Tile(icon: Icons.book_online,     title: '授课申请',   subtitle: '授课实验室申请',     onTap: () => push(AppRoutes.teachingApps)),
          _Tile(icon: Icons.edit_note,       title: '使用登记',   subtitle: '实验室使用登记',     onTap: () => push(AppRoutes.usageRecords)),
          _Tile(icon: Icons.bar_chart,       title: '统计报表',   subtitle: '数据统计分析',       onTap: () => push(AppRoutes.statistics)),
        ],
      ),
    );
  }
}

/// 菜单卡片
class _Tile extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  const _Tile({required this.icon, required this.title, required this.subtitle, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: AppTheme.primary.withOpacity(0.1),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Icon(icon, color: AppTheme.primary),
      ),
      title: Text(title, style: const TextStyle(fontWeight: FontWeight.w500)),
      subtitle: Text(subtitle, style: const TextStyle(fontSize: 12, color: AppTheme.textSecondary)),
      trailing: const Icon(Icons.chevron_right, color: AppTheme.textHint),
      onTap: onTap,
    );
  }
}

/// 空 tab 占位
class _NavPlaceholder extends StatelessWidget {
  final String title;
  final IconData icon;
  const _NavPlaceholder(this.title, this.icon);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(title)),
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 64, color: AppTheme.textHint),
            const SizedBox(height: 16),
            Text(title, style: const TextStyle(fontSize: 18, color: AppTheme.textSecondary)),
          ],
        ),
      ),
    );
  }
}
