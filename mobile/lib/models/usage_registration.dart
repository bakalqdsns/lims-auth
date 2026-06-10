import 'lab.dart';
import '../utils/num_utils.dart';

class UsageRegistration {
  final String id;
  final String? semesterId;
  final String? semesterName;
  final String? labId;
  final String? labName;
  final String? useDate;
  final int weekNumber;
  final int dayOfWeek;
  final int periodNumber;
  final String? source;
  final String? scheduleEntryId;
  final String? reservationId;
  final String? teachingApplicationId;
  final String? courseName;
  final String? projectName;
  final String? experimentItemName;
  final String? experimentItemType;
  final int plannedHours;
  final double actualHours;
  final String? className;
  final int expectedStudentCount;
  final int actualStudentCount;
  final String? attendanceRecord;
  final String? teachingCondition;
  final String? equipmentCondition;
  final String? status;
  final String? remindedAt;
  final String? filledById;
  final String? filledByName;
  final String? filledAt;
  final String? createdAt;
  final Lab? lab;

  UsageRegistration({
    required this.id,
    this.semesterId,
    this.semesterName,
    this.labId,
    this.labName,
    this.useDate,
    this.weekNumber = 0,
    this.dayOfWeek = 0,
    this.periodNumber = 0,
    this.source,
    this.scheduleEntryId,
    this.reservationId,
    this.teachingApplicationId,
    this.courseName,
    this.projectName,
    this.experimentItemName,
    this.experimentItemType,
    this.plannedHours = 0,
    this.actualHours = 0,
    this.className,
    this.expectedStudentCount = 0,
    this.actualStudentCount = 0,
    this.attendanceRecord,
    this.teachingCondition,
    this.equipmentCondition,
    this.status,
    this.remindedAt,
    this.filledById,
    this.filledByName,
    this.filledAt,
    this.createdAt,
    this.lab,
  });

  factory UsageRegistration.fromJson(Map<String, dynamic> json) {
    return UsageRegistration(
      id: json['id']?.toString() ?? '',
      semesterId: json['semesterId']?.toString(),
      semesterName: json['semesterName']?.toString(),
      labId: json['labId']?.toString(),
      labName: json['labName']?.toString(),
      useDate: json['useDate']?.toString(),
      weekNumber: safeInt(json['weekNumber']),
      dayOfWeek: safeInt(json['dayOfWeek']),
      periodNumber: safeInt(json['periodNumber']),
      source: json['source']?.toString(),
      scheduleEntryId: json['scheduleEntryId']?.toString(),
      reservationId: json['reservationId']?.toString(),
      teachingApplicationId: json['teachingApplicationId']?.toString(),
      courseName: json['courseName']?.toString(),
      projectName: json['projectName']?.toString(),
      experimentItemName: json['experimentItemName']?.toString(),
      experimentItemType: json['experimentItemType']?.toString(),
      plannedHours: safeInt(json['plannedHours']),
      actualHours: safeDouble(json['actualHours']),
      className: json['className']?.toString(),
      expectedStudentCount: safeInt(json['expectedStudentCount']),
      actualStudentCount: safeInt(json['actualStudentCount']),
      attendanceRecord: json['attendanceRecord']?.toString(),
      teachingCondition: json['teachingCondition']?.toString(),
      equipmentCondition: json['equipmentCondition']?.toString(),
      status: json['status']?.toString(),
      remindedAt: json['remindedAt']?.toString(),
      filledById: json['filledById']?.toString(),
      filledByName: json['filledByName']?.toString(),
      filledAt: json['filledAt']?.toString(),
      createdAt: json['createdAt']?.toString(),
      lab: json['lab'] != null
          ? Lab.fromJson(json['lab'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'labId': labId,
        'labName': labName,
        'useDate': useDate,
        'weekNumber': weekNumber,
        'dayOfWeek': dayOfWeek,
        'periodNumber': periodNumber,
        'source': source,
        'scheduleEntryId': scheduleEntryId,
        'reservationId': reservationId,
        'teachingApplicationId': teachingApplicationId,
        'courseName': courseName,
        'projectName': projectName,
        'experimentItemName': experimentItemName,
        'experimentItemType': experimentItemType,
        'plannedHours': plannedHours,
        'actualHours': actualHours,
        'className': className,
        'expectedStudentCount': expectedStudentCount,
        'actualStudentCount': actualStudentCount,
        'attendanceRecord': attendanceRecord,
        'teachingCondition': teachingCondition,
        'equipmentCondition': equipmentCondition,
      };
}

class CompletionRate {
  final int total;
  final int completed;
  final int pending;
  final int overdue;
  final double rate;

  CompletionRate({
    this.total = 0,
    this.completed = 0,
    this.pending = 0,
    this.overdue = 0,
    this.rate = 0,
  });

  factory CompletionRate.fromJson(Map<String, dynamic> json) {
    return CompletionRate(
      total: safeInt(json['total']),
      completed: safeInt(json['completed']),
      pending: safeInt(json['pending']),
      overdue: safeInt(json['overdue']),
      rate: safeDouble(json['rate']),
    );
  }
}
