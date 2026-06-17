/**
 * 设备借还 API
 * 对应后端 BorrowRecordsController
 *  POST /api/v1/borrow-records
 *  GET  /api/v1/borrow-records/my
 *  GET  /api/v1/borrow-records/pending
 *  GET  /api/v1/borrow-records
 *  GET  /api/v1/borrow-records/{id}
 *  GET  /api/v1/borrow-records/no/{recordNo}
 *  POST /api/v1/borrow-records/{id}/supervisor-approve
 *  POST /api/v1/borrow-records/{id}/admin-approve
 *  POST /api/v1/borrow-records/{id}/approve-return
 *  POST /api/v1/borrow-records/{id}/approve-renew
 *  POST /api/v1/borrow-records/{id}/confirm-borrow
 *  POST /api/v1/borrow-records/{id}/submit-return
 *  POST /api/v1/borrow-records/{id}/confirm-return
 *  POST /api/v1/borrow-records/{id}/renew
 *  GET  /api/v1/borrow-records/overdue
 *  GET  /api/v1/borrow-records/expiring
 *  GET  /api/v1/borrow-records/flow
 *  GET  /api/v1/borrow-records/{id}/flow
 *  DEL  /api/v1/borrow-records/{id}
 */
import { get, post, del } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  BorrowRecord,
  BorrowFlowStep,
  BorrowQuery,
  CreateBorrowRequest,
} from '@/types/borrow'

/* =============== 查询 =============== */

export function getAllBorrowRecords(query?: BorrowQuery) {
  return get<ApiResponse<BorrowRecord[]>>('/borrow-records', query as Record<string, string>)
}

export function getMyBorrowRecords(status?: string) {
  return get<ApiResponse<BorrowRecord[]>>('/borrow-records/my', status ? { status } : {})
}

export function getPendingBorrowApprovals() {
  return get<ApiResponse<BorrowRecord[]>>('/borrow-records/pending')
}

export function getBorrowRecordById(id: string) {
  return get<ApiResponse<BorrowRecord>>(`/borrow-records/${id}`)
}

export function getBorrowRecordByNo(recordNo: string) {
  return get<ApiResponse<BorrowRecord>>(`/borrow-records/no/${recordNo}`)
}

export function getOverdueBorrowRecords() {
  return get<ApiResponse<BorrowRecord[]>>('/borrow-records/overdue')
}

export function getExpiringBorrowRecords(daysBefore = 1) {
  return get<ApiResponse<BorrowRecord[]>>('/borrow-records/expiring', { daysBefore })
}

export function getBorrowFlowRecords(startDate?: string, endDate?: string) {
  return get<ApiResponse<BorrowRecord[]>>('/borrow-records/flow', {
    startDate,
    endDate,
  } as Record<string, string>)
}

export function getBorrowRecordFlow(id: string) {
  return get<ApiResponse<{ steps: BorrowFlowStep[] }>>(`/borrow-records/${id}/flow`)
}

/* =============== 申请 =============== */

export function createBorrowRequest(data: CreateBorrowRequest) {
  return post<ApiResponse>('/borrow-records', data)
}

export function deleteBorrowRecord(id: string) {
  return del<ApiResponse>(`/borrow-records/${id}`)
}

/* =============== 审批 =============== */

export function supervisorApprove(id: string, approved: boolean, remark?: string) {
  return post<ApiResponse>(`/borrow-records/${id}/supervisor-approve`, { approved, remark })
}

export function adminApprove(id: string, approved: boolean, remark?: string) {
  return post<ApiResponse>(`/borrow-records/${id}/admin-approve`, { approved, remark })
}

export function approveReturn(
  id: string,
  approved: boolean,
  condition?: string,
  remarks?: string
) {
  return post<ApiResponse>(`/borrow-records/${id}/approve-return`, { approved, condition, remarks })
}

export function approveRenew(id: string, approved: boolean, remark?: string) {
  return post<ApiResponse>(`/borrow-records/${id}/approve-renew`, { approved, remark })
}

/* =============== 借出/归还执行 =============== */

export function confirmBorrow(id: string) {
  return post<ApiResponse>(`/borrow-records/${id}/confirm-borrow`, {})
}

export function submitReturn(id: string) {
  return post<ApiResponse>(`/borrow-records/${id}/submit-return`, {})
}

export function confirmReturn(id: string, condition = '完好', remarks?: string) {
  return post<ApiResponse>(`/borrow-records/${id}/confirm-return`, { condition, remarks })
}

export function renewBorrow(id: string, newReturnDate: string) {
  return post<ApiResponse>(`/borrow-records/${id}/renew`, { newReturnDate })
}
