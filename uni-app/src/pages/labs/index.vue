<template>
  <view class="labs-page">
    <!-- 顶部导航 -->
    <view class="header">
      <view class="search-bar">
        <text class="search-icon">&#xe6c0;</text>
        <input
          v-model="keyword"
          class="search-input"
          placeholder="搜索实验室名称"
          placeholder-class="search-placeholder"
          confirm-type="search"
          @confirm="onSearch"
        />
      </view>
      <!-- 筛选按钮 -->
      <view class="filter-btn" @tap="showFilter = !showFilter">
        <text>&#xe6c1;</text>
      </view>
    </view>

    <!-- 筛选面板 -->
    <view v-if="showFilter" class="filter-panel">
      <view class="filter-row">
        <text class="filter-label">校区</text>
        <picker mode="selector" :range="campusOptions" range-key="name" @change="onCampusChange">
          <view class="picker-value">
            {{ selectedCampusName || '全部校区' }}
            <text class="picker-arrow">&#xe6c5;</text>
          </view>
        </picker>
      </view>
      <view class="filter-row">
        <text class="filter-label">状态</text>
        <picker mode="selector" :range="statusOptions" range-key="label" @change="onStatusChange">
          <view class="picker-value">
            {{ selectedStatusLabel || '全部状态' }}
            <text class="picker-arrow">&#xe6c5;</text>
          </view>
        </picker>
      </view>
      <view class="filter-actions">
        <button class="filter-reset" @tap="resetFilter">重置</button>
        <button class="filter-confirm" @tap="confirmFilter">确定</button>
      </view>
    </view>

    <!-- 实验室列表 -->
    <scroll-view
      class="lab-list"
      scroll-y
      :refresher-enabled="true"
      :refresher-triggered="isRefreshing"
      @refresherrefresh="loadLabs"
      @scrolltolower="loadMore"
    >
      <view
        v-for="lab in labs"
        :key="lab.id"
        class="lab-card"
        @tap="goDetail(lab.id)"
      >
        <!-- 左侧图标 -->
        <view class="lab-card__left">
          <view class="lab-icon">
            <text>&#xe6c8;</text>
          </view>
        </view>

        <!-- 中间内容 -->
        <view class="lab-card__mid">
          <text class="lab-name">{{ lab.name }}</text>
          <text class="lab-code">{{ lab.code }}</text>
          <view class="lab-meta">
            <text class="meta-tag" v-if="lab.type">{{ lab.type }}</text>
            <text class="meta-capacity">
              可容纳 {{ lab.capacity }} 人
            </text>
            <text class="meta-equip">
              {{ lab.equipmentCount }} 台设备
            </text>
          </view>
        </view>

        <!-- 右侧状态 -->
        <view class="lab-card__status">
          <view
            class="status-dot"
            :class="lab.status === 1 ? 'available' : 'inactive'"
          />
          <text class="status-text">
            {{ lab.status === 1 ? '可用' : '不可用' }}
          </text>
        </view>
      </view>

      <!-- 空状态 -->
      <view v-if="!isLoading && labs.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无实验室</text>
      </view>

      <!-- 加载更多 -->
      <view v-if="hasMore && labs.length > 0" class="load-more">
        <text>加载更多...</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useLabStore } from '@/stores/lab'
import type { Lab } from '@/types/campus'

const labStore = useLabStore()

const keyword = ref('')
const showFilter = ref(false)
const selectedCampusId = ref<string | undefined>()
const selectedStatus = ref<number | undefined>()
const page = ref(1)
const pageSize = 20

const isRefreshing = ref(false)
const labs = computed(() => labStore.labs)
const isLoading = computed(() => labStore.isLoading)
const total = computed(() => labStore.total)
const hasMore = computed(() => labs.value.length < total.value)

const campusOptions = computed(() => [
  { id: undefined as string | undefined, name: '全部校区' },
  ...labStore.campuses.map((c) => ({ id: c.id, name: c.name })),
])

const statusOptions = [
  { value: undefined, label: '全部状态' },
  { value: 1, label: '可用' },
  { value: 0, label: '不可用' },
]

const selectedCampusName = computed(() => {
  if (!selectedCampusId.value) return ''
  return labStore.campuses.find((c) => c.id === selectedCampusId.value)?.name ?? ''
})

const selectedStatusLabel = computed(() => {
  if (selectedStatus.value === undefined) return ''
  return statusOptions.find((s) => s.value === selectedStatus.value)?.label ?? ''
})

function goDetail(id: string) {
  uni.navigateTo({ url: `/pages/lab-detail/index?id=${id}` })
}

