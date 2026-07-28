<template>
  <view class="borrow-page">
    <!-- 顶部 Tab -->
    <view class="tab-bar">
      <view
        v-for="tab in tabs"
        :key="tab.value"
        class="tab-item"
        :class="{ active: currentTab === tab.value }"
        @tap="switchTab(tab.value)"
      >
        <Icon v-if="tab.icon" :name="tab.icon" :size="16" class="tab-icon" />
        <text>{{ tab.label }}</text>
        <view v-if="tab.count" class="tab-badge">{{ tab.count }}</view>
      </view>
    </view>

    <!-- 列表 -->
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData" @scrolltolower="loadMore">
      <view v-for="item in records" :key="item.id" class="borrow-card" @tap="goDetail(String(item.id))">
        <view class="borrow-card__header">
          <view class="borrow-card__equip">
            <text class="borrow-card__name">{{ item.equipmentName }}</text>
            <text class="borrow-card__code">{{ item.equipmentCode }}</text>
          </view>
          <view class="status-tag" :class="'status--' + statusClass(item.status)">
            <Icon :name="statusIcon(item.status)" :size="12" class="status-icon" />
            <text>{{ statusLabel(item.status) }}</text>
          </view>
        </view>
        <view class="borrow-card__body">
          <view class="borrow-meta">
            <view class="meta-item">
              <Icon name="calendar" :size="12" class="meta-icon" />
              <text>借出: {{ item.borrowDate }}</text>
            </view>
            <view class="meta-item">
              <Icon name="clock" :size="12" class="meta-icon" />
              <text>应还: {{ item.expectedReturnDate }}</text>
            </view>
          </view>
          <text v-if="item.purpose" class="borrow-purpose">{{ item.purpose }}</text>
        </view>
        <view class="borrow-card__footer">
          <view class="borrow-no">
            <Icon name="doc" :size="12" class="meta-icon" />
            <text>{{ item.recordNo }}</text>
          </view>
          <view v-if="currentTab === 'pending_approval' && (canApproveAsSupervisor || canApproveAsAdmin)" class="approve-actions">
            <view class="approve-btn approve-btn--primary" @tap.stop="approveRecord(item)">
              <Icon name="check" :size="12" color="#67c23a" />
              <text>通过</text>
            </view>
            <view class="approve-btn approve-btn--danger" @tap.stop="rejectRecord(item)">
              <Icon name="close" :size="12" color="#f56c6c" />
              <text>拒绝</text>
            </view>
          </view>
          <view v-else class="borrow-detail">
            <text>查看详情</text>
            <Icon name="chevron-right" :size="12" color="#667eea" />
          </view>
        </view>
      </view>

      <view v-if="!isLoading && records.length === 0" class="empty">
        <Icon name="inbox" :size="48" color="#d0d0d0" />
        <text class="empty-text">暂无借用记录</text>
      </view>

      <view v-if="hasMore && records.length > 0" class="load-more">
        <Icon name="arrow-down" :size="12" color="#909399" />
        <text>加载更多...</text>
      </view>
      <view :style="{ height: '40px' }" />
    </scroll-view>

    <!-- 借用按钮 -->
    <view class="bottom-bar">
      <button class="new-borrow-btn" @tap="showBorrowSheet = true">
        <Icon name="plus" :size="14" color="#fff" />
        <text>新建借用</text>
      </button>
    </view>

    <!-- 新建借用弹窗 -->
    <view v-if="showBorrowSheet" class="borrow-overlay" @tap="showBorrowSheet = false">
      <view class="borrow-sheet" @tap.stop>
        <view class="borrow-sheet__header">
          <text class="borrow-sheet__title">新建借用</text>
          <view class="borrow-sheet__close" @tap="showBorrowSheet = false">
            <Icon name="close" :size="16" color="#909399" />
          </view>
        </view>
        <scroll-view class="borrow-sheet__body" scroll-y>
          <view class="form-item">
            <text class="form-label">选择设备</text>
            <picker mode="selector" :range="equipmentOptions" range-key="name" @change="onEquipChange">
              <view class="picker-val">
                <Icon name="flask" :size="14" color="#909399" class="meta-icon" />
                <text>{{ newBorrow.equipmentName || '请选择设备' }}</text>
              </view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">借用日期</text>
            <picker mode="date" :value="newBorrow.borrowDate" @change="onBorrowDateChange">
              <view class="picker-val">
                <Icon name="calendar" :size="14" color="#909399" class="meta-icon" />
                <text>{{ newBorrow.borrowDate || '请选择日期' }}</text>
              </view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">预计归还日期</text>
            <picker mode="date" :value="newBorrow.expectedReturnDate" @change="onReturnDateChange">
              <view class="picker-val">
                <Icon name="calendar" :size="14" color="#909399" class="meta-icon" />
                <text>{{ newBorrow.expectedReturnDate || '请选择日期' }}</text>
              </view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">借用用途</text>
            <textarea v-model="newBorrow.purpose" class="form-textarea" placeholder="请输入借用用途" :maxlength="200" />
          </view>
          <view class="form-item">
            <text class="form-label">备注</text>
            <input v-model="newBorrow.remark" class="form-input" placeholder="备注信息（可选）" />
          </view>
        </scroll-view>
        <view class="borrow-sheet__footer">
          <button class="submit-btn" :disabled="!canSubmit" @tap="submitBorrow">
            <Icon name="check" :size="14" color="#fff" />
            <text>提交申请</text>
          </button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Icon from '@/components/Icon.vue'
