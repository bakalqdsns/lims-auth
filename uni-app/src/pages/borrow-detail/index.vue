<template>
  <view class="borrow-detail-page">
    <view v-if="isLoading" class="loading-state"><text>加载中...</text></view>

    <template v-else-if="record">
      <!-- 状态头部 -->
      <view class="detail-header" :class="'header--' + record.status">
        <text class="detail-header__status">{{ statusLabel(record.status) }}</text>
        <text class="detail-header__no">{{ record.recordNo }}</text>
      </view>

      <!-- 设备信息 -->
      <view class="section">
        <text class="section-title">设备信息</text>
        <view class="info-card">
          <view class="info-row"><text class="info-label">设备名称</text><text class="info-value">{{ record.equipmentName }}</text></view>
          <view class="info-row"><text class="info-label">设备编号</text><text class="info-value">{{ record.equipmentCode }}</text></view>
        </view>
      </view>

      <!-- 借用信息 -->
      <view class="section">
        <text class="section-title">借用信息</text>
        <view class="info-card">
          <view class="info-row"><text class="info-label">借用人</text><text class="info-value">{{ record.userName }}</text></view>
          <view class="info-row"><text class="info-label">借用日期</text><text class="info-value">{{ record.borrowDate }}</text></view>
          <view class="info-row"><text class="info-label">应还日期</text><text class="info-value">{{ record.expectedReturnDate }}</text></view>
          <view v-if="record.actualReturnDate" class="info-row"><text class="info-label">实还日期</text><text class="info-value">{{ record.actualReturnDate }}</text></view>
          <view v-if="record.purpose" class="info-row"><text class="info-label">借用用途</text><text class="info-value">{{ record.purpose }}</text></view>
          <view v-if="record.approverName" class="info-row"><text class="info-label">审批人</text><text class="info-value">{{ record.approverName }}</text></view>
        </view>
      </view>

      <!-- 流程时间线 -->
      <view class="section" v-if="flowSteps && flowSteps.length > 0">
        <text class="section-title">审批流程</text>
        <view class="timeline">
          <view v-for="(step, index) in flowSteps" :key="index" class="timeline-item">
            <view class="timeline-dot" :class="index === 0 ? 'dot--active' : 'dot--done'" />
            <view v-if="index < flowSteps.length - 1" class="timeline-line" />
            <view class="timeline-content">
              <text class="timeline-action">{{ step.action }}</text>
              <text class="timeline-operator">{{ step.operatorName }} {{ step.operatedAt }}</text>
              <text v-if="step.remark" class="timeline-remark">{{ step.remark }}</text>
            </view>
          </view>
        </view>
      </view>

      <!-- 操作按钮 -->
      <view class="bottom-bar">
        <button v-if="record.status === 'borrowed'" class="action-btn action-btn--return" @tap="submitReturn">提交归还</button>
        <button v-if="record.status === 'pending' && isAdmin" class="action-btn action-btn--primary" @tap="approveRecord">审批通过</button>
        <button v-if="record.status === 'pending' && isAdmin" class="action-btn action-btn--danger" @tap="rejectRecord">拒绝</button>
        <button v-if="record.status === 'borrowed'" class="action-btn action-btn--renew" @tap="showRenewSheet = true">申请续借</button>
      </view>
    </template>

    <!-- 续借弹窗 -->
    <view v-if="showRenewSheet" class="overlay" @tap="showRenewSheet = false">
      <view class="sheet" @tap.stop>
        <view class="sheet__header">
          <text class="sheet__title">申请续借</text>
          <text class="sheet__close" @tap="showRenewSheet = false">&#xe6c7;</text>
        </view>
        <view class="sheet__body">
          <view class="form-item">
            <text class="form-label">新的归还日期</text>
            <picker mode="date" :value="newReturnDate" @change="(e: any) => newReturnDate = e.detail.value">
              <view class="picker-val">{{ newReturnDate || '请选择日期' }}</view>
            </picker>
          </view>
        </view>
        <view class="sheet__footer">
          <button class="submit-btn" :disabled="!newReturnDate" @tap="confirmRenew">确认续借</button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { getBorrowRecordById, getBorrowRecordFlow, submitReturn as submitReturnApi, renewBorrow, adminApprove } from '@/api/borrow-record'
