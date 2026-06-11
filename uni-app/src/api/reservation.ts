/**
 * 预约 API
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { Reservation, ReservationQuery, CreateReservationRequest } from '@/types/reservation'

export function getReservations(query?: ReservationQuery) {
  return get<PagedResponse<Reservation>>('/reservations', query as Record<string, string | number>)
}

export function getReservationById(id: number) {
  return get<Reservation>(`/reservations/${id}`)
}

export function createReservation(data: CreateReservationRequest) {
  return post<ApiResponse>('/reservations', data)
}

export function approveReservation(id: number) {
  return put<ApiResponse>(`/reservations/${id}/approve`, {})
}

export function rejectReservation(id: number, reason?: string) {
  return put<ApiResponse>(`/reservations/${id}/reject`, { reason })
}

export function cancelReservation(id: number) {
  return put<ApiResponse>(`/reservations/${id}/cancel`, {})
}

export function getPendingReservations() {
  return get<PagedResponse<Reservation>>('/reservations/pending')
}

export function getMyReservations() {
  return get<PagedResponse<Reservation>>('/reservations/my')
}
