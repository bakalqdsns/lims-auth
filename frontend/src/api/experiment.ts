import axios from 'axios'

const API_BASE_URL = '/api/experiments'
const EXPORT_BASE_URL = '/api/export/experiment'

export const experimentApi = {
  getTasks: (params?: { semesterId?: string; majorId?: string; classId?: string }) =>
    axios.get(`${API_BASE_URL}/tasks`, { params }),
  getTaskById: (id: string) =>
    axios.get(`${API_BASE_URL}/tasks/${id}`),
  createTask: (data: ExperimentTaskRequest) =>
    axios.post(`${API_BASE_URL}/tasks`, data),
  updateTask: (id: string, data: ExperimentTaskRequest) =>
    axios.put(`${API_BASE_URL}/tasks/${id}`, data),
  deleteTask: (id: string) =>
    axios.delete(`${API_BASE_URL}/tasks/${id}`),

  getItems: (params?: { courseCode?: string; experimentType?: string }) =>
    axios.get(`${API_BASE_URL}/items`, { params }),
  createItem: (data: ExperimentItemRequest) =>
    axios.post(`${API_BASE_URL}/items`, data),
  updateItem: (id: string, data: ExperimentItemRequest) =>
    axios.put(`${API_BASE_URL}/items/${id}`, data),
  deleteItem: (id: string) =>
    axios.delete(`${API_BASE_URL}/items/${id}`),

  getSchedules: (params?: { taskId?: string; weekNumber?: number }) =>
    axios.get(`${API_BASE_URL}/schedules`, { params }),
  createSchedule: (data: ExperimentScheduleRequest) =>
    axios.post(`${API_BASE_URL}/schedules`, data),
  updateSchedule: (id: string, data: ExperimentScheduleRequest) =>
    axios.put(`${API_BASE_URL}/schedules/${id}`, data),
  deleteSchedule: (id: string) =>
    axios.delete(`${API_BASE_URL}/schedules/${id}`),

  getQualityList: () =>
    axios.get(`${API_BASE_URL}/quality`),
  createQuality: (data: ExperimentQualityRequest) =>
    axios.post(`${API_BASE_URL}/quality`, data),
  updateQuality: (id: string, data: ExperimentQualityRequest) =>
    axios.put(`${API_BASE_URL}/quality/${id}`, data),
  deleteQuality: (id: string) =>
    axios.delete(`${API_BASE_URL}/quality/${id}`),

  getTrainingPlans: (params?: { courseId?: string; status?: string; approvalStatus?: string }) =>
    axios.get(`${API_BASE_URL}/training-plans`, { params }),
  createTrainingPlan: (data: TrainingPlanRequest) =>
    axios.post(`${API_BASE_URL}/training-plans`, data),
  updateTrainingPlan: (id: string, data: TrainingPlanRequest) =>
    axios.put(`${API_BASE_URL}/training-plans/${id}`, data),
  deleteTrainingPlan: (id: string) =>
    axios.delete(`${API_BASE_URL}/training-plans/${id}`)
}

export const exportApi = {
  exportTaskList: (params?: { semesterId?: string; majorId?: string; classId?: string }) =>
    axios.get(`${EXPORT_BASE_URL}/task-list`, { params, responseType: 'blob' }),
  exportSchedulePlan: (params?: { semesterId?: string; majorId?: string; classId?: string }) =>
    axios.get(`${EXPORT_BASE_URL}/schedule-plan`, { params, responseType: 'blob' })
}

export interface ExperimentTaskDto {
  id: string
  semesterId: string
  majorId: string
  classId: string
  studentCount: number
  studentLevel?: string
  courseName: string
  courseType?: string
  isIndependentCourse: boolean
  totalExperimentHours: number
  currentSemesterExperimentHours: number
  totalPracticeHours: number
  currentSemesterPracticeHours: number
  totalTrainingHours: number
  currentSemesterTrainingHours: number
  institutionId?: string
  departmentId?: string
  teacherIds?: string
  teacherNames?: string
  teacherTitles?: string
  technicalStaff?: string
  technicalTitle?: string
  textbookName?: string
  experimentGuideName?: string
  status: string
  sortOrder: number
  description?: string
  semester?: { id: string; name: string }
  major?: { id: string; name: string }
  class?: { id: string; name: string }
}

export type ExperimentTaskRequest = Omit<ExperimentTaskDto, 'id' | 'semester' | 'major' | 'class'>

export interface ExperimentItemDto {
  id: string
  courseCode: string
  experimentName: string
  experimentHours: number
  experimentType?: string
  experimentRequirement?: string
  status: string
  sortOrder: number
  description?: string
}

export type ExperimentItemRequest = Omit<ExperimentItemDto, 'id'>

export interface ExperimentScheduleDto {
  id: string
  experimentTaskId: string
  experimentItemId: string
  weekNumber?: number
  dayOfWeek?: number
  periodNumber?: number
  parallelGroups?: number
  studentsPerGroup?: number
  cycleCount?: number
  experimentRequirement?: string
  location?: string
  labId?: string
  isConducted: boolean
  reasonIfNotConducted?: string
  status: string
  sortOrder: number
  description?: string
  experimentTask?: { id: string; courseName: string }
  experimentItem?: { id: string; experimentName: string }
  lab?: {
    id: string
    code: string
    name: string
    buildingId?: string
    roomNumber?: string
    location?: string
    building?: { id: string; name: string; campus?: { id: string; name: string } }
  }
}

export type ExperimentScheduleRequest = Omit<ExperimentScheduleDto, 'id' | 'experimentTask' | 'experimentItem' | 'lab'>

export interface ExperimentQualityDto {
  id: string
  experimentTaskId: string
  institutionId?: string
  courseName: string
  experimentHours: number
  isIndependentCourse: boolean
  mainTeacher?: string
  teacherTitle?: string
  technicalStaff?: string
  technicalTitle?: string
  className?: string
  classStudentCount: number
  plannedExperimentCount: number
  actualExperimentCount: number
  missedExperimentItems?: string
  assessmentMethod?: string
  assessmentStudentCount?: number
  assessmentTime?: string
  status: string
  sortOrder: number
  description?: string
  experimentTask?: { id: string; courseName: string }
  institution?: { id: string; name: string }
}

export type ExperimentQualityRequest = Omit<ExperimentQualityDto, 'id' | 'experimentTask' | 'institution'>

export interface TrainingPlanDto {
  id: string
  semesterId: string
  courseId: string
  courseName?: string
  courseCode?: string
  majorId?: string
  classId?: string
  studentCount: number
  studentLevel?: string
  teachingOrganizationMethod?: string
  teachingLocation?: string
  teachingPurpose?: string
  teachingRequirements?: string
  teachingContent?: string
  teachingProgressSchedule?: string
  trainingMethod?: string
  cycleGroupInfo?: string
  assessmentMethod?: string
  assessmentRequirements?: string
  qualityAssuranceMeasures?: string
  qualityAssuranceDetails?: string
  experimentCenterOpinion?: string
  experimentCenterOpinionStatus?: string
  experimentCenterApprovedBy?: string
  experimentCenterApprovalDate?: string
  departmentOpinion?: string
  departmentOpinionStatus?: string
  departmentApprovedBy?: string
  departmentApprovalDate?: string
  status: string
  sortOrder: number
  description?: string
  updatedAt?: string
  course?: { id: string; code: string; name: string }
  semester?: { id: string; name: string }
  major?: { id: string; name: string }
  class?: { id: string; name: string }
}

export type TrainingPlanRequest = Omit<TrainingPlanDto, 'id' | 'course' | 'semester' | 'major' | 'class' | 'updatedAt'>

