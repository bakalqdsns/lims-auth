/**
 * 设备管理 API
 * 对应后端 EquipmentsController
 *  GET    /api/v1/equipments
 *  GET    /api/v1/equipments/{id}
 *  POST   /api/v1/equipments
 *  PUT    /api/v1/equipments/{id}
 *  DELETE /api/v1/equipments/{id}
 *  PATCH  /api/v1/equipments/{id}/status
 *  PATCH  /api/v1/equipments/{id}/equipment-status
 *  GET    /api/v1/equipments/statistics
 *  GET    /api/v1/equipments/export            (返回二进制 xlsx)
 *  POST   /api/v1/equipments/import            (上传文件)
 *  GET    /api/v1/equipments/import-template   (返回二进制 xlsx)
 */
import { get, post, put, del, patch, uploadFile } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  Equipment,
  EquipmentQuery,
  CreateEquipmentRequest,
  UpdateEquipmentRequest,
  EquipmentStatistics,
  ImportEquipmentResult,
  EquipmentStatus,
} from '@/types/equipment'

/** 设备列表 (分页, 后端返回 { items, total, page, pageSize }) */
export function getEquipments(query?: EquipmentQuery) {
  return get<{ items: Equipment[]; total: number; page: number; pageSize: number }>(
    '/equipments',
    query as Record<string, string | number>
  )
}

/** 设备详情 */
export function getEquipmentById(id: string) {
  return get<ApiResponse<Equipment>>(`/equipments/${id}`)
}

/** 创建设备 */
export function createEquipment(data: CreateEquipmentRequest) {
  return post<ApiResponse>('/equipments', data)
}

/** 更新设备 */
export function updateEquipment(id: string, data: UpdateEquipmentRequest) {
  return put<ApiResponse>(`/equipments/${id}`, data)
}

/** 删除设备 */
export function deleteEquipment(id: string) {
  return del<ApiResponse>(`/equipments/${id}`)
}

/** 启用/禁用 (isActive) */
export function toggleEquipmentStatus(id: string, isActive: boolean) {
  return patch<ApiResponse>(`/equipments/${id}/status`, { isActive })
}

/** 设备业务状态 (在用 / 空闲 / 维修 / 报废) */
export function updateEquipmentBusinessStatus(id: string, status: EquipmentStatus) {
  return patch<ApiResponse>(`/equipments/${id}/equipment-status`, { status })
}

/** 设备统计 */
export function getEquipmentStatistics() {
  return get<ApiResponse<EquipmentStatistics>>('/equipments/statistics')
}

/** 导出 Excel (file blob) */
export function exportEquipments(query?: EquipmentQuery) {
  return get<ArrayBuffer>('/equipments/export', query as Record<string, string | number>, {
    loadingText: '导出中...',
    showError: false,
  })
}

/** 下载导入模板 */
export function getEquipmentImportTemplate() {
  return get<ArrayBuffer>('/equipments/import-template', {}, { showError: false })
}

/** 导入 Excel */
export function importEquipments(filePath: string, fileName: string) {
  return uploadFile('/equipments/import', filePath, 'file').then(
    () => ({ code: 200, message: 'ok', data: null } as ApiResponse<ImportEquipmentResult>)
  )
}
