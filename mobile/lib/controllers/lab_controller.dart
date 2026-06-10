import 'package:get/get.dart';
import '../models/models.dart';
import '../services/services.dart';

class LabController extends GetxController {
  final _labService = LabService();
  final _equipmentService = EquipmentService();

  final labs = <Lab>[].obs;
  final selectedLab = Rxn<Lab>();
  final isLoading = false.obs;
  final error = RxnString();
  final keyword = ''.obs;
  final selectedBuildingId = RxnString();

  Future<void> loadLabs({bool refresh = false}) async {
    if (refresh) labs.clear();
    isLoading.value = true;
    error.value = null;
    try {
      labs.value = await _labService.getLabs(
        keyword: keyword.value.isEmpty ? null : keyword.value,
        buildingId: selectedBuildingId.value,
      );
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<bool> createLab(CreateLabRequest req) async {
    try {
      final lab = await _labService.createLab(req);
      if (lab != null) {
        labs.insert(0, lab);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> updateLab(String id, CreateLabRequest req) async {
    try {
      final ok = await _labService.updateLab(id, req);
      if (ok) await loadLabs(refresh: true);
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> deleteLab(String id) async {
    try {
      final ok = await _labService.deleteLab(id);
      if (ok) labs.removeWhere((l) => l.id == id);
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> toggleStatus(String id, bool isActive) async {
    try {
      return await _labService.toggleLabStatus(id, isActive);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }
}

class EquipmentController extends GetxController {
  final _service = EquipmentService();

  final equipments = <Equipment>[].obs;
  final statistics = Rxn<EquipmentStatistics>();
  final isLoading = false.obs;
  final error = RxnString();
  final keyword = ''.obs;
  final selectedCategory = RxnString();
  final selectedStatus = RxnString();

  Future<void> loadEquipments({bool refresh = false}) async {
    if (refresh) equipments.clear();
    isLoading.value = true;
    error.value = null;
    try {
      equipments.value = await _service.getEquipments(
        keyword: keyword.value.isEmpty ? null : keyword.value,
        category: selectedCategory.value,
        status: selectedStatus.value,
      );
    } on ApiException catch (e) {
      error.value = e.message;
    } finally {
      isLoading.value = false;
    }
  }

  Future<void> loadStatistics() async {
    try {
      statistics.value = await _service.getStatistics();
    } catch (_) {}
  }

  Future<bool> createEquipment(Equipment req) async {
    try {
      final eq = await _service.createEquipment(req);
      if (eq != null) {
        equipments.insert(0, eq);
        return true;
      }
      return false;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> updateEquipment(String id, Equipment req) async {
    try {
      final ok = await _service.updateEquipment(id, req);
      if (ok) await loadEquipments(refresh: true);
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> deleteEquipment(String id) async {
    try {
      final ok = await _service.deleteEquipment(id);
      if (ok) equipments.removeWhere((e) => e.id == id);
      return ok;
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }

  Future<bool> updateStatus(String id, String status) async {
    try {
      return await _service.updateEquipmentStatus(id, status);
    } on ApiException catch (e) {
      error.value = e.message;
      return false;
    }
  }
}
