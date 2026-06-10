import 'department.dart';
import '../utils/num_utils.dart';

class Major {
  final String id;
  final String code;
  final String name;
  final String? englishName;
  final String? departmentId;
  final int duration;
  final String? degreeType;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final Department? department;

  Major({
    required this.id,
    required this.code,
    required this.name,
    this.englishName,
    this.departmentId,
    this.duration = 0,
    this.degreeType,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.department,
  });

  factory Major.fromJson(Map<String, dynamic> json) {
    return Major(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      englishName: json['englishName']?.toString(),
      departmentId: json['departmentId']?.toString(),
      duration: safeInt(json['duration']),
      degreeType: json['degreeType']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      department: json['department'] != null
          ? Department.fromJson(json['department'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'englishName': englishName,
        'departmentId': departmentId,
        'duration': duration,
        'degreeType': degreeType,
        'description': description,
      };
}
