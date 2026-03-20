import 'dart:io' show Platform;
import 'package:flutter/foundation.dart' show kIsWeb;

class ApiConfig {
  // 开发环境配置
  static const String _devEmulatorUrl = 'http://10.0.2.2:5047';
  static const String _devDeviceUrl = 'http://192.168.1.100:5047';
  static const String _devLocalhostUrl = 'http://localhost:5047';
  
  // 生产环境配置（需要替换为实际的服务器地址）
  static const String _prodUrl = 'https://your-production-api.com';
  
  /// 获取基础 API URL
  /// 
  /// 使用方式：
  /// - 模拟器：自动使用 10.0.2.2
  /// - 真机：需要修改为实际的服务器 IP
  /// - Web：使用 localhost
  static String get baseUrl {
    if (kIsWeb) {
      return '$_devLocalhostUrl/api/v1';
    }
    
    // 检测是否在模拟器中运行
    if (_isEmulator) {
      return '$_devEmulatorUrl/api/v1';
    }
    
    // 真机 - 使用配置的服务器地址
    // TODO: 修改为实际的服务器 IP 地址
    return '$_devDeviceUrl/api/v1';
  }
  
  /// 设置自定义服务器地址（用于测试不同环境）
  static String customBaseUrl = '';
  
  static String get effectiveBaseUrl {
    if (customBaseUrl.isNotEmpty) {
      return '$customBaseUrl/api/v1';
    }
    return baseUrl;
  }
  
  /// 检测是否在 Android 模拟器中
  static bool get _isEmulator {
    if (Platform.isAndroid) {
      // 通过检查设备特征来判断是否在模拟器中
      return _checkAndroidEmulator();
    }
    return false;
  }
  
  static bool _checkAndroidEmulator() {
    // 简化的检测逻辑
    // 实际项目中可以使用 device_info_plus 包来获取更准确的信息
    return false; // 默认假设是真机，避免连接问题
  }
  
  /// 获取当前使用的完整 API URL（用于调试）
  static String get debugInfo => '''
API 配置信息：
- 当前基础 URL: $effectiveBaseUrl
- 是否 Web: $kIsWeb
- 平台: ${Platform.operatingSystem}
- 是模拟器: $_isEmulator
''';
}
