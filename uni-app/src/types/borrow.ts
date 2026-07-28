/**
 * 借用记录相关类型定义
 * 与后端 EquipmentBorrow.cs + BorrowStatus 常量对齐
 *  后端 Status 字段存的是中文字符串 (e.g. "已借出" / "待归还")
 */

export const BORROW_STATUS = {
  PendingTeacherApproval: '待老师审批',
  PendingAdminApproval: '待管理员审批',
  AwaitingPickup: '待领取',
  Borrowed: '已借出',
  AwaitingReturn: '待归还',
  Returned: '已归还',
  Rejected: '已拒绝',
  Overdue: '已逾期',
  RenewPending: '续借审批中',
  ReturnPending: '管理员审批中',
} as const

export type BorrowStatus = (typeof BORROW_STATUS)[keyof typeof BORROW_STATUS]

export const BORROW_STATUS_LABELS: Record<BorrowStatus, string> = {
  待老师审批: '待老师审批',
  待管理员审批: '待管理员审批',
  待领取: '待领取',
  已借出: '已借出',
  待归还: '待归还',
  已归还: '已归还',
  已拒绝: '已拒绝',
  已逾期: '已逾期',
  续借审批中: '续借审批中',
  管理员审批中: '管理员审批中',
}

/**
 * 借用状态 -> CSS 类名后缀（仅英文，避免微信 WXSS 不支持中文字符选择器）
 * 后端 BorrowStatus 使用中文字符串,前端 class 不能直接拼接中文
 */
export const BORROW_STATUS_CLASS: Record<string, string> = {
  待老师审批: 'pending-teacher',
  待管理员审批: 'pending-admin',
  待领取: 'awaiting-pickup',
  已借出: 'borrowed',
  待归还: 'awaiting-return',
  已归还: 'returned',
  已拒绝: 'rejected',
  已逾期: 'overdue',
  续借审批中: 'renew-pending',
  管理员审批中: 'return-pending',
  // 历史英文值
  pending: 'pending',
  approved: 'approved',
  borrowed: 'borrowed',
  returning: 'returning',
  returned: 'returned',
  renewing: 'renewing',
  rejected: 'rejected',
  cancelled: 'cancelled',
  overdue: 'overdue',
  pending_supervisor: 'pending-supervisor',
  pending_admin: 'pending-admin',
  PendingSupervisor: 'pending-supervisor',
  PendingAdmin: 'pending-admin',
}

export function borrowStatusClass(status: string): string {
  return BORROW_STATUS_CLASS[status] || 'default'
}

export interface BorrowRecord {
  id: string
  recordNo: string
  equipmentId: string
  equipmentName: string
  equipmentCode: string
  userId: string
  userName: string
  userPhone?: string
  borrowDate: string
  expectedReturnDate: string
  actualReturnDate?: string
  status: BorrowStatus
  purpose?: string
  remark?: string
  approverId?: string
  approverName?: string
  approvedAt?: string
  flowSteps?: BorrowFlowStep[]
  createdAt: string
}

export interface BorrowFlowStep {
  step: number
  action: string
  operatorId: string
  operatorName: string
  operatedAt: string
  remark?: string
}

export interface BorrowQuery {
  page?: number
  pageSize?: number
  status?: BorrowStatus
  equipmentId?: string
  userId?: string
  keyword?: string
  startDate?: string
  endDate?: string
}

export interface CreateBorrowRequest {
  equipmentId: string
  equipmentName?: string
  quantity?: number
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
