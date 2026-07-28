/**
 * 学期相关类型定义
 * 与后端 SemesterDto / Calendar / PeriodTime 对齐
 */

export type SemesterStatus = 'Planning' | 'Active' | 'InProgress' | 'Finished' | 'Archived'
export type SemesterType = 'Spring' | 'Autumn' | 'Summer' | 'Winter'

export interface Semester {
  id: string
  name: string
  code?: string
  academicYear?: string
  type?: SemesterType
  startDate: string
  endDate: string
  weekCount: number
  currentWeek: number
  isCurrent: boolean
  status: SemesterStatus
  description?: string
  templateId?: string
  isTemplate?: boolean
  parentSemesterId?: string
  createdBy?: string
  createdAt: string
}

export interface SemesterQuery {
  keyword?: string
  status?: SemesterStatus
  type?: SemesterType
  isCurrent?: boolean
}

export interface CreateSemesterRequest {
  name: string
  code?: string
  academicYear?: string
  type?: SemesterType
  startDate: string
  endDate: string
  description?: string
}

export interface UpdateSemesterRequest {
  name?: string
  code?: string
  academicYear?: string
  type?: SemesterType
  startDate?: string
  endDate?: string
  description?: string
  status?: SemesterStatus
}

export interface CalendarItem {
  id: string
  semesterId: string
  date: string
  weekNumber: number
  dayOfWeek: number
  eventType?: string
  eventName?: string
  isHoliday: boolean
  isWorkday: boolean
  isTeachingDay: boolean
  isExamDay?: boolean
  isAdjusted?: boolean
  holidayName?: string
  holidayType?: string
  description?: string
  color?: string
}

export interface WeekInfo {
  weekNumber: number
  startDate: string
  endDate: string
  days?: CalendarItem[]
}

export interface PeriodTime {
  id: string
  name: string
  startTime: string
  endTime: string
  order: number
  type?: 'Morning' | 'Afternoon' | 'Evening' | 'Night'
  isActive: boolean
  description?: string
}

export interface CreatePeriodTimeRequest {
  name: string
  startTime: string
  endTime: string
  order: number
  type?: PeriodTime['type']
  description?: string
}

export interface UpdatePeriodTimeRequest {
  name?: string
  startTime?: string
  endTime?: string
  order?: number
  type?: PeriodTime['type']
  description?: string
}

export interface GenerateCalendarRequest {
  startWeek?: number
  includeHolidays?: boolean
  excludeDates?: string[]
}

export interface CopyOptions {
  copyTasks?: boolean
  copyReservations?: boolean
  copySchedules?: boolean
  copyEquipment?: boolean
}

export interface CopyFromTemplateRequest {
  templateId: string
  name: string
  code?: string
  academicYear?: string
  startDate: string
  endDate: string
}

export interface CopyFromSemesterRequest {
  sourceSemesterId: string
  name: string
  code?: string
  academicYear?: string
  startDate: string
  endDate: string
  copyOptions?: CopyOptions
}

export interface Sandbox {
  id: string
  semesterId: string
  name: string
  createdBy: string
  createdAt: string
  expiresAt?: string
}

export interface OperationLog {
  id: string
  semesterId: string
  action: string
  operatorId: string
  operatorName: string
  detail?: string
  createdAt: string
}
