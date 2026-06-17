/**
 * 授课任务 API
 * 对应后端 TeachingTasksController
 *  GET    /api/v1/teaching-tasks
 *  GET    /api/v1/teaching-tasks/{id}
 *  POST   /api/v1/teaching-tasks
 *  PUT    /api/v1/teaching-tasks/{id}
 *  DELETE /api/v1/teaching-tasks/{id}
 *  PATCH  /api/v1/teaching-tasks/{id}/status
 *  POST   /api/v1/teaching-tasks/{id}/teachers
 *  DELETE /api/v1/teaching-tasks/{id}/teachers/{teacherId}
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  TeachingTask,
  TeachingTaskQuery,
  CreateTeachingTaskRequest,
  UpdateTeachingTaskRequest,
  AddTeachingTaskTeacherRequest,
} from '@/types/teaching'

/** 授课任务列表 */
export function getTeachingTasks(query?: TeachingTaskQuery) {
  return get<ApiResponse<TeachingTask[]>>('/teaching-tasks', query as Record<string, string>)
}

/** 授课任务详情 */
export function getTeachingTaskById(id: string) {
  return get<ApiResponse<TeachingTask>>(`/teaching-tasks/${id}`)
}

/** 创建授课任务 */
export function createTeachingTask(data: CreateTeachingTaskRequest) {
  return post<ApiResponse>('/teaching-tasks', data)
}

/** 更新授课任务 */
export function updateTeachingTask(id: string, data: UpdateTeachingTaskRequest) {
  return put<ApiResponse>(`/teaching-tasks/${id}`, data)
}

/** 删除授课任务 */
export function deleteTeachingTask(id: string) {
  return del<ApiResponse>(`/teaching-tasks/${id}`)
}

/** 启用/禁用 */
export function toggleTeachingTaskStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/teaching-tasks/${id}/status`, { isActive })
}

/** 添加教师 */
export function addTeachingTaskTeacher(taskId: string, data: AddTeachingTaskTeacherRequest) {
  return post<ApiResponse>(`/teaching-tasks/${taskId}/teachers`, data)
}

/** 移除教师 */
export function removeTeachingTaskTeacher(taskId: string, teacherId: string) {
  return del<ApiResponse>(`/teaching-tasks/${taskId}/teachers/${teacherId}`)
}
