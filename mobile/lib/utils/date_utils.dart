import 'package:intl/intl.dart';

class DateUtils {
  /// 计算某个日期在学期中的周次
  static int getWeekNumber(DateTime date, DateTime semesterStart) {
    final diff = date.difference(semesterStart).inDays;
    if (diff < 0) return 0;
    return (diff / 7).floor() + 1;
  }

  /// 获取周次的开始日期
  static DateTime getWeekStartDate(DateTime semesterStart, int weekNumber) {
    return semesterStart.add(Duration(days: (weekNumber - 1) * 7));
  }

  /// 获取周次的结束日期
  static DateTime getWeekEndDate(DateTime semesterStart, int weekNumber) {
    return semesterStart.add(Duration(days: weekNumber * 7 - 1));
  }

  /// 格式化日期为 YYYY-MM-DD
  static String formatDate(DateTime date) {
    return DateFormat('yyyy-MM-dd').format(date);
  }

  /// 格式化日期为 YYYY-MM-DD HH:mm
  static String formatDateTime(DateTime date) {
    return DateFormat('yyyy-MM-dd HH:mm').format(date);
  }

  /// 解析日期字符串
  static DateTime? parseDate(String? dateStr) {
    if (dateStr == null || dateStr.isEmpty) return null;
    try {
      return DateTime.parse(dateStr);
    } catch (_) {
      try {
        return DateFormat('yyyy-MM-dd').parse(dateStr);
      } catch (_) {
        return null;
      }
    }
  }

  /// 格式化星期
  static String formatDayOfWeek(int dayOfWeek) {
    const days = ['周一', '周二', '周三', '周四', '周五', '周六', '周日'];
    if (dayOfWeek >= 1 && dayOfWeek <= 7) {
      return days[dayOfWeek - 1];
    }
    return '';
  }

  /// 获取当前是第几周
  static int getCurrentWeek(DateTime semesterStart) {
    return getWeekNumber(DateTime.now(), semesterStart);
  }

  /// 判断是否是今天
  static bool isToday(DateTime date) {
    final now = DateTime.now();
    return date.year == now.year && date.month == now.month && date.day == now.day;
  }

  /// 判断是否是周末
  static bool isWeekend(DateTime date) {
    return date.weekday == 6 || date.weekday == 7;
  }
}
