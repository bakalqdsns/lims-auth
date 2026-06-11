<template>
  <view class="lab-detail-page">
    <!-- 加载状态 -->
    <view v-if="isLoading" class="loading-state">
      <text>加载中...</text>
    </view>

    <template v-else-if="lab">
      <!-- 实验室信息头部 -->
      <view class="lab-header">
        <view class="lab-header__bg" />
        <view class="lab-header__content">
          <view class="lab-header__top">
            <view class="status-badge" :class="labStatus === 1 ? 'active' : 'inactive'">
              {{ labStatus === 1 ? '可预约' : '不可预约' }}
            </view>
          </view>
          <text class="lab-header__name">{{ lab.name }}</text>
          <text class="lab-header__code">{{ lab.code }}</text>
        </view>
      </view>

      <!-- 信息卡片 -->
      <view class="info-card">
        <view class="info-row">
          <text class="info-icon">&#xe6c2;</text>
          <view class="info-content">
            <text class="info-label">位置</text>
            <text class="info-value">{{ lab.buildingName }} {{ lab.roomNumber || '' }}</text>
          </view>
        </view>
        <view class="info-row">
          <text class="info-icon">&#xe6c3;</text>
          <view class="info-content">
            <text class="info-label">容量</text>
            <text class="info-value">{{ lab.capacity }} 人</text>
          </view>
        </view>
        <view v-if="labType" class="info-row">
          <text class="info-icon">&#xe6a4;</text>
          <view class="info-content">
            <text class="info-label">类型</text>
            <text class="info-value">{{ labType }}</text>
          </view>
        </view>
        <view v-if="labArea" class="info-row">
          <text class="info-icon">&#xe6d0;</text>
          <view class="info-content">
            <text class="info-label">面积</text>
            <text class="info-value">{{ labArea }} m2</text>
          </view>
        </view>
        <view v-if="labManager" class="info-row">
          <text class="info-icon">&#xe6d1;</text>
          <view class="info-content">
            <text class="info-label">管理员</text>
            <text class="info-value">{{ labManager }}</text>
          </view>
        </view>
        <view v-if="lab.openingHours" class="info-row">
          <text class="info-icon">&#xe6d2;</text>
          <view class="info-content">
            <text class="info-label">开放时间</text>
            <text class="info-value">{{ lab.openingHours }}</text>
          </view>
        </view>
      </view>

      <!-- 描述 -->
      <view v-if="lab.description" class="desc-card">
        <text class="desc-card__title">简介</text>
        <text class="desc-card__text">{{ lab.description }}</text>
      </view>

      <!-- 设备列表 -->
      <view class="equip-card">
        <view class="equip-card__header">
          <text class="equip-card__title">相关设备</text>
          <text class="equip-card__count">{{ lab.equipmentCount }} 台</text>
        </view>
        <view v-if="equipments.length > 0" class="equip-list">
          <view v-for="eq in equipments" :key="eq.id" class="equip-item" @tap="goEquipDetail(eq.id)">
            <text class="equip-item__name">{{ eq.name }}</text>
            <text class="equip-item__status" :class="eq.availableQuantity > 0 ? 'available' : 'unavailable'">
              可用 {{ eq.availableQuantity }}/{{ eq.totalQuantity }}
            </text>
          </view>
        </view>
        <view v-else class="equip-empty">
          <text>暂无设备</text>
        </view>
      </view>

      <!-- 预约按钮 -->
      <view class="bottom-bar">
        <button
          class="reserve-btn"
          :disabled="labStatus !== 1"
          @tap="showReserveSheet = true"
        >
          {{ labStatus === 1 ? '立即预约' : '暂不可预约' }}
        </button>
      </view>
    </template>

    <!-- 预约弹窗 -->
    <view v-if="showReserveSheet" class="reserve-overlay" @tap="showReserveSheet = false">
      <view class="reserve-sheet" @tap.stop>
        <view class="reserve-sheet__header">
          <text class="reserve-sheet__title">预约实验室</text>
          <text class="reserve-sheet__close" @tap="showReserveSheet = false">&#xe6c7;</text>
        </view>

        <scroll-view class="reserve-sheet__body" scroll-y>
          <view class="form-item">
            <text class="form-label">预约日期</text>
            <picker mode="date" :value="reserveDate" start="" @change="onDateChange">
              <view class="picker-val">{{ reserveDate || '请选择日期' }}</view>
            </picker>
          </view>

          <view class="form-item">
            <text class="form-label">时间段</text>
            <picker mode="selector" :range="timeSlotOptions" range-key="name" @change="onTimeSlotChange">
              <view class="picker-val">{{ reserveTimeSlot || '请选择时间段' }}</view>
            </picker>
          </view>

          <view class="form-item">
            <text class="form-label">预约人数</text>
            <input
              v-model="attendeeCount"
              type="number"
              class="form-input"
              placeholder="请输入人数"
              :max="lab?.capacity"
            />
          </view>

          <view class="form-item">
            <text class="form-label">预约用途</text>
            <textarea
              v-model="purpose"
              class="form-textarea"
              placeholder="请输入预约用途"
              :maxlength="200"
            />
          </view>

          <view class="form-item">
            <text class="form-label">备注</text>
            <input v-model="remark" class="form-input" placeholder="备注信息（可选）" />
          </view>
        </scroll-view>

        <view class="reserve-sheet__footer">
          <button class="cancel-btn" @tap="showReserveSheet = false">取消</button>
          <button class="submit-btn" :disabled="!canSubmit" @tap="submitReservation">提交预约</button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useLabStore } from '@/stores/lab'
