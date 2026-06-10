import 'department.dart';
import 'major.dart';
import '../utils/num_utils.dart';
import 'user.dart' hide Department;

class ClassModel {
  final String id;
  final String code;
  final String name;
  final String? grade;
  final String? majorId;
  final String? departmentId;
  final String? headTeacherId;
  final String? adminStudentId;
  final int studentCount;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final Major? major;
  final Department? department;
  final User? headTeacher;
  final User? adminStudent;

  ClassModel({
    required this.id,
    required this.code,
    required this.name,
    this.grade,
    this.majorId,
    this.departmentId,
    this.headTeacherId,
    this.adminStudentId,
    this.studentCount = 0,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.major,
    this.department,
    this.headTeacher,
    this.adminStudent,
  });

  factory ClassModel.fromJson(Map<String, dynamic> json) {
    return ClassModel(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      grade: json['grade']?.toString(),
      majorId: json['majorId']?.toString(),
      departmentId: json['departmentId']?.toString(),
      headTeacherId: json['headTeacherId']?.toString(),
      adminStudentId: json['adminStudentId']?.toString(),
      studentCount: safeInt(json['studentCount']),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      major: json['major'] != null
          ? Major.fromJson(json['major'] as Map<String, dynamic>)
          : null,
      department: json['department'] != null
          ? Department.fromJson(json['department'] as Map<String, dynamic>)
          : null,
      headTeacher: json['headTeacher'] != null
          ? User.fromJson(json['headTeacher'] as Map<String, dynamic>)
          : null,
      adminStudent: json['adminStudent'] != null
          ? User.fromJson(json['adminStudent'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'grade': grade,
        'majorId': majorId,
        'departmentId': departmentId,
        'headTeacherId': headTeacherId,
        'adminStudentId': adminStudentId,
        'description': description,
      };
}

class ClassStudent {
  final String? classId;
  final String? studentId;
  final String? joinedAt;
  final User? student;

  ClassStudent({this.classId, this.studentId, this.joinedAt, this.student});

  factory ClassStudent.fromJson(Map<String, dynamic> json) {
    return ClassStudent(
      classId: json['classId']?.toString(),
      studentId: json['studentId']?.toString(),
      joinedAt: json['joinedAt']?.toString(),
      student: json['student'] != null
          ? User.fromJson(json['student'] as Map<String, dynamic>)
          : null,
    );
  }
}
