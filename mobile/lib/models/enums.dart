/// 閫氱敤 ID + Name 妯″瀷
class IdName {
  final String id;
  final String name;

  IdName({required this.id, required this.name});

  factory IdName.fromJson(Map<String, dynamic> json) {
    return IdName(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
    );
  }

  Map<String, dynamic> toJson() => {'id': id, 'name': name};
}

/// 瀛︽湡鐘舵€佹灇涓?
class SemesterStatus {
  static const String draft = 'Draft';
  static const String active = 'Active';
  static const String completed = 'Completed';
  static const String archived = 'Archived';
}

/// 瀛︽湡绫诲瀷鏋氫妇
class SemesterType {
  static const String first = 'First';
  static const String second = 'Second';
  static const String summer = 'Summer';
  static const String winter = 'Winter';
}
