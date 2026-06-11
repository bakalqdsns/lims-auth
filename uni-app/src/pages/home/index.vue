<template>
  <view class="home-page">
    <!-- 顶部状态栏占位 -->
    <view :style="{ height: statusBarHeight + 'px' }" />

    <!-- 顶部导航 -->
    <view class="nav-bar" :style="{ paddingTop: statusBarHeight + 'px' }">
      <view class="nav-bar__left">
        <text class="nav-bar__greeting">您好，{{ authStore.currentUser?.fullName || '用户' }}</text>
        <text class="nav-bar__role">{{ roleLabel }}</text>
      </view>
      <view class="nav-bar__right">
        <view class="avatar">
          <text>{{ userAvatar }}</text>
        </view>
      </view>
    </view>

    <!-- 主内容区域 -->
    <scroll-view
      class="home-content"
      scroll-y
      refresher-enabled
      :refresher-triggered="isRefreshing"
      @refresherrefresh="onRefresh"
    >
      <!-- 快捷入口卡片 -->
      <view class="quick-entry">
        <view class="quick-entry__title">常用功能</view>
        <view class="quick-grid">
          <view class="quick-item" @tap="goPage('/pages/labs/index')">
            <view class="quick-item__icon" style="background: #e8f4ff">
              <text style="color: #409eff">&#xe6a1;</text>
            </view>
            <text class="quick-item__label">实验室</text>
          </view>
          <view class="quick-item" @tap="goPage('/pages/reservations/index')">
            <view class="quick-item__icon" style="background: #fdf6ec">
              <text style="color: #e6a23c">&#xe6a2;</text>
            </view>
            <text class="quick-item__label">我的预约</text>
          </view>
          <view class="quick-item" @tap="goPage('/pages/equipment/index')">
            <view class="quick-item__icon" style="background: #f0f9eb">
              <text style="color: #67c23a">&#xe6a3;</text>
            </view>
            <text class="quick-item__label">设备借还</text>
          </view>
          <view class="quick-item" @tap="goPage('/pages/courses/index')">
            <view class="quick-item__icon" style="background: #fef0f0">
              <text style="color: #f56c6c">&#xe6a4;</text>
            </view>
            <text class="quick-item__label">课程中心</text>
          </view>
          <view class="quick-item" @tap="goPage('/pages/schedules/index')">
            <view class="quick-item__icon" style="background: #f4f4f5">
              <text style="color: #909399">&#xe6a5;</text>
            </view>
            <text class="quick-item__label">我的课表</text>
          </view>
          <view class="quick-item" @tap="goPage('/pages/borrow/index')">
            <view class="quick-item__icon" style="background: #ecf5ff">
              <text style="color: #667eea">&#xe6a6;</text>
            </view>
            <text class="quick-item__label">借用记录</text>
          </view>
        </view>
      </view>

      <!-- 管理员入口 -->
      <view v-if="authStore.isAdmin" class="admin-entry">
        <view class="admin-entry__title">
          <text>管理端</text>
          <view class="admin-entry__badge">
            <text>管理员</text>
          </view>
        </view>
        <view class="admin-grid">
          <view class="admin-item" @tap="goPage('/pages/admin/users/index')">
            <view class="admin-item__icon">
              <text>&#xe6b0;</text>
            </view>
            <text class="admin-item__label">用户管理</text>
          </view>
          <view class="admin-item" @tap="goPage('/pages/admin/equipment/index')">
            <view class="admin-item__icon">
              <text>&#xe6b1;</text>
            </view>
            <text class="admin-item__label">设备管理</text>
          </view>
          <view class="admin-item" @tap="goPage('/pages/admin/teaching-apps/index')">
            <view class="admin-item__icon">
              <text>&#xe6b2;</text>
            </view>
            <text class="admin-item__label">审批中心</text>
          </view>
          <view class="admin-item" @tap="goPage('/pages/admin/statistics/index')">
            <view class="admin-item__icon">
              <text>&#xe6b3;</text>
            </view>
            <text class="admin-item__label">数据统计</text>
          </view>
          <view class="admin-item" @tap="goPage('/pages/admin/semesters/index')">
            <view class="admin-item__icon">
              <text>&#xe6b4;</text>
            </view>
            <text class="admin-item__label">学期管理</text>
          </view>
          <view class="admin-item" @tap="goPage('/pages/admin/roles/index')">
            <view class="admin-item__icon">
              <text>&#xe6b5;</text>
            </view>
            <text class="admin-item__label">角色权限</text>
          </view>
        </view>
      </view>

      <!-- 底部安全区域占位 -->
      <view :style="{ height: safeAreaBottom + 40 + 'px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'
