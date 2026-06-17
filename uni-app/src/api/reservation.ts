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
  ApprovalRequest,
} from '@/types/reservation'

/** 预约分页列表 */
export function getReservations(query?: ReservationQuery) {
  return get<ApiResponse<Reservation[]>>('/reservations', query as Record<string, string>)
}

/** 我的预约 */
export function getMyReservations() {
  return get<ApiResponse<Reservation[]>>('/reservations/my')
}

/** 待审批预约 */
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

/** 审批通过 */
export function approveReservation(id: string, request: ApprovalRequest = { approved: true }) {
  return put<ApiResponse>(`/reservations/${id}/approve`, request)
}

/** 审批驳回 */
export function rejectReservation(id: string, request: ApprovalRequest) {
  return put<ApiResponse>(`/reservations/${id}/reject`, request)
}

/** 取消预约 */
export function cancelReservation(id: string, request: CancelRequest) {
  return put<ApiResponse>(`/reservations/${id}/cancel`, request)
}
