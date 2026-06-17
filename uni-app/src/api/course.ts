/**
 * 课程管理 API
 * 对应后端 CoursesController
 *  GET    /api/v1/courses
 *  GET    /api/v1/courses/{id}
 *  POST   /api/v1/courses
 *  PUT    /api/v1/courses/{id}
 *  DELETE /api/v1/courses/{id}
 *  PATCH  /api/v1/courses/{id}/status
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type { Course, CourseQuery, CreateCourseRequest, UpdateCourseRequest } from '@/types/teaching'

/** 课程列表 */
export function getCourses(query?: CourseQuery) {
  return get<ApiResponse<Course[]>>('/courses', query as Record<string, string>)
}

/** 课程详情 */
export function getCourseById(id: string) {
  return get<ApiResponse<Course>>(`/courses/${id}`)
}

/** 创建课程 */
export function createCourse(data: CreateCourseRequest) {
  return post<ApiResponse>('/courses', data)
}

/** 更新课程 */
export function updateCourse(id: string, data: UpdateCourseRequest) {
  return put<ApiResponse>(`/courses/${id}`, data)
}

/** 删除课程 */
export function deleteCourse(id: string) {
  return del<ApiResponse>(`/courses/${id}`)
}

/** 启用/禁用课程 */
export function toggleCourseStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/courses/${id}/status`, { isActive })
}
