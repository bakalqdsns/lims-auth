/**
 * 课程、专业、班级、授课任务相关类型定义
 * 与后端对应 DTO 对齐
 */

/* =============== 课程 =============== */
export interface Course {
  id: string
  name: string
  code: string
  courseType?: string
  credits?: number
  hours?: number
  theoryHours?: number
  experimentHours?: number
  category?: string
  description?: string
  departmentId?: string
  departmentName?: string
  teacherCount: number
  studentCount: number
  status: number
  createdAt: string
}

export interface CourseQuery {
  keyword?: string
  departmentId?: string
  courseType?: string
}

export interface CreateCourseRequest {
  name: string
  code: string
  courseType?: string
  credits?: number
  hours?: number
  theoryHours?: number
  experimentHours?: number
  category?: string
  description?: string
  departmentId?: string
}

export interface UpdateCourseRequest {
  name?: string
  code?: string
  courseType?: string
  credits?: number
  hours?: number
  theoryHours?: number
  experimentHours?: number
  category?: string
  description?: string
  departmentId?: string
}

/* =============== 专业 =============== */
export interface Major {
  id: string
  name: string
  code: string
  departmentId?: string
  departmentName?: string
  duration?: number
  description?: string
  classCount: number
  studentCount: number
  status: number
  createdAt: string
}

export interface MajorQuery {
  keyword?: string
  departmentId?: string
}

export interface CreateMajorRequest {
  name: string
  code: string
  departmentId?: string
  duration?: number
  description?: string
}

export interface UpdateMajorRequest {
  name?: string
  code?: string
  departmentId?: string
  duration?: number
  description?: string
}

/* =============== 班级 =============== */
export interface ClassInfo {
  id: string
  name: string
  code: string
  majorId: string
  majorName?: string
  departmentId?: string
  departmentName?: string
  grade: number
  studentCount: number
  counselorId?: string
  counselorName?: string
  teacherName?: string
  status: number
  createdAt: string
}

export interface ClassStudent {
  id: string
  username: string
  fullName: string
  studentId?: string
  gender?: string
  status: number
}

export interface ClassQuery {
  keyword?: string
  departmentId?: string
  majorId?: string
  grade?: string | number
}

export interface CreateClassRequest {
  name: string
  code: string
  majorId: string
  departmentId?: string
  grade: number
  counselorId?: string
}

export interface UpdateClassRequest {
  name?: string
  code?: string
  majorId?: string
  departmentId?: string
  grade?: number
  counselorId?: string
}

/* =============== 授课任务 =============== */
export interface TeachingTask {
  id: string
  taskNo?: string
  courseId: string
  courseName: string
  courseCode?: string
  classId: string
  className: string
  semesterId: string
  semesterName: string
  teacherId: string
  teacherName: string
  labId?: string
  labName?: string
  weeklyHours: number
  totalHours: number
  weekPattern?: string
  startWeek?: number
  endWeek?: number
  status: number
  description?: string
  createdAt: string
}

export interface TeachingTaskQuery {
  semesterId?: string
  courseId?: string
  classId?: string
  teacherId?: string
}

export interface CreateTeachingTaskRequest {
  courseId: string
  classId: string
  semesterId: string
  teacherId: string
  weeklyHours: number
  totalHours: number
  weekPattern?: string
  startWeek?: number
  endWeek?: number
  description?: string
}

export interface UpdateTeachingTaskRequest {
  weeklyHours?: number
  totalHours?: number
  weekPattern?: string
  startWeek?: number
  endWeek?: number
  description?: string
  status?: number
}

export interface AddTeachingTaskTeacherRequest {
  teacherId: string
  isMainTeacher: boolean
}

/* =============== 授课申请 =============== */
export type TeachingApplicationStatus = 'Pending' | 'Approved' | 'Rejected' | 'Cancelled'

