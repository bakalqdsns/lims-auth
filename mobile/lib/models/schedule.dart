import 'lab.dart';
import '../utils/num_utils.dart';
import 'semester.dart';

class ScheduleEntry {
  final String id;
  final String? semesterId;
  final String? semesterName;
  final String? labId;
  final String? labName;
  final String? buildingName;
  final int weekNumber;
  final int startWeek;
  final int endWeek;
  final int dayOfWeek;
  final int periodNumber;
  final String? source;
  final String? status;
  final String? reservationId;
  final String? teachingApplicationId;
  final String? experimentTaskId;
  final String? teachingTaskId;
  final String? courseName;
  final String? projectName;
  final String? courseId;
  final String? teacherId;
  final String? teacherName;
  final String? classId;
  final String? className;
  final String? majorId;
  final String? majorName;
  final int studentCount;
  final String? remark;
  final bool hasConflict;
  final String? conflictInfo;
  final String? createdAt;
  final Semester? semester;
  final Lab? lab;

  ScheduleEntry({
    required this.id,
    this.semesterId,
    this.semesterName,
    this.labId,
    this.labName,
    this.buildingName,
    this.weekNumber = 0,
    this.startWeek = 0,
    this.endWeek = 0,
    this.dayOfWeek = 0,
    this.periodNumber = 0,
    this.source,
    this.status,
    this.reservationId,
    this.teachingApplicationId,
    this.experimentTaskId,
    this.teachingTaskId,
    this.courseName,
    this.projectName,
    this.courseId,
    this.teacherId,
    this.teacherName,
    this.classId,
    this.className,
    this.majorId,
    this.majorName,
    this.studentCount = 0,
    this.remark,
    this.hasConflict = false,
    this.conflictInfo,
    this.createdAt,
    this.semester,
    this.lab,
  });

