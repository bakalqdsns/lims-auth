/**
 * 实验室管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Lab, LabQuery, CreateLabRequest } from '@/types/campus'

export function getLabs(query?: LabQuery) {
  return get<PagedResponse<Lab>>('/labs', query as Record<string, string | number>)
}

export function getLabById(id: string) {
  return get<Lab>(`/labs/${id}`)
}

export function createLab(data: CreateLabRequest) {
  return post<ApiResponse>('/labs', data)
}

export function updateLab(id: string, data: Partial<CreateLabRequest>) {
  return put<ApiResponse>(`/labs/${id}`, data)
}

export function deleteLab(id: string) {
  return del<ApiResponse>(`/labs/${id}`)
}

export function updateLabStatus(id: string, status: number) {
  return patch<ApiResponse>(`/labs/${id}/status`, { status })
}
