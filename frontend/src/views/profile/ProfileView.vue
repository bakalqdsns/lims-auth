/**
 * 个人资料页面
 * 显示并编辑当前登录用户的个人信息
 */
<template>
  <div class="profile-container">
    <div class="page-header">
      <el-button text @click="router.back()">
        <el-icon style="margin-right:4px"><ArrowLeft /></el-icon>
      </el-button>
      <h2>个人资料</h2>
    </div>

    <div class="profile-content">
      <!-- 左侧信息卡片 -->
      <el-card class="info-card" shadow="never">
        <div class="avatar-section">
          <el-avatar :size="80" :src="authStore.user?.avatarUrl" class="avatar">
            {{ (authStore.user?.fullName || authStore.user?.username || '?')[0].toUpperCase() }}
          </el-avatar>
          <div class="user-basic">
            <h3 class="username">{{ authStore.user?.fullName || authStore.user?.username }}</h3>
            <el-tag size="small" effect="plain" type="info">{{ getRoleLabel(authStore.userRole) }}</el-tag>
          </div>
        </div>

        <el-divider />

        <div class="stat-list">
          <div class="stat-item">
            <span class="stat-label">用户名</span>
            <span class="stat-value">{{ authStore.user?.username }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">角色</span>
            <span class="stat-value">{{ authStore.userRoles.join('、') }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">邮箱</span>
            <span class="stat-value">{{ authStore.user?.email || '-' }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">手机号</span>
            <span class="stat-value">{{ authStore.user?.phone || '-' }}</span>
          </div>
        </div>
      </el-card>

      <!-- 右侧编辑表单 -->
      <el-card class="form-card" shadow="never">
        <template #header>
          <div class="card-header">
            <span>编辑资料</span>
          </div>
        </template>

        <el-form ref="formRef" :model="form" :rules="formRules" label-width="90px" status-icon>
          <el-form-item label="姓名" prop="fullName">
            <el-input v-model="form.fullName" placeholder="请输入姓名" maxlength="50" show-word-limit />
          </el-form-item>
          <el-form-item label="邮箱" prop="email">
            <el-input v-model="form.email" placeholder="请输入邮箱" />
          </el-form-item>
          <el-form-item label="手机号" prop="phone">
            <el-input v-model="form.phone" placeholder="请输入手机号" maxlength="20" />
          </el-form-item>
          <el-form-item label="用户名">
            <el-input :model-value="authStore.user?.username" disabled />
          </el-form-item>
        </el-form>

        <div class="form-actions">
          <el-button type="primary" :loading="saving" @click="handleSave">保存修改</el-button>
          <el-button @click="handleReset">重置</el-button>
        </div>
      </el-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { ArrowLeft } from '@element-plus/icons-vue'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const formRef = ref<FormInstance>()
const saving = ref(false)

const form = reactive({
  fullName: '',
  email: '',
  phone: ''
})

const formRules: FormRules = {
  fullName: [
    { required: true, message: '请输入姓名', trigger: 'blur' },
    { min: 2, max: 50, message: '姓名长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  email: [
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' }
  ],
  phone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ]
}

const roleLabels: Record<string, string> = {
  super_admin: '超级管理员',
  admin: '管理员',
  lab_admin: '实验室管理员',
  teacher: '教师',
  student: '学生'
}

const getRoleLabel = (role: string) => roleLabels[role] || role

const handleReset = () => {
  form.fullName = authStore.user?.fullName || ''
  form.email = authStore.user?.email || ''
  form.phone = authStore.user?.phone || ''
  formRef.value?.clearValidate()
}

const handleSave = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  saving.value = true
  try {
    const success = await authStore.updateProfile({
      fullName: form.fullName,
      email: form.email || undefined,
      phone: form.phone || undefined
    })
    if (success) {
      ElMessage.success('个人资料已更新')
    } else {
      ElMessage.error(authStore.error || '保存失败')
    }
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  handleReset()
})
</script>

<style scoped>
.profile-container {
  padding: 20px;
  max-width: 900px;
}

.page-header {
  margin-bottom: 24px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.profile-content {
  display: flex;
  gap: 20px;
  align-items: flex-start;
}

.info-card {
  width: 300px;
  flex-shrink: 0;
}

.form-card {
  flex: 1;
  min-width: 0;
}

.avatar-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
  padding: 8px 0;
}

.avatar {
  background: #409eff;
  font-size: 28px;
}

.user-basic {
  text-align: center;
}

.username {
  margin: 0 0 8px;
  font-size: 18px;
  font-weight: 500;
}

.stat-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}

.stat-label {
  color: #909399;
  font-size: 14px;
  flex-shrink: 0;
}

.stat-value {
  color: #303133;
  font-size: 14px;
  text-align: right;
  word-break: break-all;
}

.card-header {
  font-weight: 500;
  font-size: 15px;
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 24px;
}

@media (max-width: 700px) {
  .profile-content {
    flex-direction: column;
  }

  .info-card {
    width: 100%;
  }
}
</style>
