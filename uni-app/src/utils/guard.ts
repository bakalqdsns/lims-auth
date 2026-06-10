/**
 * 路由守卫 / 权限控制
 * 与 Flutter lib/routes/routes.dart 对齐
 * 使用 uni.addInterceptor 实现路由拦截
 */
import { useAuthStore } from '@/stores/auth'

// 需要登录才能访问的页面
const AUTH_PAGES = [
  '/pages/home/index',
  '/pages/dashboard/index',
  '/pages/labs/index',
  '/pages/lab-detail/index',
  '/pages/reservations/index',
  '/pages/equipment/index',
  '/pages/borrow/index',
  '/pages/borrow-detail/index',
  '/pages/courses/index',
  '/pages/schedules/index',
  '/pages/profile/index',
  '/pages/admin/users/index',
  '/pages/admin/roles/index',
  '/pages/admin/departments/index',
  '/pages/admin/semesters/index',
  '/pages/admin/equipment/index',
  '/pages/admin/teaching-apps/index',
  '/pages/admin/statistics/index',
]

// 仅管理员可访问
const ADMIN_PAGES = [
  '/pages/admin/users/index',
  '/pages/admin/roles/index',
  '/pages/admin/departments/index',
  '/pages/admin/equipment/index',
]

// 仅教师可访问
const TEACHER_PAGES = [
  '/pages/admin/semesters/index',
  '/pages/admin/teaching-apps/index',
  '/pages/admin/statistics/index',
]

/**
 * 初始化路由拦截器
 */
export function setupRouteGuard() {
  // 页面跳转拦截
  uni.addInterceptor('navigateTo', {
    invoke(args) {
      return handleRouteGuard(args.url)
    },
    fail() {
      // ignore
    },
  })

  uni.addInterceptor('redirectTo', {
    invoke(args) {
      return handleRouteGuard(args.url)
    },
    fail() {},
  })

  uni.addInterceptor('reLaunch', {
    invoke(args) {
      return handleRouteGuard(args.url)
    },
    fail() {},
  })

  uni.addInterceptor('switchTab', {
    invoke() {
      // switchTab 通常用于 tabBar 页面, 不拦截
      return true
    },
    fail() {},
  })
}

/**
 * 处理路由守卫逻辑
 */
function handleRouteGuard(url: string): boolean {
  const authStore = useAuthStore()

  // 提取路径
  const path = getPagePath(url)

  // 登录页直接放行
  if (path === '/pages/login/index') {
    return true
  }

  // 需要登录
  if (AUTH_PAGES.includes(path)) {
    if (!authStore.isLoggedIn) {
      uni.showToast({ title: '请先登录', icon: 'none' })
      uni.reLaunch({ url: '/pages/login/index' })
      return false
    }

    // 管理员页面
    if (ADMIN_PAGES.includes(path) && !authStore.isAdmin) {
      uni.showToast({ title: '无权限访问', icon: 'none' })
      return false
    }

    // 教师页面 (管理员也可访问)
    if (TEACHER_PAGES.includes(path) && !authStore.isAdmin && !authStore.isTeacher) {
      uni.showToast({ title: '无权限访问', icon: 'none' })
      return false
    }
  }

  return true
}

/**
 * 从 URL 中提取页面路径
 */
export function getPagePath(url: string): string {
  if (!url) return ''
  // 去掉 query string
  const idx = url.indexOf('?')
  return idx > -1 ? url.substring(0, idx) : url
}

/**
 * 带参数跳转页面 (统一封装)
 */
export function navigateTo(url: string, params?: Record<string, string | number>) {
  let fullUrl = url
  if (params) {
    const query = Object.entries(params)
      .map(([k, v]) => `${k}=${encodeURIComponent(v)}`)
      .join('&')
    fullUrl = `${url}?${query}`
  }
  uni.navigateTo({ url: fullUrl })
}

/**
 * 跳转 Tab 页面
 */
export function switchTab(url: string) {
  uni.switchTab({ url })
}

/**
 * 关闭所有页面并跳转
 */
export function reLaunch(url: string) {
  uni.reLaunch({ url })
}

/**
 * 关闭当前页并返回
 */
export function navigateBack(delta = 1) {
  uni.navigateBack({ delta })
}

/**
 * 重定向
 */
export function redirectTo(url: string) {
  uni.redirectTo({ url })
}
