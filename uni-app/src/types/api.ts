/**
 * API 统一响应类型 (与后端 ApiResponse<T> 对齐)
 */
export interface ApiResponse<T = unknown> {
  code: number
  message: string
  data: T | null
}

/**
 * 分页响应 (与后端 PagedResponse<T> 对齐)
 */
export interface PagedResponse<T = unknown> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

/**
 * 分页请求参数
 */
export interface PagedQuery {
  page?: number
  pageSize?: number
  search?: string
  keyword?: string
}

/**
 * API 错误类型
 */
export interface ApiError {
  code: number
  message: string
}

/**
 * 文件上传响应
 */
export interface UploadResponse {
  url: string
  fileName: string
  fileSize: number
}

/**
 * 通用 ID Name 项 (下拉选项)
 */
export interface IdNameItem {
  id: number | string
  name: string
}

/**
 * 通用树形节点
 */
export interface TreeNode<T = unknown> {
  id: string
  name: string
  parentId: string | null
  children?: TreeNode<T>[]
  [key: string]: unknown
}

/** 通用开关请求 */
export interface ToggleStatusRequest {
  isActive: boolean
}

/** 通用审批请求 */
export interface ApprovalRequest {
  approved: boolean
  comment?: string
}

/** 通用导入结果 */
export interface ImportResult {
  success: number
  failed: number
  total: number
  errors: string[]
}
