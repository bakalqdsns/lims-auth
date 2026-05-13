<template>
  <div class="usage-registration-container">
    <div class="page-header">
      <h2>使用登记</h2>
    </div>

    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="学期">
          <el-select v-model="queryForm.semesterId" placeholder="请选择学期" style="width: 200px" @change="fetchData">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryForm.status" clearable placeholder="全部" style="width: 120px" @change="fetchData">
            <el-option label="待登记" value="Pending" />
            <el-option label="已登记" value="Registered" />
            <el-option label="已逾期" value="Overdue" />
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
        <el-table-column prop="labName" label="实验室" width="140" show-overflow-tooltip />
        <el-table-column label="使用时间" width="200">
          <template #default="{ row }">
            {{ (row.useDate || '').slice(0, 10) }} 第{{ row.weekNumber }}周 {{ formatDayOfWeek(row.dayOfWeek) }} 第{{ row.periodNumber }}节
          </template>
        </el-table-column>
        <el-table-column prop="courseName" label="课程/项目" min-width="140" show-overflow-tooltip />
        <el-table-column prop="className" label="班级" width="140" show-overflow-tooltip />
        <el-table-column prop="expectedStudentCount" label="应到人数" width="80" />
        <el-table-column prop="actualStudentCount" label="实到人数" width="80" />
        <el-table-column prop="plannedHours" label="计划学时" width="80" />
        <el-table-column prop="actualHours" label="实际学时" width="80" />
        <el-table-column label="状态" width="90">
          <template #default="{ row }">{{ formatStatus(row.status) }}</template>
        </el-table-column>
        <el-table-column prop="filledByName" label="登记人" width="100" />
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button v-if="row.status === 'Pending' || row.status === 'Overdue'" type="primary" link @click="handleFill(row)">登记</el-button>
            <el-button type="primary" link @click="handleView(row)">详情</el-button>
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

    <el-dialog v-model="formDialogVisible" title="填写使用登记" width="700px" destroy-on-close>
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="120px">
        <el-row :gutter="16">
          <el-col :span="12"><el-form-item label="实验室名称">{{ currentRow?.labName }}</el-form-item></el-col>
          <el-col :span="12"><el-form-item label="课程/项目">{{ currentRow?.courseName || currentRow?.projectName }}</el-form-item></el-col>
          <el-col :span="8">
            <el-form-item label="应到人数">
              <el-input-number v-model="form.expectedStudentCount" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="实到人数" prop="actualStudentCount">
              <el-input-number v-model="form.actualStudentCount" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="实际学时" prop="actualHours">
              <el-input-number v-model="form.actualHours" :min="0" :step="0.5" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="考勤情况" prop="attendanceRecord">
              <el-select v-model="form.attendanceRecord" placeholder="请选择" style="width: 100%">
                <el-option label="全部出勤" value="无" />
                <el-option label="有迟到" value="有迟到" />
                <el-option label="有旷课" value="有旷课" />
                <el-option label="迟到+旷课" value="迟到+旷课" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="教学情况" prop="teachingCondition">
              <el-select v-model="form.teachingCondition" placeholder="请选择" style="width: 100%">
                <el-option label="正常" value="正常" />
                <el-option label="设备故障" value="设备故障" />
                <el-option label="其他异常" value="其他异常" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="设备状况" prop="equipmentCondition">
              <el-select v-model="form.equipmentCondition" placeholder="请选择" style="width: 100%">
                <el-option label="正常" value="正常" />
                <el-option label="部分损坏" value="部分损坏" />
                <el-option label="严重损坏" value="严重损坏" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="formDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">提交登记</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="detailDialogVisible" title="使用登记详情" width="600px" destroy-on-close>
      <el-descriptions v-if="currentRow" :column="2" border>
        <el-descriptions-item label="实验室">{{ currentRow.labName }}</el-descriptions-item>
        <el-descriptions-item label="地点">{{ currentRow.buildingName }} {{ currentRow.roomNumber }}</el-descriptions-item>
        <el-descriptions-item label="使用时间">{{ (currentRow.useDate || '').slice(0, 10) }} 第{{ currentRow.weekNumber }}周 {{ formatDayOfWeek(currentRow.dayOfWeek) }} 第{{ currentRow.periodNumber }}节</el-descriptions-item>
        <el-descriptions-item label="课程/项目">{{ currentRow.courseName || currentRow.projectName }}</el-descriptions-item>
        <el-descriptions-item label="班级">{{ currentRow.className || '-' }}</el-descriptions-item>
        <el-descriptions-item label="来源">{{ currentRow.source }}</el-descriptions-item>
        <el-descriptions-item label="应到人数">{{ currentRow.expectedStudentCount || '-' }}</el-descriptions-item>
        <el-descriptions-item label="实到人数">{{ currentRow.actualStudentCount || '-' }}</el-descriptions-item>
        <el-descriptions-item label="计划学时">{{ currentRow.plannedHours }}</el-descriptions-item>
        <el-descriptions-item label="实际学时">{{ currentRow.actualHours }}</el-descriptions-item>
        <el-descriptions-item label="考勤情况">{{ currentRow.attendanceRecord || '-' }}</el-descriptions-item>
        <el-descriptions-item label="教学情况">{{ currentRow.teachingCondition || '-' }}</el-descriptions-item>
        <el-descriptions-item label="设备状况">{{ currentRow.equipmentCondition || '-' }}</el-descriptions-item>
        <el-descriptions-item label="登记人">{{ currentRow.filledByName }}</el-descriptions-item>
        <el-descriptions-item label="登记时间">{{ currentRow.filledAt ? new Date(currentRow.filledAt).toLocaleString() : '-' }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Search } from '@element-plus/icons-vue'

