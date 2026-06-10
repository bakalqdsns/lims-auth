import 'building.dart';

class Campus {
  final String id;
  final String code;
  final String name;
  final String? address;
  final double? area;
  final String? campusType;
  final String? contactPhone;
  final String? managerId;
  final String? description;
  final bool isActive;
  final String? createdAt;

  Campus({
    required this.id,
    required this.code,
    required this.name,
    this.address,
    this.area,
    this.campusType,
    this.contactPhone,
    this.managerId,
    this.description,
    this.isActive = true,
    this.createdAt,
  });

  factory Campus.fromJson(Map<String, dynamic> json) {
    return Campus(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      address: json['address']?.toString(),
      area: (json['area'] as num?)?.toDouble(),
      campusType: json['campusType']?.toString(),
      contactPhone: json['contactPhone']?.toString(),
      managerId: json['managerId']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'address': address,
        'area': area,
        'campusType': campusType,
        'contactPhone': contactPhone,
        'managerId': managerId,
        'description': description,
      };
}
