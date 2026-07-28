/**
 * 认证 API
 * 对应后端 AuthController
 *  POST /api/v1/auth/login
 *  GET  /api/v1/auth/me
 *  PUT  /api/v1/auth/profile
 *  POST /api/v1/auth/refresh
 *  GET  /api/v1/auth/health
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { UserInfo, LoginRequest, UpdateProfileRequest } from '@/types/user'

export interface LoginData {
  token: string
  expiresAt?: string
  user: UserInfo
}

/** 登录 */
export function login(data: LoginRequest) {
  return post<LoginData>('/auth/login', data, { loadingText: '登录中...' })
}

/** 获取当前登录用户信息 */
export function getCurrentUser() {
  return get<ApiResponse<UserInfo>>('/auth/me')
}

/** 更新个人资料 */
export function updateProfile(data: UpdateProfileRequest) {
  return put<ApiResponse>('/auth/profile', data)
}

/** 刷新 Token */
export function refreshToken() {
  return post<ApiResponse<{ token: string }>>('/auth/refresh')
}

/** 健康检查 */
export function healthCheck() {
  return get<ApiResponse<{ time: string }>>('/auth/health', {}, { loading: false, showError: false })
}
