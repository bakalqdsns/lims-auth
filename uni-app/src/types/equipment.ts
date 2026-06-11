/**
 * 设备相关类型定义
 * 与 Flutter lib/models/equipment.dart 对齐
 */

export interface Equipment {
  id: number
  name: string
  code: string
  model?: string
  manufacturer?: string
  serialNumber?: string
  category?: string
  labId?: string
  labName?: string
  purchaseDate?: string
  warrantyExpiry?: string
  status: number
  totalQuantity: number
  availableQuantity: number
  description?: string
  image?: string
  createdAt: string
}

export interface EquipmentQuery {
  page?: number
  pageSize?: number
  keyword?: string
  category?: string
  labId?: string
  status?: number
}

export interface CreateEquipmentRequest {
  name: string
  code: string
  model?: string
  manufacturer?: string
  serialNumber?: string
  category?: string
  labId?: string
  totalQuantity: number
  description?: string
}

export interface EquipmentStatistics {
  totalCount: number
  availableCount: number
  borrowedCount: number
  maintenanceCount: number
  categories: { name: string; count: number }[]
}
