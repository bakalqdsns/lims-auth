<template>
  <view class="courses-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <Icon name="search" :size="14" color="rgba(255,255,255,0.8)" class="search-icon" />
      <input v-model="keyword" class="search-input" placeholder="搜索课程名称/编号" confirm-type="search" @confirm="loadData" />
    </view>

    <!-- 课程列表 -->
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData" @scrolltolower="loadMore">
      <view v-for="course in courses" :key="course.id" class="course-card">
        <view class="course-card__header">
          <text class="course-card__name">{{ course.name }}</text>
          <text class="course-card__code">{{ course.code }}</text>
        </view>
        <view class="course-card__body">
          <view class="course-meta">
            <text class="meta-item">学分: {{ course.credits }}</text>
            <text v-if="course.category" class="meta-item">{{ course.category }}</text>
            <text v-if="course.departmentName" class="meta-item">{{ course.departmentName }}</text>
          </view>
        </view>
        <view class="course-card__footer">
          <text class="course-teacher">授课教师: {{ course.teacherCount }} 位</text>
          <text class="course-student">选课学生: {{ course.studentCount }} 人</text>
        </view>
      </view>

      <view v-if="!isLoading && courses.length === 0" class="empty">
        <Icon name="inbox" :size="48" color="#d0d0d0" />
        <text class="empty-text">暂无课程</text>
      </view>

      <view v-if="hasMore && courses.length > 0" class="load-more">
        <Icon name="arrow-down" :size="10" color="#909399" />
        <text>加载更多...</text>
      </view>
      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getCourses } from '@/api/course'
import Icon from '@/components/Icon.vue'
import type { Course } from '@/types/teaching'

const keyword = ref('')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const courses = ref<Course[]>([])
const total = ref(0)

const hasMore = computed(() => courses.value.length < total.value)

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const resp = await getCourses({ page: 1, pageSize, search: keyword.value || undefined })
    courses.value = resp?.items ?? []
    total.value = resp?.total ?? 0
  } catch { /* ignore */ } finally { isLoading.value = false }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  try {
    const resp = await getCourses({ page: page.value, pageSize, search: keyword.value || undefined })
    courses.value.push(...(resp?.items ?? []))
  } catch { /* ignore */ }
}

onMounted(async () => { await loadData() })
</script>

<style lang="scss" scoped>
$primary: #667eea;

.courses-page { min-height: 100vh; background: #f5f7fa; }

.search-bar {
  display: flex;
  align-items: center;
  background: $primary;
  padding: 20rpx 24rpx;

  .search-icon { font-size: 32rpx; color: rgba(255,255,255,0.8); margin-right: 12rpx; }

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

.list-area { height: calc(100vh - 104rpx); padding: 24rpx; }

.course-card {
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

  &__name { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__code { font-size: 24rpx; color: #909399; }

  &__body { margin-bottom: 16rpx; }
}

.course-meta { display: flex; flex-wrap: wrap; gap: 20rpx; }
.meta-item { font-size: 24rpx; color: #909399; }

.course-card__footer {
  display: flex;
  justify-content: space-between;
  padding-top: 16rpx;
  border-top: 1rpx solid #f0f0f0;
}

.course-teacher, .course-student { font-size: 24rpx; color: #c0c4cc; }

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
