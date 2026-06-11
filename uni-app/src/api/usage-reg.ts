/**
 * 使用登记 API
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { UsageRegistration } from '@/types/teaching'

export function getUsageRegistrations(query?: {
  page?: number
  pageSize?: number
  labId?: number
  status?: number
  date?: string
}) {
  return get<PagedResponse<UsageRegistration>>('/usage-registrations', query as Record<string, string | number>)
}

export function getUsageRegistrationById(id: number) {
  return get<UsageRegistration>(`/usage-registrations/${id}`)
}

export function createUsageRegistration(data: Partial<UsageRegistration>) {
  return post<ApiResponse>('/usage-registrations', data)
}

export function remindUsageRegistration(id: number) {
  return put<ApiResponse>(`/usage-registrations/${id}/remind`, {})
}

export function getPendingUsageRegistrations() {
  return get<PagedResponse<UsageRegistration>>('/usage-registrations/pending')
}

export function getOverdueUsageRegistrations() {
  return get<PagedResponse<UsageRegistration>>('/usage-registrations/overdue')
}

export function getCompletionRate() {
  return get<{ total: number; completed: number; rate: number }>('/usage-registrations/statistics/completion')
}
