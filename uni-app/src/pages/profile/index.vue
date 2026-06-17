<template>
  <view class="profile-page">
    <!-- 顶部背景 -->
    <view class="profile-header" :style="{ paddingTop: statusBarHeight + 'px' }">
      <view class="profile-bg" />
      <view class="profile-info">
        <view class="avatar">
          <text>{{ userAvatar }}</text>
        </view>
        <view class="profile-info__text">
          <text class="profile-name">{{ authStore.currentUser?.fullName || '用户' }}</text>
          <text class="profile-username">{{ authStore.currentUser?.username }}</text>
        </view>
        <view class="role-badge">
          <text>{{ roleLabel }}</text>
        </view>
      </view>
    </view>

    <!-- 个人信息卡片 -->
    <view class="profile-card">
      <view class="card-item">
        <text class="card-label">手机号码</text>
        <text class="card-value">{{ authStore.currentUser?.phone || '未设置' }}</text>
      </view>
      <view class="card-item">
        <text class="card-label">电子邮箱</text>
        <text class="card-value">{{ authStore.currentUser?.email || '未设置' }}</text>
      </view>
      <view class="card-item">
        <text class="card-label">所属部门</text>
        <text class="card-value">{{ authStore.currentUser?.departmentName || '未设置' }}</text>
      </view>
      <view class="card-item">
        <text class="card-label">学号/工号</text>
        <text class="card-value">{{ authStore.currentUser?.studentId || authStore.currentUser?.employeeId || '未设置' }}</text>
      </view>
    </view>

    <!-- 操作菜单 -->
    <view class="menu-section">
      <view class="menu-item" @tap="goPage('/pages/profile/index')">
        <Icon name="pencil" :size="16" color="#667eea" class="menu-icon" />
        <text class="menu-label">编辑资料</text>
        <Icon name="chevron-right" :size="14" color="#c0c4cc" class="menu-arrow" />
      </view>
      <view class="menu-item" @tap="showChangePassword">
        <Icon name="lock" :size="16" color="#667eea" class="menu-icon" />
        <text class="menu-label">修改密码</text>
        <Icon name="chevron-right" :size="14" color="#c0c4cc" class="menu-arrow" />
      </view>
      <view class="menu-item" @tap="showAbout">
        <Icon name="info" :size="16" color="#667eea" class="menu-icon" />
        <text class="menu-label">关于我们</text>
        <Icon name="chevron-right" :size="14" color="#c0c4cc" class="menu-arrow" />
      </view>
    </view>

    <!-- 退出登录 -->
    <view class="logout-section">
      <button class="logout-btn" @tap="handleLogout">退出登录</button>
    </view>

    <!-- 版本信息 -->
    <view class="version">
      <text>v{{ appStore.version }}</text>
    </view>
  </view>

  <!-- 修改密码弹窗 -->
  <view v-if="showPwdSheet" class="overlay" @tap="showPwdSheet = false">
    <view class="sheet" @tap.stop>
      <view class="sheet__header">
        <text class="sheet__title">修改密码</text>
        <view class="sheet__close" @tap="showPwdSheet = false">
          <Icon name="close" :size="14" color="#909399" />
        </view>
      </view>
      <view class="sheet__body">
        <view class="form-item">
          <text class="form-label">旧密码</text>
          <input v-model="pwdForm.oldPassword" type="password" class="form-input" placeholder="请输入旧密码" />
        </view>
        <view class="form-item">
          <text class="form-label">新密码</text>
          <input v-model="pwdForm.newPassword" type="password" class="form-input" placeholder="请输入新密码" />
        </view>
        <view class="form-item">
          <text class="form-label">确认密码</text>
          <input v-model="pwdForm.confirmPassword" type="password" class="form-input" placeholder="请再次输入新密码" />
        </view>
      </view>
      <view class="sheet__footer">
        <button class="submit-btn" :disabled="!canSubmitPwd" @tap="submitPassword">确认修改</button>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, reactive } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useAppStore } from '@/stores/app'
import Icon from '@/components/Icon.vue'

const authStore = useAuthStore()
const appStore = useAppStore()

const statusBarHeight = computed(() => appStore.getStatusBarHeight())
const showPwdSheet = ref(false)

const pwdForm = reactive({ oldPassword: '', newPassword: '', confirmPassword: '' })

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

