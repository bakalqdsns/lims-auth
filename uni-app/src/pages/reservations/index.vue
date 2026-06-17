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
          <text class="res-card__lab">{{ item.labName }}</text>
          <view class="status-tag" :class="'status--' + item.status">{{ statusLabel(item.status) }}</view>
        </view>
        <view class="res-card__body">
          <view class="res-meta">
            <text class="meta-item">&#xe6d0; {{ item.date }}</text>
            <text class="meta-item">&#xe6d2; {{ item.timeSlot }}</text>
          </view>
          <text v-if="item.purpose" class="res-purpose">{{ item.purpose }}</text>
          <view class="res-info">
            <text class="res-info__text">{{ item.userName }}</text>
            <text class="res-info__text">{{ item.attendeeCount ? '共' + item.attendeeCount + '人' : '' }}</text>
          </view>
        </view>
        <view class="res-card__footer">
          <text class="res-no">{{ item.reservationNo }}</text>
          <view class="res-actions">
            <view v-if="canCancel(item)" class="action-btn" @tap="cancelReservation(item)">
              取消预约
            </view>
            <view v-if="item.status === 'approved' && authStore.isAdmin" class="action-btn action-btn--primary" @tap="checkIn(item)">
              签到
            </view>
          </view>
        </view>
      </view>

      <!-- 空状态 -->
      <view v-if="!isLoading && reservations.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无预约记录</text>
      </view>

      <view v-if="hasMore && reservations.length > 0" class="load-more">
        <text>加载更多...</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, reactive } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { getReservations, cancelReservation as cancelApi } from '@/api/reservation'
import type { Reservation, ReservationStatus } from '@/types/reservation'

const authStore = useAuthStore()

const currentTab = ref<ReservationStatus | 'all'>('all')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const reservations = ref<Reservation[]>([])
const total = ref(0)
const tabCounts = reactive({ pending: 0, approved: 0, all: 0 })

const tabs = computed(() => [
  { label: '全部', value: 'all' as const, count: tabCounts.all || null },
  { label: '待审批', value: 'pending' as const, count: tabCounts.pending || null },
  { label: '已通过', value: 'approved' as const, count: tabCounts.approved || null },
])

const hasMore = computed(() => reservations.value.length < total.value)

function statusLabel(status: ReservationStatus | string): string {
  const map: Record<string, string> = {
    pending: '待审批',
    approved: '已通过',
    rejected: '已拒绝',
    cancelled: '已取消',
    in_use: '使用中',
    completed: '已完成',
    no_show: '未签到',
  }
  return map[status] || status
}

function canCancel(item: Reservation): boolean {
  return ['pending', 'approved'].includes(item.status) && item.userId === authStore.currentUser?.id
}

async function switchTab(tab: ReservationStatus | 'all') {
  currentTab.value = tab
  page.value = 1
  await loadData()
}

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const query: Record<string, string | number> = { page: 1, pageSize }
    if (currentTab.value !== 'all') {
      query.status = currentTab.value
    }
    const resp = await getReservations(query)
    reservations.value = resp?.items ?? []
    total.value = resp?.total ?? 0
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
    const query: Record<string, string | number> = { page: page.value, pageSize }
    if (currentTab.value !== 'all') {
      query.status = currentTab.value
    }
    const resp = await getReservations(query)
    reservations.value.push(...(resp?.items ?? []))
  } catch {
    // ignore
  }
}

async function cancelReservation(item: Reservation) {
  uni.showModal({
    title: '确认取消',
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

async function checkIn(item: Reservation) {
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

  &.status--pending { background: #fef0f0; color: #f56c6c; }
  &.status--approved { background: #f0f9eb; color: #67c23a; }
  &.status--rejected { background: #f5f5f5; color: #909399; }
  &.status--cancelled { background: #f5f5f5; color: #909399; }
  &.status--in_use { background: #e8f4ff; color: #409eff; }
  &.status--completed { background: #e8f4ff; color: #909399; }
  &.status--no_show { background: #fef0f0; color: #f56c6c; }
}

.res-meta {
  display: flex;
  gap: 24rpx;
  margin-bottom: 8rpx;
}

.meta-item {
  font-size: 26rpx;
  color: #909399;
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
  }
}

.res-no {
  font-size: 22rpx;
  color: #c0c4cc;
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
}
</style>
