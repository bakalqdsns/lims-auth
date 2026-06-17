/**
 * 预约相关类型定义
 * 与 Flutter lib/models/reservation.dart 对齐
 */

export type ReservationStatus =
  | 'pending'    // 待审批
  | 'approved'   // 已通过
  | 'rejected'  // 已拒绝
  | 'cancelled' // 已取消
  | 'in_use'    // 使用中
  | 'completed' // 已完成
  | 'no_show'   // 未签到

export interface Reservation {
  id: string
  reservationNo: string
  labId: string
  labName: string
  labCode: string
  userId: string
  userName: string
  userPhone?: string
  date: string
  timeSlot: string
  periodStart?: string
  periodEnd?: string
  purpose?: string
  attendeeCount?: number
  status: ReservationStatus
  approverId?: string
  approverName?: string
  approvedAt?: string
  checkedInAt?: string
  checkedOutAt?: string
  remark?: string
  createdAt: string
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

export interface ApprovalRequest {
  approved: boolean
  comment?: string
  remark?: string
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
