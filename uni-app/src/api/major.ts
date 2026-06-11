/**
 * 专业管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Major } from '@/types/teaching'

export function getMajors(query?: { page?: number; pageSize?: number; search?: string }) {
  return get<PagedResponse<Major>>('/majors', query as Record<string, string | number>)
}

export function getAllMajors() {
  return get<Major[]>('/majors/all')
}

export function getMajorById(id: number) {
  return get<Major>(`/majors/${id}`)
}

export function createMajor(data: Partial<Major>) {
  return post<ApiResponse>('/majors', data)
}

export function updateMajor(id: number, data: Partial<Major>) {
  return put<ApiResponse>(`/majors/${id}`, data)
}

export function deleteMajor(id: number) {
  return del<ApiResponse>(`/majors/${id}`)
}

export function updateMajorStatus(id: number, status: number) {
  return patch<ApiResponse>(`/majors/${id}/status`, { status })
}