async function loadLabs() {
  page.value = 1
  await labStore.loadLabs({
    keyword: keyword.value || undefined,
    campusId: selectedCampusId.value,
    status: selectedStatus.value,
    page: 1,
    pageSize,
  })
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  await labStore.loadLabs({
    keyword: keyword.value || undefined,
    campusId: selectedCampusId.value,
    status: selectedStatus.value,
    page: page.value,
    pageSize,
  })
}

async function onSearch() {
  await loadLabs()
}

function onCampusChange(e: { detail: { value: number } }) {
  selectedCampusId.value = campusOptions.value[e.detail.value].id
}

function onStatusChange(e: { detail: { value: number } }) {
  selectedStatus.value = statusOptions[e.detail.value].value
}

function resetFilter() {
  selectedCampusId.value = undefined
  selectedStatus.value = undefined
  showFilter.value = false
  loadLabs()
}

function confirmFilter() {
  showFilter.value = false
  loadLabs()
}

onMounted(async () => {
  await labStore.loadCampuses()
  await loadLabs()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.labs-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.header {
  display: flex;
  align-items: center;
  gap: 16rpx;
  padding: 24rpx;
  background: $primary;
}

.search-bar {
  flex: 1;
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 999rpx;
  padding: 0 24rpx;
  height: 72rpx;
}

.search-icon {
  font-size: 32rpx;
  color: rgba(255, 255, 255, 0.8);
  margin-right: 12rpx;
}

.search-input {
  flex: 1;
  font-size: 28rpx;
  color: #fff;
}

.search-placeholder {
  color: rgba(255, 255, 255, 0.6);
}

.filter-btn {
  width: 72rpx;
  height: 72rpx;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;

  text {
    font-size: 32rpx;
    color: #fff;
  }
}

.filter-panel {
  background: #fff;
  padding: 24rpx;
  border-bottom: 1rpx solid #e4e7ed;
}

.filter-row {
  display: flex;
  align-items: center;
  margin-bottom: 20rpx;
}

.filter-label {
  font-size: 28rpx;
  color: #606266;
  width: 100rpx;
}

.picker-value {
  flex: 1;
  height: 64rpx;
  background: #f5f7fa;
  border-radius: 12rpx;
  padding: 0 20rpx;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 28rpx;
  color: #303133;
}

.picker-arrow {
  font-size: 24rpx;
  color: #909399;
}

.filter-actions {
  display: flex;
  gap: 16rpx;
  margin-top: 16rpx;
}

.filter-reset,
.filter-confirm {
  flex: 1;
  height: 72rpx;
  border-radius: 12rpx;
  font-size: 28rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0;
  border: none;

  &::after {
    border: none;
  }
}

.filter-reset {
  background: #f5f7fa;
  color: #606266;
}

.filter-confirm {
  background: $primary;
  color: #fff;
}

.lab-list {
  height: calc(100vh - 200rpx);
  padding: 24rpx;
}

.lab-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
  display: flex;
  align-items: center;
  gap: 20rpx;

  &__left {}

  &__mid {
    flex: 1;
  }

  &__status {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8rpx;
  }
}

.lab-icon {
  width: 80rpx;
  height: 80rpx;
  background: #edf2ff;
  border-radius: 20rpx;
  display: flex;
  align-items: center;
  justify-content: center;

  text {
    font-size: 40rpx;
    color: $primary;
  }
}

.lab-name {
  font-size: 30rpx;
  font-weight: bold;
  color: #303133;
  display: block;
  margin-bottom: 6rpx;
}

.lab-code {
  font-size: 24rpx;
  color: #909399;
  display: block;
  margin-bottom: 12rpx;
}

.lab-meta {
  display: flex;
  align-items: center;
  gap: 12rpx;
}

.meta-tag {
  background: #f0f0f0;
  color: #909399;
  font-size: 22rpx;
  padding: 4rpx 12rpx;
  border-radius: 8rpx;
}

.meta-capacity {
  font-size: 24rpx;
  color: #67c23a;
}

.meta-equip {
  font-size: 24rpx;
  color: #909399;
}

.status-dot {
  width: 16rpx;
  height: 16rpx;
  border-radius: 50%;

  &.available { background: #67c23a; }
  &.inactive { background: #c0c4cc; }
}

.status-text {
  font-size: 22rpx;
  color: #909399;
}

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 120rpx 0;
  gap: 16rpx;
}

.empty-icon {
  font-size: 80rpx;
  color: #d0d0d0;
}

.empty-text {
  font-size: 28rpx;
  color: #909399;
}

.load-more {
  text-align: center;
  padding: 24rpx;
  font-size: 24rpx;
  color: #909399;
}
</style>
