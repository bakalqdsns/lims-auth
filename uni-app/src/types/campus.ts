/**
 * 校区、楼宇、实验室相关类型定义
 * 与 Flutter lib/models/campus.dart, building.dart, lab.dart 对齐
 */

export interface Campus {
  id: string
  name: string
  code: string
  address?: string
  status: number
  buildingCount: number
  labCount: number
}

export interface Building {
  id: string
  name: string
  code: string
  campusId: string
  campusName: string
  floors?: number
  status: number
}

export interface Lab {
  id: string
  name: string
  code: string
  buildingId: string
  buildingName: string
  campusId: string
  campusName: string
  floor?: number
  roomNumber?: string
  capacity: number
  area?: number
  type?: string
  description?: string
  equipmentCount: number
  status: number
  images?: string[]
  openingHours?: string
  manager?: string
  managerPhone?: string
}

export interface LabQuery {
  page?: number
  pageSize?: number
  campusId?: string
  buildingId?: string
  keyword?: string
  status?: number
}

export interface CreateLabRequest {
  name: string
  code: string
  buildingId: string
  campusId: string
  floor?: number
  roomNumber?: string
  capacity: number
  area?: number
  type?: string
  description?: string
}
