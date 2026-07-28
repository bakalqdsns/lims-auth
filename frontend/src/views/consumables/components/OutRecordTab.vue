<template>
  <div class="tab-content">
    <div class="toolbar">
      <el-button type="primary" @click="handleCreate">
        <el-icon><Plus /></el-icon> 提交领用申请
      </el-button>
      <el-button v-if="!myMode" @click="handleMyRecords">
        <el-icon><User /></el-icon> 我的申请
      </el-button>
      <el-button v-else @click="handleAllRecords">
        <el-icon><User /></el-icon> 返回全部
      </el-button>
    </div>

    <el-card shadow="never" class="search-card">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input v-model="queryForm.keyword" placeholder="单号/耗材名称" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryForm.status" placeholder="全部" clearable style="width: 120px">
            <el-option v-for="s in STOCK_STATUSES" :key="s" :label="STOCK_STATUS_MAP[s]" :value="s" />
          </el-select>
        </el-form-item>
        <el-form-item label="时间范围">
          <el-date-picker v-model="dateRange" type="daterange" range-separator="至"
            start-placeholder="开始" end-placeholder="结束"
            value-format="YYYY-MM-DD" style="width: 240px" @change="onDateChange" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch"><el-icon><Search /></el-icon> 搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never">
      <el-table v-loading="loading" :data="list" stripe>
        <el-table-column prop="recordNo" label="领用单号" width="200" />
        <el-table-column prop="consumableName" label="耗材名称" min-width="150" />
        <el-table-column prop="categoryName" label="分类" width="100" />
        <el-table-column prop="quantity" label="数量" width="80" align="right" />
        <el-table-column prop="unit" label="单位" width="60" />
        <el-table-column prop="usagePurpose" label="用途" min-width="120" />
        <el-table-column prop="usageLab" label="使用实验室" width="120" />
        <el-table-column prop="applicantName" label="申请人" width="100" />
        <el-table-column prop="outTime" label="申请时间" width="160">
          <template #default="{ row }">{{ formatDate(row.outTime) }}</template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="90">
          <template #default="{ row }">
            <el-tag :type="STOCK_STATUS_TYPE[row.status]">{{ STOCK_STATUS_MAP[row.status] }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <template v-if="row.status === 'Pending' && hasPermission('consumable:approve')">
              <el-button link type="success" @click="handleApprove(row, true)">通过</el-button>
              <el-button link type="danger" @click="handleApprove(row, false)">驳回</el-button>
            </template>
            <el-popconfirm
              v-if="row.applicantId === currentUserId && row.status === 'Pending'"
              title="确定取消该申请吗？" @confirm="handleCancel(row)">
              <template #reference>
                <el-button link type="warning">取消</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 领用表单 -->
    <el-dialog v-model="dialogVisible" title="提交领用申请" width="550px" destroy-on-close>
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-form-item label="耗材" prop="consumableId">
          <el-select v-model="form.consumableId" filterable placeholder="选择耗材" style="width: 100%" @change="onConsumableChange">
            <el-option v-for="c in consumables" :key="c.id"
              :label="`${c.name}（${c.code}，库存:${c.availableStock}）`"
              :value="c.id">
              <div style="display:flex;justify-content:space-between">
                <span>{{ c.name }}</span>
                <span style="color:#999">库存: {{ c.availableStock }} {{ c.unit }}</span>
              </div>
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="申请数量" prop="quantity">
          <el-input-number v-model="form.quantity" :min="0.001" :precision="3" style="width: 100%" />
          <div class="form-tip" v-if="selectedConsumable">
            可用库存: {{ selectedConsumable.availableStock }} {{ selectedConsumable.unit }}，
            单次最大: {{ selectedConsumable.maxSingleRequest }}
          </div>
        </el-form-item>
        <el-form-item label="使用用途" prop="usagePurpose">
          <el-input v-model="form.usagePurpose" type="textarea" :rows="2" placeholder="请描述使用用途" />
        </el-form-item>
        <el-form-item label="使用实验室">
          <el-select v-model="form.usageLab" placeholder="请选择实验室" filterable clearable style="width: 100%">
            <el-option v-for="lab in labs" :key="lab.id" :label="`${lab.code} - ${lab.name}`" :value="lab.name" />
          </el-select>
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="form.remark" type="textarea" :rows="2" placeholder="备注信息" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">提交申请</el-button>
      </template>
    </el-dialog>

    <!-- 审批对话框 -->
    <el-dialog v-model="approvalDialogVisible" title="审批领用申请" width="400px">
      <el-form label-width="80px">
        <el-form-item label="审批结果">
          <el-radio-group v-model="approvalForm.approved">
            <el-radio :value="true">通过</el-radio>
            <el-radio :value="false">驳回</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="审批意见">
          <el-input v-model="approvalForm.comment" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="approvalDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmApproval">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Plus, Search, User } from '@element-plus/icons-vue'
import { consumableApi, type ConsumableOutRecordDto, type CreateOutRecordRequest,
  STOCK_STATUSES, STOCK_STATUS_MAP, STOCK_STATUS_TYPE, type ConsumableDto } from '../../../api/consumable'
import { labApi, type LabDto } from '../../../api/lab'
import { useAuthStore } from '../../../stores/auth'

