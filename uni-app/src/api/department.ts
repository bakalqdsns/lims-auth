/**
 * 部门管理 API
 * 对应后端 DepartmentsController
 *  GET    /api/v1/departments
 *  GET    /api/v1/departments/all
 *  GET    /api/v1/departments/{id}
 *  POST   /api/v1/departments
 *  PUT    /api/v1/departments/{id}
 *  DELETE /api/v1/departments/{id}
 */
import { get, post, put, del } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  DepartmentDto,
  DepartmentBrief,
  CreateDepartmentRequest,
  UpdateDepartmentRequest,
} from '@/types/department'

/** 部门树 */
export function getDepartmentTree() {
  return get<ApiResponse<DepartmentDto[]>>('/departments')
}

/** 部门扁平列表 (下拉) */
export function getAllDepartments() {
  return get<ApiResponse<DepartmentBrief[]>>('/departments/all')
}

/** 部门详情 */
export function getDepartmentById(id: string) {
  return get<ApiResponse<DepartmentDto>>(`/departments/${id}`)
}

/** 创建部门 */
export function createDepartment(data: CreateDepartmentRequest) {
  return post<ApiResponse>('/departments', data)
}

/** 更新部门 */
export function updateDepartment(id: string, data: UpdateDepartmentRequest) {
  return put<ApiResponse>(`/departments/${id}`, data)
}

/** 删除部门 */
export function deleteDepartment(id: string) {
  return del<ApiResponse>(`/departments/${id}`)
}
