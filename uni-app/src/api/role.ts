/**
 * 角色管理 API
 */
import { get, post, put, del } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Role, CreateRoleRequest } from '@/types/role'

export function getRoles(query?: { page?: number; pageSize?: number; search?: string }) {
  return get<PagedResponse<Role>>('/roles', query as Record<string, string | number>)
}

export function getAllRoles() {
  return get<Role[]>('/roles/all')
}

export function getRoleById(id: number) {
  return get<Role>(`/roles/${id}`)
}

export function createRole(data: CreateRoleRequest) {
  return post<ApiResponse>('/roles', data)
}

export function updateRole(id: number, data: Partial<CreateRoleRequest>) {
  return put<ApiResponse>(`/roles/${id}`, data)
}

export function deleteRole(id: number) {
  return del<ApiResponse>(`/roles/${id}`)
}

export function assignRolePermissions(id: number, permissionIds: number[]) {
  return put<ApiResponse>(`/roles/${id}/permissions`, { permissionIds })
}

export function getRoleUsers(id: number) {
  return get<{ id: number; username: string; fullName: string }[]>(`/roles/${id}/users`)
}
