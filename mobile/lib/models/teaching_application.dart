import 'lab.dart';
import '../utils/num_utils.dart';

class TeachingApplication {
  final String id;
  final String? semesterId;
  final String? semesterName;
  final String? teachingTaskId;
  final String? courseName;
  final String? majorId;
  final String? majorName;
  final String? classId;
  final String? className;
  final List<int>? weekNumbers;
  final int dayOfWeek;
  final List<int>? periodNumbers;
  final String? expectedLabId;
  final String? expectedLabName;
  final String? remark;
  final String? applicantId;
  final String? applicantName;
  final String? status;
  final String? approvalComment;
  final String? approvedBy;
  final String? approverName;
  final String? approvedAt;
  final bool isCancelled;
  final String? cancelReason;
  final String? createdAt;

  TeachingApplication({
    required this.id,
    this.semesterId,
    this.semesterName,
    this.teachingTaskId,
    this.courseName,
    this.majorId,
    this.majorName,
    this.classId,
    this.className,
    this.weekNumbers,
    this.dayOfWeek = 0,
    this.periodNumbers,
    this.expectedLabId,
    this.expectedLabName,
    this.remark,
    this.applicantId,
    this.applicantName,
    this.status,
    this.approvalComment,
    this.approvedBy,
    this.approverName,
    this.approvedAt,
    this.isCancelled = false,
    this.cancelReason,
    this.createdAt,
  });

  factory TeachingApplication.fromJson(Map<String, dynamic> json) {
    return TeachingApplication(
      id: json['id']?.toString() ?? '',
      semesterId: json['semesterId']?.toString(),
      semesterName: json['semesterName']?.toString(),
      teachingTaskId: json['teachingTaskId']?.toString(),
      courseName: json['courseName']?.toString(),
      majorId: json['majorId']?.toString(),
      majorName: json['majorName']?.toString(),
      classId: json['classId']?.toString(),
      className: json['className']?.toString(),
      weekNumbers: (json['weekNumbers'] as List<dynamic>?)?.cast<int>(),
      dayOfWeek: safeInt(json['dayOfWeek']),
      periodNumbers: (json['periodNumbers'] as List<dynamic>?)?.cast<int>(),
      expectedLabId: json['expectedLabId']?.toString(),
      expectedLabName: json['expectedLabName']?.toString(),
      remark: json['remark']?.toString(),
      applicantId: json['applicantId']?.toString(),
      applicantName: json['applicantName']?.toString(),
      status: json['status']?.toString(),
      approvalComment: json['approvalComment']?.toString(),
      approvedBy: json['approvedBy']?.toString(),
      approverName: json['approverName']?.toString(),
      approvedAt: json['approvedAt']?.toString(),
      isCancelled: json['isCancelled'] as bool? ?? false,
      cancelReason: json['cancelReason']?.toString(),
      createdAt: json['createdAt']?.toString(),
    );
  }

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'teachingTaskId': teachingTaskId,
        'courseName': courseName,
        'majorId': majorId,
        'majorName': majorName,
        'classId': classId,
        'className': className,
        'weekNumbers': weekNumbers,
        'dayOfWeek': dayOfWeek,
        'periodNumbers': periodNumbers,
        'expectedLabId': expectedLabId,
        'remark': remark,
      };
}

class CreateTeachingApplicationRequest {
  final String semesterId;
  final String teachingTaskId;
  final String? courseName;
  final String? majorId;
  final String? majorName;
  final String? classId;
  final String? className;
  final List<int>? weekNumbers;
  final int dayOfWeek;
  final List<int>? periodNumbers;
  final String? expectedLabId;
  final String? remark;

  CreateTeachingApplicationRequest({
    required this.semesterId,
    required this.teachingTaskId,
    this.courseName,
    this.majorId,
    this.majorName,
    this.classId,
    this.className,
    this.weekNumbers,
    this.dayOfWeek = 0,
    this.periodNumbers,
    this.expectedLabId,
    this.remark,
  });

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'teachingTaskId': teachingTaskId,
        'courseName': courseName,
        'majorId': majorId,
        'majorName': majorName,
        'classId': classId,
        'className': className,
        'weekNumbers': weekNumbers,
        'dayOfWeek': dayOfWeek,
        'periodNumbers': periodNumbers,
        'expectedLabId': expectedLabId,
        'remark': remark,
      };
}
