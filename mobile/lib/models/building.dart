import '../utils/num_utils.dart';

class Building {
  final String id;
  final String code;
  final String name;
  final String? campusId;
  final String? address;
  final int floorCount;
  final double? buildingArea;
  final String? buildingType;
  final int builtYear;
  final String? managerId;
  final String? description;
  final bool isActive;
  final String? createdAt;

  Building({
    required this.id,
    required this.code,
    required this.name,
    this.campusId,
    this.address,
    this.floorCount = 0,
    this.buildingArea,
    this.buildingType,
    this.builtYear = 0,
    this.managerId,
    this.description,
    this.isActive = true,
    this.createdAt,
  });

  factory Building.fromJson(Map<String, dynamic> json) {
    return Building(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      campusId: json['campusId']?.toString(),
      address: json['address']?.toString(),
      floorCount: safeInt(json['floorCount']),
      buildingArea: (json['buildingArea'] as num?)?.toDouble(),
      buildingType: json['buildingType']?.toString(),
      builtYear: safeInt(json['builtYear']),
      managerId: json['managerId']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'campusId': campusId,
        'address': address,
        'floorCount': floorCount,
        'buildingArea': buildingArea,
        'buildingType': buildingType,
        'builtYear': builtYear,
        'managerId': managerId,
        'description': description,
      };
}
