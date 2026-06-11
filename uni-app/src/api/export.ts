/**
 * 导出 API
 */
import { get } from '@/utils/request'

export function exportSchedulePlan(semesterId?: number) {
  return get<string>('/export/experiment/schedule-plan', { semesterId } as Record<string, string | number>)
}

export function exportTaskList(semesterId?: number) {
  return get<string>('/export/experiment/task-list', { semesterId } as Record<string, string | number>)
}

export function getExportTemplates() {
  return get<{ name: string; url: string }[]>('/export/templates')
}