const emit = defineEmits<{ (e: 'refresh'): void }>()
const authStore = useAuthStore()
const hasPermission = authStore.hasPermission
const currentUserId = computed(() => authStore.user?.id || '')

const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)
const approvalDialogVisible = ref(false)
const myMode = ref(false)

const list = ref<ConsumableOutRecordDto[]>([])
const consumables = ref<ConsumableDto[]>([])
const labs = ref<LabDto[]>([])
const total = ref(0)
const pagination = reactive({ page: 1, pageSize: 20 })
const dateRange = ref<string[]>([])

const queryForm = reactive({ keyword: '', status: '', startDate: '', endDate: '' })

const form = reactive<Record<string, any>>({
  consumableId: '', quantity: 1, usagePurpose: '', usageLab: '', remark: ''
})

const selectedConsumable = computed(() =>
  consumables.value.find(c => c.id === form.consumableId)
)

const currentApprovalId = ref('')
const approvalForm = reactive({ approved: true, comment: '' })
const formRef = ref<FormInstance>()

const formRules: FormRules = {
  consumableId: [{ required: true, message: '请选择耗材', trigger: 'change' }],
  quantity: [{ required: true, message: '请输入数量', trigger: 'blur' }]
}

const formatDate = (d: string) => d ? new Date(d).toLocaleString('zh-CN', { hour12: false }) : '-'
const onDateChange = (val: string[]) => {
  queryForm.startDate = val?.[0] || ''
  queryForm.endDate = val?.[1] || ''
}
const onConsumableChange = () => { form.quantity = 1 }

const loadData = async () => {
  loading.value = true
  try {
    const params: any = { page: pagination.page, pageSize: pagination.pageSize }
    if (queryForm.keyword) params.keyword = queryForm.keyword
    if (queryForm.status) params.status = queryForm.status
    if (queryForm.startDate) params.startDate = queryForm.startDate
    if (queryForm.endDate) params.endDate = queryForm.endDate

    const res = myMode.value
      ? await consumableApi.getMyOutRecords(params)
      : await consumableApi.getOutRecords(params)
    if (res.data.code === 200) {
      list.value = res.data.data
      total.value = res.data.total
    }
  } catch {
    ElMessage.error('加载失败')
  } finally {
    loading.value = false
  }
}

const loadConsumables = async () => {
  try {
    const res = await consumableApi.getConsumables({ page: 1, pageSize: 500 } as any)
    if (res.data.code === 200) {
      consumables.value = res.data.data
    }
  } catch {}
}

const loadLabs = async () => {
  try {
    const res = await labApi.getList({ page: 1, pageSize: 500 })
    if (res.data.code === 200) {
      labs.value = res.data.data
    }
  } catch {}
}

const handleSearch = () => { pagination.page = 1; loadData() }
const handleReset = () => {
  Object.assign(queryForm, { keyword: '', status: '', startDate: '', endDate: '' })
  dateRange.value = []
  pagination.page = 1; loadData()
}
const handleSizeChange = () => { pagination.page = 1; loadData() }
const handlePageChange = () => { loadData() }
const handleMyRecords = () => { myMode.value = true; pagination.page = 1; loadData() }
const handleAllRecords = () => { myMode.value = false; pagination.page = 1; loadData() }
const handleCreate = () => {
  myMode.value = true
  Object.assign(form, { consumableId: '', quantity: 1, usagePurpose: '', usageLab: '', remark: '' })
  dialogVisible.value = true
  loadLabs()
}

const handleSave = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  saving.value = true
  try {
    const data: CreateOutRecordRequest = {
      consumableId: form.consumableId,
      quantity: form.quantity,
      usagePurpose: form.usagePurpose || undefined,
      usageLab: form.usageLab || undefined,
      remark: form.remark || undefined
    }
    const res = await consumableApi.createOutRecord(data)
    if (res.data.code === 200) {
      ElMessage.success('申请提交成功')
      dialogVisible.value = false
      loadData()
      emit('refresh')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '提交失败')
  } finally {
    saving.value = false
  }
}

const handleApprove = (row: ConsumableOutRecordDto, approved: boolean) => {
  currentApprovalId.value = row.id
  approvalForm.approved = approved
  approvalForm.comment = ''
  approvalDialogVisible.value = true
}

const confirmApproval = async () => {
  try {
    const res = await consumableApi.approveOutRecord(currentApprovalId.value, {
      approved: approvalForm.approved,
      comment: approvalForm.comment || undefined
    })
    if (res.data.code === 200) {
      ElMessage.success(approvalForm.approved ? '审批通过' : '已驳回')
      approvalDialogVisible.value = false
      loadData()
      emit('refresh')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch {
    ElMessage.error('审批失败')
  }
}

const handleCancel = async (row: ConsumableOutRecordDto) => {
  try {
    const res = await consumableApi.deleteOutRecord(row.id)
    if (res.data.code === 200) {
      ElMessage.success('已取消')
      loadData()
      emit('refresh')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch {
    ElMessage.error('取消失败')
  }
}

onMounted(() => {
  loadData()
  loadConsumables()
})
</script>

<style scoped>
.tab-content { padding-top: 12px; }
.toolbar { margin-bottom: 12px; display: flex; gap: 8px; }
.search-card { margin-bottom: 12px; }
.pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
.form-tip { font-size: 12px; color: #909399; margin-top: 4px; }
</style>
