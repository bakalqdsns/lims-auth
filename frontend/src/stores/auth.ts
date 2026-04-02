import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const API_BASE_URL = '/api/v1'

interface User {
  id: string
  username: string
  email?: string
  phone?: string
  fullName?: string
  avatarUrl?: string
  isActive: boolean
  roles: string[]
  permissions: string[]
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

interface ApiResponse<T> {
  code: number
  message: string
  data: T
}

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<User | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const userRole = computed(() => user.value?.roles?.[0] || '')
  const userRoles = computed(() => user.value?.roles || [])
  const userPermissions = computed(() => user.value?.permissions || [])

  // 检查是否有指定权限
  const hasPermission = (permission: string | string[]): boolean => {
    if (!user.value?.permissions) return false
    if (Array.isArray(permission)) {
      return permission.some(p => user.value!.permissions.includes(p))
    }
    return user.value.permissions.includes(permission)
  }

  // 检查是否有指定角色
  const hasRole = (role: string | string[]): boolean => {
    if (!user.value?.roles) return false
    if (Array.isArray(role)) {
      return role.some(r => user.value!.roles.includes(r))
    }
    return user.value.roles.includes(role)
  }

  // 检查是否是超级管理员
  const isSuperAdmin = computed(() => user.value?.roles?.includes('super_admin') || false)

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
        error.value = response.data.message || '登录失败'
        return false
      }
    } catch (err: any) {
      console.error('登录请求失败:', err)

      if (err.code === 'ECONNREFUSED' || err.code === 'ERR_NETWORK') {
        error.value = '无法连接到服务器，请检查网络连接'
      } else if (err.code === 'ETIMEDOUT') {
        error.value = '连接超时，请检查网络'
      } else if (err.response?.status >= 500) {
        error.value = '服务器错误，请稍后重试'
      } else {
        error.value = err.response?.data?.message || '登录失败，请稍后重试'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  // 获取当前用户信息
  async function fetchCurrentUser(): Promise<boolean> {
    if (!token.value) return false

    try {
      const response = await axios.get<ApiResponse<User>>(`${API_BASE_URL}/auth/me`)
      if (response.data.code === 200) {
        user.value = response.data.data
        return true
      }
      return false
    } catch (err) {
      console.error('获取用户信息失败:', err)
      return false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    delete axios.defaults.headers.common['Authorization']
  }

  // 初始化时如果有 token，设置 axios header 并获取用户信息
  if (token.value) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token.value}`
    fetchCurrentUser()
  }

  return {
    token,
    user,
    loading,
    error,
    isAuthenticated,
    userRole,
    userRoles,
    userPermissions,
    isSuperAdmin,
    hasPermission,
    hasRole,
    login,
    logout,
    fetchCurrentUser
  }
})
