/**
 * 班级管理 API
 * 对应后端 ClassesController
 *  GET    /api/v1/classes
 *  GET    /api/v1/classes/{id}
 *  POST   /api/v1/classes
 *  PUT    /api/v1/classes/{id}
 *  DELETE /api/v1/classes/{id}
 *  PATCH  /api/v1/classes/{id}/status
 *  GET    /api/v1/classes/{id}/students
 *  POST   /api/v1/classes/{id}/students
 *  DELETE /api/v1/classes/{id}/students/{studentId}
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  ClassInfo,
  ClassQuery,
  CreateClassRequest,
  UpdateClassRequest,
  ClassStudent,
} from '@/types/teaching'

/** 班级列表 */
export function getClasses(query?: ClassQuery) {
  return get<ApiResponse<ClassInfo[]>>('/classes', query as Record<string, string>)
}

/** 班级详情 */
export function getClassById(id: string) {
  return get<ApiResponse<ClassInfo>>(`/classes/${id}`)
}

/** 创建班级 */
export function createClass(data: CreateClassRequest) {
  return post<ApiResponse>('/classes', data)
}

/** 更新班级 */
export function updateClass(id: string, data: UpdateClassRequest) {
  return put<ApiResponse>(`/classes/${id}`, data)
}

/** 删除班级 */
export function deleteClass(id: string) {
  return del<ApiResponse>(`/classes/${id}`)
}

/** 启用/禁用班级 */
export function toggleClassStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/classes/${id}/status`, { isActive })
}

/** 班级学生 */
export function getClassStudents(classId: string) {
  return get<ApiResponse<ClassStudent[]>>(`/classes/${classId}/students`)
}

/** 添加学生 */
export function addClassStudents(classId: string, studentIds: string[]) {
  return post<ApiResponse>(`/classes/${classId}/students`, { studentIds })
}

/** 移除学生 */
export function removeClassStudent(classId: string, studentId: string) {
  return del<ApiResponse>(`/classes/${classId}/students/${studentId}`)
}
