import axios from 'axios'

const API_BASE_URL = '/api/v1'

// ============================================================
// 排课管理 API
// ============================================================
export const scheduleApi = {
  getList: (params?: ScheduleQuery) =>
    axios.get(`${API_BASE_URL}/schedules`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/schedules/${id}`),
  getTableView: (params?: ScheduleQuery) =>
    axios.get(`${API_BASE_URL}/schedules/table-view`, { params }),
  getAvailableLabs: (params?: AvailabilityQuery) =>
    axios.get(`${API_BASE_URL}/schedules/available-labs`, { params }),
  checkConflicts: (data: any) =>
    axios.post(`${API_BASE_URL}/schedules/check-conflicts`, data),
  create: (data: CreateScheduleEntryRequest) =>
    axios.post(`${API_BASE_URL}/schedules`, data),
  update: (id: string, data: UpdateScheduleEntryRequest) =>
    axios.put(`${API_BASE_URL}/schedules/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/schedules/${id}`),
  getByLab: (labId: string, params?: ScheduleQuery) =>
    axios.get(`${API_BASE_URL}/schedules/by-lab/${labId}`, { params }),
  getByTeacher: (teacherId: string, params?: ScheduleQuery) =>
    axios.get(`${API_BASE_URL}/schedules/by-teacher/${teacherId}`, { params }),
  getByClass: (classId: string, params?: ScheduleQuery) =>
    axios.get(`${API_BASE_URL}/schedules/by-class/${classId}`, { params })
}

// ============================================================
// 预约申请 API
// ============================================================
export const reservationApi = {
  getList: (params?: ReservationQuery) =>
    axios.get(`${API_BASE_URL}/reservations`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/reservations/${id}`),
  getPending: (semesterId?: string) =>
    axios.get(`${API_BASE_URL}/reservations/pending`, { params: { semesterId } }),
  create: (data: CreateReservationRequest) =>
    axios.post(`${API_BASE_URL}/reservations`, data),
  approve: (id: string, data: ApprovalRequest) =>
    axios.put(`${API_BASE_URL}/reservations/${id}/approve`, data),
  reject: (id: string, data: ApprovalRequest) =>
    axios.put(`${API_BASE_URL}/reservations/${id}/reject`, data),
  cancel: (id: string, data: CancelRequest) =>
    axios.put(`${API_BASE_URL}/reservations/${id}/cancel`, data)
}

// ============================================================
// 授课申请 API
// ============================================================
export const teachingApplicationApi = {
  getList: (params?: TeachingApplicationQuery) =>
    axios.get(`${API_BASE_URL}/teaching-applications`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/teaching-applications/${id}`),
  getPending: (semesterId?: string) =>
    axios.get(`${API_BASE_URL}/teaching-applications/pending`, { params: { semesterId } }),
  getMy: (semesterId?: string) =>
    axios.get(`${API_BASE_URL}/teaching-applications/my`, { params: { semesterId } }),
  create: (data: CreateTeachingApplicationRequest) =>
    axios.post(`${API_BASE_URL}/teaching-applications`, data),
  approve: (id: string, data: ApprovalRequest) =>
    axios.put(`${API_BASE_URL}/teaching-applications/${id}/approve`, data),
  reject: (id: string, data: ApprovalRequest) =>
    axios.put(`${API_BASE_URL}/teaching-applications/${id}/reject`, data),
  cancel: (id: string) =>
    axios.put(`${API_BASE_URL}/teaching-applications/${id}/cancel`, {})
}

// ============================================================
// 使用登记 API
// ============================================================
export const usageRegistrationApi = {
  getList: (params?: UsageRegistrationQuery) =>
    axios.get(`${API_BASE_URL}/usage-registrations`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/usage-registrations/${id}`),
  getPending: (userId?: string, semesterId?: string) =>
    axios.get(`${API_BASE_URL}/usage-registrations/pending`, { params: { userId, semesterId } }),
  getOverdue: (semesterId?: string) =>
    axios.get(`${API_BASE_URL}/usage-registrations/overdue`, { params: { semesterId } }),
  create: (data: CreateUsageRegistrationRequest) =>
    axios.post(`${API_BASE_URL}/usage-registrations`, data),
  remind: (id: string) =>
    axios.put(`${API_BASE_URL}/usage-registrations/${id}/remind`, {}),
  getCompletionRate: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/usage-registrations/statistics/completion`, { params })
}

// ============================================================
// 统计 API
// ============================================================
export const statisticsApi = {
  getWeeklySummary: (params?: ScheduleStatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/weekly-summary`, { params }),
  getLabUsage: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/lab-usage`, { params }),
  getByMajor: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/by-major`, { params }),
  getByClass: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/by-class`, { params }),
  getByGrade: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/by-grade`, { params }),
  getByCourse: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/by-course`, { params }),
  getReservationStats: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/reservation`, { params }),
  getCompletionRate: (params?: StatisticsQuery) =>
    axios.get(`${API_BASE_URL}/statistics/completion-rate`, { params }),
  getDashboard: (params?: DashboardQuery) =>
    axios.get(`${API_BASE_URL}/statistics/dashboard`, { params }),
  exportExcel: (params?: StatisticsQuery & { type?: string }) =>
    axios.get(`${API_BASE_URL}/statistics/export`, { params, responseType: 'blob' })
}

