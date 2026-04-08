<template>
  <div class="home-container">
    <el-container class="layout-container">
      <!-- 侧边栏 -->
      <el-aside width="220px" class="sidebar">
        <div class="logo">
          <el-icon class="logo-icon"><Collection /></el-icon>
          <span class="logo-text">LIMS 系统</span>
        </div>

        <el-menu
          :default-active="activeMenu"
          class="sidebar-menu"
          router
          background-color="#304156"
          text-color="#bfcbd9"
          active-text-color="#409EFF"
        >
          <el-menu-item index="/home">
            <el-icon><HomeFilled /></el-icon>
            <span>首页</span>
          </el-menu-item>

          <el-sub-menu index="/system" v-if="canAccessSystem">
            <template #title>
              <el-icon><Setting /></el-icon>
              <span>系统管理</span>
            </template>
            <el-menu-item v-if="hasPermission('user:read')" index="/system/users">
              <el-icon><User /></el-icon>
              <span>用户管理</span>
            </el-menu-item>
            <el-menu-item v-if="hasPermission('role:read')" index="/system/roles">
              <el-icon><UserFilled /></el-icon>
              <span>角色管理</span>
            </el-menu-item>
            <el-menu-item v-if="hasPermission('department:read')" index="/system/departments">
              <el-icon><OfficeBuilding /></el-icon>
              <span>部门管理</span>
            </el-menu-item>
          </el-sub-menu>

          <el-sub-menu index="/teaching">
            <template #title>
              <el-icon><School /></el-icon>
              <span>教学管理</span>
            </template>
            <el-menu-item index="/teaching/semesters">
              <el-icon><Calendar /></el-icon>
              <span>学期管理</span>
            </el-menu-item>
            <el-menu-item index="/teaching/courses">
              <el-icon><Reading /></el-icon>
              <span>课程管理</span>
            </el-menu-item>
            <el-menu-item index="/teaching/majors">
              <el-icon><School /></el-icon>
              <span>专业管理</span>
            </el-menu-item>
            <el-menu-item index="/teaching/classes">
              <el-icon><UserFilled /></el-icon>
              <span>班级管理</span>
            </el-menu-item>
            <el-menu-item index="/teaching/tasks">
              <el-icon><Timer /></el-icon>
              <span>教学任务</span>
            </el-menu-item>
            <el-menu-item index="/teaching/periods">
              <el-icon><Timer /></el-icon>
              <span>节次时间</span>
            </el-menu-item>
          </el-sub-menu>

          <el-sub-menu index="/lab" v-if="hasPermission('lab:read') || hasPermission('equipment:read')">
            <template #title>
              <el-icon><OfficeBuilding /></el-icon>
              <span>实验室管理</span>
            </template>
            <el-menu-item v-if="hasPermission('lab:read')" index="/lab/labs">
              <el-icon><OfficeBuilding /></el-icon>
              <span>实验室管理</span>
            </el-menu-item>
            <el-menu-item v-if="hasPermission('equipment:read')" index="/lab/equipments">
              <el-icon><Tools /></el-icon>
              <span>设备管理</span>
            </el-menu-item>
          </el-sub-menu>
        </el-menu>
      </el-aside>

      <el-container>
        <!-- 顶部导航 -->
        <el-header class="header">
          <div class="header-left">
            <breadcrumb />
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
                  <el-dropdown-item command="profile">个人资料</el-dropdown-item>
                  <el-dropdown-item command="password">修改密码</el-dropdown-item>
                  <el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </el-header>

        <!-- 主内容区 -->
        <el-main class="main-content">
          <router-view />
        </el-main>
      </el-container>
    </el-container>

    <!-- 修改密码对话框 -->
    <el-dialog
      v-model="passwordDialogVisible"
      title="修改密码"
      width="400px"
      destroy-on-close
    >
      <el-form
        ref="passwordFormRef"
        :model="passwordForm"
        :rules="passwordRules"
        label-width="100px"
      >
        <el-form-item label="原密码" prop="oldPassword">
          <el-input
            v-model="passwordForm.oldPassword"
            type="password"
            placeholder="请输入原密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="新密码" prop="newPassword">
          <el-input
            v-model="passwordForm.newPassword"
            type="password"
            placeholder="请输入新密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="passwordForm.confirmPassword"
            type="password"
            placeholder="请再次输入新密码"
            show-password
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="passwordDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="changingPassword" @click="handleChangePassword">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import {
  HomeFilled,
  Setting,
  User,
  UserFilled,
  OfficeBuilding,
  Collection,
  ArrowDown,
  Calendar,
  Reading,
  School,
  Timer,
  Tools
} from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'
import { userApi } from '../api/system'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const hasPermission = authStore.hasPermission

// 当前激活的菜单
const activeMenu = computed(() => route.path)

// 是否可以访问系统管理
const canAccessSystem = computed(() => {
  return hasPermission('user:read') ||
         hasPermission('role:read') ||
         hasPermission('department:read')
})

// 用户菜单命令
const handleCommand = (command: string) => {
  switch (command) {
    case 'profile':
      ElMessage.info('个人资料功能开发中...')
      break
    case 'password':
      passwordDialogVisible.value = true
      break
    case 'logout':
      authStore.logout()
      router.push('/')
      break
  }
}

// 修改密码
const passwordDialogVisible = ref(false)
const changingPassword = ref(false)
const passwordFormRef = ref<FormInstance>()

const passwordForm = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const validateConfirmPassword = (_rule: any, value: string, callback: any) => {
  if (value !== passwordForm.newPassword) {
    callback(new Error('两次输入的密码不一致'))
  } else {
    callback()
  }
}

const passwordRules: FormRules = {
  oldPassword: [
    { required: true, message: '请输入原密码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请再次输入新密码', trigger: 'blur' },
    { validator: validateConfirmPassword, trigger: 'blur' }
  ]
}

const handleChangePassword = async () => {
  const valid = await passwordFormRef.value?.validate().catch(() => false)
  if (!valid) return

  changingPassword.value = true
  try {
    const res = await userApi.changePassword(passwordForm.oldPassword, passwordForm.newPassword)
    if (res.data.code === 200) {
      ElMessage.success('密码修改成功，请重新登录')
      passwordDialogVisible.value = false
      authStore.logout()
      router.push('/')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('密码修改失败')
  } finally {
    changingPassword.value = false
  }
}
</script>

<style scoped>
.home-container {
  height: 100vh;
}

.layout-container {
  height: 100%;
}

/* 侧边栏 */
.sidebar {
  background-color: #304156;
  color: #fff;
}

.logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #2b3649;
  border-bottom: 1px solid #1f2d3d;
}

.logo-icon {
  font-size: 28px;
  color: #409EFF;
  margin-right: 10px;
}

.logo-text {
  font-size: 18px;
  font-weight: 600;
  color: #fff;
}

.sidebar-menu {
  border-right: none;
}

/* 顶部导航 */
.header {
  background-color: #fff;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
}

.header-left {
  display: flex;
  align-items: center;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 0 10px;
}

.username {
  margin: 0 8px;
  font-size: 14px;
  color: #606266;
}

/* 主内容区 */
.main-content {
  background-color: #f0f2f5;
  padding: 20px;
  overflow-y: auto;
}
</style>