  factory ScheduleEntry.fromJson(Map<String, dynamic> json) {
    return ScheduleEntry(
      id: json['id']?.toString() ?? '',
      semesterId: json['semesterId']?.toString(),
      semesterName: json['semesterName']?.toString(),
      labId: json['labId']?.toString(),
      labName: json['labName']?.toString(),
      buildingName: json['buildingName']?.toString(),
      weekNumber: safeInt(json['weekNumber']),
      startWeek: safeInt(json['startWeek']),
      endWeek: safeInt(json['endWeek']),
      dayOfWeek: safeInt(json['dayOfWeek']),
      periodNumber: safeInt(json['periodNumber']),
      source: json['source']?.toString(),
      status: json['status']?.toString(),
      reservationId: json['reservationId']?.toString(),
      teachingApplicationId: json['teachingApplicationId']?.toString(),
      experimentTaskId: json['experimentTaskId']?.toString(),
      teachingTaskId: json['teachingTaskId']?.toString(),
      courseName: json['courseName']?.toString(),
      projectName: json['projectName']?.toString(),
      courseId: json['courseId']?.toString(),
      teacherId: json['teacherId']?.toString(),
      teacherName: json['teacherName']?.toString(),
      classId: json['classId']?.toString(),
      className: json['className']?.toString(),
      majorId: json['majorId']?.toString(),
      majorName: json['majorName']?.toString(),
      studentCount: safeInt(json['studentCount']),
      remark: json['remark']?.toString(),
      hasConflict: json['hasConflict'] as bool? ?? false,
      conflictInfo: json['conflictInfo']?.toString(),
      createdAt: json['createdAt']?.toString(),
      semester: json['semester'] != null
          ? Semester.fromJson(json['semester'] as Map<String, dynamic>)
          : null,
      lab: json['lab'] != null
          ? Lab.fromJson(json['lab'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'semesterId': semesterId,
        'labId': labId,
        'weekNumber': weekNumber,
        'startWeek': startWeek,
        'endWeek': endWeek,
        'dayOfWeek': dayOfWeek,
        'periodNumber': periodNumber,
        'source': source,
        'reservationId': reservationId,
        'teachingApplicationId': teachingApplicationId,
        'experimentTaskId': experimentTaskId,
        'teachingTaskId': teachingTaskId,
        'courseName': courseName,
        'projectName': projectName,
        'courseId': courseId,
        'teacherId': teacherId,
        'teacherName': teacherName,
        'classId': classId,
        'className': className,
        'majorId': majorId,
        'majorName': majorName,
        'studentCount': studentCount,
        'remark': remark,
      };
}

class ScheduleTableRow {
  final int periodNumber;
  final String? periodName;
  final List<ScheduleTableCell>? cells;

  ScheduleTableRow({
    required this.periodNumber,
    this.periodName,
    this.cells,
  });

  factory ScheduleTableRow.fromJson(Map<String, dynamic> json) {
    return ScheduleTableRow(
      periodNumber: safeInt(json['periodNumber']),
      periodName: json['periodName']?.toString(),
      cells: (json['cells'] as List<dynamic>?)
          ?.map((e) => ScheduleTableCell.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}

class ScheduleTableCell {
  final String? scheduleEntryId;
  final String? courseName;
  final String? teacherName;
  final String? className;
  final String? labName;
  final String? source;
  final String? status;
  final bool hasConflict;
  final int studentCount;

  ScheduleTableCell({
    this.scheduleEntryId,
    this.courseName,
    this.teacherName,
    this.className,
    this.labName,
    this.source,
    this.status,
    this.hasConflict = false,
    this.studentCount = 0,
  });

  factory ScheduleTableCell.fromJson(Map<String, dynamic> json) {
    return ScheduleTableCell(
      scheduleEntryId: json['scheduleEntryId']?.toString(),
      courseName: json['courseName']?.toString(),
      teacherName: json['teacherName']?.toString(),
      className: json['className']?.toString(),
      labName: json['labName']?.toString(),
      source: json['source']?.toString(),
      status: json['status']?.toString(),
      hasConflict: json['hasConflict'] as bool? ?? false,
      studentCount: safeInt(json['studentCount']),
    );
  }
}

class ConflictCheckResult {
  final bool hasHardConflict;
  final bool hasSoftConflict;
  final List<ConflictItem>? hardConflicts;
  final List<ConflictItem>? softConflicts;
  final bool canForceSchedule;

  ConflictCheckResult({
    this.hasHardConflict = false,
    this.hasSoftConflict = false,
    this.hardConflicts,
    this.softConflicts,
    this.canForceSchedule = false,
  });

  bool get hasConflict => hasHardConflict || hasSoftConflict;

  factory ConflictCheckResult.fromJson(Map<String, dynamic> json) {
    return ConflictCheckResult(
      hasHardConflict: json['hasHardConflict'] as bool? ?? false,
      hasSoftConflict: json['hasSoftConflict'] as bool? ?? false,
      hardConflicts: (json['hardConflicts'] as List<dynamic>?)
          ?.map((e) => ConflictItem.fromJson(e as Map<String, dynamic>))
          .toList(),
      softConflicts: (json['softConflicts'] as List<dynamic>?)
          ?.map((e) => ConflictItem.fromJson(e as Map<String, dynamic>))
          .toList(),
      canForceSchedule: json['canForceSchedule'] as bool? ?? false,
    );
  }
}

class ConflictItem {
  final String? id;
  final String? type;
  final String? message;
  final String? labName;
  final int weekNumber;
  final int dayOfWeek;
  final int periodNumber;

  ConflictItem({
    this.id,
    this.type,
    this.message,
    this.labName,
    this.weekNumber = 0,
    this.dayOfWeek = 0,
    this.periodNumber = 0,
  });

  factory ConflictItem.fromJson(Map<String, dynamic> json) {
    return ConflictItem(
      id: json['id']?.toString(),
      type: json['type']?.toString(),
      message: json['message']?.toString(),
      labName: json['labName']?.toString(),
      weekNumber: safeInt(json['weekNumber']),
      dayOfWeek: safeInt(json['dayOfWeek']),
      periodNumber: safeInt(json['periodNumber']),
    );
  }
}
