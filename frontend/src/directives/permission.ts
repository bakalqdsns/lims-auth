import { useAuthStore } from '../stores/auth'
import type { Directive, DirectiveBinding } from 'vue'

// 检查是否有指定权限（user 未加载时返回 null）
function checkPermission(value: string | string[]): boolean | null {
  const authStore = useAuthStore()
  if (!authStore.user) return null // user 未加载，延迟决策
  const permissions = authStore.user?.permissions || []

  if (Array.isArray(value)) {
    return value.some(p => permissions.includes(p))
  }
  return permissions.includes(value)
}

// 检查是否有指定角色（user 未加载时返回 null）
function checkRole(value: string | string[]): boolean | null {
  const authStore = useAuthStore()
  if (!authStore.user) return null
  const roles = authStore.user?.roles || []
  const roleCodes = roles.map((r: any) => r.code)

  if (Array.isArray(value)) {
    return value.some(r => roleCodes.includes(r))
  }
  return roleCodes.includes(value)
}

// 权限指令
export const permission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    const has = checkPermission(value)
    if (has === false) el.style.display = 'none' // 仅当明确无权限时隐藏
    // has === null 时不处理，等 updated 重新检查
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    const has = checkPermission(value)
    if (has === false) {
      el.style.display = 'none'
    } else if (has === true) {
      el.style.display = ''
    }
    // has === null 时不处理
  }
}

// 角色指令
export const role: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    const has = checkRole(value)
    if (has === false) el.style.display = 'none'
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    const has = checkRole(value)
    if (has === false) {
      el.style.display = 'none'
    } else if (has === true) {
      el.style.display = ''
    }
  }
}

// 注册指令的插件
export default {
  install(app: any) {
    app.directive('permission', permission)
    app.directive('role', role)
  }
}
