<template>
  <div class="home-container">
    <el-header class="header">
      <div class="header-left">
        <el-icon size="28" color="#409EFF"><School /></el-icon>
        <span class="title">高校实验室管理系统</span>
      </div>
      <div class="header-right">
        <el-dropdown @command="handleCommand">
          <span class="user-info">
            <el-avatar :size="32" :icon="UserFilled" />
            <span class="username">{{ authStore.user?.fullName || authStore.user?.username }}</span>
            <el-icon><ArrowDown /></el-icon>
          </span>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="profile">个人信息</el-dropdown-item>
              <el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </el-header>

    <el-container class="main-container">
      <el-aside width="200px" class="sidebar">
        <el-menu
          default-active="1"
          class="menu"
          background-color="#304156"
          text-color="#bfcbd9"
          active-text-color="#409EFF"
        >
          <el-menu-item index="1">
            <el-icon><HomeFilled /></el-icon>
            <span>首页</span>
          </el-menu-item>
          <el-menu-item index="2">
            <el-icon><User /></el-icon>
            <span>用户管理</span>
          </el-menu-item>
          <el-menu-item index="3">
            <el-icon><OfficeBuilding /></el-icon>
            <span>实验室管理</span>
          </el-menu-item>
          <el-menu-item index="4">
            <el-icon><Tools /></el-icon>
            <span>设备管理</span>
          </el-menu-item>
        </el-menu>
      </el-aside>

      <el-main class="main-content">
        <div class="welcome-card">
          <el-card>
            <template #header>
              <div class="card-header">
                <span>欢迎回来</span>
                <el-tag :type="roleType">{{ roleText }}</el-tag>
              </div>
            </template>
            <div class="user-details">
              <p><strong>用户名：</strong>{{ authStore.user?.username }}</p>
              <p><strong>姓名：</strong>{{ authStore.user?.fullName }}</p>
              <p><strong>角色：</strong>{{ roleText }}</p>
              <p><strong>登录时间：</strong>{{ currentTime }}</p>
            </div>
          </el-card>
        </div>

        <div class="stats-row">
          <el-row :gutter="20">
            <el-col :span="8">
              <el-card class="stat-card">
                <div class="stat-icon blue">
                  <el-icon><OfficeBuilding /></el-icon>
                </div>
                <div class="stat-info">
                  <div class="stat-value">12</div>
                  <div class="stat-label">实验室</div>
                </div>
              </el-card>
            </el-col>
            <el-col :span="8">
              <el-card class="stat-card">
                <div class="stat-icon green">
                  <el-icon><Tools /></el-icon>
                </div>
                <div class="stat-info">
                  <div class="stat-value">156</div>
                  <div class="stat-label">设备数量</div>
                </div>
              </el-card>
            </el-col>
            <el-col :span="8">
              <el-card class="stat-card">
                <div class="stat-icon orange">
                  <el-icon><User /></el-icon>
                </div>
                <div class="stat-info">
                  <div class="stat-value">89</div>
                  <div class="stat-label">在线用户</div>
                </div>
              </el-card>
            </el-col>
          </el-row>
        </div>
      </el-main>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  School,
  UserFilled,
  ArrowDown,
  HomeFilled,
  User,
  OfficeBuilding,
  Tools
} from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const currentTime = ref(new Date().toLocaleString())

const roleText = computed(() => {
  const roles: Record<string, string> = {
    Admin: '系统管理员',
    Teacher: '教师',
    Student: '学生'
  }
  return roles[authStore.userRole] || authStore.userRole
})

const roleType = computed(() => {
  const types: Record<string, any> = {
    Admin: 'danger',
    Teacher: 'success',
    Student: 'info'
  }
  return types[authStore.userRole] || 'info'
})

const handleCommand = (command: string) => {
  if (command === 'logout') {
    ElMessageBox.confirm('确定要退出登录吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(() => {
      authStore.logout()
      ElMessage.success('已退出登录')
      router.push('/')
    })
  } else if (command === 'profile') {
    ElMessage.info('个人信息功能开发中...')
  }
}
</script>

<style scoped>
.home-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  height: 60px;
  background: white;
  border-bottom: 1px solid #e4e7ed;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header-left .title {
  font-size: 18px;
  font-weight: 600;
  color: #303133;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background 0.3s;
}

.user-info:hover {
  background: #f5f7fa;
}

.username {
  font-size: 14px;
  color: #606266;
}

.main-container {
  flex: 1;
  display: flex;
}

.sidebar {
  background: #304156;
}

.menu {
  border-right: none;
  height: 100%;
}

.main-content {
  background: #f0f2f5;
  padding: 20px;
}

.welcome-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 16px;
  font-weight: 600;
}

.user-details p {
  margin: 12px 0;
  color: #606266;
}

.stats-row {
  margin-top: 20px;
}

.stat-card {
  display: flex;
  align-items: center;
  padding: 20px;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  margin-right: 16px;
}

.stat-icon.blue {
  background: #ecf5ff;
  color: #409eff;
}

.stat-icon.green {
  background: #f0f9eb;
  color: #67c23a;
}

.stat-icon.orange {
  background: #fdf6ec;
  color: #e6a23c;
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 24px;
  font-weight: 600;
  color: #303133;
}

.stat-label {
  font-size: 14px;
  color: #909399;
  margin-top: 4px;
}
</style>
