<template>
  <view class="login-page">
    <!-- 渐变背景 -->
    <view class="login-bg">
      <view class="login-bg__gradient" />

      <!-- Logo 区域 -->
      <view class="login-header">
        <view class="logo-wrapper">
          <view class="logo">
            <text class="logo-icon">&#xe6ba;</text>
          </view>
        </view>
        <text class="app-title">实验室管理系统</text>
        <text class="app-subtitle">Laboratory Information Management System</text>
      </view>

      <!-- 登录表单卡片 -->
      <view class="login-card">
        <view class="login-card__title">登录</view>

        <view class="form-item">
          <view class="form-label">用户名</view>
          <input
            v-model="username"
            class="form-input"
            type="text"
            placeholder="请输入用户名"
            placeholder-class="input-placeholder"
            maxlength="50"
            @confirm="handleLogin"
          />
        </view>

        <view class="form-item">
          <view class="form-label">密码</view>
          <input
            v-model="password"
            class="form-input"
            :password="!showPassword"
            placeholder="请输入密码"
            placeholder-class="input-placeholder"
            maxlength="50"
            @confirm="handleLogin"
          />
          <view class="password-toggle" @tap="showPassword = !showPassword">
            <text>{{ showPassword ? '&#xe6c7;' : '&#xe6c8;' }}</text>
          </view>
        </view>

        <!-- 登录按钮 -->
        <button
          class="login-btn"
          :class="{ 'login-btn--loading': isLoading }"
          :disabled="isLoading"
          @click="handleLogin"
        >
          <text v-if="!isLoading">登 录</text>
          <text v-else>登录中...</text>
        </button>

        <!-- 错误提示 -->
        <view v-if="errorMsg" class="error-tip">
          <text>{{ errorMsg }}</text>
        </view>
      </view>

      <!-- 测试账号快捷入口 -->
      <view class="test-accounts">
        <text class="test-accounts__label">快速填充测试账号</text>
        <view class="test-accounts__chips">
          <view class="chip" @tap="fillAccount('admin', 'admin123')">
            <text>管理员</text>
          </view>
          <view class="chip" @tap="fillAccount('teacher', 'teacher123')">
            <text>教师</text>
          </view>
          <view class="chip" @tap="fillAccount('student', 'student123')">
            <text>学生</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const showPassword = ref(false)
const isLoading = ref(false)
const errorMsg = ref('')

function fillAccount(user: string, pwd: string) {
  username.value = user
  password.value = pwd
}

async function handleLogin() {
  if (!username.value.trim()) {
    errorMsg.value = '请输入用户名'
    return
  }
  if (!password.value) {
    errorMsg.value = '请输入密码'
    return
  }

  errorMsg.value = ''
  isLoading.value = true

  const success = await authStore.login(username.value.trim(), password.value)

  isLoading.value = false

  if (success) {
    // 登录成功后跳转到首页
    uni.switchTab({ url: '/pages/home/index' })
  } else {
    errorMsg.value = authStore.error || '登录失败，请检查用户名和密码'
  }
}
</script>

<style lang="scss" scoped>
$primary: #667eea;
$primary-dark: #764ba2;

.login-page {
  min-height: 100vh;
  width: 100%;
}

.login-bg {
  min-height: 100vh;
  background: linear-gradient(135deg, $primary 0%, $primary-dark 100%);
  padding: 0 48rpx;
  box-sizing: border-box;
}

.login-header {
  padding-top: 160rpx;
  padding-bottom: 60rpx;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.logo-wrapper {
  width: 160rpx;
  height: 160rpx;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 40rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 32rpx;
}

.logo {
  width: 100rpx;
  height: 100rpx;
  display: flex;
  align-items: center;
  justify-content: center;
}

.logo-icon {
  font-size: 80rpx;
  color: #fff;
}

.app-title {
  font-size: 52rpx;
  font-weight: bold;
  color: #fff;
  margin-bottom: 12rpx;
}

.app-subtitle {
  font-size: 22rpx;
  color: rgba(255, 255, 255, 0.7);
  letter-spacing: 1rpx;
}

.login-card {
  background: #fff;
  border-radius: 32rpx;
  padding: 56rpx 48rpx;
  box-shadow: 0 8rpx 32rpx rgba(0, 0, 0, 0.15);

  &__title {
    font-size: 40rpx;
    font-weight: bold;
    color: #303133;
    margin-bottom: 48rpx;
  }
}

.form-item {
  position: relative;
  margin-bottom: 32rpx;
}

.form-label {
  font-size: 26rpx;
  color: #606266;
  margin-bottom: 12rpx;
}

.form-input {
  width: 100%;
  height: 88rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 32rpx;
  font-size: 30rpx;
  color: #303133;
  box-sizing: border-box;
  border: 2rpx solid transparent;
  transition: border-color 0.2s;

  &:focus {
    border-color: $primary;
    background: #fff;
  }
}

.input-placeholder {
  color: #c0c4cc;
  font-size: 28rpx;
}

.password-toggle {
  position: absolute;
  right: 24rpx;
  bottom: 24rpx;
  font-size: 36rpx;
  color: #909399;
}

.login-btn {
  width: 100%;
  height: 96rpx;
  background: linear-gradient(135deg, $primary 0%, $primary-dark 100%);
  border-radius: 16rpx;
  color: #fff;
  font-size: 32rpx;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 24rpx;
  border: none;

  &::after {
    border: none;
  }

  &--loading {
    opacity: 0.7;
  }
}

.error-tip {
  margin-top: 24rpx;
  padding: 16rpx 24rpx;
  background: rgba(245, 108, 108, 0.1);
  border-radius: 12rpx;
  text {
    font-size: 24rpx;
    color: #f56c6c;
  }
}

.test-accounts {
  margin-top: 48rpx;
  display: flex;
  flex-direction: column;
  align-items: center;

  &__label {
    font-size: 22rpx;
    color: rgba(255, 255, 255, 0.7);
    margin-bottom: 16rpx;
  }

  &__chips {
    display: flex;
    gap: 16rpx;
  }
}

.chip {
  padding: 12rpx 24rpx;
  background: rgba(255, 255, 255, 0.15);
  border: 2rpx solid rgba(255, 255, 255, 0.3);
  border-radius: 999rpx;
  color: #fff;
  font-size: 24rpx;
}
</style>
