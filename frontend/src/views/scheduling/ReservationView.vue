<template>
  <div class="reservation-view-container">
    <div class="page-header">
      <h2>预约申请</h2>
      <div class="header-actions">
        <el-button type="primary" @click="handleOpenDialog">
          <el-icon><Plus /></el-icon>
          提交预约
        </el-button>
      </div>
    </div>

    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="学期">
          <el-select v-model="queryForm.semesterId" placeholder="请选择学期" style="width: 200px" @change="fetchData">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="实验室">
          <el-select v-model="queryForm.labId" clearable placeholder="全部" style="width: 180px" @change="fetchData">
            <el-option v-for="l in labs" :key="l.id" :label="l.name" :value="l.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryForm.status" clearable placeholder="全部" style="width: 120px" @change="fetchData">
            <el-option label="待审批" value="Pending" />
            <el-option label="已通过" value="Approved" />
            <el-option label="已驳回" value="Rejected" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData"><el-icon><Search /></el-icon>搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never" v-loading="loading">
      <el-table :data="listData" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="projectName" label="项目名称" min-width="140" show-overflow-tooltip />
        <el-table-column prop="projectCategory" label="项目类别" width="120" />
        <el-table-column prop="labName" label="实验室" width="140" show-overflow-tooltip />
        <el-table-column label="预约时间" width="180">
          <template #default="{ row }">
            第{{ row.weekNumber }}周 {{ formatDayOfWeek(row.dayOfWeek) }} 第{{ (row.periodNumbers || []).join(',') }}节
          </template>
        </el-table-column>
        <el-table-column prop="applicantName" label="申请人" width="100" />
        <el-table-column prop="memberCount" label="参与人数" width="80" />
        <el-table-column label="状态" width="90">
          <template #default="{ row }">{{ formatStatus(row.status) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
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
          @size-change="fetchData"
          @current-change="fetchData"
        />
      </div>
    </el-card>

    <el-dialog v-model="dialogVisible" title="提交预约申请" width="700px" destroy-on-close>
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="130px">
        <el-form-item label="实验室" prop="labId">
          <el-select v-model="form.labId" placeholder="请选择实验室" style="width: 100%">
            <el-option v-for="l in labs" :key="l.id" :label="l.name" :value="l.id" />
          </el-select>
        </el-form-item>
        <el-row :gutter="12">
          <el-col :span="8">
            <el-form-item label="教学周" prop="weekNumber">
              <el-input-number v-model="form.weekNumber" :min="1" :max="20" style="width: 100%" />
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
            <el-form-item label="节次" prop="periodNumbers">
              <el-select v-model="form.periodNumbers" multiple placeholder="选择节次" style="width: 100%">
                <el-option v-for="p in periodOptions" :key="p" :label="`第${p}节`" :value="p" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="项目名称" prop="projectName">
              <el-input v-model="form.projectName" placeholder="请输入项目名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="项目类别" prop="projectCategory">
              <el-select v-model="form.projectCategory" placeholder="请选择类别" style="width: 100%">
                <el-option label="课程教学" value="CourseTeaching" />
                <el-option label="教师科研" value="TeacherResearch" />
                <el-option label="学生科研" value="StudentResearch" />
                <el-option label="创新创业" value="InnovationEntrepreneurship" />
                <el-option label="毕业论文" value="GraduationThesis" />
                <el-option label="学生课外活动" value="StudentActivity" />
                <el-option label="机构活动" value="InstitutionActivity" />
                <el-option label="其他" value="Other" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话">
              <el-input v-model="form.applicantPhone" placeholder="请输入联系电话" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="参与人数">
              <el-input-number v-model="form.memberCount" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="年级">
              <el-input v-model="form.memberGrade" placeholder="如：2024级" />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input v-model="form.remark" type="textarea" :rows="2" placeholder="选填" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">提交</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="detailDialogVisible" title="预约详情" width="600px" destroy-on-close>
      <el-descriptions v-if="currentRow" :column="2" border>
        <el-descriptions-item label="项目名称">{{ currentRow.projectName }}</el-descriptions-item>
        <el-descriptions-item label="项目类别">{{ currentRow.projectCategory }}</el-descriptions-item>
        <el-descriptions-item label="实验室">{{ currentRow.labName }}</el-descriptions-item>
        <el-descriptions-item label="预约时间">第{{ currentRow.weekNumber }}周 {{ formatDayOfWeek(currentRow.dayOfWeek) }} 第{{ (currentRow.periodNumbers || []).join(',') }}节</el-descriptions-item>
        <el-descriptions-item label="申请人">{{ currentRow.applicantName }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ currentRow.applicantPhone }}</el-descriptions-item>
        <el-descriptions-item label="参与人数">{{ currentRow.memberCount || '-' }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ formatStatus(currentRow.status) }}</el-descriptions-item>
        <el-descriptions-item label="审批意见" :span="2">{{ currentRow.approvalComment || '-' }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{ currentRow.remark || '-' }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'

const authHeaders = () => ({ 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` })

const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const semesters = ref<any[]>([])
const labs = ref<any[]>([])
const listData = ref<any[]>([])
const total = ref(0)
const currentRow = ref<any | null>(null)
const formRef = ref<FormInstance>()

const queryForm = reactive({ semesterId: null as string | null, labId: '', status: '', page: 1, pageSize: 20 })

const form = reactive({
  labId: '', weekNumber: 1, dayOfWeek: 1,
  periodNumbers: [] as number[], projectName: '', projectCategory: '',
  applicantPhone: '', memberCount: 0, memberGrade: '', remark: ''
})

const formRules: FormRules = {
  labId: [{ required: true, message: '请选择实验室', trigger: 'change' }],
  weekNumber: [{ required: true, message: '请填写周次', trigger: 'blur' }],
  dayOfWeek: [{ required: true, message: '请选择星期', trigger: 'change' }],
  periodNumbers: [{ required: true, message: '请选择节次', trigger: 'change' }],
  projectName: [{ required: true, message: '请输入项目名称', trigger: 'blur' }],
  projectCategory: [{ required: true, message: '请选择类别', trigger: 'change' }]
}

const weekDays = [
  { label: '周一', value: 1 }, { label: '周二', value: 2 }, { label: '周三', value: 3 },
  { label: '周四', value: 4 }, { label: '周五', value: 5 }, { label: '周六', value: 6 }, { label: '周日', value: 7 }
]
const periodOptions = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]

const formatDayOfWeek = (val?: number) => ['周一', '周二', '周三', '周四', '周五', '周六', '周日'][(val || 1) - 1] || '-'
const formatStatus = (val?: string) => ({ Pending: '待审批', Approved: '已通过', Rejected: '已驳回', Cancelled: '已取消' }[val || ''] || val || '-')

const buildQuery = () => {
  const params = new URLSearchParams()
  if (queryForm.semesterId) params.append('semesterId', queryForm.semesterId)
  if (queryForm.labId) params.append('labId', queryForm.labId)
  if (queryForm.status) params.append('status', queryForm.status)
  params.append('page', String(queryForm.page))
  params.append('pageSize', String(queryForm.pageSize))
  return params
}

const fetchData = async () => {
  if (!queryForm.semesterId) return
  loading.value = true
  try {
    const res = await fetch(`/api/v1/reservations?${buildQuery()}`, { headers: authHeaders() }).then(r => r.json())
    if (res.code === 200) {
      listData.value = res.data || []
      total.value = res.data?.length || 0
    }
  } catch {
    listData.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryForm.labId = ''
  queryForm.status = ''
  queryForm.page = 1
  fetchData()
}

const handleOpenDialog = () => {
  Object.assign(form, { labId: '', weekNumber: 1, dayOfWeek: 1, periodNumbers: [], projectName: '', projectCategory: '', applicantPhone: '', memberCount: 0, memberGrade: '', remark: '' })
  dialogVisible.value = true
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    const res = await fetch('/api/v1/reservations', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({ ...form, semesterId: queryForm.semesterId, useDate: new Date().toISOString() })
    }).then(r => r.json())
    if (res.code === 200) {
      ElMessage.success('提交成功')
      dialogVisible.value = false
      fetchData()
    } else {
      ElMessage.error(res.message || '提交失败')
    }
  } catch {
    ElMessage.error('提交失败')
  } finally {
    submitting.value = false
  }
}

const handleView = (row: any) => { currentRow.value = row; detailDialogVisible.value = true }

onMounted(async () => {
  try {
    const [semRes, labRes] = await Promise.all([
      fetch('/api/v1/semesters', { headers: authHeaders() }).then(r => r.json()),
      fetch('/api/v1/labs', { headers: authHeaders() }).then(r => r.json())
    ])
    if (semRes.code === 200) {
      semesters.value = semRes.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) { queryForm.semesterId = current.id }
    }
    if (labRes.code === 200) { labs.value = labRes.data || [] }
  } catch {}
  if (queryForm.semesterId) await fetchData()
})
</script>

<style scoped>
.reservation-view-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 12px; }
.search-card { margin-bottom: 20px; }
.pagination-container { display: flex; justify-content: flex-end; margin-top: 20px; }
</style>
