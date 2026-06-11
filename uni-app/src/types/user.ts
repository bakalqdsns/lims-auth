/**
 * 用户相关类型定义
 * 与 Flutter lib/models/user.dart 对齐
 */

export interface UserInfo {
  id: number
  username: string
  fullName: string
  email?: string
  phone?: string
  employeeId?: string
  studentId?: string
  avatar?: string
  departmentId?: number
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
 * 登录响应
 */
export interface LoginResponse {
  token: string
  user: UserInfo
}

/**
 * 更新个人信息请求
 */
export interface UpdateProfileRequest {
  fullName?: string
  email?: string
  phone?: string
}

/**
 * 修改密码请求
 */
export interface ChangePasswordRequest {
  oldPassword: string
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
  departmentId?: number
  roleIds?: number[]
}

/**
 * 用户列表查询
 */
export interface UserQuery {
  page?: number
  pageSize?: number
  search?: string
  keyword?: string
  roleId?: number
  departmentId?: number
  status?: number
}
