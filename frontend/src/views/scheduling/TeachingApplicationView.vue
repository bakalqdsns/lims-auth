<template>
  <div class="teaching-application-container">
    <div class="page-header">
      <h2>授课申请</h2>
      <div class="header-actions">
        <el-button type="primary" @click="handleOpenDialog">
          <el-icon><Plus /></el-icon>提交申请
        </el-button>
      </div>
    </div>

    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="学期">
          <el-select v-model="queryForm.semesterId" placeholder="请选择学期" style="width:200px" @change="fetchList">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryForm.status" clearable placeholder="全部" style="width:120px" @change="fetchList">
            <el-option label="待审批" value="Pending" />
            <el-option label="已通过" value="Approved" />
            <el-option label="已驳回" value="Rejected" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchList"><el-icon><Search /></el-icon>搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never" v-loading="loading">
      <el-table :data="listData" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="courseName" label="课程名称" min-width="130" show-overflow-tooltip />
        <el-table-column prop="majorName" label="专业" min-width="120" show-overflow-tooltip />
        <el-table-column prop="className" label="班级" min-width="150" show-overflow-tooltip />
        <el-table-column label="上课时间" width="220">
          <template #default="{ row }">
            <span>周{{ formatDayOfWeek(row.dayOfWeek) }} 第{{ (row.periodNumbers || []).join('、') }}节</span>
            <br />
            <span style="font-size:12px;color:#909399">第{{ row.startWeek }}周 ~ 第{{ row.endWeek }}周</span>
          </template>
        </el-table-column>
        <el-table-column prop="expectedLabName" label="期望实验室" width="130" show-overflow-tooltip />
        <el-table-column prop="applicantName" label="申请人" width="90" />
        <el-table-column label="状态" width="85">
          <template #default="{ row }">
            <el-tag v-if="row.status === 'Pending'" type="warning" size="small">待审批</el-tag>
            <el-tag v-else-if="row.status === 'Approved'" type="success" size="small">已通过</el-tag>
            <el-tag v-else-if="row.status === 'Rejected'" type="danger" size="small">已驳回</el-tag>
            <el-tag v-else size="small">{{ row.status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleView(row)">详情</el-button>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-if="total > 0"
          v-model:current-page="queryForm.page"
          v-model:page-size="queryForm.pageSize"
          :page-sizes="[10, 20, 50]"
          :total="total"
          layout="total, sizes, prev, pager, next"
          @size-change="fetchList"
          @current-change="fetchList"
        />
      </div>
    </el-card>

    <!-- 提交申请对话框 -->
    <el-dialog v-model="dialogVisible" title="提交授课申请" width="700px" destroy-on-close>
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="110px">
        <el-form-item label="学期" prop="semesterId">
          <el-select v-model="form.semesterId" placeholder="请选择学期" style="width:100%">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>

        <el-form-item label="实验教学任务" prop="teachingTaskId">
          <el-select v-model="form.teachingTaskId" placeholder="请选择实验教学任务" style="width:100%" @change="handleTaskChange">
            <el-option
              v-for="t in experimentTasks"
              :key="t.id"
              :label="`${t.courseName || ''} / ${t.className || t.class?.name || ''}`"
              :value="t.id"
            />
          </el-select>
        </el-form-item>

        <el-divider content-position="left">
          <span style="font-size:12px;color:#909399">以下信息自动填充</span>
        </el-divider>

        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="上课课程">
              <el-input :model-value="form.courseName" readonly placeholder="选择任务后自动填充" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="专业">
              <el-input :model-value="form.majorName" readonly placeholder="选择任务后自动填充" />
            </el-form-item>
          </el-col>
            <el-col :span="8">
            <el-form-item label="班级">
              <el-input :model-value="form.className" readonly placeholder="选择任务后自动填充" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-divider content-position="left">
          <span style="font-size:12px;color:#909399">填写上课安排</span>
        </el-divider>

        <el-row :gutter="12">
          <el-col :span="8">
            <el-form-item label="星期" prop="dayOfWeek">
              <el-select v-model="form.dayOfWeek" placeholder="请选择" style="width:100%">
                <el-option v-for="d in weekDays" :key="d.value" :label="d.label" :value="d.value" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="节次" prop="periodNumbers">
              <el-select v-model="form.periodNumbers" multiple placeholder="选择节次" style="width:100%">
                <el-option v-for="p in 12" :key="p" :label="`第${p}节`" :value="p" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="起始周" prop="startWeek">
              <el-input-number v-model="form.startWeek" :min="1" :max="20" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="结束周" prop="endWeek">
              <el-input-number v-model="form.endWeek" :min="form.startWeek" :max="20" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="期望实验室">
              <el-select v-model="form.expectedLabId" clearable placeholder="可选" style="width:100%">
                <el-option v-for="l in labs" :key="l.id" :label="l.name" :value="l.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="备注">
              <el-input v-model="form.remark" placeholder="选填" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">提交</el-button>
      </template>
    </el-dialog>

    <!-- 详情对话框 -->
    <el-dialog v-model="detailVisible" title="授课申请详情" width="580px" destroy-on-close>
      <el-descriptions v-if="currentRow" :column="2" border>
        <el-descriptions-item label="课程名称">{{ currentRow.courseName }}</el-descriptions-item>
        <el-descriptions-item label="专业">{{ currentRow.majorName }}</el-descriptions-item>
        <el-descriptions-item label="班级">{{ currentRow.className }}</el-descriptions-item>
        <el-descriptions-item label="上课时间" :span="2">
          周{{ formatDayOfWeek(currentRow.dayOfWeek) }} 第{{ (currentRow.periodNumbers || []).join('、') }}节
        </el-descriptions-item>
        <el-descriptions-item label="教学周次" :span="2">
          第{{ currentRow.startWeek }}周 ~ 第{{ currentRow.endWeek }}周
        </el-descriptions-item>
        <el-descriptions-item label="期望实验室">{{ currentRow.expectedLabName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="申请人">{{ currentRow.applicantName }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag v-if="currentRow.status === 'Pending'" type="warning" size="small">待审批</el-tag>
          <el-tag v-else-if="currentRow.status === 'Approved'" type="success" size="small">已通过</el-tag>
          <el-tag v-else-if="currentRow.status === 'Rejected'" type="danger" size="small">已驳回</el-tag>
          <span v-else>{{ currentRow.status }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="审批意见" :span="2">{{ currentRow.approvalComment || '-' }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{ currentRow.remark || '-' }}</el-descriptions-item>
        <el-descriptions-item label="申请时间" :span="2">{{ formatDate(currentRow.createdAt) }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { teachingApplicationApi } from '@/api/schedule'
import type { TeachingApplicationDto, CreateTeachingApplicationRequest } from '@/api/schedule'

const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const detailVisible = ref(false)
const semesters = ref<any[]>([])
const experimentTasks = ref<any[]>([])
const labs = ref<any[]>([])
const listData = ref<TeachingApplicationDto[]>([])
const total = ref(0)
const currentRow = ref<TeachingApplicationDto | null>(null)
const formRef = ref<FormInstance>()

const queryForm = reactive({
  semesterId: null as string | null,
  status: '',
  page: 1,
  pageSize: 20
})

const form = reactive<{
  semesterId: string
  teachingTaskId: string
  courseName: string
  majorId: string
  majorName: string
  classId: string
  className: string
  dayOfWeek: number
  periodNumbers: number[]
  startWeek: number
  endWeek: number
  expectedLabId: string
  remark: string
}>({
  semesterId: '',
  teachingTaskId: '',
  courseName: '',
  majorId: '',
  majorName: '',
  classId: '',
  className: '',
  dayOfWeek: 1,
  periodNumbers: [],
  startWeek: 1,
  endWeek: 1,
  expectedLabId: '',
  remark: ''
})

const formRules: FormRules = {
  semesterId: [{ required: true, message: '请选择学期', trigger: 'change' }],
  teachingTaskId: [{ required: true, message: '请选择实验教学任务', trigger: 'change' }],
  dayOfWeek: [{ required: true, message: '请选择星期', trigger: 'change' }],
  periodNumbers: [{ required: true, message: '请选择节次', trigger: 'change', type: 'array', min: 1 }],
  startWeek: [{ required: true, message: '请填写起始周', trigger: 'blur' }],
  endWeek: [{ required: true, message: '请填写结束周', trigger: 'blur' }]
}

const weekDays = [
  { label: '周一', value: 1 }, { label: '周二', value: 2 }, { label: '周三', value: 3 },
  { label: '周四', value: 4 }, { label: '周五', value: 5 }, { label: '周六', value: 6 }, { label: '周日', value: 7 }
]

const formatDayOfWeek = (val?: number) => ['周一', '周二', '周三', '周四', '周五', '周六', '周日'][(val || 1) - 1] || '-'

const formatDate = (d?: string) => d ? new Date(d).toLocaleString('zh-CN') : '-'

const fetchList = async () => {
  if (!queryForm.semesterId) {
    listData.value = []
    total.value = 0
    return
  }
  loading.value = true
  try {
    const res = await teachingApplicationApi.getList({
      semesterId: queryForm.semesterId,
      status: queryForm.status || undefined,
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    if (res.data.code === 200) {
      listData.value = res.data.data || []
      total.value = res.data.data?.length || 0
    }
  } catch {
    listData.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryForm.status = ''
  queryForm.page = 1
  fetchList()
}

const handleOpenDialog = async () => {
  Object.assign(form, {
    semesterId: queryForm.semesterId || semesters.value[0]?.id || '',
    teachingTaskId: '', courseName: '', majorId: '', majorName: '',
    classId: '', className: '',
    dayOfWeek: 1, periodNumbers: [], startWeek: 1, endWeek: 1,
    expectedLabId: '', remark: ''
  })
  dialogVisible.value = true
}

const handleTaskChange = (taskId: string) => {
  const task = experimentTasks.value.find(t => t.id === taskId)
  if (!task) return
  form.courseName = task.courseName || ''
  form.majorName = task.majorName || task.major?.name || ''
  form.className = task.className || task.class?.name || ''
  form.classId = task.classId || task.class?.id || ''
  form.majorId = task.majorId || task.major?.id || ''
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  const payload: CreateTeachingApplicationRequest = {
    semesterId: form.semesterId,
    teachingTaskId: form.teachingTaskId,
    courseName: form.courseName,
    majorId: form.majorId || undefined,
    majorName: form.majorName,
    classId: form.classId || undefined,
    className: form.className,
    dayOfWeek: form.dayOfWeek,
    periodNumbers: form.periodNumbers,
    startWeek: form.startWeek,
    endWeek: form.endWeek,
    expectedLabId: form.expectedLabId || undefined,
    remark: form.remark || undefined
  }

  submitting.value = true
  try {
    const res = await teachingApplicationApi.create(payload)
    if (res.data.code === 200) {
      ElMessage.success('提交成功')
      dialogVisible.value = false
      fetchList()
    } else {
      ElMessage.error(res.data.message || '提交失败')
    }
  } catch (e: any) {
    ElMessage.error('提交失败: ' + (e?.response?.data?.message || e?.message || '未知错误'))
  } finally {
    submitting.value = false
  }
}

const handleView = (row: TeachingApplicationDto) => {
  currentRow.value = row
  detailVisible.value = true
}

onMounted(async () => {
  try {
    const [semRes, taskRes, labRes] = await Promise.all([
      fetch('/api/v1/semesters', { headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` } }).then(r => r.json()),
      fetch('/api/experiments/tasks', { headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` } }).then(r => r.json()),
      fetch('/api/v1/labs', { headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` } }).then(r => r.json())
    ])
    if (semRes.code === 200) {
      semesters.value = semRes.data || []
      const cur = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (cur) queryForm.semesterId = cur.id
    }
    if (taskRes.code === 200) experimentTasks.value = taskRes.data || []
    if (labRes.code === 200) labs.value = labRes.data || []
  } catch {}
  if (queryForm.semesterId) await fetchList()
})
</script>

<style scoped>
.teaching-application-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 12px; }
.search-card { margin-bottom: 20px; }
.pagination-container { display: flex; justify-content: flex-end; margin-top: 20px; }
</style>