const authHeaders = () => ({ 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` })

const loading = ref(false)
const submitting = ref(false)
const formDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const semesters = ref<any[]>([])
const listData = ref<any[]>([])
const total = ref(0)
const currentRow = ref<any | null>(null)
const formRef = ref<FormInstance>()

const queryForm = reactive({ semesterId: null as string | null, status: '', page: 1, pageSize: 20 })

const form = reactive({
  actualStudentCount: 0, actualHours: 0,
  attendanceRecord: '无', teachingCondition: '正常',
  equipmentCondition: '正常', expectedStudentCount: 0
})

const formRules: FormRules = {
  actualStudentCount: [{ required: true, message: '请填写实到人数', trigger: 'blur' }],
  actualHours: [{ required: true, message: '请填写实际学时', trigger: 'blur' }]
}

const formatDayOfWeek = (val?: number) => ['周一', '周二', '周三', '周四', '周五', '周六', '周日'][(val || 1) - 1] || '-'
const formatStatus = (val?: string) => ({ Pending: '待登记', Registered: '已登记', Overdue: '已逾期' }[val || ''] || val || '-')

const fetchData = async () => {
  if (!queryForm.semesterId) return
  loading.value = true
  try {
    const params = new URLSearchParams()
    params.append('semesterId', queryForm.semesterId)
    if (queryForm.status) params.append('status', queryForm.status)
    params.append('page', String(queryForm.page))
    params.append('pageSize', String(queryForm.pageSize))
    const res = await fetch(`/api/v1/usage-registrations?${params}`, { headers: authHeaders() }).then(r => r.json())
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

const handleReset = () => { queryForm.status = ''; queryForm.page = 1; fetchData() }

const handleFill = (row: any) => {
  currentRow.value = row
  form.actualStudentCount = row.actualStudentCount || row.expectedStudentCount || 0
  form.actualHours = row.actualHours || row.plannedHours || 0
  form.attendanceRecord = row.attendanceRecord || '无'
  form.teachingCondition = row.teachingCondition || '正常'
  form.equipmentCondition = row.equipmentCondition || '正常'
  form.expectedStudentCount = row.expectedStudentCount || 0
  formDialogVisible.value = true
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid || !currentRow.value) return
  submitting.value = true
  try {
    const res = await fetch('/api/v1/usage-registrations', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({
        semesterId: currentRow.value.semesterId,
        labId: currentRow.value.labId,
        labName: currentRow.value.labName,
        buildingName: currentRow.value.buildingName,
        roomNumber: currentRow.value.roomNumber,
        useDate: currentRow.value.useDate,
        weekNumber: currentRow.value.weekNumber,
        dayOfWeek: currentRow.value.dayOfWeek,
        periodNumber: currentRow.value.periodNumber,
        source: currentRow.value.source,
        scheduleEntryId: currentRow.value.scheduleEntryId,
        reservationId: currentRow.value.reservationId,
        teachingApplicationId: currentRow.value.teachingApplicationId,
        courseName: currentRow.value.courseName,
        projectName: currentRow.value.projectName,
        className: currentRow.value.className,
        expectedStudentCount: currentRow.value.expectedStudentCount,
        actualStudentCount: form.actualStudentCount,
        plannedHours: currentRow.value.plannedHours,
        actualHours: form.actualHours,
        attendanceRecord: form.attendanceRecord,
        teachingCondition: form.teachingCondition,
        equipmentCondition: form.equipmentCondition
      })
    }).then(r => r.json())
    if (res.code === 200) {
      ElMessage.success('登记成功')
      formDialogVisible.value = false
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
    const semRes = await fetch('/api/v1/semesters', { headers: authHeaders() }).then(r => r.json())
    if (semRes.code === 200) {
      semesters.value = semRes.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) queryForm.semesterId = current.id
    }
  } catch {}
  if (queryForm.semesterId) await fetchData()
})
</script>

<style scoped>
.usage-registration-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.search-card { margin-bottom: 20px; }
.pagination-container { display: flex; justify-content: flex-end; margin-top: 20px; }
</style>
