/**
 * 统计 API
 */
import { get } from '@/utils/request'
import type { DashboardStats, LabUsageStat, ReservationStat, WeeklySummary } from '@/types/teaching'

export function getDashboardStats() {
  return get<DashboardStats>('/statistics/dashboard')
}

export function getStatisticsByClass(semesterId?: number) {
  return get<{ className: string; completed: number; total: number; rate: number }[]>(
    '/statistics/by-class',
    { semesterId } as Record<string, string | number>
  )
}

export function getStatisticsByCourse(semesterId?: number) {
  return get<{ courseName: string; completed: number; total: number; rate: number }[]>(
    '/statistics/by-course',
    { semesterId } as Record<string, string | number>
  )
}

export function getStatisticsByGrade(grade?: number) {
  return get<{ grade: number; completed: number; total: number; rate: number }[]>(
    '/statistics/by-grade',
    { grade } as Record<string, string | number>
  )
}

export function getLabUsageStats() {
  return get<LabUsageStat[]>('/statistics/lab-usage')
}

export function getReservationStats(startDate?: string, endDate?: string) {
  return get<ReservationStat[]>('/statistics/reservation', {
    startDate,
    endDate,
  } as Record<string, string>)
}

export function getWeeklySummary() {
  return get<WeeklySummary[]>('/statistics/weekly-summary')
}

export function exportStatistics(query?: Record<string, string | number>) {
  return get<string>('/statistics/export', query as Record<string, string | number>)
}
