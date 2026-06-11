<template>
  <view class="equipment-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <text class="search-icon">&#xe6c0;</text>
      <input
        v-model="keyword"
        class="search-input"
        placeholder="搜索设备名称/编号"
        confirm-type="search"
        @confirm="loadData"
      />
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
        v-for="item in equipments"
        :key="item.id"
        class="equip-card"
        @tap="showDetail(item)"
      >
        <view class="equip-card__left">
          <view class="equip-icon">
            <text>&#xe6a3;</text>
          </view>
        </view>
        <view class="equip-card__right">
          <text class="equip-name">{{ item.name }}</text>
          <text class="equip-code">{{ item.code }}</text>
          <view class="equip-meta">
            <text v-if="item.category" class="meta-tag">{{ item.category }}</text>
            <text class="meta-quantity">
              可用 {{ item.availableQuantity }}/{{ item.totalQuantity }}
            </text>
          </view>
        </view>
        <view class="equip-card__status">
          <view class="status-dot" :class="getStatusClass(item)" />
          <text class="status-text">{{ getStatusText(item) }}</text>
        </view>
      </view>

      <view v-if="!isLoading && equipments.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无设备</text>
      </view>

      <view v-if="hasMore && equipments.length > 0" class="load-more">
        <text>加载更多...</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>

    <!-- 设备详情弹窗 -->
    <view v-if="showDetailSheet" class="detail-overlay" @tap="showDetailSheet = false">
      <view class="detail-sheet" @tap.stop>
        <view class="detail-sheet__header">
          <text class="detail-sheet__title">设备详情</text>
          <text class="detail-sheet__close" @tap="showDetailSheet = false">&#xe6c7;</text>
        </view>
        <scroll-view class="detail-sheet__body" scroll-y v-if="selectedEquip">
          <view class="detail-row">
            <text class="detail-label">设备名称</text>
            <text class="detail-value">{{ selectedEquip.name }}</text>
          </view>
          <view class="detail-row">
            <text class="detail-label">设备编号</text>
            <text class="detail-value">{{ selectedEquip.code }}</text>
          </view>
          <view v-if="selectedEquip.model" class="detail-row">
            <text class="detail-label">型号</text>
            <text class="detail-value">{{ selectedEquip.model }}</text>
          </view>
          <view v-if="selectedEquip.manufacturer" class="detail-row">
            <text class="detail-label">厂商</text>
            <text class="detail-value">{{ selectedEquip.manufacturer }}</text>
          </view>
          <view v-if="selectedEquip.category" class="detail-row">
            <text class="detail-label">分类</text>
            <text class="detail-value">{{ selectedEquip.category }}</text>
          </view>
          <view class="detail-row">
            <text class="detail-label">库存</text>
            <text class="detail-value">{{ selectedEquip.availableQuantity }}/{{ selectedEquip.totalQuantity }}</text>
          </view>
          <view v-if="selectedEquip.labName" class="detail-row">
            <text class="detail-label">存放地点</text>
            <text class="detail-value">{{ selectedEquip.labName }}</text>
          </view>
          <view v-if="selectedEquip.description" class="detail-row">
            <text class="detail-label">描述</text>
            <text class="detail-value">{{ selectedEquip.description }}</text>
          </view>
        </scroll-view>
        <view class="detail-sheet__footer">
          <button
            v-if="selectedEquip && selectedEquip.availableQuantity > 0"
            class="borrow-btn"
            @tap="goBorrow"
          >
            立即借用
          </button>
          <button v-else class="borrow-btn" disabled>暂无库存</button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useEquipmentStore } from '@/stores/equipment'
import type { Equipment } from '@/types/equipment'

const equipStore = useEquipmentStore()

const keyword = ref('')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const showDetailSheet = ref(false)
const selectedEquip = ref<Equipment | null>(null)

const equipments = computed(() => equipStore.equipments)
const total = computed(() => equipStore.total)
const hasMore = computed(() => equipments.value.length < total.value)

function getStatusClass(item: Equipment): string {
  if (item.availableQuantity > 0) return 'available'
  if (item.status === 0) return 'inactive'
  return 'borrowed'
}

function getStatusText(item: Equipment): string {
  if (item.availableQuantity > 0) return '可用'
  if (item.status === 0) return '停用'
  return '全部借出'
}

function showDetail(item: Equipment) {
  selectedEquip.value = item
  showDetailSheet.value = true
}

function goBorrow() {
  showDetailSheet.value = false
  uni.navigateTo({ url: `/pages/borrow/index?equipmentId=${selectedEquip.value?.id}` })
}

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    await equipStore.loadEquipments({
      keyword: keyword.value || undefined,
      page: 1,
      pageSize,
    })
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
    await equipStore.loadEquipments({
      keyword: keyword.value || undefined,
      page: page.value,
      pageSize,
    })
  } catch {
    // ignore
  }
}

onMounted(async () => {
  await loadData()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.equipment-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.search-bar {
  display: flex;
  align-items: center;
  background: $primary;
  padding: 20rpx 24rpx;

  .search-icon {
    font-size: 32rpx;
    color: rgba(255, 255, 255, 0.8);
    margin-right: 12rpx;
  }

  .search-input {
    flex: 1;
    height: 64rpx;
    background: rgba(255, 255, 255, 0.2);
    border-radius: 999rpx;
    padding: 0 24rpx;
    font-size: 28rpx;
    color: #fff;
  }
}

.list-area {
  height: calc(100vh - 104rpx);
  padding: 24rpx;
}

.equip-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
  display: flex;
  align-items: center;
  gap: 20rpx;

  &__left {}

  &__right {
    flex: 1;
  }

  &__status {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8rpx;
  }
}

.equip-icon {
  width: 80rpx;
  height: 80rpx;
  background: #f0f9eb;
  border-radius: 20rpx;
  display: flex;
  align-items: center;
  justify-content: center;

  text {
    font-size: 40rpx;
    color: #67c23a;
  }
}

.equip-name {
  font-size: 30rpx;
  font-weight: bold;
  color: #303133;
  display: block;
  margin-bottom: 6rpx;
}

.equip-code {
  font-size: 24rpx;
  color: #909399;
  display: block;
  margin-bottom: 12rpx;
}

.equip-meta {
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

.meta-quantity {
  font-size: 24rpx;
  color: #67c23a;
}

.status-dot {
  width: 16rpx;
  height: 16rpx;
  border-radius: 50%;

  &.available { background: #67c23a; }
  &.borrowed { background: #e6a23c; }
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

.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }

.load-more {
  text-align: center;
  padding: 24rpx;
  font-size: 24rpx;
  color: #909399;
}

.detail-overlay {
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

.detail-sheet {
  width: 100%;
  max-height: 70vh;
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

  &__title {
    font-size: 32rpx;
    font-weight: bold;
    color: #303133;
  }

  &__close {
    font-size: 36rpx;
    color: #909399;
  }

  &__body {
    flex: 1;
    padding: 24rpx 32rpx;
    max-height: 45vh;
  }

  &__footer {
    padding: 24rpx 32rpx;
    border-top: 1rpx solid #f0f0f0;
  }
}

.detail-row {
  display: flex;
  padding: 16rpx 0;
  border-bottom: 1rpx solid #f5f7fa;

  &:last-child { border-bottom: none; }
}

.detail-label {
  width: 160rpx;
  font-size: 26rpx;
  color: #909399;
}

.detail-value {
  flex: 1;
  font-size: 26rpx;
  color: #303133;
}

.borrow-btn {
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

  &[disabled] {
    background: #d0d0d0;
    color: #909399;
  }
}
</style>