// ============================================================
// 类型定义
// ============================================================

// 通用查询
export interface ScheduleQuery {
  semesterId?: string
  weekNumber?: number
  dayOfWeek?: number
  labId?: string
  buildingId?: string
  classId?: string
  teacherId?: string
  courseId?: string
  source?: string
  status?: string
  page?: number
  pageSize?: number
}

export interface AvailabilityQuery {
  semesterId: string
  weekNumber: number
  dayOfWeek: number
  periodNumbers: number[]
  buildingId?: string
}

export interface ApprovalRequest {
  comment: string
  approverName?: string
}

export interface CancelRequest {
  reason: string
}

export interface StatisticsQuery {
  semesterId?: string
  weekNumber?: number
  startWeek?: number
  endWeek?: number
  buildingId?: string
  labId?: string
  majorId?: string
  classId?: string
  courseId?: string
}

export interface ScheduleStatisticsQuery {
  semesterId?: string
  weekNumber?: number
  buildingId?: string
  labId?: string
  page?: number
  pageSize?: number
}

export interface DashboardQuery {
  semesterId?: string
  weekNumber?: number
}

// DTO 类型
export interface ScheduleEntryDto {
  id: string
  semesterId: string
  semesterName?: string
  labId?: string
  labName?: string
  weekNumber: number
  dayOfWeek: number
  periodNumber: number
  source: string
  status: string
  reservationId?: string
  teachingApplicationId?: string
  experimentTaskId?: string
  teachingTaskId?: string
  courseName?: string
  projectName?: string
  courseId?: string
  teacherId?: string
  teacherName?: string
  classId?: string
  className?: string
  majorId?: string
  majorName?: string
  studentCount?: number
  buildingName?: string
  roomNumber?: string
  remark?: string
  hasConflict: boolean
  conflictInfo?: string
  createdAt: string
  createdBy?: string
}

export interface CreateScheduleEntryRequest {
  semesterId: string
  labId?: string
  weekNumber: number
  dayOfWeek: number
  periodNumber: number
  source?: string
  reservationId?: string
  teachingApplicationId?: string
  experimentTaskId?: string
  teachingTaskId?: string
  courseName?: string
  projectName?: string
  courseId?: string
  teacherId?: string
  teacherName?: string
  classId?: string
  className?: string
  majorId?: string
  majorName?: string
  studentCount?: number
  buildingName?: string
  roomNumber?: string
  remark?: string
  forceSchedule?: boolean
}

export interface UpdateScheduleEntryRequest {
  labId?: string
  weekNumber?: number
  dayOfWeek?: number
  periodNumber?: number
  status?: string
  teacherId?: string
  teacherName?: string
  remark?: string
}

export interface ScheduleTableCell {
  scheduleEntryId?: string
  courseName?: string
  teacherName?: string
  className?: string
  labName?: string
  source?: string
  status?: string
  hasConflict: boolean
  studentCount: number
}

export interface ScheduleTableRow {
  periodNumber: number
  periodName: string
  cells: (ScheduleTableCell | null)[]
}

export interface ConflictCheckResult {
  hasHardConflict: boolean
  hasSoftConflict: boolean
  hardConflicts: ConflictItem[]
  softConflicts: ConflictItem[]
  canForceSchedule: boolean
}

export interface ConflictItem {
  id: string
  type: string
  message: string
  labName?: string
  weekNumber?: number
  dayOfWeek?: number
  periodNumber?: number
}

// 预约 DTO
export interface ReservationDto {
  id: string
  semesterId: string
  semesterName?: string
  labId: string
  labName?: string
  useDate: string
  dayOfWeek: number
  periodNumbers: number[]
  weekNumber: number
  expectedDurationHours?: number
  projectName: string
  projectCategory: string
  remark?: string
  applicantId: string
  applicantName: string
  applicantPhone: string
  projectLeaderId?: string
  projectLeaderName?: string
  projectLeaderPhone?: string
  memberGrade?: string
  memberClassId?: string
  memberClassName?: string
  memberCount?: number
  status: string
  approvalComment?: string
  approvedBy?: string
  approverName?: string
  approvedAt?: string
  isCancelled: boolean
  cancelReason?: string
  createdAt: string
  createdBy?: string
}