import {
  getMyBorrowRecords,
  getPendingBorrowApprovals,
  createBorrowRequest,
  supervisorApprove,
  adminApprove,
} from '@/api/borrow-record'
import { getEquipments } from '@/api/equipment'
import type { BorrowRecord, BorrowStatus, CreateBorrowRequest } from '@/types/borrow'
import { BORROW_STATUS as BorrowStatusConst, BORROW_STATUS_LABELS, borrowStatusClass } from '@/types/borrow'
import type { Equipment } from '@/types/equipment'

const authStore = useAuthStore()
const BorrowStatus = BorrowStatusConst

type TabValue = 'all' | 'pending_approval' | 'borrowed' | 'pending' | 'overdue'

const currentTab = ref<TabValue>('all')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const records = ref<BorrowRecord[]>([])
const total = ref(0)
const showBorrowSheet = ref(false)
const allEquipments = ref<Equipment[]>([])

const newBorrow = reactive({
  equipmentId: '' as string,
  equipmentName: '',
  borrowDate: '',
  expectedReturnDate: '',
  purpose: '',
  remark: '',
})

const tabs = computed(() => {
  const base: { label: string; value: TabValue; icon: string; count: number | null }[] = [
    { label: '全部', value: 'all', icon: 'list', count: null },
    { label: '我借的', value: 'borrowed', icon: 'flask', count: null },
    { label: '待归还', value: 'pending', icon: 'clock', count: null },
    { label: '逾期', value: 'overdue', icon: 'warning', count: null },
  ]
  if (authStore.isTeacher || authStore.isAdmin) {
    base.push({ label: '待审批', value: 'pending_approval', icon: 'bell', count: null })
  }
  return base
})

const canApproveAsSupervisor = computed(() => authStore.isTeacher && !authStore.isAdmin)
const canApproveAsAdmin = computed(() => authStore.isAdmin)

const equipmentOptions = computed(() =>
  allEquipments.value.map((e) => ({ id: e.id, name: `${e.name} (${e.code}) 可用:${e.availableQuantity}` }))
)

const hasMore = computed(() => records.value.length < total.value)

const canSubmit = computed(
  () => newBorrow.equipmentId && newBorrow.borrowDate && newBorrow.expectedReturnDate && newBorrow.purpose.trim()
)

function statusClass(status: string): string {
  return borrowStatusClass(status)
}

function statusLabel(status: BorrowStatus | string): string {
  if (status in BORROW_STATUS_LABELS) return BORROW_STATUS_LABELS[status as BorrowStatus]
  const map: Record<string, string> = {
    pending: '待审批',
    pending_supervisor: '待导师审批',
    pending_admin: '待管理员审批',
    PendingSupervisor: '待导师审批',
    PendingAdmin: '待管理员审批',
    approved: '已通过',
    borrowed: '已借出',
    returning: '归还中',
    returned: '已归还',
    renewing: '续借中',
    rejected: '已拒绝',
    cancelled: '已取消',
    overdue: '已逾期',
  }
  return map[status] || status
}

function statusIcon(status: string): string {
  const iconMap: Record<string, string> = {
    [BorrowStatus.PendingTeacherApproval]: 'clock',
    [BorrowStatus.PendingAdminApproval]: 'clock',
    [BorrowStatus.AwaitingPickup]: 'clock',
    [BorrowStatus.Borrowed]: 'arrow-right',
    [BorrowStatus.AwaitingReturn]: 'arrow-left',
    [BorrowStatus.ReturnPending]: 'arrow-left',
    [BorrowStatus.Returned]: 'circle-check',
    [BorrowStatus.Rejected]: 'circle-x',
    [BorrowStatus.Overdue]: 'warning',
    [BorrowStatus.RenewPending]: 'clock',
    pending: 'clock',
    pending_supervisor: 'user',
    pending_admin: 'user',
    PendingSupervisor: 'user',
    PendingAdmin: 'user',
    approved: 'circle-check',
    borrowed: 'arrow-right',
    returning: 'arrow-left',
    returned: 'circle-check',
    renewing: 'clock',
    rejected: 'circle-x',
    cancelled: 'close',
    overdue: 'warning',
  }
  return iconMap[status] || 'tag'
}

