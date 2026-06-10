import 'lab.dart';
import '../utils/num_utils.dart';

class Equipment {
  final String id;
  final String code;
  final String name;
  final String? model;
  final String? manufacturer;
  final String? serialNumber;
  final String? labId;
  final String? category;
  final String? unit;
  final String? status;
  final String? purchaseDate;
  final int warrantyMonths;
  final double price;
  final String? location;
  final String? imageUrl;
  final String? instructions;
  final bool requiresBooking;
  final int maxBookingHours;
  final int totalQuantity;
  final int availableQuantity;
  final String? brand;
  final String? supplier;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final String? updatedAt;
  final Lab? lab;

  Equipment({
    required this.id,
    required this.code,
    required this.name,
    this.model,
    this.manufacturer,
    this.serialNumber,
    this.labId,
    this.category,
    this.unit,
    this.status,
    this.purchaseDate,
    this.warrantyMonths = 0,
    this.price = 0,
    this.location,
    this.imageUrl,
    this.instructions,
    this.requiresBooking = false,
    this.maxBookingHours = 0,
    this.totalQuantity = 0,
    this.availableQuantity = 0,
    this.brand,
    this.supplier,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.updatedAt,
    this.lab,
  });

  factory Equipment.fromJson(Map<String, dynamic> json) {
    return Equipment(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      model: json['model']?.toString(),
      manufacturer: json['manufacturer']?.toString(),
      serialNumber: json['serialNumber']?.toString(),
      labId: json['labId']?.toString(),
      category: json['category']?.toString(),
      unit: json['unit']?.toString(),
      status: json['status']?.toString(),
      purchaseDate: json['purchaseDate']?.toString(),
      warrantyMonths: safeInt(json['warrantyMonths']),
      price: safeDouble(json['price']),
      location: json['location']?.toString(),
      imageUrl: json['imageUrl']?.toString(),
      instructions: json['instructions']?.toString(),
      requiresBooking: json['requiresBooking'] as bool? ?? false,
      maxBookingHours: safeInt(json['maxBookingHours']),
      totalQuantity: safeInt(json['totalQuantity']),
      availableQuantity: safeInt(json['availableQuantity']),
      brand: json['brand']?.toString(),
      supplier: json['supplier']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      updatedAt: json['updatedAt']?.toString(),
      lab: json['lab'] != null
          ? Lab.fromJson(json['lab'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'model': model,
        'manufacturer': manufacturer,
        'serialNumber': serialNumber,
        'labId': labId,
        'category': category,
        'unit': unit,
        'status': status,
        'purchaseDate': purchaseDate,
        'warrantyMonths': warrantyMonths,
        'price': price,
        'location': location,
        'imageUrl': imageUrl,
        'instructions': instructions,
        'requiresBooking': requiresBooking,
        'maxBookingHours': maxBookingHours,
        'totalQuantity': totalQuantity,
        'availableQuantity': availableQuantity,
        'brand': brand,
        'supplier': supplier,
        'description': description,
      };
}

class EquipmentStatistics {
  final int totalCount;
  final int availableCount;
  final int inUseCount;
  final int maintenanceCount;

  EquipmentStatistics({
    this.totalCount = 0,
    this.availableCount = 0,
    this.inUseCount = 0,
    this.maintenanceCount = 0,
  });

  factory EquipmentStatistics.fromJson(Map<String, dynamic> json) {
    return EquipmentStatistics(
      totalCount: safeInt(json['totalCount']),
      availableCount: safeInt(json['availableCount']),
      inUseCount: safeInt(json['inUseCount']),
      maintenanceCount: safeInt(json['maintenanceCount']),
    );
  }
}
