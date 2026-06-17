/**
 * 角色管理 API
 * 对应后端 RolesController
 *  GET    /api/v1/roles
 *  GET    /api/v1/roles/all
 *  GET    /api/v1/roles/{id}
 *  POST   /api/v1/roles
 *  PUT    /api/v1/roles/{id}
 *  DELETE /api/v1/roles/{id}
 *  PUT    /api/v1/roles/{id}/permissions
 *  GET    /api/v1/roles/{id}/users
 */
import { get, post, put, del } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type {
  Role,
  RoleBrief,
  RoleDetail,
  RoleQuery,
  CreateRoleRequest,
  UpdateRoleRequest,
  UpdateRolePermissionsRequest,
} from '@/types/role'

/** 角色分页列表 */
export function getRoles(query?: RoleQuery) {
  return get<PagedResponse<Role>>('/roles', query as Record<string, string | number>)
}

/** 所有角色 (下拉) */
export function getAllRoles() {
  return get<ApiResponse<RoleBrief[]>>('/roles/all')
}

/** 角色详情 */
export function getRoleById(id: string) {
  return get<ApiResponse<RoleDetail>>(`/roles/${id}`)
}

/** 创建角色 */
export function createRole(data: CreateRoleRequest) {
  return post<ApiResponse>('/roles', data)
}

/** 更新角色 */
export function updateRole(id: string, data: UpdateRoleRequest) {
  return put<ApiResponse>(`/roles/${id}`, data)
}

/** 删除角色 */
export function deleteRole(id: string) {
  return del<ApiResponse>(`/roles/${id}`)
}

/** 分配权限 */
export function updateRolePermissions(id: string, data: UpdateRolePermissionsRequest) {
  return put<ApiResponse>(`/roles/${id}/permissions`, data)
}

/** 角色下的用户 */
export function getRoleUsers(id: string) {
  return get<ApiResponse<{ id: string; username: string; fullName: string; status: number }[]>>(
    `/roles/${id}/users`
  )
}
