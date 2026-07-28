<template>
  <div class="central-scheduling-container">
    <div class="page-header">
      <h2>集中排课</h2>
      <el-button type="primary" @click="showImportDialog = true">
        <el-icon><Upload /></el-icon>
        从实验教学任务导入
      </el-button>
    </div>

    <!-- 导入对话框 -->
    <el-dialog v-model="showImportDialog" title="从实验教学任务导入" width="800px">
      <el-form label-width="120px">
        <el-form-item label="选择学期">
          <el-select v-model="importSemesterId" placeholder="请选择学期" style="width: 300px" @change="loadImportableTasks">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
      </el-form>
      <el-table :data="importableTasks" v-loading="tasksLoading" max-height="400" @selection-change="handleTaskSelection">
        <el-table-column type="selection" width="55" />
        <el-table-column prop="courseName" label="课程名称" />
        <el-table-column prop="className" label="班级" />
        <el-table-column prop="majorName" label="专业" />
        <el-table-column prop="scheduleCount" label="已排课次" width="100" />
        <el-table-column prop="teacherNames" label="教师" />
      </el-table>
      <template #footer>
        <el-button @click="showImportDialog = false">取消</el-button>
        <el-button type="primary" @click="handleImportTasks" :loading="importing" :disabled="selectedTaskIds.length === 0">
          导入选中任务（{{ selectedTaskIds.length }}）
        </el-button>
      </template>
    </el-dialog>

    <el-card shadow="never">
      <el-steps :active="currentStep" finish-status="success" style="margin-bottom: 24px">
        <el-step title="选择学期" />
        <el-step title="填写信息" />
        <el-step title="选择实验室" />
        <el-step title="确认提交" />
      </el-steps>

      <!-- Step 1: 选择学期 -->
      <div v-if="currentStep === 0" class="step-content">
        <el-form :model="form" label-width="120px">
          <el-form-item label="选择学期" required>
            <el-select v-model="form.semesterId" placeholder="请选择学期" style="width: 300px">
              <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
            </el-select>
          </el-form-item>
          <el-form-item label="周次范围">
            <el-select v-model="form.startWeek" placeholder="起始周" style="width: 150px">
              <el-option v-for="w in 20" :key="w" :label="w + '周'" :value="w" />
            </el-select>
            <span style="margin: 0 8px">至</span>
            <el-select v-model="form.endWeek" placeholder="结束周" style="width: 150px">
              <el-option v-for="w in 20" :key="w" :label="w + '周'" :value="w" />
            </el-select>
          </el-form-item>
        </el-form>
      </div>

      <!-- Step 2: 填写排课信息 -->
      <div v-if="currentStep === 1" class="step-content">
        <el-form :model="form" ref="formRef" label-width="120px">
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="课程名称" prop="courseId">
                <el-select v-model="form.courseId" placeholder="请选择课程" style="width: 100%" filterable @change="handleCourseChange">
                  <el-option v-for="c in courses" :key="c.id" :label="c.name" :value="c.id" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="教师">
                <el-select v-model="form.teacherId" placeholder="请选择教师" style="width: 100%" filterable @change="handleTeacherChange">
                  <el-option v-for="t in teachers" :key="t.id" :label="t.fullName || t.username" :value="t.id" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="星期" prop="dayOfWeek">
                <el-select v-model="form.dayOfWeek" placeholder="请选择" style="width: 100%">
                  <el-option v-for="d in weekDays" :key="d.value" :label="d.label" :value="d.value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="节次" prop="periodNumber">
                <el-input-number v-model="form.periodNumber" :min="1" :max="12" style="width: 100%" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="周次范围">
                <el-select v-model="form.startWeek" placeholder="起始周" style="width: 45%">
                  <el-option v-for="w in 20" :key="w" :label="w + '周'" :value="w" />
                </el-select>
                <span style="margin: 0 4px">至</span>
                <el-select v-model="form.endWeek" placeholder="结束周" style="width: 45%">
                  <el-option v-for="w in 20" :key="w" :label="w + '周'" :value="w" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="班级">
                <el-select v-model="form.classId" placeholder="请选择班级" style="width: 100%" filterable @change="handleClassChange">
                  <el-option v-for="c in classes" :key="c.id" :label="c.name" :value="c.id" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="专业">
                <el-select v-model="form.majorId" placeholder="请选择专业" style="width: 100%" filterable @change="handleMajorChange">
                  <el-option v-for="m in majors" :key="m.id" :label="m.name" :value="m.id" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="学生人数">
                <el-input-number v-model="form.studentCount" :min="0" style="width: 100%" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="备注">
                <el-input v-model="form.remark" type="textarea" :rows="2" placeholder="选填" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
      </div>

      <!-- Step 3: 选择实验室 -->
      <div v-if="currentStep === 2" class="step-content">
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="楼宇筛选">
              <el-select v-model="selectedBuildingId" clearable placeholder="全部楼宇" style="width: 100%">
                <el-option v-for="b in buildings" :key="b.id" :label="b.name" :value="b.id" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <div class="lab-selector-wrapper">
              <div class="lab-selector-header">
                <span>实验室列表</span>
                <span class="lab-count">共 {{ availableLabs.length }} 个</span>
              </div>
              <div class="lab-selector-list">
                <el-empty v-if="labsLoading" description="加载中..." />
                <el-empty v-else-if="availableLabs.length === 0" description="暂无实验室" />
                <div
                  v-for="lab in availableLabs"
                  :key="lab.id"
                  class="lab-item"
                  :class="{ 'is-selected': form.labId === lab.id }"
                  @click="handleLabSelect(lab)"
                >
                  <div class="lab-name">{{ lab.name }}</div>
                  <div class="lab-meta">
                    <span>{{ lab.labType || '实验室' }}</span>
                    <span>{{ lab.capacity }}人</span>
                  </div>
                </div>
              </div>
            </div>
          </el-col>
          <el-col :span="12">
            <div class="selected-lab-info" v-if="selectedLab">
              <div class="selected-lab-title">已选实验室</div>
              <el-descriptions :column="1" border size="small">
                <el-descriptions-item label="名称">{{ selectedLab.name }}</el-descriptions-item>
                <el-descriptions-item label="编号">{{ selectedLab.code || '-' }}</el-descriptions-item>
                <el-descriptions-item label="容纳人数">{{ selectedLab.capacity || '-' }}</el-descriptions-item>
                <el-descriptions-item label="类型">{{ selectedLab.labType || '-' }}</el-descriptions-item>
                <el-descriptions-item label="地点">{{ selectedLab.location || selectedLab.roomNumber || '-' }}</el-descriptions-item>
              </el-descriptions>
            </div>
            <el-button
              type="primary"
              :loading="checkingConflicts"
              @click="handleCheckConflicts"
              :disabled="!form.labId || !form.semesterId"
              style="margin-top: 16px"
            >
              检测冲突
            </el-button>
            <el-alert
              v-if="conflictResult"
              :type="conflictResult.hasHardConflict ? 'error' : conflictResult.hasSoftConflict ? 'warning' : 'success'"
              :title="conflictResult.hasHardConflict ? `存在 ${conflictResult.hardConflicts?.length || 0} 个硬冲突` : conflictResult.hasSoftConflict ? '存在软冲突，可强制排课' : '检测通过：无冲突'"
              style="margin-top: 12px"
              show-icon
            />
            <div v-if="conflictResult?.hasSoftConflict" style="margin-top: 8px">
              <el-checkbox v-model="form.forceSchedule">强制排课（忽略软冲突）</el-checkbox>
            </div>
          </el-col>
        </el-row>
      </div>

      <!-- Step 4: 确认提交 -->
      <div v-if="currentStep === 3" class="step-content">
        <el-alert
          v-if="conflictResult?.hasHardConflict"
          type="error"
          title="存在硬冲突，无法提交"
          style="margin-bottom: 16px"
        />
        <el-descriptions :column="2" border>
          <el-descriptions-item label="学期">{{ selectedSemesterName }}</el-descriptions-item>
          <el-descriptions-item label="周次范围">第{{ form.startWeek }}-{{ form.endWeek }}周</el-descriptions-item>
          <el-descriptions-item label="星期">{{ weekDays.find(d => d.value === form.dayOfWeek)?.label }}</el-descriptions-item>
          <el-descriptions-item label="节次">第{{ form.periodNumber }}节</el-descriptions-item>
          <el-descriptions-item label="课程名称">{{ form.courseName }}</el-descriptions-item>
          <el-descriptions-item label="教师">{{ form.teacherName || '-' }}</el-descriptions-item>
          <el-descriptions-item label="班级">{{ form.className || '-' }}</el-descriptions-item>
          <el-descriptions-item label="人数">{{ form.studentCount || '-' }}</el-descriptions-item>
          <el-descriptions-item label="实验室">{{ selectedLab?.name || '-' }}</el-descriptions-item>
          <el-descriptions-item label="备注">{{ form.remark || '-' }}</el-descriptions-item>
        </el-descriptions>
      </div>

      <div class="step-actions">
        <el-button v-if="currentStep > 0" @click="currentStep--">上一步</el-button>
        <el-button v-if="currentStep < 3" type="primary" @click="handleNext">下一步</el-button>
        <el-button
          v-if="currentStep === 3"
          type="primary"
          @click="handleSubmit"
          :loading="submitting"
          :disabled="conflictResult?.hasHardConflict"
        >
          确认提交
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage, type FormInstance } from 'element-plus'
import { Upload } from '@element-plus/icons-vue'
import { courseApi, type CourseDto, semesterApi, majorApi, classApi } from '@/api/teaching'
import { scheduleApi } from '@/api/schedule'

