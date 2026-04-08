import axios from 'axios'

const API_BASE_URL = '/api/v1'

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
  getList: (params?: { keyword?: string; labId?: string; category?: string; status?: string }) =>
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
    axios.patch(`${API_BASE_URL}/equipments/${id}/equipment-status`, { status })
}

// 类型定义
export interface CreateLabRequest {
  code: string
  name: string
  departmentId?: string
  location?: string
  capacity?: number
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
  status?: string
  purchaseDate?: string
  warrantyMonths?: number
  price?: number
  location?: string
  imageUrl?: string
  instructions?: string
  requiresBooking?: boolean
  maxBookingHours?: number
  description?: string
}

export interface UpdateEquipmentRequest extends Partial<CreateEquipmentRequest> {
  isActive?: boolean
}

// DTO 类型
export interface LabDto {
  id: string
  code: string
  name: string
  departmentId?: string
  departmentName?: string
  location?: string
  capacity: number
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
  status: string
  purchaseDate?: string
  warrantyMonths?: number
  price?: number
  location?: string
  imageUrl?: string
  instructions?: string
  requiresBooking: boolean
  maxBookingHours?: number
  description?: string
  isActive: boolean
  createdAt: string
}

// 常量
export const LAB_TYPES = ['普通实验室', '计算机实验室', '化学实验室', '物理实验室', '生物实验室', '专业实验室']
export const SAFETY_LEVELS = ['一般', '中等', '高危']
export const EQUIPMENT_CATEGORIES = ['通用设备', '计算机设备', '实验仪器', '测量设备', '办公设备', '安全设备', '其他']
export const EQUIPMENT_STATUSES = ['正常', '维修中', '报废', '借用中', '闲置']