import type { BorrowRecord, BorrowStatus } from '@/types/borrow'

interface BorrowFlowStepData {
  step: number
  action: string
  operatorId: string
  operatorName: string
  operatedAt: string
  remark?: string
}

const authStore = useAuthStore()
const isLoading = ref(true)
const record = ref<BorrowRecord | null>(null)
const flowSteps = ref<BorrowFlowStepData[]>([])
const showRenewSheet = ref(false)
const newReturnDate = ref('')

const isAdmin = computed(() => authStore.isAdmin)

function statusLabel(status: BorrowStatus | string): string {
  const map: Record<string, string> = {
    pending: '待审批', approved: '已通过', borrowed: '已借出', returning: '归还中',
    returned: '已归还', renewing: '续借中', rejected: '已拒绝', cancelled: '已取消', overdue: '已逾期',
  }
  return map[status] || status
}

async function submitReturn() {
  uni.showModal({
    title: '确认归还',
    content: '确定要提交归还申请吗？',
    success: async (res) => {
      if (res.confirm && record.value) {
        try {
          uni.showLoading({ title: '提交中...' })
          await submitReturnApi(record.value.id)
          uni.hideLoading()
          uni.showToast({ title: '已提交归还', icon: 'success' })
          await loadDetail()
        } catch { uni.hideLoading() }
      }
    },
  })
}

async function approveRecord() {
  if (!record.value) return
  try {
    uni.showLoading({ title: '审批中...' })
    await adminApprove(record.value.id, true)
    uni.hideLoading()
    uni.showToast({ title: '已通过', icon: 'success' })
    await loadDetail()
  } catch { uni.hideLoading() }
}

async function rejectRecord() {
  if (!record.value) return
  uni.showModal({
    title: '拒绝申请',
    editable: true,
    placeholderText: '请输入拒绝原因（可选）',
    success: async (res) => {
      if (res.confirm && record.value) {
        try {
          uni.showLoading({ title: '提交中...' })
          await adminApprove(record.value.id, false, res.content || undefined)
          uni.hideLoading()
          uni.showToast({ title: '已拒绝', icon: 'none' })
          await loadDetail()
        } catch { uni.hideLoading() }
      }
    },
  })
}

async function confirmRenew() {
  if (!record.value || !newReturnDate.value) return
  try {
    uni.showLoading({ title: '提交中...' })
    await renewBorrow(record.value.id, newReturnDate.value)
    uni.hideLoading()
    uni.showToast({ title: '续借申请已提交', icon: 'success' })
    showRenewSheet.value = false
    await loadDetail()
  } catch { uni.hideLoading() }
}

async function loadDetail() {
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1] as { options?: { id?: string } }
  const id = currentPage.options?.id || '1'

  try {
    record.value = await getBorrowRecordById(id)
    try {
      const flow = await getBorrowRecordFlow(id) as unknown as { steps: BorrowFlowStepData[] }
      flowSteps.value = flow?.steps ?? []
    } catch {
      // ignore flow error
    }
  } finally {
    isLoading.value = false
  }
}

