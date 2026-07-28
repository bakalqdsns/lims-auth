/**
 * 校区管理 API
 * 对应后端 CampusesController
 *  GET    /api/v1/campuses
 *  GET    /api/v1/campuses/{id}
 *  POST   /api/v1/campuses
 *  PUT    /api/v1/campuses/{id}
 *  DELETE /api/v1/campuses/{id}
 *  PATCH  /api/v1/campuses/{id}/status
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { Campus, CreateCampusRequest, UpdateCampusRequest } from '@/types/campus'

/** 校区列表 (按关键字过滤) */
export function getCampuses(keyword?: string) {
  return get<ApiResponse<Campus[]>>('/campuses', keyword ? { keyword } : {})
}

/** 校区详情 */
export function getCampusById(id: string) {
  return get<ApiResponse<Campus>>(`/campuses/${id}`)
}

/** 创建校区 */
export function createCampus(data: CreateCampusRequest) {
  return post<ApiResponse>('/campuses', data)
}

/** 更新校区 */
export function updateCampus(id: string, data: UpdateCampusRequest) {
  return put<ApiResponse>(`/campuses/${id}`, data)
}

/** 删除校区 */
export function deleteCampus(id: string) {
  return del<ApiResponse>(`/campuses/${id}`)
}

/** 启用/禁用校区 */
export function toggleCampusStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/campuses/${id}/status`, { isActive })
}
