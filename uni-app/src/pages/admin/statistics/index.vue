<template>
  <view class="admin-stats-page">
    <!-- 顶部概览 -->
    <view class="header">
      <text class="header__title">数据统计</text>
      <text class="header__subtitle">{{ semesterStore.currentSemester?.name || '' }}</text>
    </view>

    <scroll-view class="content" scroll-y refresher-enabled @refresherrefresh="loadData">
      <!-- 关键指标 -->
      <view class="stats-grid" v-if="dashboard">
        <view class="stat-card stat-card--primary">
          <text class="stat-card__num">{{ dashboard.totalLabs }}</text>
          <text class="stat-card__label">实验室总数</text>
        </view>
        <view class="stat-card stat-card--green">
          <text class="stat-card__num">{{ dashboard.totalEquipments }}</text>
          <text class="stat-card__label">设备总数</text>
        </view>
        <view class="stat-card stat-card--orange">
          <text class="stat-card__num">{{ dashboard.pendingApprovals }}</text>
          <text class="stat-card__label">待审批</text>
        </view>
        <view class="stat-card stat-card--blue">
          <text class="stat-card__num">{{ dashboard.overdueBorrows }}</text>
          <text class="stat-card__label">逾期借用</text>
        </view>
      </view>

      <!-- 实验室使用排行 -->
      <view class="section" v-if="labUsage.length > 0">
        <text class="section-title">实验室使用排行</text>
        <view class="rank-list">
          <view v-for="(item, i) in labUsage" :key="i" class="rank-item">
            <view class="rank-num" :class="{ top: i < 3 }">{{ i + 1 }}</view>
            <view class="rank-info">
              <text class="rank-name">{{ item.labName }}</text>
              <view class="rank-bar">
                <view class="rank-bar__fill" :style="{ width: (item.reservationRate * 100) + '%' }" />
              </view>
            </view>
            <text class="rank-rate">{{ (item.reservationRate * 100).toFixed(0) }}%</text>
          </view>
        </view>
      </view>

      <!-- 周报摘要 -->
      <view class="section" v-if="weeklySummary.length > 0">
        <text class="section-title">本周摘要</text>
        <view class="summary-grid">
          <view v-for="item in weeklySummary.slice(-4)" :key="item.weekLabel" class="summary-item">
            <text class="summary-week">{{ item.weekLabel }}</text>
            <view class="summary-nums">
              <view class="summary-num-item">
                <text class="num">{{ item.reservations }}</text>
                <text class="label">预约</text>
              </view>
              <view class="summary-num-item">
                <text class="num">{{ item.borrowRecords }}</text>
                <text class="label">借用</text>
              </view>
              <view class="summary-num-item">
                <text class="num">{{ item.usageHours }}h</text>
                <text class="label">时长</text>
              </view>
            </view>
          </view>
        </view>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useSemesterStore } from '@/stores/semester'
import { useStatisticsStore } from '@/stores/statistics'

const semesterStore = useSemesterStore()
const statsStore = useStatisticsStore()

const dashboard = computed(() => statsStore.dashboard)
const labUsage = computed(() => statsStore.labUsage)
const weeklySummary = computed(() => statsStore.weeklySummary)

async function loadData() {
  await statsStore.loadAll()
}

onMounted(async () => {
  await Promise.all([statsStore.loadAll(), semesterStore.loadCurrentSemester()])
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.admin-stats-page { min-height: 100vh; background: #f5f7fa; }

.header {
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  padding: 20rpx 32rpx 40rpx;

  &__title { font-size: 40rpx; font-weight: bold; color: #fff; display: block; }
  &__subtitle { font-size: 24rpx; color: rgba(255,255,255,0.8); margin-top: 8rpx; display: block; }
}

.content { margin-top: -20rpx; height: calc(100vh - 160rpx); }

.stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20rpx;
  padding: 0 24rpx;
  margin-bottom: 24rpx;
}

.stat-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 32rpx;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12rpx;
  box-shadow: 0 4rpx 16rpx rgba(0,0,0,0.06);

  &__num { font-size: 44rpx; font-weight: bold; color: #303133; }
  &__label { font-size: 24rpx; color: #909399; }

  &--primary .stat-card__num { color: $primary; }
  &--green .stat-card__num { color: #67c23a; }
  &--orange .stat-card__num { color: #e6a23c; }
  &--blue .stat-card__num { color: #409eff; }
}

.section {
  background: #fff;
  margin: 0 24rpx 24rpx;
  border-radius: 24rpx;
  padding: 32rpx;
  box-shadow: 0 4rpx 16rpx rgba(0,0,0,0.06);
}

.section-title { font-size: 30rpx; font-weight: bold; color: #303133; display: block; margin-bottom: 24rpx; }

.rank-list { display: flex; flex-direction: column; gap: 20rpx; }

.rank-item { display: flex; align-items: center; gap: 16rpx; }

.rank-num {
  width: 40rpx;
  height: 40rpx;
  border-radius: 8rpx;
  background: #f0f0f0;
  color: #909399;
  font-size: 24rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  &.top { background: $primary; color: #fff; }
}

.rank-info { flex: 1; }

.rank-name { font-size: 26rpx; color: #303133; display: block; margin-bottom: 8rpx; }

.rank-bar { width: 100%; height: 8rpx; background: #f0f0f0; border-radius: 4rpx; }
.rank-bar__fill { height: 100%; background: linear-gradient(90deg, $primary, #764ba2); border-radius: 4rpx; transition: width 0.3s; }

.rank-rate { font-size: 24rpx; color: $primary; font-weight: bold; width: 80rpx; text-align: right; }

.summary-grid { display: flex; flex-direction: column; gap: 20rpx; }

.summary-item {
  display: flex;
  align-items: center;
  gap: 24rpx;
  padding: 16rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
}

.summary-week { font-size: 26rpx; color: #909399; width: 80rpx; }

.summary-nums { flex: 1; display: flex; justify-content: space-around; }

.summary-num-item { display: flex; flex-direction: column; align-items: center; gap: 4rpx; }
.num { font-size: 32rpx; font-weight: bold; color: $primary; }
.label { font-size: 22rpx; color: #909399; }
</style>
