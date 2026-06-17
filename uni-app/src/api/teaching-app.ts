/**
 * 授课申请 API
 * 对应后端 TeachingApplicationsController
 *  GET  /api/v1/teaching-applications
 *  GET  /api/v1/teaching-applications/{id}
 *  POST /api/v1/teaching-applications
 *  PUT  /api/v1/teaching-applications/{id}/approve
 *  PUT  /api/v1/teaching-applications/{id}/reject
 *  PUT  /api/v1/teaching-applications/{id}/cancel
 *  GET  /api/v1/teaching-applications/pending
 *  GET  /api/v1/teaching-applications/my
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  TeachingApplication,
  TeachingApplicationQuery,
  CreateTeachingApplicationRequest,
  ApprovalRequest,
} from '@/types/teaching'

/** 申请分页列表 */
export function getTeachingApplications(query?: TeachingApplicationQuery) {
  return get<ApiResponse<TeachingApplication[]>>(
    '/teaching-applications',
    query as Record<string, string>
  )
}

/** 我的申请 */
export function getMyTeachingApplications(semesterId?: string) {
  return get<ApiResponse<TeachingApplication[]>>(
    '/teaching-applications/my',
    semesterId ? { semesterId } : {}
  )
}

/** 待审批申请 */
export function getPendingTeachingApplications(semesterId?: string) {
  return get<ApiResponse<TeachingApplication[]>>(
    '/teaching-applications/pending',
    semesterId ? { semesterId } : {}
  )
}

/** 申请详情 */
export function getTeachingApplicationById(id: string) {
  return get<ApiResponse<TeachingApplication>>(`/teaching-applications/${id}`)
}

/** 提交申请 */
export function createTeachingApplication(data: CreateTeachingApplicationRequest) {
  return post<ApiResponse>('/teaching-applications', data)
}

/** 审批通过 */
export function approveTeachingApplication(id: string, request: ApprovalRequest = { approved: true }) {
  return put<ApiResponse>(`/teaching-applications/${id}/approve`, request)
}

/** 审批驳回 */
export function rejectTeachingApplication(id: string, request: ApprovalRequest) {
  return put<ApiResponse>(`/teaching-applications/${id}/reject`, request)
}

/** 取消申请 */
export function cancelTeachingApplication(id: string) {
  return put<ApiResponse>(`/teaching-applications/${id}/cancel`, {})
}
