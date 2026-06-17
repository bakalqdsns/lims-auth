/**
 * 实验教学相关类型
 * 与后端 ExperimentsController 实体对齐
 */

export interface ExperimentTeachingTask {
  id: string
  semesterId: string
  semester?: { id: string; name: string }
  courseId: string
  courseName: string
  courseCode?: string
  majorId?: string
  major?: { id: string; name: string }
  classId?: string
  class?: { id: string; name: string }
  departmentId?: string
  department?: { id: string; name: string }
  institutionId?: string
  institution?: { id: string; name: string }
  teacherId?: string
  teacherName?: string
  experimentType?: string
  studentCount?: number
  hours?: number
  weekHours?: number
  startWeek?: number
  endWeek?: number
  description?: string
  status: number
  createdAt: string
  updatedAt?: string
  schedules?: ExperimentItemSchedule[]
  qualityAssessment?: ExperimentQualityAssessment
}

export interface ExperimentItem {
  id: string
  courseCode: string
  courseName?: string
  experimentCode: string
  experimentName: string
  experimentType?: string
  experimentPurpose?: string
  experimentContent?: string
  hours?: number
  studentCount?: number
  deviceCount?: number
  safetyLevel?: string
  isRequired: boolean
  sortOrder: number
  createdAt: string
  updatedAt?: string
}

export interface ExperimentItemSchedule {
  id: string
  experimentTaskId: string
  experimentItemId: string
  experimentItem?: ExperimentItem
  experimentTask?: ExperimentTeachingTask
  labId?: string
  lab?: {
    id: string
    name: string
    code: string
    building?: { id: string; name: string; campus?: { id: string; name: string } }
  }
  weekNumber: number
  dayOfWeek: number
  startPeriod: number
  endPeriod: number
  studentCount?: number
  status: number
  remark?: string
  createdAt: string
  updatedAt?: string
}

export interface ExperimentQualityAssessment {
  id: string
  experimentTaskId: string
  experimentTask?: ExperimentTeachingTask
  institutionId?: string
  institution?: { id: string; name: string }
  assessorId?: string
  assessorName?: string
  assessmentDate?: string
  contentCompletion?: number
  qualityScore?: number
  studentPerformance?: string
  issues?: string
  suggestions?: string
  status: number
  createdAt: string
  updatedAt?: string
}

export type TrainingPlanApprovalType = 'ExperimentCenter' | 'Department'
export type TrainingPlanOpinionStatus = 'Pending' | 'Approved' | 'Rejected'

export interface TrainingTeachingPlan {
  id: string
  title: string
  courseId: string
  course?: { id: string; name: string; code: string }
  semesterId: string
  semester?: { id: string; name: string }
  majorId?: string
  major?: { id: string; name: string }
  classId?: string
  class?: { id: string; name: string }
  teacherId?: string
  teacherName?: string
  trainingType?: string
  trainingPlace?: string
  startDate?: string
  endDate?: string
  studentCount?: number
  content?: string
  objectives?: string
  assessment?: string
  status: string
  experimentCenterOpinion?: string
  experimentCenterOpinionStatus?: TrainingPlanOpinionStatus
  experimentCenterApprovedBy?: string
  experimentCenterApprovalDate?: string
  departmentOpinion?: string
  departmentOpinionStatus?: TrainingPlanOpinionStatus
  departmentApprovedBy?: string
  departmentApprovalDate?: string
  createdAt: string
  updatedAt?: string
}

export interface ApproveTrainingPlanRequest {
  approvalType: TrainingPlanApprovalType
  opinion: string
  status: TrainingPlanOpinionStatus
  approver?: string
}
