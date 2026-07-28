import { watch } from 'vue'
import { useAuthStore } from '../stores/auth'
import type { Directive, DirectiveBinding } from 'vue'

function checkPermission(value: string | string[]): boolean | null {
  const authStore = useAuthStore()
  if (!authStore.user) return null
  const permissions = authStore.user.permissions || []

  if (Array.isArray(value)) {
    return value.some(p => permissions.includes(p))
  }
  return permissions.includes(value)
}

function applyVisibility(el: HTMLElement, binding: DirectiveBinding) {
  const { value } = binding
  if (!value) return
  const has = checkPermission(value as string | string[])
  if (has === false) {
    el.style.display = 'none'
  } else if (has === true) {
    el.style.display = ''
  }
}

export const permission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    applyVisibility(el, binding)

    const authStore = useAuthStore()
    const stop = watch(
      () => authStore.user,
      () => {
        applyVisibility(el, binding)
        if (authStore.user) stop()
      },
      { immediate: false }
    )
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    applyVisibility(el, binding)
  }
}

export const role: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const authStore = useAuthStore()
    const roles = authStore.user?.roles || []
    const { value } = binding
    if (!value) return

    const has = Array.isArray(value) ? value.some(r => roles.includes(r)) : roles.includes(value)
    if (!has) el.style.display = 'none'

    const stop = watch(
      () => authStore.user,
      () => {
        const updatedRoles = authStore.user?.roles || []
        const updatedHas = Array.isArray(value)
          ? value.some(r => updatedRoles.includes(r))
          : updatedRoles.includes(value as string)
        el.style.display = updatedHas ? '' : 'none'
        if (authStore.user) stop()
      },
      { immediate: false }
    )
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const authStore = useAuthStore()
    const roles = authStore.user?.roles || []
    const { value } = binding
    if (!value) return
    const has = Array.isArray(value) ? value.some(r => roles.includes(r)) : roles.includes(value)
    el.style.display = has ? '' : 'none'
  }
}

export default {
  install(app: any) {
    app.directive('permission', permission)
    app.directive('role', role)
  }
}
