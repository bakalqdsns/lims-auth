import 'package:dio/dio.dart';
import 'package:get/get.dart' hide Response;
import 'package:shared_preferences/shared_preferences.dart';
import '../config/api_config.dart';

/// Dio 实例管理
class ApiService extends GetxService {
  late final Dio _dio;
  static ApiService get to => Get.find();

  Dio get dio => _dio;

  Future<ApiService> init() async {
    _dio = Dio(BaseOptions(
      baseUrl: ApiConfig.effectiveBaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 30),
      sendTimeout: const Duration(seconds: 30),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    ));

    _dio.interceptors.add(_AuthInterceptor());
    _dio.interceptors.add(_LoggingInterceptor());

    return this;
  }

  /// GET 请求
  Future<Response<T>> get<T>(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
  }) {
    return _dio.get<T>(
      path,
      queryParameters: queryParameters,
      options: options,
      cancelToken: cancelToken,
    );
  }

  /// POST 请求
  Future<Response<T>> post<T>(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
  }) {
    return _dio.post<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
      cancelToken: cancelToken,
    );
  }

  /// PUT 请求
  Future<Response<T>> put<T>(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
  }) {
    return _dio.put<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
      cancelToken: cancelToken,
    );
  }

  /// PATCH 请求
  Future<Response<T>> patch<T>(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
  }) {
    return _dio.patch<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
      cancelToken: cancelToken,
    );
  }

  /// DELETE 请求
  Future<Response<T>> delete<T>(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
    CancelToken? cancelToken,
  }) {
    return _dio.delete<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
      cancelToken: cancelToken,
    );
  }

  /// 下载文件
  Future<Response> download(
    String path,
    String savePath, {
    ProgressCallback? onReceiveProgress,
    CancelToken? cancelToken,
  }) {
    return _dio.download(path, savePath,
        onReceiveProgress: onReceiveProgress, cancelToken: cancelToken);
  }
}

/// Token 认证拦截器
class _AuthInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    // 跳过登录页面的 token 注入
    final noAuthPaths = ['/auth/login', '/auth/health'];
    final isNoAuth = noAuthPaths.any((p) => options.path.toLowerCase().contains(p.toLowerCase()));

    if (!isNoAuth) {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('token');
      if (token != null && token.isNotEmpty) {
        options.headers['Authorization'] = 'Bearer $token';
      }
    }
    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode == 401) {
      // Token 过期，清除并跳转登录
      final prefs = await SharedPreferences.getInstance();
      await prefs.remove('token');
      await prefs.remove('user');
      // 避免重复跳转
      if (!err.requestOptions.path.contains('/auth/login')) {
        Get.offAllNamed('/');
        Get.snackbar('登录已过期', '请重新登录',
            snackPosition: SnackPosition.TOP);
      }
    }
    handler.next(err);
  }
}

/// 日志拦截器
class _LoggingInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    // ignore: avoid_print
    print('[API] ${options.method} ${options.path}');
    handler.next(options);
  }

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    // ignore: avoid_print
    print('[API] ${response.statusCode} ${response.requestOptions.path}');
    handler.next(response);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    // ignore: avoid_print
    print('[API] Error: ${err.type} ${err.message}');
    handler.next(err);
  }
}

/// API 错误处理
class ApiException implements Exception {
  final int code;
  final String message;
  final dynamic originalError;

  ApiException({
    required this.code,
    required this.message,
    this.originalError,
  });

  factory ApiException.fromDio(DioException err) {
    String msg;
    switch (err.type) {
      case DioExceptionType.connectionTimeout:
        msg = '连接超时，请检查网络';
        break;
      case DioExceptionType.sendTimeout:
        msg = '发送请求超时';
        break;
      case DioExceptionType.receiveTimeout:
        msg = '接收响应超时';
        break;
      case DioExceptionType.badResponse:
        final statusCode = err.response?.statusCode;
        if (statusCode == 401) {
          msg = '登录已过期，请重新登录';
        } else if (statusCode == 403) {
          msg = '无权限访问';
        } else if (statusCode == 404) {
          msg = '请求的资源不存在';
        } else if (statusCode != null && statusCode >= 500) {
          msg = '服务器错误 ($statusCode)';
        } else {
          final data = err.response?.data;
          if (data is Map && data['message'] != null) {
            msg = data['message'].toString();
          } else {
            msg = '请求失败 ($statusCode)';
          }
        }
        break;
      case DioExceptionType.cancel:
        msg = '请求已取消';
        break;
      case DioExceptionType.connectionError:
        msg = '网络连接失败，请检查网络';
        break;
      default:
        msg = err.message ?? '未知错误';
    }
    return ApiException(
      code: err.response?.statusCode ?? -1,
      message: msg,
      originalError: err,
    );
  }

  @override
  String toString() => 'ApiException($code): $message';
}
