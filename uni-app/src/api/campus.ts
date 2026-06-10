/**
 * 校区管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Campus, Building } from '@/types/campus'

export function getCampuses() {
  return get<Campus[]>('/campuses')
}

export function getCampusById(id: string) {
  return get<Campus>(`/campuses/${id}`)
}

export function createCampus(data: Partial<Campus>) {
  return post<ApiResponse>('/campuses', data)
}

export function updateCampus(id: string, data: Partial<Campus>) {
  return put<ApiResponse>(`/campuses/${id}`, data)
}

export function deleteCampus(id: string) {
  return del<ApiResponse>(`/campuses/${id}`)
}

export function updateCampusStatus(id: string, status: number) {
  return patch<ApiResponse>(`/campuses/${id}/status`, { status })
}

// 楼宇
export function getBuildings(campusId?: string) {
  return get<Building[]>('/buildings', campusId ? { campusId } as Record<string, string | number> : {})
}

export function getBuildingsByCampus(campusId: string) {
  return get<Building[]>(`/buildings/by-campus/${campusId}`)
}

export function getBuildingById(id: string) {
  return get<Building>(`/buildings/${id}`)
}

export function createBuilding(data: Partial<Building>) {
  return post<ApiResponse>('/buildings', data)
}

export function updateBuilding(id: string, data: Partial<Building>) {
  return put<ApiResponse>(`/buildings/${id}`, data)
}

export function deleteBuilding(id: string) {
  return del<ApiResponse>(`/buildings/${id}`)
}

export function updateBuildingStatus(id: string, status: number) {
  return patch<ApiResponse>(`/buildings/${id}/status`, { status })
}
