import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const API_BASE_URL = '/api/v1'

interface User {
  id: string
  username: string
  role: string
  fullName: string
}

interface LoginResponse {
  code: number
  message: string
  data?: {
    token: string
    tokenType: string
    expiresIn: number
    user: User
  }
}

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<User | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value)
  const userRole = computed(() => user.value?.role || '')

  // Actions
  async function login(username: string, password: string): Promise<boolean> {
    loading.value = true
    error.value = null

    try {
      const response = await axios.post<LoginResponse>(`${API_BASE_URL}/auth/login`, {
        username,
        password
      })

      if (response.data.code === 200 && response.data.data) {
        token.value = response.data.data.token
        user.value = response.data.data.user
        localStorage.setItem('token', response.data.data.token)
        
        // 设置 axios 默认 header
        axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.data.token}`
        
        return true
      } else {
        error.value = response.data.message
        return false
      }
    } catch (err: any) {
      // 优先显示后端返回的错误信息
      if (err.response?.data?.message) {
        error.value = err.response.data.message
      } else if (err.response?.status === 401) {
        error.value = '用户名或密码错误'
      } else if (err.code === 'ECONNREFUSED' || err.code === 'ERR_NETWORK') {
        error.value = '无法连接到服务器，请检查网络连接'
      } else {
        error.value = '登录失败，请稍后重试'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    delete axios.defaults.headers.common['Authorization']
  }

  // 初始化时如果有 token，设置 axios header
  if (token.value) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token.value}`
  }

  return {
    token,
    user,
    loading,
    error,
    isAuthenticated,
    userRole,
    login,
    logout
  }
})
