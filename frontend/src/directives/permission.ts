import { useAuthStore } from '../stores/auth'
import type { Directive, DirectiveBinding } from 'vue'

// 检查是否有指定权限
function checkPermission(value: string | string[]): boolean {
  const authStore = useAuthStore()
  const permissions = authStore.user?.permissions || []

  if (Array.isArray(value)) {
    return value.some(p => permissions.includes(p))
  }
  return permissions.includes(value)
}

// 检查是否有指定角色
function checkRole(value: string | string[]): boolean {
  const authStore = useAuthStore()
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

    if (!checkPermission(value)) {
      el.style.display = 'none'
    }
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    if (checkPermission(value)) {
      el.style.display = ''
    } else {
      el.style.display = 'none'
    }
  }
}

// 角色指令
export const role: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    if (!checkRole(value)) {
      el.style.display = 'none'
    }
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    if (!value) return

    if (checkRole(value)) {
      el.style.display = ''
    } else {
      el.style.display = 'none'
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