const authHeaders = () => ({
  'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
})

const currentStep = ref(0)
const semesters = ref<{ id: string; name: string }[]>([])
const buildings = ref<{ id: string; name: string }[]>([])
const availableLabs = ref<any[]>([])
const labsLoading = ref(false)
const checkingConflicts = ref(false)
const submitting = ref(false)
const selectedBuildingId = ref<string>('')
const conflictResult = ref<any>(null)
const formRef = ref<FormInstance>()

const showImportDialog = ref(false)
const importSemesterId = ref<string>('')
const importableTasks = ref<any[]>([])
const tasksLoading = ref(false)
const importing = ref(false)
const selectedTaskIds = ref<string[]>([])

const teachers = ref<any[]>([])
const classes = ref<any[]>([])
const majors = ref<any[]>([])
const courses = ref<CourseDto[]>([])

const form = reactive({
  semesterId: null as string | null,
  labId: undefined as string | undefined,
  startWeek: 1,
  endWeek: 1,
  dayOfWeek: 1,
  periodNumber: 1,
  courseName: '',
  courseId: '',
  teacherId: '',
  teacherName: '',
  className: '',
  majorId: '',
  majorName: '',
  classId: '',
  studentCount: 0,
  remark: '',
  forceSchedule: false
})

const weekDays = [
  { label: '周一', value: 1 },
  { label: '周二', value: 2 },
  { label: '周三', value: 3 },
  { label: '周四', value: 4 },
  { label: '周五', value: 5 },
  { label: '周六', value: 6 },
  { label: '周日', value: 7 }
]

