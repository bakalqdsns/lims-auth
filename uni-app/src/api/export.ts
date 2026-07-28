/**
 * 文档导出 API
 * 对应后端 ExportController
 *  GET /api/v1/export/templates
 *  GET /api/v1/export/experiment/task-list
 *  GET /api/v1/export/experiment/schedule-plan
 */
import { get } from '@/utils/request'
import type { ApiResponse } from '@/types/api'

export interface ExportTemplate {
  key: string
  name: string
}

/** 可用模板 */
export function getExportTemplates() {
  return get<ApiResponse<ExportTemplate[]>>('/export/templates')
}

/** 导出实验课程教学任务一览表 (docx) */
export function exportExperimentTaskList(query?: {
  semesterId?: string
  majorId?: string
  classId?: string
}) {
  return get<ArrayBuffer>('/export/experiment/task-list', query as Record<string, string>, {
    loadingText: '导出中...',
    showError: false,
  })
}

/** 导出实验教学授课计划表 (docx) */
export function exportExperimentSchedulePlan(query?: {
  semesterId?: string
  majorId?: string
  classId?: string
}) {
  return get<ArrayBuffer>('/export/experiment/schedule-plan', query as Record<string, string>, {
    loadingText: '导出中...',
    showError: false,
  })
}

/** 把 ArrayBuffer 保存为本地文件并触发下载 (浏览器/小程序通用) */
export function saveArrayBufferToFile(
  buffer: ArrayBuffer,
  fileName: string,
  mimeType: string
): Promise<string> {
  return new Promise((resolve, reject) => {
    // #ifdef H5
    try {
      const blob = new Blob([buffer], { type: mimeType })
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = fileName
      a.click()
      URL.revokeObjectURL(url)
      resolve(url)
    } catch (e) {
      reject(e)
    }
    // #endif

    // #ifndef H5
    const fileManager = uni.getFileSystemManager()
    const filePath = `${uni.env?.USER_DATA_PATH ?? ''}/${fileName}`
    fileManager.writeFile({
      filePath,
      data: buffer as unknown as ArrayBuffer,
      encoding: 'binary',
      success: () => {
        uni.openDocument({
          filePath,
          showMenu: true,
          success: () => resolve(filePath),
          fail: (err) => reject(err),
        })
      },
      fail: (err) => reject(err),
    })
    // #endif
  })
}
