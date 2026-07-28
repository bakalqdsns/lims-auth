/**
 * 校区、楼宇、实验室相关类型定义
 * 与后端 Campus / Building / Lab DTO 对齐
 */

export interface Campus {
  id: string
  name: string
  code: string
  address?: string
  contact?: string
  phone?: string
  description?: string
  status: number
  buildingCount: number
  labCount: number
  createdAt: string
}

export interface CreateCampusRequest {
  name: string
  code: string
  address?: string
  contact?: string
  phone?: string
  description?: string
  status?: number
}

export interface UpdateCampusRequest {
  name?: string
  code?: string
  address?: string
  contact?: string
  phone?: string
  description?: string
  status?: number
}

export interface Building {
  id: string
  name: string
  code: string
  campusId: string
  campusName?: string
  floors?: number
  totalArea?: number
  description?: string
  status: number
  labCount: number
  createdAt: string
}

export interface CreateBuildingRequest {
  name: string
  code: string
  campusId: string
  floors?: number
  totalArea?: number
  description?: string
  status?: number
}

export interface UpdateBuildingRequest {
  name?: string
  code?: string
  floors?: number
  totalArea?: number
  description?: string
  status?: number
}

export interface Lab {
  id: string
  name: string
  code: string
  campusId: string
  campusName?: string
  buildingId: string
  buildingName?: string
  departmentId?: string
  departmentName?: string
  floor?: number
  roomNumber?: string
  capacity: number
  area?: number
  type?: string
  labType?: string
  description?: string
  managerId?: string
  managerName?: string
  managerPhone?: string
  equipmentCount: number
  status: number
  images?: string[]
  openingHours?: string
  createdAt: string
}

export interface LabQuery {
  keyword?: string
  campusId?: string
  buildingId?: string
  departmentId?: string
  labType?: string
  status?: number
}

export interface CreateLabRequest {
  name: string
  code: string
  campusId: string
  buildingId: string
  departmentId?: string
  floor?: number
  roomNumber?: string
  capacity: number
  area?: number
  labType?: string
  description?: string
  managerId?: string
  managerPhone?: string
  status?: number
}

export interface UpdateLabRequest {
  name?: string
  code?: string
  floor?: number
  roomNumber?: string
  capacity?: number
  area?: number
  labType?: string
  description?: string
  managerId?: string
  managerPhone?: string
  status?: number
}