import { useEquipmentStore } from '@/stores/equipment'
import { createReservation } from '@/api/reservation'
import type { Equipment } from '@/types/equipment'

const labStore = useLabStore()
const equipStore = useEquipmentStore()

const isLoading = ref(true)
const showReserveSheet = ref(false)
const reserveDate = ref('')
const reserveTimeSlot = ref('')
const reserveTimeSlotIndex = ref(-1)
const attendeeCount = ref<number | undefined>()
const purpose = ref('')
const remark = ref('')

const lab = computed(() => labStore.currentLab)
const equipments = computed(() => equipStore.equipments)

const labStatus = computed(() => {
  if (!lab.value) return 0
  return (lab.value as Record<string, unknown>).isActive ? 1 : 0
})

const labType = computed(() => (lab.value as Record<string, unknown>)?.labType as string ?? undefined)
const labArea = computed(() => (lab.value as Record<string, unknown>)?.area as number | undefined)
const labManager = computed(() => (lab.value as Record<string, unknown>)?.managerName as string | undefined)

const timeSlotOptions = [
  { name: '上午 (08:00-12:00)', value: 'morning' },
  { name: '下午 (13:30-17:30)', value: 'afternoon' },
  { name: '晚上 (18:00-21:00)', value: 'evening' },
  { name: '全天 (08:00-21:00)', value: 'fullday' },
]

const canSubmit = computed(() => {
  return reserveDate.value && reserveTimeSlot.value && (attendeeCount.value ?? 0) > 0 && purpose.value.trim()
})

function onDateChange(e: { detail: { value: string } }) {
  reserveDate.value = e.detail.value
}

function onTimeSlotChange(e: { detail: { value: number } }) {
  reserveTimeSlotIndex.value = e.detail.value
  reserveTimeSlot.value = timeSlotOptions[e.detail.value].value
}

async function submitReservation() {
  if (!lab.value || !canSubmit.value) return

  try {
    uni.showLoading({ title: '提交中...' })
    await createReservation({
      labId: lab.value.id as unknown as number,
      date: reserveDate.value,
      timeSlot: reserveTimeSlot.value,
      purpose: purpose.value,
      attendeeCount: attendeeCount.value,
      remark: remark.value || undefined,
    })
    uni.hideLoading()
    uni.showToast({ title: '预约成功，请等待审批', icon: 'success' })
    showReserveSheet.value = false
    setTimeout(() => uni.navigateBack(), 1500)
  } catch {
    uni.hideLoading()
  }
}

function goEquipDetail(id: number) {
  uni.navigateTo({ url: `/pages/equipment/index?id=${id}` })
}

