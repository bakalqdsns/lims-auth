/**
 * 课程、专业、班级、授课任务相关类型定义
 * 与 Flutter lib/models/course.dart, major.dart, class_model.dart, teaching_task.dart 对齐
 */

export interface Course {
  id: number
  name: string
  code: string
  credits: number
  category?: string
  departmentId?: number
  departmentName?: string
  teacherCount: number
  studentCount: number
  status: number
}

export interface Major {
  id: number
  name: string
  code: string
  departmentId?: number
  departmentName?: string
  classCount: number
  studentCount: number
  status: number
}

export interface ClassInfo {
  id: number
  name: string
  code: string
  majorId: number
  majorName: string
  grade: number
  studentCount: number
  teacherName?: string
  status: number
}

export interface TeachingTask {
  id: number
  courseId: number
  courseName: string
  courseCode: string
  classId: number
  className: string
  semesterId: number
  semesterName: string
  teacherId: number
  teacherName: string
  labId?: number
  labName?: string
  weeklyHours: number
  totalHours: number
  weekPattern: string
  status: number
  description?: string
  createdAt: string
}

export interface TeachingApplication {
  id: number
  applicationNo: string
  teacherId: number
  teacherName: string
  courseId: number
  courseName: string
  classId: number
  className: string
  labId: number
  labName: string
  semesterId: number
  semesterName: string
  date: string
  timeSlot: string
  purpose?: string
  studentCount: number
  status: number
  approverId?: number
  approverName?: string
  approvedAt?: string
  remark?: string
  createdAt: string
}

export interface UsageRegistration {
  id: number
  registrationNo: string
  labId: number
  labName: string
  userId: number
  userName: string
  userPhone?: string
  date: string
  timeSlot: string
  signInTime?: string
  signOutTime?: string
  actualEndTime?: string
  equipmentUsed?: string
  experimentContent?: string
  remark?: string
  status: number
  completed: boolean
  createdAt: string
}

export interface StatisticsData {
  dashboard: DashboardStats
  labUsage: LabUsageStat[]
  reservationStat: ReservationStat[]
  weeklySummary: WeeklySummary[]
  completionRate: CompletionRateStat
}

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
  reservationRate: number
}

export interface ReservationStat {
  date: string
  count: number
}

export interface WeeklySummary {
  weekLabel: string
  reservations: number
  borrowRecords: number
  usageHours: number
}

export interface CompletionRateStat {
  total: number
  completed: number
  rate: number
}
