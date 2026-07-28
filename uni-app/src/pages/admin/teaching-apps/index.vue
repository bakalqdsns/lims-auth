<template>
  <view class="approval-page">
    <!-- Tab -->
    <view class="tab-bar">
      <view
        v-for="tab in tabs"
        :key="tab.key"
        class="tab-item"
        :class="{ active: currentTab === tab.key }"
        @tap="switchTab(tab.key)"
      >
        <Icon :name="tab.icon" :size="12" class="tab-icon" />
        <text>{{ tab.label }}</text>
        <view v-if="tab.key === 'pending' && pendingCount > 0" class="tab-badge">
          {{ pendingCount }}
        </view>
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
      <view v-for="app in applications" :key="app.id" class="app-card">
        <view class="app-card__header">
          <text class="app-title">{{ app.courseName }}</text>
          <view class="status-tag" :class="'status--' + app.status">
            <Icon :name="statusIcon(app.status)" :size="10" class="status-icon" />
            <text>{{ statusLabel(app.status) }}</text>
          </view>
        </view>
        <view class="app-card__body">
          <text class="app-meta">申请人: {{ app.applicantName }}</text>
          <text class="app-meta">班级: {{ app.className }}</text>
          <text v-if="app.majorName" class="app-meta">专业: {{ app.majorName }}</text>
          <text class="app-meta">期望实验室: {{ app.expectedLabName || '未指定' }}</text>
          <text class="app-meta">周期: 第{{ app.startWeek }}-{{ app.endWeek }}周 星期{{ app.dayOfWeek }}</text>
          <text class="app-meta">节次: {{ formatPeriods(app.periodNumbers) }}</text>
          <text v-if="app.remark" class="app-meta">备注: {{ app.remark }}</text>
          <text v-if="app.approvalComment" class="app-meta app-meta--comment">
            审批意见: {{ app.approvalComment }}
          </text>
        </view>
        <view class="app-card__footer">
          <text class="app-no">提交时间: {{ formatDate(app.createdAt) }}</text>
          <view v-if="app.status === 'Pending'" class="action-btns">
            <button class="btn btn--approve" @tap="approve(app)">
              <Icon name="check" :size="10" color="#67c23a" />
              <text>通过</text>
            </button>
            <button class="btn btn--reject" @tap="reject(app)">
              <Icon name="close" :size="10" color="#f56c6c" />
              <text>拒绝</text>
            </button>
          </view>
        </view>
      </view>

      <view v-if="!isLoading && applications.length === 0" class="empty">
        <Icon name="inbox" :size="48" color="#d0d0d0" />
        <text class="empty-text">{{ emptyText }}</text>
      </view>

      <view v-if="hasMore && applications.length > 0" class="load-more">
        <Icon name="arrow-down" :size="10" color="#909399" />
        <text>加载更多...</text>
      </view>
      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  getTeachingApplications,
  approveTeachingApplication,
  rejectTeachingApplication,
} from '@/api/teaching-app'
import { useAuthStore } from '@/stores/auth'
import Icon from '@/components/Icon.vue'
import type { TeachingApplication, TeachingApplicationStatus } from '@/types/teaching'
import { TEACHING_STATUS_LABELS } from '@/types/teaching'

type TabKey = 'pending' | 'approved' | 'rejected' | 'all'

interface TabDef {
  key: TabKey
  label: string
  icon: string
  status?: TeachingApplicationStatus | ''
}

const tabs: TabDef[] = [
  { key: 'pending', label: '待审批', icon: 'bell', status: 'Pending' },
  { key: 'approved', label: '已通过', icon: 'circle-check', status: 'Approved' },
  { key: 'rejected', label: '已拒绝', icon: 'circle-x', status: 'Rejected' },
  { key: 'all', label: '全部', icon: 'list', status: '' },
]

const authStore = useAuthStore()

const currentTab = ref<TabKey>('pending')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const applications = ref<TeachingApplication[]>([])
const total = ref(0)
const pendingCount = ref(0)

const hasMore = computed(() => applications.value.length < total.value)
const emptyText = computed(() => {
  const map: Record<TabKey, string> = {
    pending: '暂无待审批申请',
    approved: '暂无已通过的申请',
    rejected: '暂无已拒绝的申请',
    all: '暂无申请',
  }
  return map[currentTab.value]
})

function statusLabel(status: string): string {
  return TEACHING_STATUS_LABELS[status as TeachingApplicationStatus] ?? status
}