onMounted(async () => {
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1] as { options?: { id?: string } }
  const id = currentPage.options?.id || ''

  try {
    await labStore.loadLabById(id)
    await equipStore.loadEquipments({ labId: id })
  } finally {
    isLoading.value = false
  }
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.lab-detail-page {
  min-height: 100vh;
  background: #f5f7fa;
  padding-bottom: 160rpx;
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 200rpx;
  color: #909399;
  font-size: 28rpx;
}

.lab-header {
  position: relative;
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  padding: 60rpx 32rpx 48rpx;

  &__bg {
    position: absolute;
    top: 0;
    right: 0;
    width: 200rpx;
    height: 200rpx;
    background: rgba(255, 255, 255, 0.05);
    border-radius: 0 0 0 100%;
  }

  &__content {
    position: relative;
    z-index: 1;
  }

  &__top {
    margin-bottom: 16rpx;
  }

  &__name {
    font-size: 44rpx;
    font-weight: bold;
    color: #fff;
    display: block;
    margin-bottom: 8rpx;
  }

  &__code {
    font-size: 26rpx;
    color: rgba(255, 255, 255, 0.7);
  }
}

.status-badge {
  display: inline-block;
  padding: 6rpx 20rpx;
  border-radius: 999rpx;
  font-size: 24rpx;
  font-weight: 500;

  &.active {
    background: rgba(103, 194, 58, 0.2);
    color: #b3e19d;
  }

  &.inactive {
    background: rgba(255, 255, 255, 0.2);
    color: rgba(255, 255, 255, 0.7);
  }
}

.info-card {
  background: #fff;
  margin: -20rpx 24rpx 24rpx;
  border-radius: 24rpx;
  padding: 8rpx 0;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
}

.info-row {
  display: flex;
  align-items: flex-start;
  padding: 24rpx 32rpx;
  border-bottom: 1rpx solid #f0f0f0;

  &:last-child {
    border-bottom: none;
  }
}

.info-icon {
  font-size: 32rpx;
  color: $primary;
  margin-right: 20rpx;
  margin-top: 4rpx;
}

.info-content {
  display: flex;
  flex-direction: column;
  gap: 6rpx;
}

.info-label {
  font-size: 24rpx;
  color: #909399;
}

.info-value {
  font-size: 28rpx;
  color: #303133;
}

.desc-card {
  background: #fff;
  margin: 0 24rpx 24rpx;
  border-radius: 24rpx;
  padding: 32rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__title {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
    display: block;
    margin-bottom: 16rpx;
  }

  &__text {
    font-size: 28rpx;
    color: #606266;
    line-height: 1.6;
  }
}

.equip-card {
  background: #fff;
  margin: 0 24rpx 24rpx;
  border-radius: 24rpx;
  padding: 32rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20rpx;
  }

  &__title {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
  }

  &__count {
    font-size: 24rpx;
    color: #909399;
  }
}

.equip-list {
  display: flex;
  flex-direction: column;
  gap: 16rpx;
}

.equip-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16rpx 20rpx;
  background: #f5f7fa;
  border-radius: 12rpx;

  &__name {
    font-size: 26rpx;
    color: #303133;
  }

  &__status {
    font-size: 24rpx;

    &.available {
      color: #67c23a;
    }

    &.unavailable {
      color: #f56c6c;
    }
  }
}

.equip-empty {
  text-align: center;
  padding: 32rpx;
  color: #909399;
  font-size: 26rpx;
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

.reserve-btn {
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

  &::after {
    border: none;
  }

  &[disabled] {
    background: #d0d0d0;
    color: #909399;
  }
}

.reserve-overlay {
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

.reserve-sheet {
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
    padding: 32rpx;
    max-height: 50vh;
  }

  &__footer {
    display: flex;
    gap: 24rpx;
    padding: 24rpx 32rpx;
    border-top: 1rpx solid #f0f0f0;
  }
}

.form-item {
  margin-bottom: 28rpx;
}

.form-label {
  font-size: 28rpx;
  color: #606266;
  display: block;
  margin-bottom: 12rpx;
}

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

  &:focus {
    border-color: $primary;
  }
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

  &:focus {
    border-color: $primary;
  }
}

.cancel-btn,
.submit-btn {
  flex: 1;
  height: 80rpx;
  border-radius: 16rpx;
  font-size: 28rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  margin: 0;

  &::after {
    border: none;
  }
}

.cancel-btn {
  background: #f5f7fa;
  color: #606266;
}

.submit-btn {
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  color: #fff;

  &[disabled] {
    background: #d0d0d0;
    color: #909399;
  }
}
</style>