const selectedSemesterName = computed(() => semesters.value.find(s => s.id === form.semesterId)?.name || '')
const selectedLab = computed(() => availableLabs.value.find(l => l.id === form.labId))

const handleCourseChange = (courseId: string) => {
  const course = courses.value.find(c => c.id === courseId)
  if (course) {
    form.courseName = course.name
  }
}

const handleTeacherChange = (teacherId: string) => {
  const teacher = teachers.value.find(t => t.id === teacherId)
  if (teacher) {
    form.teacherName = teacher.fullName || teacher.username
  }
}

const handleClassChange = (classId: string) => {
  const cls = classes.value.find(c => c.id === classId)
  if (cls) {
    form.className = cls.name
  }
}

const handleMajorChange = (majorId: string) => {
  const major = majors.value.find(m => m.id === majorId)
  if (major) {
    form.majorName = major.name
  }
}

const handleLabSelect = (lab: any) => {
  form.labId = lab.id === form.labId ? undefined : lab.id
  conflictResult.value = null
}

const handleCheckConflicts = async () => {
  if (!form.labId || !form.semesterId) return
  checkingConflicts.value = true
  conflictResult.value = null
  try {
    const res = await fetch('/api/v1/schedules/check-conflicts', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({
        semesterId: form.semesterId,
        labId: form.labId,
        startWeek: form.startWeek,
        endWeek: form.endWeek,
        dayOfWeek: form.dayOfWeek,
        periodNumber: form.periodNumber
      })
    }).then(r => r.json())
    if (res.code === 200) {
      conflictResult.value = res.data
    }
  } catch {
    ElMessage.error('冲突检测失败')
  } finally {
    checkingConflicts.value = false
  }
}