async function switchTab(tab: TabValue) {
  currentTab.value = tab
  page.value = 1
  await loadData()
}

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    if (currentTab.value === 'pending_approval') {
      const resp = await getPendingBorrowApprovals()
      records.value = resp?.items ?? []
      total.value = resp?.total ?? records.value.length
      return
    }

    // 后端 status 是中文字符串 (BorrowStatus 常量), Tab 内部值需要映射
    const status = currentTab.value === 'all'
      ? undefined
      : currentTab.value === 'pending'
        ? BorrowStatus.AwaitingReturn
        : currentTab.value

    const resp = await getMyBorrowRecords(status)
    records.value = resp?.items ?? []
    total.value = resp?.total ?? records.value.length
  } catch {
    // ignore
  } finally {
    isLoading.value = false
  }
}

async function loadMore() {
  if (currentTab.value === 'pending_approval') return
  await loadData()
}

function goDetail(id: string) {
  uni.navigateTo({ url: `/pages/borrow-detail/index?id=${id}` })
}

function onEquipChange(e: { detail: { value: number } }) {
  const eq = allEquipments.value[e.detail.value]
  newBorrow.equipmentId = eq.id
  newBorrow.equipmentName = eq.name
}

function onBorrowDateChange(e: { detail: { value: string } }) {
  newBorrow.borrowDate = e.detail.value
}

function onReturnDateChange(e: { detail: { value: string } }) {
  newBorrow.expectedReturnDate = e.detail.value
}

async function submitBorrow() {
  if (!canSubmit.value || !newBorrow.equipmentId) return
  try {
    uni.showLoading({ title: '提交中...' })
    await createBorrowRequest({
      equipmentId: newBorrow.equipmentId,
      borrowDate: newBorrow.borrowDate,
      expectedReturnDate: newBorrow.expectedReturnDate,
      purpose: newBorrow.purpose,
      remark: newBorrow.remark || undefined,
    })
    uni.hideLoading()
    uni.showToast({ title: '申请已提交', icon: 'success' })
    showBorrowSheet.value = false
    await loadData()
  } catch {
    uni.hideLoading()
  }
}

async function approveRecord(item: BorrowRecord) {
  uni.showModal({
    title: '审批通过',
    content: `确定通过 "${item.equipmentName}" 的借用申请吗？`,
    editable: true,
    placeholderText: '审批备注（可选）',
    success: async (res) => {
      if (!res.confirm) return
      try {
        uni.showLoading({ title: '审批中...' })
        const remark = res.content || undefined
        if (canApproveAsSupervisor.value) {
          await supervisorApprove(String(item.id), true, remark)
        } else if (canApproveAsAdmin.value) {
          await adminApprove(String(item.id), true, remark)
        }
        uni.hideLoading()
        uni.showToast({ title: '已通过', icon: 'success' })
        await loadData()
      } catch {
        uni.hideLoading()
      }
    },
  })
}

async function rejectRecord(item: BorrowRecord) {
  uni.showModal({
    title: '审批拒绝',
    content: `确定拒绝 "${item.equipmentName}" 的借用申请吗？`,
    editable: true,
    placeholderText: '请输入拒绝原因',
    success: async (res) => {
      if (!res.confirm) return
      try {
        uni.showLoading({ title: '提交中...' })
        const remark = res.content || '不符合借用条件'
        if (canApproveAsSupervisor.value) {
          await supervisorApprove(String(item.id), false, remark)
        } else if (canApproveAsAdmin.value) {
          await adminApprove(String(item.id), false, remark)
        }
        uni.hideLoading()
        uni.showToast({ title: '已拒绝', icon: 'none' })
        await loadData()
      } catch {
        uni.hideLoading()
      }
    },
  })
}

