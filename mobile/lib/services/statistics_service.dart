import 'dart:io';
import 'package:get/get.dart' hide Response;
import 'package:path_provider/path_provider.dart';
import '../models/models.dart';
import 'api_service.dart';

class StatisticsService {
  ApiService get _api => ApiService.to;

  Future<DashboardData> getDashboard() async {
    final resp = await _api.get('/statistics/dashboard');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return DashboardData.fromJson(data['data'] as Map<String, dynamic>);
    }
    return DashboardData();
  }

  Future<List<StatisticsByCategory>> getByClass({String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/statistics/by-class', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => StatisticsByCategory.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<StatisticsByCategory>> getByMajor({String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/statistics/by-major', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => StatisticsByCategory.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<StatisticsByCategory>> getByCourse({String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/statistics/by-course', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => StatisticsByCategory.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<StatisticsByCategory>> getByGrade({String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/statistics/by-grade', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => StatisticsByCategory.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<List<LabUsage>> getLabUsage({String? semesterId}) async {
    final params = <String, dynamic>{};
    if (semesterId != null) params['semesterId'] = semesterId;
    final resp = await _api.get('/statistics/lab-usage', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => LabUsage.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<ReservationStats> getReservationStats() async {
    final resp = await _api.get('/statistics/reservation');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return ReservationStats.fromJson(data['data'] as Map<String, dynamic>);
    }
    return ReservationStats();
  }

  Future<CompletionRate> getCompletionRate() async {
    final resp = await _api.get('/statistics/completion-rate');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return CompletionRate.fromJson(data['data'] as Map<String, dynamic>);
    }
    return CompletionRate();
  }

  Future<List<WeeklySummary>> getWeeklySummary({int weeks = 8}) async {
    final resp = await _api.get('/statistics/weekly-summary', queryParameters: {'weeks': weeks});
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => WeeklySummary.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<File?> exportExcel() async {
    try {
      final dir = await getApplicationDocumentsDirectory();
      final savePath = '${dir.path}/statistics_export.xlsx';
      await _api.download('/statistics/export', savePath);
      return File(savePath);
    } catch (_) {
      return null;
    }
  }
}
