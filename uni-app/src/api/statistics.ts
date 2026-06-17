/**
 * 统计中心 API
 * 对应后端 StatisticsController
 *  GET /api/v1/statistics/weekly-summary
 *  GET /api/v1/statistics/lab-usage
 *  GET /api/v1/statistics/by-major
 *  GET /api/v1/statistics/by-class
 *  GET /api/v1/statistics/by-grade
 *  GET /api/v1/statistics/by-course
 *  GET /api/v1/statistics/reservation
 *  GET /api/v1/statistics/completion-rate
 *  GET /api/v1/statistics/dashboard
 *  GET /api/v1/statistics/export
 */
import { get } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  DashboardStats,
  LabUsageStat,
  MajorUsageStat,
  ClassUsageStat,
  GradeUsageStat,
  CourseUsageStat,
  ReservationStat,
  WeeklySummary,
  CompletionRateStat,
} from '@/types/teaching'

export interface StatisticsQuery {
  semesterId?: string
  startDate?: string
  endDate?: string
  labId?: string
  majorId?: string
  classId?: string
  grade?: string | number
  courseId?: string
}

export interface ScheduleStatisticsQuery extends StatisticsQuery {
  weekNumber?: number
  dayOfWeek?: number
}

export interface DashboardQuery {
  semesterId?: string
  startDate?: string
  endDate?: string
}

/* =============== 概览 =============== */

export function getDashboardStats(query?: DashboardQuery) {
  return get<ApiResponse<DashboardStats>>('/statistics/dashboard', query as Record<string, string>)
}

/** 周汇总 */
export function getWeeklySummary(query?: ScheduleStatisticsQuery) {
  return get<ApiResponse<WeeklySummary[]>>(
    '/statistics/weekly-summary',
    query as Record<string, string | number>
  )
}

/** 完成率 */
export function getCompletionRate(query?: StatisticsQuery) {
  return get<ApiResponse<CompletionRateStat>>(
    '/statistics/completion-rate',
    query as Record<string, string>
  )
}

/* =============== 实验室使用率 =============== */

export function getLabUsageStats(query?: StatisticsQuery) {
  return get<ApiResponse<LabUsageStat[]>>('/statistics/lab-usage', query as Record<string, string>)
}

/* =============== 维度统计 =============== */

export function getMajorUsageStats(query?: StatisticsQuery) {
  return get<ApiResponse<MajorUsageStat[]>>('/statistics/by-major', query as Record<string, string>)
}

export function getClassUsageStats(query?: StatisticsQuery) {
  return get<ApiResponse<ClassUsageStat[]>>('/statistics/by-class', query as Record<string, string>)
}

export function getGradeUsageStats(query?: StatisticsQuery) {
  return get<ApiResponse<GradeUsageStat[]>>('/statistics/by-grade', query as Record<string, string>)
}

export function getCourseUsageStats(query?: StatisticsQuery) {
  return get<ApiResponse<CourseUsageStat[]>>('/statistics/by-course', query as Record<string, string>)
}

/* =============== 预约统计 =============== */

export function getReservationStats(query?: StatisticsQuery) {
  return get<ApiResponse<ReservationStat[]>>(
    '/statistics/reservation',
    query as Record<string, string>
  )
}

/* =============== 导出 =============== */

export function exportStatistics(query?: StatisticsQuery & { type?: string }) {
  return get<ApiResponse>('/statistics/export', query as Record<string, string>, {
    loadingText: '导出中...',
  })
}
