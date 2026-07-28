/**
 * 实验室管理 API
 * 对应后端 LabsController
 *  GET    /api/v1/labs
 *  GET    /api/v1/labs/{id}
 *  POST   /api/v1/labs
 *  PUT    /api/v1/labs/{id}
 *  DELETE /api/v1/labs/{id}
 *  PATCH  /api/v1/labs/{id}/status
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { Lab, LabQuery, CreateLabRequest, UpdateLabRequest } from '@/types/campus'

/** 实验室列表 */
export function getLabs(query?: LabQuery) {
  return get<ApiResponse<Lab[]>>('/labs', query as Record<string, string>)
}

/** 实验室详情 */
export function getLabById(id: string) {
  return get<ApiResponse<Lab>>(`/labs/${id}`)
}

/** 创建实验室 */
export function createLab(data: CreateLabRequest) {
  return post<ApiResponse>('/labs', data)
}

/** 更新实验室 */
export function updateLab(id: string, data: UpdateLabRequest) {
  return put<ApiResponse>(`/labs/${id}`, data)
}

/** 删除实验室 */
export function deleteLab(id: string) {
  return del<ApiResponse>(`/labs/${id}`)
}

/** 启用/禁用实验室 */
export function toggleLabStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/labs/${id}/status`, { isActive })
}
