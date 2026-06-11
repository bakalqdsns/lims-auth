<template>
  <view class="dashboard-page">
    <view :style="{ height: statusBarHeight + 'px' }" />

    <!-- 顶部导航 -->
    <view class="header" :style="{ paddingTop: statusBarHeight + 'px' }">
      <view class="header__left">
        <text class="header__title">数据看板</text>
        <text v-if="semesterStore.currentSemester" class="header__subtitle">
          {{ semesterStore.currentSemester.name }} 第{{ semesterStore.currentWeek }}周
        </text>
      </view>
      <view class="header__right">
        <text class="header__date">{{ today }}</text>
      </view>
    </view>

    <!-- 内容区 -->
    <scroll-view class="content" scroll-y refresher-enabled @refresherrefresh="loadData">
      <!-- 统计数据卡片 -->
      <view class="stat-cards">
        <view class="stat-card" @tap="goPage('/pages/labs/index')">
          <view class="stat-card__icon" style="background: #e8f4ff">
            <text style="color: #409eff">&#xe6a1;</text>
          </view>
          <view class="stat-card__info">
            <text class="stat-card__num">{{ dashboard?.totalLabs ?? '--' }}</text>
            <text class="stat-card__label">实验室</text>
          </view>
        </view>

        <view class="stat-card" @tap="goPage('/pages/admin/equipment/index')">
          <view class="stat-card__icon" style="background: #f0f9eb">
            <text style="color: #67c23a">&#xe6a3;</text>
          </view>
          <view class="stat-card__info">
            <text class="stat-card__num">{{ dashboard?.totalEquipments ?? '--' }}</text>
            <text class="stat-card__label">设备总数</text>
          </view>
        </view>

        <view class="stat-card" @tap="goPage('/pages/admin/teaching-apps/index')">
          <view class="stat-card__icon" style="background: #fef0f0">
            <text style="color: #f56c6c">&#xe6a2;</text>
          </view>
          <view class="stat-card__info">
            <text class="stat-card__num">{{ dashboard?.pendingApprovals ?? '--' }}</text>
            <text class="stat-card__label">待审批</text>
          </view>
        </view>

        <view class="stat-card" @tap="goPage('/pages/reservations/index')">
          <view class="stat-card__icon" style="background: #fdf6ec">
            <text style="color: #e6a23c">&#xe6b2;</text>
          </view>
          <view class="stat-card__info">
            <text class="stat-card__num">{{ dashboard?.myReservations ?? '--' }}</text>
            <text class="stat-card__label">我的预约</text>
          </view>
        </view>
      </view>

      <!-- 额外指标 -->
      <view class="extra-stats" v-if="authStore.isAdmin">
        <view class="extra-stat" @tap="goPage('/pages/borrow/index')">
          <view class="extra-stat__dot" style="background: #f56c6c" />
          <text class="extra-stat__label">逾期借用</text>
          <text class="extra-stat__num">{{ dashboard?.overdueBorrows ?? 0 }}</text>
        </view>
        <view class="extra-stat" @tap="goPage('/pages/labs/index')">
          <view class="extra-stat__dot" style="background: #409eff" />
          <text class="extra-stat__label">今日使用</text>
          <text class="extra-stat__num">{{ dashboard?.todayUsage ?? 0 }}</text>
        </view>
        <view class="extra-stat" @tap="goPage('/pages/courses/index')">
          <view class="extra-stat__dot" style="background: #67c23a" />
          <text class="extra-stat__label">进行中课程</text>
          <text class="extra-stat__num">{{ dashboard?.activeCourses ?? 0 }}</text>
        </view>
      </view>

      <!-- 实验室使用排行 -->
      <view class="section" v-if="authStore.isAdmin && labUsage.length > 0">
        <view class="section__header">
          <text class="section__title">实验室使用排行</text>
        </view>
        <view class="lab-rank-list">
          <view
            v-for="(item, index) in labUsage"
            :key="index"
            class="lab-rank-item"
          >
            <view class="rank-num" :class="{ 'rank-num--top': index < 3 }">{{ index + 1 }}</view>
            <view class="lab-rank-item__info">
              <text class="lab-rank-item__name">{{ item.labName }}</text>
              <view class="progress-bar">
                <view
                  class="progress-bar__fill"
                  :style="{ width: item.reservationRate * 100 + '%' }"
                />
              </view>
            </view>
            <text class="lab-rank-item__rate">{{ (item.reservationRate * 100).toFixed(0) }}%</text>
          </view>
        </view>
      </view>

      <!-- 我的预约趋势 -->
      <view class="section" v-if="reservationTrend.length > 0">
        <view class="section__header">
          <text class="section__title">预约趋势（近7天）</text>
        </view>
        <view class="trend-chart">
          <view
            v-for="(item, index) in reservationTrend.slice(-7)"
            :key="index"
            class="trend-bar"
          >
            <view class="trend-bar__fill" :style="{ height: getBarHeight(item.count) + 'rpx' }" />
            <text class="trend-bar__label">{{ formatDateLabel(item.date) }}</text>
          </view>
        </view>
      </view>

      <!-- 周报摘要 -->
      <view class="section" v-if="weeklySummary.length > 0 && authStore.isAdmin">
        <view class="section__header">
          <text class="section__title">本周摘要</text>
        </view>
        <view class="week-summary">
          <view class="week-item">
            <text class="week-item__num">{{ weeklySummary[weeklySummary.length - 1]?.reservations ?? 0 }}</text>
            <text class="week-item__label">预约数</text>
          </view>
          <view class="week-divider" />
          <view class="week-item">
            <text class="week-item__num">{{ weeklySummary[weeklySummary.length - 1]?.borrowRecords ?? 0 }}</text>
            <text class="week-item__label">借用数</text>
          </view>
          <view class="week-divider" />
          <view class="week-item">
            <text class="week-item__num">{{ weeklySummary[weeklySummary.length - 1]?.usageHours ?? 0 }}h</text>
            <text class="week-item__label">使用时长</text>
          </view>
        </view>
      </view>

      <view :style="{ height: safeAreaBottom + 40 + 'px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'
