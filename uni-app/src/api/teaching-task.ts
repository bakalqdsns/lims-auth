/**
 * 授课任务 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { TeachingTask } from '@/types/teaching'

export function getTeachingTasks(query?: {
  page?: number
  pageSize?: number
  semesterId?: number
  teacherId?: number
  classId?: number
}) {
  return get<PagedResponse<TeachingTask>>('/teaching-tasks', query as Record<string, string | number>)
}

export function getTeachingTaskById(id: number) {
  return get<TeachingTask>(`/teaching-tasks/${id}`)
}

export function createTeachingTask(data: Partial<TeachingTask>) {
  return post<ApiResponse>('/teaching-tasks', data)
}

export function updateTeachingTask(id: number, data: Partial<TeachingTask>) {
  return put<ApiResponse>(`/teaching-tasks/${id}`, data)
}

export function deleteTeachingTask(id: number) {
  return del<ApiResponse>(`/teaching-tasks/${id}`)
}

export function updateTeachingTaskStatus(id: number, status: number) {
  return patch<ApiResponse>(`/teaching-tasks/${id}/status`, { status })
}

export function addTeachingTaskTeacher(taskId: number, teacherId: number) {
  return post<ApiResponse>(`/teaching-tasks/${taskId}/teachers`, { teacherId })
}

export function removeTeachingTaskTeacher(taskId: number, teacherId: number) {
  return del<ApiResponse>(`/teaching-tasks/${taskId}/teachers/${teacherId}`)
}
