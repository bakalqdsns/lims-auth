import 'lab.dart';
import '../utils/num_utils.dart';
import 'user.dart';

class Reservation {
  final String id;
  final String? semesterId;
  final String? semesterName;
  final String? labId;
  final String? labName;
  final String? useDate;
  final int dayOfWeek;
  final List<int>? periodNumbers;
  final int weekNumber;
  final double expectedDurationHours;
  final String? projectName;
  final String? projectCategory;
  final String? remark;
  final String? applicantId;
  final String? applicantName;
  final String? applicantPhone;
  final String? projectLeaderId;
  final String? projectLeaderName;
  final String? projectLeaderPhone;
  final String? memberGrade;
  final String? memberClassId;
  final String? memberClassName;
  final int memberCount;
  final String? status;
  final String? approvalComment;
  final String? approvedBy;
  final String? approverName;
  final String? approvedAt;
  final bool isCancelled;
  final String? cancelReason;
  final String? createdAt;
  final Lab? lab;

  Reservation({
    required this.id,
    this.semesterId,
    this.semesterName,
    this.labId,
    this.labName,
    this.useDate,
    this.dayOfWeek = 0,
    this.periodNumbers,
    this.weekNumber = 0,
    this.expectedDurationHours = 0,
    this.projectName,
    this.projectCategory,
    this.remark,
    this.applicantId,
    this.applicantName,
    this.applicantPhone,
    this.projectLeaderId,
    this.projectLeaderName,
    this.projectLeaderPhone,
    this.memberGrade,
    this.memberClassId,
    this.memberClassName,
    this.memberCount = 0,
    this.status,
    this.approvalComment,
    this.approvedBy,
    this.approverName,
    this.approvedAt,
    this.isCancelled = false,
    this.cancelReason,
    this.createdAt,
    this.lab,
  });

  factory Reservation.fromJson(Map<String, dynamic> json) {
    return Reservation(
      id: json['id']?.toString() ?? '',
      semesterId: json['semesterId']?.toString(),
      semesterName: json['semesterName']?.toString(),
      labId: json['labId']?.toString(),
      labName: json['labName']?.toString(),
      useDate: json['useDate']?.toString(),
      dayOfWeek: safeInt(json['dayOfWeek']),
      periodNumbers: (json['periodNumbers'] as List<dynamic>?)?.cast<int>(),
      weekNumber: safeInt(json['weekNumber']),
      expectedDurationHours: safeDouble(json['expectedDurationHours']),
      projectName: json['projectName']?.toString(),
      projectCategory: json['projectCategory']?.toString(),
      remark: json['remark']?.toString(),
      applicantId: json['applicantId']?.toString(),
      applicantName: json['applicantName']?.toString(),
      applicantPhone: json['applicantPhone']?.toString(),
      projectLeaderId: json['projectLeaderId']?.toString(),
      projectLeaderName: json['projectLeaderName']?.toString(),
      projectLeaderPhone: json['projectLeaderPhone']?.toString(),
      memberGrade: json['memberGrade']?.toString(),
      memberClassId: json['memberClassId']?.toString(),
      memberClassName: json['memberClassName']?.toString(),
      memberCount: safeInt(json['memberCount']),
      status: json['status']?.toString(),
      approvalComment: json['approvalComment']?.toString(),
      approvedBy: json['approvedBy']?.toString(),
      approverName: json['approverName']?.toString(),
      approvedAt: json['approvedAt']?.toString(),
      isCancelled: json['isCancelled'] as bool? ?? false,
      cancelReason: json['cancelReason']?.toString(),
      createdAt: json['createdAt']?.toString(),
      lab: json['lab'] != null
          ? Lab.fromJson(json['lab'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'labId': labId,
        'useDate': useDate,
        'dayOfWeek': dayOfWeek,
        'periodNumbers': periodNumbers,
        'weekNumber': weekNumber,
        'expectedDurationHours': expectedDurationHours,
        'projectName': projectName,
        'projectCategory': projectCategory,
        'remark': remark,
        'projectLeaderId': projectLeaderId,
        'projectLeaderName': projectLeaderName,
        'projectLeaderPhone': projectLeaderPhone,
        'memberGrade': memberGrade,
        'memberClassId': memberClassId,
        'memberClassName': memberClassName,
        'memberCount': memberCount,
      };
}

class CreateReservationRequest {
  final String semesterId;
  final String labId;
  final String? useDate;
  final int dayOfWeek;
  final List<int>? periodNumbers;
  final int weekNumber;
  final double expectedDurationHours;
  final String projectName;
  final String? projectCategory;
  final String? remark;
  final String? projectLeaderId;
  final String? projectLeaderName;
  final String? projectLeaderPhone;
  final String? memberGrade;
  final String? memberClassId;
  final String? memberClassName;
  final int memberCount;

  CreateReservationRequest({
    required this.semesterId,
    required this.labId,
    this.useDate,
    this.dayOfWeek = 0,
    this.periodNumbers,
    this.weekNumber = 0,
    this.expectedDurationHours = 0,
    required this.projectName,
    this.projectCategory,
    this.remark,
    this.projectLeaderId,
    this.projectLeaderName,
    this.projectLeaderPhone,
    this.memberGrade,
    this.memberClassId,
    this.memberClassName,
    this.memberCount = 0,
  });

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'labId': labId,
        'useDate': useDate,
        'dayOfWeek': dayOfWeek,
        'periodNumbers': periodNumbers,
        'weekNumber': weekNumber,
        'expectedDurationHours': expectedDurationHours,
        'projectName': projectName,
        'projectCategory': projectCategory,
        'remark': remark,
        'projectLeaderId': projectLeaderId,
        'projectLeaderName': projectLeaderName,
        'projectLeaderPhone': projectLeaderPhone,
        'memberGrade': memberGrade,
        'memberClassId': memberClassId,
        'memberClassName': memberClassName,
        'memberCount': memberCount,
      };
}

class ApprovalRequest {
  final String? comment;
  final String? approverName;

  ApprovalRequest({this.comment, this.approverName});

  Map<String, dynamic> toJson() => {
        'comment': comment,
        'approverName': approverName,
      };
}
