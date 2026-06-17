/**
 * 专业管理 API
 * 对应后端 MajorsController
 *  GET    /api/v1/majors
 *  GET    /api/v1/majors/all
 *  GET    /api/v1/majors/{id}
 *  POST   /api/v1/majors
 *  PUT    /api/v1/majors/{id}
 *  DELETE /api/v1/majors/{id}
 *  PATCH  /api/v1/majors/{id}/status
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { Major, MajorQuery, CreateMajorRequest, UpdateMajorRequest } from '@/types/teaching'

/** 专业列表 */
export function getMajors(query?: MajorQuery) {
  return get<ApiResponse<Major[]>>('/majors', query as Record<string, string>)
}

/** 所有专业 (下拉) */
export function getAllMajors() {
  return get<ApiResponse<Major[]>>('/majors/all')
}

/** 专业详情 */
export function getMajorById(id: string) {
  return get<ApiResponse<Major>>(`/majors/${id}`)
}

/** 创建专业 */
export function createMajor(data: CreateMajorRequest) {
  return post<ApiResponse>('/majors', data)
}

/** 更新专业 */
export function updateMajor(id: string, data: UpdateMajorRequest) {
  return put<ApiResponse>(`/majors/${id}`, data)
}

/** 删除专业 */
export function deleteMajor(id: string) {
  return del<ApiResponse>(`/majors/${id}`)
}

/** 启用/禁用专业 */
export function toggleMajorStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/majors/${id}/status`, { isActive })
}
