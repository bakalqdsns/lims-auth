/**
 * 用户相关类型定义
 * 与 Flutter lib/models/user.dart 对齐
 */

export interface UserInfo {
  id: string
  username: string
  fullName: string
  email?: string
  phone?: string
  employeeId?: string
  studentId?: string
  avatar?: string
  departmentId?: string
  departmentName?: string
  roles: string[]
  permissions: string[]
  status: number
  createdAt: string
  lastLoginAt?: string
}

/**
 * 登录请求
 */
export interface LoginRequest {
  username: string
  password: string
}

/**
 * 登录响应 (后端 LoginData)
 */
export interface LoginResponse {
  token: string
  expiresAt?: string
  user: UserInfo
}

/**
 * 更新个人信息请求
 */
export interface UpdateProfileRequest {
  fullName?: string
  email?: string
  phone?: string
  avatar?: string
}

/**
 * 修改密码请求
 */
export interface ChangePasswordRequest {
  oldPassword: string
  newPassword: string
}

/**
 * 重置密码请求 (管理员操作)
 */
export interface ResetPasswordRequest {
  newPassword: string
}

/**
 * 创建用户请求
 */
export interface CreateUserRequest {
  username: string
  password: string
  fullName: string
  email?: string
  phone?: string
  employeeId?: string
  studentId?: string
  departmentId?: string
  roleIds?: string[]
  status?: number
}

/**
 * 更新用户请求
 */
export interface UpdateUserRequest {
  fullName?: string
  email?: string
  phone?: string
  employeeId?: string
  studentId?: string
  departmentId?: string
  status?: number
}

/**
 * 用户列表项 (UserListItemDto)
 */
export interface UserListItem {
  id: string
  username: string
  fullName: string
  email?: string
  phone?: string
  employeeId?: string
  studentId?: string
  departmentId?: string
  departmentName?: string
  roles: { id: string; name: string; code: string }[]
  status: number
  lastLoginAt?: string
  createdAt: string
}

/**
 * 用户详情 (UserDetailDto)
 */
export interface UserDetail extends UserListItem {
  permissions: string[]
  classIds?: string[]
  majorId?: string
}

/**
 * 用户列表查询
 */
export interface UserQuery {
  page?: number
  pageSize?: number
  search?: string
  keyword?: string
  roleId?: string
  departmentId?: string
  status?: number
}