const loadAvailableLabs = async () => {
  if (!form.semesterId) return
  
  labsLoading.value = true
  try {
    const params = new URLSearchParams({
      semesterId: form.semesterId,
      startWeek: String(form.startWeek),
      endWeek: String(form.endWeek),
      dayOfWeek: String(form.dayOfWeek),
      periodNumbers: String(form.periodNumber || 1)
    })
    if (selectedBuildingId.value) params.append('buildingId', selectedBuildingId.value)
    
    console.log('请求可用实验室，参数:', params.toString())
    const res = await fetch(`/api/v1/schedules/available-labs?${params}`, {
      headers: authHeaders()
    }).then(r => r.json())
    console.log('可用实验室响应:', res)
    if (res.code === 200) {
      availableLabs.value = res.data || []
      console.log('可用实验室数量:', availableLabs.value.length)
    }
  } catch (error) {
    console.error('获取可用实验室失败:', error)
    ElMessage.error('获取实验室列表失败')
  } finally {
    labsLoading.value = false
  }
}

watch(selectedBuildingId, () => {
  if (currentStep.value >= 2) {
    loadAvailableLabs()
  }
})

const handleNext = async () => {
  if (currentStep.value === 0 && !form.semesterId) {
    ElMessage.warning('请选择学期')
    return
  }
  if (currentStep.value === 1 && !form.courseId) {
    ElMessage.warning('请选择课程')
    return
  }
  if (currentStep.value === 2) {
    await loadAvailableLabs()
  }
  if (currentStep.value === 0) {
    await loadAvailableLabs()
  }
  currentStep.value++
}

const handleSubmit = async () => {
  submitting.value = true
  try {
    const startWeek = form.startWeek
    const endWeek = form.endWeek || form.startWeek
    
    if (startWeek > endWeek) {
      ElMessage.warning('起始周不能大于结束周')
      submitting.value = false
      return
    }
    
    const res = await fetch('/api/v1/schedules', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({
        semesterId: form.semesterId,
        labId: form.labId || undefined,
        weekNumber: startWeek,
        startWeek: startWeek,
        endWeek: endWeek,
        dayOfWeek: form.dayOfWeek,
        periodNumber: form.periodNumber,
        courseId: form.courseId || undefined,
        courseName: form.courseName,
        teacherId: form.teacherId || undefined,
        teacherName: form.teacherName,
        classId: form.classId || undefined,
        className: form.className,
        majorId: form.majorId || undefined,
        majorName: form.majorName,
        studentCount: form.studentCount || undefined,
        remark: form.remark,
        forceSchedule: form.forceSchedule
      })
    }).then(r => r.json())
    
    if (res.code === 200) {
      ElMessage.success(`排课成功，周次范围: 第${startWeek}-${endWeek}周`)
      currentStep.value = 0
      Object.assign(form, {
        semesterId: semesters.value[0]?.id || null,
        labId: undefined,
        startWeek: 1,
        endWeek: 1,
        dayOfWeek: 1,
        periodNumber: 1,
        courseName: '',
        courseId: '',
        teacherId: '',
        teacherName: '',
        className: '',
        majorId: '',
        majorName: '',
        classId: '',
        studentCount: 0,
        remark: '',
        forceSchedule: false
      })
      conflictResult.value = null
    } else {
      ElMessage.error(res.message || '提交失败')
    }
  } catch (err) {
    console.error('提交错误:', err)
    ElMessage.error('提交失败: ' + (err instanceof Error ? err.message : String(err)))
  } finally {
    submitting.value = false
  }
}

