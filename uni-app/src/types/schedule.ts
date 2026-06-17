/**
 * 排课相关类型定义
 * 与后端 ScheduleEntryDto / ScheduleTableRow / ConflictCheckResult 对齐
 */

export interface Schedule {
  id: string
  semesterId: string
  semesterName?: string
  labId?: string
  labName?: string
  buildingName?: string
  weekNumber: number
  startWeek?: number
  endWeek?: number
  dayOfWeek: number
  periodNumber: number
  source?: string
  status?: string
  courseId?: string
  courseName?: string
  projectName?: string
  teacherId?: string
  teacherName?: string
  classId?: string
  className?: string
  majorId?: string
  majorName?: string
  studentCount?: number
  remark?: string
  hasConflict?: boolean
  conflictInfo?: string
  createdAt?: string
  /** 前端派生字段 */
  startPeriod?: number
  endPeriod?: number
  startTime?: string
  endTime?: string
  labCode?: string
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