onMounted(async () => {
  if (authStore.isTeacher || authStore.isAdmin) {
    currentTab.value = 'pending_approval'
  }
  await loadData()
  try {
    const resp = await getEquipments({ pageSize: 100 })
    allEquipments.value = (resp?.items ?? []).filter((e) => e.availableQuantity > 0)
  } catch {
    // ignore
  }
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.borrow-page { min-height: 100vh; background: #f5f7fa; }

.tab-bar {
  display: flex;
  background: #fff;
  padding: 0 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.tab-item {
  flex: 1;
  height: 88rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8rpx;
  font-size: 28rpx;
  color: #606266;
  position: relative;

  &.active {
    color: $primary;
    font-weight: bold;
    &::after {
      content: '';
      position: absolute;
      bottom: 0;
      left: 50%;
      transform: translateX(-50%);
      width: 48rpx;
      height: 6rpx;
      background: $primary;
      border-radius: 3rpx;
    }
  }
}

.tab-badge {
  background: #f56c6c;
  color: #fff;
  font-size: 20rpx;
  padding: 2rpx 10rpx;
  border-radius: 999rpx;
}

.list-area { height: calc(100vh - 88rpx); padding: 24rpx; }

.borrow-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    margin-bottom: 16rpx;
  }

  &__equip {}

  &__name {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
    display: block;
    margin-bottom: 6rpx;
  }

  &__code {
    font-size: 24rpx;
    color: #909399;
  }

  &__body { margin-bottom: 16rpx; }

  &__footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-top: 16rpx;
    border-top: 1rpx solid #f0f0f0;
  }
}

.status-tag {
  font-size: 24rpx;
  padding: 4rpx 16rpx;
  border-radius: 999rpx;
  display: inline-flex;
  align-items: center;
  gap: 6rpx;
  &.status--pending,
  &.status--pending-teacher,
  &.status--pending-admin,
  &.status--pending-supervisor,
  &.status--renew-pending,
  &.status--return-pending { background: #fef0f0; color: #f56c6c; }
  &.status--approved,
  &.status--borrowed { background: #f0f9eb; color: #67c23a; }
  &.status--awaiting-pickup,
  &.status--awaiting-return,
  &.status--returning { background: #fdf6ec; color: #e6a23c; }
  &.status--returned { background: #e8f4ff; color: #909399; }
  &.status--overdue { background: #f56c6c; color: #fff; }
  &.status--rejected,
  &.status--cancelled { background: #f5f5f5; color: #909399; }
  &.status--default { background: #f5f5f5; color: #909399; }
}

.tab-icon { margin-right: 6rpx; }
.status-icon { vertical-align: middle; }
.meta-icon { margin-right: 4rpx; vertical-align: middle; }

.borrow-meta {
  display: flex;
  gap: 24rpx;
  margin-bottom: 8rpx;
}

.meta-item {
  font-size: 26rpx;
  color: #909399;
  display: inline-flex;
  align-items: center;
  gap: 4rpx;
}

.borrow-purpose {
  font-size: 26rpx;
  color: #606266;
  display: block;
}

.borrow-no {
  font-size: 22rpx;
  color: #c0c4cc;
  display: inline-flex;
  align-items: center;
  gap: 4rpx;
}

.borrow-detail {
  font-size: 24rpx;
  color: $primary;
  display: inline-flex;
  align-items: center;
  gap: 4rpx;
}

.approve-actions { display: flex; gap: 16rpx; }

.approve-btn {
  font-size: 24rpx;
  padding: 6rpx 20rpx;
  border-radius: 999rpx;
  border: 1rpx solid transparent;
  display: inline-flex;
  align-items: center;
  gap: 6rpx;

  &--primary { color: #67c23a; border-color: #67c23a; }
  &--danger  { color: #f56c6c; border-color: #f56c6c; }
}

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 120rpx 0;
  gap: 16rpx;
}

.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }

.load-more {
  text-align: center;
  padding: 24rpx;
  font-size: 24rpx;
  color: #909399;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8rpx;
}

.bottom-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 24rpx;
  background: #fff;
  box-shadow: 0 -4rpx 16rpx rgba(0, 0, 0, 0.06);
}

.new-borrow-btn {
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
  gap: 12rpx;
  border: none;
  &::after { border: none; }
}

.borrow-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 999;
  display: flex;
  align-items: flex-end;
}

.borrow-sheet {
  width: 100%;
  max-height: 80vh;
  background: #fff;
  border-radius: 32rpx 32rpx 0 0;
  display: flex;
  flex-direction: column;

  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 32rpx;
    border-bottom: 1rpx solid #f0f0f0;
  }

  &__title { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__close { font-size: 36rpx; color: #909399; }

  &__body {
    flex: 1;
    padding: 32rpx;
    max-height: 50vh;
  }

  &__footer {
    padding: 24rpx 32rpx;
    border-top: 1rpx solid #f0f0f0;
  }
}

.form-item { margin-bottom: 28rpx; }
.form-label { font-size: 28rpx; color: #606266; display: block; margin-bottom: 12rpx; }

.form-input,
.picker-val {
  width: 100%;
  height: 80rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  border: 2rpx solid transparent;
}

.form-textarea {
  width: 100%;
  min-height: 160rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 20rpx 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  border: 2rpx solid transparent;
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
  gap: 12rpx;
  border: none;
  &::after { border: none; }
  &[disabled] { background: #d0d0d0; color: #909399; }
}
</style>
