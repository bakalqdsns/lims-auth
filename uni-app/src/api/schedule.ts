/**
 * 排课 API
 */
import { get, post, put, del } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Schedule, ScheduleQuery } from '@/types/schedule'

export function getSchedules(query?: ScheduleQuery) {
  return get<PagedResponse<Schedule>>('/schedules', query as Record<string, string | number>)
}

export function getScheduleById(id: number) {
  return get<Schedule>(`/schedules/${id}`)
}

export function createSchedule(data: Partial<Schedule>) {
  return post<ApiResponse>('/schedules', data)
}

export function updateSchedule(id: number, data: Partial<Schedule>) {
  return put<ApiResponse>(`/schedules/${id}`, data)
}

export function deleteSchedule(id: number) {
  return del<ApiResponse>(`/schedules/${id}`)
}

export function getScheduleTableView(query?: {
  semesterId?: number
  classId?: number
  teacherId?: number
  labId?: number
}) {
  return get<{ days: { dayOfWeek: number; dayName: string; periods: { period: number; courses: Schedule[] }[] }[] }>(
    '/schedules/table-view',
    query as Record<string, string | number>
  )
}

export function checkScheduleConflicts(data: Partial<Schedule>) {
  return post<{ hasConflict: boolean; conflicts: { labName: string; date: string; period: string }[] }>(
    '/schedules/check-conflicts',
    data
  )
}

export function getAvailableLabs(date: string, startPeriod: number, endPeriod: number) {
  return get<{ id: number; name: string; code: string; capacity: number }[]>(
    '/schedules/available-labs',
    { date, startPeriod, endPeriod } as Record<string, string | number>
  )
}

export function getSchedulesByClass(classId: number) {
  return get<Schedule[]>(`/schedules/by-class/${classId}`)
}

export function getSchedulesByLab(labId: number) {
  return get<Schedule[]>(`/schedules/by-lab/${labId}`)
}

export function getSchedulesByTeacher(teacherId: number) {
  return get<Schedule[]>(`/schedules/by-teacher/${teacherId}`)
}

export function importSchedulesFromTasks(semesterId: number) {
  return post<ApiResponse>('/schedules/import-from-tasks', { semesterId })
}
