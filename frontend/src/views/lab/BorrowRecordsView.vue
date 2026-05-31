<template>
  <div class="borrow-container">
    <div class="page-header">
      <h2>设备借还管理</h2>
      <div class="header-actions">
        <el-button @click="scanDialogVisible = true">
          <el-icon><Document /></el-icon> 扫码核验
        </el-button>
      </div>
    </div>

    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 我的申请 -->
      <el-tab-pane label="我的申请" name="my">
        <el-card shadow="never">
          <el-form inline>
            <el-form-item label="状态">
              <el-select v-model="myStatus" placeholder="全部" clearable style="width: 150px">
                <el-option v-for="s in BORROW_STATUSES" :key="s" :label="s" :value="s" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="fetchMyRecords">查询</el-button>
            </el-form-item>
          </el-form>

          <el-table :data="myRecords" v-loading="myLoading" stripe>
            <el-table-column prop="recordNo" label="单号" width="180" />
            <el-table-column prop="equipmentName" label="设备名称" min-width="150" show-overflow-tooltip />
            <el-table-column prop="equipmentCode" label="资产编号" width="120" />
            <el-table-column prop="phone" label="联系电话" width="120" />
            <el-table-column prop="borrowDate" label="计划借出" width="110">
              <template #default="{ row }">{{ row.borrowDate?.slice(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="expectedReturnDate" label="计划归还" width="110">
              <template #default="{ row }">
                {{ row.expectedReturnDate?.slice(0, 10) }}
                <el-tag v-if="row.status === '已借出' && isOverdue(row)" type="danger" size="small">已逾期</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="110">
              <template #default="{ row }">
                <el-tag :type="getStatusTagType(row.status)">{{ row.status }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="purpose" label="用途" min-width="120" show-overflow-tooltip />
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="showRecordDetail(row)">详情</el-button>
                <el-button v-if="row.status === '已借出' || row.status === '已逾期'" link type="warning" @click="handleRenew(row)">续借</el-button>
                <el-button v-if="row.status === '已借出' || row.status === '已逾期'" link type="success" @click="handleSubmitReturn(row)">归还</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-tab-pane>

      <!-- 待审批 -->
      <el-tab-pane label="待审批" name="pending">
        <el-card v-if="canApprove" shadow="never">
          <el-table :data="pendingRecords" v-loading="pendingLoading" stripe>
            <el-table-column prop="recordNo" label="单号" width="180" />
            <el-table-column prop="equipmentName" label="设备名称" min-width="150" show-overflow-tooltip />
            <el-table-column prop="applicantName" label="申请人" width="100" />
            <el-table-column prop="phone" label="联系电话" width="120" />
            <el-table-column prop="borrowDate" label="计划借出" width="110">
              <template #default="{ row }">{{ row.borrowDate?.slice(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="expectedReturnDate" label="计划归还" width="110">
              <template #default="{ row }">{{ row.expectedReturnDate?.slice(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="120">
              <template #default="{ row }">
                <el-tag :type="getStatusTagType(row.status)">{{ row.status }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="purpose" label="用途" min-width="120" show-overflow-tooltip />
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="showRecordDetail(row)">详情</el-button>
                <el-button v-if="['待老师审批','续借审批中','待管理员审批'].includes(row.status)" link type="success" @click="handleApprove(row, true)">批准</el-button>
                <el-button v-if="['待老师审批','续借审批中','待管理员审批'].includes(row.status)" link type="danger" @click="handleApprove(row, false)">拒绝</el-button>
                <el-button v-if="row.status === '管理员审批中'" link type="success" @click="handleApprove(row, true)">批准归还</el-button>
                <el-button v-if="row.status === '管理员审批中'" link type="danger" @click="handleApprove(row, false)">驳回归还</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
        <el-empty v-else description="您没有审批权限" />
      </el-tab-pane>

      <!-- 全部记录 -->
      <el-tab-pane label="全部记录" name="all">
        <el-card v-if="canReadAll" shadow="never">
          <el-form inline>
            <el-form-item label="关键词">
              <el-input v-model="allKeyword" placeholder="单号/设备/申请人" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="allStatus" placeholder="全部" clearable style="width: 150px">
                <el-option v-for="s in BORROW_STATUSES" :key="s" :label="s" :value="s" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="fetchAllRecords">查询</el-button>
            </el-form-item>
          </el-form>

          <el-table :data="allRecords" v-loading="allLoading" stripe>
            <el-table-column prop="recordNo" label="单号" width="180" />
            <el-table-column prop="equipmentName" label="设备名称" min-width="150" show-overflow-tooltip />
            <el-table-column prop="applicantName" label="申请人" width="100" />
            <el-table-column prop="phone" label="联系电话" width="120" />
            <el-table-column prop="status" label="状态" width="120">
              <template #default="{ row }">
                <el-tag :type="getStatusTagType(row.status)">{{ row.status }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="borrowDate" label="借出日期" width="110">
              <template #default="{ row }">{{ row.actualBorrowDate?.slice(0, 10) || row.borrowDate?.slice(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="expectedReturnDate" label="计划归还" width="110">
              <template #default="{ row }">
                {{ (row.renewedReturnDate || row.expectedReturnDate)?.slice(0, 10) }}
              </template>
            </el-table-column>
            <el-table-column prop="actualReturnDate" label="实际归还" width="110">
              <template #default="{ row }">{{ row.actualReturnDate?.slice(0, 10) || '--' }}</template>
            </el-table-column>
            <el-table-column prop="returnCondition" label="归还状态" width="90">
              <template #default="{ row }">
                <el-tag v-if="row.returnCondition" size="small" :type="getConditionType(row.returnCondition)">{{ row.returnCondition }}</el-tag>
                <span v-else class="text-gray">--</span>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="110" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="showRecordDetail(row)">详情</el-button>
                <el-button v-if="row.status === '已归还' && canDelete" link type="danger" @click="handleDelete(row)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
        <el-empty v-else description="您没有查看全部记录的权限" />
      </el-tab-pane>

      <!-- 逾期清单 -->
      <el-tab-pane label="逾期清单" name="overdue">
        <el-card shadow="never">
          <el-table :data="overdueRecords" v-loading="overdueLoading" stripe>
            <el-table-column prop="recordNo" label="单号" width="180" />
            <el-table-column prop="equipmentName" label="设备名称" min-width="150" />
            <el-table-column prop="equipmentCode" label="资产编号" width="120" />
            <el-table-column prop="applicantName" label="申请人" width="100" />
            <el-table-column prop="phone" label="联系电话" width="120" />
            <el-table-column prop="expectedReturnDate" label="应还日期" width="110">
              <template #default="{ row }">{{ row.expectedReturnDate?.slice(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="daysOverdue" label="逾期天数" width="100">
              <template #default="{ row }">
                <el-tag type="danger">{{ row.daysOverdue }}天</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <!-- 详情弹窗 -->
    <el-dialog v-model="detailVisible" title="借还记录详情" width="650px" destroy-on-close>
        <el-descriptions v-if="currentRecord" :column="2" border>
        <el-descriptions-item label="单号">{{ currentRecord.recordNo }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="getStatusTagType(currentRecord.status)">{{ currentRecord.status }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="设备名称">{{ currentRecord.equipmentName }}</el-descriptions-item>
        <el-descriptions-item label="资产编号">{{ currentRecord.equipmentCode }}</el-descriptions-item>
        <el-descriptions-item label="申请人">{{ currentRecord.applicantName }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ currentRecord.phone || '--' }}</el-descriptions-item>
        <el-descriptions-item label="计划借出">{{ currentRecord.borrowDate?.slice(0, 10) }}</el-descriptions-item>
        <el-descriptions-item label="计划归还">{{ (currentRecord.renewedReturnDate || currentRecord.expectedReturnDate)?.slice(0, 10) }}</el-descriptions-item>
        <el-descriptions-item label="实际借出">{{ currentRecord.actualBorrowDate?.slice(0, 10) || '--' }}</el-descriptions-item>
        <el-descriptions-item label="实际归还">{{ currentRecord.actualReturnDate?.slice(0, 10) || '--' }}</el-descriptions-item>
        <el-descriptions-item label="使用地点" :span="2">{{ currentRecord.usageLocation || '--' }}</el-descriptions-item>
        <el-descriptions-item label="借出用途" :span="2">{{ currentRecord.purpose }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{ currentRecord.remarks || '--' }}</el-descriptions-item>
        <el-descriptions-item label="导师审批">
          <span v-if="currentRecord.supervisorApprovalStatus">{{ currentRecord.supervisorApprovalStatus }} ({{ currentRecord.supervisorApprovalDate?.slice(0, 10) }})</span>
          <span v-else class="text-gray">待审批</span>
        </el-descriptions-item>
        <el-descriptions-item label="管理员审批">
          <span v-if="currentRecord.adminApprovalStatus">{{ currentRecord.adminApprovalStatus }} ({{ currentRecord.adminApprovalDate?.slice(0, 10) }})</span>
          <span v-else class="text-gray">待审批</span>
        </el-descriptions-item>
        <el-descriptions-item label="归还条件">
          <el-tag v-if="currentRecord.returnCondition" :type="getConditionType(currentRecord.returnCondition)">{{ currentRecord.returnCondition }}</el-tag>
          <span v-else class="text-gray">--</span>
        </el-descriptions-item>
        <el-descriptions-item label="验收人">{{ currentRecord.returnCheckerName || '--' }}</el-descriptions-item>
        <el-descriptions-item v-if="currentRecord.returnRemarks" label="归还备注" :span="2">{{ currentRecord.returnRemarks }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="detailVisible = false">关闭</el-button>
        <template v-if="currentRecord">
          <el-button v-if="currentRecord.status === '已借出' || currentRecord.status === '已逾期'" type="warning" @click="handleRenew(currentRecord)">续借</el-button>
          <el-button v-if="currentRecord.status === '已借出' || currentRecord.status === '已逾期'" type="success" @click="handleSubmitReturn(currentRecord)">归还</el-button>
          <el-button v-if="['待老师审批','续借审批中'].includes(currentRecord.status)" type="primary" @click="handleApprove(currentRecord, true)">批准</el-button>
          <el-button v-if="['待老师审批','续借审批中'].includes(currentRecord.status)" type="danger" @click="handleApprove(currentRecord, false)">拒绝</el-button>
          <el-button v-if="currentRecord.status === '待管理员审批'" type="primary" @click="handleApprove(currentRecord, true)">批准</el-button>
          <el-button v-if="currentRecord.status === '待管理员审批'" type="danger" @click="handleApprove(currentRecord, false)">拒绝</el-button>
          <el-button v-if="currentRecord.status === '管理员审批中'" type="primary" @click="handleApprove(currentRecord, true)">批准归还</el-button>
          <el-button v-if="currentRecord.status === '管理员审批中'" type="danger" @click="handleApprove(currentRecord, false)">驳回归还</el-button>
        </template>
      </template>
    </el-dialog>

    <!-- 审批弹窗 -->
    <el-dialog v-model="approvalDialogVisible" title="审批确认" width="450px">
      <el-form label-width="80px">
        <el-form-item label="审批结果">
          <el-radio-group v-model="approvalResult">
            <el-radio :label="true">批准</el-radio>
            <el-radio :label="false">拒绝</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="approvalResult && (currentRecord?.status === '管理员审批中')" label="归还条件" required>
          <el-select v-model="returnCondition" style="width: 100%">
            <el-option v-for="c in RETURN_CONDITIONS" :key="c" :label="c" :value="c" />
          </el-select>
        </el-form-item>
        <el-form-item label="审批备注">
          <el-input v-model="approvalRemark" type="textarea" :rows="3" placeholder="请输入审批备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="approvalDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="approvalLoading" @click="confirmApproval">确定</el-button>
      </template>
    </el-dialog>

    <!-- 续借弹窗 -->
    <el-dialog v-model="renewDialogVisible" title="续借申请" width="400px">
      <el-form label-width="100px">
        <el-form-item label="当前计划归还">
          {{ currentRecord?.expectedReturnDate?.slice(0, 10) }}
        </el-form-item>
        <el-form-item label="新计划归还日期" required>
          <el-date-picker v-model="newReturnDate" type="date" value-format="YYYY-MM-DD" placeholder="选择日期" style="width: 100%" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="renewDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="renewLoading" @click="confirmRenew">提交续借</el-button>
      </template>
    </el-dialog>

    <!-- 归还确认弹窗 -->
    <el-dialog v-model="returnDialogVisible" title="归还确认" width="400px">
      <el-form label-width="100px">
        <el-form-item label="归还设备">{{ currentRecord?.equipmentName }}</el-form-item>
        <el-form-item label="归还条件" required>
          <el-select v-model="returnCondition" style="width: 100%">
            <el-option v-for="c in RETURN_CONDITIONS" :key="c" :label="c" :value="c" />
          </el-select>
        </el-form-item>
        <el-form-item label="归还备注">
          <el-input v-model="returnRemarks" type="textarea" :rows="2" placeholder="备注信息" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="returnDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="returnLoading" @click="confirmReturn">确认归还</el-button>
      </template>
    </el-dialog>

    <!-- 扫码核验弹窗 -->
    <el-dialog v-model="scanDialogVisible" title="扫码核验" width="500px">
      <el-form label-width="100px">
        <el-form-item label="借还单号">
          <el-input v-model="scanRecordNo" placeholder="请扫描或输入借还单号" @keyup.enter="handleScanSearch">
            <template #append>
              <el-button :loading="scanLoading" @click="handleScanSearch">查询</el-button>
            </template>
          </el-input>
        </el-form-item>
      </el-form>
      <el-descriptions v-if="scanRecord" :column="2" border size="small" style="margin-top: 12px">
        <el-descriptions-item label="单号">{{ scanRecord.recordNo }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="getStatusTagType(scanRecord.status)">{{ scanRecord.status }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="设备">{{ scanRecord.equipmentName }}</el-descriptions-item>
        <el-descriptions-item label="资产编号">{{ scanRecord.equipmentCode }}</el-descriptions-item>
        <el-descriptions-item label="申请人">{{ scanRecord.applicantName }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ scanRecord.phone || '--' }}</el-descriptions-item>
        <el-descriptions-item label="使用地点" :span="2">{{ scanRecord.usageLocation || '--' }}</el-descriptions-item>
        <el-descriptions-item label="计划归还" :span="2">{{ scanRecord.expectedReturnDate?.slice(0, 10) }}</el-descriptions-item>
        <el-descriptions-item v-if="scanRecord.recipientName" label="领取人" :span="2">
          <el-tag type="success">{{ scanRecord.recipientName }}</el-tag>
          <span style="margin-left: 8px; color: #909399; font-size: 12px">
            {{ scanRecord.actualBorrowDate?.slice(0, 16) || '' }}
          </span>
        </el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="scanDialogVisible = false">关闭</el-button>
        <el-button v-if="scanRecord?.status === '待领取'" type="success" :loading="scanLoading" @click="handleConfirmBorrow">扫码确认借出</el-button>
        <el-button v-if="scanRecord?.status === '已借出'" type="primary" :loading="scanLoading" @click="openReturnDialog">提交归还申请</el-button>
        <el-button v-if="scanRecord?.status === '待归还'" type="primary" :loading="scanLoading" @click="confirmReturnFromScan">扫码确认归还</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Document } from '@element-plus/icons-vue'
import { borrowApi, type BorrowRecordDto, BORROW_STATUSES, RETURN_CONDITIONS } from '@/api/lab'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const canApprove = computed(() => authStore.hasPermission('equipment:approve') || authStore.isSuperAdmin)
const canReadAll = computed(() => authStore.hasPermission('equipment:read') || authStore.isSuperAdmin)
const canDelete = computed(() => authStore.hasPermission('equipment:delete') || authStore.isSuperAdmin)

const activeTab = ref('my')
const myStatus = ref('')
const myRecords = ref<BorrowRecordDto[]>([])
const myLoading = ref(false)

const pendingRecords = ref<BorrowRecordDto[]>([])
const pendingLoading = ref(false)

const allKeyword = ref('')
const allStatus = ref('')
const allRecords = ref<BorrowRecordDto[]>([])
const allLoading = ref(false)

const overdueRecords = ref<BorrowRecordDto[]>([])
const overdueLoading = ref(false)

const detailVisible = ref(false)
const currentRecord = ref<BorrowRecordDto | null>(null)

const approvalDialogVisible = ref(false)
const approvalResult = ref(true)
const approvalRemark = ref('')
const approvalLoading = ref(false)

const renewDialogVisible = ref(false)
const newReturnDate = ref('')
const renewLoading = ref(false)

const returnDialogVisible = ref(false)
const returnCondition = ref('完好')
const returnRemarks = ref('')
const returnLoading = ref(false)
const scanLoading = ref(false)

const scanDialogVisible = ref(false)
const scanRecordNo = ref('')
const scanRecord = ref<BorrowRecordDto | null>(null)

const fetchMyRecords = async () => {
  myLoading.value = true
  try {
    const res = await borrowApi.getMyRecords(myStatus.value || undefined)
    if (res.data.code === 200) myRecords.value = res.data.data
  } catch { ElMessage.error('获取申请记录失败') }
  finally { myLoading.value = false }
}

const fetchPending = async () => {
  pendingLoading.value = true
  try {
    const res = await borrowApi.getPendingApprovals()
    if (res.data.code === 200) pendingRecords.value = res.data.data
  } catch (err: any) { 
    const message = err?.response?.data?.message || '获取待审批记录失败'
    ElMessage.error(message)
  }
  finally { pendingLoading.value = false }
}

const fetchAllRecords = async () => {
  allLoading.value = true
  try {
    const res = await borrowApi.getAllRecords({ status: allStatus.value || undefined, keyword: allKeyword.value || undefined })
    if (res.data.code === 200) allRecords.value = res.data.data
  } catch { ElMessage.error('获取记录失败') }
  finally { allLoading.value = false }
}

const fetchOverdue = async () => {
  overdueLoading.value = true
  try {
    const res = await borrowApi.getOverdueRecords()
    if (res.data.code === 200) overdueRecords.value = res.data.data
  } catch { ElMessage.error('获取逾期清单失败') }
  finally { overdueLoading.value = false }
}

const handleTabChange = (tab: string) => {
  if (tab === 'my') fetchMyRecords()
  else if (tab === 'pending') fetchPending()
  else if (tab === 'all') fetchAllRecords()
  else if (tab === 'overdue') fetchOverdue()
}

const showRecordDetail = (row: BorrowRecordDto) => {
  currentRecord.value = row
  detailVisible.value = true
}

const handleApprove = async (row: BorrowRecordDto, approved: boolean) => {
  const action = approved ? '批准' : '拒绝'
  try {
    await ElMessageBox.confirm(`确定${action}该借还申请吗？`, '审批确认', { type: 'warning' })
    approvalResult.value = approved
    approvalRemark.value = ''
    currentRecord.value = row
    approvalDialogVisible.value = true
  } catch {}
}

const confirmApproval = async () => {
  approvalLoading.value = true
  try {
    let res: any
    if (currentRecord.value?.status === '管理员审批中') {
      res = await borrowApi.approveReturn(currentRecord.value!.id, { approved: approvalResult.value, condition: returnCondition.value, remarks: approvalRemark.value || undefined })
    } else if (currentRecord.value?.status === '续借审批中') {
      res = await borrowApi.approveRenew(currentRecord.value!.id, { approved: approvalResult.value, remark: approvalRemark.value || undefined })
    } else if (currentRecord.value?.status === '待管理员审批') {
      res = await borrowApi.adminApprove(currentRecord.value!.id, { approved: approvalResult.value, remark: approvalRemark.value || undefined })
    } else {
      res = await borrowApi.supervisorApprove(currentRecord.value!.id, { approved: approvalResult.value, remark: approvalRemark.value || undefined })
    }
    if (res.data.code === 200) {
      ElMessage.success(res.data.message)
      approvalDialogVisible.value = false
      detailVisible.value = false
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '审批失败')
  } finally {
    approvalLoading.value = false
  }
}

const handleAdminApprove = async (row: BorrowRecordDto, approved: boolean) => {
  try {
    await ElMessageBox.confirm(`确定${approved ? '批准' : '拒绝'}该申请吗？`, '审批确认', { type: 'warning' })
    const res = await borrowApi.adminApprove(row.id, { approved, remark: undefined })
    if (res.data.code === 200) {
      ElMessage.success(res.data.message)
      fetchAllRecords()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch {}
}

const handleRenew = (row: BorrowRecordDto) => {
  currentRecord.value = row
  newReturnDate.value = ''
  renewDialogVisible.value = true
}

const confirmRenew = async () => {
  if (!newReturnDate.value) { ElMessage.warning('请选择新的归还日期'); return }
  renewLoading.value = true
  try {
    const res = await borrowApi.renew(currentRecord.value!.id, { newReturnDate: newReturnDate.value })
    if (res.data.code === 200) {
      ElMessage.success('续借申请已提交')
      renewDialogVisible.value = false
      detailVisible.value = false
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '续借申请失败')
  } finally {
    renewLoading.value = false
  }
}

const handleSubmitReturn = async (row: BorrowRecordDto) => {
  try {
    await ElMessageBox.confirm('确定提交归还申请吗？', '归还确认', { type: 'warning' })
    const res = await borrowApi.submitReturn(row.id)
    if (res.data.code === 200) {
      ElMessage.success('归还申请已提交')
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch {}
}

const openReturnDialog = () => {
  if (scanRecord.value) currentRecord.value = scanRecord.value
  returnCondition.value = '完好'
  returnRemarks.value = ''
  scanDialogVisible.value = false
  returnDialogVisible.value = true
}

const confirmReturn = async () => {
  returnLoading.value = true
  try {
    const res = await borrowApi.confirmReturn(currentRecord.value!.id, { condition: returnCondition.value, remarks: returnRemarks.value || undefined })
    if (res.data.code === 200) {
      ElMessage.success('归还确认成功')
      returnDialogVisible.value = false
      detailVisible.value = false
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '归还确认失败')
  } finally {
    returnLoading.value = false
  }
}

const confirmReturnFromScan = async () => {
  scanLoading.value = true
  try {
    const res = await borrowApi.confirmReturn(scanRecord.value!.id, { condition: scanRecord.value!.returnCondition || '完好', remarks: undefined })
    if (res.data.code === 200) {
      ElMessage.success('归还确认成功')
      scanDialogVisible.value = false
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '归还确认失败')
  } finally {
    scanLoading.value = false
  }
}

const handleDelete = async (row: BorrowRecordDto) => {
  try {
    await ElMessageBox.confirm(`确定要删除借还记录「${row.recordNo}」吗？删除后不可恢复。`, '删除确认', {
      confirmButtonText: '删除',
      cancelButtonText: '取消',
      type: 'warning',
    })
  } catch {
    return
  }
  try {
    const res = await borrowApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(`删除失败：${res.data.message || '未知错误'}`)
    }
  } catch (err: any) {
    const errorMsg = err?.response?.data?.message || err?.message || '删除失败'
    ElMessage.error(`删除记录「${row.recordNo}」失败：${errorMsg}`)
    console.error('删除失败:', err)
  }
}

const handleScanSearch = async () => {
  if (!scanRecordNo.value) return
  try {
    const res = await borrowApi.getByRecordNo(scanRecordNo.value)
    if (res.data.code === 200) {
      scanRecord.value = res.data.data
    } else {
      ElMessage.error('未找到该借还记录')
      scanRecord.value = null
    }
  } catch {
    ElMessage.error('查询失败')
    scanRecord.value = null
  }
}

const handleConfirmBorrow = async () => {
  try {
    const res = await borrowApi.confirmBorrow(scanRecord.value!.id)
    if (res.data.code === 200) {
      ElMessage.success('借出确认成功')
      scanDialogVisible.value = false
      handleTabChange(activeTab.value)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '借出确认失败')
  }
}

const isOverdue = (row: BorrowRecordDto) => {
  if (!row.expectedReturnDate) return false
  return new Date(row.expectedReturnDate) < new Date()
}

const getStatusTagType = (status: string): string => {
  const map: Record<string, string> = {
    '待老师审批': 'info', '待管理员审批': 'warning',
    '待领取': 'warning', '已借出': 'primary', '已归还': 'success', '已拒绝': 'danger',
    '已逾期': 'danger', '续借审批中': 'warning', '管理员审批中': 'info', '待归还': 'warning'
  }
  return map[status] || ''
}

const getConditionType = (c: string): string => {
  if (c === '完好') return 'success'
  if (c === '损坏') return 'danger'
  return 'warning'
}

onMounted(() => { fetchMyRecords() })
</script>

<style scoped>
.borrow-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 8px; }
.text-gray { color: #909399; }
</style>
