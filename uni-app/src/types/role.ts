/**
 * 角色与权限相关类型定义
 * 与后端 RoleDto / RoleDetailDto / PermissionDto 对齐
 */

export interface Role {
  id: string
  name: string
  code: string
  description?: string
  isSystem: boolean
  userCount: number
  permissions: string[]
  createdAt: string
}

/** 下拉角色 */
export interface RoleBrief {
  id: string
  name: string
  code: string
}

export interface RoleDetail extends Role {
  permissionIds: string[]
  createdBy?: string
  updatedAt?: string
}

/**
 * 创建角色请求
 */
export interface CreateRoleRequest {
  name: string
  code: string
  description?: string
  permissionIds?: string[]
}

/**
 * 更新角色请求
 */
export interface UpdateRoleRequest {
  name?: string
  code?: string
  description?: string
}

/**
 * 分配权限请求
 */
export interface UpdateRolePermissionsRequest {
  permissionIds: string[]
}

/**
 * 角色列表查询
 */
export interface RoleQuery {
  page?: number
  pageSize?: number
  keyword?: string
  name?: string
  code?: string
}

/**
 * 权限项
 */
export interface Permission {
  id: string
  name: string
  code: string
  module: string
  moduleName?: string
  description?: string
}

/**
 * 权限模块
 */
export interface PermissionModule {
  module: string
  moduleName: string
  items: Permission[]
}
