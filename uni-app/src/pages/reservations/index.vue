<template>
  <view class="reservations-page">
    <!-- 顶部 Tab -->
    <view class="tab-bar">
      <view
        v-for="tab in tabs"
        :key="tab.value"
        class="tab-item"
        :class="{ active: currentTab === tab.value }"
        @tap="switchTab(tab.value)"
      >
        <Icon v-if="tab.icon" :name="tab.icon" :size="14" class="tab-icon" />
        <text>{{ tab.label }}</text>
        <view v-if="tab.count" class="tab-badge">{{ tab.count }}</view>
      </view>
    </view>

    <!-- 列表 -->
    <scroll-view
      class="list-area"
      scroll-y
      refresher-enabled
      @refresherrefresh="loadData"
      @scrolltolower="loadMore"
    >
      <view
        v-for="item in reservations"
        :key="item.id"
        class="res-card"
      >
        <view class="res-card__header">
          <view class="res-card__lab">
            <Icon name="flask" :size="14" color="#667eea" class="meta-icon" />
            <text>{{ item.labName || '实验室' }}</text>
          </view>
          <view class="status-tag" :class="'status--' + item.status">
            <Icon :name="statusIcon(item.status)" :size="12" class="status-icon" />
            <text>{{ statusLabel(item.status) }}</text>
          </view>
        </view>
        <view class="res-card__body">
          <view class="res-meta">
            <view class="meta-item">
              <Icon name="calendar" :size="12" class="meta-icon" />
              <text>{{ item.date || formatDate(item.useDate) }} 周{{ item.dayOfWeek || '-' }}</text>
            </view>
            <view class="meta-item">
              <Icon name="clock" :size="12" class="meta-icon" />
              <text>{{ item.timeSlot || formatPeriods(item.periodNumbers) }}</text>
            </view>
          </view>
          <text v-if="item.purpose || item.projectName" class="res-purpose">{{ item.purpose || item.projectName }}</text>
          <view class="res-info">
            <view class="res-info__text">
              <Icon name="user" :size="12" class="meta-icon" />
              <text>{{ item.applicantName || item.userName }}</text>
            </view>
            <text v-if="item.attendeeCount || item.memberCount" class="res-info__text">
              <Icon name="users" :size="12" class="meta-icon" />
              <text>共 {{ item.attendeeCount || item.memberCount }} 人</text>
            </text>
          </view>
          <text v-if="item.approvalComment" class="res-info__text">
            <Icon name="doc" :size="12" class="meta-icon" />
            <text>审批意见: {{ item.approvalComment }}</text>
          </text>
        </view>
        <view class="res-card__footer">
          <view class="res-no">
            <Icon name="doc" :size="12" class="meta-icon" />
            <text>{{ item.reservationNo || item.id }}</text>
          </view>
          <view class="res-actions">
            <view v-if="canCancel(item)" class="action-btn" @tap="cancelReservation(item)">
              <Icon name="close" :size="12" color="#f56c6c" />
              <text>取消预约</text>
            </view>
            <template v-if="canApprove(item)">
              <view class="action-btn action-btn--primary" @tap="approveReservation(item)">
                <Icon name="check" :size="12" color="#667eea" />
                <text>通过</text>
              </view>
              <view class="action-btn" @tap="rejectReservation(item)">
                <Icon name="close" :size="12" color="#f56c6c" />
                <text>拒绝</text>
              </view>
            </template>
            <view v-if="item.status === 'Approved' && authStore.isAdmin" class="action-btn action-btn--primary" @tap="checkIn(item)">
              <Icon name="signin" :size="12" color="#667eea" />
              <text>签到</text>
            </view>
          </view>
        </view>
      </view>

      <!-- 空状态 -->
      <view v-if="!isLoading && reservations.length === 0" class="empty">
        <Icon name="inbox" :size="48" color="#d0d0d0" />
        <text class="empty-text">{{ emptyText }}</text>
      </view>

      <view v-if="hasMore && reservations.length > 0" class="load-more">
        <Icon name="arrow-down" :size="12" color="#909399" />
        <text>加载更多...</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Icon from '@/components/Icon.vue'
