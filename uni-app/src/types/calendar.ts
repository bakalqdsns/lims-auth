/**
 * 校历相关类型
 * 与后端 AcademicCalendarDto 对齐
 */

export type CalendarEventType =
  | 'Teaching'
  | 'Exam'
  | 'Holiday'
  | 'Registration'
  | 'CourseSelection'
  | 'GradeEntry'
  | 'Sports'
  | 'Activity'
  | 'Maintenance'
  | 'Custom'

export interface AcademicCalendar {
  id: string
  semesterId: string
  date: string
  weekNumber: number
  dayOfWeek: number
  eventType: string
  eventName?: string
  eventPriority?: string
  isHoliday: boolean
  isWorkday: boolean
  isTeachingDay: boolean
  isExamDay?: boolean
  isAdjusted?: boolean
  adjustedFrom?: string
  holidayName?: string
  holidayType?: string
  description?: string
  color?: string
  icon?: string
  affectsCourseSelection: boolean
  affectsScheduling: boolean
  affectsGradeEntry: boolean
  affectsRegistration: boolean
  autoTriggerAction?: string
  triggeredAt?: string
  createdAt: string
  updatedAt?: string
}

export interface UpdateCalendarRequest {
  eventType?: CalendarEventType
  eventName?: string
  isHoliday?: boolean
  isWorkday?: boolean
  isTeachingDay?: boolean
  holidayName?: string
  description?: string
  color?: string
}

export interface HolidayDto {
  id: string
  date: string
  name?: string
  type?: string
  isWorkday: boolean
  description?: string
}

export interface AddHolidayRequest {
  date: string
  name: string
  type?: string
  isWorkday?: boolean
  description?: string
}

export interface AdjustWorkdayRequest {
  date: string
  isWorkday: boolean
  adjustedFrom?: string
  description?: string
}

export interface BusinessPermission {
  isAllowed: boolean
  message: string
  reason: string
  currentDate: string
  weekNumber: number
  dayOfWeek: number
}
