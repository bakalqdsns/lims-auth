import 'dart:io';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';

class NetworkDiagnostics {
  /// 运行完整的网络诊断
  static Future<Map<String, dynamic>> runDiagnostics() async {
    final results = <String, dynamic>{};
    
    // 1. 检查当前配置
    results['config'] = {
      'baseUrl': ApiConfig.effectiveBaseUrl,
      'rawBaseUrl': ApiConfig.baseUrl,
    };
    
    // 2. 测试连接
    final connectionTest = await _testConnection(ApiConfig.effectiveBaseUrl);
    results['connection'] = connectionTest;
    
    // 3. 获取网络信息
    results['network'] = await _getNetworkInfo();
    
    return results;
  }
  
  /// 测试连接到指定 URL
  static Future<Map<String, dynamic>> _testConnection(String baseUrl) async {
    try {
      final url = Uri.parse('$baseUrl/auth/login');
      final stopwatch = Stopwatch()..start();
      
      final response = await http.get(url).timeout(
        const Duration(seconds: 5),
        onTimeout: () => throw Exception('连接超时'),
      );
      
      stopwatch.stop();
      
      return {
        'success': true,
        'statusCode': response.statusCode,
        'responseTime': '${stopwatch.elapsedMilliseconds}ms',
        'url': url.toString(),
      };
    } catch (e) {
      return {
        'success': false,
        'error': e.toString(),
        'url': '$baseUrl/auth/login',
      };
    }
  }
  
  /// 获取网络信息
  static Future<Map<String, dynamic>> _getNetworkInfo() async {
    final info = <String, dynamic>{};
    
    try {
      final interfaces = await NetworkInterface.list();
      info['interfaces'] = interfaces.map((interface) => {
        'name': interface.name,
        'addresses': interface.addresses.map((addr) => addr.address).toList(),
      }).toList();
    } catch (e) {
      info['interfaces_error'] = e.toString();
    }
    
    return info;
  }
  
  /// 生成诊断报告文本
  static String generateReport(Map<String, dynamic> results) {
    final buffer = StringBuffer();
    buffer.writeln('=== 网络诊断报告 ===');
    buffer.writeln();
    
    // 配置信息
    buffer.writeln('【配置信息】');
    final config = results['config'] as Map<String, dynamic>;
    buffer.writeln('当前 API 地址: ${config['baseUrl']}');
    buffer.writeln();
    
    // 连接测试
    buffer.writeln('【连接测试】');
    final connection = results['connection'] as Map<String, dynamic>;
    if (connection['success'] == true) {
      buffer.writeln('✅ 连接成功');
      buffer.writeln('状态码: ${connection['statusCode']}');
      buffer.writeln('响应时间: ${connection['responseTime']}');
    } else {
      buffer.writeln('❌ 连接失败');
      buffer.writeln('错误: ${connection['error']}');
      buffer.writeln();
      buffer.writeln('可能的原因:');
      buffer.writeln('1. 服务器未启动');
      buffer.writeln('2. 服务器地址配置错误');
      buffer.writeln('3. 手机和服务器不在同一网络');
      buffer.writeln('4. 防火墙阻止了连接');
    }
    buffer.writeln();
    
    // 网络信息
    buffer.writeln('【本机网络信息】');
    final network = results['network'] as Map<String, dynamic>;
    if (network.containsKey('interfaces')) {
      final interfaces = network['interfaces'] as List;
      for (final interface in interfaces) {
        buffer.writeln('接口: ${interface['name']}');
        buffer.writeln('  IP: ${(interface['addresses'] as List).join(', ')}');
      }
    }
    
    return buffer.toString();
  }
}