onMounted(async () => {
  await loadDetail()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.borrow-detail-page { min-height: 100vh; background: #f5f7fa; padding-bottom: 200rpx; }

.loading-state { display: flex; align-items: center; justify-content: center; padding: 200rpx; color: #909399; font-size: 28rpx; }

.detail-header {
  padding: 48rpx 32rpx;
  text-align: center;

  &__status { font-size: 40rpx; font-weight: bold; color: #fff; display: block; margin-bottom: 12rpx; }
  &__no { font-size: 24rpx; color: rgba(255, 255, 255, 0.7); }

  &.header--pending { background: linear-gradient(135deg, #e6a23c, #f56c6c); }
  &.header--borrowed, &.header--approved { background: linear-gradient(135deg, #67c23a, #85ce61); }
  &.header--returned { background: linear-gradient(135deg, #909399, #a6a9ad); }
  &.header--overdue { background: linear-gradient(135deg, #f56c6c, #f78989); }
  &.header--rejected { background: linear-gradient(135deg, #c0c4cc, #d3d6db); }
  &.header--cancelled { background: linear-gradient(135deg, #909399, #a6a9ad); }
}

.section { padding: 0 24rpx 24rpx; }

.section-title { font-size: 28rpx; font-weight: bold; color: #303133; display: block; margin-bottom: 16rpx; }

.info-card { background: #fff; border-radius: 24rpx; padding: 8rpx 0; box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06); }

.info-row { display: flex; padding: 20rpx 28rpx; border-bottom: 1rpx solid #f5f7fa; &:last-child { border-bottom: none; } }

.info-label { width: 160rpx; font-size: 26rpx; color: #909399; }
.info-value { flex: 1; font-size: 26rpx; color: #303133; }

.timeline { background: #fff; border-radius: 24rpx; padding: 32rpx; box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06); }

.timeline-item { display: flex; gap: 20rpx; position: relative; padding-bottom: 32rpx; &:last-child { padding-bottom: 0; } }

.timeline-dot { width: 24rpx; height: 24rpx; border-radius: 50%; flex-shrink: 0; margin-top: 4rpx;
  &.dot--active { background: $primary; }
  &.dot--done { background: #67c23a; }
}

.timeline-line { position: absolute; left: 11rpx; top: 28rpx; bottom: 0; width: 2rpx; background: #e4e7ed; }

.timeline-content { display: flex; flex-direction: column; gap: 6rpx; }

.timeline-action { font-size: 28rpx; color: #303133; font-weight: 500; }
.timeline-operator { font-size: 24rpx; color: #909399; }
.timeline-remark { font-size: 24rpx; color: #606266; }

.bottom-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 24rpx;
  background: #fff;
  box-shadow: 0 -4rpx 16rpx rgba(0, 0, 0, 0.06);
  display: flex;
  gap: 16rpx;
  flex-wrap: wrap;
}

.action-btn {
  flex: 1;
  min-width: 200rpx;
  height: 80rpx;
  border-radius: 16rpx;
  font-size: 28rpx;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  margin: 0;
  &::after { border: none; }

  &--primary { background: $primary; color: #fff; }
  &--danger { background: #f56c6c; color: #fff; }
  &--return { background: #67c23a; color: #fff; }
  &--renew { background: #e6a23c; color: #fff; }
}

.overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0, 0, 0, 0.5); z-index: 999; display: flex; align-items: flex-end; }

.sheet {
  width: 100%;
  max-height: 60vh;
  background: #fff;
  border-radius: 32rpx 32rpx 0 0;
  display: flex;
  flex-direction: column;

  &__header { display: flex; align-items: center; justify-content: space-between; padding: 32rpx; border-bottom: 1rpx solid #f0f0f0; }
  &__title { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__close { font-size: 36rpx; color: #909399; }
  &__body { flex: 1; padding: 32rpx; max-height: 35vh; }
  &__footer { padding: 24rpx 32rpx; border-top: 1rpx solid #f0f0f0; }
}

.form-item { margin-bottom: 28rpx; }
.form-label { font-size: 28rpx; color: #606266; display: block; margin-bottom: 12rpx; }

.picker-val {
  width: 100%;
  height: 80rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  display: flex;
  align-items: center;
}

.submit-btn {
  width: 100%;
  height: 88rpx;
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  border-radius: 16rpx;
  color: #fff;
  font-size: 32rpx;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  &::after { border: none; }
  &[disabled] { background: #d0d0d0; color: #909399; }
}
</style>
