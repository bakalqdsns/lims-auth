/**
 * 权限管理 API
 * 对应后端 PermissionsController
 *  GET /api/v1/permissions
 *  GET /api/v1/permissions/by-module
 *  GET /api/v1/permissions/modules
 */
import { get } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { Permission, PermissionModule } from '@/types/role'

/** 所有权限 (扁平) */
export function getAllPermissions() {
  return get<ApiResponse<Permission[]>>('/permissions')
}

/** 按模块分组 */
export function getPermissionsByModule() {
  return get<ApiResponse<PermissionModule[]>>('/permissions/by-module')
}

/** 模块元数据 */
export function getPermissionModules() {
  return get<ApiResponse<PermissionModule[]>>('/permissions/modules')
}
