/**
 * 预约相关类型定义
 * 与后端 ScheduleDtos.cs + ReservationService.cs 对齐:
 * 后端 ApprovalStatus 枚举的 ToString() 为 PascalCase (Pending / Approved / Rejected)
 */

export type ReservationStatus =
  | 'Pending'    // 待审批
  | 'Approved'   // 已通过
  | 'Rejected'   // 已拒绝
  | 'Cancelled'  // 已取消
  | 'InUse'      // 使用中
  | 'Completed'  // 已完成
  | 'NoShow'     // 未签到

export const RESERVATION_STATUS_LABELS: Record<ReservationStatus, string> = {
  Pending: '待审批',
  Approved: '已通过',
  Rejected: '已拒绝',
  Cancelled: '已取消',
  InUse: '使用中',
  Completed: '已完成',
  NoShow: '未签到',
}

export interface Reservation {
  id: string
  reservationNo?: string
  semesterId?: string
  semesterName?: string
  labId: string
  labName?: string
  labCode?: string
  applicantId: string
  applicantName: string
  applicantPhone?: string
  useDate?: string
  date?: string
  weekNumber?: number
  dayOfWeek?: number
  periodNumbers?: number[]
  startPeriod?: number
  endPeriod?: number
  timeSlot?: string
  projectName?: string
  projectCategory?: string
  purpose?: string
  attendeeCount?: number
  memberCount?: number
  /** 后端返回字符串枚举名 (Pending / Approved / Rejected ...) */
  status: ReservationStatus | string
  approvalComment?: string
  approvedBy?: string
  approverName?: string
  approvedAt?: string
  isCancelled?: boolean
  cancelReason?: string
  checkedInAt?: string
  checkedOutAt?: string
  remark?: string
  createdAt: string
  /** 历史字段兼容: 部分旧调用使用 userName/userId */
  userId?: string
  userName?: string
  userPhone?: string
}

export interface ReservationQuery {
  page?: number
  pageSize?: number
  status?: ReservationStatus
  labId?: string
  userId?: string
  keyword?: string
  date?: string
  startDate?: string
  endDate?: string
}

export interface CreateReservationRequest {
  labId: string
  semesterId?: string
  date: string
  weekNumber?: number
  dayOfWeek?: number
  startPeriod?: number
  endPeriod?: number
  timeSlot?: string
  purpose?: string
  attendeeCount?: number
  remark?: string
}

export interface ReservationApprovalRequest {
  comment: string
  approverName?: string
}

export interface CancelRequest {
  reason: string
}

export interface TimeSlot {
  id: string
  name: string
  startTime: string
  endTime: string
  available: boolean
}

export interface DaySchedule {
  date: string
  weekday: string
  timeSlots: TimeSlot[]
}
