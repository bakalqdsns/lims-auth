/**
 * 学期管理 API
 * 对应后端 SemestersController + PeriodTimesController
 *
 *  Semesters:
 *   GET    /api/v1/semesters
 *   GET    /api/v1/semesters/current
 *   GET    /api/v1/semesters/{id}
 *   POST   /api/v1/semesters
 *   PUT    /api/v1/semesters/{id}
 *   DELETE /api/v1/semesters/{id}
 *   POST   /api/v1/semesters/{id}/set-current
 *   POST   /api/v1/semesters/{id}/status
 *   POST   /api/v1/semesters/{id}/archive
 *   POST   /api/v1/semesters/auto-transition
 *   POST   /api/v1/semesters/validate
 *   GET    /api/v1/semesters/check-overlap
 *   POST   /api/v1/semesters/{id}/generate-calendar
 *   GET    /api/v1/semesters/{id}/calendar
 *   GET    /api/v1/semesters/{id}/week-info
 *   GET    /api/v1/semesters/current/today
 *   POST   /api/v1/semesters/copy-from-template
 *   POST   /api/v1/semesters/copy-from-semester
 *   POST   /api/v1/semesters/{id}/sandbox
 *   POST   /api/v1/semesters/sandbox/{sandboxId}/apply
 *   DELETE /api/v1/semesters/sandbox/{sandboxId}
 *   GET    /api/v1/semesters/{id}/logs
 *
 *  PeriodTimes:
 *   GET    /api/v1/period-times
 *   GET    /api/v1/period-times/{id}
 *   POST   /api/v1/period-times
 *   PUT    /api/v1/period-times/{id}
 *   DELETE /api/v1/period-times/{id}
 *   PATCH  /api/v1/period-times/{id}/status
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  Semester,
  SemesterQuery,
  CreateSemesterRequest,
  UpdateSemesterRequest,
  SemesterStatus,
  SemesterType,
  CopyOptions,
  CalendarItem,
  WeekInfo,
  PeriodTime,
  CreatePeriodTimeRequest,
  UpdatePeriodTimeRequest,
  GenerateCalendarRequest,
  CopyFromTemplateRequest,
  CopyFromSemesterRequest,
  Sandbox,
  OperationLog,
} from '@/types/semester'

/* =============== 学期 CRUD =============== */

export function getSemesters(query?: SemesterQuery) {
  return get<ApiResponse<Semester[]>>('/semesters', query as Record<string, string>)
}

export function getCurrentSemester() {
  return get<ApiResponse<Semester>>('/semesters/current')
}

export function getSemesterById(id: string) {
  return get<ApiResponse<Semester>>(`/semesters/${id}`)
}

export function createSemester(data: CreateSemesterRequest) {
  return post<ApiResponse>('/semesters', data)
}

export function updateSemester(id: string, data: UpdateSemesterRequest) {
  return put<ApiResponse>(`/semesters/${id}`, data)
}

export function deleteSemester(id: string) {
  return del<ApiResponse>(`/semesters/${id}`)
}

/* =============== 学期状态 =============== */

export function setCurrentSemester(id: string) {
  return post<ApiResponse>(`/semesters/${id}/set-current`, {})
}

export function updateSemesterStatus(id: string, status: SemesterStatus) {
  return post<ApiResponse>(`/semesters/${id}/status`, { status })
}

export function archiveSemester(id: string) {
  return post<ApiResponse>(`/semesters/${id}/archive`, {})
}

export function autoTransitionSemesterStatus() {
  return post<ApiResponse>('/semesters/auto-transition', {})
}

/* =============== 校验 =============== */

export function validateSemester(data: CreateSemesterRequest) {
  return post<ApiResponse<{ isValid: boolean; errors: string[] }>>('/semesters/validate', data)
}

export function checkSemesterOverlap(startDate: string, endDate: string, excludeId?: string) {
  return get<ApiResponse<{ hasOverlap: boolean }>>('/semesters/check-overlap', {
    startDate,
    endDate,
    excludeId,
  } as Record<string, string>)
}

/* =============== 校历 =============== */

export function generateCalendar(semesterId: string, request?: GenerateCalendarRequest) {
  return post<ApiResponse<CalendarItem[]>>(`/semesters/${semesterId}/generate-calendar`, request ?? {})
}

export function getSemesterCalendar(semesterId: string) {
  return get<ApiResponse<CalendarItem[]>>(`/semesters/${semesterId}/calendar`)
}

export function getWeekInfo(semesterId: string, weekNumber?: number) {
  return get<ApiResponse<WeekInfo | WeekInfo[]>>(
    `/semesters/${semesterId}/week-info`,
    weekNumber !== undefined ? { weekNumber } : {}
  )
}

export function getTodayCalendar() {
  return get<ApiResponse<CalendarItem>>('/semesters/current/today')
}

/* =============== 复制 =============== */

export function copySemesterFromTemplate(request: CopyFromTemplateRequest) {
  return post<ApiResponse>('/semesters/copy-from-template', request)
}

export function copySemesterFromSemester(request: CopyFromSemesterRequest) {
  return post<ApiResponse>('/semesters/copy-from-semester', request)
}

/* =============== 沙箱 =============== */

export function createSemesterSandbox(id: string, name: string) {
  return post<ApiResponse<Sandbox>>(`/semesters/${id}/sandbox`, { name })
}

export function applySemesterSandbox(sandboxId: string) {
  return post<ApiResponse>(`/semesters/sandbox/${sandboxId}/apply`, {})
}

export function discardSemesterSandbox(sandboxId: string) {
  return del<ApiResponse>(`/semesters/sandbox/${sandboxId}`)
}

/* =============== 日志 =============== */

export function getSemesterLogs(id: string) {
  return get<ApiResponse<OperationLog[]>>(`/semesters/${id}/logs`)
}

/* =============== 节次时间 =============== */

export function getPeriodTimes() {
  return get<ApiResponse<PeriodTime[]>>('/period-times')
}

export function getPeriodTimeById(id: string) {
  return get<ApiResponse<PeriodTime>>(`/period-times/${id}`)
}

export function createPeriodTime(data: CreatePeriodTimeRequest) {
  return post<ApiResponse>('/period-times', data)
}

export function updatePeriodTime(id: string, data: UpdatePeriodTimeRequest) {
  return put<ApiResponse>(`/period-times/${id}`, data)
}

export function deletePeriodTime(id: string) {
  return del<ApiResponse>(`/period-times/${id}`)
}

export function togglePeriodTimeStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/period-times/${id}/status`, { isActive })
}

export type { SemesterStatus, SemesterType, CopyOptions }
