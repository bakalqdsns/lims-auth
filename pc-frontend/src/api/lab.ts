import axios from 'axios'
import { API_BASE_URL } from '../config/api'

// 实验室管理 API
export const labApi = {
  getList: (params?: { keyword?: string; departmentId?: string; labType?: string }) =>
    axios.get(`${API_BASE_URL}/labs`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/labs/${id}`),
  create: (data: CreateLabRequest) =>
    axios.post(`${API_BASE_URL}/labs`, data),
  update: (id: string, data: UpdateLabRequest) =>
    axios.put(`${API_BASE_URL}/labs/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/labs/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/labs/${id}/status`, { isActive })
}

// 设备管理 API
export const equipmentApi = {
  getList: (params?: { keyword?: string; labId?: string; category?: string; status?: string; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/equipments`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/equipments/${id}`),
  create: (data: CreateEquipmentRequest) =>
    axios.post(`${API_BASE_URL}/equipments`, data),
  update: (id: string, data: UpdateEquipmentRequest) =>
    axios.put(`${API_BASE_URL}/equipments/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/equipments/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/equipments/${id}/status`, { isActive }),
  updateEquipmentStatus: (id: string, status: string) =>
    axios.patch(`${API_BASE_URL}/equipments/${id}/equipment-status`, { status }),
  getStatistics: () =>
    axios.get(`${API_BASE_URL}/equipments/statistics`),
  exportExcel: (params?: { keyword?: string; category?: string; status?: string }) =>
    axios.get(`${API_BASE_URL}/equipments/export`, { params, responseType: 'blob' }),
  importExcel: (file: File) => {
    const formData = new FormData()
    formData.append('file', file)
    return axios.post(`${API_BASE_URL}/equipments/import`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  },
  downloadTemplate: () =>
    axios.get(`${API_BASE_URL}/equipments/import-template`, { responseType: 'blob' })
}

// 设备借还 API
export const borrowApi = {
  createRequest: (data: CreateBorrowRequest) =>
    axios.post(`${API_BASE_URL}/borrow-records`, data),
  getMyRecords: (status?: string) =>
    axios.get(`${API_BASE_URL}/borrow-records/my`, { params: { status } }),
  getPendingApprovals: () =>
    axios.get(`${API_BASE_URL}/borrow-records/pending`),
  getAllRecords: (params?: { status?: string; keyword?: string }) =>
    axios.get(`${API_BASE_URL}/borrow-records`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/borrow-records/${id}`),
  getByRecordNo: (recordNo: string) =>
    axios.get(`${API_BASE_URL}/borrow-records/no/${recordNo}`),
  supervisorApprove: (id: string, data: ApprovalDto) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/supervisor-approve`, data),
  adminApprove: (id: string, data: ApprovalDto) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/admin-approve`, data),
  confirmBorrow: (id: string) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/confirm-borrow`),
  submitReturn: (id: string) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/submit-return`),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/borrow-records/${id}`),
  confirmReturn: (id: string, data: ReturnConfirmDto) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/confirm-return`, data),
  approveReturn: (id: string, data: ReturnApprovalDto) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/approve-return`, data),
  approveRenew: (id: string, data: ApprovalDto) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/approve-renew`, data),
  renew: (id: string, data: RenewDto) =>
    axios.post(`${API_BASE_URL}/borrow-records/${id}/renew`, data),
  getOverdueRecords: () =>
    axios.get(`${API_BASE_URL}/borrow-records/overdue`),
  getExpiringRecords: (daysBefore?: number) =>
    axios.get(`${API_BASE_URL}/borrow-records/expiring`, { params: { daysBefore } }),
  getBorrowFlow: (params?: { startDate?: string; endDate?: string }) =>
    axios.get(`${API_BASE_URL}/borrow-records/flow`, { params })
}

// 类型定义
export interface CreateLabRequest {
  code: string
  name: string
  departmentId?: string
  buildingId?: string
  floor?: number
  roomNumber?: string
  location?: string
  capacity?: number
  seatCount?: number
  floorNo?: number
  area?: number
  roomType?: string
  photo?: string
  isAvailable?: boolean
  experimentLocationCode?: string
  labType?: string
  safetyLevel?: string
  managerId?: string
  description?: string
}

export interface UpdateLabRequest extends Partial<CreateLabRequest> {
  isActive?: boolean
}

export interface CreateEquipmentRequest {
  code: string
  name: string
  model?: string
  manufacturer?: string
  serialNumber?: string
  labId?: string
  category?: string
  unit?: string
  status?: string
  purchaseDate?: string
  warrantyMonths?: number
  price?: number
  location?: string
  imageUrl?: string
  instructions?: string
  requiresBooking?: boolean
  maxBookingHours?: number
  totalQuantity?: number
  availableQuantity?: number
  brand?: string
  supplier?: string
  description?: string
}

