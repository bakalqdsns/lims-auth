/**
 * 耗材相关类型
 * 与后端 Consumable / InRecord / OutRecord / StockLog 等 DTO 对齐
 */

export interface ConsumableCategory {
  id: string
  name: string
  code: string
  parentId?: string | null
  description?: string
  sortOrder?: number
}

export interface CreateConsumableCategoryRequest {
  name: string
  code: string
  parentId?: string | null
  description?: string
  sortOrder?: number
}

export interface UpdateConsumableCategoryRequest {
  name?: string
  code?: string
  parentId?: string | null
  description?: string
  sortOrder?: number
}

export interface Consumable {
  id: string
  name: string
  code: string
  categoryId?: string
  categoryName?: string
  specification?: string
  unit?: string
  brand?: string
  manufacturer?: string
  safetyStock?: number
  currentStock: number
  availableStock: number
  unitPrice?: number
  location?: string
  description?: string
  status: number
  createdAt: string
}

export interface ConsumableQuery {
  page?: number
  pageSize?: number
  keyword?: string
  categoryId?: string
  labId?: string
  status?: number
  lowStock?: boolean
}

export interface CreateConsumableRequest {
  name: string
  code: string
  categoryId?: string
  specification?: string
  unit?: string
  brand?: string
  manufacturer?: string
  safetyStock?: number
  unitPrice?: number
  location?: string
  description?: string
}

export interface UpdateConsumableRequest {
  name?: string
  code?: string
  categoryId?: string
  specification?: string
  unit?: string
  brand?: string
  manufacturer?: string
  safetyStock?: number
  unitPrice?: number
  location?: string
  description?: string
}

export interface ConsumableInRecord {
  id: string
  recordNo: string
  consumableId: string
  consumableName: string
  quantity: number
  unitPrice?: number
  totalAmount?: number
  supplier?: string
  batchNo?: string
  productionDate?: string
  expiryDate?: string
  operatorId: string
  operatorName: string
  approverId?: string
  approverName?: string
  approvedAt?: string
  remark?: string
  status: number
  createdAt: string
}

export interface ConsumableInRecordQuery {
  page?: number
  pageSize?: number
  keyword?: string
  consumableId?: string
  supplier?: string
  status?: number
  startDate?: string
  endDate?: string
}

export interface CreateConsumableInRecordRequest {
  consumableId: string
  quantity: number
  unitPrice?: number
  supplier?: string
  batchNo?: string
  productionDate?: string
  expiryDate?: string
  remark?: string
}

export interface BatchCreateConsumableInRecordRequest {
  records: CreateConsumableInRecordRequest[]
}

export interface ConsumableOutRecord {
  id: string
  recordNo: string
  consumableId: string
  consumableName: string
  quantity: number
  purpose?: string
  applicantId: string
  applicantName: string
  approverId?: string
  approverName?: string
  approvedAt?: string
  labId?: string
  labName?: string
  experimentId?: string
  remark?: string
  status: number
  createdAt: string
}

export interface ConsumableOutRecordQuery {
  page?: number
  pageSize?: number
  keyword?: string
  consumableId?: string
  labId?: string
  status?: number
  startDate?: string
  endDate?: string
}

export interface CreateConsumableOutRecordRequest {
  consumableId: string
  quantity: number
  purpose?: string
  labId?: string
  experimentId?: string
  remark?: string
}

export interface BatchCreateConsumableOutRecordRequest {
  records: CreateConsumableOutRecordRequest[]
}

export interface ConsumableStockLog {
  id: string
  consumableId: string
  consumableName: string
  changeType: 'In' | 'Out' | 'Adjust' | 'Check'
  changeQuantity: number
  beforeStock: number
  afterStock: number
  refType?: string
  refId?: string
  operatorId: string
  operatorName: string
  remark?: string
  createdAt: string
}

export interface ConsumableStockLogQuery {
  page?: number
  pageSize?: number
  consumableId?: string
  changeType?: string
  startDate?: string
  endDate?: string
}

export interface ConsumableStockAdjustment {
  id: string
  consumableId: string
  consumableName: string
  beforeStock: number
  afterStock: number
  difference: number
  reason: string
  operatorId: string
  operatorName: string
  createdAt: string
}

export interface ConsumableStockAdjustmentQuery {
  page?: number
  pageSize?: number
  consumableId?: string
  startDate?: string
  endDate?: string
}

export interface CreateConsumableStockAdjustmentRequest {
  consumableId: string
  afterStock: number
  reason: string
}

export interface StockCheckRequest {
  items: { consumableId: string; actualStock: number; remark?: string }[]
}

export interface ConsumableStatistics {
  totalItems: number
  totalStockValue: number
  monthlyIn: number
  monthlyOut: number
  lowStockItems: number
  expiringItems: number
  byCategory?: { categoryName: string; count: number; stockValue: number }[]
  topUsed?: { consumableName: string; totalOut: number }[]
}

export interface ConsumableStatisticsQuery {
  startDate?: string
  endDate?: string
  categoryId?: string
  labId?: string
}

export interface ConsumableNotification {
  id: string
  type: 'LowStock' | 'Expiring' | 'Approval' | 'System'
  title: string
  content: string
  refId?: string
  refType?: string
  isRead: boolean
  readAt?: string
  createdAt: string
}

export interface ConsumableNotificationQuery {
  page?: number
  pageSize?: number
  type?: string
  isRead?: boolean
}

export interface ImportConsumableResult {
  success: number
  failed: number
  total: number
  errors: string[]
}
