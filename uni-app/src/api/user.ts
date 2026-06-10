/**
 * 用户管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { UserInfo, UserQuery, CreateUserRequest } from '@/types/user'

export function getUsers(query?: UserQuery) {
  return get<PagedResponse<UserInfo>>('/users', query as Record<string, string | number>)
}

export function getUserById(id: number) {
  return get<UserInfo>(`/users/${id}`)
}

export function createUser(data: CreateUserRequest) {
  return post<ApiResponse>('/users', data)
}

export function updateUser(id: number, data: Partial<CreateUserRequest>) {
  return put<ApiResponse>(`/users/${id}`, data)
}

export function deleteUser(id: number) {
  return del<ApiResponse>(`/users/${id}`)
}

export function updateUserStatus(id: number, status: number) {
  return patch<ApiResponse>(`/users/${id}/status`, { status })
}

export function updateUserRoles(id: number, roleIds: number[]) {
  return put<ApiResponse>(`/users/${id}/roles`, { roleIds })
}

export function resetUserPassword(id: number, newPassword: string) {
  return put<ApiResponse>(`/users/${id}/password`, { password: newPassword })
}

export function getMyPermissions() {
  return get<string[]>('/users/permissions/my')
}