export const TEACHING_STATUS_LABELS: Record<TeachingApplicationStatus, string> = {
  Pending: '待审批',
  Approved: '已通过',
  Rejected: '已拒绝',
  Cancelled: '已取消',
}

export interface TeachingApplication {
  id: string
  applicationNo?: string
  semesterId?: string
  semesterName?: string
  teachingTaskId?: string
  courseName: string
  majorId?: string
  majorName?: string
  classId?: string
  className: string
  startWeek: number
  endWeek: number
  dayOfWeek: number
  periodNumbers: number[]
  expectedLabId?: string
  expectedLabName?: string
  remark?: string
  applicantId: string
  applicantName: string
  /** 后端返回字符串枚举名 (Pending / Approved / Rejected) */
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

export interface TeachingApplicationQuery {
  semesterId?: string
  status?: number | string
  applicantId?: string
  keyword?: string
  page?: number
  pageSize?: number
}

/** 授课申请审批请求 (与后端 ScheduleDtos.ApprovalRequest 对齐) */
export interface TeachingApprovalRequest {
  comment?: string
  approverName?: string
}

export interface CreateTeachingApplicationRequest {
  courseId: string
  classId: string
  labId: string
  semesterId: string
  weekNumber: number
  dayOfWeek: number
  startPeriod: number
  endPeriod: number
  purpose?: string
  studentCount: number
  remark?: string
}

/* =============== 使用登记 =============== */
export interface UsageRegistration {
  id: string
  registrationNo?: string
  labId: string
  labName: string
  userId: string
  userName: string
  userPhone?: string
  semesterId?: string
  semesterName?: string
  courseId?: string
  courseName?: string
  scheduleId?: string
  date: string
  weekNumber?: number
  dayOfWeek?: number
  startPeriod?: number
  endPeriod?: number
  timeSlot?: string
  signInTime?: string
  signOutTime?: string
  actualEndTime?: string
  equipmentUsed?: string
  experimentContent?: string
  studentCount?: number
  issue?: string
  remark?: string
  status: number
  completed: boolean
  reviewedById?: string
  reviewedByName?: string
  reviewedAt?: string
  createdAt: string
}

export interface UsageRegistrationQuery {
  semesterId?: string
  userId?: string
  labId?: string
  status?: number
  startDate?: string
  endDate?: string
}

export interface CreateUsageRegistrationRequest {
  labId: string
  scheduleId?: string
  semesterId?: string
  courseId?: string
  date: string
  weekNumber?: number
  dayOfWeek?: number
  startPeriod?: number
  endPeriod?: number
  studentCount?: number
  equipmentUsed?: string
  experimentContent?: string
  issue?: string
  remark?: string
}

/* =============== 统计 =============== */
export interface DashboardStats {
  totalLabs: number
  totalEquipments: number
  pendingApprovals: number
  myReservations: number
  activeCourses: number
  overdueBorrows: number
  todayUsage: number
}

export interface LabUsageStat {
  labName: string
  usageCount: number
  usageHours: number
  reservationRate: number
}

export interface ReservationStat {
  date: string
  count: number
}

export interface WeeklySummary {
  weekLabel: string
  weekNumber: number
  reservations: number
  borrowRecords: number
  usageHours: number
  teachingHours: number
}

export interface CompletionRateStat {
  total: number
  completed: number
  rate: number
  byMajor?: { majorName: string; total: number; completed: number; rate: number }[]
  byClass?: { className: string; total: number; completed: number; rate: number }[]
  byGrade?: { grade: number; total: number; completed: number; rate: number }[]
}

export interface ClassUsageStat {
  className: string
  majorName?: string
  total: number
  completed: number
  rate: number
}

export interface CourseUsageStat {
  courseName: string
  total: number
  completed: number
  rate: number
}

export interface MajorUsageStat {
  majorName: string
  total: number
  completed: number
  rate: number
}

export interface GradeUsageStat {
  grade: number
  total: number
  completed: number
  rate: number
}
