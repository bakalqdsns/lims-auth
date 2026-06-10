import 'dart:io';
import 'package:dio/dio.dart' hide MultipartFile;
import 'package:dio/dio.dart' as dio show MultipartFile;
import 'package:get/get.dart' hide Response, FormData;
import 'package:path_provider/path_provider.dart';
import '../models/models.dart';
import 'api_service.dart';

class EquipmentService {
  ApiService get _api => ApiService.to;

  Future<List<Equipment>> getEquipments({
    String? keyword,
    String? labId,
    String? category,
    String? status,
    bool? isActive,
    int page = 1,
    int pageSize = 20,
  }) async {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };
    if (keyword != null) params['keyword'] = keyword;
    if (labId != null) params['labId'] = labId;
    if (category != null) params['category'] = category;
    if (status != null) params['status'] = status;
    if (isActive != null) params['isActive'] = isActive;

    final resp = await _api.get('/equipments', queryParameters: params);
    final data = resp.data;
    if (data['code'] == 200) {
      final items = data['data']?['items'] ?? data['data'] ?? [];
      if (items is List) {
        return items.map((e) => Equipment.fromJson(e as Map<String, dynamic>)).toList();
      }
    }
    return [];
  }

  Future<Equipment?> getEquipmentById(String id) async {
    final resp = await _api.get('/equipments/$id');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Equipment.fromJson(data['data'] as Map<String, dynamic>);
    }
    return null;
  }

  Future<Equipment?> createEquipment(Equipment req) async {
    final resp = await _api.post('/equipments', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return Equipment.fromJson(data['data'] as Map<String, dynamic>);
    }
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '创建设备失败');
  }

  Future<bool> updateEquipment(String id, Equipment req) async {
    final resp = await _api.put('/equipments/$id', data: req.toJson());
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '更新设备失败');
  }

  Future<bool> deleteEquipment(String id) async {
    final resp = await _api.delete('/equipments/$id');
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '删除设备失败');
  }

  Future<bool> toggleEquipmentStatus(String id, bool isActive) async {
    final resp = await _api.patch('/equipments/$id/status', data: {'isActive': isActive});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改状态失败');
  }

  Future<bool> updateEquipmentStatus(String id, String status) async {
    final resp = await _api.patch('/equipments/$id/equipment-status', data: {'status': status});
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '修改设备状态失败');
  }

  Future<EquipmentStatistics> getStatistics() async {
    final resp = await _api.get('/equipments/statistics');
    final data = resp.data;
    if (data['code'] == 200 && data['data'] != null) {
      return EquipmentStatistics.fromJson(data['data'] as Map<String, dynamic>);
    }
    return EquipmentStatistics();
  }

  /// 导入设备（上传 Excel 文件）
  Future<bool> importExcel(File file) async {
    final formData = FormData.fromMap({
      'file': await dio.MultipartFile.fromFile(file.path, filename: file.path.split(Platform.pathSeparator).last),
    });
    final resp = await _api.post('/equipments/import', data: formData);
    final data = resp.data;
    if (data['code'] == 200) return true;
    throw ApiException(code: data['code'] ?? -1, message: data['message'] ?? '导入失败');
  }

  /// 导出设备列表
  Future<File?> exportExcel() async {
    try {
      final dir = await getApplicationDocumentsDirectory();
      final savePath = '${dir.path}/equipments_export.xlsx';
      await _api.download('/equipments/export', savePath);
      return File(savePath);
    } catch (_) {
      return null;
    }
  }

  /// 下载导入模板
  Future<File?> downloadTemplate() async {
    try {
      final dir = await getApplicationDocumentsDirectory();
      final savePath = '${dir.path}/equipment_template.xlsx';
      await _api.download('/equipments/import-template', savePath);
      return File(savePath);
    } catch (_) {
      return null;
    }
  }
}
