/**
 * 排课相关类型定义
 * 与 Flutter lib/models/schedule.dart 对齐
 */

export interface Schedule {
  id: number
  courseId: number
  courseName: string
  classId: number
  className: string
  labId: number
  labName: string
  labCode: string
  teacherId: number
  teacherName: string
  semesterId: number
  semesterName: string
  dayOfWeek: number
  weekNumber: number
  startPeriod: number
  endPeriod: number
  startTime: string
  endTime: string
  weekPattern: string // e.g. "1-16周(单)"
  status: number
  description?: string
}

export interface ScheduleQuery {
  page?: number
  pageSize?: number
  semesterId?: number
  classId?: number
  teacherId?: number
  labId?: number
  dayOfWeek?: number
  weekNumber?: number
}

export interface ScheduleConflict {
  labId: number
  labName: string
  date: string
  period: string
  conflictCourse: string
  conflictClass: string
}

/**
 * 课表视图 (按星期排列)
 */
export interface ScheduleTableView {
  dayOfWeek: number
  dayName: string
  periods: PeriodSchedule[]
}

export interface PeriodSchedule {
  period: number
  startTime: string
  endTime: string
  courses: Schedule[]
}
