import '../utils/num_utils.dart';

class DashboardData {
  final int totalLabs;
  final int totalEquipments;
  final int activeSchedules;
  final int pendingApprovals;
  final int todayReservations;
  final int overdueRecords;

  DashboardData({
    this.totalLabs = 0,
    this.totalEquipments = 0,
    this.activeSchedules = 0,
    this.pendingApprovals = 0,
    this.todayReservations = 0,
    this.overdueRecords = 0,
  });

  factory DashboardData.fromJson(Map<String, dynamic> json) {
    return DashboardData(
      totalLabs: safeInt(json['totalLabs'] ?? json['labCount']),
      totalEquipments: safeInt(json['totalEquipments'] ?? json['equipmentCount'] ?? json['equipmentStatistics']?['totalCount']),
      activeSchedules: safeInt(json['activeSchedules'] ?? json['scheduleCount']),
      pendingApprovals: safeInt(json['pendingApprovals']),
      todayReservations: safeInt(json['todayReservations']),
      overdueRecords: safeInt(json['overdueRecords']),
    );
  }
}

class LabUsage {
  final String? labId;
  final String? labName;
  final int totalBookings;
  final int completedBookings;
  final double utilizationRate;

  LabUsage({
    this.labId,
    this.labName,
    this.totalBookings = 0,
    this.completedBookings = 0,
    this.utilizationRate = 0,
  });

  factory LabUsage.fromJson(Map<String, dynamic> json) {
    return LabUsage(
      labId: json['labId']?.toString(),
      labName: json['labName']?.toString(),
      totalBookings: safeInt(json['totalBookings']),
      completedBookings: safeInt(json['completedBookings']),
      utilizationRate: safeDouble(json['utilizationRate'] ?? json['utilization']),
    );
  }
}

class ReservationStats {
  final int total;
  final int pending;
  final int approved;
  final int rejected;
  final int cancelled;
  final int completed;

  ReservationStats({
    this.total = 0,
    this.pending = 0,
    this.approved = 0,
    this.rejected = 0,
    this.cancelled = 0,
    this.completed = 0,
  });

  factory ReservationStats.fromJson(Map<String, dynamic> json) {
    return ReservationStats(
      total: safeInt(json['total']),
      pending: safeInt(json['pending']),
      approved: safeInt(json['approved']),
      rejected: safeInt(json['rejected']),
      cancelled: safeInt(json['cancelled']),
      completed: safeInt(json['completed']),
    );
  }
}

class WeeklySummary {
  final String weekLabel;
  final int reservationCount;
  final int usageCount;
  final int equipmentUsage;

  WeeklySummary({
    this.weekLabel = '',
    this.reservationCount = 0,
    this.usageCount = 0,
    this.equipmentUsage = 0,
  });

  factory WeeklySummary.fromJson(Map<String, dynamic> json) {
    return WeeklySummary(
      weekLabel: json['weekLabel']?.toString() ?? json['week']?.toString() ?? '',
      reservationCount: safeInt(json['reservationCount']),
      usageCount: safeInt(json['usageCount']),
      equipmentUsage: safeInt(json['equipmentUsage']),
    );
  }
}

class StatisticsByCategory {
  final String category;
  final int count;
  final double percentage;

  StatisticsByCategory({
    this.category = '',
    this.count = 0,
    this.percentage = 0,
  });

  factory StatisticsByCategory.fromJson(Map<String, dynamic> json) {
    return StatisticsByCategory(
      category: json['category']?.toString() ?? '',
      count: safeInt(json['count']),
      percentage: safeDouble(json['percentage']),
    );
  }
}
