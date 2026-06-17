/**
 * 使用登记 API
 * 对应后端 UsageRegistrationsController
 *  GET  /api/v1/usage-registrations
 *  GET  /api/v1/usage-registrations/{id}
 *  POST /api/v1/usage-registrations
 *  GET  /api/v1/usage-registrations/pending
 *  GET  /api/v1/usage-registrations/overdue
 *  PUT  /api/v1/usage-registrations/{id}/remind
 *  GET  /api/v1/usage-registrations/statistics/completion
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  UsageRegistration,
  UsageRegistrationQuery,
  CreateUsageRegistrationRequest,
  CompletionRateStat,
} from '@/types/teaching'

/** 登记列表 */
export function getUsageRegistrations(query?: UsageRegistrationQuery) {
  return get<ApiResponse<UsageRegistration[]>>('/usage-registrations', query as Record<string, string>)
}

/** 登记详情 */
export function getUsageRegistrationById(id: string) {
  return get<ApiResponse<UsageRegistration>>(`/usage-registrations/${id}`)
}

/** 创建登记 */
export function createUsageRegistration(data: CreateUsageRegistrationRequest) {
  return post<ApiResponse>('/usage-registrations', data)
}

/** 待补登记录 */
export function getPendingUsageRegistrations(params?: { userId?: string; semesterId?: string }) {
  return get<ApiResponse<UsageRegistration[]>>('/usage-registrations/pending', params as Record<string, string>)
}

/** 逾期未登记 */
export function getOverdueUsageRegistrations(semesterId?: string) {
  return get<ApiResponse<UsageRegistration[]>>(
    '/usage-registrations/overdue',
    semesterId ? { semesterId } : {}
  )
}

/** 发送提醒 */
export function remindUsageRegistration(id: string) {
  return put<ApiResponse>(`/usage-registrations/${id}/remind`, {})
}

/** 完成率统计 */
export function getCompletionRate(params?: { semesterId?: string; startDate?: string; endDate?: string }) {
  return get<ApiResponse<CompletionRateStat>>(
    '/usage-registrations/statistics/completion',
    params as Record<string, string>
  )
}