import {
  getReservations,
  getMyReservations,
  getPendingReservations,
  approveReservation as approveApi,
  rejectReservation as rejectApi,
  cancelReservation as cancelApi,
} from '@/api/reservation'
import type { Reservation, ReservationStatus } from '@/types/reservation'
import { RESERVATION_STATUS_LABELS } from '@/types/reservation'

const authStore = useAuthStore()

type TabValue = ReservationStatus | 'all' | 'pending_approval'

const currentTab = ref<TabValue>('all')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const reservations = ref<Reservation[]>([])
const total = ref(0)

const isApprover = computed(() => authStore.isTeacher || authStore.isAdmin)

const emptyText = computed(() => {
  const map: Record<TabValue, string> = {
    all: '暂无预约记录',
    pending_approval: '暂无待审批预约',
    Pending: '暂无待审批预约',
    Approved: '暂无已通过预约',
    Rejected: '暂无已拒绝预约',
    Cancelled: '暂无已取消预约',
    InUse: '暂无使用中的预约',
    Completed: '暂无已完成的预约',
    NoShow: '暂无未签到的预约',
  }
  return map[currentTab.value]
})

const tabs = computed(() => {
  const base: { label: string; value: TabValue; icon: string; count: number | null }[] = [
    { label: '全部', value: 'all', icon: 'list', count: null },
  ]
  base.push({ label: '待审批', value: 'pending_approval', icon: 'bell', count: null })
  base.push({ label: '已通过', value: 'Approved', icon: 'circle-check', count: null })
  return base
})

const hasMore = computed(() => reservations.value.length < total.value)

function statusLabel(status: ReservationStatus | string): string {
  return RESERVATION_STATUS_LABELS[status as ReservationStatus] ?? status
}

function statusIcon(status: string): string {
  const map: Record<string, string> = {
    Pending: 'clock',
    Approved: 'circle-check',
    Rejected: 'circle-x',
    Cancelled: 'close',
    InUse: 'beaker',
    Completed: 'circle-check',
    NoShow: 'warning',
  }
  return map[status] || 'tag'
}

function formatDate(value: string | undefined): string {
  if (!value) return '-'
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return value
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
}

function formatPeriods(periods: number[] | undefined): string {
  if (!periods || periods.length === 0) return '-'
  return periods.map((p) => `第${p}节`).join('、')
}

function isOwn(item: Reservation): boolean {
  const userId = authStore.currentUser?.id
  return !!(userId && (item.applicantId === userId || item.userId === userId))
}

function canCancel(item: Reservation): boolean {
  return ['Pending', 'Approved'].includes(item.status) && isOwn(item)
}

function canApprove(item: Reservation): boolean {
  if (!isApprover.value) return false
  if (item.status !== 'Pending') return false
  if (isOwn(item)) return false
  return true
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
    let resp

    if (currentTab.value === 'pending_approval') {
      // 待审批 Tab
      if (isApprover.value) {
        resp = await getPendingReservations()
      } else {
        resp = await getMyReservations()
        const list = (resp?.items ?? []) as Reservation[]
        reservations.value = list.filter((r) => r.status === 'Pending')
        total.value = reservations.value.length
        return
      }
    } else if (currentTab.value === 'all') {
      if (authStore.isStudent) {
        resp = await getMyReservations()
      } else {
        resp = await getReservations()
      }
    } else {
      // 已通过 / 已拒绝 等状态过滤
      if (authStore.isStudent) {
        resp = await getMyReservations()
        const list = (resp?.items ?? []) as Reservation[]
        reservations.value = list.filter((r) => r.status === currentTab.value)
        total.value = reservations.value.length
        return
      } else {
        resp = await getReservations({ status: currentTab.value })
      }
    }

    reservations.value = resp?.items ?? []
    total.value = resp?.total ?? reservations.value.length
  } catch {
    // ignore
  } finally {
    isLoading.value = false
  }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  try {
    const resp = await getReservations({ page: page.value, pageSize })
    reservations.value.push(...(resp?.items ?? []))
    total.value = resp?.total ?? reservations.value.length
  } catch {
    // ignore
  }
}

