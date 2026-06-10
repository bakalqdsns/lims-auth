import 'building.dart';
import '../utils/num_utils.dart';

class Lab {
  final String id;
  final String code;
  final String name;
  final String? departmentId;
  final String? buildingId;
  final int floor;
  final String? roomNumber;
  final String? location;
  final int capacity;
  final String? labType;
  final String? safetyLevel;
  final String? managerId;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final int? seatCount;
  final double? area;
  final String? roomType;
  final String? photo;
  final bool isAvailable;
  final Building? building;

  Lab({
    required this.id,
    required this.code,
    required this.name,
    this.departmentId,
    this.buildingId,
    this.floor = 0,
    this.roomNumber,
    this.location,
    this.capacity = 0,
    this.labType,
    this.safetyLevel,
    this.managerId,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.seatCount,
    this.area,
    this.roomType,
    this.photo,
    this.isAvailable = true,
    this.building,
  });

  factory Lab.fromJson(Map<String, dynamic> json) {
    return Lab(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      departmentId: json['departmentId']?.toString(),
      buildingId: json['buildingId']?.toString(),
      floor: safeInt(json['floor']),
      roomNumber: json['roomNumber']?.toString(),
      location: json['location']?.toString(),
      capacity: safeInt(json['capacity']),
      labType: json['labType']?.toString(),
      safetyLevel: json['safetyLevel']?.toString(),
      managerId: json['managerId']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      seatCount: json['seatCount'] as int?,
      area: (json['area'] as num?)?.toDouble(),
      roomType: json['roomType']?.toString(),
      photo: json['photo']?.toString(),
      isAvailable: json['isAvailable'] as bool? ?? true,
      building: json['building'] != null
          ? Building.fromJson(json['building'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'departmentId': departmentId,
        'buildingId': buildingId,
        'floor': floor,
        'roomNumber': roomNumber,
        'location': location,
        'capacity': capacity,
        'labType': labType,
        'safetyLevel': safetyLevel,
        'managerId': managerId,
        'description': description,
      };
}

class CreateLabRequest {
  final String code;
  final String name;
  final String? departmentId;
  final String? buildingId;
  final int floor;
  final String? roomNumber;
  final String? location;
  final int capacity;
  final String? labType;
  final String? safetyLevel;
  final String? managerId;
  final String? description;

  CreateLabRequest({
    required this.code,
    required this.name,
    this.departmentId,
    this.buildingId,
    this.floor = 0,
    this.roomNumber,
    this.location,
    this.capacity = 0,
    this.labType,
    this.safetyLevel,
    this.managerId,
    this.description,
  });

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'departmentId': departmentId,
        'buildingId': buildingId,
        'floor': floor,
        'roomNumber': roomNumber,
        'location': location,
        'capacity': capacity,
        'labType': labType,
        'safetyLevel': safetyLevel,
        'managerId': managerId,
        'description': description,
      };
}
