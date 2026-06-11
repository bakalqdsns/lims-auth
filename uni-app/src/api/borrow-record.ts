/**
 * 借用记录 API
 */
import { get, post, del } from '@/utils/request'
import type { ApiResponse, PagedResponse } from '@/types/api'
import type { BorrowRecord, BorrowQuery, CreateBorrowRequest, BorrowStat } from '@/types/borrow'

export function getBorrowRecords(query?: BorrowQuery) {
  return get<PagedResponse<BorrowRecord>>('/borrow-records', query as Record<string, string | number>)
}

export function getMyBorrowRecords() {
  return get<PagedResponse<BorrowRecord>>('/borrow-records/my')
}

export function getPendingBorrowRecords() {
  return get<PagedResponse<BorrowRecord>>('/borrow-records/pending')
}

export function getExpiringBorrowRecords() {
  return get<PagedResponse<BorrowRecord>>('/borrow-records/expiring')
}

export function getOverdueBorrowRecords() {
  return get<PagedResponse<BorrowRecord>>('/borrow-records/overdue')
}

export function getBorrowRecordById(id: number) {
  return get<BorrowRecord>(`/borrow-records/${id}`)
}

export function getBorrowRecordByNo(recordNo: string) {
  return get<BorrowRecord>(`/borrow-records/no/${recordNo}`)
}

export function getBorrowRecordFlow(id: number) {
  return get<{ steps: { step: number; action: string; operatorId: number; operatorName: string; operatedAt: string; remark?: string }[] }>(
    `/borrow-records/${id}/flow`
  )
}

export function createBorrowRecord(data: CreateBorrowRequest) {
  return post<ApiResponse>('/borrow-records', data)
}

export function deleteBorrowRecord(id: number) {
  return del<ApiResponse>(`/borrow-records/${id}`)
}

export function confirmBorrow(id: number) {
  return post<ApiResponse>(`/borrow-records/${id}/confirm-borrow`, {})
}

export function submitReturn(id: number, remark?: string) {
  return post<ApiResponse>(`/borrow-records/${id}/submit-return`, { remark })
}

export function confirmReturn(id: number, remark?: string) {
  return post<ApiResponse>(`/borrow-records/${id}/confirm-return`, { remark })
}

export function renewBorrow(id: number, newReturnDate: string) {
  return post<ApiResponse>(`/borrow-records/${id}/renew`, { newReturnDate })
}

export function approveRenew(id: number) {
  return post<ApiResponse>(`/borrow-records/${id}/approve-renew`, {})
}

export function supervisorApprove(id: number) {
  return post<ApiResponse>(`/borrow-records/${id}/supervisor-approve`, {})
}

export function adminApprove(id: number) {
  return post<ApiResponse>(`/borrow-records/${id}/admin-approve`, {})
}

export function approveReturn(id: number) {
  return post<ApiResponse>(`/borrow-records/${id}/approve-return`, {})
}

export function getBorrowStats() {
  return get<BorrowStat>('/borrow-records/statistics')
}
