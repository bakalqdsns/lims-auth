/**
 * 耗材管理 API
 * 对应后端 ConsumablesController
 *
 * 分类:
 *  GET    /api/v1/consumables/categories
 *  GET    /api/v1/consumables/categories/{id}
 *  POST   /api/v1/consumables/categories
 *  PUT    /api/v1/consumables/categories/{id}
 *  DELETE /api/v1/consumables/categories/{id}
 *
 * 基础:
 *  GET    /api/v1/consumables
 *  GET    /api/v1/consumables/{id}
 *  POST   /api/v1/consumables
 *  POST   /api/v1/consumables/import
 *  GET    /api/v1/consumables/import-template
 *  PUT    /api/v1/consumables/{id}
 *  DELETE /api/v1/consumables/{id}
 *
 * 入库:
 *  GET    /api/v1/consumables/in-records
 *  POST   /api/v1/consumables/in-records
 *  POST   /api/v1/consumables/in-records/batch
 *  POST   /api/v1/consumables/in-records/{id}/approve
 *  DELETE /api/v1/consumables/in-records/{id}
 *
 * 出库:
 *  GET    /api/v1/consumables/out-records
 *  GET    /api/v1/consumables/out-records/my
 *  POST   /api/v1/consumables/out-records
 *  POST   /api/v1/consumables/out-records/batch
 *  POST   /api/v1/consumables/out-records/{id}/approve
 *  DELETE /api/v1/consumables/out-records/{id}
 *
 * 库存:
 *  POST   /api/v1/consumables/stock/adjust
 *  POST   /api/v1/consumables/stock/check
 *  GET    /api/v1/consumables/stock/logs
 *  GET    /api/v1/consumables/stock/adjustments
 *
 * 统计/通知:
 *  GET    /api/v1/consumables/statistics
 *  GET    /api/v1/consumables/notifications
 *  GET    /api/v1/consumables/notifications/unread-count
 *  PUT    /api/v1/consumables/notifications/{id}/read
 */
import { get, post, put, del, uploadFile } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  Consumable,
  ConsumableCategory,
  ConsumableQuery,
  CreateConsumableRequest,
  UpdateConsumableRequest,
  CreateConsumableCategoryRequest,
  UpdateConsumableCategoryRequest,
  ConsumableInRecord,
  ConsumableInRecordQuery,
  CreateConsumableInRecordRequest,
  BatchCreateConsumableInRecordRequest,
  ConsumableOutRecord,
  ConsumableOutRecordQuery,
  CreateConsumableOutRecordRequest,
  BatchCreateConsumableOutRecordRequest,
  ConsumableStockLog,
  ConsumableStockLogQuery,
  ConsumableStockAdjustment,
  ConsumableStockAdjustmentQuery,
  CreateConsumableStockAdjustmentRequest,
  StockCheckRequest,
  ConsumableStatistics,
  ConsumableStatisticsQuery,
  ConsumableNotification,
  ConsumableNotificationQuery,
  ApprovalRequest,
  ImportConsumableResult,
} from '@/types/consumable'

/* =============== 分类 =============== */

export function getConsumableCategories() {
  return get<ApiResponse<ConsumableCategory[]>>('/consumables/categories')
}

export function getConsumableCategoryById(id: string) {
  return get<ApiResponse<ConsumableCategory>>(`/consumables/categories/${id}`)
}

export function createConsumableCategory(data: CreateConsumableCategoryRequest) {
  return post<ApiResponse>('/consumables/categories', data)
}

export function updateConsumableCategory(id: string, data: UpdateConsumableCategoryRequest) {
  return put<ApiResponse>(`/consumables/categories/${id}`, data)
}

export function deleteConsumableCategory(id: string) {
  return del<ApiResponse>(`/consumables/categories/${id}`)
}

/* =============== 耗材基础 =============== */

export function getConsumables(query?: ConsumableQuery) {
  return get<ApiResponse<{ items: Consumable[]; total: number }>>(
    '/consumables',
    query as Record<string, string | number>
  )
}

export function getConsumableById(id: string) {
  return get<ApiResponse<Consumable>>(`/consumables/${id}`)
}