export interface UpdateEquipmentRequest extends Partial<CreateEquipmentRequest> {
  isActive?: boolean
}

export interface LabDto {
  id: string
  code: string
  name: string
  departmentId?: string
  departmentName?: string
  buildingId?: string
  building?: { id: string; name: string }
  floor?: number
  floorNo?: number
  roomNumber?: string
  location?: string
  capacity: number
  seatCount: number
  area?: number
  roomType?: string
  photo?: string
  isAvailable: boolean
  experimentLocationCode?: string
  labType: string
  safetyLevel: string
  managerId?: string
  managerName?: string
  description?: string
  isActive: boolean
  createdAt: string
  equipmentCount: number
}

export interface EquipmentDto {
  id: string
  code: string
  name: string
  model?: string
  manufacturer?: string
  serialNumber?: string
  labId?: string
  labName?: string
  category: string
  unit?: string
  status: string
  purchaseDate?: string
  warrantyMonths?: number
  price?: number
  location?: string
  imageUrl?: string
  instructions?: string
  requiresBooking: boolean
  maxBookingHours?: number
  totalQuantity?: number
  availableQuantity?: number
  brand?: string
  supplier?: string
  description?: string
  isActive: boolean
  createdAt: string
}

export interface EquipmentStatisticsDto {
  total: number
  activeCount: number
  inactiveCount: number
  normalCount: number
  maintenanceCount: number
  borrowedCount: number
  scrappedCount: number
  requiresBookingCount: number
  totalValue: number
  byCategory: Record<string, number>
  byStatus: Record<string, number>
  byLab: Record<string, number>
}

// 借还相关类型
export interface CreateBorrowRequest {
  equipmentId: string
  borrowDate: string
  expectedReturnDate: string
  purpose: string
  phone?: string
  usageLocation?: string
  remarks?: string
}

export interface BorrowRecordDto {
  id: string
  recordNo: string
  equipmentId: string
  equipmentCode: string
  equipmentName: string
  equipmentModel?: string
  labName?: string
  applicantId: string
  applicantName: string
  approverId?: string
  approverName?: string
  status: string
  borrowDate: string
  expectedReturnDate: string
  actualBorrowDate?: string
  actualReturnDate?: string
  purpose: string
  phone?: string
  usageLocation?: string
  remarks?: string
  supervisorApprovalStatus?: string
  supervisorApprovalRemark?: string
  supervisorApprovalDate?: string
  adminApprovalStatus?: string
  adminApprovalRemark?: string
  adminApprovalDate?: string
  returnCondition?: string
  returnRemarks?: string
  returnCheckerName?: string
  returnCheckDate?: string
  isRenewed: boolean
  renewedReturnDate?: string
  daysOverdue: number
  createdAt: string
  recipientId?: string
  recipientName?: string
}

export interface OverdueRecordDto {
  recordId: string
  recordNo: string
  equipmentName: string
  equipmentCode: string
  applicantName: string
  borrowDate: string
  expectedReturnDate: string
  daysOverdue: number
}

export interface ExpiringRecordDto {
  recordId: string
  recordNo: string
  equipmentName: string
  equipmentCode: string
  applicantName: string
  borrowDate: string
  expectedReturnDate: string
  daysUntilDue: number
}

export interface BorrowFlowDto {
  recordNo: string
  equipmentName: string
  equipmentCode: string
  applicantName: string
  status: string
  borrowDate: string
  expectedReturnDate: string
  actualReturnDate?: string
  purpose: string
  returnCondition?: string
}

export interface ApprovalDto {
  approved: boolean
  remark?: string
}

export interface ReturnConfirmDto {
  condition: string
  remarks?: string
}

export interface ReturnApprovalDto {
  approved: boolean
  condition?: string
  remarks?: string
}

export interface RenewDto {
  newReturnDate: string
}

// 常量
export const LAB_TYPES = ['普通实验室', '计算机实验室', '化学实验室', '物理实验室', '生物实验室', '专业实验室']
export const SAFETY_LEVELS = ['一般', '中等', '高危']
export const EQUIPMENT_CATEGORIES = ['通用设备', '计算机设备', '实验仪器', '测量设备', '办公设备', '安全设备', '其他']
export const EQUIPMENT_STATUSES = ['在库-可用', '在库-待维修', '在库-已预约', '借出', '送修', '报废', '丢失']
export const BORROW_STATUSES = ['待老师审批', '待管理员审批', '待领取', '已借出', '待归还', '已归还', '已拒绝', '已逾期', '续借审批中', '管理员审批中']
export const RETURN_CONDITIONS = ['完好', '损坏', '缺件']
