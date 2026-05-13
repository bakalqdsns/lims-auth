<template>
  <div class="reservation-approval-container">
    <div class="page-header">
      <h2>预约审批</h2>
    </div>

    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="学期">
          <el-select v-model="queryForm.semesterId" placeholder="请选择学期" style="width: 200px" @change="fetchData">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData"><el-icon><Refresh /></el-icon>刷新</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never" v-loading="loading">
      <el-table :data="listData" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="projectName" label="项目名称" min-width="140" show-overflow-tooltip />
        <el-table-column prop="projectCategory" label="项目类别" width="120" />
        <el-table-column prop="labName" label="实验室" width="120" show-overflow-tooltip />
        <el-table-column label="预约时间" width="160">
          <template #default="{ row }">
            第{{ row.weekNumber }}周 {{ formatDayOfWeek(row.dayOfWeek) }} 第{{ (row.periodNumbers || []).join(',') }}节
          </template>
        </el-table-column>
        <el-table-column prop="applicantName" label="申请人" width="100" />
        <el-table-column prop="memberCount" label="人数" width="70" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="success" link @click="handleApprove(row)">通过</el-button>
            <el-button type="danger" link @click="handleReject(row)">驳回</el-button>
            <el-button type="primary" link @click="handleView(row)">详情</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-empty v-if="!loading && listData.length === 0" description="暂无待审批预约" />

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

    <el-dialog v-model="approvalDialogVisible" :title="approvalAction === 'approve' ? '通过预约申请' : '驳回预约申请'" width="500px" destroy-on-close>
      <el-form :model="approvalForm" label-width="100px">
        <el-form-item label="预约信息">
          <el-descriptions v-if="currentRow" :column="1" border size="small">
            <el-descriptions-item label="项目名称">{{ currentRow.projectName }}</el-descriptions-item>
            <el-descriptions-item label="实验室">{{ currentRow.labName }}</el-descriptions-item>
            <el-descriptions-item label="时间">第{{ currentRow.weekNumber }}周 {{ formatDayOfWeek(currentRow.dayOfWeek) }} 第{{ (currentRow.periodNumbers || []).join(',') }}节</el-descriptions-item>
            <el-descriptions-item label="申请人">{{ currentRow.applicantName }}</el-descriptions-item>
          </el-descriptions>
        </el-form-item>
        <el-form-item :label="approvalAction === 'approve' ? '审批意见' : '驳回原因'" required>
          <el-input v-model="approvalForm.comment" type="textarea" :rows="3" :placeholder="approvalAction === 'approve' ? '选填审批意见' : '请输入驳回原因'" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="approvalDialogVisible = false">取消</el-button>
        <el-button :type="approvalAction === 'approve' ? 'success' : 'danger'" :loading="submitting" @click="handleSubmitApproval">
          {{ approvalAction === 'approve' ? '确认通过' : '确认驳回' }}
        </el-button>
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
import { ElMessage } from 'element-plus'
import { Refresh } from '@element-plus/icons-vue'

const authHeaders = () => ({ 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` })

const loading = ref(false)
const submitting = ref(false)
const semesters = ref<any[]>([])
const listData = ref<any[]>([])
const total = ref(0)
const approvalDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const currentRow = ref<any | null>(null)
const approvalAction = ref<'approve' | 'reject'>('approve')

const queryForm = reactive({ semesterId: null as string | null, page: 1, pageSize: 100 })
const approvalForm = reactive({ comment: '' })

const formatDayOfWeek = (val?: number) => ['周一', '周二', '周三', '周四', '周五', '周六', '周日'][(val || 1) - 1] || '-'
const formatStatus = (val?: string) => ({ Pending: '待审批', Approved: '已通过', Rejected: '已驳回', Cancelled: '已取消' }[val || ''] || val || '-')

const fetchData = async () => {
  if (!queryForm.semesterId) return
  loading.value = true
  try {
    const params = new URLSearchParams({ semesterId: queryForm.semesterId })
    const res = await fetch(`/api/v1/reservations/pending?${params}`, { headers: authHeaders() }).then(r => r.json())
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

const handleApprove = (row: any) => { currentRow.value = row; approvalAction.value = 'approve'; approvalForm.comment = ''; approvalDialogVisible.value = true }
const handleReject = (row: any) => { currentRow.value = row; approvalAction.value = 'reject'; approvalForm.comment = ''; approvalDialogVisible.value = true }

const handleSubmitApproval = async () => {
  if (approvalAction.value === 'reject' && !approvalForm.comment.trim()) {
    ElMessage.warning('请输入驳回原因')
    return
  }
  if (!currentRow.value) return
  submitting.value = true
  try {
    const action = approvalAction.value === 'approve' ? 'approve' : 'reject'
    const res = await fetch(`/api/v1/reservations/${currentRow.value.id}/${action}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify({ comment: approvalForm.comment })
    }).then(r => r.json())
    if (res.code === 200) {
      ElMessage.success(approvalAction.value === 'approve' ? '预约已通过' : '预约已驳回')
      approvalDialogVisible.value = false
      fetchData()
    } else {
      ElMessage.error(res.message || '操作失败')
    }
  } catch {
    ElMessage.error('操作失败')
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
      if (current) { queryForm.semesterId = current.id; await fetchData() }
    }
  } catch {}
})
</script>

<style scoped>
.reservation-approval-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.search-card { margin-bottom: 20px; }
.pagination-container { display: flex; justify-content: flex-end; margin-top: 20px; }
</style>
