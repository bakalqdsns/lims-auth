import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'services/api_service.dart';
import 'controllers/auth_controller.dart';
import 'controllers/statistics_controller.dart';
import 'controllers/app_controller.dart';
import 'controllers/semester_controller.dart';
import 'routes/routes.dart';
import 'utils/theme/app_theme.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // 初始化 API 服务
  await Get.putAsync(() => ApiService().init());

  // 预注册全局控制器
  Get.put(AuthController());
  Get.put(AppController());
  Get.put(StatisticsController());
  Get.put(SemesterController());

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
      defaultTransition: Transition.fadeIn,
      transitionDuration: const Duration(milliseconds: 200),
    );
  }
}