const loadImportableTasks = async () => {
  if (!importSemesterId.value) return
  tasksLoading.value = true
  try {
    const res = await fetch(`/api/v1/schedules/importable-tasks?semesterId=${importSemesterId.value}`, {
      headers: authHeaders()
    }).then(r => r.json())
    if (res.code === 200) {
      importableTasks.value = res.data || []
    }
  } catch {
    ElMessage.error('获取可导入任务失败')
  } finally {
    tasksLoading.value = false
  }
}

const handleTaskSelection = (selection: any[]) => {
  selectedTaskIds.value = selection.map(t => t.id)
}

const handleImportTasks = async () => {
  if (selectedTaskIds.value.length === 0) return
  importing.value = true
  try {
    const res = await fetch('/api/v1/schedules/import-from-tasks', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({ taskIds: selectedTaskIds.value })
    }).then(r => r.json())
    if (res.code === 200) {
      ElMessage.success(`成功导入 ${res.data} 条排课记录`)
      showImportDialog.value = false
      selectedTaskIds.value = []
      importableTasks.value = []
    } else {
      ElMessage.error(res.message || '导入失败')
    }
  } catch {
    ElMessage.error('导入失败')
  } finally {
    importing.value = false
  }
}

onMounted(async () => {
  try {
    const [semRes, buildRes, teacherRes, classRes, majorRes, courseRes] = await Promise.all([
      semesterApi.getList(),
      fetch('/api/v1/buildings', { headers: authHeaders() }).then(r => r.json()),
      fetch('/api/v1/users?page=1&pageSize=1000', { headers: authHeaders() }).then(r => r.json()),
      classApi.getList(),
      majorApi.getList(),
      courseApi.getList()
    ])
    
    console.log('学期数据:', semRes)
    console.log('楼宇数据:', buildRes)
    console.log('教师数据:', teacherRes)
    console.log('班级数据:', classRes)
    console.log('专业数据:', majorRes)
    console.log('课程数据:', courseRes)
    
    if (semRes.data?.code === 200) {
      semesters.value = semRes.data.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) form.semesterId = current.id
    }
    if (buildRes.code === 200) {
      buildings.value = buildRes.data || []
    }
    if (teacherRes.code === 200) {
      const allUsers = teacherRes.data?.items || teacherRes.data || []
      teachers.value = allUsers.filter((u: any) => u.roles?.some((r: any) => r.code?.toLowerCase() === 'teacher' || r.name?.includes('教师')))
    }
    if (classRes.data?.code === 200) {
      classes.value = classRes.data.data || []
    }
    if (majorRes.data?.code === 200) {
      majors.value = majorRes.data.data || []
    }
    if (courseRes.data?.code === 200) {
      courses.value = courseRes.data.data || []
    }
  } catch (err) {
    console.error('加载数据失败:', err)
    ElMessage.error('加载数据失败: ' + (err instanceof Error ? err.message : String(err)))
  }
})
</script>

<style scoped>
.central-scheduling-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.step-content {
  min-height: 300px;
  padding: 20px 0;
}

.step-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 24px;
  border-top: 1px solid #ebeef5;
  padding-top: 16px;
}

.lab-selector-wrapper {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  overflow: hidden;
}

.lab-selector-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #f5f7fa;
  border-bottom: 1px solid #ebeef5;
  font-weight: 600;
}

.lab-count {
  font-size: 12px;
  color: #909399;
  font-weight: normal;
}

.lab-selector-list {
  max-height: 400px;
  overflow-y: auto;
  padding: 8px;
}

.lab-item {
  padding: 10px 12px;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  margin-bottom: 8px;
  cursor: pointer;
  transition: all 0.15s;
}

.lab-item:hover {
  background: #f0f9ff;
  border-color: #409eff;
}

.lab-item.is-selected {
  background: #ecf5ff;
  border-color: #409eff;
}

.lab-name {
  font-weight: 600;
  color: #303133;
  margin-bottom: 4px;
}

.lab-meta {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: #909399;
}

.selected-lab-info {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 12px;
  background: #fafafa;
}

.selected-lab-title {
  font-weight: 600;
  color: #409eff;
  margin-bottom: 12px;
}
</style>
