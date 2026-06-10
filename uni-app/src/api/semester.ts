/**
 * 学期管理 API
 */
import { get, post, put, del } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Semester, CreateSemesterRequest, CalendarItem, PeriodTime } from '@/types/semester'

export function getSemesters(query?: { page?: number; pageSize?: number; search?: string }) {
  return get<PagedResponse<Semester>>('/semesters', query as Record<string, string | number>)
}

export function getCurrentSemester() {
  return get<Semester>('/semesters/current')
}

export function getSemesterById(id: number) {
  return get<Semester>(`/semesters/${id}`)
}

export function createSemester(data: CreateSemesterRequest) {
  return post<ApiResponse>('/semesters', data)
}

export function updateSemester(id: number, data: Partial<CreateSemesterRequest>) {
  return put<ApiResponse>(`/semesters/${id}`, data)
}

export function deleteSemester(id: number) {
  return del<ApiResponse>(`/semesters/${id}`)
}

export function setCurrentSemester(id: number) {
  return post<ApiResponse>(`/semesters/${id}/set-current`)
}

export function getSemesterCalendar(semesterId: number) {
  return get<CalendarItem[]>(`/semesters/${semesterId}/calendar`)
}

export function generateCalendar(semesterId: number) {
  return post<ApiResponse>(`/semesters/${semesterId}/generate-calendar`)
}

export function getWeekInfo(semesterId: number) {
  return get<{ currentWeek: number; totalWeeks: number; startDate: string; endDate: string }>(
    `/semesters/${semesterId}/week-info`
  )
}

// 节次时间
export function getPeriodTimes() {
  return get<PeriodTime[]>('/period-times')
}

export function createPeriodTime(data: Partial<PeriodTime>) {
  return post<ApiResponse>('/period-times', data)
}

export function updatePeriodTime(id: number, data: Partial<PeriodTime>) {
  return put<ApiResponse>(`/period-times/${id}`, data)
}

export function deletePeriodTime(id: number) {
  return del<ApiResponse>(`/period-times/${id}`)
}
