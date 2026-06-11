/**
 * 学期相关类型定义
 * 与 Flutter lib/models/semester.dart 对齐
 */

export interface Semester {
  id: number
  name: string
  startDate: string
  endDate: string
  currentWeek: number
  isCurrent: boolean
  status: number
  weekCount: number
  createdAt: string
}

/**
 * 创建学期请求
 */
export interface CreateSemesterRequest {
  name: string
  startDate: string
  endDate: string
}

/**
 * 学期日历项
 */
export interface CalendarItem {
  id: number
  date: string
  dayOfWeek: number
  weekNumber: number
  isHoliday: boolean
  holidayName?: string
  eventType?: string
  eventName?: string
}

/**
 * 节次时间
 */
export interface PeriodTime {
  id: number
  name: string
  startTime: string
  endTime: string
  order: number
  enabled: boolean
}

/**
 * 学期日历查询
 */
export interface CalendarQuery {
  semesterId: number
  startDate?: string
  endDate?: string
}
