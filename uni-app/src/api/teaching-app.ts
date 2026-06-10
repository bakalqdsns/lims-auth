/**
 * 授课申请 API
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { TeachingApplication } from '@/types/teaching'

export function getTeachingApplications(query?: {
  page?: number
  pageSize?: number
  status?: number
  semesterId?: number
}) {
  return get<PagedResponse<TeachingApplication>>('/teaching-applications', query as Record<string, string | number>)
}

export function getMyTeachingApplications() {
  return get<PagedResponse<TeachingApplication>>('/teaching-applications/my')
}

export function getPendingTeachingApplications() {
  return get<PagedResponse<TeachingApplication>>('/teaching-applications/pending')
}

export function getTeachingApplicationById(id: number) {
  return get<TeachingApplication>(`/teaching-applications/${id}`)
}

export function createTeachingApplication(data: Partial<TeachingApplication>) {
  return post<ApiResponse>('/teaching-applications', data)
}

export function approveTeachingApplication(id: number, remark?: string) {
  return put<ApiResponse>(`/teaching-applications/${id}/approve`, { remark })
}

export function rejectTeachingApplication(id: number, remark?: string) {
  return put<ApiResponse>(`/teaching-applications/${id}/reject`, { remark })
}

export function cancelTeachingApplication(id: number) {
  return put<ApiResponse>(`/teaching-applications/${id}/cancel`, {})
}
