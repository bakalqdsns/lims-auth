/**
 * 设备管理 API
 */
import { get, post, put, del, patch } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Equipment, EquipmentQuery, CreateEquipmentRequest, EquipmentStatistics } from '@/types/equipment'

export function getEquipments(query?: EquipmentQuery) {
  return get<PagedResponse<Equipment>>('/equipments', query as Record<string, string | number>)
}

export function getEquipmentById(id: number) {
  return get<Equipment>(`/equipments/${id}`)
}

export function createEquipment(data: CreateEquipmentRequest) {
  return post<ApiResponse>('/equipments', data)
}

export function updateEquipment(id: number, data: Partial<CreateEquipmentRequest>) {
  return put<ApiResponse>(`/equipments/${id}`, data)
}

export function deleteEquipment(id: number) {
  return del<ApiResponse>(`/equipments/${id}`)
}

export function updateEquipmentStatus(id: number, status: number) {
  return patch<ApiResponse>(`/equipments/${id}/status`, { status })
}

export function getEquipmentStatistics() {
  return get<EquipmentStatistics>('/equipments/statistics')
}

export function importEquipments(filePath: string) {
  return post<ApiResponse>('/equipments/import', { filePath })
}

export function getEquipmentImportTemplate() {
  return get<string>('/equipments/import-template')
}

export function exportEquipments(query?: EquipmentQuery) {
  return get<string>('/equipments/export', query as Record<string, string | number>)
}
