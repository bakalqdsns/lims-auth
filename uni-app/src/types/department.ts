/**
 * 部门相关类型定义
 * 与后端 DepartmentDto / DepartmentBriefDto 对齐
 */

export interface DepartmentDto {
  id: string
  name: string
  code: string
  parentId?: string | null
  parentName?: string
  description?: string
  sortOrder: number
  leaderId?: string
  leaderName?: string
  phone?: string
  email?: string
  status: number
  userCount: number
  children?: DepartmentDto[]
  createdAt: string
}

export interface DepartmentBrief {
  id: string
  name: string
  code: string
  parentId?: string | null
}

export interface CreateDepartmentRequest {
  name: string
  code: string
  parentId?: string | null
  description?: string
  sortOrder?: number
  leaderId?: string
  phone?: string
  email?: string
  status?: number
}

export interface UpdateDepartmentRequest {
  name?: string
  code?: string
  parentId?: string | null
  description?: string
  sortOrder?: number
  leaderId?: string
  phone?: string
  email?: string
  status?: number
}
