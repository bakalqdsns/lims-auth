class Department {
  final String id;
  final String code;
  final String name;
  final String? parentId;
  final String? managerId;
  final String? description;
  final bool isActive;
  final String? createdAt;
  final List<Department>? children;
  final dynamic manager;

  Department({
    required this.id,
    required this.code,
    required this.name,
    this.parentId,
    this.managerId,
    this.description,
    this.isActive = true,
    this.createdAt,
    this.children,
    this.manager,
  });

  factory Department.fromJson(Map<String, dynamic> json) {
    return Department(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      parentId: json['parentId']?.toString(),
      managerId: json['managerId']?.toString(),
      description: json['description']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      children: (json['children'] as List<dynamic>?)
          ?.map((e) => Department.fromJson(e as Map<String, dynamic>))
          .toList(),
      manager: json['manager'],
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'parentId': parentId,
        'managerId': managerId,
        'description': description,
      };
}

class CreateDepartmentRequest {
  final String code;
  final String name;
  final String? parentId;
  final String? managerId;
  final String? description;

  CreateDepartmentRequest({
    required this.code,
    required this.name,
    this.parentId,
    this.managerId,
    this.description,
  });

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'parentId': parentId,
        'managerId': managerId,
        'description': description,
      };
}

class UpdateDepartmentRequest {
  final String name;
  final String? managerId;
  final String? description;
  final bool isActive;

  UpdateDepartmentRequest({
    required this.name,
    this.managerId,
    this.description,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() => {
        'name': name,
        'managerId': managerId,
        'description': description,
        'isActive': isActive,
      };
}
