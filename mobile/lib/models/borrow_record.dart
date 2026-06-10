import 'equipment.dart';
import '../utils/num_utils.dart';

class BorrowRecord {
  final String id;
  final String? recordNo;
  final String? equipmentId;
  final String? equipmentName;
  final String? borrowerId;
  final String? borrowerName;
  final String? borrowDate;
  final String? expectedReturnDate;
  final String? actualReturnDate;
  final String? purpose;
  final String? phone;
  final String? usageLocation;
  final String? status;
  final String? remarks;
  final String? createdAt;
  final BorrowEquipment? equipment;

  BorrowRecord({
    required this.id,
    this.recordNo,
    this.equipmentId,
    this.equipmentName,
    this.borrowerId,
    this.borrowerName,
    this.borrowDate,
    this.expectedReturnDate,
    this.actualReturnDate,
    this.purpose,
    this.phone,
    this.usageLocation,
    this.status,
    this.remarks,
    this.createdAt,
    this.equipment,
  });

  factory BorrowRecord.fromJson(Map<String, dynamic> json) {
    return BorrowRecord(
      id: json['id']?.toString() ?? '',
      recordNo: json['recordNo']?.toString(),
      equipmentId: json['equipmentId']?.toString(),
      equipmentName: json['equipmentName']?.toString(),
      borrowerId: json['borrowerId']?.toString(),
      borrowerName: json['borrowerName']?.toString(),
      borrowDate: json['borrowDate']?.toString(),
      expectedReturnDate: json['expectedReturnDate']?.toString(),
      actualReturnDate: json['actualReturnDate']?.toString(),
      purpose: json['purpose']?.toString(),
      phone: json['phone']?.toString(),
      usageLocation: json['usageLocation']?.toString(),
      status: json['status']?.toString(),
      remarks: json['remarks']?.toString(),
      createdAt: json['createdAt']?.toString(),
      equipment: json['equipment'] != null
          ? BorrowEquipment.fromJson(json['equipment'] as Map<String, dynamic>)
          : null,
    );
  }
}

/// 鍊熺敤璁板綍涓殑璁惧绠€鐣ヤ俊鎭紙閬垮厤涓?equipment.dart 涓殑 Equipment 鍐茬獊锛?
class BorrowEquipment {
  final String id;
  final String code;
  final String name;
  final String? model;
  final String? category;
  final String? status;
  final int availableQuantity;

  BorrowEquipment({
    required this.id,
    required this.code,
    required this.name,
    this.model,
    this.category,
    this.status,
    this.availableQuantity = 0,
  });

  factory BorrowEquipment.fromJson(Map<String, dynamic> json) {
    return BorrowEquipment(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      model: json['model']?.toString(),
      category: json['category']?.toString(),
      status: json['status']?.toString(),
      availableQuantity: safeInt(json['availableQuantity']),
    );
  }
}

class CreateBorrowRequest {
  final String equipmentId;
  final String borrowDate;
  final String expectedReturnDate;
  final String purpose;
  final String phone;
  final String? usageLocation;
  final String? remarks;

  CreateBorrowRequest({
    required this.equipmentId,
    required this.borrowDate,
    required this.expectedReturnDate,
    required this.purpose,
    required this.phone,
    this.usageLocation,
    this.remarks,
  });

  Map<String, dynamic> toJson() => {
        'equipmentId': equipmentId,
        'borrowDate': borrowDate,
        'expectedReturnDate': expectedReturnDate,
        'purpose': purpose,
        'phone': phone,
        'usageLocation': usageLocation,
        'remarks': remarks,
      };
}

class BorrowApprovalRequest {
  final bool approved;
  final String? remark;

  BorrowApprovalRequest({required this.approved, this.remark});

  Map<String, dynamic> toJson() => {'approved': approved, 'remark': remark};
}

class RenewRequest {
  final String newReturnDate;

  RenewRequest({required this.newReturnDate});

  Map<String, dynamic> toJson() => {'newReturnDate': newReturnDate};
}

class ReturnConfirmRequest {
  final String? condition;
  final String? remarks;

  ReturnConfirmRequest({this.condition, this.remarks});

  Map<String, dynamic> toJson() => {'condition': condition, 'remarks': remarks};
}

class BorrowStatus {
  static const String pending = 'Pending';
  static const String supervisorApproved = 'SupervisorApproved';
  static const String adminApproved = 'AdminApproved';
  static const String borrowed = 'Borrowed';
  static const String returning = 'Returning';
  static const String returned = 'Returned';
  static const String rejected = 'Rejected';
  static const String cancelled = 'Cancelled';
  static const String overdue = 'Overdue';
}
