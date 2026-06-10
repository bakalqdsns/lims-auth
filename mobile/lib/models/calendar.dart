import '../utils/num_utils.dart';

class AcademicCalendar {
  final String id;
  final String? semesterId;
  final String? date;
  final int weekNumber;
  final int dayOfWeek;
  final String? eventType;
  final String? eventPriority;
  final String? eventName;
  final String? eventCode;
  final bool isHoliday;
  final bool isWorkday;
  final bool isTeachingDay;
  final bool isExamDay;
  final bool isAdjusted;
  final String? holidayName;
  final String? holidayType;
  final String? description;
  final String? createdAt;

  AcademicCalendar({
    required this.id,
    this.semesterId,
    this.date,
    this.weekNumber = 0,
    this.dayOfWeek = 0,
    this.eventType,
    this.eventPriority,
    this.eventName,
    this.eventCode,
    this.isHoliday = false,
    this.isWorkday = false,
    this.isTeachingDay = false,
    this.isExamDay = false,
    this.isAdjusted = false,
    this.holidayName,
    this.holidayType,
    this.description,
    this.createdAt,
  });

  factory AcademicCalendar.fromJson(Map<String, dynamic> json) {
    return AcademicCalendar(
      id: json['id']?.toString() ?? '',
      semesterId: json['semesterId']?.toString(),
      date: json['date']?.toString(),
      weekNumber: safeInt(json['weekNumber']),
      dayOfWeek: safeInt(json['dayOfWeek']),
      eventType: json['eventType']?.toString(),
      eventPriority: json['eventPriority']?.toString(),
      eventName: json['eventName']?.toString(),
      eventCode: json['eventCode']?.toString(),
      isHoliday: json['isHoliday'] as bool? ?? false,
      isWorkday: json['isWorkday'] as bool? ?? false,
      isTeachingDay: json['isTeachingDay'] as bool? ?? false,
      isExamDay: json['isExamDay'] as bool? ?? false,
      isAdjusted: json['isAdjusted'] as bool? ?? false,
      holidayName: json['holidayName']?.toString(),
      holidayType: json['holidayType']?.toString(),
      description: json['description']?.toString(),
      createdAt: json['createdAt']?.toString(),
    );
  }
}

class AddHolidayRequest {
  final String date;
  final String name;
  final String? type;
  final bool isWorkday;
  final String? description;

  AddHolidayRequest({
    required this.date,
    required this.name,
    this.type,
    this.isWorkday = false,
    this.description,
  });

  Map<String, dynamic> toJson() => {
        'date': date,
        'name': name,
        'type': type,
        'isWorkday': isWorkday,
        'description': description,
      };
}

class WeekInfo {
  final int currentWeek;
  final String? weekStartDate;
  final String? weekEndDate;
  final bool isTeachingWeek;
  final String? semesterName;

  WeekInfo({
    this.currentWeek = 0,
    this.weekStartDate,
    this.weekEndDate,
    this.isTeachingWeek = false,
    this.semesterName,
  });

  factory WeekInfo.fromJson(Map<String, dynamic> json) {
    return WeekInfo(
      currentWeek: safeInt(json['currentWeek']),
      weekStartDate: json['weekStartDate']?.toString(),
      weekEndDate: json['weekEndDate']?.toString(),
      isTeachingWeek: json['isTeachingWeek'] as bool? ?? false,
      semesterName: json['semesterName']?.toString(),
    );
  }
}
