import 'department.dart';
import '../utils/num_utils.dart';

class Course {
  final String id;
  final String code;
  final String name;
  final String? englishName;
  final String? courseType;
  final double credits;
  final int totalHours;
  final int theoryHours;
  final int practiceHours;
  final int experimentHours;
  final int onlineHours;
  final int? semesterType;
  final String? departmentId;
  final String? managerId;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final Department? department;
  final dynamic manager;

  Course({
    required this.id,
    required this.code,
    required this.name,
    this.englishName,
    this.courseType,
    this.credits = 0,
    this.totalHours = 0,
    this.theoryHours = 0,
    this.practiceHours = 0,
    this.experimentHours = 0,
    this.onlineHours = 0,
    this.semesterType,
    this.departmentId,
    this.managerId,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.department,
    this.manager,
  });

  factory Course.fromJson(Map<String, dynamic> json) {
    return Course(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      englishName: json['englishName']?.toString(),
      courseType: json['courseType']?.toString(),
      credits: (json['credits'] as num?)?.toDouble() ?? 0,
      totalHours: safeInt(json['totalHours']),
      theoryHours: safeInt(json['theoryHours']),
      practiceHours: safeInt(json['practiceHours']),
      experimentHours: safeInt(json['experimentHours']),
      onlineHours: safeInt(json['onlineHours']),
      semesterType: json['semesterType'] as int?,
      departmentId: json['departmentId']?.toString(),
      managerId: json['managerId']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      department: json['department'] != null
          ? Department.fromJson(json['department'] as Map<String, dynamic>)
          : null,
      manager: json['manager'],
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'englishName': englishName,
        'courseType': courseType,
        'credits': credits,
        'totalHours': totalHours,
        'theoryHours': theoryHours,
        'practiceHours': practiceHours,
        'experimentHours': experimentHours,
        'onlineHours': onlineHours,
        'semesterType': semesterType,
        'departmentId': departmentId,
        'managerId': managerId,
        'description': description,
      };
}
