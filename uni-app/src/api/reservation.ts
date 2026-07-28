/**
 * 实验室预约 API
 * 对应后端 ReservationsController
 *  GET  /api/v1/reservations
 *  GET  /api/v1/reservations/{id}
 *  POST /api/v1/reservations
 *  PUT  /api/v1/reservations/{id}/approve
 *  PUT  /api/v1/reservations/{id}/reject
 *  PUT  /api/v1/reservations/{id}/cancel
 *  GET  /api/v1/reservations/pending
 *  GET  /api/v1/reservations/my
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  Reservation,
  ReservationQuery,
  CreateReservationRequest,
  CancelRequest,
  ReservationApprovalRequest,
} from '@/types/reservation'

/** 预约分页列表 (管理员/教师用: 看全部; 学生需传 applicantId=自己, 否则会看到他人) */
export function getReservations(query?: ReservationQuery) {
  return get<ApiResponse<Reservation[]>>('/reservations', query as Record<string, string>)
}

/** 我的预约 (后端自动按当前用户过滤) */
export function getMyReservations() {
  return get<ApiResponse<Reservation[]>>('/reservations/my')
}

/** 待审批预约 (返回所有 status=Pending 的预约, 给审批者用) */
export function getPendingReservations(semesterId?: string) {
  return get<ApiResponse<Reservation[]>>('/reservations/pending', semesterId ? { semesterId } : {})
}

/** 预约详情 */
export function getReservationById(id: string) {
  return get<ApiResponse<Reservation>>(`/reservations/${id}`)
}

/** 提交预约申请 */
export function createReservation(data: CreateReservationRequest) {
  return post<ApiResponse>('/reservations', data)
}

/** 审批通过 (PUT /approve 即通过, 后端不需要 approved 字段) */
export function approveReservation(id: string, comment?: string) {
  const body: ReservationApprovalRequest = { comment: comment || '审批通过' }
  return put<ApiResponse>(`/reservations/${id}/approve`, body)
}

/** 审批驳回 (后端 RejectReservationAsync 用 ApprovalRequest) */
export function rejectReservation(id: string, comment: string) {
  const body: ReservationApprovalRequest = { comment }
  return put<ApiResponse>(`/reservations/${id}/reject`, body)
}

/** 取消预约 */
export function cancelReservation(id: string, request: CancelRequest) {
  return put<ApiResponse>(`/reservations/${id}/cancel`, request)
}

