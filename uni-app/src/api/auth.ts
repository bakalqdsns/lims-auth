/**
 * 认证 API
 * POST /api/v1/auth/login
 * GET  /api/v1/auth/me
 * PUT  /api/v1/auth/profile
 * POST /api/v1/auth/refresh
 * GET  /api/v1/auth/health
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { UserInfo, LoginRequest, UpdateProfileRequest, ChangePasswordRequest } from '@/types/user'

export interface LoginApiResponse {
  token: string
  user: UserInfo
}

/** 登录 */
export function login(data: LoginRequest) {
  return post<LoginApiResponse>('/auth/login', data, { loadingText: '登录中...' })
}

/** 获取当前用户信息 */
export function getCurrentUser() {
  return get<UserInfo>('/auth/me')
}

/** 更新个人资料 */
export function updateProfile(data: UpdateProfileRequest) {
  return put<ApiResponse>('/auth/profile', data)
}

/** 修改密码 */
export function changePassword(data: ChangePasswordRequest) {
  return post<ApiResponse>('/users/change-password', data)
}

/** 刷新 Token */
export function refreshToken() {
  return post<{ token: string }>('/auth/refresh')
}

/** 健康检查 */
export function healthCheck() {
  return get<{ status: string }>('/auth/health', {}, { loading: false, showError: false })
}
