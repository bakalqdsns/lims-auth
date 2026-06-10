class User {
  final String id;
  final String username;
  final String? passwordHash;
  final String email;
  final String phone;
  final String fullName;
  final String? avatarUrl;
  final String? departmentId;
  final bool isActive;
  final String? createdAt;
  final String? updatedAt;
  final String? lastLoginAt;
  final String? employeeId;
  final String? studentId;
  final List<UserRole>? userRoles;
  final Department? department;

  User({
    required this.id,
    required this.username,
    this.passwordHash,
    required this.email,
    required this.phone,
    required this.fullName,
    this.avatarUrl,
    this.departmentId,
    required this.isActive,
    this.createdAt,
    this.updatedAt,
    this.lastLoginAt,
    this.employeeId,
    this.studentId,
    this.userRoles,
    this.department,
  });

  List<String> get roles =>
      userRoles?.map((e) => e.roleCode ?? '').where((e) => e.isNotEmpty).toList() ?? [];

  bool get isAdmin => roles.any((r) => ['super_admin', 'admin', 'lab_admin'].contains(r));
  bool get isTeacher => roles.contains('teacher');
  bool get isStudent => roles.contains('student');

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id']?.toString() ?? '',
      username: json['username']?.toString() ?? '',
      passwordHash: json['passwordHash']?.toString(),
      email: json['email']?.toString() ?? '',
      phone: json['phone']?.toString() ?? '',
      fullName: json['fullName']?.toString() ?? '',
      avatarUrl: json['avatarUrl']?.toString(),
      departmentId: json['departmentId']?.toString(),
      isActive: json['isActive'] as bool? ?? true,
      createdAt: json['createdAt']?.toString(),
      updatedAt: json['updatedAt']?.toString(),
      lastLoginAt: json['lastLoginAt']?.toString(),
      employeeId: json['employeeId']?.toString(),
      studentId: json['studentId']?.toString(),
      userRoles: (json['userRoles'] as List<dynamic>?)
          ?.map((e) => UserRole.fromJson(e as Map<String, dynamic>))
          .toList(),
      department: json['department'] != null
          ? Department.fromJson(json['department'] as Map<String, dynamic>)
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
        'username': username,
        'email': email,
        'phone': phone,
        'fullName': fullName,
        'departmentId': departmentId,
        'isActive': isActive,
      };
}

class UserRole {
  final String? id;
  final String? userId;
  final String? roleId;
  final String? roleCode;
  final String? roleName;

  UserRole({
    this.id,
    this.userId,
    this.roleId,
    this.roleCode,
    this.roleName,
  });

  factory UserRole.fromJson(Map<String, dynamic> json) {
    return UserRole(
      id: json['id']?.toString(),
      userId: json['userId']?.toString(),
      roleId: json['roleId']?.toString(),
      roleCode: json['roleCode']?.toString(),
      roleName: json['roleName']?.toString(),
    );
  }
}

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
  final User? manager;

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
      manager: json['manager'] != null
          ? User.fromJson(json['manager'] as Map<String, dynamic>)
          : null,
    );
  }
}

/// 鍒涘缓鐢ㄦ埛璇锋眰
class CreateUserRequest {
  final String username;
  final String password;
  final String email;
  final String phone;
  final String fullName;
  final String? departmentId;
  final List<String>? roleIds;
  final bool isActive;

  CreateUserRequest({
    required this.username,
    required this.password,
    required this.email,
    required this.phone,
    required this.fullName,
    this.departmentId,
    this.roleIds,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() => {
        'username': username,
        'password': password,
        'email': email,
        'phone': phone,
        'fullName': fullName,
        'departmentId': departmentId,
        'roleIds': roleIds,
        'isActive': isActive,
      };
}

/// 鏇存柊鐢ㄦ埛璇锋眰
class UpdateUserRequest {
  final String email;
  final String phone;
  final String fullName;
  final String? departmentId;
  final bool isActive;

  UpdateUserRequest({
    required this.email,
    required this.phone,
    required this.fullName,
    this.departmentId,
    this.isActive = true,
  });

  Map<String, dynamic> toJson() => {
        'email': email,
        'phone': phone,
        'fullName': fullName,
        'departmentId': departmentId,
        'isActive': isActive,
      };
}

/// 鏇存柊鐢ㄦ埛瑙掕壊璇锋眰
class UpdateUserRolesRequest {
  final List<String> roleIds;

  UpdateUserRolesRequest({required this.roleIds});

  Map<String, dynamic> toJson() => {'roleIds': roleIds};
}

/// 鐧诲綍璇锋眰
class LoginRequest {
  final String username;
  final String password;

  LoginRequest({required this.username, required this.password});

  Map<String, dynamic> toJson() => {'username': username, 'password': password};
}

/// 鐧诲綍鍝嶅簲
class LoginResponse {
  final String token;
  final User user;

  LoginResponse({required this.token, required this.user});

  factory LoginResponse.fromJson(Map<String, dynamic> json) {
    return LoginResponse(
      token: json['token']?.toString() ?? '',
      user: User.fromJson(json['user'] as Map<String, dynamic>),
    );
  }
}

/// 鏇存柊璧勬枡璇锋眰
class UpdateProfileRequest {
  final String fullName;
  final String email;
  final String phone;

  UpdateProfileRequest({
    required this.fullName,
    required this.email,
    required this.phone,
  });

  Map<String, dynamic> toJson() =>
      {'fullName': fullName, 'email': email, 'phone': phone};
}

/// 淇敼瀵嗙爜璇锋眰
class ChangePasswordRequest {
  final String oldPassword;
  final String newPassword;

  ChangePasswordRequest({
    required this.oldPassword,
    required this.newPassword,
  });

  Map<String, dynamic> toJson() =>
      {'oldPassword': oldPassword, 'newPassword': newPassword};
}
