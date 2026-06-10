/// 安全地将任意值解析为 num，再转 int。
/// 能容忍字符串（如 "AUTO"）、null、混合类型。
int safeInt(dynamic value, [int fallback = 0]) {
  if (value == null) return fallback;
  if (value is int) return value;
  if (value is double) return value.toInt();
  if (value is String) return int.tryParse(value) ?? fallback;
  if (value is num) return value.toInt();
  return fallback;
}

/// 安全地将任意值解析为 double。
double safeDouble(dynamic value, [double fallback = 0.0]) {
  if (value == null) return fallback;
  if (value is double) return value;
  if (value is int) return value.toDouble();
  if (value is String) return double.tryParse(value) ?? fallback;
  if (value is num) return value.toDouble();
  return fallback;
}
