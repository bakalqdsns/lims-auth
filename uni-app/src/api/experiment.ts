/**
 * 实验教学 API
 * 对应后端 ExperimentsController
 * 注意: 后端路由前缀是 /api/(不是 /api/v1/), 通过 request({ prefix: false }) 直连
 *
 * 实验教学任务:
 *  GET    /api/experiments/tasks
 *  GET    /api/experiments/tasks/{id}
 *  POST   /api/experiments/tasks
 *  PUT    /api/experiments/tasks/{id}
 *  DELETE /api/experiments/tasks/{id}
 *
 * 实验项目:
 *  GET    /api/experiments/items
 *  GET    /api/experiments/items/by-task/{taskId}
 *  GET    /api/experiments/items/{id}
 *  POST   /api/experiments/items
 *  PUT    /api/experiments/items/{id}
 *  DELETE /api/experiments/items/{id}
 *
 * 项目开出安排:
 *  GET    /api/experiments/schedules
 *  GET    /api/experiments/schedules/{id}
 *  POST   /api/experiments/schedules
 *  PUT    /api/experiments/schedules/{id}
 *  DELETE /api/experiments/schedules/{id}
 *
 * 质量评估:
 *  GET    /api/experiments/quality
 *  GET    /api/experiments/quality/{id}
 *  POST   /api/experiments/quality
 *  PUT    /api/experiments/quality/{id}
 *  DELETE /api/experiments/quality/{id}
 *
 * 实训教学计划:
 *  GET    /api/experiments/training-plans
 *  GET    /api/experiments/training-plans/{id}
 *  POST   /api/experiments/training-plans
 *  PUT    /api/experiments/training-plans/{id}
 *  DELETE /api/experiments/training-plans/{id}
 *  POST   /api/experiments/training-plans/{id}/approve
 */
import { request } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  ExperimentTeachingTask,
  ExperimentItem,
  ExperimentItemSchedule,
  ExperimentQualityAssessment,
  TrainingTeachingPlan,
  ApproveTrainingPlanRequest,
} from '@/types/experiment'

/** 调用 ExperimentsController 时关闭默认前缀 */
function expOptions<T>() {
  return { prefix: false as const } satisfies Pick<Parameters<typeof request<T>>[0], 'prefix'>
}

/* =============== 实验教学任务 =============== */

export function getExperimentTasks(query?: {
  semesterId?: string
  majorId?: string
  classId?: string
}) {
  return request<ApiResponse<ExperimentTeachingTask[]>>({
    url: '/api/experiments/tasks',
    method: 'GET',
    params: query as Record<string, string>,
    ...expOptions(),
  })
}

export function getExperimentTaskById(id: string) {
  return request<ApiResponse<ExperimentTeachingTask>>({
    url: `/api/experiments/tasks/${id}`,
    method: 'GET',
    ...expOptions(),
  })
}

export function createExperimentTask(data: Partial<ExperimentTeachingTask>) {
  return request<ApiResponse<ExperimentTeachingTask>>({
    url: '/api/experiments/tasks',
    method: 'POST',
    data,
    ...expOptions(),
  })
}

export function updateExperimentTask(id: string, data: Partial<ExperimentTeachingTask>) {
  return request<ApiResponse>({
    url: `/api/experiments/tasks/${id}`,
    method: 'PUT',
    data,
    ...expOptions(),
  })
}

export function deleteExperimentTask(id: string) {
  return request<ApiResponse>({
    url: `/api/experiments/tasks/${id}`,
    method: 'DELETE',
    ...expOptions(),
  })
}

/* =============== 实验项目 =============== */

export function getExperimentItems(query?: { courseCode?: string; experimentType?: string }) {
  return request<ApiResponse<ExperimentItem[]>>({
    url: '/api/experiments/items',
    method: 'GET',
    params: query as Record<string, string>,
    ...expOptions(),
  })
}

export function getExperimentItemsByTask(taskId: string) {
  return request<ApiResponse<ExperimentItem[]>>({
    url: `/api/experiments/items/by-task/${taskId}`,
    method: 'GET',
    ...expOptions(),
  })
}

export function getExperimentItemById(id: string) {
  return request<ApiResponse<ExperimentItem>>({
    url: `/api/experiments/items/${id}`,
    method: 'GET',
    ...expOptions(),
  })
}

export function createExperimentItem(data: Partial<ExperimentItem>) {
  return request<ApiResponse<ExperimentItem>>({
    url: '/api/experiments/items',
    method: 'POST',
    data,
    ...expOptions(),
  })
}

