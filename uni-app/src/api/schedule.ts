/**
 * 排课管理 API
 * 对应后端 SchedulesController
 *  GET  /api/v1/schedules
 *  GET  /api/v1/schedules/{id}
 *  POST /api/v1/schedules
 *  PUT  /api/v1/schedules/{id}
 *  DEL  /api/v1/schedules/{id}
 *  GET  /api/v1/schedules/table-view
 *  GET  /api/v1/schedules/available-labs
 *  POST /api/v1/schedules/check-conflicts
 *  GET  /api/v1/schedules/by-lab/{labId}
 *  GET  /api/v1/schedules/by-teacher/{teacherId}
 *  GET  /api/v1/schedules/by-class/{classId}
 *  GET  /api/v1/schedules/my
 *  GET  /api/v1/schedules/importable-tasks
 *  POST /api/v1/schedules/import-from-tasks
 */
import { get, post, put, del } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  Schedule,
  ScheduleQuery,
  CreateScheduleRequest,
  UpdateScheduleRequest,
  ScheduleTableRow,
  AvailabilityQuery,
  ScheduleEntry,
  ConflictCheckResult,
  ExperimentTaskImportDto,
  ImportTasksRequest,
} from '@/types/schedule'

/** 排课列表 */
export function getSchedules(query?: ScheduleQuery) {
  return get<ApiResponse<Schedule[]>>('/schedules', query as Record<string, string>)
}

/** 排课详情 */
export function getScheduleById(id: string) {
  return get<ApiResponse<Schedule>>(`/schedules/${id}`)
}

/** 创建排课 */
export function createSchedule(data: CreateScheduleRequest) {
  return post<ApiResponse>('/schedules', data)
}

/** 更新排课 */
export function updateSchedule(id: string, data: UpdateScheduleRequest) {
  return put<ApiResponse>(`/schedules/${id}`, data)
}

/** 删除排课 */
export function deleteSchedule(id: string) {
  return del<ApiResponse>(`/schedules/${id}`)
}

/** 课表视图 (按天/节次聚合) */
export function getScheduleTableView(query?: ScheduleQuery) {
  return get<ApiResponse<ScheduleTableRow[]>>('/schedules/table-view', query as Record<string, string>)
}

/** 可用实验室 */
export function getAvailableLabs(query: AvailabilityQuery) {
  return get<ApiResponse<{ id: string; name: string; code: string; capacity: number }[]>>(
    '/schedules/available-labs',
    query as unknown as Record<string, string>
  )
}

/** 冲突检测 */
export function checkScheduleConflicts(entry: Partial<ScheduleEntry>) {
  return post<ApiResponse<ConflictCheckResult>>('/schedules/check-conflicts', entry)
}

/** 按实验室筛选 */
export function getSchedulesByLab(labId: string, query?: Omit<ScheduleQuery, 'labId'>) {
  return get<ApiResponse<Schedule[]>>(`/schedules/by-lab/${labId}`, query as Record<string, string>)
}

/** 按教师筛选 */
export function getSchedulesByTeacher(teacherId: string, query?: Omit<ScheduleQuery, 'teacherId'>) {
  return get<ApiResponse<Schedule[]>>(
    `/schedules/by-teacher/${teacherId}`,
    query as Record<string, string>
  )
}

/** 按班级筛选 */
export function getSchedulesByClass(classId: string, query?: Omit<ScheduleQuery, 'classId'>) {
  return get<ApiResponse<Schedule[]>>(
    `/schedules/by-class/${classId}`,
    query as Record<string, string>
  )
}

/** 当前用户的课表（学生按所在班级 + 教师按 teacherId） */
export function getMySchedules(query?: ScheduleQuery) {
  return get<ApiResponse<Schedule[]>>('/schedules/my', query as Record<string, string>)
}

/** 可导入的实验任务 */
export function getImportableTasks(semesterId: string) {
  return get<ApiResponse<ExperimentTaskImportDto[]>>('/schedules/importable-tasks', { semesterId })
}

/** 从实验任务批量导入 */
export function importSchedulesFromTasks(data: ImportTasksRequest) {
  return post<ApiResponse<number>>('/schedules/import-from-tasks', data)
}