export interface CreateReservationRequest {
  semesterId: string
  labId: string
  useDate: string
  dayOfWeek: number
  periodNumbers: number[]
  weekNumber: number
  expectedDurationHours?: number
  projectName: string
  projectCategory: string
  remark?: string
  projectLeaderId?: string
  projectLeaderName?: string
  projectLeaderPhone?: string
  memberGrade?: string
  memberClassId?: string
  memberClassName?: string
  memberCount?: number
}

export interface ReservationQuery {
  semesterId?: string
  labId?: string
  weekNumber?: number
  status?: string
  applicantId?: string
  keyword?: string
  page?: number
  pageSize?: number
}

// 授课申请 DTO
export interface TeachingApplicationDto {
  id: string
  semesterId: string
  semesterName?: string
  teachingTaskId: string
  courseName: string
  majorId: string
  majorName: string
  classId: string
  className: string
  weekNumbers: number[]
  dayOfWeek: number
  periodNumbers: number[]
  expectedLabId?: string
  expectedLabName?: string
  remark?: string
  applicantId: string
  applicantName: string
  status: string
  approvalComment?: string
  approvedBy?: string
  approverName?: string
  approvedAt?: string
  isCancelled: boolean
  cancelReason?: string
  createdAt: string
  createdBy?: string
}

export interface CreateTeachingApplicationRequest {
  semesterId: string
  teachingTaskId: string
  courseName: string
  majorId: string
  majorName: string
  classId: string
  className: string
  weekNumbers: number[]
  dayOfWeek: number
  periodNumbers: number[]
  expectedLabId?: string
  remark?: string
}

export interface TeachingApplicationQuery {
  semesterId?: string
  teachingTaskId?: string
  applicantId?: string
  status?: string
  page?: number
  pageSize?: number
}

// 使用登记 DTO
export interface UsageRegistrationDto {
  id: string
  semesterId: string
  semesterName?: string
  labId?: string
  labName: string
  buildingName?: string
  roomNumber?: string
  useDate: string
  weekNumber: number
  dayOfWeek: number
  periodNumber: number
  source: string
  scheduleEntryId?: string
  reservationId?: string
  teachingApplicationId?: string
  courseName?: string
  projectName?: string
  experimentItemName?: string
  experimentItemType?: string
  plannedHours: number
  actualHours: number
  className?: string
  expectedStudentCount?: number
  actualStudentCount?: number
  attendanceRecord?: string
  teachingCondition?: string
  equipmentCondition?: string
  status: string
  remindedAt?: string
  filledById: string
  filledByName: string
  filledAt: string
  createdAt: string
  createdBy?: string
}

export interface CreateUsageRegistrationRequest {
  semesterId: string
  labId?: string
  labName: string
  buildingName?: string
  roomNumber?: string
  useDate: string
  weekNumber: number
  dayOfWeek: number
  periodNumber: number
  source: string
  scheduleEntryId?: string
  reservationId?: string
  teachingApplicationId?: string
  experimentTaskId?: string
  teachingTaskId?: string
  courseName?: string
  projectName?: string
  experimentItemName?: string
  experimentItemType?: string
  plannedHours: number
  actualHours: number
  className?: string
  expectedStudentCount?: number
  actualStudentCount?: number
  attendanceRecord?: string
  teachingCondition?: string
  equipmentCondition?: string
}

export interface UsageRegistrationQuery {
  semesterId?: string
  labId?: string
  filledById?: string
  status?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

// 统计 DTO
export interface ScheduleStatisticsDto {
  id: string
  semesterId: string
  semesterName?: string
  buildingId?: string
  buildingName?: string
  labId?: string
  labName?: string
  weekNumber: number
  totalSlots: number
  usedSlots: number
  reservationSlots: number
  occupancyRate: number
  totalStudentCount: number
  generatedAt: string
}

export interface LabOccupancy {
  labName: string
  occupancyRate: number
  usedSlots: number
  totalSlots: number
}

export interface CategoryStat {
  category: string
  count: number
  percentage: number
}

export interface CompletionRate {
  total: number
  completed: number
  pending: number
  overdue: number
  rate: number
}

export interface DashboardData {
  today: TodaySummary
  week: WeekSummary
  occupancyTrends: OccupancyTrend[]
  labOccupancyList: LabOccupancy[]
  categoryStats: CategoryStat[]
  completionRate: CompletionRate
  alerts: AlertItem[]
}

export interface TodaySummary {
  totalLabs: number
  occupiedLabs: number
  availableLabs: number
  occupancyRate: number
  totalSchedules: number
  pendingReservations: number
  pendingRegistrations: number
}

export interface WeekSummary {
  totalSlots: number
  usedSlots: number
  occupancyRate: number
  totalStudentCount: number
  totalReservations: number
  approvedReservations: number
  totalTeachingApplications: number
  approvedTeachingApplications: number
}

export interface OccupancyTrend {
  label: string
  value: number
}

export interface AlertItem {
  type: string
  message: string
  time: string
  relatedId?: string
}
