/**
 * 设备相关类型定义
 * 与后端 EquipmentDto / EquipmentStatisticsDto 对齐
 */

export type EquipmentStatus = 'Available' | 'InUse' | 'Maintenance' | 'Scrapped' | 'Reserved'

export interface Equipment {
  id: string
  name: string
  code: string
  model?: string
  specification?: string
  manufacturer?: string
  serialNumber?: string
  assetNumber?: string
  category?: string
  categoryName?: string
  labId?: string
  labName?: string
  unitPrice?: number
  purchaseDate?: string
  warrantyExpiry?: string
  location?: string
  status: EquipmentStatus | number
  totalQuantity: number
  availableQuantity: number
  description?: string
  image?: string
  images?: string[]
  custodianId?: string
  custodianName?: string
  createdAt: string
  updatedAt?: string
}

export interface EquipmentQuery {
  page?: number
  pageSize?: number
  keyword?: string
  labId?: string
  category?: string
  status?: string | number
}

export interface CreateEquipmentRequest {
  name: string
  code: string
  model?: string
  specification?: string
  manufacturer?: string
  serialNumber?: string
  assetNumber?: string
  category?: string
  labId?: string
  unitPrice?: number
  purchaseDate?: string
  warrantyExpiry?: string
  location?: string
  status?: EquipmentStatus
  totalQuantity: number
  description?: string
  custodianId?: string
}

export interface UpdateEquipmentRequest {
  name?: string
  code?: string
  model?: string
  specification?: string
  manufacturer?: string
  serialNumber?: string
  assetNumber?: string
  category?: string
  labId?: string
  unitPrice?: number
  purchaseDate?: string
  warrantyExpiry?: string
  location?: string
  totalQuantity?: number
  description?: string
  custodianId?: string
}

export interface EquipmentStatistics {
  totalCount: number
  availableCount: number
  inUseCount: number
  maintenanceCount: number
  scrappedCount: number
  totalValue?: number
  categories: { name: string; count: number; available: number }[]
  byLab?: { labName: string; count: number }[]
}

export interface ImportEquipmentResult {
  success: number
  failed: number
  total: number
  errors: string[]
}
