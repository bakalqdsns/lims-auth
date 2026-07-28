/**
 * 楼宇管理 API
 * 对应后端 BuildingsController
 *  GET    /api/v1/buildings
 *  GET    /api/v1/buildings/{id}
 *  GET    /api/v1/buildings/by-campus/{campusId}
 *  POST   /api/v1/buildings
 *  PUT    /api/v1/buildings/{id}
 *  DELETE /api/v1/buildings/{id}
 *  PATCH  /api/v1/buildings/{id}/status
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { Building, CreateBuildingRequest, UpdateBuildingRequest } from '@/types/campus'

/** 楼宇列表 */
export function getBuildings(params?: { keyword?: string; campusId?: string }) {
  return get<ApiResponse<Building[]>>('/buildings', params as Record<string, string>)
}

/** 楼宇详情 */
export function getBuildingById(id: string) {
  return get<ApiResponse<Building>>(`/buildings/${id}`)
}

/** 按校区筛选楼宇 */
export function getBuildingsByCampus(campusId: string) {
  return get<ApiResponse<Building[]>>(`/buildings/by-campus/${campusId}`)
}

/** 创建楼宇 */
export function createBuilding(data: CreateBuildingRequest) {
  return post<ApiResponse>('/buildings', data)
}

/** 更新楼宇 */
export function updateBuilding(id: string, data: UpdateBuildingRequest) {
  return put<ApiResponse>(`/buildings/${id}`, data)
}

/** 删除楼宇 */
export function deleteBuilding(id: string) {
  return del<ApiResponse>(`/buildings/${id}`)
}

/** 启用/禁用楼宇 */
export function toggleBuildingStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/buildings/${id}/status`, { isActive })
}
