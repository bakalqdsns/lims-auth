<template>
  <view class="approval-page">
    <!-- Tab -->
    <view class="tab-bar">
      <view class="tab-item" :class="{ active: currentTab === 'pending' }" @tap="switchTab('pending')">
        <text>待审批</text>
        <view v-if="pendingCount > 0" class="tab-badge">{{ pendingCount }}</view>
      </view>
      <view class="tab-item" :class="{ active: currentTab === 'all' }" @tap="switchTab('all')">
        <text>全部</text>
      </view>
    </view>

    <!-- 列表 -->
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData" @scrolltolower="loadMore">
      <view v-for="app in applications" :key="app.id" class="app-card">
        <view class="app-card__header">
          <text class="app-title">{{ app.courseName }}</text>
          <view class="status-tag" :class="'status--' + app.status">{{ statusLabel(app.status) }}</view>
        </view>
        <view class="app-card__body">
          <text class="app-meta">教师: {{ app.teacherName }}</text>
          <text class="app-meta">班级: {{ app.className }}</text>
          <text class="app-meta">实验室: {{ app.labName }}</text>
          <text class="app-meta">日期: {{ app.date }} {{ app.timeSlot }}</text>
          <text class="app-meta">学生人数: {{ app.studentCount }} 人</text>
          <text v-if="app.purpose" class="app-meta">用途: {{ app.purpose }}</text>
        </view>
        <view class="app-card__footer">
          <text class="app-no">{{ app.applicationNo }}</text>
          <view v-if="app.status === 1" class="action-btns">
            <button class="btn btn--approve" @tap="approve(app)">通过</button>
            <button class="btn btn--reject" @tap="reject(app)">拒绝</button>
          </view>
        </view>
      </view>

      <view v-if="!isLoading && applications.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无申请</text>
      </view>

      <view v-if="hasMore && applications.length > 0" class="load-more"><text>加载更多...</text></view>
      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getTeachingApplications, approveTeachingApplication, rejectTeachingApplication } from '@/api/teaching-app'
import type { TeachingApplication } from '@/types/teaching'

const currentTab = ref<'pending' | 'all'>('pending')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const applications = ref<TeachingApplication[]>([])
const total = ref(0)
const pendingCount = ref(0)

const hasMore = computed(() => applications.value.length < total.value)

function statusLabel(status: number): string {
  const map: Record<number, string> = { 1: '待审批', 2: '已通过', 3: '已拒绝', 0: '已取消' }
  return map[status] || '未知'
}

async function switchTab(tab: 'pending' | 'all') {
  currentTab.value = tab
  page.value = 1
  await loadData()
}

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const resp = await getTeachingApplications({ page: 1, pageSize })
    applications.value = resp?.items ?? []
    total.value = resp?.total ?? 0
  } catch { /* ignore */ } finally { isLoading.value = false }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  try {
    const resp = await getTeachingApplications({ page: page.value, pageSize })
    applications.value.push(...(resp?.items ?? []))
  } catch { /* ignore */ }
}

async function approve(app: TeachingApplication) {
  uni.showModal({
    title: '确认通过',
    content: `确定通过 "${app.courseName}" 的申请吗？`,
    editable: true,
    placeholderText: '审批备注（可选）',
    success: async (res) => {
      if (res.confirm) {
        try {
          uni.showLoading({ title: '处理中...' })
          await approveTeachingApplication(app.id, { approved: true, remark: res.content || undefined })
          uni.hideLoading()
          uni.showToast({ title: '已通过', icon: 'success' })
          await loadData()
        } catch { uni.hideLoading() }
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
      if (res.confirm) {
        try {
          uni.showLoading({ title: '处理中...' })
          await rejectTeachingApplication(app.id, { approved: false, remark: res.content || '不符合条件' })
          uni.hideLoading()
          uni.showToast({ title: '已拒绝', icon: 'success' })
          await loadData()
        } catch { uni.hideLoading() }
      }
    },
  })
}

onMounted(async () => { await loadData() })
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
  &.status--1 { background: #fef0f0; color: #f56c6c; }
  &.status--2 { background: #f0f9eb; color: #67c23a; }
  &.status--3 { background: #f5f5f5; color: #909399; }
}

.app-meta { font-size: 26rpx; color: #606266; }
.app-no { font-size: 22rpx; color: #c0c4cc; }

.action-btns { display: flex; gap: 12rpx; }

.btn {
  padding: 8rpx 24rpx;
  border-radius: 999rpx;
  font-size: 24rpx;
  border: none;
  margin: 0;
  &::after { border: none; }

  &--approve { background: #67c23a; color: #fff; }
  &--reject { background: #f5f5f5; color: #909399; }
}

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }
.load-more { text-align: center; padding: 24rpx; font-size: 24rpx; color: #909399; }
</style>
