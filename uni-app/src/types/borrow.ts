/**
 * 借用记录相关类型定义
 * 与 Flutter lib/models/borrow_record.dart 对齐
 */

export type BorrowStatus =
  | 'pending'    // 待审批
  | 'approved'   // 已通过
  | 'borrowed'   // 已借出
  | 'returning' // 归还中
  | 'returned'  // 已归还
  | 'renewing'  // 续借中
  | 'rejected'  // 已拒绝
  | 'cancelled' // 已取消
  | 'overdue'   // 已逾期

export interface BorrowRecord {
  id: number
  recordNo: string
  equipmentId: number
  equipmentName: string
  equipmentCode: string
  userId: number
  userName: string
  userPhone?: string
  borrowDate: string
  expectedReturnDate: string
  actualReturnDate?: string
  status: BorrowStatus
  purpose?: string
  remark?: string
  approverId?: number
  approverName?: string
  approvedAt?: string
  flowSteps?: BorrowFlowStep[]
  createdAt: string
}

export interface BorrowFlowStep {
  step: number
  action: string
  operatorId: number
  operatorName: string
  operatedAt: string
  remark?: string
}

export interface BorrowQuery {
  page?: number
  pageSize?: number
  status?: BorrowStatus
  equipmentId?: number
  userId?: number
  keyword?: string
  startDate?: string
  endDate?: string
}

export interface CreateBorrowRequest {
  equipmentId: number
  borrowDate: string
  expectedReturnDate: string
  purpose?: string
  remark?: string
}

export interface BorrowStat {
  total: number
  pending: number
  borrowed: number
  overdue: number
  expiringSoon: number
}
