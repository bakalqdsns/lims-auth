<template>
  <div class="central-scheduling-container">
    <div class="page-header">
      <h2>集中排课</h2>
    </div>

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
          <el-form-item label="教学周">
            <el-input-number v-model="form.weekNumber" :min="1" :max="20" style="width: 200px" />
          </el-form-item>
        </el-form>
      </div>

      <!-- Step 2: 填写排课信息 -->
      <div v-if="currentStep === 1" class="step-content">
        <el-form :model="form" ref="formRef" label-width="120px">
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="课程名称" prop="courseName">
                <el-input v-model="form.courseName" placeholder="请输入课程名称" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="课程编号">
                <el-input v-model="form.courseId" placeholder="请输入课程编号" />
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
              <el-form-item label="周次">
                <el-input-number v-model="form.weekNumber" :min="1" :max="20" style="width: 100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="教师姓名">
                <el-input v-model="form.teacherName" placeholder="请输入教师姓名" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="班级名称">
                <el-input v-model="form.className" placeholder="请输入班级名称" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="专业名称">
                <el-input v-model="form.majorName" placeholder="请输入专业名称" />
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
          <el-descriptions-item label="教学周">第{{ form.weekNumber }}周</el-descriptions-item>
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
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, type FormInstance } from 'element-plus'

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

const form = reactive({
  semesterId: null as string | null,
  labId: undefined as string | undefined,
  weekNumber: 1,
  dayOfWeek: 1,
  periodNumber: 1,
  courseName: '',
  courseId: '',
  teacherName: '',
  className: '',
  majorName: '',
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
        weekNumber: form.weekNumber,
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

const handleNext = async () => {
  if (currentStep.value === 1 && !form.courseName) {
    ElMessage.warning('请输入课程名称')
    return
  }
  if (currentStep.value === 2 && form.labId && form.semesterId) {
    labsLoading.value = true
    try {
      const params = new URLSearchParams({
        semesterId: form.semesterId,
        weekNumber: String(form.weekNumber),
        dayOfWeek: String(form.dayOfWeek),
        periodNumbers: String(form.periodNumber)
      })
      if (selectedBuildingId.value) params.append('buildingId', selectedBuildingId.value)
      const res = await fetch(`/api/v1/schedules/available-labs?${params}`, {
        headers: authHeaders()
      }).then(r => r.json())
      if (res.code === 200) {
        availableLabs.value = res.data || []
      }
    } catch {}
    labsLoading.value = false
  }
  currentStep.value++
}

const handleSubmit = async () => {
  submitting.value = true
  try {
    const res = await fetch('/api/v1/schedules', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({
        ...form,
        labId: form.labId || undefined
      })
    }).then(r => r.json())
    if (res.code === 200) {
      ElMessage.success('排课成功')
      currentStep.value = 0
      Object.assign(form, {
        semesterId: semesters.value[0]?.id || null,
        labId: undefined,
        weekNumber: 1,
        dayOfWeek: 1,
        periodNumber: 1,
        courseName: '',
        courseId: '',
        teacherName: '',
        className: '',
        majorName: '',
        studentCount: 0,
        remark: '',
        forceSchedule: false
      })
      conflictResult.value = null
    } else {
      ElMessage.error(res.message || '提交失败')
    }
  } catch {
    ElMessage.error('提交失败')
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  try {
    const [semRes, buildRes] = await Promise.all([
      fetch('/api/v1/semesters', { headers: authHeaders() }).then(r => r.json()),
      fetch('/api/v1/buildings', { headers: authHeaders() }).then(r => r.json())
    ])
    if (semRes.code === 200) {
      semesters.value = semRes.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) form.semesterId = current.id
    }
    if (buildRes.code === 200) {
      buildings.value = buildRes.data || []
    }
  } catch {}
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
