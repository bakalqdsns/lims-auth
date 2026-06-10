/**
 * 认证状态管理
 * 与 Flutter lib/controllers/auth_controller.dart 对齐
 * 使用 Pinia + unistorage 持久化
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { storage } from '@/utils/storage'
import { login as loginApi, getCurrentUser as getMeApi, updateProfile as updateProfileApi, changePassword as changePwdApi } from '@/api/auth'
import { getMyPermissions } from '@/api/user'
import type { UserInfo, LoginRequest, UpdateProfileRequest } from '@/types/user'

export const useAuthStore = defineStore(
  'auth',
  () => {
    const token = ref<string | null>(null)
    const currentUser = ref<UserInfo | null>(null)
    const permissions = ref<string[]>([])
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    // --- 计算属性 ---
    const isLoggedIn = computed(() => !!token.value)

    // 超级管理员 (super_admin)
    const isSuperAdmin = computed(() =>
      currentUser.value?.roles?.includes('super_admin') ?? false
    )

    // 管理员 (super_admin | admin | lab_admin) — 与 frontend src/stores/auth.ts 保持一致
    const isAdmin = computed(() =>
      ['super_admin', 'admin', 'lab_admin'].some(r => currentUser.value?.roles?.includes(r)) ?? false
    )

    const isTeacher = computed(() =>
      currentUser.value?.roles?.includes('teacher') ?? false
    )

    const isStudent = computed(() =>
      currentUser.value?.roles?.includes('student') ?? false
    )

    // --- 方法 ---
    function initFromStorage() {
      const savedToken = storage.getToken()
      const savedUser = storage.getUser<UserInfo>()
      if (savedToken && savedUser) {
        token.value = savedToken
        currentUser.value = savedUser
      }
    }

    async function login(username: string, password: string) {
      isLoading.value = true
      error.value = null
      try {
        const req: LoginRequest = { username, password }
        const resp = await loginApi(req)

        token.value = resp.token
        currentUser.value = resp.user

        // 持久化
        storage.setToken(resp.token)
        storage.setUser(resp.user)

        // 加载权限
        try {
          permissions.value = await getMyPermissions()
        } catch {
          // 权限获取失败不影响登录
        }

        return true
      } catch (e: unknown) {
        const err = e as { message?: string }
        error.value = err.message || '登录失败'
        return false
      } finally {
        isLoading.value = false
      }
    }

    async function fetchCurrentUser() {
      if (!token.value) return
      try {
        const user = await getMeApi()
        currentUser.value = user
        storage.setUser(user)
        permissions.value = await getMyPermissions()
      } catch {
        // ignore
      }
    }

    async function updateProfile(req: UpdateProfileRequest) {
      try {
        await updateProfileApi(req)
        await fetchCurrentUser()
        return true
      } catch (e: unknown) {
        const err = e as { message?: string }
        error.value = err.message || '更新失败'
        return false
      }
    }

    async function changePassword(oldPassword: string, newPassword: string) {
      isLoading.value = true
      try {
        await changePwdApi({ oldPassword, newPassword })
        return true
      } catch (e: unknown) {
        const err = e as { message?: string }
        error.value = err.message || '修改密码失败'
        return false
      } finally {
        isLoading.value = false
      }
    }

    function hasPermission(permission: string): boolean {
      if (!currentUser.value) return false
      if (isSuperAdmin.value) return true
      return currentUser.value.permissions?.includes(permission) ?? false
    }

    function hasRole(role: string): boolean {
      if (!currentUser.value) return false
      return currentUser.value.roles?.includes(role) ?? false
    }

    function logout() {
      token.value = null
      currentUser.value = null
      permissions.value = []
      error.value = null
      storage.removeToken()
      storage.removeUser()
      uni.reLaunch({ url: '/pages/login/index' })
    }

    return {
      token,
      currentUser,
      permissions,
      isLoading,
      error,
      isLoggedIn,
      isSuperAdmin,
      isAdmin,
      isTeacher,
      isStudent,
      initFromStorage,
      login,
      fetchCurrentUser,
      updateProfile,
      changePassword,
      hasPermission,
      hasRole,
      logout,
    }
  },
  {
    unistorage: true, // 整个 store 持久化
  }
)
