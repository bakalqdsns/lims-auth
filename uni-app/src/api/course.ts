/**
 * 课程管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Course } from '@/types/teaching'

export function getCourses(query?: { page?: number; pageSize?: number; search?: string }) {
  return get<PagedResponse<Course>>('/courses', query as Record<string, string | number>)
}

export function getCourseById(id: number) {
  return get<Course>(`/courses/${id}`)
}

export function createCourse(data: Partial<Course>) {
  return post<ApiResponse>('/courses', data)
}

export function updateCourse(id: number, data: Partial<Course>) {
  return put<ApiResponse>(`/courses/${id}`, data)
}

export function deleteCourse(id: number) {
  return del<ApiResponse>(`/courses/${id}`)
}

export function updateCourseStatus(id: number, status: number) {
  return patch<ApiResponse>(`/courses/${id}/status`, { status })
}
