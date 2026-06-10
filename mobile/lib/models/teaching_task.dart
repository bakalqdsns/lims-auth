import 'semester.dart';
import 'course.dart';
import 'class_model.dart';
import 'user.dart';

class TeachingTask {
  final String id;
  final String? semesterId;
  final String? courseId;
  final String? classId;
  final String? taskType;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final Semester? semester;
  final Course? course;
  final ClassModel? classModel;
  final List<TeachingTaskTeacher>? teachers;

  TeachingTask({
    required this.id,
    this.semesterId,
    this.courseId,
    this.classId,
    this.taskType,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.semester,
    this.course,
    this.classModel,
    this.teachers,
  });

  factory TeachingTask.fromJson(Map<String, dynamic> json) {
    return TeachingTask(
      id: json['id']?.toString() ?? '',
      semesterId: json['semesterId']?.toString(),
      courseId: json['courseId']?.toString(),
      classId: json['classId']?.toString(),
      taskType: json['taskType']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      semester: json['semester'] != null
          ? Semester.fromJson(json['semester'] as Map<String, dynamic>)
          : null,
      course: json['course'] != null
          ? Course.fromJson(json['course'] as Map<String, dynamic>)
          : null,
      classModel: json['class'] != null
          ? ClassModel.fromJson(json['class'] as Map<String, dynamic>)
          : null,
      teachers: (json['teachers'] as List<dynamic>?)
          ?.map((e) => TeachingTaskTeacher.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'courseId': courseId,
        'classId': classId,
        'taskType': taskType,
        'description': description,
        'teacherIds': teachers?.map((t) => t.teacherId ?? '').where((id) => id.isNotEmpty).toList(),
      };
}

class TeachingTaskTeacher {
  final String? teachingTaskId;
  final String? teacherId;
  final bool isMainTeacher;
  final String? assignedAt;
  final User? teacher;

  TeachingTaskTeacher({
    this.teachingTaskId,
    this.teacherId,
    this.isMainTeacher = false,
    this.assignedAt,
    this.teacher,
  });

  factory TeachingTaskTeacher.fromJson(Map<String, dynamic> json) {
    return TeachingTaskTeacher(
      teachingTaskId: json['teachingTaskId']?.toString(),
      teacherId: json['teacherId']?.toString(),
      isMainTeacher: json['isMainTeacher'] as bool? ?? false,
      assignedAt: json['assignedAt']?.toString(),
      teacher: json['teacher'] != null
          ? User.fromJson(json['teacher'] as Map<String, dynamic>)
          : null,
    );
  }
}