import { useSemesterStore } from '@/stores/semester'

const authStore = useAuthStore()
const appStore = useAppStore()
const semesterStore = useSemesterStore()

const isRefreshing = ref(false)

const statusBarHeight = computed(() => appStore.getStatusBarHeight())
const safeAreaBottom = computed(() => {
  const insets = (appStore.deviceInfo as unknown as { safeAreaInsets?: { bottom?: number } })?.safeAreaInsets
  return insets?.bottom ?? 0
})

const userAvatar = computed(() => {
  const name = authStore.currentUser?.fullName || '用户'
  return name.charAt(0).toUpperCase()
})

const roleLabel = computed(() => {
  const user = authStore.currentUser
  if (!user) return ''
  if (user.roles?.includes('super_admin')) return '超级管理员'
  if (user.roles?.includes('admin')) return '管理员'
  if (user.roles?.includes('lab_admin')) return '实验室管理员'
  if (user.roles?.includes('teacher')) return '教师'
  if (user.roles?.includes('student')) return '学生'
  return ''
})

function goPage(url: string) {
  const tabBarPages = ['/pages/home/index', '/pages/labs/index', '/pages/reservations/index', '/pages/profile/index']
  const path = tabBarPages.includes(url)
  if (path) {
    uni.switchTab({ url })
  } else {
    uni.navigateTo({ url })
  }
}

async function onRefresh() {
  await authStore.fetchCurrentUser()
  await semesterStore.loadCurrentSemester()
  isRefreshing.value = false
}

onMounted(async () => {
  await authStore.fetchCurrentUser()
  await semesterStore.loadCurrentSemester()
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.home-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.nav-bar {
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  padding: 20rpx 32rpx 40rpx;
  display: flex;
  align-items: center;
  justify-content: space-between;

  &__left {
    display: flex;
    flex-direction: column;
  }

  &__greeting {
    font-size: 36rpx;
    font-weight: bold;
    color: #fff;
  }

  &__role {
    font-size: 24rpx;
    color: rgba(255, 255, 255, 0.8);
    margin-top: 6rpx;
  }

  &__right {
    display: flex;
    align-items: center;
  }
}

.avatar {
  width: 72rpx;
  height: 72rpx;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.25);
  border: 2rpx solid rgba(255, 255, 255, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;

  text {
    font-size: 28rpx;
    font-weight: bold;
    color: #fff;
  }
}

.home-content {
  margin-top: -20rpx;
  height: calc(100vh - 200rpx);
}

.quick-entry {
  background: #fff;
  margin: 0 24rpx;
  border-radius: 24rpx;
  padding: 32rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__title {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
    margin-bottom: 24rpx;
  }
}

.quick-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 32rpx;
}

.quick-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12rpx;

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

  &__label {
    font-size: 24rpx;
    color: #606266;
  }
}

.admin-entry {
  background: #fff;
  margin: 24rpx;
  border-radius: 24rpx;
  padding: 32rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__title {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
    margin-bottom: 24rpx;
    display: flex;
    align-items: center;
    gap: 12rpx;
  }

  &__badge {
    background: linear-gradient(135deg, $primary, #764ba2);
    color: #fff;
    font-size: 20rpx;
    padding: 4rpx 12rpx;
    border-radius: 999rpx;
  }
}

.admin-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 32rpx;
}

.admin-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12rpx;

  &__icon {
    width: 80rpx;
    height: 80rpx;
    border-radius: 50%;
    background: #f5f7fa;
    display: flex;
    align-items: center;
    justify-content: center;

    text {
      font-size: 36rpx;
      color: $primary;
    }
  }

  &__label {
    font-size: 24rpx;
    color: #606266;
  }
}
</style>
