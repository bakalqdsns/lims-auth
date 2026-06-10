/**
 * 班级管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { ClassInfo } from '@/types/teaching'

export function getClasses(query?: { page?: number; pageSize?: number; search?: string; majorId?: number }) {
  return get<PagedResponse<ClassInfo>>('/classes', query as Record<string, string | number>)
}

export function getClassById(id: number) {
  return get<ClassInfo>(`/classes/${id}`)
}

export function createClass(data: Partial<ClassInfo>) {
  return post<ApiResponse>('/classes', data)
}

export function updateClass(id: number, data: Partial<ClassInfo>) {
  return put<ApiResponse>(`/classes/${id}`, data)
}

export function deleteClass(id: number) {
  return del<ApiResponse>(`/classes/${id}`)
}

export function updateClassStatus(id: number, status: number) {
  return patch<ApiResponse>(`/classes/${id}/status`, { status })
}

export function getClassStudents(classId: number) {
  return get<{ id: number; username: string; fullName: string; studentId?: string }[]>(
    `/classes/${classId}/students`
  )
}

export function addClassStudents(classId: number, studentIds: number[]) {
  return post<ApiResponse>(`/classes/${classId}/students`, { studentIds })
}

export function removeClassStudent(classId: number, studentId: number) {
  return del<ApiResponse>(`/classes/${classId}/students/${studentId}`)
}
