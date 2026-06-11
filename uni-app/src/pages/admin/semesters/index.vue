<template>
  <view class="admin-semesters-page">
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData">
      <view v-for="sem in semesters" :key="sem.id" class="sem-card" :class="{ 'sem-card--current': sem.isCurrent }">
        <view class="sem-card__header">
          <text class="sem-name">{{ sem.name }}</text>
          <view v-if="sem.isCurrent" class="current-badge">当前学期</view>
          <view class="status-tag" :class="sem.status === 1 ? 'active' : 'inactive'">{{ sem.status === 1 ? '进行中' : '已结束' }}</view>
        </view>
        <view class="sem-card__body">
          <text class="sem-meta">起止: {{ sem.startDate }} ~ {{ sem.endDate }}</text>
          <text class="sem-meta">共 {{ sem.weekCount }} 周 | 当前第 {{ sem.currentWeek }} 周</text>
        </view>
        <view class="sem-card__footer">
          <text class="sem-date">创建: {{ sem.createdAt?.substring(0, 10) }}</text>
          <view class="action-btns" v-if="authStore.isAdmin">
            <text v-if="!sem.isCurrent" class="action-text" @tap="setCurrent(sem)">设为当前</text>
            <text class="action-text action-text--danger" @tap="deleteSem(sem)">删除</text>
          </view>
        </view>
      </view>

      <view v-if="!isLoading && semesters.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无学期</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>

    <!-- 新增按钮 -->
    <view class="bottom-bar">
      <button class="add-btn" @tap="showAddSheet = true">+ 新增学期</button>
    </view>

    <!-- 新增弹窗 -->
    <view v-if="showAddSheet" class="overlay" @tap="showAddSheet = false">
      <view class="sheet" @tap.stop>
        <view class="sheet__header">
          <text class="sheet__title">新增学期</text>
          <text class="sheet__close" @tap="showAddSheet = false">&#xe6c7;</text>
        </view>
        <view class="sheet__body">
          <view class="form-item">
            <text class="form-label">学期名称 *</text>
            <input v-model="formData.name" class="form-input" placeholder="如: 2024-2025学年 第一学期" />
          </view>
          <view class="form-item">
            <text class="form-label">开始日期 *</text>
            <picker mode="date" :value="formData.startDate" @change="(e: any) => formData.startDate = e.detail.value">
              <view class="picker-val">{{ formData.startDate || '请选择' }}</view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">结束日期 *</text>
            <picker mode="date" :value="formData.endDate" @change="(e: any) => formData.endDate = e.detail.value">
              <view class="picker-val">{{ formData.endDate || '请选择' }}</view>
            </picker>
          </view>
        </view>
        <view class="sheet__footer">
          <button class="submit-btn" :disabled="!canSubmit" @tap="submitForm">创建学期</button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { getSemesters, createSemester, deleteSemester, setCurrentSemester } from '@/api/semester'
import type { Semester } from '@/types/semester'

const authStore = useAuthStore()

const isLoading = ref(false)
const semesters = ref<Semester[]>([])
const showAddSheet = ref(false)
const formData = reactive({ name: '', startDate: '', endDate: '' })

const canSubmit = computed(() => formData.name.trim() && formData.startDate && formData.endDate)

async function loadData() {
  isLoading.value = true
  try {
    const resp = await getSemesters({ pageSize: 100 })
    semesters.value = resp.items
  } catch { /* ignore */ } finally { isLoading.value = false }
}

async function setCurrent(sem: Semester) {
  uni.showModal({
    title: '确认',
    content: `将 "${sem.name}" 设为当前学期？`,
    success: async (res) => {
      if (res.confirm) {
        try {
          uni.showLoading({ title: '设置中...' })
          await setCurrentSemester(sem.id)
          uni.hideLoading()
          uni.showToast({ title: '设置成功', icon: 'success' })
          await loadData()
        } catch { uni.hideLoading() }
      }
    },
  })
}

function deleteSem(sem: Semester) {
  if (sem.isCurrent) {
    uni.showToast({ title: '不能删除当前学期', icon: 'none' })
    return
  }
  uni.showModal({
    title: '确认删除',
    content: `确定删除 "${sem.name}" 吗？`,
    success: async (res) => {
      if (res.confirm) {
        try {
          await deleteSemester(sem.id)
          uni.showToast({ title: '已删除', icon: 'success' })
          await loadData()
        } catch { /* ignore */ }
      }
    },
  })
}

async function submitForm() {
  if (!canSubmit.value) return
  try {
    uni.showLoading({ title: '创建中...' })
    await createSemester(formData)
    uni.hideLoading()
    uni.showToast({ title: '创建成功', icon: 'success' })
    showAddSheet.value = false
    Object.assign(formData, { name: '', startDate: '', endDate: '' })
    await loadData()
  } catch { uni.hideLoading() }
}

onMounted(async () => { await loadData() })
</script>

<style lang="scss" scoped>
$primary: #667eea;

.admin-semesters-page { min-height: 100vh; background: #f5f7fa; }

.list-area { height: calc(100vh - 160rpx); padding: 24rpx; }

.sem-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0,0,0,0.06);

  &--current { border: 2rpx solid $primary; }

  &__header {
    display: flex;
    align-items: center;
    gap: 12rpx;
    margin-bottom: 16rpx;
  }

  &__body { margin-bottom: 16rpx; }
  &__footer { display: flex; align-items: center; justify-content: space-between; padding-top: 16rpx; border-top: 1rpx solid #f0f0f0; }
}

.sem-name { font-size: 30rpx; font-weight: bold; color: #303133; flex: 1; }

.current-badge {
  background: $primary;
  color: #fff;
  font-size: 22rpx;
  padding: 4rpx 16rpx;
  border-radius: 999rpx;
}

.status-tag {
  font-size: 24rpx;
  padding: 4rpx 16rpx;
  border-radius: 999rpx;
  &.active { background: #e8f8e8; color: #67c23a; }
  &.inactive { background: #f5f5f5; color: #909399; }
}

.sem-meta { font-size: 26rpx; color: #909399; display: block; }
.sem-date { font-size: 22rpx; color: #c0c4cc; }

.action-btns { display: flex; gap: 16rpx; }
.action-text { font-size: 26rpx; color: $primary; }
.action-text--danger { color: #f56c6c; }

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }

.bottom-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 24rpx;
  background: #fff;
  box-shadow: 0 -4rpx 16rpx rgba(0,0,0,0.06);
}

.add-btn {
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
}

.overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); z-index: 999; display: flex; align-items: flex-end; }

.sheet {
  width: 100%;
  background: #fff;
  border-radius: 32rpx 32rpx 0 0;
  display: flex;
  flex-direction: column;

  &__header { display: flex; align-items: center; justify-content: space-between; padding: 32rpx; border-bottom: 1rpx solid #f0f0f0; }
  &__title { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__close { font-size: 36rpx; color: #909399; }
  &__body { padding: 32rpx; }
  &__footer { padding: 24rpx 32rpx; border-top: 1rpx solid #f0f0f0; }
}

.form-item { margin-bottom: 28rpx; }
.form-label { font-size: 28rpx; color: #606266; display: block; margin-bottom: 12rpx; }

.form-input {
  width: 100%;
  height: 80rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
}

.picker-val {
  width: 100%;
  height: 80rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  display: flex;
  align-items: center;
}

.submit-btn {
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
  &[disabled] { background: #d0d0d0; color: #909399; }
}
</style>
