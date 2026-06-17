/**
 * 排课相关类型定义
 * 与后端 ScheduleEntryDto / ScheduleTableRow / ConflictCheckResult 对齐
 */

export interface Schedule {
  id: string
  courseId: string
  courseName: string
  courseCode?: string
  classId: string
  className: string
  teacherId: string
  teacherName: string
  semesterId: string
  semesterName?: string
  labId: string
  labName: string
  labCode?: string
  dayOfWeek: number
  weekNumber?: number
  startPeriod: number
  endPeriod: number
  startTime?: string
  endTime?: string
  weekPattern: string
  startWeek: number
  endWeek: number
  status: number
  description?: string
  createdAt: string
}

export interface ScheduleQuery {
  semesterId?: string
  classId?: string
  teacherId?: string
  labId?: string
  dayOfWeek?: number
  weekNumber?: number
  startDate?: string
  endDate?: string
}

export interface CreateScheduleRequest {
  courseId: string
  classId: string
  teacherId: string
  semesterId: string
  labId: string
  dayOfWeek: number
  startPeriod: number
  endPeriod: number
  weekPattern: string
  startWeek: number
  endWeek: number
  description?: string
}

export interface UpdateScheduleRequest {
  labId?: string
  dayOfWeek?: number
  startPeriod?: number
  endPeriod?: number
  weekPattern?: string
  startWeek?: number
  endWeek?: number
  description?: string
}

export interface ScheduleTableRow {
  dayOfWeek: number
  dayName: string
  periods: {
    period: number
    startTime: string
    endTime: string
    schedules: Schedule[]
  }[]
}

export interface AvailabilityQuery {
  date: string
  startPeriod: number
  endPeriod: number
  semesterId?: string
  weekNumber?: number
}

export interface ScheduleEntry {
  id?: string
  semesterId: string
  labId: string
  teacherId?: string
  classId?: string
  courseId?: string
  dayOfWeek: number
  weekPattern: string
  startWeek: number
  endWeek: number
  startPeriod: number
  endPeriod: number
}

export interface ConflictItem {
  type: 'lab' | 'teacher' | 'class'
  resourceId: string
  resourceName: string
  weekNumber: number
  dayOfWeek: number
  startPeriod: number
  endPeriod: number
  description: string
}

export interface ConflictCheckResult {
  hasConflict: boolean
  conflicts: ConflictItem[]
}

export interface ExperimentTaskImportDto {
  taskId: string
  courseId: string
  courseName: string
  classId: string
  className: string
  teacherId: string
  teacherName: string
  weekNumber: number
  dayOfWeek: number
  startPeriod: number
  endPeriod: number
  selected?: boolean
}

export interface ImportTasksRequest {
  taskIds: string[]
  defaultLabId?: string
}
