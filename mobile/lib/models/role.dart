class Role {
  final String id;
  final String code;
  final String name;
  final String? description;
  final bool isSystem;
  final bool isActive;
  final String? createdAt;
  final List<RolePermission>? rolePermissions;

  Role({
    required this.id,
    required this.code,
    required this.name,
    this.description,
    this.isSystem = false,
    this.isActive = true,
    this.createdAt,
    this.rolePermissions,
  });

  factory Role.fromJson(Map<String, dynamic> json) {
    return Role(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      description: json['description']?.toString(),
      isSystem: json['isSystem'] as bool? ?? false,
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      rolePermissions: (json['rolePermissions'] as List<dynamic>?)
          ?.map((e) => RolePermission.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'description': description,
        'isActive': isActive,
      };
}

class RolePermission {
  final String? id;
  final String? roleId;
  final String? permissionId;
  final Permission? permission;

  RolePermission({this.id, this.roleId, this.permissionId, this.permission});

  factory RolePermission.fromJson(Map<String, dynamic> json) {
    return RolePermission(
      id: json['id']?.toString(),
      roleId: json['roleId']?.toString(),
      permissionId: json['permissionId']?.toString(),
      permission: json['permission'] != null
          ? Permission.fromJson(json['permission'] as Map<String, dynamic>)
          : null,
    );
  }
}

class Permission {
  final String id;
  final String code;
  final String name;
  final String module;
  final String? description;
  final String? createdAt;

  Permission({
    required this.id,
    required this.code,
    required this.name,
    required this.module,
    this.description,
    this.createdAt,
  });

  factory Permission.fromJson(Map<String, dynamic> json) {
    return Permission(
      id: json['id']?.toString() ?? '',
      code: json['code']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      module: json['module']?.toString() ?? '',
      description: json['description']?.toString(),
      createdAt: json['createdAt']?.toString(),
    );
  }
}

/// 鎸夋ā鍧楀垎缁勭殑鏉冮檺
class PermissionModule {
  final String module;
  final List<Permission> permissions;

  PermissionModule({required this.module, required this.permissions});

  factory PermissionModule.fromJson(Map<String, dynamic> json) {
    return PermissionModule(
      module: json['module']?.toString() ?? json['key']?.toString() ?? '',
      permissions: (json['permissions'] as List<dynamic>?)
              ?.map((e) => Permission.fromJson(e as Map<String, dynamic>))
              .toList() ??
          (json['value'] as List<dynamic>?)
              ?.map((e) => Permission.fromJson(e as Map<String, dynamic>))
              .toList() ??
          [],
    );
  }
}

class CreateRoleRequest {
  final String code;
  final String name;
  final String? description;

  CreateRoleRequest({required this.code, required this.name, this.description});

  Map<String, dynamic> toJson() => {
        'code': code,
        'name': name,
        'description': description,
      };
}

class UpdateRoleRequest {
  final String name;
  final String? description;
  final bool isActive;

  UpdateRoleRequest({
    required this.name,
    this.description,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() => {
        'name': name,
        'description': description,
        'isActive': isActive,
      };
}

class UpdateRolePermissionsRequest {
  final List<String> permissionIds;

  UpdateRolePermissionsRequest({required this.permissionIds});

  Map<String, dynamic> toJson() => {'permissionIds': permissionIds};
}