async function approveReservation(item: Reservation) {
  uni.showModal({
    title: '审批通过',
    content: `确定通过 "${item.applicantName || item.userName}" 的 "${item.labName}" 预约吗？`,
    editable: true,
    placeholderText: '审批备注（可选）',
    success: async (res) => {
      if (!res.confirm) return
      try {
        uni.showLoading({ title: '处理中...' })
        await approveApi(item.id, res.content || '审批通过')
        uni.hideLoading()
        uni.showToast({ title: '已通过', icon: 'success' })
        await loadData()
      } catch {
        uni.hideLoading()
      }
    },
  })
}

async function rejectReservation(item: Reservation) {
  uni.showModal({
    title: '审批拒绝',
    content: `确定拒绝 "${item.applicantName || item.userName}" 的 "${item.labName}" 预约吗？`,
    editable: true,
    placeholderText: '请输入拒绝原因',
    success: async (res) => {
      if (!res.confirm) return
      try {
        uni.showLoading({ title: '处理中...' })
        await rejectApi(item.id, res.content || '不符合预约条件')
        uni.hideLoading()
        uni.showToast({ title: '已拒绝', icon: 'none' })
        await loadData()
      } catch {
        uni.hideLoading()
      }
    },
  })
}

async function cancelReservation(item: Reservation) {
  uni.showModal({
    title: '确认取消',
    content: `确定取消 "${item.labName}" 的预约吗？`,
    editable: true,
    placeholderText: '请输入取消原因（可选）',
    success: async (res) => {
      if (res.confirm) {
        try {
          uni.showLoading({ title: '处理中...' })
          await cancelApi(item.id, { reason: res.content || '用户取消' })
          uni.hideLoading()
          uni.showToast({ title: '已取消', icon: 'success' })
          await loadData()
        } catch {
          uni.hideLoading()
        }
      }
    },
  })
}

async function checkIn(_item: Reservation) {
  uni.showToast({ title: '签到功能开发中', icon: 'none' })
}

onMounted(async () => {
  await loadData()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.reservations-page {
  min-height: 100vh;
  background: #f5f7fa;
}

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
  min-width: 32rpx;
  text-align: center;
}

.list-area {
  height: calc(100vh - 88rpx);
  padding: 24rpx;
}

.res-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16rpx;
  }

  &__lab {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
    display: inline-flex;
    align-items: center;
    gap: 6rpx;
  }

  &__body {
    margin-bottom: 16rpx;
  }

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

  &.status--Pending { background: #fef0f0; color: #f56c6c; }
  &.status--Approved { background: #f0f9eb; color: #67c23a; }
  &.status--Rejected { background: #f5f5f5; color: #909399; }
  &.status--Cancelled { background: #f5f5f5; color: #909399; }
  &.status--InUse { background: #e8f4ff; color: #409eff; }
  &.status--Completed { background: #e8f4ff; color: #909399; }
  &.status--NoShow { background: #fef0f0; color: #f56c6c; }
}

.tab-icon { margin-right: 6rpx; }
.status-icon { vertical-align: middle; }
.meta-icon { margin-right: 4rpx; vertical-align: middle; }

.res-meta {
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

.res-purpose {
  font-size: 26rpx;
  color: #606266;
  display: block;
  margin-bottom: 8rpx;
}

.res-info {
  display: flex;
  gap: 16rpx;

  &__text {
    font-size: 24rpx;
    color: #909399;
    display: inline-flex;
    align-items: center;
    gap: 4rpx;
  }
}

.res-no {
  font-size: 22rpx;
  color: #c0c4cc;
  display: inline-flex;
  align-items: center;
  gap: 4rpx;
}

.res-actions {
  display: flex;
  gap: 12rpx;
}

.action-btn {
  font-size: 24rpx;
  color: #f56c6c;
  padding: 8rpx 20rpx;
  border: 1rpx solid #f56c6c;
  border-radius: 999rpx;
  display: inline-flex;
  align-items: center;
  gap: 6rpx;

  &--primary {
    color: $primary;
    border-color: $primary;
  }
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
</style>
