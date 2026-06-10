import axios from 'axios'

const API_BASE_URL = '/api/v1'

// ============================================================
// 耗材管理 API
// ============================================================

export const consumableApi = {
  // 分类
  getCategories: () =>
    axios.get(`${API_BASE_URL}/consumables/categories`),
  getCategoryById: (id: string) =>
    axios.get(`${API_BASE_URL}/consumables/categories/${id}`),
  createCategory: (data: CreateCategoryRequest) =>
    axios.post(`${API_BASE_URL}/consumables/categories`, data),
  updateCategory: (id: string, data: UpdateCategoryRequest) =>
    axios.put(`${API_BASE_URL}/consumables/categories/${id}`, data),
  deleteCategory: (id: string) =>
    axios.delete(`${API_BASE_URL}/consumables/categories/${id}`),

  // 耗材
  getConsumables: (params?: ConsumableQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables`, { params }),
  getConsumableById: (id: string) =>
    axios.get(`${API_BASE_URL}/consumables/${id}`),
  createConsumable: (data: CreateConsumableRequest) =>
    axios.post(`${API_BASE_URL}/consumables`, data),
  updateConsumable: (id: string, data: UpdateConsumableRequest) =>
    axios.put(`${API_BASE_URL}/consumables/${id}`, data),
  deleteConsumable: (id: string) =>
    axios.delete(`${API_BASE_URL}/consumables/${id}`),
  importConsumables: (formData: FormData) =>
    axios.post(`${API_BASE_URL}/consumables/import`, formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  downloadConsumableTemplate: () =>
    axios.get(`${API_BASE_URL}/consumables/import-template`, { responseType: 'blob' }),

  // 入库
  getInRecords: (params?: InRecordQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/in-records`, { params }),
  createInRecord: (data: CreateInRecordRequest) =>
    axios.post(`${API_BASE_URL}/consumables/in-records`, data),
  batchCreateInRecords: (data: BatchInRecordRequest) =>
    axios.post(`${API_BASE_URL}/consumables/in-records/batch`, data),
  approveInRecord: (id: string, data: ApprovalRequest) =>
    axios.post(`${API_BASE_URL}/consumables/in-records/${id}/approve`, data),
  deleteInRecord: (id: string) =>
    axios.delete(`${API_BASE_URL}/consumables/in-records/${id}`),

  // 出库
  getOutRecords: (params?: OutRecordQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/out-records`, { params }),
  getMyOutRecords: (params?: OutRecordQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/out-records/my`, { params }),
  createOutRecord: (data: CreateOutRecordRequest) =>
    axios.post(`${API_BASE_URL}/consumables/out-records`, data),
  batchCreateOutRecords: (data: BatchOutRecordRequest) =>
    axios.post(`${API_BASE_URL}/consumables/out-records/batch`, data),
  approveOutRecord: (id: string, data: ApprovalRequest) =>
    axios.post(`${API_BASE_URL}/consumables/out-records/${id}/approve`, data),
  deleteOutRecord: (id: string) =>
    axios.delete(`${API_BASE_URL}/consumables/out-records/${id}`),

  // 库存
  adjustStock: (data: AdjustStockRequest) =>
    axios.post(`${API_BASE_URL}/consumables/stock/adjust`, data),
  stockCheck: (data: StockCheckRequest) =>
    axios.post(`${API_BASE_URL}/consumables/stock/check`, data),
  getStockLogs: (params?: StockLogQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/stock/logs`, { params }),
  getStockAdjustments: (params?: StockAdjustmentQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/stock/adjustments`, { params }),

  // 统计
  getStatistics: (params?: StatisticsQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/statistics`, { params }),

  // 通知
  getNotifications: (params?: NotificationQueryParams) =>
    axios.get(`${API_BASE_URL}/consumables/notifications`, { params }),
  getUnreadCount: () =>
    axios.get(`${API_BASE_URL}/consumables/notifications/unread-count`),
  markRead: (id: string) =>
    axios.put(`${API_BASE_URL}/consumables/notifications/${id}/read`)
}

// ============================================================
// 类型定义
// ============================================================

export interface CreateCategoryRequest {
  name: string
  remark?: string
}

export interface UpdateCategoryRequest {
  name?: string
  remark?: string
  isActive?: boolean
}

export interface ConsumableQueryParams {
  keyword?: string
  categoryId?: string
  supplier?: string
  isLowStock?: boolean
  isActive?: boolean
  page?: number
  pageSize?: number
}

export interface CreateConsumableRequest {
  code: string
  name: string
  categoryId?: string
  specification?: string
  unit?: string
  currentStock?: number
  minStock?: number
  location?: string
  supplier?: string
  unitPrice?: number
  maxSingleRequest?: number
  monthlyLimit?: number
  description?: string
}

export interface UpdateConsumableRequest {
  name?: string
  categoryId?: string
  specification?: string
  unit?: string
  minStock?: number
  location?: string
  supplier?: string
  unitPrice?: number
  maxSingleRequest?: number
  monthlyLimit?: number
  description?: string
  isActive?: boolean
}

export interface ConsumableDto {
  id: string
  code: string
  name: string
  categoryId?: string
  categoryName?: string
  specification?: string
  unit: string
  currentStock: number
  availableStock: number
  lockedStock: number
  minStock: number
  location?: string
  supplier?: string
  unitPrice?: number
  maxSingleRequest: number
  monthlyLimit: number
  description?: string
  isActive: boolean
  createdAt: string
  isLowStock: boolean
}

export interface InRecordQueryParams {
  keyword?: string
  consumableId?: string
  handlerId?: string
  status?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

export interface CreateInRecordRequest {
  consumableId: string
  quantity: number
  unitPrice?: number
  supplier?: string
  inTime?: string
  remark?: string
}

export interface BatchInRecordRequest {
  items: CreateInRecordRequest[]
}

export interface ConsumableInRecordDto {
  id: string
  recordNo: string
  consumableId: string
  consumableName: string
  consumableCode?: string
  quantity: number
  unitPrice?: number
  totalAmount?: number
  supplier?: string
  inTime: string
  handlerId: string
  handlerName?: string
  remark?: string
  status: string
  approvedBy?: string
  approverName?: string
  approvedAt?: string
  approvalRemark?: string
  createdAt: string
}

export interface OutRecordQueryParams {
  keyword?: string
  consumableId?: string
  applicantId?: string
  status?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

export interface CreateOutRecordRequest {
  consumableId: string
  quantity: number
  usagePurpose?: string
  usageLab?: string
  remark?: string
}

export interface BatchOutRecordRequest {
  items: CreateOutRecordRequest[]
}

export interface ConsumableOutRecordDto {
  id: string
  recordNo: string
  consumableId: string
  consumableName: string
  consumableCode?: string
  categoryName?: string
  quantity: number
  usagePurpose?: string
  usageLab?: string
  outTime: string
  applicantId: string
  applicantName?: string
  remark?: string
  status: string
  approvedBy?: string
  approverName?: string
  approvedAt?: string
  approvalRemark?: string
  createdAt: string
}

export interface AdjustStockRequest {
  consumableId: string
  adjustmentType: string
  adjustmentQuantity: number
  reason?: string
}

export interface StockCheckRequest {
  items: StockCheckItem[]
}

export interface StockCheckItem {
  consumableId: string
  actualQuantity: number
}

export interface StockLogQueryParams {
  consumableId?: string
  changeType?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

export interface ConsumableStockLogDto {
  id: string
  consumableId: string
  consumableName: string
  changeType: string
  changeQuantity: number
  beforeStock: number
  afterStock: number
  referenceId?: string
  referenceNo?: string
  operatorId: string
  operatorName?: string
  remark?: string
  createdAt: string
}

export interface StockAdjustmentQueryParams {
  consumableId?: string
  adjustmentType?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

export interface ConsumableStockAdjustmentDto {
  id: string
  consumableId: string
  consumableName: string
  consumableCode?: string
  adjustmentType: string
  beforeQuantity: number
  adjustmentQuantity: number
  afterQuantity: number
  reason?: string
  operatorId: string
  operatorName?: string
  createdAt: string
}

export interface StatisticsQueryParams {
  categoryId?: string
  startDate?: string
  endDate?: string
}

export interface ConsumableStatisticsDto {
  totalTypes: number
  lowStockTypes: number
  outOfStockTypes: number
  activeTypes: number
  totalStockValue: number
  totalInRecords: number
  totalOutRecords: number
  totalInAmount: number
  totalOutAmount: number
  byCategory: Record<string, number>
  lowStockItems: LowStockItem[]
  monthlyConsumptions: MonthlyConsumption[]
}

export interface LowStockItem {
  consumableId: string
  consumableName: string
  categoryName?: string
  currentStock: number
  minStock: number
  unit: string
}

export interface MonthlyConsumption {
  month: string
  quantity: number
  recordCount: number
}

export interface NotificationQueryParams {
  isRead?: boolean
  page?: number
  pageSize?: number
}

export interface ConsumableNotificationDto {
  id: string
  userId: string
  type: string
  title: string
  content?: string
  relatedId?: string
  isRead: boolean
  readAt?: string
  createdAt: string
}

export interface ApprovalRequest {
  approved: boolean
  comment?: string
}

export interface CategoryDto {
  id: string
  name: string
  remark?: string
  isActive: boolean
  createdAt: string
}

// 常量
export const STOCK_STATUSES = ['Pending', 'Approved', 'Rejected']
export const STOCK_STATUS_MAP: Record<string, string> = {
  'Pending': '待审批',
  'Approved': '已通过',
  'Rejected': '已驳回'
}
export const STOCK_STATUS_TYPE: Record<string, string> = {
  'Pending': 'warning',
  'Approved': 'success',
  'Rejected': 'danger'
}

export const ADJUSTMENT_TYPES = ['盘点', '损耗', '报废', '误差修正', '其他']
export const STOCK_CHANGE_TYPES: Record<string, string> = {
  'In': '入库',
  'Out': '出库',
  'Lock': '锁定',
  'Unlock': '解锁',
  'Check': '盘点',
  'Adjust_盘点': '盘点调整',
  'Adjust_损耗': '损耗调整',
  'Adjust_报废': '报废调整',
  'Adjust_误差修正': '误差修正',
  'Adjust_其他': '其他调整'
}