export function createConsumable(data: CreateConsumableRequest) {
  return post<ApiResponse>('/consumables', data)
}

export function updateConsumable(id: string, data: UpdateConsumableRequest) {
  return put<ApiResponse>(`/consumables/${id}`, data)
}

export function deleteConsumable(id: string) {
  return del<ApiResponse>(`/consumables/${id}`)
}

export function importConsumables(filePath: string, fileName: string) {
  return uploadFile('/consumables/import', filePath, 'file')
}

export function getConsumableImportTemplate() {
  return get<ArrayBuffer>('/consumables/import-template', {}, { showError: false })
}

/* =============== 入库 =============== */

export function getConsumableInRecords(query?: ConsumableInRecordQuery) {
  return get<ApiResponse<{ items: ConsumableInRecord[]; total: number }>>(
    '/consumables/in-records',
    query as Record<string, string | number>
  )
}

export function createConsumableInRecord(data: CreateConsumableInRecordRequest) {
  return post<ApiResponse>('/consumables/in-records', data)
}

export function batchCreateConsumableInRecords(data: BatchCreateConsumableInRecordRequest) {
  return post<ApiResponse>('/consumables/in-records/batch', data)
}

export function approveConsumableInRecord(id: string, request: ApprovalRequest) {
  return post<ApiResponse>(`/consumables/in-records/${id}/approve`, request)
}

export function deleteConsumableInRecord(id: string) {
  return del<ApiResponse>(`/consumables/in-records/${id}`)
}

/* =============== 出库 =============== */

export function getConsumableOutRecords(query?: ConsumableOutRecordQuery) {
  return get<ApiResponse<{ items: ConsumableOutRecord[]; total: number }>>(
    '/consumables/out-records',
    query as Record<string, string | number>
  )
}

export function getMyConsumableOutRecords(query?: ConsumableOutRecordQuery) {
  return get<ApiResponse<{ items: ConsumableOutRecord[]; total: number }>>(
    '/consumables/out-records/my',
    query as Record<string, string | number>
  )
}

export function createConsumableOutRecord(data: CreateConsumableOutRecordRequest) {
  return post<ApiResponse>('/consumables/out-records', data)
}

export function batchCreateConsumableOutRecords(data: BatchCreateConsumableOutRecordRequest) {
  return post<ApiResponse>('/consumables/out-records/batch', data)
}

export function approveConsumableOutRecord(id: string, request: ApprovalRequest) {
  return post<ApiResponse>(`/consumables/out-records/${id}/approve`, request)
}

export function deleteConsumableOutRecord(id: string) {
  return del<ApiResponse>(`/consumables/out-records/${id}`)
}

/* =============== 库存 =============== */

export function adjustConsumableStock(data: CreateConsumableStockAdjustmentRequest) {
  return post<ApiResponse>('/consumables/stock/adjust', data)
}

export function stockCheck(data: StockCheckRequest) {
  return post<ApiResponse>('/consumables/stock/check', data)
}

export function getConsumableStockLogs(query?: ConsumableStockLogQuery) {
  return get<ApiResponse<{ items: ConsumableStockLog[]; total: number }>>(
    '/consumables/stock/logs',
    query as Record<string, string | number>
  )
}

export function getConsumableStockAdjustments(query?: ConsumableStockAdjustmentQuery) {
  return get<ApiResponse<{ items: ConsumableStockAdjustment[]; total: number }>>(
    '/consumables/stock/adjustments',
    query as Record<string, string | number>
  )
}

/* =============== 统计 / 通知 =============== */

export function getConsumableStatistics(query?: ConsumableStatisticsQuery) {
  return get<ApiResponse<ConsumableStatistics>>(
    '/consumables/statistics',
    query as Record<string, string>
  )
}

export function getConsumableNotifications(query?: ConsumableNotificationQuery) {
  return get<ApiResponse<ConsumableNotification[]>>(
    '/consumables/notifications',
    query as Record<string, string | number>
  )
}

export function getConsumableUnreadCount() {
  return get<ApiResponse<number>>('/consumables/notifications/unread-count')
}

export function markConsumableNotificationRead(id: string) {
  return put<ApiResponse>(`/consumables/notifications/${id}/read`, {})
}

export type { ImportConsumableResult }
