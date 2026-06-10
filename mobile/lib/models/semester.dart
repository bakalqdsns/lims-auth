import 'department.dart';
import '../utils/num_utils.dart';

class Semester {
  final String id;
  final String name;
  final String code;
  final String? externalId;
  final String academicYear;
  final String semesterType;
  final int displayOrder;
  final String? startDate;
  final String? endDate;
  final String? teachingStartDate;
  final String? teachingEndDate;
  final int totalWeeks;
  final int teachingWeeks;
  final String? courseSelectionStart;
  final String? courseSelectionEnd;
  final String? courseSelectionEndWithdraw;
  final String? schedulingStart;
  final String? schedulingEnd;
  final String? schedulePublishTime;
  final String? examWeekStart;
  final String? examWeekEnd;
  final String? gradeEntryStart;
  final String? gradeEntryEnd;
  final String? gradePublishTime;
  final String? registrationStart;
  final String? registrationEnd;
  final String? tuitionPaymentStart;
  final String? tuitionPaymentEnd;
  final String status;
  final bool isCurrent;
  final bool isActive;
  final bool isSandbox;
  final bool isTemplate;
  final String? description;
  final String? createdAt;

  Semester({
    required this.id,
    required this.name,
    required this.code,
    this.externalId,
    required this.academicYear,
    required this.semesterType,
    this.displayOrder = 0,
    this.startDate,
    this.endDate,
    this.teachingStartDate,
    this.teachingEndDate,
    this.totalWeeks = 0,
    this.teachingWeeks = 0,
    this.courseSelectionStart,
    this.courseSelectionEnd,
    this.courseSelectionEndWithdraw,
    this.schedulingStart,
    this.schedulingEnd,
    this.schedulePublishTime,
    this.examWeekStart,
    this.examWeekEnd,
    this.gradeEntryStart,
    this.gradeEntryEnd,
    this.gradePublishTime,
    this.registrationStart,
    this.registrationEnd,
    this.tuitionPaymentStart,
    this.tuitionPaymentEnd,
    this.status = 'Draft',
    this.isCurrent = false,
    this.isActive = true,
    this.isSandbox = false,
    this.isTemplate = false,
    this.description,
    this.createdAt,
  });

  factory Semester.fromJson(Map<String, dynamic> json) {
    return Semester(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      externalId: json['externalId']?.toString(),
      academicYear: json['academicYear']?.toString() ?? '',
      semesterType: json['semesterType']?.toString() ?? '',
      displayOrder: safeInt(json['displayOrder']),
      startDate: json['startDate']?.toString(),
      endDate: json['endDate']?.toString(),
      teachingStartDate: json['teachingStartDate']?.toString(),
      teachingEndDate: json['teachingEndDate']?.toString(),
      totalWeeks: safeInt(json['totalWeeks']),
      teachingWeeks: safeInt(json['teachingWeeks']),
      courseSelectionStart: json['courseSelectionStart']?.toString(),
      courseSelectionEnd: json['courseSelectionEnd']?.toString(),
      courseSelectionEndWithdraw: json['courseSelectionEndWithdraw']?.toString(),
      schedulingStart: json['schedulingStart']?.toString(),
      schedulingEnd: json['schedulingEnd']?.toString(),
      schedulePublishTime: json['schedulePublishTime']?.toString(),
      examWeekStart: json['examWeekStart']?.toString(),
      examWeekEnd: json['examWeekEnd']?.toString(),
      gradeEntryStart: json['gradeEntryStart']?.toString(),
      gradeEntryEnd: json['gradeEntryEnd']?.toString(),
      gradePublishTime: json['gradePublishTime']?.toString(),
      registrationStart: json['registrationStart']?.toString(),
      registrationEnd: json['registrationEnd']?.toString(),
      tuitionPaymentStart: json['tuitionPaymentStart']?.toString(),
      tuitionPaymentEnd: json['tuitionPaymentEnd']?.toString(),
      status: json['status']?.toString() ?? 'Draft',
      isCurrent: json['isCurrent'] as bool? ?? false,
      isActive: json['isActive'] as bool? ?? true,
      isSandbox: json['isSandbox'] as bool? ?? false,
      isTemplate: json['isTemplate'] as bool? ?? false,
      description: json['description']?.toString(),
      createdAt: json['createdAt']?.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
        'name': name,
        'code': code,
        'academicYear': academicYear,
        'semesterType': semesterType,
        'startDate': startDate,
        'endDate': endDate,
        'teachingStartDate': teachingStartDate,
        'teachingEndDate': teachingEndDate,
        'totalWeeks': totalWeeks,
        'teachingWeeks': teachingWeeks,
        'courseSelectionStart': courseSelectionStart,
        'courseSelectionEnd': courseSelectionEnd,
        'schedulingStart': schedulingStart,
        'schedulingEnd': schedulingEnd,
        'examWeekStart': examWeekStart,
        'examWeekEnd': examWeekEnd,
        'description': description,
      };
}

class SemesterStatus {
  static const String draft = 'Draft';
  static const String active = 'Active';
  static const String completed = 'Completed';
  static const String archived = 'Archived';
}