const canSubmitPwd = computed(() =>
  pwdForm.oldPassword && pwdForm.newPassword && pwdForm.newPassword === pwdForm.confirmPassword && pwdForm.newPassword.length >= 6
)

function goPage(url: string) {
  uni.showToast({ title: '编辑资料功能开发中', icon: 'none' })
}

function showChangePassword() {
  showPwdSheet.value = true
}

function showAbout() {
  uni.showModal({
    title: '关于我们',
    content: `实验室预约管理系统\n版本: ${appStore.version}\n基于 UniApp + Vue 3 构建`,
    showCancel: false,
  })
}

async function submitPassword() {
  if (!canSubmitPwd.value) return
  try {
    uni.showLoading({ title: '修改中...' })
    await authStore.changePassword(pwdForm.oldPassword, pwdForm.newPassword)
    uni.hideLoading()
    uni.showToast({ title: '密码修改成功', icon: 'success' })
    showPwdSheet.value = false
    pwdForm.oldPassword = ''
    pwdForm.newPassword = ''
    pwdForm.confirmPassword = ''
  } catch {
    uni.hideLoading()
    uni.showToast({ title: authStore.error || '修改失败', icon: 'none' })
  }
}

function handleLogout() {
  uni.showModal({
    title: '确认退出',
    content: '确定要退出登录吗？',
    success: (res) => {
      if (res.confirm) {
        authStore.logout()
      }
    },
  })
}
</script>

<style lang="scss" scoped>
$primary: #667eea;

.profile-page { min-height: 100vh; background: #f5f7fa; }

.profile-header {
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  padding: 40rpx 32rpx 80rpx;
  position: relative;
}

.profile-bg {
  position: absolute;
  top: 0;
  right: 0;
  width: 300rpx;
  height: 300rpx;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 0 0 0 150rpx;
}

.profile-info {
  display: flex;
  align-items: center;
  gap: 24rpx;
  position: relative;
  z-index: 1;
}

.avatar {
  width: 120rpx;
  height: 120rpx;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.25);
  border: 4rpx solid rgba(255, 255, 255, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;

  text {
    font-size: 48rpx;
    font-weight: bold;
    color: #fff;
  }
}

.profile-info__text {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8rpx;
}

.profile-name { font-size: 36rpx; font-weight: bold; color: #fff; }
.profile-username { font-size: 26rpx; color: rgba(255, 255, 255, 0.8); }

.role-badge {
  background: rgba(255, 255, 255, 0.2);
  color: #fff;
  font-size: 22rpx;
  padding: 8rpx 20rpx;
  border-radius: 999rpx;
}

.profile-card {
  background: #fff;
  margin: -40rpx 24rpx 24rpx;
  border-radius: 24rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
}

.card-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 28rpx 32rpx;
  border-bottom: 1rpx solid #f0f0f0;

  &:last-child { border-bottom: none; }
}

.card-label { font-size: 28rpx; color: #909399; }
.card-value { font-size: 28rpx; color: #303133; }

.menu-section {
  background: #fff;
  margin: 0 24rpx 24rpx;
  border-radius: 24rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 32rpx;
  border-bottom: 1rpx solid #f0f0f0;

  &:last-child { border-bottom: none; }
}

.menu-icon {
  width: 36rpx;
  height: 36rpx;
  margin-right: 20rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  color: $primary;
}

.menu-label { flex: 1; font-size: 28rpx; color: #303133; }

.menu-arrow { color: #c0c4cc; }

.logout-section { padding: 0 24rpx; }

.logout-btn {
  width: 100%;
  height: 88rpx;
  background: #fff;
  border-radius: 24rpx;
  color: #f56c6c;
  font-size: 30rpx;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;

  &::after { border: none; }
}

.version {
  text-align: center;
  padding: 40rpx;
  font-size: 24rpx;
  color: #c0c4cc;
}

.overlay {
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

.sheet {
  width: 100%;
  background: #fff;
  border-radius: 32rpx 32rpx 0 0;
  display: flex;
  flex-direction: column;

  &__header { display: flex; align-items: center; justify-content: space-between; padding: 32rpx; border-bottom: 1rpx solid #f0f0f0; }
  &__title { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__close {
    width: 48rpx;
    height: 48rpx;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #909399;
  }
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
