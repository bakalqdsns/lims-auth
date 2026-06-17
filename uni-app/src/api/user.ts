/**
 * 用户管理 API
 * 对应后端 UsersController
 *  GET    /api/v1/users
 *  GET    /api/v1/users/{id}
 *  POST   /api/v1/users
 *  PUT    /api/v1/users/{id}
 *  DELETE /api/v1/users/{id}
 *  PATCH  /api/v1/users/{id}/status
 *  PUT    /api/v1/users/{id}/roles
 *  PUT    /api/v1/users/{id}/password
 *  POST   /api/v1/users/change-password
 *  GET    /api/v1/users/permissions/my
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type {
  UserInfo,
  UserQuery,
  CreateUserRequest,
  UpdateUserRequest,
  UserDetail,
  UserListItem,
  ResetPasswordRequest,
} from '@/types/user'

/** 用户列表 */
export function getUsers(query?: UserQuery) {
  return get<PagedResponse<UserListItem>>('/users', query as Record<string, string | number>)
}

/** 用户详情 */
export function getUserById(id: string) {
  return get<ApiResponse<UserDetail>>(`/users/${id}`)
}

/** 创建用户 */
export function createUser(data: CreateUserRequest) {
  return post<ApiResponse>('/users', data)
}

/** 更新用户 */
export function updateUser(id: string, data: UpdateUserRequest) {
  return put<ApiResponse>(`/users/${id}`, data)
}

/** 删除用户 */
export function deleteUser(id: string) {
  return del<ApiResponse>(`/users/${id}`)
}

/** 启用/禁用用户 */
export function updateUserStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/users/${id}/status`, { isActive })
}

/** 分配角色 */
export function updateUserRoles(id: string, roleIds: string[]) {
  return put<ApiResponse>(`/users/${id}/roles`, { roleIds })
}

/** 重置密码 (管理员) */
export function resetUserPassword(id: string, data: ResetPasswordRequest) {
  return put<ApiResponse>(`/users/${id}/password`, data)
}

/** 修改自己的密码 */
export function changePassword(oldPassword: string, newPassword: string) {
  return post<ApiResponse>('/users/change-password', { oldPassword, newPassword })
}

/** 获取当前用户权限码集合 */
export function getMyPermissions() {
  return get<ApiResponse<string[]>>('/users/permissions/my')
}

/** 当前登录用户简要信息 */
export function getMyProfile() {
  return get<UserInfo>('/auth/me')
}
