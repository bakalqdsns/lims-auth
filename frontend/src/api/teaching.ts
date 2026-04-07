import axios from 'axios'

const API_BASE_URL = '/api/v1'

// 学期管理 API
export const semesterApi = {
  getList: (params?: { keyword?: string; isCurrent?: boolean; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/semesters`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/semesters/${id}`),
  create: (data: CreateSemesterRequest) =>
    axios.post(`${API_BASE_URL}/semesters`, data),
  update: (id: string, data: UpdateSemesterRequest) =>
    axios.put(`${API_BASE_URL}/semesters/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/semesters/${id}`),
  setCurrent: (id: string) =>
    axios.post(`${API_BASE_URL}/semesters/${id}/set-current`),
  getCurrent: () =>
    axios.get(`${API_BASE_URL}/semesters/current`),
  generateCalendar: (id: string) =>
    axios.post(`${API_BASE_URL}/semesters/${id}/generate-calendar`)
}

// 校历管理 API
export const calendarApi = {
  getBySemester: (semesterId: string) =>
    axios.get(`${API_BASE_URL}/calendar`, { params: { semesterId } }),
  generate: (semesterId: string) =>
    axios.post(`${API_BASE_URL}/calendar/generate`, { semesterId }),
  update: (id: string, data: UpdateCalendarRequest) =>
    axios.put(`${API_BASE_URL}/calendar/${id}`, data),
  getToday: () =>
    axios.get(`${API_BASE_URL}/calendar/today`),
  getByDate: (date: string) =>
    axios.get(`${API_BASE_URL}/calendar/date/${date}`),
  getWeekInfo: (semesterId: string, weekNumber: number) =>
    axios.get(`${API_BASE_URL}/calendar/week-info`, { params: { semesterId, weekNumber } })
}

// 课程管理 API
export const courseApi = {
  getList: (params?: { keyword?: string; departmentId?: string; courseType?: string; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/courses`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/courses/${id}`),
  create: (data: CreateCourseRequest) =>
    axios.post(`${API_BASE_URL}/courses`, data),
  update: (id: string, data: UpdateCourseRequest) =>
    axios.put(`${API_BASE_URL}/courses/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/courses/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/courses/${id}/status`, { isActive })
}

// 专业管理 API
export const majorApi = {
  getList: (params?: { keyword?: string; departmentId?: string; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/majors`, { params }),
  getAll: () =>
    axios.get(`${API_BASE_URL}/majors/all`),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/majors/${id}`),
  create: (data: CreateMajorRequest) =>
    axios.post(`${API_BASE_URL}/majors`, data),
  update: (id: string, data: UpdateMajorRequest) =>
    axios.put(`${API_BASE_URL}/majors/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/majors/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/majors/${id}/status`, { isActive })
}

// 班级管理 API
export const classApi = {
  getList: (params?: { keyword?: string; departmentId?: string; majorId?: string; grade?: string; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/classes`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/classes/${id}`),
  create: (data: CreateClassRequest) =>
    axios.post(`${API_BASE_URL}/classes`, data),
  update: (id: string, data: UpdateClassRequest) =>
    axios.put(`${API_BASE_URL}/classes/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/classes/${id}`),
  getStudents: (classId: string) =>
    axios.get(`${API_BASE_URL}/classes/${classId}/students`),
  addStudents: (classId: string, studentIds: string[]) =>
    axios.post(`${API_BASE_URL}/classes/${classId}/students`, { studentIds }),
  removeStudent: (classId: string, studentId: string) =>
    axios.delete(`${API_BASE_URL}/classes/${classId}/students/${studentId}`)
}

// 教学任务管理 API
export const teachingTaskApi = {
  getList: (params?: { semesterId?: string; courseId?: string; classId?: string; teacherId?: string; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/teaching-tasks`, { params }),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/teaching-tasks/${id}`),
  create: (data: CreateTeachingTaskRequest) =>
    axios.post(`${API_BASE_URL}/teaching-tasks`, data),
  update: (id: string, data: UpdateTeachingTaskRequest) =>
    axios.put(`${API_BASE_URL}/teaching-tasks/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/teaching-tasks/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/teaching-tasks/${id}/status`, { isActive }),
  addTeacher: (taskId: string, teacherId: string, isMainTeacher?: boolean) =>
    axios.post(`${API_BASE_URL}/teaching-tasks/${taskId}/teachers`, { teacherId, isMainTeacher }),
  removeTeacher: (taskId: string, teacherId: string) =>
    axios.delete(`${API_BASE_URL}/teaching-tasks/${taskId}/teachers/${teacherId}`)
}

// 节次时间管理 API
export const periodTimeApi = {
  getList: () =>
    axios.get(`${API_BASE_URL}/period-times`),
  getById: (id: string) =>
    axios.get(`${API_BASE_URL}/period-times/${id}`),
  create: (data: CreatePeriodTimeRequest) =>
    axios.post(`${API_BASE_URL}/period-times`, data),
  update: (id: string, data: UpdatePeriodTimeRequest) =>
    axios.put(`${API_BASE_URL}/period-times/${id}`, data),
  delete: (id: string) =>
    axios.delete(`${API_BASE_URL}/period-times/${id}`),
  toggleStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/period-times/${id}/status`, { isActive })
}

// 类型定义
export interface CreateSemesterRequest {
  name: string
  startDate: string
  endDate: string
  isCurrent?: boolean
}

export interface UpdateSemesterRequest {
  name?: string
  startDate?: string
  endDate?: string
  isCurrent?: boolean
  isActive?: boolean
}

export interface UpdateCalendarRequest {
  isHoliday?: boolean
  holidayName?: string
  description?: string
}

export interface CreateCourseRequest {
  code: string
  name: string
  englishName?: string
  courseType: string
  credits: number
  totalHours: number
  theoryHours: number
  practiceHours: number
  experimentHours: number
  onlineHours: number
  semesterType: number
  departmentId?: string
  managerId?: string
  description?: string
}

export interface UpdateCourseRequest extends Partial<CreateCourseRequest> {
  isActive?: boolean
}

export interface CreateMajorRequest {
  code: string
  name: string
  departmentId: string
  description?: string
}

export interface UpdateMajorRequest extends Partial<CreateMajorRequest> {
  isActive?: boolean
}

export interface CreateClassRequest {
  code: string
  name: string
  grade: string
  majorId: string
  departmentId: string
  headTeacherId?: string
  adminStudentId?: string
  description?: string
}

export interface UpdateClassRequest extends Partial<CreateClassRequest> {
  isActive?: boolean
}

export interface CreateTeachingTaskRequest {
  semesterId: string
  courseId: string
  classId: string
  taskType?: string
  description?: string
  teacherIds?: string[]
}

export interface UpdateTeachingTaskRequest extends Partial<CreateTeachingTaskRequest> {
  isActive?: boolean
}

export interface CreatePeriodTimeRequest {
  periodNumber: number
  name: string
  startTime: string
  endTime: string
  description?: string
}

export interface UpdatePeriodTimeRequest extends Partial<CreatePeriodTimeRequest> {
  isActive?: boolean
}

// DTO 类型
export interface SemesterDto {
  id: string
  name: string
  startDate: string
  endDate: string
  isCurrent: boolean
  isActive: boolean
  createdAt: string
}

export interface AcademicCalendarDto {
  id: string
  semesterId: string
  date: string
  weekNumber: number
  dayOfWeek: number
  isHoliday: boolean
  holidayName?: string
  description?: string
}

export interface CourseDto {
  id: string
  code: string
  name: string
  englishName?: string
  courseType: string
  credits: number
  totalHours: number
  theoryHours: number
  practiceHours: number
  experimentHours: number
  onlineHours: number
  semesterType: number
  departmentId?: string
  departmentName?: string
  managerId?: string
  managerName?: string
  description?: string
  isActive: boolean
  createdAt: string
}

export interface MajorDto {
  id: string
  code: string
  name: string
  departmentId: string
  departmentName?: string
  description?: string
  isActive: boolean
  createdAt: string
}

export interface ClassDto {
  id: string
  code: string
  name: string
  grade: string
  majorId: string
  majorName?: string
  departmentId: string
  departmentName?: string
  headTeacherId?: string
  headTeacherName?: string
  adminStudentId?: string
  adminStudentName?: string
  studentCount: number
  description?: string
  isActive: boolean
  createdAt: string
}

export interface TeachingTaskDto {
  id: string
  semesterId: string
  semesterName?: string
  courseId: string
  courseName?: string
  courseCode?: string
  classId: string
  className?: string
  taskType: string
  teachers: TaskTeacherDto[]
  description?: string
  isActive: boolean
  createdAt: string
  studentCount: number
}

export interface TaskTeacherDto {
  id: string
  name: string
  isMainTeacher: boolean
}

export interface PeriodTimeDto {
  id: string
  periodNumber: number
  name: string
  startTime: string
  endTime: string
  description?: string
  isActive: boolean
  createdAt: string
}

export interface WeekInfoDto {
  semesterId: string
  weekNumber: number
  startDate: string
  endDate: string
  days: CalendarDayDto[]
}

export interface CalendarDayDto {
  date: string
  dayOfWeek: number
  isHoliday: boolean
  holidayName?: string
}