function statusIcon(status: string): string {
  const map: Record<string, string> = {
    Pending: 'clock',
    Approved: 'circle-check',
    Rejected: 'circle-x',
    Cancelled: 'close',
  }
  return map[status] || 'tag'
}

function formatPeriods(periods: number[] | undefined): string {
  if (!periods || periods.length === 0) return '-'
  return periods.map((p) => `第${p}节`).join('、')
}

function formatDate(value: string): string {
  if (!value) return ''
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return value
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}`
}

function currentStatus(): TeachingApplicationStatus | '' {
  return tabs.find((t) => t.key === currentTab.value)?.status ?? ''
}

async function switchTab(tab: TabKey) {
  if (currentTab.value === tab) return
  currentTab.value = tab
  page.value = 1
  await loadData()
}

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const status = currentStatus()
    const resp = await getTeachingApplications({
      page: 1,
      pageSize,
      ...(status ? { status } : {}),
    } as Record<string, string | number>)
    applications.value = resp?.items ?? []
    total.value = resp?.total ?? applications.value.length

    if (currentTab.value !== 'pending' && pendingCount.value === 0) {
      await loadPendingCount()
    }
  } catch {
    applications.value = []
    total.value = 0
  } finally {
    isLoading.value = false
  }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  const next = page.value + 1
  try {
    const status = currentStatus()
    const resp = await getTeachingApplications({
      page: next,
      pageSize,
      ...(status ? { status } : {}),
    } as Record<string, string | number>)
    applications.value.push(...(resp?.items ?? []))
    total.value = resp?.total ?? applications.value.length
    page.value = next
  } catch {
    /* ignore */
  }
}

async function loadPendingCount() {
  try {
    const resp = await getTeachingApplications({ status: 'Pending', page: 1, pageSize: 1 } as Record<string, string | number>)
    pendingCount.value = resp?.total ?? (resp?.items?.length ?? 0)
  } catch {
    pendingCount.value = 0
  }
}

async function approve(app: TeachingApplication) {
  uni.showModal({
    title: '确认通过',
    content: `确定通过 "${app.courseName}" 的申请吗？`,
    editable: true,
    placeholderText: '审批备注（可选）',
    success: async (res) => {
      if (!res.confirm) return
      try {
        uni.showLoading({ title: '处理中...' })
        await approveTeachingApplication(app.id, {
          comment: (res.content || '').trim() || '同意',
          approverName: authStore.currentUser?.fullName ?? authStore.currentUser?.username,
        })
        uni.hideLoading()
        uni.showToast({ title: '已通过', icon: 'success' })
        await Promise.all([loadData(), loadPendingCount()])
      } catch {
        uni.hideLoading()
      }
    },
  })
}

async function reject(app: TeachingApplication) {
  uni.showModal({
    title: '确认拒绝',
    content: `确定拒绝 "${app.courseName}" 的申请吗？`,
    editable: true,
    placeholderText: '请输入拒绝原因',
    success: async (res) => {
      if (!res.confirm) return
      const reason = (res.content || '').trim() || '不符合条件'
      try {
        uni.showLoading({ title: '处理中...' })
        await rejectTeachingApplication(app.id, {
          comment: reason,
          approverName: authStore.currentUser?.fullName ?? authStore.currentUser?.username,
        })
        uni.hideLoading()
        uni.showToast({ title: '已拒绝', icon: 'success' })
        await Promise.all([loadData(), loadPendingCount()])
      } catch {
        uni.hideLoading()
      }
    },
  })
}

onMounted(async () => {
  await loadData()
  await loadPendingCount()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.approval-page { min-height: 100vh; background: #f5f7fa; }

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

.tab-icon { margin-right: 6rpx; }
.status-icon { vertical-align: middle; }

.list-area { height: calc(100vh - 88rpx); padding: 24rpx; }

.app-card {
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

  &__body {
    display: flex;
    flex-direction: column;
    gap: 6rpx;
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

.app-title { font-size: 30rpx; font-weight: bold; color: #303133; }

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
}

.app-meta { font-size: 26rpx; color: #606266; }
.app-meta--comment { color: #909399; font-style: italic; }
.app-no { font-size: 22rpx; color: #c0c4cc; }

.action-btns { display: flex; gap: 12rpx; }

.btn {
  padding: 8rpx 24rpx;
  border-radius: 999rpx;
  font-size: 24rpx;
  border: none;
  margin: 0;
  display: inline-flex;
  align-items: center;
  gap: 6rpx;
  &::after { border: none; }

  &--approve { background: #67c23a; color: #fff; }
  &--reject { background: #f5f5f5; color: #909399; }
}

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
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