import { useSemesterStore } from '@/stores/semester'
import { useStatisticsStore } from '@/stores/statistics'
import { formatDate } from '@/utils/date'

const authStore = useAuthStore()
const appStore = useAppStore()
const semesterStore = useSemesterStore()
const statsStore = useStatisticsStore()

const statusBarHeight = computed(() => appStore.getStatusBarHeight())
const safeAreaBottom = computed(() => {
  const insets = (appStore.deviceInfo as unknown as { safeAreaInsets?: { bottom?: number } })?.safeAreaInsets
  return insets?.bottom ?? 0
})

const dashboard = computed(() => statsStore.dashboard)
const labUsage = computed(() => statsStore.labUsage)
const reservationTrend = computed(() => statsStore.reservationTrend)
const weeklySummary = computed(() => statsStore.weeklySummary)
const isLoading = computed(() => statsStore.isLoading)

const today = computed(() => formatDate(new Date(), 'Y-M-D'))

const maxCount = computed(() => {
  const counts = reservationTrend.value.slice(-7).map((i) => i.count)
  return Math.max(...counts, 1)
})

function getBarHeight(count: number): number {
  return Math.max(20, (count / maxCount.value) * 120)
}

function formatDateLabel(dateStr: string): string {
  const d = new Date(dateStr)
  return `${d.getMonth() + 1}/${d.getDate()}`
}

function goPage(url: string) {
  const tabBarPages = ['/pages/home/index', '/pages/labs/index', '/pages/reservations/index', '/pages/profile/index']
  const path = tabBarPages.includes(url)
  if (path) {
    uni.switchTab({ url })
  } else {
    uni.navigateTo({ url })
  }
}

