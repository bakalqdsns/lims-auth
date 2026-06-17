<template>
  <view class="schedule-page">
    <!-- 星期导航 -->
    <view class="week-nav">
      <view
        v-for="day in weekDays"
        :key="day.dayOfWeek"
        class="week-day"
        :class="{ active: selectedDay === day.dayOfWeek }"
        @tap="selectDay(day.dayOfWeek)"
      >
        <text class="week-day__name">{{ day.dayName }}</text>
        <text class="week-day__date">{{ day.dateStr }}</text>
      </view>
    </view>

    <!-- 课表内容 -->
    <scroll-view class="schedule-content" scroll-y refresher-enabled @refresherrefresh="loadData">
      <view v-if="todaySchedules.length > 0" class="schedule-list">
        <view v-for="(item, index) in todaySchedules" :key="index" class="schedule-item">
          <view class="schedule-item__time">
            <text class="period">{{ item.startPeriod }}-{{ item.endPeriod }}节</text>
            <text class="time-range">{{ item.startTime }} - {{ item.endTime }}</text>
          </view>
          <view class="schedule-item__card">
            <text class="schedule-course">{{ item.courseName }}</text>
            <text class="schedule-lab">{{ item.labName }} ({{ item.labCode }})</text>
            <text class="schedule-class">班级: {{ item.className }}</text>
          </view>
        </view>
      </view>

      <view v-else class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">今日无课程安排</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSemesterStore } from '@/stores/semester'
import { useAuthStore } from '@/stores/auth'
import { getSchedulesByClass } from '@/api/schedule'
import { getSchedulesByTeacher } from '@/api/schedule'
import { formatDate } from '@/utils/date'
import type { Schedule } from '@/types/schedule'

const semesterStore = useSemesterStore()
const authStore = useAuthStore()

const selectedDay = ref(new Date().getDay() || 7) // 周一=1, 周日=7
const schedules = ref<Schedule[]>([])
const isLoading = ref(false)

const weekDays = computed(() => {
  const days = []
  const now = new Date()
  for (let i = 0; i < 7; i++) {
    const d = new Date(now)
    d.setDate(now.getDate() - now.getDay() + 1 + i)
    days.push({
      dayOfWeek: i === 6 ? 7 : i + 1,
      dayName: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'][i],
      dateStr: `${d.getMonth() + 1}/${d.getDate()}`,
      date: formatDate(d, 'Y-M-D'),
    })
  }
  return days
})

const todaySchedules = computed(() =>
  schedules.value
    .filter((s) => s.dayOfWeek === selectedDay.value)
    .sort((a, b) => a.startPeriod - b.startPeriod)
)

function selectDay(day: number) {
  selectedDay.value = day
}

async function loadData() {
  isLoading.value = true
  try {
    const user = authStore.currentUser
    if (!user) return

    if (authStore.isStudent) {
      const resp = await getSchedulesByClass(user.id)
      schedules.value = resp?.items ?? []
    } else if (authStore.isTeacher) {
      const resp = await getSchedulesByTeacher(user.id)
      schedules.value = resp?.items ?? []
    } else {
      const resp = await getSchedulesByClass(user.id)
      schedules.value = resp?.items ?? []
    }
  } catch { /* ignore */ } finally { isLoading.value = false }
}

onMounted(async () => {
  await loadData()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.schedule-page { min-height: 100vh; background: #f5f7fa; }

.week-nav {
  display: flex;
  background: #fff;
  padding: 16rpx 8rpx;
  overflow-x: auto;
}

.week-day {
  flex: 1;
  min-width: 100rpx;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8rpx;
  padding: 16rpx 8rpx;
  border-radius: 16rpx;
  transition: all 0.2s;

  &.active {
    background: $primary;
    .week-day__name { color: #fff; }
    .week-day__date { color: rgba(255, 255, 255, 0.8); }
  }

  &__name { font-size: 28rpx; font-weight: bold; color: #303133; }
  &__date { font-size: 22rpx; color: #909399; }
}

.schedule-content { height: calc(100vh - 160rpx); padding: 24rpx; }

.schedule-list { display: flex; flex-direction: column; gap: 20rpx; }

.schedule-item {
  display: flex;
  gap: 20rpx;

  &__time {
    width: 120rpx;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding-top: 8rpx;
  }

  &__card {
    flex: 1;
    background: #fff;
    border-radius: 20rpx;
    padding: 24rpx;
    box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
    border-left: 6rpx solid $primary;
  }
}

.period { font-size: 28rpx; font-weight: bold; color: $primary; }
.time-range { font-size: 20rpx; color: #909399; margin-top: 6rpx; }

.schedule-course { font-size: 30rpx; font-weight: bold; color: #303133; display: block; margin-bottom: 8rpx; }
.schedule-lab { font-size: 24rpx; color: #606266; display: block; margin-bottom: 6rpx; }
.schedule-class { font-size: 24rpx; color: #909399; display: block; }

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }
</style>
