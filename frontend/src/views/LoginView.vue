<template>
  <div class="login-container" :class="{ 'login-success': loginSuccess }">
    <div class="login-box" :class="{ 'fade-out': loginSuccess }">
      <div class="login-header">
        <el-icon size="48" color="#409EFF"><School /></el-icon>
        <h1>高校实验室管理系统</h1>
        <p>Laboratory Information Management System</p>
      </div>

      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        class="login-form"
        @keyup.enter="handleLogin"
      >
        <el-form-item prop="username">
          <el-input
            v-model="form.username"
            placeholder="用户名"
            size="large"
            :prefix-icon="User"
            clearable
          />
        </el-form-item>

        <el-form-item prop="password">
          <el-input
            v-model="form.password"
            type="password"
            placeholder="密码"
            size="large"
            :prefix-icon="Lock"
            show-password
            clearable
          />
        </el-form-item>

        <el-form-item>
          <el-button
            type="primary"
            size="large"
            class="login-button"
            :loading="authStore.loading"
            :disabled="authStore.loading"
            @click="handleLogin"
          >
            <template #loading>
              <el-icon class="is-loading"><Loading /></el-icon>
              <span>登录中...</span>
            </template>
            <template #default>
              <el-icon><Key /></el-icon>
              <span>登 录</span>
            </template>
          </el-button>
        </el-form-item>
      </el-form>

      <div class="login-tips">
        <p>测试账号：</p>
        <el-tag size="small">admin / admin123</el-tag>
        <el-tag size="small" type="success">teacher / teacher123</el-tag>
        <el-tag size="small" type="warning">student / student123</el-tag>
      </div>
    </div>

    <div class="login-footer">
      <p>© 2024 高校实验室管理系统 - 版权所有</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock, School, Key, Loading } from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const formRef = ref()
const loginSuccess = ref(false)

const form = reactive({
  username: '',
  password: ''
})

const rules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
  ]
}

const handleLogin = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid: boolean) => {
    if (valid) {
      const success = await authStore.login(form.username, form.password)
      
      if (success) {
        // 登录成功反馈
        const userName = authStore.user?.fullName || authStore.user?.username
        const roleText = getRoleText(authStore.user?.role || '')
        
        // 触发成功动画
        loginSuccess.value = true
        
        ElMessage.success({
          message: `欢迎回来，${userName} (${roleText})`,
          duration: 2000,
          showClose: true
        })
        
        // 添加短暂延迟让用户看到成功提示和动画
        setTimeout(() => {
          router.push('/home')
        }, 800)
      } else {
        ElMessage.error({
          message: authStore.error || '登录失败',
          duration: 3000,
          showClose: true
        })
      }
    }
  })
}

// 获取角色中文名称
const getRoleText = (role: string): string => {
  const roleMap: Record<string, string> = {
    'Admin': '管理员',
    'Teacher': '教师',
    'Student': '学生'
  }
  return roleMap[role] || role
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
  transition: all 0.5s ease;
}

.login-container.login-success {
  background: linear-gradient(135deg, #67c23a 0%, #409eff 100%);
}

.login-box {
  transition: all 0.5s ease;
}

.login-box.fade-out {
  transform: scale(0.95);
  opacity: 0;
}

.login-box {
  width: 100%;
  max-width: 420px;
  background: white;
  border-radius: 16px;
  padding: 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.login-header {
  text-align: center;
  margin-bottom: 32px;
}

.login-header h1 {
  font-size: 24px;
  color: #303133;
  margin: 16px 0 8px;
  font-weight: 600;
}

.login-header p {
  font-size: 14px;
  color: #909399;
}

.login-form {
  margin-top: 24px;
}

.login-button {
  width: 100%;
  font-size: 16px;
  font-weight: 500;
}

.login-button .el-icon {
  margin-right: 6px;
  vertical-align: middle;
}

.login-tips {
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #ebeef5;
  text-align: center;
}

.login-tips p {
  font-size: 12px;
  color: #909399;
  margin-bottom: 8px;
}

.login-tips .el-tag {
  margin: 4px;
}

.login-footer {
  margin-top: 40px;
  color: rgba(255, 255, 255, 0.8);
  font-size: 14px;
}

/* 响应式 */
@media (max-width: 480px) {
  .login-box {
    padding: 24px;
  }
  
  .login-header h1 {
    font-size: 20px;
  }
}
</style>
