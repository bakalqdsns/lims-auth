import axios from 'axios'

const API_BASE_URL = '/api/v1'

// 校区管理 API
export const campusApi = {
  getList: (keyword?: string) =>
    axios.get(`${API_BASE_URL}/campuses`, { params: { keyword } }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/campuses/${id}`),
  create: (data: CreateCampusRequest) =>
    axios.post(`${API_BASE_URL}/campuses`, data),
  update: (id: string, data: UpdateCampusRequest) =>
    axios.put(`${API_BASE_URL}/campuses/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/campuses/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/campuses/${id}/status`, { isActive })
}

// 楼宇管理 API
export const buildingApi = {
  getList: (keyword?: string, campusId?: string) =>
    axios.get(`${API_BASE_URL}/buildings`, { params: { keyword, campusId } }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/buildings/${id}`),
  getByCampusId: (campusId: string) =>
    axios.get(`${API_BASE_URL}/buildings/by-campus/${campusId}`),
  create: (data: CreateBuildingRequest) =>
    axios.post(`${API_BASE_URL}/buildings`, data),
  update: (id: string, data: UpdateBuildingRequest) =>
    axios.put(`${API_BASE_URL}/buildings/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/buildings/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/buildings/${id}/status`, { isActive })
}

// 类型定义
export interface CreateCampusRequest {
  code: string
  name: string
  campusType: string
  address?: string
  area?: number
  contactPhone?: string
  managerId?: string
  description?: string
}

export interface UpdateCampusRequest extends Partial<CreateCampusRequest> {
  isActive?: boolean
}

export interface CreateBuildingRequest {
  code: string
  name: string
  campusId: string
  buildingType: string
  address?: string
  floorCount: number
  buildingArea?: number
  builtYear?: number
  managerId?: string
  description?: string
}

export interface UpdateBuildingRequest extends Partial<CreateBuildingRequest> {
  isActive?: boolean
}

// DTO 类型
export interface CampusDto {
  id: string
  code: string
  name: string
  campusType: string
  address?: string
  area?: number
  contactPhone?: string
  managerId?: string
  managerName?: string
  description?: string
  isActive: boolean
  createdAt: string
  buildingCount: number
}

export interface BuildingDto {
  id: string
  code: string
  name: string
  campusId: string
  campusName: string
  buildingType: string
  address?: string
  floorCount: number
  buildingArea?: number
  builtYear?: number
  managerId?: string
  managerName?: string
  description?: string
  isActive: boolean
  createdAt: string
  labCount: number
}

// 常量
export const CAMPUS_TYPES = ['主校区', '分校区']
export const BUILDING_TYPES = ['实验楼', '教学楼', '办公楼', '图书馆', '体育馆', '其他']
