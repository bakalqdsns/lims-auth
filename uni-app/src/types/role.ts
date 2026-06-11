/**
 * 角色相关类型定义
 * 与 Flutter lib/models/role.dart 对齐
 */

export interface Role {
  id: number
  name: string
  code: string
  description?: string
  isSystem: boolean
  userCount: number
  permissions: string[]
  createdAt: string
}

/**
 * 创建角色请求
 */
export interface CreateRoleRequest {
  name: string
  code: string
  description?: string
  permissionIds?: number[]
}

/**
 * 角色列表查询
 */
export interface RoleQuery {
  page?: number
  pageSize?: number
  search?: string
}

/**
 * 权限项
 */
export interface Permission {
  id: number
  name: string
  code: string
  module: string
  description?: string
}

/**
 * 权限模块
 */
export interface PermissionModule {
  module: string
  items: Permission[]
}