export function updateExperimentItem(id: string, data: Partial<ExperimentItem>) {
  return request<ApiResponse>({
    url: `/api/experiments/items/${id}`,
    method: 'PUT',
    data,
    ...expOptions(),
  })
}

export function deleteExperimentItem(id: string) {
  return request<ApiResponse>({
    url: `/api/experiments/items/${id}`,
    method: 'DELETE',
    ...expOptions(),
  })
}

/* =============== 实验项目开出安排 =============== */

export function getExperimentSchedules(query?: { taskId?: string; weekNumber?: number }) {
  return request<ApiResponse<ExperimentItemSchedule[]>>({
    url: '/api/experiments/schedules',
    method: 'GET',
    params: query as Record<string, string | number>,
    ...expOptions(),
  })
}

export function getExperimentScheduleById(id: string) {
  return request<ApiResponse<ExperimentItemSchedule>>({
    url: `/api/experiments/schedules/${id}`,
    method: 'GET',
    ...expOptions(),
  })
}

export function createExperimentSchedule(data: Partial<ExperimentItemSchedule>) {
  return request<ApiResponse<ExperimentItemSchedule>>({
    url: '/api/experiments/schedules',
    method: 'POST',
    data,
    ...expOptions(),
  })
}

export function updateExperimentSchedule(id: string, data: Partial<ExperimentItemSchedule>) {
  return request<ApiResponse>({
    url: `/api/experiments/schedules/${id}`,
    method: 'PUT',
    data,
    ...expOptions(),
  })
}

export function deleteExperimentSchedule(id: string) {
  return request<ApiResponse>({
    url: `/api/experiments/schedules/${id}`,
    method: 'DELETE',
    ...expOptions(),
  })
}

/* =============== 质量评估 =============== */

export function getQualityAssessments() {
  return request<ApiResponse<ExperimentQualityAssessment[]>>({
    url: '/api/experiments/quality',
    method: 'GET',
    ...expOptions(),
  })
}

export function getQualityAssessmentById(id: string) {
  return request<ApiResponse<ExperimentQualityAssessment>>({
    url: `/api/experiments/quality/${id}`,
    method: 'GET',
    ...expOptions(),
  })
}

export function createQualityAssessment(data: Partial<ExperimentQualityAssessment>) {
  return request<ApiResponse<ExperimentQualityAssessment>>({
    url: '/api/experiments/quality',
    method: 'POST',
    data,
    ...expOptions(),
  })
}

export function updateQualityAssessment(id: string, data: Partial<ExperimentQualityAssessment>) {
  return request<ApiResponse>({
    url: `/api/experiments/quality/${id}`,
    method: 'PUT',
    data,
    ...expOptions(),
  })
}

export function deleteQualityAssessment(id: string) {
  return request<ApiResponse>({
    url: `/api/experiments/quality/${id}`,
    method: 'DELETE',
    ...expOptions(),
  })
}

/* =============== 实训教学计划 =============== */

export function getTrainingPlans(query?: {
  courseId?: string
  status?: string
  approvalStatus?: string
}) {
  return request<ApiResponse<TrainingTeachingPlan[]>>({
    url: '/api/experiments/training-plans',
    method: 'GET',
    params: query as Record<string, string>,
    ...expOptions(),
  })
}

export function getTrainingPlanById(id: string) {
  return request<ApiResponse<TrainingTeachingPlan>>({
    url: `/api/experiments/training-plans/${id}`,
    method: 'GET',
    ...expOptions(),
  })
}

export function createTrainingPlan(data: Partial<TrainingTeachingPlan>) {
  return request<ApiResponse<TrainingTeachingPlan>>({
    url: '/api/experiments/training-plans',
    method: 'POST',
    data,
    ...expOptions(),
  })
}

export function updateTrainingPlan(id: string, data: Partial<TrainingTeachingPlan>) {
  return request<ApiResponse>({
    url: `/api/experiments/training-plans/${id}`,
    method: 'PUT',
    data,
    ...expOptions(),
  })
}

export function deleteTrainingPlan(id: string) {
  return request<ApiResponse>({
    url: `/api/experiments/training-plans/${id}`,
    method: 'DELETE',
    ...expOptions(),
  })
}

export function approveTrainingPlan(id: string, data: ApproveTrainingPlanRequest) {
  return request<ApiResponse>({
    url: `/api/experiments/training-plans/${id}/approve`,
    method: 'POST',
    data,
    ...expOptions(),
  })
}