async function loadData() {
  await statsStore.loadAll()
}

onMounted(async () => {
  await Promise.all([statsStore.loadAll(), semesterStore.loadCurrentSemester()])
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.dashboard-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.header {
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  padding: 20rpx 32rpx 40rpx;
  display: flex;
  align-items: flex-end;
  justify-content: space-between;

  &__title {
    font-size: 40rpx;
    font-weight: bold;
    color: #fff;
  }

  &__subtitle {
    font-size: 24rpx;
    color: rgba(255, 255, 255, 0.8);
    margin-top: 8rpx;
    display: block;
  }

  &__date {
    font-size: 24rpx;
    color: rgba(255, 255, 255, 0.8);
  }
}

.content {
  margin-top: -20rpx;
  height: calc(100vh - 200rpx);
}

.stat-cards {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24rpx;
  padding: 0 24rpx;
  margin-bottom: 24rpx;
}

.stat-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 32rpx;
  display: flex;
  align-items: center;
  gap: 24rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__icon {
    width: 88rpx;
    height: 88rpx;
    border-radius: 24rpx;
    display: flex;
    align-items: center;
    justify-content: center;

    text {
      font-size: 44rpx;
    }
  }

  &__info {
    display: flex;
    flex-direction: column;
  }

  &__num {
    font-size: 40rpx;
    font-weight: bold;
    color: #303133;
  }

  &__label {
    font-size: 24rpx;
    color: #909399;
    margin-top: 6rpx;
  }
}

.extra-stats {
  display: flex;
  background: #fff;
  margin: 0 24rpx;
  border-radius: 24rpx;
  padding: 24rpx 0;
  margin-bottom: 24rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
}

.extra-stat {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8rpx;

  &__dot {
    width: 12rpx;
    height: 12rpx;
    border-radius: 50%;
  }

  &__label {
    font-size: 22rpx;
    color: #909399;
  }

  &__num {
    font-size: 32rpx;
    font-weight: bold;
    color: #303133;
  }
}

.section {
  background: #fff;
  margin: 0 24rpx 24rpx;
  border-radius: 24rpx;
  padding: 32rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__header {
    margin-bottom: 24rpx;
  }

  &__title {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
  }
}

.lab-rank-list {
  display: flex;
  flex-direction: column;
  gap: 20rpx;
}

.lab-rank-item {
  display: flex;
  align-items: center;
  gap: 16rpx;

  &__info {
    flex: 1;
  }

  &__name {
    font-size: 26rpx;
    color: #303133;
    display: block;
    margin-bottom: 8rpx;
  }

  &__rate {
    font-size: 24rpx;
    color: #667eea;
    font-weight: bold;
    width: 80rpx;
    text-align: right;
  }
}

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

  &--top {
    background: #667eea;
    color: #fff;
  }
}

.progress-bar {
  width: 100%;
  height: 8rpx;
  background: #f0f0f0;
  border-radius: 4rpx;

  &__fill {
    height: 100%;
    background: linear-gradient(90deg, $primary, #764ba2);
    border-radius: 4rpx;
    transition: width 0.3s;
  }
}

.trend-chart {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  height: 180rpx;
  padding-top: 20rpx;
}

.trend-bar {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8rpx;

  &__fill {
    width: 48rpx;
    background: linear-gradient(180deg, $primary, #764ba2);
    border-radius: 8rpx 8rpx 0 0;
    min-height: 20rpx;
    transition: height 0.3s;
  }

  &__label {
    font-size: 20rpx;
    color: #909399;
  }
}

.week-summary {
  display: flex;
  align-items: center;
  justify-content: space-around;
}

.week-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8rpx;

  &__num {
    font-size: 40rpx;
    font-weight: bold;
    color: #667eea;
  }

  &__label {
    font-size: 24rpx;
    color: #909399;
  }
}

.week-divider {
  width: 1rpx;
  height: 60rpx;
  background: #e4e7ed;
}
</style>
