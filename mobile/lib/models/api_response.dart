/// 閫氱敤 API 鍝嶅簲灏佽

import '../utils/num_utils.dart';

class ApiResponse<T> {
  final int code;
  final String message;
  final T? data;

  ApiResponse({required this.code, required this.message, this.data});

  bool get isSuccess => code == 200;

  factory ApiResponse.fromJson(
    Map<String, dynamic> json,
    T Function(dynamic)? fromJsonT,
  ) {
    return ApiResponse(
      code: safeInt(json['code']),
      message: json['message'] as String? ?? '',
      data: json['data'] != null && fromJsonT != null
          ? fromJsonT(json['data'])
          : json['data'] as T?,
    );
  }
}

/// 鍒嗛〉鍝嶅簲
class PagedResponse<T> {
  final List<T> items;
  final int total;
  final int page;
  final int pageSize;

  PagedResponse({
    required this.items,
    required this.total,
    required this.page,
    required this.pageSize,
  });

  int get totalPages => (total / pageSize).ceil();
  bool get hasNextPage => page < totalPages;
  bool get hasPrevPage => page > 1;

  factory PagedResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJsonT,
  ) {
    return PagedResponse(
      items: (json['items'] as List<dynamic>?)
              ?.map((e) => fromJsonT(e as Map<String, dynamic>))
              .toList() ??
          [],
      total: safeInt(json['total']),
      page: safeInt(json['page'], 1),
      pageSize: safeInt(json['pageSize'], 10),
    );
  }
}
